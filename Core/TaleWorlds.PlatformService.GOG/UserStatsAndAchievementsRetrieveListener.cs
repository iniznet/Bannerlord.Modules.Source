using System;
using Galaxy.Api;

namespace TaleWorlds.PlatformService.GOG
{
	public class UserStatsAndAchievementsRetrieveListener : GlobalUserStatsAndAchievementsRetrieveListener
	{
		public event UserStatsAndAchievementsRetrieveListener.UserStatsAndAchievementsRetrieved OnUserStatsAndAchievementsRetrieved;

		public override void OnUserStatsAndAchievementsRetrieveSuccess(GalaxyID userID)
		{
			UserStatsAndAchievementsRetrieveListener.UserStatsAndAchievementsRetrieved onUserStatsAndAchievementsRetrieved = this.OnUserStatsAndAchievementsRetrieved;
			if (onUserStatsAndAchievementsRetrieved == null)
			{
				return;
			}
			onUserStatsAndAchievementsRetrieved(userID, true, null);
		}

		public override void OnUserStatsAndAchievementsRetrieveFailure(GalaxyID userID, IUserStatsAndAchievementsRetrieveListener.FailureReason failureReason)
		{
			UserStatsAndAchievementsRetrieveListener.UserStatsAndAchievementsRetrieved onUserStatsAndAchievementsRetrieved = this.OnUserStatsAndAchievementsRetrieved;
			if (onUserStatsAndAchievementsRetrieved == null)
			{
				return;
			}
			onUserStatsAndAchievementsRetrieved(userID, false, new IUserStatsAndAchievementsRetrieveListener.FailureReason?(failureReason));
		}

		public delegate void UserStatsAndAchievementsRetrieved(GalaxyID userID, bool success, IUserStatsAndAchievementsRetrieveListener.FailureReason? failureReason);
	}
}
