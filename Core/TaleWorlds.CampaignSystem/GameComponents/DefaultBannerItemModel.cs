using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Extensions;
using TaleWorlds.Core;
using TaleWorlds.LinQuick;

namespace TaleWorlds.CampaignSystem.GameComponents
{
	public class DefaultBannerItemModel : BannerItemModel
	{
		public override IEnumerable<ItemObject> GetPossibleRewardBannerItems()
		{
			return Items.All.WhereQ((ItemObject i) => i.IsBannerItem && i.StringId != "campaign_banner_small");
		}

		public override IEnumerable<ItemObject> GetPossibleRewardBannerItemsForHero(Hero hero)
		{
			IEnumerable<ItemObject> possibleRewardBannerItems = this.GetPossibleRewardBannerItems();
			int bannerItemLevelForHero = this.GetBannerItemLevelForHero(hero);
			List<ItemObject> list = new List<ItemObject>();
			foreach (ItemObject itemObject in possibleRewardBannerItems)
			{
				if ((itemObject.Culture == null || itemObject.Culture == hero.Culture) && (itemObject.ItemComponent as BannerComponent).BannerLevel == bannerItemLevelForHero)
				{
					list.Add(itemObject);
				}
			}
			return list;
		}

		public override int GetBannerItemLevelForHero(Hero hero)
		{
			if (hero.Clan == null || hero.Clan.Leader != hero)
			{
				return 1;
			}
			if (hero.MapFaction.IsKingdomFaction && hero.Clan.Kingdom.RulingClan == hero.Clan)
			{
				return 3;
			}
			return 2;
		}

		public override bool CanBannerBeUpdated(ItemObject item)
		{
			return true;
		}

		public const int BannerLevel1 = 1;

		public const int BannerLevel2 = 2;

		public const int BannerLevel3 = 3;

		private const string MapBannerId = "campaign_banner_small";
	}
}
