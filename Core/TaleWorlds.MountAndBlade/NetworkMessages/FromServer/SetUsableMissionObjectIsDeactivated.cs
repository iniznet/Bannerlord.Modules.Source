using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class SetUsableMissionObjectIsDeactivated : GameNetworkMessage
	{
		public MissionObjectId UsableGameObjectId { get; private set; }

		public bool IsDeactivated { get; private set; }

		public SetUsableMissionObjectIsDeactivated(MissionObjectId usableGameObjectId, bool isDeactivated)
		{
			this.UsableGameObjectId = usableGameObjectId;
			this.IsDeactivated = isDeactivated;
		}

		public SetUsableMissionObjectIsDeactivated()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.UsableGameObjectId = GameNetworkMessage.ReadMissionObjectIdFromPacket(ref flag);
			this.IsDeactivated = GameNetworkMessage.ReadBoolFromPacket(ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteMissionObjectIdToPacket(this.UsableGameObjectId);
			GameNetworkMessage.WriteBoolToPacket(this.IsDeactivated);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.MissionObjects;
		}

		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"Set IsDeactivated: ",
				this.IsDeactivated ? "True" : "False",
				" on UsableMissionObject with ID: ",
				this.UsableGameObjectId
			});
		}
	}
}
