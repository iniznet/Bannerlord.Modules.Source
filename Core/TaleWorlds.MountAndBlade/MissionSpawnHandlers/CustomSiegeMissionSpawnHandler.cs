using System;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade.MissionSpawnHandlers
{
	// Token: 0x020003EA RID: 1002
	public class CustomSiegeMissionSpawnHandler : CustomMissionSpawnHandler
	{
		// Token: 0x06003496 RID: 13462 RVA: 0x000DA2C8 File Offset: 0x000D84C8
		public CustomSiegeMissionSpawnHandler(IBattleCombatant defenderBattleCombatant, IBattleCombatant attackerBattleCombatant, bool spawnWithHorses)
		{
			this._battleCombatants = new CustomBattleCombatant[]
			{
				(CustomBattleCombatant)defenderBattleCombatant,
				(CustomBattleCombatant)attackerBattleCombatant
			};
			this._spawnWithHorses = spawnWithHorses;
		}

		// Token: 0x06003497 RID: 13463 RVA: 0x000DA2F8 File Offset: 0x000D84F8
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

		// Token: 0x04001674 RID: 5748
		private CustomBattleCombatant[] _battleCombatants;

		// Token: 0x04001675 RID: 5749
		private bool _spawnWithHorses;
	}
}
