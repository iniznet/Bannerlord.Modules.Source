using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Generic;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.MountAndBlade.ViewModelCollection.Input;
using TaleWorlds.PlatformService;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.Friends
{
	public class MPLobbyFriendsVM : ViewModel
	{
		private PlayerId? _partyLeaderId
		{
			get
			{
				PartyPlayerInLobbyClient partyPlayerInLobbyClient = NetworkMain.GameClient.PlayersInParty.SingleOrDefault((PartyPlayerInLobbyClient p) => p.IsPartyLeader);
				if (partyPlayerInLobbyClient == null)
				{
					return null;
				}
				return new PlayerId?(partyPlayerInLobbyClient.PlayerId);
			}
		}

		public MPLobbyFriendsVM()
		{
			this.Player = new MPLobbyPartyPlayerVM(NetworkMain.GameClient.PlayerID, new Action<MPLobbyPartyPlayerVM>(this.ActivatePlayerActions));
			this.PartyFriends = new MBBindingList<MPLobbyPartyPlayerVM>();
			this.PlayerActions = new MBBindingList<StringPairItemWithActionVM>();
			this.FriendServices = new MBBindingList<MPLobbyFriendServiceVM>();
			IFriendListService[] friendListServices = PlatformServices.Instance.GetFriendListServices();
			for (int i = 0; i < friendListServices.Length; i++)
			{
				MPLobbyFriendServiceVM mplobbyFriendServiceVM = new MPLobbyFriendServiceVM(friendListServices[i], new Action<PlayerId>(this.OnFriendRequestAnswered), new Action<MPLobbyPlayerBaseVM>(this.ActivatePlayerActions));
				this.FriendServices.Add(mplobbyFriendServiceVM);
			}
			this._activeServiceIndex = 0;
			this.UpdateActiveService();
			this._activeNotifications = new List<LobbyNotification>();
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.TitleText = new TextObject("{=abxndmIh}Social", null).ToString();
			this.InGameText = new TextObject("{=uUoSmCBS}In Bannerlord", null).ToString();
			this.OnlineText = new TextObject("{=V305MaOP}Online", null).ToString();
			this.OfflineText = new TextObject("{=Zv1lg272}Offline", null).ToString();
			this.FriendListHint = new HintViewModel(new TextObject("{=tjioq56N}Friend List", null), null);
			this.PartyFriends.ApplyActionOnAllItems(delegate(MPLobbyPartyPlayerVM x)
			{
				x.RefreshValues();
			});
			this.PlayerActions.ApplyActionOnAllItems(delegate(StringPairItemWithActionVM x)
			{
				x.RefreshValues();
			});
			this.FriendServices.ApplyActionOnAllItems(delegate(MPLobbyFriendServiceVM x)
			{
				x.RefreshValues();
			});
		}

		public override void OnFinalize()
		{
			base.OnFinalize();
			foreach (MPLobbyFriendServiceVM mplobbyFriendServiceVM in this.FriendServices)
			{
				mplobbyFriendServiceVM.OnFinalize();
			}
			this.ToggleInputKey.OnFinalize();
		}

		public void OnStateActivate()
		{
			this.IsPartyAvailable = NetworkMain.GameClient.PartySystemAvailable;
			this.GetPartyData();
			foreach (MPLobbyFriendServiceVM mplobbyFriendServiceVM in this.FriendServices)
			{
				mplobbyFriendServiceVM.OnStateActivate();
			}
		}

		private void IsEnabledUpdated()
		{
			this.GetPartyData();
		}

		private void GetPartyData()
		{
			this.PartyFriends.Clear();
			if (NetworkMain.GameClient.IsInParty)
			{
				foreach (PartyPlayerInLobbyClient partyPlayerInLobbyClient in NetworkMain.GameClient.PlayersInParty)
				{
					if (partyPlayerInLobbyClient.WaitingInvitation)
					{
						this.OnPlayerInvitedToParty(partyPlayerInLobbyClient.PlayerId);
					}
					else
					{
						this.OnPlayerAddedToParty(partyPlayerInLobbyClient.PlayerId);
					}
				}
			}
		}

		public void OnTick(float dt)
		{
			int num = 0;
			foreach (MPLobbyFriendServiceVM mplobbyFriendServiceVM in this.FriendServices)
			{
				mplobbyFriendServiceVM.OnTick(dt);
				if (mplobbyFriendServiceVM.FriendListService.IncludeInAllFriends)
				{
					num += mplobbyFriendServiceVM.OnlineFriends.FriendList.Count;
					num += mplobbyFriendServiceVM.InGameFriends.FriendList.Count;
				}
			}
			this.TotalOnlineFriendCount = num;
			this.IsInParty = NetworkMain.GameClient.IsInParty;
		}

		public void OnPlayerInvitedToParty(PlayerId playerId)
		{
			if (playerId != NetworkMain.GameClient.PlayerData.PlayerId)
			{
				MPLobbyPartyPlayerVM mplobbyPartyPlayerVM = new MPLobbyPartyPlayerVM(playerId, new Action<MPLobbyPartyPlayerVM>(this.ActivatePlayerActions));
				mplobbyPartyPlayerVM.IsWaitingConfirmation = true;
				this.PartyFriends.Add(mplobbyPartyPlayerVM);
			}
		}

		public void OnPlayerAddedToParty(PlayerId playerId)
		{
			if (playerId != NetworkMain.GameClient.PlayerData.PlayerId)
			{
				MPLobbyPartyPlayerVM mplobbyPartyPlayerVM = this.FindPartyFriend(playerId);
				if (mplobbyPartyPlayerVM == null)
				{
					mplobbyPartyPlayerVM = new MPLobbyPartyPlayerVM(playerId, new Action<MPLobbyPartyPlayerVM>(this.ActivatePlayerActions));
					this.PartyFriends.Add(mplobbyPartyPlayerVM);
				}
				else
				{
					mplobbyPartyPlayerVM.IsWaitingConfirmation = false;
				}
			}
			this.UpdateCanInviteOtherPlayersToParty();
			this.UpdatePartyLeader();
		}

		public void OnPlayerRemovedFromParty(PlayerId playerId)
		{
			if (playerId == NetworkMain.GameClient.PlayerData.PlayerId)
			{
				this.PartyFriends.Clear();
			}
			else
			{
				int num = -1;
				for (int i = 0; i < this.PartyFriends.Count; i++)
				{
					if (this.PartyFriends[i].ProvidedID == playerId)
					{
						num = i;
						break;
					}
				}
				if (this.PartyFriends.Count > 0 && num > -1 && num < this.PartyFriends.Count)
				{
					this.PartyFriends.RemoveAt(num);
				}
			}
			this.UpdateCanInviteOtherPlayersToParty();
			this.UpdatePartyLeader();
		}

		private MPLobbyPartyPlayerVM FindPartyFriend(PlayerId playerId)
		{
			foreach (MPLobbyPartyPlayerVM mplobbyPartyPlayerVM in this.PartyFriends)
			{
				if (mplobbyPartyPlayerVM.ProvidedID == playerId)
				{
					return mplobbyPartyPlayerVM;
				}
			}
			return null;
		}

		internal void OnPlayerAssignedPartyLeader()
		{
			this.UpdatePartyLeader();
		}

		internal void OnClanInfoChanged()
		{
			MPLobbyFriendServiceVM mplobbyFriendServiceVM = this.FriendServices.FirstOrDefault((MPLobbyFriendServiceVM f) => f.FriendListService.GetServiceCodeName() == "ClanFriends");
			if (NetworkMain.GameClient.IsInClan && mplobbyFriendServiceVM == null)
			{
				IFriendListService[] friendListServices = PlatformServices.Instance.GetFriendListServices();
				IFriendListService friendListService = friendListServices.FirstOrDefault((IFriendListService f) => f.GetServiceCodeName() == "ClanFriends");
				if (friendListService != null)
				{
					MPLobbyFriendServiceVM mplobbyFriendServiceVM2 = new MPLobbyFriendServiceVM(friendListService, new Action<PlayerId>(this.OnFriendRequestAnswered), new Action<MPLobbyPlayerBaseVM>(this.ActivatePlayerActions));
					this.FriendServices.Insert(friendListServices.Length - 2, mplobbyFriendServiceVM2);
					mplobbyFriendServiceVM2.IsInGameStatusActive = true;
					mplobbyFriendServiceVM2.ForceRefresh();
					return;
				}
			}
			else
			{
				if (NetworkMain.GameClient.IsInClan && mplobbyFriendServiceVM != null)
				{
					mplobbyFriendServiceVM.ForceRefresh();
					return;
				}
				if (!NetworkMain.GameClient.IsInClan && mplobbyFriendServiceVM != null)
				{
					for (int i = this.FriendServices.Count - 1; i >= 0; i--)
					{
						IFriendListService friendListService2 = this.FriendServices[i].FriendListService;
						if (!NetworkMain.GameClient.IsInClan && friendListService2.GetServiceCodeName() == "ClanFriends")
						{
							this.FriendServices[i].OnFinalize();
							this.FriendServices.RemoveAt(i);
							return;
						}
					}
				}
			}
		}

		private void ActivatePlayerActions(MPLobbyPlayerBaseVM player)
		{
			this.PlayerActions.Clear();
			MPLobbyPartyPlayerVM mplobbyPartyPlayerVM;
			MPLobbyFriendItemVM mplobbyFriendItemVM;
			if ((mplobbyPartyPlayerVM = player as MPLobbyPartyPlayerVM) != null)
			{
				this.ActivatePartyPlayerActions(mplobbyPartyPlayerVM);
			}
			else if ((mplobbyFriendItemVM = player as MPLobbyFriendItemVM) != null)
			{
				this.ActivateFriendPlayerActions(mplobbyFriendItemVM);
			}
			this.IsPlayerActionsActive = false;
			this.IsPlayerActionsActive = this.PlayerActions.Count > 0;
		}

		private void ExecuteSetPlayerAsLeader(object playerObj)
		{
			MPLobbyPlayerBaseVM mplobbyPlayerBaseVM = playerObj as MPLobbyPlayerBaseVM;
			NetworkMain.GameClient.PromotePlayerToPartyLeader(mplobbyPlayerBaseVM.ProvidedID);
			this.UpdatePartyLeader();
		}

		private void ExecuteKickPlayerFromParty(object playerObj)
		{
			MPLobbyPlayerBaseVM mplobbyPlayerBaseVM = playerObj as MPLobbyPlayerBaseVM;
			if (NetworkMain.GameClient.IsInParty && NetworkMain.GameClient.IsPartyLeader)
			{
				NetworkMain.GameClient.KickPlayerFromParty(mplobbyPlayerBaseVM.ProvidedID);
			}
			this.UpdatePartyLeader();
		}

		private void ExecuteLeaveParty(object playerObj)
		{
			MPLobbyPlayerBaseVM mplobbyPlayerBaseVM = playerObj as MPLobbyPlayerBaseVM;
			if (NetworkMain.GameClient.IsInParty && mplobbyPlayerBaseVM.ProvidedID == NetworkMain.GameClient.PlayerData.PlayerId)
			{
				NetworkMain.GameClient.KickPlayerFromParty(NetworkMain.GameClient.PlayerData.PlayerId);
			}
		}

		private void ExecuteInviteFriend(PlayerId providedId)
		{
			bool dontUseNameForUnknownPlayer = BannerlordConfig.EnableGenericNames && !NetworkMain.GameClient.IsKnownPlayer(providedId);
			PermissionResult <>9__1;
			PlatformServices.Instance.CheckPrivilege(Privilege.Communication, true, delegate(bool result)
			{
				if (result)
				{
					PlayerIdProvidedTypes providedType = providedId.ProvidedType;
					LobbyClient gameClient = NetworkMain.GameClient;
					PlayerIdProvidedTypes? playerIdProvidedTypes = ((gameClient != null) ? new PlayerIdProvidedTypes?(gameClient.PlayerID.ProvidedType) : null);
					if (!((providedType == playerIdProvidedTypes.GetValueOrDefault()) & (playerIdProvidedTypes != null)))
					{
						NetworkMain.GameClient.InviteToParty(providedId, dontUseNameForUnknownPlayer);
						return;
					}
					IPlatformServices instance = PlatformServices.Instance;
					Permission permission = Permission.PlayMultiplayer;
					PlayerId providedId2 = providedId;
					PermissionResult permissionResult2;
					if ((permissionResult2 = <>9__1) == null)
					{
						permissionResult2 = (<>9__1 = delegate(bool permissionResult)
						{
							if (permissionResult)
							{
								NetworkMain.GameClient.InviteToParty(providedId, dontUseNameForUnknownPlayer);
								return;
							}
							string text = new TextObject("{=ZwN6rzTC}No permission", null).ToString();
							string text2 = new TextObject("{=wlz3eQWp}No permission to invite player.", null).ToString();
							InformationManager.ShowInquiry(new InquiryData(text, text2, false, true, "", new TextObject("{=dismissnotification}Dismiss", null).ToString(), null, delegate
							{
								InformationManager.HideInquiry();
							}, "event:/ui/notification/quest_update", 0f, null, null, null), false, false);
						});
					}
					instance.CheckPermissionWithUser(permission, providedId2, permissionResult2);
				}
			});
		}

		private void ExecuteRequestFriendship(object playerObj)
		{
			MPLobbyPlayerBaseVM mplobbyPlayerBaseVM = playerObj as MPLobbyPlayerBaseVM;
			bool flag = BannerlordConfig.EnableGenericNames && !NetworkMain.GameClient.IsKnownPlayer(mplobbyPlayerBaseVM.ProvidedID);
			NetworkMain.GameClient.AddFriend(mplobbyPlayerBaseVM.ProvidedID, flag);
		}

		private void ExecuteTerminateFriendship(object memberObj)
		{
			MPLobbyPlayerBaseVM mplobbyPlayerBaseVM = memberObj as MPLobbyPlayerBaseVM;
			NetworkMain.GameClient.RemoveFriend(mplobbyPlayerBaseVM.ProvidedID);
		}

		public void UpdateCanInviteOtherPlayersToParty()
		{
			foreach (MPLobbyFriendServiceVM mplobbyFriendServiceVM in this.FriendServices)
			{
				mplobbyFriendServiceVM.UpdateCanInviteOtherPlayersToParty();
			}
		}

		public void UpdatePartyLeader()
		{
			this.Player.IsPartyLeader = NetworkMain.GameClient.IsInParty && this.Player.ProvidedID == this._partyLeaderId;
			foreach (MPLobbyPartyPlayerVM mplobbyPartyPlayerVM in this.PartyFriends)
			{
				mplobbyPartyPlayerVM.IsPartyLeader = mplobbyPartyPlayerVM.ProvidedID == this._partyLeaderId;
			}
		}

		public void OnFriendRequestNotificationsReceived(List<LobbyNotification> notifications)
		{
			foreach (LobbyNotification lobbyNotification in this._activeNotifications.Except(notifications).ToList<LobbyNotification>())
			{
				this._activeNotifications.Remove(lobbyNotification);
				int notificationCount = this.NotificationCount;
				this.NotificationCount = notificationCount - 1;
			}
			using (List<LobbyNotification>.Enumerator enumerator = notifications.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					MPLobbyFriendsVM.<>c__DisplayClass27_0 CS$<>8__locals1 = new MPLobbyFriendsVM.<>c__DisplayClass27_0();
					CS$<>8__locals1.<>4__this = this;
					CS$<>8__locals1.notification = enumerator.Current;
					string notificationPlayerIDString = CS$<>8__locals1.notification.Parameters["friend_requester"];
					if (this._activeNotifications.Where((LobbyNotification n) => n.Parameters["friend_requester"].Equals(notificationPlayerIDString)).IsEmpty<LobbyNotification>())
					{
						PlayerId notificationPlayerID = PlayerId.FromString(notificationPlayerIDString);
						PermissionResult <>9__2;
						PlatformServices.Instance.CheckPrivilege(Privilege.Communication, false, delegate(bool privilegeResult)
						{
							if (!privilegeResult)
							{
								CS$<>8__locals1.<>4__this.ProcessNotification(CS$<>8__locals1.notification, notificationPlayerID, false);
								return;
							}
							IPlatformServices instance = PlatformServices.Instance;
							Permission permission = Permission.CommunicateUsingText;
							PlayerId notificationPlayerID2 = notificationPlayerID;
							PermissionResult permissionResult2;
							if ((permissionResult2 = <>9__2) == null)
							{
								permissionResult2 = (<>9__2 = delegate(bool permissionResult)
								{
									CS$<>8__locals1.<>4__this.ProcessNotification(CS$<>8__locals1.notification, notificationPlayerID, permissionResult);
								});
							}
							instance.CheckPermissionWithUser(permission, notificationPlayerID2, permissionResult2);
						});
					}
				}
			}
		}

		public void ProcessNotification(LobbyNotification notification, PlayerId notificationPlayerID, bool allowed)
		{
			if (!allowed)
			{
				NetworkMain.GameClient.MarkNotificationAsRead(notification.Id);
				return;
			}
			if (MultiplayerPlayerHelper.IsBlocked(notificationPlayerID))
			{
				bool flag = BannerlordConfig.EnableGenericNames && !NetworkMain.GameClient.IsKnownPlayer(notificationPlayerID);
				NetworkMain.GameClient.RespondToFriendRequest(notificationPlayerID, flag, false, true);
				NetworkMain.GameClient.MarkNotificationAsRead(notification.Id);
				return;
			}
			bool flag2 = false;
			foreach (MPLobbyFriendServiceVM mplobbyFriendServiceVM in this.FriendServices)
			{
				if (mplobbyFriendServiceVM.FriendListService.IncludeInAllFriends)
				{
					using (IEnumerator<MPLobbyPlayerBaseVM> enumerator2 = mplobbyFriendServiceVM.AllFriends.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							if (enumerator2.Current.ProvidedID == notificationPlayerID)
							{
								flag2 = true;
								break;
							}
						}
					}
				}
				if (flag2)
				{
					break;
				}
			}
			if (flag2)
			{
				NetworkMain.GameClient.MarkNotificationAsRead(notification.Id);
				return;
			}
			this._activeNotifications.Add(notification);
			int notificationCount = this.NotificationCount;
			this.NotificationCount = notificationCount + 1;
		}

		private void OnFriendRequestAnswered(PlayerId playerID)
		{
			IEnumerable<LobbyNotification> enumerable = this._activeNotifications.Where((LobbyNotification n) => n.Parameters["friend_requester"].Equals(playerID.ToString()));
			foreach (LobbyNotification lobbyNotification in enumerable)
			{
				int notificationCount = this.NotificationCount;
				this.NotificationCount = notificationCount - 1;
				NetworkMain.GameClient.MarkNotificationAsRead(lobbyNotification.Id);
			}
			this._activeNotifications = this._activeNotifications.Except(enumerable).ToList<LobbyNotification>();
		}

		public MBBindingList<MPLobbyPlayerBaseVM> GetAllFriends()
		{
			MBBindingList<MPLobbyPlayerBaseVM> mbbindingList = new MBBindingList<MPLobbyPlayerBaseVM>();
			foreach (MPLobbyFriendServiceVM mplobbyFriendServiceVM in this.FriendServices)
			{
				if (mplobbyFriendServiceVM.FriendListService.IncludeInAllFriends)
				{
					foreach (MPLobbyFriendItemVM mplobbyFriendItemVM in mplobbyFriendServiceVM.InGameFriends.FriendList)
					{
						mbbindingList.Add(mplobbyFriendItemVM);
					}
					foreach (MPLobbyFriendItemVM mplobbyFriendItemVM2 in mplobbyFriendServiceVM.OnlineFriends.FriendList)
					{
						mbbindingList.Add(mplobbyFriendItemVM2);
					}
				}
			}
			return mbbindingList;
		}

		public void OnSupportedFeaturesRefreshed(SupportedFeatures supportedFeatures)
		{
			if (!supportedFeatures.SupportsFeatures(Features.BannerlordFriendList))
			{
				MPLobbyFriendServiceVM mplobbyFriendServiceVM = this.FriendServices.FirstOrDefault((MPLobbyFriendServiceVM fs) => fs.FriendListService.GetType() == typeof(BannerlordFriendListService));
				if (mplobbyFriendServiceVM != null)
				{
					mplobbyFriendServiceVM.OnFinalize();
				}
				this.FriendServices.Remove(mplobbyFriendServiceVM);
			}
		}

		public void OnFriendListUpdated(bool forceUpdate = false)
		{
			foreach (MPLobbyFriendServiceVM mplobbyFriendServiceVM in this.FriendServices)
			{
				mplobbyFriendServiceVM.OnFriendListUpdated(forceUpdate);
			}
			this.Player.UpdateNameAndAvatar(forceUpdate);
			foreach (MPLobbyPartyPlayerVM mplobbyPartyPlayerVM in this.PartyFriends)
			{
				mplobbyPartyPlayerVM.UpdateNameAndAvatar(forceUpdate);
			}
		}

		public void SetToggleFriendListKey(HotKey hotkey)
		{
			this.ToggleInputKey = InputKeyItemVM.CreateFromHotKey(hotkey, true);
		}

		private void ActivatePartyPlayerActions(MPLobbyPartyPlayerVM player)
		{
			if (NetworkMain.GameClient.IsPartyLeader && player.ProvidedID != NetworkMain.GameClient.PlayerData.PlayerId)
			{
				PartyPlayerInLobbyClient partyPlayerInLobbyClient = NetworkMain.GameClient.PlayersInParty.SingleOrDefault((PartyPlayerInLobbyClient p) => p.PlayerId == player.ProvidedID);
				if (partyPlayerInLobbyClient != null && !partyPlayerInLobbyClient.WaitingInvitation && PlatformServices.InvitationServices == null)
				{
					this.PlayerActions.Add(new StringPairItemWithActionVM(new Action<object>(this.ExecuteSetPlayerAsLeader), new TextObject("{=P7moPm3F}Set as party leader", null).ToString(), "PromoteToPartyLeader", player));
				}
				this.PlayerActions.Add(new StringPairItemWithActionVM(new Action<object>(this.ExecuteKickPlayerFromParty), new TextObject("{=partykick}Kick", null).ToString(), "Kick", player));
			}
			if (player.ProvidedID == NetworkMain.GameClient.PlayerData.PlayerId)
			{
				if (NetworkMain.GameClient.IsInParty)
				{
					this.PlayerActions.Add(new StringPairItemWithActionVM(new Action<object>(this.ExecuteLeaveParty), new TextObject("{=9w9JsBYP}Leave party", null).ToString(), "LeaveParty", player));
					return;
				}
			}
			else
			{
				bool flag = false;
				FriendInfo[] friendInfos = NetworkMain.GameClient.FriendInfos;
				for (int i = 0; i < friendInfos.Length; i++)
				{
					if (friendInfos[i].Id == player.ProvidedID)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					this.PlayerActions.Add(new StringPairItemWithActionVM(new Action<object>(this.ExecuteRequestFriendship), new TextObject("{=UwkpJq9N}Add As Friend", null).ToString(), "RequestFriendship", player));
				}
				else
				{
					this.PlayerActions.Add(new StringPairItemWithActionVM(new Action<object>(this.ExecuteTerminateFriendship), new TextObject("{=2YIVRuRa}Remove From Friends", null).ToString(), "TerminateFriendship", player));
				}
				MultiplayerPlayerContextMenuHelper.AddLobbyViewProfileOptions(player, this.PlayerActions);
			}
		}

		private void ActivateFriendPlayerActions(MPLobbyFriendItemVM player)
		{
			MultiplayerPlayerContextMenuHelper.AddLobbyViewProfileOptions(player, this.PlayerActions);
		}

		private void ExecuteSwitchToNextService()
		{
			this._activeServiceIndex++;
			if (this._activeServiceIndex == this.FriendServices.Count)
			{
				this._activeServiceIndex = 0;
			}
			this.UpdateActiveService();
		}

		private void ExecuteSwitchToPreviousService()
		{
			this._activeServiceIndex--;
			if (this._activeServiceIndex < 0)
			{
				this._activeServiceIndex = this.FriendServices.Count - 1;
			}
			this.UpdateActiveService();
		}

		private void UpdateActiveService()
		{
			this.ActiveService = this.FriendServices[this._activeServiceIndex];
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
					this.IsEnabledUpdated();
				}
			}
		}

		[DataSourceProperty]
		public bool IsListEnabled
		{
			get
			{
				return this._isListEnabled;
			}
			set
			{
				if (value != this._isListEnabled)
				{
					this._isListEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsListEnabled");
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
		public bool IsPartyAvailable
		{
			get
			{
				return this._isPartyAvailable;
			}
			set
			{
				if (value != this._isPartyAvailable)
				{
					this._isPartyAvailable = value;
					base.OnPropertyChangedWithValue(value, "IsPartyAvailable");
				}
			}
		}

		[DataSourceProperty]
		public bool IsPartyFull
		{
			get
			{
				return this._isPartyFull;
			}
			set
			{
				if (value != this._isPartyFull)
				{
					this._isPartyFull = value;
					base.OnPropertyChangedWithValue(value, "IsPartyFull");
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
		public MPLobbyPartyPlayerVM Player
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
					base.OnPropertyChangedWithValue<MPLobbyPartyPlayerVM>(value, "Player");
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<MPLobbyPartyPlayerVM> PartyFriends
		{
			get
			{
				return this._partyFriends;
			}
			set
			{
				if (value != this._partyFriends)
				{
					this._partyFriends = value;
					base.OnPropertyChangedWithValue<MBBindingList<MPLobbyPartyPlayerVM>>(value, "PartyFriends");
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
		public string InGameText
		{
			get
			{
				return this._inGameText;
			}
			set
			{
				if (value != this._inGameText)
				{
					this._inGameText = value;
					base.OnPropertyChangedWithValue<string>(value, "InGameText");
				}
			}
		}

		[DataSourceProperty]
		public string OnlineText
		{
			get
			{
				return this._onlineText;
			}
			set
			{
				if (value != this._onlineText)
				{
					this._onlineText = value;
					base.OnPropertyChangedWithValue<string>(value, "OnlineText");
				}
			}
		}

		[DataSourceProperty]
		public string OfflineText
		{
			get
			{
				return this._offlineText;
			}
			set
			{
				if (value != this._offlineText)
				{
					this._offlineText = value;
					base.OnPropertyChangedWithValue<string>(value, "OfflineText");
				}
			}
		}

		[DataSourceProperty]
		public HintViewModel FriendListHint
		{
			get
			{
				return this._friendListHint;
			}
			set
			{
				if (value != this._friendListHint)
				{
					this._friendListHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "FriendListHint");
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<MPLobbyFriendServiceVM> FriendServices
		{
			get
			{
				return this._friendServices;
			}
			set
			{
				if (value != this._friendServices)
				{
					this._friendServices = value;
					base.OnPropertyChangedWithValue<MBBindingList<MPLobbyFriendServiceVM>>(value, "FriendServices");
				}
			}
		}

		[DataSourceProperty]
		public MPLobbyFriendServiceVM ActiveService
		{
			get
			{
				return this._activeService;
			}
			set
			{
				if (value != this._activeService)
				{
					this._activeService = value;
					base.OnPropertyChangedWithValue<MPLobbyFriendServiceVM>(value, "ActiveService");
				}
			}
		}

		[DataSourceProperty]
		public int TotalOnlineFriendCount
		{
			get
			{
				return this._totalOnlineFriendCount;
			}
			set
			{
				if (value != this._totalOnlineFriendCount)
				{
					this._totalOnlineFriendCount = value;
					base.OnPropertyChangedWithValue(value, "TotalOnlineFriendCount");
				}
			}
		}

		[DataSourceProperty]
		public int NotificationCount
		{
			get
			{
				return this._notificationCount;
			}
			set
			{
				if (value != this._notificationCount)
				{
					this._notificationCount = value;
					base.OnPropertyChangedWithValue(value, "NotificationCount");
					this.HasNotification = value > 0;
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
		public InputKeyItemVM ToggleInputKey
		{
			get
			{
				return this._toggleInputKey;
			}
			set
			{
				if (value != this._toggleInputKey)
				{
					this._toggleInputKey = value;
					base.OnPropertyChanged("ToggleInputKey");
				}
			}
		}

		private const string _inviteFailedSoundEvent = "event:/ui/notification/quest_update";

		private List<LobbyNotification> _activeNotifications;

		private int _activeServiceIndex;

		private bool _isEnabled;

		private bool _isListEnabled = true;

		private bool _isPartyAvailable;

		private bool _isPartyFull;

		private bool _isPlayerActionsActive;

		private bool _isInParty;

		private MPLobbyPartyPlayerVM _player;

		private MBBindingList<MPLobbyPartyPlayerVM> _partyFriends;

		private MBBindingList<StringPairItemWithActionVM> _playerActions;

		private string _titleText;

		private string _inGameText;

		private string _onlineText;

		private string _offlineText;

		private int _totalOnlineFriendCount;

		private int _notificationCount;

		private bool _hasNotification;

		private HintViewModel _friendListHint;

		private MBBindingList<MPLobbyFriendServiceVM> _friendServices;

		private MPLobbyFriendServiceVM _activeService;

		private InputKeyItemVM _toggleInputKey;
	}
}
