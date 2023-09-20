using System;

namespace TaleWorlds.TwoDimension.Standalone.Native.OpenGL
{
	// Token: 0x02000036 RID: 54
	internal enum AttribueMask : uint
	{
		// Token: 0x04000244 RID: 580
		CurrentBit = 1U,
		// Token: 0x04000245 RID: 581
		PointBit,
		// Token: 0x04000246 RID: 582
		LineBit = 4U,
		// Token: 0x04000247 RID: 583
		PolygonBit = 8U,
		// Token: 0x04000248 RID: 584
		PolygonStippleBit = 16U,
		// Token: 0x04000249 RID: 585
		PixelModeBit = 32U,
		// Token: 0x0400024A RID: 586
		LightingBit = 64U,
		// Token: 0x0400024B RID: 587
		FogBit = 128U,
		// Token: 0x0400024C RID: 588
		DepthBufferBit = 256U,
		// Token: 0x0400024D RID: 589
		AccumBufferBit = 512U,
		// Token: 0x0400024E RID: 590
		StencilBufferBit = 1024U,
		// Token: 0x0400024F RID: 591
		ViewportBit = 2048U,
		// Token: 0x04000250 RID: 592
		TransformBit = 4096U,
		// Token: 0x04000251 RID: 593
		EnableBit = 8192U,
		// Token: 0x04000252 RID: 594
		ColorBufferBit = 16384U,
		// Token: 0x04000253 RID: 595
		HintBit = 32768U,
		// Token: 0x04000254 RID: 596
		EvalBit = 65536U,
		// Token: 0x04000255 RID: 597
		ListBit = 131072U,
		// Token: 0x04000256 RID: 598
		TextureBit = 262144U,
		// Token: 0x04000257 RID: 599
		ScissorBit = 524288U,
		// Token: 0x04000258 RID: 600
		AllAttribBits = 1048575U
	}
}
