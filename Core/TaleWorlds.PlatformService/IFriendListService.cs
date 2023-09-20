using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaleWorlds.Localization;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.PlatformService
{
	// Token: 0x02000004 RID: 4
	public interface IFriendListService
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000002 RID: 2
		bool InGameStatusFetchable { get; }

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000003 RID: 3
		bool AllowsFriendOperations { get; }

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x06000004 RID: 4
		bool CanInvitePlayersToPlatformSession { get; }

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x06000005 RID: 5
		bool IncludeInAllFriends { get; }

		// Token: 0x06000006 RID: 6
		string GetServiceCodeName();

		// Token: 0x06000007 RID: 7
		TextObject GetServiceLocalizedName();

		// Token: 0x06000008 RID: 8
		FriendListServiceType GetFriendListServiceType();

		// Token: 0x06000009 RID: 9
		IEnumerable<PlayerId> GetAllFriends();

		// Token: 0x14000001 RID: 1
		// (add) Token: 0x0600000A RID: 10
		// (remove) Token: 0x0600000B RID: 11
		event Action<PlayerId> OnUserStatusChanged;

		// Token: 0x14000002 RID: 2
		// (add) Token: 0x0600000C RID: 12
		// (remove) Token: 0x0600000D RID: 13
		event Action<PlayerId> OnFriendRemoved;

		// Token: 0x0600000E RID: 14
		Task<bool> GetUserOnlineStatus(PlayerId providedId);

		// Token: 0x0600000F RID: 15
		Task<bool> IsPlayingThisGame(PlayerId providedId);

		// Token: 0x06000010 RID: 16
		Task<string> GetUserName(PlayerId providedId);

		// Token: 0x06000011 RID: 17
		Task<PlayerId> GetUserWithName(string name);

		// Token: 0x14000003 RID: 3
		// (add) Token: 0x06000012 RID: 18
		// (remove) Token: 0x06000013 RID: 19
		event Action OnFriendListChanged;

		// Token: 0x06000014 RID: 20
		IEnumerable<PlayerId> GetPendingRequests();

		// Token: 0x06000015 RID: 21
		IEnumerable<PlayerId> GetReceivedRequests();
	}
}
