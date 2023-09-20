using System;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements.Workshops;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.ComponentInterfaces
{
	// Token: 0x020001AA RID: 426
	public abstract class ClanFinanceModel : GameModel
	{
		// Token: 0x06001AAE RID: 6830
		public abstract ExplainedNumber CalculateClanGoldChange(Clan clan, bool includeDescriptions = false, bool applyWithdrawals = false, bool includeDetails = false);

		// Token: 0x06001AAF RID: 6831
		public abstract ExplainedNumber CalculateClanIncome(Clan clan, bool includeDescriptions = false, bool applyWithdrawals = false, bool includeDetails = false);

		// Token: 0x06001AB0 RID: 6832
		public abstract ExplainedNumber CalculateClanExpenses(Clan clan, bool includeDescriptions = false, bool applyWithdrawals = false, bool includeDetails = false);

		// Token: 0x06001AB1 RID: 6833
		public abstract int CalculateNotableDailyGoldChange(Hero hero, bool applyWithdrawals);

		// Token: 0x06001AB2 RID: 6834
		public abstract int CalculateOwnerIncomeFromCaravan(MobileParty caravan);

		// Token: 0x06001AB3 RID: 6835
		public abstract int CalculateOwnerIncomeFromWorkshop(Workshop workshop);

		// Token: 0x06001AB4 RID: 6836
		public abstract int CalculateOwnerExpenseFromWorkshop(Workshop workshop);

		// Token: 0x06001AB5 RID: 6837
		public abstract float RevenueSmoothenFraction();

		// Token: 0x06001AB6 RID: 6838
		public abstract int PartyGoldLowerTreshold();
	}
}
