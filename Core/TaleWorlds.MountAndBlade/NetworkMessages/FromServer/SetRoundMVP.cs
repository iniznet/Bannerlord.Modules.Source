using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	// Token: 0x020000A4 RID: 164
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class SetRoundMVP : GameNetworkMessage
	{
		// Token: 0x1700017D RID: 381
		// (get) Token: 0x060006AC RID: 1708 RVA: 0x0000C4F8 File Offset: 0x0000A6F8
		// (set) Token: 0x060006AD RID: 1709 RVA: 0x0000C500 File Offset: 0x0000A700
		public NetworkCommunicator MVPPeer { get; private set; }

		// Token: 0x060006AE RID: 1710 RVA: 0x0000C509 File Offset: 0x0000A709
		public SetRoundMVP(NetworkCommunicator mvpPeer, int mvpCount)
		{
			this.MVPPeer = mvpPeer;
			this.MVPCount = mvpCount;
		}

		// Token: 0x060006AF RID: 1711 RVA: 0x0000C51F File Offset: 0x0000A71F
		public SetRoundMVP()
		{
		}

		// Token: 0x060006B0 RID: 1712 RVA: 0x0000C528 File Offset: 0x0000A728
		protected override bool OnRead()
		{
			bool flag = true;
			this.MVPPeer = GameNetworkMessage.ReadNetworkPeerReferenceFromPacket(ref flag, false);
			this.MVPCount = GameNetworkMessage.ReadIntFromPacket(CompressionBasic.RoundTotalCompressionInfo, ref flag);
			return flag;
		}

		// Token: 0x060006B1 RID: 1713 RVA: 0x0000C558 File Offset: 0x0000A758
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteNetworkPeerReferenceToPacket(this.MVPPeer);
			GameNetworkMessage.WriteIntToPacket(this.MVPCount, CompressionBasic.RoundTotalCompressionInfo);
		}

		// Token: 0x060006B2 RID: 1714 RVA: 0x0000C575 File Offset: 0x0000A775
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Mission | MultiplayerMessageFilter.GameMode;
		}

		// Token: 0x060006B3 RID: 1715 RVA: 0x0000C57D File Offset: 0x0000A77D
		protected override string OnGetLogFormat()
		{
			return "MVP selected as: " + this.MVPPeer.UserName + ".";
		}

		// Token: 0x0400017F RID: 383
		public int MVPCount;
	}
}
