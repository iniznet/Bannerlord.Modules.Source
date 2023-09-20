using System;
using System.Linq;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.PlatformService;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.Profile
{
	public class MPLobbyProfileVM : ViewModel
	{
		public event Action OnFindGameRequested;

		public MPLobbyProfileVM(LobbyState lobbyState, Action<MPLobbyVM.LobbyPage> onChangePageRequest, Action onOpenRecentGames)
		{
			this._onChangePageRequest = onChangePageRequest;
			this._onOpenRecentGames = onOpenRecentGames;
			this.HasUnofficialModulesLoaded = NetworkMain.GameClient.HasUnofficialModulesLoaded;
			this.PlayerInfo = new MPLobbyPlayerProfileVM(lobbyState);
			this.RecentGamesSummary = new MBBindingList<MPLobbyRecentGameItemVM>();
			this.RefreshValues();
		}

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

		public void OnMatchSelectionChanged(string selectionInfo, bool isMatchFindPossible)
		{
			this.SelectionInfoText = selectionInfo;
			this.IsMatchFindPossible = isMatchFindPossible;
		}

		public void UpdatePlayerData(PlayerData playerData, bool updateStatistics = true, bool updateRating = true)
		{
			this.PlayerInfo.UpdatePlayerData(playerData, updateStatistics, updateRating);
		}

		public void OnPlayerNameUpdated(string playerName)
		{
			MPLobbyPlayerProfileVM playerInfo = this.PlayerInfo;
			if (playerInfo == null)
			{
				return;
			}
			playerInfo.OnPlayerNameUpdated(playerName);
		}

		public void OnNotificationReceived(LobbyNotification notification)
		{
			if (notification.Type == NotificationType.BadgeEarned)
			{
				this.HasBadgeNotification = true;
			}
		}

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

		private void ExecuteOpenMatchmaking()
		{
			Action<MPLobbyVM.LobbyPage> onChangePageRequest = this._onChangePageRequest;
			if (onChangePageRequest == null)
			{
				return;
			}
			onChangePageRequest(MPLobbyVM.LobbyPage.Matchmaking);
		}

		private void ExecuteOpenRecentGames()
		{
			Action onOpenRecentGames = this._onOpenRecentGames;
			if (onOpenRecentGames == null)
			{
				return;
			}
			onOpenRecentGames();
		}

		public void OnClanInfoChanged()
		{
			this.PlayerInfo.OnClanInfoChanged();
		}

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

		private readonly Action<MPLobbyVM.LobbyPage> _onChangePageRequest;

		private readonly Action _onOpenRecentGames;

		private bool _isEnabled;

		private bool _isMatchFindPossible;

		private bool _hasUnofficialModulesLoaded;

		private bool _hasBadgeNotification;

		private string _showMoreText;

		private string _findGameText;

		private string _matchFindNotPossibleText;

		private string _selectionInfoText;

		private string _recentGamesTitleText;

		private MBBindingList<MPLobbyRecentGameItemVM> _recentGamesSummary;

		private MPLobbyPlayerProfileVM _playerInfo;
	}
}
