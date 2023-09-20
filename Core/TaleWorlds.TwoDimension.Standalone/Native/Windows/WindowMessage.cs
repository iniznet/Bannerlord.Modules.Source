using System;

namespace TaleWorlds.TwoDimension.Standalone.Native.Windows
{
	// Token: 0x02000024 RID: 36
	public enum WindowMessage : uint
	{
		// Token: 0x040000BF RID: 191
		Quit = 18U,
		// Token: 0x040000C0 RID: 192
		Close = 16U,
		// Token: 0x040000C1 RID: 193
		Size = 5U,
		// Token: 0x040000C2 RID: 194
		KeyDown = 256U,
		// Token: 0x040000C3 RID: 195
		KeyUp,
		// Token: 0x040000C4 RID: 196
		RightButtonUp = 517U,
		// Token: 0x040000C5 RID: 197
		RightButtonDown = 516U,
		// Token: 0x040000C6 RID: 198
		LeftButtonUp = 514U,
		// Token: 0x040000C7 RID: 199
		LeftButtonDown = 513U,
		// Token: 0x040000C8 RID: 200
		MouseMove = 512U,
		// Token: 0x040000C9 RID: 201
		MouseWheel = 522U,
		// Token: 0x040000CA RID: 202
		KillFocus = 8U,
		// Token: 0x040000CB RID: 203
		SetFocus = 7U
	}
}
