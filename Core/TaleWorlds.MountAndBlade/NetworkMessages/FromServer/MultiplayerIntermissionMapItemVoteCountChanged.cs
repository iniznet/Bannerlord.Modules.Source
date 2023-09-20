using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	// Token: 0x02000052 RID: 82
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class MultiplayerIntermissionMapItemVoteCountChanged : GameNetworkMessage
	{
		// Token: 0x1700008D RID: 141
		// (get) Token: 0x060002E3 RID: 739 RVA: 0x00005CB5 File Offset: 0x00003EB5
		// (set) Token: 0x060002E4 RID: 740 RVA: 0x00005CBD File Offset: 0x00003EBD
		public int MapItemIndex { get; private set; }

		// Token: 0x1700008E RID: 142
		// (get) Token: 0x060002E5 RID: 741 RVA: 0x00005CC6 File Offset: 0x00003EC6
		// (set) Token: 0x060002E6 RID: 742 RVA: 0x00005CCE File Offset: 0x00003ECE
		public int VoteCount { get; private set; }

		// Token: 0x060002E7 RID: 743 RVA: 0x00005CD7 File Offset: 0x00003ED7
		public MultiplayerIntermissionMapItemVoteCountChanged()
		{
		}

		// Token: 0x060002E8 RID: 744 RVA: 0x00005CDF File Offset: 0x00003EDF
		public MultiplayerIntermissionMapItemVoteCountChanged(int mapItemIndex, int voteCount)
		{
			this.MapItemIndex = mapItemIndex;
			this.VoteCount = voteCount;
		}

		// Token: 0x060002E9 RID: 745 RVA: 0x00005CF8 File Offset: 0x00003EF8
		protected override bool OnRead()
		{
			bool flag = true;
			this.MapItemIndex = GameNetworkMessage.ReadIntFromPacket(CompressionBasic.IntermissionMapVoteItemCountCompressionInfo, ref flag);
			this.VoteCount = GameNetworkMessage.ReadIntFromPacket(CompressionBasic.IntermissionVoterCountCompressionInfo, ref flag);
			return flag;
		}

		// Token: 0x060002EA RID: 746 RVA: 0x00005D2C File Offset: 0x00003F2C
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteIntToPacket(this.MapItemIndex, CompressionBasic.IntermissionMapVoteItemCountCompressionInfo);
			GameNetworkMessage.WriteIntToPacket(this.VoteCount, CompressionBasic.IntermissionVoterCountCompressionInfo);
		}

		// Token: 0x060002EB RID: 747 RVA: 0x00005D4E File Offset: 0x00003F4E
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Administration;
		}

		// Token: 0x060002EC RID: 748 RVA: 0x00005D56 File Offset: 0x00003F56
		protected override string OnGetLogFormat()
		{
			return string.Format("Vote count changed for map with index: {0}, vote count: {1}.", this.MapItemIndex, this.VoteCount);
		}
	}
}
