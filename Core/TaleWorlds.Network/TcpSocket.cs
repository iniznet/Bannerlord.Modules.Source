using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using TaleWorlds.Library;

namespace TaleWorlds.Network
{
	internal class TcpSocket
	{
		internal int LastMessageSentTime { get; set; }

		internal int LastMessageArrivalTime { get; set; }

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

		internal string IPAddress
		{
			get
			{
				return ((IPEndPoint)this._dotNetSocket.RemoteEndPoint).Address.ToString();
			}
		}

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

		internal void Listen(int port)
		{
			this._incomingConnections = new ConcurrentQueue<TcpSocket>();
			this._dotNetSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			this._dotNetSocket.Bind(new IPEndPoint(System.Net.IPAddress.Any, port));
			this._dotNetSocket.Listen(1024);
			this.CheckAcceptClient();
		}

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

		private void AddIncomingConnection(SocketAsyncEventArgs e)
		{
			TcpSocket tcpSocket = new TcpSocket();
			tcpSocket._dotNetSocket = e.AcceptSocket;
			e.AcceptSocket = null;
			tcpSocket.Status = TcpStatus.WaitingDataLength;
			this._incomingConnections.Enqueue(tcpSocket);
			this._currentlyAcceptingClients = false;
		}

		private void EnqueueMessage(MessageBuffer messageBuffer)
		{
			this._writeNetworkMessageQueue.Enqueue(messageBuffer);
		}

		internal void SendDisconnectMessage()
		{
			this.EnqueueMessage(this._disconnectMessageBuffer);
		}

		internal void SendPeerAliveMessage()
		{
			this.EnqueueMessage(this._peerAliveMessageBuffer);
		}

		internal void SendMessage(MessageBuffer messageBuffer)
		{
			this.EnqueueMessage(messageBuffer);
		}

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

		internal event TcpMessageReceiverDelegate MessageReceived;

		internal event TcpCloseDelegate Closed;

		public const int MaxMessageSize = 16777216;

		internal const int PeerAliveCode = -1234;

		internal const int DisconnectCode = -9999;

		private int _uniqueSocketId;

		private static int _socketCount;

		private Socket _dotNetSocket;

		private SocketAsyncEventArgs _socketAsyncEventArgsWrite;

		private SocketAsyncEventArgs _socketAsyncEventArgsListener;

		private SocketAsyncEventArgs _socketAsyncEventArgsRead;

		internal MessageBuffer LastReadMessage;

		private ConcurrentQueue<MessageBuffer> _writeNetworkMessageQueue;

		private MessageBuffer _currentlySendingMessage;

		private bool _currentlyAcceptingClients;

		private ConcurrentQueue<TcpSocket> _incomingConnections;

		private int _lastMessageTotalRead;

		private TcpStatus _status;

		private bool _processingReceive;

		private string _remoteEndComputerName;

		private readonly MessageBuffer _peerAliveMessageBuffer;

		private readonly MessageBuffer _disconnectMessageBuffer;
	}
}
