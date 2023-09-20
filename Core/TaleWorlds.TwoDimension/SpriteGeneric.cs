using System;
using System.Linq;

namespace TaleWorlds.TwoDimension
{
	// Token: 0x02000031 RID: 49
	public class SpriteGeneric : Sprite
	{
		// Token: 0x170000A9 RID: 169
		// (get) Token: 0x06000207 RID: 519 RVA: 0x000084B5 File Offset: 0x000066B5
		public override Texture Texture
		{
			get
			{
				return this.SpritePart.Texture;
			}
		}

		// Token: 0x170000AA RID: 170
		// (get) Token: 0x06000208 RID: 520 RVA: 0x000084C2 File Offset: 0x000066C2
		// (set) Token: 0x06000209 RID: 521 RVA: 0x000084CA File Offset: 0x000066CA
		public SpritePart SpritePart { get; private set; }

		// Token: 0x0600020A RID: 522 RVA: 0x000084D4 File Offset: 0x000066D4
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

		// Token: 0x0600020B RID: 523 RVA: 0x00008555 File Offset: 0x00006755
		public override float GetScaleToUse(float width, float height, float scale)
		{
			return scale;
		}

		// Token: 0x0600020C RID: 524 RVA: 0x00008558 File Offset: 0x00006758
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

		// Token: 0x04000111 RID: 273
		private float[] _vertices;

		// Token: 0x04000112 RID: 274
		private float[] _uvs;

		// Token: 0x04000113 RID: 275
		private uint[] _indices;
	}
}
