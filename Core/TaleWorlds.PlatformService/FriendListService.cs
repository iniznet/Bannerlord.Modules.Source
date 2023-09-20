using System;
using System.Collections.Generic;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.PlatformService
{
	// Token: 0x02000002 RID: 2
	public static class FriendListService
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002048 File Offset: 0x00000248
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
