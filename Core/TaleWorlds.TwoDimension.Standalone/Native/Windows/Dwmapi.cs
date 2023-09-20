using System;
using System.Runtime.InteropServices;

namespace TaleWorlds.TwoDimension.Standalone.Native.Windows
{
	// Token: 0x02000016 RID: 22
	internal static class Dwmapi
	{
		// Token: 0x060000E7 RID: 231
		[DllImport("Dwmapi.dll")]
		public static extern IntPtr DwmEnableBlurBehindWindow(IntPtr hwnd, [In] ref DwmBlurBehind ppfd);
	}
}
