using System;
using System.Collections.Generic;
using System.Numerics;

namespace TaleWorlds.TwoDimension
{
	// Token: 0x02000038 RID: 56
	public class TwoDimensionDrawContext
	{
		// Token: 0x170000D1 RID: 209
		// (get) Token: 0x06000274 RID: 628 RVA: 0x0000986F File Offset: 0x00007A6F
		public bool ScissorTestEnabled
		{
			get
			{
				return this._scissorTestEnabled;
			}
		}

		// Token: 0x170000D2 RID: 210
		// (get) Token: 0x06000275 RID: 629 RVA: 0x00009877 File Offset: 0x00007A77
		public bool CircularMaskEnabled
		{
			get
			{
				return this._circularMaskEnabled;
			}
		}

		// Token: 0x170000D3 RID: 211
		// (get) Token: 0x06000276 RID: 630 RVA: 0x0000987F File Offset: 0x00007A7F
		public Vector2 CircularMaskCenter
		{
			get
			{
				return this._circularMaskCenter;
			}
		}

		// Token: 0x170000D4 RID: 212
		// (get) Token: 0x06000277 RID: 631 RVA: 0x00009887 File Offset: 0x00007A87
		public float CircularMaskRadius
		{
			get
			{
				return this._circularMaskRadius;
			}
		}

		// Token: 0x170000D5 RID: 213
		// (get) Token: 0x06000278 RID: 632 RVA: 0x0000988F File Offset: 0x00007A8F
		public float CircularMaskSmoothingRadius
		{
			get
			{
				return this._circularMaskSmoothingRadius;
			}
		}

		// Token: 0x170000D6 RID: 214
		// (get) Token: 0x06000279 RID: 633 RVA: 0x00009897 File Offset: 0x00007A97
		public ScissorTestInfo CurrentScissor
		{
			get
			{
				return this._scissorStack[this._scissorStack.Count - 1];
			}
		}

		// Token: 0x0600027A RID: 634 RVA: 0x000098B4 File Offset: 0x00007AB4
		public TwoDimensionDrawContext()
		{
			this._scissorStack = new List<ScissorTestInfo>();
			this._scissorTestEnabled = false;
			this._layers = new List<TwoDimensionDrawLayer>();
			this._cachedDrawObjects = new Dictionary<TwoDimensionDrawContext.SpriteCacheKey, DrawObject2D>();
		}

		// Token: 0x0600027B RID: 635 RVA: 0x00009908 File Offset: 0x00007B08
		public void Reset()
		{
			this._scissorStack.Clear();
			this._scissorTestEnabled = false;
			for (int i = 0; i < this._usedLayersCount; i++)
			{
				this._layers[i].Reset();
			}
			this._usedLayersCount = 0;
			this._simpleMaterialPool.ResetAll();
			this._textMaterialPool.ResetAll();
		}

		// Token: 0x0600027C RID: 636 RVA: 0x00009966 File Offset: 0x00007B66
		public SimpleMaterial CreateSimpleMaterial()
		{
			return this._simpleMaterialPool.New();
		}

		// Token: 0x0600027D RID: 637 RVA: 0x00009973 File Offset: 0x00007B73
		public TextMaterial CreateTextMaterial()
		{
			return this._textMaterialPool.New();
		}

		// Token: 0x0600027E RID: 638 RVA: 0x00009980 File Offset: 0x00007B80
		public void PushScissor(int x, int y, int width, int height)
		{
			ScissorTestInfo scissorTestInfo = new ScissorTestInfo(x, y, width, height);
			if (this._scissorStack.Count > 0)
			{
				ScissorTestInfo scissorTestInfo2 = this._scissorStack[this._scissorStack.Count - 1];
				if (width != -1)
				{
					int num = scissorTestInfo2.X + scissorTestInfo2.Width;
					int num2 = x + width;
					scissorTestInfo.X = ((scissorTestInfo.X > scissorTestInfo2.X) ? scissorTestInfo.X : scissorTestInfo2.X);
					int num3 = ((num > num2) ? num2 : num);
					scissorTestInfo.Width = num3 - scissorTestInfo.X;
				}
				else
				{
					scissorTestInfo.X = scissorTestInfo2.X;
					scissorTestInfo.Width = scissorTestInfo2.Width;
				}
				if (height != -1)
				{
					int num4 = scissorTestInfo2.Y + scissorTestInfo2.Height;
					int num5 = y + height;
					scissorTestInfo.Y = ((scissorTestInfo.Y > scissorTestInfo2.Y) ? scissorTestInfo.Y : scissorTestInfo2.Y);
					int num6 = ((num4 > num5) ? num5 : num4);
					scissorTestInfo.Height = num6 - scissorTestInfo.Y;
				}
				else
				{
					scissorTestInfo.Y = scissorTestInfo2.Y;
					scissorTestInfo.Height = scissorTestInfo2.Height;
				}
			}
			else
			{
				if (width == -1)
				{
					scissorTestInfo.Width = int.MaxValue;
				}
				if (height == -1)
				{
					scissorTestInfo.Height = int.MaxValue;
				}
			}
			this._scissorStack.Add(scissorTestInfo);
			this._scissorTestEnabled = true;
		}

