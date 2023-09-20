using System;

namespace TaleWorlds.Network
{
	public abstract class NetworkSession
	{
		protected NetworkSession()
		{
			this._messageContractHandlerManager = new MessageContractHandlerManager();
		}

		public void SendDisconnectMessage()
		{
			this.Socket.SendDisconnectMessage();
		}

		protected internal virtual void OnConnected()
		{
		}

		protected internal virtual void OnSocketSet()
		{
		}

		protected internal virtual void OnDisconnected()
		{
		}

		protected internal virtual void OnCantConnect()
		{
		}

		protected internal virtual void OnMessageReceived(INetworkMessageReader networkMessage)
		{
		}

		public virtual void Tick()
		{
		}

		internal void HandleMessage(MessageContract messageContract)
		{
			this._messageContractHandlerManager.HandleMessage(messageContract);
		}

		public void AddMessageHandler<T>(MessageContractHandlerDelegate<T> handler) where T : MessageContract
		{
			this._messageContractHandlerManager.AddMessageHandler<T>(handler);
		}

		internal Type GetMessageContractType(byte id)
		{
			return this._messageContractHandlerManager.GetMessageContractType(id);
		}

		internal bool ContainsMessageHandler(byte id)
		{
			return this._messageContractHandlerManager.ContainsMessageHandler(id);
		}

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

		protected void SendPlainMessage(MessageContract message)
		{
			NetworkMessage networkMessage = NetworkMessage.CreateForWriting();
			networkMessage.BeginWrite();
			networkMessage.Write(message);
			networkMessage.FinalizeWrite();
			networkMessage.UpdateHeader();
			this.Socket.SendMessage(networkMessage.MessageBuffer);
		}

		public bool IsActive
		{
			get
			{
				return this.Socket != null;
			}
		}

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

		public string Address
		{
			get
			{
				return this.Socket.IPAddress;
			}
		}

		public int LastMessageSentTime
		{
			get
			{
				return this.Socket.LastMessageSentTime;
			}
		}

		public bool IsConnected
		{
			get
			{
				return this.Socket != null && this.Socket.IsConnected;
			}
		}

		public const double AliveMessageIntervalInSecs = 5.0;

		private MessageContractHandlerManager _messageContractHandlerManager;

		private TcpSocket _socket;

		public delegate void ComponentMessageHandlerDelegate(NetworkMessage networkMessage);
	}
}
