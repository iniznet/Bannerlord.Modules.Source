using System;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.WeaponCrafting.WeaponDesign
{
	public class TierFilterTypeVM : ViewModel
	{
		public WeaponDesignVM.CraftingPieceTierFilter FilterType { get; }

		public TierFilterTypeVM(WeaponDesignVM.CraftingPieceTierFilter filterType, Action<WeaponDesignVM.CraftingPieceTierFilter> onSelect, string tierName)
		{
			this.FilterType = filterType;
			this._onSelect = onSelect;
			this.TierName = tierName;
		}

		public void ExecuteSelectTier()
		{
			Action<WeaponDesignVM.CraftingPieceTierFilter> onSelect = this._onSelect;
			if (onSelect == null)
			{
				return;
			}
			onSelect(this.FilterType);
		}

		[DataSourceProperty]
		public bool IsSelected
		{
			get
			{
				return this._isSelected;
			}
			set
			{
				if (value != this._isSelected)
				{
					this._isSelected = value;
					base.OnPropertyChangedWithValue(value, "IsSelected");
				}
			}
		}

		[DataSourceProperty]
		public string TierName
		{
			get
			{
				return this._tierName;
			}
			set
			{
				if (value != this._tierName)
				{
					this._tierName = value;
					base.OnPropertyChangedWithValue<string>(value, "TierName");
				}
			}
		}

		private readonly Action<WeaponDesignVM.CraftingPieceTierFilter> _onSelect;

		private bool _isSelected;

		private string _tierName;
	}
}
