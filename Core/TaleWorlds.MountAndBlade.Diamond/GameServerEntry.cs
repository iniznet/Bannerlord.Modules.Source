using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using TaleWorlds.Library;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade.Diamond
{
	[Serializable]
	public class GameServerEntry
	{
		[JsonProperty]
		public CustomBattleId Id { get; private set; }

		[JsonProperty]
		public string Address { get; private set; }

		[JsonProperty]
		public int Port { get; private set; }

		[JsonProperty]
		public string Region { get; private set; }

		[JsonProperty]
		public int PlayerCount { get; private set; }

		[JsonProperty]
		public int MaxPlayerCount { get; private set; }

		[JsonProperty]
		public string ServerName { get; private set; }

		[JsonProperty]
		public string GameModule { get; private set; }

		[JsonProperty]
		public string GameType { get; private set; }

		[JsonProperty]
		public string Map { get; private set; }

		[JsonProperty]
		public string UniqueMapId { get; private set; }

		[JsonProperty]
		public int Ping { get; private set; }

		[JsonProperty]
		public bool IsOfficial { get; private set; }

		[JsonProperty]
		public bool ByOfficialProvider { get; private set; }

		[JsonProperty]
		public bool PasswordProtected { get; private set; }

		[JsonProperty]
		public int Permission { get; private set; }

		[JsonProperty]
		public bool CrossplayEnabled { get; private set; }

		[JsonProperty]
		public PlayerId HostId { get; private set; }

		[JsonProperty]
		public string HostName { get; private set; }

		[JsonProperty]
		public List<ModuleInfoModel> LoadedModules { get; private set; }

		[JsonProperty]
		public bool AllowsOptionalModules { get; private set; }

		public GameServerEntry()
		{
		}

		public GameServerEntry(CustomBattleId id, string serverName, string address, int port, string region, string gameModule, string gameType, string map, string uniqueMapId, int playerCount, int maxPlayerCount, bool isOfficial, bool byOfficialProvider, bool crossplayEnabled, PlayerId hostId, string hostName, List<ModuleInfoModel> loadedModules, bool allowsOptionalModules, bool passwordProtected = false, int permission = 0)
		{
			this.Id = id;
			this.ServerName = serverName;
			this.Address = address;
			this.GameModule = gameModule;
			this.GameType = gameType;
			this.Map = map;
			this.UniqueMapId = uniqueMapId;
			this.PlayerCount = playerCount;
			this.MaxPlayerCount = maxPlayerCount;
			this.Port = port;
			this.Region = region;
			this.IsOfficial = isOfficial;
			this.ByOfficialProvider = byOfficialProvider;
			this.CrossplayEnabled = crossplayEnabled;
			this.HostId = hostId;
			this.HostName = hostName;
			this.LoadedModules = loadedModules;
			this.AllowsOptionalModules = allowsOptionalModules;
			this.PasswordProtected = passwordProtected;
			this.Permission = permission;
		}

		public static void FilterGameServerEntriesBasedOnCrossplay(ref List<GameServerEntry> serverList, bool hasCrossplayPrivilege)
		{
			bool flag = ApplicationPlatform.CurrentPlatform == Platform.GDKDesktop;
			if (flag && !hasCrossplayPrivilege)
			{
				serverList.RemoveAll((GameServerEntry s) => s.CrossplayEnabled);
				return;
			}
			if (!flag)
			{
				serverList.RemoveAll((GameServerEntry s) => !s.CrossplayEnabled);
			}
		}
	}
}
