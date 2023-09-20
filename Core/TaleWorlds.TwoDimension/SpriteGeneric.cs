using System;
using System.Linq;

namespace TaleWorlds.TwoDimension
{
	public class SpriteGeneric : Sprite
	{
		public override Texture Texture
		{
			get
			{
				return this.SpritePart.Texture;
			}
		}

		public SpritePart SpritePart { get; private set; }

		public SpriteGeneric(string name, SpritePart spritePart)
			: base(name, spritePart.Width, spritePart.Height)
		{
			this.SpritePart = spritePart;
			this._vertices = new float[8];
			this._uvs = new float[8];
			this._indices = new uint[6];
			this._indices[0] = 0U;
			this._indices[1] = 1U;
			this._indices[2] = 2U;
			this._indices[3] = 0U;
			this._indices[4] = 2U;
			this._indices[5] = 3U;
		}

		public override float GetScaleToUse(float width, float height, float scale)
		{
			return scale;
		}

		protected internal override DrawObject2D GetArrays(SpriteDrawData spriteDrawData)
		{
			if (this.CachedDrawObject != null && this.CachedDrawData == spriteDrawData)
			{
				return this.CachedDrawObject;
			}
			if (spriteDrawData.MapX == 0f && spriteDrawData.MapY == 0f)
			{
				float num = spriteDrawData.Width / (float)this.SpritePart.Width;
				float num2 = spriteDrawData.Height / (float)this.SpritePart.Height;
				float num3 = (float)base.Width * 1f * num;
				float num4 = (float)base.Height * 1f * num2;
				this.SpritePart.DrawSpritePart(spriteDrawData.MapX, spriteDrawData.MapY, this._vertices, this._uvs, 0, 0, 1f, spriteDrawData.Width, spriteDrawData.Height, spriteDrawData.HorizontalFlip, spriteDrawData.VerticalFlip);
				DrawObject2D drawObject2D = new DrawObject2D(MeshTopology.Triangles, this._vertices.ToArray<float>(), this._uvs.ToArray<float>(), this._indices.ToArray<uint>(), 6);
				drawObject2D.DrawObjectType = DrawObjectType.Quad;
				drawObject2D.Width = num3;
				drawObject2D.Height = num4;
				drawObject2D.MinU = this.SpritePart.MinU;
				drawObject2D.MaxU = this.SpritePart.MaxU;
				if (spriteDrawData.HorizontalFlip)
				{
					drawObject2D.MinU = this.SpritePart.MaxU;
					drawObject2D.MaxU = this.SpritePart.MinU;
				}
				drawObject2D.MinV = this.SpritePart.MinV;
				drawObject2D.MaxV = this.SpritePart.MaxV;
				if (spriteDrawData.VerticalFlip)
				{
					drawObject2D.MinV = this.SpritePart.MaxV;
					drawObject2D.MaxV = this.SpritePart.MinV;
				}
				this.CachedDrawData = spriteDrawData;
				this.CachedDrawObject = drawObject2D;
				return drawObject2D;
			}
			this.SpritePart.DrawSpritePart(spriteDrawData.MapX, spriteDrawData.MapY, this._vertices, this._uvs, 0, 0, 1f, spriteDrawData.Width, spriteDrawData.Height, spriteDrawData.HorizontalFlip, spriteDrawData.VerticalFlip);
			DrawObject2D drawObject2D2 = new DrawObject2D(MeshTopology.Triangles, this._vertices.ToArray<float>(), this._uvs.ToArray<float>(), this._indices.ToArray<uint>(), 6);
			drawObject2D2.DrawObjectType = DrawObjectType.Mesh;
			this.CachedDrawData = spriteDrawData;
			this.CachedDrawObject = drawObject2D2;
			return drawObject2D2;
		}

		private float[] _vertices;

		private float[] _uvs;

		private uint[] _indices;
	}
}
