using System;

namespace TaleWorlds.TwoDimension
{
	// Token: 0x02000015 RID: 21
	internal class TextMeshGenerator
	{
		// Token: 0x060000B8 RID: 184 RVA: 0x00005A3B File Offset: 0x00003C3B
		internal TextMeshGenerator()
		{
			this._scaleValue = 1f;
		}

		// Token: 0x060000B9 RID: 185 RVA: 0x00005A50 File Offset: 0x00003C50
		internal void Refresh(Font font, int possibleMaxCharacterLength, float scaleValue)
		{
			this._font = font;
			this._textMeshCharacterCount = 0;
			int num = possibleMaxCharacterLength * 8 * 2;
			int num2 = possibleMaxCharacterLength * 8 * 2;
			if (this._vertices == null || this._vertices.Length < num)
			{
				this._vertices = new float[num];
			}
			if (this._uvs == null || this._uvs.Length < num2)
			{
				this._uvs = new float[num2];
			}
			this._scaleValue = scaleValue;
		}

		// Token: 0x060000BA RID: 186 RVA: 0x00005ABC File Offset: 0x00003CBC
		internal DrawObject2D GenerateMesh()
		{
			int num = this._textMeshCharacterCount * 6;
			if ((this._indices == null || this._indices.Length != num) && (this._indices == null || this._indices.Length < num))
			{
				this._indices = new uint[num];
				uint num2 = 0U;
				while ((ulong)num2 < (ulong)((long)this._textMeshCharacterCount))
				{
					int num3 = (int)(6U * num2);
					uint num4 = 4U * num2;
					this._indices[num3] = num4;
					this._indices[num3 + 1] = 1U + num4;
					this._indices[num3 + 2] = 2U + num4;
					this._indices[num3 + 3] = num4;
					this._indices[num3 + 4] = 2U + num4;
					this._indices[num3 + 5] = 3U + num4;
					num2 += 1U;
				}
			}
			DrawObject2D drawObject2D = new DrawObject2D(MeshTopology.Triangles, this._vertices, this._uvs, this._indices, num);
			drawObject2D.RecalculateProperties();
			return drawObject2D;
		}

		// Token: 0x060000BB RID: 187 RVA: 0x00005B8C File Offset: 0x00003D8C
		internal void AddCharacterToMesh(float x, float y, BitmapFontCharacter fontCharacter)
		{
			float minU = this._font.FontSprite.MinU;
			float minV = this._font.FontSprite.MinV;
			float num = 1f / (float)this._font.FontSprite.Texture.Width;
			float num2 = 1f / (float)this._font.FontSprite.Texture.Height;
			float num3 = minU + (float)fontCharacter.X * num;
			float num4 = minV + (float)fontCharacter.Y * num2;
			float num5 = num3 + (float)fontCharacter.Width * num;
			float num6 = num4 + (float)fontCharacter.Height * num2;
			float num7 = (float)fontCharacter.Width * this._scaleValue;
			float num8 = (float)fontCharacter.Height * this._scaleValue;
			this._uvs[8 * this._textMeshCharacterCount] = num3;
			this._uvs[8 * this._textMeshCharacterCount + 1] = num4;
			this._uvs[8 * this._textMeshCharacterCount + 2] = num5;
			this._uvs[8 * this._textMeshCharacterCount + 3] = num4;
			this._uvs[8 * this._textMeshCharacterCount + 4] = num5;
			this._uvs[8 * this._textMeshCharacterCount + 5] = num6;
			this._uvs[8 * this._textMeshCharacterCount + 6] = num3;
			this._uvs[8 * this._textMeshCharacterCount + 7] = num6;
			this._vertices[8 * this._textMeshCharacterCount] = x;
			this._vertices[8 * this._textMeshCharacterCount + 1] = y;
			this._vertices[8 * this._textMeshCharacterCount + 2] = x + num7;
			this._vertices[8 * this._textMeshCharacterCount + 3] = y;
			this._vertices[8 * this._textMeshCharacterCount + 4] = x + num7;
			this._vertices[8 * this._textMeshCharacterCount + 5] = y + num8;
			this._vertices[8 * this._textMeshCharacterCount + 6] = x;
			this._vertices[8 * this._textMeshCharacterCount + 7] = y + num8;
			this._textMeshCharacterCount++;
		}

		// Token: 0x060000BC RID: 188 RVA: 0x00005D84 File Offset: 0x00003F84
		internal void AddValueToX(float value)
		{
			for (int i = 0; i < this._vertices.Length; i += 2)
			{
				this._vertices[i] += value;
			}
		}

		// Token: 0x060000BD RID: 189 RVA: 0x00005DB8 File Offset: 0x00003FB8
		internal void AddValueToY(float value)
		{
			for (int i = 0; i < this._vertices.Length; i += 2)
			{
				this._vertices[i + 1] += value;
			}
		}

		// Token: 0x0400007C RID: 124
		private Font _font;

		// Token: 0x0400007D RID: 125
		private float[] _vertices;

		// Token: 0x0400007E RID: 126
		private float[] _uvs;

		// Token: 0x0400007F RID: 127
		private uint[] _indices;

		// Token: 0x04000080 RID: 128
		private int _textMeshCharacterCount;

		// Token: 0x04000081 RID: 129
		private float _scaleValue;
	}
}
