using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class RoundCountChange : GameNetworkMessage
	{
		public int RoundCount { get; private set; }

		public RoundCountChange(int roundCount)
		{
			this.RoundCount = roundCount;
		}

		public RoundCountChange()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.RoundCount = GameNetworkMessage.ReadIntFromPacket(CompressionMission.MissionRoundCountCompressionInfo, ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteIntToPacket(this.RoundCount, CompressionMission.MissionRoundCountCompressionInfo);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Mission;
		}

		protected override string OnGetLogFormat()
		{
			return "Change round count to: " + this.RoundCount;
		}
	}
}
