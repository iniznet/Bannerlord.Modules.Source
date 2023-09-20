using System;
using System.Runtime.CompilerServices;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.ComponentInterfaces
{
	// Token: 0x0200017C RID: 380
	public abstract class CombatSimulationModel : GameModel
	{
		// Token: 0x06001936 RID: 6454
		public abstract int SimulateHit(CharacterObject strikerTroop, CharacterObject struckTroop, PartyBase strikerParty, PartyBase struckParty, float strikerAdvantage, MapEvent battle);

		// Token: 0x06001937 RID: 6455
		[return: TupleElementNames(new string[] { "defenderRounds", "attackerRounds" })]
		public abstract ValueTuple<int, int> GetSimulationRoundsForBattle(MapEvent mapEvent, int numDefenders, int numAttackers);

		// Token: 0x06001938 RID: 6456
		public abstract int GetNumberOfEquipmentsBuilt(Settlement settlement);

		// Token: 0x06001939 RID: 6457
		public abstract float GetMaximumSiegeEquipmentProgress(Settlement settlement);

		// Token: 0x0600193A RID: 6458
		public abstract float GetSettlementAdvantage(Settlement settlement);

		// Token: 0x0600193B RID: 6459
		[return: TupleElementNames(new string[] { "defenderAdvantage", "attackerAdvantage" })]
		public abstract ValueTuple<float, float> GetBattleAdvantage(PartyBase defenderParty, PartyBase attackerParty, MapEvent.BattleTypes mapEventType, Settlement settlement);
	}
}
