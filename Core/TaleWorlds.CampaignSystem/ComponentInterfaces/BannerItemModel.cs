using System;
using System.Collections.Generic;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.ComponentInterfaces
{
	// Token: 0x020001C9 RID: 457
	public abstract class BannerItemModel : GameModel
	{
		// Token: 0x06001B75 RID: 7029
		public abstract IEnumerable<ItemObject> GetPossibleRewardBannerItems();

		// Token: 0x06001B76 RID: 7030
		public abstract IEnumerable<ItemObject> GetPossibleRewardBannerItemsForHero(Hero hero);

		// Token: 0x06001B77 RID: 7031
		public abstract int GetBannerItemLevelForHero(Hero hero);

		// Token: 0x06001B78 RID: 7032
		public abstract bool CanBannerBeUpdated(ItemObject item);
	}
}
