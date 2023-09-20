using System;

namespace TaleWorlds.MountAndBlade.CustomBattle
{
	public class CPUBenchmarkMissionSpawnHandler : MissionLogic
	{
		public CPUBenchmarkMissionSpawnHandler()
		{
		}

		public CPUBenchmarkMissionSpawnHandler(CustomBattleCombatant defenderParty, CustomBattleCombatant attackerParty)
		{
			this._defenderParty = defenderParty;
			this._attackerParty = attackerParty;
		}

		public override void OnBehaviorInitialize()
		{
			base.OnBehaviorInitialize();
			this._missionAgentSpawnLogic = base.Mission.GetMissionBehavior<MissionAgentSpawnLogic>();
		}

		public override void AfterStart()
		{
			int numberOfHealthyMembers = this._defenderParty.NumberOfHealthyMembers;
			int numberOfHealthyMembers2 = this._attackerParty.NumberOfHealthyMembers;
			base.Mission.PlayerTeam.GetFormation(2).ArrangementOrder = ArrangementOrder.ArrangementOrderLine;
			base.Mission.PlayerTeam.GetFormation(0).ArrangementOrder = ArrangementOrder.ArrangementOrderLine;
			base.Mission.PlayerEnemyTeam.GetFormation(2).ArrangementOrder = ArrangementOrder.ArrangementOrderLine;
			base.Mission.PlayerEnemyTeam.GetFormation(0).ArrangementOrder = ArrangementOrder.ArrangementOrderLine;
			this._missionAgentSpawnLogic.SetSpawnHorses(0, true);
			this._missionAgentSpawnLogic.SetSpawnHorses(1, true);
			MissionSpawnSettings missionSpawnSettings = MissionSpawnSettings.CreateDefaultSpawnSettings();
			this._missionAgentSpawnLogic.InitWithSinglePhase(numberOfHealthyMembers, numberOfHealthyMembers2, numberOfHealthyMembers, numberOfHealthyMembers2, true, true, ref missionSpawnSettings);
		}

		private MissionAgentSpawnLogic _missionAgentSpawnLogic;

		private CustomBattleCombatant _defenderParty;

		private CustomBattleCombatant _attackerParty;
	}
}
