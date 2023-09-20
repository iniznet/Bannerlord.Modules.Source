using System;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.ComponentInterfaces
{
	// Token: 0x02000187 RID: 391
	public abstract class PartyFoodBuyingModel : GameModel
	{
		// Token: 0x060019B1 RID: 6577
		public abstract void FindItemToBuy(MobileParty mobileParty, Settlement settlement, out ItemRosterElement itemRosterElement, out float itemElementsPrice);

		// Token: 0x17000698 RID: 1688
		// (get) Token: 0x060019B2 RID: 6578
		public abstract float MinimumDaysFoodToLastWhileBuyingFoodFromTown { get; }

		// Token: 0x17000699 RID: 1689
		// (get) Token: 0x060019B3 RID: 6579
		public abstract float MinimumDaysFoodToLastWhileBuyingFoodFromVillage { get; }

		// Token: 0x1700069A RID: 1690
		// (get) Token: 0x060019B4 RID: 6580
		public abstract float LowCostFoodPriceAverage { get; }
	}
}
