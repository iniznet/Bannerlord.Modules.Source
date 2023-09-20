using System;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension.Standalone.Native.Windows;

namespace TaleWorlds.MountAndBlade.Launcher.Library
{
	// Token: 0x02000002 RID: 2
	public class LauncherDebugManager : IDebugManager
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002048 File Offset: 0x00000248
		public LauncherDebugManager()
		{
			PlatformDirectoryPath platformDirectoryPath = new PlatformDirectoryPath(PlatformFileType.Application, "logs");
			this.TryDeletePreviousLogs(platformDirectoryPath);
			this._logFilePath = new PlatformFilePath(platformDirectoryPath, "launcher_log_" + new Random().Next(10000, 99999) + ".txt");
		}

		// Token: 0x06000002 RID: 2 RVA: 0x000020A4 File Offset: 0x000002A4
		private void TryDeletePreviousLogs(PlatformDirectoryPath directoryPath)
		{
			PlatformFilePath[] array = new PlatformFilePath[0];
			try
			{
				array = FileHelper.GetFiles(directoryPath, "launcher_log_*.txt");
			}
			catch (Exception)
			{
				return;
			}
			for (int i = 0; i < array.Length; i++)
			{
				try
				{
					FileHelper.DeleteFile(array[i]);
				}
				catch (Exception)
				{
				}
			}
		}

		// Token: 0x06000003 RID: 3 RVA: 0x00002108 File Offset: 0x00000308
		public void OnFinalize()
		{
		}

		// Token: 0x06000004 RID: 4 RVA: 0x0000210A File Offset: 0x0000030A
		private void AppendLineToLog(string message, bool forceSave = true)
		{
			FileHelper.AppendLineToFileString(this._logFilePath, message);
		}

		// Token: 0x06000005 RID: 5 RVA: 0x00002119 File Offset: 0x00000319
		void IDebugManager.SetCrashReportCustomString(string customString)
		{
		}

		// Token: 0x06000006 RID: 6 RVA: 0x0000211B File Offset: 0x0000031B
		void IDebugManager.SetCrashReportCustomStack(string customStack)
		{
		}

		// Token: 0x06000007 RID: 7 RVA: 0x0000211D File Offset: 0x0000031D
		void IDebugManager.ShowError(string message)
		{
		}

		// Token: 0x06000008 RID: 8 RVA: 0x0000211F File Offset: 0x0000031F
		void IDebugManager.ShowWarning(string message)
		{
			this.AppendLineToLog(message, true);
		}

		// Token: 0x06000009 RID: 9 RVA: 0x00002129 File Offset: 0x00000329
		void IDebugManager.ShowMessageBox(string lpText, string lpCaption, uint uType)
		{
			User32.MessageBox(IntPtr.Zero, lpText, lpCaption, 16U);
			this.AppendLineToLog(lpCaption, true);
			this.AppendLineToLog(lpText, true);
		}

		// Token: 0x0600000A RID: 10 RVA: 0x0000214A File Offset: 0x0000034A
		void IDebugManager.Assert(bool condition, string message, string callerFile, string callerMethod, int callerLine)
		{
			if (!condition)
			{
				this.AppendLineToLog("ASSERT!\n" + message, true);
			}
		}

		// Token: 0x0600000B RID: 11 RVA: 0x00002161 File Offset: 0x00000361
		void IDebugManager.SilentAssert(bool condition, string message, bool getDump, string callerFile, string callerMethod, int callerLine)
		{
			if (!condition)
			{
				this.AppendLineToLog("ASSERT!\n" + message, true);
			}
		}

		// Token: 0x0600000C RID: 12 RVA: 0x00002178 File Offset: 0x00000378
		void IDebugManager.Print(string message, int logLevel, Debug.DebugColor color, ulong debugFilter)
		{
			this.AppendLineToLog(message, true);
		}

		// Token: 0x0600000D RID: 13 RVA: 0x00002182 File Offset: 0x00000382
		void IDebugManager.PrintError(string error, string stackTrace, ulong debugFilter)
		{
			this.AppendLineToLog("ERROR!\n" + error, true);
		}

		// Token: 0x0600000E RID: 14 RVA: 0x00002196 File Offset: 0x00000396
		void IDebugManager.PrintWarning(string warning, ulong debugFilter)
		{
			this.AppendLineToLog("warning!\n" + warning, true);
		}

		// Token: 0x0600000F RID: 15 RVA: 0x000021AA File Offset: 0x000003AA
		void IDebugManager.DisplayDebugMessage(string message)
		{
			this.AppendLineToLog(message, true);
		}

		// Token: 0x06000010 RID: 16 RVA: 0x000021B4 File Offset: 0x000003B4
		void IDebugManager.WatchVariable(string name, object value)
		{
		}

		// Token: 0x06000011 RID: 17 RVA: 0x000021B6 File Offset: 0x000003B6
		void IDebugManager.WriteDebugLineOnScreen(string message)
		{
		}

		// Token: 0x06000012 RID: 18 RVA: 0x000021B8 File Offset: 0x000003B8
		void IDebugManager.RenderDebugLine(Vec3 position, Vec3 direction, uint color, bool depthCheck, float time)
		{
		}

		// Token: 0x06000013 RID: 19 RVA: 0x000021BA File Offset: 0x000003BA
		void IDebugManager.RenderDebugSphere(Vec3 position, float radius, uint color, bool depthCheck, float time)
		{
		}

		// Token: 0x06000014 RID: 20 RVA: 0x000021BC File Offset: 0x000003BC
		void IDebugManager.RenderDebugFrame(MatrixFrame frame, float lineLength, float time)
		{
		}

		// Token: 0x06000015 RID: 21 RVA: 0x000021BE File Offset: 0x000003BE
		void IDebugManager.RenderDebugText(float screenX, float screenY, string text, uint color, float time)
		{
		}

		// Token: 0x06000016 RID: 22 RVA: 0x000021C0 File Offset: 0x000003C0
		void IDebugManager.RenderDebugText3D(Vec3 position, string text, uint color, int screenPosOffsetX, int screenPosOffsetY, float time)
		{
		}

		// Token: 0x06000017 RID: 23 RVA: 0x000021C2 File Offset: 0x000003C2
		void IDebugManager.RenderDebugRectWithColor(float left, float bottom, float right, float top, uint color)
		{
		}

		// Token: 0x06000018 RID: 24 RVA: 0x000021C4 File Offset: 0x000003C4
		Vec3 IDebugManager.GetDebugVector()
		{
			return Vec3.Zero;
		}

		// Token: 0x06000019 RID: 25 RVA: 0x000021CB File Offset: 0x000003CB
		void IDebugManager.SetTestModeEnabled(bool testModeEnabled)
		{
		}

		// Token: 0x0600001A RID: 26 RVA: 0x000021CD File Offset: 0x000003CD
		void IDebugManager.AbortGame()
		{
		}

		// Token: 0x0600001B RID: 27 RVA: 0x000021CF File Offset: 0x000003CF
		void IDebugManager.DoDelayedexit(int returnCode)
		{
		}

		// Token: 0x0600001C RID: 28 RVA: 0x000021D1 File Offset: 0x000003D1
		void IDebugManager.ReportMemoryBookmark(string message)
		{
		}

		// Token: 0x04000001 RID: 1
		private readonly PlatformFilePath _logFilePath;
	}
}
