using System;
using System.Collections.Generic;

namespace TaleWorlds.TwoDimension
{
	// Token: 0x02000032 RID: 50
	public class SpriteNineRegion : Sprite
	{
		// Token: 0x170000AB RID: 171
		// (get) Token: 0x0600020D RID: 525 RVA: 0x000087A5 File Offset: 0x000069A5
		public override Texture Texture
		{
			get
			{
				return this.BaseSprite.Texture;
			}
		}

		// Token: 0x170000AC RID: 172
		// (get) Token: 0x0600020E RID: 526 RVA: 0x000087B2 File Offset: 0x000069B2
		// (set) Token: 0x0600020F RID: 527 RVA: 0x000087BA File Offset: 0x000069BA
		public SpritePart BaseSprite { get; private set; }

		// Token: 0x170000AD RID: 173
		// (get) Token: 0x06000210 RID: 528 RVA: 0x000087C3 File Offset: 0x000069C3
		// (set) Token: 0x06000211 RID: 529 RVA: 0x000087CB File Offset: 0x000069CB
		public int LeftWidth { get; private set; }

		// Token: 0x170000AE RID: 174
		// (get) Token: 0x06000212 RID: 530 RVA: 0x000087D4 File Offset: 0x000069D4
		// (set) Token: 0x06000213 RID: 531 RVA: 0x000087DC File Offset: 0x000069DC
		public int RightWidth { get; private set; }

		// Token: 0x170000AF RID: 175
		// (get) Token: 0x06000214 RID: 532 RVA: 0x000087E5 File Offset: 0x000069E5
		// (set) Token: 0x06000215 RID: 533 RVA: 0x000087ED File Offset: 0x000069ED
		public int TopHeight { get; private set; }

		// Token: 0x170000B0 RID: 176
		// (get) Token: 0x06000216 RID: 534 RVA: 0x000087F6 File Offset: 0x000069F6
		// (set) Token: 0x06000217 RID: 535 RVA: 0x000087FE File Offset: 0x000069FE
		public int BottomHeight { get; private set; }

		// Token: 0x06000218 RID: 536 RVA: 0x00008808 File Offset: 0x00006A08
		public SpriteNineRegion(string name, SpritePart baseSprite, int leftWidth, int rightWidth, int topHeight, int bottomHeight)
			: base(name, baseSprite.Width, baseSprite.Height)
		{
			this.BaseSprite = baseSprite;
			this.LeftWidth = leftWidth;
			this.RightWidth = rightWidth;
			this.TopHeight = topHeight;
			this.BottomHeight = bottomHeight;
			this._centerWidth = baseSprite.Width - leftWidth - rightWidth;
			this._centerHeight = baseSprite.Height - topHeight - bottomHeight;
		}

		// Token: 0x06000219 RID: 537 RVA: 0x00008874 File Offset: 0x00006A74
		public override float GetScaleToUse(float width, float height, float scale)
		{
			float num = 1f;
			float num2 = (float)(this.LeftWidth + this.RightWidth) * scale;
			if (width < num2)
			{
				num = width / num2;
			}
			float num3 = (float)(this.TopHeight + this.BottomHeight) * scale;
			float num4 = 1f;
			if (height < num3)
			{
				num4 = height / num3;
			}
			float num5 = ((num < num4) ? num : num4);
			if (this._centerWidth == 0)
			{
				num = width / num2;
				num5 = ((num < num5) ? num5 : num);
			}
			if (this._centerHeight == 0)
			{
				num4 = height / num3;
				num5 = ((num4 < num5) ? num5 : num4);
			}
			return scale * num5;
		}

