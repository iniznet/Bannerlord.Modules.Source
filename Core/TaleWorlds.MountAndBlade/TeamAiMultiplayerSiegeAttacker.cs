using System;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000173 RID: 371
	public class TeamAiMultiplayerSiegeAttacker : TeamAISiegeComponent
	{
		// Token: 0x06001340 RID: 4928 RVA: 0x0004AE4A File Offset: 0x0004904A
		public TeamAiMultiplayerSiegeAttacker(Mission currentMission, Team currentTeam, float thinkTimerTime, float applyTimerTime)
			: base(currentMission, currentTeam, thinkTimerTime, applyTimerTime)
		{
		}

		// Token: 0x06001341 RID: 4929 RVA: 0x0004AE57 File Offset: 0x00049057
		public override void OnUnitAddedToFormationForTheFirstTime(Formation formation)
		{
		}
	}
}
