using System;

namespace TaleWorlds.Engine
{
	// Token: 0x02000061 RID: 97
	[Flags]
	public enum VisibilityMaskFlags : uint
	{
		// Token: 0x0400010E RID: 270
		Final = 1U,
		// Token: 0x0400010F RID: 271
		ShadowStatic = 16U,
		// Token: 0x04000110 RID: 272
		ShadowDynamic = 32U,
		// Token: 0x04000111 RID: 273
		Contour = 64U,
		// Token: 0x04000112 RID: 274
		EditModeAtmosphere = 268435456U,
		// Token: 0x04000113 RID: 275
		EditModeLight = 536870912U,
		// Token: 0x04000114 RID: 276
		EditModeParticleSystem = 1073741824U,
		// Token: 0x04000115 RID: 277
		EditModeHelpers = 2147483648U,
		// Token: 0x04000116 RID: 278
		EditModeTerrain = 16777216U,
		// Token: 0x04000117 RID: 279
		EditModeGameEntity = 33554432U,
		// Token: 0x04000118 RID: 280
		EditModeFloraEntity = 67108864U,
		// Token: 0x04000119 RID: 281
		EditModeLayerFlora = 134217728U,
		// Token: 0x0400011A RID: 282
		EditModeShadows = 1048576U,
		// Token: 0x0400011B RID: 283
		EditModeBorders = 2097152U,
		// Token: 0x0400011C RID: 284
		EditModeEditingEntity = 4194304U,
		// Token: 0x0400011D RID: 285
		EditModeAnimations = 8388608U,
		// Token: 0x0400011E RID: 286
		EditModeAny = 4293918720U,
		// Token: 0x0400011F RID: 287
		Default = 1U,
		// Token: 0x04000120 RID: 288
		DefaultStatic = 49U,
		// Token: 0x04000121 RID: 289
		DefaultDynamic = 33U,
		// Token: 0x04000122 RID: 290
		DefaultStaticWithoutDynamic = 17U
	}
}
