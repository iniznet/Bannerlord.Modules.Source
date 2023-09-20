using System;
using System.Collections.Generic;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.PlatformService;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade
{
	public class RecentPlayersFriendListService : BannerlordFriendListService, IFriendListService
	{
		bool IFriendListService.IncludeInAllFriends
		{
			get
			{
				return false;
			}
		}

		bool IFriendListService.CanInvitePlayersToPlatformSession
		{
			get
			{
				return PlatformServices.InvitationServices != null;
			}
		}

		TextObject IFriendListService.GetServiceLocalizedName()
		{
			return new TextObject("{=XvSRoOzM}Recently Played Players", null);
		}

		string IFriendListService.GetServiceCodeName()
		{
			return "RecentlyPlayedPlayers";
		}

		IEnumerable<PlayerId> IFriendListService.GetAllFriends()
		{
			return RecentPlayersManager.GetPlayersOrdered();
		}

		FriendListServiceType IFriendListService.GetFriendListServiceType()
		{
			return FriendListServiceType.RecentPlayers;
		}
	}
}
