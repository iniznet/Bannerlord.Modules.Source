using System;

namespace TaleWorlds.MountAndBlade.Diamond
{
	// Token: 0x02000104 RID: 260
	[Serializable]
	public class ClanStats
	{
		// Token: 0x170001D1 RID: 465
		// (get) Token: 0x060004AF RID: 1199 RVA: 0x00006A3A File Offset: 0x00004C3A
		// (set) Token: 0x060004B0 RID: 1200 RVA: 0x00006A42 File Offset: 0x00004C42
		public int WinCount { get; private set; }

		// Token: 0x170001D2 RID: 466
		// (get) Token: 0x060004B1 RID: 1201 RVA: 0x00006A4B File Offset: 0x00004C4B
		// (set) Token: 0x060004B2 RID: 1202 RVA: 0x00006A53 File Offset: 0x00004C53
		public int LossCount { get; private set; }

		// Token: 0x060004B3 RID: 1203 RVA: 0x00006A5C File Offset: 0x00004C5C
		public ClanStats(int winCount, int lossCount)
		{
			this.WinCount = winCount;
			this.LossCount = lossCount;
		}
	}
}
