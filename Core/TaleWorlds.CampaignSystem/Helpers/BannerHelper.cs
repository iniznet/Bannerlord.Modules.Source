using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace Helpers
{
	public static class BannerHelper
	{
		public static ItemObject GetRandomBannerItemForHero(Hero hero)
		{
			return Campaign.Current.Models.BannerItemModel.GetPossibleRewardBannerItemsForHero(hero).GetRandomElementInefficiently<ItemObject>();
		}

		public static void AddBannerBonusForBanner(BannerEffect bannerEffect, BannerComponent bannerComponent, ref ExplainedNumber bonuses)
		{
			if (bannerComponent != null && bannerComponent.BannerEffect == bannerEffect)
			{
				BannerHelper.AddBannerEffectToStat(ref bonuses, bannerEffect.IncrementType, bannerComponent.GetBannerEffectBonus(), bannerEffect.Name);
			}
		}

		private static void AddBannerEffectToStat(ref ExplainedNumber stat, BannerEffect.EffectIncrementType effectIncrementType, float number, TextObject effectName)
		{
			if (effectIncrementType == BannerEffect.EffectIncrementType.Add)
			{
				stat.Add(number, effectName, null);
				return;
			}
			if (effectIncrementType == BannerEffect.EffectIncrementType.AddFactor)
			{
				stat.AddFactor(number, effectName);
			}
		}
	}
}
