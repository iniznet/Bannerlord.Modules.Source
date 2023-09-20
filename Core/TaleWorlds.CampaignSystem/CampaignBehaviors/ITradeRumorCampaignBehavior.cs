using System;
using System.Collections.Generic;

namespace TaleWorlds.CampaignSystem.CampaignBehaviors
{
	// Token: 0x020003A7 RID: 935
	public interface ITradeRumorCampaignBehavior : ICampaignBehavior
	{
		// Token: 0x17000CD2 RID: 3282
		// (get) Token: 0x060037C6 RID: 14278
		IEnumerable<TradeRumor> TradeRumors { get; }
	}
}
