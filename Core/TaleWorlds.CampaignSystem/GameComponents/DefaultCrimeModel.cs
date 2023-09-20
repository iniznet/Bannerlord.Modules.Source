using System;
using System.Linq;
using Helpers;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.GameComponents
{
	public class DefaultCrimeModel : CrimeModel
	{
		public override bool DoesPlayerHaveAnyCrimeRating(IFaction faction)
		{
			return faction.MainHeroCrimeRating > 0f;
		}

		public override bool IsPlayerCrimeRatingSevere(IFaction faction)
		{
			return faction.MainHeroCrimeRating >= 65f;
		}

		public override bool IsPlayerCrimeRatingModerate(IFaction faction)
		{
			return faction.MainHeroCrimeRating > 30f && faction.MainHeroCrimeRating <= 65f;
		}

		public override bool IsPlayerCrimeRatingMild(IFaction faction)
		{
			return faction.MainHeroCrimeRating > 0f && faction.MainHeroCrimeRating <= 30f;
		}

		public override float GetCost(IFaction faction, CrimeModel.PaymentMethod paymentMethod, float minimumCrimeRating)
		{
			float num = MathF.Max(0f, faction.MainHeroCrimeRating - minimumCrimeRating);
			if (paymentMethod == CrimeModel.PaymentMethod.Gold)
			{
				return (float)((int)(MathF.Pow(num, 1.2f) * 100f));
			}
			if (paymentMethod != CrimeModel.PaymentMethod.Influence)
			{
				return 0f;
			}
			return MathF.Pow(num, 1.2f);
		}

		public override ExplainedNumber GetDailyCrimeRatingChange(IFaction faction, bool includeDescriptions = false)
		{
			ExplainedNumber explainedNumber = new ExplainedNumber(0f, includeDescriptions, null);
			int num = faction.Settlements.Count(delegate(Settlement x)
			{
				if (x.IsTown)
				{
					return x.Alleys.Any((Alley y) => y.Owner == Hero.MainHero);
				}
				return false;
			});
			explainedNumber.Add((float)num * Campaign.Current.Models.AlleyModel.GetDailyCrimeRatingOfAlley, new TextObject("{=t87T82jq}Owned alleys", null), null);
			if (faction.MainHeroCrimeRating.ApproximatelyEqualsTo(0f, 1E-05f))
			{
				return explainedNumber;
			}
			Clan clan = faction as Clan;
			if (Hero.MainHero.Clan == faction)
			{
				explainedNumber.Add(-5f, includeDescriptions ? new TextObject("{=eNtRt6F5}Your own Clan", null) : TextObject.Empty, null);
			}
			else if (Hero.MainHero.Clan.Kingdom != null && Hero.MainHero.Clan.Kingdom.Leader == Hero.MainHero)
			{
				explainedNumber.Add(-5f, includeDescriptions ? new TextObject("{=xer2bta5}Your own Kingdom", null) : TextObject.Empty, null);
			}
			else if (Hero.MainHero.MapFaction == faction)
			{
				explainedNumber.Add(-1.5f, includeDescriptions ? new TextObject("{=QRwaQIbm}Is in Kingdom", null) : TextObject.Empty, null);
			}
			else if (clan != null && Hero.MainHero.MapFaction == clan.Kingdom)
			{
				explainedNumber.Add(-1.25f, includeDescriptions ? new TextObject("{=hXGByLG9}Sharing the same Kingdom", null) : TextObject.Empty, null);
			}
			else if (Hero.MainHero.Clan.IsAtWarWith(faction))
			{
				explainedNumber.Add(-0.25f, includeDescriptions ? new TextObject("{=BYTrUJyj}In War", null) : TextObject.Empty, null);
			}
			else
			{
				explainedNumber.Add(-1f, includeDescriptions ? new TextObject("{=basevalue}Base", null) : TextObject.Empty, null);
			}
			PerkHelper.AddPerkBonusForCharacter(DefaultPerks.Roguery.WhiteLies, Hero.MainHero.CharacterObject, true, ref explainedNumber);
			return explainedNumber;
		}

		public override int DeclareWarCrimeRatingThreshold
		{
			get
			{
				return 60;
			}
		}

		public override float GetMaxCrimeRating()
		{
			return 100f;
		}

		public override float GetMinAcceptableCrimeRating(IFaction faction)
		{
			if (faction != Hero.MainHero.MapFaction)
			{
				return 30f;
			}
			return 20f;
		}

		private const float ModerateCrimeRatingThreshold = 30f;

		private const float SevereCrimeRatingThreshold = 65f;
	}
}
