using System;
using TaleWorlds.MountAndBlade;

namespace SandBox.Missions.MissionLogics
{
	// Token: 0x02000054 RID: 84
	public class SandBoxSiegeMissionSpawnHandler : SandBoxMissionSpawnHandler
	{
		// Token: 0x060003BB RID: 955 RVA: 0x0001B558 File Offset: 0x00019758
		public override void AfterStart()
		{
			int numberOfInvolvedMen = this._mapEvent.GetNumberOfInvolvedMen(0);
			int numberOfInvolvedMen2 = this._mapEvent.GetNumberOfInvolvedMen(1);
			int num = numberOfInvolvedMen;
			int num2 = numberOfInvolvedMen2;
			this._missionAgentSpawnLogic.SetSpawnHorses(0, false);
			this._missionAgentSpawnLogic.SetSpawnHorses(1, false);
			MissionSpawnSettings missionSpawnSettings = SandBoxMissionSpawnHandler.CreateSandBoxBattleWaveSpawnSettings();
			missionSpawnSettings.DefenderAdvantageFactor = 1.5f;
			this._missionAgentSpawnLogic.InitWithSinglePhase(numberOfInvolvedMen, numberOfInvolvedMen2, num, num2, false, false, ref missionSpawnSettings);
		}
	}
}
