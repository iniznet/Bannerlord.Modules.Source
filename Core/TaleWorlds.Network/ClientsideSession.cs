using System;
using System.Collections.Concurrent;
using System.Threading;
using TaleWorlds.Library;

namespace TaleWorlds.Network
{
	public abstract class ClientsideSession : NetworkSession
	{
		protected void SendMessagePeerAlive()
		{
			base.Socket.SendPeerAliveMessage();
		}

		protected internal override void OnDisconnected()
		{
		}

		public int Port { get; set; }

		protected ClientsideSession()
		{
			this._incomingMessages = new ConcurrentQueue<MessageBuffer>();
		}

		public virtual void Connect(string ip, int port, bool useSessionThread = true)
		{
			this._useSessionThread = useSessionThread;
			base.Socket = new TcpSocket();
			this.Port = port;
			base.Socket.MessageReceived += this.OnSocketMessageReceived;
			base.Socket.Connect(ip, port);
			if (this._useSessionThread)
			{
				this._thread = new Thread(new ThreadStart(this.Process));
				this._thread.IsBackground = true;
				this._thread.Name = this + " - Client Thread";
				this._thread.Start();
			}
		}

		private void OnSocketMessageReceived(MessageBuffer messageBuffer)
		{
			this._incomingMessages.Enqueue(messageBuffer);
		}

		public void Process()
		{
			while (this.ProcessTick())
			{
				Thread.Sleep(1);
			}
		}

		private bool ProcessTick()
		{
			base.Socket.ProcessWrite();
			base.Socket.ProcessRead();
			if (base.Socket != null && base.Socket.IsConnected && (double)((Environment.TickCount - base.Socket.LastMessageSentTime) / 1000) > 5.0)
			{
				this.SendMessagePeerAlive();
			}
			return base.Socket.Status != TcpStatus.SocketClosed && base.Socket.Status != TcpStatus.ConnectionClosed;
		}

		public override void Tick()
		{
			if (base.Socket == null)
			{
				return;
			}
			if (base.Socket.IsConnected)
			{
				if (!this._connectionResultHandled)
				{
					Debug.Print("Client connected! Connection result handle begin.", 0, Debug.DebugColor.White, 17592186044416UL);
					this.OnConnected();
					this._connectionResultHandled = true;
				}
				if (!this._useSessionThread)
				{
					this.ProcessTick();
				}
				int count = this._incomingMessages.Count;
				for (int i = 0; i < count; i++)
				{
					MessageBuffer messageBuffer = null;
					this._incomingMessages.TryDequeue(out messageBuffer);
					NetworkMessage networkMessage = NetworkMessage.CreateForReading(messageBuffer);
					networkMessage.BeginRead();
					byte b = networkMessage.ReadByte();
					Type messageContractType = base.GetMessageContractType(b);
					MessageContract messageContract = MessageContract.CreateMessageContract(messageContractType);
					Debug.Print(string.Concat(new object[] { "Message with id: ", b, " / contract: ", messageContractType, " received from server" }), 0, Debug.DebugColor.White, 17592186044416UL);
					messageContract.DeserializeFromNetworkMessage(networkMessage);
					base.HandleMessage(messageContract);
				}
				return;
			}
			if (base.Socket.Status == TcpStatus.ConnectionClosed)
			{
				if (!this._connectionResultHandled)
				{
					Debug.Print("ClientTcpSession can't connect!", 0, Debug.DebugColor.White, 17592186044416UL);
					this._connectionResultHandled = true;
					this.OnCantConnect();
				}
				else
				{
					Debug.Print("Peer disconnected!", 0, Debug.DebugColor.White, 17592186044416UL);
					this.OnDisconnected();
				}
				base.Socket.Close();
				this._connectionResultHandled = false;
			}
		}

		private bool _connectionResultHandled;

		private Thread _thread;

		private ConcurrentQueue<MessageBuffer> _incomingMessages;

		private bool _useSessionThread;
	}
}
