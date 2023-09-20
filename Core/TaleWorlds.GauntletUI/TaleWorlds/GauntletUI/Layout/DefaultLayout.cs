using System;
using System.Numerics;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.GauntletUI.Layout
{
	public class DefaultLayout : ILayout
	{
		private void ParallelMeasureChildren(Widget widget, Vector2 measureSpec)
		{
			DefaultLayout.<>c__DisplayClass0_0 CS$<>8__locals1 = new DefaultLayout.<>c__DisplayClass0_0();
			CS$<>8__locals1.widget = widget;
			CS$<>8__locals1.measureSpec = measureSpec;
			TWParallel.For(0, CS$<>8__locals1.widget.ChildCount, new TWParallel.ParallelForAuxPredicate(CS$<>8__locals1.<ParallelMeasureChildren>g__UpdateChildWidgetMT|0), 16);
		}

		Vector2 ILayout.MeasureChildren(Widget widget, Vector2 measureSpec, SpriteData spriteData, float renderScale)
		{
			Vector2 vector = default(Vector2);
			if (widget.ChildCount > 0)
			{
				if (widget.ChildCount >= 64)
				{
					this.ParallelMeasureChildren(widget, measureSpec);
				}
				for (int i = 0; i < widget.ChildCount; i++)
				{
					Widget child = widget.GetChild(i);
					if (child != null && child.IsVisible)
					{
						if (widget.ChildCount < 64)
						{
							child.Measure(measureSpec);
						}
						Vector2 measuredSize = child.MeasuredSize;
						measuredSize.X += child.ScaledMarginLeft + child.ScaledMarginRight;
						measuredSize.Y += child.ScaledMarginTop + child.ScaledMarginBottom;
						if (measuredSize.X > vector.X)
						{
							vector.X = measuredSize.X;
						}
						if (measuredSize.Y > vector.Y)
						{
							vector.Y = measuredSize.Y;
						}
					}
				}
			}
			return vector;
		}

		void ILayout.OnLayout(Widget widget, float left, float bottom, float right, float top)
		{
			float num = 0f;
			float num2 = 0f;
			float num3 = right - left;
			float num4 = bottom - top;
			for (int i = 0; i < widget.ChildCount; i++)
			{
				Widget child = widget.GetChild(i);
				if (child != null && child.IsVisible)
				{
					child.Layout(num, num4, num3, num2);
				}
			}
		}
	}
}
