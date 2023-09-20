using System;
using System.Collections.Generic;
using System.Linq;
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

		public GridDirection Direction { get; set; }

		public IReadOnlyList<float> RowHeights { get; private set; } = new List<float>();

		public IReadOnlyList<float> ColumnWidths { get; private set; } = new List<float>();

		public GridLayout()
		{
			this.VerticalLayoutMethod = GridVerticalLayoutMethod.TopToBottom;
			this.HorizontalLayoutMethod = GridHorizontalLayoutMethod.LeftToRight;
		}

		Vector2 ILayout.MeasureChildren(Widget widget, Vector2 measureSpec, SpriteData spriteData, float renderScale)
		{
			GridWidget gridWidget = (GridWidget)widget;
			Vector2 vector = default(Vector2);
			int num = gridWidget.Children.Count((Widget x) => x.IsVisible);
			if (num > 0)
			{
				foreach (Widget widget2 in gridWidget.Children)
				{
					if (widget2.IsVisible && (widget2.WidthSizePolicy == SizePolicy.CoverChildren || widget2.HeightSizePolicy == SizePolicy.CoverChildren))
					{
						widget2.Measure(default(Vector2));
					}
				}
				int num2;
				int num3;
				int num4;
				int num5;
				this.CalculateRowColumnCounts(gridWidget, this.Direction, num, out num2, out num3, out num4, out num5);
				this.UpdateCellSizes(gridWidget, num2, num3, num4, num5, measureSpec.X, measureSpec.Y);
				int num6 = 0;
				for (int i = 0; i < gridWidget.Children.Count; i++)
				{
					Widget widget3 = gridWidget.Children[i];
					if (widget3.IsVisible)
					{
						int num7;
						int num8;
						this.CalculateRowColumnIndices(num6, num2, num4, out num7, out num8);
						float element = this.GetElement(this.ColumnWidths, num8);
						float element2 = this.GetElement(this.RowHeights, num7);
						widget3.Measure(new Vector2(element, element2));
						num6++;
					}
				}
				vector = new Vector2(this.ColumnWidths.Sum(), this.RowHeights.Sum());
			}
			return vector;
		}

		void ILayout.OnLayout(Widget widget, float left, float bottom, float right, float top)
		{
			GridWidget gridWidget = (GridWidget)widget;
			int num = gridWidget.Children.Count((Widget x) => x.IsVisible);
			if (num > 0)
			{
				float num2 = right - left;
				float num3 = bottom - top;
				int num4;
				int num5;
				int num6;
				int num7;
				this.CalculateRowColumnCounts(gridWidget, this.Direction, num, out num4, out num5, out num6, out num7);
				this.UpdateCellSizes(gridWidget, num4, num5, num6, num7, num2, num3);
				float[] array = new float[this.RowHeights.Count + 1];
				float[] array2 = new float[this.ColumnWidths.Count + 1];
				array[0] = 0f;
				array2[0] = 0f;
				for (int i = 0; i < this.RowHeights.Count; i++)
				{
					array[i + 1] = this.RowHeights[i] + array[i];
				}
				for (int j = 0; j < this.ColumnWidths.Count; j++)
				{
					array2[j + 1] = this.ColumnWidths[j] + array2[j];
				}
				int num8 = 0;
				for (int k = 0; k < gridWidget.Children.Count; k++)
				{
					Widget widget2 = gridWidget.Children[k];
					if (widget2.IsVisible)
					{
						int num9;
						int num10;
						this.CalculateRowColumnIndices(num8, num4, num6, out num9, out num10);
						int num11 = num5 - num9 - 1;
						int num12 = num7 - num10 - 1;
						float element = this.GetElement(this.ColumnWidths, num10);
						float element2 = this.GetElement(this.RowHeights, num9);
						float num13 = 0f;
						float num14 = 0f;
						if (this.VerticalLayoutMethod == GridVerticalLayoutMethod.TopToBottom)
						{
							num13 = this.GetElement(array, num9);
						}
						else if (this.VerticalLayoutMethod == GridVerticalLayoutMethod.Center)
						{
							if (this.Direction == GridDirection.ColumnFirst)
							{
								int num15 = MathF.Max(0, (num10 + 1) * num5 - num) / 2 + num9;
								num13 = this.GetElement(array, num15);
							}
						}
						else if (this.VerticalLayoutMethod == GridVerticalLayoutMethod.BottomToTop)
						{
							num13 = this.GetElement(array, num11);
						}
						if (this.HorizontalLayoutMethod == GridHorizontalLayoutMethod.LeftToRight)
						{
							num14 = this.GetElement(array2, num10);
						}
						else if (this.HorizontalLayoutMethod == GridHorizontalLayoutMethod.Center)
						{
							if (this.Direction == GridDirection.RowFirst)
							{
								int num16 = MathF.Max(0, (num9 + 1) * num7 - num) / 2 + num10;
								num14 = this.GetElement(array2, num16);
							}
						}
						else if (this.HorizontalLayoutMethod == GridHorizontalLayoutMethod.RightToLeft)
						{
							num14 = this.GetElement(array2, num12);
						}
						widget2.Layout(num14, num13 + element2, num14 + element, num13);
						num8++;
					}
				}
			}
		}

		private void UpdateCellSizes(GridWidget gridWidget, int rowCount, int usedRowCount, int columnCount, int usedColumnCount, float totalWidth, float totalHeight)
		{
			float[] array = new float[usedRowCount];
			float[] array2 = new float[usedColumnCount];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = 0f;
			}
			for (int j = 0; j < array2.Length; j++)
			{
				array2[j] = 0f;
			}
			int num = 0;
			for (int k = 0; k < gridWidget.Children.Count; k++)
			{
				Widget widget = gridWidget.Children[k];
				if (widget.IsVisible)
				{
					int num2;
					int num3;
					this.CalculateRowColumnIndices(num, rowCount, columnCount, out num2, out num3);
					float num4;
					if (gridWidget.WidthSizePolicy == SizePolicy.CoverChildren)
					{
						if (gridWidget.UseDynamicCellWidth && widget.WidthSizePolicy != SizePolicy.StretchToParent)
						{
							num4 = widget.MeasuredSize.X + widget.ScaledMarginLeft + widget.ScaledMarginRight;
						}
						else
						{
							num4 = gridWidget.DefaultScaledCellWidth;
						}
					}
					else
					{
						num4 = totalWidth / (float)columnCount;
					}
					float num5;
					if (gridWidget.HeightSizePolicy == SizePolicy.CoverChildren)
					{
						if (gridWidget.UseDynamicCellHeight && widget.HeightSizePolicy != SizePolicy.StretchToParent)
						{
							num5 = widget.MeasuredSize.Y + widget.ScaledMarginTop + widget.ScaledMarginBottom;
						}
						else
						{
							num5 = gridWidget.DefaultScaledCellHeight;
						}
					}
					else
					{
						num5 = totalHeight / (float)rowCount;
					}
					if (num2 >= 0 && num2 < array.Length)
					{
						array[num2] = MathF.Max(num5, array[num2]);
					}
					if (num3 >= 0 && num3 < array2.Length)
					{
						array2[num3] = MathF.Max(num4, array2[num3]);
					}
					num++;
				}
			}
			this.RowHeights = array;
			this.ColumnWidths = array2;
		}

		private void CalculateRowColumnIndices(int visibleIndex, int rowCount, int columnCount, out int row, out int column)
		{
			if (this.Direction == GridDirection.RowFirst)
			{
				row = visibleIndex / columnCount;
				column = visibleIndex % columnCount;
				return;
			}
			row = visibleIndex % rowCount;
			column = visibleIndex / rowCount;
		}

		private void CalculateRowColumnCounts(GridWidget gridWidget, GridDirection direction, int visibleChildrenCount, out int rowCount, out int usedRowCount, out int columnCount, out int usedColumnCount)
		{
			bool flag = gridWidget.RowCount < 0;
			bool flag2 = gridWidget.ColumnCount < 0;
			rowCount = (flag ? 3 : gridWidget.RowCount);
			columnCount = (flag2 ? 3 : gridWidget.ColumnCount);
			int num;
			int num2;
			if (direction == GridDirection.RowFirst)
			{
				num = MathF.Min(visibleChildrenCount, columnCount);
				num2 = ((visibleChildrenCount % columnCount > 0) ? 1 : 0) + visibleChildrenCount / columnCount;
			}
			else
			{
				num2 = MathF.Min(visibleChildrenCount, rowCount);
				num = ((visibleChildrenCount % rowCount > 0) ? 1 : 0) + visibleChildrenCount / rowCount;
			}
			bool flag3 = gridWidget.HeightSizePolicy != SizePolicy.CoverChildren;
			bool flag4 = gridWidget.WidthSizePolicy != SizePolicy.CoverChildren;
			usedRowCount = (flag3 ? rowCount : num2);
			usedColumnCount = (flag4 ? columnCount : num);
		}

		private float GetElement(IReadOnlyList<float> elements, int index)
		{
			if (index < 0 || index >= elements.Count)
			{
				return 0f;
			}
			return elements[index];
		}
	}
}
