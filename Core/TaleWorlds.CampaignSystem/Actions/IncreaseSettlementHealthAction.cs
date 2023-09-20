using System;
using TaleWorlds.CampaignSystem.Settlements;

namespace TaleWorlds.CampaignSystem.Actions
{
	// Token: 0x02000446 RID: 1094
	public static class IncreaseSettlementHealthAction
	{
		// Token: 0x06003F1B RID: 16155 RVA: 0x0012D440 File Offset: 0x0012B640
		private static void ApplyInternal(Settlement settlement, float percentage)
		{
			settlement.SettlementHitPoints += percentage;
			settlement.SettlementHitPoints = ((settlement.SettlementHitPoints > 1f) ? 1f : settlement.SettlementHitPoints);
			if (settlement.SettlementHitPoints >= 1f && settlement.IsVillage && settlement.Village.VillageState != Village.VillageStates.Normal)
			{
				ChangeVillageStateAction.ApplyBySettingToNormal(settlement);
				settlement.Militia += 20f;
			}
		}

		// Token: 0x06003F1C RID: 16156 RVA: 0x0012D4B5 File Offset: 0x0012B6B5
		public static void Apply(Settlement settlement, float percentage)
		{
			IncreaseSettlementHealthAction.ApplyInternal(settlement, percentage);
		}
	}
}
