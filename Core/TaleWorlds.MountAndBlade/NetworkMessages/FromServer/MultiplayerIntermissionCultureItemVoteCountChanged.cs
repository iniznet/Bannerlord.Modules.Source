using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class MultiplayerIntermissionCultureItemVoteCountChanged : GameNetworkMessage
	{
		public int CultureItemIndex { get; private set; }

		public int VoteCount { get; private set; }

		public MultiplayerIntermissionCultureItemVoteCountChanged()
		{
		}

		public MultiplayerIntermissionCultureItemVoteCountChanged(int cultureItemIndex, int voteCount)
		{
			this.CultureItemIndex = cultureItemIndex;
			this.VoteCount = voteCount;
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.CultureItemIndex = GameNetworkMessage.ReadIntFromPacket(CompressionBasic.CultureIndexCompressionInfo, ref flag);
			this.VoteCount = GameNetworkMessage.ReadIntFromPacket(CompressionBasic.IntermissionVoterCountCompressionInfo, ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteIntToPacket(this.CultureItemIndex, CompressionBasic.CultureIndexCompressionInfo);
			GameNetworkMessage.WriteIntToPacket(this.VoteCount, CompressionBasic.IntermissionVoterCountCompressionInfo);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Administration;
		}

		protected override string OnGetLogFormat()
		{
			return string.Format("Vote count changed for culture with index: {0}, vote count: {1}.", this.CultureItemIndex, this.VoteCount);
		}
	}
}
