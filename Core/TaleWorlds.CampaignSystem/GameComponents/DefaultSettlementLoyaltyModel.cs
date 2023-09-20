using System;
using Helpers;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Issues;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Buildings;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.GameComponents
{
	public class DefaultSettlementLoyaltyModel : SettlementLoyaltyModel
	{
		public override float HighLoyaltyProsperityEffect
		{
			get
			{
				return 0.5f;
			}
		}

		public override int LowLoyaltyProsperityEffect
		{
			get
			{
				return -1;
			}
		}

		public override int ThresholdForTaxBoost
		{
			get
			{
				return 75;
			}
		}

		public override int ThresholdForTaxCorruption
		{
			get
			{
				return 50;
			}
		}

		public override int ThresholdForHigherTaxCorruption
		{
			get
			{
				return 25;
			}
		}

		public override int ThresholdForProsperityBoost
		{
			get
			{
				return 75;
			}
		}

		public override int ThresholdForProsperityPenalty
		{
			get
			{
				return 25;
			}
		}

		public override int AdditionalStarvationPenaltyStartDay
		{
			get
			{
				return 14;
			}
		}

		public override int AdditionalStarvationLoyaltyEffect
		{
			get
			{
				return -1;
			}
		}

		public override int RebellionStartLoyaltyThreshold
		{
			get
			{
				return 15;
			}
		}

		public override int RebelliousStateStartLoyaltyThreshold
		{
			get
			{
				return 25;
			}
		}

		public override int LoyaltyBoostAfterRebellionStartValue
		{
			get
			{
				return 5;
			}
		}

		public override int MilitiaBoostPercentage
		{
			get
			{
				return 200;
			}
		}

		public override float ThresholdForNotableRelationBonus
		{
			get
			{
				return 75f;
			}
		}

		public override int DailyNotableRelationBonus
		{
			get
			{
				return 1;
			}
		}

		public override int SettlementLoyaltyChangeDueToSecurityThreshold
		{
			get
			{
				return 50;
			}
		}

		public override int MaximumLoyaltyInSettlement
		{
			get
			{
				return 100;
			}
		}

		public override int LoyaltyDriftMedium
		{
			get
			{
				return 50;
			}
		}

		public override float HighSecurityLoyaltyEffect
		{
			get
			{
				return 1f;
			}
		}

		public override float LowSecurityLoyaltyEffect
		{
			get
			{
				return -2f;
			}
		}

		public override float GovernorSameCultureLoyaltyEffect
		{
			get
			{
				return 1f;
			}
		}

		public override float GovernorDifferentCultureLoyaltyEffect
		{
			get
			{
				return -1f;
			}
		}

		public override float SettlementOwnerDifferentCultureLoyaltyEffect
		{
			get
			{
				return -3f;
			}
		}

		public override ExplainedNumber CalculateLoyaltyChange(Town town, bool includeDescriptions = false)
		{
			return this.CalculateLoyaltyChangeInternal(town, includeDescriptions);
		}

		public override void CalculateGoldGainDueToHighLoyalty(Town town, ref ExplainedNumber explainedNumber)
		{
			float num = MBMath.Map(town.Loyalty, (float)this.ThresholdForTaxBoost, 100f, 0f, 20f);
			explainedNumber.AddFactor(num * 0.01f, DefaultSettlementLoyaltyModel.LoyaltyText);
		}

		public override void CalculateGoldCutDueToLowLoyalty(Town town, ref ExplainedNumber explainedNumber)
		{
			float num = MBMath.Map(town.Loyalty, (float)this.ThresholdForHigherTaxCorruption, (float)this.ThresholdForTaxCorruption, 50f, 0f);
			explainedNumber.AddFactor(-1f * num * 0.01f, DefaultSettlementLoyaltyModel.CorruptionText);
		}

		private ExplainedNumber CalculateLoyaltyChangeInternal(Town town, bool includeDescriptions = false)
		{
			ExplainedNumber explainedNumber = new ExplainedNumber(0f, includeDescriptions, null);
			this.GetSettlementLoyaltyChangeDueToFoodStocks(town, ref explainedNumber);
			this.GetSettlementLoyaltyChangeDueToGovernorCulture(town, ref explainedNumber);
			this.GetSettlementLoyaltyChangeDueToOwnerCulture(town, ref explainedNumber);
			this.GetSettlementLoyaltyChangeDueToPolicies(town, ref explainedNumber);
			this.GetSettlementLoyaltyChangeDueToProjects(town, ref explainedNumber);
			this.GetSettlementLoyaltyChangeDueToIssues(town, ref explainedNumber);
			this.GetSettlementLoyaltyChangeDueToSecurity(town, ref explainedNumber);
			this.GetSettlementLoyaltyChangeDueToNotableRelations(town, ref explainedNumber);
			this.GetSettlementLoyaltyChangeDueToGovernorPerks(town, ref explainedNumber);
			this.GetSettlementLoyaltyChangeDueToLoyaltyDrift(town, ref explainedNumber);
			return explainedNumber;
		}

		private void GetSettlementLoyaltyChangeDueToGovernorPerks(Town town, ref ExplainedNumber explainedNumber)
		{
			PerkHelper.AddPerkBonusForTown(DefaultPerks.Leadership.HeroicLeader, town, ref explainedNumber);
			PerkHelper.AddPerkBonusForTown(DefaultPerks.Medicine.PhysicianOfPeople, town, ref explainedNumber);
			PerkHelper.AddPerkBonusForTown(DefaultPerks.Athletics.Durable, town, ref explainedNumber);
			PerkHelper.AddPerkBonusForTown(DefaultPerks.Bow.Discipline, town, ref explainedNumber);
			PerkHelper.AddPerkBonusForTown(DefaultPerks.Riding.WellStraped, town, ref explainedNumber);
			float num = 0f;
			for (int i = 0; i < town.Settlement.Parties.Count; i++)
			{
				MobileParty mobileParty = town.Settlement.Parties[i];
				if (mobileParty.ActualClan == town.OwnerClan)
				{
					if (mobileParty.IsMainParty)
					{
						for (int j = 0; j < mobileParty.MemberRoster.Count; j++)
						{
							CharacterObject characterAtIndex = mobileParty.MemberRoster.GetCharacterAtIndex(j);
							if (characterAtIndex.IsHero && characterAtIndex.HeroObject.GetPerkValue(DefaultPerks.Charm.Parade))
							{
								num += DefaultPerks.Charm.Parade.PrimaryBonus;
							}
						}
					}
					else if (mobileParty.LeaderHero != null && mobileParty.LeaderHero.GetPerkValue(DefaultPerks.Charm.Parade))
					{
						num += DefaultPerks.Charm.Parade.PrimaryBonus;
					}
				}
			}
			foreach (Hero hero in town.Settlement.HeroesWithoutParty)
			{
				if (hero.Clan == town.OwnerClan && hero.GetPerkValue(DefaultPerks.Charm.Parade))
				{
					num += DefaultPerks.Charm.Parade.PrimaryBonus;
				}
			}
			if (num > 0f)
			{
				explainedNumber.Add(num, DefaultPerks.Charm.Parade.Name, null);
			}
		}

		private void GetSettlementLoyaltyChangeDueToNotableRelations(Town town, ref ExplainedNumber explainedNumber)
		{
			float num = 0f;
			foreach (Hero hero in town.Settlement.Notables)
			{
				if (hero.SupporterOf != null)
				{
					if (hero.SupporterOf == town.Settlement.OwnerClan)
					{
						num += 0.5f;
					}
					else if (town.Settlement.OwnerClan.IsAtWarWith(hero.SupporterOf))
					{
						num += -0.5f;
					}
				}
			}
			if (!num.ApproximatelyEqualsTo(0f, 1E-05f))
			{
				explainedNumber.Add(num, DefaultSettlementLoyaltyModel.NotableText, null);
			}
		}

		private void GetSettlementLoyaltyChangeDueToOwnerCulture(Town town, ref ExplainedNumber explainedNumber)
		{
			if (town.Settlement.OwnerClan.Culture != town.Settlement.Culture)
			{
				explainedNumber.Add(this.SettlementOwnerDifferentCultureLoyaltyEffect, DefaultSettlementLoyaltyModel.CultureText, null);
			}
		}

		private void GetSettlementLoyaltyChangeDueToPolicies(Town town, ref ExplainedNumber explainedNumber)
		{
			Kingdom kingdom = town.Owner.Settlement.OwnerClan.Kingdom;
			if (kingdom != null)
			{
				if (kingdom.ActivePolicies.Contains(DefaultPolicies.Citizenship))
				{
					if (town.Settlement.OwnerClan.Culture == town.Settlement.Culture)
					{
						explainedNumber.Add(0.5f, DefaultPolicies.Citizenship.Name, null);
					}
					else
					{
						explainedNumber.Add(-0.5f, DefaultPolicies.Citizenship.Name, null);
					}
				}
				if (kingdom.ActivePolicies.Contains(DefaultPolicies.HuntingRights))
				{
					explainedNumber.Add(-0.2f, DefaultPolicies.HuntingRights.Name, null);
				}
				if (kingdom.ActivePolicies.Contains(DefaultPolicies.GrazingRights))
				{
					explainedNumber.Add(0.5f, DefaultPolicies.GrazingRights.Name, null);
				}
				if (kingdom.ActivePolicies.Contains(DefaultPolicies.TrialByJury))
				{
					explainedNumber.Add(0.5f, DefaultPolicies.TrialByJury.Name, null);
				}
				if (kingdom.ActivePolicies.Contains(DefaultPolicies.ImperialTowns))
				{
					if (kingdom.RulingClan == town.Settlement.OwnerClan)
					{
						explainedNumber.Add(1f, DefaultPolicies.ImperialTowns.Name, null);
					}
					else
					{
						explainedNumber.Add(-0.3f, DefaultPolicies.ImperialTowns.Name, null);
					}
				}
				if (kingdom.ActivePolicies.Contains(DefaultPolicies.ForgivenessOfDebts))
				{
					explainedNumber.Add(2f, DefaultPolicies.ForgivenessOfDebts.Name, null);
				}
				if (kingdom.ActivePolicies.Contains(DefaultPolicies.TribunesOfThePeople))
				{
					explainedNumber.Add(1f, DefaultPolicies.TribunesOfThePeople.Name, null);
				}
				if (kingdom.ActivePolicies.Contains(DefaultPolicies.DebasementOfTheCurrency))
				{
					explainedNumber.Add(-1f, DefaultPolicies.DebasementOfTheCurrency.Name, null);
				}
			}
		}

		private void GetSettlementLoyaltyChangeDueToGovernorCulture(Town town, ref ExplainedNumber explainedNumber)
		{
			if (town.Governor != null)
			{
				explainedNumber.Add((town.Governor.Culture == town.Culture) ? this.GovernorSameCultureLoyaltyEffect : this.GovernorDifferentCultureLoyaltyEffect, DefaultSettlementLoyaltyModel.GovernorCultureText, null);
			}
		}

		private void GetSettlementLoyaltyChangeDueToFoodStocks(Town town, ref ExplainedNumber explainedNumber)
		{
			if (town.Settlement.IsStarving)
			{
				float num = -1f;
				if (town.Settlement.Party.DaysStarving > 14f)
				{
					num += -1f;
				}
				explainedNumber.Add(num, DefaultSettlementLoyaltyModel.StarvingText, null);
			}
		}

		private void GetSettlementLoyaltyChangeDueToSecurity(Town town, ref ExplainedNumber explainedNumber)
		{
			float num = ((town.Security > (float)this.SettlementLoyaltyChangeDueToSecurityThreshold) ? MBMath.Map(town.Security, (float)this.SettlementLoyaltyChangeDueToSecurityThreshold, (float)this.MaximumLoyaltyInSettlement, 0f, this.HighSecurityLoyaltyEffect) : MBMath.Map(town.Security, 0f, (float)this.SettlementLoyaltyChangeDueToSecurityThreshold, this.LowSecurityLoyaltyEffect, 0f));
			explainedNumber.Add(num, DefaultSettlementLoyaltyModel.SecurityText, null);
		}

		private void GetSettlementLoyaltyChangeDueToProjects(Town town, ref ExplainedNumber explainedNumber)
		{
			if (town.BuildingsInProgress.IsEmpty<Building>())
			{
				BuildingHelper.AddDefaultDailyBonus(town, BuildingEffectEnum.LoyaltyDaily, ref explainedNumber);
			}
			foreach (Building building in town.Buildings)
			{
				float buildingEffectAmount = building.GetBuildingEffectAmount(BuildingEffectEnum.Loyalty);
				if (!building.BuildingType.IsDefaultProject && buildingEffectAmount > 0f)
				{
					explainedNumber.Add(buildingEffectAmount, building.Name, null);
				}
			}
		}

		private void GetSettlementLoyaltyChangeDueToIssues(Town town, ref ExplainedNumber explainedNumber)
		{
			Campaign.Current.Models.IssueModel.GetIssueEffectsOfSettlement(DefaultIssueEffects.SettlementLoyalty, town.Settlement, ref explainedNumber);
		}

		private void GetSettlementLoyaltyChangeDueToLoyaltyDrift(Town town, ref ExplainedNumber explainedNumber)
		{
			explainedNumber.Add(-1f * (town.Loyalty - (float)this.LoyaltyDriftMedium) * 0.1f, DefaultSettlementLoyaltyModel.LoyaltyDriftText, null);
		}

		private const float StarvationLoyaltyEffect = -1f;

		private const int AdditionalStarvationLoyaltyEffectAfterDays = 14;

		private const float NotableSupportsOwnerLoyaltyEffect = 0.5f;

		private const float NotableSupportsEnemyLoyaltyEffect = -0.5f;

		private static readonly TextObject StarvingText = GameTexts.FindText("str_starving", null);

		private static readonly TextObject CultureText = new TextObject("{=YjoXyFDX}Owner Culture", null);

		private static readonly TextObject NotableText = GameTexts.FindText("str_notable_relations", null);

		private static readonly TextObject CrimeText = GameTexts.FindText("str_governor_criminal", null);

		private static readonly TextObject GovernorText = GameTexts.FindText("str_notable_governor", null);

		private static readonly TextObject GovernorCultureText = new TextObject("{=5Vo8dJub}Governor's Culture", null);

		private static readonly TextObject NoGovernorText = new TextObject("{=NH5N3kP5}No governor", null);

		private static readonly TextObject SecurityText = GameTexts.FindText("str_security", null);

		private static readonly TextObject LoyaltyText = GameTexts.FindText("str_loyalty", null);

		private static readonly TextObject LoyaltyDriftText = GameTexts.FindText("str_loyalty_drift", null);

		private static readonly TextObject CorruptionText = GameTexts.FindText("str_corruption", null);
	}
}
