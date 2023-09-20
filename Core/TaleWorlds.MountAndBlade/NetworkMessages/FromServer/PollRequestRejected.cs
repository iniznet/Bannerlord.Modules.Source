using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class PollRequestRejected : GameNetworkMessage
	{
		public int Reason { get; private set; }

		public PollRequestRejected(int reason)
		{
			this.Reason = reason;
		}

		public PollRequestRejected()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.Reason = GameNetworkMessage.ReadIntFromPacket(CompressionMission.MultiplayerPollRejectReasonCompressionInfo, ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteIntToPacket(this.Reason, CompressionMission.MultiplayerPollRejectReasonCompressionInfo);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Administration;
		}

		protected override string OnGetLogFormat()
		{
			return "Poll request rejected (" + (MultiplayerPollRejectReason)this.Reason + ")";
		}
	}
}
