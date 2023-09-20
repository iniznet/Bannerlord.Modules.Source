using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Core.ViewModelCollection.Selector;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection.Lobby.CustomGame;
using TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection.Lobby.OfficialGame;
using TaleWorlds.PlatformService;

namespace TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection.Lobby
{
	public class MPMatchmakingVM : ViewModel
	{
		public MPMatchmakingVM.MatchmakingSubPages CurrentSubPage
		{
			get
			{
				return this._currentSubPage;
			}
		}

		private bool IsServerQuickPlayAvailable
		{
			get
			{
				return NetworkMain.GameClient.IsAbleToSearchForGame;
			}
		}

		private bool IsServerCustomGameListAvailable
		{
			get
			{
				return NetworkMain.GameClient.IsCustomBattleAvailable && !this.IsFindingMatch;
			}
		}

		public MPMatchmakingVM(LobbyState lobbyState, Action<MPLobbyVM.LobbyPage> onChangePageRequest, Action<string, bool> onMatchSelectionChanged, Action<bool> onGameFindRequestStateChanged)
		{
			this._lobbyState = lobbyState;
			this._onChangePageRequest = onChangePageRequest;
			this._onMatchSelectionChanged = onMatchSelectionChanged;
			this._onGameFindRequestStateChanged = onGameFindRequestStateChanged;
			this.HasUnofficialModulesLoaded = NetworkMain.GameClient.HasUnofficialModulesLoaded;
			this.RankedGameTypes = new MBBindingList<MPMatchmakingItemVM>();
			this.CustomGameTypes = new MBBindingList<MPMatchmakingItemVM>();
			this.QuickplayGameTypes = new MBBindingList<MPMatchmakingItemVM>();
			this.CustomServer = new MPCustomGameVM(lobbyState, MPCustomGameVM.CustomGameMode.CustomServer);
			this.PremadeMatches = new MPCustomGameVM(lobbyState, MPCustomGameVM.CustomGameMode.PremadeGame);
			this.RefreshSubPageStates();
			this._selectionInfoTextObject = new TextObject("{=wuKqRvc3}Game: {GAME_TYPES}  |  Region: {REGIONS}", null);
			InformationManager.OnHideInquiry += this.OnHideInquiry;
			this._defaultSelectedGameTypes = MultiplayerMain.GetUserSelectedGameTypes();
			this.SelectionInfo = new MPMatchmakingSelectionInfoVM();
			this.UpdateQuickPlayGameTypeList();
			this.UpdateCustomGameTypeList();
			this._heroClasses = MultiplayerClassDivisions.GetMPHeroClasses();
			this.IsRanked = true;
			this.Regions = new SelectorVM<MPMatchmakingRegionSelectorItemVM>(0, null);
			this.RefreshValues();
			this.RefreshWaitingTime();
			this.OnSelectionChanged(true, true);
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.PlayText = new TextObject("{=wTtyFa89}PLAY", null).ToString();
			this.MatchFindNotPossibleText = new TextObject("{=BrYUHFsg}CHOOSE GAME", null).ToString();
			this.AutoFindText = new TextObject("{=S2bKbhTc}AUTO FIND GAME", null).ToString();
			this.RankedText = GameTexts.FindText("str_multiplayer_ranked", null).ToString();
			this.CasualText = new TextObject("{=GXosklej}Casual", null).ToString();
			this.RankedText = GameTexts.FindText("str_multiplayer_ranked", null).ToString();
			this.QuickPlayText = GameTexts.FindText("str_multiplayer_quick_play", null).ToString();
			this.CustomGameText = GameTexts.FindText("str_multiplayer_custom_game", null).ToString();
			this.CustomServerListText = GameTexts.FindText("str_multiplayer_custom_server_list", null).ToString();
			this.TeamMatchesText = new TextObject("{=PE5LqC9O}Team Matches", null).ToString();
			this.CommunityGameText = new TextObject("{=SIIgjILk}Community Games", null).ToString();
			this.RegionsHint = new HintViewModel(new TextObject("{=LzdUwRJo}Select a region for Quick Play and Custom Game", null), null);
			this.QuickplayGameTypes.ApplyActionOnAllItems(delegate(MPMatchmakingItemVM x)
			{
				x.RefreshValues();
			});
			this.RankedGameTypes.ApplyActionOnAllItems(delegate(MPMatchmakingItemVM x)
			{
				x.RefreshValues();
			});
			this.CustomGameTypes.ApplyActionOnAllItems(delegate(MPMatchmakingItemVM x)
			{
				x.RefreshValues();
			});
			this.CustomServer.RefreshValues();
			this.PremadeMatches.RefreshValues();
			this._regionsRequireRefresh = true;
			this.Regions.RefreshValues();
			this.SelectionInfo.RefreshValues();
		}

