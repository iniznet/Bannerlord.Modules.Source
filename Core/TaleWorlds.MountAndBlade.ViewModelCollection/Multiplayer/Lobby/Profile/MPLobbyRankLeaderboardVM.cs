using System;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Generic;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.MountAndBlade.ViewModelCollection.Input;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.Profile
{
	public class MPLobbyRankLeaderboardVM : ViewModel
	{
		public MPLobbyRankLeaderboardVM(LobbyState lobbyState)
		{
			this._lobbyState = lobbyState;
			this.LeaderboardPlayers = new MBBindingList<MPLobbyLeaderboardPlayerItemVM>();
			this.PlayerActions = new MBBindingList<StringPairItemWithActionVM>();
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.TitleText = new TextObject("{=vGF5S2hE}Leaderboard", null).ToString();
			this.CloseText = new TextObject("{=yQtzabbe}Close", null).ToString();
			this.NoDataAvailableText = this._noDataAvailableTextObject.ToString();
		}

		public async void OpenWith(string gameType)
		{
			this.HasData = true;
			this.LeaderboardPlayers.Clear();
			this.IsEnabled = true;
			this.IsDataLoading = true;
			PlayerLeaderboardData[] array = await NetworkMain.GameClient.GetRankedLeaderboard(gameType);
			PlayerLeaderboardData[] leaderboardPlayerInfos = array;
			await this._lobbyState.UpdateHasUserGeneratedContentPrivilege(true);
			if (leaderboardPlayerInfos != null)
			{
				this.HasData = leaderboardPlayerInfos.Length != 0;
				for (int i = 0; i < leaderboardPlayerInfos.Length; i++)
				{
					MPLobbyLeaderboardPlayerItemVM mplobbyLeaderboardPlayerItemVM = new MPLobbyLeaderboardPlayerItemVM(i + 1, leaderboardPlayerInfos[i], new Action<MPLobbyLeaderboardPlayerItemVM>(this.ActivatePlayerActions));
					this.LeaderboardPlayers.Add(mplobbyLeaderboardPlayerItemVM);
				}
			}
			else
			{
				this.HasData = false;
			}
			this.IsDataLoading = false;
		}

		public void ExecuteClosePopup()
		{
			this.IsEnabled = false;
		}

		public void ActivatePlayerActions(MPLobbyLeaderboardPlayerItemVM playerVM)
		{
			this.PlayerActions.Clear();
			if (playerVM.ProvidedID != NetworkMain.GameClient.PlayerID)
			{
				bool flag = false;
				FriendInfo[] friendInfos = NetworkMain.GameClient.FriendInfos;
				for (int i = 0; i < friendInfos.Length; i++)
				{
					if (friendInfos[i].Id == playerVM.ProvidedID)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					this.PlayerActions.Add(new StringPairItemWithActionVM(new Action<object>(this.ExecuteRequestFriendship), new TextObject("{=UwkpJq9N}Add As Friend", null).ToString(), "RequestFriendship", playerVM));
				}
				else
				{
					this.PlayerActions.Add(new StringPairItemWithActionVM(new Action<object>(this.ExecuteTerminateFriendship), new TextObject("{=2YIVRuRa}Remove From Friends", null).ToString(), "TerminateFriendship", playerVM));
				}
				MultiplayerPlayerContextMenuHelper.AddLobbyViewProfileOptions(playerVM, this.PlayerActions);
				StringPairItemWithActionVM stringPairItemWithActionVM = new StringPairItemWithActionVM(new Action<object>(this.ExecuteReport), GameTexts.FindText("str_mp_scoreboard_context_report", null).ToString(), "Report", playerVM);
				if (MultiplayerReportPlayerManager.IsPlayerReportedOverLimit(playerVM.ProvidedID))
				{
					stringPairItemWithActionVM.IsEnabled = false;
					stringPairItemWithActionVM.Hint.HintText = new TextObject("{=klkYFik9}You've already reported this player.", null);
				}
				this.PlayerActions.Add(stringPairItemWithActionVM);
			}
			this.IsPlayerActionsActive = false;
			this.IsPlayerActionsActive = this.PlayerActions.Count > 0;
		}

		private void ExecuteRequestFriendship(object playerObj)
		{
			PlayerId providedID = (playerObj as MPLobbyLeaderboardPlayerItemVM).ProvidedID;
			bool flag = BannerlordConfig.EnableGenericNames && !NetworkMain.GameClient.IsKnownPlayer(providedID);
			NetworkMain.GameClient.AddFriend(providedID, flag);
		}

		private void ExecuteTerminateFriendship(object playerObj)
		{
			NetworkMain.GameClient.RemoveFriend((playerObj as MPLobbyLeaderboardPlayerItemVM).ProvidedID);
		}

		private void ExecuteReport(object playerObj)
		{
			MultiplayerReportPlayerManager.RequestReportPlayer(Guid.Empty.ToString(), (playerObj as MPLobbyLeaderboardPlayerItemVM).ProvidedID, (playerObj as MPLobbyLeaderboardPlayerItemVM).Name, false);
		}

		public override void OnFinalize()
		{
			base.OnFinalize();
			InputKeyItemVM cancelInputKey = this.CancelInputKey;
			if (cancelInputKey == null)
			{
				return;
			}
			cancelInputKey.OnFinalize();
		}

		public void SetCancelInputKey(HotKey hotKey)
		{
			this.CancelInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
		}

		[DataSourceProperty]
		public InputKeyItemVM CancelInputKey
		{
			get
			{
				return this._cancelInputKey;
			}
			set
			{
				if (value != this._cancelInputKey)
				{
					this._cancelInputKey = value;
					base.OnPropertyChanged("CancelInputKey");
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
				}
			}
		}

		[DataSourceProperty]
		public bool IsDataLoading
		{
			get
			{
				return this._isDataLoading;
			}
			set
			{
				if (value != this._isDataLoading)
				{
					this._isDataLoading = value;
					base.OnPropertyChangedWithValue(value, "IsDataLoading");
				}
			}
		}

		[DataSourceProperty]
		public bool HasData
		{
			get
			{
				return this._hasData;
			}
			set
			{
				if (value != this._hasData)
				{
					this._hasData = value;
					base.OnPropertyChangedWithValue(value, "HasData");
				}
			}
		}

		[DataSourceProperty]
		public bool IsPlayerActionsActive
		{
			get
			{
				return this._isPlayerActionsActive;
			}
			set
			{
				if (value != this._isPlayerActionsActive)
				{
					this._isPlayerActionsActive = value;
					base.OnPropertyChangedWithValue(value, "IsPlayerActionsActive");
				}
			}
		}

		[DataSourceProperty]
		public string TitleText
		{
			get
			{
				return this._titleText;
			}
			set
			{
				if (value != this._titleText)
				{
					this._titleText = value;
					base.OnPropertyChangedWithValue<string>(value, "TitleText");
				}
			}
		}

		[DataSourceProperty]
		public string CloseText
		{
			get
			{
				return this._closeText;
			}
			set
			{
				if (value != this._closeText)
				{
					this._closeText = value;
					base.OnPropertyChangedWithValue<string>(value, "CloseText");
				}
			}
		}

		[DataSourceProperty]
		public string NoDataAvailableText
		{
			get
			{
				return this._noDataAvailableText;
			}
			set
			{
				if (value != this._noDataAvailableText)
				{
					this._noDataAvailableText = value;
					base.OnPropertyChangedWithValue<string>(value, "NoDataAvailableText");
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<MPLobbyLeaderboardPlayerItemVM> LeaderboardPlayers
		{
			get
			{
				return this._leaderboardPlayers;
			}
			set
			{
				if (value != this._leaderboardPlayers)
				{
					this._leaderboardPlayers = value;
					base.OnPropertyChangedWithValue<MBBindingList<MPLobbyLeaderboardPlayerItemVM>>(value, "LeaderboardPlayers");
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<StringPairItemWithActionVM> PlayerActions
		{
			get
			{
				return this._playerActions;
			}
			set
			{
				if (value != this._playerActions)
				{
					this._playerActions = value;
					base.OnPropertyChangedWithValue<MBBindingList<StringPairItemWithActionVM>>(value, "PlayerActions");
				}
			}
		}

		private readonly LobbyState _lobbyState;

		private readonly TextObject _noDataAvailableTextObject = new TextObject("{=*}There are currently no players in the leaderboard.", null);

		private InputKeyItemVM _cancelInputKey;

		private bool _isEnabled;

		private bool _isDataLoading;

		private bool _hasData;

		private bool _isPlayerActionsActive;

		private string _titleText;

		private string _closeText;

		private string _noDataAvailableText;

		private MBBindingList<MPLobbyLeaderboardPlayerItemVM> _leaderboardPlayers;

		private MBBindingList<StringPairItemWithActionVM> _playerActions;
	}
}
