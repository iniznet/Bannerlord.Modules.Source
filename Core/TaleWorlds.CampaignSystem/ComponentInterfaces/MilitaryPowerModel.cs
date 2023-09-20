using System;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.ComponentInterfaces
{
	// Token: 0x020001B8 RID: 440
	public abstract class MilitaryPowerModel : GameModel
	{
		// Token: 0x06001AF9 RID: 6905
		public abstract float GetTroopPowerBasedOnContext(CharacterObject troop, MapEvent.BattleTypes battleType = MapEvent.BattleTypes.None, BattleSideEnum battleSideEnum = BattleSideEnum.None, bool isSimulation = false);

		// Token: 0x06001AFA RID: 6906
		public abstract float GetTroopPowerToCalculateSecurity(CharacterObject troop);

		// Token: 0x0200055C RID: 1372
		public enum PowerCalculationContext
		{
			// Token: 0x0400169D RID: 5789
			FieldBattle,
			// Token: 0x0400169E RID: 5790
			FieldBattleSimulation,
			// Token: 0x0400169F RID: 5791
			RaidAsAttacker,
			// Token: 0x040016A0 RID: 5792
			RaidAsDefender,
			// Token: 0x040016A1 RID: 5793
			RaidSimulationAsAttacker,
			// Token: 0x040016A2 RID: 5794
			RaidSimulationAsDefender,
			// Token: 0x040016A3 RID: 5795
			SiegeSimulationAsAttacker,
			// Token: 0x040016A4 RID: 5796
			SiegeSimulationAsDefender,
			// Token: 0x040016A5 RID: 5797
			SiegeAsAttacker,
			// Token: 0x040016A6 RID: 5798
			SiegeAsDefender,
			// Token: 0x040016A7 RID: 5799
			ToCalculateSettlementSecurity,
			// Token: 0x040016A8 RID: 5800
			Hideout,
			// Token: 0x040016A9 RID: 5801
			Default
		}
	}
}
