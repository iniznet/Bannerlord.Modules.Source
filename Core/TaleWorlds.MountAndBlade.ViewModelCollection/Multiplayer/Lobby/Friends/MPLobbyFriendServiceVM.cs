using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.PlatformService;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.Friends
{
	// Token: 0x02000082 RID: 130
	public class MPLobbyFriendServiceVM : ViewModel
	{
		// Token: 0x17000384 RID: 900
		// (get) Token: 0x06000B52 RID: 2898 RVA: 0x00027ECC File Offset: 0x000260CC
		public IEnumerable<MPLobbyPlayerBaseVM> AllFriends
		{
			get
			{
				return this.InGameFriends.FriendList.Union(this.OnlineFriends.FriendList.Union(this.OfflineFriends.FriendList));
			}
		}

		// Token: 0x06000B53 RID: 2899 RVA: 0x00027EFC File Offset: 0x000260FC
		public MPLobbyFriendServiceVM(IFriendListService friendListService, Action<PlayerId> onFriendRequestAnswered, Action<MPLobbyPlayerBaseVM> activatePlayerActions)
		{
			this.FriendListService = friendListService;
			this._onFriendRequestAnswered = onFriendRequestAnswered;
			this._activatePlayerActions = activatePlayerActions;
			this._playerStateComparer = new MPLobbyFriendServiceVM.PlayerStateComparer();
			this.InGameFriends = new MPLobbyFriendGroupVM(MPLobbyFriendGroupVM.FriendGroupType.InGame);
			this.OnlineFriends = new MPLobbyFriendGroupVM(MPLobbyFriendGroupVM.FriendGroupType.Online);
			this.OfflineFriends = new MPLobbyFriendGroupVM(MPLobbyFriendGroupVM.FriendGroupType.Offline);
			this.FriendRequests = new MPLobbyFriendGroupVM(MPLobbyFriendGroupVM.FriendGroupType.FriendRequests);
			this.PendingRequests = new MPLobbyFriendGroupVM(MPLobbyFriendGroupVM.FriendGroupType.PendingRequests);
			FriendListServiceType friendListServiceType = friendListService.GetFriendListServiceType();
			this._isInGameFriendsRelevant = friendListServiceType != FriendListServiceType.Bannerlord && friendListServiceType != FriendListServiceType.RecentPlayers && friendListServiceType != FriendListServiceType.Clan && friendListServiceType != FriendListServiceType.PlayStation;
			PlatformServices.Instance.OnBlockedUserListUpdated += this.BlockedUserListChanged;
			this.FriendListService.OnUserStatusChanged += this.UserOnlineStatusChanged;
			this.FriendListService.OnFriendRemoved += this.FriendRemoved;
			this.FriendListService.OnFriendListChanged += this.FriendListChanged;
			this.ServiceName = friendListService.GetServiceCodeName();
			this.RefreshValues();
			this.UpdateCanInviteOtherPlayersToParty();
		}

		// Token: 0x06000B54 RID: 2900 RVA: 0x00028004 File Offset: 0x00026204
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.InGameText = new TextObject("{=uUoSmCBS}In Bannerlord", null).ToString();
			this.OnlineText = new TextObject("{=V305MaOP}Online", null).ToString();
			this.OfflineText = new TextObject("{=Zv1lg272}Offline", null).ToString();
			this.ServiceNameHint = new HintViewModel(this.FriendListService.GetServiceLocalizedName(), null);
			this.InGameFriends.RefreshValues();
			this.OnlineFriends.RefreshValues();
			this.OfflineFriends.RefreshValues();
			this.FriendRequests.RefreshValues();
			this.PendingRequests.RefreshValues();
		}

		// Token: 0x06000B55 RID: 2901 RVA: 0x000280A8 File Offset: 0x000262A8
		public override void OnFinalize()
		{
			PlatformServices.Instance.OnBlockedUserListUpdated -= this.BlockedUserListChanged;
			this.FriendListService.OnUserStatusChanged -= this.UserOnlineStatusChanged;
			this.FriendListService.OnFriendRemoved -= this.FriendRemoved;
			this.FriendListService.OnFriendListChanged -= this.FriendListChanged;
			base.OnFinalize();
		}

		// Token: 0x06000B56 RID: 2902 RVA: 0x00028116 File Offset: 0x00026316
		public void OnStateActivate()
		{
			this._isPartyAvailable = NetworkMain.GameClient.PartySystemAvailable;
			this.IsInGameStatusActive = this.FriendListService.InGameStatusFetchable && this._isInGameFriendsRelevant;
			this.GetFriends();
		}

		// Token: 0x06000B57 RID: 2903 RVA: 0x0002814C File Offset: 0x0002634C
		private async void GetFriends()
		{
			IEnumerable<PlayerId> allFriends = this.FriendListService.GetAllFriends();
			if (allFriends != null && !this._populatingFriends)
			{
				this._populatingFriends = true;
				this.InGameFriends.FriendList.Clear();
				this.OnlineFriends.FriendList.Clear();
				this.OfflineFriends.FriendList.Clear();
				foreach (PlayerId playerId in allFriends)
				{
					await this.CreateAndAddFriendToList(playerId);
				}
				IEnumerator<PlayerId> enumerator = null;
				this._lastStateRequestTimePassed = 11f;
				this._populatingFriends = false;
			}
		}

		// Token: 0x06000B58 RID: 2904 RVA: 0x00028188 File Offset: 0x00026388
		public void OnTick(float dt)
		{
			this.UpdateFriendStates(dt);
			this._lastUpdateTimePassed += dt;
			if (this._lastUpdateTimePassed >= 2f)
			{
				this._lastUpdateTimePassed = 0f;
				if (this.FriendListService.AllowsFriendOperations)
				{
					this.TickFriendOperations(dt);
				}
			}
			MPLobbyFriendGroupVM inGameFriends = this.InGameFriends;
			if (inGameFriends != null)
			{
				inGameFriends.Tick();
			}
			MPLobbyFriendGroupVM onlineFriends = this.OnlineFriends;
			if (onlineFriends != null)
			{
				onlineFriends.Tick();
			}
			MPLobbyFriendGroupVM offlineFriends = this.OfflineFriends;
			if (offlineFriends != null)
			{
				offlineFriends.Tick();
			}
			MPLobbyFriendGroupVM friendRequests = this.FriendRequests;
			if (friendRequests != null)
			{
				friendRequests.Tick();
			}
			MPLobbyFriendGroupVM pendingRequests = this.PendingRequests;
			if (pendingRequests == null)
			{
				return;
			}
			pendingRequests.Tick();
		}

		// Token: 0x06000B59 RID: 2905 RVA: 0x0002822C File Offset: 0x0002642C
		private void TimeoutProcessedFriendRequests()
		{
			foreach (PlayerId playerId in MPLobbyFriendServiceVM._friendRequestsInProcess.Keys.ToArray<PlayerId>())
			{
				if ((long)Environment.TickCount - MPLobbyFriendServiceVM._friendRequestsInProcess[playerId] > 10000L)
				{
					MPLobbyFriendServiceVM._friendRequestsInProcess.Remove(playerId);
				}
			}
		}

		// Token: 0x06000B5A RID: 2906 RVA: 0x00028288 File Offset: 0x00026488
		private void BlockFriendRequest(PlayerId friendId)
		{
			bool flag = BannerlordConfig.EnableGenericNames && !NetworkMain.GameClient.IsKnownPlayer(friendId);
			NetworkMain.GameClient.RespondToFriendRequest(friendId, flag, false, true);
		}

		// Token: 0x06000B5B RID: 2907 RVA: 0x000282BC File Offset: 0x000264BC
		private void ProcessFriendRequest(PlayerId friendId)
		{
			if (MPLobbyFriendServiceVM._friendRequestsInProcess.ContainsKey(friendId))
			{
				return;
			}
			MPLobbyFriendServiceVM._friendRequestsInProcess[friendId] = (long)Environment.TickCount;
			PermissionResult <>9__1;
			PlatformServices.Instance.CheckPrivilege(Privilege.Communication, false, delegate(bool privilegeResult)
			{
				if (!privilegeResult)
				{
					this.BlockFriendRequest(friendId);
					return;
				}
				if (friendId.ProvidedType != NetworkMain.GameClient.PlayerID.ProvidedType)
				{
					this.AddFriendRequestItem(friendId);
					return;
				}
				IPlatformServices instance = PlatformServices.Instance;
				Permission permission = Permission.CommunicateUsingText;
				PlayerId friendId2 = friendId;
				PermissionResult permissionResult2;
				if ((permissionResult2 = <>9__1) == null)
				{
					permissionResult2 = (<>9__1 = delegate(bool permissionResult)
					{
						if (!permissionResult)
						{
							this.BlockFriendRequest(friendId);
							return;
						}
						this.AddFriendRequestItem(friendId);
					});
				}
				instance.CheckPermissionWithUser(permission, friendId2, permissionResult2);
			});
		}

		// Token: 0x06000B5C RID: 2908 RVA: 0x00028320 File Offset: 0x00026520
		private void AddFriendRequestItem(PlayerId playerID)
		{
			MPLobbyFriendItemVM mplobbyFriendItemVM = new MPLobbyFriendItemVM(playerID, this._activatePlayerActions, null, this._onFriendRequestAnswered);
			mplobbyFriendItemVM.IsFriendRequest = true;
			mplobbyFriendItemVM.CanRemove = false;
			this.FriendRequests.AddFriend(mplobbyFriendItemVM);
		}

		// Token: 0x06000B5D RID: 2909 RVA: 0x0002835C File Offset: 0x0002655C
		private void TickFriendOperations(float dt)
		{
			IEnumerable<PlayerId> receivedRequests = this.FriendListService.GetReceivedRequests();
			if (receivedRequests != null)
			{
				this.GotAnyFriendRequests = receivedRequests.Any<PlayerId>();
				this.TimeoutProcessedFriendRequests();
				using (IEnumerator<PlayerId> enumerator = receivedRequests.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						PlayerId friendId2 = enumerator.Current;
						if (this.FriendRequests.FriendList.FirstOrDefault((MPLobbyFriendItemVM p) => p.ProvidedID == friendId2) == null)
						{
							this.ProcessFriendRequest(friendId2);
						}
					}
				}
				int num;
				int j;
				for (j = this.FriendRequests.FriendList.Count - 1; j >= 0; j = num - 1)
				{
					if (!receivedRequests.FirstOrDefault((PlayerId p) => p == this.FriendRequests.FriendList[j].ProvidedID).IsValid)
					{
						this.FriendRequests.FriendList.RemoveAt(j);
					}
					num = j;
				}
			}
			else
			{
				this.GotAnyFriendRequests = false;
			}
			IEnumerable<PlayerId> pendingRequests = this.FriendListService.GetPendingRequests();
			if (pendingRequests != null)
			{
				this.GotAnyPendingRequests = pendingRequests.Any<PlayerId>();
				using (IEnumerator<PlayerId> enumerator = pendingRequests.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						PlayerId friendId = enumerator.Current;
						if (this.PendingRequests.FriendList.FirstOrDefault((MPLobbyFriendItemVM p) => p.ProvidedID == friendId) == null)
						{
							MPLobbyFriendItemVM mplobbyFriendItemVM = new MPLobbyFriendItemVM(friendId, this._activatePlayerActions, null, null);
							mplobbyFriendItemVM.IsPendingRequest = true;
							mplobbyFriendItemVM.CanRemove = false;
							this.PendingRequests.AddFriend(mplobbyFriendItemVM);
						}
					}
				}
				int num;
				int i;
				for (i = this.PendingRequests.FriendList.Count - 1; i >= 0; i = num - 1)
				{
					if (!pendingRequests.FirstOrDefault((PlayerId p) => p == this.PendingRequests.FriendList[i].ProvidedID).IsValid)
					{
						this.PendingRequests.FriendList.RemoveAt(i);
					}
					num = i;
				}
				return;
			}
			this.GotAnyPendingRequests = false;
		}

		// Token: 0x06000B5E RID: 2910 RVA: 0x000285AC File Offset: 0x000267AC
		private void UpdateFriendStates(float dt)
		{
			if (!NetworkMain.GameClient.AtLobby || this._isStateRequestActive)
			{
				return;
			}
			this._lastStateRequestTimePassed += dt;
			if (this._lastStateRequestTimePassed >= 10f)
			{
				List<PlayerId> list = new List<PlayerId>();
				list.AddRange(this.InGameFriends.FriendList.Select((MPLobbyFriendItemVM p) => p.ProvidedID));
				list.AddRange(this.OnlineFriends.FriendList.Select((MPLobbyFriendItemVM p) => p.ProvidedID));
				list.AddRange(this.OfflineFriends.FriendList.Select((MPLobbyFriendItemVM p) => p.ProvidedID));
				this._lastStateRequestTimePassed = 0f;
				this.UpdatePlayerStates(list);
			}
		}

		// Token: 0x06000B5F RID: 2911 RVA: 0x000286A4 File Offset: 0x000268A4
		private async void UpdatePlayerStates(List<PlayerId> players)
		{
			if (players != null && players.Count > 0)
			{
				this._isStateRequestActive = true;
				List<ValueTuple<PlayerId, AnotherPlayerData>> list = await NetworkMain.GameClient.GetOtherPlayersState(players);
				if (list != null)
				{
					foreach (ValueTuple<PlayerId, AnotherPlayerData> valueTuple in list)
					{
						PlayerId item = valueTuple.Item1;
						AnotherPlayerData item2 = valueTuple.Item2;
						MPLobbyPlayerBaseVM friendWithID = this.GetFriendWithID(item);
						if (friendWithID != null)
						{
							friendWithID.UpdatePlayerState(item2);
						}
					}
					MPLobbyFriendGroupVM inGameFriends = this.InGameFriends;
					if (inGameFriends != null)
					{
						MBBindingList<MPLobbyFriendItemVM> friendList = inGameFriends.FriendList;
						if (friendList != null)
						{
							friendList.Sort(this._playerStateComparer);
						}
					}
					MPLobbyFriendGroupVM onlineFriends = this.OnlineFriends;
					if (onlineFriends != null)
					{
						MBBindingList<MPLobbyFriendItemVM> friendList2 = onlineFriends.FriendList;
						if (friendList2 != null)
						{
							friendList2.Sort(this._playerStateComparer);
						}
					}
					MPLobbyFriendGroupVM offlineFriends = this.OfflineFriends;
					if (offlineFriends != null)
					{
						MBBindingList<MPLobbyFriendItemVM> friendList3 = offlineFriends.FriendList;
						if (friendList3 != null)
						{
							friendList3.Sort(this._playerStateComparer);
						}
					}
				}
				this._isStateRequestActive = false;
				this.UpdateCanInviteOtherPlayersToParty();
			}
		}

		// Token: 0x06000B60 RID: 2912 RVA: 0x000286E5 File Offset: 0x000268E5
		private void BlockedUserListChanged()
		{
			this.GetFriends();
		}

		// Token: 0x06000B61 RID: 2913 RVA: 0x000286ED File Offset: 0x000268ED
		private void FriendListChanged()
		{
			this.GetFriends();
		}

		// Token: 0x06000B62 RID: 2914 RVA: 0x000286F5 File Offset: 0x000268F5
		public void ForceRefresh()
		{
			this.GetFriends();
		}

		// Token: 0x06000B63 RID: 2915 RVA: 0x00028700 File Offset: 0x00026900
		private async void UserOnlineStatusChanged(PlayerId providedId)
		{
			await this.CreateAndAddFriendToList(providedId);
		}

		// Token: 0x06000B64 RID: 2916 RVA: 0x00028741 File Offset: 0x00026941
		private void FriendRemoved(PlayerId providedId)
		{
			this.RemoveFriend(providedId);
		}

		// Token: 0x06000B65 RID: 2917 RVA: 0x0002874C File Offset: 0x0002694C
		private void RemoveFriend(PlayerId providedId)
		{
			MPLobbyFriendItemVM mplobbyFriendItemVM = this.InGameFriends.FriendList.FirstOrDefault((MPLobbyFriendItemVM p) => p.ProvidedID == providedId);
			if (mplobbyFriendItemVM != null)
			{
				this.InGameFriends.RemoveFriend(mplobbyFriendItemVM);
			}
			if (mplobbyFriendItemVM == null)
			{
				mplobbyFriendItemVM = this.OnlineFriends.FriendList.FirstOrDefault((MPLobbyFriendItemVM p) => p.ProvidedID == providedId);
				if (mplobbyFriendItemVM != null)
				{
					this.OnlineFriends.RemoveFriend(mplobbyFriendItemVM);
				}
			}
			if (mplobbyFriendItemVM == null)
			{
				mplobbyFriendItemVM = this.OfflineFriends.FriendList.FirstOrDefault((MPLobbyFriendItemVM p) => p.ProvidedID == providedId);
				if (mplobbyFriendItemVM != null)
				{
					this.OfflineFriends.RemoveFriend(mplobbyFriendItemVM);
				}
			}
		}

		// Token: 0x06000B66 RID: 2918 RVA: 0x000287F4 File Offset: 0x000269F4
		private async Task CreateAndAddFriendToList(PlayerId playerId)
		{
			if (!MultiplayerPlayerHelper.IsBlocked(playerId))
			{
				this.RemoveFriend(playerId);
				MPLobbyPlayerBaseVM.OnlineStatus onlineStatus = await this.GetOnlineStatus(playerId);
				MPLobbyFriendItemVM mplobbyFriendItemVM = new MPLobbyFriendItemVM(playerId, this._activatePlayerActions, new Action<PlayerId>(this.ExecuteInviteToClan), null)
				{
					CanRemove = (this.FriendListService.AllowsFriendOperations && this.FriendListService.IncludeInAllFriends)
				};
				if (this._isInGameFriendsRelevant)
				{
					if (onlineStatus == MPLobbyPlayerBaseVM.OnlineStatus.InGame)
					{
						this.InGameFriends.AddFriend(mplobbyFriendItemVM);
					}
					else if (onlineStatus == MPLobbyPlayerBaseVM.OnlineStatus.Online)
					{
						this.OnlineFriends.AddFriend(mplobbyFriendItemVM);
					}
					else if (onlineStatus == MPLobbyPlayerBaseVM.OnlineStatus.Offline)
					{
						this.OfflineFriends.AddFriend(mplobbyFriendItemVM);
					}
				}
				else if (onlineStatus == MPLobbyPlayerBaseVM.OnlineStatus.InGame || onlineStatus == MPLobbyPlayerBaseVM.OnlineStatus.Online)
				{
					this.OnlineFriends.AddFriend(mplobbyFriendItemVM);
				}
				else
				{
					this.OfflineFriends.AddFriend(mplobbyFriendItemVM);
				}
				mplobbyFriendItemVM.OnStatusChanged(onlineStatus, this.IsInGameStatusActive);
			}
		}

		// Token: 0x06000B67 RID: 2919 RVA: 0x00028844 File Offset: 0x00026A44
		private MPLobbyPlayerBaseVM GetFriendWithID(PlayerId playerId)
		{
			MPLobbyFriendItemVM mplobbyFriendItemVM = this._onlineFriends.FriendList.FirstOrDefault((MPLobbyFriendItemVM p) => p.ProvidedID == playerId);
			if (mplobbyFriendItemVM != null)
			{
				return mplobbyFriendItemVM;
			}
			MPLobbyFriendItemVM mplobbyFriendItemVM2 = this._inGameFriends.FriendList.FirstOrDefault((MPLobbyFriendItemVM p) => p.ProvidedID == playerId);
			if (mplobbyFriendItemVM2 != null)
			{
				return mplobbyFriendItemVM2;
			}
			MPLobbyFriendItemVM mplobbyFriendItemVM3 = this._offlineFriends.FriendList.FirstOrDefault((MPLobbyFriendItemVM p) => p.ProvidedID == playerId);
			if (mplobbyFriendItemVM3 != null)
			{
				return mplobbyFriendItemVM3;
			}
			return null;
		}

		// Token: 0x06000B68 RID: 2920 RVA: 0x000288C8 File Offset: 0x00026AC8
		public void UpdateCanInviteOtherPlayersToParty()
		{
			this.OfflineFriends.FriendList.ApplyActionOnAllItems(delegate(MPLobbyFriendItemVM f)
			{
				f.SetOnInvite(null);
			});
			this.PendingRequests.FriendList.ApplyActionOnAllItems(delegate(MPLobbyFriendItemVM f)
			{
				f.SetOnInvite(null);
			});
			this.FriendRequests.FriendList.ApplyActionOnAllItems(delegate(MPLobbyFriendItemVM f)
			{
				f.SetOnInvite(null);
			});
			this.OnlineFriends.FriendList.ApplyActionOnAllItems(delegate(MPLobbyFriendItemVM f)
			{
				f.SetOnInvite(this.GetOnInvite(f.ProvidedID, f.CurrentOnlineStatus, f.State));
			});
			this.InGameFriends.FriendList.ApplyActionOnAllItems(delegate(MPLobbyFriendItemVM f)
			{
				f.SetOnInvite(this.GetOnInvite(f.ProvidedID, f.CurrentOnlineStatus, f.State));
			});
		}

		// Token: 0x06000B69 RID: 2921 RVA: 0x0002899C File Offset: 0x00026B9C
		public void OnFriendListUpdated(bool updateForced = false)
		{
			if (!this._populatingFriends)
			{
				this.InGameFriends.FriendList.ApplyActionOnAllItems(delegate(MPLobbyFriendItemVM f)
				{
					f.UpdateNameAndAvatar(updateForced);
				});
				this.OnlineFriends.FriendList.ApplyActionOnAllItems(delegate(MPLobbyFriendItemVM f)
				{
					f.UpdateNameAndAvatar(updateForced);
				});
				this.OfflineFriends.FriendList.ApplyActionOnAllItems(delegate(MPLobbyFriendItemVM f)
				{
					f.UpdateNameAndAvatar(updateForced);
				});
				this.FriendRequests.FriendList.ApplyActionOnAllItems(delegate(MPLobbyFriendItemVM f)
				{
					f.UpdateNameAndAvatar(updateForced);
				});
				this.PendingRequests.FriendList.ApplyActionOnAllItems(delegate(MPLobbyFriendItemVM f)
				{
					f.UpdateNameAndAvatar(updateForced);
				});
			}
		}

		// Token: 0x06000B6A RID: 2922 RVA: 0x00028A50 File Offset: 0x00026C50
		private async Task<MPLobbyPlayerBaseVM.OnlineStatus> GetOnlineStatus(PlayerId playerId)
		{
			bool flag = await this.FriendListService.GetUserOnlineStatus(playerId);
			bool isOnline = flag;
			bool flag2 = false;
			if (this.IsInGameStatusActive)
			{
				flag2 = await this.FriendListService.IsPlayingThisGame(playerId);
			}
			MPLobbyPlayerBaseVM.OnlineStatus onlineStatus;
			if (isOnline)
			{
				if (!flag2)
				{
					onlineStatus = MPLobbyPlayerBaseVM.OnlineStatus.Online;
				}
				else
				{
					onlineStatus = MPLobbyPlayerBaseVM.OnlineStatus.InGame;
				}
			}
			else
			{
				onlineStatus = MPLobbyPlayerBaseVM.OnlineStatus.Offline;
			}
			return onlineStatus;
		}

		// Token: 0x06000B6B RID: 2923 RVA: 0x00028AA0 File Offset: 0x00026CA0
		private Action<PlayerId> GetOnInvite(PlayerId playerId, MPLobbyPlayerBaseVM.OnlineStatus onlineStatus, AnotherPlayerState state)
		{
			Action<PlayerId> action = null;
			if (playerId.ProvidedType == PlayerIdProvidedTypes.GDK)
			{
				return null;
			}
			if (PlatformServices.InvitationServices != null && playerId.ProvidedType == NetworkMain.GameClient.PlayerID.ProvidedType)
			{
				action = new Action<PlayerId>(this.ExecuteInviteToPlatformSession);
			}
			else if (onlineStatus == MPLobbyPlayerBaseVM.OnlineStatus.Offline || onlineStatus == MPLobbyPlayerBaseVM.OnlineStatus.None)
			{
				action = null;
			}
			else if (state == AnotherPlayerState.AtLobby)
			{
				action = new Action<PlayerId>(this.ExecuteInviteToParty);
			}
			return action;
		}

		// Token: 0x06000B6C RID: 2924 RVA: 0x00028B0C File Offset: 0x00026D0C
		private void ExecuteInviteToParty(PlayerId providedId)
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

		// Token: 0x06000B6D RID: 2925 RVA: 0x00028B64 File Offset: 0x00026D64
		private void ExecuteInviteToPlatformSession(PlayerId providedId)
		{
			MPLobbyFriendServiceVM.<>c__DisplayClass43_0 CS$<>8__locals1 = new MPLobbyFriendServiceVM.<>c__DisplayClass43_0();
			CS$<>8__locals1.providedId = providedId;
			CS$<>8__locals1.friend = this.GetFriendWithID(CS$<>8__locals1.providedId);
			CS$<>8__locals1.friend.CanBeInvited = false;
			PlatformServices.Instance.CheckPrivilege(Privilege.Communication, true, delegate(bool result)
			{
				if (result)
				{
					IPlatformServices instance = PlatformServices.Instance;
					Permission permission = Permission.PlayMultiplayer;
					PlayerId providedId2 = CS$<>8__locals1.providedId;
					PermissionResult permissionResult2;
					if ((permissionResult2 = CS$<>8__locals1.<>9__1) == null)
					{
						permissionResult2 = (CS$<>8__locals1.<>9__1 = delegate(bool permissionResult)
						{
							MPLobbyFriendServiceVM.<>c__DisplayClass43_0.<<ExecuteInviteToPlatformSession>b__1>d <<ExecuteInviteToPlatformSession>b__1>d;
							<<ExecuteInviteToPlatformSession>b__1>d.<>4__this = CS$<>8__locals1;
							<<ExecuteInviteToPlatformSession>b__1>d.permissionResult = permissionResult;
							<<ExecuteInviteToPlatformSession>b__1>d.<>t__builder = AsyncVoidMethodBuilder.Create();
							<<ExecuteInviteToPlatformSession>b__1>d.<>1__state = -1;
							AsyncVoidMethodBuilder <>t__builder = <<ExecuteInviteToPlatformSession>b__1>d.<>t__builder;
							<>t__builder.Start<MPLobbyFriendServiceVM.<>c__DisplayClass43_0.<<ExecuteInviteToPlatformSession>b__1>d>(ref <<ExecuteInviteToPlatformSession>b__1>d);
						});
					}
					instance.CheckPermissionWithUser(permission, providedId2, permissionResult2);
					return;
				}
				CS$<>8__locals1.friend.CanBeInvited = true;
			});
		}

		// Token: 0x06000B6E RID: 2926 RVA: 0x00028BB4 File Offset: 0x00026DB4
		private void ExecuteInviteToClan(PlayerId providedId)
		{
			PlatformServices.Instance.CheckPrivilege(Privilege.Clan, true, delegate(bool result)
			{
				if (result)
				{
					bool flag = BannerlordConfig.EnableGenericNames && !NetworkMain.GameClient.IsKnownPlayer(providedId);
					NetworkMain.GameClient.InviteToClan(providedId, flag);
				}
			});
		}

		// Token: 0x17000385 RID: 901
		// (get) Token: 0x06000B6F RID: 2927 RVA: 0x00028BE6 File Offset: 0x00026DE6
		// (set) Token: 0x06000B70 RID: 2928 RVA: 0x00028BEE File Offset: 0x00026DEE
		[DataSourceProperty]
		public bool IsInGameStatusActive
		{
			get
			{
				return this._isInGameStatusActive;
			}
			set
			{
				if (value != this._isInGameStatusActive)
				{
					this._isInGameStatusActive = value;
					base.OnPropertyChangedWithValue(value, "IsInGameStatusActive");
				}
			}
		}

		// Token: 0x17000386 RID: 902
		// (get) Token: 0x06000B71 RID: 2929 RVA: 0x00028C0C File Offset: 0x00026E0C
		// (set) Token: 0x06000B72 RID: 2930 RVA: 0x00028C14 File Offset: 0x00026E14
		[DataSourceProperty]
		public MPLobbyFriendGroupVM InGameFriends
		{
			get
			{
				return this._inGameFriends;
			}
			set
			{
				if (value != this._inGameFriends)
				{
					this._inGameFriends = value;
					base.OnPropertyChangedWithValue<MPLobbyFriendGroupVM>(value, "InGameFriends");
				}
			}
		}

		// Token: 0x17000387 RID: 903
		// (get) Token: 0x06000B73 RID: 2931 RVA: 0x00028C32 File Offset: 0x00026E32
		// (set) Token: 0x06000B74 RID: 2932 RVA: 0x00028C3A File Offset: 0x00026E3A
		[DataSourceProperty]
		public MPLobbyFriendGroupVM OnlineFriends
		{
			get
			{
				return this._onlineFriends;
			}
			set
			{
				if (value != this._onlineFriends)
				{
					this._onlineFriends = value;
					base.OnPropertyChangedWithValue<MPLobbyFriendGroupVM>(value, "OnlineFriends");
				}
			}
		}

		// Token: 0x17000388 RID: 904
		// (get) Token: 0x06000B75 RID: 2933 RVA: 0x00028C58 File Offset: 0x00026E58
		// (set) Token: 0x06000B76 RID: 2934 RVA: 0x00028C60 File Offset: 0x00026E60
		[DataSourceProperty]
		public MPLobbyFriendGroupVM OfflineFriends
		{
			get
			{
				return this._offlineFriends;
			}
			set
			{
				if (value != this._offlineFriends)
				{
					this._offlineFriends = value;
					base.OnPropertyChangedWithValue<MPLobbyFriendGroupVM>(value, "OfflineFriends");
				}
			}
		}

		// Token: 0x17000389 RID: 905
		// (get) Token: 0x06000B77 RID: 2935 RVA: 0x00028C7E File Offset: 0x00026E7E
		// (set) Token: 0x06000B78 RID: 2936 RVA: 0x00028C86 File Offset: 0x00026E86
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

		// Token: 0x1700038A RID: 906
		// (get) Token: 0x06000B79 RID: 2937 RVA: 0x00028CA9 File Offset: 0x00026EA9
		// (set) Token: 0x06000B7A RID: 2938 RVA: 0x00028CB1 File Offset: 0x00026EB1
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

		// Token: 0x1700038B RID: 907
		// (get) Token: 0x06000B7B RID: 2939 RVA: 0x00028CD4 File Offset: 0x00026ED4
		// (set) Token: 0x06000B7C RID: 2940 RVA: 0x00028CDC File Offset: 0x00026EDC
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

		// Token: 0x1700038C RID: 908
		// (get) Token: 0x06000B7D RID: 2941 RVA: 0x00028CFF File Offset: 0x00026EFF
		// (set) Token: 0x06000B7E RID: 2942 RVA: 0x00028D07 File Offset: 0x00026F07
		[DataSourceProperty]
		public string ServiceName
		{
			get
			{
				return this._serviceName;
			}
			set
			{
				if (value != this._serviceName)
				{
					this._serviceName = value;
					base.OnPropertyChangedWithValue<string>(value, "ServiceName");
				}
			}
		}

		// Token: 0x1700038D RID: 909
		// (get) Token: 0x06000B7F RID: 2943 RVA: 0x00028D2A File Offset: 0x00026F2A
		// (set) Token: 0x06000B80 RID: 2944 RVA: 0x00028D32 File Offset: 0x00026F32
		[DataSourceProperty]
		public MPLobbyFriendGroupVM FriendRequests
		{
			get
			{
				return this._friendRequests;
			}
			set
			{
				if (value != this._friendRequests)
				{
					this._friendRequests = value;
					base.OnPropertyChangedWithValue<MPLobbyFriendGroupVM>(value, "FriendRequests");
				}
			}
		}

		// Token: 0x1700038E RID: 910
		// (get) Token: 0x06000B81 RID: 2945 RVA: 0x00028D50 File Offset: 0x00026F50
		// (set) Token: 0x06000B82 RID: 2946 RVA: 0x00028D58 File Offset: 0x00026F58
		[DataSourceProperty]
		public bool GotAnyFriendRequests
		{
			get
			{
				return this._gotAnyFriendRequests;
			}
			set
			{
				if (value != this._gotAnyFriendRequests)
				{
					this._gotAnyFriendRequests = value;
					base.OnPropertyChangedWithValue(value, "GotAnyFriendRequests");
				}
			}
		}

		// Token: 0x1700038F RID: 911
		// (get) Token: 0x06000B83 RID: 2947 RVA: 0x00028D76 File Offset: 0x00026F76
		// (set) Token: 0x06000B84 RID: 2948 RVA: 0x00028D7E File Offset: 0x00026F7E
		[DataSourceProperty]
		public MPLobbyFriendGroupVM PendingRequests
		{
			get
			{
				return this._pendingRequests;
			}
			set
			{
				if (value != this._pendingRequests)
				{
					this._pendingRequests = value;
					base.OnPropertyChangedWithValue<MPLobbyFriendGroupVM>(value, "PendingRequests");
				}
			}
		}

		// Token: 0x17000390 RID: 912
		// (get) Token: 0x06000B85 RID: 2949 RVA: 0x00028D9C File Offset: 0x00026F9C
		// (set) Token: 0x06000B86 RID: 2950 RVA: 0x00028DA4 File Offset: 0x00026FA4
		[DataSourceProperty]
		public bool GotAnyPendingRequests
		{
			get
			{
				return this._gotAnyPendingRequests;
			}
			set
			{
				if (value != this._gotAnyPendingRequests)
				{
					this._gotAnyPendingRequests = value;
					base.OnPropertyChangedWithValue(value, "GotAnyPendingRequests");
				}
			}
		}

		// Token: 0x17000391 RID: 913
		// (get) Token: 0x06000B87 RID: 2951 RVA: 0x00028DC2 File Offset: 0x00026FC2
		// (set) Token: 0x06000B88 RID: 2952 RVA: 0x00028DCA File Offset: 0x00026FCA
		[DataSourceProperty]
		public HintViewModel ServiceNameHint
		{
			get
			{
				return this._serviceNameHint;
			}
			set
			{
				if (value != this._serviceNameHint)
				{
					this._serviceNameHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "ServiceNameHint");
				}
			}
		}

		// Token: 0x0400056F RID: 1391
		private const string _inviteFailedSoundEvent = "event:/ui/notification/quest_update";

		// Token: 0x04000570 RID: 1392
		public readonly IFriendListService FriendListService;

		// Token: 0x04000571 RID: 1393
		private readonly Action<MPLobbyPlayerBaseVM> _activatePlayerActions;

		// Token: 0x04000572 RID: 1394
		private bool _populatingFriends;

		// Token: 0x04000573 RID: 1395
		private bool _isInGameFriendsRelevant;

		// Token: 0x04000574 RID: 1396
		private const float UpdateInterval = 2f;

		// Token: 0x04000575 RID: 1397
		private float _lastUpdateTimePassed;

		// Token: 0x04000576 RID: 1398
		private const float StateRequestInterval = 10f;

		// Token: 0x04000577 RID: 1399
		private float _lastStateRequestTimePassed;

		// Token: 0x04000578 RID: 1400
		private bool _isStateRequestActive;

		// Token: 0x04000579 RID: 1401
		private readonly MPLobbyFriendServiceVM.PlayerStateComparer _playerStateComparer;

		// Token: 0x0400057A RID: 1402
		private Action<PlayerId> _onFriendRequestAnswered;

		// Token: 0x0400057B RID: 1403
		private bool _isPartyAvailable;

		// Token: 0x0400057C RID: 1404
		private static Dictionary<PlayerId, long> _friendRequestsInProcess = new Dictionary<PlayerId, long>();

		// Token: 0x0400057D RID: 1405
		private const int BlockedFriendRequestTimeout = 10000;

		// Token: 0x0400057E RID: 1406
		private bool _isInGameStatusActive;

		// Token: 0x0400057F RID: 1407
		private MPLobbyFriendGroupVM _inGameFriends;

		// Token: 0x04000580 RID: 1408
		private MPLobbyFriendGroupVM _onlineFriends;

		// Token: 0x04000581 RID: 1409
		private MPLobbyFriendGroupVM _offlineFriends;

		// Token: 0x04000582 RID: 1410
		private string _inGameText;

		// Token: 0x04000583 RID: 1411
		private string _onlineText;

		// Token: 0x04000584 RID: 1412
		private string _offlineText;

		// Token: 0x04000585 RID: 1413
		private string _serviceName;

		// Token: 0x04000586 RID: 1414
		private HintViewModel _serviceNameHint;

		// Token: 0x04000587 RID: 1415
		private MPLobbyFriendGroupVM _friendRequests;

		// Token: 0x04000588 RID: 1416
		private bool _gotAnyFriendRequests;

		// Token: 0x04000589 RID: 1417
		private MPLobbyFriendGroupVM _pendingRequests;

		// Token: 0x0400058A RID: 1418
		private bool _gotAnyPendingRequests;

		// Token: 0x020001AB RID: 427
		private class PlayerStateComparer : IComparer<MPLobbyPlayerBaseVM>
		{
			// Token: 0x060019E6 RID: 6630 RVA: 0x00053800 File Offset: 0x00051A00
			public int Compare(MPLobbyPlayerBaseVM x, MPLobbyPlayerBaseVM y)
			{
				int stateImportanceOrder = this.GetStateImportanceOrder(x.State);
				int stateImportanceOrder2 = this.GetStateImportanceOrder(y.State);
				if (stateImportanceOrder != stateImportanceOrder2)
				{
					return stateImportanceOrder.CompareTo(stateImportanceOrder2);
				}
				return x.Name.CompareTo(y.Name);
			}

			// Token: 0x060019E7 RID: 6631 RVA: 0x00053845 File Offset: 0x00051A45
			private int GetStateImportanceOrder(AnotherPlayerState state)
			{
				switch (state)
				{
				case AnotherPlayerState.AtLobby:
					return 0;
				case AnotherPlayerState.InParty:
					return 1;
				case AnotherPlayerState.InMultiplayerGame:
					return 2;
				default:
					return int.MaxValue;
				}
			}
		}
	}
}
