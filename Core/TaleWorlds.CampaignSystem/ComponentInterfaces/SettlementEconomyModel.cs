using System;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.ComponentInterfaces
{
	// Token: 0x0200019D RID: 413
	public abstract class SettlementEconomyModel : GameModel
	{
		// Token: 0x06001A39 RID: 6713
		public abstract float GetEstimatedDemandForCategory(Town town, ItemData itemData, ItemCategory category);

		// Token: 0x06001A3A RID: 6714
		public abstract float GetDailyDemandForCategory(Town town, ItemCategory category, int extraProsperity = 0);

		// Token: 0x06001A3B RID: 6715
		public abstract float GetDemandChangeFromValue(float purchaseValue);

		// Token: 0x06001A3C RID: 6716
		public abstract ValueTuple<float, float> GetSupplyDemandForCategory(Town town, ItemCategory category, float dailySupply, float dailyDemand, float oldSupply, float oldDemand);

		// Token: 0x06001A3D RID: 6717
		public abstract int GetTownGoldChange(Town town);
	}
}
