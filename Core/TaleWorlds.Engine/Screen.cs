using System;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
	public static class Screen
	{
		public static float RealScreenResolutionWidth { get; private set; }

		public static float RealScreenResolutionHeight { get; private set; }

		public static Vec2 RealScreenResolution
		{
			get
			{
				return new Vec2(Screen.RealScreenResolutionWidth, Screen.RealScreenResolutionHeight);
			}
		}

		public static float AspectRatio { get; private set; }

		public static Vec2 DesktopResolution { get; private set; }

		internal static void Update()
		{
			Screen.RealScreenResolutionWidth = EngineApplicationInterface.IScreen.GetRealScreenResolutionWidth();
			Screen.RealScreenResolutionHeight = EngineApplicationInterface.IScreen.GetRealScreenResolutionHeight();
			Screen.AspectRatio = EngineApplicationInterface.IScreen.GetAspectRatio();
			Screen.DesktopResolution = new Vec2(EngineApplicationInterface.IScreen.GetDesktopWidth(), EngineApplicationInterface.IScreen.GetDesktopHeight());
		}

		public static bool GetMouseVisible()
		{
			return EngineApplicationInterface.IScreen.GetMouseVisible();
		}
	}
}
