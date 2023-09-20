using System;
using TaleWorlds.Diamond;

namespace Messages.FromCustomBattleServer.ToCustomBattleServerManager
{
	// Token: 0x02000060 RID: 96
	[MessageDescription("CustomBattleServer", "CustomBattleServerManager")]
	[Serializable]
	public class RegisterCustomGameMessage : Message
	{
		// Token: 0x1700008F RID: 143
		// (get) Token: 0x0600016B RID: 363 RVA: 0x00003039 File Offset: 0x00001239
		// (set) Token: 0x0600016C RID: 364 RVA: 0x00003041 File Offset: 0x00001241
		public int GameDefinitionId { get; private set; }

		// Token: 0x17000090 RID: 144
		// (get) Token: 0x0600016D RID: 365 RVA: 0x0000304A File Offset: 0x0000124A
		// (set) Token: 0x0600016E RID: 366 RVA: 0x00003052 File Offset: 0x00001252
		public string GameModule { get; private set; }

		// Token: 0x17000091 RID: 145
		// (get) Token: 0x0600016F RID: 367 RVA: 0x0000305B File Offset: 0x0000125B
		// (set) Token: 0x06000170 RID: 368 RVA: 0x00003063 File Offset: 0x00001263
		public string GameType { get; private set; }

		// Token: 0x17000092 RID: 146
		// (get) Token: 0x06000171 RID: 369 RVA: 0x0000306C File Offset: 0x0000126C
		// (set) Token: 0x06000172 RID: 370 RVA: 0x00003074 File Offset: 0x00001274
		public string ServerName { get; private set; }

		// Token: 0x17000093 RID: 147
		// (get) Token: 0x06000173 RID: 371 RVA: 0x0000307D File Offset: 0x0000127D
		// (set) Token: 0x06000174 RID: 372 RVA: 0x00003085 File Offset: 0x00001285
		public string ServerAddress { get; private set; }

		// Token: 0x17000094 RID: 148
		// (get) Token: 0x06000175 RID: 373 RVA: 0x0000308E File Offset: 0x0000128E
		// (set) Token: 0x06000176 RID: 374 RVA: 0x00003096 File Offset: 0x00001296
		public int MaxPlayerCount { get; private set; }

		// Token: 0x17000095 RID: 149
		// (get) Token: 0x06000177 RID: 375 RVA: 0x0000309F File Offset: 0x0000129F
		// (set) Token: 0x06000178 RID: 376 RVA: 0x000030A7 File Offset: 0x000012A7
		public string Map { get; private set; }

		// Token: 0x17000096 RID: 150
		// (get) Token: 0x06000179 RID: 377 RVA: 0x000030B0 File Offset: 0x000012B0
		// (set) Token: 0x0600017A RID: 378 RVA: 0x000030B8 File Offset: 0x000012B8
		public string UniqueMapId { get; private set; }

		// Token: 0x17000097 RID: 151
		// (get) Token: 0x0600017B RID: 379 RVA: 0x000030C1 File Offset: 0x000012C1
		// (set) Token: 0x0600017C RID: 380 RVA: 0x000030C9 File Offset: 0x000012C9
		public string GamePassword { get; private set; }

		// Token: 0x17000098 RID: 152
		// (get) Token: 0x0600017D RID: 381 RVA: 0x000030D2 File Offset: 0x000012D2
		// (set) Token: 0x0600017E RID: 382 RVA: 0x000030DA File Offset: 0x000012DA
		public string AdminPassword { get; private set; }

		// Token: 0x17000099 RID: 153
		// (get) Token: 0x0600017F RID: 383 RVA: 0x000030E3 File Offset: 0x000012E3
		// (set) Token: 0x06000180 RID: 384 RVA: 0x000030EB File Offset: 0x000012EB
		public int Port { get; private set; }

		// Token: 0x1700009A RID: 154
		// (get) Token: 0x06000181 RID: 385 RVA: 0x000030F4 File Offset: 0x000012F4
		// (set) Token: 0x06000182 RID: 386 RVA: 0x000030FC File Offset: 0x000012FC
		public string Region { get; private set; }

		// Token: 0x1700009B RID: 155
		// (get) Token: 0x06000183 RID: 387 RVA: 0x00003105 File Offset: 0x00001305
		// (set) Token: 0x06000184 RID: 388 RVA: 0x0000310D File Offset: 0x0000130D
		public int Permission { get; private set; }

		// Token: 0x1700009C RID: 156
		// (get) Token: 0x06000185 RID: 389 RVA: 0x00003116 File Offset: 0x00001316
		// (set) Token: 0x06000186 RID: 390 RVA: 0x0000311E File Offset: 0x0000131E
		public bool IsOverridingIP { get; private set; }

		// Token: 0x1700009D RID: 157
		// (get) Token: 0x06000187 RID: 391 RVA: 0x00003127 File Offset: 0x00001327
		// (set) Token: 0x06000188 RID: 392 RVA: 0x0000312F File Offset: 0x0000132F
		public bool CrossplayEnabled { get; private set; }

		// Token: 0x06000189 RID: 393 RVA: 0x00003138 File Offset: 0x00001338
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
