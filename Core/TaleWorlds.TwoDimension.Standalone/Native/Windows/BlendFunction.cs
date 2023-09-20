using System;

namespace TaleWorlds.TwoDimension.Standalone.Native.Windows
{
	// Token: 0x02000015 RID: 21
	public struct BlendFunction
	{
		// Token: 0x060000E5 RID: 229 RVA: 0x00004DE1 File Offset: 0x00002FE1
		public BlendFunction(AlphaFormatFlags op, byte flags, byte alpha, AlphaFormatFlags format)
		{
			this.BlendOp = (byte)op;
			this.BlendFlags = flags;
			this.SourceConstantAlpha = alpha;
			this.AlphaFormat = (byte)format;
		}

		// Token: 0x0400006D RID: 109
		public byte BlendOp;

		// Token: 0x0400006E RID: 110
		public byte BlendFlags;

		// Token: 0x0400006F RID: 111
		public byte SourceConstantAlpha;

		// Token: 0x04000070 RID: 112
		public byte AlphaFormat;

		// Token: 0x04000071 RID: 113
		public static readonly BlendFunction Default = new BlendFunction(AlphaFormatFlags.Over, 0, byte.MaxValue, AlphaFormatFlags.Alpha);
	}
}
