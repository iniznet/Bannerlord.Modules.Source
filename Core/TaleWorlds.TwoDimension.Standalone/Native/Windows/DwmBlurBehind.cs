using System;
using System.Runtime.InteropServices;

namespace TaleWorlds.TwoDimension.Standalone.Native.Windows
{
	// Token: 0x02000017 RID: 23
	internal struct DwmBlurBehind
	{
		// Token: 0x04000072 RID: 114
		public BlurBehindConstraints dwFlags;

		// Token: 0x04000073 RID: 115
		[MarshalAs(UnmanagedType.Bool)]
		public bool fEnable;

		// Token: 0x04000074 RID: 116
		public IntPtr hRgnBlur;

		// Token: 0x04000075 RID: 117
		[MarshalAs(UnmanagedType.Bool)]
		public bool fTransitionOnMaximized;
	}
}
