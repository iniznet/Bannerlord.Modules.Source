using System;

namespace TaleWorlds.MountAndBlade
{
	[Flags]
	public enum CombatHitResultFlags : byte
	{
		NormalHit = 0,
		HitWithStartOfTheAnimation = 1,
		HitWithArm = 2,
		HitWithBackOfTheWeapon = 4
	}
}
