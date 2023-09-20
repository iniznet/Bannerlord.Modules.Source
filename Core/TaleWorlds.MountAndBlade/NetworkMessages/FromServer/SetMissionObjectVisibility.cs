using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class SetMissionObjectVisibility : GameNetworkMessage
	{
		public MissionObjectId MissionObjectId { get; private set; }

		public bool Visible { get; private set; }

		public SetMissionObjectVisibility(MissionObjectId missionObjectId, bool visible)
		{
			this.MissionObjectId = missionObjectId;
			this.Visible = visible;
		}

		public SetMissionObjectVisibility()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.MissionObjectId = GameNetworkMessage.ReadMissionObjectIdFromPacket(ref flag);
			this.Visible = GameNetworkMessage.ReadBoolFromPacket(ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteMissionObjectIdToPacket(this.MissionObjectId);
			GameNetworkMessage.WriteBoolToPacket(this.Visible);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.MissionObjects;
		}

		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"Set Visibility of MissionObject with ID: ",
				this.MissionObjectId,
				" to: ",
				this.Visible ? "True" : "False"
			});
		}
	}
}
