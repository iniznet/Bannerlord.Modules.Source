using System;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.ScreenSystem;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020001CD RID: 461
	public class MBWindowManager
	{
		// Token: 0x06001A28 RID: 6696 RVA: 0x0005C586 File Offset: 0x0005A786
		public static float WorldToScreen(Camera camera, Vec3 worldSpacePosition, ref float screenX, ref float screenY, ref float w)
		{
			return MBAPI.IMBWindowManager.WorldToScreen(camera.Pointer, worldSpacePosition, ref screenX, ref screenY, ref w);
		}

		// Token: 0x06001A29 RID: 6697 RVA: 0x0005C5A0 File Offset: 0x0005A7A0
		public static float WorldToScreenInsideUsableArea(Camera camera, Vec3 worldSpacePosition, ref float screenX, ref float screenY, ref float w)
		{
			float num = MBAPI.IMBWindowManager.WorldToScreen(camera.Pointer, worldSpacePosition, ref screenX, ref screenY, ref w);
			screenX -= (Screen.RealScreenResolutionWidth - ScreenManager.UsableArea.X * Screen.RealScreenResolutionWidth) / 2f;
			screenY -= (Screen.RealScreenResolutionHeight - ScreenManager.UsableArea.Y * Screen.RealScreenResolutionHeight) / 2f;
			return num;
		}

		// Token: 0x06001A2A RID: 6698 RVA: 0x0005C60A File Offset: 0x0005A80A
		public static float WorldToScreenWithFixedZ(Camera camera, Vec3 cameraPosition, Vec3 worldSpacePosition, ref float screenX, ref float screenY, ref float w)
		{
			return MBAPI.IMBWindowManager.WorldToScreenWithFixedZ(camera.Pointer, cameraPosition, worldSpacePosition, ref screenX, ref screenY, ref w);
		}

		// Token: 0x06001A2B RID: 6699 RVA: 0x0005C623 File Offset: 0x0005A823
		public static void ScreenToWorld(Camera camera, float screenX, float screenY, float w, ref Vec3 worldSpacePosition)
		{
			MBAPI.IMBWindowManager.ScreenToWorld(camera.Pointer, screenX, screenY, w, ref worldSpacePosition);
		}

		// Token: 0x06001A2C RID: 6700 RVA: 0x0005C63A File Offset: 0x0005A83A
		public static void PreDisplay()
		{
			MBAPI.IMBWindowManager.PreDisplay();
		}

		// Token: 0x06001A2D RID: 6701 RVA: 0x0005C646 File Offset: 0x0005A846
		public static void DontChangeCursorPos()
		{
			MBAPI.IMBWindowManager.DontChangeCursorPos();
		}
	}
}
