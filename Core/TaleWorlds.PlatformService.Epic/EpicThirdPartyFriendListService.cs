using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaleWorlds.Localization;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.PlatformService.Epic
{
	// Token: 0x02000006 RID: 6
	public class EpicThirdPartyFriendListService : IFriendListService
	{
		// Token: 0x14000008 RID: 8
		// (add) Token: 0x0600005C RID: 92 RVA: 0x00002F98 File Offset: 0x00001198
		// (remove) Token: 0x0600005D RID: 93 RVA: 0x00002FD0 File Offset: 0x000011D0
		public event Action<PlayerId> OnUserStatusChanged;

		// Token: 0x14000009 RID: 9
		// (add) Token: 0x0600005E RID: 94 RVA: 0x00003008 File Offset: 0x00001208
		// (remove) Token: 0x0600005F RID: 95 RVA: 0x00003040 File Offset: 0x00001240
		public event Action<PlayerId> OnFriendRemoved;

		// Token: 0x1400000A RID: 10
		// (add) Token: 0x06000060 RID: 96 RVA: 0x00003078 File Offset: 0x00001278
		// (remove) Token: 0x06000061 RID: 97 RVA: 0x000030B0 File Offset: 0x000012B0
		public event Action OnFriendListChanged;

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x06000062 RID: 98 RVA: 0x000030E5 File Offset: 0x000012E5
		bool IFriendListService.InGameStatusFetchable
		{
			get
			{
				return false;
			}
		}

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x06000063 RID: 99 RVA: 0x000030E8 File Offset: 0x000012E8
		bool IFriendListService.AllowsFriendOperations
		{
			get
			{
				return false;
			}
		}

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x06000064 RID: 100 RVA: 0x000030EB File Offset: 0x000012EB
		bool IFriendListService.CanInvitePlayersToPlatformSession
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x06000065 RID: 101 RVA: 0x000030EE File Offset: 0x000012EE
		bool IFriendListService.IncludeInAllFriends
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06000066 RID: 102 RVA: 0x000030F1 File Offset: 0x000012F1
		string IFriendListService.GetServiceCodeName()
		{
			return "EpicThirdParty";
		}

		// Token: 0x06000067 RID: 103 RVA: 0x000030F8 File Offset: 0x000012F8
		TextObject IFriendListService.GetServiceLocalizedName()
		{
			return new TextObject("{=!}Epic Third Party", null);
		}

		// Token: 0x06000068 RID: 104 RVA: 0x00003105 File Offset: 0x00001305
		FriendListServiceType IFriendListService.GetFriendListServiceType()
		{
			return FriendListServiceType.EpicThirdParty;
		}

		// Token: 0x06000069 RID: 105 RVA: 0x00003108 File Offset: 0x00001308
		IEnumerable<PlayerId> IFriendListService.GetAllFriends()
		{
			return null;
		}

		// Token: 0x0600006A RID: 106 RVA: 0x0000310B File Offset: 0x0000130B
		Task<bool> IFriendListService.GetUserOnlineStatus(PlayerId providedId)
		{
			return Task.FromResult<bool>(false);
		}

		// Token: 0x0600006B RID: 107 RVA: 0x00003113 File Offset: 0x00001313
		Task<bool> IFriendListService.IsPlayingThisGame(PlayerId providedId)
		{
			return Task.FromResult<bool>(false);
		}

		// Token: 0x0600006C RID: 108 RVA: 0x0000311B File Offset: 0x0000131B
		Task<string> IFriendListService.GetUserName(PlayerId providedId)
		{
			return Task.FromResult<string>("-");
		}

		// Token: 0x0600006D RID: 109 RVA: 0x00003127 File Offset: 0x00001327
		Task<PlayerId> IFriendListService.GetUserWithName(string name)
		{
			return Task.FromResult<PlayerId>(PlayerId.Empty);
		}

		// Token: 0x0600006E RID: 110 RVA: 0x00003134 File Offset: 0x00001334
		private void Dummy()
		{
			if (this.OnUserStatusChanged != null)
			{
				this.OnUserStatusChanged(default(PlayerId));
			}
			if (this.OnFriendRemoved != null)
			{
				this.OnFriendRemoved(default(PlayerId));
			}
			if (this.OnFriendListChanged != null)
			{
				this.OnFriendListChanged();
			}
		}

		// Token: 0x0600006F RID: 111 RVA: 0x0000318C File Offset: 0x0000138C
		IEnumerable<PlayerId> IFriendListService.GetPendingRequests()
		{
			return null;
		}

		// Token: 0x06000070 RID: 112 RVA: 0x0000318F File Offset: 0x0000138F
		IEnumerable<PlayerId> IFriendListService.GetReceivedRequests()
		{
			return null;
		}
	}
}
