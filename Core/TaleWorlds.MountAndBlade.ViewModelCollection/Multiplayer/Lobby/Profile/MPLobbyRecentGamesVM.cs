using System;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Generic;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Diamond;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.Profile
{
	public class MPLobbyRecentGamesVM : ViewModel
	{
		public MPLobbyRecentGamesVM()
		{
			this._games = new MBBindingList<MPLobbyRecentGameItemVM>();
			this.PlayerActions = new MBBindingList<StringPairItemWithActionVM>();
			this.NoRecentGamesFoundText = new TextObject("{=TzYWE9tA}No Recent Games Found", null).ToString();
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.RecentGamesText = new TextObject("{=NJolh9ye}Recent Games", null).ToString();
			this.CloseText = GameTexts.FindText("str_close", null).ToString();
			this.Games.ApplyActionOnAllItems(delegate(MPLobbyRecentGameItemVM x)
			{
				x.RefreshValues();
			});
		}

		public void RefreshData(MBReadOnlyList<MatchInfo> matches)
		{
			this.Games.Clear();
			if (matches != null)
			{
				foreach (MatchInfo matchInfo in matches.OrderByDescending((MatchInfo m) => m.MatchDate))
				{
					if (matchInfo != null)
					{
						MPLobbyRecentGameItemVM mplobbyRecentGameItemVM = new MPLobbyRecentGameItemVM(new Action<MPLobbyRecentGamePlayerItemVM>(this.ActivatePlayerActions));
						mplobbyRecentGameItemVM.FillFrom(matchInfo);
						this.Games.Add(mplobbyRecentGameItemVM);
					}
				}
			}
			this.GotItems = matches.Count > 0;
		}

		public void ActivatePlayerActions(MPLobbyRecentGamePlayerItemVM playerVM)
		{
			this.PlayerActions.Clear();
			this._currentMatchOfTheActivePlayer = playerVM.MatchOfThePlayer;
			if (playerVM.ProvidedID != NetworkMain.GameClient.PlayerID)
			{
				StringPairItemWithActionVM stringPairItemWithActionVM = new StringPairItemWithActionVM(new Action<object>(this.ExecuteReport), GameTexts.FindText("str_mp_scoreboard_context_report", null).ToString(), "Report", playerVM);
				if (MultiplayerReportPlayerManager.IsPlayerReportedOverLimit(playerVM.ProvidedID))
				{
					stringPairItemWithActionVM.IsEnabled = false;
					stringPairItemWithActionVM.Hint.HintText = new TextObject("{=klkYFik9}You've already reported this player.", null);
				}
				this.PlayerActions.Add(stringPairItemWithActionVM);
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
			}
			this.IsPlayerActionsActive = false;
			this.IsPlayerActionsActive = this.PlayerActions.Count > 0;
		}

		private void ExecuteRequestFriendship(object playerObj)
		{
			MPLobbyRecentGamePlayerItemVM mplobbyRecentGamePlayerItemVM = playerObj as MPLobbyRecentGamePlayerItemVM;
			bool flag = BannerlordConfig.EnableGenericNames && !NetworkMain.GameClient.IsKnownPlayer(mplobbyRecentGamePlayerItemVM.ProvidedID);
			NetworkMain.GameClient.AddFriend(mplobbyRecentGamePlayerItemVM.ProvidedID, flag);
		}

		private void ExecuteTerminateFriendship(object memberObj)
		{
			MPLobbyRecentGamePlayerItemVM mplobbyRecentGamePlayerItemVM = memberObj as MPLobbyRecentGamePlayerItemVM;
			NetworkMain.GameClient.RemoveFriend(mplobbyRecentGamePlayerItemVM.ProvidedID);
		}

		private void ExecuteReport(object playerObj)
		{
			MPLobbyRecentGamePlayerItemVM mplobbyRecentGamePlayerItemVM = playerObj as MPLobbyRecentGamePlayerItemVM;
			MultiplayerReportPlayerManager.RequestReportPlayer(this._currentMatchOfTheActivePlayer.MatchId, mplobbyRecentGamePlayerItemVM.ProvidedID, mplobbyRecentGamePlayerItemVM.Name, false);
		}

		public void ExecuteOpenPopup()
		{
			this.IsEnabled = true;
		}

		public void ExecuteClosePopup()
		{
			this.IsEnabled = false;
		}

		public void OnFriendListUpdated(bool forceUpdate = false)
		{
			foreach (MPLobbyRecentGameItemVM mplobbyRecentGameItemVM in this.Games)
			{
				mplobbyRecentGameItemVM.OnFriendListUpdated(forceUpdate);
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
		public bool GotItems
		{
			get
			{
				return this._gotItems;
			}
			set
			{
				if (value != this._gotItems)
				{
					this._gotItems = value;
					base.OnPropertyChangedWithValue(value, "GotItems");
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
		public string RecentGamesText
		{
			get
			{
				return this._recentGamesText;
			}
			set
			{
				if (value != this._recentGamesText)
				{
					this._recentGamesText = value;
					base.OnPropertyChangedWithValue<string>(value, "RecentGamesText");
				}
			}
		}

		[DataSourceProperty]
		public string NoRecentGamesFoundText
		{
			get
			{
				return this._noRecentGamesFoundText;
			}
			set
			{
				if (value != this._noRecentGamesFoundText)
				{
					this._noRecentGamesFoundText = value;
					base.OnPropertyChangedWithValue<string>(value, "NoRecentGamesFoundText");
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

		[DataSourceProperty]
		public MBBindingList<MPLobbyRecentGameItemVM> Games
		{
			get
			{
				return this._games;
			}
			set
			{
				if (value != this._games)
				{
					this._games = value;
					base.OnPropertyChangedWithValue<MBBindingList<MPLobbyRecentGameItemVM>>(value, "Games");
				}
			}
		}

		private MatchInfo _currentMatchOfTheActivePlayer;

		private bool _isEnabled;

		private bool _gotItems;

		private bool _isPlayerActionsActive;

		private string _recentGamesText;

		private string _noRecentGamesFoundText;

		private string _closeText;

		private MBBindingList<StringPairItemWithActionVM> _playerActions;

		private MBBindingList<MPLobbyRecentGameItemVM> _games;
	}
}
