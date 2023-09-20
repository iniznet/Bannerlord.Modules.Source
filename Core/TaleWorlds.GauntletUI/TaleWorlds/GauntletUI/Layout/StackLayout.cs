using System;
using System.Collections.Generic;
using System.Numerics;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.GauntletUI.Layout
{
	// Token: 0x0200003F RID: 63
	public class StackLayout : ILayout
	{
		// Token: 0x17000121 RID: 289
		// (get) Token: 0x060003AF RID: 943 RVA: 0x0000F9EC File Offset: 0x0000DBEC
		// (set) Token: 0x060003B0 RID: 944 RVA: 0x0000F9F4 File Offset: 0x0000DBF4
		public ContainerItemDescription DefaultItemDescription { get; private set; }

		// Token: 0x17000122 RID: 290
		// (get) Token: 0x060003B1 RID: 945 RVA: 0x0000F9FD File Offset: 0x0000DBFD
		// (set) Token: 0x060003B2 RID: 946 RVA: 0x0000FA05 File Offset: 0x0000DC05
		public LayoutMethod LayoutMethod { get; set; }

		// Token: 0x060003B3 RID: 947 RVA: 0x0000FA0E File Offset: 0x0000DC0E
		public StackLayout()
		{
			this.DefaultItemDescription = new ContainerItemDescription();
			this._layoutBoxes = new Dictionary<int, LayoutBox>(64);
			this._parallelMeasureBasicChildDelegate = new TWParallel.ParallelForAuxPredicate(this.ParallelMeasureBasicChild);
		}

		// Token: 0x060003B4 RID: 948 RVA: 0x0000FA40 File Offset: 0x0000DC40
		public ContainerItemDescription GetItemDescription(Widget owner, Widget child, int childIndex)
		{
			Container container;
			if ((container = owner as Container) != null)
			{
				return container.GetItemDescription(child.Id, childIndex);
			}
			return this.DefaultItemDescription;
		}

		// Token: 0x060003B5 RID: 949 RVA: 0x0000FA6C File Offset: 0x0000DC6C
		public Vector2 MeasureChildren(Widget widget, Vector2 measureSpec, SpriteData spriteData, float renderScale)
		{
			Container container = widget as Container;
			Vector2 vector = default(Vector2);
			if (widget.ChildCount > 0)
			{
				if (this.LayoutMethod == LayoutMethod.HorizontalLeftToRight || this.LayoutMethod == LayoutMethod.HorizontalRightToLeft || this.LayoutMethod == LayoutMethod.HorizontalCentered || this.LayoutMethod == LayoutMethod.HorizontalSpaced)
				{
					vector = this.MeasureLinear(widget, measureSpec, AlignmentAxis.Horizontal);
					if (container != null && container.IsDragHovering)
					{
						vector.X += 20f;
					}
				}
				else if (this.LayoutMethod == LayoutMethod.VerticalBottomToTop || this.LayoutMethod == LayoutMethod.VerticalTopToBottom || this.LayoutMethod == LayoutMethod.VerticalCentered)
				{
					vector = this.MeasureLinear(widget, measureSpec, AlignmentAxis.Vertical);
					if (container != null && container.IsDragHovering)
					{
						vector.Y += 20f;
					}
				}
			}
			return vector;
		}

		// Token: 0x060003B6 RID: 950 RVA: 0x0000FB20 File Offset: 0x0000DD20
		public void OnLayout(Widget widget, float left, float bottom, float right, float top)
		{
			if (this.LayoutMethod == LayoutMethod.HorizontalLeftToRight || this.LayoutMethod == LayoutMethod.HorizontalRightToLeft || this.LayoutMethod == LayoutMethod.HorizontalCentered || this.LayoutMethod == LayoutMethod.HorizontalSpaced)
			{
				this.LayoutLinearHorizontalLocal(widget, left, bottom, right, top);
				return;
			}
			if (this.LayoutMethod == LayoutMethod.VerticalBottomToTop || this.LayoutMethod == LayoutMethod.VerticalTopToBottom || this.LayoutMethod == LayoutMethod.VerticalCentered)
			{
				this.LayoutLinearVertical(widget, left, bottom, right, top);
			}
		}

		// Token: 0x060003B7 RID: 951 RVA: 0x0000FB86 File Offset: 0x0000DD86
		private static float GetData(Vector2 vector2, int row)
		{
			if (row == 0)
			{
				return vector2.X;
			}
			return vector2.Y;
		}

		// Token: 0x060003B8 RID: 952 RVA: 0x0000FB98 File Offset: 0x0000DD98
		private static void SetData(ref Vector2 vector2, int row, float data)
		{
			if (row == 0)
			{
				vector2.X = data;
			}
			vector2.Y = data;
		}

		// Token: 0x060003B9 RID: 953 RVA: 0x0000FBAC File Offset: 0x0000DDAC
		public int GetIndexForDrop(Container widget, Vector2 draggedWidgetPosition)
		{
			int num = 0;
			if (this.LayoutMethod == LayoutMethod.VerticalBottomToTop || this.LayoutMethod == LayoutMethod.VerticalTopToBottom || this.LayoutMethod == LayoutMethod.VerticalCentered)
			{
				num = 1;
			}
			bool flag = this.LayoutMethod == LayoutMethod.HorizontalRightToLeft || this.LayoutMethod == LayoutMethod.VerticalTopToBottom || this.LayoutMethod == LayoutMethod.VerticalCentered;
			float data = StackLayout.GetData(draggedWidgetPosition, num);
			int num2 = 0;
			bool flag2 = false;
			int num3 = 0;
			while (num3 != widget.ChildCount && !flag2)
			{
				Widget child = widget.GetChild(num3);
				if (child != null)
				{
					float data2 = StackLayout.GetData(child.GlobalPosition * child.Context.CustomScale, num);
					float num4 = data2 + StackLayout.GetData(child.Size, num);
					float num5 = (data2 + num4) / 2f;
					if (!flag)
					{
						if (data < num5)
						{
							num2 = num3;
							flag2 = true;
						}
					}
					else if (data > num5)
					{
						num2 = num3;
						flag2 = true;
					}
				}
				num3++;
			}
			if (!flag2)
			{
				num2 = widget.ChildCount;
			}
			return num2;
		}

		// Token: 0x060003BA RID: 954 RVA: 0x0000FC8C File Offset: 0x0000DE8C
		private void ParallelMeasureBasicChild(int startInclusive, int endExclusive)
		{
			for (int i = startInclusive; i < endExclusive; i++)
			{
				Widget child = this._parallelMeasureBasicChildWidget.GetChild(i);
				if (child == null)
				{
					Debug.FailedAssert("Trying to measure a null child for parent" + this._parallelMeasureBasicChildWidget.GetFullIDPath(), "C:\\Develop\\MB3\\TaleWorlds.Shared\\Source\\GauntletUI\\TaleWorlds.GauntletUI\\Layout\\StackLayout.cs", "ParallelMeasureBasicChild", 184);
				}
				else if (child.IsVisible)
				{
					AlignmentAxis parallelMeasureBasicChildAlignmentAxis = this._parallelMeasureBasicChildAlignmentAxis;
					if (parallelMeasureBasicChildAlignmentAxis != AlignmentAxis.Horizontal)
					{
						if (parallelMeasureBasicChildAlignmentAxis == AlignmentAxis.Vertical)
						{
							if (child.HeightSizePolicy != SizePolicy.StretchToParent)
							{
								child.Measure(this._parallelMeasureBasicChildMeasureSpec);
							}
						}
					}
					else if (child.WidthSizePolicy != SizePolicy.StretchToParent)
					{
						child.Measure(this._parallelMeasureBasicChildMeasureSpec);
					}
				}
			}
		}

		// Token: 0x060003BB RID: 955 RVA: 0x0000FD2C File Offset: 0x0000DF2C
		private Vector2 MeasureLinear(Widget widget, Vector2 measureSpec, AlignmentAxis alignmentAxis)
		{
			this._parallelMeasureBasicChildWidget = widget;
			this._parallelMeasureBasicChildMeasureSpec = measureSpec;
			this._parallelMeasureBasicChildAlignmentAxis = alignmentAxis;
			TWParallel.For(0, widget.ChildCount, this._parallelMeasureBasicChildDelegate, 64);
			this._parallelMeasureBasicChildWidget = null;
			float num = 0f;
			float num2 = 0f;
			float num3 = 0f;
			int num4 = 0;
			for (int i = 0; i < widget.ChildCount; i++)
			{
				Widget child = widget.GetChild(i);
				if (child == null)
				{
					Debug.FailedAssert("Trying to measure a null child for parent" + widget.GetFullIDPath(), "C:\\Develop\\MB3\\TaleWorlds.Shared\\Source\\GauntletUI\\TaleWorlds.GauntletUI\\Layout\\StackLayout.cs", "MeasureLinear", 234);
				}
				else if (child.IsVisible)
				{
					ContainerItemDescription itemDescription = this.GetItemDescription(widget, child, i);
					if (alignmentAxis == AlignmentAxis.Horizontal)
					{
						if (child.WidthSizePolicy == SizePolicy.StretchToParent)
						{
							num4++;
							num3 += itemDescription.WidthStretchRatio;
						}
						else
						{
							num2 += child.MeasuredSize.X + child.ScaledMarginLeft + child.ScaledMarginRight;
						}
						num = MathF.Max(num, child.MeasuredSize.Y + child.ScaledMarginTop + child.ScaledMarginBottom);
					}
					else if (alignmentAxis == AlignmentAxis.Vertical)
					{
						if (child.HeightSizePolicy == SizePolicy.StretchToParent)
						{
							num4++;
							num3 += itemDescription.HeightStretchRatio;
						}
						else
						{
							num += child.MeasuredSize.Y + child.ScaledMarginTop + child.ScaledMarginBottom;
						}
						num2 = MathF.Max(num2, child.MeasuredSize.X + child.ScaledMarginLeft + child.ScaledMarginRight);
					}
				}
			}
			if (num4 > 0)
			{
				float num5 = 0f;
				if (alignmentAxis == AlignmentAxis.Horizontal)
				{
					num5 = measureSpec.X - num2;
				}
				else if (alignmentAxis == AlignmentAxis.Vertical)
				{
					num5 = measureSpec.Y - num;
				}
				float num6 = num5;
				int num7 = num4;
				for (int j = 0; j < widget.ChildCount; j++)
				{
					Widget child2 = widget.GetChild(j);
					if (child2 == null)
					{
						Debug.FailedAssert("Trying to measure a null child for parent" + widget.GetFullIDPath(), "C:\\Develop\\MB3\\TaleWorlds.Shared\\Source\\GauntletUI\\TaleWorlds.GauntletUI\\Layout\\StackLayout.cs", "MeasureLinear", 296);
					}
					else if (child2.IsVisible && ((alignmentAxis == AlignmentAxis.Horizontal && child2.WidthSizePolicy == SizePolicy.StretchToParent) || (alignmentAxis == AlignmentAxis.Vertical && child2.HeightSizePolicy == SizePolicy.StretchToParent)))
					{
						ContainerItemDescription itemDescription2 = this.GetItemDescription(widget, child2, j);
						Vector2 vector = new Vector2(0f, 0f);
						if (num6 <= 0f)
						{
							if (alignmentAxis == AlignmentAxis.Horizontal)
							{
								vector = new Vector2(0f, measureSpec.Y);
							}
							else if (alignmentAxis == AlignmentAxis.Vertical)
							{
								vector = new Vector2(measureSpec.X, 0f);
							}
						}
						else if (alignmentAxis == AlignmentAxis.Horizontal)
						{
							float num8 = num5 * itemDescription2.WidthStretchRatio / num3;
							if (num7 == 1)
							{
								num8 = num6;
							}
							vector = new Vector2(num8, measureSpec.Y);
						}
						else if (alignmentAxis == AlignmentAxis.Vertical)
						{
							float num9 = num5 * itemDescription2.HeightStretchRatio / num3;
							if (num7 == 1)
							{
								num9 = num6;
							}
							vector = new Vector2(measureSpec.X, num9);
						}
						child2.Measure(vector);
						num7--;
						if (alignmentAxis == AlignmentAxis.Horizontal)
						{
							num6 -= child2.MeasuredSize.X;
							num2 += child2.MeasuredSize.X;
							num = MathF.Max(num, child2.MeasuredSize.Y);
						}
						else if (alignmentAxis == AlignmentAxis.Vertical)
						{
							num6 -= child2.MeasuredSize.Y;
							num += child2.MeasuredSize.Y;
							num2 = MathF.Max(num2, child2.MeasuredSize.X);
						}
					}
				}
			}
			float num10 = num2;
			float num11 = num;
			return new Vector2(num10, num11);
		}

		// Token: 0x060003BC RID: 956 RVA: 0x00010090 File Offset: 0x0000E290
		private void ParallelUpdateLayouts(Widget widget)
		{
			StackLayout.<>c__DisplayClass23_0 CS$<>8__locals1 = new StackLayout.<>c__DisplayClass23_0();
			CS$<>8__locals1.widget = widget;
			CS$<>8__locals1.<>4__this = this;
			TWParallel.For(0, CS$<>8__locals1.widget.ChildCount, new TWParallel.ParallelForAuxPredicate(CS$<>8__locals1.<ParallelUpdateLayouts>g__UpdateChildLayoutMT|0), 16);
		}

		// Token: 0x060003BD RID: 957 RVA: 0x000100D0 File Offset: 0x0000E2D0
		private void LayoutLinearHorizontalLocal(Widget widget, float left, float bottom, float right, float top)
		{
			Container container = widget as Container;
			float num = 0f;
			float num2 = 0f;
			float num3 = right - left;
			float num4 = bottom - top;
			if (this.LayoutMethod != LayoutMethod.HorizontalRightToLeft && this.LayoutMethod == LayoutMethod.HorizontalCentered)
			{
				float num5 = 0f;
				for (int i = 0; i < widget.ChildCount; i++)
				{
					Widget child = widget.GetChild(i);
					if (child == null)
					{
						Debug.FailedAssert("Trying to measure a null child for parent" + widget.GetFullIDPath(), "C:\\Develop\\MB3\\TaleWorlds.Shared\\Source\\GauntletUI\\TaleWorlds.GauntletUI\\Layout\\StackLayout.cs", "LayoutLinearHorizontalLocal", 417);
					}
					else if (child.IsVisible)
					{
						num5 += child.MeasuredSize.X + child.ScaledMarginLeft + child.ScaledMarginRight;
					}
				}
				num = (right - left) / 2f - num5 / 2f;
			}
			this._layoutBoxes.Clear();
			int num6 = 0;
			for (int j = 0; j < widget.ChildCount; j++)
			{
				if (widget.Children[j].IsVisible)
				{
					num6++;
				}
			}
			if (num6 > 0)
			{
				for (int k = 0; k < widget.ChildCount; k++)
				{
					Widget widget2 = widget.Children[k];
					if (widget2 == null)
					{
						Debug.FailedAssert("Trying to measure a null child for parent" + widget.GetFullIDPath(), "C:\\Develop\\MB3\\TaleWorlds.Shared\\Source\\GauntletUI\\TaleWorlds.GauntletUI\\Layout\\StackLayout.cs", "LayoutLinearHorizontalLocal", 448);
					}
					else if (widget2.IsVisible)
					{
						float num7 = widget2.MeasuredSize.X + widget2.ScaledMarginLeft + widget2.ScaledMarginRight;
						if (container != null && container.IsDragHovering && k == container.DragHoverInsertionIndex)
						{
							num7 += 20f;
						}
						if (this.LayoutMethod == LayoutMethod.HorizontalRightToLeft)
						{
							num = num3 - num7;
						}
						else if (this.LayoutMethod == LayoutMethod.HorizontalSpaced)
						{
							if (num6 > 1)
							{
								if (k == 0)
								{
									num = 0f;
									num3 = left + widget2.MeasuredSize.X;
								}
								else if (k == widget.ChildCount - 1)
								{
									num3 = right - left;
									num = num3 - widget2.MeasuredSize.X;
								}
								else
								{
									float num8 = (widget.MeasuredSize.X - widget2.MeasuredSize.X * (float)num6) / (float)(num6 - 1);
									num += widget2.MeasuredSize.X + num8;
									num3 = num + widget2.MeasuredSize.X;
								}
							}
							else
							{
								num = widget.MeasuredSize.X / 2f - widget2.MeasuredSize.X / 2f;
								num3 = num + widget2.MeasuredSize.X / 2f;
							}
						}
						else
						{
							num3 = num + num7;
						}
						if (widget.ChildCount < 64)
						{
							widget2.Layout(num, num4, num3, num2);
						}
						else
						{
							LayoutBox layoutBox = default(LayoutBox);
							layoutBox.Left = num;
							layoutBox.Right = num3;
							layoutBox.Bottom = num4;
							layoutBox.Top = num2;
							this._layoutBoxes.Add(k, layoutBox);
						}
						if (this.LayoutMethod == LayoutMethod.HorizontalRightToLeft)
						{
							num3 = num;
						}
						else if (this.LayoutMethod == LayoutMethod.HorizontalLeftToRight || this.LayoutMethod == LayoutMethod.HorizontalCentered)
						{
							num = num3;
						}
					}
					else
					{
						this._layoutBoxes.Add(k, default(LayoutBox));
					}
				}
			}
			if (widget.ChildCount >= 64)
			{
				this.ParallelUpdateLayouts(widget);
			}
		}

		// Token: 0x060003BE RID: 958 RVA: 0x00010414 File Offset: 0x0000E614
		private void LayoutLinearVertical(Widget widget, float left, float bottom, float right, float top)
		{
			Container container = widget as Container;
			float num = 0f;
			float num2 = 0f;
			float num3 = bottom - top;
			float num4 = right - left;
			if (this.LayoutMethod != LayoutMethod.VerticalTopToBottom && this.LayoutMethod == LayoutMethod.VerticalCentered)
			{
				float num5 = 0f;
				for (int i = 0; i < widget.ChildCount; i++)
				{
					Widget child = widget.GetChild(i);
					if (child != null && child.IsVisible)
					{
						num5 += child.MeasuredSize.Y + child.ScaledMarginTop + child.ScaledMarginBottom;
					}
				}
				num3 = (bottom - top) / 2f - num5 / 2f;
				num2 = (bottom - top) / 2f + num5 / 2f;
			}
			this._layoutBoxes.Clear();
			for (int j = 0; j < widget.ChildCount; j++)
			{
				Widget child2 = widget.GetChild(j);
				if (child2 != null && child2.IsVisible)
				{
					if (container != null && container.IsDragHovering && j == container.DragHoverInsertionIndex)
					{
						if (this.LayoutMethod == LayoutMethod.VerticalBottomToTop)
						{
							num2 += 20f;
						}
						else
						{
							num3 -= 20f;
						}
					}
					float num6 = child2.MeasuredSize.Y + child2.ScaledMarginTop + child2.ScaledMarginBottom;
					if (this.LayoutMethod == LayoutMethod.VerticalBottomToTop)
					{
						num3 = num2 + num6;
					}
					else
					{
						num2 = num3 - num6;
					}
					if (widget.ChildCount < 64)
					{
						child2.Layout(num, num3, num4, num2);
					}
					else
					{
						LayoutBox layoutBox = default(LayoutBox);
						layoutBox.Left = num;
						layoutBox.Right = num4;
						layoutBox.Bottom = num3;
						layoutBox.Top = num2;
						this._layoutBoxes.Add(j, layoutBox);
					}
					if (this.LayoutMethod == LayoutMethod.VerticalBottomToTop)
					{
						num2 = num3;
					}
					else
					{
						num3 = num2;
					}
				}
				else
				{
					this._layoutBoxes.Add(j, default(LayoutBox));
				}
			}
			if (widget.ChildCount >= 64)
			{
				this.ParallelUpdateLayouts(widget);
			}
		}

		// Token: 0x060003BF RID: 959 RVA: 0x00010600 File Offset: 0x0000E800
		public Vector2 GetDropGizmoPosition(Container widget, Vector2 draggedWidgetPosition)
		{
			int num = 0;
			if (this.LayoutMethod == LayoutMethod.VerticalBottomToTop || this.LayoutMethod == LayoutMethod.VerticalTopToBottom || this.LayoutMethod == LayoutMethod.VerticalCentered)
			{
				num = 1;
			}
			bool flag = this.LayoutMethod == LayoutMethod.HorizontalRightToLeft || this.LayoutMethod == LayoutMethod.VerticalTopToBottom || this.LayoutMethod == LayoutMethod.VerticalCentered;
			int indexForDrop = this.GetIndexForDrop(widget, draggedWidgetPosition);
			int num2 = indexForDrop - 1;
			Vector2 globalPosition = widget.GlobalPosition;
			Vector2 globalPosition2 = widget.GlobalPosition;
			if (!flag)
			{
				if (num2 >= 0 && num2 < widget.ChildCount)
				{
					Widget child = widget.GetChild(num2);
					StackLayout.SetData(ref globalPosition, num, StackLayout.GetData(child.GlobalPosition, num) + StackLayout.GetData(child.Size, num));
				}
				if (indexForDrop >= 0 && indexForDrop < widget.ChildCount)
				{
					StackLayout.SetData(ref globalPosition2, num, StackLayout.GetData(widget.GetChild(indexForDrop).GlobalPosition, num));
				}
				else if (indexForDrop >= widget.ChildCount && widget.ChildCount > 0)
				{
					StackLayout.SetData(ref globalPosition2, num, StackLayout.GetData(globalPosition, num) + 20f);
				}
			}
			else
			{
				StackLayout.SetData(ref globalPosition, num, StackLayout.GetData(globalPosition, num) + StackLayout.GetData(widget.Size, num));
				StackLayout.SetData(ref globalPosition2, num, StackLayout.GetData(globalPosition2, num) + StackLayout.GetData(widget.Size, num));
				if (num2 >= 0 && num2 < widget.ChildCount)
				{
					Widget child2 = widget.GetChild(num2);
					StackLayout.SetData(ref globalPosition, num, StackLayout.GetData(child2.GlobalPosition, num));
				}
				if (indexForDrop >= 0 && indexForDrop < widget.ChildCount)
				{
					Widget child3 = widget.GetChild(indexForDrop);
					StackLayout.SetData(ref globalPosition2, num, StackLayout.GetData(child3.GlobalPosition, num) + StackLayout.GetData(child3.Size, num));
				}
				else if (indexForDrop >= widget.ChildCount && widget.ChildCount > 0)
				{
					StackLayout.SetData(ref globalPosition2, num, StackLayout.GetData(globalPosition, num) - 20f);
				}
			}
			return new Vector2((globalPosition.X + globalPosition2.X) / 2f, (globalPosition.Y + globalPosition2.Y) / 2f);
		}

		// Token: 0x040001E9 RID: 489
		private const int DragHoverAperture = 20;

		// Token: 0x040001EA RID: 490
		private readonly Dictionary<int, LayoutBox> _layoutBoxes;

		// Token: 0x040001EB RID: 491
		private Widget _parallelMeasureBasicChildWidget;

		// Token: 0x040001EC RID: 492
		private Vector2 _parallelMeasureBasicChildMeasureSpec;

		// Token: 0x040001ED RID: 493
		private AlignmentAxis _parallelMeasureBasicChildAlignmentAxis;

		// Token: 0x040001EE RID: 494
		private TWParallel.ParallelForAuxPredicate _parallelMeasureBasicChildDelegate;
	}
}
