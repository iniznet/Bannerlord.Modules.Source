using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.ComponentInterfaces
{
	// Token: 0x0200018F RID: 399
	public abstract class ArmyManagementCalculationModel : GameModel
	{
		// Token: 0x170006AB RID: 1707
		// (get) Token: 0x060019DE RID: 6622
		public abstract int InfluenceValuePerGold { get; }

		// Token: 0x170006AC RID: 1708
		// (get) Token: 0x060019DF RID: 6623
		public abstract int AverageCallToArmyCost { get; }

		// Token: 0x170006AD RID: 1709
		// (get) Token: 0x060019E0 RID: 6624
		public abstract int CohesionThresholdForDispersion { get; }

		// Token: 0x060019E1 RID: 6625
		public abstract int CalculatePartyInfluenceCost(MobileParty armyLeaderParty, MobileParty party);

		// Token: 0x060019E2 RID: 6626
		public abstract float DailyBeingAtArmyInfluenceAward(MobileParty armyMemberParty);

		// Token: 0x060019E3 RID: 6627
		public abstract List<MobileParty> GetMobilePartiesToCallToArmy(MobileParty leaderParty);

		// Token: 0x060019E4 RID: 6628
		public abstract int CalculateTotalInfluenceCost(Army army, float percentage);

		// Token: 0x060019E5 RID: 6629
		public abstract float GetPartySizeScore(MobileParty party);

		// Token: 0x060019E6 RID: 6630
		public abstract bool CheckPartyEligibility(MobileParty party);

		// Token: 0x060019E7 RID: 6631
		public abstract int GetPartyRelation(Hero hero);

		// Token: 0x060019E8 RID: 6632
		public abstract ExplainedNumber CalculateDailyCohesionChange(Army army, bool includeDescriptions = false);

		// Token: 0x060019E9 RID: 6633
		public abstract int CalculateNewCohesion(Army army, PartyBase newParty, int calculatedCohesion, int sign);

		// Token: 0x060019EA RID: 6634
		public abstract int GetCohesionBoostInfluenceCost(Army army, int percentageToBoost = 100);

		// Token: 0x060019EB RID: 6635
		public abstract int GetCohesionBoostGoldCost(Army army, float percentageToBoost = 100f);

		// Token: 0x060019EC RID: 6636
		public abstract int GetPartyStrength(PartyBase party);
	}
}
