using System;
using System.Collections.Generic;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.PlatformService;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200038C RID: 908
	public class RecentPlayersFriendListService : BannerlordFriendListService, IFriendListService
	{
		// Token: 0x170008DF RID: 2271
		// (get) Token: 0x060031C2 RID: 12738 RVA: 0x000CF0B5 File Offset: 0x000CD2B5
		bool IFriendListService.IncludeInAllFriends
		{
			get
			{
				return false;
			}
		}

		// Token: 0x170008E0 RID: 2272
		// (get) Token: 0x060031C3 RID: 12739 RVA: 0x000CF0B8 File Offset: 0x000CD2B8
		bool IFriendListService.CanInvitePlayersToPlatformSession
		{
			get
			{
				return PlatformServices.InvitationServices != null;
			}
		}

		// Token: 0x060031C4 RID: 12740 RVA: 0x000CF0C2 File Offset: 0x000CD2C2
		TextObject IFriendListService.GetServiceLocalizedName()
		{
			return new TextObject("{=XvSRoOzM}Recently Played Players", null);
		}

		// Token: 0x060031C5 RID: 12741 RVA: 0x000CF0CF File Offset: 0x000CD2CF
		string IFriendListService.GetServiceCodeName()
		{
			return "RecentlyPlayedPlayers";
		}

		// Token: 0x060031C6 RID: 12742 RVA: 0x000CF0D6 File Offset: 0x000CD2D6
		IEnumerable<PlayerId> IFriendListService.GetAllFriends()
		{
			return RecentPlayersManager.GetPlayersOrdered();
		}

		// Token: 0x060031C7 RID: 12743 RVA: 0x000CF0DD File Offset: 0x000CD2DD
		FriendListServiceType IFriendListService.GetFriendListServiceType()
		{
			return FriendListServiceType.RecentPlayers;
		}
	}
}
