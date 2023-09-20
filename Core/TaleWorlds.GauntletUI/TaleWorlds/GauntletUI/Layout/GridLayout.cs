using System;
using System.Collections.Generic;
using System.Numerics;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.GauntletUI.Layout
{
	public class GridLayout : ILayout
	{
		public GridVerticalLayoutMethod VerticalLayoutMethod { get; set; }

		public GridHorizontalLayoutMethod HorizontalLayoutMethod { get; set; }

		public GridLayout()
		{
			this.VerticalLayoutMethod = GridVerticalLayoutMethod.TopToBottom;
			this.HorizontalLayoutMethod = GridHorizontalLayoutMethod.LeftToRight;
		}

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
				measureSpec..ctor(num5, num6);
				foreach (Widget widget2 in gridWidget.Children)
				{
					if (widget2.IsVisible)
					{
						widget2.Measure(measureSpec);
					}
				}
				vector..ctor((float)num3 * num5, (float)num2 * num6);
			}
			return vector;
		}

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
