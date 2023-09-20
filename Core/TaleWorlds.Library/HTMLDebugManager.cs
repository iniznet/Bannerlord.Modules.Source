using System;

namespace TaleWorlds.Library
{
	// Token: 0x02000030 RID: 48
	public class HTMLDebugManager : IDebugManager
	{
		// Token: 0x06000172 RID: 370 RVA: 0x00005FDC File Offset: 0x000041DC
		public HTMLDebugManager(int numFiles = 1, int totalFileSize = -1)
		{
			HTMLDebugManager._mainLogger = new Logger("__global", true, false, false, numFiles, totalFileSize, false);
		}

		// Token: 0x1700001E RID: 30
		// (get) Token: 0x06000173 RID: 371 RVA: 0x00005FF9 File Offset: 0x000041F9
		// (set) Token: 0x06000174 RID: 372 RVA: 0x00006005 File Offset: 0x00004205
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

		// Token: 0x06000175 RID: 373 RVA: 0x00006012 File Offset: 0x00004212
		void IDebugManager.SetCrashReportCustomString(string customString)
		{
		}

		// Token: 0x06000176 RID: 374 RVA: 0x00006014 File Offset: 0x00004214
		void IDebugManager.SetCrashReportCustomStack(string customStack)
		{
		}

		// Token: 0x06000177 RID: 375 RVA: 0x00006016 File Offset: 0x00004216
		void IDebugManager.ShowMessageBox(string lpText, string lpCaption, uint uType)
		{
		}

		// Token: 0x06000178 RID: 376 RVA: 0x00006018 File Offset: 0x00004218
		void IDebugManager.ShowError(string message)
		{
			HTMLDebugManager._mainLogger.Print(message, HTMLDebugCategory.Error, false);
		}

		// Token: 0x06000179 RID: 377 RVA: 0x00006027 File Offset: 0x00004227
		void IDebugManager.ShowWarning(string message)
		{
			HTMLDebugManager._mainLogger.Print(message, HTMLDebugCategory.Warning, false);
		}

		// Token: 0x0600017A RID: 378 RVA: 0x00006036 File Offset: 0x00004236
		void IDebugManager.Assert(bool condition, string message, string callerFile, string callerMethod, int callerLine)
		{
			this.Assert(condition, message, callerFile, callerMethod, callerLine);
		}

		// Token: 0x0600017B RID: 379 RVA: 0x00006045 File Offset: 0x00004245
		void IDebugManager.SilentAssert(bool condition, string message, bool getDump, string callerFile, string callerMethod, int callerLine)
		{
			this.SilentAssert(condition, message, getDump, callerFile, callerMethod, callerLine);
		}

		// Token: 0x0600017C RID: 380 RVA: 0x00006056 File Offset: 0x00004256
		void IDebugManager.Print(string message, int logLevel, Debug.DebugColor color, ulong debugFilter)
		{
			HTMLDebugManager._mainLogger.Print(message, HTMLDebugCategory.General, false);
		}

		// Token: 0x0600017D RID: 381 RVA: 0x00006065 File Offset: 0x00004265
		void IDebugManager.PrintError(string error, string stackTrace, ulong debugFilter)
		{
			HTMLDebugManager._mainLogger.Print(error, HTMLDebugCategory.Error, false);
		}

		// Token: 0x0600017E RID: 382 RVA: 0x00006074 File Offset: 0x00004274
		void IDebugManager.PrintWarning(string warning, ulong debugFilter)
		{
			HTMLDebugManager._mainLogger.Print(warning, HTMLDebugCategory.Warning, false);
		}

		// Token: 0x0600017F RID: 383 RVA: 0x00006083 File Offset: 0x00004283
		void IDebugManager.DisplayDebugMessage(string message)
		{
		}

		// Token: 0x06000180 RID: 384 RVA: 0x00006085 File Offset: 0x00004285
		void IDebugManager.WatchVariable(string name, object value)
		{
		}

		// Token: 0x06000181 RID: 385 RVA: 0x00006087 File Offset: 0x00004287
		void IDebugManager.WriteDebugLineOnScreen(string message)
		{
		}

		// Token: 0x06000182 RID: 386 RVA: 0x00006089 File Offset: 0x00004289
		void IDebugManager.RenderDebugLine(Vec3 position, Vec3 direction, uint color, bool depthCheck, float time)
		{
		}

		// Token: 0x06000183 RID: 387 RVA: 0x0000608B File Offset: 0x0000428B
		void IDebugManager.RenderDebugSphere(Vec3 position, float radius, uint color, bool depthCheck, float time)
		{
		}

		// Token: 0x06000184 RID: 388 RVA: 0x0000608D File Offset: 0x0000428D
		void IDebugManager.RenderDebugFrame(MatrixFrame frame, float lineLength, float time)
		{
		}

		// Token: 0x06000185 RID: 389 RVA: 0x0000608F File Offset: 0x0000428F
		void IDebugManager.RenderDebugText(float screenX, float screenY, string text, uint color, float time)
		{
		}

		// Token: 0x06000186 RID: 390 RVA: 0x00006091 File Offset: 0x00004291
		void IDebugManager.RenderDebugText3D(Vec3 position, string text, uint color, int screenPosOffsetX, int screenPosOffsetY, float time)
		{
		}

		// Token: 0x06000187 RID: 391 RVA: 0x00006093 File Offset: 0x00004293
		void IDebugManager.RenderDebugRectWithColor(float left, float bottom, float right, float top, uint color)
		{
		}

		// Token: 0x06000188 RID: 392 RVA: 0x00006095 File Offset: 0x00004295
		Vec3 IDebugManager.GetDebugVector()
		{
			return Vec3.Zero;
		}

		// Token: 0x06000189 RID: 393 RVA: 0x0000609C File Offset: 0x0000429C
		void IDebugManager.SetTestModeEnabled(bool testModeEnabled)
		{
			this._testModeEnabled = testModeEnabled;
		}

		// Token: 0x0600018A RID: 394 RVA: 0x000060A5 File Offset: 0x000042A5
		void IDebugManager.AbortGame()
		{
			Environment.Exit(-5);
		}

		// Token: 0x0600018B RID: 395 RVA: 0x000060AE File Offset: 0x000042AE
		void IDebugManager.DoDelayedexit(int returnCode)
		{
		}

		// Token: 0x0600018C RID: 396 RVA: 0x000060B0 File Offset: 0x000042B0
		protected void PrintMessage(string message, HTMLDebugCategory debugCategory, bool printOnGlobal)
		{
			HTMLDebugManager._mainLogger.Print(message, debugCategory, printOnGlobal);
		}

		// Token: 0x0600018D RID: 397 RVA: 0x000060BF File Offset: 0x000042BF
		protected virtual void Assert(bool condition, string message, string callerFile, string callerMethod, int callerLine)
		{
			if (!condition)
			{
				HTMLDebugManager._mainLogger.Print(message, HTMLDebugCategory.Error, false);
			}
		}

		// Token: 0x0600018E RID: 398 RVA: 0x000060D1 File Offset: 0x000042D1
		protected virtual void SilentAssert(bool condition, string message, bool getDump, string callerFile, string callerMethod, int callerLine)
		{
			this.Assert(condition, message, callerFile, callerMethod, callerLine);
		}

		// Token: 0x0600018F RID: 399 RVA: 0x000060E1 File Offset: 0x000042E1
		void IDebugManager.ReportMemoryBookmark(string message)
		{
		}

		// Token: 0x04000084 RID: 132
		private static Logger _mainLogger;

		// Token: 0x04000085 RID: 133
		private bool _testModeEnabled;
	}
}
