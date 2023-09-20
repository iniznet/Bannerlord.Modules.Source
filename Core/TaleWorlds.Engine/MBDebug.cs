using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
	public static class MBDebug
	{
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

		[Conditional("_RGL_KEEP_ASSERTS")]
		public static void AssertMemoryUsage(int memoryMB)
		{
			EngineApplicationInterface.IDebug.AssertMemoryUsage(memoryMB);
		}

		public static void AbortGame(int ExitCode = 5)
		{
			EngineApplicationInterface.IDebug.AbortGame(ExitCode);
		}

		public static void ShowWarning(string message)
		{
			bool flag = EngineApplicationInterface.IDebug.Warning(message);
			if (Debugger.IsAttached && flag)
			{
				Debugger.Break();
			}
		}

		public static void ShowError(string message)
		{
			bool flag = EngineApplicationInterface.IDebug.Error(message);
			if (Debugger.IsAttached && flag)
			{
				Debugger.Break();
			}
		}

		public static void ShowMessageBox(string lpText, string lpCaption, uint uType)
		{
			EngineApplicationInterface.IDebug.MessageBox(lpText, lpCaption, uType);
		}

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

		[Conditional("_RGL_KEEP_ASSERTS")]
		public static void FailedAssert(string message, [CallerFilePath] string callerFile = "", [CallerMemberName] string callerMethod = "", [CallerLineNumber] int callerLine = 0)
		{
		}

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

		[Conditional("DEBUG_MORE")]
		public static void AssertConditionOrCallerClassName(bool condition, string name)
		{
			StackFrame frame = new StackTrace(2, true).GetFrame(0);
			if (!condition)
			{
				string name2 = frame.GetMethod().DeclaringType.Name;
			}
		}

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

		[Conditional("_RGL_KEEP_ASSERTS")]
		public static void WriteDebugLineOnScreen(string str)
		{
			EngineApplicationInterface.IDebug.WriteDebugLineOnScreen(str);
		}

		[Conditional("_RGL_KEEP_ASSERTS")]
		public static void RenderDebugText(float screenX, float screenY, string text, uint color = 4294967295U, float time = 0f)
		{
			EngineApplicationInterface.IDebug.RenderDebugText(screenX, screenY, text, color, time);
		}

		public static void RenderText(float screenX, float screenY, string text, uint color = 4294967295U, float time = 0f)
		{
			EngineApplicationInterface.IDebug.RenderDebugText(screenX, screenY, text, color, time);
		}

		[Conditional("_RGL_KEEP_ASSERTS")]
		public static void RenderDebugRect(float left, float bottom, float right, float top)
		{
			EngineApplicationInterface.IDebug.RenderDebugRect(left, bottom, right, top);
		}

		[Conditional("_RGL_KEEP_ASSERTS")]
		public static void RenderDebugRectWithColor(float left, float bottom, float right, float top, uint color = 4294967295U)
		{
			EngineApplicationInterface.IDebug.RenderDebugRectWithColor(left, bottom, right, top, color);
		}

		[Conditional("_RGL_KEEP_ASSERTS")]
		public static void RenderDebugFrame(MatrixFrame frame, float lineLength, float time = 0f)
		{
			EngineApplicationInterface.IDebug.RenderDebugFrame(ref frame, lineLength, time);
		}

		[Conditional("_RGL_KEEP_ASSERTS")]
		public static void RenderDebugText3D(Vec3 worldPosition, string str, uint color = 4294967295U, int screenPosOffsetX = 0, int screenPosOffsetY = 0, float time = 0f)
		{
			EngineApplicationInterface.IDebug.RenderDebugText3d(worldPosition, str, color, screenPosOffsetX, screenPosOffsetY, time);
		}

		[Conditional("_RGL_KEEP_ASSERTS")]
		public static void RenderDebugDirectionArrow(Vec3 position, Vec3 direction, uint color = 4294967295U, bool depthCheck = false)
		{
			EngineApplicationInterface.IDebug.RenderDebugDirectionArrow(position, direction, color, depthCheck);
		}

		[Conditional("_RGL_KEEP_ASSERTS")]
		public static void RenderDebugLine(Vec3 position, Vec3 direction, uint color = 4294967295U, bool depthCheck = false, float time = 0f)
		{
			EngineApplicationInterface.IDebug.RenderDebugLine(position, direction, color, depthCheck, time);
		}

		[Conditional("_RGL_KEEP_ASSERTS")]
		public static void RenderDebugSphere(Vec3 position, float radius, uint color = 4294967295U, bool depthCheck = false, float time = 0f)
		{
			EngineApplicationInterface.IDebug.RenderDebugSphere(position, radius, color, depthCheck, time);
		}

		[Conditional("_RGL_KEEP_ASSERTS")]
		public static void RenderDebugCapsule(Vec3 p0, Vec3 p1, float radius, uint color = 4294967295U, bool depthCheck = false, float time = 0f)
		{
			EngineApplicationInterface.IDebug.RenderDebugCapsule(p0, p1, radius, color, depthCheck, time);
		}

		[Conditional("_RGL_KEEP_ASSERTS")]
		public static void ClearRenderObjects()
		{
			EngineApplicationInterface.IDebug.ClearAllDebugRenderObjects();
		}

		public static Vec3 DebugVector
		{
			get
			{
				return EngineApplicationInterface.IDebug.GetDebugVector();
			}
		}

		[Conditional("_RGL_KEEP_ASSERTS")]
		public static void RenderDebugBoxObject(Vec3 min, Vec3 max, uint color = 4294967295U, bool depthCheck = false, float time = 0f)
		{
			EngineApplicationInterface.IDebug.RenderDebugBoxObject(min, max, color, depthCheck, time);
		}

		[Conditional("_RGL_KEEP_ASSERTS")]
		public static void RenderDebugBoxObject(Vec3 min, Vec3 max, MatrixFrame frame, uint color = 4294967295U, bool depthCheck = false, float time = 0f)
		{
			EngineApplicationInterface.IDebug.RenderDebugBoxObjectWithFrame(min, max, ref frame, color, depthCheck, time);
		}

		[Conditional("_RGL_KEEP_ASSERTS")]
		public static void PostWarningLine(string line)
		{
			EngineApplicationInterface.IDebug.PostWarningLine(line);
		}

		public static bool IsErrorReportModeActive()
		{
			return EngineApplicationInterface.IDebug.IsErrorReportModeActive();
		}

		public static bool IsErrorReportModePauseMission()
		{
			return EngineApplicationInterface.IDebug.IsErrorReportModePauseMission();
		}

		public static void SetErrorReportScene(Scene scene)
		{
			UIntPtr uintPtr = ((scene == null) ? UIntPtr.Zero : scene.Pointer);
			EngineApplicationInterface.IDebug.SetErrorReportScene(uintPtr);
		}

		public static void SetDumpGenerationDisabled(bool value)
		{
			EngineApplicationInterface.IDebug.SetDumpGenerationDisabled(value);
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("clear", "console")]
		public static string ClearConsole(List<string> strings)
		{
			Console.Clear();
			return "Debug console cleared.";
		}

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

		public static bool IsTestMode()
		{
			return EngineApplicationInterface.IDebug.IsTestMode();
		}

		public static bool DisableAllUI;

		public static bool TestModeEnabled;

		public static bool ShouldAssertThrowException;

		public static bool IsDisplayingHighLevelAI;

		public static bool DisableLogging;

		private static readonly Dictionary<string, int> ProcessedFrameList = new Dictionary<string, int>();

		[Flags]
		public enum MessageBoxTypeFlag
		{
			Ok = 1,
			Warning = 2,
			Error = 4,
			OkCancel = 8,
			RetryCancel = 16,
			YesNo = 32,
			YesNoCancel = 64,
			Information = 128,
			Exclamation = 256,
			Question = 512,
			AssertFailed = 1024
		}
	}
}
