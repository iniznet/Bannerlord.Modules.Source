using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets
{
	// Token: 0x02000022 RID: 34
	public class ImageIdentifierWidget : TextureWidget
	{
		// Token: 0x060001A9 RID: 425 RVA: 0x0000697C File Offset: 0x00004B7C
		public ImageIdentifierWidget(UIContext context)
			: base(context)
		{
			base.TextureProviderName = "ImageIdentifierTextureProvider";
			this._calculateSizeFirstFrame = false;
		}

		// Token: 0x060001AA RID: 426 RVA: 0x00006997 File Offset: 0x00004B97
		private void RefreshVisibility()
		{
			if (this.HideWhenNull)
			{
				base.IsVisible = this.ImageTypeCode != 0;
			}
		}

		// Token: 0x060001AB RID: 427 RVA: 0x000069B0 File Offset: 0x00004BB0
		protected override void OnDisconnectedFromRoot()
		{
			base.SetTextureProviderProperty("IsReleased", true);
			base.OnDisconnectedFromRoot();
		}

		// Token: 0x1700008F RID: 143
		// (get) Token: 0x060001AC RID: 428 RVA: 0x000069C9 File Offset: 0x00004BC9
		// (set) Token: 0x060001AD RID: 429 RVA: 0x000069D4 File Offset: 0x00004BD4
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
					if (!string.IsNullOrEmpty(this._imageId))
					{
						base.SetTextureProviderProperty("IsReleased", true);
					}
					this._imageId = value;
					base.OnPropertyChanged<string>(value, "ImageId");
					base.SetTextureProviderProperty("ImageId", value);
					base.SetTextureProviderProperty("IsReleased", false);
					this.RefreshVisibility();
					base.Texture = null;
					base.ClearTextureOfTextureProvier();
				}
			}
		}

		// Token: 0x17000090 RID: 144
		// (get) Token: 0x060001AE RID: 430 RVA: 0x00006A50 File Offset: 0x00004C50
		// (set) Token: 0x060001AF RID: 431 RVA: 0x00006A58 File Offset: 0x00004C58
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
					if (!string.IsNullOrEmpty(this._additionalArgs))
					{
						base.SetTextureProviderProperty("IsReleased", true);
					}
					this._additionalArgs = value;
					base.OnPropertyChanged<string>(value, "AdditionalArgs");
					base.SetTextureProviderProperty("AdditionalArgs", value);
					base.SetTextureProviderProperty("IsReleased", false);
					this.RefreshVisibility();
					base.Texture = null;
					base.ClearTextureOfTextureProvier();
				}
			}
		}

		// Token: 0x17000091 RID: 145
		// (get) Token: 0x060001B0 RID: 432 RVA: 0x00006AD4 File Offset: 0x00004CD4
		// (set) Token: 0x060001B1 RID: 433 RVA: 0x00006ADC File Offset: 0x00004CDC
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
					this.RefreshVisibility();
					base.Texture = null;
					base.ClearTextureOfTextureProvier();
				}
			}
		}

		// Token: 0x17000092 RID: 146
		// (get) Token: 0x060001B2 RID: 434 RVA: 0x00006B29 File Offset: 0x00004D29
		// (set) Token: 0x060001B3 RID: 435 RVA: 0x00006B34 File Offset: 0x00004D34
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
					this.RefreshVisibility();
					base.Texture = null;
					base.ClearTextureOfTextureProvier();
				}
			}
		}

		// Token: 0x17000093 RID: 147
		// (get) Token: 0x060001B4 RID: 436 RVA: 0x00006B81 File Offset: 0x00004D81
		// (set) Token: 0x060001B5 RID: 437 RVA: 0x00006B89 File Offset: 0x00004D89
		[Editor(false)]
		public bool HideWhenNull
		{
			get
			{
				return this._hideWhenNull;
			}
			set
			{
				if (this._hideWhenNull != value)
				{
					this._hideWhenNull = value;
					base.OnPropertyChanged(value, "HideWhenNull");
					this.RefreshVisibility();
					base.Texture = null;
					base.ClearTextureOfTextureProvier();
				}
			}
		}

		// Token: 0x040000CE RID: 206
		private string _imageId;

		// Token: 0x040000CF RID: 207
		private string _additionalArgs;

		// Token: 0x040000D0 RID: 208
		private int _imageTypeCode;

		// Token: 0x040000D1 RID: 209
		private bool _isBig;

		// Token: 0x040000D2 RID: 210
		private bool _hideWhenNull;
	}
}
