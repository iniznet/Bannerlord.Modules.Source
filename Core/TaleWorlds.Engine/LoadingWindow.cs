using System;

namespace TaleWorlds.Engine
{
	public static class LoadingWindow
	{
		public static bool IsLoadingWindowActive { get; private set; }

		public static void Initialize(ILoadingWindowManager loadingWindowManager)
		{
			LoadingWindow._loadingWindowManager = loadingWindowManager;
		}

		public static void Destroy()
		{
			if (LoadingWindow.IsLoadingWindowActive)
			{
				LoadingWindow.DisableGlobalLoadingWindow();
			}
			LoadingWindow._loadingWindowManager = null;
		}

		public static void DisableGlobalLoadingWindow()
		{
			if (LoadingWindow._loadingWindowManager == null)
			{
				return;
			}
			if (LoadingWindow.IsLoadingWindowActive)
			{
				LoadingWindow._loadingWindowManager.DisableLoadingWindow();
				Utilities.DisableGlobalLoadingWindow();
				Utilities.OnLoadingWindowDisabled();
			}
			LoadingWindow.IsLoadingWindowActive = false;
			Utilities.DebugSetGlobalLoadingWindowState(false);
		}

		public static bool GetGlobalLoadingWindowState()
		{
			return LoadingWindow.IsLoadingWindowActive;
		}

		public static void EnableGlobalLoadingWindow()
		{
			if (LoadingWindow._loadingWindowManager == null)
			{
				return;
			}
			LoadingWindow.IsLoadingWindowActive = true;
			Utilities.DebugSetGlobalLoadingWindowState(true);
			if (LoadingWindow.IsLoadingWindowActive)
			{
				LoadingWindow._loadingWindowManager.EnableLoadingWindow();
				Utilities.OnLoadingWindowEnabled();
			}
		}

		private static ILoadingWindowManager _loadingWindowManager;
	}
}
