using System;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade.Diamond
{
	// Token: 0x020000E6 RID: 230
	[Serializable]
	public class BattlePlayerEntry
	{
		// Token: 0x17000165 RID: 357
		// (get) Token: 0x0600038A RID: 906 RVA: 0x000048D0 File Offset: 0x00002AD0
		// (set) Token: 0x0600038B RID: 907 RVA: 0x000048D8 File Offset: 0x00002AD8
		public PlayerId PlayerId { get; set; }

		// Token: 0x17000166 RID: 358
		// (get) Token: 0x0600038C RID: 908 RVA: 0x000048E1 File Offset: 0x00002AE1
		// (set) Token: 0x0600038D RID: 909 RVA: 0x000048E9 File Offset: 0x00002AE9
		public int TeamNo { get; set; }

		// Token: 0x17000167 RID: 359
		// (get) Token: 0x0600038E RID: 910 RVA: 0x000048F2 File Offset: 0x00002AF2
		// (set) Token: 0x0600038F RID: 911 RVA: 0x000048FA File Offset: 0x00002AFA
		public Guid Party { get; set; }

		// Token: 0x17000168 RID: 360
		// (get) Token: 0x06000390 RID: 912 RVA: 0x00004903 File Offset: 0x00002B03
		// (set) Token: 0x06000391 RID: 913 RVA: 0x0000490B File Offset: 0x00002B0B
		public BattlePlayerStatsBase PlayerStats { get; set; }

		// Token: 0x17000169 RID: 361
		// (get) Token: 0x06000392 RID: 914 RVA: 0x00004914 File Offset: 0x00002B14
		// (set) Token: 0x06000393 RID: 915 RVA: 0x0000491C File Offset: 0x00002B1C
		public int PlayTime { get; set; }

		// Token: 0x1700016A RID: 362
		// (get) Token: 0x06000394 RID: 916 RVA: 0x00004925 File Offset: 0x00002B25
		// (set) Token: 0x06000395 RID: 917 RVA: 0x0000492D File Offset: 0x00002B2D
		public DateTime LastJoinTime { get; set; }

		// Token: 0x1700016B RID: 363
		// (get) Token: 0x06000396 RID: 918 RVA: 0x00004936 File Offset: 0x00002B36
		// (set) Token: 0x06000397 RID: 919 RVA: 0x0000493E File Offset: 0x00002B3E
		public bool Disconnected { get; set; }

		// Token: 0x1700016C RID: 364
		// (get) Token: 0x06000398 RID: 920 RVA: 0x00004947 File Offset: 0x00002B47
		// (set) Token: 0x06000399 RID: 921 RVA: 0x0000494F File Offset: 0x00002B4F
		public string GameType { get; set; }

		// Token: 0x1700016D RID: 365
		// (get) Token: 0x0600039A RID: 922 RVA: 0x00004958 File Offset: 0x00002B58
		// (set) Token: 0x0600039B RID: 923 RVA: 0x00004960 File Offset: 0x00002B60
		public bool Won { get; set; }

		// Token: 0x1700016E RID: 366
		// (get) Token: 0x0600039C RID: 924 RVA: 0x00004969 File Offset: 0x00002B69
		// (set) Token: 0x0600039D RID: 925 RVA: 0x00004971 File Offset: 0x00002B71
		public BattleJoinType BattleJoinType { get; set; }
	}
}
