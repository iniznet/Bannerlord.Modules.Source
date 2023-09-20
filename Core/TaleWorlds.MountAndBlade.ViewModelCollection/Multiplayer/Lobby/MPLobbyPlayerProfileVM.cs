using System;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.Friends;
using TaleWorlds.PlatformService;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby
{
	// Token: 0x0200005F RID: 95
	public class MPLobbyPlayerProfileVM : ViewModel
	{
		// Token: 0x06000805 RID: 2053 RVA: 0x0001EB04 File Offset: 0x0001CD04
		public MPLobbyPlayerProfileVM(LobbyState lobbyState)
		{
			this._lobbyState = lobbyState;
			this.Player = new MPLobbyPlayerBaseVM(PlayerId.Empty, "", null, null);
			MPLobbyPlayerBaseVM player = this.Player;
			player.OnRankInfoChanged = (Action<string>)Delegate.Combine(player.OnRankInfoChanged, new Action<string>(this.OnPlayerRankInfoChanged));
			this.RefreshValues();
		}

		// Token: 0x06000806 RID: 2054 RVA: 0x0001EB64 File Offset: 0x0001CD64
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.CloseText = GameTexts.FindText("str_close", null).ToString();
			this.StatsTitleText = new TextObject("{=GmU1to3Y}Statistics", null).ToString();
			MPLobbyPlayerBaseVM player = this.Player;
			if (player == null)
			{
				return;
			}
			player.RefreshValues();
		}

		// Token: 0x06000807 RID: 2055 RVA: 0x0001EBB3 File Offset: 0x0001CDB3
		public override void OnFinalize()
		{
			base.OnFinalize();
			MPLobbyPlayerBaseVM player = this.Player;
			player.OnRankInfoChanged = (Action<string>)Delegate.Remove(player.OnRankInfoChanged, new Action<string>(this.OnPlayerRankInfoChanged));
		}

		// Token: 0x06000808 RID: 2056 RVA: 0x0001EBE4 File Offset: 0x0001CDE4
		public async void SetPlayerID(PlayerId playerID)
		{
			this.IsEnabled = true;
			this.IsDataLoading = true;
			this._activePlayerID = playerID;
			PlayerData playerData = await NetworkMain.GameClient.GetAnotherPlayerData(playerID);
			this._activePlayerData = playerData;
			if (this._activePlayerData != null)
			{
				IPlatformServices instance = PlatformServices.Instance;
				if (instance != null)
				{
					instance.CheckPrivilege(Privilege.Chat, true, delegate(bool result)
					{
						if (!result)
						{
							PlatformServices.Instance.ShowRestrictedInformation();
						}
					});
				}
				PlatformServices.Instance.CheckPermissionWithUser(Permission.ViewUserGeneratedContent, this._activePlayerID, delegate(bool hasPermission)
				{
					this.Player.IsBannerlordIDSupported = hasPermission;
				});
				await this._lobbyState.UpdateHasUserGeneratedContentPrivilege(true);
				this.Player.UpdateWith(this._activePlayerData);
				if (NetworkMain.GameClient.SupportedFeatures.SupportsFeatures(Features.Clan))
				{
					this.Player.UpdateClanInfo();
				}
				this.Player.RefreshCharacterVisual();
				this.Player.UpdateStats(new Action(this.OnStatsReceived));
				this.Player.UpdateRating(new Action(this.OnRatingReceived));
			}
			else
			{
				InformationManager.ShowInquiry(new InquiryData(new TextObject("{=bhQiSzOU}Profile is not available", null).ToString(), new TextObject("{=goQ0MZhr}This player does not have an active Bannerlord player profile.", null).ToString(), true, false, new TextObject("{=yS7PvrTD}OK", null).ToString(), null, new Action(this.ExecuteClosePopup), null, "", 0f, null, null, null), false, false);
			}
		}

		// Token: 0x06000809 RID: 2057 RVA: 0x0001EC28 File Offset: 0x0001CE28
		public void OpenWith(PlayerId playerID)
		{
			PlatformServices.Instance.CheckPermissionWithUser(Permission.ViewUserGeneratedContent, playerID, delegate(bool hasBannerlordIDPrivilege)
			{
				this.Player.IsBannerlordIDSupported = hasBannerlordIDPrivilege;
				this.SetPlayerID(playerID);
			});
		}

		// Token: 0x0600080A RID: 2058 RVA: 0x0001EC68 File Offset: 0x0001CE68
		public void UpdatePlayerData(PlayerData playerData, bool updateStatistics = false, bool updateRating = false)
		{
			this.IsDataLoading = true;
			this._activePlayerID = playerData.PlayerId;
			MPLobbyPlayerBaseVM player = this.Player;
			if (player != null)
			{
				player.UpdateWith(playerData);
			}
			if (updateStatistics)
			{
				this.Player.UpdateStats(new Action(this.OnStatsReceived));
			}
			if (updateRating)
			{
				this.Player.UpdateRating(new Action(this.OnRatingReceived));
			}
			this.Player.UpdateExperienceData();
			this.Player.RefreshSelectableGameTypes(false, new Action<string>(this.Player.UpdateDisplayedRankInfo), "");
			this.IsDataLoading = false;
		}

		// Token: 0x0600080B RID: 2059 RVA: 0x0001ED02 File Offset: 0x0001CF02
		private void OnPlayerRankInfoChanged(string gameType)
		{
			this.Player.FilterStatsForGameMode(gameType);
		}

		// Token: 0x0600080C RID: 2060 RVA: 0x0001ED10 File Offset: 0x0001CF10
		public void ExecuteClosePopup()
		{
			this.IsEnabled = false;
			this.IsDataLoading = false;
		}

		// Token: 0x0600080D RID: 2061 RVA: 0x0001ED20 File Offset: 0x0001CF20
		public void OnClanInfoChanged()
		{
			if (this.Player.ProvidedID == NetworkMain.GameClient.PlayerID)
			{
				this.Player.UpdateClanInfo();
			}
		}

		// Token: 0x0600080E RID: 2062 RVA: 0x0001ED49 File Offset: 0x0001CF49
		private void OnStatsReceived()
		{
			this._isStatsReceived = true;
			this.CheckAndUpdateStatsAndRatingData();
		}

		// Token: 0x0600080F RID: 2063 RVA: 0x0001ED58 File Offset: 0x0001CF58
		private void OnRatingReceived()
		{
			this._isRatingReceived = true;
			this.CheckAndUpdateStatsAndRatingData();
		}

		// Token: 0x06000810 RID: 2064 RVA: 0x0001ED67 File Offset: 0x0001CF67
		public void OnPlayerNameUpdated(string playerName)
		{
			MPLobbyPlayerBaseVM player = this.Player;
			if (player == null)
			{
				return;
			}
			player.UpdateNameAndAvatar(true);
		}

		// Token: 0x06000811 RID: 2065 RVA: 0x0001ED7C File Offset: 0x0001CF7C
		private void CheckAndUpdateStatsAndRatingData()
		{
			if (this._isRatingReceived && this._isStatsReceived)
			{
				this.Player.UpdateExperienceData();
				this.Player.RefreshSelectableGameTypes(false, new Action<string>(this.Player.UpdateDisplayedRankInfo), "");
				this.IsDataLoading = false;
			}
		}

		// Token: 0x1700027A RID: 634
		// (get) Token: 0x06000812 RID: 2066 RVA: 0x0001EDCD File Offset: 0x0001CFCD
		// (set) Token: 0x06000813 RID: 2067 RVA: 0x0001EDD5 File Offset: 0x0001CFD5
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

		// Token: 0x1700027B RID: 635
		// (get) Token: 0x06000814 RID: 2068 RVA: 0x0001EDF3 File Offset: 0x0001CFF3
		// (set) Token: 0x06000815 RID: 2069 RVA: 0x0001EDFB File Offset: 0x0001CFFB
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

		// Token: 0x1700027C RID: 636
		// (get) Token: 0x06000816 RID: 2070 RVA: 0x0001EE19 File Offset: 0x0001D019
		// (set) Token: 0x06000817 RID: 2071 RVA: 0x0001EE21 File Offset: 0x0001D021
		[DataSourceProperty]
		public string StatsTitleText
		{
			get
			{
				return this._statsTitleText;
			}
			set
			{
				if (value != this._statsTitleText)
				{
					this._statsTitleText = value;
					base.OnPropertyChangedWithValue<string>(value, "StatsTitleText");
				}
			}
		}

		// Token: 0x1700027D RID: 637
		// (get) Token: 0x06000818 RID: 2072 RVA: 0x0001EE44 File Offset: 0x0001D044
		// (set) Token: 0x06000819 RID: 2073 RVA: 0x0001EE4C File Offset: 0x0001D04C
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

		// Token: 0x1700027E RID: 638
		// (get) Token: 0x0600081A RID: 2074 RVA: 0x0001EE6F File Offset: 0x0001D06F
		// (set) Token: 0x0600081B RID: 2075 RVA: 0x0001EE77 File Offset: 0x0001D077
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

		// Token: 0x0400040D RID: 1037
		private readonly LobbyState _lobbyState;

		// Token: 0x0400040E RID: 1038
		private PlayerId _activePlayerID;

		// Token: 0x0400040F RID: 1039
		private PlayerData _activePlayerData;

		// Token: 0x04000410 RID: 1040
		private bool _isStatsReceived;

		// Token: 0x04000411 RID: 1041
		private bool _isRatingReceived;

		// Token: 0x04000412 RID: 1042
		private bool _isEnabled;

		// Token: 0x04000413 RID: 1043
		private bool _isDataLoading;

		// Token: 0x04000414 RID: 1044
		private string _statsTitleText;

		// Token: 0x04000415 RID: 1045
		private string _closeText;

		// Token: 0x04000416 RID: 1046
		private MPLobbyPlayerBaseVM _player;
	}
}
