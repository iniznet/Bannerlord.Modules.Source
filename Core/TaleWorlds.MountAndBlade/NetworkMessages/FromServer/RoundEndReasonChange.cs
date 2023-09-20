using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class RoundEndReasonChange : GameNetworkMessage
	{
		public RoundEndReason RoundEndReason { get; private set; }

		public RoundEndReasonChange()
		{
			this.RoundEndReason = RoundEndReason.Invalid;
		}

		public RoundEndReasonChange(RoundEndReason roundEndReason)
		{
			this.RoundEndReason = roundEndReason;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteIntToPacket((int)this.RoundEndReason, CompressionMission.RoundEndReasonCompressionInfo);
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.RoundEndReason = (RoundEndReason)GameNetworkMessage.ReadIntFromPacket(CompressionMission.RoundEndReasonCompressionInfo, ref flag);
			return flag;
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.GameMode;
		}

		protected override string OnGetLogFormat()
		{
			return "Change round end reason to: " + this.RoundEndReason.ToString();
		}
	}
}
