using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class RoundStateChange : GameNetworkMessage
	{
		public MultiplayerRoundState RoundState { get; private set; }

		public float StateStartTimeInSeconds { get; private set; }

		public int RemainingTimeOnPreviousState { get; private set; }

		public RoundStateChange(MultiplayerRoundState roundState, long stateStartTimeInTicks, int remainingTimeOnPreviousState)
		{
			this.RoundState = roundState;
			this.StateStartTimeInSeconds = (float)stateStartTimeInTicks / 10000000f;
			this.RemainingTimeOnPreviousState = remainingTimeOnPreviousState;
		}

		public RoundStateChange()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.RoundState = (MultiplayerRoundState)GameNetworkMessage.ReadIntFromPacket(CompressionMission.MissionRoundStateCompressionInfo, ref flag);
			this.StateStartTimeInSeconds = GameNetworkMessage.ReadFloatFromPacket(CompressionMatchmaker.MissionTimeCompressionInfo, ref flag);
			this.RemainingTimeOnPreviousState = GameNetworkMessage.ReadIntFromPacket(CompressionMission.RoundTimeCompressionInfo, ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteIntToPacket((int)this.RoundState, CompressionMission.MissionRoundStateCompressionInfo);
			GameNetworkMessage.WriteFloatToPacket(this.StateStartTimeInSeconds, CompressionMatchmaker.MissionTimeCompressionInfo);
			GameNetworkMessage.WriteIntToPacket(this.RemainingTimeOnPreviousState, CompressionMission.RoundTimeCompressionInfo);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Mission;
		}

		protected override string OnGetLogFormat()
		{
			return "Changing round state to: " + this.RoundState;
		}
	}
}
