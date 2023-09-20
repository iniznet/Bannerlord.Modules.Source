using System;

namespace TaleWorlds.TwoDimension
{
	// Token: 0x0200003A RID: 58
	internal struct TwoDimensionDrawData
	{
		// Token: 0x170000D7 RID: 215
		// (get) Token: 0x0600028B RID: 651 RVA: 0x00009EA9 File Offset: 0x000080A9
		public Rectangle Rectangle
		{
			get
			{
				return this._rectangle;
			}
		}

		// Token: 0x0600028C RID: 652 RVA: 0x00009EB4 File Offset: 0x000080B4
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

		// Token: 0x0600028D RID: 653 RVA: 0x00009F09 File Offset: 0x00008109
		public bool IsIntersects(Rectangle rectangle)
		{
			return this._rectangle.IsCollide(rectangle);
		}

		// Token: 0x0600028E RID: 654 RVA: 0x00009F18 File Offset: 0x00008118
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

		// Token: 0x04000149 RID: 329
		private bool _scissorTestEnabled;

		// Token: 0x0400014A RID: 330
		private ScissorTestInfo _scissorTestInfo;

		// Token: 0x0400014B RID: 331
		private float _x;

		// Token: 0x0400014C RID: 332
		private float _y;

		// Token: 0x0400014D RID: 333
		private Material _material;

		// Token: 0x0400014E RID: 334
		private DrawObject2D _drawObject2D;

		// Token: 0x0400014F RID: 335
		private Rectangle _rectangle;
	}
}
