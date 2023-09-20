using System;
using TaleWorlds.DotNet;

namespace TaleWorlds.MountAndBlade
{
	[EngineStruct("Skeleton_model_bounds_rec_flags", false)]
	public enum SkeletonModelBoundsRecFlags : sbyte
	{
		None,
		UseSmallerRadiusMultWhileHoldingShield,
		Sweep,
		DoNotScaleAccordingToAgentScale = 4
	}
}
