using System;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace MBHelpers
{
	public static class BannerHelper
	{
		public static void AddBannerBonusForBanner(BannerEffect bannerEffect, BannerComponent bannerComponent, ref FactoredNumber bonuses)
		{
			if (bannerComponent != null && bannerComponent.BannerEffect == bannerEffect)
			{
				BannerHelper.AddBannerEffectToStat(ref bonuses, bannerEffect.IncrementType, bannerComponent.GetBannerEffectBonus());
			}
		}

		private static void AddBannerEffectToStat(ref FactoredNumber stat, BannerEffect.EffectIncrementType effectIncrementType, float number)
		{
			if (effectIncrementType == BannerEffect.EffectIncrementType.Add)
			{
				stat.Add(number);
				return;
			}
			if (effectIncrementType == BannerEffect.EffectIncrementType.AddFactor)
			{
				stat.AddFactor(number);
			}
		}
	}
}
