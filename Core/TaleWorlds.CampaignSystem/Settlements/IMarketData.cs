using System;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.Settlements
{
	// Token: 0x02000367 RID: 871
	public interface IMarketData
	{
		// Token: 0x06003289 RID: 12937
		int GetPrice(ItemObject item, MobileParty tradingParty, bool isSelling, PartyBase merchantParty);

		// Token: 0x0600328A RID: 12938
		int GetPrice(EquipmentElement itemRosterElement, MobileParty tradingParty, bool isSelling, PartyBase merchantParty);
	}
}
