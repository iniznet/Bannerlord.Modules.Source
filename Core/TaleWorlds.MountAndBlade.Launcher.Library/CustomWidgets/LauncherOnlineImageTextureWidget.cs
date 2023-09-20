using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.Launcher.Library.CustomWidgets
{
	public class LauncherOnlineImageTextureWidget : TextureWidget
	{
		public LauncherOnlineImageTextureWidget.ImageSizePolicies ImageSizePolicy { get; set; }

		public LauncherOnlineImageTextureWidget(UIContext context)
			: base(context)
		{
			base.TextureProviderName = "LauncherOnlineImageTextureProvider";
			base.WidthSizePolicy = SizePolicy.Fixed;
			base.HeightSizePolicy = SizePolicy.Fixed;
		}

		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			this.UpdateSizePolicy();
		}

		protected override void OnTextureUpdated()
		{
			base.OnTextureUpdated();
			this.SetGlobalAlphaRecursively(0f);
		}

		private void UpdateSizePolicy()
		{
			if (base.Texture != null && base.ReadOnlyBrush.GlobalAlphaFactor < 1f)
			{
				float num = Mathf.Lerp(base.ReadOnlyBrush.GlobalAlphaFactor, 1f, 0.1f);
				this.SetGlobalAlphaRecursively(num);
			}
			else if (base.Texture == null)
			{
				this.SetGlobalAlphaRecursively(0f);
			}
			if (this.ImageSizePolicy == LauncherOnlineImageTextureWidget.ImageSizePolicies.OriginalSize)
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
				if (this.ImageSizePolicy == LauncherOnlineImageTextureWidget.ImageSizePolicies.Stretch)
				{
					base.WidthSizePolicy = SizePolicy.StretchToParent;
					base.HeightSizePolicy = SizePolicy.StretchToParent;
					return;
				}
				if (this.ImageSizePolicy == LauncherOnlineImageTextureWidget.ImageSizePolicies.ScaleToBiggerDimension && base.Texture != null)
				{
					base.WidthSizePolicy = SizePolicy.Fixed;
					base.HeightSizePolicy = SizePolicy.Fixed;
					float num2;
					if (base.Texture.Width > base.Texture.Height)
					{
						num2 = base.ParentWidget.Size.Y / (float)base.Texture.Height;
						if (num2 * (float)base.Texture.Width < base.ParentWidget.Size.X)
						{
							num2 = base.ParentWidget.Size.X / (float)base.Texture.Width;
						}
					}
					else
					{
						num2 = base.ParentWidget.Size.X / (float)base.Texture.Width;
						if (num2 * (float)base.Texture.Height < base.ParentWidget.Size.Y)
						{
							num2 = base.ParentWidget.Size.Y / (float)base.Texture.Height;
						}
					}
					base.SuggestedWidth = num2 * (float)base.Texture.Width * base._inverseScaleToUse;
					base.SuggestedHeight = num2 * (float)base.Texture.Height * base._inverseScaleToUse;
					base.ScaledSuggestedWidth = num2 * (float)base.Texture.Width;
					base.ScaledSuggestedHeight = num2 * (float)base.Texture.Height;
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

		private string _onlineImageSourceUrl;

		public enum ImageSizePolicies
		{
			Stretch,
			OriginalSize,
			ScaleToBiggerDimension
		}
	}
}
