using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class SetUsableMissionObjectIsDeactivated : GameNetworkMessage
	{
		public UsableMissionObject UsableGameObject { get; private set; }

		public bool IsDeactivated { get; private set; }

		public SetUsableMissionObjectIsDeactivated(UsableMissionObject usableGameObject, bool isDeactivated)
		{
			this.UsableGameObject = usableGameObject;
			this.IsDeactivated = isDeactivated;
		}

		public SetUsableMissionObjectIsDeactivated()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.UsableGameObject = GameNetworkMessage.ReadMissionObjectReferenceFromPacket(ref flag) as UsableMissionObject;
			this.IsDeactivated = GameNetworkMessage.ReadBoolFromPacket(ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteMissionObjectReferenceToPacket(this.UsableGameObject);
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
				this.UsableGameObject.Id,
				" and with name: ",
				this.UsableGameObject.GameEntity.Name
			});
		}
	}
}
