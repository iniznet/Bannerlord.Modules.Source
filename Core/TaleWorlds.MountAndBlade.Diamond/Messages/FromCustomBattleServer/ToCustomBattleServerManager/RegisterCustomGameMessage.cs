using System;
using TaleWorlds.Diamond;

namespace Messages.FromCustomBattleServer.ToCustomBattleServerManager
{
	[MessageDescription("CustomBattleServer", "CustomBattleServerManager")]
	[Serializable]
	public class RegisterCustomGameMessage : Message
	{
		public int GameDefinitionId { get; private set; }

		public string GameModule { get; private set; }

		public string GameType { get; private set; }

		public string ServerName { get; private set; }

		public string ServerAddress { get; private set; }

		public int MaxPlayerCount { get; private set; }

		public string Map { get; private set; }

		public string UniqueMapId { get; private set; }

		public string GamePassword { get; private set; }

		public string AdminPassword { get; private set; }

		public int Port { get; private set; }

		public string Region { get; private set; }

		public int Permission { get; private set; }

		public bool IsOverridingIP { get; private set; }

		public bool CrossplayEnabled { get; private set; }

		public RegisterCustomGameMessage(int gameDefinitionId, string gameModule, string gameType, string serverName, string serverAddress, int maxPlayerCount, string map, string uniqueMapId, string gamePassword, string adminPassword, int port, string region, int permission, bool crossplayEnabled, bool isOverridingIP)
		{
			this.GameDefinitionId = gameDefinitionId;
			this.GameModule = gameModule;
			this.GameType = gameType;
			this.ServerName = serverName;
			this.ServerAddress = serverAddress;
			this.MaxPlayerCount = maxPlayerCount;
			this.Map = map;
			this.UniqueMapId = uniqueMapId;
			this.GamePassword = gamePassword;
			this.AdminPassword = adminPassword;
			this.Port = port;
			this.Region = region;
			this.Permission = permission;
			this.CrossplayEnabled = crossplayEnabled;
			this.IsOverridingIP = isOverridingIP;
		}
	}
}
