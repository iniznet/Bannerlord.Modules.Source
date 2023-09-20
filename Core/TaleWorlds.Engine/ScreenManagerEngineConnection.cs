using System;
using TaleWorlds.Library;
using TaleWorlds.ScreenSystem;

namespace TaleWorlds.Engine
{
	public class ScreenManagerEngineConnection : IScreenManagerEngineConnection
	{
		float IScreenManagerEngineConnection.RealScreenResolutionWidth
		{
			get
			{
				return Screen.RealScreenResolutionWidth;
			}
		}

		float IScreenManagerEngineConnection.RealScreenResolutionHeight
		{
			get
			{
				return Screen.RealScreenResolutionHeight;
			}
		}

		float IScreenManagerEngineConnection.AspectRatio
		{
			get
			{
				return Screen.AspectRatio;
			}
		}

		Vec2 IScreenManagerEngineConnection.DesktopResolution
		{
			get
			{
				return Screen.DesktopResolution;
			}
		}

		void IScreenManagerEngineConnection.ActivateMouseCursor(CursorType mouseId)
		{
			MouseManager.ActivateMouseCursor(mouseId);
		}

		void IScreenManagerEngineConnection.SetMouseVisible(bool value)
		{
			EngineApplicationInterface.IScreen.SetMouseVisible(value);
		}

		bool IScreenManagerEngineConnection.GetMouseVisible()
		{
			return EngineApplicationInterface.IScreen.GetMouseVisible();
		}

		bool IScreenManagerEngineConnection.GetIsEnterButtonRDown()
		{
			return EngineApplicationInterface.IScreen.IsEnterButtonCross();
		}

		void IScreenManagerEngineConnection.BeginDebugPanel(string panelTitle)
		{
			Imgui.BeginMainThreadScope();
			Imgui.Begin(panelTitle);
		}

		void IScreenManagerEngineConnection.EndDebugPanel()
		{
			Imgui.End();
			Imgui.EndMainThreadScope();
		}

		void IScreenManagerEngineConnection.DrawDebugText(string text)
		{
			Imgui.Text(text);
		}

		bool IScreenManagerEngineConnection.DrawDebugTreeNode(string text)
		{
			return Imgui.TreeNode(text);
		}

		void IScreenManagerEngineConnection.PopDebugTreeNode()
		{
			Imgui.TreePop();
		}
	}
}
