using System;

namespace TaleWorlds.Engine
{
	[Flags]
	public enum BodyFlags : uint
	{
		None = 0U,
		Disabled = 1U,
		NotDestructible = 2U,
		TwoSided = 4U,
		Dynamic = 8U,
		Moveable = 16U,
		DynamicConvexHull = 32U,
		Ladder = 64U,
		OnlyCollideWithRaycast = 128U,
		AILimiter = 256U,
		Barrier = 512U,
		Barrier3D = 1024U,
		HasSteps = 2048U,
		Ragdoll = 4096U,
		RagdollLimiter = 8192U,
		DestructibleDoor = 16384U,
		DroppedItem = 32768U,
		DoNotCollideWithRaycast = 65536U,
		DontTransferToPhysicsEngine = 131072U,
		DontCollideWithCamera = 262144U,
		ExcludePathSnap = 524288U,
		IsOpoed = 1048576U,
		AfterAddFlags = 1048576U,
		AgentOnly = 2097152U,
		MissileOnly = 4194304U,
		HasMaterial = 8388608U,
		BodyFlagFilter = 16777215U,
		CommonCollisionExcludeFlags = 6402441U,
		CameraCollisionRayCastExludeFlags = 6404041U,
		CommonCollisionExcludeFlagsForAgent = 4305289U,
		CommonCollisionExcludeFlagsForMissile = 2209673U,
		CommonCollisionExcludeFlagsForCombat = 2208137U,
		CommonCollisionExcludeFlagsForEditor = 2208137U,
		CommonFlagsThatDoNotBlocksRay = 16727871U,
		CommonFocusRayCastExcludeFlags = 79617U,
		BodyOwnerNone = 0U,
		BodyOwnerEntity = 16777216U,
		BodyOwnerTerrain = 33554432U,
		BodyOwnerFlora = 67108864U,
		BodyOwnerFilter = 251658240U,
		IgnoreSoundOcclusion = 268435456U
	}
}
