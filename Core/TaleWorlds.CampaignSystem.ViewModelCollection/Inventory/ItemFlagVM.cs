using System;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Inventory
{
	public class ItemFlagVM : ViewModel
	{
		public ItemFlagVM(string iconName, TextObject hint)
		{
			this.Icon = this.GetIconPath(iconName);
			this.Hint = new HintViewModel(hint, null);
		}

		private string GetIconPath(string iconName)
		{
			MBStringBuilder mbstringBuilder = default(MBStringBuilder);
			mbstringBuilder.Initialize(16, "GetIconPath");
			mbstringBuilder.Append<string>("<img src=\"SPGeneral\\");
			mbstringBuilder.Append<string>(iconName);
			mbstringBuilder.Append<string>("\"/>");
			return mbstringBuilder.ToStringAndRelease();
		}

		[DataSourceProperty]
		public string Icon
		{
			get
			{
				return this._icon;
			}
			set
			{
				if (value != this._icon)
				{
					this._icon = value;
					base.OnPropertyChangedWithValue<string>(value, "Icon");
				}
			}
		}

		[DataSourceProperty]
		public HintViewModel Hint
		{
			get
			{
				return this._hint;
			}
			set
			{
				if (value != this._hint)
				{
					this._hint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "Hint");
				}
			}
		}

		private string _icon;

		private HintViewModel _hint;
	}
}
