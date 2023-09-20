using System;
using TaleWorlds.Library;
using TaleWorlds.ScreenSystem;

namespace TaleWorlds.Engine
{
	// Token: 0x02000043 RID: 67
	public class ScreenManagerEngineConnection : IScreenManagerEngineConnection
	{
		// Token: 0x1700000F RID: 15
		// (get) Token: 0x060005B0 RID: 1456 RVA: 0x000035D3 File Offset: 0x000017D3
		float IScreenManagerEngineConnection.RealScreenResolutionWidth
		{
			get
			{
				return Screen.RealScreenResolutionWidth;
			}
		}

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x060005B1 RID: 1457 RVA: 0x000035DA File Offset: 0x000017DA
		float IScreenManagerEngineConnection.RealScreenResolutionHeight
		{
			get
			{
				return Screen.RealScreenResolutionHeight;
			}
		}

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x060005B2 RID: 1458 RVA: 0x000035E1 File Offset: 0x000017E1
		float IScreenManagerEngineConnection.AspectRatio
		{
			get
			{
				return Screen.AspectRatio;
			}
		}

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x060005B3 RID: 1459 RVA: 0x000035E8 File Offset: 0x000017E8
		Vec2 IScreenManagerEngineConnection.DesktopResolution
		{
			get
			{
				return Screen.DesktopResolution;
			}
		}

		// Token: 0x060005B4 RID: 1460 RVA: 0x000035EF File Offset: 0x000017EF
		void IScreenManagerEngineConnection.ActivateMouseCursor(CursorType mouseId)
		{
			MouseManager.ActivateMouseCursor(mouseId);
		}

		// Token: 0x060005B5 RID: 1461 RVA: 0x000035F7 File Offset: 0x000017F7
		void IScreenManagerEngineConnection.SetMouseVisible(bool value)
		{
			EngineApplicationInterface.IScreen.SetMouseVisible(value);
		}

		// Token: 0x060005B6 RID: 1462 RVA: 0x00003604 File Offset: 0x00001804
		bool IScreenManagerEngineConnection.GetMouseVisible()
		{
			return EngineApplicationInterface.IScreen.GetMouseVisible();
		}

		// Token: 0x060005B7 RID: 1463 RVA: 0x00003610 File Offset: 0x00001810
		bool IScreenManagerEngineConnection.GetIsEnterButtonRDown()
		{
			return EngineApplicationInterface.IScreen.IsEnterButtonCross();
		}

		// Token: 0x060005B8 RID: 1464 RVA: 0x0000361C File Offset: 0x0000181C
		void IScreenManagerEngineConnection.BeginDebugPanel(string panelTitle)
		{
			Imgui.BeginMainThreadScope();
			Imgui.Begin(panelTitle);
		}

		// Token: 0x060005B9 RID: 1465 RVA: 0x00003629 File Offset: 0x00001829
		void IScreenManagerEngineConnection.EndDebugPanel()
		{
			Imgui.End();
			Imgui.EndMainThreadScope();
		}

		// Token: 0x060005BA RID: 1466 RVA: 0x00003635 File Offset: 0x00001835
		void IScreenManagerEngineConnection.DrawDebugText(string text)
		{
			Imgui.Text(text);
		}

		// Token: 0x060005BB RID: 1467 RVA: 0x0000363D File Offset: 0x0000183D
		bool IScreenManagerEngineConnection.DrawDebugTreeNode(string text)
		{
			return Imgui.TreeNode(text);
		}

		// Token: 0x060005BC RID: 1468 RVA: 0x00003645 File Offset: 0x00001845
		void IScreenManagerEngineConnection.PopDebugTreeNode()
		{
			Imgui.TreePop();
		}
	}
}
