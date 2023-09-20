using System;
using TaleWorlds.Diamond;
using TaleWorlds.MountAndBlade.Diamond;

namespace Messages.FromLobbyServer.ToClient
{
	[MessageDescription("LobbyServer", "Client")]
	[Serializable]
	public class ClientWantsToConnectCustomGameMessage : Message
	{
		public PlayerJoinGameData[] PlayerJoinGameData { get; private set; }

		public string Password { get; private set; }

		public ClientWantsToConnectCustomGameMessage(PlayerJoinGameData[] playerJoinGameData, string password)
		{
			this.PlayerJoinGameData = playerJoinGameData;
			this.Password = password;
		}
	}
}
