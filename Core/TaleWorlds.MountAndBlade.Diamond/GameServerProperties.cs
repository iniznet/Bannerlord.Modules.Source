using System;
using System.Collections.Generic;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade.Diamond
{
	// Token: 0x02000125 RID: 293
	[Serializable]
	public class GameServerProperties
	{
		// Token: 0x17000255 RID: 597
		// (get) Token: 0x0600069C RID: 1692 RVA: 0x0000B0E0 File Offset: 0x000092E0
		// (set) Token: 0x0600069D RID: 1693 RVA: 0x0000B0E8 File Offset: 0x000092E8
		public string Name { get; private set; }

		// Token: 0x17000256 RID: 598
		// (get) Token: 0x0600069E RID: 1694 RVA: 0x0000B0F1 File Offset: 0x000092F1
		// (set) Token: 0x0600069F RID: 1695 RVA: 0x0000B0F9 File Offset: 0x000092F9
		public string Address { get; private set; }

		// Token: 0x17000257 RID: 599
		// (get) Token: 0x060006A0 RID: 1696 RVA: 0x0000B102 File Offset: 0x00009302
		// (set) Token: 0x060006A1 RID: 1697 RVA: 0x0000B10A File Offset: 0x0000930A
		public int Port { get; private set; }

		// Token: 0x17000258 RID: 600
		// (get) Token: 0x060006A2 RID: 1698 RVA: 0x0000B113 File Offset: 0x00009313
		// (set) Token: 0x060006A3 RID: 1699 RVA: 0x0000B11B File Offset: 0x0000931B
		public string Region { get; private set; }

		// Token: 0x17000259 RID: 601
		// (get) Token: 0x060006A4 RID: 1700 RVA: 0x0000B124 File Offset: 0x00009324
		// (set) Token: 0x060006A5 RID: 1701 RVA: 0x0000B12C File Offset: 0x0000932C
		public string GameModule { get; private set; }

		// Token: 0x1700025A RID: 602
		// (get) Token: 0x060006A6 RID: 1702 RVA: 0x0000B135 File Offset: 0x00009335
		// (set) Token: 0x060006A7 RID: 1703 RVA: 0x0000B13D File Offset: 0x0000933D
		public string GameType { get; private set; }

		// Token: 0x1700025B RID: 603
		// (get) Token: 0x060006A8 RID: 1704 RVA: 0x0000B146 File Offset: 0x00009346
		// (set) Token: 0x060006A9 RID: 1705 RVA: 0x0000B14E File Offset: 0x0000934E
		public string Map { get; private set; }

		// Token: 0x1700025C RID: 604
		// (get) Token: 0x060006AA RID: 1706 RVA: 0x0000B157 File Offset: 0x00009357
		// (set) Token: 0x060006AB RID: 1707 RVA: 0x0000B15F File Offset: 0x0000935F
		public string UniqueMapId { get; private set; }

		// Token: 0x1700025D RID: 605
		// (get) Token: 0x060006AC RID: 1708 RVA: 0x0000B168 File Offset: 0x00009368
		// (set) Token: 0x060006AD RID: 1709 RVA: 0x0000B170 File Offset: 0x00009370
		public string GamePassword { get; private set; }

		// Token: 0x1700025E RID: 606
		// (get) Token: 0x060006AE RID: 1710 RVA: 0x0000B179 File Offset: 0x00009379
		// (set) Token: 0x060006AF RID: 1711 RVA: 0x0000B181 File Offset: 0x00009381
		public string AdminPassword { get; private set; }

		// Token: 0x1700025F RID: 607
		// (get) Token: 0x060006B0 RID: 1712 RVA: 0x0000B18A File Offset: 0x0000938A
		// (set) Token: 0x060006B1 RID: 1713 RVA: 0x0000B192 File Offset: 0x00009392
		public int MaxPlayerCount { get; private set; }

		// Token: 0x17000260 RID: 608
		// (get) Token: 0x060006B2 RID: 1714 RVA: 0x0000B19B File Offset: 0x0000939B
		// (set) Token: 0x060006B3 RID: 1715 RVA: 0x0000B1A3 File Offset: 0x000093A3
		public bool PasswordProtected { get; private set; }

		// Token: 0x17000261 RID: 609
		// (get) Token: 0x060006B4 RID: 1716 RVA: 0x0000B1AC File Offset: 0x000093AC
		// (set) Token: 0x060006B5 RID: 1717 RVA: 0x0000B1B4 File Offset: 0x000093B4
		public bool IsOfficial { get; private set; }

		// Token: 0x17000262 RID: 610
		// (get) Token: 0x060006B6 RID: 1718 RVA: 0x0000B1BD File Offset: 0x000093BD
		// (set) Token: 0x060006B7 RID: 1719 RVA: 0x0000B1C5 File Offset: 0x000093C5
		public bool ByOfficialProvider { get; private set; }

		// Token: 0x17000263 RID: 611
		// (get) Token: 0x060006B8 RID: 1720 RVA: 0x0000B1CE File Offset: 0x000093CE
		// (set) Token: 0x060006B9 RID: 1721 RVA: 0x0000B1D6 File Offset: 0x000093D6
		public bool CrossplayEnabled { get; private set; }

		// Token: 0x17000264 RID: 612
		// (get) Token: 0x060006BA RID: 1722 RVA: 0x0000B1DF File Offset: 0x000093DF
		// (set) Token: 0x060006BB RID: 1723 RVA: 0x0000B1E7 File Offset: 0x000093E7
		public int Permission { get; private set; }

		// Token: 0x17000265 RID: 613
		// (get) Token: 0x060006BC RID: 1724 RVA: 0x0000B1F0 File Offset: 0x000093F0
		// (set) Token: 0x060006BD RID: 1725 RVA: 0x0000B1F8 File Offset: 0x000093F8
		public PlayerId HostId { get; private set; }

		// Token: 0x17000266 RID: 614
		// (get) Token: 0x060006BE RID: 1726 RVA: 0x0000B201 File Offset: 0x00009401
		// (set) Token: 0x060006BF RID: 1727 RVA: 0x0000B209 File Offset: 0x00009409
		public string HostName { get; private set; }

		// Token: 0x17000267 RID: 615
		// (get) Token: 0x060006C0 RID: 1728 RVA: 0x0000B212 File Offset: 0x00009412
		// (set) Token: 0x060006C1 RID: 1729 RVA: 0x0000B21A File Offset: 0x0000941A
		public List<ModuleInfoModel> LoadedModules { get; private set; }

		// Token: 0x17000268 RID: 616
		// (get) Token: 0x060006C2 RID: 1730 RVA: 0x0000B223 File Offset: 0x00009423
		// (set) Token: 0x060006C3 RID: 1731 RVA: 0x0000B22B File Offset: 0x0000942B
		public bool AllowsOptionalModules { get; private set; }

		// Token: 0x060006C4 RID: 1732 RVA: 0x0000B234 File Offset: 0x00009434
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

		// Token: 0x060006C5 RID: 1733 RVA: 0x0000B2E8 File Offset: 0x000094E8
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
