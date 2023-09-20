using System;
using System.Collections.Generic;
using System.Numerics;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.GauntletUI.Layout
{
	// Token: 0x0200003B RID: 59
	public class GridLayout : ILayout
	{
		// Token: 0x1700011F RID: 287
		// (get) Token: 0x060003A6 RID: 934 RVA: 0x0000F66B File Offset: 0x0000D86B
		// (set) Token: 0x060003A7 RID: 935 RVA: 0x0000F673 File Offset: 0x0000D873
		public GridVerticalLayoutMethod VerticalLayoutMethod { get; set; }

		// Token: 0x17000120 RID: 288
		// (get) Token: 0x060003A8 RID: 936 RVA: 0x0000F67C File Offset: 0x0000D87C
		// (set) Token: 0x060003A9 RID: 937 RVA: 0x0000F684 File Offset: 0x0000D884
		public GridHorizontalLayoutMethod HorizontalLayoutMethod { get; set; }

		// Token: 0x060003AA RID: 938 RVA: 0x0000F68D File Offset: 0x0000D88D
		public GridLayout()
		{
			this.VerticalLayoutMethod = GridVerticalLayoutMethod.TopToBottom;
			this.HorizontalLayoutMethod = GridHorizontalLayoutMethod.LeftToRight;
		}

		// Token: 0x060003AB RID: 939 RVA: 0x0000F6A4 File Offset: 0x0000D8A4
		Vector2 ILayout.MeasureChildren(Widget widget, Vector2 measureSpec, SpriteData spriteData, float renderScale)
		{
			GridWidget gridWidget = (GridWidget)widget;
			Vector2 vector = default(Vector2);
			int num = 0;
			using (List<Widget>.Enumerator enumerator = gridWidget.Children.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.IsVisible)
					{
						num++;
					}
				}
			}
			if (num > 0)
			{
				int num2 = gridWidget.RowCount;
				int num3 = gridWidget.ColumnCount;
				float num5;
				if (gridWidget.WidthSizePolicy == SizePolicy.CoverChildren)
				{
					int num4 = num3;
					if (num < num3)
					{
						num4 = num;
					}
					num3 = num4;
					num5 = gridWidget.DefaultScaledCellWidth;
				}
				else
				{
					num5 = measureSpec.X / (float)num3;
				}
				float num6;
				if (gridWidget.HeightSizePolicy == SizePolicy.CoverChildren)
				{
					num2 = ((num % num3 > 0) ? 1 : 0) + num / num3;
					num6 = gridWidget.DefaultScaledCellHeight;
				}
				else
				{
					num6 = measureSpec.Y / (float)num2;
				}
				measureSpec = new Vector2(num5, num6);
				foreach (Widget widget2 in gridWidget.Children)
				{
					if (widget2.IsVisible)
					{
						widget2.Measure(measureSpec);
					}
				}
				vector = new Vector2((float)num3 * num5, (float)num2 * num6);
			}
			return vector;
		}

		// Token: 0x060003AC RID: 940 RVA: 0x0000F7F0 File Offset: 0x0000D9F0
		void ILayout.OnLayout(Widget widget, float left, float bottom, float right, float top)
		{
			GridWidget gridWidget = (GridWidget)widget;
			int num = 0;
			using (List<Widget>.Enumerator enumerator = gridWidget.Children.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.IsVisible)
					{
						num++;
					}
				}
			}
			if (num > 0)
			{
				int rowCount = gridWidget.RowCount;
				int columnCount = gridWidget.ColumnCount;
				float num2 = right - left;
				float num3 = bottom - top;
				int num4 = 0;
				for (int i = 0; i < gridWidget.Children.Count; i++)
				{
					Widget widget2 = gridWidget.Children[i];
					if (widget2.IsVisible)
					{
						int num5 = num4 / columnCount;
						if (this.VerticalLayoutMethod == GridVerticalLayoutMethod.BottomToTop)
						{
							num5 = ((num % columnCount > 0) ? 1 : 0) + num / columnCount - num5 - 1;
						}
						int num6 = num4 % columnCount;
						float num7;
						if (gridWidget.WidthSizePolicy == SizePolicy.CoverChildren)
						{
							num7 = gridWidget.DefaultScaledCellWidth;
						}
						else
						{
							num7 = num2 / (float)columnCount;
						}
						float num8;
						if (gridWidget.HeightSizePolicy == SizePolicy.CoverChildren)
						{
							num8 = gridWidget.DefaultScaledCellHeight;
						}
						else
						{
							num8 = num3 / (float)rowCount;
						}
						float num9 = 0f;
						float num10 = 0f;
						if (this.VerticalLayoutMethod == GridVerticalLayoutMethod.TopToBottom)
						{
							num9 = num8 * (float)num5;
							num10 = num8 * (float)(num5 + 1);
						}
						else if (this.VerticalLayoutMethod == GridVerticalLayoutMethod.BottomToTop)
						{
							num9 = num8 * (float)num5;
							num10 = num8 * (float)(num5 - 1);
						}
						float num11 = num7 * (float)num6;
						float num12 = num7 * (float)(num6 + 1);
						if (this.HorizontalLayoutMethod == GridHorizontalLayoutMethod.Center)
						{
							int num13 = MathF.Max(0, (num5 + 1) * columnCount - num);
							float num14 = (float)(columnCount - num13) * num7;
							float num15 = (widget.Size.X - num14) / 2f;
							num11 += num15;
							num12 += num15;
						}
						else if (this.HorizontalLayoutMethod == GridHorizontalLayoutMethod.RightToLeft)
						{
							num11 = num7 * (float)(MathF.Min(columnCount, num) - num6 - 1);
							num12 = num11 + num7;
						}
						widget2.Layout(num11, num10, num12, num9);
						num4++;
					}
				}
			}
		}
	}
}
