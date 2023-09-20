using System;
using System.Numerics;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.GauntletUI.Layout
{
	// Token: 0x02000038 RID: 56
	public class DragCarrierLayout : ILayout
	{
		// Token: 0x060003A3 RID: 931 RVA: 0x0000F61A File Offset: 0x0000D81A
		Vector2 ILayout.MeasureChildren(Widget widget, Vector2 measureSpec, SpriteData spriteData, float renderScale)
		{
			Widget child = widget.GetChild(0);
			child.Measure(measureSpec);
			return child.MeasuredSize;
		}

		// Token: 0x060003A4 RID: 932 RVA: 0x0000F630 File Offset: 0x0000D830
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
