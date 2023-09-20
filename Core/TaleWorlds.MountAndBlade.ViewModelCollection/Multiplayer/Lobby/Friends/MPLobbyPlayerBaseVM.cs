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
	public class MPLobbyPlayerBaseVM : ViewModel
	{
		public MPLobbyPlayerBaseVM.OnlineStatus CurrentOnlineStatus { get; private set; }

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

		public PlayerData PlayerData { get; private set; }

		public AnotherPlayerState State { get; protected set; }

		public float TimeSinceLastStateUpdate { get; protected set; }

		public PlayerStatsBase[] PlayerStats { get; private set; }

		public GameTypeRankInfo[] RankInfo { get; private set; }

		public string RankInfoGameTypeID { get; private set; }

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

		protected void UpdateAvatar(bool isKnownPlayer)
		{
			this.UpdateForcedAvatarIndex(isKnownPlayer);
			this.Avatar = new ImageIdentifierVM(this.ProvidedID, this._forcedAvatarIndex);
		}

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

		public void OnStatusChanged(MPLobbyPlayerBaseVM.OnlineStatus status, bool isInGameStatusActive)
		{
			this.CurrentOnlineStatus = status;
			this.StateText = "";
			this.TimeSinceLastStateUpdate = 0f;
			this.CanInviteToParty = this._onInviteToParty != null && (status == MPLobbyPlayerBaseVM.OnlineStatus.InGame || (status == MPLobbyPlayerBaseVM.OnlineStatus.Online && !isInGameStatusActive));
			this.ShowLevel = status == MPLobbyPlayerBaseVM.OnlineStatus.InGame || (status == MPLobbyPlayerBaseVM.OnlineStatus.Online && !isInGameStatusActive);
			this.CanInviteToClan = this._onInviteToClan != null && status == MPLobbyPlayerBaseVM.OnlineStatus.Online;
		}

		public void SetOnInvite(Action<PlayerId> onInvite)
		{
			this._onInviteToParty = onInvite;
			this.CanInviteToParty = onInvite != null;
			this.RefreshValues();
		}

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

		public void ExecuteSelectPlayer()
		{
			this.IsSelected = !this.IsSelected;
		}

		public void ExecuteInviteToParty()
		{
			Action<PlayerId> onInviteToParty = this._onInviteToParty;
			if (onInviteToParty == null)
			{
				return;
			}
			onInviteToParty(this.ProvidedID);
		}

		public void ExecuteInviteToClan()
		{
			Action<PlayerId> onInviteToClan = this._onInviteToClan;
			if (onInviteToClan == null)
			{
				return;
			}
			onInviteToClan(this.ProvidedID);
		}

		public void ExecuteKickFromParty()
		{
			if (NetworkMain.GameClient.IsInParty && NetworkMain.GameClient.IsPartyLeader)
			{
				NetworkMain.GameClient.KickPlayerFromParty(this.ProvidedID);
			}
		}

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

		public void ExecuteCancelPendingFriendRequest()
		{
			NetworkMain.GameClient.RemoveFriend(this.ProvidedID);
		}

		public void ExecuteRemoveFriend()
		{
			NetworkMain.GameClient.RemoveFriend(this.ProvidedID);
		}

		public void ExecuteCopyBannerlordID()
		{
			Input.SetClipboardText(this.BannerlordID);
		}

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

		public void ExecuteShowProfile()
		{
			Action<PlayerId> onPlayerProfileRequested = MPLobbyPlayerBaseVM.OnPlayerProfileRequested;
			if (onPlayerProfileRequested == null)
			{
				return;
			}
			onPlayerProfileRequested(this.ProvidedID);
		}

		private void ExecuteActivateSigilChangeInformation()
		{
			this.IsSigilChangeInformationEnabled = true;
		}

		private void ExecuteDeactivateSigilChangeInformation()
		{
			this.IsSigilChangeInformationEnabled = false;
		}

		private void ExecuteChangeSigil()
		{
			Action<PlayerId> onSigilChangeRequested = MPLobbyPlayerBaseVM.OnSigilChangeRequested;
			if (onSigilChangeRequested == null)
			{
				return;
			}
			onSigilChangeRequested(this.ProvidedID);
		}

		private void ExecuteChangeBannerlordID()
		{
			Action<PlayerId> onBannerlordIDChangeRequested = MPLobbyPlayerBaseVM.OnBannerlordIDChangeRequested;
			if (onBannerlordIDChangeRequested == null)
			{
				return;
			}
			onBannerlordIDChangeRequested(this.ProvidedID);
		}

		private void ExecuteAddFriendWithBannerlordID()
		{
			Action<PlayerId> onAddFriendWithBannerlordIDRequested = MPLobbyPlayerBaseVM.OnAddFriendWithBannerlordIDRequested;
			if (onAddFriendWithBannerlordIDRequested == null)
			{
				return;
			}
			onAddFriendWithBannerlordIDRequested(this.ProvidedID);
		}

		private void ExecuteChangeBadge()
		{
			Action<PlayerId> onBadgeChangeRequested = MPLobbyPlayerBaseVM.OnBadgeChangeRequested;
			if (onBadgeChangeRequested == null)
			{
				return;
			}
			onBadgeChangeRequested(this.ProvidedID);
		}

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

		private void ExecuteShowClanPage()
		{
			Action onClanPageRequested = MPLobbyPlayerBaseVM.OnClanPageRequested;
			if (onClanPageRequested == null)
			{
				return;
			}
			onClanPageRequested();
		}

		private void ExecuteShowClanLeaderboard()
		{
			Action onClanLeaderboardRequested = MPLobbyPlayerBaseVM.OnClanLeaderboardRequested;
			if (onClanLeaderboardRequested == null)
			{
				return;
			}
			onClanLeaderboardRequested();
		}

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

		public static Action<PlayerId> OnPlayerProfileRequested;

		public static Action<PlayerId> OnBannerlordIDChangeRequested;

		public static Action<PlayerId> OnAddFriendWithBannerlordIDRequested;

		public static Action<PlayerId> OnSigilChangeRequested;

		public static Action<PlayerId> OnBadgeChangeRequested;

		public static Action<MPLobbyPlayerBaseVM> OnRankProgressionRequested;

		public static Action<string> OnRankLeaderboardRequested;

		public static Action OnClanPageRequested;

		public static Action OnClanLeaderboardRequested;

		private const int DefaultBannerBackgroundColorId = 99;

		private PlayerId _providedID;

		private readonly string _forcedName = string.Empty;

		private int _forcedAvatarIndex = -1;

		private Action<PlayerId> _onInviteToParty;

		private readonly Action<PlayerId> _onInviteToClan;

		private readonly Action<PlayerId> _onFriendRequestAnswered;

		private bool _isKnownPlayer;

		private readonly TextObject _genericPlayerName = new TextObject("{=RN6zHak0}Player", null);

		public Action OnPlayerStatsReceived;

		protected bool _hasReceivedPlayerStats;

		protected bool _isReceivingPlayerStats;

		private const string _skirmishGameTypeID = "Skirmish";

		private const string _captainGameTypeID = "Captain";

		private const string _duelGameTypeID = "Duel";

		private const string _teamDeathmatchGameTypeID = "TeamDeathmatch";

		private const string _siegeGameTypeID = "Siege";

		public Action<string> OnRankInfoChanged;

		private bool _canCopyID;

		private bool _showLevel;

		private bool _isSelected;

		private bool _hasNotification;

		private bool _isFriendRequest;

		private bool _isPendingRequest;

		private bool _canRemove;

		private bool _canBeInvited;

		private bool _canInviteToParty;

		private bool _canInviteToClan;

		private bool _isSigilChangeInformationEnabled;

		private bool _isRankInfoLoading;

		private bool _isRankInfoCasual;

		private bool _isClanInfoSupported;

		private bool _isBannerlordIDSupported;

		private int _level;

		private int _rating;

		private int _loot;

		private int _experienceRatio;

		private int _ratingRatio;

		private string _name = "";

		private string _stateText;

		private string _levelText;

		private string _levelTitleText;

		private string _ratingText;

		private string _ratingID;

		private string _clanName;

		private string _clanTag;

		private string _changeText;

		private string _clanInfoTitleText;

		private string _badgeInfoTitleText;

		private string _avatarInfoTitleText;

		private string _experienceText;

		private string _rankText;

		private string _bannerlordID;

		private string _selectedBadgeID;

		private HintViewModel _nameHint;

		private HintViewModel _inviteToPartyHint;

		private HintViewModel _removeFriendHint;

		private HintViewModel _acceptFriendRequestHint;

		private HintViewModel _declineFriendRequestHint;

		private HintViewModel _cancelFriendRequestHint;

		private HintViewModel _inviteToClanHint;

		private HintViewModel _changeBannerlordIDHint;

		private HintViewModel _copyBannerlordIDHint;

		private HintViewModel _addFriendWithBannerlordIDHint;

		private HintViewModel _experienceHint;

		private HintViewModel _ratingHint;

		private HintViewModel _lootHint;

		private HintViewModel _skirmishRatingHint;

		private HintViewModel _captainRatingHint;

		private HintViewModel _clanLeaderboardHint;

		private ImageIdentifierVM _avatar;

		private ImageIdentifierVM _clanBanner;

		private MPLobbySigilItemVM _sigil;

		private MPLobbyBadgeItemVM _shownBadge;

		private CharacterViewModel _characterVisual;

		private MBBindingList<MPLobbyPlayerStatItemVM> _displayedStats;

		private MBBindingList<MPLobbyGameTypeVM> _gameTypes;

		public enum OnlineStatus
		{
			None,
			InGame,
			Online,
			Offline
		}
	}
}
