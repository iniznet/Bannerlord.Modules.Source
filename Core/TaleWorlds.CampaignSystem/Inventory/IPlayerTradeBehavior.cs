using System;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.Inventory
{
	// Token: 0x020000D0 RID: 208
	public interface IPlayerTradeBehavior
	{
		// Token: 0x060012AD RID: 4781
		int GetProjectedProfit(ItemRosterElement itemRosterElement, int itemCost);
	}
}
