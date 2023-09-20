using System;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.WeaponCrafting
{
	public class CraftingHeroPopupVM : ViewModel
	{
		public CraftingHeroPopupVM(Func<MBBindingList<CraftingAvailableHeroItemVM>> getCraftingHeroes)
		{
			this.GetCraftingHeroes = getCraftingHeroes;
			this.SelectHeroText = new TextObject("{=xaeXEj8J}Select character for smithing", null).ToString();
		}

		public void ExecuteOpenPopup()
		{
			this.IsVisible = true;
		}

		public void ExecuteClosePopup()
		{
			this.IsVisible = false;
		}

		[DataSourceProperty]
		public bool IsVisible
		{
			get
			{
				return this._isVisible;
			}
			set
			{
				if (value != this._isVisible)
				{
					this._isVisible = value;
					base.OnPropertyChangedWithValue(value, "IsVisible");
				}
			}
		}

		[DataSourceProperty]
		public string SelectHeroText
		{
			get
			{
				return this._selectHeroText;
			}
			set
			{
				if (value != this._selectHeroText)
				{
					this._selectHeroText = value;
					base.OnPropertyChangedWithValue<string>(value, "SelectHeroText");
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<CraftingAvailableHeroItemVM> CraftingHeroes
		{
			get
			{
				return this.GetCraftingHeroes();
			}
		}

		private readonly Func<MBBindingList<CraftingAvailableHeroItemVM>> GetCraftingHeroes;

		private bool _isVisible;

		private string _selectHeroText;
	}
}
