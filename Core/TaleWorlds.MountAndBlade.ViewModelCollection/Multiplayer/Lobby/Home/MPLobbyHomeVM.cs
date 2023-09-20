using System;
using TaleWorlds.Library;
using TaleWorlds.Library.NewsManager;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.Friends;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.Home
{
	public class MPLobbyHomeVM : ViewModel
	{
		public event Action OnFindGameRequested;

		public MPLobbyHomeVM(NewsManager newsManager, Action<MPLobbyVM.LobbyPage> onChangePageRequest)
		{
			this._onChangePageRequest = onChangePageRequest;
			this.HasUnofficialModulesLoaded = NetworkMain.GameClient.HasUnofficialModulesLoaded;
			this.Player = new MPLobbyPlayerBaseVM(NetworkMain.GameClient.PlayerID, "", null, null);
			this.News = new MPNewsVM(newsManager);
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.FindGameText = new TextObject("{=yA45PqFc}FIND GAME", null).ToString();
			this.MatchFindNotPossibleText = new TextObject("{=BrYUHFsg}CHOOSE GAME", null).ToString();
			this.OpenProfileText = new TextObject("{=aBCi76ig}Show More", null).ToString();
			this.Player.RefreshValues();
			this.News.RefreshValues();
		}

		public void RefreshPlayerData(PlayerData playerData, bool updateRating = true)
		{
			this.Player.UpdateWith(playerData);
			if (updateRating)
			{
				this.Player.UpdateRating(new Action(this.OnRatingReceived));
			}
		}

		private void OnRatingReceived()
		{
			this.Player.RefreshSelectableGameTypes(true, new Action<string>(this.Player.UpdateDisplayedRankInfo), "");
		}

		public void OnMatchSelectionChanged(string selectionInfo, bool isMatchFindPossible)
		{
			this.SelectionInfoText = selectionInfo;
			this.IsMatchFindPossible = isMatchFindPossible;
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

		private void ExecuteOpenProfile()
		{
			Action<MPLobbyVM.LobbyPage> onChangePageRequest = this._onChangePageRequest;
			if (onChangePageRequest == null)
			{
				return;
			}
			onChangePageRequest(MPLobbyVM.LobbyPage.Profile);
		}

		public void OnClanInfoChanged()
		{
			this.Player.UpdateClanInfo();
		}

		public void OnPlayerNameUpdated(string playerName)
		{
			this.Player.UpdateNameAndAvatar(true);
		}

		public override void OnFinalize()
		{
			base.OnFinalize();
			this.News.OnFinalize();
			this.News = null;
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
		public string OpenProfileText
		{
			get
			{
				return this._openProfileText;
			}
			set
			{
				if (value != this._openProfileText)
				{
					this._openProfileText = value;
					base.OnPropertyChangedWithValue<string>(value, "OpenProfileText");
				}
			}
		}

		[DataSourceProperty]
		public MPLobbyPlayerBaseVM Player
		{
			get
			{
				return this._player;
			}
			set
			{
				if (value != this._player)
				{
					this._player = value;
					base.OnPropertyChangedWithValue<MPLobbyPlayerBaseVM>(value, "Player");
				}
			}
		}

		[DataSourceProperty]
		public MPNewsVM News
		{
			get
			{
				return this._news;
			}
			set
			{
				if (value != this._news)
				{
					this._news = value;
					base.OnPropertyChangedWithValue<MPNewsVM>(value, "News");
				}
			}
		}

		private readonly Action<MPLobbyVM.LobbyPage> _onChangePageRequest;

		private bool _isEnabled;

		private bool _isMatchFindPossible;

		private bool _hasUnofficialModulesLoaded;

		private string _findGameText;

		private string _matchFindNotPossibleText;

		private string _selectionInfoText;

		private string _openProfileText;

		private MPLobbyPlayerBaseVM _player;

		private MPNewsVM _news;
	}
}
