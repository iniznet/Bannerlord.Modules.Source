using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class SynchronizeMissionTimeTracker : GameNetworkMessage
	{
		public float CurrentTime { get; private set; }

		public SynchronizeMissionTimeTracker(float currentTime)
		{
			this.CurrentTime = currentTime;
		}

		public SynchronizeMissionTimeTracker()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.CurrentTime = GameNetworkMessage.ReadFloatFromPacket(CompressionMatchmaker.MissionTimeCompressionInfo, ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteFloatToPacket(this.CurrentTime, CompressionMatchmaker.MissionTimeCompressionInfo);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.MissionDetailed;
		}

		protected override string OnGetLogFormat()
		{
			return this.CurrentTime + " seconds have elapsed since the start of the mission.";
		}
	}
}
