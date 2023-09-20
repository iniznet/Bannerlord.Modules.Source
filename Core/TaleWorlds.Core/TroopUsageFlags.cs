using System;

namespace TaleWorlds.Core
{
	[Flags]
	public enum TroopUsageFlags : ushort
	{
		None = 0,
		OnFoot = 1,
		Mounted = 2,
		Melee = 4,
		Ranged = 8,
		OneHandedUser = 16,
		ShieldUser = 32,
		TwoHandedUser = 64,
		PolearmUser = 128,
		BowUser = 256,
		ThrownUser = 512,
		CrossbowUser = 1024,
		Undefined = 65535
	}
}
