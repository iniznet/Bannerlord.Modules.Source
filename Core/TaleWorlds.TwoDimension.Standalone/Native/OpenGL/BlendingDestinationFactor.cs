using System;

namespace TaleWorlds.TwoDimension.Standalone.Native.OpenGL
{
	// Token: 0x02000033 RID: 51
	internal enum BlendingDestinationFactor : uint
	{
		// Token: 0x04000231 RID: 561
		Zero,
		// Token: 0x04000232 RID: 562
		One,
		// Token: 0x04000233 RID: 563
		SourceColor = 768U,
		// Token: 0x04000234 RID: 564
		OneMinusSourceColor,
		// Token: 0x04000235 RID: 565
		SourceAlpha,
		// Token: 0x04000236 RID: 566
		OneMinusSourceAlpha,
		// Token: 0x04000237 RID: 567
		DestinationAlpha,
		// Token: 0x04000238 RID: 568
		OneMinusDestinationAlpha,
		// Token: 0x04000239 RID: 569
		DestinationColor,
		// Token: 0x0400023A RID: 570
		OneMinusDestinationColor,
		// Token: 0x0400023B RID: 571
		SourceAlphaSaturate
	}
}
