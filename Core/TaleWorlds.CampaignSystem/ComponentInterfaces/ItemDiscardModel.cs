﻿using System;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.ComponentInterfaces
{
	public abstract class ItemDiscardModel : GameModel
	{
		public abstract int GetXpBonusForDiscardingItems(ItemRoster itemRoster);

		public abstract int GetXpBonusForDiscardingItem(ItemObject item, int amount = 1);

		public abstract bool PlayerCanDonateItem(ItemObject item);
	}
}
