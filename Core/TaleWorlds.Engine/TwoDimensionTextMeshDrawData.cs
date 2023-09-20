using System;
using TaleWorlds.DotNet;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
	// Token: 0x02000091 RID: 145
	[EngineStruct("rglTwo_dimension_text_mesh_draw_data")]
	public struct TwoDimensionTextMeshDrawData
	{
		// Token: 0x040001C8 RID: 456
		public float DrawX;

		// Token: 0x040001C9 RID: 457
		public float DrawY;

		// Token: 0x040001CA RID: 458
		public float ScreenWidth;

		// Token: 0x040001CB RID: 459
		public float ScreenHeight;

		// Token: 0x040001CC RID: 460
		public uint Color;

		// Token: 0x040001CD RID: 461
		public float ScaleFactor;

		// Token: 0x040001CE RID: 462
		public float SmoothingConstant;

		// Token: 0x040001CF RID: 463
		public float ColorFactor;

		// Token: 0x040001D0 RID: 464
		public float AlphaFactor;

		// Token: 0x040001D1 RID: 465
		public float HueFactor;

		// Token: 0x040001D2 RID: 466
		public float SaturationFactor;

		// Token: 0x040001D3 RID: 467
		public float ValueFactor;

		// Token: 0x040001D4 RID: 468
		public Vec2 ClipRectPosition;

		// Token: 0x040001D5 RID: 469
		public Vec2 ClipRectSize;

		// Token: 0x040001D6 RID: 470
		public uint GlowColor;

		// Token: 0x040001D7 RID: 471
		public Vec3 OutlineColor;

		// Token: 0x040001D8 RID: 472
		public float OutlineAmount;

		// Token: 0x040001D9 RID: 473
		public float GlowRadius;

		// Token: 0x040001DA RID: 474
		public float Blur;

		// Token: 0x040001DB RID: 475
		public float ShadowOffset;

		// Token: 0x040001DC RID: 476
		public float ShadowAngle;

		// Token: 0x040001DD RID: 477
		public int Layer;

		// Token: 0x040001DE RID: 478
		public ulong HashCode1;

		// Token: 0x040001DF RID: 479
		public ulong HashCode2;
	}
}
