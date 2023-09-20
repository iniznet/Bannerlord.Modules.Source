using System;
using TaleWorlds.Diamond;

namespace Messages.FromClient.ToLobbyServer
{
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class RegisterCustomGameMessage : Message
	{
		public string GameModule { get; private set; }

		public string GameType { get; private set; }

		public string ServerName { get; private set; }

		public int MaxPlayerCount { get; private set; }

		public string Map { get; private set; }

		public string UniqueMapId { get; private set; }

		public int Port { get; private set; }

		public string GamePassword { get; private set; }

		public string AdminPassword { get; private set; }

		public RegisterCustomGameMessage(string gameModule, string gameType, string serverName, int maxPlayerCount, string map, string uniqueMapId, string gamePassword, string adminPassword, int port)
		{
			this.GameModule = gameModule;
			this.GameType = gameType;
			this.ServerName = serverName;
			this.MaxPlayerCount = maxPlayerCount;
			this.Map = map;
			this.UniqueMapId = uniqueMapId;
			this.GamePassword = gamePassword;
			this.AdminPassword = adminPassword;
			this.Port = port;
		}
	}
}
