using System;

namespace TaleWorlds.Network
{
	public abstract class ServersideSession : NetworkSession
	{
		public int Index { get; internal set; }

		internal ServersideSessionManager Server { get; private set; }

		protected ServersideSession(ServersideSessionManager server)
		{
			this.Server = server;
		}

		protected internal override void OnDisconnected()
		{
		}

		protected internal override void OnConnected()
		{
		}

		protected internal override void OnSocketSet()
		{
		}

		internal void InitializeSocket(int id, TcpSocket socket)
		{
			this.Index = id;
			base.Socket = socket;
			base.Socket.MessageReceived += this.OnTcpSocketMessageReceived;
			base.Socket.Closed += this.OnTcpSocketClosed;
		}

		private void OnTcpSocketMessageReceived(MessageBuffer messageBuffer)
		{
			IncomingServerSessionMessage incomingServerSessionMessage = new IncomingServerSessionMessage();
			incomingServerSessionMessage.Peer = this;
			incomingServerSessionMessage.NetworkMessage = NetworkMessage.CreateForReading(messageBuffer);
			this.Server.AddIncomingMessage(incomingServerSessionMessage);
		}

		private void OnTcpSocketClosed()
		{
			this.Server.AddDisconnectedPeer(this);
		}
	}
}