		// Token: 0x0600027F RID: 639 RVA: 0x00009ADF File Offset: 0x00007CDF
		public void PopScissor()
		{
			this._scissorStack.RemoveAt(this._scissorStack.Count - 1);
			if (this._scissorTestEnabled && this._scissorStack.Count == 0)
			{
				this._scissorTestEnabled = false;
			}
		}

		// Token: 0x06000280 RID: 640 RVA: 0x00009B15 File Offset: 0x00007D15
		public void SetCircualMask(Vector2 position, float radius, float smoothingRadius)
		{
			this._circularMaskEnabled = true;
			this._circularMaskCenter = position;
			this._circularMaskRadius = radius;
			this._circularMaskSmoothingRadius = smoothingRadius;
		}

		// Token: 0x06000281 RID: 641 RVA: 0x00009B33 File Offset: 0x00007D33
		public void ClearCircualMask()
		{
			this._circularMaskEnabled = false;
		}

		// Token: 0x06000282 RID: 642 RVA: 0x00009B3C File Offset: 0x00007D3C
		public void DrawTo(TwoDimensionContext twoDimensionContext)
		{
			for (int i = 0; i < this._usedLayersCount; i++)
			{
				this._layers[i].DrawTo(twoDimensionContext, i + 1);
			}
		}

		// Token: 0x06000283 RID: 643 RVA: 0x00009B70 File Offset: 0x00007D70
		public void DrawSprite(Sprite sprite, SimpleMaterial material, float x, float y, float scale, float width, float height, bool horizontalFlip, bool verticalFlip)
		{
			SpriteDrawData spriteDrawData = new SpriteDrawData(0f, 0f, scale, width, height, horizontalFlip, verticalFlip);
			DrawObject2D drawObject2D = null;
			TwoDimensionDrawContext.SpriteCacheKey spriteCacheKey = new TwoDimensionDrawContext.SpriteCacheKey(sprite, spriteDrawData);
			if (!this._cachedDrawObjects.TryGetValue(spriteCacheKey, out drawObject2D))
			{
				drawObject2D = sprite.GetArrays(spriteDrawData);
				this._cachedDrawObjects.Add(spriteCacheKey, drawObject2D);
			}
			material.Texture = sprite.Texture;
			if (this._circularMaskEnabled)
			{
				material.CircularMaskingEnabled = true;
				material.CircularMaskingCenter = this._circularMaskCenter;
				material.CircularMaskingRadius = this._circularMaskRadius;
				material.CircularMaskingSmoothingRadius = this._circularMaskSmoothingRadius;
			}
			this.Draw(x, y, material, drawObject2D, width, height);
		}

		// Token: 0x06000284 RID: 644 RVA: 0x00009C18 File Offset: 0x00007E18
		public void Draw(float x, float y, Material material, DrawObject2D drawObject2D, float width, float height)
		{
			TwoDimensionDrawData twoDimensionDrawData = new TwoDimensionDrawData(this._scissorTestEnabled, this._scissorTestEnabled ? this.CurrentScissor : default(ScissorTestInfo), x, y, material, drawObject2D, width, height);
			TwoDimensionDrawLayer twoDimensionDrawLayer = null;
			if (this._layers.Count == 0)
			{
				twoDimensionDrawLayer = new TwoDimensionDrawLayer();
				this._layers.Add(twoDimensionDrawLayer);
				this._usedLayersCount++;
			}
			else
			{
				for (int i = this._usedLayersCount - 1; i >= 0; i--)
				{
					TwoDimensionDrawLayer twoDimensionDrawLayer2 = this._layers[i];
					if (twoDimensionDrawLayer2.IsIntersects(twoDimensionDrawData.Rectangle))
					{
						break;
					}
					twoDimensionDrawLayer = twoDimensionDrawLayer2;
				}
				if (twoDimensionDrawLayer == null)
				{
					if (this._usedLayersCount == this._layers.Count)
					{
						twoDimensionDrawLayer = new TwoDimensionDrawLayer();
						this._layers.Add(twoDimensionDrawLayer);
					}
					else
					{
						twoDimensionDrawLayer = this._layers[this._usedLayersCount];
					}
					this._usedLayersCount++;
				}
			}
			this._totalDrawCount++;
			twoDimensionDrawLayer.AddDrawData(twoDimensionDrawData);
		}

