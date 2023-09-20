using System;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.GameComponents
{
	public class DefaultPrisonerRecruitmentCalculationModel : PrisonerRecruitmentCalculationModel
	{
		public override int GetConformityNeededToRecruitPrisoner(CharacterObject character)
		{
			return (character.Level + 6) * (character.Level + 6) - 10;
		}

		public override int GetConformityChangePerHour(PartyBase party, CharacterObject troopToBoost)
		{
			ExplainedNumber explainedNumber = new ExplainedNumber(10f, false, null);
			if (party.LeaderHero != null)
			{
				explainedNumber.Add((float)party.LeaderHero.GetSkillValue(DefaultSkills.Leadership) * 0.05f, null, null);
			}
			if (troopToBoost.Tier <= 3 && party.MobileParty.HasPerk(DefaultPerks.Leadership.FerventAttacker, true))
			{
				explainedNumber.AddFactor(DefaultPerks.Leadership.FerventAttacker.SecondaryBonus, null);
			}
			if (troopToBoost.Tier >= 4 && party.MobileParty.HasPerk(DefaultPerks.Leadership.StoutDefender, true))
			{
				explainedNumber.AddFactor(DefaultPerks.Leadership.StoutDefender.SecondaryBonus, null);
			}
			if (troopToBoost.Occupation != Occupation.Bandit && party.MobileParty.HasPerk(DefaultPerks.Leadership.LoyaltyAndHonor, true))
			{
				explainedNumber.AddFactor(DefaultPerks.Leadership.LoyaltyAndHonor.SecondaryBonus, null);
			}
			if (troopToBoost.IsInfantry && party.MobileParty.HasPerk(DefaultPerks.Leadership.LeadByExample, false))
			{
				explainedNumber.AddFactor(DefaultPerks.Leadership.LeadByExample.PrimaryBonus, null);
			}
			if (troopToBoost.IsRanged && party.MobileParty.HasPerk(DefaultPerks.Leadership.TrustedCommander, false))
			{
				explainedNumber.AddFactor(DefaultPerks.Leadership.TrustedCommander.PrimaryBonus, null);
			}
			if (troopToBoost.Occupation == Occupation.Bandit && party.MobileParty.HasPerk(DefaultPerks.Roguery.Promises, true))
			{
				explainedNumber.AddFactor(DefaultPerks.Roguery.Promises.SecondaryBonus, null);
			}
			return MathF.Round(explainedNumber.ResultNumber);
		}

		public override int GetPrisonerRecruitmentMoraleEffect(PartyBase party, CharacterObject character, int num)
		{
			CultureObject culture = character.Culture;
			Hero leaderHero = party.LeaderHero;
			if (culture == ((leaderHero != null) ? leaderHero.Culture : null))
			{
				MobileParty mobileParty = party.MobileParty;
				if (mobileParty != null && mobileParty.HasPerk(DefaultPerks.Leadership.Presence, true))
				{
					return 0;
				}
			}
			if (character.Occupation == Occupation.Bandit)
			{
				MobileParty mobileParty2 = party.MobileParty;
				if (mobileParty2 != null && mobileParty2.HasPerk(DefaultPerks.Roguery.TwoFaced, true))
				{
					return 0;
				}
			}
			int num2;
			if (character.Occupation == Occupation.Bandit)
			{
				num2 = -2;
			}
			else
			{
				num2 = -1;
			}
			return num2 * num;
		}

		public override bool IsPrisonerRecruitable(PartyBase party, CharacterObject character, out int conformityNeeded)
		{
			if (!character.IsRegular || character.Tier > Campaign.Current.Models.CharacterStatsModel.MaxCharacterTier)
			{
				conformityNeeded = 0;
				return false;
			}
			int elementXp = party.MobileParty.PrisonRoster.GetElementXp(character);
			conformityNeeded = this.GetConformityNeededToRecruitPrisoner(character);
			return elementXp >= conformityNeeded;
		}

		public override bool ShouldPartyRecruitPrisoners(PartyBase party)
		{
			return (party.MobileParty.Morale > 30f || party.MobileParty.HasPerk(DefaultPerks.Leadership.Presence, true)) && party.PartySizeLimit > party.MobileParty.MemberRoster.TotalManCount;
		}

		public override int CalculateRecruitableNumber(PartyBase party, CharacterObject character)
		{
			if (character.IsHero || party.PrisonRoster.Count == 0 || party.PrisonRoster.TotalRegulars <= 0)
			{
				return 0;
			}
			int conformityNeededToRecruitPrisoner = Campaign.Current.Models.PrisonerRecruitmentCalculationModel.GetConformityNeededToRecruitPrisoner(character);
			int elementXp = party.PrisonRoster.GetElementXp(character);
			int elementNumber = party.PrisonRoster.GetElementNumber(character);
			return MathF.Min(elementXp / conformityNeededToRecruitPrisoner, elementNumber);
		}
	}
}
