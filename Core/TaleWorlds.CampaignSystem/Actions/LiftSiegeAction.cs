using System;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;

namespace TaleWorlds.CampaignSystem.Actions
{
	// Token: 0x0200044A RID: 1098
	public static class LiftSiegeAction
	{
		// Token: 0x06003F2F RID: 16175 RVA: 0x0012E573 File Offset: 0x0012C773
		private static void ApplyInternal(MobileParty side1Party, Settlement settlement)
		{
			settlement.SiegeEvent.BesiegerCamp.RemoveAllSiegeParties();
		}

		// Token: 0x06003F30 RID: 16176 RVA: 0x0012E585 File Offset: 0x0012C785
		public static void GetGameAction(MobileParty side1Party)
		{
			LiftSiegeAction.ApplyInternal(side1Party, side1Party.BesiegedSettlement);
		}
	}
}
