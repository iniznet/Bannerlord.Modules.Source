using System;

namespace TaleWorlds.Engine
{
	[Flags]
	public enum VisibilityMaskFlags : uint
	{
		Final = 1U,
		ShadowStatic = 16U,
		ShadowDynamic = 32U,
		Contour = 64U,
		EditModeAtmosphere = 268435456U,
		EditModeLight = 536870912U,
		EditModeParticleSystem = 1073741824U,
		EditModeHelpers = 2147483648U,
		EditModeTerrain = 16777216U,
		EditModeGameEntity = 33554432U,
		EditModeFloraEntity = 67108864U,
		EditModeLayerFlora = 134217728U,
		EditModeShadows = 1048576U,
		EditModeBorders = 2097152U,
		EditModeEditingEntity = 4194304U,
		EditModeAnimations = 8388608U,
		EditModeAny = 4293918720U,
		Default = 1U,
		DefaultStatic = 49U,
		DefaultDynamic = 33U,
		DefaultStaticWithoutDynamic = 17U
	}
}
