using System;

namespace TaleWorlds.TwoDimension.Standalone.Native.Windows
{
	// Token: 0x0200001B RID: 27
	public struct NativeMessage
	{
		// Token: 0x0400007A RID: 122
		public IntPtr handle;

		// Token: 0x0400007B RID: 123
		public WindowMessage msg;

		// Token: 0x0400007C RID: 124
		public IntPtr wParam;

		// Token: 0x0400007D RID: 125
		public IntPtr lParam;

		// Token: 0x0400007E RID: 126
		public uint time;

		// Token: 0x0400007F RID: 127
		public Point p;
	}
}
