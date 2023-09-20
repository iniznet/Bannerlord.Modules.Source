using System;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade.MissionSpawnHandlers
{
	// Token: 0x020003E7 RID: 999
	public class CustomBattleMissionSpawnHandler : CustomMissionSpawnHandler
	{
		// Token: 0x0600348F RID: 13455 RVA: 0x000DA1AE File Offset: 0x000D83AE
		public CustomBattleMissionSpawnHandler(CustomBattleCombatant defenderParty, CustomBattleCombatant attackerParty)
		{
			this._defenderParty = defenderParty;
			this._attackerParty = attackerParty;
		}

		// Token: 0x06003490 RID: 13456 RVA: 0x000DA1C4 File Offset: 0x000D83C4
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

		// Token: 0x04001670 RID: 5744
		private CustomBattleCombatant _defenderParty;

		// Token: 0x04001671 RID: 5745
		private CustomBattleCombatant _attackerParty;
	}
}
