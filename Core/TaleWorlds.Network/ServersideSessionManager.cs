using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace TaleWorlds.Network
{
	public abstract class ServersideSessionManager
	{
		public float PeerAliveCoeff { get; set; }

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

		public ServersideSession GetPeer(int peerIndex)
		{
			ServersideSession serversideSession = null;
			this._peers[peerIndex % this._readWriteThreadCount].TryGetValue(peerIndex, out serversideSession);
			return serversideSession;
		}

		public virtual void Tick()
		{
			this.IncomingConnectionsTick();
			this.MessagingTick();
			this.PeerAliveCheckTick();
			this.HandleRemovedPeersTick();
		}

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

		internal void AddIncomingMessage(IncomingServerSessionMessage incomingMessage)
		{
			this._incomingMessages[incomingMessage.Peer.Index % this._readWriteThreadCount].Enqueue(incomingMessage);
		}

		internal void AddDisconnectedPeer(ServersideSession peer)
		{
			this._disconnectedPeers[peer.Index % this._readWriteThreadCount].TryAdd(peer.Index, peer);
		}

		protected abstract ServersideSession OnNewConnection();

		protected abstract void OnRemoveConnection(ServersideSession peer);

		private int _readWriteThreadCount = 1;

		private ServersideSessionManager.ThreadType _threadType;

		private ushort _listenPort;

		private TcpSocket _serverSocket;

		private int _lastUniqueClientId;

		private Thread _serverThread;

		private long _lastPeerAliveCheck;

		private List<ConcurrentQueue<IncomingServerSessionMessage>> _incomingMessages;

		private List<ConcurrentDictionary<int, ServersideSession>> _peers;

		private List<ConcurrentDictionary<int, ServersideSession>> _disconnectedPeers;

		private List<Thread> _readerThreads;

		private List<Thread> _writerThreads;

		private List<Thread> _singleThreads;

		public enum ThreadType
		{
			Single,
			MultipleIOAndListener,
			MultipleSeperateIOAndListener
		}
	}
}
