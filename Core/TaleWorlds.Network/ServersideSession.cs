using System;

namespace TaleWorlds.Network
{
	// Token: 0x02000022 RID: 34
	public abstract class ServersideSession : NetworkSession
	{
		// Token: 0x17000028 RID: 40
		// (get) Token: 0x060000EC RID: 236 RVA: 0x00003BE4 File Offset: 0x00001DE4
		// (set) Token: 0x060000ED RID: 237 RVA: 0x00003BEC File Offset: 0x00001DEC
		public int Index { get; internal set; }

		// Token: 0x17000029 RID: 41
		// (get) Token: 0x060000EE RID: 238 RVA: 0x00003BF5 File Offset: 0x00001DF5
		// (set) Token: 0x060000EF RID: 239 RVA: 0x00003BFD File Offset: 0x00001DFD
		internal ServersideSessionManager Server { get; private set; }

		// Token: 0x060000F0 RID: 240 RVA: 0x00003C06 File Offset: 0x00001E06
		protected ServersideSession(ServersideSessionManager server)
		{
			this.Server = server;
		}

		// Token: 0x060000F1 RID: 241 RVA: 0x00003C15 File Offset: 0x00001E15
		protected internal override void OnDisconnected()
		{
		}

		// Token: 0x060000F2 RID: 242 RVA: 0x00003C17 File Offset: 0x00001E17
		protected internal override void OnConnected()
		{
		}

		// Token: 0x060000F3 RID: 243 RVA: 0x00003C19 File Offset: 0x00001E19
		protected internal override void OnSocketSet()
		{
		}

		// Token: 0x060000F4 RID: 244 RVA: 0x00003C1B File Offset: 0x00001E1B
		internal void InitializeSocket(int id, TcpSocket socket)
		{
			this.Index = id;
			base.Socket = socket;
			base.Socket.MessageReceived += this.OnTcpSocketMessageReceived;
			base.Socket.Closed += this.OnTcpSocketClosed;
		}

		// Token: 0x060000F5 RID: 245 RVA: 0x00003C5C File Offset: 0x00001E5C
		private void OnTcpSocketMessageReceived(MessageBuffer messageBuffer)
		{
			IncomingServerSessionMessage incomingServerSessionMessage = new IncomingServerSessionMessage();
			incomingServerSessionMessage.Peer = this;
			incomingServerSessionMessage.NetworkMessage = NetworkMessage.CreateForReading(messageBuffer);
			this.Server.AddIncomingMessage(incomingServerSessionMessage);
		}

		// Token: 0x060000F6 RID: 246 RVA: 0x00003C8E File Offset: 0x00001E8E
		private void OnTcpSocketClosed()
		{
			this.Server.AddDisconnectedPeer(this);
		}
	}
}
