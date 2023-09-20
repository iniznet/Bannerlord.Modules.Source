using System;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.ComponentInterfaces
{
	// Token: 0x02000180 RID: 384
	public abstract class MobilePartyFoodConsumptionModel : GameModel
	{
		// Token: 0x17000688 RID: 1672
		// (get) Token: 0x06001957 RID: 6487
		public abstract int NumberOfMenOnMapToEatOneFood { get; }

		// Token: 0x06001958 RID: 6488
		public abstract ExplainedNumber CalculateDailyBaseFoodConsumptionf(MobileParty party, bool includeDescription = false);

		// Token: 0x06001959 RID: 6489
		public abstract ExplainedNumber CalculateDailyFoodConsumptionf(MobileParty party, ExplainedNumber baseConsumption);

		// Token: 0x0600195A RID: 6490
		public abstract bool DoesPartyConsumeFood(MobileParty mobileParty);
	}
}
