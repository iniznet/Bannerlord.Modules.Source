using System;
using System.Runtime.InteropServices;

namespace TaleWorlds.TwoDimension.Standalone.Native.Windows
{
	// Token: 0x02000012 RID: 18
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct BitmapInfo
	{
		// Token: 0x0400005A RID: 90
		public BitmapInfoHeader bmiHeader;

		// Token: 0x0400005B RID: 91
		public byte r;

		// Token: 0x0400005C RID: 92
		public byte g;

		// Token: 0x0400005D RID: 93
		public byte b;

		// Token: 0x0400005E RID: 94
		public byte a;
	}
}
