using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaleWorlds.Localization;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.PlatformService.Epic
{
	// Token: 0x02000003 RID: 3
	public class EpicFriendListService : IFriendListService
	{
		// Token: 0x06000006 RID: 6 RVA: 0x00002085 File Offset: 0x00000285
		public EpicFriendListService(EpicPlatformServices epicPlatformServices)
		{
			this._epicPlatformServices = epicPlatformServices;
		}

		// Token: 0x14000001 RID: 1
		// (add) Token: 0x06000007 RID: 7 RVA: 0x00002094 File Offset: 0x00000294
		// (remove) Token: 0x06000008 RID: 8 RVA: 0x000020CC File Offset: 0x000002CC
		public event Action<PlayerId> OnUserStatusChanged;

		// Token: 0x14000002 RID: 2
		// (add) Token: 0x06000009 RID: 9 RVA: 0x00002104 File Offset: 0x00000304
		// (remove) Token: 0x0600000A RID: 10 RVA: 0x0000213C File Offset: 0x0000033C
		public event Action<PlayerId> OnFriendRemoved;

		// Token: 0x14000003 RID: 3
		// (add) Token: 0x0600000B RID: 11 RVA: 0x00002174 File Offset: 0x00000374
		// (remove) Token: 0x0600000C RID: 12 RVA: 0x000021AC File Offset: 0x000003AC
		public event Action OnFriendListChanged;

		// Token: 0x17000001 RID: 1
		// (get) Token: 0x0600000D RID: 13 RVA: 0x000021E1 File Offset: 0x000003E1
		bool IFriendListService.InGameStatusFetchable
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x0600000E RID: 14 RVA: 0x000021E4 File Offset: 0x000003E4
		bool IFriendListService.AllowsFriendOperations
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x0600000F RID: 15 RVA: 0x000021E7 File Offset: 0x000003E7
		bool IFriendListService.CanInvitePlayersToPlatformSession
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x06000010 RID: 16 RVA: 0x000021EA File Offset: 0x000003EA
		bool IFriendListService.IncludeInAllFriends
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06000011 RID: 17 RVA: 0x000021ED File Offset: 0x000003ED
		string IFriendListService.GetServiceCodeName()
		{
			return "Epic";
		}

		// Token: 0x06000012 RID: 18 RVA: 0x000021F4 File Offset: 0x000003F4
		TextObject IFriendListService.GetServiceLocalizedName()
		{
			return new TextObject("{=!}Epic", null);
		}

		// Token: 0x06000013 RID: 19 RVA: 0x00002201 File Offset: 0x00000401
		FriendListServiceType IFriendListService.GetFriendListServiceType()
		{
			return FriendListServiceType.Epic;
		}

		// Token: 0x06000014 RID: 20 RVA: 0x00002204 File Offset: 0x00000404
		IEnumerable<PlayerId> IFriendListService.GetAllFriends()
		{
			return this._epicPlatformServices.GetAllFriends();
		}

		// Token: 0x06000015 RID: 21 RVA: 0x00002211 File Offset: 0x00000411
		Task<bool> IFriendListService.GetUserOnlineStatus(PlayerId providedId)
		{
			return this._epicPlatformServices.GetUserOnlineStatus(providedId);
		}

		// Token: 0x06000016 RID: 22 RVA: 0x0000221F File Offset: 0x0000041F
		Task<bool> IFriendListService.IsPlayingThisGame(PlayerId providedId)
		{
			return this._epicPlatformServices.IsPlayingThisGame(providedId);
		}

		// Token: 0x06000017 RID: 23 RVA: 0x0000222D File Offset: 0x0000042D
		Task<string> IFriendListService.GetUserName(PlayerId providedId)
		{
			return this._epicPlatformServices.GetUserName(providedId);
		}

		// Token: 0x06000018 RID: 24 RVA: 0x0000223B File Offset: 0x0000043B
		Task<PlayerId> IFriendListService.GetUserWithName(string name)
		{
			return this._epicPlatformServices.GetUserWithName(name);
		}

		// Token: 0x06000019 RID: 25 RVA: 0x0000224C File Offset: 0x0000044C
		public void UserStatusChanged(PlayerId playerId)
		{
			if (this.OnUserStatusChanged != null)
			{
				this.OnUserStatusChanged(default(PlayerId));
			}
		}

		// Token: 0x0600001A RID: 26 RVA: 0x00002275 File Offset: 0x00000475
		IEnumerable<PlayerId> IFriendListService.GetPendingRequests()
		{
			return null;
		}

		// Token: 0x0600001B RID: 27 RVA: 0x00002278 File Offset: 0x00000478
		IEnumerable<PlayerId> IFriendListService.GetReceivedRequests()
		{
			return null;
		}

		// Token: 0x0600001C RID: 28 RVA: 0x0000227C File Offset: 0x0000047C
		private void Dummy()
		{
			if (this.OnFriendRemoved != null)
			{
				this.OnFriendRemoved(default(PlayerId));
			}
			if (this.OnFriendListChanged != null)
			{
				this.OnFriendListChanged();
			}
		}

		// Token: 0x04000002 RID: 2
		private EpicPlatformServices _epicPlatformServices;
	}
}
