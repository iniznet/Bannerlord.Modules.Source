using System;
using System.Runtime.InteropServices;

namespace TaleWorlds.TwoDimension.Standalone.Native.Windows
{
	// Token: 0x02000019 RID: 25
	internal static class Gdi32
	{
		// Token: 0x060000E8 RID: 232
		[DllImport("gdi32.dll")]
		public static extern int ChoosePixelFormat(IntPtr hdc, [In] ref PixelFormatDescriptor ppfd);

		// Token: 0x060000E9 RID: 233
		[DllImport("gdi32.dll")]
		public static extern bool SetPixelFormat(IntPtr hdc, int iPixelFormat, ref PixelFormatDescriptor ppfd);

		// Token: 0x060000EA RID: 234
		[DllImport("gdi32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool SwapBuffers(IntPtr hdc);

		// Token: 0x060000EB RID: 235
		[DllImport("gdi32.dll")]
		public static extern IntPtr CreateRectRgn(int x1, int y1, int x2, int y2);

		// Token: 0x060000EC RID: 236
		[DllImport("gdi32.dll")]
		public static extern IntPtr CreateSolidBrush(IntPtr colorRef);

		// Token: 0x060000ED RID: 237
		[DllImport("gdi32.dll")]
		public static extern IntPtr CreateCompatibleDC(IntPtr hdc);

		// Token: 0x060000EE RID: 238
		[DllImport("gdi32.dll")]
		public static extern IntPtr SelectObject(IntPtr hdc, IntPtr h);

		// Token: 0x060000EF RID: 239
		[DllImport("gdi32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool DeleteObject(IntPtr ho);

		// Token: 0x060000F0 RID: 240
		[DllImport("gdi32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool DeleteDC(IntPtr hdc);

		// Token: 0x060000F1 RID: 241
		[DllImport("gdi32.dll")]
		public static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int cx, int cy);

		// Token: 0x060000F2 RID: 242
		[DllImport("gdi32.dll")]
		public static extern int StretchDIBits(IntPtr hdc, int xDest, int yDest, int DestWidth, int DestHeight, int xSrc, int ySrc, int SrcWidth, int SrcHeight, byte[] lpBits, ref BitmapInfo lpbmi, uint iUsage, int rop);
	}
}