		// Token: 0x0600021A RID: 538 RVA: 0x00008900 File Offset: 0x00006B00
		protected internal override DrawObject2D GetArrays(SpriteDrawData spriteDrawData)
		{
			if (this.CachedDrawObject != null && this.CachedDrawData == spriteDrawData)
			{
				return this.CachedDrawObject;
			}
			this._outVertices = new float[32];
			this._outIndices = new uint[54];
			this._verticesStartIndex = 0;
			this._uvsStartIndex = 0;
			this._indicesStartIndex = 0U;
			this._scale = this.GetScaleToUse(spriteDrawData.Width, spriteDrawData.Height, spriteDrawData.Scale);
			this._customWidth = spriteDrawData.Width;
			this._customHeight = spriteDrawData.Height;
			this.SetVerticesData(spriteDrawData.HorizontalFlip, spriteDrawData.VerticalFlip);
			this.SetIndicesData();
			if (this._outUvs == null)
			{
				this._outUvs = new List<float[]>();
				this._outUvs.Add(new float[32]);
				this._outUvs.Add(new float[32]);
				this._outUvs.Add(new float[32]);
				this._outUvs.Add(new float[32]);
				this.CalculateTextureCoordinates(this._outUvs[this.GetUVArrayIndex(false, false)], false, false);
				this.CalculateTextureCoordinates(this._outUvs[this.GetUVArrayIndex(true, false)], true, false);
				this.CalculateTextureCoordinates(this._outUvs[this.GetUVArrayIndex(false, true)], false, true);
				this.CalculateTextureCoordinates(this._outUvs[this.GetUVArrayIndex(true, true)], true, true);
			}
			for (int i = 0; i < 16; i++)
			{
				this._outVertices[2 * i] += spriteDrawData.MapX;
				this._outVertices[2 * i + 1] += spriteDrawData.MapY;
			}
			DrawObject2D drawObject2D = new DrawObject2D(MeshTopology.Triangles, this._outVertices, this._outUvs[this.GetUVArrayIndex(spriteDrawData.HorizontalFlip, spriteDrawData.VerticalFlip)], this._outIndices, this._outIndices.Length);
			drawObject2D.DrawObjectType = DrawObjectType.NineGrid;
			this._outVertices = null;
			this._outIndices = null;
			this.CachedDrawObject = drawObject2D;
			this.CachedDrawData = spriteDrawData;
			return drawObject2D;
		}

		// Token: 0x0600021B RID: 539 RVA: 0x00008B0C File Offset: 0x00006D0C
		private int GetUVArrayIndex(bool horizontalFlip, bool verticalFlip)
		{
			int num;
			if (horizontalFlip && verticalFlip)
			{
				num = 3;
			}
			else if (verticalFlip)
			{
				num = 2;
			}
			else if (horizontalFlip)
			{
				num = 1;
			}
			else
			{
				num = 0;
			}
			return num;
		}

		// Token: 0x0600021C RID: 540 RVA: 0x00008B35 File Offset: 0x00006D35
		private void SetVertexData(float x, float y)
		{
			this._outVertices[this._verticesStartIndex] = x;
			this._verticesStartIndex++;
			this._outVertices[this._verticesStartIndex] = y;
			this._verticesStartIndex++;
		}

		// Token: 0x0600021D RID: 541 RVA: 0x00008B6F File Offset: 0x00006D6F
		private void SetTextureData(float[] outUvs, float u, float v)
		{
			outUvs[this._uvsStartIndex] = u;
			this._uvsStartIndex++;
			outUvs[this._uvsStartIndex] = v;
			this._uvsStartIndex++;
		}

		// Token: 0x0600021E RID: 542 RVA: 0x00008BA0 File Offset: 0x00006DA0
		private void AddQuad(uint i1, uint i2, uint i3, uint i4)
		{
			this._outIndices[(int)this._indicesStartIndex] = i1;
			this._indicesStartIndex += 1U;
			this._outIndices[(int)this._indicesStartIndex] = i2;
			this._indicesStartIndex += 1U;
			this._outIndices[(int)this._indicesStartIndex] = i4;
			this._indicesStartIndex += 1U;
			this._outIndices[(int)this._indicesStartIndex] = i1;
			this._indicesStartIndex += 1U;
			this._outIndices[(int)this._indicesStartIndex] = i4;
			this._indicesStartIndex += 1U;
			this._outIndices[(int)this._indicesStartIndex] = i3;
			this._indicesStartIndex += 1U;
		}

		// Token: 0x0600021F RID: 543 RVA: 0x00008C58 File Offset: 0x00006E58
		private void SetIndicesData()
		{
			this.AddQuad(0U, 1U, 4U, 5U);
			this.AddQuad(1U, 2U, 5U, 6U);
			this.AddQuad(2U, 3U, 6U, 7U);
			this.AddQuad(4U, 5U, 8U, 9U);
			this.AddQuad(5U, 6U, 9U, 10U);
			this.AddQuad(6U, 7U, 10U, 11U);
			this.AddQuad(8U, 9U, 12U, 13U);
			this.AddQuad(9U, 10U, 13U, 14U);
			this.AddQuad(10U, 11U, 14U, 15U);
		}

