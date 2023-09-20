using System;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000174 RID: 372
	public class TeamAiMultiplayerSiegeDefender : TeamAISiegeComponent
	{
		// Token: 0x06001342 RID: 4930 RVA: 0x0004AE59 File Offset: 0x00049059
		public TeamAiMultiplayerSiegeDefender(Mission currentMission, Team currentTeam, float thinkTimerTime, float applyTimerTime)
			: base(currentMission, currentTeam, thinkTimerTime, applyTimerTime)
		{
		}

		// Token: 0x06001343 RID: 4931 RVA: 0x0004AE66 File Offset: 0x00049066
		public override void OnUnitAddedToFormationForTheFirstTime(Formation formation)
		{
		}
	}
}
