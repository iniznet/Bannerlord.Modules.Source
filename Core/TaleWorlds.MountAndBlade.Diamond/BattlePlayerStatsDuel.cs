using System;

namespace TaleWorlds.MountAndBlade.Diamond
{
	// Token: 0x020000EA RID: 234
	[Serializable]
	public class BattlePlayerStatsDuel : BattlePlayerStatsBase
	{
		// Token: 0x17000178 RID: 376
		// (get) Token: 0x060003B4 RID: 948 RVA: 0x00004A33 File Offset: 0x00002C33
		// (set) Token: 0x060003B5 RID: 949 RVA: 0x00004A3B File Offset: 0x00002C3B
		public int DuelsWon { get; set; }

		// Token: 0x17000179 RID: 377
		// (get) Token: 0x060003B6 RID: 950 RVA: 0x00004A44 File Offset: 0x00002C44
		// (set) Token: 0x060003B7 RID: 951 RVA: 0x00004A4C File Offset: 0x00002C4C
		public int InfantryWins { get; set; }

		// Token: 0x1700017A RID: 378
		// (get) Token: 0x060003B8 RID: 952 RVA: 0x00004A55 File Offset: 0x00002C55
		// (set) Token: 0x060003B9 RID: 953 RVA: 0x00004A5D File Offset: 0x00002C5D
		public int ArcherWins { get; set; }

		// Token: 0x1700017B RID: 379
		// (get) Token: 0x060003BA RID: 954 RVA: 0x00004A66 File Offset: 0x00002C66
		// (set) Token: 0x060003BB RID: 955 RVA: 0x00004A6E File Offset: 0x00002C6E
		public int CavalryWins { get; set; }
	}
}
