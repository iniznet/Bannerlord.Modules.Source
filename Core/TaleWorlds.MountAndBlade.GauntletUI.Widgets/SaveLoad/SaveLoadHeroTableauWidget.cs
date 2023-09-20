using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.SaveLoad
{
	// Token: 0x0200004F RID: 79
	public class SaveLoadHeroTableauWidget : TextureWidget
	{
		// Token: 0x17000179 RID: 377
		// (get) Token: 0x06000433 RID: 1075 RVA: 0x0000D6EE File Offset: 0x0000B8EE
		public bool IsVersionCompatible
		{
			get
			{
				return (bool)base.GetTextureProviderProperty("IsVersionCompatible");
			}
		}

		// Token: 0x1700017A RID: 378
		// (get) Token: 0x06000434 RID: 1076 RVA: 0x0000D700 File Offset: 0x0000B900
		// (set) Token: 0x06000435 RID: 1077 RVA: 0x0000D708 File Offset: 0x0000B908
		[Editor(false)]
		public string HeroVisualCode
		{
			get
			{
				return this._heroVisualCode;
			}
			set
			{
				if (value != this._heroVisualCode)
				{
					this._heroVisualCode = value;
					base.OnPropertyChanged<string>(value, "HeroVisualCode");
					base.SetTextureProviderProperty("HeroVisualCode", value);
				}
			}
		}

		// Token: 0x1700017B RID: 379
		// (get) Token: 0x06000436 RID: 1078 RVA: 0x0000D737 File Offset: 0x0000B937
		// (set) Token: 0x06000437 RID: 1079 RVA: 0x0000D73F File Offset: 0x0000B93F
		[Editor(false)]
		public string BannerCode
		{
			get
			{
				return this._bannerCode;
			}
			set
			{
				if (value != this._bannerCode)
				{
					this._bannerCode = value;
					base.OnPropertyChanged<string>(value, "BannerCode");
					base.SetTextureProviderProperty("BannerCode", value);
				}
			}
		}

		// Token: 0x06000438 RID: 1080 RVA: 0x0000D76E File Offset: 0x0000B96E
		public SaveLoadHeroTableauWidget(UIContext context)
			: base(context)
		{
			base.TextureProviderName = "SaveLoadHeroTableauTextureProvider";
			this._isRenderRequestedPreviousFrame = true;
		}

		// Token: 0x06000439 RID: 1081 RVA: 0x0000D789 File Offset: 0x0000B989
		protected override void OnMousePressed()
		{
			base.SetTextureProviderProperty("CurrentlyRotating", true);
		}

		// Token: 0x0600043A RID: 1082 RVA: 0x0000D79C File Offset: 0x0000B99C
		protected override void OnMouseReleased()
		{
			base.SetTextureProviderProperty("CurrentlyRotating", false);
		}

		// Token: 0x040001D4 RID: 468
		private string _heroVisualCode;

		// Token: 0x040001D5 RID: 469
		private string _bannerCode;
	}
}
