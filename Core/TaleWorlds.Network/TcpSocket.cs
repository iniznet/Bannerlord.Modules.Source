using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using TaleWorlds.Library;

namespace TaleWorlds.Network
{
	// Token: 0x02000025 RID: 37
	internal class TcpSocket
	{
		// Token: 0x1700002D RID: 45
		// (get) Token: 0x06000112 RID: 274 RVA: 0x00004546 File Offset: 0x00002746
		// (set) Token: 0x06000113 RID: 275 RVA: 0x0000454E File Offset: 0x0000274E
		internal int LastMessageSentTime { get; set; }

		// Token: 0x1700002E RID: 46
		// (get) Token: 0x06000114 RID: 276 RVA: 0x00004557 File Offset: 0x00002757
		// (set) Token: 0x06000115 RID: 277 RVA: 0x0000455F File Offset: 0x0000275F
		internal int LastMessageArrivalTime { get; set; }

		// Token: 0x1700002F RID: 47
		// (get) Token: 0x06000116 RID: 278 RVA: 0x00004568 File Offset: 0x00002768
		// (set) Token: 0x06000117 RID: 279 RVA: 0x00004570 File Offset: 0x00002770
		internal TcpStatus Status
		{
			get
			{
				return this._status;
			}
			private set
			{
				if (value == TcpStatus.ConnectionClosed && this._status != value && this.Closed != null)
				{
					this.Closed();
				}
				this._status = value;
			}
		}

		// Token: 0x17000030 RID: 48
		// (get) Token: 0x06000118 RID: 280 RVA: 0x0000459C File Offset: 0x0000279C
		internal string RemoteEndComputerName
		{
			get
			{
				if (this._remoteEndComputerName == "")
				{
					try
					{
						this._remoteEndComputerName = Dns.GetHostEntry(((IPEndPoint)this._dotNetSocket.RemoteEndPoint).Address).HostName;
					}
					catch (Exception)
					{
						this._remoteEndComputerName = "Unknown";
					}
				}
				return this._remoteEndComputerName;
			}
		}

		// Token: 0x17000031 RID: 49
		// (get) Token: 0x06000119 RID: 281 RVA: 0x00004608 File Offset: 0x00002808
		internal string IPAddress
		{
			get
			{
				return ((IPEndPoint)this._dotNetSocket.RemoteEndPoint).Address.ToString();
			}
		}

		// Token: 0x0600011A RID: 282 RVA: 0x00004624 File Offset: 0x00002824
		internal TcpSocket()
		{
			this._uniqueSocketId = TcpSocket._socketCount;
			TcpSocket._socketCount++;
			this.LastReadMessage = null;
			this.LastMessageSentTime = 0;
			this._writeNetworkMessageQueue = new ConcurrentQueue<MessageBuffer>();
			this._socketAsyncEventArgsWrite = new SocketAsyncEventArgs();
			this._socketAsyncEventArgsWrite.Completed += this.ProcessIO;
			this._socketAsyncEventArgsRead = new SocketAsyncEventArgs();
			this._socketAsyncEventArgsRead.Completed += this.ProcessIO;
			this._socketAsyncEventArgsListener = new SocketAsyncEventArgs();
			this._socketAsyncEventArgsListener.Completed += this.ProcessIO;
			byte[] array = new byte[] { 46, 251, byte.MaxValue, byte.MaxValue };
			this._peerAliveMessageBuffer = new MessageBuffer(array, array.Length);
			byte[] array2 = new byte[] { 241, 216, byte.MaxValue, byte.MaxValue };
			this._disconnectMessageBuffer = new MessageBuffer(array2, array2.Length);
		}

		// Token: 0x0600011B RID: 283 RVA: 0x00004738 File Offset: 0x00002938
		internal TcpSocket GetLastIncomingConnection()
		{
			TcpSocket tcpSocket;
			if (this._incomingConnections.TryDequeue(out tcpSocket))
			{
				tcpSocket.LastMessageArrivalTime = Environment.TickCount;
				tcpSocket.LastMessageSentTime = Environment.TickCount;
			}
			return tcpSocket;
		}

