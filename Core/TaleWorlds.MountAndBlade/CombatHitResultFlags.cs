using System;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200017D RID: 381
	[Flags]
	public enum CombatHitResultFlags : byte
	{
		// Token: 0x040005E7 RID: 1511
		NormalHit = 0,
		// Token: 0x040005E8 RID: 1512
		HitWithStartOfTheAnimation = 1,
		// Token: 0x040005E9 RID: 1513
		HitWithArm = 2,
		// Token: 0x040005EA RID: 1514
		HitWithBackOfTheWeapon = 4
	}
}
