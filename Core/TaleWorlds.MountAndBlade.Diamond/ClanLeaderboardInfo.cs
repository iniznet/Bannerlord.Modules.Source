using System;

namespace TaleWorlds.MountAndBlade.Diamond
{
	// Token: 0x02000100 RID: 256
	[Serializable]
	public class ClanLeaderboardInfo
	{
		// Token: 0x170001C5 RID: 453
		// (get) Token: 0x06000493 RID: 1171 RVA: 0x000068F5 File Offset: 0x00004AF5
		// (set) Token: 0x06000494 RID: 1172 RVA: 0x000068FC File Offset: 0x00004AFC
		public static ClanLeaderboardInfo Empty { get; private set; } = new ClanLeaderboardInfo(new ClanLeaderboardEntry[0]);

		// Token: 0x170001C6 RID: 454
		// (get) Token: 0x06000496 RID: 1174 RVA: 0x00006916 File Offset: 0x00004B16
		// (set) Token: 0x06000497 RID: 1175 RVA: 0x0000691E File Offset: 0x00004B1E
		public ClanLeaderboardEntry[] ClanEntries { get; private set; }

		// Token: 0x06000498 RID: 1176 RVA: 0x00006927 File Offset: 0x00004B27
		public ClanLeaderboardInfo(ClanLeaderboardEntry[] entries)
		{
			this.ClanEntries = entries;
		}
	}
}
