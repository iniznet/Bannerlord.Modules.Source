using System;

namespace TaleWorlds.TwoDimension.Standalone.Native.Windows
{
	public struct BlendFunction
	{
		public BlendFunction(AlphaFormatFlags op, byte flags, byte alpha, AlphaFormatFlags format)
		{
			this.BlendOp = (byte)op;
			this.BlendFlags = flags;
			this.SourceConstantAlpha = alpha;
			this.AlphaFormat = (byte)format;
		}

		public byte BlendOp;

		public byte BlendFlags;

		public byte SourceConstantAlpha;

		public byte AlphaFormat;

		public static readonly BlendFunction Default = new BlendFunction(AlphaFormatFlags.Over, 0, byte.MaxValue, AlphaFormatFlags.Alpha);
	}
}
