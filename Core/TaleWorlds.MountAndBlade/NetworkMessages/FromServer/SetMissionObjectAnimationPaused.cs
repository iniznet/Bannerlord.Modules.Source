using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class SetMissionObjectAnimationPaused : GameNetworkMessage
	{
		public MissionObjectId MissionObjectId { get; private set; }

		public bool IsPaused { get; private set; }

		public SetMissionObjectAnimationPaused(MissionObjectId missionObjectId, bool isPaused)
		{
			this.MissionObjectId = missionObjectId;
			this.IsPaused = isPaused;
		}

		public SetMissionObjectAnimationPaused()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.MissionObjectId = GameNetworkMessage.ReadMissionObjectIdFromPacket(ref flag);
			this.IsPaused = GameNetworkMessage.ReadBoolFromPacket(ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteMissionObjectIdToPacket(this.MissionObjectId);
			GameNetworkMessage.WriteBoolToPacket(this.IsPaused);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.MissionObjectsDetailed;
		}

		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"Set animation to be: ",
				this.IsPaused ? "Paused" : "Not paused",
				" on MissionObject with ID: ",
				this.MissionObjectId
			});
		}
	}
}
