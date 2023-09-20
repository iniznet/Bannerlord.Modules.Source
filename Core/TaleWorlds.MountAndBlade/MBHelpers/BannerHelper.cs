using System;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace MBHelpers
{
	// Token: 0x020000CB RID: 203
	public static class BannerHelper
	{
		// Token: 0x06000858 RID: 2136 RVA: 0x0000F152 File Offset: 0x0000D352
		public static void AddBannerBonusForBanner(BannerEffect bannerEffect, BannerComponent bannerComponent, ref FactoredNumber bonuses)
		{
			if (bannerComponent != null && bannerComponent.BannerEffect == bannerEffect)
			{
				BannerHelper.AddBannerEffectToStat(ref bonuses, bannerEffect.IncrementType, bannerComponent.GetBannerEffectBonus());
			}
		}

		// Token: 0x06000859 RID: 2137 RVA: 0x0000F172 File Offset: 0x0000D372
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
