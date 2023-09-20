using System;

namespace TaleWorlds.MountAndBlade.Diamond
{
	// Token: 0x020000E8 RID: 232
	[Serializable]
	public class BattlePlayerStatsBattle : BattlePlayerStatsBase
	{
		// Token: 0x17000173 RID: 371
		// (get) Token: 0x060003A8 RID: 936 RVA: 0x000049CE File Offset: 0x00002BCE
		// (set) Token: 0x060003A9 RID: 937 RVA: 0x000049D6 File Offset: 0x00002BD6
		public int RoundsWon { get; set; }

		// Token: 0x17000174 RID: 372
		// (get) Token: 0x060003AA RID: 938 RVA: 0x000049DF File Offset: 0x00002BDF
		// (set) Token: 0x060003AB RID: 939 RVA: 0x000049E7 File Offset: 0x00002BE7
		public int RoundsLost { get; set; }
	}
}
