using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem.Encyclopedia;
using TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia.Pages;
using TaleWorlds.CampaignSystem.ViewModelCollection.Input;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Tutorial;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia
{
	// Token: 0x020000AE RID: 174
	public class EncyclopediaNavigatorVM : ViewModel
	{
		// Token: 0x17000595 RID: 1429
		// (get) Token: 0x060010EF RID: 4335 RVA: 0x00043347 File Offset: 0x00041547
		public Tuple<string, object> LastActivePage
		{
			get
			{
				if (!this.History.IsEmpty<Tuple<string, object>>())
				{
					return this.History[this.HistoryIndex];
				}
				return null;
			}
		}

		// Token: 0x060010F0 RID: 4336 RVA: 0x0004336C File Offset: 0x0004156C
		public EncyclopediaNavigatorVM(Func<string, object, bool, EncyclopediaPageVM> goToLink, Action closeEncyclopedia)
		{
			this._closeEncyclopedia = closeEncyclopedia;
			this.History = new List<Tuple<string, object>>();
			this.HistoryIndex = 0;
			this.MinCharAmountToShowResults = 3;
			this.SearchResults = new MBBindingList<EncyclopediaSearchResultVM>();
			Campaign.Current.EncyclopediaManager.SetLinkCallback(new Action<string, object>(this.ExecuteLink));
			this._goToLink = goToLink;
			this._searchResultComparer = new EncyclopediaNavigatorVM.SearchResultComparer(string.Empty);
			this.AddHistory("Home", null);
			this.RefreshValues();
			Game.Current.EventManager.RegisterEvent<TutorialNotificationElementChangeEvent>(new Action<TutorialNotificationElementChangeEvent>(this.OnTutorialNotificationElementIDChange));
		}

		// Token: 0x060010F1 RID: 4337 RVA: 0x00043414 File Offset: 0x00041614
		private void OnTutorialNotificationElementIDChange(TutorialNotificationElementChangeEvent evnt)
		{
			this.IsHighlightEnabled = evnt.NewNotificationElementID == "EncyclopediaSearchButton";
		}

		// Token: 0x060010F2 RID: 4338 RVA: 0x0004342C File Offset: 0x0004162C
		public override void OnFinalize()
		{
			base.OnFinalize();
			InputKeyItemVM previousPageInputKey = this.PreviousPageInputKey;
			if (previousPageInputKey != null)
			{
				previousPageInputKey.OnFinalize();
			}
			InputKeyItemVM nextPageInputKey = this.NextPageInputKey;
			if (nextPageInputKey == null)
			{
				return;
			}
			nextPageInputKey.OnFinalize();
		}

		// Token: 0x060010F3 RID: 4339 RVA: 0x00043455 File Offset: 0x00041655
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.DoneText = GameTexts.FindText("str_done", null).ToString();
			this.LeaderText = GameTexts.FindText("str_done", null).ToString();
		}

		// Token: 0x060010F4 RID: 4340 RVA: 0x00043489 File Offset: 0x00041689
		public void ExecuteHome()
		{
			Campaign.Current.EncyclopediaManager.GoToLink("Home", "-1");
		}

		// Token: 0x060010F5 RID: 4341 RVA: 0x000434A4 File Offset: 0x000416A4
		public void ExecuteBarLink(string targetID)
		{
			if (targetID.Contains("Home"))
			{
				this.ExecuteHome();
				return;
			}
			if (targetID.Contains("ListPage"))
			{
				string text = targetID.Split(new char[] { '-' })[1];
				if (text == "Clans")
				{
					Campaign.Current.EncyclopediaManager.GoToLink("ListPage", "Faction");
					return;
				}
				if (text == "Kingdoms")
				{
					Campaign.Current.EncyclopediaManager.GoToLink("ListPage", "Kingdom");
					return;
				}
				if (text == "Heroes")
				{
					Campaign.Current.EncyclopediaManager.GoToLink("ListPage", "Hero");
					return;
				}
				if (text == "Settlements")
				{
					Campaign.Current.EncyclopediaManager.GoToLink("ListPage", "Settlement");
					return;
				}
				if (text == "Units")
				{
					Campaign.Current.EncyclopediaManager.GoToLink("ListPage", "NPCCharacter");
					return;
				}
				if (text == "Concept")
				{
					Campaign.Current.EncyclopediaManager.GoToLink("ListPage", "Concept");
				}
			}
		}

		// Token: 0x060010F6 RID: 4342 RVA: 0x000435D2 File Offset: 0x000417D2
		public void ExecuteCloseEncyclopedia()
		{
			this._closeEncyclopedia();
		}

		// Token: 0x060010F7 RID: 4343 RVA: 0x000435E0 File Offset: 0x000417E0
		private void ExecuteLink(string pageId, object target)
		{
			if (pageId != "LastPage" && target != this.LastActivePage.Item2)
			{
				if (!(pageId != "Home"))
				{
					pageId != this.LastActivePage.Item1;
				}
				this.AddHistory(pageId, target);
			}
			this._goToLink(pageId, target, true);
			this.PageName = GameTexts.FindText("str_encyclopedia_name", null).ToString();
			this.ResetSearch();
		}

		// Token: 0x060010F8 RID: 4344 RVA: 0x00043665 File Offset: 0x00041865
		public void ResetHistory()
		{
			this.HistoryIndex = 0;
			this.History.Clear();
			this.AddHistory("Home", null);
		}

		// Token: 0x060010F9 RID: 4345 RVA: 0x00043688 File Offset: 0x00041888
		public void ExecuteBack()
		{
			if (this.HistoryIndex == 0)
			{
				return;
			}
			int num = this.HistoryIndex - 1;
			Tuple<string, object> tuple = this.History[num];
			if (tuple.Item1 != "LastPage" && (tuple.Item1 != this.LastActivePage.Item1 || tuple.Item2 != this.LastActivePage.Item2))
			{
				if (!(tuple.Item1 != "Home"))
				{
					tuple.Item1 != this.LastActivePage.Item1;
				}
			}
			this.UpdateHistoryIndex(num);
			this._goToLink(tuple.Item1, tuple.Item2, true);
			this.PageName = GameTexts.FindText("str_encyclopedia_name", null).ToString();
		}

		// Token: 0x060010FA RID: 4346 RVA: 0x00043758 File Offset: 0x00041958
		public void ExecuteForward()
		{
			if (this.HistoryIndex == this.History.Count - 1)
			{
				return;
			}
			int num = this.HistoryIndex + 1;
			Tuple<string, object> tuple = this.History[num];
			if (tuple.Item1 != "LastPage" && (tuple.Item1 != this.LastActivePage.Item1 || tuple.Item2 != this.LastActivePage.Item2))
			{
				if (!(tuple.Item1 != "Home"))
				{
					tuple.Item1 != this.LastActivePage.Item1;
				}
			}
			this.UpdateHistoryIndex(num);
			this._goToLink(tuple.Item1, tuple.Item2, true);
			this.PageName = GameTexts.FindText("str_encyclopedia_name", null).ToString();
		}

		// Token: 0x060010FB RID: 4347 RVA: 0x00043833 File Offset: 0x00041A33
		public Tuple<string, object> GetLastPage()
		{
			return this.History[this.HistoryIndex];
		}

		// Token: 0x060010FC RID: 4348 RVA: 0x00043848 File Offset: 0x00041A48
		public void AddHistory(string pageId, object obj)
		{
			if (this.HistoryIndex < this.History.Count - 1)
			{
				Tuple<string, object> tuple = this.History[this.HistoryIndex];
				if (tuple.Item1 == pageId && tuple.Item2 == obj)
				{
					return;
				}
				this.History.RemoveRange(this.HistoryIndex + 1, this.History.Count - this.HistoryIndex - 1);
			}
			this.History.Add(new Tuple<string, object>(pageId, obj));
			this.UpdateHistoryIndex(this.History.Count - 1);
		}

		// Token: 0x060010FD RID: 4349 RVA: 0x000438E0 File Offset: 0x00041AE0
		private void UpdateHistoryIndex(int newIndex)
		{
			this.HistoryIndex = newIndex;
			this.IsBackEnabled = newIndex > 0;
			this.IsForwardEnabled = newIndex < this.History.Count - 1;
		}

		// Token: 0x060010FE RID: 4350 RVA: 0x00043909 File Offset: 0x00041B09
		public void UpdatePageName(string value)
		{
			this.PageName = GameTexts.FindText("str_encyclopedia_name", null).ToString();
		}

		// Token: 0x060010FF RID: 4351 RVA: 0x00043924 File Offset: 0x00041B24
		private void RefreshSearch(bool isAppending, bool isPasted)
		{
			int firstAsianCharIndex = EncyclopediaNavigatorVM.GetFirstAsianCharIndex(this.SearchText);
			this.MinCharAmountToShowResults = ((firstAsianCharIndex > -1 && firstAsianCharIndex < 3) ? (firstAsianCharIndex + 1) : 3);
			if (this.SearchText.Length < this.MinCharAmountToShowResults)
			{
				this.SearchResults.Clear();
				return;
			}
			if (!isAppending || this.SearchText.Length == this.MinCharAmountToShowResults || isPasted)
			{
				this.SearchResults.Clear();
				foreach (EncyclopediaPage encyclopediaPage in Campaign.Current.EncyclopediaManager.GetEncyclopediaPages())
				{
					foreach (EncyclopediaListItem encyclopediaListItem in encyclopediaPage.GetListItems())
					{
						int num = encyclopediaListItem.Name.IndexOf(this._searchText, StringComparison.OrdinalIgnoreCase);
						if (num >= 0)
						{
							this.SearchResults.Add(new EncyclopediaSearchResultVM(encyclopediaListItem, this._searchText, num));
						}
					}
				}
				this._searchResultComparer.SearchText = this._searchText;
				this.SearchResults.Sort(this._searchResultComparer);
				return;
			}
			if (isAppending)
			{
				foreach (EncyclopediaSearchResultVM encyclopediaSearchResultVM in this.SearchResults.ToList<EncyclopediaSearchResultVM>())
				{
					if (encyclopediaSearchResultVM.OrgNameText.IndexOf(this._searchText, StringComparison.OrdinalIgnoreCase) < 0)
					{
						this.SearchResults.Remove(encyclopediaSearchResultVM);
					}
					else
					{
						encyclopediaSearchResultVM.UpdateSearchedText(this._searchText);
					}
				}
			}
		}

		// Token: 0x06001100 RID: 4352 RVA: 0x00043AE0 File Offset: 0x00041CE0
		private static int GetFirstAsianCharIndex(string searchText)
		{
			for (int i = 0; i < searchText.Length; i++)
			{
				if (Common.IsCharAsian(searchText[i]))
				{
					return i;
				}
			}
			return -1;
		}

		// Token: 0x06001101 RID: 4353 RVA: 0x00043B0F File Offset: 0x00041D0F
		public void ResetSearch()
		{
			this.SearchText = string.Empty;
		}

		// Token: 0x06001102 RID: 4354 RVA: 0x00043B1C File Offset: 0x00041D1C
		public void ExecuteOnSearchActivated()
		{
			Game.Current.EventManager.TriggerEvent<OnEncyclopediaSearchActivatedEvent>(new OnEncyclopediaSearchActivatedEvent());
		}

		// Token: 0x17000596 RID: 1430
		// (get) Token: 0x06001103 RID: 4355 RVA: 0x00043B32 File Offset: 0x00041D32
		// (set) Token: 0x06001104 RID: 4356 RVA: 0x00043B3A File Offset: 0x00041D3A
		[DataSourceProperty]
		public bool CanSwitchTabs
		{
			get
			{
				return this._canSwitchTabs;
			}
			set
			{
				if (value != this._canSwitchTabs)
				{
					this._canSwitchTabs = value;
					base.OnPropertyChangedWithValue(value, "CanSwitchTabs");
				}
			}
		}

		// Token: 0x17000597 RID: 1431
		// (get) Token: 0x06001105 RID: 4357 RVA: 0x00043B58 File Offset: 0x00041D58
		// (set) Token: 0x06001106 RID: 4358 RVA: 0x00043B60 File Offset: 0x00041D60
		[DataSourceProperty]
		public bool IsBackEnabled
		{
			get
			{
				return this._isBackEnabled;
			}
			set
			{
				if (value != this._isBackEnabled)
				{
					this._isBackEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsBackEnabled");
				}
			}
		}

		// Token: 0x17000598 RID: 1432
		// (get) Token: 0x06001107 RID: 4359 RVA: 0x00043B7E File Offset: 0x00041D7E
		// (set) Token: 0x06001108 RID: 4360 RVA: 0x00043B86 File Offset: 0x00041D86
		[DataSourceProperty]
		public bool IsForwardEnabled
		{
			get
			{
				return this._isForwardEnabled;
			}
			set
			{
				if (value != this._isForwardEnabled)
				{
					this._isForwardEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsForwardEnabled");
				}
			}
		}

		// Token: 0x17000599 RID: 1433
		// (get) Token: 0x06001109 RID: 4361 RVA: 0x00043BA4 File Offset: 0x00041DA4
		// (set) Token: 0x0600110A RID: 4362 RVA: 0x00043BAC File Offset: 0x00041DAC
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

		// Token: 0x1700059A RID: 1434
		// (get) Token: 0x0600110B RID: 4363 RVA: 0x00043BCA File Offset: 0x00041DCA
		// (set) Token: 0x0600110C RID: 4364 RVA: 0x00043BD2 File Offset: 0x00041DD2
		[DataSourceProperty]
		public bool IsSearchResultsShown
		{
			get
			{
				return this._isSearchResultsShown;
			}
			set
			{
				if (value != this._isSearchResultsShown)
				{
					this._isSearchResultsShown = value;
					base.OnPropertyChangedWithValue(value, "IsSearchResultsShown");
				}
			}
		}

		// Token: 0x1700059B RID: 1435
		// (get) Token: 0x0600110D RID: 4365 RVA: 0x00043BF0 File Offset: 0x00041DF0
		// (set) Token: 0x0600110E RID: 4366 RVA: 0x00043BF8 File Offset: 0x00041DF8
		[DataSourceProperty]
		public string NavBarString
		{
			get
			{
				return this._navBarString;
			}
			set
			{
				if (value != this._navBarString)
				{
					this._navBarString = value;
					base.OnPropertyChangedWithValue<string>(value, "NavBarString");
				}
			}
		}

		// Token: 0x1700059C RID: 1436
		// (get) Token: 0x0600110F RID: 4367 RVA: 0x00043C1B File Offset: 0x00041E1B
		// (set) Token: 0x06001110 RID: 4368 RVA: 0x00043C23 File Offset: 0x00041E23
		[DataSourceProperty]
		public string PageName
		{
			get
			{
				return this._pageName;
			}
			set
			{
				if (value != this._pageName)
				{
					this._pageName = value;
					base.OnPropertyChangedWithValue<string>(value, "PageName");
				}
			}
		}

		// Token: 0x1700059D RID: 1437
		// (get) Token: 0x06001111 RID: 4369 RVA: 0x00043C46 File Offset: 0x00041E46
		// (set) Token: 0x06001112 RID: 4370 RVA: 0x00043C4E File Offset: 0x00041E4E
		[DataSourceProperty]
		public string DoneText
		{
			get
			{
				return this._doneText;
			}
			set
			{
				if (value != this._doneText)
				{
					this._doneText = value;
					base.OnPropertyChangedWithValue<string>(value, "DoneText");
				}
			}
		}

		// Token: 0x1700059E RID: 1438
		// (get) Token: 0x06001113 RID: 4371 RVA: 0x00043C71 File Offset: 0x00041E71
		// (set) Token: 0x06001114 RID: 4372 RVA: 0x00043C79 File Offset: 0x00041E79
		[DataSourceProperty]
		public string LeaderText
		{
			get
			{
				return this._leaderText;
			}
			set
			{
				if (value != this._leaderText)
				{
					this._leaderText = value;
					base.OnPropertyChangedWithValue<string>(value, "LeaderText");
				}
			}
		}

		// Token: 0x1700059F RID: 1439
		// (get) Token: 0x06001115 RID: 4373 RVA: 0x00043C9C File Offset: 0x00041E9C
		// (set) Token: 0x06001116 RID: 4374 RVA: 0x00043CA4 File Offset: 0x00041EA4
		[DataSourceProperty]
		public MBBindingList<EncyclopediaSearchResultVM> SearchResults
		{
			get
			{
				return this._searchResults;
			}
			set
			{
				if (value != this._searchResults)
				{
					this._searchResults = value;
					base.OnPropertyChangedWithValue<MBBindingList<EncyclopediaSearchResultVM>>(value, "SearchResults");
				}
			}
		}

		// Token: 0x170005A0 RID: 1440
		// (get) Token: 0x06001117 RID: 4375 RVA: 0x00043CC2 File Offset: 0x00041EC2
		// (set) Token: 0x06001118 RID: 4376 RVA: 0x00043CCC File Offset: 0x00041ECC
		[DataSourceProperty]
		public string SearchText
		{
			get
			{
				return this._searchText;
			}
			set
			{
				if (value != this._searchText)
				{
					bool flag = value.ToLower().Contains(this._searchText);
					bool flag2 = string.IsNullOrEmpty(this._searchText) && !string.IsNullOrEmpty(value);
					this._searchText = value.ToLower();
					Debug.Print("isAppending: " + flag.ToString() + " isPasted: " + flag2.ToString(), 0, Debug.DebugColor.White, 17592186044416UL);
					this.RefreshSearch(flag, flag2);
					base.OnPropertyChangedWithValue<string>(value, "SearchText");
				}
			}
		}

		// Token: 0x170005A1 RID: 1441
		// (get) Token: 0x06001119 RID: 4377 RVA: 0x00043D61 File Offset: 0x00041F61
		// (set) Token: 0x0600111A RID: 4378 RVA: 0x00043D69 File Offset: 0x00041F69
		[DataSourceProperty]
		public int MinCharAmountToShowResults
		{
			get
			{
				return this._minCharAmountToShowResults;
			}
			set
			{
				if (value != this._minCharAmountToShowResults)
				{
					this._minCharAmountToShowResults = value;
					base.OnPropertyChangedWithValue(value, "MinCharAmountToShowResults");
				}
			}
		}

		// Token: 0x170005A2 RID: 1442
		// (get) Token: 0x0600111B RID: 4379 RVA: 0x00043D87 File Offset: 0x00041F87
		// (set) Token: 0x0600111C RID: 4380 RVA: 0x00043D8F File Offset: 0x00041F8F
		[DataSourceProperty]
		public InputKeyItemVM PreviousPageInputKey
		{
			get
			{
				return this._previousPageInputKey;
			}
			set
			{
				if (value != this._previousPageInputKey)
				{
					this._previousPageInputKey = value;
					base.OnPropertyChangedWithValue<InputKeyItemVM>(value, "PreviousPageInputKey");
				}
			}
		}

		// Token: 0x170005A3 RID: 1443
		// (get) Token: 0x0600111D RID: 4381 RVA: 0x00043DAD File Offset: 0x00041FAD
		// (set) Token: 0x0600111E RID: 4382 RVA: 0x00043DB5 File Offset: 0x00041FB5
		[DataSourceProperty]
		public InputKeyItemVM NextPageInputKey
		{
			get
			{
				return this._nextPageInputKey;
			}
			set
			{
				if (value != this._nextPageInputKey)
				{
					this._nextPageInputKey = value;
					base.OnPropertyChangedWithValue<InputKeyItemVM>(value, "NextPageInputKey");
				}
			}
		}

		// Token: 0x0600111F RID: 4383 RVA: 0x00043DD3 File Offset: 0x00041FD3
		public void SetPreviousPageInputKey(HotKey hotkey)
		{
			this.PreviousPageInputKey = InputKeyItemVM.CreateFromHotKey(hotkey, true);
		}

		// Token: 0x06001120 RID: 4384 RVA: 0x00043DE2 File Offset: 0x00041FE2
		public void SetNextPageInputKey(HotKey hotkey)
		{
			this.NextPageInputKey = InputKeyItemVM.CreateFromHotKey(hotkey, true);
		}

		// Token: 0x040007E5 RID: 2021
		private List<Tuple<string, object>> History;

		// Token: 0x040007E6 RID: 2022
		private int HistoryIndex;

		// Token: 0x040007E7 RID: 2023
		private readonly Func<string, object, bool, EncyclopediaPageVM> _goToLink;

		// Token: 0x040007E8 RID: 2024
		private readonly Action _closeEncyclopedia;

		// Token: 0x040007E9 RID: 2025
		private EncyclopediaNavigatorVM.SearchResultComparer _searchResultComparer;

		// Token: 0x040007EA RID: 2026
		private MBBindingList<EncyclopediaSearchResultVM> _searchResults;

		// Token: 0x040007EB RID: 2027
		private string _searchText = "";

		// Token: 0x040007EC RID: 2028
		private string _pageName;

		// Token: 0x040007ED RID: 2029
		private string _doneText;

		// Token: 0x040007EE RID: 2030
		private string _leaderText;

		// Token: 0x040007EF RID: 2031
		private bool _canSwitchTabs;

		// Token: 0x040007F0 RID: 2032
		private bool _isBackEnabled;

		// Token: 0x040007F1 RID: 2033
		private bool _isForwardEnabled;

		// Token: 0x040007F2 RID: 2034
		private bool _isHighlightEnabled;

		// Token: 0x040007F3 RID: 2035
		private bool _isSearchResultsShown;

		// Token: 0x040007F4 RID: 2036
		private string _navBarString;

		// Token: 0x040007F5 RID: 2037
		private int _minCharAmountToShowResults;

		// Token: 0x040007F6 RID: 2038
		private InputKeyItemVM _previousPageInputKey;

		// Token: 0x040007F7 RID: 2039
		private InputKeyItemVM _nextPageInputKey;

		// Token: 0x020001F0 RID: 496
		private class SearchResultComparer : IComparer<EncyclopediaSearchResultVM>
		{
			// Token: 0x17000A6E RID: 2670
			// (get) Token: 0x0600208F RID: 8335 RVA: 0x0006FD53 File Offset: 0x0006DF53
			// (set) Token: 0x06002090 RID: 8336 RVA: 0x0006FD5B File Offset: 0x0006DF5B
			public string SearchText
			{
				get
				{
					return this._searchText;
				}
				set
				{
					if (value != this._searchText)
					{
						this._searchText = value;
					}
				}
			}

			// Token: 0x06002091 RID: 8337 RVA: 0x0006FD72 File Offset: 0x0006DF72
			public SearchResultComparer(string searchText)
			{
				this.SearchText = searchText;
			}

			// Token: 0x06002092 RID: 8338 RVA: 0x0006FD84 File Offset: 0x0006DF84
			private int CompareBasedOnCapitalization(EncyclopediaSearchResultVM x, EncyclopediaSearchResultVM y)
			{
				int num = ((x.NameText.Length > 0 && char.IsUpper(x.NameText[0])) ? 1 : (-1));
				int num2 = ((y.NameText.Length > 0 && char.IsUpper(y.NameText[0])) ? 1 : (-1));
				return num.CompareTo(num2);
			}

			// Token: 0x06002093 RID: 8339 RVA: 0x0006FDE8 File Offset: 0x0006DFE8
			public int Compare(EncyclopediaSearchResultVM x, EncyclopediaSearchResultVM y)
			{
				if (x.MatchStartIndex != y.MatchStartIndex)
				{
					return y.MatchStartIndex.CompareTo(x.MatchStartIndex);
				}
				int num = this.CompareBasedOnCapitalization(x, y);
				if (num == 0)
				{
					return y.NameText.Length.CompareTo(x.NameText.Length);
				}
				return num;
			}

			// Token: 0x0400102F RID: 4143
			private string _searchText;
		}
	}
}
