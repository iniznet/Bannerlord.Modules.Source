using System;
using TaleWorlds.CampaignSystem.Encyclopedia;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Selector;
using TaleWorlds.Core.ViewModelCollection.Tutorial;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia.List
{
	public class EncyclopediaListSortControllerVM : ViewModel
	{
		public EncyclopediaListSortControllerVM(EncyclopediaPage page, MBBindingList<EncyclopediaListItemVM> items)
		{
			this._page = page;
			this._items = items;
			this.UpdateSortItemsFromPage(page);
			Game.Current.EventManager.RegisterEvent<TutorialNotificationElementChangeEvent>(new Action<TutorialNotificationElementChangeEvent>(this.OnTutorialNotificationElementIDChange));
		}

		private void OnTutorialNotificationElementIDChange(TutorialNotificationElementChangeEvent evnt)
		{
			this.IsHighlightEnabled = evnt.NewNotificationElementID == "EncyclopediaSortButton";
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.NameLabel = GameTexts.FindText("str_sort_by_name_label", null).ToString();
			this.SortByLabel = GameTexts.FindText("str_sort_by_label", null).ToString();
			this.SortedValueLabelText = this._sortedValueLabel.ToString();
		}

		public void SetSortSelection(int index)
		{
			this.SortSelection.SelectedIndex = index;
			this.OnSortSelectionChanged(this.SortSelection);
		}

		private void UpdateSortItemsFromPage(EncyclopediaPage page)
		{
			this.SortSelection = new EncyclopediaListSelectorVM(0, new Action<SelectorVM<EncyclopediaListSelectorItemVM>>(this.OnSortSelectionChanged), new Action(this.OnSortSelectionActivated));
			foreach (EncyclopediaSortController encyclopediaSortController in page.GetSortControllers())
			{
				EncyclopediaListItemComparer encyclopediaListItemComparer = new EncyclopediaListItemComparer(encyclopediaSortController);
				this.SortSelection.AddItem(new EncyclopediaListSelectorItemVM(encyclopediaListItemComparer));
			}
		}

		private void UpdateAlternativeSortState(EncyclopediaListItemComparerBase comparer)
		{
			EncyclopediaListSortControllerVM.SortState sortState = (comparer.IsAscending ? EncyclopediaListSortControllerVM.SortState.Ascending : EncyclopediaListSortControllerVM.SortState.Descending);
			this.AlternativeSortState = (int)sortState;
		}

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

		public void ExecuteSwitchSortOrder()
		{
			EncyclopediaListItemComparer comparer = this.SortSelection.SelectedItem.Comparer;
			comparer.SortController.Comparer.SwitchSortOrder();
			this._items.Sort(comparer);
			this.UpdateAlternativeSortState(comparer.SortController.Comparer);
		}

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

		public bool GetSortOrder()
		{
			return this.SortSelection.SelectedItem.Comparer.SortController.Comparer.IsAscending;
		}

		private void OnSortSelectionActivated()
		{
			Game.Current.EventManager.TriggerEvent<OnEncyclopediaListSortedEvent>(new OnEncyclopediaListSortedEvent());
		}

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

		private TextObject _sortedValueLabel = TextObject.Empty;

		private MBBindingList<EncyclopediaListItemVM> _items;

		private EncyclopediaPage _page;

		private EncyclopediaListSelectorVM _sortSelection;

		private string _nameLabel;

		private string _sortedValueLabelText;

		private string _sortByLabel;

		private int _alternativeSortState;

		private bool _isAlternativeSortVisible;

		private bool _isHighlightEnabled;

		private enum SortState
		{
			Default,
			Ascending,
			Descending
		}
	}
}
