using System;

namespace TaleWorlds.Engine
{
	[Flags]
	public enum EntityFlags : uint
	{
		ForceLodMask = 240U,
		ForceLodBits = 4U,
		AnimateWhenVisible = 256U,
		NoOcclusionCulling = 512U,
		IsHelper = 1024U,
		ComputePerComponentLod = 2048U,
		DoesNotAffectParentsLocalBb = 4096U,
		ForceAsStatic = 8192U,
		SendInitCallback = 16384U,
		PhysicsDisabled = 32768U,
		AlignToTerrain = 65536U,
		DontSaveToScene = 131072U,
		RecordToSceneReplay = 262144U,
		GroupMeshesAfterLod4 = 524288U,
		SmoothLodTransitions = 1048576U,
		DontCheckHandness = 2097152U,
		NotAffectedBySeason = 4194304U,
		DontTickChildren = 8388608U,
		WaitUntilReady = 16777216U,
		NonModifiableFromEditor = 33554432U,
		DeferredParallelFrameSetup = 67108864U,
		PerComponentVisibility = 134217728U,
		Ignore = 268435456U,
		DoNotTick = 536870912U,
		DoNotRenderToEnvmap = 1073741824U,
		AlignRotationToTerrain = 2147483648U
	}
}
