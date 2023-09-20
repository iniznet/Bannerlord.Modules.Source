using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
	// Token: 0x0200005C RID: 92
	public static class MBDebug
	{
		// Token: 0x06000790 RID: 1936 RVA: 0x00006FED File Offset: 0x000051ED
		[CommandLineFunctionality.CommandLineArgumentFunction("toggle_ui", "ui")]
		public static string DisableUI(List<string> strings)
		{
			if (strings.Count != 0)
			{
				return "Invalid input.";
			}
			MBDebug.DisableAllUI = !MBDebug.DisableAllUI;
			if (MBDebug.DisableAllUI)
			{
				return "UI is now disabled.";
			}
			return "UI is now enabled.";
		}

		// Token: 0x06000792 RID: 1938 RVA: 0x00007028 File Offset: 0x00005228
		[Conditional("_RGL_KEEP_ASSERTS")]
		public static void AssertMemoryUsage(int memoryMB)
		{
			EngineApplicationInterface.IDebug.AssertMemoryUsage(memoryMB);
		}

		// Token: 0x06000793 RID: 1939 RVA: 0x00007035 File Offset: 0x00005235
		public static void AbortGame(int ExitCode = 5)
		{
			EngineApplicationInterface.IDebug.AbortGame(ExitCode);
		}

		// Token: 0x06000794 RID: 1940 RVA: 0x00007044 File Offset: 0x00005244
		public static void ShowWarning(string message)
		{
			bool flag = EngineApplicationInterface.IDebug.Warning(message);
			if (Debugger.IsAttached && flag)
			{
				Debugger.Break();
			}
		}

		// Token: 0x06000795 RID: 1941 RVA: 0x0000706C File Offset: 0x0000526C
		public static void ShowError(string message)
		{
			bool flag = EngineApplicationInterface.IDebug.Error(message);
			if (Debugger.IsAttached && flag)
			{
				Debugger.Break();
			}
		}

		// Token: 0x06000796 RID: 1942 RVA: 0x00007093 File Offset: 0x00005293
		public static void ShowMessageBox(string lpText, string lpCaption, uint uType)
		{
			EngineApplicationInterface.IDebug.MessageBox(lpText, lpCaption, uType);
		}

		// Token: 0x06000797 RID: 1943 RVA: 0x000070A4 File Offset: 0x000052A4
		[Conditional("_RGL_KEEP_ASSERTS")]
		public static void Assert(bool condition, string message, [CallerFilePath] string callerFile = "", [CallerMemberName] string callerMethod = "", [CallerLineNumber] int callerLine = 0)
		{
			if (!condition)
			{
				bool flag = EngineApplicationInterface.IDebug.FailedAssert(message, callerFile, callerMethod, callerLine);
				if (Debugger.IsAttached && flag)
				{
					Debugger.Break();
				}
			}
		}

		// Token: 0x06000798 RID: 1944 RVA: 0x000070D2 File Offset: 0x000052D2
		[Conditional("_RGL_KEEP_ASSERTS")]
		public static void FailedAssert(string message, [CallerFilePath] string callerFile = "", [CallerMemberName] string callerMethod = "", [CallerLineNumber] int callerLine = 0)
		{
		}

		// Token: 0x06000799 RID: 1945 RVA: 0x000070D4 File Offset: 0x000052D4
		public static void SilentAssert(bool condition, string message = "", bool getDump = false, [CallerFilePath] string callerFile = "", [CallerMemberName] string callerMethod = "", [CallerLineNumber] int callerLine = 0)
		{
			if (!condition)
			{
				bool flag = EngineApplicationInterface.IDebug.SilentAssert(message, callerFile, callerMethod, callerLine, getDump);
				if (Debugger.IsAttached && flag)
				{
					Debugger.Break();
				}
			}
		}

		// Token: 0x0600079A RID: 1946 RVA: 0x00007104 File Offset: 0x00005304
		[Conditional("DEBUG_MORE")]
		public static void AssertConditionOrCallerClassName(bool condition, string name)
		{
			StackFrame frame = new StackTrace(2, true).GetFrame(0);
			if (!condition)
			{
				string name2 = frame.GetMethod().DeclaringType.Name;
			}
		}

		// Token: 0x0600079B RID: 1947 RVA: 0x00007134 File Offset: 0x00005334
		[Conditional("DEBUG_MORE")]
		public static void AssertConditionOrCallerClassNameSearchAllCallstack(bool condition, string name)
		{
			StackTrace stackTrace = new StackTrace(true);
			if (!condition)
			{
				int num = 0;
				while (num < stackTrace.FrameCount && !(stackTrace.GetFrame(num).GetMethod().DeclaringType.Name == name))
				{
					num++;
				}
			}
		}

		// Token: 0x0600079C RID: 1948 RVA: 0x0000717C File Offset: 0x0000537C
		public static void Print(string message, int logLevel = 0, Debug.DebugColor color = Debug.DebugColor.White, ulong debugFilter = 17592186044416UL)
		{
			if (MBDebug.DisableLogging)
			{
				return;
			}
			debugFilter &= 18446744069414584320UL;
			if (debugFilter == 0UL)
			{
				return;
			}
			try
			{
				if (EngineApplicationInterface.IDebug != null)
				{
					EngineApplicationInterface.IDebug.WriteLine(logLevel, message, (int)color, debugFilter);
				}
			}
			catch
			{
			}
		}

		// Token: 0x0600079D RID: 1949 RVA: 0x000071D0 File Offset: 0x000053D0
		[Conditional("_RGL_KEEP_ASSERTS")]
		public static void ConsolePrint(string message, Debug.DebugColor color = Debug.DebugColor.White, ulong debugFilter = 17592186044416UL)
		{
			try
			{
				EngineApplicationInterface.IDebug.WriteLine(0, message, (int)color, debugFilter);
			}
			catch
			{
			}
		}

		// Token: 0x0600079E RID: 1950 RVA: 0x00007200 File Offset: 0x00005400
		[Conditional("_RGL_KEEP_ASSERTS")]
		public static void WriteDebugLineOnScreen(string str)
		{
			EngineApplicationInterface.IDebug.WriteDebugLineOnScreen(str);
		}

		// Token: 0x0600079F RID: 1951 RVA: 0x0000720D File Offset: 0x0000540D
		[Conditional("_RGL_KEEP_ASSERTS")]
		public static void RenderDebugText(float screenX, float screenY, string text, uint color = 4294967295U, float time = 0f)
		{
			EngineApplicationInterface.IDebug.RenderDebugText(screenX, screenY, text, color, time);
		}

		// Token: 0x060007A0 RID: 1952 RVA: 0x0000721F File Offset: 0x0000541F
		public static void RenderText(float screenX, float screenY, string text, uint color = 4294967295U, float time = 0f)
		{
			EngineApplicationInterface.IDebug.RenderDebugText(screenX, screenY, text, color, time);
		}

		// Token: 0x060007A1 RID: 1953 RVA: 0x00007231 File Offset: 0x00005431
		[Conditional("_RGL_KEEP_ASSERTS")]
		public static void RenderDebugRect(float left, float bottom, float right, float top)
		{
			EngineApplicationInterface.IDebug.RenderDebugRect(left, bottom, right, top);
		}

		// Token: 0x060007A2 RID: 1954 RVA: 0x00007241 File Offset: 0x00005441
		[Conditional("_RGL_KEEP_ASSERTS")]
		public static void RenderDebugRectWithColor(float left, float bottom, float right, float top, uint color = 4294967295U)
		{
			EngineApplicationInterface.IDebug.RenderDebugRectWithColor(left, bottom, right, top, color);
		}

		// Token: 0x060007A3 RID: 1955 RVA: 0x00007253 File Offset: 0x00005453
		[Conditional("_RGL_KEEP_ASSERTS")]
		public static void RenderDebugFrame(MatrixFrame frame, float lineLength, float time = 0f)
		{
			EngineApplicationInterface.IDebug.RenderDebugFrame(ref frame, lineLength, time);
		}

		// Token: 0x060007A4 RID: 1956 RVA: 0x00007263 File Offset: 0x00005463
		[Conditional("_RGL_KEEP_ASSERTS")]
		public static void RenderDebugText3D(Vec3 worldPosition, string str, uint color = 4294967295U, int screenPosOffsetX = 0, int screenPosOffsetY = 0, float time = 0f)
		{
			EngineApplicationInterface.IDebug.RenderDebugText3d(worldPosition, str, color, screenPosOffsetX, screenPosOffsetY, time);
		}

		// Token: 0x060007A5 RID: 1957 RVA: 0x00007277 File Offset: 0x00005477
		[Conditional("_RGL_KEEP_ASSERTS")]
		public static void RenderDebugDirectionArrow(Vec3 position, Vec3 direction, uint color = 4294967295U, bool depthCheck = false)
		{
			EngineApplicationInterface.IDebug.RenderDebugDirectionArrow(position, direction, color, depthCheck);
		}

		// Token: 0x060007A6 RID: 1958 RVA: 0x00007287 File Offset: 0x00005487
		[Conditional("_RGL_KEEP_ASSERTS")]
		public static void RenderDebugLine(Vec3 position, Vec3 direction, uint color = 4294967295U, bool depthCheck = false, float time = 0f)
		{
			EngineApplicationInterface.IDebug.RenderDebugLine(position, direction, color, depthCheck, time);
		}

		// Token: 0x060007A7 RID: 1959 RVA: 0x00007299 File Offset: 0x00005499
		[Conditional("_RGL_KEEP_ASSERTS")]
		public static void RenderDebugSphere(Vec3 position, float radius, uint color = 4294967295U, bool depthCheck = false, float time = 0f)
		{
			EngineApplicationInterface.IDebug.RenderDebugSphere(position, radius, color, depthCheck, time);
		}

		// Token: 0x060007A8 RID: 1960 RVA: 0x000072AB File Offset: 0x000054AB
		[Conditional("_RGL_KEEP_ASSERTS")]
		public static void RenderDebugCapsule(Vec3 p0, Vec3 p1, float radius, uint color = 4294967295U, bool depthCheck = false, float time = 0f)
		{
			EngineApplicationInterface.IDebug.RenderDebugCapsule(p0, p1, radius, color, depthCheck, time);
		}

		// Token: 0x060007A9 RID: 1961 RVA: 0x000072BF File Offset: 0x000054BF
		[Conditional("_RGL_KEEP_ASSERTS")]
		public static void ClearRenderObjects()
		{
			EngineApplicationInterface.IDebug.ClearAllDebugRenderObjects();
		}

		// Token: 0x17000043 RID: 67
		// (get) Token: 0x060007AA RID: 1962 RVA: 0x000072CB File Offset: 0x000054CB
		public static Vec3 DebugVector
		{
			get
			{
				return EngineApplicationInterface.IDebug.GetDebugVector();
			}
		}

		// Token: 0x060007AB RID: 1963 RVA: 0x000072D7 File Offset: 0x000054D7
		[Conditional("_RGL_KEEP_ASSERTS")]
		public static void RenderDebugBoxObject(Vec3 min, Vec3 max, uint color = 4294967295U, bool depthCheck = false, float time = 0f)
		{
			EngineApplicationInterface.IDebug.RenderDebugBoxObject(min, max, color, depthCheck, time);
		}

		// Token: 0x060007AC RID: 1964 RVA: 0x000072E9 File Offset: 0x000054E9
		[Conditional("_RGL_KEEP_ASSERTS")]
		public static void RenderDebugBoxObject(Vec3 min, Vec3 max, MatrixFrame frame, uint color = 4294967295U, bool depthCheck = false, float time = 0f)
		{
			EngineApplicationInterface.IDebug.RenderDebugBoxObjectWithFrame(min, max, ref frame, color, depthCheck, time);
		}

		// Token: 0x060007AD RID: 1965 RVA: 0x000072FE File Offset: 0x000054FE
		[Conditional("_RGL_KEEP_ASSERTS")]
		public static void PostWarningLine(string line)
		{
			EngineApplicationInterface.IDebug.PostWarningLine(line);
		}

		// Token: 0x060007AE RID: 1966 RVA: 0x0000730B File Offset: 0x0000550B
		public static bool IsErrorReportModeActive()
		{
			return EngineApplicationInterface.IDebug.IsErrorReportModeActive();
		}

		// Token: 0x060007AF RID: 1967 RVA: 0x00007317 File Offset: 0x00005517
		public static bool IsErrorReportModePauseMission()
		{
			return EngineApplicationInterface.IDebug.IsErrorReportModePauseMission();
		}

		// Token: 0x060007B0 RID: 1968 RVA: 0x00007324 File Offset: 0x00005524
		public static void SetErrorReportScene(Scene scene)
		{
			UIntPtr uintPtr = ((scene == null) ? UIntPtr.Zero : scene.Pointer);
			EngineApplicationInterface.IDebug.SetErrorReportScene(uintPtr);
		}

		// Token: 0x060007B1 RID: 1969 RVA: 0x00007353 File Offset: 0x00005553
		public static void SetDumpGenerationDisabled(bool value)
		{
			EngineApplicationInterface.IDebug.SetDumpGenerationDisabled(value);
		}

		// Token: 0x060007B2 RID: 1970 RVA: 0x00007360 File Offset: 0x00005560
		[CommandLineFunctionality.CommandLineArgumentFunction("clear", "console")]
		public static string ClearConsole(List<string> strings)
		{
			Console.Clear();
			return "Debug console cleared.";
		}

		// Token: 0x17000044 RID: 68
		// (get) Token: 0x060007B3 RID: 1971 RVA: 0x0000736C File Offset: 0x0000556C
		// (set) Token: 0x060007B4 RID: 1972 RVA: 0x00007378 File Offset: 0x00005578
		public static int ShowDebugInfoState
		{
			get
			{
				return EngineApplicationInterface.IDebug.GetShowDebugInfo();
			}
			set
			{
				EngineApplicationInterface.IDebug.SetShowDebugInfo(value);
			}
		}

		// Token: 0x060007B5 RID: 1973 RVA: 0x00007385 File Offset: 0x00005585
		public static bool IsTestMode()
		{
			return EngineApplicationInterface.IDebug.IsTestMode();
		}

		// Token: 0x040000F8 RID: 248
		public static bool DisableAllUI;

		// Token: 0x040000F9 RID: 249
		public static bool TestModeEnabled;

		// Token: 0x040000FA RID: 250
		public static bool ShouldAssertThrowException;

		// Token: 0x040000FB RID: 251
		public static bool IsDisplayingHighLevelAI;

		// Token: 0x040000FC RID: 252
		public static bool DisableLogging;

		// Token: 0x040000FD RID: 253
		private static readonly Dictionary<string, int> ProcessedFrameList = new Dictionary<string, int>();

		// Token: 0x020000BB RID: 187
		[Flags]
		public enum MessageBoxTypeFlag
		{
			// Token: 0x040003E3 RID: 995
			Ok = 1,
			// Token: 0x040003E4 RID: 996
			Warning = 2,
			// Token: 0x040003E5 RID: 997
			Error = 4,
			// Token: 0x040003E6 RID: 998
			OkCancel = 8,
			// Token: 0x040003E7 RID: 999
			RetryCancel = 16,
			// Token: 0x040003E8 RID: 1000
			YesNo = 32,
			// Token: 0x040003E9 RID: 1001
			YesNoCancel = 64,
			// Token: 0x040003EA RID: 1002
			Information = 128,
			// Token: 0x040003EB RID: 1003
			Exclamation = 256,
			// Token: 0x040003EC RID: 1004
			Question = 512,
			// Token: 0x040003ED RID: 1005
			AssertFailed = 1024
		}
	}
}
