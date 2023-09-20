using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromClient
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromClient)]
	public sealed class IntermissionVote : GameNetworkMessage
	{
		public int VoteCount { get; private set; }

		public string ItemID { get; private set; }

		public IntermissionVote(string itemID, int voteCount)
		{
			this.VoteCount = voteCount;
			this.ItemID = itemID;
		}

		public IntermissionVote()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.ItemID = GameNetworkMessage.ReadStringFromPacket(ref flag);
			this.VoteCount = GameNetworkMessage.ReadIntFromPacket(new CompressionInfo.Integer(-1, 1, true), ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteStringToPacket(this.ItemID);
			GameNetworkMessage.WriteIntToPacket(this.VoteCount, new CompressionInfo.Integer(-1, 1, true));
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Administration;
		}

		protected override string OnGetLogFormat()
		{
			return string.Format("Intermission vote casted for item with ID: {0} with count: {1}.", this.ItemID, this.VoteCount);
		}
	}
}
