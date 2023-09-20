using System;
using TaleWorlds.CampaignSystem.Settlements;

namespace TaleWorlds.CampaignSystem.Actions
{
	public static class ClaimSettlementAction
	{
		private static void ApplyInternal(Hero claimant, Settlement claimedSettlement)
		{
			ClaimSettlementAction.ImpactRelations(claimant, claimedSettlement);
		}

		private static void ImpactRelations(Hero claimant, Settlement claimedSettlement)
		{
			if (claimedSettlement.OwnerClan.Leader != null)
			{
				ChangeRelationAction.ApplyRelationChangeBetweenHeroes(claimant, claimedSettlement.OwnerClan.Leader, -50, false);
				if (!claimedSettlement.OwnerClan.IsMapFaction)
				{
					ChangeRelationAction.ApplyRelationChangeBetweenHeroes(claimant, claimedSettlement.OwnerClan.Leader, -20, false);
				}
			}
		}

		public static void Apply(Hero claimant, Settlement claimedSettlement)
		{
			ClaimSettlementAction.ApplyInternal(claimant, claimedSettlement);
		}
	}
}
