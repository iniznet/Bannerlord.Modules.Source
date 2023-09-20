using System;
using TaleWorlds.ScreenSystem;

namespace TaleWorlds.Engine
{
	// Token: 0x02000068 RID: 104
	public static class MouseManager
	{
		// Token: 0x0600084C RID: 2124 RVA: 0x000083FE File Offset: 0x000065FE
		public static void ActivateMouseCursor(CursorType mouseId)
		{
			EngineApplicationInterface.IMouseManager.ActivateMouseCursor((int)mouseId);
		}

		// Token: 0x0600084D RID: 2125 RVA: 0x0000840B File Offset: 0x0000660B
		public static void SetMouseCursor(CursorType mouseId, string mousePath)
		{
			EngineApplicationInterface.IMouseManager.SetMouseCursor((int)mouseId, mousePath);
		}

		// Token: 0x0600084E RID: 2126 RVA: 0x00008419 File Offset: 0x00006619
		public static void ShowCursor(bool show)
		{
			EngineApplicationInterface.IMouseManager.ShowCursor(show);
		}

		// Token: 0x0600084F RID: 2127 RVA: 0x00008426 File Offset: 0x00006626
		public static void LockCursorAtCurrentPosition(bool lockCursor)
		{
			EngineApplicationInterface.IMouseManager.LockCursorAtCurrentPosition(lockCursor);
		}

		// Token: 0x06000850 RID: 2128 RVA: 0x00008433 File Offset: 0x00006633
		public static void LockCursorAtPosition(float x, float y)
		{
			EngineApplicationInterface.IMouseManager.LockCursorAtPosition(x, y);
		}

		// Token: 0x06000851 RID: 2129 RVA: 0x00008441 File Offset: 0x00006641
		public static void UnlockCursor()
		{
			EngineApplicationInterface.IMouseManager.UnlockCursor();
		}
	}
}
