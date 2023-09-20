using System;
using System.Runtime.InteropServices;

namespace TaleWorlds.TwoDimension.Standalone.Native.Windows
{
	// Token: 0x02000023 RID: 35
	public struct WindowClass
	{
		// Token: 0x040000B4 RID: 180
		public uint style;

		// Token: 0x040000B5 RID: 181
		[MarshalAs(UnmanagedType.FunctionPtr)]
		public WndProc lpfnWndProc;

		// Token: 0x040000B6 RID: 182
		public int cbClsExtra;

		// Token: 0x040000B7 RID: 183
		public int cbWndExtra;

		// Token: 0x040000B8 RID: 184
		public IntPtr hInstance;

		// Token: 0x040000B9 RID: 185
		public IntPtr hIcon;

		// Token: 0x040000BA RID: 186
		public IntPtr hCursor;

		// Token: 0x040000BB RID: 187
		public IntPtr hbrBackground;

		// Token: 0x040000BC RID: 188
		[MarshalAs(UnmanagedType.LPTStr)]
		public string lpszMenuName;

		// Token: 0x040000BD RID: 189
		[MarshalAs(UnmanagedType.LPTStr)]
		public string lpszClassName;
	}
}