		// Token: 0x0600011C RID: 284 RVA: 0x0000476C File Offset: 0x0000296C
		internal void Connect(string address, int port)
		{
			try
			{
				this.LastMessageArrivalTime = Environment.TickCount;
				this.LastMessageSentTime = 0;
				this._dotNetSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
				IPAddress[] hostAddresses = Dns.GetHostAddresses(address);
				IPAddress ipaddress = null;
				for (int i = 0; i < hostAddresses.Length; i++)
				{
					if (hostAddresses[i].AddressFamily == AddressFamily.InterNetwork)
					{
						ipaddress = hostAddresses[i];
					}
				}
				this._socketAsyncEventArgsListener.RemoteEndPoint = new IPEndPoint(ipaddress, port);
				this.Status = TcpStatus.Connecting;
				if (!this._dotNetSocket.ConnectAsync(this._socketAsyncEventArgsListener))
				{
					this.ProcessIO(this._dotNetSocket, this._socketAsyncEventArgsListener);
				}
			}
			catch (Exception ex)
			{
				this.Status = TcpStatus.SocketClosed;
				Debug.Print("Tcp Connection Error: " + ex, 0, Debug.DebugColor.White, 17592186044416UL);
				Thread.Sleep(250);
			}
		}

		// Token: 0x0600011D RID: 285 RVA: 0x00004840 File Offset: 0x00002A40
		internal void CheckAcceptClient()
		{
			if (!this._currentlyAcceptingClients)
			{
				this._currentlyAcceptingClients = true;
				if (!this._dotNetSocket.AcceptAsync(this._socketAsyncEventArgsListener))
				{
					this.ProcessIO(this._dotNetSocket, this._socketAsyncEventArgsListener);
				}
			}
		}

		// Token: 0x0600011E RID: 286 RVA: 0x00004878 File Offset: 0x00002A78
		internal void Listen(int port)
		{
			this._incomingConnections = new ConcurrentQueue<TcpSocket>();
			this._dotNetSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			this._dotNetSocket.Bind(new IPEndPoint(System.Net.IPAddress.Any, port));
			this._dotNetSocket.Listen(1024);
			this.CheckAcceptClient();
		}

		// Token: 0x17000032 RID: 50
		// (get) Token: 0x0600011F RID: 287 RVA: 0x000048CC File Offset: 0x00002ACC
		internal bool IsConnected
		{
			get
			{
				if ((this.Status == TcpStatus.DataReady || this.Status == TcpStatus.WaitingDataLength || this.Status == TcpStatus.WaitingData) && (this._dotNetSocket == null || !this._dotNetSocket.Connected))
				{
					this.Status = TcpStatus.ConnectionClosed;
				}
				return this.Status != TcpStatus.SocketClosed && this.Status != TcpStatus.ConnectionClosed && this.Status > TcpStatus.Connecting;
			}
		}

		// Token: 0x06000120 RID: 288 RVA: 0x00004930 File Offset: 0x00002B30
		internal void ProcessRead()
		{
			if (this.IsConnected && !this._processingReceive && (this.Status == TcpStatus.WaitingData || this.Status == TcpStatus.WaitingDataLength))
			{
				if (this.LastReadMessage == null)
				{
					this.LastReadMessage = new MessageBuffer(new byte[16777216], 0);
				}
				if (this.Status == TcpStatus.WaitingDataLength)
				{
					this._processingReceive = true;
					this._socketAsyncEventArgsRead.SetBuffer(this.LastReadMessage.Buffer, this._lastMessageTotalRead, 4 - this._lastMessageTotalRead);
					if (!this._dotNetSocket.ReceiveAsync(this._socketAsyncEventArgsRead))
					{
						this.ProcessIO(this._dotNetSocket, this._socketAsyncEventArgsRead);
						return;
					}
				}
				else if (this.Status == TcpStatus.WaitingData)
				{
					this._processingReceive = true;
					this._socketAsyncEventArgsRead.SetBuffer(this.LastReadMessage.Buffer, this._lastMessageTotalRead, this.LastReadMessage.DataLength - this._lastMessageTotalRead);
					if (!this._dotNetSocket.ReceiveAsync(this._socketAsyncEventArgsRead))
					{
						this.ProcessIO(this._dotNetSocket, this._socketAsyncEventArgsRead);
					}
				}
			}
		}

		// Token: 0x06000121 RID: 289 RVA: 0x00004A44 File Offset: 0x00002C44
		internal void ProcessWrite()
		{
			if (this.IsConnected && this._currentlySendingMessage == null && this._writeNetworkMessageQueue.TryDequeue(out this._currentlySendingMessage))
			{
				try
				{
					this._socketAsyncEventArgsWrite.SetBuffer(this._currentlySendingMessage.Buffer, 0, this._currentlySendingMessage.DataLength);
					if (!this._dotNetSocket.SendAsync(this._socketAsyncEventArgsWrite))
					{
						this.ProcessIO(this._dotNetSocket, this._socketAsyncEventArgsWrite);
					}
					this.LastMessageSentTime = Environment.TickCount;
				}
				catch (Exception ex)
				{
					Debug.Print("SendMessage Error: " + ex, 0, Debug.DebugColor.White, 17592186044416UL);
					this.Status = TcpStatus.ConnectionClosed;
				}
			}
		}