		private void RefreshRegionsList()
		{
			string currentRegion = MultiplayerMain.GetUserCurrentRegion();
			string[] availableMatchmakerRegions = MultiplayerMain.GetAvailableMatchmakerRegions();
			List<MPMatchmakingRegionSelectorItemVM> list = new List<MPMatchmakingRegionSelectorItemVM>();
			if (this._isTestRegionAvailable)
			{
				MPMatchmakingRegionSelectorItemVM mpmatchmakingRegionSelectorItemVM = new MPMatchmakingRegionSelectorItemVM("Test", new TextObject("{=!}Test", null));
				list.Add(mpmatchmakingRegionSelectorItemVM);
			}
			foreach (string text in availableMatchmakerRegions)
			{
				TextObject textObject = GameTexts.FindText("str_multiplayer_region_name", text);
				list.Add(new MPMatchmakingRegionSelectorItemVM(text, textObject));
			}
			list.Add(new MPMatchmakingRegionSelectorItemVM("None", GameTexts.FindText("str_multiplayer_region_name_none", null)));
			int num = list.FindIndex((MPMatchmakingRegionSelectorItemVM r) => r.RegionCode == currentRegion);
			int num2;
			if (num == -1)
			{
				num2 = list.FindIndex((MPMatchmakingRegionSelectorItemVM r) => r.IsRegionNone);
			}
			else
			{
				num2 = num;
			}
			int num3 = num2;
			this.Regions.Refresh(list, num3, new Action<SelectorVM<MPMatchmakingRegionSelectorItemVM>>(this.OnRegionSelectionChanged));
		}

		internal void OnTick(float dt)
		{
			if (this._regionsRequireRefresh)
			{
				this.RefreshRegionsList();
				this._regionsRequireRefresh = false;
				this.OnSelectionChanged(true, true);
			}
		}

		public void TrySetMatchmakingSubPage(MPMatchmakingVM.MatchmakingSubPages newPage)
		{
			if (this._currentSubPage != newPage)
			{
				if ((newPage == MPMatchmakingVM.MatchmakingSubPages.CustomGameList || newPage == MPMatchmakingVM.MatchmakingSubPages.CustomGame) && !this.IsServerCustomGameListAvailable)
				{
					return;
				}
				if (newPage == MPMatchmakingVM.MatchmakingSubPages.QuickPlay && !this.IsServerQuickPlayAvailable)
				{
					return;
				}
				if (newPage == MPMatchmakingVM.MatchmakingSubPages.Default)
				{
					if (this.IsServerQuickPlayAvailable && !this.HasUnofficialModulesLoaded)
					{
						newPage = MPMatchmakingVM.MatchmakingSubPages.QuickPlay;
					}
					else
					{
						if (!this.IsServerCustomGameListAvailable)
						{
							Debug.FailedAssert("Trying to open matchmaking when nothing is available", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection\\Lobby\\OfficialGame\\MPMatchmakingVM.cs", "TrySetMatchmakingSubPage", 179);
							return;
						}
						newPage = MPMatchmakingVM.MatchmakingSubPages.CustomGameList;
					}
				}
				this._currentSubPage = newPage;
				this.SelectedSubPageIndex = (int)newPage;
				this.CustomServer.IsEnabled = newPage == MPMatchmakingVM.MatchmakingSubPages.CustomGameList;
				this.PremadeMatches.IsEnabled = newPage == MPMatchmakingVM.MatchmakingSubPages.PremadeMatchList;
				this.IsCustomGameStageFindEnabled = this.CustomGameTypes.Any((MPMatchmakingItemVM g) => g.IsSelected);
				this.OnSelectionChanged(false, false);
			}
		}

		public void RefreshPlayerData(PlayerData playerData)
		{
			if (!string.IsNullOrEmpty(playerData.LastRegion))
			{
				for (int i = 0; i < this.Regions.ItemList.Count; i++)
				{
					if (playerData.LastRegion == this.Regions.ItemList[i].RegionCode)
					{
						this.Regions.SelectedIndex = i;
						break;
					}
				}
			}
			else
			{
				this.Regions.SelectedIndex = Extensions.FindIndex<MPMatchmakingRegionSelectorItemVM>(this.Regions.ItemList, (MPMatchmakingRegionSelectorItemVM r) => r.IsRegionNone);
			}
			string[] lastGameTypes = playerData.LastGameTypes;
			if (lastGameTypes != null)
			{
				using (IEnumerator<MPMatchmakingItemVM> enumerator = this.QuickplayGameTypes.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						MPMatchmakingItemVM mpmatchmakingItemVM = enumerator.Current;
						mpmatchmakingItemVM.IsSelected = lastGameTypes.Contains(mpmatchmakingItemVM.Name);
					}
					return;
				}
			}
			foreach (MPMatchmakingItemVM mpmatchmakingItemVM2 in this.QuickplayGameTypes)
			{
				mpmatchmakingItemVM2.IsSelected = false;
			}
		}

