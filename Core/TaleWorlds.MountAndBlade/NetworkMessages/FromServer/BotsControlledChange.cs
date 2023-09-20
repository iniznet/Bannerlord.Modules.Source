using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	// Token: 0x02000046 RID: 70
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class BotsControlledChange : GameNetworkMessage
	{
		// Token: 0x1700006D RID: 109
		// (get) Token: 0x0600025B RID: 603 RVA: 0x00004FB0 File Offset: 0x000031B0
		// (set) Token: 0x0600025C RID: 604 RVA: 0x00004FB8 File Offset: 0x000031B8
		public NetworkCommunicator Peer { get; private set; }

		// Token: 0x1700006E RID: 110
		// (get) Token: 0x0600025D RID: 605 RVA: 0x00004FC1 File Offset: 0x000031C1
		// (set) Token: 0x0600025E RID: 606 RVA: 0x00004FC9 File Offset: 0x000031C9
		public int AliveCount { get; private set; }

		// Token: 0x1700006F RID: 111
		// (get) Token: 0x0600025F RID: 607 RVA: 0x00004FD2 File Offset: 0x000031D2
		// (set) Token: 0x06000260 RID: 608 RVA: 0x00004FDA File Offset: 0x000031DA
		public int TotalCount { get; private set; }

		// Token: 0x06000261 RID: 609 RVA: 0x00004FE3 File Offset: 0x000031E3
		public BotsControlledChange(NetworkCommunicator peer, int aliveCount, int totalCount)
		{
			this.Peer = peer;
			this.AliveCount = aliveCount;
			this.TotalCount = totalCount;
		}

		// Token: 0x06000262 RID: 610 RVA: 0x00005000 File Offset: 0x00003200
		public BotsControlledChange()
		{
		}

		// Token: 0x06000263 RID: 611 RVA: 0x00005008 File Offset: 0x00003208
		protected override bool OnRead()
		{
			bool flag = true;
			this.Peer = GameNetworkMessage.ReadNetworkPeerReferenceFromPacket(ref flag, false);
			this.AliveCount = GameNetworkMessage.ReadIntFromPacket(CompressionMission.AgentOffsetCompressionInfo, ref flag);
			this.TotalCount = GameNetworkMessage.ReadIntFromPacket(CompressionMission.AgentOffsetCompressionInfo, ref flag);
			return flag;
		}

		// Token: 0x06000264 RID: 612 RVA: 0x0000504A File Offset: 0x0000324A
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteNetworkPeerReferenceToPacket(this.Peer);
			GameNetworkMessage.WriteIntToPacket(this.AliveCount, CompressionMission.AgentOffsetCompressionInfo);
			GameNetworkMessage.WriteIntToPacket(this.TotalCount, CompressionMission.AgentOffsetCompressionInfo);
		}

		// Token: 0x06000265 RID: 613 RVA: 0x00005077 File Offset: 0x00003277
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Administration;
		}

		// Token: 0x06000266 RID: 614 RVA: 0x00005080 File Offset: 0x00003280
		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"Bot Controlled Count Changed. Peer: ",
				this.Peer.UserName,
				" now has ",
				this.AliveCount,
				" alive bots, out of: ",
				this.TotalCount,
				" total bots."
			});
		}
	}
}
