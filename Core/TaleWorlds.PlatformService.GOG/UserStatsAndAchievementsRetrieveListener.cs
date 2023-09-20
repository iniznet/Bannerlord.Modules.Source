using System;
using Galaxy.Api;

namespace TaleWorlds.PlatformService.GOG
{
	// Token: 0x0200000D RID: 13
	public class UserStatsAndAchievementsRetrieveListener : GlobalUserStatsAndAchievementsRetrieveListener
	{
		// Token: 0x14000008 RID: 8
		// (add) Token: 0x0600007A RID: 122 RVA: 0x00003148 File Offset: 0x00001348
		// (remove) Token: 0x0600007B RID: 123 RVA: 0x00003180 File Offset: 0x00001380
		public event UserStatsAndAchievementsRetrieveListener.UserStatsAndAchievementsRetrieved OnUserStatsAndAchievementsRetrieved;

		// Token: 0x0600007C RID: 124 RVA: 0x000031B8 File Offset: 0x000013B8
		public override void OnUserStatsAndAchievementsRetrieveSuccess(GalaxyID userID)
		{
			UserStatsAndAchievementsRetrieveListener.UserStatsAndAchievementsRetrieved onUserStatsAndAchievementsRetrieved = this.OnUserStatsAndAchievementsRetrieved;
			if (onUserStatsAndAchievementsRetrieved == null)
			{
				return;
			}
			onUserStatsAndAchievementsRetrieved(userID, true, null);
		}

		// Token: 0x0600007D RID: 125 RVA: 0x000031E0 File Offset: 0x000013E0
		public override void OnUserStatsAndAchievementsRetrieveFailure(GalaxyID userID, IUserStatsAndAchievementsRetrieveListener.FailureReason failureReason)
		{
			UserStatsAndAchievementsRetrieveListener.UserStatsAndAchievementsRetrieved onUserStatsAndAchievementsRetrieved = this.OnUserStatsAndAchievementsRetrieved;
			if (onUserStatsAndAchievementsRetrieved == null)
			{
				return;
			}
			onUserStatsAndAchievementsRetrieved(userID, false, new IUserStatsAndAchievementsRetrieveListener.FailureReason?(failureReason));
		}

		// Token: 0x02000018 RID: 24
		// (Invoke) Token: 0x0600009D RID: 157
		public delegate void UserStatsAndAchievementsRetrieved(GalaxyID userID, bool success, IUserStatsAndAchievementsRetrieveListener.FailureReason? failureReason);
	}
}