		private void GameModeOnSetFocusItem(MPMatchmakingItemVM sender)
		{
			this.SelectionInfo.UpdateForGameType(sender.Type);
			this.SelectionInfo.SetEnabled(true);
		}

		private void GameModeOnRemoveFocus()
		{
			this.SelectionInfo.SetEnabled(false);
		}

		private void GameModeOnSelectionChanged(MPMatchmakingItemVM sender, bool isSelected)
		{
			this.OnSelectionChanged(false, true);
		}

		private void OnRegionSelectionChanged(SelectorVM<MPMatchmakingRegionSelectorItemVM> selectorVM)
		{
			if (selectorVM.SelectedItem != null)
			{
				this.OnSelectionChanged(true, false);
				return;
			}
			this._regionsRequireRefresh = true;
		}

		private void OnSelectionChanged(bool updatedRegion = false, bool updatedGameTypes = false)
		{
			string[] array;
			bool selectedGameTypesInfo = this.GetSelectedGameTypesInfo(out array);
			this._selectionInfoTextObject.SetTextVariable("GAME_TYPES", MPLobbyVM.GetLocalizedGameTypesString(array));
			this.IsMatchFindPossible = this.SelectedSubPageIndex == 0 && selectedGameTypesInfo && NetworkMain.GameClient.IsAbleToSearchForGame;
			this.IsCustomGameStageFindEnabled = this.SelectedSubPageIndex == 1 && selectedGameTypesInfo;
			if (!this._regionsRequireRefresh && updatedRegion)
			{
				MPMatchmakingRegionSelectorItemVM selectedItem = this.Regions.SelectedItem;
				if (selectedItem != null && !selectedItem.IsRegionNone)
				{
					MPMatchmakingRegionSelectorItemVM selectedItem2 = this.Regions.SelectedItem;
					string text = ((selectedItem2 != null) ? selectedItem2.StringItem : null);
					this._selectionInfoTextObject.SetTextVariable("REGIONS", text);
					string regionCode = this.Regions.SelectedItem.RegionCode;
					NetworkMain.GameClient.ChangeRegion(regionCode);
				}
			}
			if (updatedGameTypes && this.IsMatchFindPossible)
			{
				NetworkMain.GameClient.ChangeGameTypes(array.ToArray<string>());
			}
			this.SelectionInfoText = this._selectionInfoTextObject.ToString();
			Action<string, bool> onMatchSelectionChanged = this._onMatchSelectionChanged;
			if (onMatchSelectionChanged == null)
			{
				return;
			}
			onMatchSelectionChanged(this.SelectionInfoText, this.IsMatchFindPossible);
		}

		internal void OnServerStatusReceived(ServerStatus serverStatus)
		{
			this._isServerStatusReceived = true;
			this.CustomServer.IsPlayerBasedCustomBattleEnabled = serverStatus.IsPlayerBasedCustomBattleEnabled;
			this.PremadeMatches.IsPremadeGameEnabled = serverStatus.IsPremadeGameEnabled;
			if (this._isTestRegionAvailable != serverStatus.IsTestRegionEnabled)
			{
				this._isTestRegionAvailable = serverStatus.IsTestRegionEnabled;
				this._regionsRequireRefresh = true;
			}
			this.RefreshSubPageStates();
		}

		public void OnFindingGame()
		{
			this.IsFindingMatch = true;
			this.RefreshSubPageStates();
		}

		public void OnCancelFindingGame()
		{
			this.IsFindingMatch = false;
			this.RefreshSubPageStates();
		}

		public override void OnFinalize()
		{
			InformationManager.OnHideInquiry -= this.OnHideInquiry;
			this.CustomServer.OnFinalize();
			foreach (MPMatchmakingItemVM mpmatchmakingItemVM in this.RankedGameTypes)
			{
				mpmatchmakingItemVM.OnSelectionChanged -= this.GameModeOnSelectionChanged;
				mpmatchmakingItemVM.OnSetFocusItem -= this.GameModeOnSetFocusItem;
				mpmatchmakingItemVM.OnRemoveFocus -= this.GameModeOnRemoveFocus;
			}
			foreach (MPMatchmakingItemVM mpmatchmakingItemVM2 in this.QuickplayGameTypes)
			{
				mpmatchmakingItemVM2.OnSelectionChanged -= this.GameModeOnSelectionChanged;
				mpmatchmakingItemVM2.OnSetFocusItem -= this.GameModeOnSetFocusItem;
				mpmatchmakingItemVM2.OnRemoveFocus -= this.GameModeOnRemoveFocus;
			}
			foreach (MPMatchmakingItemVM mpmatchmakingItemVM3 in this.CustomGameTypes)
			{
				mpmatchmakingItemVM3.OnSelectionChanged -= this.GameModeOnSelectionChanged;
				mpmatchmakingItemVM3.OnSetFocusItem -= this.GameModeOnSetFocusItem;
				mpmatchmakingItemVM3.OnRemoveFocus -= this.GameModeOnRemoveFocus;
			}
			base.OnFinalize();
		}

