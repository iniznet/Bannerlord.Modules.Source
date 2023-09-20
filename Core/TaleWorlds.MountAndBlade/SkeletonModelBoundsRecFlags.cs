using System;

namespace TaleWorlds.MountAndBlade
{
	public enum SkeletonModelBoundsRecFlags : sbyte
	{
		None,
		UseSmallerRadiusMultWhileHoldingShield,
		Sweep,
		DoNotScaleAccordingToAgentScale = 4
	}
}
