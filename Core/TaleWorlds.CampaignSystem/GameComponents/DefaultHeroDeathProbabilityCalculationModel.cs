using System;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.GameComponents
{
	// Token: 0x0200010F RID: 271
	public class DefaultHeroDeathProbabilityCalculationModel : HeroDeathProbabilityCalculationModel
	{
		// Token: 0x060015B7 RID: 5559 RVA: 0x00066B7C File Offset: 0x00064D7C
		public override float CalculateHeroDeathProbability(Hero hero)
		{
			return this.CalculateHeroDeathProbabilityInternal(hero);
		}

		// Token: 0x060015B8 RID: 5560 RVA: 0x00066B88 File Offset: 0x00064D88
		private float CalculateHeroDeathProbabilityInternal(Hero hero)
		{
			float num = 0f;
			if (!CampaignOptions.IsLifeDeathCycleDisabled)
			{
				int becomeOldAge = Campaign.Current.Models.AgeModel.BecomeOldAge;
				int num2 = Campaign.Current.Models.AgeModel.MaxAge - 1;
				if (hero.Age > (float)becomeOldAge)
				{
					if (hero.Age < (float)num2)
					{
						float num3 = 0.3f * ((hero.Age - (float)becomeOldAge) / (float)(Campaign.Current.Models.AgeModel.MaxAge - becomeOldAge));
						float num4 = 1f - MathF.Pow(1f - num3, 0.011904762f);
						num += num4;
					}
					else if (hero.Age >= (float)num2)
					{
						num += 1f;
					}
				}
			}
			return num;
		}
	}
}
