using System;
using Helpers;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.GameComponents
{
	public class DefaultPrisonerDonationModel : PrisonerDonationModel
	{
		public override float CalculateRelationGainAfterHeroPrisonerDonate(PartyBase donatingParty, Hero donatedHero, Settlement donatedSettlement)
		{
			float num = 0f;
			int num2 = Campaign.Current.Models.RansomValueCalculationModel.PrisonerRansomValue(donatedHero.CharacterObject, donatingParty.LeaderHero);
			int relation = donatedHero.GetRelation(donatedSettlement.OwnerClan.Leader);
			if (relation <= 0)
			{
				float num3 = 1f - (float)relation / 200f;
				if (donatedHero.IsKingdomLeader)
				{
					num = MathF.Min(40f, MathF.Pow((float)num2, 0.5f) * 0.5f) * num3;
				}
				else if (donatedHero.Clan.Leader == donatedHero)
				{
					num = MathF.Min(30f, MathF.Pow((float)num2, 0.5f) * 0.25f) * num3;
				}
				else
				{
					num = MathF.Min(20f, MathF.Pow((float)num2, 0.5f) * 0.1f) * num3;
				}
			}
			return num;
		}

		public override float CalculateInfluenceGainAfterPrisonerDonation(PartyBase donatingParty, CharacterObject character, Settlement donatedSettlement)
		{
			float num = 0f;
			if (donatingParty.LeaderHero == Hero.MainHero)
			{
				if (character.IsHero)
				{
					Hero heroObject = character.HeroObject;
					float num2 = MathF.Pow((float)Campaign.Current.Models.RansomValueCalculationModel.PrisonerRansomValue(heroObject.CharacterObject, donatingParty.LeaderHero), 0.4f);
					if (heroObject.IsKingdomLeader)
					{
						num = num2;
					}
					else if (heroObject.Clan.Leader == heroObject)
					{
						num = num2 * 0.5f;
					}
					else
					{
						num = num2 * 0.2f;
					}
				}
				else
				{
					num += character.GetPower() / 20f;
				}
			}
			else
			{
				int tier = character.Tier;
				num = (float)((2 + tier) * (8 + tier)) * 0.02f;
			}
			return num;
		}

		public override float CalculateInfluenceGainAfterTroopDonation(PartyBase donatingParty, CharacterObject donatedCharacter, Settlement donatedSettlement)
		{
			Hero leaderHero = donatingParty.LeaderHero;
			ExplainedNumber explainedNumber = new ExplainedNumber(donatedCharacter.GetPower() / 3f, false, null);
			if (leaderHero != null && leaderHero.GetPerkValue(DefaultPerks.Steward.Relocation))
			{
				PerkHelper.AddPerkBonusForParty(DefaultPerks.Steward.Relocation, donatingParty.MobileParty, true, ref explainedNumber);
			}
			return explainedNumber.ResultNumber;
		}
	}
}
