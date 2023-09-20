using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class SetBoundariesState : GameNetworkMessage
	{
		public bool IsOutside { get; private set; }

		public float StateStartTimeInSeconds { get; private set; }

		public SetBoundariesState()
		{
		}

		public SetBoundariesState(bool isOutside)
		{
			this.IsOutside = isOutside;
		}

		public SetBoundariesState(bool isOutside, long stateStartTimeInTicks)
			: this(isOutside)
		{
			this.StateStartTimeInSeconds = (float)stateStartTimeInTicks / 10000000f;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteBoolToPacket(this.IsOutside);
			if (this.IsOutside)
			{
				GameNetworkMessage.WriteFloatToPacket(this.StateStartTimeInSeconds, CompressionMatchmaker.MissionTimeCompressionInfo);
			}
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.IsOutside = GameNetworkMessage.ReadBoolFromPacket(ref flag);
			if (this.IsOutside)
			{
				this.StateStartTimeInSeconds = GameNetworkMessage.ReadFloatFromPacket(CompressionMatchmaker.MissionTimeCompressionInfo, ref flag);
			}
			return flag;
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.AgentsDetailed;
		}

		protected override string OnGetLogFormat()
		{
			if (!this.IsOutside)
			{
				return "I am now inside the level boundaries";
			}
			return "I am now outside of the level boundaries";
		}
	}
}
