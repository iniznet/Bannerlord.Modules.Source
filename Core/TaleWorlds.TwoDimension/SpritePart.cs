using System;
using TaleWorlds.Library;

namespace TaleWorlds.TwoDimension
{
	public class SpritePart
	{
		public string Name { get; private set; }

		public int Width { get; private set; }

		public int Height { get; private set; }

		public int SheetID { get; set; }

		public int SheetX { get; set; }

		public int SheetY { get; set; }

		public float MinU { get; private set; }

		public float MinV { get; private set; }

		public float MaxU { get; private set; }

		public float MaxV { get; private set; }

		public int SheetWidth { get; private set; }

		public int SheetHeight { get; private set; }

		public Texture Texture
		{
			get
			{
				if (this._category != null && this._category.IsLoaded && this._category.SpriteSheets != null && this._category.SpriteSheets.Count >= this.SheetID)
				{
					return this._category.SpriteSheets[this.SheetID - 1];
				}
				return null;
			}
		}

		public SpriteCategory Category
		{
			get
			{
				return this._category;
			}
		}

		public SpritePart(string name, SpriteCategory category, int width, int height)
		{
			this.Name = name;
			this.Width = width;
			this.Height = height;
			this._category = category;
			this._category.SpriteParts.Add(this);
		}

		public void UpdateInitValues()
		{
			Vec2i vec2i = this._category.SheetSizes[this.SheetID - 1];
			this.SheetWidth = vec2i.X;
			this.SheetHeight = vec2i.Y;
			double num = 1.0 / (double)this.SheetWidth;
			double num2 = 1.0 / (double)this.SheetHeight;
			double num3 = (double)this.SheetX * num;
			double num4 = (double)(this.SheetX + this.Width) * num;
			double num5 = (double)this.SheetY * num2;
			double num6 = (double)(this.SheetY + this.Height) * num2;
			this.MinU = (float)num3;
			this.MaxU = (float)num4;
			this.MinV = (float)num5;
			this.MaxV = (float)num6;
		}

		public void DrawSpritePart(float screenX, float screenY, float[] outVertices, float[] outUvs, int verticesStartIndex, int uvsStartIndex)
		{
			this.DrawSpritePart(screenX, screenY, outVertices, outUvs, verticesStartIndex, uvsStartIndex, 1f, (float)this.Width, (float)this.Height, false, false);
		}

		public void DrawSpritePart(float screenX, float screenY, float[] outVertices, float[] outUvs, int verticesStartIndex, int uvsStartIndex, float scale, float customWidth, float customHeight, bool horizontalFlip, bool verticalFlip)
		{
			if (this.Texture != null)
			{
				float num = customWidth / (float)this.Width;
				float num2 = customHeight / (float)this.Height;
				float num3 = (float)this.Width * scale * num;
				float num4 = (float)this.Height * scale * num2;
				outVertices[verticesStartIndex] = screenX + 0f;
				outVertices[verticesStartIndex + 1] = screenY + 0f;
				outVertices[verticesStartIndex + 2] = screenX + 0f;
				outVertices[verticesStartIndex + 3] = screenY + num4;
				outVertices[verticesStartIndex + 4] = screenX + num3;
				outVertices[verticesStartIndex + 5] = screenY + num4;
				outVertices[verticesStartIndex + 6] = screenX + num3;
				outVertices[verticesStartIndex + 7] = screenY + 0f;
				this.FillTextureCoordinates(outUvs, uvsStartIndex, horizontalFlip, verticalFlip);
			}
		}

		public void FillTextureCoordinates(float[] outUVs, int uvsStartIndex, bool horizontalFlip, bool verticalFlip)
		{
			float num = this.MinU;
			float num2 = this.MaxU;
			if (horizontalFlip)
			{
				num = this.MaxU;
				num2 = this.MinU;
			}
			float num3 = this.MinV;
			float num4 = this.MaxV;
			if (verticalFlip)
			{
				num3 = this.MaxV;
				num4 = this.MinV;
			}
			outUVs[uvsStartIndex] = num;
			outUVs[uvsStartIndex + 1] = num3;
			outUVs[uvsStartIndex + 2] = num;
			outUVs[uvsStartIndex + 3] = num4;
			outUVs[uvsStartIndex + 4] = num2;
			outUVs[uvsStartIndex + 5] = num4;
			outUVs[uvsStartIndex + 6] = num2;
			outUVs[uvsStartIndex + 7] = num3;
		}

		private SpriteCategory _category;
	}
}
