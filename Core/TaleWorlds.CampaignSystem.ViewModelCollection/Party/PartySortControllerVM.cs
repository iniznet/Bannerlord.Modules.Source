using System;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core.ViewModelCollection.Selector;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Party
{
	public class PartySortControllerVM : ViewModel
	{
		public PartySortControllerVM(PartyScreenLogic.PartyRosterSide rosterSide, Action<PartyScreenLogic.PartyRosterSide, PartyScreenLogic.TroopSortType, bool> onSort)
		{
			this._rosterSide = rosterSide;
			this.SortOptions = new SelectorVM<TroopSortSelectorItemVM>(-1, new Action<SelectorVM<TroopSortSelectorItemVM>>(this.OnSortSelected));
			this.SortOptions.AddItem(new TroopSortSelectorItemVM(new TextObject("{=zMMqgxb1}Type", null), PartyScreenLogic.TroopSortType.Type));
			this.SortOptions.AddItem(new TroopSortSelectorItemVM(new TextObject("{=PDdh1sBj}Name", null), PartyScreenLogic.TroopSortType.Name));
			this.SortOptions.AddItem(new TroopSortSelectorItemVM(new TextObject("{=zFDoDbNj}Count", null), PartyScreenLogic.TroopSortType.Count));
			this.SortOptions.AddItem(new TroopSortSelectorItemVM(new TextObject("{=cc1d7mkq}Tier", null), PartyScreenLogic.TroopSortType.Tier));
			this.SortOptions.AddItem(new TroopSortSelectorItemVM(new TextObject("{=jvOYgHOe}Custom", null), PartyScreenLogic.TroopSortType.Custom));
			this.SortOptions.SelectedIndex = this.SortOptions.ItemList.Count - 1;
			this.IsAscending = true;
			this._onSort = onSort;
		}

		private void OnSortSelected(SelectorVM<TroopSortSelectorItemVM> selector)
		{
			this._sortType = selector.SelectedItem.SortType;
			this.IsCustomSort = this._sortType == PartyScreenLogic.TroopSortType.Custom;
			Action<PartyScreenLogic.PartyRosterSide, PartyScreenLogic.TroopSortType, bool> onSort = this._onSort;
			if (onSort == null)
			{
				return;
			}
			onSort(this._rosterSide, this._sortType, this.IsAscending);
		}

		public void SelectSortType(PartyScreenLogic.TroopSortType sortType)
		{
			for (int i = 0; i < this.SortOptions.ItemList.Count; i++)
			{
				if (this.SortOptions.ItemList[i].SortType == sortType)
				{
					this.SortOptions.SelectedIndex = i;
				}
			}
		}

		public void SortWith(PartyScreenLogic.TroopSortType sortType, bool isAscending)
		{
			Action<PartyScreenLogic.PartyRosterSide, PartyScreenLogic.TroopSortType, bool> onSort = this._onSort;
			if (onSort == null)
			{
				return;
			}
			onSort(this._rosterSide, sortType, isAscending);
		}

		public void ExecuteToggleOrder()
		{
			this.IsAscending = !this.IsAscending;
			Action<PartyScreenLogic.PartyRosterSide, PartyScreenLogic.TroopSortType, bool> onSort = this._onSort;
			if (onSort == null)
			{
				return;
			}
			onSort(this._rosterSide, this._sortType, this.IsAscending);
		}

		[DataSourceProperty]
		public bool IsAscending
		{
			get
			{
				return this._isAscending;
			}
			set
			{
				if (value != this._isAscending)
				{
					this._isAscending = value;
					base.OnPropertyChangedWithValue(value, "IsAscending");
				}
			}
		}

		[DataSourceProperty]
		public bool IsCustomSort
		{
			get
			{
				return this._isCustomSort;
			}
			set
			{
				if (value != this._isCustomSort)
				{
					this._isCustomSort = value;
					base.OnPropertyChangedWithValue(value, "IsCustomSort");
				}
			}
		}

		[DataSourceProperty]
		public SelectorVM<TroopSortSelectorItemVM> SortOptions
		{
			get
			{
				return this._sortOptions;
			}
			set
			{
				if (value != this._sortOptions)
				{
					this._sortOptions = value;
					base.OnPropertyChangedWithValue<SelectorVM<TroopSortSelectorItemVM>>(value, "SortOptions");
				}
			}
		}

		private readonly PartyScreenLogic.PartyRosterSide _rosterSide;

		private readonly Action<PartyScreenLogic.PartyRosterSide, PartyScreenLogic.TroopSortType, bool> _onSort;

		private PartyScreenLogic.TroopSortType _sortType;

		private bool _isAscending;

		private bool _isCustomSort;

		private SelectorVM<TroopSortSelectorItemVM> _sortOptions;
	}
}
