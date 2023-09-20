﻿using System;
using System.Collections.Generic;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.ComponentInterfaces
{
	public abstract class BannerItemModel : GameModel
	{
		public abstract IEnumerable<ItemObject> GetPossibleRewardBannerItems();

		public abstract IEnumerable<ItemObject> GetPossibleRewardBannerItemsForHero(Hero hero);

		public abstract int GetBannerItemLevelForHero(Hero hero);

		public abstract bool CanBannerBeUpdated(ItemObject item);
	}
}
