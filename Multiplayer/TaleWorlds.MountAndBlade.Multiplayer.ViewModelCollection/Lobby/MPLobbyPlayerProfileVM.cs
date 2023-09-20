using System;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection.Lobby.Friends;
using TaleWorlds.PlatformService;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection.Lobby
{
	public class MPLobbyPlayerProfileVM : ViewModel
	{
		public MPLobbyPlayerProfileVM(LobbyState lobbyState)
		{
			this._lobbyState = lobbyState;
			this.Player = new MPLobbyPlayerBaseVM(PlayerId.Empty, "", null, null);
			MPLobbyPlayerBaseVM player = this.Player;
			player.OnRankInfoChanged = (Action<string>)Delegate.Combine(player.OnRankInfoChanged, new Action<string>(this.OnPlayerRankInfoChanged));
			this.RefreshValues();
		}

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

		public override void OnFinalize()
		{
			base.OnFinalize();
			MPLobbyPlayerBaseVM player = this.Player;
			player.OnRankInfoChanged = (Action<string>)Delegate.Remove(player.OnRankInfoChanged, new Action<string>(this.OnPlayerRankInfoChanged));
		}

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
					instance.CheckPrivilege(1, true, delegate(bool result)
					{
						if (!result)
						{
							PlatformServices.Instance.ShowRestrictedInformation();
						}
					});
				}
				PlatformServices.Instance.CheckPermissionWithUser(4, this._activePlayerID, delegate(bool hasPermission)
				{
					this.Player.IsBannerlordIDSupported = hasPermission;
				});
				await this._lobbyState.UpdateHasUserGeneratedContentPrivilege(true);
				this.Player.UpdateWith(this._activePlayerData);
				if (NetworkMain.GameClient.SupportedFeatures.SupportsFeatures(8))
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

		public void OpenWith(PlayerId playerID)
		{
			PlatformServices.Instance.CheckPermissionWithUser(4, playerID, delegate(bool hasBannerlordIDPrivilege)
			{
				this.Player.IsBannerlordIDSupported = hasBannerlordIDPrivilege;
				this.SetPlayerID(playerID);
			});
		}

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

		private void OnPlayerRankInfoChanged(string gameType)
		{
			this.Player.FilterStatsForGameMode(gameType);
		}

		public void ExecuteClosePopup()
		{
			this.IsEnabled = false;
			this.IsDataLoading = false;
		}

		public void OnClanInfoChanged()
		{
			if (this.Player.ProvidedID == NetworkMain.GameClient.PlayerID)
			{
				this.Player.UpdateClanInfo();
			}
		}

		private void OnStatsReceived()
		{
			this._isStatsReceived = true;
			this.CheckAndUpdateStatsAndRatingData();
		}

		private void OnRatingReceived()
		{
			this._isRatingReceived = true;
			this.CheckAndUpdateStatsAndRatingData();
		}

		public void OnPlayerNameUpdated(string playerName)
		{
			MPLobbyPlayerBaseVM player = this.Player;
			if (player == null)
			{
				return;
			}
			player.UpdateNameAndAvatar(true);
		}

		private void CheckAndUpdateStatsAndRatingData()
		{
			if (this._isRatingReceived && this._isStatsReceived)
			{
				this.Player.UpdateExperienceData();
				this.Player.RefreshSelectableGameTypes(false, new Action<string>(this.Player.UpdateDisplayedRankInfo), "");
				this.IsDataLoading = false;
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

		private readonly LobbyState _lobbyState;

		private PlayerId _activePlayerID;

		private PlayerData _activePlayerData;

		private bool _isStatsReceived;

		private bool _isRatingReceived;

		private bool _isEnabled;

		private bool _isDataLoading;

		private string _statsTitleText;

		private string _closeText;

		private MPLobbyPlayerBaseVM _player;
	}
}
