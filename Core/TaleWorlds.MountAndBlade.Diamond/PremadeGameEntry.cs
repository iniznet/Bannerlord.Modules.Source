using System;

namespace TaleWorlds.MountAndBlade.Diamond
{
	// Token: 0x02000147 RID: 327
	[Serializable]
	public class PremadeGameEntry
	{
		// Token: 0x170002DD RID: 733
		// (get) Token: 0x06000831 RID: 2097 RVA: 0x0000D1A9 File Offset: 0x0000B3A9
		// (set) Token: 0x06000832 RID: 2098 RVA: 0x0000D1B1 File Offset: 0x0000B3B1
		public Guid Id { get; private set; }

		// Token: 0x170002DE RID: 734
		// (get) Token: 0x06000833 RID: 2099 RVA: 0x0000D1BA File Offset: 0x0000B3BA
		// (set) Token: 0x06000834 RID: 2100 RVA: 0x0000D1C2 File Offset: 0x0000B3C2
		public string Name { get; private set; }

		// Token: 0x170002DF RID: 735
		// (get) Token: 0x06000835 RID: 2101 RVA: 0x0000D1CB File Offset: 0x0000B3CB
		// (set) Token: 0x06000836 RID: 2102 RVA: 0x0000D1D3 File Offset: 0x0000B3D3
		public string Region { get; private set; }

		// Token: 0x170002E0 RID: 736
		// (get) Token: 0x06000837 RID: 2103 RVA: 0x0000D1DC File Offset: 0x0000B3DC
		// (set) Token: 0x06000838 RID: 2104 RVA: 0x0000D1E4 File Offset: 0x0000B3E4
		public string GameType { get; private set; }

		// Token: 0x170002E1 RID: 737
		// (get) Token: 0x06000839 RID: 2105 RVA: 0x0000D1ED File Offset: 0x0000B3ED
		// (set) Token: 0x0600083A RID: 2106 RVA: 0x0000D1F5 File Offset: 0x0000B3F5
		public string MapName { get; private set; }

		// Token: 0x170002E2 RID: 738
		// (get) Token: 0x0600083B RID: 2107 RVA: 0x0000D1FE File Offset: 0x0000B3FE
		// (set) Token: 0x0600083C RID: 2108 RVA: 0x0000D206 File Offset: 0x0000B406
		public string FactionA { get; private set; }

		// Token: 0x170002E3 RID: 739
		// (get) Token: 0x0600083D RID: 2109 RVA: 0x0000D20F File Offset: 0x0000B40F
		// (set) Token: 0x0600083E RID: 2110 RVA: 0x0000D217 File Offset: 0x0000B417
		public string FactionB { get; private set; }

		// Token: 0x170002E4 RID: 740
		// (get) Token: 0x0600083F RID: 2111 RVA: 0x0000D220 File Offset: 0x0000B420
		// (set) Token: 0x06000840 RID: 2112 RVA: 0x0000D228 File Offset: 0x0000B428
		public bool IsPasswordProtected { get; private set; }

		// Token: 0x170002E5 RID: 741
		// (get) Token: 0x06000841 RID: 2113 RVA: 0x0000D231 File Offset: 0x0000B431
		// (set) Token: 0x06000842 RID: 2114 RVA: 0x0000D239 File Offset: 0x0000B439
		public PremadeGameType PremadeGameType { get; private set; }

		// Token: 0x06000843 RID: 2115 RVA: 0x0000D244 File Offset: 0x0000B444
		public PremadeGameEntry(Guid id, string name, string region, string gameType, string mapName, string factionA, string factionB, bool isPasswordProtected, PremadeGameType premadeGameType)
		{
			this.Id = id;
			this.Name = name;
			this.Region = region;
			this.GameType = gameType;
			this.MapName = mapName;
			this.FactionA = factionA;
			this.FactionB = factionB;
			this.IsPasswordProtected = isPasswordProtected;
			this.PremadeGameType = premadeGameType;
		}
	}
}
