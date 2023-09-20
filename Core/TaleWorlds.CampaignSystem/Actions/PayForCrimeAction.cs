using System;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.Actions
{
	public static class PayForCrimeAction
	{
		private static void ApplyInternal(IFaction faction, CrimeModel.PaymentMethod paymentMethod)
		{
			bool flag = false;
			if (paymentMethod.HasAnyFlag(CrimeModel.PaymentMethod.Gold))
			{
				GiveGoldAction.ApplyBetweenCharacters(Hero.MainHero, null, (int)PayForCrimeAction.GetClearCrimeCost(faction, CrimeModel.PaymentMethod.Gold), false);
				SkillLevelingManager.OnBribeGiven((int)PayForCrimeAction.GetClearCrimeCost(faction, CrimeModel.PaymentMethod.Gold));
			}
			if (paymentMethod.HasAnyFlag(CrimeModel.PaymentMethod.Influence))
			{
				ChangeClanInfluenceAction.Apply(Clan.PlayerClan, -PayForCrimeAction.GetClearCrimeCost(faction, CrimeModel.PaymentMethod.Influence));
			}
			if (paymentMethod.HasAnyFlag(CrimeModel.PaymentMethod.Punishment))
			{
				if (MathF.Clamp(1f - (float)Hero.MainHero.HitPoints * 0.01f, 0.001f, 1f) * 0.25f > MBRandom.RandomFloat)
				{
					flag = true;
					KillCharacterAction.ApplyByMurder(Hero.MainHero, null, true);
				}
				else
				{
					Hero.MainHero.MakeWounded(null, KillCharacterAction.KillCharacterActionDetail.None);
					float num = 0.5f;
					if (MBRandom.RandomFloat < num)
					{
						SkillLevelingManager.OnMainHeroTortured();
					}
				}
			}
			if (paymentMethod.HasAnyFlag(CrimeModel.PaymentMethod.Execution))
			{
				flag = true;
				KillCharacterAction.ApplyByMurder(Hero.MainHero, null, true);
			}
			if (!flag)
			{
				float num2 = MathF.Min(faction.MainHeroCrimeRating, Campaign.Current.Models.CrimeModel.GetMinAcceptableCrimeRating(faction));
				ChangeCrimeRatingAction.Apply(faction, num2 - faction.MainHeroCrimeRating, true);
			}
		}

		public static float GetClearCrimeCost(IFaction faction, CrimeModel.PaymentMethod paymentMethod)
		{
			return Campaign.Current.Models.CrimeModel.GetCost(faction, paymentMethod, Campaign.Current.Models.CrimeModel.GetMinAcceptableCrimeRating(faction));
		}

		public static void Apply(IFaction faction, CrimeModel.PaymentMethod paymentMethod)
		{
			PayForCrimeAction.ApplyInternal(faction, paymentMethod);
		}
	}
}
