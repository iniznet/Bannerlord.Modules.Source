using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace Helpers
{
	// Token: 0x02000018 RID: 24
	public static class BannerHelper
	{
		// Token: 0x060000E0 RID: 224 RVA: 0x0000B98D File Offset: 0x00009B8D
		public static ItemObject GetRandomBannerItemForHero(Hero hero)
		{
			return Campaign.Current.Models.BannerItemModel.GetPossibleRewardBannerItemsForHero(hero).GetRandomElementInefficiently<ItemObject>();
		}

		// Token: 0x060000E1 RID: 225 RVA: 0x0000B9A9 File Offset: 0x00009BA9
		public static void AddBannerBonusForBanner(BannerEffect bannerEffect, BannerComponent bannerComponent, ref ExplainedNumber bonuses)
		{
			if (bannerComponent != null && bannerComponent.BannerEffect == bannerEffect)
			{
				BannerHelper.AddBannerEffectToStat(ref bonuses, bannerEffect.IncrementType, bannerComponent.GetBannerEffectBonus(), bannerEffect.Name);
			}
		}

		// Token: 0x060000E2 RID: 226 RVA: 0x0000B9CF File Offset: 0x00009BCF
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
