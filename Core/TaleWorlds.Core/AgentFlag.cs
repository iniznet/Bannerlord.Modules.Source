using System;

namespace TaleWorlds.Core
{
	[Flags]
	public enum AgentFlag : uint
	{
		None = 0U,
		Mountable = 1U,
		CanJump = 2U,
		CanRear = 4U,
		CanAttack = 8U,
		CanDefend = 16U,
		RunsAwayWhenHit = 32U,
		CanCharge = 64U,
		CanBeCharged = 128U,
		CanClimbLadders = 256U,
		CanBeInGroup = 512U,
		CanSprint = 1024U,
		IsHumanoid = 2048U,
		CanGetScared = 4096U,
		CanRide = 8192U,
		CanWieldWeapon = 16384U,
		CanCrouch = 32768U,
		CanGetAlarmed = 65536U,
		CanWander = 131072U,
		CanKick = 524288U,
		CanRetreat = 1048576U,
		MoveAsHerd = 2097152U,
		MoveForwardOnly = 4194304U,
		IsUnique = 8388608U,
		CanUseAllBowsMounted = 16777216U,
		CanReloadAllXBowsMounted = 33554432U,
		CanDeflectArrowsWith2HSword = 67108864U
	}
}
