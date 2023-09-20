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
	// Token: 0x02000138 RID: 312
	public class DefaultSettlementLoyaltyModel : SettlementLoyaltyModel
	{
		// Token: 0x1700061E RID: 1566
		// (get) Token: 0x0600171B RID: 5915 RVA: 0x0007232D File Offset: 0x0007052D
		public override float HighLoyaltyProsperityEffect
		{
			get
			{
				return 0.5f;
			}
		}

		// Token: 0x1700061F RID: 1567
		// (get) Token: 0x0600171C RID: 5916 RVA: 0x00072334 File Offset: 0x00070534
		public override int LowLoyaltyProsperityEffect
		{
			get
			{
				return -1;
			}
		}

		// Token: 0x17000620 RID: 1568
		// (get) Token: 0x0600171D RID: 5917 RVA: 0x00072337 File Offset: 0x00070537
		public override int ThresholdForTaxBoost
		{
			get
			{
				return 75;
			}
		}

		// Token: 0x17000621 RID: 1569
		// (get) Token: 0x0600171E RID: 5918 RVA: 0x0007233B File Offset: 0x0007053B
		public override int ThresholdForTaxCorruption
		{
			get
			{
				return 50;
			}
		}

		// Token: 0x17000622 RID: 1570
		// (get) Token: 0x0600171F RID: 5919 RVA: 0x0007233F File Offset: 0x0007053F
		public override int ThresholdForHigherTaxCorruption
		{
			get
			{
				return 25;
			}
		}

		// Token: 0x17000623 RID: 1571
		// (get) Token: 0x06001720 RID: 5920 RVA: 0x00072343 File Offset: 0x00070543
		public override int ThresholdForProsperityBoost
		{
			get
			{
				return 75;
			}
		}

		// Token: 0x17000624 RID: 1572
		// (get) Token: 0x06001721 RID: 5921 RVA: 0x00072347 File Offset: 0x00070547
		public override int ThresholdForProsperityPenalty
		{
			get
			{
				return 25;
			}
		}

		// Token: 0x17000625 RID: 1573
		// (get) Token: 0x06001722 RID: 5922 RVA: 0x0007234B File Offset: 0x0007054B
		public override int AdditionalStarvationPenaltyStartDay
		{
			get
			{
				return 14;
			}
		}

		// Token: 0x17000626 RID: 1574
		// (get) Token: 0x06001723 RID: 5923 RVA: 0x0007234F File Offset: 0x0007054F
		public override int AdditionalStarvationLoyaltyEffect
		{
			get
			{
				return -1;
			}
		}

		// Token: 0x17000627 RID: 1575
		// (get) Token: 0x06001724 RID: 5924 RVA: 0x00072352 File Offset: 0x00070552
		public override int RebellionStartLoyaltyThreshold
		{
			get
			{
				return 15;
			}
		}

		// Token: 0x17000628 RID: 1576
		// (get) Token: 0x06001725 RID: 5925 RVA: 0x00072356 File Offset: 0x00070556
		public override int RebelliousStateStartLoyaltyThreshold
		{
			get
			{
				return 25;
			}
		}

		// Token: 0x17000629 RID: 1577
		// (get) Token: 0x06001726 RID: 5926 RVA: 0x0007235A File Offset: 0x0007055A
		public override int LoyaltyBoostAfterRebellionStartValue
		{
			get
			{
				return 5;
			}
		}

		// Token: 0x1700062A RID: 1578
		// (get) Token: 0x06001727 RID: 5927 RVA: 0x0007235D File Offset: 0x0007055D
		public override int MilitiaBoostPercentage
		{
			get
			{
				return 200;
			}
		}

		// Token: 0x1700062B RID: 1579
		// (get) Token: 0x06001728 RID: 5928 RVA: 0x00072364 File Offset: 0x00070564
		public override float ThresholdForNotableRelationBonus
		{
			get
			{
				return 75f;
			}
		}

		// Token: 0x1700062C RID: 1580
		// (get) Token: 0x06001729 RID: 5929 RVA: 0x0007236B File Offset: 0x0007056B
		public override int DailyNotableRelationBonus
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x1700062D RID: 1581
		// (get) Token: 0x0600172A RID: 5930 RVA: 0x0007236E File Offset: 0x0007056E
		public override int SettlementLoyaltyChangeDueToSecurityThreshold
		{
			get
			{
				return 50;
			}
		}

		// Token: 0x1700062E RID: 1582
		// (get) Token: 0x0600172B RID: 5931 RVA: 0x00072372 File Offset: 0x00070572
		public override int MaximumLoyaltyInSettlement
		{
			get
			{
				return 100;
			}
		}

		// Token: 0x1700062F RID: 1583
		// (get) Token: 0x0600172C RID: 5932 RVA: 0x00072376 File Offset: 0x00070576
		public override int LoyaltyDriftMedium
		{
			get
			{
				return 50;
			}
		}

		// Token: 0x17000630 RID: 1584
		// (get) Token: 0x0600172D RID: 5933 RVA: 0x0007237A File Offset: 0x0007057A
		public override float HighSecurityLoyaltyEffect
		{
			get
			{
				return 1f;
			}
		}

		// Token: 0x17000631 RID: 1585
		// (get) Token: 0x0600172E RID: 5934 RVA: 0x00072381 File Offset: 0x00070581
		public override float LowSecurityLoyaltyEffect
		{
			get
			{
				return -2f;
			}
		}

		// Token: 0x17000632 RID: 1586
		// (get) Token: 0x0600172F RID: 5935 RVA: 0x00072388 File Offset: 0x00070588
		public override float GovernorSameCultureLoyaltyEffect
		{
			get
			{
				return 1f;
			}
		}

		// Token: 0x17000633 RID: 1587
		// (get) Token: 0x06001730 RID: 5936 RVA: 0x0007238F File Offset: 0x0007058F
		public override float GovernorDifferentCultureLoyaltyEffect
		{
			get
			{
				return -1f;
			}
		}

		// Token: 0x17000634 RID: 1588
		// (get) Token: 0x06001731 RID: 5937 RVA: 0x00072396 File Offset: 0x00070596
		public override float SettlementOwnerDifferentCultureLoyaltyEffect
		{
			get
			{
				return -3f;
			}
		}

		// Token: 0x06001732 RID: 5938 RVA: 0x0007239D File Offset: 0x0007059D
		public override ExplainedNumber CalculateLoyaltyChange(Town town, bool includeDescriptions = false)
		{
			return this.CalculateLoyaltyChangeInternal(town, includeDescriptions);
		}

		// Token: 0x06001733 RID: 5939 RVA: 0x000723A8 File Offset: 0x000705A8
		public override void CalculateGoldGainDueToHighLoyalty(Town town, ref ExplainedNumber explainedNumber)
		{
			float num = MBMath.Map(town.Loyalty, (float)this.ThresholdForTaxBoost, 100f, 0f, 20f);
			explainedNumber.AddFactor(num * 0.01f, DefaultSettlementLoyaltyModel.LoyaltyText);
		}

		// Token: 0x06001734 RID: 5940 RVA: 0x000723EC File Offset: 0x000705EC
		public override void CalculateGoldCutDueToLowLoyalty(Town town, ref ExplainedNumber explainedNumber)
		{
			float num = MBMath.Map(town.Loyalty, (float)this.ThresholdForHigherTaxCorruption, (float)this.ThresholdForTaxCorruption, 50f, 0f);
			explainedNumber.AddFactor(-1f * num * 0.01f, DefaultSettlementLoyaltyModel.CorruptionText);
		}

		// Token: 0x06001735 RID: 5941 RVA: 0x00072438 File Offset: 0x00070638
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

		// Token: 0x06001736 RID: 5942 RVA: 0x000724B0 File Offset: 0x000706B0
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

		// Token: 0x06001737 RID: 5943 RVA: 0x00072648 File Offset: 0x00070848
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

		// Token: 0x06001738 RID: 5944 RVA: 0x00072704 File Offset: 0x00070904
		private void GetSettlementLoyaltyChangeDueToOwnerCulture(Town town, ref ExplainedNumber explainedNumber)
		{
			if (town.Settlement.OwnerClan.Culture != town.Settlement.Culture)
			{
				explainedNumber.Add(this.SettlementOwnerDifferentCultureLoyaltyEffect, DefaultSettlementLoyaltyModel.CultureText, null);
			}
		}

		// Token: 0x06001739 RID: 5945 RVA: 0x00072738 File Offset: 0x00070938
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

		// Token: 0x0600173A RID: 5946 RVA: 0x00072901 File Offset: 0x00070B01
		private void GetSettlementLoyaltyChangeDueToGovernorCulture(Town town, ref ExplainedNumber explainedNumber)
		{
			if (town.Governor != null)
			{
				explainedNumber.Add((town.Governor.Culture == town.Culture) ? this.GovernorSameCultureLoyaltyEffect : this.GovernorDifferentCultureLoyaltyEffect, DefaultSettlementLoyaltyModel.GovernorCultureText, null);
			}
		}

		// Token: 0x0600173B RID: 5947 RVA: 0x00072938 File Offset: 0x00070B38
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

		// Token: 0x0600173C RID: 5948 RVA: 0x00072984 File Offset: 0x00070B84
		private void GetSettlementLoyaltyChangeDueToSecurity(Town town, ref ExplainedNumber explainedNumber)
		{
			float num = ((town.Security > (float)this.SettlementLoyaltyChangeDueToSecurityThreshold) ? MBMath.Map(town.Security, (float)this.SettlementLoyaltyChangeDueToSecurityThreshold, (float)this.MaximumLoyaltyInSettlement, 0f, this.HighSecurityLoyaltyEffect) : MBMath.Map(town.Security, 0f, (float)this.SettlementLoyaltyChangeDueToSecurityThreshold, this.LowSecurityLoyaltyEffect, 0f));
			explainedNumber.Add(num, DefaultSettlementLoyaltyModel.SecurityText, null);
		}

		// Token: 0x0600173D RID: 5949 RVA: 0x000729FC File Offset: 0x00070BFC
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

		// Token: 0x0600173E RID: 5950 RVA: 0x00072A8C File Offset: 0x00070C8C
		private void GetSettlementLoyaltyChangeDueToIssues(Town town, ref ExplainedNumber explainedNumber)
		{
			Campaign.Current.Models.IssueModel.GetIssueEffectsOfSettlement(DefaultIssueEffects.SettlementLoyalty, town.Settlement, ref explainedNumber);
		}

		// Token: 0x0600173F RID: 5951 RVA: 0x00072AAE File Offset: 0x00070CAE
		private void GetSettlementLoyaltyChangeDueToLoyaltyDrift(Town town, ref ExplainedNumber explainedNumber)
		{
			explainedNumber.Add(-1f * (town.Loyalty - (float)this.LoyaltyDriftMedium) * 0.1f, DefaultSettlementLoyaltyModel.LoyaltyDriftText, null);
		}

		// Token: 0x04000834 RID: 2100
		private const float StarvationLoyaltyEffect = -1f;

		// Token: 0x04000835 RID: 2101
		private const int AdditionalStarvationLoyaltyEffectAfterDays = 14;

		// Token: 0x04000836 RID: 2102
		private const float NotableSupportsOwnerLoyaltyEffect = 0.5f;

		// Token: 0x04000837 RID: 2103
		private const float NotableSupportsEnemyLoyaltyEffect = -0.5f;

		// Token: 0x04000838 RID: 2104
		private static readonly TextObject StarvingText = GameTexts.FindText("str_starving", null);

		// Token: 0x04000839 RID: 2105
		private static readonly TextObject CultureText = new TextObject("{=YjoXyFDX}Owner Culture", null);

		// Token: 0x0400083A RID: 2106
		private static readonly TextObject NotableText = GameTexts.FindText("str_notable_relations", null);

		// Token: 0x0400083B RID: 2107
		private static readonly TextObject CrimeText = GameTexts.FindText("str_governor_criminal", null);

		// Token: 0x0400083C RID: 2108
		private static readonly TextObject GovernorText = GameTexts.FindText("str_notable_governor", null);

		// Token: 0x0400083D RID: 2109
		private static readonly TextObject GovernorCultureText = new TextObject("{=5Vo8dJub}Governor's Culture", null);

		// Token: 0x0400083E RID: 2110
		private static readonly TextObject NoGovernorText = new TextObject("{=NH5N3kP5}No governor", null);

		// Token: 0x0400083F RID: 2111
		private static readonly TextObject SecurityText = GameTexts.FindText("str_security", null);

		// Token: 0x04000840 RID: 2112
		private static readonly TextObject LoyaltyText = GameTexts.FindText("str_loyalty", null);

		// Token: 0x04000841 RID: 2113
		private static readonly TextObject LoyaltyDriftText = GameTexts.FindText("str_loyalty_drift", null);

		// Token: 0x04000842 RID: 2114
		private static readonly TextObject CorruptionText = GameTexts.FindText("str_corruption", null);
	}
}
