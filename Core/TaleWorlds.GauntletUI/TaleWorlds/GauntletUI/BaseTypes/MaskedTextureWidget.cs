using System;
using System.Numerics;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.GauntletUI.BaseTypes
{
	public class MaskedTextureWidget : TextureWidget
	{
		[Editor(false)]
		public string ImageId
		{
			get
			{
				return this._imageId;
			}
			set
			{
				if (this._imageId != value)
				{
					this._imageId = value;
					base.OnPropertyChanged<string>(value, "ImageId");
					base.SetTextureProviderProperty("ImageId", value);
				}
			}
		}

		[Editor(false)]
		public string AdditionalArgs
		{
			get
			{
				return this._additionalArgs;
			}
			set
			{
				if (this._additionalArgs != value)
				{
					this._additionalArgs = value;
					base.OnPropertyChanged<string>(value, "AdditionalArgs");
					base.SetTextureProviderProperty("AdditionalArgs", value);
				}
			}
		}

		[Editor(false)]
		public int ImageTypeCode
		{
			get
			{
				return this._imageTypeCode;
			}
			set
			{
				if (this._imageTypeCode != value)
				{
					this._imageTypeCode = value;
					base.OnPropertyChanged(value, "ImageTypeCode");
					base.SetTextureProviderProperty("ImageTypeCode", value);
				}
			}
		}

		[Editor(false)]
		public bool IsBig
		{
			get
			{
				return this._isBig;
			}
			set
			{
				if (this._isBig != value)
				{
					this._isBig = value;
					base.OnPropertyChanged(value, "IsBig");
					base.SetTextureProviderProperty("IsBig", value);
				}
			}
		}

		[Editor(false)]
		public float OverlayTextureScale { get; set; }

		public MaskedTextureWidget(UIContext context)
			: base(context)
		{
			base.TextureProviderName = "ImageIdentifierTextureProvider";
			this.OverlayTextureScale = 1f;
		}

		protected override void OnRender(TwoDimensionContext twoDimensionContext, TwoDimensionDrawContext drawContext)
		{
			this._isRenderRequestedPreviousFrame = true;
			if (base.TextureProvider != null)
			{
				Texture texture = base.TextureProvider.GetTexture(twoDimensionContext, "ui_backgrounds_1");
				bool flag = false;
				if (texture != this._textureCache)
				{
					base.Brush.DefaultLayer.OverlayMethod = BrushOverlayMethod.CoverWithTexture;
					this._textureCache = texture;
					flag = true;
					base.HandleUpdateNeededOnRender();
				}
				if (this._textureCache != null)
				{
					bool flag2 = this.ImageTypeCode == 3 || this.ImageTypeCode == 6;
					int num = (flag2 ? ((int)(((base.Size.X > base.Size.Y) ? base.Size.Y : base.Size.X) * 2.5f * this.OverlayTextureScale)) : ((int)(((base.Size.X > base.Size.Y) ? base.Size.X : base.Size.Y) * this.OverlayTextureScale)));
					Vector2 vector = default(Vector2);
					if (flag2)
					{
						float num2 = ((float)num - base.Size.X) * 0.5f - base.Brush.DefaultLayer.OverlayXOffset;
						float num3 = ((float)num - base.Size.Y) * 0.5f - base.Brush.DefaultLayer.OverlayYOffset;
						vector = new Vector2(num2, num3) * base._inverseScaleToUse;
					}
					if (this._overlaySpriteCache == null || flag || this._overlaySpriteSizeCache != num)
					{
						this._overlaySpriteSizeCache = num;
						this._overlaySpriteCache = new SpriteFromTexture(this._textureCache, this._overlaySpriteSizeCache, this._overlaySpriteSizeCache);
					}
					base.Brush.DefaultLayer.OverlaySprite = this._overlaySpriteCache;
					base.BrushRenderer.Render(drawContext, base.GlobalPosition, base.Size, base._scaleToUse, base.Context.ContextAlpha, vector);
				}
			}
		}

		protected override void OnDisconnectedFromRoot()
		{
			base.OnDisconnectedFromRoot();
			this._textureCache = null;
		}

		private Texture _textureCache;

		private string _imageId;

		private string _additionalArgs;

		private int _imageTypeCode;

		private bool _isBig;

		private SpriteFromTexture _overlaySpriteCache;

		private int _overlaySpriteSizeCache;
	}
}
