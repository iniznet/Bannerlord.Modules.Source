using System;
using System.Collections.Generic;
using System.Numerics;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.GauntletUI.BaseTypes
{
	public class TextureWidget : ImageWidget
	{
		internal static void RecollectProviderTypes()
		{
			TextureWidget._typeCollector.Collect();
		}

		public Widget LoadingIconWidget { get; set; }

		public TextureProvider TextureProvider { get; private set; }

		[Editor(false)]
		public string TextureProviderName
		{
			get
			{
				return this._textureProviderName;
			}
			set
			{
				if (this._textureProviderName != value)
				{
					this._textureProviderName = value;
					base.OnPropertyChanged<string>(value, "TextureProviderName");
				}
			}
		}

		public Texture Texture
		{
			get
			{
				return this._texture;
			}
			protected set
			{
				if (value != this._texture)
				{
					this._texture = value;
					this.OnTextureUpdated();
				}
			}
		}

		static TextureWidget()
		{
			TextureWidget._typeCollector.Collect();
		}

		public TextureWidget(UIContext context)
			: base(context)
		{
			this.TextureProviderName = "ResourceTextureProvider";
			this.TextureProvider = null;
			this._textureProviderProperties = new Dictionary<string, object>();
			this._cachedQuad = null;
			this._cachedQuadSize = Vector2.Zero;
		}

		protected override void OnDisconnectedFromRoot()
		{
			if (this.TextureProvider != null)
			{
				this.TextureProvider.Clear();
				this.TextureProvider = null;
			}
			base.OnDisconnectedFromRoot();
		}

		private void SetTextureProviderProperties()
		{
			if (this.TextureProvider != null)
			{
				foreach (KeyValuePair<string, object> keyValuePair in this._textureProviderProperties)
				{
					this.TextureProvider.SetProperty(keyValuePair.Key, keyValuePair.Value);
				}
			}
		}

		protected void SetTextureProviderProperty(string name, object value)
		{
			if (this._textureProviderProperties.ContainsKey(name))
			{
				this._textureProviderProperties[name] = value;
			}
			else
			{
				this._textureProviderProperties.Add(name, value);
			}
			if (this.TextureProvider != null)
			{
				this.TextureProvider.SetProperty(name, value);
			}
			this.Texture = null;
		}

		protected object GetTextureProviderProperty(string propertyName)
		{
			TextureProvider textureProvider = this.TextureProvider;
			if (textureProvider == null)
			{
				return null;
			}
			return textureProvider.GetProperty(propertyName);
		}

		protected void ClearTextureOfTextureProvier()
		{
			if (this.TextureProvider != null)
			{
				this.TextureProvider.Clear();
			}
		}

		protected void UpdateTextureWidget()
		{
			if (this._isRenderRequestedPreviousFrame)
			{
				if (this.TextureProvider != null)
				{
					if (this._lastWidth != base.Size.X || this._lastHeight != base.Size.Y || this._isTargetSizeDirty)
					{
						int num = MathF.Round(base.Size.X);
						int num2 = MathF.Round(base.Size.Y);
						this.TextureProvider.SetTargetSize(num, num2);
						this._lastWidth = base.Size.X;
						this._lastHeight = base.Size.Y;
						this._isTargetSizeDirty = false;
						return;
					}
				}
				else
				{
					this.TextureProvider = TextureWidget._typeCollector.Instantiate(this.TextureProviderName, Array.Empty<object>());
					this.SetTextureProviderProperties();
				}
			}
		}

		protected virtual void OnTextureUpdated()
		{
			bool toShow = this.Texture == null;
			if (this.LoadingIconWidget != null)
			{
				this.LoadingIconWidget.IsVisible = toShow;
				this.LoadingIconWidget.ApplyActionOnAllChildren(delegate(Widget w)
				{
					w.IsVisible = toShow;
				});
			}
		}

		protected override void OnUpdate(float dt)
		{
			base.OnUpdate(dt);
			this.UpdateTextureWidget();
			if (this._isRenderRequestedPreviousFrame)
			{
				TextureProvider textureProvider = this.TextureProvider;
				if (textureProvider != null)
				{
					textureProvider.Tick(dt);
				}
			}
			this._isRenderRequestedPreviousFrame = false;
		}

		protected override void OnRender(TwoDimensionContext twoDimensionContext, TwoDimensionDrawContext drawContext)
		{
			this._isRenderRequestedPreviousFrame = true;
			if (this.TextureProvider != null)
			{
				this.Texture = this.TextureProvider.GetTexture(twoDimensionContext, string.Empty);
				SimpleMaterial simpleMaterial = drawContext.CreateSimpleMaterial();
				List<StyleLayer> layers = base.ReadOnlyBrush.GetStyleOrDefault(base.CurrentState).Layers;
				simpleMaterial.OverlayEnabled = false;
				simpleMaterial.CircularMaskingEnabled = false;
				simpleMaterial.Texture = this.Texture;
				if (layers != null && layers.Count > 0)
				{
					StyleLayer styleLayer = layers[0];
					simpleMaterial.AlphaFactor = styleLayer.AlphaFactor * base.ReadOnlyBrush.GlobalAlphaFactor * base.Context.ContextAlpha;
					simpleMaterial.ColorFactor = styleLayer.ColorFactor * base.ReadOnlyBrush.GlobalColorFactor;
					simpleMaterial.HueFactor = styleLayer.HueFactor;
					simpleMaterial.SaturationFactor = styleLayer.SaturationFactor;
					simpleMaterial.ValueFactor = styleLayer.ValueFactor;
					simpleMaterial.Color = styleLayer.Color * base.ReadOnlyBrush.GlobalColor;
				}
				else
				{
					simpleMaterial.AlphaFactor = base.ReadOnlyBrush.GlobalAlphaFactor * base.Context.ContextAlpha;
					simpleMaterial.ColorFactor = base.ReadOnlyBrush.GlobalColorFactor;
					simpleMaterial.HueFactor = 0f;
					simpleMaterial.SaturationFactor = 0f;
					simpleMaterial.ValueFactor = 0f;
					simpleMaterial.Color = Color.White * base.ReadOnlyBrush.GlobalColor;
				}
				Vector2 globalPosition = base.GlobalPosition;
				float x = globalPosition.X;
				float y = globalPosition.Y;
				DrawObject2D drawObject2D = null;
				if (this._cachedQuad != null && this._cachedQuadSize == base.Size)
				{
					drawObject2D = this._cachedQuad;
				}
				if (drawObject2D == null)
				{
					drawObject2D = DrawObject2D.CreateQuad(base.Size);
					this._cachedQuad = drawObject2D;
					this._cachedQuadSize = base.Size;
				}
				if (drawContext.CircularMaskEnabled)
				{
					simpleMaterial.CircularMaskingEnabled = true;
					simpleMaterial.CircularMaskingCenter = drawContext.CircularMaskCenter;
					simpleMaterial.CircularMaskingRadius = drawContext.CircularMaskRadius;
					simpleMaterial.CircularMaskingSmoothingRadius = drawContext.CircularMaskSmoothingRadius;
				}
				drawContext.Draw(x, y, simpleMaterial, drawObject2D, base.Size.X, base.Size.Y);
			}
		}

		protected static TypeCollector<TextureProvider> _typeCollector = new TypeCollector<TextureProvider>();

		private string _textureProviderName;

		private Texture _texture;

		private float _lastWidth;

		private float _lastHeight;

		protected bool _isTargetSizeDirty;

		private Dictionary<string, object> _textureProviderProperties;

		protected bool _isRenderRequestedPreviousFrame;

		protected DrawObject2D _cachedQuad;

		protected Vector2 _cachedQuadSize;
	}
}
