using System;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;

namespace TaleWorlds.CampaignSystem.Actions
{
	// Token: 0x02000443 RID: 1091
	public static class GatherArmyAction
	{
		// Token: 0x06003F0A RID: 16138 RVA: 0x0012D06C File Offset: 0x0012B26C
		private static void ApplyInternal(MobileParty leaderParty, Settlement targetSettlement, float playerInvolvement = 0f)
		{
			Army army = leaderParty.Army;
			army.AIBehavior = Army.AIBehaviorFlags.Gathering;
			CampaignEventDispatcher.Instance.OnArmyGathered(army, targetSettlement);
		}

		// Token: 0x06003F0B RID: 16139 RVA: 0x0012D093 File Offset: 0x0012B293
		public static void Apply(MobileParty leaderParty, Settlement targetSettlement)
		{
			GatherArmyAction.ApplyInternal(leaderParty, targetSettlement, (leaderParty == MobileParty.MainParty) ? 1f : 0f);
		}
	}
}
