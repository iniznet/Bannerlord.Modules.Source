using System;

namespace TaleWorlds.MountAndBlade.Diamond
{
	// Token: 0x02000146 RID: 326
	[Serializable]
	public class PremadeGameList
	{
		// Token: 0x170002DB RID: 731
		// (get) Token: 0x0600082B RID: 2091 RVA: 0x0000D168 File Offset: 0x0000B368
		// (set) Token: 0x0600082C RID: 2092 RVA: 0x0000D16F File Offset: 0x0000B36F
		public static PremadeGameList Empty { get; private set; } = new PremadeGameList(new PremadeGameEntry[0]);

		// Token: 0x170002DC RID: 732
		// (get) Token: 0x0600082E RID: 2094 RVA: 0x0000D189 File Offset: 0x0000B389
		// (set) Token: 0x0600082F RID: 2095 RVA: 0x0000D191 File Offset: 0x0000B391
		public PremadeGameEntry[] PremadeGameEntries { get; private set; }

		// Token: 0x06000830 RID: 2096 RVA: 0x0000D19A File Offset: 0x0000B39A
		public PremadeGameList(PremadeGameEntry[] entries)
		{
			this.PremadeGameEntries = entries;
		}
	}
}
