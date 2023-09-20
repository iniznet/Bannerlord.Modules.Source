using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Extensions;
using TaleWorlds.Core;
using TaleWorlds.LinQuick;

namespace TaleWorlds.CampaignSystem.GameComponents
{
	// Token: 0x020000F0 RID: 240
	public class DefaultBannerItemModel : BannerItemModel
	{
		// Token: 0x06001476 RID: 5238 RVA: 0x0005B124 File Offset: 0x00059324
		public override IEnumerable<ItemObject> GetPossibleRewardBannerItems()
		{
			return Items.All.WhereQ((ItemObject i) => i.IsBannerItem && i.StringId != "campaign_banner_small");
		}

		// Token: 0x06001477 RID: 5239 RVA: 0x0005B150 File Offset: 0x00059350
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

		// Token: 0x06001478 RID: 5240 RVA: 0x0005B1D8 File Offset: 0x000593D8
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

		// Token: 0x06001479 RID: 5241 RVA: 0x0005B225 File Offset: 0x00059425
		public override bool CanBannerBeUpdated(ItemObject item)
		{
			return true;
		}

		// Token: 0x0400072E RID: 1838
		public const int BannerLevel1 = 1;

		// Token: 0x0400072F RID: 1839
		public const int BannerLevel2 = 2;

		// Token: 0x04000730 RID: 1840
		public const int BannerLevel3 = 3;

		// Token: 0x04000731 RID: 1841
		private const string MapBannerId = "campaign_banner_small";
	}
}
