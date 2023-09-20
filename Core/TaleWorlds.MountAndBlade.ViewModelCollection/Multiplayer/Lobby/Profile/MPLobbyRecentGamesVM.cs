using System;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Generic;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Diamond;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.Profile
{
	// Token: 0x02000071 RID: 113
	public class MPLobbyRecentGamesVM : ViewModel
	{
		// Token: 0x06000A73 RID: 2675 RVA: 0x000258EA File Offset: 0x00023AEA
		public MPLobbyRecentGamesVM()
		{
			this._games = new MBBindingList<MPLobbyRecentGameItemVM>();
			this.PlayerActions = new MBBindingList<StringPairItemWithActionVM>();
			this.NoRecentGamesFoundText = new TextObject("{=TzYWE9tA}No Recent Games Found", null).ToString();
			this.RefreshValues();
		}

		// Token: 0x06000A74 RID: 2676 RVA: 0x00025924 File Offset: 0x00023B24
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

		// Token: 0x06000A75 RID: 2677 RVA: 0x00025990 File Offset: 0x00023B90
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

		// Token: 0x06000A76 RID: 2678 RVA: 0x00025A3C File Offset: 0x00023C3C
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

		// Token: 0x06000A77 RID: 2679 RVA: 0x00025B9C File Offset: 0x00023D9C
		private void ExecuteRequestFriendship(object playerObj)
		{
			MPLobbyRecentGamePlayerItemVM mplobbyRecentGamePlayerItemVM = playerObj as MPLobbyRecentGamePlayerItemVM;
			bool flag = BannerlordConfig.EnableGenericNames && !NetworkMain.GameClient.IsKnownPlayer(mplobbyRecentGamePlayerItemVM.ProvidedID);
			NetworkMain.GameClient.AddFriend(mplobbyRecentGamePlayerItemVM.ProvidedID, flag);
		}

		// Token: 0x06000A78 RID: 2680 RVA: 0x00025BE0 File Offset: 0x00023DE0
		private void ExecuteTerminateFriendship(object memberObj)
		{
			MPLobbyRecentGamePlayerItemVM mplobbyRecentGamePlayerItemVM = memberObj as MPLobbyRecentGamePlayerItemVM;
			NetworkMain.GameClient.RemoveFriend(mplobbyRecentGamePlayerItemVM.ProvidedID);
		}

		// Token: 0x06000A79 RID: 2681 RVA: 0x00025C04 File Offset: 0x00023E04
		private void ExecuteReport(object playerObj)
		{
			MPLobbyRecentGamePlayerItemVM mplobbyRecentGamePlayerItemVM = playerObj as MPLobbyRecentGamePlayerItemVM;
			MultiplayerReportPlayerManager.RequestReportPlayer(this._currentMatchOfTheActivePlayer.MatchId, mplobbyRecentGamePlayerItemVM.ProvidedID, mplobbyRecentGamePlayerItemVM.Name, false);
		}

		// Token: 0x06000A7A RID: 2682 RVA: 0x00025C35 File Offset: 0x00023E35
		public void ExecuteOpenPopup()
		{
			this.IsEnabled = true;
		}

		// Token: 0x06000A7B RID: 2683 RVA: 0x00025C3E File Offset: 0x00023E3E
		public void ExecuteClosePopup()
		{
			this.IsEnabled = false;
		}

		// Token: 0x06000A7C RID: 2684 RVA: 0x00025C48 File Offset: 0x00023E48
		public void OnFriendListUpdated(bool forceUpdate = false)
		{
			foreach (MPLobbyRecentGameItemVM mplobbyRecentGameItemVM in this.Games)
			{
				mplobbyRecentGameItemVM.OnFriendListUpdated(forceUpdate);
			}
		}

		// Token: 0x17000341 RID: 833
		// (get) Token: 0x06000A7D RID: 2685 RVA: 0x00025C94 File Offset: 0x00023E94
		// (set) Token: 0x06000A7E RID: 2686 RVA: 0x00025C9C File Offset: 0x00023E9C
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

		// Token: 0x17000342 RID: 834
		// (get) Token: 0x06000A7F RID: 2687 RVA: 0x00025CBA File Offset: 0x00023EBA
		// (set) Token: 0x06000A80 RID: 2688 RVA: 0x00025CC2 File Offset: 0x00023EC2
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

		// Token: 0x17000343 RID: 835
		// (get) Token: 0x06000A81 RID: 2689 RVA: 0x00025CE0 File Offset: 0x00023EE0
		// (set) Token: 0x06000A82 RID: 2690 RVA: 0x00025CE8 File Offset: 0x00023EE8
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

		// Token: 0x17000344 RID: 836
		// (get) Token: 0x06000A83 RID: 2691 RVA: 0x00025D06 File Offset: 0x00023F06
		// (set) Token: 0x06000A84 RID: 2692 RVA: 0x00025D0E File Offset: 0x00023F0E
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

		// Token: 0x17000345 RID: 837
		// (get) Token: 0x06000A85 RID: 2693 RVA: 0x00025D31 File Offset: 0x00023F31
		// (set) Token: 0x06000A86 RID: 2694 RVA: 0x00025D39 File Offset: 0x00023F39
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

		// Token: 0x17000346 RID: 838
		// (get) Token: 0x06000A87 RID: 2695 RVA: 0x00025D5C File Offset: 0x00023F5C
		// (set) Token: 0x06000A88 RID: 2696 RVA: 0x00025D64 File Offset: 0x00023F64
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

		// Token: 0x17000347 RID: 839
		// (get) Token: 0x06000A89 RID: 2697 RVA: 0x00025D87 File Offset: 0x00023F87
		// (set) Token: 0x06000A8A RID: 2698 RVA: 0x00025D8F File Offset: 0x00023F8F
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

		// Token: 0x17000348 RID: 840
		// (get) Token: 0x06000A8B RID: 2699 RVA: 0x00025DAD File Offset: 0x00023FAD
		// (set) Token: 0x06000A8C RID: 2700 RVA: 0x00025DB5 File Offset: 0x00023FB5
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

		// Token: 0x04000514 RID: 1300
		private MatchInfo _currentMatchOfTheActivePlayer;

		// Token: 0x04000515 RID: 1301
		private bool _isEnabled;

		// Token: 0x04000516 RID: 1302
		private bool _gotItems;

		// Token: 0x04000517 RID: 1303
		private bool _isPlayerActionsActive;

		// Token: 0x04000518 RID: 1304
		private string _recentGamesText;

		// Token: 0x04000519 RID: 1305
		private string _noRecentGamesFoundText;

		// Token: 0x0400051A RID: 1306
		private string _closeText;

		// Token: 0x0400051B RID: 1307
		private MBBindingList<StringPairItemWithActionVM> _playerActions;

		// Token: 0x0400051C RID: 1308
		private MBBindingList<MPLobbyRecentGameItemVM> _games;
	}
}
