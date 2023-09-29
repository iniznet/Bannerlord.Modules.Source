using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection.Lobby.AfterBattle;
using TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection.Lobby.Armory;
using TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection.Lobby.Armory.CosmeticItem;
using TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection.Lobby.Authentication;
using TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection.Lobby.Clan;
using TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection.Lobby.CustomGame;
using TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection.Lobby.Friends;
using TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection.Lobby.Home;
using TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection.Lobby.OfficialGame;
using TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection.Lobby.Profile;
using TaleWorlds.MountAndBlade.ViewModelCollection.GameOptions;
using TaleWorlds.PlatformService;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection.Lobby
{
	public class MPLobbyVM : ViewModel
	{
		private PlayerId _partyLeaderId
		{
			get
			{
				return NetworkMain.GameClient.PlayersInParty.Find((PartyPlayerInLobbyClient p) => p.IsPartyLeader).PlayerId;
			}
		}

		public MPLobbyVM.LobbyPage CurrentPage { get; private set; }

		public List<MPLobbyVM.LobbyPage> DisallowedPages { get; private set; }

		public MPLobbyVM(LobbyState lobbyState, Action<BasicCharacterObject> onOpenFacegen, Action onForceCloseFacegen, Action<KeyOptionVM> onKeybindRequest, Func<string> getContinueKeyText, Action<bool> setNavigationRestriction)
		{
			this.CurrentPage = MPLobbyVM.LobbyPage.NotAssigned;
			this._onKeybindRequest = onKeybindRequest;
			this._onForceCloseFacegen = onForceCloseFacegen;
			this._setNavigationRestriction = setNavigationRestriction;
			this._lobbyState = lobbyState;
			this._lobbyClient = this._lobbyState.LobbyClient;
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
			this.Armory = new MPArmoryVM(onOpenFacegen, new Action<MPArmoryCosmeticItemBaseVM>(this.OnItemObtainRequested), getContinueKeyText);
			this.Friends = new MPLobbyFriendsVM();
			this.GameSearch = new MPLobbyGameSearchVM();
			this.PlayerProfile = new MPLobbyPlayerProfileVM(lobbyState);
			this.AfterBattlePopup = new MPAfterBattlePopupVM(getContinueKeyText);
			this.PartyInvitationPopup = new MPLobbyPartyInvitationPopupVM();
			this.PartyJoinRequestPopup = new MPLobbyPartyJoinRequestPopupVM();
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
			this.InitializeCallbacks();
			this.RefreshValues();
		}

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

		private void OnUserGeneratedContentPrivilegeUpdated(bool hasPrivilege)
		{
			bool? cachedHasUserGeneratedContentPrivilege = this._cachedHasUserGeneratedContentPrivilege;
			if (!((cachedHasUserGeneratedContentPrivilege.GetValueOrDefault() == hasPrivilege) & (cachedHasUserGeneratedContentPrivilege != null)))
			{
				this.OnFriendListUpdated(true);
			}
		}

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
			this.Login.SetDoneInputKey(doneInputKey);
			this.Login.SetCancelInputKey(cancelInputKey);
			this.CosmeticObtainPopup.SetDoneInputKey(doneInputKey);
			this.Popup.SetDoneInputKey(doneInputKey);
			this.Popup.SetCancelInputKey(cancelInputKey);
		}

		public override void OnFinalize()
		{
			this.FinalizeCallbacks();
			this.Clan.ClanOverview.ChangeSigilPopup.OnFinalize();
			this.Clan.ClanOverview.ChangeFactionPopup.OnFinalize();
			this.Clan.ClanOverview.SendAnnouncementPopup.OnFinalize();
			this.BannerlordIDAddFriendPopup.OnFinalize();
			this.BannerlordIDChangePopup.OnFinalize();
			this.RankLeaderboard.OnFinalize();
			this.ChangeSigilPopup.OnFinalize();
			this.ClanCreationPopup.OnFinalize();
			this.BadgeSelectionPopup.OnFinalize();
			this.CosmeticObtainPopup.OnFinalize();
			InformationManager.ClearAllMessages();
			this.Login.OnFinalize();
			this.Armory.OnFinalize();
			this.Matchmaking.OnFinalize();
			this.Friends.OnFinalize();
			this.Home.OnFindGameRequested -= this.AutoFindGameRequested;
			this.Profile.OnFindGameRequested -= this.AutoFindGameRequested;
			if (this._lobbyState != null)
			{
				this._lobbyState.UnregisterForCustomServerAction(new Func<GameServerEntry, List<CustomServerAction>>(this.OnServerActionRequested));
				LobbyState lobbyState = this._lobbyState;
				lobbyState.OnUserGeneratedContentPrivilegeUpdated = (Action<bool>)Delegate.Remove(lobbyState.OnUserGeneratedContentPrivilegeUpdated, new Action<bool>(this.OnUserGeneratedContentPrivilegeUpdated));
			}
			MPLobbyRejoinVM rejoin = this.Rejoin;
			rejoin.OnRejoinRequested = (Action)Delegate.Remove(rejoin.OnRejoinRequested, new Action(this.OnRejoinRequested));
			this.Menu.OnFinalize();
			this.Home.OnFinalize();
			this._lobbyState = null;
			base.OnFinalize();
		}

		private void InitializeCallbacks()
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
			MPArmoryCosmeticItemBaseVM.OnPurchaseRequested += this.OnItemObtainRequested;
		}

		private void FinalizeCallbacks()
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
			MPArmoryCosmeticItemBaseVM.OnPurchaseRequested -= this.OnItemObtainRequested;
		}

		public void OnActivate()
		{
			this._isRejoining = false;
			this._isDisconnecting = false;
		}

		public void OnDeactivate()
		{
			this._isRejoining = false;
		}

		public void OnTick(float dt)
		{
			this.IsLoggedIn = NetworkMain.GameClient.LoggedIn;
			this.Login.OnTick(dt);
			this.Friends.OnTick(dt);
			this.Armory.OnTick(dt);
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
			if (!this.PartyPlayerSuggestionPopup.IsEnabled && !Extensions.IsEmpty<MPLobbyPartyPlayerSuggestionPopupVM.PlayerPartySuggestionData>(this._partySuggestionQueue) && !this._lobbyClient.IsPartyFull)
			{
				MPLobbyPartyPlayerSuggestionPopupVM.PlayerPartySuggestionData playerPartySuggestionData = this._partySuggestionQueue.Dequeue();
				this.PartyPlayerSuggestionPopup.OpenWith(playerPartySuggestionData);
			}
			else if (!Extensions.IsEmpty<MPLobbyPartyPlayerSuggestionPopupVM.PlayerPartySuggestionData>(this._partySuggestionQueue) && this._lobbyClient.IsPartyFull)
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
			if (this._playerDataToRefreshWith != null && !NetworkMain.GameClient.IsRefreshingPlayerData)
			{
				this.RefreshPlayerDataInternal();
			}
			else if (this._playerDataToRefreshWith == null && NetworkMain.GameClient.IsRefreshingPlayerData)
			{
				this._playerDataToRefreshWith = null;
				NetworkMain.GameClient.IsRefreshingPlayerData = false;
			}
			this.Matchmaking.OnTick(dt);
			this.GameSearch.OnTick(dt);
			this.Armory.Cosmetics.OnTick(dt);
		}

		public void OnConfirm()
		{
			if (this.Login.IsEnabled)
			{
				SoundEvent.PlaySound2D("event:/ui/default");
				this.Login.ExecuteLogin();
				return;
			}
			if (this.Options.IsEnabled)
			{
				SoundEvent.PlaySound2D("event:/ui/default");
				this.Options.ExecuteApply();
				return;
			}
			if (this.Popup.IsEnabled)
			{
				if (this.Popup.IsInquiry)
				{
					SoundEvent.PlaySound2D("event:/ui/default");
					this.Popup.ExecuteAccept();
					return;
				}
			}
			else
			{
				if (this.ExposurePopup.Visible)
				{
					SoundEvent.PlaySound2D("event:/ui/default");
					this.ExposurePopup.ExecuteConfirm();
					return;
				}
				if (this.BrightnessPopup.Visible)
				{
					SoundEvent.PlaySound2D("event:/ui/default");
					this.BrightnessPopup.ExecuteConfirm();
					return;
				}
				if (this.ChangeSigilPopup.IsEnabled && !this.CosmeticObtainPopup.IsEnabled)
				{
					SoundEvent.PlaySound2D("event:/ui/default");
					this.ChangeSigilPopup.ExecuteChangeSigil();
					return;
				}
				if (this.CosmeticObtainPopup.IsEnabled)
				{
					if (this.CosmeticObtainPopup.ObtainState == 0 && this.CosmeticObtainPopup.CanObtain)
					{
						SoundEvent.PlaySound2D("event:/ui/multiplayer/shop_purchase_proceed");
						this.CosmeticObtainPopup.ExecuteAction();
						return;
					}
				}
				else
				{
					if (this.BannerlordIDAddFriendPopup.IsSelected)
					{
						SoundEvent.PlaySound2D("event:/ui/default");
						this.BannerlordIDAddFriendPopup.ExecuteTryAddFriend();
						return;
					}
					if (this.BannerlordIDChangePopup.IsSelected)
					{
						SoundEvent.PlaySound2D("event:/ui/default");
						this.BannerlordIDChangePopup.ExecuteApply();
						return;
					}
					if (this.Clan.ClanOverview.ChangeSigilPopup.IsSelected)
					{
						SoundEvent.PlaySound2D("event:/ui/default");
						this.Clan.ClanOverview.ChangeSigilPopup.ExecuteChangeSigil();
						return;
					}
					if (this.Clan.ClanOverview.ChangeFactionPopup.IsSelected)
					{
						SoundEvent.PlaySound2D("event:/ui/default");
						this.Clan.ClanOverview.ChangeFactionPopup.ExecuteChangeFaction();
						return;
					}
					if (this.Clan.ClanOverview.SetClanInformationPopup.IsSelected)
					{
						SoundEvent.PlaySound2D("event:/ui/default");
						this.Clan.ClanOverview.SetClanInformationPopup.ExecuteSend();
						return;
					}
					if (this.Clan.ClanOverview.SendAnnouncementPopup.IsSelected)
					{
						SoundEvent.PlaySound2D("event:/ui/default");
						this.Clan.ClanOverview.SendAnnouncementPopup.ExecuteSend();
					}
				}
			}
		}

		public async void OnEscape()
		{
			if (!this._waitingForEscapeResult)
			{
				this._waitingForEscapeResult = true;
				if (this.CurrentPage == MPLobbyVM.LobbyPage.Authentication)
				{
					SoundEvent.PlaySound2D("event:/ui/default");
					if (this.HasNoPopupOpen())
					{
						this.Login.ExecuteExit();
					}
					else
					{
						this.ForceClosePopups();
					}
				}
				else if (this.Popup.IsEnabled)
				{
					SoundEvent.PlaySound2D("event:/ui/default");
					this.Popup.ExecuteDecline();
				}
				else if (this.ExposurePopup.Visible)
				{
					SoundEvent.PlaySound2D("event:/ui/default");
					this.ExposurePopup.ExecuteCancel();
				}
				else if (this.BrightnessPopup.Visible)
				{
					SoundEvent.PlaySound2D("event:/ui/default");
					this.BrightnessPopup.ExecuteCancel();
				}
				else if (this.ClanCreationPopup.IsEnabled)
				{
					SoundEvent.PlaySound2D("event:/ui/default");
					this.ClanCreationPopup.ExecuteClosePopup();
				}
				else if (this.CosmeticObtainPopup.IsEnabled)
				{
					SoundEvent.PlaySound2D("event:/ui/default");
					this.CosmeticObtainPopup.ExecuteClosePopup();
				}
				else if (this.Friends.IsPlayerActionsActive)
				{
					SoundEvent.PlaySound2D("event:/ui/default");
					this.Friends.IsPlayerActionsActive = false;
				}
				else if (this.RankLeaderboard.IsPlayerActionsActive)
				{
					SoundEvent.PlaySound2D("event:/ui/default");
					this.RankLeaderboard.IsPlayerActionsActive = false;
				}
				else if (this.RecentGames.IsPlayerActionsActive)
				{
					SoundEvent.PlaySound2D("event:/ui/default");
					this.RecentGames.IsPlayerActionsActive = false;
				}
				else if (this.Armory.IsManagingTaunts)
				{
					SoundEvent.PlaySound2D("event:/ui/default");
					if (this.Armory.Cosmetics.SelectedTauntItem != null || this.Armory.Cosmetics.SelectedTauntSlot != null)
					{
						this.Armory.ExecuteClearTauntSelection();
					}
					else
					{
						this.Armory.ExecuteToggleManageTauntsState();
					}
				}
				else if (NetworkMain.GameClient.IsRefreshingPlayerData)
				{
					SoundEvent.PlaySound2D("event:/ui/default");
					InformationManager.ShowInquiry(new InquiryData(GameTexts.FindText("str_exit", null).ToString(), new TextObject("{=usLhlY2j}Please wait until player data is downloaded.", null).ToString(), true, false, GameTexts.FindText("str_ok", null).ToString(), null, null, null, "", 0f, null, null, null), false, false);
				}
				else if (this.HasAnyContextMenuOpen())
				{
					SoundEvent.PlaySound2D("event:/ui/default");
					this.ForceCloseContextMenus();
				}
				else if (this.HasNoPopupOpen())
				{
					SoundEvent.PlaySound2D("event:/ui/sort");
					await this.RequestExit();
				}
				else
				{
					SoundEvent.PlaySound2D("event:/ui/default");
					this.ForceClosePopups();
				}
				this._waitingForEscapeResult = false;
			}
		}

		public bool HasAnyContextMenuOpen()
		{
			return this.Clan.ClanRoster.IsMemberActionsActive || this.Friends.IsPlayerActionsActive || this.RankLeaderboard.IsPlayerActionsActive || this.RecentGames.IsPlayerActionsActive;
		}

		public void ForceCloseContextMenus()
		{
			this.Clan.ClanRoster.IsMemberActionsActive = false;
			this.Friends.IsPlayerActionsActive = false;
			this.RankLeaderboard.IsPlayerActionsActive = false;
			this.RecentGames.IsPlayerActionsActive = false;
		}

		public bool HasNoPopupOpen()
		{
			return !this.Clan.IsEnabled && !this.Clan.ClanOverview.ChangeFactionPopup.IsSelected && !this.Clan.ClanOverview.ChangeSigilPopup.IsSelected && !this.Clan.ClanOverview.SendAnnouncementPopup.IsSelected && !this.Clan.ClanOverview.SetClanInformationPopup.IsSelected && !this.PartyInvitationPopup.IsEnabled && !this.PartyPlayerSuggestionPopup.IsEnabled && !this.ClanInvitationPopup.IsEnabled && !this.ClanMatchmakingRequestPopup.IsEnabled && !this.BannerlordIDChangePopup.IsSelected && !this.BannerlordIDAddFriendPopup.IsSelected && !this.CosmeticObtainPopup.IsEnabled && !this.AfterBattlePopup.IsEnabled && !this.ClanCreationPopup.IsEnabled && !this.ClanCreationInformationPopup.IsEnabled && !this.ClanInviteFriendsPopup.IsEnabled && !this.ClanLeaderboardPopup.IsEnabled && !this.BadgeProgressionInformation.IsEnabled && !this.BadgeSelectionPopup.IsEnabled && !this.ChangeSigilPopup.IsEnabled && !this.RecentGames.IsEnabled && !this.PlayerProfile.IsEnabled && !this.RankProgressInformation.IsEnabled && !this.RankLeaderboard.IsEnabled && !this.ExposurePopup.Visible && !this.BrightnessPopup.Visible && !this.Popup.IsEnabled;
		}

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

		public async Task RequestExit()
		{
			if (NetworkMain.GameClient.CurrentState != 15 && NetworkMain.GameClient.CurrentState != 12 && NetworkMain.GameClient.CurrentState != 2 && NetworkMain.GameClient.CurrentState != 3 && NetworkMain.GameClient.CurrentState != 1)
			{
				while (NetworkMain.GameClient.CurrentState == 6 || NetworkMain.GameClient.CurrentState == 7)
				{
					await Task.Yield();
				}
				if (NetworkMain.GameClient.CurrentState == 8)
				{
					if (!NetworkMain.GameClient.IsInParty || NetworkMain.GameClient.IsPartyLeader)
					{
						NetworkMain.GameClient.CancelFindGame();
					}
				}
				else if (!this._isDisconnecting)
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

		private void OnExit()
		{
			this._isDisconnecting = true;
			Action<bool> setNavigationRestriction = this._setNavigationRestriction;
			if (setNavigationRestriction != null)
			{
				setNavigationRestriction(true);
			}
			NetworkMain.GameClient.Logout(null);
		}

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

		public void ShowOptionsChangedInquiry(Action onAccept = null, Action onDecline = null)
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

		public void SetPage(MPLobbyVM.LobbyPage lobbyPage, MPMatchmakingVM.MatchmakingSubPages matchmakingSubPage = MPMatchmakingVM.MatchmakingSubPages.Default)
		{
			if (this.CurrentPage != lobbyPage)
			{
				if (lobbyPage != MPLobbyVM.LobbyPage.Authentication && this.CurrentPage == MPLobbyVM.LobbyPage.Options && this.Options.IsOptionsChanged())
				{
					this.ShowOptionsChangedInquiry(null, null);
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
				this._isDisconnecting = false;
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

		private async void RefreshRecentGames()
		{
			MBReadOnlyList<MatchInfo> mbreadOnlyList = await MatchHistory.GetMatches();
			if (mbreadOnlyList != null)
			{
				this.Profile.RefreshRecentGames(mbreadOnlyList);
				this.RecentGames.RefreshData(mbreadOnlyList);
			}
		}

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

		private void AutoFindGameRequested()
		{
			this.Matchmaking.ExecuteAutoFindGame();
		}

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

		public void OnRejoinBattleRequestAnswered(bool isSuccessful)
		{
			if (isSuccessful && this._isRejoinRequested)
			{
				this._isRejoining = true;
				return;
			}
			this.SetPage(MPLobbyVM.LobbyPage.Home, MPMatchmakingVM.MatchmakingSubPages.Default);
		}

		public void OnRequestedToSearchBattle()
		{
			this.IsSearchGameRequested = true;
		}

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

		public void OnRequestedToCancelSearchBattle()
		{
			this.GameSearch.OnRequestedToCancelSearchBattle();
		}

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
			if (playerID == NetworkMain.GameClient.PlayerData.PlayerId && reason != 3)
			{
				this.IsPartyLeader = false;
				this.IsInParty = false;
				TextObject textObject = GameTexts.FindText("str_youve_been_removed", null);
				if (reason == 1)
				{
					textObject = GameTexts.FindText("str_left_party", null);
				}
				else if (reason == 2)
				{
					textObject = GameTexts.FindText("str_party_disbanded", null);
				}
				else if (reason == 4)
				{
					textObject = GameTexts.FindText("str_party_join_declined", null);
				}
				else if (reason == 5)
				{
					textObject = GameTexts.FindText("str_party_join_permission_failed", null);
				}
				InformationManager.ShowInquiry(new InquiryData(string.Empty, textObject.ToString(), true, false, GameTexts.FindText("str_ok", null).ToString(), string.Empty, null, null, "", 0f, null, null, null), false, false);
			}
			this.GameSearch.UpdateCanCancel();
		}

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

		public void OnPlayerRemovedFromParty(PlayerId playerId, PartyRemoveReason reason)
		{
			this._partyActionQueue.Enqueue(new ValueTuple<MPLobbyVM.PartyActionType, PlayerId, PartyRemoveReason>(MPLobbyVM.PartyActionType.Remove, playerId, reason));
		}

		public void OnPlayerAddedToParty(PlayerId playerId)
		{
			this._partyActionQueue.Enqueue(new ValueTuple<MPLobbyVM.PartyActionType, PlayerId, PartyRemoveReason>(MPLobbyVM.PartyActionType.Add, playerId, 0));
		}

		public void OnPlayerAssignedPartyLeader(PlayerId newPartyLeaderId)
		{
			this._partyActionQueue.Enqueue(new ValueTuple<MPLobbyVM.PartyActionType, PlayerId, PartyRemoveReason>(MPLobbyVM.PartyActionType.AssignLeader, newPartyLeaderId, 0));
		}

		private void HandlePlayerAssignedPartyLeader(PlayerId playerID)
		{
			this.Friends.OnPlayerAssignedPartyLeader();
			this.IsInParty = NetworkMain.GameClient.IsInParty;
			this.IsPartyLeader = NetworkMain.GameClient.IsPartyLeader;
			this.GameSearch.UpdateCanCancel();
			this.Friends.UpdateCanInviteOtherPlayersToParty();
		}

		public void OnPlayerSuggestedToParty(PlayerId playerId, string playerName, PlayerId suggestingPlayerId, string suggestingPlayerName)
		{
			this._partySuggestionQueue.Enqueue(new MPLobbyPartyPlayerSuggestionPopupVM.PlayerPartySuggestionData(playerId, playerName, suggestingPlayerId, suggestingPlayerName));
		}

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

		public void OnSearchBattleCanceled()
		{
			this.IsSearchGameRequested = false;
			this.IsSearchingGame = false;
			this.Clan.ClanOverview.AreActionButtonsEnabled = true;
			this.GameSearch.SetEnabled(false);
			this.Armory.SetCanOpenFacegen(true);
			this.Matchmaking.OnCancelFindingGame();
		}

		private async void RefreshPlayerDataInternal()
		{
			NetworkMain.GameClient.IsRefreshingPlayerData = true;
			if (NetworkMain.GameClient.Connected)
			{
				await NetworkMain.GameClient.GetCosmeticsInfo();
				this.Armory.RefreshPlayerData(this._playerDataToRefreshWith);
				this.Friends.Player.UpdateWith(this._playerDataToRefreshWith);
				this.BadgeSelectionPopup.RefreshPlayerData(this._playerDataToRefreshWith);
				this.Matchmaking.RefreshPlayerData(this._playerDataToRefreshWith);
			}
			if (NetworkMain.GameClient.Connected)
			{
				await NetworkMain.GameClient.GetClanHomeInfo();
				this.Home.RefreshPlayerData(this._playerDataToRefreshWith, true);
				this.Profile.UpdatePlayerData(this._playerDataToRefreshWith, true, true);
			}
			this._playerDataToRefreshWith = null;
			NetworkMain.GameClient.IsRefreshingPlayerData = false;
			this.RefreshSupportedFeatures();
		}

		public void RefreshPlayerData(PlayerData playerData)
		{
			if (playerData != null)
			{
				if (this._playerDataToRefreshWith != playerData)
				{
					this._playerDataToRefreshWith = playerData;
					return;
				}
			}
			else
			{
				this.SetPage(MPLobbyVM.LobbyPage.Authentication, MPMatchmakingVM.MatchmakingSubPages.Default);
			}
		}

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

		private void UpdateBlockerState()
		{
			LobbyClient.State currentState = this._lobbyClient.CurrentState;
			bool flag = NetworkMain.GameClient.IsRefreshingPlayerData || this._isDisconnecting || this._isRejoining || this._isStartingGameFind || currentState == 9 || currentState == 10 || currentState == 13 || currentState == 14 || currentState == 15 || currentState == 16;
			if (flag && !this.BlockerState.IsEnabled)
			{
				TextObject textObject = new TextObject("{=Rc95Kq8r}Please wait...", null);
				this.BlockerState.OnLobbyStateIsBlocker(textObject);
				return;
			}
			if (!flag && this.BlockerState.IsEnabled)
			{
				this.BlockerState.OnLobbyStateNotBlocker();
			}
		}

		private void OnChangePageRequest(MPLobbyVM.LobbyPage page)
		{
			this.SetPage(page, MPMatchmakingVM.MatchmakingSubPages.Default);
		}

		private void OnMatchSelectionChanged(string selectionInfo, bool isMatchFindPossible)
		{
			this.Home.OnMatchSelectionChanged(selectionInfo, isMatchFindPossible);
			this.Profile.OnMatchSelectionChanged(selectionInfo, isMatchFindPossible);
		}

		private void OnGameFindStateChanged(bool isStartingGameFind)
		{
			this._isStartingGameFind = isStartingGameFind;
		}

		private void OnShowPlayerProfile(PlayerId playerID)
		{
			this.PlayerProfile.OpenWith(playerID);
		}

		private void ExecuteShowBrightness()
		{
			this.BrightnessPopup.Visible = true;
		}

		private void ExecuteShowExposure()
		{
			this.ExposurePopup.Visible = true;
		}

		private void OpenClanCreationPopup()
		{
			this.ClanCreationPopup.ExecuteOpenPopup();
		}

		private void CloseClanCreationPopup()
		{
			this.ClanCreationPopup.ExecuteClosePopup();
		}

		private void ExecuteOpenRecentGames()
		{
			this.RecentGames.ExecuteOpenPopup();
		}

		private void OpenInviteClanMemberPopup()
		{
			this.ClanInviteFriendsPopup.Open();
		}

		private void OnBadgeProgressInfoRequested(MPLobbyAchievementBadgeGroupVM achivementGroup)
		{
			this.BadgeProgressionInformation.OpenWith(achivementGroup);
		}

		private void OnBadgeNotificationRead()
		{
			this.Profile.HasBadgeNotification = this.BadgeSelectionPopup.HasNotifications;
			this.Menu.HasProfileNotification = this.BadgeSelectionPopup.HasNotifications;
		}

		private void OnBadgeSelectionUpdated()
		{
			PlayerData playerData = NetworkMain.GameClient.PlayerData;
			if (playerData != null)
			{
				this.Home.RefreshPlayerData(playerData, false);
				this.Profile.UpdatePlayerData(playerData, false, false);
			}
		}

		private void OnBadgeChangeRequested(PlayerId playerID)
		{
			if (playerID == NetworkMain.GameClient.PlayerID)
			{
				this.BadgeSelectionPopup.Open();
			}
		}

		private void OnRankProgressionRequested(MPLobbyPlayerBaseVM player)
		{
			this.RankProgressInformation.OpenWith(player);
		}

		private void OnRankLeaderboardRequested(string gameMode)
		{
			this.RankLeaderboard.OpenWith(gameMode);
		}

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

		private void OnClanLeaderboardRequested()
		{
			this.ClanLeaderboardPopup.ExecuteOpenPopup();
		}

		public void OnNotificationsReceived(LobbyNotification[] notifications)
		{
			List<LobbyNotification> list = notifications.Where((LobbyNotification n) => n.Type == 1).ToList<LobbyNotification>();
			this.Friends.OnFriendRequestNotificationsReceived(list);
			foreach (LobbyNotification lobbyNotification in notifications)
			{
				if (lobbyNotification.Type == 2)
				{
					this.Clan.OnNotificationReceived(lobbyNotification);
				}
				else if (lobbyNotification.Type == null)
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

		private void OnAddFriendWithBannerlordIDRequested(PlayerId playerID)
		{
			if (playerID == NetworkMain.GameClient.PlayerID)
			{
				this.BannerlordIDAddFriendPopup.ExecuteOpenPopup();
			}
		}

		private void OnBannerlordIDChangeRequested(PlayerId playerID)
		{
			if (playerID == NetworkMain.GameClient.PlayerID)
			{
				this.BannerlordIDChangePopup.ExecuteOpenPopup();
			}
		}

		private void OnItemObtainRequested(MPLobbyCosmeticSigilItemVM sigilItem)
		{
			this.CosmeticObtainPopup.OpenWith(sigilItem);
		}

		private void OnItemObtainRequested(MPArmoryCosmeticItemBaseVM cosmeticItem)
		{
			MPArmoryCosmeticClothingItemVM mparmoryCosmeticClothingItemVM;
			if ((mparmoryCosmeticClothingItemVM = cosmeticItem as MPArmoryCosmeticClothingItemVM) != null)
			{
				this.CosmeticObtainPopup.OpenWith(mparmoryCosmeticClothingItemVM);
				return;
			}
			MPArmoryCosmeticTauntItemVM mparmoryCosmeticTauntItemVM;
			if ((mparmoryCosmeticTauntItemVM = cosmeticItem as MPArmoryCosmeticTauntItemVM) != null)
			{
				this.CosmeticObtainPopup.OpenWith(mparmoryCosmeticTauntItemVM, this.Armory.HeroPreview.HeroVisual);
			}
		}

		private void OnItemObtained(string cosmeticID, int finalLoot)
		{
			this.Armory.Cosmetics.OnItemObtained(cosmeticID, finalLoot);
			this.ChangeSigilPopup.OnLootUpdated(finalLoot);
			this.Home.Player.Loot = finalLoot;
		}

		private void OnSigilChangeRequested(PlayerId playerID)
		{
			if (playerID == NetworkMain.GameClient.PlayerID)
			{
				this.ChangeSigilPopup.Open();
			}
		}

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

		public void OnClanCreationFinished()
		{
			this.ClanCreationPopup.IsWaiting = false;
			this.ClanCreationPopup.HasCreationStarted = false;
			this.ClanInvitationPopup.Close();
			this.ClanCreationPopup.ExecuteClosePopup();
			this.RefreshClanInfo();
		}

		public void OnEnableGenericAvatarsChanged()
		{
			this.OnFriendListUpdated(true);
		}

		public void OnEnableGenericNamesChanged()
		{
			this.OnFriendListUpdated(true);
		}

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

		private async void RefreshClanInfo()
		{
			await NetworkMain.GameClient.GetClanHomeInfo();
		}

		private void OnRejoinRequested()
		{
			this._isRejoinRequested = true;
		}

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

		[DataSourceProperty]
		public MPLobbyPartyJoinRequestPopupVM PartyJoinRequestPopup
		{
			get
			{
				return this._partyJoinRequestPopup;
			}
			set
			{
				if (value != this._partyJoinRequestPopup)
				{
					this._partyJoinRequestPopup = value;
					base.OnPropertyChangedWithValue<MPLobbyPartyJoinRequestPopupVM>(value, "PartyJoinRequestPopup");
				}
			}
		}

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
						return new TextObject("{=mprankgeneral}General", null).ToString();
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

		private LobbyClient _lobbyClient;

		private LobbyState _lobbyState;

		private Action _onForceCloseFacegen;

		private Action<KeyOptionVM> _onKeybindRequest;

		private readonly Action<bool> _setNavigationRestriction;

		private const float PlayerCountInQueueTimerInterval = 5f;

		private float _playerCountInQueueTimer;

		private PlayerData _playerDataToRefreshWith;

		private MBQueue<MPLobbyPartyPlayerSuggestionPopupVM.PlayerPartySuggestionData> _partySuggestionQueue;

		private ConcurrentQueue<ValueTuple<MPLobbyVM.PartyActionType, PlayerId, PartyRemoveReason>> _partyActionQueue;

		private bool _waitingForEscapeResult;

		private bool _isDisconnecting;

		private bool _isRejoinRequested;

		private bool _isRejoining;

		private bool _isStartingGameFind;

		private bool? _cachedHasUserGeneratedContentPrivilege;

		private const string _defaultSound = "event:/ui/default";

		private const string _tabSound = "event:/ui/tab";

		private const string _sortSound = "event:/ui/sort";

		private const string _purchaseSound = "event:/ui/multiplayer/shop_purchase_proceed";

		private bool _isLoggedIn;

		private bool _isArmoryActive;

		private bool _isSearchGameRequested;

		private bool _isSearchingGame;

		private bool _isMatchmakingEnabled;

		private bool _isCustomGameFindEnabled;

		private bool _isPartyLeader;

		private bool _isInParty;

		private MPLobbyBlockerStateVM _blockerState;

		private MPLobbyMenuVM _menu;

		private MPAuthenticationVM _login;

		private MPLobbyRejoinVM _rejoin;

		private MPLobbyFriendsVM _friends;

		private MPLobbyHomeVM _home;

		private MPMatchmakingVM _matchmaking;

		private MPArmoryVM _armory;

		private MPLobbyGameSearchVM _gameSearch;

		private MPLobbyPlayerProfileVM _playerProfile;

		private MPAfterBattlePopupVM _afterBattlePopup;

		private MPLobbyPartyInvitationPopupVM _partyInvitationPopup;

		private MPLobbyPartyJoinRequestPopupVM _partyJoinRequestPopup;

		private MPLobbyPopupVM _popup;

		private MPLobbyPartyPlayerSuggestionPopupVM _partyPlayerSuggestionPopup;

		private MPOptionsVM _options;

		private MPLobbyProfileVM _profile;

		private BrightnessOptionVM _brightnessPopup;

		private ExposureOptionVM _exposurePopup;

		private MPLobbyClanVM _clan;

		private MPLobbyClanCreationPopupVM _clanCreationPopup;

		private MPLobbyClanCreationInformationVM _clanCreationInformationPopup;

		private MPLobbyClanInvitationPopupVM _clanInvitationPopup;

		private MPLobbyClanMatchmakingRequestPopupVM _clanMatchmakingRequestPopup;

		private MPLobbyClanInviteFriendsPopupVM _clanInviteFriendsPopup;

		private MPLobbyClanLeaderboardVM _clanLeaderboardPopup;

		private MPCosmeticObtainPopupVM _cosmeticObtainPopup;

		private MPLobbyBannerlordIDChangePopup _bannerlordIDChangePopup;

		private MPLobbyBannerlordIDAddFriendPopupVM _bannerlordIDAddFriendPopup;

		private MPLobbyBadgeProgressInformationVM _badgeProgressionInformation;

		private MPLobbyBadgeSelectionPopupVM _badgeSelectionPopup;

		private MPLobbyHomeChangeSigilPopupVM _changeSigilPopup;

		private MPLobbyRecentGamesVM _recentGames;

		private MPLobbyRankProgressInformationVM _rankProgressInformation;

		private MPLobbyRankLeaderboardVM _rankLeaderboard;

		public enum LobbyPage
		{
			NotAssigned,
			Authentication,
			Rejoin,
			Options,
			Home,
			Armory,
			Matchmaking,
			Profile,
			HotkeySelectablePageBegin = 3,
			HotkeySelectablePageEnd = 7
		}

		private enum PartyActionType
		{
			Add,
			Remove,
			AssignLeader
		}
	}
}