		public bool GetSelectedGameTypesInfo(out string[] gameTypes)
		{
			List<string> list = new List<string>();
			bool flag = false;
			MBBindingList<MPMatchmakingItemVM> currentSubPageList = this.GetCurrentSubPageList();
			for (int i = 0; i < currentSubPageList.Count; i++)
			{
				MPMatchmakingItemVM mpmatchmakingItemVM = currentSubPageList[i];
				if (mpmatchmakingItemVM.IsSelected)
				{
					list.Add(mpmatchmakingItemVM.Type);
					flag = true;
				}
			}
			gameTypes = list.ToArray();
			return flag;
		}

		private MBBindingList<MPMatchmakingItemVM> GetCurrentSubPageList()
		{
			switch (this.SelectedSubPageIndex)
			{
			case 1:
				return this.CustomGameTypes;
			}
			return this.QuickplayGameTypes;
		}

		private void OnHideInquiry()
		{
			if (this.IsFindingMatch)
			{
				this.ExecuteCancelFindingGame();
			}
		}

		public void RefreshWaitingTime()
		{
			MBTextManager.SetTextVariable("WAIT_TIME", 10);
		}

		public void ExecuteAutoFindGame()
		{
			if (!this.QuickplayGameTypes.Where((MPMatchmakingItemVM q) => q.IsSelected).Any<MPMatchmakingItemVM>())
			{
				for (int i = 0; i < this.QuickplayGameTypes.Count; i++)
				{
					this.QuickplayGameTypes[i].IsSelected = true;
				}
			}
			this.ExecuteFindGame();
		}

		private async void ExecuteFindGame()
		{
			if (!this.IsFindingMatch)
			{
				if (this.Regions.SelectedItem.IsRegionNone)
				{
					InformationManager.ShowInquiry(new InquiryData(GameTexts.FindText("str_multiplayer_no_region_query_title", null).ToString(), GameTexts.FindText("str_multiplayer_no_region_query_description", null).ToString(), true, false, GameTexts.FindText("str_ok", null).ToString(), "", delegate
					{
						Action<MPLobbyVM.LobbyPage> onChangePageRequest = this._onChangePageRequest;
						if (onChangePageRequest == null)
						{
							return;
						}
						onChangePageRequest(MPLobbyVM.LobbyPage.Matchmaking);
					}, null, "", 0f, null, null, null), false, false);
				}
				else
				{
					string[] array = (from q in this.GetCurrentSubPageList()
						where q.IsSelected
						select q.Type).ToArray<string>();
					if (array.Length != 0)
					{
						if (this.SelectedSubPageIndex == 1)
						{
							TaskAwaiter<bool> taskAwaiter = NetworkMain.GameClient.FindCustomGame(array, this._lobbyState.HasCrossplayPrivilege, MultiplayerMain.GetUserCurrentRegion()).GetAwaiter();
							if (!taskAwaiter.IsCompleted)
							{
								await taskAwaiter;
								TaskAwaiter<bool> taskAwaiter2;
								taskAwaiter = taskAwaiter2;
								taskAwaiter2 = default(TaskAwaiter<bool>);
							}
							if (!taskAwaiter.GetResult())
							{
								InformationManager.ShowInquiry(new InquiryData("", new TextObject("{=NaZ6xg33}Couldn't find an applicable server to join.", null).ToString(), true, false, new TextObject("{=yS7PvrTD}OK", null).ToString(), "", null, null, "", 0f, null, null, null), false, false);
							}
						}
						else
						{
							Action<bool> onGameFindRequestStateChanged = this._onGameFindRequestStateChanged;
							if (onGameFindRequestStateChanged != null)
							{
								onGameFindRequestStateChanged(true);
							}
							PlatformServices.Instance.CheckPrivilege(2, true, delegate(bool result)
							{
								Action<bool> onGameFindRequestStateChanged2 = this._onGameFindRequestStateChanged;
								if (onGameFindRequestStateChanged2 != null)
								{
									onGameFindRequestStateChanged2(false);
								}
								if (result)
								{
									NetworkMain.GameClient.FindGame();
								}
							});
						}
					}
				}
			}
		}

