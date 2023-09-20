using System;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
	// Token: 0x0200003C RID: 60
	[ApplicationInterfaceBase]
	internal interface IImgui
	{
		// Token: 0x06000530 RID: 1328
		[EngineMethod("begin_main_thread_scope", false)]
		void BeginMainThreadScope();

		// Token: 0x06000531 RID: 1329
		[EngineMethod("end_main_thread_scope", false)]
		void EndMainThreadScope();

		// Token: 0x06000532 RID: 1330
		[EngineMethod("push_style_color", false)]
		void PushStyleColor(int style, ref Vec3 color);

		// Token: 0x06000533 RID: 1331
		[EngineMethod("pop_style_color", false)]
		void PopStyleColor();

		// Token: 0x06000534 RID: 1332
		[EngineMethod("new_frame", false)]
		void NewFrame();

		// Token: 0x06000535 RID: 1333
		[EngineMethod("render", false)]
		void Render();

		// Token: 0x06000536 RID: 1334
		[EngineMethod("begin", false)]
		void Begin(string text);

		// Token: 0x06000537 RID: 1335
		[EngineMethod("begin_with_close_button", false)]
		void BeginWithCloseButton(string text, ref bool is_open);

		// Token: 0x06000538 RID: 1336
		[EngineMethod("end", false)]
		void End();

		// Token: 0x06000539 RID: 1337
		[EngineMethod("text", false)]
		void Text(string text);

		// Token: 0x0600053A RID: 1338
		[EngineMethod("checkbox", false)]
		bool Checkbox(string text, ref bool is_checked);

		// Token: 0x0600053B RID: 1339
		[EngineMethod("tree_node", false)]
		bool TreeNode(string name);

		// Token: 0x0600053C RID: 1340
		[EngineMethod("tree_pop", false)]
		void TreePop();

		// Token: 0x0600053D RID: 1341
		[EngineMethod("separator", false)]
		void Separator();

		// Token: 0x0600053E RID: 1342
		[EngineMethod("button", false)]
		bool Button(string text);

		// Token: 0x0600053F RID: 1343
		[EngineMethod("plot_lines", false)]
		void PlotLines(string name, float[] values, int valuesCount, int valuesOffset, string overlayText, float minScale, float maxScale, float graphWidth, float graphHeight, int stride);

		// Token: 0x06000540 RID: 1344
		[EngineMethod("progress_bar", false)]
		void ProgressBar(float value);

		// Token: 0x06000541 RID: 1345
		[EngineMethod("new_line", false)]
		void NewLine();

		// Token: 0x06000542 RID: 1346
		[EngineMethod("same_line", false)]
		void SameLine(float posX, float spacingWidth);

		// Token: 0x06000543 RID: 1347
		[EngineMethod("combo", false)]
		bool Combo(string label, ref int selectedIndex, string items);

		// Token: 0x06000544 RID: 1348
		[EngineMethod("input_int", false)]
		bool InputInt(string label, ref int value);

		// Token: 0x06000545 RID: 1349
		[EngineMethod("slider_float", false)]
		bool SliderFloat(string label, ref float value, float min, float max);

		// Token: 0x06000546 RID: 1350
		[EngineMethod("columns", false)]
		void Columns(int count = 1, string id = "", bool border = true);

		// Token: 0x06000547 RID: 1351
		[EngineMethod("next_column", false)]
		void NextColumn();

		// Token: 0x06000548 RID: 1352
		[EngineMethod("radio_button", false)]
		bool RadioButton(string label, bool active);

		// Token: 0x06000549 RID: 1353
		[EngineMethod("collapsing_header", false)]
		bool CollapsingHeader(string label);

		// Token: 0x0600054A RID: 1354
		[EngineMethod("is_item_hovered", false)]
		bool IsItemHovered();

		// Token: 0x0600054B RID: 1355
		[EngineMethod("set_tool_tip", false)]
		void SetTooltip(string label);

		// Token: 0x0600054C RID: 1356
		[EngineMethod("small_button", false)]
		bool SmallButton(string label);

		// Token: 0x0600054D RID: 1357
		[EngineMethod("input_float", false)]
		bool InputFloat(string label, ref float val, float step, float stepFast, int decimalPrecision = -1);

		// Token: 0x0600054E RID: 1358
		[EngineMethod("input_float2", false)]
		bool InputFloat2(string label, ref float val0, ref float val1, int decimalPrecision = -1);

		// Token: 0x0600054F RID: 1359
		[EngineMethod("input_float3", false)]
		bool InputFloat3(string label, ref float val0, ref float val1, ref float val2, int decimalPrecision = -1);

		// Token: 0x06000550 RID: 1360
		[EngineMethod("input_float4", false)]
		bool InputFloat4(string label, ref float val0, ref float val1, ref float val2, ref float val3, int decimalPrecision = -1);
	}
}
