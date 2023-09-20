using System;
using System.Linq;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.PlatformService;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.Profile
{
	// Token: 0x0200006C RID: 108
	public class MPLobbyProfileVM : ViewModel
	{
		// Token: 0x14000001 RID: 1
		// (add) Token: 0x060009DB RID: 2523 RVA: 0x00024174 File Offset: 0x00022374
		// (remove) Token: 0x060009DC RID: 2524 RVA: 0x000241AC File Offset: 0x000223AC
		public event Action OnFindGameRequested;

		// Token: 0x060009DD RID: 2525 RVA: 0x000241E4 File Offset: 0x000223E4
		public MPLobbyProfileVM(LobbyState lobbyState, Action<MPLobbyVM.LobbyPage> onChangePageRequest, Action onOpenRecentGames)
		{
			this._onChangePageRequest = onChangePageRequest;
			this._onOpenRecentGames = onOpenRecentGames;
			this.HasUnofficialModulesLoaded = NetworkMain.GameClient.HasUnofficialModulesLoaded;
			this.PlayerInfo = new MPLobbyPlayerProfileVM(lobbyState);
			this.RecentGamesSummary = new MBBindingList<MPLobbyRecentGameItemVM>();
			this.RefreshValues();
		}

		// Token: 0x060009DE RID: 2526 RVA: 0x00024234 File Offset: 0x00022434
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.FindGameText = new TextObject("{=yA45PqFc}FIND GAME", null).ToString();
			this.MatchFindNotPossibleText = new TextObject("{=BrYUHFsg}CHOOSE GAME", null).ToString();
			this.ShowMoreText = new TextObject("{=aBCi76ig}Show More", null).ToString();
			this.RecentGamesTitleText = new TextObject("{=NJolh9ye}Recent Games", null).ToString();
			this.RecentGamesSummary.ApplyActionOnAllItems(delegate(MPLobbyRecentGameItemVM r)
			{
				r.RefreshValues();
			});
			this.PlayerInfo.RefreshValues();
		}

		// Token: 0x060009DF RID: 2527 RVA: 0x000242D4 File Offset: 0x000224D4
		public void RefreshRecentGames(MBReadOnlyList<MatchInfo> recentGames)
		{
			this.RecentGamesSummary.Clear();
			IOrderedEnumerable<MatchInfo> orderedEnumerable = recentGames.OrderByDescending((MatchInfo m) => m.MatchDate);
			for (int i = 0; i < Math.Min(3, orderedEnumerable.Count<MatchInfo>()); i++)
			{
				MPLobbyRecentGameItemVM mplobbyRecentGameItemVM = new MPLobbyRecentGameItemVM(null);
				mplobbyRecentGameItemVM.FillFrom(orderedEnumerable.ElementAt(i));
				this.RecentGamesSummary.Add(mplobbyRecentGameItemVM);
			}
		}

		// Token: 0x060009E0 RID: 2528 RVA: 0x00024349 File Offset: 0x00022549
		public void OnMatchSelectionChanged(string selectionInfo, bool isMatchFindPossible)
		{
			this.SelectionInfoText = selectionInfo;
			this.IsMatchFindPossible = isMatchFindPossible;
		}

		// Token: 0x060009E1 RID: 2529 RVA: 0x00024359 File Offset: 0x00022559
		public void UpdatePlayerData(PlayerData playerData, bool updateStatistics = true, bool updateRating = true)
		{
			this.PlayerInfo.UpdatePlayerData(playerData, updateStatistics, updateRating);
		}

		// Token: 0x060009E2 RID: 2530 RVA: 0x00024369 File Offset: 0x00022569
		public void OnPlayerNameUpdated(string playerName)
		{
			MPLobbyPlayerProfileVM playerInfo = this.PlayerInfo;
			if (playerInfo == null)
			{
				return;
			}
			playerInfo.OnPlayerNameUpdated(playerName);
		}

		// Token: 0x060009E3 RID: 2531 RVA: 0x0002437C File Offset: 0x0002257C
		public void OnNotificationReceived(LobbyNotification notification)
		{
			if (notification.Type == NotificationType.BadgeEarned)
			{
				this.HasBadgeNotification = true;
			}
		}

		// Token: 0x060009E4 RID: 2532 RVA: 0x0002438D File Offset: 0x0002258D
		private void ExecuteFindGame()
		{
			if (this.IsMatchFindPossible)
			{
				Action onFindGameRequested = this.OnFindGameRequested;
				if (onFindGameRequested == null)
				{
					return;
				}
				onFindGameRequested();
				return;
			}
			else
			{
				Action<MPLobbyVM.LobbyPage> onChangePageRequest = this._onChangePageRequest;
				if (onChangePageRequest == null)
				{
					return;
				}
				onChangePageRequest(MPLobbyVM.LobbyPage.Matchmaking);
				return;
			}
		}

		// Token: 0x060009E5 RID: 2533 RVA: 0x000243B9 File Offset: 0x000225B9
		private void ExecuteOpenMatchmaking()
		{
			Action<MPLobbyVM.LobbyPage> onChangePageRequest = this._onChangePageRequest;
			if (onChangePageRequest == null)
			{
				return;
			}
			onChangePageRequest(MPLobbyVM.LobbyPage.Matchmaking);
		}

		// Token: 0x060009E6 RID: 2534 RVA: 0x000243CC File Offset: 0x000225CC
		private void ExecuteOpenRecentGames()
		{
			Action onOpenRecentGames = this._onOpenRecentGames;
			if (onOpenRecentGames == null)
			{
				return;
			}
			onOpenRecentGames();
		}

		// Token: 0x060009E7 RID: 2535 RVA: 0x000243DE File Offset: 0x000225DE
		public void OnClanInfoChanged()
		{
			this.PlayerInfo.OnClanInfoChanged();
		}

		// Token: 0x060009E8 RID: 2536 RVA: 0x000243EB File Offset: 0x000225EB
		private void OnEnabledChanged()
		{
			MPLobbyPlayerProfileVM playerInfo = this.PlayerInfo;
			if (((playerInfo != null) ? playerInfo.Player : null) != null)
			{
				PlatformServices.Instance.CheckPermissionWithUser(Permission.ViewUserGeneratedContent, this.PlayerInfo.Player.ProvidedID, delegate(bool hasBannerlordIDPrivilege)
				{
					this.PlayerInfo.Player.IsBannerlordIDSupported = hasBannerlordIDPrivilege;
				});
			}
		}

		// Token: 0x17000309 RID: 777
		// (get) Token: 0x060009E9 RID: 2537 RVA: 0x00024428 File Offset: 0x00022628
		// (set) Token: 0x060009EA RID: 2538 RVA: 0x00024430 File Offset: 0x00022630
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

		// Token: 0x1700030A RID: 778
		// (get) Token: 0x060009EB RID: 2539 RVA: 0x0002444E File Offset: 0x0002264E
		// (set) Token: 0x060009EC RID: 2540 RVA: 0x00024456 File Offset: 0x00022656
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

		// Token: 0x1700030B RID: 779
		// (get) Token: 0x060009ED RID: 2541 RVA: 0x00024474 File Offset: 0x00022674
		// (set) Token: 0x060009EE RID: 2542 RVA: 0x0002447C File Offset: 0x0002267C
		[DataSourceProperty]
		public string ShowMoreText
		{
			get
			{
				return this._showMoreText;
			}
			set
			{
				if (value != this._showMoreText)
				{
					this._showMoreText = value;
					base.OnPropertyChangedWithValue<string>(value, "ShowMoreText");
				}
			}
		}

		// Token: 0x1700030C RID: 780
		// (get) Token: 0x060009EF RID: 2543 RVA: 0x0002449F File Offset: 0x0002269F
		// (set) Token: 0x060009F0 RID: 2544 RVA: 0x000244A7 File Offset: 0x000226A7
		[DataSourceProperty]
		public string FindGameText
		{
			get
			{
				return this._findGameText;
			}
			set
			{
				if (value != this._findGameText)
				{
					this._findGameText = value;
					base.OnPropertyChangedWithValue<string>(value, "FindGameText");
				}
			}
		}

		// Token: 0x1700030D RID: 781
		// (get) Token: 0x060009F1 RID: 2545 RVA: 0x000244CA File Offset: 0x000226CA
		// (set) Token: 0x060009F2 RID: 2546 RVA: 0x000244D2 File Offset: 0x000226D2
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

		// Token: 0x1700030E RID: 782
		// (get) Token: 0x060009F3 RID: 2547 RVA: 0x000244F5 File Offset: 0x000226F5
		// (set) Token: 0x060009F4 RID: 2548 RVA: 0x000244FD File Offset: 0x000226FD
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
					this.OnEnabledChanged();
				}
			}
		}

		// Token: 0x1700030F RID: 783
		// (get) Token: 0x060009F5 RID: 2549 RVA: 0x00024521 File Offset: 0x00022721
		// (set) Token: 0x060009F6 RID: 2550 RVA: 0x00024529 File Offset: 0x00022729
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

		// Token: 0x17000310 RID: 784
		// (get) Token: 0x060009F7 RID: 2551 RVA: 0x0002454C File Offset: 0x0002274C
		// (set) Token: 0x060009F8 RID: 2552 RVA: 0x00024554 File Offset: 0x00022754
		[DataSourceProperty]
		public string RecentGamesTitleText
		{
			get
			{
				return this._recentGamesTitleText;
			}
			set
			{
				if (value != this._recentGamesTitleText)
				{
					this._recentGamesTitleText = value;
					base.OnPropertyChangedWithValue<string>(value, "RecentGamesTitleText");
				}
			}
		}

		// Token: 0x17000311 RID: 785
		// (get) Token: 0x060009F9 RID: 2553 RVA: 0x00024577 File Offset: 0x00022777
		// (set) Token: 0x060009FA RID: 2554 RVA: 0x0002457F File Offset: 0x0002277F
		[DataSourceProperty]
		public bool HasBadgeNotification
		{
			get
			{
				return this._hasBadgeNotification;
			}
			set
			{
				if (value != this._hasBadgeNotification)
				{
					this._hasBadgeNotification = value;
					base.OnPropertyChangedWithValue(value, "HasBadgeNotification");
				}
			}
		}

		// Token: 0x17000312 RID: 786
		// (get) Token: 0x060009FB RID: 2555 RVA: 0x0002459D File Offset: 0x0002279D
		// (set) Token: 0x060009FC RID: 2556 RVA: 0x000245A5 File Offset: 0x000227A5
		[DataSourceProperty]
		public MBBindingList<MPLobbyRecentGameItemVM> RecentGamesSummary
		{
			get
			{
				return this._recentGamesSummary;
			}
			set
			{
				if (value != this._recentGamesSummary)
				{
					this._recentGamesSummary = value;
					base.OnPropertyChangedWithValue<MBBindingList<MPLobbyRecentGameItemVM>>(value, "RecentGamesSummary");
				}
			}
		}

		// Token: 0x17000313 RID: 787
		// (get) Token: 0x060009FD RID: 2557 RVA: 0x000245C3 File Offset: 0x000227C3
		// (set) Token: 0x060009FE RID: 2558 RVA: 0x000245CB File Offset: 0x000227CB
		[DataSourceProperty]
		public MPLobbyPlayerProfileVM PlayerInfo
		{
			get
			{
				return this._playerInfo;
			}
			set
			{
				if (value != this._playerInfo)
				{
					this._playerInfo = value;
					base.OnPropertyChangedWithValue<MPLobbyPlayerProfileVM>(value, "PlayerInfo");
				}
			}
		}

		// Token: 0x040004CF RID: 1231
		private readonly Action<MPLobbyVM.LobbyPage> _onChangePageRequest;

		// Token: 0x040004D0 RID: 1232
		private readonly Action _onOpenRecentGames;

		// Token: 0x040004D1 RID: 1233
		private bool _isEnabled;

		// Token: 0x040004D2 RID: 1234
		private bool _isMatchFindPossible;

		// Token: 0x040004D3 RID: 1235
		private bool _hasUnofficialModulesLoaded;

		// Token: 0x040004D4 RID: 1236
		private bool _hasBadgeNotification;

		// Token: 0x040004D5 RID: 1237
		private string _showMoreText;

		// Token: 0x040004D6 RID: 1238
		private string _findGameText;

		// Token: 0x040004D7 RID: 1239
		private string _matchFindNotPossibleText;

		// Token: 0x040004D8 RID: 1240
		private string _selectionInfoText;

		// Token: 0x040004D9 RID: 1241
		private string _recentGamesTitleText;

		// Token: 0x040004DA RID: 1242
		private MBBindingList<MPLobbyRecentGameItemVM> _recentGamesSummary;

		// Token: 0x040004DB RID: 1243
		private MPLobbyPlayerProfileVM _playerInfo;
	}
}
