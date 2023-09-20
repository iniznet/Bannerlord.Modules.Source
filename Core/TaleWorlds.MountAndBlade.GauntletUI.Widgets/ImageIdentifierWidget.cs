using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets
{
	public class ImageIdentifierWidget : TextureWidget
	{
		public ImageIdentifierWidget(UIContext context)
			: base(context)
		{
			base.TextureProviderName = "ImageIdentifierTextureProvider";
			this._calculateSizeFirstFrame = false;
		}

		private void RefreshVisibility()
		{
			if (this.HideWhenNull)
			{
				base.IsVisible = this.ImageTypeCode != 0;
			}
		}

		protected override void OnDisconnectedFromRoot()
		{
			base.SetTextureProviderProperty("IsReleased", true);
			base.OnDisconnectedFromRoot();
		}

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
					TextureProvider textureProvider = base.TextureProvider;
					if (textureProvider == null)
					{
						return;
					}
					textureProvider.Clear(true);
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
					TextureProvider textureProvider = base.TextureProvider;
					if (textureProvider == null)
					{
						return;
					}
					textureProvider.Clear(true);
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
					this.RefreshVisibility();
					base.Texture = null;
					TextureProvider textureProvider = base.TextureProvider;
					if (textureProvider == null)
					{
						return;
					}
					textureProvider.Clear(true);
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
					this.RefreshVisibility();
					base.Texture = null;
					TextureProvider textureProvider = base.TextureProvider;
					if (textureProvider == null)
					{
						return;
					}
					textureProvider.Clear(true);
				}
			}
		}

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
					TextureProvider textureProvider = base.TextureProvider;
					if (textureProvider == null)
					{
						return;
					}
					textureProvider.Clear(true);
				}
			}
		}

		private string _imageId;

		private string _additionalArgs;

		private int _imageTypeCode;

		private bool _isBig;

		private bool _hideWhenNull;
	}
}
