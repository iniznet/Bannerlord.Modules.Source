using System;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
	// Token: 0x02000034 RID: 52
	[ApplicationInterfaceBase]
	internal interface IMouseManager
	{
		// Token: 0x0600046D RID: 1133
		[EngineMethod("activate_mouse_cursor", false)]
		void ActivateMouseCursor(int id);

		// Token: 0x0600046E RID: 1134
		[EngineMethod("set_mouse_cursor", false)]
		void SetMouseCursor(int id, string mousePath);

		// Token: 0x0600046F RID: 1135
		[EngineMethod("show_cursor", false)]
		void ShowCursor(bool show);

		// Token: 0x06000470 RID: 1136
		[EngineMethod("lock_cursor_at_current_pos", false)]
		void LockCursorAtCurrentPosition(bool lockCursor);

		// Token: 0x06000471 RID: 1137
		[EngineMethod("lock_cursor_at_position", false)]
		void LockCursorAtPosition(float x, float y);

		// Token: 0x06000472 RID: 1138
		[EngineMethod("unlock_cursor", false)]
		void UnlockCursor();
	}
}
