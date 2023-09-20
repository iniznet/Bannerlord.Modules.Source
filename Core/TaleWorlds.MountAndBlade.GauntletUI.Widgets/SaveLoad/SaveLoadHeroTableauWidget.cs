using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.SaveLoad
{
	public class SaveLoadHeroTableauWidget : TextureWidget
	{
		public bool IsVersionCompatible
		{
			get
			{
				return (bool)base.GetTextureProviderProperty("IsVersionCompatible");
			}
		}

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

		public SaveLoadHeroTableauWidget(UIContext context)
			: base(context)
		{
			base.TextureProviderName = "SaveLoadHeroTableauTextureProvider";
			this._isRenderRequestedPreviousFrame = true;
		}

		protected override void OnMousePressed()
		{
			base.SetTextureProviderProperty("CurrentlyRotating", true);
		}

		protected override void OnMouseReleased()
		{
			base.SetTextureProviderProperty("CurrentlyRotating", false);
		}

		private string _heroVisualCode;

		private string _bannerCode;
	}
}
