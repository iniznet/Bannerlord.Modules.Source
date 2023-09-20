using System;
using System.Runtime.CompilerServices;

namespace TaleWorlds.Library
{
	// Token: 0x02000027 RID: 39
	public interface IDebugManager
	{
		// Token: 0x06000119 RID: 281
		void ShowWarning(string message);

		// Token: 0x0600011A RID: 282
		void Assert(bool condition, string message, [CallerFilePath] string callerFile = "", [CallerMemberName] string callerMethod = "", [CallerLineNumber] int callerLine = 0);

		// Token: 0x0600011B RID: 283
		void SilentAssert(bool condition, string message = "", bool getDump = false, [CallerFilePath] string callerFile = "", [CallerMemberName] string callerMethod = "", [CallerLineNumber] int callerLine = 0);

		// Token: 0x0600011C RID: 284
		void Print(string message, int logLevel = 0, Debug.DebugColor color = Debug.DebugColor.White, ulong debugFilter = 17592186044416UL);

		// Token: 0x0600011D RID: 285
		void PrintError(string error, string stackTrace, ulong debugFilter = 17592186044416UL);

		// Token: 0x0600011E RID: 286
		void PrintWarning(string warning, ulong debugFilter = 17592186044416UL);

		// Token: 0x0600011F RID: 287
		void ShowError(string message);

		// Token: 0x06000120 RID: 288
		void ShowMessageBox(string lpText, string lpCaption, uint uType);

		// Token: 0x06000121 RID: 289
		void DisplayDebugMessage(string message);

		// Token: 0x06000122 RID: 290
		void WatchVariable(string name, object value);

		// Token: 0x06000123 RID: 291
		void WriteDebugLineOnScreen(string message);

		// Token: 0x06000124 RID: 292
		void RenderDebugLine(Vec3 position, Vec3 direction, uint color = 4294967295U, bool depthCheck = false, float time = 0f);

		// Token: 0x06000125 RID: 293
		void RenderDebugSphere(Vec3 position, float radius, uint color = 4294967295U, bool depthCheck = false, float time = 0f);

		// Token: 0x06000126 RID: 294
		void RenderDebugText3D(Vec3 position, string text, uint color = 4294967295U, int screenPosOffsetX = 0, int screenPosOffsetY = 0, float time = 0f);

		// Token: 0x06000127 RID: 295
		void RenderDebugFrame(MatrixFrame frame, float lineLength, float time = 0f);

		// Token: 0x06000128 RID: 296
		void RenderDebugText(float screenX, float screenY, string text, uint color = 4294967295U, float time = 0f);

		// Token: 0x06000129 RID: 297
		void RenderDebugRectWithColor(float left, float bottom, float right, float top, uint color = 4294967295U);

		// Token: 0x0600012A RID: 298
		Vec3 GetDebugVector();

		// Token: 0x0600012B RID: 299
		void SetCrashReportCustomString(string customString);

		// Token: 0x0600012C RID: 300
		void SetCrashReportCustomStack(string customStack);

		// Token: 0x0600012D RID: 301
		void SetTestModeEnabled(bool testModeEnabled);

		// Token: 0x0600012E RID: 302
		void AbortGame();

		// Token: 0x0600012F RID: 303
		void DoDelayedexit(int returnCode);

		// Token: 0x06000130 RID: 304
		void ReportMemoryBookmark(string message);
	}
}
