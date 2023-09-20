using System;
using TaleWorlds.Diamond;
using TaleWorlds.MountAndBlade.Diamond;

namespace Messages.FromLobbyServer.ToClient
{
	[MessageDescription("LobbyServer", "Client")]
	[Serializable]
	public class UpdatePlayerDataMessage : Message
	{
		public PlayerData PlayerData { get; private set; }

		public UpdatePlayerDataMessage(PlayerData playerData)
		{
			this.PlayerData = playerData;
		}
	}
}
