using System;

namespace TaleWorlds.Core
{
	[Flags]
	public enum TroopClassFlag : uint
	{
		None = 0U,
		OneHandedUser = 1U,
		ShieldUser = 2U,
		TwoHandedUser = 4U,
		PoleArmUser = 8U,
		BowUser = 16U,
		ThrownUser = 32U,
		CrossbowUser = 64U,
		Infantry = 256U,
		Cavalry = 512U,
		Ranged = 4096U,
		All = 4294967295U
	}
}
