using System;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.ComponentInterfaces
{
	// Token: 0x0200018A RID: 394
	public abstract class TargetScoreCalculatingModel : GameModel
	{
		// Token: 0x1700069B RID: 1691
		// (get) Token: 0x060019BF RID: 6591
		public abstract float TravelingToAssignmentFactor { get; }

		// Token: 0x1700069C RID: 1692
		// (get) Token: 0x060019C0 RID: 6592
		public abstract float BesiegingFactor { get; }

		// Token: 0x1700069D RID: 1693
		// (get) Token: 0x060019C1 RID: 6593
		public abstract float AssaultingTownFactor { get; }

		// Token: 0x1700069E RID: 1694
		// (get) Token: 0x060019C2 RID: 6594
		public abstract float RaidingFactor { get; }

		// Token: 0x1700069F RID: 1695
		// (get) Token: 0x060019C3 RID: 6595
		public abstract float DefendingFactor { get; }

		// Token: 0x060019C4 RID: 6596
		public abstract float GetTargetScoreForFaction(Settlement targetSettlement, Army.ArmyTypes missionType, MobileParty mobileParty, float ourStrength, int numberOfEnemyFactionSettlements = -1, float totalEnemyMobilePartyStrength = -1f);

		// Token: 0x060019C5 RID: 6597
		public abstract float CalculatePatrollingScoreForSettlement(Settlement targetSettlement, MobileParty mobileParty);

		// Token: 0x060019C6 RID: 6598
		public abstract float CurrentObjectiveValue(MobileParty mobileParty);
	}
}
