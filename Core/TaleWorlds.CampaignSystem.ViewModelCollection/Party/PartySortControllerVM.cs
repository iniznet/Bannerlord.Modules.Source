using System;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core.ViewModelCollection.Selector;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Party
{
	// Token: 0x02000025 RID: 37
	public class PartySortControllerVM : ViewModel
	{
		// Token: 0x060002E5 RID: 741 RVA: 0x00012FB4 File Offset: 0x000111B4
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

		// Token: 0x060002E6 RID: 742 RVA: 0x000130A0 File Offset: 0x000112A0
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

		// Token: 0x060002E7 RID: 743 RVA: 0x000130F0 File Offset: 0x000112F0
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

		// Token: 0x060002E8 RID: 744 RVA: 0x0001313D File Offset: 0x0001133D
		public void SortWith(PartyScreenLogic.TroopSortType sortType, bool isAscending)
		{
			Action<PartyScreenLogic.PartyRosterSide, PartyScreenLogic.TroopSortType, bool> onSort = this._onSort;
			if (onSort == null)
			{
				return;
			}
			onSort(this._rosterSide, sortType, isAscending);
		}

		// Token: 0x060002E9 RID: 745 RVA: 0x00013157 File Offset: 0x00011357
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

		// Token: 0x170000C8 RID: 200
		// (get) Token: 0x060002EA RID: 746 RVA: 0x0001318A File Offset: 0x0001138A
		// (set) Token: 0x060002EB RID: 747 RVA: 0x00013192 File Offset: 0x00011392
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

		// Token: 0x170000C9 RID: 201
		// (get) Token: 0x060002EC RID: 748 RVA: 0x000131B0 File Offset: 0x000113B0
		// (set) Token: 0x060002ED RID: 749 RVA: 0x000131B8 File Offset: 0x000113B8
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

		// Token: 0x170000CA RID: 202
		// (get) Token: 0x060002EE RID: 750 RVA: 0x000131D6 File Offset: 0x000113D6
		// (set) Token: 0x060002EF RID: 751 RVA: 0x000131DE File Offset: 0x000113DE
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

		// Token: 0x0400014C RID: 332
		private readonly PartyScreenLogic.PartyRosterSide _rosterSide;

		// Token: 0x0400014D RID: 333
		private readonly Action<PartyScreenLogic.PartyRosterSide, PartyScreenLogic.TroopSortType, bool> _onSort;

		// Token: 0x0400014E RID: 334
		private PartyScreenLogic.TroopSortType _sortType;

		// Token: 0x0400014F RID: 335
		private bool _isAscending;

		// Token: 0x04000150 RID: 336
		private bool _isCustomSort;

		// Token: 0x04000151 RID: 337
		private SelectorVM<TroopSortSelectorItemVM> _sortOptions;
	}
}
