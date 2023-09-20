using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.Launcher.Library.CustomWidgets
{
	// Token: 0x02000025 RID: 37
	public class LauncherOnlineImageTextureWidget : TextureWidget
	{
		// Token: 0x17000069 RID: 105
		// (get) Token: 0x06000171 RID: 369 RVA: 0x0000666A File Offset: 0x0000486A
		// (set) Token: 0x06000172 RID: 370 RVA: 0x00006672 File Offset: 0x00004872
		public LauncherOnlineImageTextureWidget.ImageSizePolicies ImageSizePolicy { get; set; }

		// Token: 0x06000173 RID: 371 RVA: 0x0000667B File Offset: 0x0000487B
		public LauncherOnlineImageTextureWidget(UIContext context)
			: base(context)
		{
			base.TextureProviderName = "LauncherOnlineImageTextureProvider";
			base.WidthSizePolicy = SizePolicy.Fixed;
			base.HeightSizePolicy = SizePolicy.Fixed;
		}

		// Token: 0x06000174 RID: 372 RVA: 0x0000669D File Offset: 0x0000489D
		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			this.UpdateSizePolicy();
		}

		// Token: 0x06000175 RID: 373 RVA: 0x000066AC File Offset: 0x000048AC
		protected override void OnTextureUpdated()
		{
			base.OnTextureUpdated();
			this.SetGlobalAlphaRecursively(0f);
		}

		// Token: 0x06000176 RID: 374 RVA: 0x000066C0 File Offset: 0x000048C0
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

		// Token: 0x1700006A RID: 106
		// (get) Token: 0x06000177 RID: 375 RVA: 0x000068D9 File Offset: 0x00004AD9
		// (set) Token: 0x06000178 RID: 376 RVA: 0x000068E1 File Offset: 0x00004AE1
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

		// Token: 0x040000B5 RID: 181
		private string _onlineImageSourceUrl;

		// Token: 0x02000045 RID: 69
		public enum ImageSizePolicies
		{
			// Token: 0x040000F6 RID: 246
			Stretch,
			// Token: 0x040000F7 RID: 247
			OriginalSize,
			// Token: 0x040000F8 RID: 248
			ScaleToBiggerDimension
		}
	}
}
