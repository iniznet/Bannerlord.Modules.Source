using System;

namespace TaleWorlds.GauntletUI.BaseTypes
{
	// Token: 0x02000061 RID: 97
	public class OnlineImageTextureWidget : TextureWidget
	{
		// Token: 0x170001C3 RID: 451
		// (get) Token: 0x06000621 RID: 1569 RVA: 0x0001B26E File Offset: 0x0001946E
		// (set) Token: 0x06000622 RID: 1570 RVA: 0x0001B276 File Offset: 0x00019476
		public OnlineImageTextureWidget.ImageSizePolicies ImageSizePolicy { get; set; }

		// Token: 0x06000623 RID: 1571 RVA: 0x0001B27F File Offset: 0x0001947F
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

		// Token: 0x06000624 RID: 1572 RVA: 0x0001B2AA File Offset: 0x000194AA
		protected override void OnUpdate(float dt)
		{
			base.OnUpdate(dt);
			this.UpdateSizePolicy();
		}

		// Token: 0x06000625 RID: 1573 RVA: 0x0001B2BC File Offset: 0x000194BC
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

		// Token: 0x170001C4 RID: 452
		// (get) Token: 0x06000626 RID: 1574 RVA: 0x0001B44E File Offset: 0x0001964E
		// (set) Token: 0x06000627 RID: 1575 RVA: 0x0001B456 File Offset: 0x00019656
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

		// Token: 0x040002E9 RID: 745
		private static bool _textureProviderTypeCollectionRequested;

		// Token: 0x040002EB RID: 747
		private string _onlineImageSourceUrl;

		// Token: 0x0200008D RID: 141
		public enum ImageSizePolicies
		{
			// Token: 0x04000461 RID: 1121
			Stretch,
			// Token: 0x04000462 RID: 1122
			OriginalSize,
			// Token: 0x04000463 RID: 1123
			ScaleToBiggerDimension
		}
	}
}
