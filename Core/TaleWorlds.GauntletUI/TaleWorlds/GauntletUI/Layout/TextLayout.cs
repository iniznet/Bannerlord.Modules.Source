using System;
using System.Numerics;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.GauntletUI.Layout
{
	// Token: 0x02000040 RID: 64
	public class TextLayout : ILayout
	{
		// Token: 0x060003C0 RID: 960 RVA: 0x000107F5 File Offset: 0x0000E9F5
		public TextLayout(IText text)
		{
			this._defaultLayout = new DefaultLayout();
			this._text = text;
		}

		// Token: 0x060003C1 RID: 961 RVA: 0x00010810 File Offset: 0x0000EA10
		Vector2 ILayout.MeasureChildren(Widget widget, Vector2 measureSpec, SpriteData spriteData, float renderScale)
		{
			Vector2 vector = this._defaultLayout.MeasureChildren(widget, measureSpec, spriteData, renderScale);
			bool flag = widget.WidthSizePolicy != SizePolicy.CoverChildren || widget.MaxWidth != 0f;
			bool flag2 = widget.HeightSizePolicy != SizePolicy.CoverChildren || widget.MaxHeight != 0f;
			float x = measureSpec.X;
			float y = measureSpec.Y;
			Vector2 preferredSize = this._text.GetPreferredSize(flag, x, flag2, y, spriteData, renderScale);
			if (vector.X < preferredSize.X)
			{
				vector.X = preferredSize.X;
			}
			if (vector.Y < preferredSize.Y)
			{
				vector.Y = preferredSize.Y;
			}
			return vector;
		}

		// Token: 0x060003C2 RID: 962 RVA: 0x000108C7 File Offset: 0x0000EAC7
		void ILayout.OnLayout(Widget widget, float left, float bottom, float right, float top)
		{
			this._defaultLayout.OnLayout(widget, left, bottom, right, top);
		}

		// Token: 0x040001EF RID: 495
		private ILayout _defaultLayout;

		// Token: 0x040001F0 RID: 496
		private IText _text;
	}
}
