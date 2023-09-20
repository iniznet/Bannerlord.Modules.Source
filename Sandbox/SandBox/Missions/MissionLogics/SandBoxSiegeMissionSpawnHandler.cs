using System;
using TaleWorlds.MountAndBlade;

namespace SandBox.Missions.MissionLogics
{
	public class SandBoxSiegeMissionSpawnHandler : SandBoxMissionSpawnHandler
	{
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
