using System;

namespace TaleWorlds.MountAndBlade.Diamond
{
	// Token: 0x020000E7 RID: 231
	[Serializable]
	public class BattlePlayerStatsBase
	{
		// Token: 0x1700016F RID: 367
		// (get) Token: 0x0600039F RID: 927 RVA: 0x00004982 File Offset: 0x00002B82
		// (set) Token: 0x060003A0 RID: 928 RVA: 0x0000498A File Offset: 0x00002B8A
		public int Kills { get; set; }

		// Token: 0x17000170 RID: 368
		// (get) Token: 0x060003A1 RID: 929 RVA: 0x00004993 File Offset: 0x00002B93
		// (set) Token: 0x060003A2 RID: 930 RVA: 0x0000499B File Offset: 0x00002B9B
		public int Assists { get; set; }

		// Token: 0x17000171 RID: 369
		// (get) Token: 0x060003A3 RID: 931 RVA: 0x000049A4 File Offset: 0x00002BA4
		// (set) Token: 0x060003A4 RID: 932 RVA: 0x000049AC File Offset: 0x00002BAC
		public int Deaths { get; set; }

		// Token: 0x17000172 RID: 370
		// (get) Token: 0x060003A5 RID: 933 RVA: 0x000049B5 File Offset: 0x00002BB5
		// (set) Token: 0x060003A6 RID: 934 RVA: 0x000049BD File Offset: 0x00002BBD
		public int PlayTime { get; set; }
	}
}
