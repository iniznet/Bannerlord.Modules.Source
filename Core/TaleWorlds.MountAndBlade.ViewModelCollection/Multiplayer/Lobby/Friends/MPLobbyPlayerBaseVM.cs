using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Avatar.PlayerServices;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.MountAndBlade.Diamond.MultiplayerBadges;
using TaleWorlds.MountAndBlade.Diamond.Ranked;
using TaleWorlds.ObjectSystem;
using TaleWorlds.PlatformService;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.Friends
{
	// Token: 0x02000086 RID: 134
	public class MPLobbyPlayerBaseVM : ViewModel
	{
		// Token: 0x170003AE RID: 942
		// (get) Token: 0x06000BEF RID: 3055 RVA: 0x0002A54A File Offset: 0x0002874A
		// (set) Token: 0x06000BF0 RID: 3056 RVA: 0x0002A552 File Offset: 0x00028752
		public MPLobbyPlayerBaseVM.OnlineStatus CurrentOnlineStatus { get; private set; }

		// Token: 0x170003AF RID: 943
		// (get) Token: 0x06000BF1 RID: 3057 RVA: 0x0002A55B File Offset: 0x0002875B
		// (set) Token: 0x06000BF2 RID: 3058 RVA: 0x0002A563 File Offset: 0x00028763
		public PlayerId ProvidedID
		{
			get
			{
				return this._providedID;
			}
			protected set
			{
				if (this._providedID != value)
				{
					this._providedID = value;
					LobbyClient gameClient = NetworkMain.GameClient;
					this.UpdateAvatar(gameClient != null && gameClient.IsKnownPlayer(this.ProvidedID));
				}
			}
		}

		// Token: 0x170003B0 RID: 944
		// (get) Token: 0x06000BF3 RID: 3059 RVA: 0x0002A597 File Offset: 0x00028797
		// (set) Token: 0x06000BF4 RID: 3060 RVA: 0x0002A59F File Offset: 0x0002879F
		public PlayerData PlayerData { get; private set; }

		// Token: 0x170003B1 RID: 945
		// (get) Token: 0x06000BF5 RID: 3061 RVA: 0x0002A5A8 File Offset: 0x000287A8
		// (set) Token: 0x06000BF6 RID: 3062 RVA: 0x0002A5B0 File Offset: 0x000287B0
		public AnotherPlayerState State { get; protected set; }

		// Token: 0x170003B2 RID: 946
		// (get) Token: 0x06000BF7 RID: 3063 RVA: 0x0002A5B9 File Offset: 0x000287B9
		// (set) Token: 0x06000BF8 RID: 3064 RVA: 0x0002A5C1 File Offset: 0x000287C1
		public float TimeSinceLastStateUpdate { get; protected set; }

		// Token: 0x170003B3 RID: 947
		// (get) Token: 0x06000BF9 RID: 3065 RVA: 0x0002A5CA File Offset: 0x000287CA
		// (set) Token: 0x06000BFA RID: 3066 RVA: 0x0002A5D2 File Offset: 0x000287D2
		public PlayerStatsBase[] PlayerStats { get; private set; }

		// Token: 0x170003B4 RID: 948
		// (get) Token: 0x06000BFB RID: 3067 RVA: 0x0002A5DB File Offset: 0x000287DB
		// (set) Token: 0x06000BFC RID: 3068 RVA: 0x0002A5E3 File Offset: 0x000287E3
		public GameTypeRankInfo[] RankInfo { get; private set; }

		// Token: 0x170003B5 RID: 949
		// (get) Token: 0x06000BFD RID: 3069 RVA: 0x0002A5EC File Offset: 0x000287EC
		// (set) Token: 0x06000BFE RID: 3070 RVA: 0x0002A5F4 File Offset: 0x000287F4
		public string RankInfoGameTypeID { get; private set; }

		// Token: 0x06000BFF RID: 3071 RVA: 0x0002A600 File Offset: 0x00028800
		public MPLobbyPlayerBaseVM(PlayerId id, string forcedName = "", Action<PlayerId> onInviteToClan = null, Action<PlayerId> onFriendRequestAnswered = null)
		{
			this.ProvidedID = id;
			this._forcedName = forcedName;
			this.SetOnInvite(null);
			this._onInviteToClan = onInviteToClan;
			this._onFriendRequestAnswered = onFriendRequestAnswered;
			this.NameHint = new HintViewModel();
			this.ExperienceHint = new HintViewModel();
			this.RatingHint = new HintViewModel();
			LobbyClient gameClient = NetworkMain.GameClient;
			this.UpdateName(gameClient != null && gameClient.IsKnownPlayer(this.ProvidedID));
			this.CanBeInvited = true;
			this.CanInviteToParty = this._onInviteToParty != null;
			this.CanInviteToClan = this._onInviteToClan != null;
			PlatformServices.Instance.CheckPermissionWithUser(Permission.ViewUserGeneratedContent, id, delegate(bool hasBannerlordIDPrivilege)
			{
				this.CanCopyID = hasBannerlordIDPrivilege;
			});
			this.IsRankInfoLoading = true;
			this.GameTypes = new MBBindingList<MPLobbyGameTypeVM>();
			this.RefreshValues();
		}

		// Token: 0x06000C00 RID: 3072 RVA: 0x0002A6F8 File Offset: 0x000288F8
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.ClanInfoTitleText = new TextObject("{=j4F7tTzy}Clan", null).ToString();
			this.BadgeInfoTitleText = new TextObject("{=4PrfimcK}Badge", null).ToString();
			this.AvatarInfoTitleText = new TextObject("{=5tbWdY1j}Avatar", null).ToString();
			this.ChangeText = new TextObject("{=Ba50zU7Z}Change", null).ToString();
			this.LevelTitleText = new TextObject("{=OKUTPdaa}Level", null).ToString();
			this.InviteToPartyHint = new HintViewModel(new TextObject("{=aZnS9ECC}Invite", null), null);
			this.InviteToClanHint = new HintViewModel(new TextObject("{=fLddxLjh}Invite to Clan", null), null);
			this.RemoveFriendHint = new HintViewModel(new TextObject("{=d7ysGcsN}Remove Friend", null), null);
			this.AcceptFriendRequestHint = new HintViewModel(new TextObject("{=BSUteZmt}Accept Friend Request", null), null);
			this.DeclineFriendRequestHint = new HintViewModel(new TextObject("{=942B3LfA}Decline Friend Request", null), null);
			this.CancelFriendRequestHint = new HintViewModel(new TextObject("{=lGbrWyEe}Cancel Friend Request", null), null);
			this.LootHint = new HintViewModel(new TextObject("{=Th8q8wC2}Loot", null), null);
			this.ClanLeaderboardHint = new HintViewModel(new TextObject("{=JdEiK70R}Clan Leaderboard", null), null);
			this.ChangeBannerlordIDHint = new HintViewModel(new TextObject("{=ozREO8ev}Change Bannerlord ID", null), null);
			this.AddFriendWithBannerlordIDHint = new HintViewModel(new TextObject("{=tC9C8TLi}Add Friend", null), null);
			this.CopyBannerlordIDHint = new HintViewModel(new TextObject("{=Pwi1YCjH}Copy Bannerlord ID", null), null);
			MBBindingList<MPLobbyPlayerStatItemVM> displayedStats = this.DisplayedStats;
			if (displayedStats != null)
			{
				displayedStats.ApplyActionOnAllItems(delegate(MPLobbyPlayerStatItemVM s)
				{
					s.RefreshValues();
				});
			}
			MPLobbyBadgeItemVM shownBadge = this.ShownBadge;
			if (shownBadge == null)
			{
				return;
			}
			shownBadge.RefreshValues();
		}

		// Token: 0x06000C01 RID: 3073 RVA: 0x0002A8B8 File Offset: 0x00028AB8
		public void RefreshSelectableGameTypes(bool isRankedOnly, Action<string> onRefreshed, string initialGameTypeID = "")
		{
			this.GameTypes.Clear();
			this.GameTypes.Add(new MPLobbyGameTypeVM("Skirmish", false, onRefreshed));
			this.GameTypes.Add(new MPLobbyGameTypeVM("Captain", false, onRefreshed));
			if (!isRankedOnly)
			{
				this.GameTypes.Add(new MPLobbyGameTypeVM("Duel", true, onRefreshed));
				this.GameTypes.Add(new MPLobbyGameTypeVM("TeamDeathmatch", true, onRefreshed));
				this.GameTypes.Add(new MPLobbyGameTypeVM("Siege", true, new Action<string>(this.UpdateDisplayedRankInfo)));
			}
			MPLobbyGameTypeVM mplobbyGameTypeVM = this.GameTypes.FirstOrDefault((MPLobbyGameTypeVM gt) => gt.GameTypeID == initialGameTypeID);
			if (mplobbyGameTypeVM != null)
			{
				mplobbyGameTypeVM.IsSelected = true;
				return;
			}
			this.GameTypes[0].IsSelected = true;
		}

		// Token: 0x06000C02 RID: 3074 RVA: 0x0002A994 File Offset: 0x00028B94
		private void UpdateForcedAvatarIndex(bool isKnownPlayer)
		{
			if (this.ProvidedID != NetworkMain.GameClient.PlayerID)
			{
				Game game = Game.Current;
				object obj;
				if (game == null)
				{
					obj = null;
				}
				else
				{
					GameStateManager gameStateManager = game.GameStateManager;
					obj = ((gameStateManager != null) ? gameStateManager.ActiveState : null);
				}
				LobbyState lobbyState = obj as LobbyState;
				bool flag;
				if (lobbyState == null)
				{
					flag = false;
				}
				else
				{
					bool? hasUserGeneratedContentPrivilege = lobbyState.HasUserGeneratedContentPrivilege;
					bool flag2 = false;
					flag = (hasUserGeneratedContentPrivilege.GetValueOrDefault() == flag2) & (hasUserGeneratedContentPrivilege != null);
				}
				if (flag)
				{
					this._forcedAvatarIndex = AvatarServices.GetForcedAvatarIndexOfPlayer(this.ProvidedID);
					return;
				}
			}
			if (!BannerlordConfig.EnableGenericAvatars || this.ProvidedID == NetworkMain.GameClient.PlayerID || isKnownPlayer)
			{
				this._forcedAvatarIndex = -1;
				return;
			}
			this._forcedAvatarIndex = AvatarServices.GetForcedAvatarIndexOfPlayer(this.ProvidedID);
		}

		// Token: 0x06000C03 RID: 3075 RVA: 0x0002AA4C File Offset: 0x00028C4C
		protected async void UpdateName(bool isKnownPlayer)
		{
			string genericName = this._genericPlayerName.ToString();
			this.Name = genericName;
			if (this.ProvidedID != NetworkMain.GameClient.PlayerID)
			{
				Game game = Game.Current;
				object obj;
				if (game == null)
				{
					obj = null;
				}
				else
				{
					GameStateManager gameStateManager = game.GameStateManager;
					obj = ((gameStateManager != null) ? gameStateManager.ActiveState : null);
				}
				LobbyState lobbyState = obj as LobbyState;
				bool flag;
				if (lobbyState == null)
				{
					flag = false;
				}
				else
				{
					bool? hasUserGeneratedContentPrivilege = lobbyState.HasUserGeneratedContentPrivilege;
					bool flag2 = false;
					flag = (hasUserGeneratedContentPrivilege.GetValueOrDefault() == flag2) & (hasUserGeneratedContentPrivilege != null);
				}
				if (flag)
				{
					this.Name = genericName;
					goto IL_255;
				}
			}
			if (this._forcedName != string.Empty && !BannerlordConfig.EnableGenericNames)
			{
				this.Name = this._forcedName;
			}
			else if (this.ProvidedID == NetworkMain.GameClient.PlayerID)
			{
				this.Name = NetworkMain.GameClient.Name;
			}
			else if (!isKnownPlayer && BannerlordConfig.EnableGenericNames)
			{
				this.Name = genericName;
			}
			else if (this.PlayerData != null)
			{
				string lastPlayerName = this.PlayerData.LastPlayerName;
				this.Name = lastPlayerName;
			}
			else if (this.ProvidedID.IsValid)
			{
				IFriendListService[] friendListServices = PlatformServices.Instance.GetFriendListServices();
				string foundName = genericName;
				for (int i = friendListServices.Length - 1; i >= 0; i--)
				{
					string text = await friendListServices[i].GetUserName(this.ProvidedID);
					if (!string.IsNullOrEmpty(text) && text != "-" && text != genericName)
					{
						foundName = text;
						break;
					}
				}
				this.Name = foundName;
				friendListServices = null;
				foundName = null;
			}
			IL_255:
			this.NameHint.HintText = new TextObject("{=!}" + this.Name, null);
		}

		// Token: 0x06000C04 RID: 3076 RVA: 0x0002AA8D File Offset: 0x00028C8D
		protected void UpdateAvatar(bool isKnownPlayer)
		{
			this.UpdateForcedAvatarIndex(isKnownPlayer);
			this.Avatar = new ImageIdentifierVM(this.ProvidedID, this._forcedAvatarIndex);
		}

		// Token: 0x06000C05 RID: 3077 RVA: 0x0002AAB0 File Offset: 0x00028CB0
		public void UpdatePlayerState(AnotherPlayerData playerData)
		{
			if (playerData == null)
			{
				return;
			}
			if (playerData.PlayerState != AnotherPlayerState.NoAnswer)
			{
				this.State = playerData.PlayerState;
				this.StateText = GameTexts.FindText("str_multiplayer_lobby_state", this.State.ToString()).ToString();
			}
			this.TimeSinceLastStateUpdate = Game.Current.ApplicationTime;
		}

		// Token: 0x06000C06 RID: 3078 RVA: 0x0002AB10 File Offset: 0x00028D10
		public virtual void UpdateWith(PlayerData playerData)
		{
			if (playerData == null)
			{
				return;
			}
			this.PlayerData = playerData;
			this.ProvidedID = this.PlayerData.PlayerId;
			this.UpdateNameAndAvatar(true);
			this.UpdateExperienceData();
			if (NetworkMain.GameClient != null && NetworkMain.GameClient.SupportedFeatures.SupportsFeatures(Features.Clan))
			{
				this.IsClanInfoSupported = true;
			}
			else
			{
				this.IsClanInfoSupported = false;
			}
			this.Loot = playerData.Gold;
			this.Sigil = new MPLobbySigilItemVM();
			this.Sigil.RefreshWith(playerData.Sigil);
			this.ShownBadge = new MPLobbyBadgeItemVM(BadgeManager.GetById(playerData.ShownBadgeId), null, (Badge badge) => true, null);
			this.BannerlordID = string.Format("{0}#{1}", playerData.Username, playerData.UserId);
			this.SelectedBadgeID = playerData.ShownBadgeId;
			this.StateText = "";
			this._hasReceivedPlayerStats = false;
			this._isReceivingPlayerStats = false;
		}

		// Token: 0x06000C07 RID: 3079 RVA: 0x0002AC14 File Offset: 0x00028E14
		public void UpdateNameAndAvatar(bool forceUpdate = false)
		{
			bool flag = NetworkMain.GameClient.IsKnownPlayer(this.ProvidedID);
			if (this._isKnownPlayer != flag || forceUpdate)
			{
				this._isKnownPlayer = flag;
				this.UpdateAvatar(flag);
				this.UpdateName(flag);
			}
		}

		// Token: 0x06000C08 RID: 3080 RVA: 0x0002AC58 File Offset: 0x00028E58
		public void OnStatusChanged(MPLobbyPlayerBaseVM.OnlineStatus status, bool isInGameStatusActive)
		{
			this.CurrentOnlineStatus = status;
			this.StateText = "";
			this.TimeSinceLastStateUpdate = 0f;
			this.CanInviteToParty = this._onInviteToParty != null && (status == MPLobbyPlayerBaseVM.OnlineStatus.InGame || (status == MPLobbyPlayerBaseVM.OnlineStatus.Online && !isInGameStatusActive));
			this.ShowLevel = status == MPLobbyPlayerBaseVM.OnlineStatus.InGame || (status == MPLobbyPlayerBaseVM.OnlineStatus.Online && !isInGameStatusActive);
			this.CanInviteToClan = this._onInviteToClan != null && status == MPLobbyPlayerBaseVM.OnlineStatus.Online;
		}

		// Token: 0x06000C09 RID: 3081 RVA: 0x0002ACD2 File Offset: 0x00028ED2
		public void SetOnInvite(Action<PlayerId> onInvite)
		{
			this._onInviteToParty = onInvite;
			this.CanInviteToParty = onInvite != null;
			this.RefreshValues();
		}

		// Token: 0x06000C0A RID: 3082 RVA: 0x0002ACEC File Offset: 0x00028EEC
		public async void UpdateStats(Action onDone)
		{
			if (!this._hasReceivedPlayerStats && !this._isReceivingPlayerStats)
			{
				this._isReceivingPlayerStats = true;
				PlayerStatsBase[] array = await NetworkMain.GameClient.GetPlayerStats(this.ProvidedID);
				this.PlayerStats = array;
				this._isReceivingPlayerStats = false;
				this._hasReceivedPlayerStats = this.PlayerStats != null;
				if (this._hasReceivedPlayerStats)
				{
					Action onPlayerStatsReceived = this.OnPlayerStatsReceived;
					if (onPlayerStatsReceived != null)
					{
						onPlayerStatsReceived();
					}
					if (onDone != null)
					{
						onDone();
					}
				}
			}
		}

		// Token: 0x06000C0B RID: 3083 RVA: 0x0002AD30 File Offset: 0x00028F30
		public void UpdateExperienceData()
		{
			this.Level = this.PlayerData.Level;
			int num = PlayerDataExperience.ExperienceRequiredForLevel(this.PlayerData.Level + 1);
			float num2 = (float)this.PlayerData.ExperienceInCurrentLevel / (float)num;
			this.ExperienceRatio = (int)(num2 * 100f);
			string text = this.PlayerData.ExperienceInCurrentLevel + " / " + num;
			this.ExperienceHint.HintText = new TextObject("{=!}" + text, null);
			TextObject textObject = new TextObject("{=5Z0pvuNL}Level {LEVEL}", null);
			textObject.SetTextVariable("LEVEL", this.Level);
			this.LevelText = textObject.ToString();
			int experienceToNextLevel = this.PlayerData.ExperienceToNextLevel;
			TextObject textObject2 = new TextObject("{=NUSH5bJu}{EXPERIENCE} exp to next level", null);
			textObject2.SetTextVariable("EXPERIENCE", experienceToNextLevel);
			this.ExperienceText = textObject2.ToString();
		}

		// Token: 0x06000C0C RID: 3084 RVA: 0x0002AE1C File Offset: 0x0002901C
		public async void UpdateRating(Action onDone)
		{
			this.IsRankInfoLoading = true;
			GameTypeRankInfo[] array = await NetworkMain.GameClient.GetGameTypeRankInfo(this.ProvidedID);
			this.RankInfo = array;
			this.IsRankInfoLoading = false;
			if (onDone != null)
			{
				onDone();
			}
		}

		// Token: 0x06000C0D RID: 3085 RVA: 0x0002AE60 File Offset: 0x00029060
		public void UpdateDisplayedRankInfo(string gameType)
		{
			GameTypeRankInfo gameTypeRankInfo = null;
			if (gameType == "Skirmish")
			{
				GameTypeRankInfo[] rankInfo = this.RankInfo;
				GameTypeRankInfo gameTypeRankInfo2;
				if (rankInfo == null)
				{
					gameTypeRankInfo2 = null;
				}
				else
				{
					gameTypeRankInfo2 = rankInfo.FirstOrDefault((GameTypeRankInfo r) => r.GameType == "Skirmish");
				}
				gameTypeRankInfo = gameTypeRankInfo2;
				this.RankInfoGameTypeID = "Skirmish";
			}
			else if (gameType == "Captain")
			{
				GameTypeRankInfo[] rankInfo2 = this.RankInfo;
				GameTypeRankInfo gameTypeRankInfo3;
				if (rankInfo2 == null)
				{
					gameTypeRankInfo3 = null;
				}
				else
				{
					gameTypeRankInfo3 = rankInfo2.FirstOrDefault((GameTypeRankInfo r) => r.GameType == "Captain");
				}
				gameTypeRankInfo = gameTypeRankInfo3;
				this.RankInfoGameTypeID = "Captain";
			}
			if (gameTypeRankInfo != null)
			{
				RankBarInfo rankBarInfo = gameTypeRankInfo.RankBarInfo;
				this.Rating = rankBarInfo.Rating;
				this.RatingID = rankBarInfo.RankId;
				this.RatingText = MPLobbyVM.GetLocalizedRankName(this.RatingID);
				if (rankBarInfo.IsEvaluating)
				{
					TextObject textObject = new TextObject("{=Ise5gWw3}{PLAYED_GAMES} / {TOTAL_GAMES} Evaluation matches played", null);
					textObject.SetTextVariable("PLAYED_GAMES", rankBarInfo.EvaluationMatchesPlayed);
					textObject.SetTextVariable("TOTAL_GAMES", rankBarInfo.TotalEvaluationMatchesRequired);
					this.RankText = textObject.ToString();
					this.RatingRatio = MathF.Floor((float)rankBarInfo.EvaluationMatchesPlayed / (float)rankBarInfo.TotalEvaluationMatchesRequired * 100f);
				}
				else
				{
					TextObject textObject2 = new TextObject("{=BUOtUW1u}{RATING} Points", null);
					textObject2.SetTextVariable("RATING", rankBarInfo.Rating);
					this.RankText = textObject2.ToString();
					this.RatingRatio = (string.IsNullOrEmpty(rankBarInfo.NextRankId) ? 100 : MathF.Floor(rankBarInfo.ProgressPercentage));
				}
				GameTexts.SetVariable("NUMBER", this.RatingRatio.ToString("0.00"));
				this.RatingHint.HintText = GameTexts.FindText("str_NUMBER_percent", null);
			}
			else
			{
				this.Rating = 0;
				this.RatingRatio = 0;
				this.RatingID = "norank";
				this.RatingText = new TextObject("{=GXosklej}Casual", null).ToString();
				this.RankText = new TextObject("{=56FyokuX}Game mode is casual", null).ToString();
				this.RatingHint.HintText = TextObject.Empty;
			}
			Action<string> onRankInfoChanged = this.OnRankInfoChanged;
			if (onRankInfoChanged != null)
			{
				onRankInfoChanged(gameType);
			}
			this.IsRankInfoCasual = gameType != "Skirmish" && gameType != "Captain";
		}

		// Token: 0x06000C0E RID: 3086 RVA: 0x0002B0AC File Offset: 0x000292AC
		public async void UpdateClanInfo()
		{
			if (!(this.ProvidedID == PlayerId.Empty))
			{
				bool isSelfPlayer = this.ProvidedID == NetworkMain.GameClient.PlayerID;
				ClanInfo clanInfo;
				if (isSelfPlayer)
				{
					clanInfo = NetworkMain.GameClient.ClanInfo;
				}
				else
				{
					clanInfo = await NetworkMain.GameClient.GetPlayerClanInfo(this.ProvidedID);
				}
				if (clanInfo != null && (isSelfPlayer || (!isSelfPlayer && clanInfo.Players.Length != 0)))
				{
					this.ClanBanner = new ImageIdentifierVM(BannerCode.CreateFrom(clanInfo.Sigil), true);
					this.ClanName = clanInfo.Name;
					GameTexts.SetVariable("STR", clanInfo.Tag);
					this.ClanTag = new TextObject("{=uTXYEAOg}[{STR}]", null).ToString();
				}
				else
				{
					this.ClanBanner = new ImageIdentifierVM(BannerCode.CreateFrom(Banner.CreateOneColoredEmptyBanner(99)), false);
					this.ClanName = new TextObject("{=0DnHFlia}Not In a Clan", null).ToString();
					this.ClanTag = string.Empty;
				}
			}
		}

		// Token: 0x06000C0F RID: 3087 RVA: 0x0002B0E8 File Offset: 0x000292E8
		public void FilterStatsForGameMode(string gameModeCode)
		{
			if (this.PlayerStats == null)
			{
				return;
			}
			if (this.DisplayedStats == null)
			{
				this.DisplayedStats = new MBBindingList<MPLobbyPlayerStatItemVM>();
			}
			this.DisplayedStats.Clear();
			IEnumerable<PlayerStatsBase> enumerable = this.PlayerStats.Where((PlayerStatsBase s) => s.GameType == gameModeCode);
			foreach (PlayerStatsBase playerStatsBase in enumerable)
			{
				if (gameModeCode == "Skirmish" || gameModeCode == "Captain")
				{
					this.DisplayedStats.Add(new MPLobbyPlayerStatItemVM(playerStatsBase.GameType, new TextObject("{=WW2N3zJf}Wins", null), playerStatsBase.WinCount));
					this.DisplayedStats.Add(new MPLobbyPlayerStatItemVM(playerStatsBase.GameType, new TextObject("{=4nr9Km6t}Losses", null), playerStatsBase.LoseCount));
				}
				if (gameModeCode == "Skirmish")
				{
					this.DisplayedStats.Add(new MPLobbyPlayerStatItemVM(playerStatsBase.GameType, new TextObject("{=ab2cbidI}Total Score", null), (playerStatsBase as PlayerStatsSkirmish).Score));
					this.DisplayedStats.Add(new MPLobbyPlayerStatItemVM(playerStatsBase.GameType, new TextObject("{=fdR3xpBS}MVP Badges", null), (playerStatsBase as PlayerStatsSkirmish).MVPs));
					this.DisplayedStats.Add(new MPLobbyPlayerStatItemVM(playerStatsBase.GameType, new TextObject("{=2FaZ6E1k}Kill Death Ratio", null), playerStatsBase.AverageKillPerDeath));
				}
				else if (gameModeCode == "Captain")
				{
					this.DisplayedStats.Add(new MPLobbyPlayerStatItemVM(playerStatsBase.GameType, new TextObject("{=ab2cbidI}Total Score", null), (playerStatsBase as PlayerStatsCaptain).Score));
					this.DisplayedStats.Add(new MPLobbyPlayerStatItemVM(playerStatsBase.GameType, new TextObject("{=fdR3xpBS}MVP Badges", null), (playerStatsBase as PlayerStatsCaptain).MVPs));
					this.DisplayedStats.Add(new MPLobbyPlayerStatItemVM(playerStatsBase.GameType, new TextObject("{=9FSk2daF}Captains Killed", null), (playerStatsBase as PlayerStatsCaptain).CaptainsKilled));
				}
				else if (gameModeCode == "Siege")
				{
					this.DisplayedStats.Add(new MPLobbyPlayerStatItemVM(playerStatsBase.GameType, new TextObject("{=ab2cbidI}Total Score", null), (playerStatsBase as PlayerStatsSiege).Score));
					this.DisplayedStats.Add(new MPLobbyPlayerStatItemVM(playerStatsBase.GameType, new TextObject("{=XKWGPrYt}Siege Engines Destroyed", null), (playerStatsBase as PlayerStatsSiege).SiegeEnginesDestroyed));
					this.DisplayedStats.Add(new MPLobbyPlayerStatItemVM(playerStatsBase.GameType, new TextObject("{=7APa598U}Kills With a Siege Engine", null), (playerStatsBase as PlayerStatsSiege).SiegeEngineKills));
					this.DisplayedStats.Add(new MPLobbyPlayerStatItemVM(playerStatsBase.GameType, new TextObject("{=FaKWQccs}Gold Gained From Objectives", null), (playerStatsBase as PlayerStatsSiege).ObjectiveGoldGained));
					this.DisplayedStats.Add(new MPLobbyPlayerStatItemVM(playerStatsBase.GameType, new TextObject("{=2FaZ6E1k}Kill Death Ratio", null), playerStatsBase.AverageKillPerDeath));
				}
				else if (gameModeCode == "Duel")
				{
					this.DisplayedStats.Add(new MPLobbyPlayerStatItemVM(playerStatsBase.GameType, new TextObject("{=SS5WyUWR}Duels Won", null), (playerStatsBase as PlayerStatsDuel).DuelsWon));
					this.DisplayedStats.Add(new MPLobbyPlayerStatItemVM(playerStatsBase.GameType, new TextObject("{=Iu2eFSsh}Infantry Wins", null), (playerStatsBase as PlayerStatsDuel).InfantryWins));
					this.DisplayedStats.Add(new MPLobbyPlayerStatItemVM(playerStatsBase.GameType, new TextObject("{=wyKhcvbd}Ranged Wins", null), (playerStatsBase as PlayerStatsDuel).ArcherWins));
					this.DisplayedStats.Add(new MPLobbyPlayerStatItemVM(playerStatsBase.GameType, new TextObject("{=qipBkhys}Cavalry Wins", null), (playerStatsBase as PlayerStatsDuel).CavalryWins));
				}
				else if (gameModeCode == "TeamDeathmatch")
				{
					this.DisplayedStats.Add(new MPLobbyPlayerStatItemVM(playerStatsBase.GameType, new TextObject("{=ab2cbidI}Total Score", null), (playerStatsBase as PlayerStatsTeamDeathmatch).Score));
					this.DisplayedStats.Add(new MPLobbyPlayerStatItemVM(playerStatsBase.GameType, new TextObject("{=9ET13VOe}Average Score", null), (playerStatsBase as PlayerStatsTeamDeathmatch).AverageScore));
					this.DisplayedStats.Add(new MPLobbyPlayerStatItemVM(playerStatsBase.GameType, new TextObject("{=2FaZ6E1k}Kill Death Ratio", null), playerStatsBase.AverageKillPerDeath));
				}
				if (gameModeCode != "Duel")
				{
					this.DisplayedStats.Add(new MPLobbyPlayerStatItemVM(playerStatsBase.GameType, new TextObject("{=FKe05WtJ}Kills", null), playerStatsBase.KillCount));
					this.DisplayedStats.Add(new MPLobbyPlayerStatItemVM(playerStatsBase.GameType, new TextObject("{=8eZFlPVu}Deaths", null), playerStatsBase.DeathCount));
					this.DisplayedStats.Add(new MPLobbyPlayerStatItemVM(playerStatsBase.GameType, new TextObject("{=1imGhhZl}Assists", null), playerStatsBase.AssistCount));
				}
			}
			if (enumerable.IsEmpty<PlayerStatsBase>())
			{
				if (gameModeCode == "Skirmish" || gameModeCode == "Captain")
				{
					this.DisplayedStats.Add(new MPLobbyPlayerStatItemVM(gameModeCode, new TextObject("{=WW2N3zJf}Wins", null), "-"));
					this.DisplayedStats.Add(new MPLobbyPlayerStatItemVM(gameModeCode, new TextObject("{=4nr9Km6t}Losses", null), "-"));
				}
				if (gameModeCode == "Skirmish")
				{
					this.DisplayedStats.Add(new MPLobbyPlayerStatItemVM(gameModeCode, new TextObject("{=ab2cbidI}Total Score", null), "-"));
					this.DisplayedStats.Add(new MPLobbyPlayerStatItemVM(gameModeCode, new TextObject("{=fdR3xpBS}MVP Badges", null), "-"));
					this.DisplayedStats.Add(new MPLobbyPlayerStatItemVM(gameModeCode, new TextObject("{=2FaZ6E1k}Kill Death Ratio", null), "-"));
				}
				else if (gameModeCode == "Captain")
				{
					this.DisplayedStats.Add(new MPLobbyPlayerStatItemVM(gameModeCode, new TextObject("{=ab2cbidI}Total Score", null), "-"));
					this.DisplayedStats.Add(new MPLobbyPlayerStatItemVM(gameModeCode, new TextObject("{=fdR3xpBS}MVP Badges", null), "-"));
					this.DisplayedStats.Add(new MPLobbyPlayerStatItemVM(gameModeCode, new TextObject("{=9FSk2daF}Captains Killed", null), "-"));
				}
				else if (gameModeCode == "Siege")
				{
					this.DisplayedStats.Add(new MPLobbyPlayerStatItemVM(gameModeCode, new TextObject("{=ab2cbidI}Total Score", null), "-"));
					this.DisplayedStats.Add(new MPLobbyPlayerStatItemVM(gameModeCode, new TextObject("{=XKWGPrYt}Siege Engines Destroyed", null), "-"));
					this.DisplayedStats.Add(new MPLobbyPlayerStatItemVM(gameModeCode, new TextObject("{=7APa598U}Kills With a Siege Engine", null), "-"));
					this.DisplayedStats.Add(new MPLobbyPlayerStatItemVM(gameModeCode, new TextObject("{=FaKWQccs}Gold Gained From Objectives", null), "-"));
					this.DisplayedStats.Add(new MPLobbyPlayerStatItemVM(gameModeCode, new TextObject("{=2FaZ6E1k}Kill Death Ratio", null), "-"));
				}
				else if (gameModeCode == "Duel")
				{
					this.DisplayedStats.Add(new MPLobbyPlayerStatItemVM(gameModeCode, new TextObject("{=SS5WyUWR}Duels Won", null), "-"));
					this.DisplayedStats.Add(new MPLobbyPlayerStatItemVM(gameModeCode, new TextObject("{=Iu2eFSsh}Infantry Wins", null), "-"));
					this.DisplayedStats.Add(new MPLobbyPlayerStatItemVM(gameModeCode, new TextObject("{=wyKhcvbd}Ranged Wins", null), "-"));
					this.DisplayedStats.Add(new MPLobbyPlayerStatItemVM(gameModeCode, new TextObject("{=qipBkhys}Cavalry Wins", null), "-"));
				}
				else if (gameModeCode == "TeamDeathmatch")
				{
					this.DisplayedStats.Add(new MPLobbyPlayerStatItemVM(gameModeCode, new TextObject("{=ab2cbidI}Total Score", null), "-"));
					this.DisplayedStats.Add(new MPLobbyPlayerStatItemVM(gameModeCode, new TextObject("{=9ET13VOe}Average Score", null), "-"));
					this.DisplayedStats.Add(new MPLobbyPlayerStatItemVM(gameModeCode, new TextObject("{=2FaZ6E1k}Kill Death Ratio", null), "-"));
				}
				if (gameModeCode != "Duel")
				{
					this.DisplayedStats.Add(new MPLobbyPlayerStatItemVM(gameModeCode, new TextObject("{=FKe05WtJ}Kills", null), "-"));
					this.DisplayedStats.Add(new MPLobbyPlayerStatItemVM(gameModeCode, new TextObject("{=8eZFlPVu}Deaths", null), "-"));
					this.DisplayedStats.Add(new MPLobbyPlayerStatItemVM(gameModeCode, new TextObject("{=1imGhhZl}Assists", null), "-"));
				}
			}
		}

		// Token: 0x06000C10 RID: 3088 RVA: 0x0002BA24 File Offset: 0x00029C24
		public void RefreshCharacterVisual()
		{
			this.CharacterVisual = new CharacterViewModel();
			BasicCharacterObject @object = MBObjectManager.Instance.GetObject<BasicCharacterObject>("mp_character");
			@object.UpdatePlayerCharacterBodyProperties(this.PlayerData.BodyProperties, this.PlayerData.Race, this.PlayerData.IsFemale);
			this.CharacterVisual.FillFrom(@object, -1);
			this.CharacterVisual.BodyProperties = new BodyProperties(this.PlayerData.BodyProperties.DynamicProperties, @object.BodyPropertyRange.BodyPropertyMin.StaticProperties).ToString();
			this.CharacterVisual.IsFemale = this.PlayerData.IsFemale;
			this.CharacterVisual.Race = this.PlayerData.Race;
		}

		// Token: 0x06000C11 RID: 3089 RVA: 0x0002BAF0 File Offset: 0x00029CF0
		public void ExecuteSelectPlayer()
		{
			this.IsSelected = !this.IsSelected;
		}

		// Token: 0x06000C12 RID: 3090 RVA: 0x0002BB01 File Offset: 0x00029D01
		public void ExecuteInviteToParty()
		{
			Action<PlayerId> onInviteToParty = this._onInviteToParty;
			if (onInviteToParty == null)
			{
				return;
			}
			onInviteToParty(this.ProvidedID);
		}

		// Token: 0x06000C13 RID: 3091 RVA: 0x0002BB19 File Offset: 0x00029D19
		public void ExecuteInviteToClan()
		{
			Action<PlayerId> onInviteToClan = this._onInviteToClan;
			if (onInviteToClan == null)
			{
				return;
			}
			onInviteToClan(this.ProvidedID);
		}

		// Token: 0x06000C14 RID: 3092 RVA: 0x0002BB31 File Offset: 0x00029D31
		public void ExecuteKickFromParty()
		{
			if (NetworkMain.GameClient.IsInParty && NetworkMain.GameClient.IsPartyLeader)
			{
				NetworkMain.GameClient.KickPlayerFromParty(this.ProvidedID);
			}
		}

		// Token: 0x06000C15 RID: 3093 RVA: 0x0002BB5C File Offset: 0x00029D5C
		public void ExecuteAcceptFriendRequest()
		{
			bool flag = BannerlordConfig.EnableGenericNames && !NetworkMain.GameClient.IsKnownPlayer(this.ProvidedID);
			NetworkMain.GameClient.RespondToFriendRequest(this.ProvidedID, flag, true, false);
			Action<PlayerId> onFriendRequestAnswered = this._onFriendRequestAnswered;
			if (onFriendRequestAnswered != null)
			{
				onFriendRequestAnswered(this.ProvidedID);
			}
			if (this.HasNotification)
			{
				this.HasNotification = false;
			}
		}

		// Token: 0x06000C16 RID: 3094 RVA: 0x0002BBC0 File Offset: 0x00029DC0
		public void ExecuteDeclineFriendRequest()
		{
			bool flag = BannerlordConfig.EnableGenericNames && !NetworkMain.GameClient.IsKnownPlayer(this.ProvidedID);
			NetworkMain.GameClient.RespondToFriendRequest(this.ProvidedID, flag, false, false);
			Action<PlayerId> onFriendRequestAnswered = this._onFriendRequestAnswered;
			if (onFriendRequestAnswered != null)
			{
				onFriendRequestAnswered(this.ProvidedID);
			}
			if (this.HasNotification)
			{
				this.HasNotification = false;
			}
		}

		// Token: 0x06000C17 RID: 3095 RVA: 0x0002BC24 File Offset: 0x00029E24
		public void ExecuteCancelPendingFriendRequest()
		{
			NetworkMain.GameClient.RemoveFriend(this.ProvidedID);
		}

		// Token: 0x06000C18 RID: 3096 RVA: 0x0002BC36 File Offset: 0x00029E36
		public void ExecuteRemoveFriend()
		{
			NetworkMain.GameClient.RemoveFriend(this.ProvidedID);
		}

		// Token: 0x06000C19 RID: 3097 RVA: 0x0002BC48 File Offset: 0x00029E48
		public void ExecuteCopyBannerlordID()
		{
			Input.SetClipboardText(this.BannerlordID);
		}

		// Token: 0x06000C1A RID: 3098 RVA: 0x0002BC58 File Offset: 0x00029E58
		private void ExecuteAddFriend()
		{
			string[] array = this.BannerlordID.Split(new char[] { '#' });
			string text = array[0];
			int num;
			if (int.TryParse(array[1], out num))
			{
				bool flag = BannerlordConfig.EnableGenericNames && !NetworkMain.GameClient.IsKnownPlayer(this.ProvidedID);
				NetworkMain.GameClient.AddFriendByUsernameAndId(text, num, flag);
			}
		}

		// Token: 0x06000C1B RID: 3099 RVA: 0x0002BCB5 File Offset: 0x00029EB5
		public void ExecuteShowProfile()
		{
			Action<PlayerId> onPlayerProfileRequested = MPLobbyPlayerBaseVM.OnPlayerProfileRequested;
			if (onPlayerProfileRequested == null)
			{
				return;
			}
			onPlayerProfileRequested(this.ProvidedID);
		}

		// Token: 0x06000C1C RID: 3100 RVA: 0x0002BCCC File Offset: 0x00029ECC
		private void ExecuteActivateSigilChangeInformation()
		{
			this.IsSigilChangeInformationEnabled = true;
		}

		// Token: 0x06000C1D RID: 3101 RVA: 0x0002BCD5 File Offset: 0x00029ED5
		private void ExecuteDeactivateSigilChangeInformation()
		{
			this.IsSigilChangeInformationEnabled = false;
		}

		// Token: 0x06000C1E RID: 3102 RVA: 0x0002BCDE File Offset: 0x00029EDE
		private void ExecuteChangeSigil()
		{
			Action<PlayerId> onSigilChangeRequested = MPLobbyPlayerBaseVM.OnSigilChangeRequested;
			if (onSigilChangeRequested == null)
			{
				return;
			}
			onSigilChangeRequested(this.ProvidedID);
		}

		// Token: 0x06000C1F RID: 3103 RVA: 0x0002BCF5 File Offset: 0x00029EF5
		private void ExecuteChangeBannerlordID()
		{
			Action<PlayerId> onBannerlordIDChangeRequested = MPLobbyPlayerBaseVM.OnBannerlordIDChangeRequested;
			if (onBannerlordIDChangeRequested == null)
			{
				return;
			}
			onBannerlordIDChangeRequested(this.ProvidedID);
		}

		// Token: 0x06000C20 RID: 3104 RVA: 0x0002BD0C File Offset: 0x00029F0C
		private void ExecuteAddFriendWithBannerlordID()
		{
			Action<PlayerId> onAddFriendWithBannerlordIDRequested = MPLobbyPlayerBaseVM.OnAddFriendWithBannerlordIDRequested;
			if (onAddFriendWithBannerlordIDRequested == null)
			{
				return;
			}
			onAddFriendWithBannerlordIDRequested(this.ProvidedID);
		}

		// Token: 0x06000C21 RID: 3105 RVA: 0x0002BD23 File Offset: 0x00029F23
		private void ExecuteChangeBadge()
		{
			Action<PlayerId> onBadgeChangeRequested = MPLobbyPlayerBaseVM.OnBadgeChangeRequested;
			if (onBadgeChangeRequested == null)
			{
				return;
			}
			onBadgeChangeRequested(this.ProvidedID);
		}

		// Token: 0x06000C22 RID: 3106 RVA: 0x0002BD3C File Offset: 0x00029F3C
		private void ExecuteShowRankProgression()
		{
			MPLobbyGameTypeVM mplobbyGameTypeVM = this.GameTypes.FirstOrDefault((MPLobbyGameTypeVM gt) => gt.IsSelected);
			if (mplobbyGameTypeVM != null && !mplobbyGameTypeVM.IsCasual)
			{
				Action<MPLobbyPlayerBaseVM> onRankProgressionRequested = MPLobbyPlayerBaseVM.OnRankProgressionRequested;
				if (onRankProgressionRequested == null)
				{
					return;
				}
				onRankProgressionRequested(this);
			}
		}

		// Token: 0x06000C23 RID: 3107 RVA: 0x0002BD94 File Offset: 0x00029F94
		private void ExecuteShowRankLeaderboard()
		{
			MPLobbyGameTypeVM mplobbyGameTypeVM = this.GameTypes.FirstOrDefault((MPLobbyGameTypeVM gt) => gt.IsSelected);
			if (mplobbyGameTypeVM != null && !mplobbyGameTypeVM.IsCasual)
			{
				Action<string> onRankLeaderboardRequested = MPLobbyPlayerBaseVM.OnRankLeaderboardRequested;
				if (onRankLeaderboardRequested == null)
				{
					return;
				}
				onRankLeaderboardRequested(mplobbyGameTypeVM.GameTypeID);
			}
		}

		// Token: 0x06000C24 RID: 3108 RVA: 0x0002BDEC File Offset: 0x00029FEC
		private void ExecuteShowClanPage()
		{
			Action onClanPageRequested = MPLobbyPlayerBaseVM.OnClanPageRequested;
			if (onClanPageRequested == null)
			{
				return;
			}
			onClanPageRequested();
		}

		// Token: 0x06000C25 RID: 3109 RVA: 0x0002BDFD File Offset: 0x00029FFD
		private void ExecuteShowClanLeaderboard()
		{
			Action onClanLeaderboardRequested = MPLobbyPlayerBaseVM.OnClanLeaderboardRequested;
			if (onClanLeaderboardRequested == null)
			{
				return;
			}
			onClanLeaderboardRequested();
		}

		// Token: 0x170003B6 RID: 950
		// (get) Token: 0x06000C26 RID: 3110 RVA: 0x0002BE0E File Offset: 0x0002A00E
		// (set) Token: 0x06000C27 RID: 3111 RVA: 0x0002BE16 File Offset: 0x0002A016
		[DataSourceProperty]
		public bool CanCopyID
		{
			get
			{
				return this._canCopyID;
			}
			set
			{
				if (value != this._canCopyID)
				{
					this._canCopyID = value;
					base.OnPropertyChangedWithValue(value, "CanCopyID");
				}
			}
		}

		// Token: 0x170003B7 RID: 951
		// (get) Token: 0x06000C28 RID: 3112 RVA: 0x0002BE34 File Offset: 0x0002A034
		// (set) Token: 0x06000C29 RID: 3113 RVA: 0x0002BE3C File Offset: 0x0002A03C
		[DataSourceProperty]
		public bool ShowLevel
		{
			get
			{
				return this._showLevel;
			}
			set
			{
				if (value != this._showLevel)
				{
					this._showLevel = value;
					base.OnPropertyChangedWithValue(value, "ShowLevel");
				}
			}
		}

		// Token: 0x170003B8 RID: 952
		// (get) Token: 0x06000C2A RID: 3114 RVA: 0x0002BE5A File Offset: 0x0002A05A
		// (set) Token: 0x06000C2B RID: 3115 RVA: 0x0002BE62 File Offset: 0x0002A062
		[DataSourceProperty]
		public bool IsSelected
		{
			get
			{
				return this._isSelected;
			}
			set
			{
				if (value != this._isSelected)
				{
					this._isSelected = value;
					base.OnPropertyChangedWithValue(value, "IsSelected");
				}
			}
		}

		// Token: 0x170003B9 RID: 953
		// (get) Token: 0x06000C2C RID: 3116 RVA: 0x0002BE80 File Offset: 0x0002A080
		// (set) Token: 0x06000C2D RID: 3117 RVA: 0x0002BE88 File Offset: 0x0002A088
		[DataSourceProperty]
		public bool HasNotification
		{
			get
			{
				return this._hasNotification;
			}
			set
			{
				if (value != this._hasNotification)
				{
					this._hasNotification = value;
					base.OnPropertyChangedWithValue(value, "HasNotification");
				}
			}
		}

		// Token: 0x170003BA RID: 954
		// (get) Token: 0x06000C2E RID: 3118 RVA: 0x0002BEA6 File Offset: 0x0002A0A6
		// (set) Token: 0x06000C2F RID: 3119 RVA: 0x0002BEAE File Offset: 0x0002A0AE
		[DataSourceProperty]
		public bool IsFriendRequest
		{
			get
			{
				return this._isFriendRequest;
			}
			set
			{
				if (value != this._isFriendRequest)
				{
					this._isFriendRequest = value;
					base.OnPropertyChangedWithValue(value, "IsFriendRequest");
				}
			}
		}

		// Token: 0x170003BB RID: 955
		// (get) Token: 0x06000C30 RID: 3120 RVA: 0x0002BECC File Offset: 0x0002A0CC
		// (set) Token: 0x06000C31 RID: 3121 RVA: 0x0002BED4 File Offset: 0x0002A0D4
		[DataSourceProperty]
		public bool IsPendingRequest
		{
			get
			{
				return this._isPendingRequest;
			}
			set
			{
				if (value != this._isPendingRequest)
				{
					this._isPendingRequest = value;
					base.OnPropertyChangedWithValue(value, "IsPendingRequest");
				}
			}
		}

		// Token: 0x170003BC RID: 956
		// (get) Token: 0x06000C32 RID: 3122 RVA: 0x0002BEF2 File Offset: 0x0002A0F2
		// (set) Token: 0x06000C33 RID: 3123 RVA: 0x0002BEFA File Offset: 0x0002A0FA
		[DataSourceProperty]
		public bool CanRemove
		{
			get
			{
				return this._canRemove;
			}
			set
			{
				if (value != this._canRemove)
				{
					this._canRemove = value;
					base.OnPropertyChangedWithValue(value, "CanRemove");
				}
			}
		}

		// Token: 0x170003BD RID: 957
		// (get) Token: 0x06000C34 RID: 3124 RVA: 0x0002BF18 File Offset: 0x0002A118
		// (set) Token: 0x06000C35 RID: 3125 RVA: 0x0002BF20 File Offset: 0x0002A120
		[DataSourceProperty]
		public bool CanBeInvited
		{
			get
			{
				return this._canBeInvited;
			}
			set
			{
				if (value != this._canBeInvited)
				{
					this._canBeInvited = value;
					base.OnPropertyChangedWithValue(value, "CanBeInvited");
				}
			}
		}

		// Token: 0x170003BE RID: 958
		// (get) Token: 0x06000C36 RID: 3126 RVA: 0x0002BF3E File Offset: 0x0002A13E
		// (set) Token: 0x06000C37 RID: 3127 RVA: 0x0002BF46 File Offset: 0x0002A146
		[DataSourceProperty]
		public bool CanInviteToParty
		{
			get
			{
				return this._canInviteToParty;
			}
			set
			{
				if (value != this._canInviteToParty)
				{
					this._canInviteToParty = value;
					base.OnPropertyChangedWithValue(value, "CanInviteToParty");
				}
			}
		}

		// Token: 0x170003BF RID: 959
		// (get) Token: 0x06000C38 RID: 3128 RVA: 0x0002BF64 File Offset: 0x0002A164
		// (set) Token: 0x06000C39 RID: 3129 RVA: 0x0002BF6C File Offset: 0x0002A16C
		[DataSourceProperty]
		public bool CanInviteToClan
		{
			get
			{
				return this._canInviteToClan;
			}
			set
			{
				if (value != this._canInviteToClan)
				{
					this._canInviteToClan = value;
					base.OnPropertyChangedWithValue(value, "CanInviteToClan");
				}
			}
		}

		// Token: 0x170003C0 RID: 960
		// (get) Token: 0x06000C3A RID: 3130 RVA: 0x0002BF8A File Offset: 0x0002A18A
		// (set) Token: 0x06000C3B RID: 3131 RVA: 0x0002BF92 File Offset: 0x0002A192
		[DataSourceProperty]
		public bool IsSigilChangeInformationEnabled
		{
			get
			{
				return this._isSigilChangeInformationEnabled;
			}
			set
			{
				if (value != this._isSigilChangeInformationEnabled)
				{
					this._isSigilChangeInformationEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsSigilChangeInformationEnabled");
				}
			}
		}

		// Token: 0x170003C1 RID: 961
		// (get) Token: 0x06000C3C RID: 3132 RVA: 0x0002BFB0 File Offset: 0x0002A1B0
		// (set) Token: 0x06000C3D RID: 3133 RVA: 0x0002BFB8 File Offset: 0x0002A1B8
		[DataSourceProperty]
		public bool IsRankInfoLoading
		{
			get
			{
				return this._isRankInfoLoading;
			}
			set
			{
				if (value != this._isRankInfoLoading)
				{
					this._isRankInfoLoading = value;
					base.OnPropertyChangedWithValue(value, "IsRankInfoLoading");
				}
			}
		}

		// Token: 0x170003C2 RID: 962
		// (get) Token: 0x06000C3E RID: 3134 RVA: 0x0002BFD6 File Offset: 0x0002A1D6
		// (set) Token: 0x06000C3F RID: 3135 RVA: 0x0002BFDE File Offset: 0x0002A1DE
		[DataSourceProperty]
		public bool IsRankInfoCasual
		{
			get
			{
				return this._isRankInfoCasual;
			}
			set
			{
				if (value != this._isRankInfoCasual)
				{
					this._isRankInfoCasual = value;
					base.OnPropertyChangedWithValue(value, "IsRankInfoCasual");
				}
			}
		}

		// Token: 0x170003C3 RID: 963
		// (get) Token: 0x06000C40 RID: 3136 RVA: 0x0002BFFC File Offset: 0x0002A1FC
		// (set) Token: 0x06000C41 RID: 3137 RVA: 0x0002C004 File Offset: 0x0002A204
		[DataSourceProperty]
		public bool IsClanInfoSupported
		{
			get
			{
				return this._isClanInfoSupported;
			}
			set
			{
				if (value != this._isClanInfoSupported)
				{
					this._isClanInfoSupported = value;
					base.OnPropertyChangedWithValue(value, "IsClanInfoSupported");
				}
			}
		}

		// Token: 0x170003C4 RID: 964
		// (get) Token: 0x06000C42 RID: 3138 RVA: 0x0002C022 File Offset: 0x0002A222
		// (set) Token: 0x06000C43 RID: 3139 RVA: 0x0002C02A File Offset: 0x0002A22A
		[DataSourceProperty]
		public bool IsBannerlordIDSupported
		{
			get
			{
				return this._isBannerlordIDSupported;
			}
			set
			{
				if (value != this._isBannerlordIDSupported)
				{
					this._isBannerlordIDSupported = value;
					base.OnPropertyChangedWithValue(value, "IsBannerlordIDSupported");
				}
			}
		}

		// Token: 0x170003C5 RID: 965
		// (get) Token: 0x06000C44 RID: 3140 RVA: 0x0002C048 File Offset: 0x0002A248
		// (set) Token: 0x06000C45 RID: 3141 RVA: 0x0002C050 File Offset: 0x0002A250
		[DataSourceProperty]
		public int Level
		{
			get
			{
				return this._level;
			}
			set
			{
				if (value != this._level)
				{
					this._level = value;
					base.OnPropertyChangedWithValue(value, "Level");
				}
			}
		}

		// Token: 0x170003C6 RID: 966
		// (get) Token: 0x06000C46 RID: 3142 RVA: 0x0002C06E File Offset: 0x0002A26E
		// (set) Token: 0x06000C47 RID: 3143 RVA: 0x0002C076 File Offset: 0x0002A276
		[DataSourceProperty]
		public int Rating
		{
			get
			{
				return this._rating;
			}
			set
			{
				if (value != this._rating)
				{
					this._rating = value;
					base.OnPropertyChangedWithValue(value, "Rating");
				}
			}
		}

		// Token: 0x170003C7 RID: 967
		// (get) Token: 0x06000C48 RID: 3144 RVA: 0x0002C094 File Offset: 0x0002A294
		// (set) Token: 0x06000C49 RID: 3145 RVA: 0x0002C09C File Offset: 0x0002A29C
		[DataSourceProperty]
		public int Loot
		{
			get
			{
				return this._loot;
			}
			set
			{
				if (value != this._loot)
				{
					this._loot = value;
					base.OnPropertyChangedWithValue(value, "Loot");
				}
			}
		}

		// Token: 0x170003C8 RID: 968
		// (get) Token: 0x06000C4A RID: 3146 RVA: 0x0002C0BA File Offset: 0x0002A2BA
		// (set) Token: 0x06000C4B RID: 3147 RVA: 0x0002C0C2 File Offset: 0x0002A2C2
		[DataSourceProperty]
		public int ExperienceRatio
		{
			get
			{
				return this._experienceRatio;
			}
			set
			{
				if (value != this._experienceRatio)
				{
					this._experienceRatio = value;
					base.OnPropertyChangedWithValue(value, "ExperienceRatio");
				}
			}
		}

		// Token: 0x170003C9 RID: 969
		// (get) Token: 0x06000C4C RID: 3148 RVA: 0x0002C0E0 File Offset: 0x0002A2E0
		// (set) Token: 0x06000C4D RID: 3149 RVA: 0x0002C0E8 File Offset: 0x0002A2E8
		[DataSourceProperty]
		public int RatingRatio
		{
			get
			{
				return this._ratingRatio;
			}
			set
			{
				if (value != this._ratingRatio)
				{
					this._ratingRatio = value;
					base.OnPropertyChangedWithValue(value, "RatingRatio");
				}
			}
		}

		// Token: 0x170003CA RID: 970
		// (get) Token: 0x06000C4E RID: 3150 RVA: 0x0002C106 File Offset: 0x0002A306
		// (set) Token: 0x06000C4F RID: 3151 RVA: 0x0002C10E File Offset: 0x0002A30E
		[DataSourceProperty]
		public string Name
		{
			get
			{
				return this._name;
			}
			set
			{
				if (value != this._name)
				{
					this._name = value;
					base.OnPropertyChangedWithValue<string>(value, "Name");
				}
			}
		}

		// Token: 0x170003CB RID: 971
		// (get) Token: 0x06000C50 RID: 3152 RVA: 0x0002C131 File Offset: 0x0002A331
		// (set) Token: 0x06000C51 RID: 3153 RVA: 0x0002C139 File Offset: 0x0002A339
		[DataSourceProperty]
		public string StateText
		{
			get
			{
				return this._stateText;
			}
			set
			{
				if (value != this._stateText)
				{
					this._stateText = value;
					base.OnPropertyChangedWithValue<string>(value, "StateText");
				}
			}
		}

		// Token: 0x170003CC RID: 972
		// (get) Token: 0x06000C52 RID: 3154 RVA: 0x0002C15C File Offset: 0x0002A35C
		// (set) Token: 0x06000C53 RID: 3155 RVA: 0x0002C164 File Offset: 0x0002A364
		[DataSourceProperty]
		public string LevelText
		{
			get
			{
				return this._levelText;
			}
			set
			{
				if (value != this._levelText)
				{
					this._levelText = value;
					base.OnPropertyChangedWithValue<string>(value, "LevelText");
				}
			}
		}

		// Token: 0x170003CD RID: 973
		// (get) Token: 0x06000C54 RID: 3156 RVA: 0x0002C187 File Offset: 0x0002A387
		// (set) Token: 0x06000C55 RID: 3157 RVA: 0x0002C18F File Offset: 0x0002A38F
		[DataSourceProperty]
		public string LevelTitleText
		{
			get
			{
				return this._levelTitleText;
			}
			set
			{
				if (value != this._levelTitleText)
				{
					this._levelTitleText = value;
					base.OnPropertyChangedWithValue<string>(value, "LevelTitleText");
				}
			}
		}

		// Token: 0x170003CE RID: 974
		// (get) Token: 0x06000C56 RID: 3158 RVA: 0x0002C1B2 File Offset: 0x0002A3B2
		// (set) Token: 0x06000C57 RID: 3159 RVA: 0x0002C1BA File Offset: 0x0002A3BA
		[DataSourceProperty]
		public string RatingText
		{
			get
			{
				return this._ratingText;
			}
			set
			{
				if (value != this._ratingText)
				{
					this._ratingText = value;
					base.OnPropertyChangedWithValue<string>(value, "RatingText");
				}
			}
		}

		// Token: 0x170003CF RID: 975
		// (get) Token: 0x06000C58 RID: 3160 RVA: 0x0002C1DD File Offset: 0x0002A3DD
		// (set) Token: 0x06000C59 RID: 3161 RVA: 0x0002C1E5 File Offset: 0x0002A3E5
		[DataSourceProperty]
		public string RatingID
		{
			get
			{
				return this._ratingID;
			}
			set
			{
				if (value != this._ratingID)
				{
					this._ratingID = value;
					base.OnPropertyChangedWithValue<string>(value, "RatingID");
				}
			}
		}

		// Token: 0x170003D0 RID: 976
		// (get) Token: 0x06000C5A RID: 3162 RVA: 0x0002C208 File Offset: 0x0002A408
		// (set) Token: 0x06000C5B RID: 3163 RVA: 0x0002C210 File Offset: 0x0002A410
		[DataSourceProperty]
		public string ClanName
		{
			get
			{
				return this._clanName;
			}
			set
			{
				if (value != this._clanName)
				{
					this._clanName = value;
					base.OnPropertyChangedWithValue<string>(value, "ClanName");
				}
			}
		}

		// Token: 0x170003D1 RID: 977
		// (get) Token: 0x06000C5C RID: 3164 RVA: 0x0002C233 File Offset: 0x0002A433
		// (set) Token: 0x06000C5D RID: 3165 RVA: 0x0002C23B File Offset: 0x0002A43B
		[DataSourceProperty]
		public string ClanTag
		{
			get
			{
				return this._clanTag;
			}
			set
			{
				if (value != this._clanTag)
				{
					this._clanTag = value;
					base.OnPropertyChangedWithValue<string>(value, "ClanTag");
				}
			}
		}

		// Token: 0x170003D2 RID: 978
		// (get) Token: 0x06000C5E RID: 3166 RVA: 0x0002C25E File Offset: 0x0002A45E
		// (set) Token: 0x06000C5F RID: 3167 RVA: 0x0002C266 File Offset: 0x0002A466
		[DataSourceProperty]
		public string ChangeText
		{
			get
			{
				return this._changeText;
			}
			set
			{
				if (value != this._changeText)
				{
					this._changeText = value;
					base.OnPropertyChangedWithValue<string>(value, "ChangeText");
				}
			}
		}

		// Token: 0x170003D3 RID: 979
		// (get) Token: 0x06000C60 RID: 3168 RVA: 0x0002C289 File Offset: 0x0002A489
		// (set) Token: 0x06000C61 RID: 3169 RVA: 0x0002C291 File Offset: 0x0002A491
		[DataSourceProperty]
		public string ClanInfoTitleText
		{
			get
			{
				return this._clanInfoTitleText;
			}
			set
			{
				if (value != this._clanInfoTitleText)
				{
					this._clanInfoTitleText = value;
					base.OnPropertyChangedWithValue<string>(value, "ClanInfoTitleText");
				}
			}
		}

		// Token: 0x170003D4 RID: 980
		// (get) Token: 0x06000C62 RID: 3170 RVA: 0x0002C2B4 File Offset: 0x0002A4B4
		// (set) Token: 0x06000C63 RID: 3171 RVA: 0x0002C2BC File Offset: 0x0002A4BC
		[DataSourceProperty]
		public string BadgeInfoTitleText
		{
			get
			{
				return this._badgeInfoTitleText;
			}
			set
			{
				if (value != this._badgeInfoTitleText)
				{
					this._badgeInfoTitleText = value;
					base.OnPropertyChangedWithValue<string>(value, "BadgeInfoTitleText");
				}
			}
		}

		// Token: 0x170003D5 RID: 981
		// (get) Token: 0x06000C64 RID: 3172 RVA: 0x0002C2DF File Offset: 0x0002A4DF
		// (set) Token: 0x06000C65 RID: 3173 RVA: 0x0002C2E7 File Offset: 0x0002A4E7
		[DataSourceProperty]
		public string AvatarInfoTitleText
		{
			get
			{
				return this._avatarInfoTitleText;
			}
			set
			{
				if (value != this._avatarInfoTitleText)
				{
					this._avatarInfoTitleText = value;
					base.OnPropertyChangedWithValue<string>(value, "AvatarInfoTitleText");
				}
			}
		}

		// Token: 0x170003D6 RID: 982
		// (get) Token: 0x06000C66 RID: 3174 RVA: 0x0002C30A File Offset: 0x0002A50A
		// (set) Token: 0x06000C67 RID: 3175 RVA: 0x0002C312 File Offset: 0x0002A512
		[DataSourceProperty]
		public string ExperienceText
		{
			get
			{
				return this._experienceText;
			}
			set
			{
				if (value != this._experienceText)
				{
					this._experienceText = value;
					base.OnPropertyChangedWithValue<string>(value, "ExperienceText");
				}
			}
		}

		// Token: 0x170003D7 RID: 983
		// (get) Token: 0x06000C68 RID: 3176 RVA: 0x0002C335 File Offset: 0x0002A535
		// (set) Token: 0x06000C69 RID: 3177 RVA: 0x0002C33D File Offset: 0x0002A53D
		[DataSourceProperty]
		public string RankText
		{
			get
			{
				return this._rankText;
			}
			set
			{
				if (value != this._rankText)
				{
					this._rankText = value;
					base.OnPropertyChangedWithValue<string>(value, "RankText");
				}
			}
		}

		// Token: 0x170003D8 RID: 984
		// (get) Token: 0x06000C6A RID: 3178 RVA: 0x0002C360 File Offset: 0x0002A560
		// (set) Token: 0x06000C6B RID: 3179 RVA: 0x0002C368 File Offset: 0x0002A568
		[DataSourceProperty]
		public string BannerlordID
		{
			get
			{
				return this._bannerlordID;
			}
			set
			{
				if (value != this._bannerlordID)
				{
					this._bannerlordID = value;
					base.OnPropertyChangedWithValue<string>(value, "BannerlordID");
				}
			}
		}

		// Token: 0x170003D9 RID: 985
		// (get) Token: 0x06000C6C RID: 3180 RVA: 0x0002C38B File Offset: 0x0002A58B
		// (set) Token: 0x06000C6D RID: 3181 RVA: 0x0002C393 File Offset: 0x0002A593
		[DataSourceProperty]
		public string SelectedBadgeID
		{
			get
			{
				return this._selectedBadgeID;
			}
			set
			{
				if (value != this._selectedBadgeID)
				{
					this._selectedBadgeID = value;
					base.OnPropertyChangedWithValue<string>(value, "SelectedBadgeID");
				}
			}
		}

		// Token: 0x170003DA RID: 986
		// (get) Token: 0x06000C6E RID: 3182 RVA: 0x0002C3B6 File Offset: 0x0002A5B6
		// (set) Token: 0x06000C6F RID: 3183 RVA: 0x0002C3BE File Offset: 0x0002A5BE
		[DataSourceProperty]
		public HintViewModel NameHint
		{
			get
			{
				return this._nameHint;
			}
			set
			{
				if (value != this._nameHint)
				{
					this._nameHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "NameHint");
				}
			}
		}

		// Token: 0x170003DB RID: 987
		// (get) Token: 0x06000C70 RID: 3184 RVA: 0x0002C3DC File Offset: 0x0002A5DC
		// (set) Token: 0x06000C71 RID: 3185 RVA: 0x0002C3E4 File Offset: 0x0002A5E4
		[DataSourceProperty]
		public HintViewModel InviteToPartyHint
		{
			get
			{
				return this._inviteToPartyHint;
			}
			set
			{
				if (value != this._inviteToPartyHint)
				{
					this._inviteToPartyHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "InviteToPartyHint");
				}
			}
		}

		// Token: 0x170003DC RID: 988
		// (get) Token: 0x06000C72 RID: 3186 RVA: 0x0002C402 File Offset: 0x0002A602
		// (set) Token: 0x06000C73 RID: 3187 RVA: 0x0002C40A File Offset: 0x0002A60A
		[DataSourceProperty]
		public HintViewModel RemoveFriendHint
		{
			get
			{
				return this._removeFriendHint;
			}
			set
			{
				if (value != this._removeFriendHint)
				{
					this._removeFriendHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "RemoveFriendHint");
				}
			}
		}

		// Token: 0x170003DD RID: 989
		// (get) Token: 0x06000C74 RID: 3188 RVA: 0x0002C428 File Offset: 0x0002A628
		// (set) Token: 0x06000C75 RID: 3189 RVA: 0x0002C430 File Offset: 0x0002A630
		[DataSourceProperty]
		public HintViewModel AcceptFriendRequestHint
		{
			get
			{
				return this._acceptFriendRequestHint;
			}
			set
			{
				if (value != this._acceptFriendRequestHint)
				{
					this._acceptFriendRequestHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "AcceptFriendRequestHint");
				}
			}
		}

		// Token: 0x170003DE RID: 990
		// (get) Token: 0x06000C76 RID: 3190 RVA: 0x0002C44E File Offset: 0x0002A64E
		// (set) Token: 0x06000C77 RID: 3191 RVA: 0x0002C456 File Offset: 0x0002A656
		[DataSourceProperty]
		public HintViewModel DeclineFriendRequestHint
		{
			get
			{
				return this._declineFriendRequestHint;
			}
			set
			{
				if (value != this._declineFriendRequestHint)
				{
					this._declineFriendRequestHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "DeclineFriendRequestHint");
				}
			}
		}

		// Token: 0x170003DF RID: 991
		// (get) Token: 0x06000C78 RID: 3192 RVA: 0x0002C474 File Offset: 0x0002A674
		// (set) Token: 0x06000C79 RID: 3193 RVA: 0x0002C47C File Offset: 0x0002A67C
		[DataSourceProperty]
		public HintViewModel CancelFriendRequestHint
		{
			get
			{
				return this._cancelFriendRequestHint;
			}
			set
			{
				if (value != this._cancelFriendRequestHint)
				{
					this._cancelFriendRequestHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "CancelFriendRequestHint");
				}
			}
		}

		// Token: 0x170003E0 RID: 992
		// (get) Token: 0x06000C7A RID: 3194 RVA: 0x0002C49A File Offset: 0x0002A69A
		// (set) Token: 0x06000C7B RID: 3195 RVA: 0x0002C4A2 File Offset: 0x0002A6A2
		[DataSourceProperty]
		public HintViewModel InviteToClanHint
		{
			get
			{
				return this._inviteToClanHint;
			}
			set
			{
				if (value != this._inviteToClanHint)
				{
					this._inviteToClanHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "InviteToClanHint");
				}
			}
		}

		// Token: 0x170003E1 RID: 993
		// (get) Token: 0x06000C7C RID: 3196 RVA: 0x0002C4C0 File Offset: 0x0002A6C0
		// (set) Token: 0x06000C7D RID: 3197 RVA: 0x0002C4C8 File Offset: 0x0002A6C8
		[DataSourceProperty]
		public HintViewModel ChangeBannerlordIDHint
		{
			get
			{
				return this._changeBannerlordIDHint;
			}
			set
			{
				if (value != this._changeBannerlordIDHint)
				{
					this._changeBannerlordIDHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "ChangeBannerlordIDHint");
				}
			}
		}

		// Token: 0x170003E2 RID: 994
		// (get) Token: 0x06000C7E RID: 3198 RVA: 0x0002C4E6 File Offset: 0x0002A6E6
		// (set) Token: 0x06000C7F RID: 3199 RVA: 0x0002C4EE File Offset: 0x0002A6EE
		[DataSourceProperty]
		public HintViewModel CopyBannerlordIDHint
		{
			get
			{
				return this._copyBannerlordIDHint;
			}
			set
			{
				if (value != this._copyBannerlordIDHint)
				{
					this._copyBannerlordIDHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "CopyBannerlordIDHint");
				}
			}
		}

		// Token: 0x170003E3 RID: 995
		// (get) Token: 0x06000C80 RID: 3200 RVA: 0x0002C50C File Offset: 0x0002A70C
		// (set) Token: 0x06000C81 RID: 3201 RVA: 0x0002C514 File Offset: 0x0002A714
		[DataSourceProperty]
		public HintViewModel AddFriendWithBannerlordIDHint
		{
			get
			{
				return this._addFriendWithBannerlordIDHint;
			}
			set
			{
				if (value != this._addFriendWithBannerlordIDHint)
				{
					this._addFriendWithBannerlordIDHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "AddFriendWithBannerlordIDHint");
				}
			}
		}

		// Token: 0x170003E4 RID: 996
		// (get) Token: 0x06000C82 RID: 3202 RVA: 0x0002C532 File Offset: 0x0002A732
		// (set) Token: 0x06000C83 RID: 3203 RVA: 0x0002C53A File Offset: 0x0002A73A
		[DataSourceProperty]
		public HintViewModel ExperienceHint
		{
			get
			{
				return this._experienceHint;
			}
			set
			{
				if (value != this._experienceHint)
				{
					this._experienceHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "ExperienceHint");
				}
			}
		}

		// Token: 0x170003E5 RID: 997
		// (get) Token: 0x06000C84 RID: 3204 RVA: 0x0002C558 File Offset: 0x0002A758
		// (set) Token: 0x06000C85 RID: 3205 RVA: 0x0002C560 File Offset: 0x0002A760
		[DataSourceProperty]
		public HintViewModel RatingHint
		{
			get
			{
				return this._ratingHint;
			}
			set
			{
				if (value != this._ratingHint)
				{
					this._ratingHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "RatingHint");
				}
			}
		}

		// Token: 0x170003E6 RID: 998
		// (get) Token: 0x06000C86 RID: 3206 RVA: 0x0002C57E File Offset: 0x0002A77E
		// (set) Token: 0x06000C87 RID: 3207 RVA: 0x0002C586 File Offset: 0x0002A786
		[DataSourceProperty]
		public HintViewModel LootHint
		{
			get
			{
				return this._lootHint;
			}
			set
			{
				if (value != this._lootHint)
				{
					this._lootHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "LootHint");
				}
			}
		}

		// Token: 0x170003E7 RID: 999
		// (get) Token: 0x06000C88 RID: 3208 RVA: 0x0002C5A4 File Offset: 0x0002A7A4
		// (set) Token: 0x06000C89 RID: 3209 RVA: 0x0002C5AC File Offset: 0x0002A7AC
		[DataSourceProperty]
		public HintViewModel SkirmishRatingHint
		{
			get
			{
				return this._skirmishRatingHint;
			}
			set
			{
				if (value != this._skirmishRatingHint)
				{
					this._skirmishRatingHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "SkirmishRatingHint");
				}
			}
		}

		// Token: 0x170003E8 RID: 1000
		// (get) Token: 0x06000C8A RID: 3210 RVA: 0x0002C5CA File Offset: 0x0002A7CA
		// (set) Token: 0x06000C8B RID: 3211 RVA: 0x0002C5D2 File Offset: 0x0002A7D2
		[DataSourceProperty]
		public HintViewModel CaptainRatingHint
		{
			get
			{
				return this._captainRatingHint;
			}
			set
			{
				if (value != this._captainRatingHint)
				{
					this._captainRatingHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "CaptainRatingHint");
				}
			}
		}

		// Token: 0x170003E9 RID: 1001
		// (get) Token: 0x06000C8C RID: 3212 RVA: 0x0002C5F0 File Offset: 0x0002A7F0
		// (set) Token: 0x06000C8D RID: 3213 RVA: 0x0002C5F8 File Offset: 0x0002A7F8
		[DataSourceProperty]
		public HintViewModel ClanLeaderboardHint
		{
			get
			{
				return this._clanLeaderboardHint;
			}
			set
			{
				if (value != this._clanLeaderboardHint)
				{
					this._clanLeaderboardHint = value;
					base.OnPropertyChanged("ClanLeaderboardHint");
				}
			}
		}

		// Token: 0x170003EA RID: 1002
		// (get) Token: 0x06000C8E RID: 3214 RVA: 0x0002C615 File Offset: 0x0002A815
		// (set) Token: 0x06000C8F RID: 3215 RVA: 0x0002C61D File Offset: 0x0002A81D
		[DataSourceProperty]
		public ImageIdentifierVM Avatar
		{
			get
			{
				return this._avatar;
			}
			set
			{
				if (value != this._avatar)
				{
					this._avatar = value;
					base.OnPropertyChangedWithValue<ImageIdentifierVM>(value, "Avatar");
				}
			}
		}

		// Token: 0x170003EB RID: 1003
		// (get) Token: 0x06000C90 RID: 3216 RVA: 0x0002C63B File Offset: 0x0002A83B
		// (set) Token: 0x06000C91 RID: 3217 RVA: 0x0002C643 File Offset: 0x0002A843
		[DataSourceProperty]
		public ImageIdentifierVM ClanBanner
		{
			get
			{
				return this._clanBanner;
			}
			set
			{
				if (value != this._clanBanner)
				{
					this._clanBanner = value;
					base.OnPropertyChangedWithValue<ImageIdentifierVM>(value, "ClanBanner");
				}
			}
		}

		// Token: 0x170003EC RID: 1004
		// (get) Token: 0x06000C92 RID: 3218 RVA: 0x0002C661 File Offset: 0x0002A861
		// (set) Token: 0x06000C93 RID: 3219 RVA: 0x0002C669 File Offset: 0x0002A869
		[DataSourceProperty]
		public MPLobbySigilItemVM Sigil
		{
			get
			{
				return this._sigil;
			}
			set
			{
				if (value != this._sigil)
				{
					this._sigil = value;
					base.OnPropertyChangedWithValue<MPLobbySigilItemVM>(value, "Sigil");
				}
			}
		}

		// Token: 0x170003ED RID: 1005
		// (get) Token: 0x06000C94 RID: 3220 RVA: 0x0002C687 File Offset: 0x0002A887
		// (set) Token: 0x06000C95 RID: 3221 RVA: 0x0002C68F File Offset: 0x0002A88F
		[DataSourceProperty]
		public MPLobbyBadgeItemVM ShownBadge
		{
			get
			{
				return this._shownBadge;
			}
			set
			{
				if (value != this._shownBadge)
				{
					this._shownBadge = value;
					base.OnPropertyChangedWithValue<MPLobbyBadgeItemVM>(value, "ShownBadge");
				}
			}
		}

		// Token: 0x170003EE RID: 1006
		// (get) Token: 0x06000C96 RID: 3222 RVA: 0x0002C6AD File Offset: 0x0002A8AD
		// (set) Token: 0x06000C97 RID: 3223 RVA: 0x0002C6B5 File Offset: 0x0002A8B5
		[DataSourceProperty]
		public CharacterViewModel CharacterVisual
		{
			get
			{
				return this._characterVisual;
			}
			set
			{
				if (value != this._characterVisual)
				{
					this._characterVisual = value;
					base.OnPropertyChangedWithValue<CharacterViewModel>(value, "CharacterVisual");
				}
			}
		}

		// Token: 0x170003EF RID: 1007
		// (get) Token: 0x06000C98 RID: 3224 RVA: 0x0002C6D3 File Offset: 0x0002A8D3
		// (set) Token: 0x06000C99 RID: 3225 RVA: 0x0002C6DB File Offset: 0x0002A8DB
		[DataSourceProperty]
		public MBBindingList<MPLobbyPlayerStatItemVM> DisplayedStats
		{
			get
			{
				return this._displayedStats;
			}
			set
			{
				if (value != this._displayedStats)
				{
					this._displayedStats = value;
					base.OnPropertyChangedWithValue<MBBindingList<MPLobbyPlayerStatItemVM>>(value, "DisplayedStats");
				}
			}
		}

		// Token: 0x170003F0 RID: 1008
		// (get) Token: 0x06000C9A RID: 3226 RVA: 0x0002C6F9 File Offset: 0x0002A8F9
		// (set) Token: 0x06000C9B RID: 3227 RVA: 0x0002C701 File Offset: 0x0002A901
		[DataSourceProperty]
		public MBBindingList<MPLobbyGameTypeVM> GameTypes
		{
			get
			{
				return this._gameTypes;
			}
			set
			{
				if (value != this._gameTypes)
				{
					this._gameTypes = value;
					base.OnPropertyChangedWithValue<MBBindingList<MPLobbyGameTypeVM>>(value, "GameTypes");
				}
			}
		}

		// Token: 0x040005AB RID: 1451
		public static Action<PlayerId> OnPlayerProfileRequested;

		// Token: 0x040005AC RID: 1452
		public static Action<PlayerId> OnBannerlordIDChangeRequested;

		// Token: 0x040005AD RID: 1453
		public static Action<PlayerId> OnAddFriendWithBannerlordIDRequested;

		// Token: 0x040005AE RID: 1454
		public static Action<PlayerId> OnSigilChangeRequested;

		// Token: 0x040005AF RID: 1455
		public static Action<PlayerId> OnBadgeChangeRequested;

		// Token: 0x040005B0 RID: 1456
		public static Action<MPLobbyPlayerBaseVM> OnRankProgressionRequested;

		// Token: 0x040005B1 RID: 1457
		public static Action<string> OnRankLeaderboardRequested;

		// Token: 0x040005B2 RID: 1458
		public static Action OnClanPageRequested;

		// Token: 0x040005B3 RID: 1459
		public static Action OnClanLeaderboardRequested;

		// Token: 0x040005B4 RID: 1460
		private const int DefaultBannerBackgroundColorId = 99;

		// Token: 0x040005B6 RID: 1462
		private PlayerId _providedID;

		// Token: 0x040005B7 RID: 1463
		private readonly string _forcedName = string.Empty;

		// Token: 0x040005B8 RID: 1464
		private int _forcedAvatarIndex = -1;

		// Token: 0x040005BA RID: 1466
		private Action<PlayerId> _onInviteToParty;

		// Token: 0x040005BB RID: 1467
		private readonly Action<PlayerId> _onInviteToClan;

		// Token: 0x040005BC RID: 1468
		private readonly Action<PlayerId> _onFriendRequestAnswered;

		// Token: 0x040005BF RID: 1471
		private bool _isKnownPlayer;

		// Token: 0x040005C0 RID: 1472
		private readonly TextObject _genericPlayerName = new TextObject("{=RN6zHak0}Player", null);

		// Token: 0x040005C1 RID: 1473
		public Action OnPlayerStatsReceived;

		// Token: 0x040005C2 RID: 1474
		protected bool _hasReceivedPlayerStats;

		// Token: 0x040005C3 RID: 1475
		protected bool _isReceivingPlayerStats;

		// Token: 0x040005C7 RID: 1479
		private const string _skirmishGameTypeID = "Skirmish";

		// Token: 0x040005C8 RID: 1480
		private const string _captainGameTypeID = "Captain";

		// Token: 0x040005C9 RID: 1481
		private const string _duelGameTypeID = "Duel";

		// Token: 0x040005CA RID: 1482
		private const string _teamDeathmatchGameTypeID = "TeamDeathmatch";

		// Token: 0x040005CB RID: 1483
		private const string _siegeGameTypeID = "Siege";

		// Token: 0x040005CC RID: 1484
		public Action<string> OnRankInfoChanged;

		// Token: 0x040005CD RID: 1485
		private bool _canCopyID;

		// Token: 0x040005CE RID: 1486
		private bool _showLevel;

		// Token: 0x040005CF RID: 1487
		private bool _isSelected;

		// Token: 0x040005D0 RID: 1488
		private bool _hasNotification;

		// Token: 0x040005D1 RID: 1489
		private bool _isFriendRequest;

		// Token: 0x040005D2 RID: 1490
		private bool _isPendingRequest;

		// Token: 0x040005D3 RID: 1491
		private bool _canRemove;

		// Token: 0x040005D4 RID: 1492
		private bool _canBeInvited;

		// Token: 0x040005D5 RID: 1493
		private bool _canInviteToParty;

		// Token: 0x040005D6 RID: 1494
		private bool _canInviteToClan;

		// Token: 0x040005D7 RID: 1495
		private bool _isSigilChangeInformationEnabled;

		// Token: 0x040005D8 RID: 1496
		private bool _isRankInfoLoading;

		// Token: 0x040005D9 RID: 1497
		private bool _isRankInfoCasual;

		// Token: 0x040005DA RID: 1498
		private bool _isClanInfoSupported;

		// Token: 0x040005DB RID: 1499
		private bool _isBannerlordIDSupported;

		// Token: 0x040005DC RID: 1500
		private int _level;

		// Token: 0x040005DD RID: 1501
		private int _rating;

		// Token: 0x040005DE RID: 1502
		private int _loot;

		// Token: 0x040005DF RID: 1503
		private int _experienceRatio;

		// Token: 0x040005E0 RID: 1504
		private int _ratingRatio;

		// Token: 0x040005E1 RID: 1505
		private string _name = "";

		// Token: 0x040005E2 RID: 1506
		private string _stateText;

		// Token: 0x040005E3 RID: 1507
		private string _levelText;

		// Token: 0x040005E4 RID: 1508
		private string _levelTitleText;

		// Token: 0x040005E5 RID: 1509
		private string _ratingText;

		// Token: 0x040005E6 RID: 1510
		private string _ratingID;

		// Token: 0x040005E7 RID: 1511
		private string _clanName;

		// Token: 0x040005E8 RID: 1512
		private string _clanTag;

		// Token: 0x040005E9 RID: 1513
		private string _changeText;

		// Token: 0x040005EA RID: 1514
		private string _clanInfoTitleText;

		// Token: 0x040005EB RID: 1515
		private string _badgeInfoTitleText;

		// Token: 0x040005EC RID: 1516
		private string _avatarInfoTitleText;

		// Token: 0x040005ED RID: 1517
		private string _experienceText;

		// Token: 0x040005EE RID: 1518
		private string _rankText;

		// Token: 0x040005EF RID: 1519
		private string _bannerlordID;

		// Token: 0x040005F0 RID: 1520
		private string _selectedBadgeID;

		// Token: 0x040005F1 RID: 1521
		private HintViewModel _nameHint;

		// Token: 0x040005F2 RID: 1522
		private HintViewModel _inviteToPartyHint;

		// Token: 0x040005F3 RID: 1523
		private HintViewModel _removeFriendHint;

		// Token: 0x040005F4 RID: 1524
		private HintViewModel _acceptFriendRequestHint;

		// Token: 0x040005F5 RID: 1525
		private HintViewModel _declineFriendRequestHint;

		// Token: 0x040005F6 RID: 1526
		private HintViewModel _cancelFriendRequestHint;

		// Token: 0x040005F7 RID: 1527
		private HintViewModel _inviteToClanHint;

		// Token: 0x040005F8 RID: 1528
		private HintViewModel _changeBannerlordIDHint;

		// Token: 0x040005F9 RID: 1529
		private HintViewModel _copyBannerlordIDHint;

		// Token: 0x040005FA RID: 1530
		private HintViewModel _addFriendWithBannerlordIDHint;

		// Token: 0x040005FB RID: 1531
		private HintViewModel _experienceHint;

		// Token: 0x040005FC RID: 1532
		private HintViewModel _ratingHint;

		// Token: 0x040005FD RID: 1533
		private HintViewModel _lootHint;

		// Token: 0x040005FE RID: 1534
		private HintViewModel _skirmishRatingHint;

		// Token: 0x040005FF RID: 1535
		private HintViewModel _captainRatingHint;

		// Token: 0x04000600 RID: 1536
		private HintViewModel _clanLeaderboardHint;

		// Token: 0x04000601 RID: 1537
		private ImageIdentifierVM _avatar;

		// Token: 0x04000602 RID: 1538
		private ImageIdentifierVM _clanBanner;

		// Token: 0x04000603 RID: 1539
		private MPLobbySigilItemVM _sigil;

		// Token: 0x04000604 RID: 1540
		private MPLobbyBadgeItemVM _shownBadge;

		// Token: 0x04000605 RID: 1541
		private CharacterViewModel _characterVisual;

		// Token: 0x04000606 RID: 1542
		private MBBindingList<MPLobbyPlayerStatItemVM> _displayedStats;

		// Token: 0x04000607 RID: 1543
		private MBBindingList<MPLobbyGameTypeVM> _gameTypes;

		// Token: 0x020001C6 RID: 454
		public enum OnlineStatus
		{
			// Token: 0x04000DA8 RID: 3496
			None,
			// Token: 0x04000DA9 RID: 3497
			InGame,
			// Token: 0x04000DAA RID: 3498
			Online,
			// Token: 0x04000DAB RID: 3499
			Offline
		}
	}
}
