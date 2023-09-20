using System;
using System.Numerics;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.GauntletUI.BaseTypes
{
	// Token: 0x0200005F RID: 95
	public class MaskedTextureWidget : TextureWidget
	{
		// Token: 0x170001BD RID: 445
		// (get) Token: 0x06000610 RID: 1552 RVA: 0x0001AF4B File Offset: 0x0001914B
		// (set) Token: 0x06000611 RID: 1553 RVA: 0x0001AF53 File Offset: 0x00019153
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

		// Token: 0x170001BE RID: 446
		// (get) Token: 0x06000612 RID: 1554 RVA: 0x0001AF82 File Offset: 0x00019182
		// (set) Token: 0x06000613 RID: 1555 RVA: 0x0001AF8A File Offset: 0x0001918A
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

		// Token: 0x170001BF RID: 447
		// (get) Token: 0x06000614 RID: 1556 RVA: 0x0001AFB9 File Offset: 0x000191B9
		// (set) Token: 0x06000615 RID: 1557 RVA: 0x0001AFC1 File Offset: 0x000191C1
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

		// Token: 0x170001C0 RID: 448
		// (get) Token: 0x06000616 RID: 1558 RVA: 0x0001AFF0 File Offset: 0x000191F0
		// (set) Token: 0x06000617 RID: 1559 RVA: 0x0001AFF8 File Offset: 0x000191F8
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

		// Token: 0x170001C1 RID: 449
		// (get) Token: 0x06000618 RID: 1560 RVA: 0x0001B027 File Offset: 0x00019227
		// (set) Token: 0x06000619 RID: 1561 RVA: 0x0001B02F File Offset: 0x0001922F
		[Editor(false)]
		public float OverlayTextureScale { get; set; }

		// Token: 0x0600061A RID: 1562 RVA: 0x0001B038 File Offset: 0x00019238
		public MaskedTextureWidget(UIContext context)
			: base(context)
		{
			base.TextureProviderName = "ImageIdentifierTextureProvider";
			this.OverlayTextureScale = 1f;
		}

		// Token: 0x0600061B RID: 1563 RVA: 0x0001B058 File Offset: 0x00019258
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

		// Token: 0x0600061C RID: 1564 RVA: 0x0001B233 File Offset: 0x00019433
		protected override void OnDisconnectedFromRoot()
		{
			base.OnDisconnectedFromRoot();
			this._textureCache = null;
		}

		// Token: 0x040002E0 RID: 736
		private Texture _textureCache;

		// Token: 0x040002E1 RID: 737
		private string _imageId;

		// Token: 0x040002E2 RID: 738
		private string _additionalArgs;

		// Token: 0x040002E3 RID: 739
		private int _imageTypeCode;

		// Token: 0x040002E4 RID: 740
		private bool _isBig;

		// Token: 0x040002E6 RID: 742
		private SpriteFromTexture _overlaySpriteCache;

		// Token: 0x040002E7 RID: 743
		private int _overlaySpriteSizeCache;
	}
}