		// Token: 0x06000220 RID: 544 RVA: 0x00008CD0 File Offset: 0x00006ED0
		private void SetVerticesData(bool horizontalFlip, bool verticalFlip)
		{
			float num = (float)this.LeftWidth;
			float num2 = (float)this.RightWidth;
			float num3 = (float)this.TopHeight;
			float num4 = (float)this.BottomHeight;
			if (horizontalFlip)
			{
				num = (float)this.RightWidth;
				num2 = (float)this.LeftWidth;
			}
			if (verticalFlip)
			{
				num3 = (float)this.BottomHeight;
				num4 = (float)this.TopHeight;
			}
			float num5 = 0f;
			float num6 = num3 * this._scale;
			float num7 = this._customHeight - num4 * this._scale;
			float customHeight = this._customHeight;
			float num8 = 0f;
			float num9 = num * this._scale;
			float num10 = this._customWidth - num2 * this._scale;
			float customWidth = this._customWidth;
			this.SetVertexData(num8, num5);
			this.SetVertexData(num9, num5);
			this.SetVertexData(num10, num5);
			this.SetVertexData(customWidth, num5);
			this.SetVertexData(num8, num6);
			this.SetVertexData(num9, num6);
			this.SetVertexData(num10, num6);
			this.SetVertexData(customWidth, num6);
			this.SetVertexData(num8, num7);
			this.SetVertexData(num9, num7);
			this.SetVertexData(num10, num7);
			this.SetVertexData(customWidth, num7);
			this.SetVertexData(num8, customHeight);
			this.SetVertexData(num9, customHeight);
			this.SetVertexData(num10, customHeight);
			this.SetVertexData(customWidth, customHeight);
		}

		// Token: 0x06000221 RID: 545 RVA: 0x00008E18 File Offset: 0x00007018
		private void CalculateTextureCoordinates(float[] outUvs, bool horizontalFlip, bool verticalFlip)
		{
			this._uvsStartIndex = 0;
			float minU = this.BaseSprite.MinU;
			float minV = this.BaseSprite.MinV;
			float maxU = this.BaseSprite.MaxU;
			float maxV = this.BaseSprite.MaxV;
			float num = minU;
			float num2 = minU + (maxU - minU) * ((float)this.LeftWidth / (float)base.Width);
			float num3 = minU + (maxU - minU) * ((float)(this.LeftWidth + this._centerWidth) / (float)base.Width);
			float num4 = maxU;
			if (horizontalFlip)
			{
				num4 = minU;
				num3 = minU + (maxU - minU) * ((float)this.LeftWidth / (float)base.Width);
				num2 = minU + (maxU - minU) * ((float)(this.LeftWidth + this._centerWidth) / (float)base.Width);
				num = maxU;
			}
			float num5 = minV;
			float num6 = minV + (maxV - minV) * ((float)this.TopHeight / (float)base.Height);
			float num7 = minV + (maxV - minV) * ((float)(this.TopHeight + this._centerHeight) / (float)base.Height);
			float num8 = maxV;
			if (verticalFlip)
			{
				num8 = minV;
				num7 = minV + (maxV - minV) * ((float)this.TopHeight / (float)base.Height);
				num6 = minV + (maxV - minV) * ((float)(this.TopHeight + this._centerHeight) / (float)base.Height);
				num5 = maxV;
			}
			this.SetTextureData(outUvs, num, num5);
			this.SetTextureData(outUvs, num2, num5);
			this.SetTextureData(outUvs, num3, num5);
			this.SetTextureData(outUvs, num4, num5);
			this.SetTextureData(outUvs, num, num6);
			this.SetTextureData(outUvs, num2, num6);
			this.SetTextureData(outUvs, num3, num6);
			this.SetTextureData(outUvs, num4, num6);
			this.SetTextureData(outUvs, num, num7);
			this.SetTextureData(outUvs, num2, num7);
			this.SetTextureData(outUvs, num3, num7);
			this.SetTextureData(outUvs, num4, num7);
			this.SetTextureData(outUvs, num, num8);
			this.SetTextureData(outUvs, num2, num8);
			this.SetTextureData(outUvs, num3, num8);
			this.SetTextureData(outUvs, num4, num8);
		}

		// Token: 0x04000119 RID: 281
		private int _centerWidth;

		// Token: 0x0400011A RID: 282
		private int _centerHeight;

		// Token: 0x0400011B RID: 283
		private List<float[]> _outUvs;

		// Token: 0x0400011C RID: 284
		private float[] _outVertices;

		// Token: 0x0400011D RID: 285
		private uint[] _outIndices;

		// Token: 0x0400011E RID: 286
		private int _verticesStartIndex;

		// Token: 0x0400011F RID: 287
		private int _uvsStartIndex;

		// Token: 0x04000120 RID: 288
		private uint _indicesStartIndex;

		// Token: 0x04000121 RID: 289
		private float _scale;

		// Token: 0x04000122 RID: 290
		private float _customWidth;

		// Token: 0x04000123 RID: 291
		private float _customHeight;
	}
}
