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
	// Token: 0x0200006D RID: 109
	public class MPLobbyRankLeaderboardVM : ViewModel
	{
		// Token: 0x06000A00 RID: 2560 RVA: 0x000245FC File Offset: 0x000227FC
		public MPLobbyRankLeaderboardVM(LobbyState lobbyState)
		{
			this._lobbyState = lobbyState;
			this.LeaderboardPlayers = new MBBindingList<MPLobbyLeaderboardPlayerItemVM>();
			this.PlayerActions = new MBBindingList<StringPairItemWithActionVM>();
			this.RefreshValues();
		}

		// Token: 0x06000A01 RID: 2561 RVA: 0x00024638 File Offset: 0x00022838
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.TitleText = new TextObject("{=vGF5S2hE}Leaderboard", null).ToString();
			this.CloseText = new TextObject("{=yQtzabbe}Close", null).ToString();
			this.NoDataAvailableText = this._noDataAvailableTextObject.ToString();
		}

		// Token: 0x06000A02 RID: 2562 RVA: 0x00024688 File Offset: 0x00022888
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

		// Token: 0x06000A03 RID: 2563 RVA: 0x000246C9 File Offset: 0x000228C9
		public void ExecuteClosePopup()
		{
			this.IsEnabled = false;
		}

		// Token: 0x06000A04 RID: 2564 RVA: 0x000246D4 File Offset: 0x000228D4
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

		// Token: 0x06000A05 RID: 2565 RVA: 0x00024828 File Offset: 0x00022A28
		private void ExecuteRequestFriendship(object playerObj)
		{
			PlayerId providedID = (playerObj as MPLobbyLeaderboardPlayerItemVM).ProvidedID;
			bool flag = BannerlordConfig.EnableGenericNames && !NetworkMain.GameClient.IsKnownPlayer(providedID);
			NetworkMain.GameClient.AddFriend(providedID, flag);
		}

		// Token: 0x06000A06 RID: 2566 RVA: 0x00024866 File Offset: 0x00022A66
		private void ExecuteTerminateFriendship(object playerObj)
		{
			NetworkMain.GameClient.RemoveFriend((playerObj as MPLobbyLeaderboardPlayerItemVM).ProvidedID);
		}

		// Token: 0x06000A07 RID: 2567 RVA: 0x00024880 File Offset: 0x00022A80
		private void ExecuteReport(object playerObj)
		{
			MultiplayerReportPlayerManager.RequestReportPlayer(Guid.Empty.ToString(), (playerObj as MPLobbyLeaderboardPlayerItemVM).ProvidedID, (playerObj as MPLobbyLeaderboardPlayerItemVM).Name, false);
		}

		// Token: 0x06000A08 RID: 2568 RVA: 0x000248BC File Offset: 0x00022ABC
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

		// Token: 0x06000A09 RID: 2569 RVA: 0x000248D4 File Offset: 0x00022AD4
		public void SetCancelInputKey(HotKey hotKey)
		{
			this.CancelInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
		}

		// Token: 0x17000314 RID: 788
		// (get) Token: 0x06000A0A RID: 2570 RVA: 0x000248E3 File Offset: 0x00022AE3
		// (set) Token: 0x06000A0B RID: 2571 RVA: 0x000248EB File Offset: 0x00022AEB
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

		// Token: 0x17000315 RID: 789
		// (get) Token: 0x06000A0C RID: 2572 RVA: 0x00024908 File Offset: 0x00022B08
		// (set) Token: 0x06000A0D RID: 2573 RVA: 0x00024910 File Offset: 0x00022B10
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

		// Token: 0x17000316 RID: 790
		// (get) Token: 0x06000A0E RID: 2574 RVA: 0x0002492E File Offset: 0x00022B2E
		// (set) Token: 0x06000A0F RID: 2575 RVA: 0x00024936 File Offset: 0x00022B36
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

		// Token: 0x17000317 RID: 791
		// (get) Token: 0x06000A10 RID: 2576 RVA: 0x00024954 File Offset: 0x00022B54
		// (set) Token: 0x06000A11 RID: 2577 RVA: 0x0002495C File Offset: 0x00022B5C
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

		// Token: 0x17000318 RID: 792
		// (get) Token: 0x06000A12 RID: 2578 RVA: 0x0002497A File Offset: 0x00022B7A
		// (set) Token: 0x06000A13 RID: 2579 RVA: 0x00024982 File Offset: 0x00022B82
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

		// Token: 0x17000319 RID: 793
		// (get) Token: 0x06000A14 RID: 2580 RVA: 0x000249A0 File Offset: 0x00022BA0
		// (set) Token: 0x06000A15 RID: 2581 RVA: 0x000249A8 File Offset: 0x00022BA8
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

		// Token: 0x1700031A RID: 794
		// (get) Token: 0x06000A16 RID: 2582 RVA: 0x000249CB File Offset: 0x00022BCB
		// (set) Token: 0x06000A17 RID: 2583 RVA: 0x000249D3 File Offset: 0x00022BD3
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

		// Token: 0x1700031B RID: 795
		// (get) Token: 0x06000A18 RID: 2584 RVA: 0x000249F6 File Offset: 0x00022BF6
		// (set) Token: 0x06000A19 RID: 2585 RVA: 0x000249FE File Offset: 0x00022BFE
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

		// Token: 0x1700031C RID: 796
		// (get) Token: 0x06000A1A RID: 2586 RVA: 0x00024A21 File Offset: 0x00022C21
		// (set) Token: 0x06000A1B RID: 2587 RVA: 0x00024A29 File Offset: 0x00022C29
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

		// Token: 0x1700031D RID: 797
		// (get) Token: 0x06000A1C RID: 2588 RVA: 0x00024A47 File Offset: 0x00022C47
		// (set) Token: 0x06000A1D RID: 2589 RVA: 0x00024A4F File Offset: 0x00022C4F
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

		// Token: 0x040004DC RID: 1244
		private readonly LobbyState _lobbyState;

		// Token: 0x040004DD RID: 1245
		private readonly TextObject _noDataAvailableTextObject = new TextObject("{=*}There are currently no players in the leaderboard.", null);

		// Token: 0x040004DE RID: 1246
		private InputKeyItemVM _cancelInputKey;

		// Token: 0x040004DF RID: 1247
		private bool _isEnabled;

		// Token: 0x040004E0 RID: 1248
		private bool _isDataLoading;

		// Token: 0x040004E1 RID: 1249
		private bool _hasData;

		// Token: 0x040004E2 RID: 1250
		private bool _isPlayerActionsActive;

		// Token: 0x040004E3 RID: 1251
		private string _titleText;

		// Token: 0x040004E4 RID: 1252
		private string _closeText;

		// Token: 0x040004E5 RID: 1253
		private string _noDataAvailableText;

		// Token: 0x040004E6 RID: 1254
		private MBBindingList<MPLobbyLeaderboardPlayerItemVM> _leaderboardPlayers;

		// Token: 0x040004E7 RID: 1255
		private MBBindingList<StringPairItemWithActionVM> _playerActions;
	}
}
