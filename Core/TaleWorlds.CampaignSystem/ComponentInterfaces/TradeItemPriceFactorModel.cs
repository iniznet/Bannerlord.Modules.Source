using System;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.ComponentInterfaces
{
	// Token: 0x0200019A RID: 410
	public abstract class TradeItemPriceFactorModel : GameModel
	{
		// Token: 0x06001A2A RID: 6698
		public abstract float GetTradePenalty(ItemObject item, MobileParty clientParty, PartyBase merchant, bool isSelling, float inStore, float supply, float demand);

		// Token: 0x06001A2B RID: 6699
		public abstract float GetBasePriceFactor(ItemCategory itemCategory, float inStoreValue, float supply, float demand, bool isSelling, int transferValue);

		// Token: 0x06001A2C RID: 6700
		public abstract int GetPrice(EquipmentElement itemRosterElement, MobileParty clientParty, PartyBase merchant, bool isSelling, float inStoreValue, float supply, float demand);

		// Token: 0x06001A2D RID: 6701
		public abstract int GetTheoreticalMaxItemMarketValue(ItemObject item);
	}
}
