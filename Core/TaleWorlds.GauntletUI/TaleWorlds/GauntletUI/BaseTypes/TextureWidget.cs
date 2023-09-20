using System;
using System.Collections.Generic;
using System.Numerics;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.GauntletUI.BaseTypes
{
	// Token: 0x0200006B RID: 107
	public class TextureWidget : ImageWidget
	{
		// Token: 0x060006F1 RID: 1777 RVA: 0x0001E742 File Offset: 0x0001C942
		internal static void RecollectProviderTypes()
		{
			TextureWidget._typeCollector.Collect();
		}

		// Token: 0x17000201 RID: 513
		// (get) Token: 0x060006F2 RID: 1778 RVA: 0x0001E74E File Offset: 0x0001C94E
		// (set) Token: 0x060006F3 RID: 1779 RVA: 0x0001E756 File Offset: 0x0001C956
		public Widget LoadingIconWidget { get; set; }

		// Token: 0x17000202 RID: 514
		// (get) Token: 0x060006F4 RID: 1780 RVA: 0x0001E75F File Offset: 0x0001C95F
		// (set) Token: 0x060006F5 RID: 1781 RVA: 0x0001E767 File Offset: 0x0001C967
		public TextureProvider TextureProvider { get; private set; }

		// Token: 0x17000203 RID: 515
		// (get) Token: 0x060006F6 RID: 1782 RVA: 0x0001E770 File Offset: 0x0001C970
		// (set) Token: 0x060006F7 RID: 1783 RVA: 0x0001E778 File Offset: 0x0001C978
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

		// Token: 0x17000204 RID: 516
		// (get) Token: 0x060006F8 RID: 1784 RVA: 0x0001E79B File Offset: 0x0001C99B
		// (set) Token: 0x060006F9 RID: 1785 RVA: 0x0001E7A3 File Offset: 0x0001C9A3
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

		// Token: 0x060006FA RID: 1786 RVA: 0x0001E7BB File Offset: 0x0001C9BB
		static TextureWidget()
		{
			TextureWidget._typeCollector.Collect();
		}

		// Token: 0x060006FB RID: 1787 RVA: 0x0001E7D1 File Offset: 0x0001C9D1
		public TextureWidget(UIContext context)
			: base(context)
		{
			this.TextureProviderName = "ResourceTextureProvider";
			this.TextureProvider = null;
			this._textureProviderProperties = new Dictionary<string, object>();
			this._cachedQuad = null;
			this._cachedQuadSize = Vector2.Zero;
		}

		// Token: 0x060006FC RID: 1788 RVA: 0x0001E809 File Offset: 0x0001CA09
		protected override void OnDisconnectedFromRoot()
		{
			if (this.TextureProvider != null)
			{
				this.TextureProvider.Clear();
				this.TextureProvider = null;
			}
			base.OnDisconnectedFromRoot();
		}

		// Token: 0x060006FD RID: 1789 RVA: 0x0001E82C File Offset: 0x0001CA2C
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

		// Token: 0x060006FE RID: 1790 RVA: 0x0001E89C File Offset: 0x0001CA9C
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

		// Token: 0x060006FF RID: 1791 RVA: 0x0001E8EF File Offset: 0x0001CAEF
		protected object GetTextureProviderProperty(string propertyName)
		{
			TextureProvider textureProvider = this.TextureProvider;
			if (textureProvider == null)
			{
				return null;
			}
			return textureProvider.GetProperty(propertyName);
		}

		// Token: 0x06000700 RID: 1792 RVA: 0x0001E903 File Offset: 0x0001CB03
		protected void ClearTextureOfTextureProvier()
		{
			if (this.TextureProvider != null)
			{
				this.TextureProvider.Clear();
			}
		}

		// Token: 0x06000701 RID: 1793 RVA: 0x0001E918 File Offset: 0x0001CB18
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

		// Token: 0x06000702 RID: 1794 RVA: 0x0001E9E4 File Offset: 0x0001CBE4
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

		// Token: 0x06000703 RID: 1795 RVA: 0x0001EA36 File Offset: 0x0001CC36
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

		// Token: 0x06000704 RID: 1796 RVA: 0x0001EA68 File Offset: 0x0001CC68
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

		// Token: 0x04000343 RID: 835
		protected static TypeCollector<TextureProvider> _typeCollector = new TypeCollector<TextureProvider>();

		// Token: 0x04000346 RID: 838
		private string _textureProviderName;

		// Token: 0x04000347 RID: 839
		private Texture _texture;

		// Token: 0x04000348 RID: 840
		private float _lastWidth;

		// Token: 0x04000349 RID: 841
		private float _lastHeight;

		// Token: 0x0400034A RID: 842
		protected bool _isTargetSizeDirty;

		// Token: 0x0400034B RID: 843
		private Dictionary<string, object> _textureProviderProperties;

		// Token: 0x0400034C RID: 844
		protected bool _isRenderRequestedPreviousFrame;

		// Token: 0x0400034D RID: 845
		protected DrawObject2D _cachedQuad;

		// Token: 0x0400034E RID: 846
		protected Vector2 _cachedQuadSize;
	}
}
