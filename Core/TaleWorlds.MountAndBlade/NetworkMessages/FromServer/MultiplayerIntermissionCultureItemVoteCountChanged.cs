using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	// Token: 0x02000050 RID: 80
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class MultiplayerIntermissionCultureItemVoteCountChanged : GameNetworkMessage
	{
		// Token: 0x1700008A RID: 138
		// (get) Token: 0x060002D1 RID: 721 RVA: 0x00005B81 File Offset: 0x00003D81
		// (set) Token: 0x060002D2 RID: 722 RVA: 0x00005B89 File Offset: 0x00003D89
		public int CultureItemIndex { get; private set; }

		// Token: 0x1700008B RID: 139
		// (get) Token: 0x060002D3 RID: 723 RVA: 0x00005B92 File Offset: 0x00003D92
		// (set) Token: 0x060002D4 RID: 724 RVA: 0x00005B9A File Offset: 0x00003D9A
		public int VoteCount { get; private set; }

		// Token: 0x060002D5 RID: 725 RVA: 0x00005BA3 File Offset: 0x00003DA3
		public MultiplayerIntermissionCultureItemVoteCountChanged()
		{
		}

		// Token: 0x060002D6 RID: 726 RVA: 0x00005BAB File Offset: 0x00003DAB
		public MultiplayerIntermissionCultureItemVoteCountChanged(int cultureItemIndex, int voteCount)
		{
			this.CultureItemIndex = cultureItemIndex;
			this.VoteCount = voteCount;
		}

		// Token: 0x060002D7 RID: 727 RVA: 0x00005BC4 File Offset: 0x00003DC4
		protected override bool OnRead()
		{
			bool flag = true;
			this.CultureItemIndex = GameNetworkMessage.ReadIntFromPacket(CompressionBasic.CultureIndexCompressionInfo, ref flag);
			this.VoteCount = GameNetworkMessage.ReadIntFromPacket(CompressionBasic.IntermissionVoterCountCompressionInfo, ref flag);
			return flag;
		}

		// Token: 0x060002D8 RID: 728 RVA: 0x00005BF8 File Offset: 0x00003DF8
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteIntToPacket(this.CultureItemIndex, CompressionBasic.CultureIndexCompressionInfo);
			GameNetworkMessage.WriteIntToPacket(this.VoteCount, CompressionBasic.IntermissionVoterCountCompressionInfo);
		}

		// Token: 0x060002D9 RID: 729 RVA: 0x00005C1A File Offset: 0x00003E1A
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Administration;
		}

		// Token: 0x060002DA RID: 730 RVA: 0x00005C22 File Offset: 0x00003E22
		protected override string OnGetLogFormat()
		{
			return string.Format("Vote count changed for culture with index: {0}, vote count: {1}.", this.CultureItemIndex, this.VoteCount);
		}
	}
}
