using System;
using System.Numerics;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.GauntletUI.Layout
{
	public class TextLayout : ILayout
	{
		public TextLayout(IText text)
		{
			this._defaultLayout = new DefaultLayout();
			this._text = text;
		}

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

		void ILayout.OnLayout(Widget widget, float left, float bottom, float right, float top)
		{
			this._defaultLayout.OnLayout(widget, left, bottom, right, top);
		}

		private ILayout _defaultLayout;

		private IText _text;
	}
}
