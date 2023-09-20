using System;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
	// Token: 0x02000032 RID: 50
	[ApplicationInterfaceBase]
	internal interface IDebug
	{
		// Token: 0x06000446 RID: 1094
		[EngineMethod("write_debug_line_on_screen", false)]
		void WriteDebugLineOnScreen(string line);

		// Token: 0x06000447 RID: 1095
		[EngineMethod("abort_game", false)]
		void AbortGame(int ExitCode);

		// Token: 0x06000448 RID: 1096
		[EngineMethod("assert_memory_usage", false)]
		void AssertMemoryUsage(int memoryMB);

		// Token: 0x06000449 RID: 1097
		[EngineMethod("write_line", false)]
		void WriteLine(int logLevel, string line, int color, ulong filter);

		// Token: 0x0600044A RID: 1098
		[EngineMethod("render_debug_direction_arrow", false)]
		void RenderDebugDirectionArrow(Vec3 position, Vec3 direction, uint color, bool depthCheck);

		// Token: 0x0600044B RID: 1099
		[EngineMethod("render_debug_line", false)]
		void RenderDebugLine(Vec3 position, Vec3 direction, uint color, bool depthCheck, float time);

		// Token: 0x0600044C RID: 1100
		[EngineMethod("render_debug_sphere", false)]
		void RenderDebugSphere(Vec3 position, float radius, uint color, bool depthCheck, float time);

		// Token: 0x0600044D RID: 1101
		[EngineMethod("render_debug_capsule", false)]
		void RenderDebugCapsule(Vec3 p0, Vec3 p1, float radius, uint color, bool depthCheck, float time);

		// Token: 0x0600044E RID: 1102
		[EngineMethod("render_debug_frame", false)]
		void RenderDebugFrame(ref MatrixFrame frame, float lineLength, float time);

		// Token: 0x0600044F RID: 1103
		[EngineMethod("render_debug_text3d", false)]
		void RenderDebugText3d(Vec3 worldPosition, string str, uint color, int screenPosOffsetX, int screenPosOffsetY, float time);

		// Token: 0x06000450 RID: 1104
		[EngineMethod("render_debug_text", false)]
		void RenderDebugText(float screenX, float screenY, string str, uint color, float time);

		// Token: 0x06000451 RID: 1105
		[EngineMethod("render_debug_rect", false)]
		void RenderDebugRect(float left, float bottom, float right, float top);

		// Token: 0x06000452 RID: 1106
		[EngineMethod("render_debug_rect_with_color", false)]
		void RenderDebugRectWithColor(float left, float bottom, float right, float top, uint color);

		// Token: 0x06000453 RID: 1107
		[EngineMethod("clear_all_debug_render_objects", false)]
		void ClearAllDebugRenderObjects();

		// Token: 0x06000454 RID: 1108
		[EngineMethod("get_debug_vector", false)]
		Vec3 GetDebugVector();

		// Token: 0x06000455 RID: 1109
		[EngineMethod("render_debug_box_object", false)]
		void RenderDebugBoxObject(Vec3 min, Vec3 max, uint color, bool depthCheck, float time);

		// Token: 0x06000456 RID: 1110
		[EngineMethod("render_debug_box_object_with_frame", false)]
		void RenderDebugBoxObjectWithFrame(Vec3 min, Vec3 max, ref MatrixFrame frame, uint color, bool depthCheck, float time);

		// Token: 0x06000457 RID: 1111
		[EngineMethod("post_warning_line", false)]
		void PostWarningLine(string line);

		// Token: 0x06000458 RID: 1112
		[EngineMethod("is_error_report_mode_active", false)]
		bool IsErrorReportModeActive();

		// Token: 0x06000459 RID: 1113
		[EngineMethod("is_error_report_mode_pause_mission", false)]
		bool IsErrorReportModePauseMission();

		// Token: 0x0600045A RID: 1114
		[EngineMethod("set_error_report_scene", false)]
		void SetErrorReportScene(UIntPtr scenePointer);

		// Token: 0x0600045B RID: 1115
		[EngineMethod("set_dump_generation_disabled", false)]
		void SetDumpGenerationDisabled(bool Disabled);

		// Token: 0x0600045C RID: 1116
		[EngineMethod("message_box", false)]
		int MessageBox(string lpText, string lpCaption, uint uType);

		// Token: 0x0600045D RID: 1117
		[EngineMethod("get_show_debug_info", false)]
		int GetShowDebugInfo();

		// Token: 0x0600045E RID: 1118
		[EngineMethod("set_show_debug_info", false)]
		void SetShowDebugInfo(int value);

		// Token: 0x0600045F RID: 1119
		[EngineMethod("error", false)]
		bool Error(string MessageString);

		// Token: 0x06000460 RID: 1120
		[EngineMethod("warning", false)]
		bool Warning(string MessageString);

		// Token: 0x06000461 RID: 1121
		[EngineMethod("content_warning", false)]
		bool ContentWarning(string MessageString);

		// Token: 0x06000462 RID: 1122
		[EngineMethod("failed_assert", false)]
		bool FailedAssert(string messageString, string callerFile, string callerMethod, int callerLine);

		// Token: 0x06000463 RID: 1123
		[EngineMethod("silent_assert", false)]
		bool SilentAssert(string messageString, string callerFile, string callerMethod, int callerLine, bool getDump);

		// Token: 0x06000464 RID: 1124
		[EngineMethod("is_test_mode", false)]
		bool IsTestMode();
	}
}
