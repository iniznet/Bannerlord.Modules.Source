using System;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.ComponentInterfaces
{
	// Token: 0x02000176 RID: 374
	public abstract class PartyTradeModel : GameModel
	{
		// Token: 0x17000681 RID: 1665
		// (get) Token: 0x06001912 RID: 6418
		public abstract int CaravanTransactionHighestValueItemCount { get; }

		// Token: 0x17000682 RID: 1666
		// (get) Token: 0x06001913 RID: 6419
		public abstract int SmallCaravanFormingCostForPlayer { get; }

		// Token: 0x17000683 RID: 1667
		// (get) Token: 0x06001914 RID: 6420
		public abstract int LargeCaravanFormingCostForPlayer { get; }

		// Token: 0x06001915 RID: 6421
		public abstract float GetTradePenaltyFactor(MobileParty party);
	}
}
