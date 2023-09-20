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
	public class MPLobbyFriendServiceVM : ViewModel
	{
		public IEnumerable<MPLobbyPlayerBaseVM> AllFriends
		{
			get
			{
				return this.InGameFriends.FriendList.Union(this.OnlineFriends.FriendList.Union(this.OfflineFriends.FriendList));
			}
		}

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

		public override void OnFinalize()
		{
			PlatformServices.Instance.OnBlockedUserListUpdated -= this.BlockedUserListChanged;
			this.FriendListService.OnUserStatusChanged -= this.UserOnlineStatusChanged;
			this.FriendListService.OnFriendRemoved -= this.FriendRemoved;
			this.FriendListService.OnFriendListChanged -= this.FriendListChanged;
			base.OnFinalize();
		}

		public void OnStateActivate()
		{
			this._isPartyAvailable = NetworkMain.GameClient.PartySystemAvailable;
			this.IsInGameStatusActive = this.FriendListService.InGameStatusFetchable && this._isInGameFriendsRelevant;
			this.GetFriends();
		}

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

		private void BlockFriendRequest(PlayerId friendId)
		{
			bool flag = BannerlordConfig.EnableGenericNames && !NetworkMain.GameClient.IsKnownPlayer(friendId);
			NetworkMain.GameClient.RespondToFriendRequest(friendId, flag, false, true);
		}

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

		private void AddFriendRequestItem(PlayerId playerID)
		{
			MPLobbyFriendItemVM mplobbyFriendItemVM = new MPLobbyFriendItemVM(playerID, this._activatePlayerActions, null, this._onFriendRequestAnswered);
			mplobbyFriendItemVM.IsFriendRequest = true;
			mplobbyFriendItemVM.CanRemove = false;
			this.FriendRequests.AddFriend(mplobbyFriendItemVM);
		}

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

		private void BlockedUserListChanged()
		{
			this.GetFriends();
		}

		private void FriendListChanged()
		{
			this.GetFriends();
		}

		public void ForceRefresh()
		{
			this.GetFriends();
		}

		private async void UserOnlineStatusChanged(PlayerId providedId)
		{
			await this.CreateAndAddFriendToList(providedId);
		}

		private void FriendRemoved(PlayerId providedId)
		{
			this.RemoveFriend(providedId);
		}

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

		private const string _inviteFailedSoundEvent = "event:/ui/notification/quest_update";

		public readonly IFriendListService FriendListService;

		private readonly Action<MPLobbyPlayerBaseVM> _activatePlayerActions;

		private bool _populatingFriends;

		private bool _isInGameFriendsRelevant;

		private const float UpdateInterval = 2f;

		private float _lastUpdateTimePassed;

		private const float StateRequestInterval = 10f;

		private float _lastStateRequestTimePassed;

		private bool _isStateRequestActive;

		private readonly MPLobbyFriendServiceVM.PlayerStateComparer _playerStateComparer;

		private Action<PlayerId> _onFriendRequestAnswered;

		private bool _isPartyAvailable;

		private static Dictionary<PlayerId, long> _friendRequestsInProcess = new Dictionary<PlayerId, long>();

		private const int BlockedFriendRequestTimeout = 10000;

		private bool _isInGameStatusActive;

		private MPLobbyFriendGroupVM _inGameFriends;

		private MPLobbyFriendGroupVM _onlineFriends;

		private MPLobbyFriendGroupVM _offlineFriends;

		private string _inGameText;

		private string _onlineText;

		private string _offlineText;

		private string _serviceName;

		private HintViewModel _serviceNameHint;

		private MPLobbyFriendGroupVM _friendRequests;

		private bool _gotAnyFriendRequests;

		private MPLobbyFriendGroupVM _pendingRequests;

		private bool _gotAnyPendingRequests;

		private class PlayerStateComparer : IComparer<MPLobbyPlayerBaseVM>
		{
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
