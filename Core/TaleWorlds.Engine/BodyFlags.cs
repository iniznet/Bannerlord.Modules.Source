using System;

namespace TaleWorlds.Engine
{
	// Token: 0x02000046 RID: 70
	[Flags]
	public enum BodyFlags : uint
	{
		// Token: 0x04000071 RID: 113
		None = 0U,
		// Token: 0x04000072 RID: 114
		Disabled = 1U,
		// Token: 0x04000073 RID: 115
		NotDestructible = 2U,
		// Token: 0x04000074 RID: 116
		TwoSided = 4U,
		// Token: 0x04000075 RID: 117
		Dynamic = 8U,
		// Token: 0x04000076 RID: 118
		Moveable = 16U,
		// Token: 0x04000077 RID: 119
		DynamicConvexHull = 32U,
		// Token: 0x04000078 RID: 120
		Ladder = 64U,
		// Token: 0x04000079 RID: 121
		OnlyCollideWithRaycast = 128U,
		// Token: 0x0400007A RID: 122
		AILimiter = 256U,
		// Token: 0x0400007B RID: 123
		Barrier = 512U,
		// Token: 0x0400007C RID: 124
		Barrier3D = 1024U,
		// Token: 0x0400007D RID: 125
		HasSteps = 2048U,
		// Token: 0x0400007E RID: 126
		Ragdoll = 4096U,
		// Token: 0x0400007F RID: 127
		RagdollLimiter = 8192U,
		// Token: 0x04000080 RID: 128
		DestructibleDoor = 16384U,
		// Token: 0x04000081 RID: 129
		DroppedItem = 32768U,
		// Token: 0x04000082 RID: 130
		DoNotCollideWithRaycast = 65536U,
		// Token: 0x04000083 RID: 131
		DontTransferToPhysicsEngine = 131072U,
		// Token: 0x04000084 RID: 132
		DontCollideWithCamera = 262144U,
		// Token: 0x04000085 RID: 133
		ExcludePathSnap = 524288U,
		// Token: 0x04000086 RID: 134
		IsOpoed = 1048576U,
		// Token: 0x04000087 RID: 135
		AfterAddFlags = 1048576U,
		// Token: 0x04000088 RID: 136
		AgentOnly = 2097152U,
		// Token: 0x04000089 RID: 137
		MissileOnly = 4194304U,
		// Token: 0x0400008A RID: 138
		HasMaterial = 8388608U,
		// Token: 0x0400008B RID: 139
		BodyFlagFilter = 16777215U,
		// Token: 0x0400008C RID: 140
		CommonCollisionExcludeFlags = 6402441U,
		// Token: 0x0400008D RID: 141
		CameraCollisionRayCastExludeFlags = 6404041U,
		// Token: 0x0400008E RID: 142
		CommonCollisionExcludeFlagsForAgent = 4305289U,
		// Token: 0x0400008F RID: 143
		CommonCollisionExcludeFlagsForMissile = 2209673U,
		// Token: 0x04000090 RID: 144
		CommonCollisionExcludeFlagsForCombat = 2208137U,
		// Token: 0x04000091 RID: 145
		CommonCollisionExcludeFlagsForEditor = 2208137U,
		// Token: 0x04000092 RID: 146
		CommonFlagsThatDoNotBlocksRay = 16727871U,
		// Token: 0x04000093 RID: 147
		CommonFocusRayCastExcludeFlags = 79617U,
		// Token: 0x04000094 RID: 148
		BodyOwnerNone = 0U,
		// Token: 0x04000095 RID: 149
		BodyOwnerEntity = 16777216U,
		// Token: 0x04000096 RID: 150
		BodyOwnerTerrain = 33554432U,
		// Token: 0x04000097 RID: 151
		BodyOwnerFlora = 67108864U,
		// Token: 0x04000098 RID: 152
		BodyOwnerFilter = 251658240U,
		// Token: 0x04000099 RID: 153
		IgnoreSoundOcclusion = 268435456U
	}
}
