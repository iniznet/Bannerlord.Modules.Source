using System;
using Newtonsoft.Json;
using TaleWorlds.Diamond;
using TaleWorlds.MountAndBlade.Diamond;

namespace Messages.FromCustomBattleServerManager.ToCustomBattleServer
{
	[MessageDescription("CustomBattleServerManager", "CustomBattleServer")]
	[Serializable]
	public class ClientWantsToConnectCustomGameMessage : Message
	{
		[JsonProperty]
		public PlayerJoinGameData[] PlayerJoinGameData { get; private set; }

		[JsonProperty]
		public string Password { get; private set; }

		public ClientWantsToConnectCustomGameMessage()
		{
		}

		public ClientWantsToConnectCustomGameMessage(PlayerJoinGameData[] playerJoinGameData, string password)
		{
			this.PlayerJoinGameData = playerJoinGameData;
			this.Password = password;
		}
	}
}
