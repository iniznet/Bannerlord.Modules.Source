using System;

namespace TaleWorlds.MountAndBlade.Diamond
{
	// Token: 0x020000EC RID: 236
	[Serializable]
	public class BattlePlayerStatsSkirmish : BattlePlayerStatsBase
	{
		// Token: 0x17000181 RID: 385
		// (get) Token: 0x060003C8 RID: 968 RVA: 0x00004ADC File Offset: 0x00002CDC
		// (set) Token: 0x060003C9 RID: 969 RVA: 0x00004AE4 File Offset: 0x00002CE4
		public int MVPs { get; set; }

		// Token: 0x17000182 RID: 386
		// (get) Token: 0x060003CA RID: 970 RVA: 0x00004AED File Offset: 0x00002CED
		// (set) Token: 0x060003CB RID: 971 RVA: 0x00004AF5 File Offset: 0x00002CF5
		public int Score { get; set; }
	}
}
