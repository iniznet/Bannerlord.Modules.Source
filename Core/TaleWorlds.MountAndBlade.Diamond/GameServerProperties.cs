using System;
using System.Collections.Generic;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade.Diamond
{
	[Serializable]
	public class GameServerProperties
	{
		public string Name { get; private set; }

		public string Address { get; private set; }

		public int Port { get; private set; }

		public string Region { get; private set; }

		public string GameModule { get; private set; }

		public string GameType { get; private set; }

		public string Map { get; private set; }

		public string UniqueMapId { get; private set; }

		public string GamePassword { get; private set; }

		public string AdminPassword { get; private set; }

		public int MaxPlayerCount { get; private set; }

		public bool PasswordProtected { get; private set; }

		public bool IsOfficial { get; private set; }

		public bool ByOfficialProvider { get; private set; }

		public bool CrossplayEnabled { get; private set; }

		public int Permission { get; private set; }

		public PlayerId HostId { get; private set; }

		public string HostName { get; private set; }

		public List<ModuleInfoModel> LoadedModules { get; private set; }

		public bool AllowsOptionalModules { get; private set; }

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
