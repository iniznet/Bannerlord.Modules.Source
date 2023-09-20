using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace TaleWorlds.Network
{
	// Token: 0x02000023 RID: 35
	public abstract class ServersideSessionManager
	{
		// Token: 0x1700002A RID: 42
		// (get) Token: 0x060000F7 RID: 247 RVA: 0x00003C9C File Offset: 0x00001E9C
		// (set) Token: 0x060000F8 RID: 248 RVA: 0x00003CA4 File Offset: 0x00001EA4
		public float PeerAliveCoeff { get; set; }

		// Token: 0x060000F9 RID: 249 RVA: 0x00003CB0 File Offset: 0x00001EB0
		protected ServersideSessionManager()
		{
			this._readerThreads = new List<Thread>();
			this._writerThreads = new List<Thread>();
			this._singleThreads = new List<Thread>();
			this._peers = new List<ConcurrentDictionary<int, ServersideSession>>();
			this.PeerAliveCoeff = 3f;
			this._incomingMessages = new List<ConcurrentQueue<IncomingServerSessionMessage>>();
			this._disconnectedPeers = new List<ConcurrentDictionary<int, ServersideSession>>();
		}

		// Token: 0x060000FA RID: 250 RVA: 0x00003D18 File Offset: 0x00001F18
		public void Activate(ushort port, ServersideSessionManager.ThreadType threadType = ServersideSessionManager.ThreadType.Single, int readWriteThreadCount = 1)
		{
			this._threadType = threadType;
			this._readWriteThreadCount = readWriteThreadCount;
			this._listenPort = port;
			this._serverSocket = new TcpSocket();
			this._serverSocket.Listen((int)this._listenPort);
			if (this._threadType == ServersideSessionManager.ThreadType.Single)
			{
				this._peers.Add(new ConcurrentDictionary<int, ServersideSession>(this._readWriteThreadCount * 3, 8192));
				this._incomingMessages.Add(new ConcurrentQueue<IncomingServerSessionMessage>());
				this._disconnectedPeers.Add(new ConcurrentDictionary<int, ServersideSession>());
				this._readWriteThreadCount = 1;
				this._serverThread = new Thread(new ThreadStart(this.ProcessSingle));
				this._serverThread.IsBackground = true;
				this._serverThread.Name = this.ToString() + " - Server Thread";
				this._serverThread.Start();
				return;
			}
			for (int i = 0; i < this._readWriteThreadCount; i++)
			{
				this._peers.Add(new ConcurrentDictionary<int, ServersideSession>(this._readWriteThreadCount * 4, 8192));
				this._incomingMessages.Add(new ConcurrentQueue<IncomingServerSessionMessage>());
				this._disconnectedPeers.Add(new ConcurrentDictionary<int, ServersideSession>());
			}
			for (int j = 0; j < this._readWriteThreadCount; j++)
			{
				if (this._threadType == ServersideSessionManager.ThreadType.MultipleSeperateIOAndListener)
				{
					Thread thread = new Thread(new ParameterizedThreadStart(this.ProcessRead));
					thread.IsBackground = true;
					thread.Name = this.ToString() + " - Server Reader Thread - " + j;
					thread.IsBackground = true;
					thread.Start(j);
					Thread thread2 = new Thread(new ParameterizedThreadStart(this.ProcessWriter));
					thread2.IsBackground = true;
					thread2.Name = this.ToString() + " - Server Writer Thread" + j;
					thread2.IsBackground = true;
					thread2.Start(j);
					this._readerThreads.Add(thread);
					this._writerThreads.Add(thread2);
				}
				else
				{
					Thread thread3 = new Thread(new ParameterizedThreadStart(this.ProcessReaderWriter));
					thread3.Name = this.ToString() + " - Server ReaderWriter Thread - " + j;
					thread3.IsBackground = true;
					thread3.Start(j);
					this._singleThreads.Add(thread3);
				}
			}
			this._serverThread = new Thread(new ThreadStart(this.ProcessListener));
			this._serverThread.IsBackground = true;
			this._serverThread.Name = this.ToString() + " - Server Listener Thread";
			this._serverThread.Start();
		}

		// Token: 0x060000FB RID: 251 RVA: 0x00003FA8 File Offset: 0x000021A8
		private void ProcessRead(object indexObject)
		{
			int index = (int)indexObject;
			TickManager.TickDelegate tickDelegate = delegate
			{
				foreach (ServersideSession serversideSession in this._peers[index].Values)
				{
					if (serversideSession != null)
					{
						serversideSession.Socket.ProcessRead();
					}
				}
			};
			TickManager tickManager = new TickManager(5000, tickDelegate);
			for (;;)
			{
				tickManager.Tick();
			}
		}

		// Token: 0x060000FC RID: 252 RVA: 0x00003FEC File Offset: 0x000021EC
		private void ProcessWriter(object indexObject)
		{
			int index = (int)indexObject;
			TickManager.TickDelegate tickDelegate = delegate
			{
				foreach (ServersideSession serversideSession in this._peers[index].Values)
				{
					if (serversideSession != null)
					{
						serversideSession.Socket.ProcessWrite();
					}
				}
			};
			TickManager tickManager = new TickManager(5000, tickDelegate);
			for (;;)
			{
				tickManager.Tick();
			}
		}

		// Token: 0x060000FD RID: 253 RVA: 0x00004030 File Offset: 0x00002230
		private void ProcessReaderWriter(object indexObject)
		{
			int index = (int)indexObject;
			TickManager.TickDelegate tickDelegate = delegate
			{
				foreach (ServersideSession serversideSession in this._peers[index].Values)
				{
					if (serversideSession != null)
					{
						serversideSession.Socket.ProcessWrite();
						serversideSession.Socket.ProcessRead();
					}
				}
			};
			TickManager tickManager = new TickManager(5000, tickDelegate);
			for (;;)
			{
				tickManager.Tick();
			}
		}

		// Token: 0x060000FE RID: 254 RVA: 0x00004074 File Offset: 0x00002274
		private void ProcessListener()
		{
			TickManager.TickDelegate tickDelegate = delegate
			{
				this._serverSocket.CheckAcceptClient();
			};
			TickManager tickManager = new TickManager(500, tickDelegate);
			for (;;)
			{
				tickManager.Tick();
			}
		}

		// Token: 0x060000FF RID: 255 RVA: 0x000040A4 File Offset: 0x000022A4
		private void ProcessSingle()
		{
			TickManager.TickDelegate tickDelegate = delegate
			{
				foreach (ServersideSession serversideSession in this._peers[0].Values)
				{
					if (serversideSession != null)
					{
						serversideSession.Socket.ProcessWrite();
						serversideSession.Socket.ProcessRead();
					}
				}
				this._serverSocket.CheckAcceptClient();
			};
			TickManager tickManager = new TickManager(5000, tickDelegate);
			for (;;)
			{
				tickManager.Tick();
			}
		}

		// Token: 0x06000100 RID: 256 RVA: 0x000040D4 File Offset: 0x000022D4
		private void RemovePeer(int peerNo)
		{
			ServersideSession serversideSession = null;
			if (this._peers[peerNo % this._readWriteThreadCount].TryRemove(peerNo, out serversideSession))
			{
				serversideSession.Socket.Close();
				serversideSession.OnDisconnected();
				this.OnRemoveConnection(serversideSession);
				this._disconnectedPeers[peerNo % this._readWriteThreadCount].TryRemove(serversideSession.Index, out serversideSession);
			}
		}

		// Token: 0x06000101 RID: 257 RVA: 0x0000413C File Offset: 0x0000233C
		public ServersideSession GetPeer(int peerIndex)
		{
			ServersideSession serversideSession = null;
			this._peers[peerIndex % this._readWriteThreadCount].TryGetValue(peerIndex, out serversideSession);
			return serversideSession;
		}

		// Token: 0x06000102 RID: 258 RVA: 0x00004168 File Offset: 0x00002368
		public virtual void Tick()
		{
			this.IncomingConnectionsTick();
			this.MessagingTick();
			this.PeerAliveCheckTick();
			this.HandleRemovedPeersTick();
		}

		// Token: 0x06000103 RID: 259 RVA: 0x00004184 File Offset: 0x00002384
		private void IncomingConnectionsTick()
		{
			TcpSocket lastIncomingConnection = this._serverSocket.GetLastIncomingConnection();
			if (lastIncomingConnection != null)
			{
				ServersideSession serversideSession = this.OnNewConnection();
				this._lastUniqueClientId++;
				serversideSession.InitializeSocket(this._lastUniqueClientId, lastIncomingConnection);
				this._peers[this._lastUniqueClientId % this._readWriteThreadCount].TryAdd(this._lastUniqueClientId, serversideSession);
			}
		}

		// Token: 0x06000104 RID: 260 RVA: 0x000041E8 File Offset: 0x000023E8
		private void MessagingTick()
		{
			foreach (ConcurrentQueue<IncomingServerSessionMessage> concurrentQueue in this._incomingMessages)
			{
				int count = concurrentQueue.Count;
				for (int i = 0; i < count; i++)
				{
					IncomingServerSessionMessage incomingServerSessionMessage = null;
					concurrentQueue.TryDequeue(out incomingServerSessionMessage);
					ServersideSession peer = incomingServerSessionMessage.Peer;
					NetworkMessage networkMessage = incomingServerSessionMessage.NetworkMessage;
					try
					{
						networkMessage.BeginRead();
						byte b = networkMessage.ReadByte();
						if (!peer.ContainsMessageHandler(b))
						{
							networkMessage.ResetRead();
							peer.OnMessageReceived(networkMessage);
						}
						else
						{
							MessageContract messageContract = MessageContract.CreateMessageContract(peer.GetMessageContractType(b));
							messageContract.DeserializeFromNetworkMessage(networkMessage);
							peer.HandleMessage(messageContract);
						}
					}
					catch (Exception)
					{
						this.RemovePeer(i);
					}
				}
			}
		}

		// Token: 0x06000105 RID: 261 RVA: 0x000042D8 File Offset: 0x000024D8
		private void PeerAliveCheckTick()
		{
			if ((long)Environment.TickCount > this._lastPeerAliveCheck + 3000L)
			{
				foreach (ConcurrentDictionary<int, ServersideSession> concurrentDictionary in this._peers)
				{
					foreach (KeyValuePair<int, ServersideSession> keyValuePair in concurrentDictionary)
					{
						int key = keyValuePair.Key;
						ServersideSession value = keyValuePair.Value;
						if ((double)((Environment.TickCount - value.Socket.LastMessageArrivalTime) / 1000) > (double)this.PeerAliveCoeff * 5.0)
						{
							this.RemovePeer(key);
						}
					}
				}
				this._lastPeerAliveCheck = (long)Environment.TickCount;
			}
		}

		// Token: 0x06000106 RID: 262 RVA: 0x000043BC File Offset: 0x000025BC
		private void HandleRemovedPeersTick()
		{
			foreach (ConcurrentDictionary<int, ServersideSession> concurrentDictionary in this._disconnectedPeers)
			{
				foreach (ServersideSession serversideSession in concurrentDictionary.Values)
				{
					this.RemovePeer(serversideSession.Index);
				}
			}
		}

		// Token: 0x06000107 RID: 263 RVA: 0x00004448 File Offset: 0x00002648
		internal void AddIncomingMessage(IncomingServerSessionMessage incomingMessage)
		{
			this._incomingMessages[incomingMessage.Peer.Index % this._readWriteThreadCount].Enqueue(incomingMessage);
		}

		// Token: 0x06000108 RID: 264 RVA: 0x0000446D File Offset: 0x0000266D
		internal void AddDisconnectedPeer(ServersideSession peer)
		{
			this._disconnectedPeers[peer.Index % this._readWriteThreadCount].TryAdd(peer.Index, peer);
		}

		// Token: 0x06000109 RID: 265
		protected abstract ServersideSession OnNewConnection();

		// Token: 0x0600010A RID: 266
		protected abstract void OnRemoveConnection(ServersideSession peer);

		// Token: 0x04000044 RID: 68
		private int _readWriteThreadCount = 1;

		// Token: 0x04000045 RID: 69
		private ServersideSessionManager.ThreadType _threadType;

		// Token: 0x04000046 RID: 70
		private ushort _listenPort;

		// Token: 0x04000047 RID: 71
		private TcpSocket _serverSocket;

		// Token: 0x04000048 RID: 72
		private int _lastUniqueClientId;

		// Token: 0x04000049 RID: 73
		private Thread _serverThread;

		// Token: 0x0400004A RID: 74
		private long _lastPeerAliveCheck;

		// Token: 0x0400004B RID: 75
		private List<ConcurrentQueue<IncomingServerSessionMessage>> _incomingMessages;

		// Token: 0x0400004C RID: 76
		private List<ConcurrentDictionary<int, ServersideSession>> _peers;

		// Token: 0x0400004D RID: 77
		private List<ConcurrentDictionary<int, ServersideSession>> _disconnectedPeers;

		// Token: 0x0400004E RID: 78
		private List<Thread> _readerThreads;

		// Token: 0x0400004F RID: 79
		private List<Thread> _writerThreads;

		// Token: 0x04000050 RID: 80
		private List<Thread> _singleThreads;

		// Token: 0x0200003E RID: 62
		public enum ThreadType
		{
			// Token: 0x040000CD RID: 205
			Single,
			// Token: 0x040000CE RID: 206
			MultipleIOAndListener,
			// Token: 0x040000CF RID: 207
			MultipleSeperateIOAndListener
		}
	}
}