		// Token: 0x06000285 RID: 645 RVA: 0x00009D18 File Offset: 0x00007F18
		public void Draw(Text text, TextMaterial material, float x, float y, float width, float height)
		{
			text.UpdateSize((int)width, (int)height);
			DrawObject2D drawObject2D = text.DrawObject2D;
			if (drawObject2D != null)
			{
				material.Texture = text.Font.FontSprite.Texture;
				material.ScaleFactor = text.FontSize;
				material.SmoothingConstant = text.Font.SmoothingConstant;
				material.Smooth = text.Font.Smooth;
				DrawObject2D drawObject2D2 = drawObject2D;
				if (material.GlowRadius > 0f || material.Blur > 0f || material.OutlineAmount > 0f)
				{
					TextMaterial textMaterial = this.CreateTextMaterial();
					textMaterial.CopyFrom(material);
					this.Draw(x, y, textMaterial, drawObject2D2, (float)((int)width), (float)((int)height));
				}
				material.GlowRadius = 0f;
				material.Blur = 0f;
				material.OutlineAmount = 0f;
				this.Draw(x, y, material, drawObject2D2, (float)((int)width), (float)((int)height));
			}
		}

		// Token: 0x0400013C RID: 316
		private List<ScissorTestInfo> _scissorStack;

		// Token: 0x0400013D RID: 317
		private bool _scissorTestEnabled;

		// Token: 0x0400013E RID: 318
		private bool _circularMaskEnabled;

		// Token: 0x0400013F RID: 319
		private float _circularMaskRadius;

		// Token: 0x04000140 RID: 320
		private float _circularMaskSmoothingRadius;

		// Token: 0x04000141 RID: 321
		private Vector2 _circularMaskCenter;

		// Token: 0x04000142 RID: 322
		private List<TwoDimensionDrawLayer> _layers;

		// Token: 0x04000143 RID: 323
		private int _usedLayersCount;

		// Token: 0x04000144 RID: 324
		private int _totalDrawCount;

		// Token: 0x04000145 RID: 325
		private Dictionary<TwoDimensionDrawContext.SpriteCacheKey, DrawObject2D> _cachedDrawObjects;

		// Token: 0x04000146 RID: 326
		private MaterialPool<SimpleMaterial> _simpleMaterialPool = new MaterialPool<SimpleMaterial>(8);

		// Token: 0x04000147 RID: 327
		private MaterialPool<TextMaterial> _textMaterialPool = new MaterialPool<TextMaterial>(8);

		// Token: 0x02000043 RID: 67
		public struct SpriteCacheKey : IEquatable<TwoDimensionDrawContext.SpriteCacheKey>
		{
			// Token: 0x060002AB RID: 683 RVA: 0x0000A42D File Offset: 0x0000862D
			public SpriteCacheKey(Sprite sprite, SpriteDrawData drawData)
			{
				this.Sprite = sprite;
				this.DrawData = drawData;
			}

			// Token: 0x060002AC RID: 684 RVA: 0x0000A43D File Offset: 0x0000863D
			public bool Equals(TwoDimensionDrawContext.SpriteCacheKey other)
			{
				return other.Sprite == this.Sprite && other.DrawData == this.DrawData;
			}

			// Token: 0x060002AD RID: 685 RVA: 0x0000A460 File Offset: 0x00008660
			public override int GetHashCode()
			{
				return (this.Sprite.GetHashCode() * 397) ^ this.DrawData.GetHashCode();
			}

			// Token: 0x0400016D RID: 365
			public Sprite Sprite;

			// Token: 0x0400016E RID: 366
			public SpriteDrawData DrawData;
		}
	}
}
