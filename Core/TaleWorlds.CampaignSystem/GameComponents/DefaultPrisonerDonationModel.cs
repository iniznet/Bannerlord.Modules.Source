using System;
using Helpers;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.GameComponents
{
	// Token: 0x0200012F RID: 303
	public class DefaultPrisonerDonationModel : PrisonerDonationModel
	{
		// Token: 0x060016D7 RID: 5847 RVA: 0x00070100 File Offset: 0x0006E300
		public override float CalculateRelationGainAfterHeroPrisonerDonate(PartyBase donatingParty, Hero donatedHero, Settlement donatedSettlement)
		{
			float num = 0f;
			int num2 = Campaign.Current.Models.RansomValueCalculationModel.PrisonerRansomValue(donatedHero.CharacterObject, donatingParty.LeaderHero);
			int relation = donatedHero.GetRelation(donatedSettlement.OwnerClan.Leader);
			if (relation <= 0)
			{
				float num3 = 1f - (float)relation / 200f;
				if (donatedHero.MapFaction.IsKingdomFaction && donatedHero.IsFactionLeader)
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

		// Token: 0x060016D8 RID: 5848 RVA: 0x000701E4 File Offset: 0x0006E3E4
		public override float CalculateInfluenceGainAfterPrisonerDonation(PartyBase donatingParty, CharacterObject character, Settlement donatedSettlement)
		{
			float num = 0f;
			if (donatingParty.LeaderHero == Hero.MainHero)
			{
				if (character.IsHero)
				{
					Hero heroObject = character.HeroObject;
					float num2 = MathF.Pow((float)Campaign.Current.Models.RansomValueCalculationModel.PrisonerRansomValue(heroObject.CharacterObject, donatingParty.LeaderHero), 0.4f);
					if (heroObject.MapFaction.IsKingdomFaction && heroObject.IsFactionLeader)
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

		// Token: 0x060016D9 RID: 5849 RVA: 0x000702A8 File Offset: 0x0006E4A8
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