		public void RefreshSubPageStates()
		{
			if (this._isServerStatusReceived)
			{
				this.IsCustomServerListEnabled = this.IsServerCustomGameListAvailable;
				this.IsQuickplayGamesEnabled = this.IsServerQuickPlayAvailable && !this.HasUnofficialModulesLoaded;
				this.IsCustomGamesEnabled = this.IsServerCustomGameListAvailable && !this.HasUnofficialModulesLoaded;
				this.IsRankedGamesEnabled = false;
				this._isEligibleForPremadeMatches = NetworkMain.GameClient.IsEligibleToCreatePremadeGame;
				this.IsPremadeGamesEnabled = this._isEligibleForPremadeMatches && !this.IsFindingMatch && !this.HasUnofficialModulesLoaded;
				if ((this.CurrentSubPage == MPMatchmakingVM.MatchmakingSubPages.CustomGame || this.CurrentSubPage == MPMatchmakingVM.MatchmakingSubPages.CustomGameList) && !this.IsCustomServerListEnabled)
				{
					this.TrySetMatchmakingSubPage(MPMatchmakingVM.MatchmakingSubPages.Default);
					return;
				}
				if (this.CurrentSubPage == MPMatchmakingVM.MatchmakingSubPages.QuickPlay && !this.IsServerQuickPlayAvailable)
				{
					this.TrySetMatchmakingSubPage(MPMatchmakingVM.MatchmakingSubPages.Default);
					return;
				}
			}
			else
			{
				this.IsCustomServerListEnabled = false;
				this.IsQuickplayGamesEnabled = false;
				this.IsRankedGamesEnabled = false;
				this.IsCustomGamesEnabled = false;
				this.IsPremadeGamesEnabled = false;
			}
		}

		private void ExecuteCancelFindingGame()
		{
			if (!this.IsFindingMatch)
			{
				return;
			}
			this.OnCancelFindingGame();
			NetworkMain.GameClient.CancelFindGame();
		}

		private void IsFindingMatchUpdated()
		{
			foreach (MPMatchmakingItemVM mpmatchmakingItemVM in this.RankedGameTypes)
			{
				mpmatchmakingItemVM.IsAvailable = !this.IsFindingMatch;
			}
		}

		private void ExecuteChangeEnabledSubPage(int subpageIndex)
		{
			this.TrySetMatchmakingSubPage((MPMatchmakingVM.MatchmakingSubPages)subpageIndex);
		}

		private void UpdateRankedGameTypesList()
		{
			MultiplayerGameType[] availableRankedGameModes = MultiplayerMain.GetAvailableRankedGameModes();
			for (int i = 0; i < availableRankedGameModes.Length; i++)
			{
				MPMatchmakingItemVM mpmatchmakingItemVM = new MPMatchmakingItemVM(availableRankedGameModes[i]);
				mpmatchmakingItemVM.IsSelected = this._defaultSelectedGameTypes.Contains(mpmatchmakingItemVM.Type);
				mpmatchmakingItemVM.OnSelectionChanged += this.GameModeOnSelectionChanged;
				mpmatchmakingItemVM.OnSetFocusItem += this.GameModeOnSetFocusItem;
				mpmatchmakingItemVM.OnRemoveFocus += this.GameModeOnRemoveFocus;
				this.RankedGameTypes.Add(mpmatchmakingItemVM);
			}
		}

		private void UpdateQuickPlayGameTypeList()
		{
			MultiplayerGameType[] availableQuickPlayGameModes = MultiplayerMain.GetAvailableQuickPlayGameModes();
			for (int i = 0; i < availableQuickPlayGameModes.Length; i++)
			{
				MPMatchmakingItemVM mpmatchmakingItemVM = new MPMatchmakingItemVM(availableQuickPlayGameModes[i]);
				mpmatchmakingItemVM.IsSelected = this._defaultSelectedGameTypes.Contains(mpmatchmakingItemVM.Type);
				mpmatchmakingItemVM.OnSelectionChanged += this.GameModeOnSelectionChanged;
				mpmatchmakingItemVM.OnSetFocusItem += this.GameModeOnSetFocusItem;
				mpmatchmakingItemVM.OnRemoveFocus += this.GameModeOnRemoveFocus;
				this.QuickplayGameTypes.Add(mpmatchmakingItemVM);
			}
		}

		private void UpdateCustomGameTypeList()
		{
			MultiplayerGameType[] availableCustomGameModes = MultiplayerMain.GetAvailableCustomGameModes();
			for (int i = 0; i < availableCustomGameModes.Length; i++)
			{
				MPMatchmakingItemVM mpmatchmakingItemVM = new MPMatchmakingItemVM(availableCustomGameModes[i]);
				mpmatchmakingItemVM.IsSelected = this._defaultSelectedGameTypes.Contains(mpmatchmakingItemVM.Type);
				mpmatchmakingItemVM.OnSelectionChanged += this.GameModeOnSelectionChanged;
				mpmatchmakingItemVM.OnSetFocusItem += this.GameModeOnSetFocusItem;
				mpmatchmakingItemVM.OnRemoveFocus += this.GameModeOnRemoveFocus;
				this.CustomGameTypes.Add(mpmatchmakingItemVM);
			}
		}

