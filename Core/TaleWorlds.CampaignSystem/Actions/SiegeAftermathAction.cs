using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;

namespace TaleWorlds.CampaignSystem.Actions
{
	// Token: 0x02000456 RID: 1110
	public static class SiegeAftermathAction
	{
		// Token: 0x06003F57 RID: 16215 RVA: 0x0012F828 File Offset: 0x0012DA28
		private static void ApplyInternal(MobileParty attackerParty, Settlement settlement, SiegeAftermathAction.SiegeAftermath aftermathType, Clan previousSettlementOwner, Dictionary<MobileParty, float> partyContributions)
		{
			CampaignEventDispatcher.Instance.OnSiegeAftermathApplied(attackerParty, settlement, aftermathType, previousSettlementOwner, partyContributions);
		}

		// Token: 0x06003F58 RID: 16216 RVA: 0x0012F83A File Offset: 0x0012DA3A
		public static void ApplyAftermath(MobileParty attackerParty, Settlement settlement, SiegeAftermathAction.SiegeAftermath aftermathType, Clan previousSettlementOwner, Dictionary<MobileParty, float> partyContributions)
		{
			SiegeAftermathAction.ApplyInternal(attackerParty, settlement, aftermathType, previousSettlementOwner, partyContributions);
		}

		// Token: 0x0200076B RID: 1899
		public enum SiegeAftermath
		{
			// Token: 0x04001E81 RID: 7809
			Devastate,
			// Token: 0x04001E82 RID: 7810
			Pillage,
			// Token: 0x04001E83 RID: 7811
			ShowMercy
		}
	}
}
