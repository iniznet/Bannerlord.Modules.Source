using System;
using Galaxy.Api;

namespace TaleWorlds.PlatformService.GOG
{
	public class StatsAndAchievementsStoreListener : GlobalStatsAndAchievementsStoreListener
	{
		public event StatsAndAchievementsStoreListener.UserStatsAndAchievementsStored OnUserStatsAndAchievementsStored;

		public override void OnUserStatsAndAchievementsStoreFailure(IStatsAndAchievementsStoreListener.FailureReason failureReason)
		{
			StatsAndAchievementsStoreListener.UserStatsAndAchievementsStored onUserStatsAndAchievementsStored = this.OnUserStatsAndAchievementsStored;
			if (onUserStatsAndAchievementsStored == null)
			{
				return;
			}
			onUserStatsAndAchievementsStored(false, new IStatsAndAchievementsStoreListener.FailureReason?(failureReason));
		}

		public override void OnUserStatsAndAchievementsStoreSuccess()
		{
			StatsAndAchievementsStoreListener.UserStatsAndAchievementsStored onUserStatsAndAchievementsStored = this.OnUserStatsAndAchievementsStored;
			if (onUserStatsAndAchievementsStored == null)
			{
				return;
			}
			onUserStatsAndAchievementsStored(true, null);
		}

		public delegate void UserStatsAndAchievementsStored(bool success, IStatsAndAchievementsStoreListener.FailureReason? failureReason);
	}
}
