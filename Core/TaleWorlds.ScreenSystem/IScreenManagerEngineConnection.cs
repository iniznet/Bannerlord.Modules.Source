using System;
using TaleWorlds.Library;

namespace TaleWorlds.ScreenSystem
{
	// Token: 0x02000005 RID: 5
	public interface IScreenManagerEngineConnection
	{
		// Token: 0x17000007 RID: 7
		// (get) Token: 0x0600001C RID: 28
		float RealScreenResolutionWidth { get; }

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x0600001D RID: 29
		float RealScreenResolutionHeight { get; }

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x0600001E RID: 30
		float AspectRatio { get; }

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x0600001F RID: 31
		Vec2 DesktopResolution { get; }

		// Token: 0x06000020 RID: 32
		void ActivateMouseCursor(CursorType mouseId);

		// Token: 0x06000021 RID: 33
		void SetMouseVisible(bool value);

		// Token: 0x06000022 RID: 34
		bool GetMouseVisible();

		// Token: 0x06000023 RID: 35
		bool GetIsEnterButtonRDown();

		// Token: 0x06000024 RID: 36
		void BeginDebugPanel(string panelTitle);

		// Token: 0x06000025 RID: 37
		void EndDebugPanel();

		// Token: 0x06000026 RID: 38
		void DrawDebugText(string text);

		// Token: 0x06000027 RID: 39
		bool DrawDebugTreeNode(string text);

		// Token: 0x06000028 RID: 40
		void PopDebugTreeNode();
	}
}
