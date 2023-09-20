using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class SetUsableMissionObjectIsDisabledForPlayers : GameNetworkMessage
	{
		public UsableMissionObject UsableGameObject { get; private set; }

		public bool IsDisabledForPlayers { get; private set; }

		public SetUsableMissionObjectIsDisabledForPlayers(UsableMissionObject usableGameObject, bool isDisabledForPlayers)
		{
			this.UsableGameObject = usableGameObject;
			this.IsDisabledForPlayers = isDisabledForPlayers;
		}

		public SetUsableMissionObjectIsDisabledForPlayers()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.UsableGameObject = GameNetworkMessage.ReadMissionObjectReferenceFromPacket(ref flag) as UsableMissionObject;
			this.IsDisabledForPlayers = GameNetworkMessage.ReadBoolFromPacket(ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteMissionObjectReferenceToPacket(this.UsableGameObject);
			GameNetworkMessage.WriteBoolToPacket(this.IsDisabledForPlayers);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.MissionObjects;
		}

		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"Set IsDisabled for player: ",
				this.IsDisabledForPlayers ? "True" : "False",
				" on UsableMissionObject with ID: ",
				this.UsableGameObject.Id,
				" and with name: ",
				this.UsableGameObject.GameEntity.Name
			});
		}
	}
}