		// Token: 0x06000122 RID: 290 RVA: 0x00004B04 File Offset: 0x00002D04
		private void ProcessIO(object sender, SocketAsyncEventArgs e)
		{
			try
			{
				if (e.LastOperation == SocketAsyncOperation.Accept)
				{
					if (e.SocketError == SocketError.Success)
					{
						this.Status = TcpStatus.WaitingDataLength;
						this.AddIncomingConnection(e);
					}
				}
				else if (e.LastOperation == SocketAsyncOperation.Connect)
				{
					if (e.SocketError == SocketError.Success)
					{
						this.Status = TcpStatus.WaitingDataLength;
					}
					else
					{
						Debug.Print("Connection error: " + e.SocketError, 0, Debug.DebugColor.White, 17592186044416UL);
						this.Status = TcpStatus.ConnectionClosed;
					}
				}
				else if (e.LastOperation == SocketAsyncOperation.Send)
				{
					if (e.SocketError == SocketError.Success)
					{
						if (this._currentlySendingMessage == this._disconnectMessageBuffer)
						{
							this.Status = TcpStatus.ConnectionClosed;
						}
						this._currentlySendingMessage = null;
					}
					else
					{
						Debug.Print("Message Send, error: " + e.SocketError, 0, Debug.DebugColor.White, 17592186044416UL);
						this.Status = TcpStatus.ConnectionClosed;
					}
				}
				else if (e.LastOperation != SocketAsyncOperation.Disconnect && e.LastOperation == SocketAsyncOperation.Receive)
				{
					this.LastMessageArrivalTime = Environment.TickCount;
					if (this.Status == TcpStatus.WaitingDataLength)
					{
						if (e.BytesTransferred == 4 - this._lastMessageTotalRead)
						{
							this._lastMessageTotalRead += e.BytesTransferred;
							if (this._lastMessageTotalRead == 4)
							{
								int num = BitConverter.ToInt32(this.LastReadMessage.Buffer, 0);
								if (num == -1234)
								{
									this.Status = TcpStatus.WaitingDataLength;
									this._lastMessageTotalRead = 0;
									this.LastReadMessage = null;
								}
								else if (num == -9999)
								{
									this.Status = TcpStatus.ConnectionClosed;
								}
								else
								{
									if (num > 16777216)
									{
										throw new Exception(string.Format("Message length too big: {0}", this.LastReadMessage.DataLength));
									}
									if (num <= 0)
									{
										throw new Exception(string.Format("Message length too small: {0}", this.LastReadMessage.DataLength));
									}
									this.LastReadMessage.DataLength = num + 4;
									this.Status = TcpStatus.WaitingData;
								}
								this._processingReceive = false;
							}
						}
					}
					else if (this.Status == TcpStatus.WaitingData)
					{
						this._lastMessageTotalRead += e.BytesTransferred;
						if (this._lastMessageTotalRead == this.LastReadMessage.DataLength)
						{
							if (this.MessageReceived != null)
							{
								this.MessageReceived(this.LastReadMessage);
							}
							this.Status = TcpStatus.WaitingDataLength;
							this._lastMessageTotalRead = 0;
							this.LastReadMessage = null;
						}
						this._processingReceive = false;
					}
				}
			}
			catch (Exception ex)
			{
				Debug.Print(string.Concat(new object[] { "Exception on TcpSocket::ProcessIO ", ex, " - ", ex.Message, " ", ex.StackTrace }), 0, Debug.DebugColor.White, 17592186044416UL);
			}
		}

		// Token: 0x06000123 RID: 291 RVA: 0x00004DC8 File Offset: 0x00002FC8
		private void AddIncomingConnection(SocketAsyncEventArgs e)
		{
			TcpSocket tcpSocket = new TcpSocket();
			tcpSocket._dotNetSocket = e.AcceptSocket;
			e.AcceptSocket = null;
			tcpSocket.Status = TcpStatus.WaitingDataLength;
			this._incomingConnections.Enqueue(tcpSocket);
			this._currentlyAcceptingClients = false;
		}

		// Token: 0x06000124 RID: 292 RVA: 0x00004E08 File Offset: 0x00003008
		private void EnqueueMessage(MessageBuffer messageBuffer)
		{
			this._writeNetworkMessageQueue.Enqueue(messageBuffer);
		}

		// Token: 0x06000125 RID: 293 RVA: 0x00004E16 File Offset: 0x00003016
		internal void SendDisconnectMessage()
		{
			this.EnqueueMessage(this._disconnectMessageBuffer);
		}

