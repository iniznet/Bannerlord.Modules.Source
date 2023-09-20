using System;

namespace TaleWorlds.Engine
{
	// Token: 0x02000044 RID: 68
	[Flags]
	public enum EntityFlags : uint
	{
		// Token: 0x04000050 RID: 80
		ForceLodMask = 240U,
		// Token: 0x04000051 RID: 81
		ForceLodBits = 4U,
		// Token: 0x04000052 RID: 82
		AnimateWhenVisible = 256U,
		// Token: 0x04000053 RID: 83
		NoOcclusionCulling = 512U,
		// Token: 0x04000054 RID: 84
		IsHelper = 1024U,
		// Token: 0x04000055 RID: 85
		ComputePerComponentLod = 2048U,
		// Token: 0x04000056 RID: 86
		DoesNotAffectParentsLocalBb = 4096U,
		// Token: 0x04000057 RID: 87
		ForceAsStatic = 8192U,
		// Token: 0x04000058 RID: 88
		SendInitCallback = 16384U,
		// Token: 0x04000059 RID: 89
		PhysicsDisabled = 32768U,
		// Token: 0x0400005A RID: 90
		AlignToTerrain = 65536U,
		// Token: 0x0400005B RID: 91
		DontSaveToScene = 131072U,
		// Token: 0x0400005C RID: 92
		RecordToSceneReplay = 262144U,
		// Token: 0x0400005D RID: 93
		GroupMeshesAfterLod4 = 524288U,
		// Token: 0x0400005E RID: 94
		SmoothLodTransitions = 1048576U,
		// Token: 0x0400005F RID: 95
		DontCheckHandness = 2097152U,
		// Token: 0x04000060 RID: 96
		NotAffectedBySeason = 4194304U,
		// Token: 0x04000061 RID: 97
		DontTickChildren = 8388608U,
		// Token: 0x04000062 RID: 98
		WaitUntilReady = 16777216U,
		// Token: 0x04000063 RID: 99
		NonModifiableFromEditor = 33554432U,
		// Token: 0x04000064 RID: 100
		DeferredParallelFrameSetup = 67108864U,
		// Token: 0x04000065 RID: 101
		PerComponentVisibility = 134217728U,
		// Token: 0x04000066 RID: 102
		Ignore = 268435456U,
		// Token: 0x04000067 RID: 103
		DoNotTick = 536870912U,
		// Token: 0x04000068 RID: 104
		DoNotRenderToEnvmap = 1073741824U,
		// Token: 0x04000069 RID: 105
		AlignRotationToTerrain = 2147483648U
	}
}
