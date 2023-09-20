using System;
using System.Runtime.InteropServices;

namespace TaleWorlds.TwoDimension.Standalone.Native.Windows
{
	// Token: 0x02000013 RID: 19
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct BitmapInfoHeader
	{
		// Token: 0x0400005F RID: 95
		public uint biSize;

		// Token: 0x04000060 RID: 96
		public int biWidth;

		// Token: 0x04000061 RID: 97
		public int biHeight;

		// Token: 0x04000062 RID: 98
		public ushort biPlanes;

		// Token: 0x04000063 RID: 99
		public ushort biBitCount;

		// Token: 0x04000064 RID: 100
		public uint biCompression;

		// Token: 0x04000065 RID: 101
		public uint biSizeImage;

		// Token: 0x04000066 RID: 102
		public int biXPelsPerMeter;

		// Token: 0x04000067 RID: 103
		public int biYPelsPerMeter;

		// Token: 0x04000068 RID: 104
		public uint biClrUsed;

		// Token: 0x04000069 RID: 105
		public uint biClrImportant;
	}
}
