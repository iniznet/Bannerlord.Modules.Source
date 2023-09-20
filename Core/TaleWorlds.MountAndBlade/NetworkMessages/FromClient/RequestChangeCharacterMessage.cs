using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromClient
{
	// Token: 0x0200000A RID: 10
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromClient)]
	public sealed class RequestChangeCharacterMessage : GameNetworkMessage
	{
		// Token: 0x1700000C RID: 12
		// (get) Token: 0x0600003A RID: 58 RVA: 0x0000256F File Offset: 0x0000076F
		// (set) Token: 0x0600003B RID: 59 RVA: 0x00002577 File Offset: 0x00000777
		public NetworkCommunicator NetworkPeer { get; private set; }

		// Token: 0x0600003C RID: 60 RVA: 0x00002580 File Offset: 0x00000780
		public RequestChangeCharacterMessage(NetworkCommunicator networkPeer)
		{
			this.NetworkPeer = networkPeer;
		}

		// Token: 0x0600003D RID: 61 RVA: 0x0000258F File Offset: 0x0000078F
		public RequestChangeCharacterMessage()
		{
		}

		// Token: 0x0600003E RID: 62 RVA: 0x00002597 File Offset: 0x00000797
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteNetworkPeerReferenceToPacket(this.NetworkPeer);
		}

		// Token: 0x0600003F RID: 63 RVA: 0x000025A4 File Offset: 0x000007A4
		protected override bool OnRead()
		{
			bool flag = true;
			this.NetworkPeer = GameNetworkMessage.ReadNetworkPeerReferenceFromPacket(ref flag, false);
			return flag;
		}

		// Token: 0x06000040 RID: 64 RVA: 0x000025C2 File Offset: 0x000007C2
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.GameMode;
		}

		// Token: 0x06000041 RID: 65 RVA: 0x000025CA File Offset: 0x000007CA
		protected override string OnGetLogFormat()
		{
			return this.NetworkPeer.UserName + " has requested to change character.";
		}
	}
}
