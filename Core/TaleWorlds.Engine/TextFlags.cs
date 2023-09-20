using System;

namespace TaleWorlds.Engine
{
	// Token: 0x02000067 RID: 103
	[Flags]
	public enum TextFlags
	{
		// Token: 0x04000132 RID: 306
		RglTfHAlignLeft = 1,
		// Token: 0x04000133 RID: 307
		RglTfHAlignRight = 2,
		// Token: 0x04000134 RID: 308
		RglTfHAlignCenter = 3,
		// Token: 0x04000135 RID: 309
		RglTfVAlignTop = 4,
		// Token: 0x04000136 RID: 310
		RglTfVAlignDown = 8,
		// Token: 0x04000137 RID: 311
		RglTfVAlignCenter = 12,
		// Token: 0x04000138 RID: 312
		RglTfSingleLine = 16,
		// Token: 0x04000139 RID: 313
		RglTfMultiline = 32,
		// Token: 0x0400013A RID: 314
		RglTfItalic = 64,
		// Token: 0x0400013B RID: 315
		RglTfCutTextFromLeft = 128,
		// Token: 0x0400013C RID: 316
		RglTfDoubleSpace = 256,
		// Token: 0x0400013D RID: 317
		RglTfWithOutline = 512,
		// Token: 0x0400013E RID: 318
		RglTfHalfSpace = 1024
	}
}
