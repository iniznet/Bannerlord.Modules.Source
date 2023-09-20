using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.PlatformService;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020002E3 RID: 739
	public class BannerlordFriendListService : IFriendListService
	{
		// Token: 0x14000076 RID: 118
		// (add) Token: 0x06002856 RID: 10326 RVA: 0x0009C130 File Offset: 0x0009A330
		// (remove) Token: 0x06002857 RID: 10327 RVA: 0x0009C168 File Offset: 0x0009A368
		public event Action<PlayerId> OnUserStatusChanged;

		// Token: 0x14000077 RID: 119
		// (add) Token: 0x06002858 RID: 10328 RVA: 0x0009C1A0 File Offset: 0x0009A3A0
		// (remove) Token: 0x06002859 RID: 10329 RVA: 0x0009C1D8 File Offset: 0x0009A3D8
		public event Action<PlayerId> OnFriendRemoved;

		// Token: 0x14000078 RID: 120
		// (add) Token: 0x0600285A RID: 10330 RVA: 0x0009C210 File Offset: 0x0009A410
		// (remove) Token: 0x0600285B RID: 10331 RVA: 0x0009C248 File Offset: 0x0009A448
		public event Action OnFriendListChanged;

		// Token: 0x17000748 RID: 1864
		// (get) Token: 0x0600285C RID: 10332 RVA: 0x0009C27D File Offset: 0x0009A47D
		bool IFriendListService.InGameStatusFetchable
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000749 RID: 1865
		// (get) Token: 0x0600285D RID: 10333 RVA: 0x0009C280 File Offset: 0x0009A480
		bool IFriendListService.AllowsFriendOperations
		{
			get
			{
				return true;
			}
		}

		// Token: 0x1700074A RID: 1866
		// (get) Token: 0x0600285E RID: 10334 RVA: 0x0009C283 File Offset: 0x0009A483
		bool IFriendListService.CanInvitePlayersToPlatformSession
		{
			get
			{
				return PlatformServices.InvitationServices != null;
			}
		}

		// Token: 0x1700074B RID: 1867
		// (get) Token: 0x0600285F RID: 10335 RVA: 0x0009C28D File Offset: 0x0009A48D
		bool IFriendListService.IncludeInAllFriends
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06002860 RID: 10336 RVA: 0x0009C290 File Offset: 0x0009A490
		public BannerlordFriendListService()
		{
			this.Friends = new List<FriendInfo>();
		}

		// Token: 0x06002861 RID: 10337 RVA: 0x0009C2A3 File Offset: 0x0009A4A3
		string IFriendListService.GetServiceCodeName()
		{
			return "TaleWorlds";
		}

		// Token: 0x06002862 RID: 10338 RVA: 0x0009C2AA File Offset: 0x0009A4AA
		TextObject IFriendListService.GetServiceLocalizedName()
		{
			return new TextObject("{=!}TaleWorlds", null);
		}

		// Token: 0x06002863 RID: 10339 RVA: 0x0009C2B7 File Offset: 0x0009A4B7
		FriendListServiceType IFriendListService.GetFriendListServiceType()
		{
			return FriendListServiceType.Bannerlord;
		}

		// Token: 0x06002864 RID: 10340 RVA: 0x0009C2BC File Offset: 0x0009A4BC
		IEnumerable<PlayerId> IFriendListService.GetPendingRequests()
		{
			return from f in this.Friends
				where f.Status == FriendStatus.Pending
				select f.Id;
		}

		// Token: 0x06002865 RID: 10341 RVA: 0x0009C318 File Offset: 0x0009A518
		IEnumerable<PlayerId> IFriendListService.GetReceivedRequests()
		{
			return from f in this.Friends
				where f.Status == FriendStatus.Received
				select f.Id;
		}

		// Token: 0x06002866 RID: 10342 RVA: 0x0009C374 File Offset: 0x0009A574
		IEnumerable<PlayerId> IFriendListService.GetAllFriends()
		{
			return from f in this.Friends
				where f.Status == FriendStatus.Accepted
				select f.Id;
		}

		// Token: 0x06002867 RID: 10343 RVA: 0x0009C3D0 File Offset: 0x0009A5D0
		Task<bool> IFriendListService.GetUserOnlineStatus(PlayerId providedId)
		{
			foreach (FriendInfo friendInfo in this.Friends)
			{
				if (friendInfo.Id.Equals(providedId))
				{
					return Task.FromResult<bool>(friendInfo.IsOnline);
				}
			}
			return Task.FromResult<bool>(false);
		}

		// Token: 0x06002868 RID: 10344 RVA: 0x0009C450 File Offset: 0x0009A650
		Task<bool> IFriendListService.IsPlayingThisGame(PlayerId providedId)
		{
			return ((IFriendListService)this).GetUserOnlineStatus(providedId);
		}

		// Token: 0x06002869 RID: 10345 RVA: 0x0009C45C File Offset: 0x0009A65C
		Task<string> IFriendListService.GetUserName(PlayerId providedId)
		{
			foreach (FriendInfo friendInfo in this.Friends)
			{
				if (friendInfo.Id.Equals(providedId))
				{
					return Task.FromResult<string>(friendInfo.Name);
				}
			}
			return Task.FromResult<string>(null);
		}

		// Token: 0x0600286A RID: 10346 RVA: 0x0009C4DC File Offset: 0x0009A6DC
		Task<PlayerId> IFriendListService.GetUserWithName(string name)
		{
			foreach (FriendInfo friendInfo in this.Friends)
			{
				if (friendInfo.Name == name)
				{
					return Task.FromResult<PlayerId>(friendInfo.Id);
				}
			}
			return Task.FromResult<PlayerId>(default(PlayerId));
		}

		// Token: 0x0600286B RID: 10347 RVA: 0x0009C554 File Offset: 0x0009A754
		public void OnFriendListReceived(FriendInfo[] friends)
		{
			List<FriendInfo> friends2 = this.Friends;
			this.Friends = new List<FriendInfo>(friends);
			List<PlayerId> list = null;
			bool flag = false;
			using (List<FriendInfo>.Enumerator enumerator = this.Friends.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					FriendInfo friend = enumerator.Current;
					int num = friends2.FindIndex((FriendInfo o) => o.Id.Equals(friend.Id));
					if (num < 0)
					{
						flag = true;
					}
					else
					{
						FriendInfo friendInfo = friends2[num];
						friends2.RemoveAt(num);
						if (friendInfo.Status != friend.Status)
						{
							flag = true;
						}
						else if (friendInfo.IsOnline != friend.IsOnline)
						{
							if (list == null)
							{
								list = new List<PlayerId>();
							}
							list.Add(friendInfo.Id);
						}
					}
					if (flag)
					{
						break;
					}
				}
			}
			if (!flag)
			{
				if (friends2.Count > 0)
				{
					foreach (FriendInfo friendInfo2 in friends2)
					{
						Action<PlayerId> onFriendRemoved = this.OnFriendRemoved;
						if (onFriendRemoved != null)
						{
							onFriendRemoved(friendInfo2.Id);
						}
					}
				}
				if (list != null)
				{
					foreach (PlayerId playerId in list)
					{
						Action<PlayerId> onUserStatusChanged = this.OnUserStatusChanged;
						if (onUserStatusChanged != null)
						{
							onUserStatusChanged(playerId);
						}
					}
				}
				return;
			}
			Action onFriendListChanged = this.OnFriendListChanged;
			if (onFriendListChanged == null)
			{
				return;
			}
			onFriendListChanged();
		}

		// Token: 0x04000ECF RID: 3791
		protected List<FriendInfo> Friends;
	}
}
