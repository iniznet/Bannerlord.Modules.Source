using System;
using System.Runtime.InteropServices;

namespace TaleWorlds.TwoDimension.Standalone.Native.Windows
{
	// Token: 0x0200001A RID: 26
	public static class Kernel32
	{
		// Token: 0x060000F3 RID: 243
		[DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern IntPtr LoadLibrary(string lpFileName);

		// Token: 0x060000F4 RID: 244
		[DllImport("kernel32.dll", CharSet = CharSet.Auto)]
		public static extern IntPtr GetModuleHandle(string lpModuleName);

		// Token: 0x060000F5 RID: 245
		[DllImport("kernel32.dll", CharSet = CharSet.Auto)]
		public static extern int GetLastError();

		// Token: 0x060000F6 RID: 246
		[DllImport("kernel32.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
		public static extern IntPtr GetConsoleWindow();

		// Token: 0x060000F7 RID: 247
		[DllImport("kernel32.dll", CallingConvention = CallingConvention.StdCall, ExactSpelling = true, SetLastError = true)]
		public static extern int GetUserGeoID(Kernel32.GeoTypeId type);

		// Token: 0x0200003C RID: 60
		public enum GeoTypeId
		{
			// Token: 0x040002D1 RID: 721
			Nation = 16,
			// Token: 0x040002D2 RID: 722
			Region = 14
		}
	}
}
