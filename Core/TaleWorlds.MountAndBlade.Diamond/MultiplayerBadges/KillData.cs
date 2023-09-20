using System;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade.Diamond.MultiplayerBadges
{
	// Token: 0x0200015F RID: 351
	public struct KillData
	{
		// Token: 0x17000314 RID: 788
		// (get) Token: 0x060008C2 RID: 2242 RVA: 0x0000F520 File Offset: 0x0000D720
		// (set) Token: 0x060008C3 RID: 2243 RVA: 0x0000F528 File Offset: 0x0000D728
		public PlayerId KillerId { get; set; }

		// Token: 0x17000315 RID: 789
		// (get) Token: 0x060008C4 RID: 2244 RVA: 0x0000F531 File Offset: 0x0000D731
		// (set) Token: 0x060008C5 RID: 2245 RVA: 0x0000F539 File Offset: 0x0000D739
		public PlayerId VictimId { get; set; }

		// Token: 0x17000316 RID: 790
		// (get) Token: 0x060008C6 RID: 2246 RVA: 0x0000F542 File Offset: 0x0000D742
		// (set) Token: 0x060008C7 RID: 2247 RVA: 0x0000F54A File Offset: 0x0000D74A
		public string KillerFaction { get; set; }

		// Token: 0x17000317 RID: 791
		// (get) Token: 0x060008C8 RID: 2248 RVA: 0x0000F553 File Offset: 0x0000D753
		// (set) Token: 0x060008C9 RID: 2249 RVA: 0x0000F55B File Offset: 0x0000D75B
		public string VictimFaction { get; set; }

		// Token: 0x17000318 RID: 792
		// (get) Token: 0x060008CA RID: 2250 RVA: 0x0000F564 File Offset: 0x0000D764
		// (set) Token: 0x060008CB RID: 2251 RVA: 0x0000F56C File Offset: 0x0000D76C
		public string KillerTroop { get; set; }

		// Token: 0x17000319 RID: 793
		// (get) Token: 0x060008CC RID: 2252 RVA: 0x0000F575 File Offset: 0x0000D775
		// (set) Token: 0x060008CD RID: 2253 RVA: 0x0000F57D File Offset: 0x0000D77D
		public string VictimTroop { get; set; }
	}
}
