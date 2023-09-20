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
	// Token: 0x02000083 RID: 131
	public class MPLobbyFriendsVM : ViewModel
	{
		// Token: 0x17000392 RID: 914
		// (get) Token: 0x06000B8C RID: 2956 RVA: 0x00028E34 File Offset: 0x00027034
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

		// Token: 0x06000B8D RID: 2957 RVA: 0x00028E88 File Offset: 0x00027088
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

		// Token: 0x06000B8E RID: 2958 RVA: 0x00028F48 File Offset: 0x00027148
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

		// Token: 0x06000B8F RID: 2959 RVA: 0x00029048 File Offset: 0x00027248
		public override void OnFinalize()
		{
			base.OnFinalize();
			foreach (MPLobbyFriendServiceVM mplobbyFriendServiceVM in this.FriendServices)
			{
				mplobbyFriendServiceVM.OnFinalize();
			}
			this.ToggleInputKey.OnFinalize();
		}

		// Token: 0x06000B90 RID: 2960 RVA: 0x000290A4 File Offset: 0x000272A4
		public void OnStateActivate()
		{
			this.IsPartyAvailable = NetworkMain.GameClient.PartySystemAvailable;
			this.GetPartyData();
			foreach (MPLobbyFriendServiceVM mplobbyFriendServiceVM in this.FriendServices)
			{
				mplobbyFriendServiceVM.OnStateActivate();
			}
		}

		// Token: 0x06000B91 RID: 2961 RVA: 0x00029104 File Offset: 0x00027304
		private void IsEnabledUpdated()
		{
			this.GetPartyData();
		}

		// Token: 0x06000B92 RID: 2962 RVA: 0x0002910C File Offset: 0x0002730C
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

		// Token: 0x06000B93 RID: 2963 RVA: 0x00029198 File Offset: 0x00027398
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

		// Token: 0x06000B94 RID: 2964 RVA: 0x00029234 File Offset: 0x00027434
		public void OnPlayerInvitedToParty(PlayerId playerId)
		{
			if (playerId != NetworkMain.GameClient.PlayerData.PlayerId)
			{
				MPLobbyPartyPlayerVM mplobbyPartyPlayerVM = new MPLobbyPartyPlayerVM(playerId, new Action<MPLobbyPartyPlayerVM>(this.ActivatePlayerActions));
				mplobbyPartyPlayerVM.IsWaitingConfirmation = true;
				this.PartyFriends.Add(mplobbyPartyPlayerVM);
			}
		}

		// Token: 0x06000B95 RID: 2965 RVA: 0x00029280 File Offset: 0x00027480
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

		// Token: 0x06000B96 RID: 2966 RVA: 0x000292E4 File Offset: 0x000274E4
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

		// Token: 0x06000B97 RID: 2967 RVA: 0x00029384 File Offset: 0x00027584
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

		// Token: 0x06000B98 RID: 2968 RVA: 0x000293E0 File Offset: 0x000275E0
		internal void OnPlayerAssignedPartyLeader()
		{
			this.UpdatePartyLeader();
		}

		// Token: 0x06000B99 RID: 2969 RVA: 0x000293E8 File Offset: 0x000275E8
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

		// Token: 0x06000B9A RID: 2970 RVA: 0x00029538 File Offset: 0x00027738
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

		// Token: 0x06000B9B RID: 2971 RVA: 0x00029590 File Offset: 0x00027790
		private void ExecuteSetPlayerAsLeader(object playerObj)
		{
			MPLobbyPlayerBaseVM mplobbyPlayerBaseVM = playerObj as MPLobbyPlayerBaseVM;
			NetworkMain.GameClient.PromotePlayerToPartyLeader(mplobbyPlayerBaseVM.ProvidedID);
			this.UpdatePartyLeader();
		}

		// Token: 0x06000B9C RID: 2972 RVA: 0x000295BC File Offset: 0x000277BC
		private void ExecuteKickPlayerFromParty(object playerObj)
		{
			MPLobbyPlayerBaseVM mplobbyPlayerBaseVM = playerObj as MPLobbyPlayerBaseVM;
			if (NetworkMain.GameClient.IsInParty && NetworkMain.GameClient.IsPartyLeader)
			{
				NetworkMain.GameClient.KickPlayerFromParty(mplobbyPlayerBaseVM.ProvidedID);
			}
			this.UpdatePartyLeader();
		}

		// Token: 0x06000B9D RID: 2973 RVA: 0x00029600 File Offset: 0x00027800
		private void ExecuteLeaveParty(object playerObj)
		{
			MPLobbyPlayerBaseVM mplobbyPlayerBaseVM = playerObj as MPLobbyPlayerBaseVM;
			if (NetworkMain.GameClient.IsInParty && mplobbyPlayerBaseVM.ProvidedID == NetworkMain.GameClient.PlayerData.PlayerId)
			{
				NetworkMain.GameClient.KickPlayerFromParty(NetworkMain.GameClient.PlayerData.PlayerId);
			}
		}

		// Token: 0x06000B9E RID: 2974 RVA: 0x00029658 File Offset: 0x00027858
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

		// Token: 0x06000B9F RID: 2975 RVA: 0x000296B0 File Offset: 0x000278B0
		private void ExecuteRequestFriendship(object playerObj)
		{
			MPLobbyPlayerBaseVM mplobbyPlayerBaseVM = playerObj as MPLobbyPlayerBaseVM;
			bool flag = BannerlordConfig.EnableGenericNames && !NetworkMain.GameClient.IsKnownPlayer(mplobbyPlayerBaseVM.ProvidedID);
			NetworkMain.GameClient.AddFriend(mplobbyPlayerBaseVM.ProvidedID, flag);
		}

		// Token: 0x06000BA0 RID: 2976 RVA: 0x000296F4 File Offset: 0x000278F4
		private void ExecuteTerminateFriendship(object memberObj)
		{
			MPLobbyPlayerBaseVM mplobbyPlayerBaseVM = memberObj as MPLobbyPlayerBaseVM;
			NetworkMain.GameClient.RemoveFriend(mplobbyPlayerBaseVM.ProvidedID);
		}

		// Token: 0x06000BA1 RID: 2977 RVA: 0x00029718 File Offset: 0x00027918
		public void UpdateCanInviteOtherPlayersToParty()
		{
			foreach (MPLobbyFriendServiceVM mplobbyFriendServiceVM in this.FriendServices)
			{
				mplobbyFriendServiceVM.UpdateCanInviteOtherPlayersToParty();
			}
		}

		// Token: 0x06000BA2 RID: 2978 RVA: 0x00029764 File Offset: 0x00027964
		public void UpdatePartyLeader()
		{
			this.Player.IsPartyLeader = NetworkMain.GameClient.IsInParty && this.Player.ProvidedID == this._partyLeaderId;
			foreach (MPLobbyPartyPlayerVM mplobbyPartyPlayerVM in this.PartyFriends)
			{
				mplobbyPartyPlayerVM.IsPartyLeader = mplobbyPartyPlayerVM.ProvidedID == this._partyLeaderId;
			}
		}

		// Token: 0x06000BA3 RID: 2979 RVA: 0x0002981C File Offset: 0x00027A1C
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

		// Token: 0x06000BA4 RID: 2980 RVA: 0x00029970 File Offset: 0x00027B70
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

		// Token: 0x06000BA5 RID: 2981 RVA: 0x00029A9C File Offset: 0x00027C9C
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

		// Token: 0x06000BA6 RID: 2982 RVA: 0x00029B3C File Offset: 0x00027D3C
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

		// Token: 0x06000BA7 RID: 2983 RVA: 0x00029C24 File Offset: 0x00027E24
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

		// Token: 0x06000BA8 RID: 2984 RVA: 0x00029C7C File Offset: 0x00027E7C
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

		// Token: 0x06000BA9 RID: 2985 RVA: 0x00029D10 File Offset: 0x00027F10
		public void SetToggleFriendListKey(HotKey hotkey)
		{
			this.ToggleInputKey = InputKeyItemVM.CreateFromHotKey(hotkey, true);
		}

		// Token: 0x06000BAA RID: 2986 RVA: 0x00029D20 File Offset: 0x00027F20
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

		// Token: 0x06000BAB RID: 2987 RVA: 0x00029F33 File Offset: 0x00028133
		private void ActivateFriendPlayerActions(MPLobbyFriendItemVM player)
		{
			MultiplayerPlayerContextMenuHelper.AddLobbyViewProfileOptions(player, this.PlayerActions);
		}

		// Token: 0x06000BAC RID: 2988 RVA: 0x00029F41 File Offset: 0x00028141
		private void ExecuteSwitchToNextService()
		{
			this._activeServiceIndex++;
			if (this._activeServiceIndex == this.FriendServices.Count)
			{
				this._activeServiceIndex = 0;
			}
			this.UpdateActiveService();
		}

		// Token: 0x06000BAD RID: 2989 RVA: 0x00029F71 File Offset: 0x00028171
		private void ExecuteSwitchToPreviousService()
		{
			this._activeServiceIndex--;
			if (this._activeServiceIndex < 0)
			{
				this._activeServiceIndex = this.FriendServices.Count - 1;
			}
			this.UpdateActiveService();
		}

		// Token: 0x06000BAE RID: 2990 RVA: 0x00029FA3 File Offset: 0x000281A3
		private void UpdateActiveService()
		{
			this.ActiveService = this.FriendServices[this._activeServiceIndex];
		}

		// Token: 0x17000393 RID: 915
		// (get) Token: 0x06000BAF RID: 2991 RVA: 0x00029FBC File Offset: 0x000281BC
		// (set) Token: 0x06000BB0 RID: 2992 RVA: 0x00029FC4 File Offset: 0x000281C4
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

		// Token: 0x17000394 RID: 916
		// (get) Token: 0x06000BB1 RID: 2993 RVA: 0x00029FE8 File Offset: 0x000281E8
		// (set) Token: 0x06000BB2 RID: 2994 RVA: 0x00029FF0 File Offset: 0x000281F0
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

		// Token: 0x17000395 RID: 917
		// (get) Token: 0x06000BB3 RID: 2995 RVA: 0x0002A00E File Offset: 0x0002820E
		// (set) Token: 0x06000BB4 RID: 2996 RVA: 0x0002A016 File Offset: 0x00028216
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

		// Token: 0x17000396 RID: 918
		// (get) Token: 0x06000BB5 RID: 2997 RVA: 0x0002A034 File Offset: 0x00028234
		// (set) Token: 0x06000BB6 RID: 2998 RVA: 0x0002A03C File Offset: 0x0002823C
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

		// Token: 0x17000397 RID: 919
		// (get) Token: 0x06000BB7 RID: 2999 RVA: 0x0002A05A File Offset: 0x0002825A
		// (set) Token: 0x06000BB8 RID: 3000 RVA: 0x0002A062 File Offset: 0x00028262
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

		// Token: 0x17000398 RID: 920
		// (get) Token: 0x06000BB9 RID: 3001 RVA: 0x0002A080 File Offset: 0x00028280
		// (set) Token: 0x06000BBA RID: 3002 RVA: 0x0002A088 File Offset: 0x00028288
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

		// Token: 0x17000399 RID: 921
		// (get) Token: 0x06000BBB RID: 3003 RVA: 0x0002A0A6 File Offset: 0x000282A6
		// (set) Token: 0x06000BBC RID: 3004 RVA: 0x0002A0AE File Offset: 0x000282AE
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

		// Token: 0x1700039A RID: 922
		// (get) Token: 0x06000BBD RID: 3005 RVA: 0x0002A0CC File Offset: 0x000282CC
		// (set) Token: 0x06000BBE RID: 3006 RVA: 0x0002A0D4 File Offset: 0x000282D4
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

		// Token: 0x1700039B RID: 923
		// (get) Token: 0x06000BBF RID: 3007 RVA: 0x0002A0F2 File Offset: 0x000282F2
		// (set) Token: 0x06000BC0 RID: 3008 RVA: 0x0002A0FA File Offset: 0x000282FA
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

		// Token: 0x1700039C RID: 924
		// (get) Token: 0x06000BC1 RID: 3009 RVA: 0x0002A118 File Offset: 0x00028318
		// (set) Token: 0x06000BC2 RID: 3010 RVA: 0x0002A120 File Offset: 0x00028320
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

		// Token: 0x1700039D RID: 925
		// (get) Token: 0x06000BC3 RID: 3011 RVA: 0x0002A143 File Offset: 0x00028343
		// (set) Token: 0x06000BC4 RID: 3012 RVA: 0x0002A14B File Offset: 0x0002834B
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

		// Token: 0x1700039E RID: 926
		// (get) Token: 0x06000BC5 RID: 3013 RVA: 0x0002A16E File Offset: 0x0002836E
		// (set) Token: 0x06000BC6 RID: 3014 RVA: 0x0002A176 File Offset: 0x00028376
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

		// Token: 0x1700039F RID: 927
		// (get) Token: 0x06000BC7 RID: 3015 RVA: 0x0002A199 File Offset: 0x00028399
		// (set) Token: 0x06000BC8 RID: 3016 RVA: 0x0002A1A1 File Offset: 0x000283A1
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

		// Token: 0x170003A0 RID: 928
		// (get) Token: 0x06000BC9 RID: 3017 RVA: 0x0002A1C4 File Offset: 0x000283C4
		// (set) Token: 0x06000BCA RID: 3018 RVA: 0x0002A1CC File Offset: 0x000283CC
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

		// Token: 0x170003A1 RID: 929
		// (get) Token: 0x06000BCB RID: 3019 RVA: 0x0002A1EA File Offset: 0x000283EA
		// (set) Token: 0x06000BCC RID: 3020 RVA: 0x0002A1F2 File Offset: 0x000283F2
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

		// Token: 0x170003A2 RID: 930
		// (get) Token: 0x06000BCD RID: 3021 RVA: 0x0002A210 File Offset: 0x00028410
		// (set) Token: 0x06000BCE RID: 3022 RVA: 0x0002A218 File Offset: 0x00028418
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

		// Token: 0x170003A3 RID: 931
		// (get) Token: 0x06000BCF RID: 3023 RVA: 0x0002A236 File Offset: 0x00028436
		// (set) Token: 0x06000BD0 RID: 3024 RVA: 0x0002A23E File Offset: 0x0002843E
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

		// Token: 0x170003A4 RID: 932
		// (get) Token: 0x06000BD1 RID: 3025 RVA: 0x0002A25C File Offset: 0x0002845C
		// (set) Token: 0x06000BD2 RID: 3026 RVA: 0x0002A264 File Offset: 0x00028464
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

		// Token: 0x170003A5 RID: 933
		// (get) Token: 0x06000BD3 RID: 3027 RVA: 0x0002A28C File Offset: 0x0002848C
		// (set) Token: 0x06000BD4 RID: 3028 RVA: 0x0002A294 File Offset: 0x00028494
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

		// Token: 0x170003A6 RID: 934
		// (get) Token: 0x06000BD5 RID: 3029 RVA: 0x0002A2B2 File Offset: 0x000284B2
		// (set) Token: 0x06000BD6 RID: 3030 RVA: 0x0002A2BA File Offset: 0x000284BA
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

		// Token: 0x0400058B RID: 1419
		private const string _inviteFailedSoundEvent = "event:/ui/notification/quest_update";

		// Token: 0x0400058C RID: 1420
		private List<LobbyNotification> _activeNotifications;

		// Token: 0x0400058D RID: 1421
		private int _activeServiceIndex;

		// Token: 0x0400058E RID: 1422
		private bool _isEnabled;

		// Token: 0x0400058F RID: 1423
		private bool _isListEnabled = true;

		// Token: 0x04000590 RID: 1424
		private bool _isPartyAvailable;

		// Token: 0x04000591 RID: 1425
		private bool _isPartyFull;

		// Token: 0x04000592 RID: 1426
		private bool _isPlayerActionsActive;

		// Token: 0x04000593 RID: 1427
		private bool _isInParty;

		// Token: 0x04000594 RID: 1428
		private MPLobbyPartyPlayerVM _player;

		// Token: 0x04000595 RID: 1429
		private MBBindingList<MPLobbyPartyPlayerVM> _partyFriends;

		// Token: 0x04000596 RID: 1430
		private MBBindingList<StringPairItemWithActionVM> _playerActions;

		// Token: 0x04000597 RID: 1431
		private string _titleText;

		// Token: 0x04000598 RID: 1432
		private string _inGameText;

		// Token: 0x04000599 RID: 1433
		private string _onlineText;

		// Token: 0x0400059A RID: 1434
		private string _offlineText;

		// Token: 0x0400059B RID: 1435
		private int _totalOnlineFriendCount;

		// Token: 0x0400059C RID: 1436
		private int _notificationCount;

		// Token: 0x0400059D RID: 1437
		private bool _hasNotification;

		// Token: 0x0400059E RID: 1438
		private HintViewModel _friendListHint;

		// Token: 0x0400059F RID: 1439
		private MBBindingList<MPLobbyFriendServiceVM> _friendServices;

		// Token: 0x040005A0 RID: 1440
		private MPLobbyFriendServiceVM _activeService;

		// Token: 0x040005A1 RID: 1441
		private InputKeyItemVM _toggleInputKey;
	}
}
