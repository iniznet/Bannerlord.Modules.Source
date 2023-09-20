using System;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.ComponentInterfaces
{
	// Token: 0x02000170 RID: 368
	public abstract class ItemDiscardModel : GameModel
	{
		// Token: 0x060018F0 RID: 6384
		public abstract int GetXpBonusForDiscardingItems(ItemRoster itemRoster);

		// Token: 0x060018F1 RID: 6385
		public abstract int GetXpBonusForDiscardingItem(ItemObject item, int amount = 1);

		// Token: 0x060018F2 RID: 6386
		public abstract bool PlayerCanDonateItem(ItemObject item);
	}
}
