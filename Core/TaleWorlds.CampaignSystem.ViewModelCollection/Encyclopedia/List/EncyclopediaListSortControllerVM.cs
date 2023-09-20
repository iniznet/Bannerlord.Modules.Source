using System;
using TaleWorlds.CampaignSystem.Encyclopedia;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Selector;
using TaleWorlds.Core.ViewModelCollection.Tutorial;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia.List
{
	// Token: 0x020000BE RID: 190
	public class EncyclopediaListSortControllerVM : ViewModel
	{
		// Token: 0x060012BB RID: 4795 RVA: 0x00048810 File Offset: 0x00046A10
		public EncyclopediaListSortControllerVM(EncyclopediaPage page, MBBindingList<EncyclopediaListItemVM> items)
		{
			this._page = page;
			this._items = items;
			this.UpdateSortItemsFromPage(page);
			Game.Current.EventManager.RegisterEvent<TutorialNotificationElementChangeEvent>(new Action<TutorialNotificationElementChangeEvent>(this.OnTutorialNotificationElementIDChange));
		}

		// Token: 0x060012BC RID: 4796 RVA: 0x0004885E File Offset: 0x00046A5E
		private void OnTutorialNotificationElementIDChange(TutorialNotificationElementChangeEvent evnt)
		{
			this.IsHighlightEnabled = evnt.NewNotificationElementID == "EncyclopediaSortButton";
		}

		// Token: 0x060012BD RID: 4797 RVA: 0x00048878 File Offset: 0x00046A78
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.NameLabel = GameTexts.FindText("str_sort_by_name_label", null).ToString();
			this.SortByLabel = GameTexts.FindText("str_sort_by_label", null).ToString();
			this.SortedValueLabelText = this._sortedValueLabel.ToString();
		}

		// Token: 0x060012BE RID: 4798 RVA: 0x000488C8 File Offset: 0x00046AC8
		public void SetSortSelection(int index)
		{
			this.SortSelection.SelectedIndex = index;
			this.OnSortSelectionChanged(this.SortSelection);
		}

		// Token: 0x060012BF RID: 4799 RVA: 0x000488E4 File Offset: 0x00046AE4
		private void UpdateSortItemsFromPage(EncyclopediaPage page)
		{
			this.SortSelection = new EncyclopediaListSelectorVM(0, new Action<SelectorVM<EncyclopediaListSelectorItemVM>>(this.OnSortSelectionChanged), new Action(this.OnSortSelectionActivated));
			foreach (EncyclopediaSortController encyclopediaSortController in page.GetSortControllers())
			{
				EncyclopediaListItemComparer encyclopediaListItemComparer = new EncyclopediaListItemComparer(encyclopediaSortController);
				this.SortSelection.AddItem(new EncyclopediaListSelectorItemVM(encyclopediaListItemComparer));
			}
		}

		// Token: 0x060012C0 RID: 4800 RVA: 0x00048964 File Offset: 0x00046B64
		private void UpdateAlternativeSortState(EncyclopediaListItemComparerBase comparer)
		{
			EncyclopediaListSortControllerVM.SortState sortState = (comparer.IsAscending ? EncyclopediaListSortControllerVM.SortState.Ascending : EncyclopediaListSortControllerVM.SortState.Descending);
			this.AlternativeSortState = (int)sortState;
		}

		// Token: 0x060012C1 RID: 4801 RVA: 0x00048988 File Offset: 0x00046B88
		private void OnSortSelectionChanged(SelectorVM<EncyclopediaListSelectorItemVM> s)
		{
			EncyclopediaListItemComparer comparer = s.SelectedItem.Comparer;
			comparer.SortController.Comparer.SetDefaultSortOrder();
			this._items.Sort(comparer);
			this._items.ApplyActionOnAllItems(delegate(EncyclopediaListItemVM x)
			{
				x.SetComparedValue(comparer.SortController.Comparer);
			});
			this._sortedValueLabel = comparer.SortController.Name;
			this.SortedValueLabelText = this._sortedValueLabel.ToString();
			this.IsAlternativeSortVisible = this.SortSelection.SelectedIndex != 0;
			this.UpdateAlternativeSortState(comparer.SortController.Comparer);
		}

		// Token: 0x060012C2 RID: 4802 RVA: 0x00048A3C File Offset: 0x00046C3C
		public void ExecuteSwitchSortOrder()
		{
			EncyclopediaListItemComparer comparer = this.SortSelection.SelectedItem.Comparer;
			comparer.SortController.Comparer.SwitchSortOrder();
			this._items.Sort(comparer);
			this.UpdateAlternativeSortState(comparer.SortController.Comparer);
		}

		// Token: 0x060012C3 RID: 4803 RVA: 0x00048A88 File Offset: 0x00046C88
		public void SetSortOrder(bool isAscending)
		{
			EncyclopediaListItemComparer comparer = this.SortSelection.SelectedItem.Comparer;
			if (comparer.SortController.Comparer.IsAscending != isAscending)
			{
				comparer.SortController.Comparer.SetSortOrder(isAscending);
				this._items.Sort(comparer);
				this.UpdateAlternativeSortState(comparer.SortController.Comparer);
			}
		}

		// Token: 0x060012C4 RID: 4804 RVA: 0x00048AE7 File Offset: 0x00046CE7
		public bool GetSortOrder()
		{
			return this.SortSelection.SelectedItem.Comparer.SortController.Comparer.IsAscending;
		}

		// Token: 0x060012C5 RID: 4805 RVA: 0x00048B08 File Offset: 0x00046D08
		private void OnSortSelectionActivated()
		{
			Game.Current.EventManager.TriggerEvent<OnEncyclopediaListSortedEvent>(new OnEncyclopediaListSortedEvent());
		}

		// Token: 0x17000645 RID: 1605
		// (get) Token: 0x060012C6 RID: 4806 RVA: 0x00048B1E File Offset: 0x00046D1E
		// (set) Token: 0x060012C7 RID: 4807 RVA: 0x00048B26 File Offset: 0x00046D26
		[DataSourceProperty]
		public EncyclopediaListSelectorVM SortSelection
		{
			get
			{
				return this._sortSelection;
			}
			set
			{
				if (value != this._sortSelection)
				{
					this._sortSelection = value;
					base.OnPropertyChangedWithValue<EncyclopediaListSelectorVM>(value, "SortSelection");
				}
			}
		}

		// Token: 0x17000646 RID: 1606
		// (get) Token: 0x060012C8 RID: 4808 RVA: 0x00048B44 File Offset: 0x00046D44
		// (set) Token: 0x060012C9 RID: 4809 RVA: 0x00048B4C File Offset: 0x00046D4C
		[DataSourceProperty]
		public string NameLabel
		{
			get
			{
				return this._nameLabel;
			}
			set
			{
				if (value != this._nameLabel)
				{
					this._nameLabel = value;
					base.OnPropertyChangedWithValue<string>(value, "NameLabel");
				}
			}
		}

		// Token: 0x17000647 RID: 1607
		// (get) Token: 0x060012CA RID: 4810 RVA: 0x00048B6F File Offset: 0x00046D6F
		// (set) Token: 0x060012CB RID: 4811 RVA: 0x00048B77 File Offset: 0x00046D77
		[DataSourceProperty]
		public string SortedValueLabelText
		{
			get
			{
				return this._sortedValueLabelText;
			}
			set
			{
				if (value != this._sortedValueLabelText)
				{
					this._sortedValueLabelText = value;
					base.OnPropertyChangedWithValue<string>(value, "SortedValueLabelText");
				}
			}
		}

		// Token: 0x17000648 RID: 1608
		// (get) Token: 0x060012CC RID: 4812 RVA: 0x00048B9A File Offset: 0x00046D9A
		// (set) Token: 0x060012CD RID: 4813 RVA: 0x00048BA2 File Offset: 0x00046DA2
		[DataSourceProperty]
		public string SortByLabel
		{
			get
			{
				return this._sortByLabel;
			}
			set
			{
				if (value != this._sortByLabel)
				{
					this._sortByLabel = value;
					base.OnPropertyChangedWithValue<string>(value, "SortByLabel");
				}
			}
		}

		// Token: 0x17000649 RID: 1609
		// (get) Token: 0x060012CE RID: 4814 RVA: 0x00048BC5 File Offset: 0x00046DC5
		// (set) Token: 0x060012CF RID: 4815 RVA: 0x00048BCD File Offset: 0x00046DCD
		[DataSourceProperty]
		public int AlternativeSortState
		{
			get
			{
				return this._alternativeSortState;
			}
			set
			{
				if (value != this._alternativeSortState)
				{
					this._alternativeSortState = value;
					base.OnPropertyChangedWithValue(value, "AlternativeSortState");
				}
			}
		}

		// Token: 0x1700064A RID: 1610
		// (get) Token: 0x060012D0 RID: 4816 RVA: 0x00048BEB File Offset: 0x00046DEB
		// (set) Token: 0x060012D1 RID: 4817 RVA: 0x00048BF3 File Offset: 0x00046DF3
		[DataSourceProperty]
		public bool IsAlternativeSortVisible
		{
			get
			{
				return this._isAlternativeSortVisible;
			}
			set
			{
				if (value != this._isAlternativeSortVisible)
				{
					this._isAlternativeSortVisible = value;
					base.OnPropertyChangedWithValue(value, "IsAlternativeSortVisible");
				}
			}
		}

		// Token: 0x1700064B RID: 1611
		// (get) Token: 0x060012D2 RID: 4818 RVA: 0x00048C11 File Offset: 0x00046E11
		// (set) Token: 0x060012D3 RID: 4819 RVA: 0x00048C19 File Offset: 0x00046E19
		[DataSourceProperty]
		public bool IsHighlightEnabled
		{
			get
			{
				return this._isHighlightEnabled;
			}
			set
			{
				if (value != this._isHighlightEnabled)
				{
					this._isHighlightEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsHighlightEnabled");
				}
			}
		}

		// Token: 0x040008B3 RID: 2227
		private TextObject _sortedValueLabel = TextObject.Empty;

		// Token: 0x040008B4 RID: 2228
		private MBBindingList<EncyclopediaListItemVM> _items;

		// Token: 0x040008B5 RID: 2229
		private EncyclopediaPage _page;

		// Token: 0x040008B6 RID: 2230
		private EncyclopediaListSelectorVM _sortSelection;

		// Token: 0x040008B7 RID: 2231
		private string _nameLabel;

		// Token: 0x040008B8 RID: 2232
		private string _sortedValueLabelText;

		// Token: 0x040008B9 RID: 2233
		private string _sortByLabel;

		// Token: 0x040008BA RID: 2234
		private int _alternativeSortState;

		// Token: 0x040008BB RID: 2235
		private bool _isAlternativeSortVisible;

		// Token: 0x040008BC RID: 2236
		private bool _isHighlightEnabled;

		// Token: 0x020001FA RID: 506
		private enum SortState
		{
			// Token: 0x04001049 RID: 4169
			Default,
			// Token: 0x0400104A RID: 4170
			Ascending,
			// Token: 0x0400104B RID: 4171
			Descending
		}
	}
}
