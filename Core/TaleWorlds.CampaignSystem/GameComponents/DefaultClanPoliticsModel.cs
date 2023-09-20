using System;
using System.Linq;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Election;
using TaleWorlds.CampaignSystem.Issues;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Party.PartyComponents;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Buildings;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.GameComponents
{
	public class DefaultClanPoliticsModel : ClanPoliticsModel
	{
		public override ExplainedNumber CalculateInfluenceChange(Clan clan, bool includeDescriptions = false)
		{
			ExplainedNumber explainedNumber = new ExplainedNumber(0f, includeDescriptions, null);
			this.CalculateInfluenceChangeInternal(clan, ref explainedNumber);
			return explainedNumber;
		}

		private void CalculateInfluenceChangeInternal(Clan clan, ref ExplainedNumber influenceChange)
		{
			if (clan.Leader.GetPerkValue(DefaultPerks.Charm.ImmortalCharm))
			{
				influenceChange.Add(DefaultPerks.Charm.ImmortalCharm.PrimaryBonus, DefaultPerks.Charm.ImmortalCharm.Name, null);
			}
			if (clan.IsUnderMercenaryService)
			{
				int num = MathF.Ceiling(clan.Influence * (1f / Campaign.Current.Models.ClanFinanceModel.RevenueSmoothenFraction()));
				influenceChange.Add((float)(-(float)num), DefaultClanPoliticsModel._mercenaryStr, null);
			}
			float num2 = 0f;
			foreach (WarPartyComponent warPartyComponent in clan.WarPartyComponents)
			{
				MobileParty mobileParty = warPartyComponent.MobileParty;
				if (mobileParty.Army != null && mobileParty.Army.LeaderParty != mobileParty && mobileParty.LeaderHero != null)
				{
					num2 += Campaign.Current.Models.ArmyManagementCalculationModel.DailyBeingAtArmyInfluenceAward(mobileParty);
				}
			}
			influenceChange.Add(num2, DefaultClanPoliticsModel._armyMemberStr, null);
			if (clan.MapFaction.Leader == clan.Leader && clan.MapFaction.IsKingdomFaction)
			{
				influenceChange.Add(3f, DefaultClanPoliticsModel._kingBonusStr, null);
			}
			float num3 = 0f;
			foreach (Settlement settlement in clan.Settlements)
			{
				if (settlement.IsTown)
				{
					foreach (Building building in settlement.Town.Buildings)
					{
						num3 += building.GetBuildingEffectAmount(BuildingEffectEnum.Influence);
					}
				}
			}
			if (num3 > 0f)
			{
				influenceChange.Add(num3, DefaultClanPoliticsModel._townProjectStr, null);
			}
			if (clan == Clan.PlayerClan && clan.MapFaction.MainHeroCrimeRating > 0f)
			{
				int num4 = (int)(clan.MapFaction.MainHeroCrimeRating * -0.5f);
				influenceChange.Add((float)num4, DefaultClanPoliticsModel._crimeStr, null);
			}
			float num5 = 0f;
			foreach (Hero hero in clan.SupporterNotables)
			{
				if (hero.CurrentSettlement != null)
				{
					float influenceBonusToClan = Campaign.Current.Models.NotablePowerModel.GetInfluenceBonusToClan(hero);
					num5 += influenceBonusToClan;
				}
			}
			if (num5 > 0f)
			{
				influenceChange.Add(num5, DefaultClanPoliticsModel._supporterStr, null);
			}
			if (clan.Kingdom != null && !clan.IsUnderMercenaryService)
			{
				this.CalculateInfluenceChangeDueToPolicies(clan, ref influenceChange);
			}
			this.CalculateInfluenceChangeDueToIssues(clan, ref influenceChange);
		}

		private void CalculateInfluenceChangeDueToIssues(Clan clan, ref ExplainedNumber influenceChange)
		{
			Campaign.Current.Models.IssueModel.GetIssueEffectOfClan(DefaultIssueEffects.ClanInfluence, clan, ref influenceChange);
		}

		private void CalculateInfluenceChangeDueToPolicies(Clan clan, ref ExplainedNumber influenceChange)
		{
			if (clan.Kingdom.ActivePolicies.Contains(DefaultPolicies.FeudalInheritance))
			{
				influenceChange.Add(0.1f * (float)clan.Settlements.Count, DefaultPolicies.FeudalInheritance.Name, null);
			}
			if (clan.Kingdom.ActivePolicies.Contains(DefaultPolicies.Serfdom))
			{
				influenceChange.Add(0.2f * (float)clan.Settlements.Count((Settlement t) => t.IsVillage), DefaultPolicies.Serfdom.Name, null);
			}
			if (clan.Kingdom.ActivePolicies.Contains(DefaultPolicies.LordsPrivyCouncil) && clan.Tier >= 5)
			{
				influenceChange.Add(0.5f, DefaultPolicies.LordsPrivyCouncil.Name, null);
			}
			if (clan.Kingdom.ActivePolicies.Contains(DefaultPolicies.Senate) && clan.Tier >= 3)
			{
				influenceChange.Add(0.5f, DefaultPolicies.Senate.Name, null);
			}
			if (clan.Kingdom.ActivePolicies.Contains(DefaultPolicies.NobleRetinues) && clan.Tier >= 5)
			{
				influenceChange.Add(-1f, DefaultPolicies.NobleRetinues.Name, null);
			}
			if (clan.Kingdom.ActivePolicies.Contains(DefaultPolicies.Bailiffs))
			{
				int num = clan.Settlements.Count((Settlement settlement) => settlement.IsTown && settlement.Town.Security > 60f);
				influenceChange.Add((float)num, DefaultPolicies.Bailiffs.Name, null);
			}
			if (clan.Kingdom.ActivePolicies.Contains(DefaultPolicies.CouncilOfTheCommons))
			{
				float num2 = 0f;
				foreach (Settlement settlement2 in clan.Settlements)
				{
					num2 += (float)settlement2.Notables.Count * 0.1f;
				}
				influenceChange.Add(num2, DefaultPolicies.CouncilOfTheCommons.Name, null);
			}
			if (clan.Kingdom.ActivePolicies.Contains(DefaultPolicies.TrialByJury))
			{
				influenceChange.Add(-1f, DefaultPolicies.TrialByJury.Name, null);
			}
			if (clan.Kingdom.ActivePolicies.Contains(DefaultPolicies.Lawspeakers))
			{
				float num3 = ((clan.Leader.GetSkillValue(DefaultSkills.Charm) > 100) ? 0.5f : (-0.5f));
				influenceChange.Add(num3, DefaultPolicies.Lawspeakers.Name, null);
			}
			if (clan == clan.Kingdom.RulingClan)
			{
				if (clan.Kingdom.ActivePolicies.Contains(DefaultPolicies.SacredMajesty))
				{
					influenceChange.Add(3f, DefaultPolicies.SacredMajesty.Name, null);
				}
				if (clan.Kingdom.ActivePolicies.Contains(DefaultPolicies.Marshals))
				{
					influenceChange.Add(-1f, DefaultPolicies.Marshals.Name, null);
					return;
				}
			}
			else
			{
				if (clan.Kingdom.ActivePolicies.Contains(DefaultPolicies.SacredMajesty))
				{
					influenceChange.Add(-0.5f, DefaultPolicies.SacredMajesty.Name, null);
				}
				if (clan.Kingdom.ActivePolicies.Contains(DefaultPolicies.RoyalGuard))
				{
					influenceChange.Add(-0.2f, DefaultPolicies.RoyalGuard.Name, null);
				}
			}
		}

		public override float CalculateSupportForPolicyInClan(Clan clan, PolicyObject policy)
		{
			float num = 0f;
			float num2 = 1f;
			float num3 = (float)clan.Leader.GetTraitLevel(DefaultTraits.Authoritarian) * policy.AuthoritarianWeight * num2;
			float num4 = (float)clan.Leader.GetTraitLevel(DefaultTraits.Egalitarian) * policy.EgalitarianWeight * num2;
			float num5 = (float)clan.Leader.GetTraitLevel(DefaultTraits.Oligarchic) * policy.OligarchicWeight * num2;
			float num6;
			float num7;
			float num8;
			if (clan.Tier == 1)
			{
				num6 = policy.EgalitarianWeight;
				num7 = 0f;
				num8 = 0f;
			}
			else if (clan.Tier == 2)
			{
				num6 = policy.EgalitarianWeight;
				num7 = 0f;
				num8 = 0f;
			}
			else if (clan.Tier == 3)
			{
				num6 = policy.EgalitarianWeight;
				num7 = 0f;
				num8 = 0f;
			}
			else if (clan.Tier == 4)
			{
				num6 = 0f;
				num7 = policy.OligarchicWeight;
				num8 = 0f;
			}
			else if (clan.Tier == 5)
			{
				num6 = 0f;
				num7 = policy.OligarchicWeight;
				num8 = 0f;
			}
			else
			{
				num6 = 0f;
				num7 = policy.OligarchicWeight;
				num8 = 0f;
			}
			float num9 = 0f;
			if (clan.Kingdom.RulingClan == clan)
			{
				if (clan.Leader.GetTraitLevel(DefaultTraits.Oligarchic) > 0 || clan.Leader.GetTraitLevel(DefaultTraits.Egalitarian) > 0)
				{
					num9 = -0.5f;
				}
				else if (clan.Leader.GetTraitLevel(DefaultTraits.Authoritarian) > 0)
				{
					num9 = 1f;
				}
			}
			return MathF.Clamp(num + (num3 + num4 + num5 + num6 + num7 + num8 + num9), -2f, 2f);
		}

		public override float CalculateRelationshipChangeWithSponsor(Clan clan, Clan sponsorClan)
		{
			return MathF.Lerp(-2f, 2f, MathF.Clamp((float)clan.Leader.GetRelation(sponsorClan.Leader) / 60f, 0f, 1f), 1E-05f);
		}

		public override int GetInfluenceRequiredToOverrideKingdomDecision(DecisionOutcome popularOption, DecisionOutcome overridingOption, KingdomDecision decision)
		{
			float totalSupportPoints = popularOption.TotalSupportPoints;
			float num = overridingOption.TotalSupportPoints;
			float num2 = 0f;
			if (decision.Kingdom.RulingClan == Clan.PlayerClan)
			{
				if (totalSupportPoints == num + 1f)
				{
					num += 1f;
					num2 += (float)decision.GetInfluenceCostOfSupport(Clan.PlayerClan, Supporter.SupportWeights.SlightlyFavor);
				}
				else if (totalSupportPoints == num + 2f)
				{
					num += 2f;
					num2 += (float)decision.GetInfluenceCostOfSupport(Clan.PlayerClan, Supporter.SupportWeights.StronglyFavor);
				}
				else if (totalSupportPoints > num + 2f)
				{
					num += 3f;
					num2 += (float)decision.GetInfluenceCostOfSupport(Clan.PlayerClan, Supporter.SupportWeights.FullyPush);
				}
			}
			if (totalSupportPoints > num)
			{
				float num3 = (totalSupportPoints - num) / 3f * (float)decision.GetInfluenceCostOfSupport(decision.Kingdom.RulingClan, Supporter.SupportWeights.FullyPush) * 1.4f;
				if (decision.Kingdom.ActivePolicies.Contains(DefaultPolicies.RoyalPrivilege))
				{
					num3 *= 0.8f;
				}
				if (decision.Kingdom.RulingClan != Clan.PlayerClan)
				{
					num3 *= 0.8f;
				}
				num2 += num3;
			}
			num2 = (float)(5 * (int)(num2 / 5f));
			return (int)num2;
		}

		public override bool CanHeroBeGovernor(Hero hero)
		{
			return hero.IsActive && !hero.IsChild && !hero.IsHumanPlayerCharacter && !hero.IsPartyLeader && !hero.IsFugitive && !hero.IsReleased && !hero.IsTraveling && !hero.IsPrisoner && hero.CanBeGovernorOrHavePartyRole() && !hero.IsSpecial && !hero.IsTemplate;
		}

		private static readonly TextObject _supporterStr = new TextObject("{=RzFyGnWJ}Supporters", null);

		private static readonly TextObject _crimeStr = new TextObject("{=MvxW9rmf}Criminal", null);

		private static readonly TextObject _armyMemberStr = new TextObject("{=XAdBVsXV}Clan members in an army", null);

		private static readonly TextObject _townProjectStr = new TextObject("{=8Yb3IVvb}Settlement Buildings", null);

		private static readonly TextObject _courtshipPerkStr = new TextObject("{=zgzDwZKZ}Courtship from clan parties", null);

		private static readonly TextObject _mercenaryStr = new TextObject("{=qcaaJLhx}Mercenary Contract", null);

		private static readonly TextObject _kingBonusStr = new TextObject("{=JNS46jsG}King bonus", null);
	}
}
