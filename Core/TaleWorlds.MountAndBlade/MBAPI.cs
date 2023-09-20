using System;
using System.Collections.Generic;
using TaleWorlds.Engine;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200018B RID: 395
	public static class MBAPI
	{
		// Token: 0x060014C3 RID: 5315 RVA: 0x0004F4D4 File Offset: 0x0004D6D4
		private static T GetObject<T>() where T : class
		{
			object obj;
			if (MBAPI._objects.TryGetValue(typeof(T).FullName, out obj))
			{
				return obj as T;
			}
			return default(T);
		}

		// Token: 0x060014C4 RID: 5316 RVA: 0x0004F514 File Offset: 0x0004D714
		internal static void SetObjects(Dictionary<string, object> objects)
		{
			MBAPI._objects = objects;
			MBAPI.IMBTestRun = MBAPI.GetObject<IMBTestRun>();
			MBAPI.IMBActionSet = MBAPI.GetObject<IMBActionSet>();
			MBAPI.IMBAgent = MBAPI.GetObject<IMBAgent>();
			MBAPI.IMBAnimation = MBAPI.GetObject<IMBAnimation>();
			MBAPI.IMBDelegate = MBAPI.GetObject<IMBDelegate>();
			MBAPI.IMBItem = MBAPI.GetObject<IMBItem>();
			MBAPI.IMBEditor = MBAPI.GetObject<IMBEditor>();
			MBAPI.IMBMission = MBAPI.GetObject<IMBMission>();
			MBAPI.IMBMultiplayerData = MBAPI.GetObject<IMBMultiplayerData>();
			MBAPI.IMouseManager = MBAPI.GetObject<IMouseManager>();
			MBAPI.IMBNetwork = MBAPI.GetObject<IMBNetwork>();
			MBAPI.IMBPeer = MBAPI.GetObject<IMBPeer>();
			MBAPI.IMBSkeletonExtensions = MBAPI.GetObject<IMBSkeletonExtensions>();
			MBAPI.IMBGameEntityExtensions = MBAPI.GetObject<IMBGameEntityExtensions>();
			MBAPI.IMBScreen = MBAPI.GetObject<IMBScreen>();
			MBAPI.IMBSoundEvent = MBAPI.GetObject<IMBSoundEvent>();
			MBAPI.IMBVoiceManager = MBAPI.GetObject<IMBVoiceManager>();
			MBAPI.IMBTeam = MBAPI.GetObject<IMBTeam>();
			MBAPI.IMBWorld = MBAPI.GetObject<IMBWorld>();
			MBAPI.IInput = MBAPI.GetObject<IInput>();
			MBAPI.IMBMessageManager = MBAPI.GetObject<IMBMessageManager>();
			MBAPI.IMBWindowManager = MBAPI.GetObject<IMBWindowManager>();
			MBAPI.IMBDebugExtensions = MBAPI.GetObject<IMBDebugExtensions>();
			MBAPI.IMBGame = MBAPI.GetObject<IMBGame>();
			MBAPI.IMBFaceGen = MBAPI.GetObject<IMBFaceGen>();
			MBAPI.IMBMapScene = MBAPI.GetObject<IMBMapScene>();
			MBAPI.IMBBannerlordChecker = MBAPI.GetObject<IMBBannerlordChecker>();
			MBAPI.IMBAgentVisuals = MBAPI.GetObject<IMBAgentVisuals>();
			MBAPI.IMBBannerlordTableauManager = MBAPI.GetObject<IMBBannerlordTableauManager>();
			MBAPI.IMBBannerlordConfig = MBAPI.GetObject<IMBBannerlordConfig>();
		}

		// Token: 0x0400072A RID: 1834
		internal static IMBTestRun IMBTestRun;

		// Token: 0x0400072B RID: 1835
		internal static IMBActionSet IMBActionSet;

		// Token: 0x0400072C RID: 1836
		internal static IMBAgent IMBAgent;

		// Token: 0x0400072D RID: 1837
		internal static IMBAgentVisuals IMBAgentVisuals;

		// Token: 0x0400072E RID: 1838
		internal static IMBAnimation IMBAnimation;

		// Token: 0x0400072F RID: 1839
		internal static IMBDelegate IMBDelegate;

		// Token: 0x04000730 RID: 1840
		internal static IMBItem IMBItem;

		// Token: 0x04000731 RID: 1841
		internal static IMBEditor IMBEditor;

		// Token: 0x04000732 RID: 1842
		internal static IMBMission IMBMission;

		// Token: 0x04000733 RID: 1843
		internal static IMBMultiplayerData IMBMultiplayerData;

		// Token: 0x04000734 RID: 1844
		internal static IMouseManager IMouseManager;

		// Token: 0x04000735 RID: 1845
		internal static IMBNetwork IMBNetwork;

		// Token: 0x04000736 RID: 1846
		internal static IMBPeer IMBPeer;

		// Token: 0x04000737 RID: 1847
		internal static IMBSkeletonExtensions IMBSkeletonExtensions;

		// Token: 0x04000738 RID: 1848
		internal static IMBGameEntityExtensions IMBGameEntityExtensions;

		// Token: 0x04000739 RID: 1849
		internal static IMBScreen IMBScreen;

		// Token: 0x0400073A RID: 1850
		internal static IMBSoundEvent IMBSoundEvent;

		// Token: 0x0400073B RID: 1851
		internal static IMBVoiceManager IMBVoiceManager;

		// Token: 0x0400073C RID: 1852
		internal static IMBTeam IMBTeam;

		// Token: 0x0400073D RID: 1853
		internal static IMBWorld IMBWorld;

		// Token: 0x0400073E RID: 1854
		internal static IInput IInput;

		// Token: 0x0400073F RID: 1855
		internal static IMBMessageManager IMBMessageManager;

		// Token: 0x04000740 RID: 1856
		internal static IMBWindowManager IMBWindowManager;

		// Token: 0x04000741 RID: 1857
		internal static IMBDebugExtensions IMBDebugExtensions;

		// Token: 0x04000742 RID: 1858
		internal static IMBGame IMBGame;

		// Token: 0x04000743 RID: 1859
		internal static IMBFaceGen IMBFaceGen;

		// Token: 0x04000744 RID: 1860
		internal static IMBMapScene IMBMapScene;

		// Token: 0x04000745 RID: 1861
		internal static IMBBannerlordChecker IMBBannerlordChecker;

		// Token: 0x04000746 RID: 1862
		internal static IMBBannerlordTableauManager IMBBannerlordTableauManager;

		// Token: 0x04000747 RID: 1863
		internal static IMBBannerlordConfig IMBBannerlordConfig;

		// Token: 0x04000748 RID: 1864
		private static Dictionary<string, object> _objects;
	}
}
