using System;

namespace TaleWorlds.Engine
{
	// Token: 0x02000051 RID: 81
	public static class LoadingWindow
	{
		// Token: 0x17000030 RID: 48
		// (get) Token: 0x060006EA RID: 1770 RVA: 0x000051A8 File Offset: 0x000033A8
		// (set) Token: 0x060006EB RID: 1771 RVA: 0x000051AF File Offset: 0x000033AF
		public static bool IsLoadingWindowActive { get; private set; }

		// Token: 0x060006EC RID: 1772 RVA: 0x000051B7 File Offset: 0x000033B7
		public static void Initialize(ILoadingWindowManager loadingWindowManager)
		{
			LoadingWindow._loadingWindowManager = loadingWindowManager;
		}

		// Token: 0x060006ED RID: 1773 RVA: 0x000051BF File Offset: 0x000033BF
		public static void Destroy()
		{
			if (LoadingWindow.IsLoadingWindowActive)
			{
				LoadingWindow.DisableGlobalLoadingWindow();
			}
			LoadingWindow._loadingWindowManager = null;
		}

		// Token: 0x060006EE RID: 1774 RVA: 0x000051D3 File Offset: 0x000033D3
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

		// Token: 0x060006EF RID: 1775 RVA: 0x00005204 File Offset: 0x00003404
		public static bool GetGlobalLoadingWindowState()
		{
			return LoadingWindow.IsLoadingWindowActive;
		}

		// Token: 0x060006F0 RID: 1776 RVA: 0x0000520B File Offset: 0x0000340B
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

		// Token: 0x040000A1 RID: 161
		private static ILoadingWindowManager _loadingWindowManager;
	}
}
