using System;
using System.Collections.Generic;
using System.Numerics;

namespace TaleWorlds.TwoDimension
{
	public class TwoDimensionDrawContext
	{
		public bool ScissorTestEnabled
		{
			get
			{
				return this._scissorTestEnabled;
			}
		}

		public bool CircularMaskEnabled
		{
			get
			{
				return this._circularMaskEnabled;
			}
		}

		public Vector2 CircularMaskCenter
		{
			get
			{
				return this._circularMaskCenter;
			}
		}

		public float CircularMaskRadius
		{
			get
			{
				return this._circularMaskRadius;
			}
		}

		public float CircularMaskSmoothingRadius
		{
			get
			{
				return this._circularMaskSmoothingRadius;
			}
		}

		public ScissorTestInfo CurrentScissor
		{
			get
			{
				return this._scissorStack[this._scissorStack.Count - 1];
			}
		}

		public TwoDimensionDrawContext()
		{
			this._scissorStack = new List<ScissorTestInfo>();
			this._scissorTestEnabled = false;
			this._layers = new List<TwoDimensionDrawLayer>();
			this._cachedDrawObjects = new Dictionary<TwoDimensionDrawContext.SpriteCacheKey, DrawObject2D>();
		}

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

		public SimpleMaterial CreateSimpleMaterial()
		{
			return this._simpleMaterialPool.New();
		}

		public TextMaterial CreateTextMaterial()
		{
			return this._textMaterialPool.New();
		}

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

		public void PopScissor()
		{
			this._scissorStack.RemoveAt(this._scissorStack.Count - 1);
			if (this._scissorTestEnabled && this._scissorStack.Count == 0)
			{
				this._scissorTestEnabled = false;
			}
		}

		public void SetCircualMask(Vector2 position, float radius, float smoothingRadius)
		{
			this._circularMaskEnabled = true;
			this._circularMaskCenter = position;
			this._circularMaskRadius = radius;
			this._circularMaskSmoothingRadius = smoothingRadius;
		}

		public void ClearCircualMask()
		{
			this._circularMaskEnabled = false;
		}

		public void DrawTo(TwoDimensionContext twoDimensionContext)
		{
			for (int i = 0; i < this._usedLayersCount; i++)
			{
				this._layers[i].DrawTo(twoDimensionContext, i + 1);
			}
		}

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

		private List<ScissorTestInfo> _scissorStack;

		private bool _scissorTestEnabled;

		private bool _circularMaskEnabled;

		private float _circularMaskRadius;

		private float _circularMaskSmoothingRadius;

		private Vector2 _circularMaskCenter;

		private List<TwoDimensionDrawLayer> _layers;

		private int _usedLayersCount;

		private int _totalDrawCount;

		private Dictionary<TwoDimensionDrawContext.SpriteCacheKey, DrawObject2D> _cachedDrawObjects;

		private MaterialPool<SimpleMaterial> _simpleMaterialPool = new MaterialPool<SimpleMaterial>(8);

		private MaterialPool<TextMaterial> _textMaterialPool = new MaterialPool<TextMaterial>(8);

		public struct SpriteCacheKey : IEquatable<TwoDimensionDrawContext.SpriteCacheKey>
		{
			public SpriteCacheKey(Sprite sprite, SpriteDrawData drawData)
			{
				this.Sprite = sprite;
				this.DrawData = drawData;
			}

			public bool Equals(TwoDimensionDrawContext.SpriteCacheKey other)
			{
				return other.Sprite == this.Sprite && other.DrawData == this.DrawData;
			}

			public override int GetHashCode()
			{
				return (this.Sprite.GetHashCode() * 397) ^ this.DrawData.GetHashCode();
			}

			public Sprite Sprite;

			public SpriteDrawData DrawData;
		}
	}
}
