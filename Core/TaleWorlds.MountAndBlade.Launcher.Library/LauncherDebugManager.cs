using System;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension.Standalone.Native.Windows;

namespace TaleWorlds.MountAndBlade.Launcher.Library
{
	public class LauncherDebugManager : IDebugManager
	{
		public LauncherDebugManager()
		{
			PlatformDirectoryPath platformDirectoryPath = new PlatformDirectoryPath(PlatformFileType.Application, "logs");
			this.TryDeletePreviousLogs(platformDirectoryPath);
			this._logFilePath = new PlatformFilePath(platformDirectoryPath, "launcher_log_" + new Random().Next(10000, 99999) + ".txt");
		}

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

		public void OnFinalize()
		{
		}

		private void AppendLineToLog(string message, bool forceSave = true)
		{
			FileHelper.AppendLineToFileString(this._logFilePath, message);
		}

		void IDebugManager.SetCrashReportCustomString(string customString)
		{
		}

		void IDebugManager.SetCrashReportCustomStack(string customStack)
		{
		}

		void IDebugManager.ShowError(string message)
		{
		}

		void IDebugManager.ShowWarning(string message)
		{
			this.AppendLineToLog(message, true);
		}

		void IDebugManager.ShowMessageBox(string lpText, string lpCaption, uint uType)
		{
			User32.MessageBox(IntPtr.Zero, lpText, lpCaption, 16U);
			this.AppendLineToLog(lpCaption, true);
			this.AppendLineToLog(lpText, true);
		}

		void IDebugManager.Assert(bool condition, string message, string callerFile, string callerMethod, int callerLine)
		{
			if (!condition)
			{
				this.AppendLineToLog("ASSERT!\n" + message, true);
			}
		}

		void IDebugManager.SilentAssert(bool condition, string message, bool getDump, string callerFile, string callerMethod, int callerLine)
		{
			if (!condition)
			{
				this.AppendLineToLog("ASSERT!\n" + message, true);
			}
		}

		void IDebugManager.Print(string message, int logLevel, Debug.DebugColor color, ulong debugFilter)
		{
			this.AppendLineToLog(message, true);
		}

		void IDebugManager.PrintError(string error, string stackTrace, ulong debugFilter)
		{
			this.AppendLineToLog("ERROR!\n" + error, true);
		}

		void IDebugManager.PrintWarning(string warning, ulong debugFilter)
		{
			this.AppendLineToLog("warning!\n" + warning, true);
		}

		void IDebugManager.DisplayDebugMessage(string message)
		{
			this.AppendLineToLog(message, true);
		}

		void IDebugManager.WatchVariable(string name, object value)
		{
		}

		void IDebugManager.WriteDebugLineOnScreen(string message)
		{
		}

		void IDebugManager.RenderDebugLine(Vec3 position, Vec3 direction, uint color, bool depthCheck, float time)
		{
		}

		void IDebugManager.RenderDebugSphere(Vec3 position, float radius, uint color, bool depthCheck, float time)
		{
		}

		void IDebugManager.RenderDebugFrame(MatrixFrame frame, float lineLength, float time)
		{
		}

		void IDebugManager.RenderDebugText(float screenX, float screenY, string text, uint color, float time)
		{
		}

		void IDebugManager.RenderDebugText3D(Vec3 position, string text, uint color, int screenPosOffsetX, int screenPosOffsetY, float time)
		{
		}

		void IDebugManager.RenderDebugRectWithColor(float left, float bottom, float right, float top, uint color)
		{
		}

		Vec3 IDebugManager.GetDebugVector()
		{
			return Vec3.Zero;
		}

		void IDebugManager.SetTestModeEnabled(bool testModeEnabled)
		{
		}

		void IDebugManager.AbortGame()
		{
		}

		void IDebugManager.DoDelayedexit(int returnCode)
		{
		}

		void IDebugManager.ReportMemoryBookmark(string message)
		{
		}

		private readonly PlatformFilePath _logFilePath;
	}
}
