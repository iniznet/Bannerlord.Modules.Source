using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets
{
	public class BannerIconTextureWidget : TextureWidget
	{
		[Editor(false)]
		public string Code
		{
			get
			{
				return this._code;
			}
			set
			{
				if (this._code != value)
				{
					this._code = value;
					base.OnPropertyChanged<string>(value, "Code");
					base.SetTextureProviderProperty("Code", value);
				}
			}
		}

		public BannerIconTextureWidget(UIContext context)
			: base(context)
		{
			base.TextureProviderName = "GauntletBannerTableauProvider";
		}

		private string _code;
	}
}