		public void OnPremadeGameEligibilityStatusReceived(bool isEligible)
		{
			this._isEligibleForPremadeMatches = isEligible;
			this.IsPremadeGamesEnabled = this._isEligibleForPremadeMatches && !this.IsFindingMatch && !this.HasUnofficialModulesLoaded;
		}

		public void OnSupportedFeaturesRefreshed(SupportedFeatures supportedFeatures)
		{
			this.IsCustomServerFeatureSupported = supportedFeatures.SupportsFeatures(2);
			this.IsClansFeatureSupported = supportedFeatures.SupportsFeatures(8);
		}

		[DataSourceProperty]
		public bool IsEnabled
		{
			get
			{
				return this._isEnabled;
			}
			set
			{
				if (value != this._isEnabled)
				{
					this._isEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsEnabled");
				}
			}
		}

		[DataSourceProperty]
		public bool HasUnofficialModulesLoaded
		{
			get
			{
				return this._hasUnofficialModulesLoaded;
			}
			set
			{
				if (value != this._hasUnofficialModulesLoaded)
				{
					this._hasUnofficialModulesLoaded = value;
					base.OnPropertyChangedWithValue(value, "HasUnofficialModulesLoaded");
				}
			}
		}

		[DataSourceProperty]
		public bool IsCustomGameStageFindEnabled
		{
			get
			{
				return this._isCustomGameStageFindEnabled;
			}
			set
			{
				if (value != this._isCustomGameStageFindEnabled)
				{
					this._isCustomGameStageFindEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsCustomGameStageFindEnabled");
				}
			}
		}

		[DataSourceProperty]
		public bool IsRankedGamesEnabled
		{
			get
			{
				return this._isRankedGamesEnabled;
			}
			set
			{
				if (value != this._isRankedGamesEnabled)
				{
					this._isRankedGamesEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsRankedGamesEnabled");
				}
			}
		}

		[DataSourceProperty]
		public bool IsCustomGamesEnabled
		{
			get
			{
				return this._isCustomGamesEnabled;
			}
			set
			{
				if (value != this._isCustomGamesEnabled)
				{
					this._isCustomGamesEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsCustomGamesEnabled");
				}
			}
		}

		[DataSourceProperty]
		public bool IsQuickplayGamesEnabled
		{
			get
			{
				return this._isQuickplayGamesEnabled;
			}
			set
			{
				if (value != this._isQuickplayGamesEnabled)
				{
					this._isQuickplayGamesEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsQuickplayGamesEnabled");
				}
			}
		}

		[DataSourceProperty]
		public bool IsCustomServerListEnabled
		{
			get
			{
				return this._isCustomServerListEnabled;
			}
			set
			{
				if (value != this._isCustomServerListEnabled)
				{
					this._isCustomServerListEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsCustomServerListEnabled");
				}
			}
		}

		[DataSourceProperty]
		public bool IsPremadeGamesEnabled
		{
			get
			{
				return this._isPremadeGamesEnabled;
			}
			set
			{
				if (value != this._isPremadeGamesEnabled)
				{
					this._isPremadeGamesEnabled = value;
					base.OnPropertyChanged("IsPremadeGamesEnabled");
				}
			}
		}

		[DataSourceProperty]
		public bool IsCustomServerFeatureSupported
		{
			get
			{
				return this._isCustomServerFeatureSupported;
			}
			set
			{
				if (value != this._isCustomServerFeatureSupported)
				{
					this._isCustomServerFeatureSupported = value;
					base.OnPropertyChanged("IsCustomServerFeatureSupported");
				}
			}
		}

		[DataSourceProperty]
		public bool IsClansFeatureSupported
		{
			get
			{
				return this._isClansFeatureSupported;
			}
			set
			{
				if (value != this._isClansFeatureSupported)
				{
					this._isClansFeatureSupported = value;
					base.OnPropertyChanged("IsClansFeatureSupported");
				}
			}
		}

		[DataSourceProperty]
		public MPCustomGameVM CustomServer
		{
			get
			{
				return this._customServer;
			}
			set
			{
				if (value != this._customServer)
				{
					this._customServer = value;
					base.OnPropertyChangedWithValue<MPCustomGameVM>(value, "CustomServer");
				}
			}
		}

