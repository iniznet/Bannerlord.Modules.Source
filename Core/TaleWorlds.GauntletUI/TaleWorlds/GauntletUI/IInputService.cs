using System;

namespace TaleWorlds.GauntletUI
{
	// Token: 0x02000026 RID: 38
	public interface IInputService
	{
		// Token: 0x170000E2 RID: 226
		// (get) Token: 0x060002DB RID: 731
		bool MouseEnabled { get; }

		// Token: 0x170000E3 RID: 227
		// (get) Token: 0x060002DC RID: 732
		bool KeyboardEnabled { get; }

		// Token: 0x170000E4 RID: 228
		// (get) Token: 0x060002DD RID: 733
		bool GamepadEnabled { get; }
	}
}
