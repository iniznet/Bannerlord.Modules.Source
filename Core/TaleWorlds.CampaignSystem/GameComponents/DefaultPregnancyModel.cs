using System;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.ComponentInterfaces;

namespace TaleWorlds.CampaignSystem.GameComponents
{
	// Token: 0x0200012D RID: 301
	public class DefaultPregnancyModel : PregnancyModel
	{
		// Token: 0x17000612 RID: 1554
		// (get) Token: 0x060016CA RID: 5834 RVA: 0x0006FE8C File Offset: 0x0006E08C
		public override float PregnancyDurationInDays
		{
			get
			{
				return 36f;
			}
		}

		// Token: 0x17000613 RID: 1555
		// (get) Token: 0x060016CB RID: 5835 RVA: 0x0006FE93 File Offset: 0x0006E093
		public override float MaternalMortalityProbabilityInLabor
		{
			get
			{
				return 0.015f;
			}
		}

		// Token: 0x17000614 RID: 1556
		// (get) Token: 0x060016CC RID: 5836 RVA: 0x0006FE9A File Offset: 0x0006E09A
		public override float StillbirthProbability
		{
			get
			{
				return 0.01f;
			}
		}

		// Token: 0x17000615 RID: 1557
		// (get) Token: 0x060016CD RID: 5837 RVA: 0x0006FEA1 File Offset: 0x0006E0A1
		public override float DeliveringFemaleOffspringProbability
		{
			get
			{
				return 0.51f;
			}
		}

		// Token: 0x17000616 RID: 1558
		// (get) Token: 0x060016CE RID: 5838 RVA: 0x0006FEA8 File Offset: 0x0006E0A8
		public override float DeliveringTwinsProbability
		{
			get
			{
				return 0.03f;
			}
		}

		// Token: 0x060016CF RID: 5839 RVA: 0x0006FEAF File Offset: 0x0006E0AF
		private bool IsHeroAgeSuitableForPregnancy(Hero hero)
		{
			return hero.Age >= 18f && hero.Age <= 45f;
		}

		// Token: 0x060016D0 RID: 5840 RVA: 0x0006FED0 File Offset: 0x0006E0D0
		public override float GetDailyChanceOfPregnancyForHero(Hero hero)
		{
			int num = hero.Children.Count + 1;
			float num2 = (float)(4 + 4 * hero.Clan.Tier);
			float num3 = ((hero != Hero.MainHero && hero.Spouse != Hero.MainHero) ? Math.Min(1f, (2f * num2 - (float)hero.Clan.Lords.Count) / num2) : 1f);
			float num4 = (1.2f - (hero.Age - 18f) * 0.04f) / (float)(num * num) * 0.12f * num3;
			float num5 = ((hero.Spouse != null && this.IsHeroAgeSuitableForPregnancy(hero)) ? num4 : 0f);
			ExplainedNumber explainedNumber = new ExplainedNumber(num5, false, null);
			if (hero.GetPerkValue(DefaultPerks.Charm.Virile) || hero.Spouse.GetPerkValue(DefaultPerks.Charm.Virile))
			{
				explainedNumber.AddFactor(DefaultPerks.Charm.Virile.PrimaryBonus, DefaultPerks.Charm.Virile.Name);
			}
			return explainedNumber.ResultNumber;
		}

		// Token: 0x04000817 RID: 2071
		private const int MinPregnancyAge = 18;

		// Token: 0x04000818 RID: 2072
		private const int MaxPregnancyAge = 45;
	}
}
