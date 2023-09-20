using System;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200017C RID: 380
	public enum MeleeCollisionReaction
	{
		// Token: 0x040005E0 RID: 1504
		Invalid = -1,
		// Token: 0x040005E1 RID: 1505
		SlicedThrough,
		// Token: 0x040005E2 RID: 1506
		ContinueChecking,
		// Token: 0x040005E3 RID: 1507
		Stuck,
		// Token: 0x040005E4 RID: 1508
		Bounced,
		// Token: 0x040005E5 RID: 1509
		Staggered
	}
}
