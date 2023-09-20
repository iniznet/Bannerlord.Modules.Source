using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class MissionStateChange : GameNetworkMessage
	{
		public MissionLobbyComponent.MultiplayerGameState CurrentState { get; private set; }

		public float StateStartTimeInSeconds { get; private set; }

		public MissionStateChange(MissionLobbyComponent.MultiplayerGameState currentState, long stateStartTimeInTicks)
		{
			this.CurrentState = currentState;
			this.StateStartTimeInSeconds = (float)stateStartTimeInTicks / 10000000f;
		}

		public MissionStateChange()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.CurrentState = (MissionLobbyComponent.MultiplayerGameState)GameNetworkMessage.ReadIntFromPacket(CompressionMatchmaker.MissionCurrentStateCompressionInfo, ref flag);
			if (this.CurrentState != MissionLobbyComponent.MultiplayerGameState.WaitingFirstPlayers)
			{
				this.StateStartTimeInSeconds = GameNetworkMessage.ReadFloatFromPacket(CompressionMatchmaker.MissionTimeCompressionInfo, ref flag);
			}
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteIntToPacket((int)this.CurrentState, CompressionMatchmaker.MissionCurrentStateCompressionInfo);
			if (this.CurrentState != MissionLobbyComponent.MultiplayerGameState.WaitingFirstPlayers)
			{
				GameNetworkMessage.WriteFloatToPacket(this.StateStartTimeInSeconds, CompressionMatchmaker.MissionTimeCompressionInfo);
			}
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Mission;
		}

		protected override string OnGetLogFormat()
		{
			return "Mission State has changed to: " + this.CurrentState;
		}
	}
}
