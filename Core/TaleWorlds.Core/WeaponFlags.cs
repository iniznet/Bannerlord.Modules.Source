using System;

namespace TaleWorlds.Core
{
	// Token: 0x02000090 RID: 144
	[Flags]
	public enum WeaponFlags : ulong
	{
		// Token: 0x04000413 RID: 1043
		MeleeWeapon = 1UL,
		// Token: 0x04000414 RID: 1044
		RangedWeapon = 2UL,
		// Token: 0x04000415 RID: 1045
		WeaponMask = 3UL,
		// Token: 0x04000416 RID: 1046
		FirearmAmmo = 4UL,
		// Token: 0x04000417 RID: 1047
		NotUsableWithOneHand = 16UL,
		// Token: 0x04000418 RID: 1048
		NotUsableWithTwoHand = 32UL,
		// Token: 0x04000419 RID: 1049
		HandUsageMask = 48UL,
		// Token: 0x0400041A RID: 1050
		WideGrip = 64UL,
		// Token: 0x0400041B RID: 1051
		AttachAmmoToVisual = 128UL,
		// Token: 0x0400041C RID: 1052
		Consumable = 256UL,
		// Token: 0x0400041D RID: 1053
		HasHitPoints = 512UL,
		// Token: 0x0400041E RID: 1054
		DataValueMask = 768UL,
		// Token: 0x0400041F RID: 1055
		HasString = 1024UL,
		// Token: 0x04000420 RID: 1056
		StringHeldByHand = 3072UL,
		// Token: 0x04000421 RID: 1057
		UnloadWhenSheathed = 4096UL,
		// Token: 0x04000422 RID: 1058
		AffectsArea = 8192UL,
		// Token: 0x04000423 RID: 1059
		AffectsAreaBig = 16384UL,
		// Token: 0x04000424 RID: 1060
		Burning = 32768UL,
		// Token: 0x04000425 RID: 1061
		BonusAgainstShield = 65536UL,
		// Token: 0x04000426 RID: 1062
		CanPenetrateShield = 131072UL,
		// Token: 0x04000427 RID: 1063
		CantReloadOnHorseback = 262144UL,
		// Token: 0x04000428 RID: 1064
		AutoReload = 524288UL,
		// Token: 0x04000429 RID: 1065
		TwoHandIdleOnMount = 2097152UL,
		// Token: 0x0400042A RID: 1066
		NoBlood = 4194304UL,
		// Token: 0x0400042B RID: 1067
		PenaltyWithShield = 8388608UL,
		// Token: 0x0400042C RID: 1068
		CanDismount = 16777216UL,
		// Token: 0x0400042D RID: 1069
		CanHook = 33554432UL,
		// Token: 0x0400042E RID: 1070
		CanKnockDown = 67108864UL,
		// Token: 0x0400042F RID: 1071
		CanCrushThrough = 134217728UL,
		// Token: 0x04000430 RID: 1072
		CanBlockRanged = 268435456UL,
		// Token: 0x04000431 RID: 1073
		MissileWithPhysics = 536870912UL,
		// Token: 0x04000432 RID: 1074
		MultiplePenetration = 1073741824UL,
		// Token: 0x04000433 RID: 1075
		LeavesTrail = 2147483648UL,
		// Token: 0x04000434 RID: 1076
		UseHandAsThrowBase = 4294967296UL,
		// Token: 0x04000435 RID: 1077
		AmmoBreaksOnBounceBack = 68719476736UL,
		// Token: 0x04000436 RID: 1078
		AmmoCanBreakOnBounceBack = 137438953472UL,
		// Token: 0x04000437 RID: 1079
		AmmoBreakOnBounceBackMask = 206158430208UL,
		// Token: 0x04000438 RID: 1080
		AmmoSticksWhenShot = 274877906944UL
	}
}
