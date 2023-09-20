using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.Encyclopedia;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia.Pages;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Tutorial;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia.List
{
	// Token: 0x020000C2 RID: 194
	public class EncyclopediaListVM : EncyclopediaPageVM
	{
		// Token: 0x060012DB RID: 4827 RVA: 0x00048CF0 File Offset: 0x00046EF0
		public EncyclopediaListVM(EncyclopediaPageArgs args)
			: base(args)
		{
			this.Page = base.Obj as EncyclopediaPage;
			this.Items = new MBBindingList<EncyclopediaListItemVM>();
			this.FilterGroups = new MBBindingList<EncyclopediaFilterGroupVM>();
			this.SortController = new EncyclopediaListSortControllerVM(this.Page, this.Items);
			this.IsInitializationOver = true;
			foreach (EncyclopediaFilterGroup encyclopediaFilterGroup in this.Page.GetFilterItems())
			{
				this.FilterGroups.Add(new EncyclopediaFilterGroupVM(encyclopediaFilterGroup, new Action<EncyclopediaListFilterVM>(this.UpdateFilters)));
			}
			this.IsInitializationOver = false;
			this.Items.Clear();
			foreach (EncyclopediaListItem encyclopediaListItem in this.Page.GetListItems())
			{
				EncyclopediaListItemVM encyclopediaListItemVM = new EncyclopediaListItemVM(encyclopediaListItem);
				encyclopediaListItemVM.IsFiltered = this.Page.IsFiltered(encyclopediaListItemVM.Object);
				this.Items.Add(encyclopediaListItemVM);
			}
			this.RefreshValues();
			this.IsInitializationOver = true;
			Game.Current.EventManager.RegisterEvent<TutorialNotificationElementChangeEvent>(new Action<TutorialNotificationElementChangeEvent>(this.OnTutorialNotificationElementIDChange));
		}

		// Token: 0x060012DC RID: 4828 RVA: 0x00048E40 File Offset: 0x00047040
		private void OnTutorialNotificationElementIDChange(TutorialNotificationElementChangeEvent evnt)
		{
			this.IsFilterHighlightEnabled = evnt.NewNotificationElementID == "EncyclopediaFiltersContainer";
		}

		// Token: 0x060012DD RID: 4829 RVA: 0x00048E58 File Offset: 0x00047058
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.SortController.RefreshValues();
			this.EmptyListText = GameTexts.FindText("str_encyclopedia_empty_list_error", null).ToString();
			this.Items.ApplyActionOnAllItems(delegate(EncyclopediaListItemVM x)
			{
				x.RefreshValues();
			});
			this.FilterGroups.ApplyActionOnAllItems(delegate(EncyclopediaFilterGroupVM x)
			{
				x.RefreshValues();
			});
		}

		// Token: 0x060012DE RID: 4830 RVA: 0x00048EE0 File Offset: 0x000470E0
		public override string GetName()
		{
			return this.Page.GetName().ToString();
		}

		// Token: 0x060012DF RID: 4831 RVA: 0x00048EF4 File Offset: 0x000470F4
		public override string GetNavigationBarURL()
		{
			string text = HyperlinkTexts.GetGenericHyperlinkText("Home", GameTexts.FindText("str_encyclopedia_home", null).ToString()) + " \\ ";
			if (this.Page.HasIdentifierType(typeof(Kingdom)))
			{
				text += GameTexts.FindText("str_encyclopedia_kingdoms", null).ToString();
			}
			else if (this.Page.HasIdentifierType(typeof(Clan)))
			{
				text += GameTexts.FindText("str_encyclopedia_clans", null).ToString();
			}
			else if (this.Page.HasIdentifierType(typeof(Hero)))
			{
				text += GameTexts.FindText("str_encyclopedia_heroes", null).ToString();
			}
			else if (this.Page.HasIdentifierType(typeof(Settlement)))
			{
				text += GameTexts.FindText("str_encyclopedia_settlements", null).ToString();
			}
			else if (this.Page.HasIdentifierType(typeof(CharacterObject)))
			{
				text += GameTexts.FindText("str_encyclopedia_troops", null).ToString();
			}
			else if (this.Page.HasIdentifierType(typeof(Concept)))
			{
				text += GameTexts.FindText("str_encyclopedia_concepts", null).ToString();
			}
			return text;
		}

		// Token: 0x060012E0 RID: 4832 RVA: 0x00049050 File Offset: 0x00047250
		private void ExecuteResetFilters()
		{
			foreach (EncyclopediaFilterGroupVM encyclopediaFilterGroupVM in this.FilterGroups)
			{
				foreach (EncyclopediaListFilterVM encyclopediaListFilterVM in encyclopediaFilterGroupVM.Filters)
				{
					encyclopediaListFilterVM.IsSelected = false;
				}
			}
		}

		// Token: 0x060012E1 RID: 4833 RVA: 0x000490D0 File Offset: 0x000472D0
		public void CopyFiltersFrom(Dictionary<EncyclopediaFilterItem, bool> filters)
		{
			this.FilterGroups.ApplyActionOnAllItems(delegate(EncyclopediaFilterGroupVM x)
			{
				x.CopyFiltersFrom(filters);
			});
		}

		// Token: 0x060012E2 RID: 4834 RVA: 0x00049104 File Offset: 0x00047304
		public override void Refresh()
		{
			base.Refresh();
			foreach (EncyclopediaListItemVM encyclopediaListItemVM in this.Items)
			{
				Hero hero;
				Clan clan;
				Concept concept;
				Kingdom kingdom;
				Settlement settlement;
				CharacterObject characterObject;
				if ((hero = encyclopediaListItemVM.Object as Hero) != null)
				{
					encyclopediaListItemVM.IsBookmarked = Campaign.Current.EncyclopediaManager.ViewDataTracker.IsEncyclopediaBookmarked(hero);
				}
				else if ((clan = encyclopediaListItemVM.Object as Clan) != null)
				{
					encyclopediaListItemVM.IsBookmarked = Campaign.Current.EncyclopediaManager.ViewDataTracker.IsEncyclopediaBookmarked(clan);
				}
				else if ((concept = encyclopediaListItemVM.Object as Concept) != null)
				{
					encyclopediaListItemVM.IsBookmarked = Campaign.Current.EncyclopediaManager.ViewDataTracker.IsEncyclopediaBookmarked(concept);
				}
				else if ((kingdom = encyclopediaListItemVM.Object as Kingdom) != null)
				{
					encyclopediaListItemVM.IsBookmarked = Campaign.Current.EncyclopediaManager.ViewDataTracker.IsEncyclopediaBookmarked(kingdom);
				}
				else if ((settlement = encyclopediaListItemVM.Object as Settlement) != null)
				{
					encyclopediaListItemVM.IsBookmarked = Campaign.Current.EncyclopediaManager.ViewDataTracker.IsEncyclopediaBookmarked(settlement);
				}
				else if ((characterObject = encyclopediaListItemVM.Object as CharacterObject) != null)
				{
					encyclopediaListItemVM.IsBookmarked = Campaign.Current.EncyclopediaManager.ViewDataTracker.IsEncyclopediaBookmarked(characterObject);
				}
			}
			this._isInitializationOver = false;
			this.IsInitializationOver = true;
		}

		// Token: 0x060012E3 RID: 4835 RVA: 0x00049288 File Offset: 0x00047488
		private void UpdateFilters(EncyclopediaListFilterVM filterVM)
		{
			this.IsInitializationOver = false;
			foreach (EncyclopediaListItemVM encyclopediaListItemVM in this.Items)
			{
				encyclopediaListItemVM.IsFiltered = this.Page.IsFiltered(encyclopediaListItemVM.Object);
			}
			this.IsInitializationOver = true;
		}

		// Token: 0x1700064D RID: 1613
		// (get) Token: 0x060012E4 RID: 4836 RVA: 0x000492F4 File Offset: 0x000474F4
		// (set) Token: 0x060012E5 RID: 4837 RVA: 0x000492FC File Offset: 0x000474FC
		[DataSourceProperty]
		public string EmptyListText
		{
			get
			{
				return this._emptyListText;
			}
			set
			{
				if (value != this._emptyListText)
				{
					this._emptyListText = value;
					base.OnPropertyChangedWithValue<string>(value, "EmptyListText");
				}
			}
		}

		// Token: 0x1700064E RID: 1614
		// (get) Token: 0x060012E6 RID: 4838 RVA: 0x0004931F File Offset: 0x0004751F
		// (set) Token: 0x060012E7 RID: 4839 RVA: 0x00049327 File Offset: 0x00047527
		[DataSourceProperty]
		public string LastSelectedItemId
		{
			get
			{
				return this._lastSelectedItemId;
			}
			set
			{
				if (value != this._lastSelectedItemId)
				{
					this._lastSelectedItemId = value;
					base.OnPropertyChangedWithValue<string>(value, "LastSelectedItemId");
				}
			}
		}

		// Token: 0x1700064F RID: 1615
		// (get) Token: 0x060012E8 RID: 4840 RVA: 0x0004934A File Offset: 0x0004754A
		// (set) Token: 0x060012E9 RID: 4841 RVA: 0x00049352 File Offset: 0x00047552
		[DataSourceProperty]
		public override MBBindingList<EncyclopediaListItemVM> Items
		{
			get
			{
				return this._items;
			}
			set
			{
				if (value != this._items)
				{
					this._items = value;
					base.OnPropertyChangedWithValue<MBBindingList<EncyclopediaListItemVM>>(value, "Items");
				}
			}
		}

		// Token: 0x17000650 RID: 1616
		// (get) Token: 0x060012EA RID: 4842 RVA: 0x00049370 File Offset: 0x00047570
		// (set) Token: 0x060012EB RID: 4843 RVA: 0x00049378 File Offset: 0x00047578
		[DataSourceProperty]
		public override EncyclopediaListSortControllerVM SortController
		{
			get
			{
				return this._sortController;
			}
			set
			{
				if (value != this._sortController)
				{
					this._sortController = value;
					base.OnPropertyChangedWithValue<EncyclopediaListSortControllerVM>(value, "SortController");
				}
			}
		}

		// Token: 0x17000651 RID: 1617
		// (get) Token: 0x060012EC RID: 4844 RVA: 0x00049396 File Offset: 0x00047596
		// (set) Token: 0x060012ED RID: 4845 RVA: 0x0004939E File Offset: 0x0004759E
		[DataSourceProperty]
		public bool IsInitializationOver
		{
			get
			{
				return this._isInitializationOver;
			}
			set
			{
				if (value != this._isInitializationOver)
				{
					this._isInitializationOver = value;
					base.OnPropertyChangedWithValue(value, "IsInitializationOver");
				}
			}
		}

		// Token: 0x17000652 RID: 1618
		// (get) Token: 0x060012EE RID: 4846 RVA: 0x000493BC File Offset: 0x000475BC
		// (set) Token: 0x060012EF RID: 4847 RVA: 0x000493C4 File Offset: 0x000475C4
		[DataSourceProperty]
		public bool IsFilterHighlightEnabled
		{
			get
			{
				return this._isFilterHighlightEnabled;
			}
			set
			{
				if (value != this._isFilterHighlightEnabled)
				{
					this._isFilterHighlightEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsFilterHighlightEnabled");
				}
			}
		}

		// Token: 0x17000653 RID: 1619
		// (get) Token: 0x060012F0 RID: 4848 RVA: 0x000493E2 File Offset: 0x000475E2
		// (set) Token: 0x060012F1 RID: 4849 RVA: 0x000493EA File Offset: 0x000475EA
		[DataSourceProperty]
		public override MBBindingList<EncyclopediaFilterGroupVM> FilterGroups
		{
			get
			{
				return this._filterGroups;
			}
			set
			{
				if (value != this._filterGroups)
				{
					this._filterGroups = value;
					base.OnPropertyChangedWithValue<MBBindingList<EncyclopediaFilterGroupVM>>(value, "FilterGroups");
				}
			}
		}

		// Token: 0x040008C0 RID: 2240
		public readonly EncyclopediaPage Page;

		// Token: 0x040008C1 RID: 2241
		private MBBindingList<EncyclopediaFilterGroupVM> _filterGroups;

		// Token: 0x040008C2 RID: 2242
		private MBBindingList<EncyclopediaListItemVM> _items;

		// Token: 0x040008C3 RID: 2243
		private EncyclopediaListSortControllerVM _sortController;

		// Token: 0x040008C4 RID: 2244
		private bool _isInitializationOver;

		// Token: 0x040008C5 RID: 2245
		private bool _isFilterHighlightEnabled;

		// Token: 0x040008C6 RID: 2246
		private string _emptyListText;

		// Token: 0x040008C7 RID: 2247
		private string _lastSelectedItemId;
	}
}
