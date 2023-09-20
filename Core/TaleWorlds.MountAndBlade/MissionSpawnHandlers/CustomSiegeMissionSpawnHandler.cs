using System;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade.MissionSpawnHandlers
{
	public class CustomSiegeMissionSpawnHandler : CustomMissionSpawnHandler
	{
		public CustomSiegeMissionSpawnHandler(IBattleCombatant defenderBattleCombatant, IBattleCombatant attackerBattleCombatant, bool spawnWithHorses)
		{
			this._battleCombatants = new CustomBattleCombatant[]
			{
				(CustomBattleCombatant)defenderBattleCombatant,
				(CustomBattleCombatant)attackerBattleCombatant
			};
			this._spawnWithHorses = spawnWithHorses;
		}

		public override void AfterStart()
		{
			int numberOfHealthyMembers = this._battleCombatants[0].NumberOfHealthyMembers;
			int numberOfHealthyMembers2 = this._battleCombatants[1].NumberOfHealthyMembers;
			int num = numberOfHealthyMembers;
			int num2 = numberOfHealthyMembers2;
			this._missionAgentSpawnLogic.SetSpawnHorses(BattleSideEnum.Defender, this._spawnWithHorses);
			this._missionAgentSpawnLogic.SetSpawnHorses(BattleSideEnum.Attacker, this._spawnWithHorses);
			MissionSpawnSettings missionSpawnSettings = CustomMissionSpawnHandler.CreateCustomBattleWaveSpawnSettings();
			this._missionAgentSpawnLogic.InitWithSinglePhase(numberOfHealthyMembers, numberOfHealthyMembers2, num, num2, false, false, missionSpawnSettings);
		}

		private CustomBattleCombatant[] _battleCombatants;

		private bool _spawnWithHorses;
	}
}
