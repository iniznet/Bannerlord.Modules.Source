using System;
using TaleWorlds.Library;

namespace TaleWorlds.TwoDimension
{
	// Token: 0x02000033 RID: 51
	public class SpritePart
	{
		// Token: 0x170000B1 RID: 177
		// (get) Token: 0x06000222 RID: 546 RVA: 0x00008FFE File Offset: 0x000071FE
		// (set) Token: 0x06000223 RID: 547 RVA: 0x00009006 File Offset: 0x00007206
		public string Name { get; private set; }

		// Token: 0x170000B2 RID: 178
		// (get) Token: 0x06000224 RID: 548 RVA: 0x0000900F File Offset: 0x0000720F
		// (set) Token: 0x06000225 RID: 549 RVA: 0x00009017 File Offset: 0x00007217
		public int Width { get; private set; }

		// Token: 0x170000B3 RID: 179
		// (get) Token: 0x06000226 RID: 550 RVA: 0x00009020 File Offset: 0x00007220
		// (set) Token: 0x06000227 RID: 551 RVA: 0x00009028 File Offset: 0x00007228
		public int Height { get; private set; }

		// Token: 0x170000B4 RID: 180
		// (get) Token: 0x06000228 RID: 552 RVA: 0x00009031 File Offset: 0x00007231
		// (set) Token: 0x06000229 RID: 553 RVA: 0x00009039 File Offset: 0x00007239
		public int SheetID { get; set; }

		// Token: 0x170000B5 RID: 181
		// (get) Token: 0x0600022A RID: 554 RVA: 0x00009042 File Offset: 0x00007242
		// (set) Token: 0x0600022B RID: 555 RVA: 0x0000904A File Offset: 0x0000724A
		public int SheetX { get; set; }

		// Token: 0x170000B6 RID: 182
		// (get) Token: 0x0600022C RID: 556 RVA: 0x00009053 File Offset: 0x00007253
		// (set) Token: 0x0600022D RID: 557 RVA: 0x0000905B File Offset: 0x0000725B
		public int SheetY { get; set; }

		// Token: 0x170000B7 RID: 183
		// (get) Token: 0x0600022E RID: 558 RVA: 0x00009064 File Offset: 0x00007264
		// (set) Token: 0x0600022F RID: 559 RVA: 0x0000906C File Offset: 0x0000726C
		public float MinU { get; private set; }

		// Token: 0x170000B8 RID: 184
		// (get) Token: 0x06000230 RID: 560 RVA: 0x00009075 File Offset: 0x00007275
		// (set) Token: 0x06000231 RID: 561 RVA: 0x0000907D File Offset: 0x0000727D
		public float MinV { get; private set; }

		// Token: 0x170000B9 RID: 185
		// (get) Token: 0x06000232 RID: 562 RVA: 0x00009086 File Offset: 0x00007286
		// (set) Token: 0x06000233 RID: 563 RVA: 0x0000908E File Offset: 0x0000728E
		public float MaxU { get; private set; }

		// Token: 0x170000BA RID: 186
		// (get) Token: 0x06000234 RID: 564 RVA: 0x00009097 File Offset: 0x00007297
		// (set) Token: 0x06000235 RID: 565 RVA: 0x0000909F File Offset: 0x0000729F
		public float MaxV { get; private set; }

		// Token: 0x170000BB RID: 187
		// (get) Token: 0x06000236 RID: 566 RVA: 0x000090A8 File Offset: 0x000072A8
		// (set) Token: 0x06000237 RID: 567 RVA: 0x000090B0 File Offset: 0x000072B0
		public int SheetWidth { get; private set; }

		// Token: 0x170000BC RID: 188
		// (get) Token: 0x06000238 RID: 568 RVA: 0x000090B9 File Offset: 0x000072B9
		// (set) Token: 0x06000239 RID: 569 RVA: 0x000090C1 File Offset: 0x000072C1
		public int SheetHeight { get; private set; }

		// Token: 0x170000BD RID: 189
		// (get) Token: 0x0600023A RID: 570 RVA: 0x000090CC File Offset: 0x000072CC
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

		// Token: 0x170000BE RID: 190
		// (get) Token: 0x0600023B RID: 571 RVA: 0x0000912D File Offset: 0x0000732D
		public SpriteCategory Category
		{
			get
			{
				return this._category;
			}
		}

		// Token: 0x0600023C RID: 572 RVA: 0x00009135 File Offset: 0x00007335
		public SpritePart(string name, SpriteCategory category, int width, int height)
		{
			this.Name = name;
			this.Width = width;
			this.Height = height;
			this._category = category;
			this._category.SpriteParts.Add(this);
		}

		// Token: 0x0600023D RID: 573 RVA: 0x0000916C File Offset: 0x0000736C
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

		// Token: 0x0600023E RID: 574 RVA: 0x0000922C File Offset: 0x0000742C
		public void DrawSpritePart(float screenX, float screenY, float[] outVertices, float[] outUvs, int verticesStartIndex, int uvsStartIndex)
		{
			this.DrawSpritePart(screenX, screenY, outVertices, outUvs, verticesStartIndex, uvsStartIndex, 1f, (float)this.Width, (float)this.Height, false, false);
		}

		// Token: 0x0600023F RID: 575 RVA: 0x00009260 File Offset: 0x00007460
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

		// Token: 0x06000240 RID: 576 RVA: 0x00009310 File Offset: 0x00007510
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

		// Token: 0x04000130 RID: 304
		private SpriteCategory _category;
	}
}
