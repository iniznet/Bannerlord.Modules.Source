using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace TaleWorlds.Library
{
	// Token: 0x02000026 RID: 38
	public static class Debug
	{
		// Token: 0x14000001 RID: 1
		// (add) Token: 0x060000F3 RID: 243 RVA: 0x00005484 File Offset: 0x00003684
		// (remove) Token: 0x060000F4 RID: 244 RVA: 0x000054B8 File Offset: 0x000036B8
		public static event Action<string, ulong> OnPrint;

		// Token: 0x17000019 RID: 25
		// (get) Token: 0x060000F5 RID: 245 RVA: 0x000054EB File Offset: 0x000036EB
		// (set) Token: 0x060000F6 RID: 246 RVA: 0x000054F2 File Offset: 0x000036F2
		public static IDebugManager DebugManager { get; set; }

		// Token: 0x1700001A RID: 26
		// (get) Token: 0x060000F7 RID: 247 RVA: 0x000054FA File Offset: 0x000036FA
		// (set) Token: 0x060000F8 RID: 248 RVA: 0x00005501 File Offset: 0x00003701
		public static ITelemetryManager TelemetryManager { get; set; }

		// Token: 0x060000F9 RID: 249 RVA: 0x00005509 File Offset: 0x00003709
		public static uint GetTelemetryLevelMask()
		{
			ITelemetryManager telemetryManager = Debug.TelemetryManager;
			if (telemetryManager == null)
			{
				return 4096U;
			}
			return telemetryManager.GetTelemetryLevelMask();
		}

		// Token: 0x060000FA RID: 250 RVA: 0x0000551F File Offset: 0x0000371F
		public static void SetCrashReportCustomString(string customString)
		{
			if (Debug.DebugManager != null)
			{
				Debug.DebugManager.SetCrashReportCustomString(customString);
			}
		}

		// Token: 0x060000FB RID: 251 RVA: 0x00005533 File Offset: 0x00003733
		public static void SetCrashReportCustomStack(string customStack)
		{
			if (Debug.DebugManager != null)
			{
				Debug.DebugManager.SetCrashReportCustomStack(customStack);
			}
		}

		// Token: 0x060000FC RID: 252 RVA: 0x00005547 File Offset: 0x00003747
		[Conditional("_RGL_KEEP_ASSERTS")]
		public static void Assert(bool condition, string message, [CallerFilePath] string callerFile = "", [CallerMemberName] string callerMethod = "", [CallerLineNumber] int callerLine = 0)
		{
			if (Debug.DebugManager != null)
			{
				Debug.DebugManager.Assert(condition, message, callerFile, callerMethod, callerLine);
			}
		}

		// Token: 0x060000FD RID: 253 RVA: 0x00005560 File Offset: 0x00003760
		public static void FailedAssert(string message, [CallerFilePath] string callerFile = "", [CallerMemberName] string callerMethod = "", [CallerLineNumber] int callerLine = 0)
		{
			if (Debug.DebugManager != null)
			{
				Debug.DebugManager.Assert(false, message, callerFile, callerMethod, callerLine);
			}
		}

		// Token: 0x060000FE RID: 254 RVA: 0x00005578 File Offset: 0x00003778
		public static void SilentAssert(bool condition, string message = "", bool getDump = false, [CallerFilePath] string callerFile = "", [CallerMemberName] string callerMethod = "", [CallerLineNumber] int callerLine = 0)
		{
			if (Debug.DebugManager != null)
			{
				Debug.DebugManager.SilentAssert(condition, message, getDump, callerFile, callerMethod, callerLine);
			}
		}

		// Token: 0x060000FF RID: 255 RVA: 0x00005593 File Offset: 0x00003793
		public static void ShowError(string message)
		{
			if (Debug.DebugManager != null)
			{
				Debug.DebugManager.ShowError(message);
			}
		}

		// Token: 0x06000100 RID: 256 RVA: 0x000055A7 File Offset: 0x000037A7
		internal static void DoDelayedexit(int returnCode)
		{
			if (Debug.DebugManager != null)
			{
				Debug.DebugManager.DoDelayedexit(returnCode);
			}
		}

		// Token: 0x06000101 RID: 257 RVA: 0x000055BB File Offset: 0x000037BB
		[Conditional("_RGL_KEEP_ASSERTS")]
		public static void ShowWarning(string message)
		{
			if (Debug.DebugManager != null)
			{
				Debug.DebugManager.ShowWarning(message);
			}
		}

		// Token: 0x06000102 RID: 258 RVA: 0x000055CF File Offset: 0x000037CF
		public static void ReportMemoryBookmark(string message)
		{
			IDebugManager debugManager = Debug.DebugManager;
			if (debugManager == null)
			{
				return;
			}
			debugManager.ReportMemoryBookmark(message);
		}

		// Token: 0x06000103 RID: 259 RVA: 0x000055E1 File Offset: 0x000037E1
		public static void Print(string message, int logLevel = 0, Debug.DebugColor color = Debug.DebugColor.White, ulong debugFilter = 17592186044416UL)
		{
			if (Debug.DebugManager != null)
			{
				debugFilter &= 18446744069414584320UL;
				if (debugFilter == 0UL)
				{
					return;
				}
				Debug.DebugManager.Print(message, logLevel, color, debugFilter);
				Action<string, ulong> onPrint = Debug.OnPrint;
				if (onPrint == null)
				{
					return;
				}
				onPrint(message, debugFilter);
			}
		}

		// Token: 0x06000104 RID: 260 RVA: 0x0000561A File Offset: 0x0000381A
		public static void ShowMessageBox(string lpText, string lpCaption, uint uType)
		{
			if (Debug.DebugManager != null)
			{
				Debug.DebugManager.ShowMessageBox(lpText, lpCaption, uType);
			}
		}

		// Token: 0x06000105 RID: 261 RVA: 0x00005630 File Offset: 0x00003830
		[Conditional("_RGL_KEEP_ASSERTS")]
		public static void PrintWarning(string warning, ulong debugFilter = 17592186044416UL)
		{
			if (Debug.DebugManager != null)
			{
				Debug.DebugManager.PrintWarning(warning, debugFilter);
			}
		}

		// Token: 0x06000106 RID: 262 RVA: 0x00005645 File Offset: 0x00003845
		[Conditional("_RGL_KEEP_ASSERTS")]
		public static void PrintError(string error, string stackTrace = null, ulong debugFilter = 17592186044416UL)
		{
			if (Debug.DebugManager != null)
			{
				Debug.DebugManager.PrintError(error, stackTrace, debugFilter);
			}
		}

		// Token: 0x06000107 RID: 263 RVA: 0x0000565B File Offset: 0x0000385B
		[Conditional("_RGL_KEEP_ASSERTS")]
		public static void DisplayDebugMessage(string message)
		{
			if (Debug.DebugManager != null)
			{
				Debug.DebugManager.DisplayDebugMessage(message);
			}
		}

		// Token: 0x06000108 RID: 264 RVA: 0x0000566F File Offset: 0x0000386F
		[Conditional("_RGL_KEEP_ASSERTS")]
		public static void WatchVariable(string name, object value)
		{
			IDebugManager debugManager = Debug.DebugManager;
			if (debugManager == null)
			{
				return;
			}
			debugManager.WatchVariable(name, value);
		}

		// Token: 0x06000109 RID: 265 RVA: 0x00005682 File Offset: 0x00003882
		[Conditional("NOT_SHIPPING")]
		[Conditional("ENABLE_PROFILING_APIS_IN_SHIPPING")]
		public static void StartTelemetryConnection(bool showErrors)
		{
			ITelemetryManager telemetryManager = Debug.TelemetryManager;
			if (telemetryManager == null)
			{
				return;
			}
			telemetryManager.StartTelemetryConnection(showErrors);
		}

		// Token: 0x0600010A RID: 266 RVA: 0x00005694 File Offset: 0x00003894
		[Conditional("NOT_SHIPPING")]
		[Conditional("ENABLE_PROFILING_APIS_IN_SHIPPING")]
		public static void StopTelemetryConnection()
		{
			ITelemetryManager telemetryManager = Debug.TelemetryManager;
			if (telemetryManager == null)
			{
				return;
			}
			telemetryManager.StopTelemetryConnection();
		}

		// Token: 0x0600010B RID: 267 RVA: 0x000056A5 File Offset: 0x000038A5
		[Conditional("NOT_SHIPPING")]
		[Conditional("ENABLE_PROFILING_APIS_IN_SHIPPING")]
		internal static void BeginTelemetryScopeInternal(TelemetryLevelMask levelMask, string scopeName)
		{
			ITelemetryManager telemetryManager = Debug.TelemetryManager;
			if (telemetryManager == null)
			{
				return;
			}
			telemetryManager.BeginTelemetryScopeInternal(levelMask, scopeName);
		}

		// Token: 0x0600010C RID: 268 RVA: 0x000056B8 File Offset: 0x000038B8
		[Conditional("NOT_SHIPPING")]
		[Conditional("ENABLE_PROFILING_APIS_IN_SHIPPING")]
		internal static void BeginTelemetryScopeBaseLevelInternal(TelemetryLevelMask levelMask, string scopeName)
		{
			ITelemetryManager telemetryManager = Debug.TelemetryManager;
			if (telemetryManager == null)
			{
				return;
			}
			telemetryManager.BeginTelemetryScopeBaseLevelInternal(levelMask, scopeName);
		}

		// Token: 0x0600010D RID: 269 RVA: 0x000056CB File Offset: 0x000038CB
		[Conditional("NOT_SHIPPING")]
		[Conditional("ENABLE_PROFILING_APIS_IN_SHIPPING")]
		internal static void EndTelemetryScopeInternal()
		{
			ITelemetryManager telemetryManager = Debug.TelemetryManager;
			if (telemetryManager == null)
			{
				return;
			}
			telemetryManager.EndTelemetryScopeInternal();
		}

		// Token: 0x0600010E RID: 270 RVA: 0x000056DC File Offset: 0x000038DC
		[Conditional("NOT_SHIPPING")]
		[Conditional("ENABLE_PROFILING_APIS_IN_SHIPPING")]
		internal static void EndTelemetryScopeBaseLevelInternal()
		{
			ITelemetryManager telemetryManager = Debug.TelemetryManager;
			if (telemetryManager == null)
			{
				return;
			}
			telemetryManager.EndTelemetryScopeBaseLevelInternal();
		}

		// Token: 0x0600010F RID: 271 RVA: 0x000056ED File Offset: 0x000038ED
		[Conditional("_RGL_KEEP_ASSERTS")]
		public static void WriteDebugLineOnScreen(string message)
		{
			IDebugManager debugManager = Debug.DebugManager;
			if (debugManager == null)
			{
				return;
			}
			debugManager.WriteDebugLineOnScreen(message);
		}

		// Token: 0x06000110 RID: 272 RVA: 0x000056FF File Offset: 0x000038FF
		[Conditional("_RGL_KEEP_ASSERTS")]
		public static void RenderDebugLine(Vec3 position, Vec3 direction, uint color = 4294967295U, bool depthCheck = false, float time = 0f)
		{
			IDebugManager debugManager = Debug.DebugManager;
			if (debugManager == null)
			{
				return;
			}
			debugManager.RenderDebugLine(position, direction, color, depthCheck, time);
		}

		// Token: 0x06000111 RID: 273 RVA: 0x00005716 File Offset: 0x00003916
		[Conditional("_RGL_KEEP_ASSERTS")]
		public static void RenderDebugSphere(Vec3 position, float radius, uint color = 4294967295U, bool depthCheck = false, float time = 0f)
		{
			IDebugManager debugManager = Debug.DebugManager;
			if (debugManager == null)
			{
				return;
			}
			debugManager.RenderDebugSphere(position, radius, color, depthCheck, time);
		}

		// Token: 0x06000112 RID: 274 RVA: 0x0000572D File Offset: 0x0000392D
		[Conditional("_RGL_KEEP_ASSERTS")]
		public static void RenderDebugFrame(MatrixFrame frame, float lineLength, float time = 0f)
		{
			IDebugManager debugManager = Debug.DebugManager;
			if (debugManager == null)
			{
				return;
			}
			debugManager.RenderDebugFrame(frame, lineLength, time);
		}

		// Token: 0x06000113 RID: 275 RVA: 0x00005741 File Offset: 0x00003941
		[Conditional("_RGL_KEEP_ASSERTS")]
		public static void RenderDebugText(float screenX, float screenY, string text, uint color = 4294967295U, float time = 0f)
		{
			IDebugManager debugManager = Debug.DebugManager;
			if (debugManager == null)
			{
				return;
			}
			debugManager.RenderDebugText(screenX, screenY, text, color, time);
		}

		// Token: 0x06000114 RID: 276 RVA: 0x00005758 File Offset: 0x00003958
		[Conditional("_RGL_KEEP_ASSERTS")]
		public static void RenderDebugRectWithColor(float left, float bottom, float right, float top, uint color = 4294967295U)
		{
			IDebugManager debugManager = Debug.DebugManager;
			if (debugManager == null)
			{
				return;
			}
			debugManager.RenderDebugRectWithColor(left, bottom, right, top, color);
		}

		// Token: 0x06000115 RID: 277 RVA: 0x0000576F File Offset: 0x0000396F
		[Conditional("_RGL_KEEP_ASSERTS")]
		public static void RenderDebugText3D(Vec3 position, string text, uint color = 4294967295U, int screenPosOffsetX = 0, int screenPosOffsetY = 0, float time = 0f)
		{
			IDebugManager debugManager = Debug.DebugManager;
			if (debugManager == null)
			{
				return;
			}
			debugManager.RenderDebugText3D(position, text, color, screenPosOffsetX, screenPosOffsetY, time);
		}

		// Token: 0x06000116 RID: 278 RVA: 0x00005788 File Offset: 0x00003988
		public static Vec3 GetDebugVector()
		{
			IDebugManager debugManager = Debug.DebugManager;
			if (debugManager == null)
			{
				return Vec3.Zero;
			}
			return debugManager.GetDebugVector();
		}

		// Token: 0x06000117 RID: 279 RVA: 0x0000579E File Offset: 0x0000399E
		public static void SetTestModeEnabled(bool testModeEnabled)
		{
			IDebugManager debugManager = Debug.DebugManager;
			if (debugManager == null)
			{
				return;
			}
			debugManager.SetTestModeEnabled(testModeEnabled);
		}

		// Token: 0x06000118 RID: 280 RVA: 0x000057B0 File Offset: 0x000039B0
		public static void AbortGame()
		{
			if (Debug.DebugManager != null)
			{
				Debug.DebugManager.AbortGame();
			}
		}

		// Token: 0x020000C3 RID: 195
		public enum DebugColor
		{
			// Token: 0x04000227 RID: 551
			DarkRed,
			// Token: 0x04000228 RID: 552
			DarkGreen,
			// Token: 0x04000229 RID: 553
			DarkBlue,
			// Token: 0x0400022A RID: 554
			Red,
			// Token: 0x0400022B RID: 555
			Green,
			// Token: 0x0400022C RID: 556
			Blue,
			// Token: 0x0400022D RID: 557
			DarkCyan,
			// Token: 0x0400022E RID: 558
			Cyan,
			// Token: 0x0400022F RID: 559
			DarkYellow,
			// Token: 0x04000230 RID: 560
			Yellow,
			// Token: 0x04000231 RID: 561
			Purple,
			// Token: 0x04000232 RID: 562
			Magenta,
			// Token: 0x04000233 RID: 563
			White,
			// Token: 0x04000234 RID: 564
			BrightWhite
		}

		// Token: 0x020000C4 RID: 196
		public enum DebugUserFilter : ulong
		{
			// Token: 0x04000236 RID: 566
			None,
			// Token: 0x04000237 RID: 567
			Unused0,
			// Token: 0x04000238 RID: 568
			Unused1,
			// Token: 0x04000239 RID: 569
			Koray = 4UL,
			// Token: 0x0400023A RID: 570
			Armagan = 8UL,
			// Token: 0x0400023B RID: 571
			Intern = 16UL,
			// Token: 0x0400023C RID: 572
			Mustafa = 32UL,
			// Token: 0x0400023D RID: 573
			Oguzhan = 64UL,
			// Token: 0x0400023E RID: 574
			Omer = 128UL,
			// Token: 0x0400023F RID: 575
			Ates = 256UL,
			// Token: 0x04000240 RID: 576
			Unused3 = 512UL,
			// Token: 0x04000241 RID: 577
			Basak = 1024UL,
			// Token: 0x04000242 RID: 578
			Can = 2048UL,
			// Token: 0x04000243 RID: 579
			Unused4 = 4096UL,
			// Token: 0x04000244 RID: 580
			Cem = 8192UL,
			// Token: 0x04000245 RID: 581
			Unused5 = 16384UL,
			// Token: 0x04000246 RID: 582
			Unused6 = 32768UL,
			// Token: 0x04000247 RID: 583
			Emircan = 65536UL,
			// Token: 0x04000248 RID: 584
			Unused7 = 131072UL,
			// Token: 0x04000249 RID: 585
			All = 4294967295UL,
			// Token: 0x0400024A RID: 586
			Default = 0UL,
			// Token: 0x0400024B RID: 587
			DamageDebug = 72UL
		}

		// Token: 0x020000C5 RID: 197
		public enum DebugSystemFilter : ulong
		{
			// Token: 0x0400024D RID: 589
			None,
			// Token: 0x0400024E RID: 590
			Graphics = 4294967296UL,
			// Token: 0x0400024F RID: 591
			ArtificialIntelligence = 8589934592UL,
			// Token: 0x04000250 RID: 592
			MultiPlayer = 17179869184UL,
			// Token: 0x04000251 RID: 593
			IO = 34359738368UL,
			// Token: 0x04000252 RID: 594
			Network = 68719476736UL,
			// Token: 0x04000253 RID: 595
			CampaignEvents = 137438953472UL,
			// Token: 0x04000254 RID: 596
			MemoryManager = 274877906944UL,
			// Token: 0x04000255 RID: 597
			TCP = 549755813888UL,
			// Token: 0x04000256 RID: 598
			FileManager = 1099511627776UL,
			// Token: 0x04000257 RID: 599
			NaturalInteractionDevice = 2199023255552UL,
			// Token: 0x04000258 RID: 600
			UDP = 4398046511104UL,
			// Token: 0x04000259 RID: 601
			ResourceManager = 8796093022208UL,
			// Token: 0x0400025A RID: 602
			Mono = 17592186044416UL,
			// Token: 0x0400025B RID: 603
			ONO = 35184372088832UL,
			// Token: 0x0400025C RID: 604
			Old = 70368744177664UL,
			// Token: 0x0400025D RID: 605
			Sound = 281474976710656UL,
			// Token: 0x0400025E RID: 606
			CombatLog = 562949953421312UL,
			// Token: 0x0400025F RID: 607
			Notifications = 1125899906842624UL,
			// Token: 0x04000260 RID: 608
			Quest = 2251799813685248UL,
			// Token: 0x04000261 RID: 609
			Dialog = 4503599627370496UL,
			// Token: 0x04000262 RID: 610
			Steam = 9007199254740992UL,
			// Token: 0x04000263 RID: 611
			All = 18446744069414584320UL,
			// Token: 0x04000264 RID: 612
			DefaultMask = 18446744069414584320UL
		}
	}
}
