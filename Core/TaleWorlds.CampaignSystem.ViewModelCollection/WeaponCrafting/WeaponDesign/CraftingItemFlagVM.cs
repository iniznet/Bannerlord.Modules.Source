using System;
using TaleWorlds.CampaignSystem.ViewModelCollection.Inventory;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.WeaponCrafting.WeaponDesign
{
	public class CraftingItemFlagVM : ItemFlagVM
	{
		public CraftingItemFlagVM(string iconPath, TextObject hint, bool isDisplayed)
			: base(iconPath, hint)
		{
			this.IsDisplayed = isDisplayed;
			this.IconPath = "SPGeneral\\" + iconPath;
		}

		[DataSourceProperty]
		public bool IsDisplayed
		{
			get
			{
				return this._isDisplayed;
			}
			set
			{
				if (value != this._isDisplayed)
				{
					this._isDisplayed = value;
					base.OnPropertyChangedWithValue(value, "IsDisplayed");
				}
			}
		}

		[DataSourceProperty]
		public string IconPath
		{
			get
			{
				return this._iconPath;
			}
			set
			{
				if (value != this._iconPath)
				{
					this._iconPath = value;
					base.OnPropertyChangedWithValue<string>(value, "IconPath");
				}
			}
		}

		private bool _isDisplayed;

		private string _iconPath;
	}
}