		// Token: 0x06000126 RID: 294 RVA: 0x00004E24 File Offset: 0x00003024
		internal void SendPeerAliveMessage()
		{
			this.EnqueueMessage(this._peerAliveMessageBuffer);
		}

		// Token: 0x06000127 RID: 295 RVA: 0x00004E32 File Offset: 0x00003032
		internal void SendMessage(MessageBuffer messageBuffer)
		{
			this.EnqueueMessage(messageBuffer);
		}

		// Token: 0x06000128 RID: 296 RVA: 0x00004E3C File Offset: 0x0000303C
		internal void Close()
		{
			if (this.Status == TcpStatus.SocketClosed)
			{
				Debug.Print("Socket already closed.", 0, Debug.DebugColor.White, 17592186044416UL);
				return;
			}
			this.Status = TcpStatus.SocketClosed;
			if (this._dotNetSocket != null)
			{
				try
				{
					if (this._dotNetSocket.Available > 0)
					{
						Debug.Print("Closing socket but there were " + this._dotNetSocket.Available + " bytes data", 0, Debug.DebugColor.White, 17592186044416UL);
					}
				}
				catch
				{
				}
				this._dotNetSocket.Close(0);
				Debug.Print("Socket " + this._uniqueSocketId + " closed and destroyed!", 0, Debug.DebugColor.White, 17592186044416UL);
			}
			this._dotNetSocket = null;
			if (this._socketAsyncEventArgsRead != null)
			{
				this._socketAsyncEventArgsRead.Dispose();
				this._socketAsyncEventArgsRead = null;
			}
			if (this._socketAsyncEventArgsWrite != null)
			{
				this._socketAsyncEventArgsWrite.Dispose();
				this._socketAsyncEventArgsWrite = null;
			}
			if (this._socketAsyncEventArgsListener != null)
			{
				this._socketAsyncEventArgsListener.Dispose();
				this._socketAsyncEventArgsListener = null;
			}
			if (this._dotNetSocket != null)
			{
				this._dotNetSocket.Dispose();
				this._dotNetSocket = null;
			}
		}

		// Token: 0x14000007 RID: 7
		// (add) Token: 0x06000129 RID: 297 RVA: 0x00004F74 File Offset: 0x00003174
		// (remove) Token: 0x0600012A RID: 298 RVA: 0x00004FAC File Offset: 0x000031AC
		internal event TcpMessageReceiverDelegate MessageReceived;

		// Token: 0x14000008 RID: 8
		// (add) Token: 0x0600012B RID: 299 RVA: 0x00004FE4 File Offset: 0x000031E4
		// (remove) Token: 0x0600012C RID: 300 RVA: 0x0000501C File Offset: 0x0000321C
		internal event TcpCloseDelegate Closed;

		// Token: 0x04000054 RID: 84
		public const int MaxMessageSize = 16777216;

		// Token: 0x04000055 RID: 85
		internal const int PeerAliveCode = -1234;

		// Token: 0x04000056 RID: 86
		internal const int DisconnectCode = -9999;

		// Token: 0x04000057 RID: 87
		private int _uniqueSocketId;

		// Token: 0x04000058 RID: 88
		private static int _socketCount;

		// Token: 0x04000059 RID: 89
		private Socket _dotNetSocket;

		// Token: 0x0400005C RID: 92
		private SocketAsyncEventArgs _socketAsyncEventArgsWrite;

		// Token: 0x0400005D RID: 93
		private SocketAsyncEventArgs _socketAsyncEventArgsListener;

		// Token: 0x0400005E RID: 94
		private SocketAsyncEventArgs _socketAsyncEventArgsRead;

		// Token: 0x0400005F RID: 95
		internal MessageBuffer LastReadMessage;

		// Token: 0x04000060 RID: 96
		private ConcurrentQueue<MessageBuffer> _writeNetworkMessageQueue;

		// Token: 0x04000061 RID: 97
		private MessageBuffer _currentlySendingMessage;

		// Token: 0x04000062 RID: 98
		private bool _currentlyAcceptingClients;

		// Token: 0x04000063 RID: 99
		private ConcurrentQueue<TcpSocket> _incomingConnections;

		// Token: 0x04000064 RID: 100
		private int _lastMessageTotalRead;

		// Token: 0x04000065 RID: 101
		private TcpStatus _status;

		// Token: 0x04000066 RID: 102
		private bool _processingReceive;

		// Token: 0x04000067 RID: 103
		private string _remoteEndComputerName;

		// Token: 0x04000068 RID: 104
		private readonly MessageBuffer _peerAliveMessageBuffer;

		// Token: 0x04000069 RID: 105
		private readonly MessageBuffer _disconnectMessageBuffer;
	}
}
