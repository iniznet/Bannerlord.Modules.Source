using System;
using System.Collections.Generic;
using TaleWorlds.Library;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade.Diamond
{
	// Token: 0x02000111 RID: 273
	[Serializable]
	public class GameServerEntry
	{
		// Token: 0x170001ED RID: 493
		// (get) Token: 0x0600050F RID: 1295 RVA: 0x00007ACC File Offset: 0x00005CCC
		// (set) Token: 0x06000510 RID: 1296 RVA: 0x00007AD4 File Offset: 0x00005CD4
		public CustomBattleId Id { get; private set; }

		// Token: 0x170001EE RID: 494
		// (get) Token: 0x06000511 RID: 1297 RVA: 0x00007ADD File Offset: 0x00005CDD
		// (set) Token: 0x06000512 RID: 1298 RVA: 0x00007AE5 File Offset: 0x00005CE5
		public string Address { get; private set; }

		// Token: 0x170001EF RID: 495
		// (get) Token: 0x06000513 RID: 1299 RVA: 0x00007AEE File Offset: 0x00005CEE
		// (set) Token: 0x06000514 RID: 1300 RVA: 0x00007AF6 File Offset: 0x00005CF6
		public int Port { get; private set; }

		// Token: 0x170001F0 RID: 496
		// (get) Token: 0x06000515 RID: 1301 RVA: 0x00007AFF File Offset: 0x00005CFF
		// (set) Token: 0x06000516 RID: 1302 RVA: 0x00007B07 File Offset: 0x00005D07
		public string Region { get; private set; }

		// Token: 0x170001F1 RID: 497
		// (get) Token: 0x06000517 RID: 1303 RVA: 0x00007B10 File Offset: 0x00005D10
		// (set) Token: 0x06000518 RID: 1304 RVA: 0x00007B18 File Offset: 0x00005D18
		public int PlayerCount { get; private set; }

		// Token: 0x170001F2 RID: 498
		// (get) Token: 0x06000519 RID: 1305 RVA: 0x00007B21 File Offset: 0x00005D21
		// (set) Token: 0x0600051A RID: 1306 RVA: 0x00007B29 File Offset: 0x00005D29
		public int MaxPlayerCount { get; private set; }

		// Token: 0x170001F3 RID: 499
		// (get) Token: 0x0600051B RID: 1307 RVA: 0x00007B32 File Offset: 0x00005D32
		// (set) Token: 0x0600051C RID: 1308 RVA: 0x00007B3A File Offset: 0x00005D3A
		public string ServerName { get; private set; }

		// Token: 0x170001F4 RID: 500
		// (get) Token: 0x0600051D RID: 1309 RVA: 0x00007B43 File Offset: 0x00005D43
		// (set) Token: 0x0600051E RID: 1310 RVA: 0x00007B4B File Offset: 0x00005D4B
		public string GameModule { get; private set; }

		// Token: 0x170001F5 RID: 501
		// (get) Token: 0x0600051F RID: 1311 RVA: 0x00007B54 File Offset: 0x00005D54
		// (set) Token: 0x06000520 RID: 1312 RVA: 0x00007B5C File Offset: 0x00005D5C
		public string GameType { get; private set; }

		// Token: 0x170001F6 RID: 502
		// (get) Token: 0x06000521 RID: 1313 RVA: 0x00007B65 File Offset: 0x00005D65
		// (set) Token: 0x06000522 RID: 1314 RVA: 0x00007B6D File Offset: 0x00005D6D
		public string Map { get; private set; }

		// Token: 0x170001F7 RID: 503
		// (get) Token: 0x06000523 RID: 1315 RVA: 0x00007B76 File Offset: 0x00005D76
		// (set) Token: 0x06000524 RID: 1316 RVA: 0x00007B7E File Offset: 0x00005D7E
		public string UniqueMapId { get; private set; }

		// Token: 0x170001F8 RID: 504
		// (get) Token: 0x06000525 RID: 1317 RVA: 0x00007B87 File Offset: 0x00005D87
		// (set) Token: 0x06000526 RID: 1318 RVA: 0x00007B8F File Offset: 0x00005D8F
		public int Ping { get; private set; }

		// Token: 0x170001F9 RID: 505
		// (get) Token: 0x06000527 RID: 1319 RVA: 0x00007B98 File Offset: 0x00005D98
		// (set) Token: 0x06000528 RID: 1320 RVA: 0x00007BA0 File Offset: 0x00005DA0
		public bool IsOfficial { get; private set; }

		// Token: 0x170001FA RID: 506
		// (get) Token: 0x06000529 RID: 1321 RVA: 0x00007BA9 File Offset: 0x00005DA9
		// (set) Token: 0x0600052A RID: 1322 RVA: 0x00007BB1 File Offset: 0x00005DB1
		public bool ByOfficialProvider { get; private set; }

		// Token: 0x170001FB RID: 507
		// (get) Token: 0x0600052B RID: 1323 RVA: 0x00007BBA File Offset: 0x00005DBA
		// (set) Token: 0x0600052C RID: 1324 RVA: 0x00007BC2 File Offset: 0x00005DC2
		public bool PasswordProtected { get; private set; }

		// Token: 0x170001FC RID: 508
		// (get) Token: 0x0600052D RID: 1325 RVA: 0x00007BCB File Offset: 0x00005DCB
		// (set) Token: 0x0600052E RID: 1326 RVA: 0x00007BD3 File Offset: 0x00005DD3
		public int Permission { get; private set; }

		// Token: 0x170001FD RID: 509
		// (get) Token: 0x0600052F RID: 1327 RVA: 0x00007BDC File Offset: 0x00005DDC
		// (set) Token: 0x06000530 RID: 1328 RVA: 0x00007BE4 File Offset: 0x00005DE4
		public bool CrossplayEnabled { get; private set; }

		// Token: 0x170001FE RID: 510
		// (get) Token: 0x06000531 RID: 1329 RVA: 0x00007BED File Offset: 0x00005DED
		// (set) Token: 0x06000532 RID: 1330 RVA: 0x00007BF5 File Offset: 0x00005DF5
		public PlayerId HostId { get; private set; }

		// Token: 0x170001FF RID: 511
		// (get) Token: 0x06000533 RID: 1331 RVA: 0x00007BFE File Offset: 0x00005DFE
		// (set) Token: 0x06000534 RID: 1332 RVA: 0x00007C06 File Offset: 0x00005E06
		public string HostName { get; private set; }

		// Token: 0x17000200 RID: 512
		// (get) Token: 0x06000535 RID: 1333 RVA: 0x00007C0F File Offset: 0x00005E0F
		// (set) Token: 0x06000536 RID: 1334 RVA: 0x00007C17 File Offset: 0x00005E17
		public List<ModuleInfoModel> LoadedModules { get; private set; }

		// Token: 0x17000201 RID: 513
		// (get) Token: 0x06000537 RID: 1335 RVA: 0x00007C20 File Offset: 0x00005E20
		// (set) Token: 0x06000538 RID: 1336 RVA: 0x00007C28 File Offset: 0x00005E28
		public bool AllowsOptionalModules { get; private set; }

		// Token: 0x06000539 RID: 1337 RVA: 0x00007C34 File Offset: 0x00005E34
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

		// Token: 0x0600053A RID: 1338 RVA: 0x00007CE4 File Offset: 0x00005EE4
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
