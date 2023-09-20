using System;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020001AE RID: 430
	public class MBDebugManager : IDebugManager
	{
		// Token: 0x06001902 RID: 6402 RVA: 0x0005AB2C File Offset: 0x00058D2C
		void IDebugManager.SetCrashReportCustomString(string customString)
		{
			Utilities.SetCrashReportCustomString(customString);
		}

		// Token: 0x06001903 RID: 6403 RVA: 0x0005AB34 File Offset: 0x00058D34
		void IDebugManager.SetCrashReportCustomStack(string customStack)
		{
			Utilities.SetCrashReportCustomStack(customStack);
		}

		// Token: 0x06001904 RID: 6404 RVA: 0x0005AB3C File Offset: 0x00058D3C
		void IDebugManager.ShowWarning(string message)
		{
			MBDebug.ShowWarning(message);
		}

		// Token: 0x06001905 RID: 6405 RVA: 0x0005AB44 File Offset: 0x00058D44
		void IDebugManager.ShowError(string message)
		{
			MBDebug.ShowError(message);
		}

		// Token: 0x06001906 RID: 6406 RVA: 0x0005AB4C File Offset: 0x00058D4C
		void IDebugManager.ShowMessageBox(string lpText, string lpCaption, uint uType)
		{
			MBDebug.ShowMessageBox(lpText, lpCaption, uType);
		}

		// Token: 0x06001907 RID: 6407 RVA: 0x0005AB56 File Offset: 0x00058D56
		void IDebugManager.Assert(bool condition, string message, string callerFile, string callerMethod, int callerLine)
		{
		}

		// Token: 0x06001908 RID: 6408 RVA: 0x0005AB58 File Offset: 0x00058D58
		void IDebugManager.SilentAssert(bool condition, string message, bool getDump, string callerFile, string callerMethod, int callerLine)
		{
			MBDebug.SilentAssert(condition, message, getDump, callerFile, callerMethod, callerLine);
		}

		// Token: 0x06001909 RID: 6409 RVA: 0x0005AB68 File Offset: 0x00058D68
		void IDebugManager.Print(string message, int logLevel, Debug.DebugColor color, ulong debugFilter)
		{
			MBDebug.Print(message, logLevel, color, debugFilter);
		}

		// Token: 0x0600190A RID: 6410 RVA: 0x0005AB74 File Offset: 0x00058D74
		void IDebugManager.PrintError(string error, string stackTrace, ulong debugFilter)
		{
			MBDebug.Print(error, 0, Debug.DebugColor.White, debugFilter);
		}

		// Token: 0x0600190B RID: 6411 RVA: 0x0005AB80 File Offset: 0x00058D80
		void IDebugManager.PrintWarning(string warning, ulong debugFilter)
		{
			MBDebug.Print(warning, 0, Debug.DebugColor.White, debugFilter);
		}

		// Token: 0x0600190C RID: 6412 RVA: 0x0005AB8C File Offset: 0x00058D8C
		void IDebugManager.DisplayDebugMessage(string message)
		{
		}

		// Token: 0x0600190D RID: 6413 RVA: 0x0005AB8E File Offset: 0x00058D8E
		void IDebugManager.WatchVariable(string name, object value)
		{
		}

		// Token: 0x0600190E RID: 6414 RVA: 0x0005AB90 File Offset: 0x00058D90
		void IDebugManager.WriteDebugLineOnScreen(string message)
		{
		}

		// Token: 0x0600190F RID: 6415 RVA: 0x0005AB92 File Offset: 0x00058D92
		void IDebugManager.RenderDebugLine(Vec3 position, Vec3 direction, uint color, bool depthCheck, float time)
		{
		}

		// Token: 0x06001910 RID: 6416 RVA: 0x0005AB94 File Offset: 0x00058D94
		void IDebugManager.RenderDebugSphere(Vec3 position, float radius, uint color, bool depthCheck, float time)
		{
		}

		// Token: 0x06001911 RID: 6417 RVA: 0x0005AB96 File Offset: 0x00058D96
		void IDebugManager.RenderDebugFrame(MatrixFrame frame, float lineLength, float time)
		{
		}

		// Token: 0x06001912 RID: 6418 RVA: 0x0005AB98 File Offset: 0x00058D98
		void IDebugManager.RenderDebugText(float screenX, float screenY, string text, uint color, float time)
		{
		}

		// Token: 0x06001913 RID: 6419 RVA: 0x0005AB9A File Offset: 0x00058D9A
		void IDebugManager.RenderDebugText3D(Vec3 position, string text, uint color, int screenPosOffsetX, int screenPosOffsetY, float time)
		{
		}

		// Token: 0x06001914 RID: 6420 RVA: 0x0005AB9C File Offset: 0x00058D9C
		void IDebugManager.RenderDebugRectWithColor(float left, float bottom, float right, float top, uint color)
		{
		}

		// Token: 0x06001915 RID: 6421 RVA: 0x0005AB9E File Offset: 0x00058D9E
		Vec3 IDebugManager.GetDebugVector()
		{
			return MBDebug.DebugVector;
		}

		// Token: 0x06001916 RID: 6422 RVA: 0x0005ABA5 File Offset: 0x00058DA5
		void IDebugManager.SetTestModeEnabled(bool testModeEnabled)
		{
			MBDebug.TestModeEnabled = testModeEnabled;
		}

		// Token: 0x06001917 RID: 6423 RVA: 0x0005ABAD File Offset: 0x00058DAD
		void IDebugManager.AbortGame()
		{
			MBDebug.AbortGame(5);
		}

		// Token: 0x06001918 RID: 6424 RVA: 0x0005ABB5 File Offset: 0x00058DB5
		void IDebugManager.DoDelayedexit(int returnCode)
		{
			Utilities.DoDelayedexit(returnCode);
		}

		// Token: 0x06001919 RID: 6425 RVA: 0x0005ABBD File Offset: 0x00058DBD
		void IDebugManager.ReportMemoryBookmark(string message)
		{
		}
	}
}
