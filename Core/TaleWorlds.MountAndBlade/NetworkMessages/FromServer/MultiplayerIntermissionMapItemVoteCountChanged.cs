using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class MultiplayerIntermissionMapItemVoteCountChanged : GameNetworkMessage
	{
		public int MapItemIndex { get; private set; }

		public int VoteCount { get; private set; }

		public MultiplayerIntermissionMapItemVoteCountChanged()
		{
		}

		public MultiplayerIntermissionMapItemVoteCountChanged(int mapItemIndex, int voteCount)
		{
			this.MapItemIndex = mapItemIndex;
			this.VoteCount = voteCount;
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.MapItemIndex = GameNetworkMessage.ReadIntFromPacket(CompressionBasic.IntermissionMapVoteItemCountCompressionInfo, ref flag);
			this.VoteCount = GameNetworkMessage.ReadIntFromPacket(CompressionBasic.IntermissionVoterCountCompressionInfo, ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteIntToPacket(this.MapItemIndex, CompressionBasic.IntermissionMapVoteItemCountCompressionInfo);
			GameNetworkMessage.WriteIntToPacket(this.VoteCount, CompressionBasic.IntermissionVoterCountCompressionInfo);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Administration;
		}

		protected override string OnGetLogFormat()
		{
			return string.Format("Vote count changed for map with index: {0}, vote count: {1}.", this.MapItemIndex, this.VoteCount);
		}
	}
}
