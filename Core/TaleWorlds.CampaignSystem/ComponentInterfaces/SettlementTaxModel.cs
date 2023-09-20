using System;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.ComponentInterfaces
{
	// Token: 0x020001A5 RID: 421
	public abstract class SettlementTaxModel : GameModel
	{
		// Token: 0x170006E1 RID: 1761
		// (get) Token: 0x06001A88 RID: 6792
		public abstract float SettlementCommissionRateTown { get; }

		// Token: 0x170006E2 RID: 1762
		// (get) Token: 0x06001A89 RID: 6793
		public abstract float SettlementCommissionRateVillage { get; }

		// Token: 0x170006E3 RID: 1763
		// (get) Token: 0x06001A8A RID: 6794
		public abstract int SettlementCommissionDecreaseSecurityThreshold { get; }

		// Token: 0x170006E4 RID: 1764
		// (get) Token: 0x06001A8B RID: 6795
		public abstract int MaximumDecreaseBasedOnSecuritySecurity { get; }

		// Token: 0x06001A8C RID: 6796
		public abstract float GetTownTaxRatio(Town town);

		// Token: 0x06001A8D RID: 6797
		public abstract float GetVillageTaxRatio();

		// Token: 0x06001A8E RID: 6798
		public abstract float GetTownCommissionChangeBasedOnSecurity(Town town, float commission);

		// Token: 0x06001A8F RID: 6799
		public abstract ExplainedNumber CalculateTownTax(Town town, bool includeDescriptions = false);

		// Token: 0x06001A90 RID: 6800
		public abstract int CalculateVillageTaxFromIncome(Village village, int marketIncome);
	}
}
