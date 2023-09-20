using System;

namespace TaleWorlds.GauntletUI.BaseTypes
{
	public class OnlineImageTextureWidget : TextureWidget
	{
		public OnlineImageTextureWidget.ImageSizePolicies ImageSizePolicy { get; set; }

		public OnlineImageTextureWidget(UIContext context)
			: base(context)
		{
			base.TextureProviderName = "OnlineImageTextureProvider";
			if (!OnlineImageTextureWidget._textureProviderTypeCollectionRequested)
			{
				TextureWidget._typeCollector.Collect();
				OnlineImageTextureWidget._textureProviderTypeCollectionRequested = true;
			}
		}

		protected override void OnUpdate(float dt)
		{
			base.OnUpdate(dt);
			this.UpdateSizePolicy();
		}

		private void UpdateSizePolicy()
		{
			if (this.ImageSizePolicy == OnlineImageTextureWidget.ImageSizePolicies.OriginalSize)
			{
				if (base.Texture != null)
				{
					base.WidthSizePolicy = SizePolicy.Fixed;
					base.HeightSizePolicy = SizePolicy.Fixed;
					base.SuggestedWidth = (float)base.Texture.Width;
					base.SuggestedHeight = (float)base.Texture.Height;
					return;
				}
			}
			else
			{
				if (this.ImageSizePolicy == OnlineImageTextureWidget.ImageSizePolicies.Stretch)
				{
					base.WidthSizePolicy = SizePolicy.StretchToParent;
					base.HeightSizePolicy = SizePolicy.StretchToParent;
					return;
				}
				if (this.ImageSizePolicy == OnlineImageTextureWidget.ImageSizePolicies.ScaleToBiggerDimension && base.Texture != null)
				{
					base.WidthSizePolicy = SizePolicy.Fixed;
					base.HeightSizePolicy = SizePolicy.Fixed;
					float num;
					if (base.Texture.Width > base.Texture.Height)
					{
						num = base.ParentWidget.Size.Y / (float)base.Texture.Height;
						if (num * (float)base.Texture.Width < base.ParentWidget.Size.X)
						{
							num = base.ParentWidget.Size.X / (float)base.Texture.Width;
						}
					}
					else
					{
						num = base.ParentWidget.Size.X / (float)base.Texture.Width;
						if (num * (float)base.Texture.Height < base.ParentWidget.Size.Y)
						{
							num = base.ParentWidget.Size.Y / (float)base.Texture.Height;
						}
					}
					base.ScaledSuggestedWidth = num * (float)base.Texture.Width;
					base.ScaledSuggestedHeight = num * (float)base.Texture.Height;
				}
			}
		}

		[Editor(false)]
		public string OnlineImageSourceUrl
		{
			get
			{
				return this._onlineImageSourceUrl;
			}
			set
			{
				if (this._onlineImageSourceUrl != value)
				{
					this._onlineImageSourceUrl = value;
					base.OnPropertyChanged<string>(value, "OnlineImageSourceUrl");
					base.SetTextureProviderProperty("OnlineSourceUrl", value);
					this.RefreshState();
				}
			}
		}

		private static bool _textureProviderTypeCollectionRequested;

		private string _onlineImageSourceUrl;

		public enum ImageSizePolicies
		{
			Stretch,
			OriginalSize,
			ScaleToBiggerDimension
		}
	}
}
