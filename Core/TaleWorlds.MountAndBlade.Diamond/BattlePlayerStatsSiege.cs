using System;

namespace TaleWorlds.MountAndBlade.Diamond
{
	// Token: 0x020000EB RID: 235
	[Serializable]
	public class BattlePlayerStatsSiege : BattlePlayerStatsBase
	{
		// Token: 0x1700017C RID: 380
		// (get) Token: 0x060003BD RID: 957 RVA: 0x00004A7F File Offset: 0x00002C7F
		// (set) Token: 0x060003BE RID: 958 RVA: 0x00004A87 File Offset: 0x00002C87
		public int WallsBreached { get; set; }

		// Token: 0x1700017D RID: 381
		// (get) Token: 0x060003BF RID: 959 RVA: 0x00004A90 File Offset: 0x00002C90
		// (set) Token: 0x060003C0 RID: 960 RVA: 0x00004A98 File Offset: 0x00002C98
		public int SiegeEngineKills { get; set; }

		// Token: 0x1700017E RID: 382
		// (get) Token: 0x060003C1 RID: 961 RVA: 0x00004AA1 File Offset: 0x00002CA1
		// (set) Token: 0x060003C2 RID: 962 RVA: 0x00004AA9 File Offset: 0x00002CA9
		public int SiegeEnginesDestroyed { get; set; }

		// Token: 0x1700017F RID: 383
		// (get) Token: 0x060003C3 RID: 963 RVA: 0x00004AB2 File Offset: 0x00002CB2
		// (set) Token: 0x060003C4 RID: 964 RVA: 0x00004ABA File Offset: 0x00002CBA
		public int ObjectiveGoldGained { get; set; }

		// Token: 0x17000180 RID: 384
		// (get) Token: 0x060003C5 RID: 965 RVA: 0x00004AC3 File Offset: 0x00002CC3
		// (set) Token: 0x060003C6 RID: 966 RVA: 0x00004ACB File Offset: 0x00002CCB
		public int Score { get; set; }
	}
}
