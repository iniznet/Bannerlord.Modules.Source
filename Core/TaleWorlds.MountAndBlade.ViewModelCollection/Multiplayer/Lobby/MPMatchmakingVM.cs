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
using TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.CustomGame;
using TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.OfficialGame;
using TaleWorlds.PlatformService;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby
{
	// Token: 0x02000064 RID: 100
	public class MPMatchmakingVM : ViewModel
	{
		// Token: 0x170002B9 RID: 697
		// (get) Token: 0x060008F0 RID: 2288 RVA: 0x00021C06 File Offset: 0x0001FE06
		public MPMatchmakingVM.MatchmakingSubPages CurrentSubPage
		{
			get
			{
				return this._currentSubPage;
			}
		}

		// Token: 0x170002BA RID: 698
		// (get) Token: 0x060008F1 RID: 2289 RVA: 0x00021C0E File Offset: 0x0001FE0E
		private bool IsServerQuickPlayAvailable
		{
			get
			{
				return NetworkMain.GameClient.IsAbleToSearchForGame;
			}
		}

		// Token: 0x170002BB RID: 699
		// (get) Token: 0x060008F2 RID: 2290 RVA: 0x00021C1A File Offset: 0x0001FE1A
		private bool IsServerCustomGameListAvailable
		{
			get
			{
				return NetworkMain.GameClient.IsCustomBattleAvailable && !this.IsFindingMatch;
			}
		}

		// Token: 0x060008F3 RID: 2291 RVA: 0x00021C34 File Offset: 0x0001FE34
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
			this._defaultSelectedGameTypes = NetworkMain.GetUserSelectedGameTypes();
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

		// Token: 0x060008F4 RID: 2292 RVA: 0x00021D2C File Offset: 0x0001FF2C
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

		// Token: 0x060008F5 RID: 2293 RVA: 0x00021EE4 File Offset: 0x000200E4
		private void RefreshRegionsList()
		{
			string currentRegion = NetworkMain.GetUserCurrentRegion();
			string[] availableMatchmakerRegions = NetworkMain.GetAvailableMatchmakerRegions();
			List<MPMatchmakingRegionSelectorItemVM> list = new List<MPMatchmakingRegionSelectorItemVM>();
			if (this._isTestRegionAvailable)
			{
				MPMatchmakingRegionSelectorItemVM mpmatchmakingRegionSelectorItemVM = new MPMatchmakingRegionSelectorItemVM("Test", new TextObject("{=*}Test", null));
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

		// Token: 0x060008F6 RID: 2294 RVA: 0x00021FE0 File Offset: 0x000201E0
		internal void OnTick(float dt)
		{
			if (this._regionsRequireRefresh)
			{
				this.RefreshRegionsList();
				this._regionsRequireRefresh = false;
				this.OnSelectionChanged(true, true);
			}
		}

		// Token: 0x060008F7 RID: 2295 RVA: 0x00022000 File Offset: 0x00020200
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
							Debug.FailedAssert("Trying to open matchmaking when nothing is available", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.ViewModelCollection\\Multiplayer\\Lobby\\OfficialGame\\MPMatchmakingVM.cs", "TrySetMatchmakingSubPage", 179);
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

		// Token: 0x060008F8 RID: 2296 RVA: 0x000220DC File Offset: 0x000202DC
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
				this.Regions.SelectedIndex = this.Regions.ItemList.FindIndex((MPMatchmakingRegionSelectorItemVM r) => r.IsRegionNone);
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

		// Token: 0x060008F9 RID: 2297 RVA: 0x00022210 File Offset: 0x00020410
		private void GameModeOnSetFocusItem(MPMatchmakingItemVM sender)
		{
			this.SelectionInfo.UpdateForGameType(sender.Type);
			this.SelectionInfo.SetEnabled(true);
		}

		// Token: 0x060008FA RID: 2298 RVA: 0x0002222F File Offset: 0x0002042F
		private void GameModeOnRemoveFocus()
		{
			this.SelectionInfo.SetEnabled(false);
		}

		// Token: 0x060008FB RID: 2299 RVA: 0x0002223D File Offset: 0x0002043D
		private void GameModeOnSelectionChanged(MPMatchmakingItemVM sender, bool isSelected)
		{
			this.OnSelectionChanged(false, true);
		}

		// Token: 0x060008FC RID: 2300 RVA: 0x00022247 File Offset: 0x00020447
		private void OnRegionSelectionChanged(SelectorVM<MPMatchmakingRegionSelectorItemVM> selectorVM)
		{
			if (selectorVM.SelectedItem != null)
			{
				this.OnSelectionChanged(true, false);
				return;
			}
			this._regionsRequireRefresh = true;
		}

		// Token: 0x060008FD RID: 2301 RVA: 0x00022264 File Offset: 0x00020464
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

		// Token: 0x060008FE RID: 2302 RVA: 0x0002237C File Offset: 0x0002057C
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

		// Token: 0x060008FF RID: 2303 RVA: 0x000223D9 File Offset: 0x000205D9
		public void OnFindingGame()
		{
			this.IsFindingMatch = true;
			this.RefreshSubPageStates();
		}

		// Token: 0x06000900 RID: 2304 RVA: 0x000223E8 File Offset: 0x000205E8
		public void OnCancelFindingGame()
		{
			this.IsFindingMatch = false;
			this.RefreshSubPageStates();
		}

		// Token: 0x06000901 RID: 2305 RVA: 0x000223F8 File Offset: 0x000205F8
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

		// Token: 0x06000902 RID: 2306 RVA: 0x00022568 File Offset: 0x00020768
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

		// Token: 0x06000903 RID: 2307 RVA: 0x000225C0 File Offset: 0x000207C0
		private MBBindingList<MPMatchmakingItemVM> GetCurrentSubPageList()
		{
			switch (this.SelectedSubPageIndex)
			{
			case 1:
				return this.CustomGameTypes;
			}
			return this.QuickplayGameTypes;
		}

		// Token: 0x06000904 RID: 2308 RVA: 0x000225FD File Offset: 0x000207FD
		private void OnHideInquiry()
		{
			if (this.IsFindingMatch)
			{
				this.ExecuteCancelFindingGame();
			}
		}

		// Token: 0x06000905 RID: 2309 RVA: 0x0002260D File Offset: 0x0002080D
		public void RefreshWaitingTime()
		{
			MBTextManager.SetTextVariable("WAIT_TIME", 10);
		}

		// Token: 0x06000906 RID: 2310 RVA: 0x0002261C File Offset: 0x0002081C
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

		// Token: 0x06000907 RID: 2311 RVA: 0x00022688 File Offset: 0x00020888
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
							TaskAwaiter<bool> taskAwaiter = NetworkMain.GameClient.FindCustomGame(array, this._lobbyState.HasCrossplayPrivilege, NetworkMain.GetUserCurrentRegion()).GetAwaiter();
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
							PlatformServices.Instance.CheckPrivilege(Privilege.Crossplay, true, delegate(bool result)
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

		// Token: 0x06000908 RID: 2312 RVA: 0x000226C4 File Offset: 0x000208C4
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

		// Token: 0x06000909 RID: 2313 RVA: 0x000227B2 File Offset: 0x000209B2
		private void ExecuteCancelFindingGame()
		{
			if (!this.IsFindingMatch)
			{
				return;
			}
			this.OnCancelFindingGame();
			NetworkMain.GameClient.CancelFindGame();
		}

		// Token: 0x0600090A RID: 2314 RVA: 0x000227D0 File Offset: 0x000209D0
		private void IsFindingMatchUpdated()
		{
			foreach (MPMatchmakingItemVM mpmatchmakingItemVM in this.RankedGameTypes)
			{
				mpmatchmakingItemVM.IsAvailable = !this.IsFindingMatch;
			}
		}

		// Token: 0x0600090B RID: 2315 RVA: 0x00022824 File Offset: 0x00020A24
		private void ExecuteChangeEnabledSubPage(int subpageIndex)
		{
			this.TrySetMatchmakingSubPage((MPMatchmakingVM.MatchmakingSubPages)subpageIndex);
		}

		// Token: 0x0600090C RID: 2316 RVA: 0x00022830 File Offset: 0x00020A30
		private void UpdateRankedGameTypesList()
		{
			MissionLobbyComponent.MultiplayerGameType[] availableRankedGameModes = NetworkMain.GetAvailableRankedGameModes();
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

		// Token: 0x0600090D RID: 2317 RVA: 0x000228B4 File Offset: 0x00020AB4
		private void UpdateQuickPlayGameTypeList()
		{
			MissionLobbyComponent.MultiplayerGameType[] availableQuickPlayGameModes = NetworkMain.GetAvailableQuickPlayGameModes();
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

		// Token: 0x0600090E RID: 2318 RVA: 0x00022938 File Offset: 0x00020B38
		private void UpdateCustomGameTypeList()
		{
			MissionLobbyComponent.MultiplayerGameType[] availableCustomGameModes = NetworkMain.GetAvailableCustomGameModes();
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

		// Token: 0x0600090F RID: 2319 RVA: 0x000229BB File Offset: 0x00020BBB
		public void OnPremadeGameEligibilityStatusReceived(bool isEligible)
		{
			this._isEligibleForPremadeMatches = isEligible;
			this.IsPremadeGamesEnabled = this._isEligibleForPremadeMatches && !this.IsFindingMatch && !this.HasUnofficialModulesLoaded;
		}

		// Token: 0x06000910 RID: 2320 RVA: 0x000229E6 File Offset: 0x00020BE6
		public void OnSupportedFeaturesRefreshed(SupportedFeatures supportedFeatures)
		{
			this.IsCustomServerFeatureSupported = supportedFeatures.SupportsFeatures(Features.CustomGame);
			this.IsClansFeatureSupported = supportedFeatures.SupportsFeatures(Features.Clan);
		}

		// Token: 0x170002BC RID: 700
		// (get) Token: 0x06000911 RID: 2321 RVA: 0x00022A02 File Offset: 0x00020C02
		// (set) Token: 0x06000912 RID: 2322 RVA: 0x00022A0A File Offset: 0x00020C0A
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

		// Token: 0x170002BD RID: 701
		// (get) Token: 0x06000913 RID: 2323 RVA: 0x00022A28 File Offset: 0x00020C28
		// (set) Token: 0x06000914 RID: 2324 RVA: 0x00022A30 File Offset: 0x00020C30
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

		// Token: 0x170002BE RID: 702
		// (get) Token: 0x06000915 RID: 2325 RVA: 0x00022A4E File Offset: 0x00020C4E
		// (set) Token: 0x06000916 RID: 2326 RVA: 0x00022A56 File Offset: 0x00020C56
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

		// Token: 0x170002BF RID: 703
		// (get) Token: 0x06000917 RID: 2327 RVA: 0x00022A74 File Offset: 0x00020C74
		// (set) Token: 0x06000918 RID: 2328 RVA: 0x00022A7C File Offset: 0x00020C7C
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

		// Token: 0x170002C0 RID: 704
		// (get) Token: 0x06000919 RID: 2329 RVA: 0x00022A9A File Offset: 0x00020C9A
		// (set) Token: 0x0600091A RID: 2330 RVA: 0x00022AA2 File Offset: 0x00020CA2
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

		// Token: 0x170002C1 RID: 705
		// (get) Token: 0x0600091B RID: 2331 RVA: 0x00022AC0 File Offset: 0x00020CC0
		// (set) Token: 0x0600091C RID: 2332 RVA: 0x00022AC8 File Offset: 0x00020CC8
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

		// Token: 0x170002C2 RID: 706
		// (get) Token: 0x0600091D RID: 2333 RVA: 0x00022AE6 File Offset: 0x00020CE6
		// (set) Token: 0x0600091E RID: 2334 RVA: 0x00022AEE File Offset: 0x00020CEE
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

		// Token: 0x170002C3 RID: 707
		// (get) Token: 0x0600091F RID: 2335 RVA: 0x00022B0C File Offset: 0x00020D0C
		// (set) Token: 0x06000920 RID: 2336 RVA: 0x00022B14 File Offset: 0x00020D14
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

		// Token: 0x170002C4 RID: 708
		// (get) Token: 0x06000921 RID: 2337 RVA: 0x00022B31 File Offset: 0x00020D31
		// (set) Token: 0x06000922 RID: 2338 RVA: 0x00022B39 File Offset: 0x00020D39
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

		// Token: 0x170002C5 RID: 709
		// (get) Token: 0x06000923 RID: 2339 RVA: 0x00022B56 File Offset: 0x00020D56
		// (set) Token: 0x06000924 RID: 2340 RVA: 0x00022B5E File Offset: 0x00020D5E
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

		// Token: 0x170002C6 RID: 710
		// (get) Token: 0x06000925 RID: 2341 RVA: 0x00022B7B File Offset: 0x00020D7B
		// (set) Token: 0x06000926 RID: 2342 RVA: 0x00022B83 File Offset: 0x00020D83
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

		// Token: 0x170002C7 RID: 711
		// (get) Token: 0x06000927 RID: 2343 RVA: 0x00022BA1 File Offset: 0x00020DA1
		// (set) Token: 0x06000928 RID: 2344 RVA: 0x00022BA9 File Offset: 0x00020DA9
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

		// Token: 0x170002C8 RID: 712
		// (get) Token: 0x06000929 RID: 2345 RVA: 0x00022BC6 File Offset: 0x00020DC6
		// (set) Token: 0x0600092A RID: 2346 RVA: 0x00022BCE File Offset: 0x00020DCE
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

		// Token: 0x170002C9 RID: 713
		// (get) Token: 0x0600092B RID: 2347 RVA: 0x00022BF4 File Offset: 0x00020DF4
		// (set) Token: 0x0600092C RID: 2348 RVA: 0x00022BFC File Offset: 0x00020DFC
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

		// Token: 0x170002CA RID: 714
		// (get) Token: 0x0600092D RID: 2349 RVA: 0x00022C1A File Offset: 0x00020E1A
		// (set) Token: 0x0600092E RID: 2350 RVA: 0x00022C22 File Offset: 0x00020E22
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

		// Token: 0x170002CB RID: 715
		// (get) Token: 0x0600092F RID: 2351 RVA: 0x00022C40 File Offset: 0x00020E40
		// (set) Token: 0x06000930 RID: 2352 RVA: 0x00022C48 File Offset: 0x00020E48
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

		// Token: 0x170002CC RID: 716
		// (get) Token: 0x06000931 RID: 2353 RVA: 0x00022C66 File Offset: 0x00020E66
		// (set) Token: 0x06000932 RID: 2354 RVA: 0x00022C6E File Offset: 0x00020E6E
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

		// Token: 0x170002CD RID: 717
		// (get) Token: 0x06000933 RID: 2355 RVA: 0x00022C8C File Offset: 0x00020E8C
		// (set) Token: 0x06000934 RID: 2356 RVA: 0x00022C94 File Offset: 0x00020E94
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

		// Token: 0x170002CE RID: 718
		// (get) Token: 0x06000935 RID: 2357 RVA: 0x00022CB2 File Offset: 0x00020EB2
		// (set) Token: 0x06000936 RID: 2358 RVA: 0x00022CBA File Offset: 0x00020EBA
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

		// Token: 0x170002CF RID: 719
		// (get) Token: 0x06000937 RID: 2359 RVA: 0x00022CD8 File Offset: 0x00020ED8
		// (set) Token: 0x06000938 RID: 2360 RVA: 0x00022CE0 File Offset: 0x00020EE0
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

		// Token: 0x170002D0 RID: 720
		// (get) Token: 0x06000939 RID: 2361 RVA: 0x00022D04 File Offset: 0x00020F04
		// (set) Token: 0x0600093A RID: 2362 RVA: 0x00022D0C File Offset: 0x00020F0C
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

		// Token: 0x170002D1 RID: 721
		// (get) Token: 0x0600093B RID: 2363 RVA: 0x00022D2F File Offset: 0x00020F2F
		// (set) Token: 0x0600093C RID: 2364 RVA: 0x00022D37 File Offset: 0x00020F37
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

		// Token: 0x170002D2 RID: 722
		// (get) Token: 0x0600093D RID: 2365 RVA: 0x00022D5A File Offset: 0x00020F5A
		// (set) Token: 0x0600093E RID: 2366 RVA: 0x00022D62 File Offset: 0x00020F62
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

		// Token: 0x170002D3 RID: 723
		// (get) Token: 0x0600093F RID: 2367 RVA: 0x00022D85 File Offset: 0x00020F85
		// (set) Token: 0x06000940 RID: 2368 RVA: 0x00022D8D File Offset: 0x00020F8D
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

		// Token: 0x170002D4 RID: 724
		// (get) Token: 0x06000941 RID: 2369 RVA: 0x00022DB0 File Offset: 0x00020FB0
		// (set) Token: 0x06000942 RID: 2370 RVA: 0x00022DB8 File Offset: 0x00020FB8
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

		// Token: 0x170002D5 RID: 725
		// (get) Token: 0x06000943 RID: 2371 RVA: 0x00022DDB File Offset: 0x00020FDB
		// (set) Token: 0x06000944 RID: 2372 RVA: 0x00022DE3 File Offset: 0x00020FE3
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

		// Token: 0x170002D6 RID: 726
		// (get) Token: 0x06000945 RID: 2373 RVA: 0x00022E06 File Offset: 0x00021006
		// (set) Token: 0x06000946 RID: 2374 RVA: 0x00022E0E File Offset: 0x0002100E
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

		// Token: 0x170002D7 RID: 727
		// (get) Token: 0x06000947 RID: 2375 RVA: 0x00022E31 File Offset: 0x00021031
		// (set) Token: 0x06000948 RID: 2376 RVA: 0x00022E39 File Offset: 0x00021039
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

		// Token: 0x170002D8 RID: 728
		// (get) Token: 0x06000949 RID: 2377 RVA: 0x00022E5C File Offset: 0x0002105C
		// (set) Token: 0x0600094A RID: 2378 RVA: 0x00022E64 File Offset: 0x00021064
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

		// Token: 0x170002D9 RID: 729
		// (get) Token: 0x0600094B RID: 2379 RVA: 0x00022E87 File Offset: 0x00021087
		// (set) Token: 0x0600094C RID: 2380 RVA: 0x00022E8F File Offset: 0x0002108F
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

		// Token: 0x170002DA RID: 730
		// (get) Token: 0x0600094D RID: 2381 RVA: 0x00022EAD File Offset: 0x000210AD
		// (set) Token: 0x0600094E RID: 2382 RVA: 0x00022EB5 File Offset: 0x000210B5
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

		// Token: 0x170002DB RID: 731
		// (get) Token: 0x0600094F RID: 2383 RVA: 0x00022ED7 File Offset: 0x000210D7
		// (set) Token: 0x06000950 RID: 2384 RVA: 0x00022EDF File Offset: 0x000210DF
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

		// Token: 0x04000465 RID: 1125
		private LobbyState _lobbyState;

		// Token: 0x04000466 RID: 1126
		private bool _isTestRegionAvailable;

		// Token: 0x04000467 RID: 1127
		private bool _regionsRequireRefresh;

		// Token: 0x04000468 RID: 1128
		private MPMatchmakingVM.MatchmakingSubPages _currentSubPage;

		// Token: 0x04000469 RID: 1129
		private IEnumerable<MultiplayerClassDivisions.MPHeroClass> _heroClasses;

		// Token: 0x0400046A RID: 1130
		private TextObject _selectionInfoTextObject;

		// Token: 0x0400046B RID: 1131
		private string[] _defaultSelectedGameTypes;

		// Token: 0x0400046C RID: 1132
		private bool _isServerStatusReceived;

		// Token: 0x0400046D RID: 1133
		private bool _isEligibleForPremadeMatches;

		// Token: 0x0400046E RID: 1134
		private readonly Action<MPLobbyVM.LobbyPage> _onChangePageRequest;

		// Token: 0x0400046F RID: 1135
		private readonly Action<string, bool> _onMatchSelectionChanged;

		// Token: 0x04000470 RID: 1136
		private readonly Action<bool> _onGameFindRequestStateChanged;

		// Token: 0x04000471 RID: 1137
		private bool _isEnabled;

		// Token: 0x04000472 RID: 1138
		private bool _isRanked;

		// Token: 0x04000473 RID: 1139
		private bool _isCustomGameStageFindEnabled;

		// Token: 0x04000474 RID: 1140
		private bool _hasUnofficialModulesLoaded;

		// Token: 0x04000475 RID: 1141
		private int _selectedSubPageIndex;

		// Token: 0x04000476 RID: 1142
		private MBBindingList<MPMatchmakingItemVM> _quickplayGameTypes;

		// Token: 0x04000477 RID: 1143
		private MBBindingList<MPMatchmakingItemVM> _rankedGameTypes;

		// Token: 0x04000478 RID: 1144
		private MBBindingList<MPMatchmakingItemVM> _customGameTypes;

		// Token: 0x04000479 RID: 1145
		private SelectorVM<MPMatchmakingRegionSelectorItemVM> _regions;

		// Token: 0x0400047A RID: 1146
		private MPMatchmakingSelectionInfoVM _selectionInfo;

		// Token: 0x0400047B RID: 1147
		private MPCustomGameVM _customServer;

		// Token: 0x0400047C RID: 1148
		private MPCustomGameVM _premadeMatches;

		// Token: 0x0400047D RID: 1149
		private bool _isMatchFindPossible;

		// Token: 0x0400047E RID: 1150
		private bool _isFindingMatch;

		// Token: 0x0400047F RID: 1151
		private bool _isRankedGamesEnabled;

		// Token: 0x04000480 RID: 1152
		private bool _isCustomGamesEnabled;

		// Token: 0x04000481 RID: 1153
		private bool _isQuickplayGamesEnabled;

		// Token: 0x04000482 RID: 1154
		private bool _isCustomServerListEnabled;

		// Token: 0x04000483 RID: 1155
		private bool _isCustomServerFeatureSupported;

		// Token: 0x04000484 RID: 1156
		private bool _isPremadeGamesEnabled;

		// Token: 0x04000485 RID: 1157
		private bool _isClansFeatureSupported;

		// Token: 0x04000486 RID: 1158
		private string _playText;

		// Token: 0x04000487 RID: 1159
		private string _autoFindText;

		// Token: 0x04000488 RID: 1160
		private string _matchFindNotPossibleText;

		// Token: 0x04000489 RID: 1161
		private string _rankedText;

		// Token: 0x0400048A RID: 1162
		private string _casualText;

		// Token: 0x0400048B RID: 1163
		private string _selectionInfoText;

		// Token: 0x0400048C RID: 1164
		private string _quickPlayText;

		// Token: 0x0400048D RID: 1165
		private string _customGameText;

		// Token: 0x0400048E RID: 1166
		private string _customServerListText;

		// Token: 0x0400048F RID: 1167
		private string _teamMatchesText;

		// Token: 0x04000490 RID: 1168
		private HintViewModel _regionsHint;

		// Token: 0x0200018B RID: 395
		public enum MatchmakingSubPages
		{
			// Token: 0x04000CF3 RID: 3315
			QuickPlay,
			// Token: 0x04000CF4 RID: 3316
			CustomGame,
			// Token: 0x04000CF5 RID: 3317
			CustomGameList,
			// Token: 0x04000CF6 RID: 3318
			PremadeMatchList,
			// Token: 0x04000CF7 RID: 3319
			Default
		}
	}
}
