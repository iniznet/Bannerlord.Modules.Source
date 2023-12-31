﻿using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Party.PartyComponents;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Siege;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.GameComponents
{
	public class DefaultArmyManagementCalculationModel : ArmyManagementCalculationModel
	{
		public override int InfluenceValuePerGold
		{
			get
			{
				return 40;
			}
		}

		public override int AverageCallToArmyCost
		{
			get
			{
				return 20;
			}
		}

		public override int CohesionThresholdForDispersion
		{
			get
			{
				return 10;
			}
		}

		public override float DailyBeingAtArmyInfluenceAward(MobileParty armyMemberParty)
		{
			float num = (armyMemberParty.Party.TotalStrength + 20f) / 200f;
			if (PartyBaseHelper.HasFeat(armyMemberParty.Party, DefaultCulturalFeats.EmpireArmyInfluenceFeat))
			{
				num += num * DefaultCulturalFeats.EmpireArmyInfluenceFeat.EffectBonus;
			}
			return num;
		}

		public override int CalculatePartyInfluenceCost(MobileParty armyLeaderParty, MobileParty party)
		{
			if (armyLeaderParty.LeaderHero != null && party.LeaderHero != null && armyLeaderParty.LeaderHero.Clan == party.LeaderHero.Clan)
			{
				return 0;
			}
			float num = (float)armyLeaderParty.LeaderHero.GetRelation(party.LeaderHero);
			float partySizeScore = this.GetPartySizeScore(party);
			float num2 = (float)MathF.Round(party.Party.TotalStrength);
			float num3 = ((num < 0f) ? (1f + MathF.Sqrt(MathF.Abs(MathF.Max(-100f, num))) / 10f) : (1f - MathF.Sqrt(MathF.Abs(MathF.Min(100f, num))) / 20f));
			float num4 = 0.5f + MathF.Min(1000f, num2) / 100f;
			float num5 = 0.5f + 1f * (1f - (partySizeScore - this._minimumPartySizeScoreNeeded) / (1f - this._minimumPartySizeScoreNeeded));
			float num6 = 1f + 1f * MathF.Pow(MathF.Min(Campaign.MapDiagonal * 10f, MathF.Max(1f, Campaign.Current.Models.MapDistanceModel.GetDistance(armyLeaderParty, party)) / Campaign.MapDiagonal), 0.67f);
			float num7 = ((party.LeaderHero != null) ? party.LeaderHero.RandomFloat(0.75f, 1.25f) : 1f);
			float num8 = 1f;
			float num9 = 1f;
			float num10 = 1f;
			Hero leaderHero = armyLeaderParty.LeaderHero;
			if (((leaderHero != null) ? leaderHero.Clan.Kingdom : null) != null)
			{
				if (armyLeaderParty.LeaderHero.Clan.Tier >= 5 && armyLeaderParty.LeaderHero.Clan.Kingdom.ActivePolicies.Contains(DefaultPolicies.Marshals))
				{
					num8 -= 0.1f;
				}
				if (armyLeaderParty.LeaderHero.Clan.Kingdom.ActivePolicies.Contains(DefaultPolicies.RoyalCommissions))
				{
					if (armyLeaderParty.LeaderHero == armyLeaderParty.LeaderHero.Clan.Kingdom.Leader)
					{
						num8 -= 0.3f;
					}
					else
					{
						num8 += 0.1f;
					}
				}
				if (party.LeaderHero != null)
				{
					if (armyLeaderParty.LeaderHero.Clan.Kingdom.ActivePolicies.Contains(DefaultPolicies.LordsPrivyCouncil) && party.LeaderHero.Clan.Tier <= 4)
					{
						num8 += 0.2f;
					}
					if (armyLeaderParty.LeaderHero.Clan.Kingdom.ActivePolicies.Contains(DefaultPolicies.Senate) && party.LeaderHero.Clan.Tier <= 2)
					{
						num8 += 0.1f;
					}
				}
				if (armyLeaderParty.LeaderHero.GetPerkValue(DefaultPerks.Leadership.InspiringLeader))
				{
					num9 += DefaultPerks.Leadership.InspiringLeader.PrimaryBonus;
				}
				if (armyLeaderParty.LeaderHero.GetPerkValue(DefaultPerks.Tactics.CallToArms))
				{
					num9 += DefaultPerks.Tactics.CallToArms.SecondaryBonus;
				}
			}
			if (PartyBaseHelper.HasFeat(armyLeaderParty.Party, DefaultCulturalFeats.VlandianArmyInfluenceFeat))
			{
				num10 += DefaultCulturalFeats.VlandianArmyInfluenceFeat.EffectBonus;
			}
			return (int)(0.65f * num3 * num4 * num7 * num6 * num5 * num8 * num9 * num10 * (float)this.AverageCallToArmyCost);
		}

		public override List<MobileParty> GetMobilePartiesToCallToArmy(MobileParty leaderParty)
		{
			List<MobileParty> list = new List<MobileParty>();
			bool flag = false;
			bool flag2 = false;
			if (leaderParty.LeaderHero != null)
			{
				foreach (Settlement settlement in leaderParty.MapFaction.Settlements)
				{
					if (settlement.IsFortification && settlement.SiegeEvent != null)
					{
						flag = true;
						if (settlement.OwnerClan == leaderParty.LeaderHero.Clan)
						{
							flag2 = true;
						}
					}
				}
			}
			int num = ((leaderParty.MapFaction.IsKingdomFaction && (Kingdom)leaderParty.MapFaction != null) ? ((Kingdom)leaderParty.MapFaction).Armies.Count : 0);
			float num2 = (0.55f - (float)MathF.Min(2, num) * 0.05f - ((Hero.MainHero.MapFaction == leaderParty.MapFaction) ? 0.05f : 0f)) * (1f - 0.5f * MathF.Sqrt(MathF.Min(leaderParty.LeaderHero.Clan.Influence, 900f)) * 0.033333335f);
			num2 *= (flag2 ? 1.25f : 1f);
			num2 *= (flag ? 1.125f : 1f);
			num2 *= leaderParty.LeaderHero.RandomFloat(0.85f, 1f);
			float num3 = MathF.Min(leaderParty.LeaderHero.Clan.Influence, 900f) * MathF.Min(1f, num2);
			List<ValueTuple<MobileParty, float>> list2 = new List<ValueTuple<MobileParty, float>>();
			foreach (WarPartyComponent warPartyComponent in leaderParty.MapFaction.WarPartyComponents)
			{
				MobileParty mobileParty = warPartyComponent.MobileParty;
				Hero leaderHero = mobileParty.LeaderHero;
				if (mobileParty.IsLordParty && mobileParty.Army == null && mobileParty != leaderParty && leaderHero != null && !mobileParty.IsMainParty && leaderHero != leaderHero.MapFaction.Leader && !mobileParty.Ai.DoNotMakeNewDecisions)
				{
					Settlement currentSettlement = mobileParty.CurrentSettlement;
					if (((currentSettlement != null) ? currentSettlement.SiegeEvent : null) == null && !mobileParty.IsDisbanding && mobileParty.Food > -(mobileParty.FoodChange * 5f) && mobileParty.PartySizeRatio > 0.6f && leaderHero.CanLeadParty() && mobileParty.MapEvent == null && mobileParty.BesiegedSettlement == null)
					{
						IDisbandPartyCampaignBehavior campaignBehavior = Campaign.Current.GetCampaignBehavior<IDisbandPartyCampaignBehavior>();
						if (campaignBehavior == null || !campaignBehavior.IsPartyWaitingForDisband(mobileParty))
						{
							bool flag3 = false;
							using (List<ValueTuple<MobileParty, float>>.Enumerator enumerator3 = list2.GetEnumerator())
							{
								while (enumerator3.MoveNext())
								{
									if (enumerator3.Current.Item1 == mobileParty)
									{
										flag3 = true;
										break;
									}
								}
							}
							if (!flag3)
							{
								int num4 = Campaign.Current.Models.ArmyManagementCalculationModel.CalculatePartyInfluenceCost(leaderParty, mobileParty);
								float totalStrength = mobileParty.Party.TotalStrength;
								float num5 = 1f - (float)mobileParty.Party.MemberRoster.TotalWounded / (float)mobileParty.Party.MemberRoster.TotalManCount;
								float num6 = totalStrength / ((float)num4 + 0.1f) * num5;
								list2.Add(new ValueTuple<MobileParty, float>(mobileParty, num6));
							}
						}
					}
				}
			}
			int num8;
			do
			{
				float num7 = 0.01f;
				num8 = -1;
				for (int i = 0; i < list2.Count; i++)
				{
					ValueTuple<MobileParty, float> valueTuple = list2[i];
					if (valueTuple.Item2 > num7)
					{
						num8 = i;
						num7 = valueTuple.Item2;
					}
				}
				if (num8 >= 0)
				{
					MobileParty item = list2[num8].Item1;
					int num9 = Campaign.Current.Models.ArmyManagementCalculationModel.CalculatePartyInfluenceCost(leaderParty, item);
					list2[num8] = new ValueTuple<MobileParty, float>(item, 0f);
					if (num3 > (float)num9)
					{
						num3 -= (float)num9;
						list.Add(item);
					}
				}
			}
			while (num8 >= 0);
			return list;
		}

		public override int CalculateTotalInfluenceCost(Army army, float percentage)
		{
			int num = 0;
			foreach (MobileParty mobileParty in army.Parties.Where((MobileParty p) => !p.IsMainParty))
			{
				num += this.CalculatePartyInfluenceCost(army.LeaderParty, mobileParty);
			}
			ExplainedNumber explainedNumber = new ExplainedNumber((float)num, false, null);
			if (army.LeaderParty.MapFaction.IsKingdomFaction && ((Kingdom)army.LeaderParty.MapFaction).ActivePolicies.Contains(DefaultPolicies.RoyalCommissions))
			{
				explainedNumber.AddFactor(-0.3f, null);
			}
			if (army.LeaderParty.LeaderHero.GetPerkValue(DefaultPerks.Tactics.Encirclement))
			{
				explainedNumber.AddFactor(DefaultPerks.Tactics.Encirclement.SecondaryBonus, null);
			}
			return MathF.Ceiling(explainedNumber.ResultNumber * percentage / 100f);
		}

		public override float GetPartySizeScore(MobileParty party)
		{
			return MathF.Min(1f, party.PartySizeRatio);
		}

		public override ExplainedNumber CalculateDailyCohesionChange(Army army, bool includeDescriptions = false)
		{
			ExplainedNumber explainedNumber = new ExplainedNumber(-2f, includeDescriptions, null);
			this.CalculateCohesionChangeInternal(army, ref explainedNumber);
			if (army.LeaderParty.HasPerk(DefaultPerks.Tactics.HordeLeader, true))
			{
				explainedNumber.AddFactor(DefaultPerks.Tactics.HordeLeader.SecondaryBonus, DefaultPerks.Tactics.HordeLeader.Name);
			}
			SiegeEvent siegeEvent = army.LeaderParty.SiegeEvent;
			if (siegeEvent != null && siegeEvent.BesiegerCamp.IsBesiegerSideParty(army.LeaderParty) && army.LeaderParty.HasPerk(DefaultPerks.Engineering.CampBuilding, false))
			{
				explainedNumber.AddFactor(DefaultPerks.Engineering.CampBuilding.PrimaryBonus, DefaultPerks.Engineering.CampBuilding.Name);
			}
			MobileParty leaderParty = army.LeaderParty;
			if (PartyBaseHelper.HasFeat((leaderParty != null) ? leaderParty.Party : null, DefaultCulturalFeats.SturgianArmyCohesionFeat))
			{
				explainedNumber.AddFactor(DefaultCulturalFeats.SturgianArmyCohesionFeat.EffectBonus, GameTexts.FindText("str_culture", null));
			}
			return explainedNumber;
		}

		private void CalculateCohesionChangeInternal(Army army, ref ExplainedNumber cohesionChange)
		{
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			foreach (MobileParty mobileParty in army.LeaderParty.AttachedParties)
			{
				if (mobileParty.Party.IsStarving)
				{
					num++;
				}
				if (mobileParty.Morale <= 25f)
				{
					num2++;
				}
				if (mobileParty.Party.NumberOfHealthyMembers <= 10)
				{
					num3++;
				}
				num4++;
			}
			cohesionChange.Add((float)(-(float)num4), DefaultArmyManagementCalculationModel._numberOfPartiesText, null);
			cohesionChange.Add((float)(-(float)((num + 1) / 2)), DefaultArmyManagementCalculationModel._numberOfStarvingPartiesText, null);
			cohesionChange.Add((float)(-(float)((num2 + 1) / 2)), DefaultArmyManagementCalculationModel._numberOfLowMoralePartiesText, null);
			cohesionChange.Add((float)(-(float)((num3 + 1) / 2)), DefaultArmyManagementCalculationModel._numberOfLessMemberPartiesText, null);
		}

		public override int CalculateNewCohesion(Army army, PartyBase newParty, int calculatedCohesion, int sign)
		{
			if (army == null)
			{
				return calculatedCohesion;
			}
			sign = MathF.Sign(sign);
			int num = ((sign == 1) ? (army.Parties.Count - 1) : army.Parties.Count);
			int num2 = (calculatedCohesion * num + 100 * sign) / (num + sign);
			if (num2 > 100)
			{
				return 100;
			}
			if (num2 >= 0)
			{
				return num2;
			}
			return 0;
		}

		public override int GetCohesionBoostInfluenceCost(Army army, int percentageToBoost = 100)
		{
			return this.CalculateTotalInfluenceCost(army, (float)percentageToBoost);
		}

		public override int GetCohesionBoostGoldCost(Army army, float percentageToBoost = 100f)
		{
			return this.CalculateTotalInfluenceCost(army, percentageToBoost) * this.InfluenceValuePerGold;
		}

		public override int GetPartyRelation(Hero hero)
		{
			if (hero == null)
			{
				return -101;
			}
			if (hero == Hero.MainHero)
			{
				return 101;
			}
			return Hero.MainHero.GetRelation(hero);
		}

		public override int GetPartyStrength(PartyBase party)
		{
			return MathF.Round(party.TotalStrength);
		}

		public override bool CheckPartyEligibility(MobileParty party)
		{
			return party.Army == null && this.GetPartySizeScore(party) > this._minimumPartySizeScoreNeeded && party.MapEvent == null && party.SiegeEvent == null;
		}

		private const float MobilePartySizeRatioToCallToArmy = 0.6f;

		private const float MinimumNeededFoodInDaysToCallToArmy = 5f;

		private static readonly TextObject _numberOfPartiesText = GameTexts.FindText("str_number_of_parties", null);

		private static readonly TextObject _numberOfStarvingPartiesText = GameTexts.FindText("str_number_of_starving_parties", null);

		private static readonly TextObject _numberOfLowMoralePartiesText = GameTexts.FindText("str_number_of_low_morale_parties", null);

		private static readonly TextObject _numberOfLessMemberPartiesText = GameTexts.FindText("str_number_of_less_member_parties", null);

		private float _minimumPartySizeScoreNeeded = 0.4f;
	}
}
