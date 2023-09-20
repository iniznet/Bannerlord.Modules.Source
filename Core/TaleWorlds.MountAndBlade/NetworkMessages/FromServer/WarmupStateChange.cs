using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class WarmupStateChange : GameNetworkMessage
	{
		public MultiplayerWarmupComponent.WarmupStates WarmupState { get; private set; }

		public float StateStartTimeInSeconds { get; private set; }

		public WarmupStateChange(MultiplayerWarmupComponent.WarmupStates warmupState, long stateStartTimeInTicks)
		{
			this.WarmupState = warmupState;
			this.StateStartTimeInSeconds = (float)stateStartTimeInTicks / 10000000f;
		}

		public WarmupStateChange()
		{
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteIntToPacket((int)this.WarmupState, CompressionMission.MissionRoundStateCompressionInfo);
			GameNetworkMessage.WriteFloatToPacket(this.StateStartTimeInSeconds, CompressionMatchmaker.MissionTimeCompressionInfo);
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.WarmupState = (MultiplayerWarmupComponent.WarmupStates)GameNetworkMessage.ReadIntFromPacket(CompressionMission.MissionRoundStateCompressionInfo, ref flag);
			this.StateStartTimeInSeconds = GameNetworkMessage.ReadFloatFromPacket(CompressionMatchmaker.MissionTimeCompressionInfo, ref flag);
			return flag;
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.MissionDetailed;
		}

		protected override string OnGetLogFormat()
		{
			return "Warmup state set to " + this.WarmupState;
		}
	}
}
