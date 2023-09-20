using System;
using System.Numerics;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.GauntletUI.Layout
{
	// Token: 0x0200003C RID: 60
	public interface ILayout
	{
		// Token: 0x060003AD RID: 941
		Vector2 MeasureChildren(Widget widget, Vector2 measureSpec, SpriteData spriteData, float renderScale);

		// Token: 0x060003AE RID: 942
		void OnLayout(Widget widget, float left, float bottom, float right, float top);
	}
}
