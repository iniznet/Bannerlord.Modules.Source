using System;
using System.Numerics;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.GauntletUI.Layout
{
	public class DragCarrierLayout : ILayout
	{
		Vector2 ILayout.MeasureChildren(Widget widget, Vector2 measureSpec, SpriteData spriteData, float renderScale)
		{
			Widget child = widget.GetChild(0);
			child.Measure(measureSpec);
			return child.MeasuredSize;
		}

		void ILayout.OnLayout(Widget widget, float left, float bottom, float right, float top)
		{
			float num = 0f;
			float num2 = 0f;
			float num3 = right - left;
			float num4 = bottom - top;
			widget.GetChild(0).Layout(num, num4, num3, num2);
		}
	}
}
