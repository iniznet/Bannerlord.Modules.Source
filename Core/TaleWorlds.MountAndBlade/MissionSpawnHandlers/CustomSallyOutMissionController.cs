using System;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade.MissionSpawnHandlers
{
	// Token: 0x020003E9 RID: 1001
	public class CustomSallyOutMissionController : SallyOutMissionController
	{
		// Token: 0x06003494 RID: 13460 RVA: 0x000DA282 File Offset: 0x000D8482
		public CustomSallyOutMissionController(IBattleCombatant defenderBattleCombatant, IBattleCombatant attackerBattleCombatant)
		{
			this._battleCombatants = new CustomBattleCombatant[]
			{
				(CustomBattleCombatant)defenderBattleCombatant,
				(CustomBattleCombatant)attackerBattleCombatant
			};
		}

		// Token: 0x06003495 RID: 13461 RVA: 0x000DA2A8 File Offset: 0x000D84A8
		protected override void GetInitialTroopCounts(out int besiegedTotalTroopCount, out int besiegerTotalTroopCount)
		{
			besiegedTotalTroopCount = this._battleCombatants[0].NumberOfHealthyMembers;
			besiegerTotalTroopCount = this._battleCombatants[1].NumberOfHealthyMembers;
		}

		// Token: 0x04001673 RID: 5747
		private readonly CustomBattleCombatant[] _battleCombatants;
	}
}
