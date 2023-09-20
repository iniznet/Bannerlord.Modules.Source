using System;
using Galaxy.Api;

namespace TaleWorlds.PlatformService.GOG
{
	// Token: 0x0200000E RID: 14
	public class StatsAndAchievementsStoreListener : GlobalStatsAndAchievementsStoreListener
	{
		// Token: 0x14000009 RID: 9
		// (add) Token: 0x0600007F RID: 127 RVA: 0x00003204 File Offset: 0x00001404
		// (remove) Token: 0x06000080 RID: 128 RVA: 0x0000323C File Offset: 0x0000143C
		public event StatsAndAchievementsStoreListener.UserStatsAndAchievementsStored OnUserStatsAndAchievementsStored;

		// Token: 0x06000081 RID: 129 RVA: 0x00003271 File Offset: 0x00001471
		public override void OnUserStatsAndAchievementsStoreFailure(IStatsAndAchievementsStoreListener.FailureReason failureReason)
		{
			StatsAndAchievementsStoreListener.UserStatsAndAchievementsStored onUserStatsAndAchievementsStored = this.OnUserStatsAndAchievementsStored;
			if (onUserStatsAndAchievementsStored == null)
			{
				return;
			}
			onUserStatsAndAchievementsStored(false, new IStatsAndAchievementsStoreListener.FailureReason?(failureReason));
		}

		// Token: 0x06000082 RID: 130 RVA: 0x0000328C File Offset: 0x0000148C
		public override void OnUserStatsAndAchievementsStoreSuccess()
		{
			StatsAndAchievementsStoreListener.UserStatsAndAchievementsStored onUserStatsAndAchievementsStored = this.OnUserStatsAndAchievementsStored;
			if (onUserStatsAndAchievementsStored == null)
			{
				return;
			}
			onUserStatsAndAchievementsStored(true, null);
		}

		// Token: 0x02000019 RID: 25
		// (Invoke) Token: 0x060000A1 RID: 161
		public delegate void UserStatsAndAchievementsStored(bool success, IStatsAndAchievementsStoreListener.FailureReason? failureReason);
	}
}
