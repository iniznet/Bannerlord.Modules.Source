using System;

namespace TaleWorlds.Network
{
	// Token: 0x02000021 RID: 33
	public abstract class NetworkSession
	{
		// Token: 0x060000D8 RID: 216 RVA: 0x00003A99 File Offset: 0x00001C99
		protected NetworkSession()
		{
			this._messageContractHandlerManager = new MessageContractHandlerManager();
		}

		// Token: 0x060000D9 RID: 217 RVA: 0x00003AAC File Offset: 0x00001CAC
		public void SendDisconnectMessage()
		{
			this.Socket.SendDisconnectMessage();
		}

		// Token: 0x060000DA RID: 218 RVA: 0x00003AB9 File Offset: 0x00001CB9
		protected internal virtual void OnConnected()
		{
		}

		// Token: 0x060000DB RID: 219 RVA: 0x00003ABB File Offset: 0x00001CBB
		protected internal virtual void OnSocketSet()
		{
		}

		// Token: 0x060000DC RID: 220 RVA: 0x00003ABD File Offset: 0x00001CBD
		protected internal virtual void OnDisconnected()
		{
		}

		// Token: 0x060000DD RID: 221 RVA: 0x00003ABF File Offset: 0x00001CBF
		protected internal virtual void OnCantConnect()
		{
		}

		// Token: 0x060000DE RID: 222 RVA: 0x00003AC1 File Offset: 0x00001CC1
		protected internal virtual void OnMessageReceived(INetworkMessageReader networkMessage)
		{
		}

		// Token: 0x060000DF RID: 223 RVA: 0x00003AC3 File Offset: 0x00001CC3
		public virtual void Tick()
		{
		}

		// Token: 0x060000E0 RID: 224 RVA: 0x00003AC5 File Offset: 0x00001CC5
		internal void HandleMessage(MessageContract messageContract)
		{
			this._messageContractHandlerManager.HandleMessage(messageContract);
		}

		// Token: 0x060000E1 RID: 225 RVA: 0x00003AD3 File Offset: 0x00001CD3
		public void AddMessageHandler<T>(MessageContractHandlerDelegate<T> handler) where T : MessageContract
		{
			this._messageContractHandlerManager.AddMessageHandler<T>(handler);
		}

		// Token: 0x060000E2 RID: 226 RVA: 0x00003AE1 File Offset: 0x00001CE1
		internal Type GetMessageContractType(byte id)
		{
			return this._messageContractHandlerManager.GetMessageContractType(id);
		}

		// Token: 0x060000E3 RID: 227 RVA: 0x00003AEF File Offset: 0x00001CEF
		internal bool ContainsMessageHandler(byte id)
		{
			return this._messageContractHandlerManager.ContainsMessageHandler(id);
		}

		// Token: 0x060000E4 RID: 228 RVA: 0x00003B00 File Offset: 0x00001D00
		public void SendMessage(MessageContract message)
		{
			if (this.IsActive)
			{
				NetworkMessage networkMessage = NetworkMessage.CreateForWriting();
				networkMessage.BeginWrite();
				networkMessage.Write(message);
				networkMessage.FinalizeWrite();
				int dataLength = networkMessage.DataLength;
				networkMessage.DataLength = dataLength;
				networkMessage.UpdateHeader();
				this.Socket.SendMessage(networkMessage.MessageBuffer);
			}
		}

		// Token: 0x060000E5 RID: 229 RVA: 0x00003B54 File Offset: 0x00001D54
		protected void SendPlainMessage(MessageContract message)
		{
			NetworkMessage networkMessage = NetworkMessage.CreateForWriting();
			networkMessage.BeginWrite();
			networkMessage.Write(message);
			networkMessage.FinalizeWrite();
			networkMessage.UpdateHeader();
			this.Socket.SendMessage(networkMessage.MessageBuffer);
		}

		// Token: 0x17000023 RID: 35
		// (get) Token: 0x060000E6 RID: 230 RVA: 0x00003B91 File Offset: 0x00001D91
		public bool IsActive
		{
			get
			{
				return this.Socket != null;
			}
		}

		// Token: 0x17000024 RID: 36
		// (get) Token: 0x060000E7 RID: 231 RVA: 0x00003B9C File Offset: 0x00001D9C
		// (set) Token: 0x060000E8 RID: 232 RVA: 0x00003BA4 File Offset: 0x00001DA4
		internal TcpSocket Socket
		{
			get
			{
				return this._socket;
			}
			set
			{
				this._socket = value;
				this.OnSocketSet();
			}
		}

		// Token: 0x17000025 RID: 37
		// (get) Token: 0x060000E9 RID: 233 RVA: 0x00003BB3 File Offset: 0x00001DB3
		public string Address
		{
			get
			{
				return this.Socket.IPAddress;
			}
		}

		// Token: 0x17000026 RID: 38
		// (get) Token: 0x060000EA RID: 234 RVA: 0x00003BC0 File Offset: 0x00001DC0
		public int LastMessageSentTime
		{
			get
			{
				return this.Socket.LastMessageSentTime;
			}
		}

		// Token: 0x17000027 RID: 39
		// (get) Token: 0x060000EB RID: 235 RVA: 0x00003BCD File Offset: 0x00001DCD
		public bool IsConnected
		{
			get
			{
				return this.Socket != null && this.Socket.IsConnected;
			}
		}

		// Token: 0x0400003F RID: 63
		public const double AliveMessageIntervalInSecs = 5.0;

		// Token: 0x04000040 RID: 64
		private MessageContractHandlerManager _messageContractHandlerManager;

		// Token: 0x04000041 RID: 65
		private TcpSocket _socket;

		// Token: 0x0200003D RID: 61
		// (Invoke) Token: 0x0600018A RID: 394
		public delegate void ComponentMessageHandlerDelegate(NetworkMessage networkMessage);
	}
}
