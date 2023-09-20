using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	// Token: 0x020000C9 RID: 201
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class SendVoiceToPlay : GameNetworkMessage
	{
		// Token: 0x170001DE RID: 478
		// (get) Token: 0x06000844 RID: 2116 RVA: 0x0000EC9B File Offset: 0x0000CE9B
		// (set) Token: 0x06000845 RID: 2117 RVA: 0x0000ECA3 File Offset: 0x0000CEA3
		public NetworkCommunicator Peer { get; private set; }

		// Token: 0x170001DF RID: 479
		// (get) Token: 0x06000846 RID: 2118 RVA: 0x0000ECAC File Offset: 0x0000CEAC
		// (set) Token: 0x06000847 RID: 2119 RVA: 0x0000ECB4 File Offset: 0x0000CEB4
		public byte[] Buffer { get; private set; }

		// Token: 0x170001E0 RID: 480
		// (get) Token: 0x06000848 RID: 2120 RVA: 0x0000ECBD File Offset: 0x0000CEBD
		// (set) Token: 0x06000849 RID: 2121 RVA: 0x0000ECC5 File Offset: 0x0000CEC5
		public int BufferLength { get; private set; }

		// Token: 0x0600084A RID: 2122 RVA: 0x0000ECCE File Offset: 0x0000CECE
		public SendVoiceToPlay()
		{
		}

		// Token: 0x0600084B RID: 2123 RVA: 0x0000ECD6 File Offset: 0x0000CED6
		public SendVoiceToPlay(NetworkCommunicator peer, byte[] buffer, int bufferLength)
		{
			this.Peer = peer;
			this.Buffer = buffer;
			this.BufferLength = bufferLength;
		}

		// Token: 0x0600084C RID: 2124 RVA: 0x0000ECF3 File Offset: 0x0000CEF3
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteNetworkPeerReferenceToPacket(this.Peer);
			GameNetworkMessage.WriteByteArrayToPacket(this.Buffer, 0, this.BufferLength);
		}

		// Token: 0x0600084D RID: 2125 RVA: 0x0000ED14 File Offset: 0x0000CF14
		protected override bool OnRead()
		{
			bool flag = true;
			this.Peer = GameNetworkMessage.ReadNetworkPeerReferenceFromPacket(ref flag, false);
			this.Buffer = new byte[1440];
			this.BufferLength = GameNetworkMessage.ReadByteArrayFromPacket(this.Buffer, 0, 1440, ref flag);
			return flag;
		}

		// Token: 0x0600084E RID: 2126 RVA: 0x0000ED5B File Offset: 0x0000CF5B
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.None;
		}

		// Token: 0x0600084F RID: 2127 RVA: 0x0000ED5F File Offset: 0x0000CF5F
		protected override string OnGetLogFormat()
		{
			return string.Empty;
		}
	}
}
