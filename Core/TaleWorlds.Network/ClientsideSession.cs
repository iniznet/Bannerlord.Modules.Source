using System;
using System.Collections.Concurrent;
using System.Threading;
using TaleWorlds.Library;

namespace TaleWorlds.Network
{
	// Token: 0x02000013 RID: 19
	public abstract class ClientsideSession : NetworkSession
	{
		// Token: 0x06000067 RID: 103 RVA: 0x00002D3E File Offset: 0x00000F3E
		protected void SendMessagePeerAlive()
		{
			base.Socket.SendPeerAliveMessage();
		}

		// Token: 0x06000068 RID: 104 RVA: 0x00002D4B File Offset: 0x00000F4B
		protected internal override void OnDisconnected()
		{
		}

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x06000069 RID: 105 RVA: 0x00002D4D File Offset: 0x00000F4D
		// (set) Token: 0x0600006A RID: 106 RVA: 0x00002D55 File Offset: 0x00000F55
		public int Port { get; set; }

		// Token: 0x0600006B RID: 107 RVA: 0x00002D5E File Offset: 0x00000F5E
		protected ClientsideSession()
		{
			this._incomingMessages = new ConcurrentQueue<MessageBuffer>();
		}

		// Token: 0x0600006C RID: 108 RVA: 0x00002D74 File Offset: 0x00000F74
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

		// Token: 0x0600006D RID: 109 RVA: 0x00002E0A File Offset: 0x0000100A
		private void OnSocketMessageReceived(MessageBuffer messageBuffer)
		{
			this._incomingMessages.Enqueue(messageBuffer);
		}

		// Token: 0x0600006E RID: 110 RVA: 0x00002E18 File Offset: 0x00001018
		public void Process()
		{
			while (this.ProcessTick())
			{
				Thread.Sleep(1);
			}
		}

		// Token: 0x0600006F RID: 111 RVA: 0x00002E2C File Offset: 0x0000102C
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

		// Token: 0x06000070 RID: 112 RVA: 0x00002EAC File Offset: 0x000010AC
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

		// Token: 0x0400002D RID: 45
		private bool _connectionResultHandled;

		// Token: 0x0400002E RID: 46
		private Thread _thread;

		// Token: 0x0400002F RID: 47
		private ConcurrentQueue<MessageBuffer> _incomingMessages;

		// Token: 0x04000030 RID: 48
		private bool _useSessionThread;
	}
}
