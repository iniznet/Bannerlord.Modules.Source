using System;
using TaleWorlds.MountAndBlade;

namespace SandBox.Missions.MissionLogics
{
	public class SandBoxBattleMissionSpawnHandler : SandBoxMissionSpawnHandler
	{
		public override void AfterStart()
		{
			int numberOfInvolvedMen = this._mapEvent.GetNumberOfInvolvedMen(0);
			int numberOfInvolvedMen2 = this._mapEvent.GetNumberOfInvolvedMen(1);
			int num = numberOfInvolvedMen;
			int num2 = numberOfInvolvedMen2;
			this._missionAgentSpawnLogic.SetSpawnHorses(0, !this._mapEvent.IsSiegeAssault);
			this._missionAgentSpawnLogic.SetSpawnHorses(1, !this._mapEvent.IsSiegeAssault);
			MissionSpawnSettings missionSpawnSettings = SandBoxMissionSpawnHandler.CreateSandBoxBattleWaveSpawnSettings();
			this._missionAgentSpawnLogic.InitWithSinglePhase(numberOfInvolvedMen, numberOfInvolvedMen2, num, num2, true, true, ref missionSpawnSettings);
		}
	}
}
