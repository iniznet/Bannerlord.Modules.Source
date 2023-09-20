using System;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade.MissionSpawnHandlers
{
	public class CustomBattleMissionSpawnHandler : CustomMissionSpawnHandler
	{
		public CustomBattleMissionSpawnHandler(CustomBattleCombatant defenderParty, CustomBattleCombatant attackerParty)
		{
			this._defenderParty = defenderParty;
			this._attackerParty = attackerParty;
		}

		public override void AfterStart()
		{
			int numberOfHealthyMembers = this._defenderParty.NumberOfHealthyMembers;
			int numberOfHealthyMembers2 = this._attackerParty.NumberOfHealthyMembers;
			int num = numberOfHealthyMembers;
			int num2 = numberOfHealthyMembers2;
			this._missionAgentSpawnLogic.SetSpawnHorses(BattleSideEnum.Defender, true);
			this._missionAgentSpawnLogic.SetSpawnHorses(BattleSideEnum.Attacker, true);
			MissionSpawnSettings missionSpawnSettings = CustomMissionSpawnHandler.CreateCustomBattleWaveSpawnSettings();
			this._missionAgentSpawnLogic.InitWithSinglePhase(numberOfHealthyMembers, numberOfHealthyMembers2, num, num2, true, true, missionSpawnSettings);
		}

		private CustomBattleCombatant _defenderParty;

		private CustomBattleCombatant _attackerParty;
	}
}
