using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.MountAndBlade.ViewModelCollection.GameOptions;
using TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.AfterBattle;
using TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.Armory;
using TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.Authentication;
using TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.Clan;
using TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.CustomGame;
using TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.Friends;
using TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.Home;
using TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.OfficialGame;
using TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.Profile;
using TaleWorlds.PlatformService;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby
{
	// Token: 0x02000063 RID: 99
	public class MPLobbyVM : ViewModel
	{
		// Token: 0x1700028C RID: 652
		// (get) Token: 0x06000846 RID: 2118 RVA: 0x0001F2F4 File Offset: 0x0001D4F4
		private PlayerId _partyLeaderId
		{
			get
			{
				return NetworkMain.GameClient.PlayersInParty.Find((PartyPlayerInLobbyClient p) => p.IsPartyLeader).PlayerId;
			}
		}

		// Token: 0x1700028D RID: 653
		// (get) Token: 0x06000847 RID: 2119 RVA: 0x0001F329 File Offset: 0x0001D529
		// (set) Token: 0x06000848 RID: 2120 RVA: 0x0001F331 File Offset: 0x0001D531
		public MPLobbyVM.LobbyPage CurrentPage { get; private set; }

		// Token: 0x1700028E RID: 654
		// (get) Token: 0x06000849 RID: 2121 RVA: 0x0001F33A File Offset: 0x0001D53A
		// (set) Token: 0x0600084A RID: 2122 RVA: 0x0001F342 File Offset: 0x0001D542
		public List<MPLobbyVM.LobbyPage> DisallowedPages { get; private set; }

		// Token: 0x0600084B RID: 2123 RVA: 0x0001F34C File Offset: 0x0001D54C
		public MPLobbyVM(LobbyState lobbyState, Action<BasicCharacterObject> onOpenFacegen, Action onForceCloseFacegen, Action<KeyOptionVM> onKeybindRequest, Func<string> getContinueKeyText, Action<bool> setNavigationRestriction)
		{
			this.CurrentPage = MPLobbyVM.LobbyPage.NotAssigned;
			this._onKeybindRequest = onKeybindRequest;
			this._onForceCloseFacegen = onForceCloseFacegen;
			this._setNavigationRestriction = setNavigationRestriction;
			this._lobbyState = lobbyState;
			this._lobbyClient = this._lobbyState.LobbyClient;
			this.RefreshPlayerCallbacks();
			this._partySuggestionQueue = new MBQueue<MPLobbyPartyPlayerSuggestionPopupVM.PlayerPartySuggestionData>();
			this._partyActionQueue = new ConcurrentQueue<ValueTuple<MPLobbyVM.PartyActionType, PlayerId, PartyRemoveReason>>();
			this.BlockerState = new MPLobbyBlockerStateVM(this._setNavigationRestriction);
			this.Menu = new MPLobbyMenuVM(lobbyState, this._setNavigationRestriction, new Func<Task>(this.RequestExit));
			bool isAbleToSearchForGame = NetworkMain.GameClient.IsAbleToSearchForGame;
			this.IsMatchmakingEnabled = !isAbleToSearchForGame;
			this.IsMatchmakingEnabled = isAbleToSearchForGame;
			this.IsCustomGameFindEnabled = NetworkMain.GameClient.IsCustomBattleAvailable;
			this.IsPartyLeader = NetworkMain.GameClient.IsPartyLeader;
			this.IsInParty = NetworkMain.GameClient.IsInParty;
			this.Login = new MPAuthenticationVM(lobbyState);
			this.Rejoin = new MPLobbyRejoinVM(new Action<MPLobbyVM.LobbyPage>(this.OnChangePageRequest));
			this.Home = new MPLobbyHomeVM(lobbyState.NewsManager, new Action<MPLobbyVM.LobbyPage>(this.OnChangePageRequest));
			this.Profile = new MPLobbyProfileVM(lobbyState, new Action<MPLobbyVM.LobbyPage>(this.OnChangePageRequest), new Action(this.ExecuteOpenRecentGames));
			this.Matchmaking = new MPMatchmakingVM(lobbyState, new Action<MPLobbyVM.LobbyPage>(this.OnChangePageRequest), new Action<string, bool>(this.OnMatchSelectionChanged), new Action<bool>(this.OnGameFindStateChanged));
			this.Armory = new MPArmoryVM(onOpenFacegen, new Action<MPArmoryCosmeticItemVM>(this.OnItemObtainRequested));
			this.Friends = new MPLobbyFriendsVM();
			this.GameSearch = new MPLobbyGameSearchVM();
			this.PlayerProfile = new MPLobbyPlayerProfileVM(lobbyState);
			this.AfterBattlePopup = new MPAfterBattlePopupVM(getContinueKeyText);
			this.PartyInvitationPopup = new MPLobbyPartyInvitationPopupVM();
			this.PartyPlayerSuggestionPopup = new MPLobbyPartyPlayerSuggestionPopupVM();
			this.Popup = new MPLobbyPopupVM();
			this.Options = new MPOptionsVM(false, new Action(this.ExecuteShowBrightness), new Action(this.ExecuteShowExposure), this._onKeybindRequest);
			this.BrightnessPopup = new BrightnessOptionVM(null);
			this.ExposurePopup = new ExposureOptionVM(null);
			this.BannerlordIDChangePopup = new MPLobbyBannerlordIDChangePopup();
			this.BannerlordIDAddFriendPopup = new MPLobbyBannerlordIDAddFriendPopupVM();
			this.Clan = new MPLobbyClanVM(new Action(this.OpenInviteClanMemberPopup));
			this.ClanCreationPopup = new MPLobbyClanCreationPopupVM();
			this.ClanCreationInformationPopup = new MPLobbyClanCreationInformationVM(new Action(this.OpenClanCreationPopup));
			this.ClanInvitationPopup = new MPLobbyClanInvitationPopupVM();
			this.ClanMatchmakingRequestPopup = new MPLobbyClanMatchmakingRequestPopupVM();
			this.ClanInviteFriendsPopup = new MPLobbyClanInviteFriendsPopupVM(new Func<MBBindingList<MPLobbyPlayerBaseVM>>(this.Friends.GetAllFriends));
			this.ClanLeaderboardPopup = new MPLobbyClanLeaderboardVM();
			this.CosmeticObtainPopup = new MPCosmeticObtainPopupVM(new Action<string, int>(this.OnItemObtained), getContinueKeyText);
			this.ChangeSigilPopup = new MPLobbyHomeChangeSigilPopupVM(new Action<MPLobbyCosmeticSigilItemVM>(this.OnItemObtainRequested));
			this.BadgeProgressionInformation = new MPLobbyBadgeProgressInformationVM(getContinueKeyText);
			this.BadgeSelectionPopup = new MPLobbyBadgeSelectionPopupVM(new Action(this.OnBadgeNotificationRead), new Action(this.OnBadgeSelectionUpdated), new Action<MPLobbyAchievementBadgeGroupVM>(this.OnBadgeProgressInfoRequested));
			this.RecentGames = new MPLobbyRecentGamesVM();
			this.RankProgressInformation = new MPLobbyRankProgressInformationVM(getContinueKeyText);
			this.RankLeaderboard = new MPLobbyRankLeaderboardVM(lobbyState);
			this.Home.OnFindGameRequested += this.AutoFindGameRequested;
			this.Profile.OnFindGameRequested += this.AutoFindGameRequested;
			MPLobbyRejoinVM rejoin = this.Rejoin;
			rejoin.OnRejoinRequested = (Action)Delegate.Combine(rejoin.OnRejoinRequested, new Action(this.OnRejoinRequested));
			if (this._lobbyClient.LoggedIn)
			{
				this.SetPage(MPLobbyVM.LobbyPage.Home, MPMatchmakingVM.MatchmakingSubPages.Default);
			}
			this.DisallowedPages = new List<MPLobbyVM.LobbyPage>();
			InformationManager.ClearAllMessages();
			this._lobbyState.RegisterForCustomServerAction(new Func<GameServerEntry, List<CustomServerAction>>(this.OnServerActionRequested));
			LobbyState lobbyState2 = this._lobbyState;
			lobbyState2.OnUserGeneratedContentPrivilegeUpdated = (Action<bool>)Delegate.Combine(lobbyState2.OnUserGeneratedContentPrivilegeUpdated, new Action<bool>(this.OnUserGeneratedContentPrivilegeUpdated));
			this.RefreshValues();
		}

		// Token: 0x0600084C RID: 2124 RVA: 0x0001F744 File Offset: 0x0001D944
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.BlockerState.RefreshValues();
			this.Login.RefreshValues();
			this.Rejoin.RefreshValues();
			this.Menu.RefreshValues();
			this.Home.RefreshValues();
			this.Matchmaking.RefreshValues();
			this.Armory.RefreshValues();
			this.Profile.RefreshValues();
			this.Friends.RefreshValues();
			this.GameSearch.RefreshValues();
			this.PlayerProfile.RefreshValues();
			this.Options.RefreshValues();
			this.Popup.RefreshValues();
			this.AfterBattlePopup.RefreshValues();
			this.PartyInvitationPopup.RefreshValues();
			this.PartyPlayerSuggestionPopup.RefreshValues();
			this.Clan.RefreshValues();
			this.ClanCreationPopup.RefreshValues();
			this.ClanCreationInformationPopup.RefreshValues();
			this.ClanInvitationPopup.RefreshValues();
			this.ClanMatchmakingRequestPopup.RefreshValues();
			this.ClanInviteFriendsPopup.RefreshValues();
			this.ClanLeaderboardPopup.RefreshValues();
			this.BannerlordIDChangePopup.RefreshValues();
			this.BannerlordIDAddFriendPopup.RefreshValues();
			this.CosmeticObtainPopup.RefreshValues();
			this.ChangeSigilPopup.RefreshValues();
			this.BadgeProgressionInformation.RefreshValues();
			this.BadgeSelectionPopup.RefreshValues();
			this.RecentGames.RefreshValues();
			this.RankProgressInformation.RefreshValues();
			this.RankLeaderboard.RefreshValues();
			this.BrightnessPopup.RefreshValues();
			this.ExposurePopup.RefreshValues();
		}

		// Token: 0x0600084D RID: 2125 RVA: 0x0001F8D0 File Offset: 0x0001DAD0
		private void OnUserGeneratedContentPrivilegeUpdated(bool hasPrivilege)
		{
			bool? cachedHasUserGeneratedContentPrivilege = this._cachedHasUserGeneratedContentPrivilege;
			if (!((cachedHasUserGeneratedContentPrivilege.GetValueOrDefault() == hasPrivilege) & (cachedHasUserGeneratedContentPrivilege != null)))
			{
				this.OnFriendListUpdated(true);
			}
		}

		// Token: 0x0600084E RID: 2126 RVA: 0x0001F904 File Offset: 0x0001DB04
		private List<CustomServerAction> OnServerActionRequested(GameServerEntry arg)
		{
			GameServerEntry arg2 = arg;
			if (arg2 != null && !arg2.IsOfficial)
			{
				return new List<CustomServerAction>
				{
					new CustomServerAction(delegate
					{
						this.OnShowPlayerProfile(arg.HostId);
					}, arg, GameTexts.FindText("str_mp_scoreboard_context_viewprofile", null).ToString())
				};
			}
			return null;
		}

		// Token: 0x0600084F RID: 2127 RVA: 0x0001F970 File Offset: 0x0001DB70
		public void CreateInputKeyVisuals(HotKey cancelInputKey, HotKey doneInputKey)
		{
			this.BannerlordIDAddFriendPopup.SetCancelInputKey(cancelInputKey);
			this.BannerlordIDAddFriendPopup.SetDoneInputKey(doneInputKey);
			this.BannerlordIDChangePopup.SetCancelInputKey(cancelInputKey);
			this.BannerlordIDChangePopup.SetDoneInputKey(doneInputKey);
			this.RankLeaderboard.SetCancelInputKey(cancelInputKey);
			this.ChangeSigilPopup.SetCancelInputKey(cancelInputKey);
			this.ChangeSigilPopup.SetDoneInputKey(doneInputKey);
			this.ClanCreationPopup.SetCancelInputKey(cancelInputKey);
			this.Clan.ClanOverview.ChangeSigilPopup.SetCancelInputKey(cancelInputKey);
			this.Clan.ClanOverview.ChangeSigilPopup.SetDoneInputKey(doneInputKey);
			this.Clan.ClanOverview.ChangeFactionPopup.SetCancelInputKey(cancelInputKey);
			this.Clan.ClanOverview.ChangeFactionPopup.SetDoneInputKey(doneInputKey);
			this.Clan.ClanOverview.SendAnnouncementPopup.SetCancelInputKey(cancelInputKey);
			this.Clan.ClanOverview.SendAnnouncementPopup.SetDoneInputKey(doneInputKey);
			this.Clan.ClanOverview.SetClanInformationPopup.SetCancelInputKey(doneInputKey);
			this.Clan.ClanOverview.SetClanInformationPopup.SetDoneInputKey(doneInputKey);
			this.BadgeSelectionPopup.SetCancelInputKey(cancelInputKey);
		}

		// Token: 0x06000850 RID: 2128 RVA: 0x0001FA9C File Offset: 0x0001DC9C
		public override void OnFinalize()
		{
			this.Clan.ClanOverview.ChangeSigilPopup.OnFinalize();
			this.Clan.ClanOverview.ChangeFactionPopup.OnFinalize();
			this.Clan.ClanOverview.SendAnnouncementPopup.OnFinalize();
			this.BannerlordIDAddFriendPopup.OnFinalize();
			this.BannerlordIDChangePopup.OnFinalize();
			this.RankLeaderboard.OnFinalize();
			this.ChangeSigilPopup.OnFinalize();
			this.ClanCreationPopup.OnFinalize();
			this.BadgeSelectionPopup.OnFinalize();
			InformationManager.ClearAllMessages();
			this.Login.OnFinalize();
			this.Armory.OnFinalize();
			this.Matchmaking.OnFinalize();
			this.Friends.OnFinalize();
			this.Home.OnFindGameRequested -= this.AutoFindGameRequested;
			this.Profile.OnFindGameRequested -= this.AutoFindGameRequested;
			this._lobbyState.UnregisterForCustomServerAction(new Func<GameServerEntry, List<CustomServerAction>>(this.OnServerActionRequested));
			LobbyState lobbyState = this._lobbyState;
			lobbyState.OnUserGeneratedContentPrivilegeUpdated = (Action<bool>)Delegate.Remove(lobbyState.OnUserGeneratedContentPrivilegeUpdated, new Action<bool>(this.OnUserGeneratedContentPrivilegeUpdated));
			MPLobbyRejoinVM rejoin = this.Rejoin;
			rejoin.OnRejoinRequested = (Action)Delegate.Remove(rejoin.OnRejoinRequested, new Action(this.OnRejoinRequested));
			this.Menu.OnFinalize();
			this.Home.OnFinalize();
			this.FinalizePlayerCallbacks();
			this._lobbyState = null;
			base.OnFinalize();
		}

		// Token: 0x06000851 RID: 2129 RVA: 0x0001FC18 File Offset: 0x0001DE18
		private void RefreshPlayerCallbacks()
		{
			MPCustomGameItemVM.OnPlayerProfileRequested = new Action<PlayerId>(this.OnShowPlayerProfile);
			MPLobbyPlayerBaseVM.OnPlayerProfileRequested = new Action<PlayerId>(this.OnShowPlayerProfile);
			MPLobbyPlayerBaseVM.OnBannerlordIDChangeRequested = new Action<PlayerId>(this.OnBannerlordIDChangeRequested);
			MPLobbyPlayerBaseVM.OnAddFriendWithBannerlordIDRequested = new Action<PlayerId>(this.OnAddFriendWithBannerlordIDRequested);
			MPLobbyPlayerBaseVM.OnSigilChangeRequested = new Action<PlayerId>(this.OnSigilChangeRequested);
			MPLobbyPlayerBaseVM.OnBadgeChangeRequested = new Action<PlayerId>(this.OnBadgeChangeRequested);
			MPLobbyPlayerBaseVM.OnRankProgressionRequested = new Action<MPLobbyPlayerBaseVM>(this.OnRankProgressionRequested);
			MPLobbyPlayerBaseVM.OnRankLeaderboardRequested = new Action<string>(this.OnRankLeaderboardRequested);
			MPLobbyPlayerBaseVM.OnClanPageRequested = new Action(this.OnClanPageRequested);
			MPLobbyPlayerBaseVM.OnClanLeaderboardRequested = new Action(this.OnClanLeaderboardRequested);
		}

		// Token: 0x06000852 RID: 2130 RVA: 0x0001FCCF File Offset: 0x0001DECF
		private void FinalizePlayerCallbacks()
		{
			MPCustomGameItemVM.OnPlayerProfileRequested = null;
			MPLobbyPlayerBaseVM.OnPlayerProfileRequested = null;
			MPLobbyPlayerBaseVM.OnBannerlordIDChangeRequested = null;
			MPLobbyPlayerBaseVM.OnAddFriendWithBannerlordIDRequested = null;
			MPLobbyPlayerBaseVM.OnSigilChangeRequested = null;
			MPLobbyPlayerBaseVM.OnBadgeChangeRequested = null;
			MPLobbyPlayerBaseVM.OnRankProgressionRequested = null;
			MPLobbyPlayerBaseVM.OnRankLeaderboardRequested = null;
			MPLobbyPlayerBaseVM.OnClanPageRequested = null;
			MPLobbyPlayerBaseVM.OnClanLeaderboardRequested = null;
		}

		// Token: 0x06000853 RID: 2131 RVA: 0x0001FD10 File Offset: 0x0001DF10
		public void OnTick(float dt)
		{
			this.IsLoggedIn = NetworkMain.GameClient.LoggedIn;
			this.Login.OnTick(dt);
			this.Friends.OnTick(dt);
			this.UpdateBlockerState();
			if (this.IsSearchingGame)
			{
				this._playerCountInQueueTimer += dt;
				if (this._playerCountInQueueTimer >= 5f)
				{
					this.UpdatePlayerCountInQueue();
					this._playerCountInQueueTimer = 0f;
				}
			}
			else
			{
				this._playerCountInQueueTimer = 5f;
			}
			if (!this.PartyPlayerSuggestionPopup.IsEnabled && !this._partySuggestionQueue.IsEmpty<MPLobbyPartyPlayerSuggestionPopupVM.PlayerPartySuggestionData>() && !this._lobbyClient.IsPartyFull)
			{
				MPLobbyPartyPlayerSuggestionPopupVM.PlayerPartySuggestionData playerPartySuggestionData = this._partySuggestionQueue.Dequeue();
				this.PartyPlayerSuggestionPopup.OpenWith(playerPartySuggestionData);
			}
			else if (!this._partySuggestionQueue.IsEmpty<MPLobbyPartyPlayerSuggestionPopupVM.PlayerPartySuggestionData>() && this._lobbyClient.IsPartyFull)
			{
				this._partySuggestionQueue.Clear();
			}
			ValueTuple<MPLobbyVM.PartyActionType, PlayerId, PartyRemoveReason> valueTuple;
			if (this._partyActionQueue.TryDequeue(out valueTuple))
			{
				MPLobbyVM.PartyActionType item = valueTuple.Item1;
				PlayerId item2 = valueTuple.Item2;
				PartyRemoveReason item3 = valueTuple.Item3;
				switch (item)
				{
				case MPLobbyVM.PartyActionType.Add:
					this.HandlePlayerAddedToParty(item2);
					break;
				case MPLobbyVM.PartyActionType.Remove:
					this.HandlePlayerRemovedFromParty(item2, item3);
					break;
				case MPLobbyVM.PartyActionType.AssignLeader:
					this.HandlePlayerAssignedPartyLeader(item2);
					break;
				}
			}
			this.Matchmaking.OnTick(dt);
			this.GameSearch.OnTick(dt);
		}

		// Token: 0x06000854 RID: 2132 RVA: 0x0001FE60 File Offset: 0x0001E060
		public void OnConfirm()
		{
			if (this.ExposurePopup.Visible)
			{
				this.ExposurePopup.ExecuteConfirm();
				return;
			}
			if (this.BrightnessPopup.Visible)
			{
				this.BrightnessPopup.ExecuteConfirm();
				return;
			}
			if (this.ChangeSigilPopup.IsEnabled && !this.CosmeticObtainPopup.IsEnabled)
			{
				this.ChangeSigilPopup.ExecuteChangeSigil();
				return;
			}
			if (this.BannerlordIDAddFriendPopup.IsSelected)
			{
				this.BannerlordIDAddFriendPopup.ExecuteTryAddFriend();
				return;
			}
			if (this.BannerlordIDChangePopup.IsSelected)
			{
				this.BannerlordIDChangePopup.ExecuteApply();
				return;
			}
			if (this.Clan.ClanOverview.ChangeSigilPopup.IsSelected)
			{
				this.Clan.ClanOverview.ChangeSigilPopup.ExecuteChangeSigil();
				return;
			}
			if (this.Clan.ClanOverview.ChangeFactionPopup.IsSelected)
			{
				this.Clan.ClanOverview.ChangeFactionPopup.ExecuteChangeFaction();
				return;
			}
			if (this.Clan.ClanOverview.SetClanInformationPopup.IsSelected)
			{
				this.Clan.ClanOverview.SetClanInformationPopup.ExecuteSend();
				return;
			}
			if (this.Clan.ClanOverview.SendAnnouncementPopup.IsSelected)
			{
				this.Clan.ClanOverview.SendAnnouncementPopup.ExecuteSend();
			}
		}

		// Token: 0x06000855 RID: 2133 RVA: 0x0001FFAC File Offset: 0x0001E1AC
		public async void OnEscape()
		{
			if (this.CurrentPage == MPLobbyVM.LobbyPage.Authentication)
			{
				if (this.HasNoPopupOpen())
				{
					this.Login.ExecuteExit();
				}
				else
				{
					this.ForceClosePopups();
				}
			}
			else if (this.ExposurePopup.Visible)
			{
				this.ExposurePopup.ExecuteCancel();
			}
			else if (this.BrightnessPopup.Visible)
			{
				this.BrightnessPopup.ExecuteCancel();
			}
			else if (this.ClanCreationPopup.IsEnabled)
			{
				this.ClanCreationPopup.ExecuteClosePopup();
			}
			else if (this.CosmeticObtainPopup.IsEnabled)
			{
				this.CosmeticObtainPopup.ExecuteClosePopup();
			}
			else if (this.Options.IsOptionsChanged())
			{
				this.ShowOptionsChangedInquiry();
			}
			else if (this.Friends.IsPlayerActionsActive)
			{
				this.Friends.IsPlayerActionsActive = false;
			}
			else if (this.RankLeaderboard.IsPlayerActionsActive)
			{
				this.RankLeaderboard.IsPlayerActionsActive = false;
			}
			else if (this.RecentGames.IsPlayerActionsActive)
			{
				this.RecentGames.IsPlayerActionsActive = false;
			}
			else if (NetworkMain.GameClient.IsRefreshingPlayerData)
			{
				InformationManager.ShowInquiry(new InquiryData(GameTexts.FindText("str_exit", null).ToString(), new TextObject("{=usLhlY2j}Please wait until player data is downloaded.", null).ToString(), true, false, GameTexts.FindText("str_ok", null).ToString(), null, null, null, "", 0f, null, null, null), false, false);
			}
			else if (this.HasAnyContextMenuOpen())
			{
				this.ForceCloseContextMenus();
			}
			else if (this.HasNoPopupOpen())
			{
				await this.RequestExit();
			}
			else
			{
				this.ForceClosePopups();
			}
		}

		// Token: 0x06000856 RID: 2134 RVA: 0x0001FFE5 File Offset: 0x0001E1E5
		public bool HasAnyContextMenuOpen()
		{
			return this.Clan.ClanRoster.IsMemberActionsActive || this.Friends.IsPlayerActionsActive || this.RankLeaderboard.IsPlayerActionsActive || this.RecentGames.IsPlayerActionsActive;
		}

		// Token: 0x06000857 RID: 2135 RVA: 0x00020020 File Offset: 0x0001E220
		public void ForceCloseContextMenus()
		{
			this.Clan.ClanRoster.IsMemberActionsActive = false;
			this.Friends.IsPlayerActionsActive = false;
			this.RankLeaderboard.IsPlayerActionsActive = false;
			this.RecentGames.IsPlayerActionsActive = false;
		}

		// Token: 0x06000858 RID: 2136 RVA: 0x00020058 File Offset: 0x0001E258
		public bool HasNoPopupOpen()
		{
			return !this.Clan.IsEnabled && !this.Clan.ClanOverview.ChangeFactionPopup.IsSelected && !this.Clan.ClanOverview.ChangeSigilPopup.IsSelected && !this.Clan.ClanOverview.SendAnnouncementPopup.IsSelected && !this.Clan.ClanOverview.SetClanInformationPopup.IsSelected && !this.PartyInvitationPopup.IsEnabled && !this.PartyPlayerSuggestionPopup.IsEnabled && !this.ClanInvitationPopup.IsEnabled && !this.ClanMatchmakingRequestPopup.IsEnabled && !this.BannerlordIDChangePopup.IsSelected && !this.BannerlordIDAddFriendPopup.IsSelected && !this.CosmeticObtainPopup.IsEnabled && !this.AfterBattlePopup.IsEnabled && !this.ClanCreationPopup.IsEnabled && !this.ClanCreationInformationPopup.IsEnabled && !this.ClanInviteFriendsPopup.IsEnabled && !this.ClanLeaderboardPopup.IsEnabled && !this.BadgeProgressionInformation.IsEnabled && !this.BadgeSelectionPopup.IsEnabled && !this.ChangeSigilPopup.IsEnabled && !this.RecentGames.IsEnabled && !this.PlayerProfile.IsEnabled && !this.RankProgressInformation.IsEnabled && !this.RankLeaderboard.IsEnabled && !this.ExposurePopup.Visible && !this.BrightnessPopup.Visible;
		}

		// Token: 0x06000859 RID: 2137 RVA: 0x00020214 File Offset: 0x0001E414
		private void ForceClosePopups()
		{
			this.Clan.ExecuteClosePopup();
			this.Clan.ClanOverview.ChangeFactionPopup.ExecuteClosePopup();
			this.Clan.ClanOverview.ChangeSigilPopup.ExecuteClosePopup();
			this.Clan.ClanOverview.SendAnnouncementPopup.ExecuteClosePopup();
			this.Clan.ClanOverview.SetClanInformationPopup.ExecuteClosePopup();
			this.PartyInvitationPopup.Close();
			this.PartyPlayerSuggestionPopup.Close();
			this.ClanInvitationPopup.Close();
			this.ClanMatchmakingRequestPopup.Close();
			this.BannerlordIDChangePopup.ExecuteClosePopup();
			this.BannerlordIDAddFriendPopup.ExecuteClosePopup();
			this.CosmeticObtainPopup.ExecuteClosePopup();
			this.AfterBattlePopup.ExecuteClose();
			this.ClanCreationPopup.ExecuteClosePopup();
			this.ClanCreationInformationPopup.ExecuteClosePopup();
			this.ClanInviteFriendsPopup.ExecuteClosePopup();
			this.ClanLeaderboardPopup.ExecuteClosePopup();
			this.BadgeProgressionInformation.ExecuteClosePopup();
			this.BadgeSelectionPopup.ExecuteClosePopup();
			this.ChangeSigilPopup.ExecuteClosePopup();
			this.RecentGames.ExecuteClosePopup();
			this.PlayerProfile.ExecuteClosePopup();
			this.RankProgressInformation.ExecuteClosePopup();
			this.RankLeaderboard.ExecuteClosePopup();
			this.ExposurePopup.ExecuteCancel();
			this.BrightnessPopup.ExecuteCancel();
		}

		// Token: 0x0600085A RID: 2138 RVA: 0x00020368 File Offset: 0x0001E568
		public async Task RequestExit()
		{
			if (NetworkMain.GameClient.CurrentState != LobbyClient.State.WaitingToJoinCustomGame && NetworkMain.GameClient.CurrentState != LobbyClient.State.WaitingToJoinPremadeGame && NetworkMain.GameClient.CurrentState != LobbyClient.State.Connected && NetworkMain.GameClient.CurrentState != LobbyClient.State.SessionRequested && NetworkMain.GameClient.CurrentState != LobbyClient.State.Working)
			{
				while (NetworkMain.GameClient.CurrentState != LobbyClient.State.AtLobby)
				{
					if (NetworkMain.GameClient.CurrentState == LobbyClient.State.SearchingBattle)
					{
						NetworkMain.GameClient.CancelFindGame();
					}
					await Task.Yield();
				}
				if (!this._isDisconnecting)
				{
					if (Input.IsGamepadActive)
					{
						InformationManager.ShowInquiry(new InquiryData(new TextObject("{=3CsACce8}Exit", null).ToString(), new TextObject("{=NMh61YLB}Are you sure you want to exit?", null).ToString(), true, true, new TextObject("{=aeouhelq}Yes", null).ToString(), new TextObject("{=8OkPHu4f}No", null).ToString(), new Action(this.OnExit), null, "", 0f, null, null, null), false, false);
					}
					else
					{
						this.OnExit();
					}
				}
			}
		}

		// Token: 0x0600085B RID: 2139 RVA: 0x000203AD File Offset: 0x0001E5AD
		private void OnExit()
		{
			this._isDisconnecting = true;
			Action<bool> setNavigationRestriction = this._setNavigationRestriction;
			if (setNavigationRestriction != null)
			{
				setNavigationRestriction(true);
			}
			NetworkMain.GameClient.Logout(null);
			Task.Run(delegate
			{
				while (!NetworkMain.GameClient.IsIdle)
				{
				}
				this._isDisconnecting = false;
			});
		}

		// Token: 0x0600085C RID: 2140 RVA: 0x000203E8 File Offset: 0x0001E5E8
		private async void UpdatePlayerCountInQueue()
		{
			LobbyClient gameClient = NetworkMain.GameClient;
			if (gameClient.Connected && this.GameSearch.CustomGameMode != MPCustomGameVM.CustomGameMode.PremadeGame)
			{
				MatchmakingWaitTimeStats matchmakingWaitTimeStats = await gameClient.GetMatchmakingWaitTimes();
				(from t in this.Matchmaking.QuickplayGameTypes
					where t.IsSelected
					select t.Type).ToArray<string>();
				this.GameSearch.UpdateData(matchmakingWaitTimeStats, null);
			}
		}

		// Token: 0x0600085D RID: 2141 RVA: 0x00020421 File Offset: 0x0001E621
		public void ConnectionStateUpdated(bool isAuthenticated)
		{
			if (isAuthenticated)
			{
				this.RefreshRecentGames();
				this.Friends.OnStateActivate();
			}
			else
			{
				this.PartyInvitationPopup.Close();
				this.ClanInvitationPopup.Close();
			}
			this.OnSearchBattleCanceled();
		}

		// Token: 0x0600085E RID: 2142 RVA: 0x00020455 File Offset: 0x0001E655
		private void ShowOptionsChangedInquiry()
		{
			this.Popup.ShowInquiry(new TextObject("{=Rov73lC3}Unsaved Changes", null), new TextObject("{=u0HNU5pA}You have unsaved changes. Do you want to apply them before you continue?", null), delegate
			{
				this.Options.ExecuteApply();
			}, delegate
			{
				this.Options.ForceCancel();
			});
		}

		// Token: 0x0600085F RID: 2143 RVA: 0x00020490 File Offset: 0x0001E690
		public void ShowCustomOptionsChangedInquiry(Action onAccept, Action onDecline)
		{
			this.Popup.ShowInquiry(new TextObject("{=Rov73lC3}Unsaved Changes", null), new TextObject("{=u0HNU5pA}You have unsaved changes. Do you want to apply them before you continue?", null), delegate
			{
				this.Options.ExecuteApply();
				Action onAccept2 = onAccept;
				if (onAccept2 == null)
				{
					return;
				}
				onAccept2();
			}, delegate
			{
				this.Options.ForceCancel();
				Action onDecline2 = onDecline;
				if (onDecline2 == null)
				{
					return;
				}
				onDecline2();
			});
		}

		// Token: 0x06000860 RID: 2144 RVA: 0x000204F4 File Offset: 0x0001E6F4
		public void SetPage(MPLobbyVM.LobbyPage lobbyPage, MPMatchmakingVM.MatchmakingSubPages matchmakingSubPage = MPMatchmakingVM.MatchmakingSubPages.Default)
		{
			if (this.CurrentPage != lobbyPage)
			{
				if (lobbyPage != MPLobbyVM.LobbyPage.Authentication && this.CurrentPage == MPLobbyVM.LobbyPage.Options && this.Options.IsOptionsChanged())
				{
					this.ShowOptionsChangedInquiry();
				}
				bool flag = false;
				bool flag2 = false;
				bool flag3 = false;
				bool flag4 = false;
				bool flag5 = false;
				bool flag6 = false;
				bool flag7 = false;
				bool flag8 = false;
				bool flag9 = false;
				switch (lobbyPage)
				{
				case MPLobbyVM.LobbyPage.Authentication:
					flag2 = true;
					break;
				case MPLobbyVM.LobbyPage.Rejoin:
					flag3 = true;
					break;
				case MPLobbyVM.LobbyPage.Options:
					flag = true;
					flag7 = true;
					break;
				case MPLobbyVM.LobbyPage.Home:
					flag = true;
					flag8 = true;
					flag4 = true;
					break;
				case MPLobbyVM.LobbyPage.Armory:
					flag = true;
					flag8 = true;
					flag6 = true;
					break;
				case MPLobbyVM.LobbyPage.Matchmaking:
					flag = true;
					flag8 = true;
					flag5 = true;
					this.Matchmaking.TrySetMatchmakingSubPage(matchmakingSubPage);
					break;
				case MPLobbyVM.LobbyPage.Profile:
					flag = true;
					flag9 = true;
					flag8 = true;
					break;
				}
				this.Menu.IsEnabled = flag;
				this.Login.IsEnabled = flag2;
				this.Rejoin.IsEnabled = flag3;
				this.Home.IsEnabled = flag4;
				this.Matchmaking.IsEnabled = flag5;
				this.Armory.IsEnabled = flag6;
				this.Options.IsEnabled = flag7;
				this.Friends.IsEnabled = flag8;
				this.Profile.IsEnabled = flag9;
				this.IsArmoryActive = this.Armory.IsEnabled;
				if (this.CurrentPage == MPLobbyVM.LobbyPage.Matchmaking)
				{
					this.Matchmaking.TrySetMatchmakingSubPage(MPMatchmakingVM.MatchmakingSubPages.QuickPlay);
				}
				this.CurrentPage = lobbyPage;
				this._setNavigationRestriction(this.CurrentPage == MPLobbyVM.LobbyPage.Authentication);
				this.Menu.SetPage(lobbyPage);
				if (lobbyPage != MPLobbyVM.LobbyPage.Profile)
				{
					this.Menu.HasProfileNotification = this.BadgeSelectionPopup.HasNotifications;
					return;
				}
				this.Menu.HasProfileNotification = false;
			}
		}

		// Token: 0x06000861 RID: 2145 RVA: 0x00020690 File Offset: 0x0001E890
		private async void RefreshRecentGames()
		{
			MBReadOnlyList<MatchInfo> mbreadOnlyList = await MatchHistory.GetMatches();
			if (mbreadOnlyList != null)
			{
				this.Profile.RefreshRecentGames(mbreadOnlyList);
				this.RecentGames.RefreshData(mbreadOnlyList);
			}
		}

		// Token: 0x06000862 RID: 2146 RVA: 0x000206C9 File Offset: 0x0001E8C9
		public void OnDisconnected()
		{
			if (this.CurrentPage != MPLobbyVM.LobbyPage.Authentication)
			{
				this.SetPage(MPLobbyVM.LobbyPage.Authentication, MPMatchmakingVM.MatchmakingSubPages.Default);
			}
			this.Friends.UpdateCanInviteOtherPlayersToParty();
			this.IsPartyLeader = false;
			this.ForceClosePopups();
		}

		// Token: 0x06000863 RID: 2147 RVA: 0x000206F4 File Offset: 0x0001E8F4
		private void AutoFindGameRequested()
		{
			this.Matchmaking.ExecuteAutoFindGame();
		}

		// Token: 0x06000864 RID: 2148 RVA: 0x00020704 File Offset: 0x0001E904
		public void OnServerStatusReceived(ServerStatus serverStatus)
		{
			this.Matchmaking.OnServerStatusReceived(serverStatus);
			this.IsMatchmakingEnabled = NetworkMain.GameClient.IsAbleToSearchForGame;
			this.IsCustomGameFindEnabled = NetworkMain.GameClient.IsCustomBattleAvailable;
			if (!NetworkMain.GameClient.IsAbleToSearchForGame && this.CurrentPage == MPLobbyVM.LobbyPage.Matchmaking && this.Matchmaking.CurrentSubPage == MPMatchmakingVM.MatchmakingSubPages.QuickPlay)
			{
				this.SetPage(MPLobbyVM.LobbyPage.Home, MPMatchmakingVM.MatchmakingSubPages.Default);
				InformationManager.ShowInquiry(new InquiryData(new TextObject("{=aO3avkK9}Matchmaking Disabled", null).ToString(), new TextObject("{=5baU17n0}Matchmaking feature is currently disabled", null).ToString(), true, false, new TextObject("{=yS7PvrTD}OK", null).ToString(), null, null, null, "", 0f, null, null, null), false, false);
			}
			if (!serverStatus.IsCustomBattleEnabled && this.CurrentPage == MPLobbyVM.LobbyPage.Matchmaking && (this.Matchmaking.CurrentSubPage == MPMatchmakingVM.MatchmakingSubPages.CustomGameList || this.Matchmaking.CurrentSubPage == MPMatchmakingVM.MatchmakingSubPages.CustomGame))
			{
				this.SetPage(MPLobbyVM.LobbyPage.Home, MPMatchmakingVM.MatchmakingSubPages.Default);
				InformationManager.ShowInquiry(new InquiryData(new TextObject("{=i0OEsfLt}Custom Battle Disabled", null).ToString(), new TextObject("{=F7dBjl83}Custom Battle feature is currently disabled", null).ToString(), true, false, new TextObject("{=yS7PvrTD}OK", null).ToString(), null, null, null, "", 0f, null, null, null), false, false);
			}
		}

		// Token: 0x06000865 RID: 2149 RVA: 0x00020838 File Offset: 0x0001EA38
		public void OnRejoinBattleRequestAnswered(bool isSuccessful)
		{
			if (isSuccessful && this._isRejoinRequested)
			{
				this._isRejoining = true;
				return;
			}
			this.SetPage(MPLobbyVM.LobbyPage.Home, MPMatchmakingVM.MatchmakingSubPages.Default);
		}

		// Token: 0x06000866 RID: 2150 RVA: 0x00020855 File Offset: 0x0001EA55
		public void OnRequestedToSearchBattle()
		{
			this.IsSearchGameRequested = true;
		}

		// Token: 0x06000867 RID: 2151 RVA: 0x00020860 File Offset: 0x0001EA60
		public void OnUpdateFindingGame(MatchmakingWaitTimeStats matchmakingWaitTimeStats, string[] gameTypeInfo)
		{
			if (!this.IsSearchingGame)
			{
				this.IsSearchGameRequested = false;
				this.IsSearchingGame = true;
				this.GameSearch.SetEnabled(true);
				this.Armory.SetCanOpenFacegen(false);
				this.Matchmaking.OnFindingGame();
			}
			if (this.IsSearchingGame)
			{
				Action onForceCloseFacegen = this._onForceCloseFacegen;
				if (onForceCloseFacegen != null)
				{
					onForceCloseFacegen();
				}
			}
			if (gameTypeInfo == null)
			{
				this.Matchmaking.GetSelectedGameTypesInfo(out gameTypeInfo);
			}
			this.GameSearch.UpdateData(matchmakingWaitTimeStats, gameTypeInfo);
		}

		// Token: 0x06000868 RID: 2152 RVA: 0x000208E0 File Offset: 0x0001EAE0
		public void OnPremadeGameCreated()
		{
			this.IsSearchingGame = true;
			this.Matchmaking.OnFindingGame();
			this.GameSearch.UpdatePremadeGameData();
			this.GameSearch.SetEnabled(true);
			this.Clan.ClanOverview.AreActionButtonsEnabled = false;
			this.Armory.SetCanOpenFacegen(false);
			Action onForceCloseFacegen = this._onForceCloseFacegen;
			if (onForceCloseFacegen != null)
			{
				onForceCloseFacegen();
			}
			this.SetPage(MPLobbyVM.LobbyPage.Profile, MPMatchmakingVM.MatchmakingSubPages.Default);
		}

		// Token: 0x06000869 RID: 2153 RVA: 0x0002094C File Offset: 0x0001EB4C
		public void OnRequestedToCancelSearchBattle()
		{
			this.GameSearch.OnRequestedToCancelSearchBattle();
		}

		// Token: 0x0600086A RID: 2154 RVA: 0x0002095C File Offset: 0x0001EB5C
		private void HandlePlayerRemovedFromParty(PlayerId playerID, PartyRemoveReason reason)
		{
			this.Friends.OnPlayerRemovedFromParty(playerID);
			if (NetworkMain.GameClient.ClanHomeInfo == null || !NetworkMain.GameClient.ClanHomeInfo.IsInClan)
			{
				this.RefreshClanInfo();
			}
			if (this.ClanCreationPopup.IsEnabled)
			{
				this.ClanCreationPopup.ExecuteClosePopup();
			}
			if (playerID == NetworkMain.GameClient.PlayerData.PlayerId && reason != PartyRemoveReason.DeclinedInvitation)
			{
				this.IsPartyLeader = false;
				this.IsInParty = false;
				TextObject textObject = GameTexts.FindText("str_youve_been_removed", null);
				if (reason == PartyRemoveReason.Left)
				{
					textObject = GameTexts.FindText("str_left_party", null);
				}
				else if (reason == PartyRemoveReason.Disband)
				{
					textObject = GameTexts.FindText("str_party_disbanded", null);
				}
				InformationManager.ShowInquiry(new InquiryData(string.Empty, textObject.ToString(), true, false, GameTexts.FindText("str_ok", null).ToString(), string.Empty, null, null, "", 0f, null, null, null), false, false);
			}
			this.GameSearch.UpdateCanCancel();
		}

		// Token: 0x0600086B RID: 2155 RVA: 0x00020A50 File Offset: 0x0001EC50
		private void HandlePlayerAddedToParty(PlayerId playerID)
		{
			this.Friends.OnPlayerAddedToParty(playerID);
			if (NetworkMain.GameClient.ClanHomeInfo == null || !NetworkMain.GameClient.ClanHomeInfo.IsInClan)
			{
				this.RefreshClanInfo();
			}
			this.IsInParty = NetworkMain.GameClient.IsInParty;
			this.IsPartyLeader = NetworkMain.GameClient.IsPartyLeader;
			this.GameSearch.UpdateCanCancel();
		}

		// Token: 0x0600086C RID: 2156 RVA: 0x00020AB7 File Offset: 0x0001ECB7
		public void OnPlayerRemovedFromParty(PlayerId playerId, PartyRemoveReason reason)
		{
			this._partyActionQueue.Enqueue(new ValueTuple<MPLobbyVM.PartyActionType, PlayerId, PartyRemoveReason>(MPLobbyVM.PartyActionType.Remove, playerId, reason));
		}

		// Token: 0x0600086D RID: 2157 RVA: 0x00020ACC File Offset: 0x0001ECCC
		public void OnPlayerAddedToParty(PlayerId playerId)
		{
			this._partyActionQueue.Enqueue(new ValueTuple<MPLobbyVM.PartyActionType, PlayerId, PartyRemoveReason>(MPLobbyVM.PartyActionType.Add, playerId, PartyRemoveReason.Kicked));
		}

		// Token: 0x0600086E RID: 2158 RVA: 0x00020AE1 File Offset: 0x0001ECE1
		public void OnPlayerAssignedPartyLeader(PlayerId newPartyLeaderId)
		{
			this._partyActionQueue.Enqueue(new ValueTuple<MPLobbyVM.PartyActionType, PlayerId, PartyRemoveReason>(MPLobbyVM.PartyActionType.AssignLeader, newPartyLeaderId, PartyRemoveReason.Kicked));
		}

		// Token: 0x0600086F RID: 2159 RVA: 0x00020AF8 File Offset: 0x0001ECF8
		private void HandlePlayerAssignedPartyLeader(PlayerId playerID)
		{
			this.Friends.OnPlayerAssignedPartyLeader();
			this.IsInParty = NetworkMain.GameClient.IsInParty;
			this.IsPartyLeader = NetworkMain.GameClient.IsPartyLeader;
			this.GameSearch.UpdateCanCancel();
			this.Friends.UpdateCanInviteOtherPlayersToParty();
		}

		// Token: 0x06000870 RID: 2160 RVA: 0x00020B46 File Offset: 0x0001ED46
		public void OnPlayerSuggestedToParty(PlayerId playerId, string playerName, PlayerId suggestingPlayerId, string suggestingPlayerName)
		{
			this._partySuggestionQueue.Enqueue(new MPLobbyPartyPlayerSuggestionPopupVM.PlayerPartySuggestionData(playerId, playerName, suggestingPlayerId, suggestingPlayerName));
		}

		// Token: 0x06000871 RID: 2161 RVA: 0x00020B60 File Offset: 0x0001ED60
		public void OnPlayerNameUpdated(string playerName)
		{
			MPLobbyHomeVM home = this.Home;
			if (home != null)
			{
				home.OnPlayerNameUpdated(playerName);
			}
			MPLobbyProfileVM profile = this.Profile;
			if (profile != null)
			{
				profile.OnPlayerNameUpdated(playerName);
			}
			MPLobbyClanVM clan = this.Clan;
			if (clan != null)
			{
				clan.OnPlayerNameUpdated(playerName);
			}
			MPLobbyClanCreationInformationVM clanCreationInformationPopup = this.ClanCreationInformationPopup;
			if (clanCreationInformationPopup == null)
			{
				return;
			}
			clanCreationInformationPopup.OnPlayerNameUpdated();
		}

		// Token: 0x06000872 RID: 2162 RVA: 0x00020BB4 File Offset: 0x0001EDB4
		public void OnSearchBattleCanceled()
		{
			this.IsSearchGameRequested = false;
			this.IsSearchingGame = false;
			this.Clan.ClanOverview.AreActionButtonsEnabled = true;
			this.GameSearch.SetEnabled(false);
			this.Armory.SetCanOpenFacegen(true);
			this.Matchmaking.OnCancelFindingGame();
		}

		// Token: 0x06000873 RID: 2163 RVA: 0x00020C04 File Offset: 0x0001EE04
		public async void RefreshPlayerData(PlayerData playerData)
		{
			if (playerData != null)
			{
				if (!NetworkMain.GameClient.IsRefreshingPlayerData && NetworkMain.GameClient.Connected)
				{
					NetworkMain.GameClient.IsRefreshingPlayerData = true;
					await NetworkMain.GameClient.GetCosmeticsInfo();
					this.Armory.RefreshPlayerData(playerData);
					this.Friends.Player.UpdateWith(playerData);
					this.BadgeSelectionPopup.RefreshPlayerData(playerData);
					this.Matchmaking.RefreshPlayerData(playerData);
					await NetworkMain.GameClient.GetClanHomeInfo();
					this.Home.RefreshPlayerData(playerData, true);
					this.Profile.UpdatePlayerData(playerData, true, true);
					NetworkMain.GameClient.IsRefreshingPlayerData = false;
				}
			}
			else
			{
				this.SetPage(MPLobbyVM.LobbyPage.Authentication, MPMatchmakingVM.MatchmakingSubPages.Default);
			}
			this.RefreshSupportedFeatures();
		}

		// Token: 0x06000874 RID: 2164 RVA: 0x00020C48 File Offset: 0x0001EE48
		public void RefreshSupportedFeatures()
		{
			SupportedFeatures supportedFeatures = this._lobbyClient.SupportedFeatures;
			this.Matchmaking.OnSupportedFeaturesRefreshed(supportedFeatures);
			this.Menu.OnSupportedFeaturesRefreshed(supportedFeatures);
			this.Friends.OnSupportedFeaturesRefreshed(supportedFeatures);
			if (!this.Menu.IsMatchmakingSupported)
			{
				this.DisallowedPages.Add(MPLobbyVM.LobbyPage.Matchmaking);
			}
		}

		// Token: 0x06000875 RID: 2165 RVA: 0x00020CA0 File Offset: 0x0001EEA0
		private void UpdateBlockerState()
		{
			LobbyClient.State currentState = this._lobbyClient.CurrentState;
			if (NetworkMain.GameClient.IsRefreshingPlayerData || this._isDisconnecting || this._isRejoining || this._isStartingGameFind || currentState == LobbyClient.State.AtBattle || currentState == LobbyClient.State.QuittingFromBattle || currentState == LobbyClient.State.WaitingToRegisterCustomGame || currentState == LobbyClient.State.HostingCustomGame || currentState == LobbyClient.State.WaitingToJoinCustomGame || currentState == LobbyClient.State.InCustomGame)
			{
				if (!this.BlockerState.IsEnabled)
				{
					TextObject textObject = new TextObject("{=Rc95Kq8r}Please wait...", null);
					this.BlockerState.OnLobbyStateIsBlocker(textObject);
					return;
				}
			}
			else if (this.BlockerState.IsEnabled)
			{
				this.BlockerState.OnLobbyStateNotBlocker();
			}
		}

		// Token: 0x06000876 RID: 2166 RVA: 0x00020D39 File Offset: 0x0001EF39
		private void OnChangePageRequest(MPLobbyVM.LobbyPage page)
		{
			this.SetPage(page, MPMatchmakingVM.MatchmakingSubPages.Default);
		}

		// Token: 0x06000877 RID: 2167 RVA: 0x00020D43 File Offset: 0x0001EF43
		private void OnMatchSelectionChanged(string selectionInfo, bool isMatchFindPossible)
		{
			this.Home.OnMatchSelectionChanged(selectionInfo, isMatchFindPossible);
			this.Profile.OnMatchSelectionChanged(selectionInfo, isMatchFindPossible);
		}

		// Token: 0x06000878 RID: 2168 RVA: 0x00020D5F File Offset: 0x0001EF5F
		private void OnGameFindStateChanged(bool isStartingGameFind)
		{
			this._isStartingGameFind = isStartingGameFind;
		}

		// Token: 0x06000879 RID: 2169 RVA: 0x00020D68 File Offset: 0x0001EF68
		private void OnShowPlayerProfile(PlayerId playerID)
		{
			this.PlayerProfile.OpenWith(playerID);
		}

		// Token: 0x0600087A RID: 2170 RVA: 0x00020D76 File Offset: 0x0001EF76
		private void ExecuteShowBrightness()
		{
			this.BrightnessPopup.Visible = true;
		}

		// Token: 0x0600087B RID: 2171 RVA: 0x00020D84 File Offset: 0x0001EF84
		private void ExecuteShowExposure()
		{
			this.ExposurePopup.Visible = true;
		}

		// Token: 0x0600087C RID: 2172 RVA: 0x00020D92 File Offset: 0x0001EF92
		private void OpenClanCreationPopup()
		{
			this.ClanCreationPopup.ExecuteOpenPopup();
		}

		// Token: 0x0600087D RID: 2173 RVA: 0x00020D9F File Offset: 0x0001EF9F
		private void CloseClanCreationPopup()
		{
			this.ClanCreationPopup.ExecuteClosePopup();
		}

		// Token: 0x0600087E RID: 2174 RVA: 0x00020DAC File Offset: 0x0001EFAC
		private void ExecuteOpenRecentGames()
		{
			this.RecentGames.ExecuteOpenPopup();
		}

		// Token: 0x0600087F RID: 2175 RVA: 0x00020DB9 File Offset: 0x0001EFB9
		private void OpenInviteClanMemberPopup()
		{
			this.ClanInviteFriendsPopup.Open();
		}

		// Token: 0x06000880 RID: 2176 RVA: 0x00020DC6 File Offset: 0x0001EFC6
		private void OnBadgeProgressInfoRequested(MPLobbyAchievementBadgeGroupVM achivementGroup)
		{
			this.BadgeProgressionInformation.OpenWith(achivementGroup);
		}

		// Token: 0x06000881 RID: 2177 RVA: 0x00020DD4 File Offset: 0x0001EFD4
		private void OnBadgeNotificationRead()
		{
			this.Profile.HasBadgeNotification = this.BadgeSelectionPopup.HasNotifications;
			this.Menu.HasProfileNotification = this.BadgeSelectionPopup.HasNotifications;
		}

		// Token: 0x06000882 RID: 2178 RVA: 0x00020E04 File Offset: 0x0001F004
		private void OnBadgeSelectionUpdated()
		{
			PlayerData playerData = NetworkMain.GameClient.PlayerData;
			if (playerData != null)
			{
				this.Home.RefreshPlayerData(playerData, false);
				this.Profile.UpdatePlayerData(playerData, false, false);
			}
		}

		// Token: 0x06000883 RID: 2179 RVA: 0x00020E3A File Offset: 0x0001F03A
		private void OnBadgeChangeRequested(PlayerId playerID)
		{
			if (playerID == NetworkMain.GameClient.PlayerID)
			{
				this.BadgeSelectionPopup.Open();
			}
		}

		// Token: 0x06000884 RID: 2180 RVA: 0x00020E59 File Offset: 0x0001F059
		private void OnRankProgressionRequested(MPLobbyPlayerBaseVM player)
		{
			this.RankProgressInformation.OpenWith(player);
		}

		// Token: 0x06000885 RID: 2181 RVA: 0x00020E67 File Offset: 0x0001F067
		private void OnRankLeaderboardRequested(string gameMode)
		{
			this.RankLeaderboard.OpenWith(gameMode);
		}

		// Token: 0x06000886 RID: 2182 RVA: 0x00020E75 File Offset: 0x0001F075
		private void OnClanPageRequested()
		{
			if (NetworkMain.GameClient.IsInClan)
			{
				this.Clan.ExecuteOpenPopup();
				return;
			}
			this.ClanCreationInformationPopup.RefreshWith(NetworkMain.GameClient.ClanHomeInfo);
			this.ClanCreationInformationPopup.ExecuteOpenPopup();
		}

		// Token: 0x06000887 RID: 2183 RVA: 0x00020EAF File Offset: 0x0001F0AF
		private void OnClanLeaderboardRequested()
		{
			this.ClanLeaderboardPopup.ExecuteOpenPopup();
		}

		// Token: 0x06000888 RID: 2184 RVA: 0x00020EBC File Offset: 0x0001F0BC
		public void OnNotificationsReceived(LobbyNotification[] notifications)
		{
			List<LobbyNotification> list = notifications.Where((LobbyNotification n) => n.Type == NotificationType.FriendRequest).ToList<LobbyNotification>();
			this.Friends.OnFriendRequestNotificationsReceived(list);
			foreach (LobbyNotification lobbyNotification in notifications)
			{
				if (lobbyNotification.Type == NotificationType.ClanAnnouncement)
				{
					this.Clan.OnNotificationReceived(lobbyNotification);
				}
				else if (lobbyNotification.Type == NotificationType.BadgeEarned)
				{
					this.Profile.OnNotificationReceived(lobbyNotification);
					this.BadgeSelectionPopup.OnNotificationReceived(lobbyNotification);
					if (!this.Profile.IsEnabled)
					{
						this.Menu.HasProfileNotification = this.BadgeSelectionPopup.HasNotifications;
					}
				}
			}
		}

		// Token: 0x06000889 RID: 2185 RVA: 0x00020F6E File Offset: 0x0001F16E
		private void OnAddFriendWithBannerlordIDRequested(PlayerId playerID)
		{
			if (playerID == NetworkMain.GameClient.PlayerID)
			{
				this.BannerlordIDAddFriendPopup.ExecuteOpenPopup();
			}
		}

		// Token: 0x0600088A RID: 2186 RVA: 0x00020F8D File Offset: 0x0001F18D
		private void OnBannerlordIDChangeRequested(PlayerId playerID)
		{
			if (playerID == NetworkMain.GameClient.PlayerID)
			{
				this.BannerlordIDChangePopup.ExecuteOpenPopup();
			}
		}

		// Token: 0x0600088B RID: 2187 RVA: 0x00020FAC File Offset: 0x0001F1AC
		private void OnItemObtainRequested(MPLobbyCosmeticSigilItemVM sigilItem)
		{
			this.CosmeticObtainPopup.OpenWith(sigilItem);
		}

		// Token: 0x0600088C RID: 2188 RVA: 0x00020FBA File Offset: 0x0001F1BA
		private void OnItemObtainRequested(MPArmoryCosmeticItemVM troopItem)
		{
			this.CosmeticObtainPopup.OpenWith(troopItem);
		}

		// Token: 0x0600088D RID: 2189 RVA: 0x00020FC8 File Offset: 0x0001F1C8
		private void OnItemObtained(string cosmeticID, int finalLoot)
		{
			this.Armory.Cosmetics.OnItemObtained(cosmeticID, finalLoot);
			this.ChangeSigilPopup.OnLootUpdated(finalLoot);
			this.Home.Player.Loot = finalLoot;
		}

		// Token: 0x0600088E RID: 2190 RVA: 0x00020FF9 File Offset: 0x0001F1F9
		private void OnSigilChangeRequested(PlayerId playerID)
		{
			if (playerID == NetworkMain.GameClient.PlayerID)
			{
				this.ChangeSigilPopup.Open();
			}
		}

		// Token: 0x0600088F RID: 2191 RVA: 0x00021018 File Offset: 0x0001F218
		public void OnSigilChanged(int iconID)
		{
			this.Home.Player.Sigil.RefreshWith(iconID);
			MPLobbyPlayerBaseVM player = this.Profile.PlayerInfo.Player;
			if (player != null)
			{
				player.Sigil.RefreshWith(iconID);
			}
			Banner banner = Banner.CreateOneColoredEmptyBanner(0);
			BannerData bannerData = new BannerData(iconID, 0, 0, new Vec2(512f, 512f), new Vec2(764f, 764f), false, false, 0f);
			banner.AddIconData(bannerData);
			NetworkMain.GameClient.PlayerData.Sigil = banner.Serialize();
		}

		// Token: 0x06000890 RID: 2192 RVA: 0x000210AD File Offset: 0x0001F2AD
		public void OnClanCreationFinished()
		{
			this.ClanCreationPopup.IsWaiting = false;
			this.ClanCreationPopup.HasCreationStarted = false;
			this.ClanInvitationPopup.Close();
			this.ClanCreationPopup.ExecuteClosePopup();
			this.RefreshClanInfo();
		}

		// Token: 0x06000891 RID: 2193 RVA: 0x000210E3 File Offset: 0x0001F2E3
		public void OnEnableGenericAvatarsChanged()
		{
			this.OnFriendListUpdated(true);
		}

		// Token: 0x06000892 RID: 2194 RVA: 0x000210EC File Offset: 0x0001F2EC
		public void OnEnableGenericNamesChanged()
		{
			this.OnFriendListUpdated(true);
		}

		// Token: 0x06000893 RID: 2195 RVA: 0x000210F8 File Offset: 0x0001F2F8
		public void OnFriendListUpdated(bool forceUpdate = false)
		{
			this._lobbyClient.FriendIDs.Clear();
			foreach (IFriendListService friendListService in PlatformServices.Instance.GetFriendListServices())
			{
				if (friendListService.IncludeInAllFriends)
				{
					this._lobbyClient.FriendIDs.AddRange(friendListService.GetAllFriends());
				}
			}
			this.Friends.OnFriendListUpdated(forceUpdate);
			this.RecentGames.OnFriendListUpdated(forceUpdate);
			MPLobbyClanCreationInformationVM clanCreationInformationPopup = this.ClanCreationInformationPopup;
			if (clanCreationInformationPopup != null)
			{
				clanCreationInformationPopup.OnFriendListUpdated(forceUpdate);
			}
			this.Home.Player.UpdateNameAndAvatar(forceUpdate);
		}

		// Token: 0x06000894 RID: 2196 RVA: 0x0002118C File Offset: 0x0001F38C
		public void OnClanInfoChanged()
		{
			this.Clan.OnClanInfoChanged();
			this.Friends.OnClanInfoChanged();
			this.Home.OnClanInfoChanged();
			this.Profile.OnClanInfoChanged();
			this.PlayerProfile.OnClanInfoChanged();
			if (this.Clan.IsEnabled && !NetworkMain.GameClient.ClanHomeInfo.IsInClan)
			{
				this.SetPage(MPLobbyVM.LobbyPage.Home, MPMatchmakingVM.MatchmakingSubPages.Default);
			}
		}

		// Token: 0x06000895 RID: 2197 RVA: 0x000211F8 File Offset: 0x0001F3F8
		private async void RefreshClanInfo()
		{
			await NetworkMain.GameClient.GetClanHomeInfo();
		}

		// Token: 0x06000896 RID: 2198 RVA: 0x00021229 File Offset: 0x0001F429
		private void OnRejoinRequested()
		{
			this._isRejoinRequested = true;
		}

		// Token: 0x1700028F RID: 655
		// (get) Token: 0x06000897 RID: 2199 RVA: 0x00021232 File Offset: 0x0001F432
		// (set) Token: 0x06000898 RID: 2200 RVA: 0x0002123A File Offset: 0x0001F43A
		[DataSourceProperty]
		public bool IsLoggedIn
		{
			get
			{
				return this._isLoggedIn;
			}
			set
			{
				if (value != this._isLoggedIn)
				{
					this._isLoggedIn = value;
					base.OnPropertyChangedWithValue(value, "IsLoggedIn");
				}
			}
		}

		// Token: 0x17000290 RID: 656
		// (get) Token: 0x06000899 RID: 2201 RVA: 0x00021258 File Offset: 0x0001F458
		// (set) Token: 0x0600089A RID: 2202 RVA: 0x00021260 File Offset: 0x0001F460
		[DataSourceProperty]
		public BrightnessOptionVM BrightnessPopup
		{
			get
			{
				return this._brightnessPopup;
			}
			set
			{
				if (value != this._brightnessPopup)
				{
					this._brightnessPopup = value;
					base.OnPropertyChangedWithValue<BrightnessOptionVM>(value, "BrightnessPopup");
				}
			}
		}

		// Token: 0x17000291 RID: 657
		// (get) Token: 0x0600089B RID: 2203 RVA: 0x0002127E File Offset: 0x0001F47E
		// (set) Token: 0x0600089C RID: 2204 RVA: 0x00021286 File Offset: 0x0001F486
		[DataSourceProperty]
		public ExposureOptionVM ExposurePopup
		{
			get
			{
				return this._exposurePopup;
			}
			set
			{
				if (value != this._exposurePopup)
				{
					this._exposurePopup = value;
					base.OnPropertyChangedWithValue<ExposureOptionVM>(value, "ExposurePopup");
				}
			}
		}

		// Token: 0x17000292 RID: 658
		// (get) Token: 0x0600089D RID: 2205 RVA: 0x000212A4 File Offset: 0x0001F4A4
		// (set) Token: 0x0600089E RID: 2206 RVA: 0x000212AC File Offset: 0x0001F4AC
		[DataSourceProperty]
		public bool IsArmoryActive
		{
			get
			{
				return this._isArmoryActive;
			}
			set
			{
				if (value != this._isArmoryActive)
				{
					this._isArmoryActive = value;
					base.OnPropertyChangedWithValue(value, "IsArmoryActive");
				}
			}
		}

		// Token: 0x17000293 RID: 659
		// (get) Token: 0x0600089F RID: 2207 RVA: 0x000212CA File Offset: 0x0001F4CA
		// (set) Token: 0x060008A0 RID: 2208 RVA: 0x000212D2 File Offset: 0x0001F4D2
		[DataSourceProperty]
		public bool IsSearchGameRequested
		{
			get
			{
				return this._isSearchGameRequested;
			}
			set
			{
				if (value != this._isSearchGameRequested)
				{
					this._isSearchGameRequested = value;
					base.OnPropertyChangedWithValue(value, "IsSearchGameRequested");
				}
			}
		}

		// Token: 0x17000294 RID: 660
		// (get) Token: 0x060008A1 RID: 2209 RVA: 0x000212F0 File Offset: 0x0001F4F0
		// (set) Token: 0x060008A2 RID: 2210 RVA: 0x000212F8 File Offset: 0x0001F4F8
		[DataSourceProperty]
		public bool IsInParty
		{
			get
			{
				return this._isInParty;
			}
			set
			{
				if (value != this._isInParty)
				{
					this._isInParty = value;
					base.OnPropertyChangedWithValue(value, "IsInParty");
				}
			}
		}

		// Token: 0x17000295 RID: 661
		// (get) Token: 0x060008A3 RID: 2211 RVA: 0x00021316 File Offset: 0x0001F516
		// (set) Token: 0x060008A4 RID: 2212 RVA: 0x0002131E File Offset: 0x0001F51E
		[DataSourceProperty]
		public bool IsSearchingGame
		{
			get
			{
				return this._isSearchingGame;
			}
			set
			{
				if (value != this._isSearchingGame)
				{
					this._isSearchingGame = value;
					base.OnPropertyChangedWithValue(value, "IsSearchingGame");
				}
			}
		}

		// Token: 0x17000296 RID: 662
		// (get) Token: 0x060008A5 RID: 2213 RVA: 0x0002133C File Offset: 0x0001F53C
		// (set) Token: 0x060008A6 RID: 2214 RVA: 0x00021344 File Offset: 0x0001F544
		[DataSourceProperty]
		public bool IsMatchmakingEnabled
		{
			get
			{
				return this._isMatchmakingEnabled;
			}
			set
			{
				if (value != this._isMatchmakingEnabled)
				{
					this._isMatchmakingEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsMatchmakingEnabled");
				}
			}
		}

		// Token: 0x17000297 RID: 663
		// (get) Token: 0x060008A7 RID: 2215 RVA: 0x00021362 File Offset: 0x0001F562
		// (set) Token: 0x060008A8 RID: 2216 RVA: 0x0002136A File Offset: 0x0001F56A
		[DataSourceProperty]
		public bool IsCustomGameFindEnabled
		{
			get
			{
				return this._isCustomGameFindEnabled;
			}
			set
			{
				if (value != this._isCustomGameFindEnabled)
				{
					this._isCustomGameFindEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsCustomGameFindEnabled");
				}
			}
		}

		// Token: 0x17000298 RID: 664
		// (get) Token: 0x060008A9 RID: 2217 RVA: 0x00021388 File Offset: 0x0001F588
		// (set) Token: 0x060008AA RID: 2218 RVA: 0x00021390 File Offset: 0x0001F590
		[DataSourceProperty]
		public bool IsPartyLeader
		{
			get
			{
				return this._isPartyLeader;
			}
			set
			{
				if (value != this._isPartyLeader)
				{
					this._isPartyLeader = value;
					base.OnPropertyChangedWithValue(value, "IsPartyLeader");
				}
			}
		}

		// Token: 0x17000299 RID: 665
		// (get) Token: 0x060008AB RID: 2219 RVA: 0x000213AE File Offset: 0x0001F5AE
		// (set) Token: 0x060008AC RID: 2220 RVA: 0x000213B6 File Offset: 0x0001F5B6
		[DataSourceProperty]
		public MPLobbyBlockerStateVM BlockerState
		{
			get
			{
				return this._blockerState;
			}
			set
			{
				if (value != this._blockerState)
				{
					this._blockerState = value;
					base.OnPropertyChangedWithValue<MPLobbyBlockerStateVM>(value, "BlockerState");
				}
			}
		}

		// Token: 0x1700029A RID: 666
		// (get) Token: 0x060008AD RID: 2221 RVA: 0x000213D4 File Offset: 0x0001F5D4
		// (set) Token: 0x060008AE RID: 2222 RVA: 0x000213DC File Offset: 0x0001F5DC
		[DataSourceProperty]
		public MPLobbyMenuVM Menu
		{
			get
			{
				return this._menu;
			}
			set
			{
				if (value != this._menu)
				{
					this._menu = value;
					base.OnPropertyChangedWithValue<MPLobbyMenuVM>(value, "Menu");
				}
			}
		}

		// Token: 0x1700029B RID: 667
		// (get) Token: 0x060008AF RID: 2223 RVA: 0x000213FA File Offset: 0x0001F5FA
		// (set) Token: 0x060008B0 RID: 2224 RVA: 0x00021402 File Offset: 0x0001F602
		[DataSourceProperty]
		public MPAuthenticationVM Login
		{
			get
			{
				return this._login;
			}
			set
			{
				if (value != this._login)
				{
					this._login = value;
					base.OnPropertyChangedWithValue<MPAuthenticationVM>(value, "Login");
				}
			}
		}

		// Token: 0x1700029C RID: 668
		// (get) Token: 0x060008B1 RID: 2225 RVA: 0x00021420 File Offset: 0x0001F620
		// (set) Token: 0x060008B2 RID: 2226 RVA: 0x00021428 File Offset: 0x0001F628
		[DataSourceProperty]
		public MPLobbyRejoinVM Rejoin
		{
			get
			{
				return this._rejoin;
			}
			set
			{
				if (value != this._rejoin)
				{
					this._rejoin = value;
					base.OnPropertyChangedWithValue<MPLobbyRejoinVM>(value, "Rejoin");
				}
			}
		}

		// Token: 0x1700029D RID: 669
		// (get) Token: 0x060008B3 RID: 2227 RVA: 0x00021446 File Offset: 0x0001F646
		// (set) Token: 0x060008B4 RID: 2228 RVA: 0x0002144E File Offset: 0x0001F64E
		[DataSourceProperty]
		public MPLobbyFriendsVM Friends
		{
			get
			{
				return this._friends;
			}
			set
			{
				if (value != this._friends)
				{
					this._friends = value;
					base.OnPropertyChangedWithValue<MPLobbyFriendsVM>(value, "Friends");
				}
			}
		}

		// Token: 0x1700029E RID: 670
		// (get) Token: 0x060008B5 RID: 2229 RVA: 0x0002146C File Offset: 0x0001F66C
		// (set) Token: 0x060008B6 RID: 2230 RVA: 0x00021474 File Offset: 0x0001F674
		[DataSourceProperty]
		public MPLobbyHomeVM Home
		{
			get
			{
				return this._home;
			}
			set
			{
				if (value != this._home)
				{
					this._home = value;
					base.OnPropertyChangedWithValue<MPLobbyHomeVM>(value, "Home");
				}
			}
		}

		// Token: 0x1700029F RID: 671
		// (get) Token: 0x060008B7 RID: 2231 RVA: 0x00021492 File Offset: 0x0001F692
		// (set) Token: 0x060008B8 RID: 2232 RVA: 0x0002149A File Offset: 0x0001F69A
		[DataSourceProperty]
		public MPMatchmakingVM Matchmaking
		{
			get
			{
				return this._matchmaking;
			}
			set
			{
				if (value != this._matchmaking)
				{
					this._matchmaking = value;
					base.OnPropertyChangedWithValue<MPMatchmakingVM>(value, "Matchmaking");
				}
			}
		}

		// Token: 0x170002A0 RID: 672
		// (get) Token: 0x060008B9 RID: 2233 RVA: 0x000214B8 File Offset: 0x0001F6B8
		// (set) Token: 0x060008BA RID: 2234 RVA: 0x000214C0 File Offset: 0x0001F6C0
		[DataSourceProperty]
		public MPArmoryVM Armory
		{
			get
			{
				return this._armory;
			}
			set
			{
				if (value != this._armory)
				{
					this._armory = value;
					base.OnPropertyChangedWithValue<MPArmoryVM>(value, "Armory");
				}
			}
		}

		// Token: 0x170002A1 RID: 673
		// (get) Token: 0x060008BB RID: 2235 RVA: 0x000214DE File Offset: 0x0001F6DE
		// (set) Token: 0x060008BC RID: 2236 RVA: 0x000214E6 File Offset: 0x0001F6E6
		[DataSourceProperty]
		public MPLobbyGameSearchVM GameSearch
		{
			get
			{
				return this._gameSearch;
			}
			set
			{
				if (value != this._gameSearch)
				{
					this._gameSearch = value;
					base.OnPropertyChangedWithValue<MPLobbyGameSearchVM>(value, "GameSearch");
				}
			}
		}

		// Token: 0x170002A2 RID: 674
		// (get) Token: 0x060008BD RID: 2237 RVA: 0x00021504 File Offset: 0x0001F704
		// (set) Token: 0x060008BE RID: 2238 RVA: 0x0002150C File Offset: 0x0001F70C
		[DataSourceProperty]
		public MPLobbyPlayerProfileVM PlayerProfile
		{
			get
			{
				return this._playerProfile;
			}
			set
			{
				if (value != this._playerProfile)
				{
					this._playerProfile = value;
					base.OnPropertyChangedWithValue<MPLobbyPlayerProfileVM>(value, "PlayerProfile");
				}
			}
		}

		// Token: 0x170002A3 RID: 675
		// (get) Token: 0x060008BF RID: 2239 RVA: 0x0002152A File Offset: 0x0001F72A
		// (set) Token: 0x060008C0 RID: 2240 RVA: 0x00021532 File Offset: 0x0001F732
		[DataSourceProperty]
		public MPAfterBattlePopupVM AfterBattlePopup
		{
			get
			{
				return this._afterBattlePopup;
			}
			set
			{
				if (value != this._afterBattlePopup)
				{
					this._afterBattlePopup = value;
					base.OnPropertyChangedWithValue<MPAfterBattlePopupVM>(value, "AfterBattlePopup");
				}
			}
		}

		// Token: 0x170002A4 RID: 676
		// (get) Token: 0x060008C1 RID: 2241 RVA: 0x00021550 File Offset: 0x0001F750
		// (set) Token: 0x060008C2 RID: 2242 RVA: 0x00021558 File Offset: 0x0001F758
		[DataSourceProperty]
		public MPLobbyPartyInvitationPopupVM PartyInvitationPopup
		{
			get
			{
				return this._partyInvitationPopup;
			}
			set
			{
				if (value != this._partyInvitationPopup)
				{
					this._partyInvitationPopup = value;
					base.OnPropertyChangedWithValue<MPLobbyPartyInvitationPopupVM>(value, "PartyInvitationPopup");
				}
			}
		}

		// Token: 0x170002A5 RID: 677
		// (get) Token: 0x060008C3 RID: 2243 RVA: 0x00021576 File Offset: 0x0001F776
		// (set) Token: 0x060008C4 RID: 2244 RVA: 0x0002157E File Offset: 0x0001F77E
		[DataSourceProperty]
		public MPLobbyPopupVM Popup
		{
			get
			{
				return this._popup;
			}
			set
			{
				if (value != this._popup)
				{
					this._popup = value;
					base.OnPropertyChangedWithValue<MPLobbyPopupVM>(value, "Popup");
				}
			}
		}

		// Token: 0x170002A6 RID: 678
		// (get) Token: 0x060008C5 RID: 2245 RVA: 0x0002159C File Offset: 0x0001F79C
		// (set) Token: 0x060008C6 RID: 2246 RVA: 0x000215A4 File Offset: 0x0001F7A4
		[DataSourceProperty]
		public MPLobbyPartyPlayerSuggestionPopupVM PartyPlayerSuggestionPopup
		{
			get
			{
				return this._partyPlayerSuggestionPopup;
			}
			set
			{
				if (value != this._partyPlayerSuggestionPopup)
				{
					this._partyPlayerSuggestionPopup = value;
					base.OnPropertyChanged("PartyPlayerSuggestionPopup");
				}
			}
		}

		// Token: 0x170002A7 RID: 679
		// (get) Token: 0x060008C7 RID: 2247 RVA: 0x000215C1 File Offset: 0x0001F7C1
		// (set) Token: 0x060008C8 RID: 2248 RVA: 0x000215C9 File Offset: 0x0001F7C9
		[DataSourceProperty]
		public MPOptionsVM Options
		{
			get
			{
				return this._options;
			}
			set
			{
				if (value != this._options)
				{
					this._options = value;
					base.OnPropertyChangedWithValue<MPOptionsVM>(value, "Options");
				}
			}
		}

		// Token: 0x170002A8 RID: 680
		// (get) Token: 0x060008C9 RID: 2249 RVA: 0x000215E7 File Offset: 0x0001F7E7
		// (set) Token: 0x060008CA RID: 2250 RVA: 0x000215EF File Offset: 0x0001F7EF
		[DataSourceProperty]
		public MPLobbyProfileVM Profile
		{
			get
			{
				return this._profile;
			}
			set
			{
				if (value != this._profile)
				{
					this._profile = value;
					base.OnPropertyChangedWithValue<MPLobbyProfileVM>(value, "Profile");
				}
			}
		}

		// Token: 0x170002A9 RID: 681
		// (get) Token: 0x060008CB RID: 2251 RVA: 0x0002160D File Offset: 0x0001F80D
		// (set) Token: 0x060008CC RID: 2252 RVA: 0x00021615 File Offset: 0x0001F815
		[DataSourceProperty]
		public MPLobbyClanVM Clan
		{
			get
			{
				return this._clan;
			}
			set
			{
				if (value != this._clan)
				{
					this._clan = value;
					base.OnPropertyChanged("Clan");
				}
			}
		}

		// Token: 0x170002AA RID: 682
		// (get) Token: 0x060008CD RID: 2253 RVA: 0x00021632 File Offset: 0x0001F832
		// (set) Token: 0x060008CE RID: 2254 RVA: 0x0002163A File Offset: 0x0001F83A
		[DataSourceProperty]
		public MPLobbyClanCreationPopupVM ClanCreationPopup
		{
			get
			{
				return this._clanCreationPopup;
			}
			set
			{
				if (value != this._clanCreationPopup)
				{
					this._clanCreationPopup = value;
					base.OnPropertyChanged("ClanCreationPopup");
				}
			}
		}

		// Token: 0x170002AB RID: 683
		// (get) Token: 0x060008CF RID: 2255 RVA: 0x00021657 File Offset: 0x0001F857
		// (set) Token: 0x060008D0 RID: 2256 RVA: 0x0002165F File Offset: 0x0001F85F
		[DataSourceProperty]
		public MPLobbyClanCreationInformationVM ClanCreationInformationPopup
		{
			get
			{
				return this._clanCreationInformationPopup;
			}
			set
			{
				if (value != this._clanCreationInformationPopup)
				{
					this._clanCreationInformationPopup = value;
					base.OnPropertyChanged("ClanCreationInformationPopup");
				}
			}
		}

		// Token: 0x170002AC RID: 684
		// (get) Token: 0x060008D1 RID: 2257 RVA: 0x0002167C File Offset: 0x0001F87C
		// (set) Token: 0x060008D2 RID: 2258 RVA: 0x00021684 File Offset: 0x0001F884
		[DataSourceProperty]
		public MPLobbyClanInvitationPopupVM ClanInvitationPopup
		{
			get
			{
				return this._clanInvitationPopup;
			}
			set
			{
				if (value != this._clanInvitationPopup)
				{
					this._clanInvitationPopup = value;
					base.OnPropertyChanged("ClanInvitationPopup");
				}
			}
		}

		// Token: 0x170002AD RID: 685
		// (get) Token: 0x060008D3 RID: 2259 RVA: 0x000216A1 File Offset: 0x0001F8A1
		// (set) Token: 0x060008D4 RID: 2260 RVA: 0x000216A9 File Offset: 0x0001F8A9
		[DataSourceProperty]
		public MPLobbyClanMatchmakingRequestPopupVM ClanMatchmakingRequestPopup
		{
			get
			{
				return this._clanMatchmakingRequestPopup;
			}
			set
			{
				if (value != this._clanMatchmakingRequestPopup)
				{
					this._clanMatchmakingRequestPopup = value;
					base.OnPropertyChanged("ClanMatchmakingRequestPopup");
				}
			}
		}

		// Token: 0x170002AE RID: 686
		// (get) Token: 0x060008D5 RID: 2261 RVA: 0x000216C6 File Offset: 0x0001F8C6
		// (set) Token: 0x060008D6 RID: 2262 RVA: 0x000216CE File Offset: 0x0001F8CE
		[DataSourceProperty]
		public MPLobbyClanInviteFriendsPopupVM ClanInviteFriendsPopup
		{
			get
			{
				return this._clanInviteFriendsPopup;
			}
			set
			{
				if (value != this._clanInviteFriendsPopup)
				{
					this._clanInviteFriendsPopup = value;
					base.OnPropertyChanged("ClanInviteFriendsPopup");
				}
			}
		}

		// Token: 0x170002AF RID: 687
		// (get) Token: 0x060008D7 RID: 2263 RVA: 0x000216EB File Offset: 0x0001F8EB
		// (set) Token: 0x060008D8 RID: 2264 RVA: 0x000216F3 File Offset: 0x0001F8F3
		[DataSourceProperty]
		public MPLobbyClanLeaderboardVM ClanLeaderboardPopup
		{
			get
			{
				return this._clanLeaderboardPopup;
			}
			set
			{
				if (value != this._clanLeaderboardPopup)
				{
					this._clanLeaderboardPopup = value;
					base.OnPropertyChanged("ClanLeaderboardPopup");
				}
			}
		}

		// Token: 0x170002B0 RID: 688
		// (get) Token: 0x060008D9 RID: 2265 RVA: 0x00021710 File Offset: 0x0001F910
		// (set) Token: 0x060008DA RID: 2266 RVA: 0x00021718 File Offset: 0x0001F918
		[DataSourceProperty]
		public MPCosmeticObtainPopupVM CosmeticObtainPopup
		{
			get
			{
				return this._cosmeticObtainPopup;
			}
			set
			{
				if (value != this._cosmeticObtainPopup)
				{
					this._cosmeticObtainPopup = value;
					base.OnPropertyChangedWithValue<MPCosmeticObtainPopupVM>(value, "CosmeticObtainPopup");
				}
			}
		}

		// Token: 0x170002B1 RID: 689
		// (get) Token: 0x060008DB RID: 2267 RVA: 0x00021736 File Offset: 0x0001F936
		// (set) Token: 0x060008DC RID: 2268 RVA: 0x0002173E File Offset: 0x0001F93E
		[DataSourceProperty]
		public MPLobbyBannerlordIDAddFriendPopupVM BannerlordIDAddFriendPopup
		{
			get
			{
				return this._bannerlordIDAddFriendPopup;
			}
			set
			{
				if (value != this._bannerlordIDAddFriendPopup)
				{
					this._bannerlordIDAddFriendPopup = value;
					base.OnPropertyChanged("BannerlordIDAddFriendPopup");
				}
			}
		}

		// Token: 0x170002B2 RID: 690
		// (get) Token: 0x060008DD RID: 2269 RVA: 0x0002175B File Offset: 0x0001F95B
		// (set) Token: 0x060008DE RID: 2270 RVA: 0x00021763 File Offset: 0x0001F963
		[DataSourceProperty]
		public MPLobbyBannerlordIDChangePopup BannerlordIDChangePopup
		{
			get
			{
				return this._bannerlordIDChangePopup;
			}
			set
			{
				if (value != this._bannerlordIDChangePopup)
				{
					this._bannerlordIDChangePopup = value;
					base.OnPropertyChanged("BannerlordIDChangePopup");
				}
			}
		}

		// Token: 0x170002B3 RID: 691
		// (get) Token: 0x060008DF RID: 2271 RVA: 0x00021780 File Offset: 0x0001F980
		// (set) Token: 0x060008E0 RID: 2272 RVA: 0x00021788 File Offset: 0x0001F988
		[DataSourceProperty]
		public MPLobbyBadgeProgressInformationVM BadgeProgressionInformation
		{
			get
			{
				return this._badgeProgressionInformation;
			}
			set
			{
				if (value != this._badgeProgressionInformation)
				{
					this._badgeProgressionInformation = value;
					base.OnPropertyChangedWithValue<MPLobbyBadgeProgressInformationVM>(value, "BadgeProgressionInformation");
				}
			}
		}

		// Token: 0x170002B4 RID: 692
		// (get) Token: 0x060008E1 RID: 2273 RVA: 0x000217A6 File Offset: 0x0001F9A6
		// (set) Token: 0x060008E2 RID: 2274 RVA: 0x000217AE File Offset: 0x0001F9AE
		[DataSourceProperty]
		public MPLobbyBadgeSelectionPopupVM BadgeSelectionPopup
		{
			get
			{
				return this._badgeSelectionPopup;
			}
			set
			{
				if (value != this._badgeSelectionPopup)
				{
					this._badgeSelectionPopup = value;
					base.OnPropertyChangedWithValue<MPLobbyBadgeSelectionPopupVM>(value, "BadgeSelectionPopup");
				}
			}
		}

		// Token: 0x170002B5 RID: 693
		// (get) Token: 0x060008E3 RID: 2275 RVA: 0x000217CC File Offset: 0x0001F9CC
		// (set) Token: 0x060008E4 RID: 2276 RVA: 0x000217D4 File Offset: 0x0001F9D4
		[DataSourceProperty]
		public MPLobbyHomeChangeSigilPopupVM ChangeSigilPopup
		{
			get
			{
				return this._changeSigilPopup;
			}
			set
			{
				if (value != this._changeSigilPopup)
				{
					this._changeSigilPopup = value;
					base.OnPropertyChangedWithValue<MPLobbyHomeChangeSigilPopupVM>(value, "ChangeSigilPopup");
				}
			}
		}

		// Token: 0x170002B6 RID: 694
		// (get) Token: 0x060008E5 RID: 2277 RVA: 0x000217F2 File Offset: 0x0001F9F2
		// (set) Token: 0x060008E6 RID: 2278 RVA: 0x000217FA File Offset: 0x0001F9FA
		[DataSourceProperty]
		public MPLobbyRecentGamesVM RecentGames
		{
			get
			{
				return this._recentGames;
			}
			set
			{
				if (value != this._recentGames)
				{
					this._recentGames = value;
					base.OnPropertyChangedWithValue<MPLobbyRecentGamesVM>(value, "RecentGames");
				}
			}
		}

		// Token: 0x170002B7 RID: 695
		// (get) Token: 0x060008E7 RID: 2279 RVA: 0x00021818 File Offset: 0x0001FA18
		// (set) Token: 0x060008E8 RID: 2280 RVA: 0x00021820 File Offset: 0x0001FA20
		[DataSourceProperty]
		public MPLobbyRankProgressInformationVM RankProgressInformation
		{
			get
			{
				return this._rankProgressInformation;
			}
			set
			{
				if (value != this._rankProgressInformation)
				{
					this._rankProgressInformation = value;
					base.OnPropertyChangedWithValue<MPLobbyRankProgressInformationVM>(value, "RankProgressInformation");
				}
			}
		}

		// Token: 0x170002B8 RID: 696
		// (get) Token: 0x060008E9 RID: 2281 RVA: 0x0002183E File Offset: 0x0001FA3E
		// (set) Token: 0x060008EA RID: 2282 RVA: 0x00021846 File Offset: 0x0001FA46
		[DataSourceProperty]
		public MPLobbyRankLeaderboardVM RankLeaderboard
		{
			get
			{
				return this._rankLeaderboard;
			}
			set
			{
				if (value != this._rankLeaderboard)
				{
					this._rankLeaderboard = value;
					base.OnPropertyChangedWithValue<MPLobbyRankLeaderboardVM>(value, "RankLeaderboard");
				}
			}
		}

		// Token: 0x060008EB RID: 2283 RVA: 0x00021864 File Offset: 0x0001FA64
		public static string GetLocalizedGameTypesString(string[] gameTypes)
		{
			if (gameTypes.Length == 0)
			{
				return GameTexts.FindText("str_multiplayer_official_game_type_name", "None").ToString();
			}
			string text = "";
			for (int i = 0; i < gameTypes.Length; i++)
			{
				text += GameTexts.FindText("str_multiplayer_official_game_type_name", gameTypes[i]).ToString();
				if (i != gameTypes.Length - 1)
				{
					text += ", ";
				}
			}
			return text;
		}

		// Token: 0x060008EC RID: 2284 RVA: 0x000218CC File Offset: 0x0001FACC
		public static string GetLocalizedRankName(string rankID)
		{
			uint num = <PrivateImplementationDetails>.ComputeStringHash(rankID);
			if (num <= 1440162539U)
			{
				if (num <= 1082301734U)
				{
					if (num != 616112491U)
					{
						if (num != 1048746496U)
						{
							if (num == 1082301734U)
							{
								if (rankID == "bronze3")
								{
									return new TextObject("{=Xsq7z0PG}Bronze III", null).ToString();
								}
							}
						}
						else if (rankID == "bronze1")
						{
							return new TextObject("{=CacPs8hA}Bronze I", null).ToString();
						}
					}
					else if (rankID == "general")
					{
						return new TextObject("{=8byRf9zO}General", null).ToString();
					}
				}
				else if (num <= 1099079353U)
				{
					if (num != 1087912039U)
					{
						if (num == 1099079353U)
						{
							if (rankID == "bronze2")
							{
								return new TextObject("{=e3IeNR9W}Bronze II", null).ToString();
							}
						}
					}
					else if (rankID == "conqueror")
					{
						return new TextObject("{=wwbIcqsq}Conqueror", null).ToString();
					}
				}
				else if (num != 1412953442U)
				{
					if (num == 1440162539U)
					{
						if (rankID == "captain")
						{
							return new TextObject("{=F70rOpkK}Captain", null).ToString();
						}
					}
				}
				else if (rankID == "sergeant")
				{
					return new TextObject("{=g9VIbA9s}Sergeant", null).ToString();
				}
			}
			else if (num <= 2260997974U)
			{
				if (num != 2166136261U)
				{
					if (num != 2244220355U)
					{
						if (num == 2260997974U)
						{
							if (rankID == "silver2")
							{
								return new TextObject("{=zpAamvDv}Silver II", null).ToString();
							}
						}
					}
					else if (rankID == "silver1")
					{
						return new TextObject("{=DUrxKsJj}Silver I", null).ToString();
					}
				}
				else if (rankID != null)
				{
					if (rankID.Length == 0)
					{
						return new TextObject("{=E3Bqugs0}Unranked", null).ToString();
					}
				}
			}
			else if (num <= 3320148192U)
			{
				if (num != 2277775593U)
				{
					if (num == 3320148192U)
					{
						if (rankID == "gold3")
						{
							return new TextObject("{=0FVlAbbJ}Gold III", null).ToString();
						}
					}
				}
				else if (rankID == "silver3")
				{
					return new TextObject("{=HGqvwRJt}Silver III", null).ToString();
				}
			}
			else if (num != 3336925811U)
			{
				if (num == 3353703430U)
				{
					if (rankID == "gold1")
					{
						return new TextObject("{=2faDtaGz}Gold I", null).ToString();
					}
				}
			}
			else if (rankID == "gold2")
			{
				return new TextObject("{=9hJtoWot}Gold II", null).ToString();
			}
			return string.Empty;
		}

		// Token: 0x0400042C RID: 1068
		private LobbyClient _lobbyClient;

		// Token: 0x0400042D RID: 1069
		private LobbyState _lobbyState;

		// Token: 0x0400042E RID: 1070
		private Action _onForceCloseFacegen;

		// Token: 0x0400042F RID: 1071
		private Action<KeyOptionVM> _onKeybindRequest;

		// Token: 0x04000430 RID: 1072
		private readonly Action<bool> _setNavigationRestriction;

		// Token: 0x04000431 RID: 1073
		private const float PlayerCountInQueueTimerInterval = 5f;

		// Token: 0x04000432 RID: 1074
		private float _playerCountInQueueTimer;

		// Token: 0x04000434 RID: 1076
		private MBQueue<MPLobbyPartyPlayerSuggestionPopupVM.PlayerPartySuggestionData> _partySuggestionQueue;

		// Token: 0x04000435 RID: 1077
		private ConcurrentQueue<ValueTuple<MPLobbyVM.PartyActionType, PlayerId, PartyRemoveReason>> _partyActionQueue;

		// Token: 0x04000436 RID: 1078
		private bool _isDisconnecting;

		// Token: 0x04000437 RID: 1079
		private bool _isRejoinRequested;

		// Token: 0x04000438 RID: 1080
		private bool _isRejoining;

		// Token: 0x04000439 RID: 1081
		private bool _isStartingGameFind;

		// Token: 0x0400043A RID: 1082
		private bool? _cachedHasUserGeneratedContentPrivilege;

		// Token: 0x0400043B RID: 1083
		private bool _isLoggedIn;

		// Token: 0x0400043C RID: 1084
		private bool _isArmoryActive;

		// Token: 0x0400043D RID: 1085
		private bool _isSearchGameRequested;

		// Token: 0x0400043E RID: 1086
		private bool _isSearchingGame;

		// Token: 0x0400043F RID: 1087
		private bool _isMatchmakingEnabled;

		// Token: 0x04000440 RID: 1088
		private bool _isCustomGameFindEnabled;

		// Token: 0x04000441 RID: 1089
		private bool _isPartyLeader;

		// Token: 0x04000442 RID: 1090
		private bool _isInParty;

		// Token: 0x04000443 RID: 1091
		private MPLobbyBlockerStateVM _blockerState;

		// Token: 0x04000444 RID: 1092
		private MPLobbyMenuVM _menu;

		// Token: 0x04000445 RID: 1093
		private MPAuthenticationVM _login;

		// Token: 0x04000446 RID: 1094
		private MPLobbyRejoinVM _rejoin;

		// Token: 0x04000447 RID: 1095
		private MPLobbyFriendsVM _friends;

		// Token: 0x04000448 RID: 1096
		private MPLobbyHomeVM _home;

		// Token: 0x04000449 RID: 1097
		private MPMatchmakingVM _matchmaking;

		// Token: 0x0400044A RID: 1098
		private MPArmoryVM _armory;

		// Token: 0x0400044B RID: 1099
		private MPLobbyGameSearchVM _gameSearch;

		// Token: 0x0400044C RID: 1100
		private MPLobbyPlayerProfileVM _playerProfile;

		// Token: 0x0400044D RID: 1101
		private MPAfterBattlePopupVM _afterBattlePopup;

		// Token: 0x0400044E RID: 1102
		private MPLobbyPartyInvitationPopupVM _partyInvitationPopup;

		// Token: 0x0400044F RID: 1103
		private MPLobbyPopupVM _popup;

		// Token: 0x04000450 RID: 1104
		private MPLobbyPartyPlayerSuggestionPopupVM _partyPlayerSuggestionPopup;

		// Token: 0x04000451 RID: 1105
		private MPOptionsVM _options;

		// Token: 0x04000452 RID: 1106
		private MPLobbyProfileVM _profile;

		// Token: 0x04000453 RID: 1107
		private BrightnessOptionVM _brightnessPopup;

		// Token: 0x04000454 RID: 1108
		private ExposureOptionVM _exposurePopup;

		// Token: 0x04000455 RID: 1109
		private MPLobbyClanVM _clan;

		// Token: 0x04000456 RID: 1110
		private MPLobbyClanCreationPopupVM _clanCreationPopup;

		// Token: 0x04000457 RID: 1111
		private MPLobbyClanCreationInformationVM _clanCreationInformationPopup;

		// Token: 0x04000458 RID: 1112
		private MPLobbyClanInvitationPopupVM _clanInvitationPopup;

		// Token: 0x04000459 RID: 1113
		private MPLobbyClanMatchmakingRequestPopupVM _clanMatchmakingRequestPopup;

		// Token: 0x0400045A RID: 1114
		private MPLobbyClanInviteFriendsPopupVM _clanInviteFriendsPopup;

		// Token: 0x0400045B RID: 1115
		private MPLobbyClanLeaderboardVM _clanLeaderboardPopup;

		// Token: 0x0400045C RID: 1116
		private MPCosmeticObtainPopupVM _cosmeticObtainPopup;

		// Token: 0x0400045D RID: 1117
		private MPLobbyBannerlordIDChangePopup _bannerlordIDChangePopup;

		// Token: 0x0400045E RID: 1118
		private MPLobbyBannerlordIDAddFriendPopupVM _bannerlordIDAddFriendPopup;

		// Token: 0x0400045F RID: 1119
		private MPLobbyBadgeProgressInformationVM _badgeProgressionInformation;

		// Token: 0x04000460 RID: 1120
		private MPLobbyBadgeSelectionPopupVM _badgeSelectionPopup;

		// Token: 0x04000461 RID: 1121
		private MPLobbyHomeChangeSigilPopupVM _changeSigilPopup;

		// Token: 0x04000462 RID: 1122
		private MPLobbyRecentGamesVM _recentGames;

		// Token: 0x04000463 RID: 1123
		private MPLobbyRankProgressInformationVM _rankProgressInformation;

		// Token: 0x04000464 RID: 1124
		private MPLobbyRankLeaderboardVM _rankLeaderboard;

		// Token: 0x02000180 RID: 384
		public enum LobbyPage
		{
			// Token: 0x04000CC1 RID: 3265
			NotAssigned,
			// Token: 0x04000CC2 RID: 3266
			Authentication,
			// Token: 0x04000CC3 RID: 3267
			Rejoin,
			// Token: 0x04000CC4 RID: 3268
			Options,
			// Token: 0x04000CC5 RID: 3269
			Home,
			// Token: 0x04000CC6 RID: 3270
			Armory,
			// Token: 0x04000CC7 RID: 3271
			Matchmaking,
			// Token: 0x04000CC8 RID: 3272
			Profile,
			// Token: 0x04000CC9 RID: 3273
			HotkeySelectablePageBegin = 3,
			// Token: 0x04000CCA RID: 3274
			HotkeySelectablePageEnd = 7
		}

		// Token: 0x02000181 RID: 385
		private enum PartyActionType
		{
			// Token: 0x04000CCC RID: 3276
			Add,
			// Token: 0x04000CCD RID: 3277
			Remove,
			// Token: 0x04000CCE RID: 3278
			AssignLeader
		}
	}
}
