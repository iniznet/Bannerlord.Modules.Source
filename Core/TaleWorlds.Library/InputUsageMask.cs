using System;

namespace TaleWorlds.Library
{
	// Token: 0x0200003D RID: 61
	[Flags]
	public enum InputUsageMask
	{
		// Token: 0x040000C3 RID: 195
		Invalid = 0,
		// Token: 0x040000C4 RID: 196
		MouseButtons = 1,
		// Token: 0x040000C5 RID: 197
		MouseWheels = 2,
		// Token: 0x040000C6 RID: 198
		Keyboardkeys = 4,
		// Token: 0x040000C7 RID: 199
		BlockEverythingWithoutHitTest = 8,
		// Token: 0x040000C8 RID: 200
		Mouse = 3,
		// Token: 0x040000C9 RID: 201
		All = 7
	}
}
