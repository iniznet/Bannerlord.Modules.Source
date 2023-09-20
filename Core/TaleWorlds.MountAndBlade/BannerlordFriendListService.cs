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
	public class BannerlordFriendListService : IFriendListService
	{
		public event Action<PlayerId> OnUserStatusChanged;

		public event Action<PlayerId> OnFriendRemoved;

		public event Action OnFriendListChanged;

		bool IFriendListService.InGameStatusFetchable
		{
			get
			{
				return false;
			}
		}

		bool IFriendListService.AllowsFriendOperations
		{
			get
			{
				return true;
			}
		}

		bool IFriendListService.CanInvitePlayersToPlatformSession
		{
			get
			{
				return PlatformServices.InvitationServices != null;
			}
		}

		bool IFriendListService.IncludeInAllFriends
		{
			get
			{
				return true;
			}
		}

		public BannerlordFriendListService()
		{
			this.Friends = new List<FriendInfo>();
		}

		string IFriendListService.GetServiceCodeName()
		{
			return "TaleWorlds";
		}

		TextObject IFriendListService.GetServiceLocalizedName()
		{
			return new TextObject("{=!}TaleWorlds", null);
		}

		FriendListServiceType IFriendListService.GetFriendListServiceType()
		{
			return FriendListServiceType.Bannerlord;
		}

		IEnumerable<PlayerId> IFriendListService.GetPendingRequests()
		{
			return from f in this.Friends
				where f.Status == FriendStatus.Pending
				select f.Id;
		}

		IEnumerable<PlayerId> IFriendListService.GetReceivedRequests()
		{
			return from f in this.Friends
				where f.Status == FriendStatus.Received
				select f.Id;
		}

		IEnumerable<PlayerId> IFriendListService.GetAllFriends()
		{
			return from f in this.Friends
				where f.Status == FriendStatus.Accepted
				select f.Id;
		}

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

		Task<bool> IFriendListService.IsPlayingThisGame(PlayerId providedId)
		{
			return ((IFriendListService)this).GetUserOnlineStatus(providedId);
		}

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

		protected List<FriendInfo> Friends;
	}
}
