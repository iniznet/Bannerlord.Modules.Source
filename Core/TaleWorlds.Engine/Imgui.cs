using System;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
	// Token: 0x0200004D RID: 77
	public class Imgui
	{
		// Token: 0x060006AD RID: 1709 RVA: 0x00004D2B File Offset: 0x00002F2B
		public static void BeginMainThreadScope()
		{
			EngineApplicationInterface.IImgui.BeginMainThreadScope();
		}

		// Token: 0x060006AE RID: 1710 RVA: 0x00004D37 File Offset: 0x00002F37
		public static void EndMainThreadScope()
		{
			EngineApplicationInterface.IImgui.EndMainThreadScope();
		}

		// Token: 0x060006AF RID: 1711 RVA: 0x00004D43 File Offset: 0x00002F43
		public static void PushStyleColor(Imgui.ColorStyle style, ref Vec3 color)
		{
			EngineApplicationInterface.IImgui.PushStyleColor((int)style, ref color);
		}

		// Token: 0x060006B0 RID: 1712 RVA: 0x00004D51 File Offset: 0x00002F51
		public static void PopStyleColor()
		{
			EngineApplicationInterface.IImgui.PopStyleColor();
		}

		// Token: 0x060006B1 RID: 1713 RVA: 0x00004D5D File Offset: 0x00002F5D
		public static void NewFrame()
		{
			EngineApplicationInterface.IImgui.NewFrame();
		}

		// Token: 0x060006B2 RID: 1714 RVA: 0x00004D69 File Offset: 0x00002F69
		public static void Render()
		{
			EngineApplicationInterface.IImgui.Render();
		}

		// Token: 0x060006B3 RID: 1715 RVA: 0x00004D75 File Offset: 0x00002F75
		public static void Begin(string text)
		{
			EngineApplicationInterface.IImgui.Begin(text);
		}

		// Token: 0x060006B4 RID: 1716 RVA: 0x00004D82 File Offset: 0x00002F82
		public static void Begin(string text, ref bool is_open)
		{
			EngineApplicationInterface.IImgui.BeginWithCloseButton(text, ref is_open);
		}

		// Token: 0x060006B5 RID: 1717 RVA: 0x00004D90 File Offset: 0x00002F90
		public static void End()
		{
			EngineApplicationInterface.IImgui.End();
		}

		// Token: 0x060006B6 RID: 1718 RVA: 0x00004D9C File Offset: 0x00002F9C
		public static void Text(string text)
		{
			EngineApplicationInterface.IImgui.Text(text);
		}

		// Token: 0x060006B7 RID: 1719 RVA: 0x00004DA9 File Offset: 0x00002FA9
		public static bool Checkbox(string text, ref bool is_checked)
		{
			return EngineApplicationInterface.IImgui.Checkbox(text, ref is_checked);
		}

		// Token: 0x060006B8 RID: 1720 RVA: 0x00004DB7 File Offset: 0x00002FB7
		public static bool TreeNode(string name)
		{
			return EngineApplicationInterface.IImgui.TreeNode(name);
		}

		// Token: 0x060006B9 RID: 1721 RVA: 0x00004DC4 File Offset: 0x00002FC4
		public static void TreePop()
		{
			EngineApplicationInterface.IImgui.TreePop();
		}

		// Token: 0x060006BA RID: 1722 RVA: 0x00004DD0 File Offset: 0x00002FD0
		public static void Separator()
		{
			EngineApplicationInterface.IImgui.Separator();
		}

		// Token: 0x060006BB RID: 1723 RVA: 0x00004DDC File Offset: 0x00002FDC
		public static bool Button(string text)
		{
			return EngineApplicationInterface.IImgui.Button(text);
		}

		// Token: 0x060006BC RID: 1724 RVA: 0x00004DEC File Offset: 0x00002FEC
		public static void PlotLines(string name, float[] values, int valuesCount, int valuesOffset, string overlayText, float minScale, float maxScale, float graphWidth, float graphHeight, int stride)
		{
			EngineApplicationInterface.IImgui.PlotLines(name, values, valuesCount, valuesOffset, overlayText, minScale, maxScale, graphWidth, graphHeight, stride);
		}

		// Token: 0x060006BD RID: 1725 RVA: 0x00004E13 File Offset: 0x00003013
		public static void ProgressBar(float progress)
		{
			EngineApplicationInterface.IImgui.ProgressBar(progress);
		}

		// Token: 0x060006BE RID: 1726 RVA: 0x00004E20 File Offset: 0x00003020
		public static void NewLine()
		{
			EngineApplicationInterface.IImgui.NewLine();
		}

		// Token: 0x060006BF RID: 1727 RVA: 0x00004E2C File Offset: 0x0000302C
		public static void SameLine(float posX = 0f, float spacingWidth = 0f)
		{
			EngineApplicationInterface.IImgui.SameLine(posX, spacingWidth);
		}

		// Token: 0x060006C0 RID: 1728 RVA: 0x00004E3A File Offset: 0x0000303A
		public static bool Combo(string label, ref int selectedIndex, string items)
		{
			return EngineApplicationInterface.IImgui.Combo(label, ref selectedIndex, items);
		}

		// Token: 0x060006C1 RID: 1729 RVA: 0x00004E49 File Offset: 0x00003049
		public static bool InputInt(string label, ref int value)
		{
			return EngineApplicationInterface.IImgui.InputInt(label, ref value);
		}

		// Token: 0x060006C2 RID: 1730 RVA: 0x00004E57 File Offset: 0x00003057
		public static bool SliderFloat(string label, ref float value, float min, float max)
		{
			return EngineApplicationInterface.IImgui.SliderFloat(label, ref value, min, max);
		}

		// Token: 0x060006C3 RID: 1731 RVA: 0x00004E67 File Offset: 0x00003067
		public static void Columns(int count = 1, string id = "", bool border = true)
		{
			EngineApplicationInterface.IImgui.Columns(count, id, border);
		}

		// Token: 0x060006C4 RID: 1732 RVA: 0x00004E76 File Offset: 0x00003076
		public static void NextColumn()
		{
			EngineApplicationInterface.IImgui.NextColumn();
		}

		// Token: 0x060006C5 RID: 1733 RVA: 0x00004E82 File Offset: 0x00003082
		public static bool RadioButton(string label, bool active)
		{
			return EngineApplicationInterface.IImgui.RadioButton(label, active);
		}

		// Token: 0x060006C6 RID: 1734 RVA: 0x00004E90 File Offset: 0x00003090
		public static bool CollapsingHeader(string label)
		{
			return EngineApplicationInterface.IImgui.CollapsingHeader(label);
		}

		// Token: 0x060006C7 RID: 1735 RVA: 0x00004E9D File Offset: 0x0000309D
		public static bool IsItemHovered()
		{
			return EngineApplicationInterface.IImgui.IsItemHovered();
		}

		// Token: 0x060006C8 RID: 1736 RVA: 0x00004EA9 File Offset: 0x000030A9
		public static void SetTooltip(string label)
		{
			EngineApplicationInterface.IImgui.SetTooltip(label);
		}

		// Token: 0x060006C9 RID: 1737 RVA: 0x00004EB6 File Offset: 0x000030B6
		public static bool SmallButton(string label)
		{
			return EngineApplicationInterface.IImgui.SmallButton(label);
		}

		// Token: 0x060006CA RID: 1738 RVA: 0x00004EC3 File Offset: 0x000030C3
		public static bool InputFloat(string label, ref float val, float step, float stepFast, int decimalPrecision = -1)
		{
			return EngineApplicationInterface.IImgui.InputFloat(label, ref val, step, stepFast, decimalPrecision);
		}

		// Token: 0x060006CB RID: 1739 RVA: 0x00004ED5 File Offset: 0x000030D5
		public static bool InputFloat2(string label, ref float val0, ref float val1, int decimalPrecision = -1)
		{
			return EngineApplicationInterface.IImgui.InputFloat2(label, ref val0, ref val1, decimalPrecision);
		}

		// Token: 0x060006CC RID: 1740 RVA: 0x00004EE5 File Offset: 0x000030E5
		public static bool InputFloat3(string label, ref float val0, ref float val1, ref float val2, int decimalPrecision = -1)
		{
			return EngineApplicationInterface.IImgui.InputFloat3(label, ref val0, ref val1, ref val2, decimalPrecision);
		}

		// Token: 0x060006CD RID: 1741 RVA: 0x00004EF7 File Offset: 0x000030F7
		public static bool InputFloat4(string label, ref float val0, ref float val1, ref float val2, ref float val3, int decimalPrecision = -1)
		{
			return EngineApplicationInterface.IImgui.InputFloat4(label, ref val0, ref val1, ref val2, ref val3, decimalPrecision);
		}

		// Token: 0x020000B6 RID: 182
		public enum ColorStyle
		{
			// Token: 0x04000388 RID: 904
			Text,
			// Token: 0x04000389 RID: 905
			TextDisabled,
			// Token: 0x0400038A RID: 906
			WindowBg,
			// Token: 0x0400038B RID: 907
			ChildWindowBg,
			// Token: 0x0400038C RID: 908
			PopupBg,
			// Token: 0x0400038D RID: 909
			Border,
			// Token: 0x0400038E RID: 910
			BorderShadow,
			// Token: 0x0400038F RID: 911
			FrameBg,
			// Token: 0x04000390 RID: 912
			FrameBgHovered,
			// Token: 0x04000391 RID: 913
			FrameBgActive,
			// Token: 0x04000392 RID: 914
			TitleBg,
			// Token: 0x04000393 RID: 915
			TitleBgCollapsed,
			// Token: 0x04000394 RID: 916
			TitleBgActive,
			// Token: 0x04000395 RID: 917
			MenuBarBg,
			// Token: 0x04000396 RID: 918
			ScrollbarBg,
			// Token: 0x04000397 RID: 919
			ScrollbarGrab,
			// Token: 0x04000398 RID: 920
			ScrollbarGrabHovered,
			// Token: 0x04000399 RID: 921
			ScrollbarGrabActive,
			// Token: 0x0400039A RID: 922
			ComboBg,
			// Token: 0x0400039B RID: 923
			CheckMark,
			// Token: 0x0400039C RID: 924
			SliderGrab,
			// Token: 0x0400039D RID: 925
			SliderGrabActive,
			// Token: 0x0400039E RID: 926
			Button,
			// Token: 0x0400039F RID: 927
			ButtonHovered,
			// Token: 0x040003A0 RID: 928
			ButtonActive,
			// Token: 0x040003A1 RID: 929
			Header,
			// Token: 0x040003A2 RID: 930
			HeaderHovered,
			// Token: 0x040003A3 RID: 931
			HeaderActive,
			// Token: 0x040003A4 RID: 932
			Column,
			// Token: 0x040003A5 RID: 933
			ColumnHovered,
			// Token: 0x040003A6 RID: 934
			ColumnActive,
			// Token: 0x040003A7 RID: 935
			ResizeGrip,
			// Token: 0x040003A8 RID: 936
			ResizeGripHovered,
			// Token: 0x040003A9 RID: 937
			ResizeGripActive,
			// Token: 0x040003AA RID: 938
			CloseButton,
			// Token: 0x040003AB RID: 939
			CloseButtonHovered,
			// Token: 0x040003AC RID: 940
			CloseButtonActive,
			// Token: 0x040003AD RID: 941
			PlotLines,
			// Token: 0x040003AE RID: 942
			PlotLinesHovered,
			// Token: 0x040003AF RID: 943
			PlotHistogram,
			// Token: 0x040003B0 RID: 944
			PlotHistogramHovered,
			// Token: 0x040003B1 RID: 945
			TextSelectedBg,
			// Token: 0x040003B2 RID: 946
			ModalWindowDarkening
		}
	}
}
