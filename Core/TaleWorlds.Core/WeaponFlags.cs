using System;

namespace TaleWorlds.Core
{
	[Flags]
	public enum WeaponFlags : ulong
	{
		MeleeWeapon = 1UL,
		RangedWeapon = 2UL,
		WeaponMask = 3UL,
		FirearmAmmo = 4UL,
		NotUsableWithOneHand = 16UL,
		NotUsableWithTwoHand = 32UL,
		HandUsageMask = 48UL,
		WideGrip = 64UL,
		AttachAmmoToVisual = 128UL,
		Consumable = 256UL,
		HasHitPoints = 512UL,
		DataValueMask = 768UL,
		HasString = 1024UL,
		StringHeldByHand = 3072UL,
		UnloadWhenSheathed = 4096UL,
		AffectsArea = 8192UL,
		AffectsAreaBig = 16384UL,
		Burning = 32768UL,
		BonusAgainstShield = 65536UL,
		CanPenetrateShield = 131072UL,
		CantReloadOnHorseback = 262144UL,
		AutoReload = 524288UL,
		CanKillEvenIfBlunt = 1048576UL,
		TwoHandIdleOnMount = 2097152UL,
		NoBlood = 4194304UL,
		PenaltyWithShield = 8388608UL,
		CanDismount = 16777216UL,
		CanHook = 33554432UL,
		CanKnockDown = 67108864UL,
		CanCrushThrough = 134217728UL,
		CanBlockRanged = 268435456UL,
		MissileWithPhysics = 536870912UL,
		MultiplePenetration = 1073741824UL,
		LeavesTrail = 2147483648UL,
		UseHandAsThrowBase = 4294967296UL,
		AmmoBreaksOnBounceBack = 68719476736UL,
		AmmoCanBreakOnBounceBack = 137438953472UL,
		AmmoBreakOnBounceBackMask = 206158430208UL,
		AmmoSticksWhenShot = 274877906944UL
	}
}
