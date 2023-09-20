using System;
using System.Collections.Generic;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.PlatformService
{
	public static class FriendListService
	{
		public static IEnumerable<PlayerId> GetAllFriendsInAllPlatforms()
		{
			IFriendListService[] friendListServices = PlatformServices.Instance.GetFriendListServices();
			foreach (IFriendListService friendListService in friendListServices)
			{
				if (friendListService.IncludeInAllFriends)
				{
					IEnumerable<PlayerId> allFriends = friendListService.GetAllFriends();
					if (allFriends != null)
					{
						foreach (PlayerId playerId in allFriends)
						{
							yield return playerId;
						}
						IEnumerator<PlayerId> enumerator = null;
					}
				}
			}
			IFriendListService[] array = null;
			yield break;
			yield break;
		}
	}
}
