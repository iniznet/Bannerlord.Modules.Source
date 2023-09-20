using System;

namespace TaleWorlds.TwoDimension
{
	internal struct TwoDimensionDrawData
	{
		public Rectangle Rectangle
		{
			get
			{
				return this._rectangle;
			}
		}

		public TwoDimensionDrawData(bool scissorTestEnabled, ScissorTestInfo scissorTestInfo, float x, float y, Material material, DrawObject2D drawObject2D, float width, float height)
		{
			this._scissorTestEnabled = scissorTestEnabled;
			this._scissorTestInfo = scissorTestInfo;
			this._x = x;
			this._y = y;
			this._material = material;
			this._drawObject2D = drawObject2D;
			this._rectangle = new Rectangle(this._x, this._y, width, height);
		}

		public bool IsIntersects(Rectangle rectangle)
		{
			return this._rectangle.IsCollide(rectangle);
		}

		public void DrawTo(TwoDimensionContext twoDimensionContext, int layer)
		{
			if (this._scissorTestEnabled)
			{
				twoDimensionContext.SetScissor(this._scissorTestInfo);
			}
			twoDimensionContext.Draw(this._x, this._y, this._material, this._drawObject2D, layer);
			if (this._scissorTestEnabled)
			{
				twoDimensionContext.ResetScissor();
			}
		}

		private bool _scissorTestEnabled;

		private ScissorTestInfo _scissorTestInfo;

		private float _x;

		private float _y;

		private Material _material;

		private DrawObject2D _drawObject2D;

		private Rectangle _rectangle;
	}
}
