using System;
using SandBox.Tournaments.MissionLogics;
using TaleWorlds.MountAndBlade;

namespace SandBox.Missions.MissionLogics.Arena
{
	// Token: 0x02000063 RID: 99
	public class ArenaDuelMissionBehavior : MissionLogic
	{
		// Token: 0x06000437 RID: 1079 RVA: 0x0001F1BE File Offset: 0x0001D3BE
		public override void AfterStart()
		{
			TournamentBehavior.DeleteTournamentSetsExcept(base.Mission.Scene.FindEntityWithTag("tournament_fight"));
		}
	}
}
