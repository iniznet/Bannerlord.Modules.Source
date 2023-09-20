using System;

namespace TaleWorlds.Library
{
	public class HTMLDebugManager : IDebugManager
	{
		public HTMLDebugManager(int numFiles = 1, int totalFileSize = -1)
		{
			HTMLDebugManager._mainLogger = new Logger("__global", true, false, false, numFiles, totalFileSize, false);
		}

		public static bool LogOnlyErrors
		{
			get
			{
				return HTMLDebugManager._mainLogger.LogOnlyErrors;
			}
			set
			{
				HTMLDebugManager._mainLogger.LogOnlyErrors = value;
			}
		}

		void IDebugManager.SetCrashReportCustomString(string customString)
		{
		}

		void IDebugManager.SetCrashReportCustomStack(string customStack)
		{
		}

		void IDebugManager.ShowMessageBox(string lpText, string lpCaption, uint uType)
		{
		}

		void IDebugManager.ShowError(string message)
		{
			HTMLDebugManager._mainLogger.Print(message, HTMLDebugCategory.Error, false);
		}

		void IDebugManager.ShowWarning(string message)
		{
			HTMLDebugManager._mainLogger.Print(message, HTMLDebugCategory.Warning, false);
		}

		void IDebugManager.Assert(bool condition, string message, string callerFile, string callerMethod, int callerLine)
		{
			this.Assert(condition, message, callerFile, callerMethod, callerLine);
		}

		void IDebugManager.SilentAssert(bool condition, string message, bool getDump, string callerFile, string callerMethod, int callerLine)
		{
			this.SilentAssert(condition, message, getDump, callerFile, callerMethod, callerLine);
		}

		void IDebugManager.Print(string message, int logLevel, Debug.DebugColor color, ulong debugFilter)
		{
			HTMLDebugManager._mainLogger.Print(message, HTMLDebugCategory.General, false);
		}

		void IDebugManager.PrintError(string error, string stackTrace, ulong debugFilter)
		{
			HTMLDebugManager._mainLogger.Print(error, HTMLDebugCategory.Error, false);
		}

		void IDebugManager.PrintWarning(string warning, ulong debugFilter)
		{
			HTMLDebugManager._mainLogger.Print(warning, HTMLDebugCategory.Warning, false);
		}

		void IDebugManager.DisplayDebugMessage(string message)
		{
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
			this._testModeEnabled = testModeEnabled;
		}

		void IDebugManager.AbortGame()
		{
			Environment.Exit(-5);
		}

		void IDebugManager.DoDelayedexit(int returnCode)
		{
		}

		protected void PrintMessage(string message, HTMLDebugCategory debugCategory, bool printOnGlobal)
		{
			HTMLDebugManager._mainLogger.Print(message, debugCategory, printOnGlobal);
		}

		protected virtual void Assert(bool condition, string message, string callerFile, string callerMethod, int callerLine)
		{
			if (!condition)
			{
				HTMLDebugManager._mainLogger.Print(message, HTMLDebugCategory.Error, false);
			}
		}

		protected virtual void SilentAssert(bool condition, string message, bool getDump, string callerFile, string callerMethod, int callerLine)
		{
			this.Assert(condition, message, callerFile, callerMethod, callerLine);
		}

		void IDebugManager.ReportMemoryBookmark(string message)
		{
		}

		private static Logger _mainLogger;

		private bool _testModeEnabled;
	}
}
