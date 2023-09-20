using System;

namespace TaleWorlds.MountAndBlade.Diamond
{
	// Token: 0x020000E9 RID: 233
	[Serializable]
	public class BattlePlayerStatsCaptain : BattlePlayerStatsBase
	{
		// Token: 0x17000175 RID: 373
		// (get) Token: 0x060003AD RID: 941 RVA: 0x000049F8 File Offset: 0x00002BF8
		// (set) Token: 0x060003AE RID: 942 RVA: 0x00004A00 File Offset: 0x00002C00
		public int CaptainsKilled { get; set; }

		// Token: 0x17000176 RID: 374
		// (get) Token: 0x060003AF RID: 943 RVA: 0x00004A09 File Offset: 0x00002C09
		// (set) Token: 0x060003B0 RID: 944 RVA: 0x00004A11 File Offset: 0x00002C11
		public int MVPs { get; set; }

		// Token: 0x17000177 RID: 375
		// (get) Token: 0x060003B1 RID: 945 RVA: 0x00004A1A File Offset: 0x00002C1A
		// (set) Token: 0x060003B2 RID: 946 RVA: 0x00004A22 File Offset: 0x00002C22
		public int Score { get; set; }
	}
}
