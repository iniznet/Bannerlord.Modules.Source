using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class SetUsableMissionObjectIsDisabledForPlayers : GameNetworkMessage
	{
		public MissionObjectId UsableGameObjectId { get; private set; }

		public bool IsDisabledForPlayers { get; private set; }

		public SetUsableMissionObjectIsDisabledForPlayers(MissionObjectId usableGameObjectId, bool isDisabledForPlayers)
		{
			this.UsableGameObjectId = usableGameObjectId;
			this.IsDisabledForPlayers = isDisabledForPlayers;
		}

		public SetUsableMissionObjectIsDisabledForPlayers()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.UsableGameObjectId = GameNetworkMessage.ReadMissionObjectIdFromPacket(ref flag);
			this.IsDisabledForPlayers = GameNetworkMessage.ReadBoolFromPacket(ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteMissionObjectIdToPacket(this.UsableGameObjectId);
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
				this.UsableGameObjectId
			});
		}
	}
}