		[DataSourceProperty]
		public MPCustomGameVM PremadeMatches
		{
			get
			{
				return this._premadeMatches;
			}
			set
			{
				if (value != this._premadeMatches)
				{
					this._premadeMatches = value;
					base.OnPropertyChanged("PremadeMatches");
				}
			}
		}

		[DataSourceProperty]
		public bool IsRanked
		{
			get
			{
				return this._isRanked;
			}
			set
			{
				if (value != this._isRanked)
				{
					this._isRanked = value;
					base.OnPropertyChangedWithValue(value, "IsRanked");
					this.OnSelectionChanged(false, false);
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<MPMatchmakingItemVM> RankedGameTypes
		{
			get
			{
				return this._rankedGameTypes;
			}
			set
			{
				if (value != this._rankedGameTypes)
				{
					this._rankedGameTypes = value;
					base.OnPropertyChangedWithValue<MBBindingList<MPMatchmakingItemVM>>(value, "RankedGameTypes");
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<MPMatchmakingItemVM> QuickplayGameTypes
		{
			get
			{
				return this._quickplayGameTypes;
			}
			set
			{
				if (value != this._quickplayGameTypes)
				{
					this._quickplayGameTypes = value;
					base.OnPropertyChangedWithValue<MBBindingList<MPMatchmakingItemVM>>(value, "QuickplayGameTypes");
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<MPMatchmakingItemVM> CustomGameTypes
		{
			get
			{
				return this._customGameTypes;
			}
			set
			{
				if (value != this._customGameTypes)
				{
					this._customGameTypes = value;
					base.OnPropertyChangedWithValue<MBBindingList<MPMatchmakingItemVM>>(value, "CustomGameTypes");
				}
			}
		}

		[DataSourceProperty]
		public SelectorVM<MPMatchmakingRegionSelectorItemVM> Regions
		{
			get
			{
				return this._regions;
			}
			set
			{
				if (value != this._regions)
				{
					this._regions = value;
					base.OnPropertyChangedWithValue<SelectorVM<MPMatchmakingRegionSelectorItemVM>>(value, "Regions");
				}
			}
		}

		[DataSourceProperty]
		public MPMatchmakingSelectionInfoVM SelectionInfo
		{
			get
			{
				return this._selectionInfo;
			}
			set
			{
				if (value != this._selectionInfo)
				{
					this._selectionInfo = value;
					base.OnPropertyChangedWithValue<MPMatchmakingSelectionInfoVM>(value, "SelectionInfo");
				}
			}
		}

		[DataSourceProperty]
		public bool IsMatchFindPossible
		{
			get
			{
				return this._isMatchFindPossible;
			}
			set
			{
				if (value != this._isMatchFindPossible)
				{
					this._isMatchFindPossible = value;
					base.OnPropertyChangedWithValue(value, "IsMatchFindPossible");
				}
			}
		}

		[DataSourceProperty]
		public bool IsFindingMatch
		{
			get
			{
				return this._isFindingMatch;
			}
			set
			{
				if (value != this._isFindingMatch)
				{
					this._isFindingMatch = value;
					base.OnPropertyChangedWithValue(value, "IsFindingMatch");
					this.IsFindingMatchUpdated();
				}
			}
		}

		[DataSourceProperty]
		public string PlayText
		{
			get
			{
				return this._playText;
			}
			set
			{
				if (value != this._playText)
				{
					this._playText = value;
					base.OnPropertyChangedWithValue<string>(value, "PlayText");
				}
			}
		}

		[DataSourceProperty]
		public string QuickPlayText
		{
			get
			{
				return this._quickPlayText;
			}
			set
			{
				if (value != this._quickPlayText)
				{
					this._quickPlayText = value;
					base.OnPropertyChangedWithValue<string>(value, "QuickPlayText");
				}
			}
		}

		[DataSourceProperty]
		public string CustomGameText
		{
			get
			{
				return this._customGameText;
			}
			set
			{
				if (value != this._customGameText)
				{
					this._customGameText = value;
					base.OnPropertyChangedWithValue<string>(value, "CustomGameText");
				}
			}
		}

		[DataSourceProperty]
		public string CommunityGameText
		{
			get
			{
				return this._communityGameText;
			}
			set
			{
				if (value != this._communityGameText)
				{
					this._communityGameText = value;
					base.OnPropertyChangedWithValue<string>(value, "CommunityGameText");
				}
			}
		}

		[DataSourceProperty]
		public string CustomServerListText
		{
			get
			{
				return this._customServerListText;
			}
			set
			{
				if (value != this._customServerListText)
				{
					this._customServerListText = value;
					base.OnPropertyChangedWithValue<string>(value, "CustomServerListText");
				}
			}
		}

		[DataSourceProperty]
		public string AutoFindText
		{
			get
			{
				return this._autoFindText;
			}
			set
			{
				if (value != this._autoFindText)
				{
					this._autoFindText = value;
					base.OnPropertyChangedWithValue<string>(value, "AutoFindText");
				}
			}
		}

		[DataSourceProperty]
		public string MatchFindNotPossibleText
		{
			get
			{
				return this._matchFindNotPossibleText;
			}
			set
			{
				if (value != this._matchFindNotPossibleText)
				{
					this._matchFindNotPossibleText = value;
					base.OnPropertyChangedWithValue<string>(value, "MatchFindNotPossibleText");
				}
			}
		}

		[DataSourceProperty]
		public string RankedText
		{
			get
			{
				return this._rankedText;
			}
			set
			{
				if (value != this._rankedText)
				{
					this._rankedText = value;
					base.OnPropertyChangedWithValue<string>(value, "RankedText");
				}
			}
		}

		[DataSourceProperty]
		public string CasualText
		{
			get
			{
				return this._casualText;
			}
			set
			{
				if (value != this._casualText)
				{
					this._casualText = value;
					base.OnPropertyChangedWithValue<string>(value, "CasualText");
				}
			}
		}

		[DataSourceProperty]
		public string SelectionInfoText
		{
			get
			{
				return this._selectionInfoText;
			}
			set
			{
				if (value != this._selectionInfoText)
				{
					this._selectionInfoText = value;
					base.OnPropertyChangedWithValue<string>(value, "SelectionInfoText");
				}
			}
		}

		[DataSourceProperty]
		public int SelectedSubPageIndex
		{
			get
			{
				return this._selectedSubPageIndex;
			}
			set
			{
				if (value != this._selectedSubPageIndex)
				{
					this._selectedSubPageIndex = value;
					base.OnPropertyChangedWithValue(value, "SelectedSubPageIndex");
				}
			}
		}

		[DataSourceProperty]
		public string TeamMatchesText
		{
			get
			{
				return this._teamMatchesText;
			}
			set
			{
				if (value != this._teamMatchesText)
				{
					this._teamMatchesText = value;
					base.OnPropertyChanged("TeamMatchesText");
				}
			}
		}

		[DataSourceProperty]
		public HintViewModel RegionsHint
		{
			get
			{
				return this._regionsHint;
			}
			set
			{
				if (value != this._regionsHint)
				{
					this._regionsHint = value;
					base.OnPropertyChanged("RegionsHint");
				}
			}
		}

		private LobbyState _lobbyState;

		private bool _isTestRegionAvailable;

		private bool _regionsRequireRefresh;

		private MPMatchmakingVM.MatchmakingSubPages _currentSubPage;

		private IEnumerable<MultiplayerClassDivisions.MPHeroClass> _heroClasses;

		private TextObject _selectionInfoTextObject;

		private string[] _defaultSelectedGameTypes;

		private bool _isServerStatusReceived;

		private bool _isEligibleForPremadeMatches;

		private readonly Action<MPLobbyVM.LobbyPage> _onChangePageRequest;

		private readonly Action<string, bool> _onMatchSelectionChanged;

		private readonly Action<bool> _onGameFindRequestStateChanged;

		private bool _isEnabled;

		private bool _isRanked;

		private bool _isCustomGameStageFindEnabled;

		private bool _hasUnofficialModulesLoaded;

		private int _selectedSubPageIndex;

		private MBBindingList<MPMatchmakingItemVM> _quickplayGameTypes;

		private MBBindingList<MPMatchmakingItemVM> _rankedGameTypes;

		private MBBindingList<MPMatchmakingItemVM> _customGameTypes;

		private SelectorVM<MPMatchmakingRegionSelectorItemVM> _regions;

		private MPMatchmakingSelectionInfoVM _selectionInfo;

		private MPCustomGameVM _customServer;

		private MPCustomGameVM _premadeMatches;

		private bool _isMatchFindPossible;

		private bool _isFindingMatch;

		private bool _isRankedGamesEnabled;

		private bool _isCustomGamesEnabled;

		private bool _isQuickplayGamesEnabled;

		private bool _isCustomServerListEnabled;

		private bool _isCustomServerFeatureSupported;

		private bool _isPremadeGamesEnabled;

		private bool _isClansFeatureSupported;

		private string _playText;

		private string _autoFindText;

		private string _matchFindNotPossibleText;

		private string _rankedText;

		private string _casualText;

		private string _selectionInfoText;

		private string _quickPlayText;

		private string _customGameText;

		private string _customServerListText;

		private string _communityGameText;

		private string _teamMatchesText;

		private HintViewModel _regionsHint;

		public enum MatchmakingSubPages
		{
			QuickPlay,
			CustomGame,
			CustomGameList,
			PremadeMatchList,
			Default
		}
	}
}
