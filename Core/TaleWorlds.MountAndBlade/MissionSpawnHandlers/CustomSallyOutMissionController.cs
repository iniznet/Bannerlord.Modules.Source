using System;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade.MissionSpawnHandlers
{
	public class CustomSallyOutMissionController : SallyOutMissionController
	{
		public CustomSallyOutMissionController(IBattleCombatant defenderBattleCombatant, IBattleCombatant attackerBattleCombatant)
		{
			this._battleCombatants = new CustomBattleCombatant[]
			{
				(CustomBattleCombatant)defenderBattleCombatant,
				(CustomBattleCombatant)attackerBattleCombatant
			};
		}

		protected override void GetInitialTroopCounts(out int besiegedTotalTroopCount, out int besiegerTotalTroopCount)
		{
			besiegedTotalTroopCount = this._battleCombatants[0].NumberOfHealthyMembers;
			besiegerTotalTroopCount = this._battleCombatants[1].NumberOfHealthyMembers;
		}

		private readonly CustomBattleCombatant[] _battleCombatants;
	}
}
