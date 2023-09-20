using System;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Siege;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.ComponentInterfaces
{
	// Token: 0x020001B4 RID: 436
	public abstract class TroopSacrificeModel : GameModel
	{
		// Token: 0x06001AE3 RID: 6883
		public abstract int GetLostTroopCountForBreakingInBesiegedSettlement(MobileParty party, SiegeEvent siegeEvent);

		// Token: 0x06001AE4 RID: 6884
		public abstract int GetLostTroopCountForBreakingOutOfBesiegedSettlement(MobileParty party, SiegeEvent siegeEvent);

		// Token: 0x06001AE5 RID: 6885
		public abstract int GetNumberOfTroopsSacrificedForTryingToGetAway(BattleSideEnum battleSide, MapEvent mapEvent);
	}
}
