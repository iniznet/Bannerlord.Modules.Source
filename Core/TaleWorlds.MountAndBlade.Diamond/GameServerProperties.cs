using System;
using System.Collections.Generic;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade.Diamond
{
	[Serializable]
	public class GameServerProperties
	{
		public string Name { get; set; }

		public string Address { get; set; }

		public int Port { get; set; }

		public string Region { get; set; }

		public string GameModule { get; set; }

		public string GameType { get; set; }

		public string Map { get; set; }

		public string UniqueMapId { get; set; }

		public string GamePassword { get; set; }

		public string AdminPassword { get; set; }

		public int MaxPlayerCount { get; set; }

		public bool PasswordProtected { get; set; }

		public bool IsOfficial { get; set; }

		public bool ByOfficialProvider { get; set; }

		public bool CrossplayEnabled { get; set; }

		public int Permission { get; set; }

		public PlayerId HostId { get; set; }

		public string HostName { get; set; }

		public List<ModuleInfoModel> LoadedModules { get; set; }

		public bool AllowsOptionalModules { get; set; }

		public GameServerProperties()
		{
		}

		public GameServerProperties(string name, string address, int port, string region, string gameModule, string gameType, string map, string uniqueMapId, string gamePassword, string adminPassword, int maxPlayerCount, bool isOfficial, bool byOfficialProvider, bool crossplayEnabled, PlayerId hostId, string hostName, List<ModuleInfoModel> loadedModules, bool allowsOptionalModules, int permission)
		{
			this.Name = name;
			this.Address = address;
			this.Port = port;
			this.Region = region;
			this.GameModule = gameModule;
			this.GameType = gameType;
			this.Map = map;
			this.GamePassword = gamePassword;
			this.UniqueMapId = uniqueMapId;
			this.AdminPassword = adminPassword;
			this.MaxPlayerCount = maxPlayerCount;
			this.IsOfficial = isOfficial;
			this.ByOfficialProvider = byOfficialProvider;
			this.CrossplayEnabled = crossplayEnabled;
			this.HostId = hostId;
			this.HostName = hostName;
			this.LoadedModules = loadedModules;
			this.AllowsOptionalModules = allowsOptionalModules;
			this.PasswordProtected = gamePassword != null;
			this.Permission = permission;
		}

		public void CheckAndReplaceProxyAddress(IReadOnlyDictionary<string, string> proxyAddressMap)
		{
			string text;
			if (proxyAddressMap != null && proxyAddressMap.TryGetValue(this.Address, out text))
			{
				this.Address = text;
			}
		}
	}
}
