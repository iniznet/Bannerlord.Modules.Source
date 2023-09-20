using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class SetMissionObjectVisibility : GameNetworkMessage
	{
		public MissionObject MissionObject { get; private set; }

		public bool Visible { get; private set; }

		public SetMissionObjectVisibility(MissionObject missionObject, bool visible)
		{
			this.MissionObject = missionObject;
			this.Visible = visible;
		}

		public SetMissionObjectVisibility()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.MissionObject = GameNetworkMessage.ReadMissionObjectReferenceFromPacket(ref flag);
			this.Visible = GameNetworkMessage.ReadBoolFromPacket(ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteMissionObjectReferenceToPacket(this.MissionObject);
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
				this.MissionObject.Id.Id,
				" and with name: ",
				this.MissionObject.GameEntity.Name,
				" to: ",
				this.Visible ? "True" : "False"
			});
		}
	}
}
