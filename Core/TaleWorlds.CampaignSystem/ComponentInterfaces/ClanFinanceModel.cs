using System;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements.Workshops;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.ComponentInterfaces
{
	public abstract class ClanFinanceModel : GameModel
	{
		public abstract ExplainedNumber CalculateClanGoldChange(Clan clan, bool includeDescriptions = false, bool applyWithdrawals = false, bool includeDetails = false);

		public abstract ExplainedNumber CalculateClanIncome(Clan clan, bool includeDescriptions = false, bool applyWithdrawals = false, bool includeDetails = false);

		public abstract ExplainedNumber CalculateClanExpenses(Clan clan, bool includeDescriptions = false, bool applyWithdrawals = false, bool includeDetails = false);

		public abstract int CalculateNotableDailyGoldChange(Hero hero, bool applyWithdrawals);

		public abstract int CalculateOwnerIncomeFromCaravan(MobileParty caravan);

		public abstract int CalculateOwnerIncomeFromWorkshop(Workshop workshop);

		public abstract int CalculateOwnerExpenseFromWorkshop(Workshop workshop);

		public abstract float RevenueSmoothenFraction();

		public abstract int PartyGoldLowerTreshold();
	}
}
