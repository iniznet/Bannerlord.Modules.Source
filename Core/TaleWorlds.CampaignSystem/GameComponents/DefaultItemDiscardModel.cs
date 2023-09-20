﻿using System;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.GameComponents
{
	public class DefaultItemDiscardModel : ItemDiscardModel
	{
		public override bool PlayerCanDonateItem(ItemObject item)
		{
			bool flag = false;
			if (item.HasWeaponComponent)
			{
				flag = MobileParty.MainParty.HasPerk(DefaultPerks.Steward.GivingHands, false);
			}
			else if (item.HasArmorComponent)
			{
				flag = MobileParty.MainParty.HasPerk(DefaultPerks.Steward.PaidInPromise, true);
			}
			return flag;
		}

		public override int GetXpBonusForDiscardingItem(ItemObject item, int amount = 1)
		{
			int num = 0;
			if (this.PlayerCanDonateItem(item))
			{
				switch (item.Tier)
				{
				case ItemObject.ItemTiers.Tier1:
					num = 75;
					break;
				case ItemObject.ItemTiers.Tier2:
					num = 150;
					break;
				case ItemObject.ItemTiers.Tier3:
					num = 250;
					break;
				case ItemObject.ItemTiers.Tier4:
				case ItemObject.ItemTiers.Tier5:
				case ItemObject.ItemTiers.Tier6:
					num = 300;
					break;
				default:
					num = 35;
					break;
				}
			}
			return num * amount;
		}

		public override int GetXpBonusForDiscardingItems(ItemRoster itemRoster)
		{
			float num = 0f;
			for (int i = 0; i < itemRoster.Count; i++)
			{
				ItemObject itemAtIndex = itemRoster.GetItemAtIndex(i);
				num += (float)this.GetXpBonusForDiscardingItem(itemAtIndex, itemRoster.GetElementNumber(i));
			}
			return MathF.Floor(num);
		}
	}
}
