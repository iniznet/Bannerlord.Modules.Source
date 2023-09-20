using System;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020001CB RID: 459
	public class MBTestRun
	{
		// Token: 0x06001A1C RID: 6684 RVA: 0x0005C4FA File Offset: 0x0005A6FA
		public static bool EnterEditMode()
		{
			return MBAPI.IMBTestRun.EnterEditMode();
		}

		// Token: 0x06001A1D RID: 6685 RVA: 0x0005C506 File Offset: 0x0005A706
		public static bool NewScene()
		{
			return MBAPI.IMBTestRun.NewScene();
		}

		// Token: 0x06001A1E RID: 6686 RVA: 0x0005C512 File Offset: 0x0005A712
		public static bool LeaveEditMode()
		{
			return MBAPI.IMBTestRun.LeaveEditMode();
		}

		// Token: 0x06001A1F RID: 6687 RVA: 0x0005C51E File Offset: 0x0005A71E
		public static bool OpenScene(string sceneName)
		{
			return MBAPI.IMBTestRun.OpenScene(sceneName);
		}

		// Token: 0x06001A20 RID: 6688 RVA: 0x0005C52B File Offset: 0x0005A72B
		public static bool CloseScene()
		{
			return MBAPI.IMBTestRun.CloseScene();
		}

		// Token: 0x06001A21 RID: 6689 RVA: 0x0005C537 File Offset: 0x0005A737
		public static int GetFPS()
		{
			return MBAPI.IMBTestRun.GetFPS();
		}

		// Token: 0x06001A22 RID: 6690 RVA: 0x0005C543 File Offset: 0x0005A743
		public static void StartMission()
		{
			MBAPI.IMBTestRun.StartMission();
		}
	}
}
