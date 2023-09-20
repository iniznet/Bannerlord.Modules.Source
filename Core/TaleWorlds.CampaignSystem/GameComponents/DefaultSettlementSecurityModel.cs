using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Issues;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.GameComponents
{
	public class DefaultSettlementSecurityModel : SettlementSecurityModel
	{
		public override int MaximumSecurityInSettlement
		{
			get
			{
				return 100;
			}
		}

		public override int SecurityDriftMedium
		{
			get
			{
				return 50;
			}
		}

		public override float MapEventSecurityEffectRadius
		{
			get
			{
				return 50f;
			}
		}

		public override float HideoutClearedSecurityEffectRadius
		{
			get
			{
				return 100f;
			}
		}

		public override int HideoutClearedSecurityGain
		{
			get
			{
				return 6;
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
				return 0;
			}
		}

		public override int ThresholdForTaxBoost
		{
			get
			{
				return 75;
			}
		}

		public override int SettlementTaxBoostPercentage
		{
			get
			{
				return 5;
			}
		}

		public override int SettlementTaxPenaltyPercentage
		{
			get
			{
				return 10;
			}
		}

		public override int ThresholdForNotableRelationBonus
		{
			get
			{
				return 75;
			}
		}

		public override int ThresholdForNotableRelationPenalty
		{
			get
			{
				return 50;
			}
		}

		public override int DailyNotableRelationBonus
		{
			get
			{
				return 1;
			}
		}

		public override int DailyNotableRelationPenalty
		{
			get
			{
				return -1;
			}
		}

		public override int DailyNotablePowerBonus
		{
			get
			{
				return 1;
			}
		}

		public override int DailyNotablePowerPenalty
		{
			get
			{
				return -1;
			}
		}

		public override ExplainedNumber CalculateSecurityChange(Town town, bool includeDescriptions = false)
		{
			ExplainedNumber explainedNumber = new ExplainedNumber(0f, includeDescriptions, null);
			this.CalculateInfestedHideoutEffectsOnSecurity(town, ref explainedNumber);
			this.CalculateRaidedVillageEffectsOnSecurity(town, ref explainedNumber);
			this.CalculateUnderSiegeEffectsOnSecurity(town, ref explainedNumber);
			this.CalculateProsperityEffectOnSecurity(town, ref explainedNumber);
			this.CalculateGarrisonEffectsOnSecurity(town, ref explainedNumber);
			this.CalculatePolicyEffectsOnSecurity(town, ref explainedNumber);
			this.CalculateGovernorEffectsOnSecurity(town, ref explainedNumber);
			this.CalculateProjectEffectsOnSecurity(town, ref explainedNumber);
			this.CalculateIssueEffectsOnSecurity(town, ref explainedNumber);
			this.CalculatePerkEffectsOnSecurity(town, ref explainedNumber);
			this.CalculateSecurityDrift(town, ref explainedNumber);
			return explainedNumber;
		}

		private void CalculateProsperityEffectOnSecurity(Town town, ref ExplainedNumber explainedNumber)
		{
			explainedNumber.Add(MathF.Max(-5f, -0.0005f * town.Prosperity), DefaultSettlementSecurityModel.ProsperityText, null);
		}

		private void CalculateUnderSiegeEffectsOnSecurity(Town town, ref ExplainedNumber explainedNumber)
		{
			if (town.Settlement.IsUnderSiege)
			{
				explainedNumber.Add(-3f, DefaultSettlementSecurityModel.UnderSiegeText, null);
			}
		}

		private void CalculateRaidedVillageEffectsOnSecurity(Town town, ref ExplainedNumber explainedNumber)
		{
			float num = 0f;
			using (List<Village>.Enumerator enumerator = town.Settlement.BoundVillages.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.VillageState == Village.VillageStates.Looted)
					{
						num += -2f;
						break;
					}
				}
			}
			explainedNumber.Add(num, DefaultSettlementSecurityModel.LootedVillagesText, null);
		}

		private void CalculateInfestedHideoutEffectsOnSecurity(Town town, ref ExplainedNumber explainedNumber)
		{
			float num = 40f;
			float num2 = num * num;
			int num3 = 0;
			foreach (Hideout hideout in Hideout.All)
			{
				if (hideout.IsInfested && town.Settlement.Position2D.DistanceSquared(hideout.Settlement.Position2D) < num2)
				{
					num3++;
					break;
				}
			}
			if (num3 > 0)
			{
				explainedNumber.Add(-2f, DefaultSettlementSecurityModel.NearbyHideoutText, null);
			}
		}

		private void CalculateSecurityDrift(Town town, ref ExplainedNumber explainedNumber)
		{
			explainedNumber.Add(-1f * (town.Security - (float)this.SecurityDriftMedium) / 15f, DefaultSettlementSecurityModel.SecurityDriftText, null);
		}

		private void CalculatePolicyEffectsOnSecurity(Town town, ref ExplainedNumber explainedNumber)
		{
			Kingdom kingdom = town.Settlement.OwnerClan.Kingdom;
			if (kingdom != null)
			{
				if (kingdom.ActivePolicies.Contains(DefaultPolicies.Bailiffs))
				{
					explainedNumber.Add(1f, DefaultPolicies.Bailiffs.Name, null);
				}
				if (kingdom.ActivePolicies.Contains(DefaultPolicies.Magistrates))
				{
					explainedNumber.Add(1f, DefaultPolicies.Magistrates.Name, null);
				}
				if (kingdom.ActivePolicies.Contains(DefaultPolicies.Serfdom) && town.IsTown)
				{
					explainedNumber.Add(1f, DefaultPolicies.Serfdom.Name, null);
				}
				if (kingdom.ActivePolicies.Contains(DefaultPolicies.TrialByJury))
				{
					explainedNumber.Add(-0.2f, DefaultPolicies.TrialByJury.Name, null);
				}
			}
		}

		private void CalculateGovernorEffectsOnSecurity(Town town, ref ExplainedNumber explainedNumber)
		{
		}

		private void CalculateGarrisonEffectsOnSecurity(Town town, ref ExplainedNumber result)
		{
			if (town.GarrisonParty != null && town.GarrisonParty.MemberRoster.Count != 0 && town.GarrisonParty.MemberRoster.TotalHealthyCount != 0)
			{
				ExplainedNumber explainedNumber = new ExplainedNumber(0.01f, false, null);
				PerkHelper.AddPerkBonusForTown(DefaultPerks.OneHanded.StandUnited, town, ref explainedNumber);
				float num;
				float num2;
				float num3;
				this.CalculateStrengthOfGarrisonParty(town.GarrisonParty.Party, out num, out num2, out num3);
				float num4 = num * explainedNumber.ResultNumber;
				result.Add(num4, DefaultSettlementSecurityModel.GarrisonText, null);
				if (PerkHelper.GetPerkValueForTown(DefaultPerks.Leadership.Authority, town))
				{
					result.Add(num4 * DefaultPerks.Leadership.Authority.PrimaryBonus, DefaultPerks.Leadership.Authority.Name, null);
				}
				if (PerkHelper.GetPerkValueForTown(DefaultPerks.Riding.ReliefForce, town))
				{
					float num5 = num3 / num;
					result.Add(num4 * num5 * DefaultPerks.Riding.ReliefForce.SecondaryBonus, DefaultPerks.Riding.ReliefForce.Name, null);
				}
				float num6 = num2 / num;
				if (PerkHelper.GetPerkValueForTown(DefaultPerks.Bow.MountedArchery, town))
				{
					result.Add(num4 * num6 * DefaultPerks.Bow.MountedArchery.SecondaryBonus, DefaultPerks.Bow.MountedArchery.Name, null);
				}
				if (PerkHelper.GetPerkValueForTown(DefaultPerks.Bow.RangersSwiftness, town))
				{
					result.Add(num4 * num6 * DefaultPerks.Bow.RangersSwiftness.SecondaryBonus, DefaultPerks.Bow.RangersSwiftness.Name, null);
				}
				if (PerkHelper.GetPerkValueForTown(DefaultPerks.Crossbow.RenownMarksmen, town))
				{
					result.Add(num4 * num6 * DefaultPerks.Crossbow.RenownMarksmen.SecondaryBonus, DefaultPerks.Crossbow.RenownMarksmen.Name, null);
				}
			}
		}

		private void CalculateStrengthOfGarrisonParty(PartyBase party, out float totalStrength, out float archerStrength, out float cavalryStrength)
		{
			totalStrength = 0f;
			archerStrength = 0f;
			cavalryStrength = 0f;
			float num = 0f;
			MapEvent.PowerCalculationContext powerCalculationContext = MapEvent.PowerCalculationContext.Default;
			BattleSideEnum battleSideEnum = BattleSideEnum.Defender;
			if (party.MapEvent != null)
			{
				battleSideEnum = party.Side;
				num = Campaign.Current.Models.MilitaryPowerModel.GetLeaderModifierInMapEvent(party.MapEvent, battleSideEnum);
				powerCalculationContext = party.MapEvent.SimulationContext;
			}
			for (int i = 0; i < party.MemberRoster.Count; i++)
			{
				TroopRosterElement elementCopyAtIndex = party.MemberRoster.GetElementCopyAtIndex(i);
				if (elementCopyAtIndex.Character != null)
				{
					float troopPower = Campaign.Current.Models.MilitaryPowerModel.GetTroopPower(elementCopyAtIndex.Character, battleSideEnum, powerCalculationContext, num);
					float num2 = (float)(elementCopyAtIndex.Number - elementCopyAtIndex.WoundedNumber) * troopPower;
					if (elementCopyAtIndex.Character.IsMounted)
					{
						cavalryStrength += num2;
					}
					if (elementCopyAtIndex.Character.IsRanged)
					{
						archerStrength += num2;
					}
					totalStrength += num2;
				}
			}
		}

		private void CalculatePerkEffectsOnSecurity(Town town, ref ExplainedNumber result)
		{
			float num = (float)town.Settlement.Parties.Where(delegate(MobileParty x)
			{
				Clan actualClan = x.ActualClan;
				if (actualClan != null && !actualClan.IsAtWarWith(town.MapFaction))
				{
					Hero leaderHero = x.LeaderHero;
					return leaderHero != null && leaderHero.GetPerkValue(DefaultPerks.Leadership.Presence);
				}
				return false;
			}).Count<MobileParty>() * DefaultPerks.Leadership.Presence.PrimaryBonus;
			if (num > 0f)
			{
				result.Add(num, DefaultPerks.Leadership.Presence.Name, null);
			}
			if (town.Governor != null && town.Governor.GetPerkValue(DefaultPerks.Roguery.KnowHow))
			{
				PerkHelper.AddPerkBonusForTown(DefaultPerks.Roguery.KnowHow, town, ref result);
			}
			PerkHelper.AddPerkBonusForTown(DefaultPerks.OneHanded.ToBeBlunt, town, ref result);
			PerkHelper.AddPerkBonusForTown(DefaultPerks.Throwing.Focus, town, ref result);
			PerkHelper.AddPerkBonusForTown(DefaultPerks.Polearm.Skewer, town, ref result);
			PerkHelper.AddPerkBonusForTown(DefaultPerks.Tactics.Gensdarmes, town, ref result);
		}

		private void CalculateProjectEffectsOnSecurity(Town town, ref ExplainedNumber explainedNumber)
		{
		}

		private void CalculateIssueEffectsOnSecurity(Town town, ref ExplainedNumber explainedNumber)
		{
			Campaign.Current.Models.IssueModel.GetIssueEffectsOfSettlement(DefaultIssueEffects.SettlementSecurity, town.Settlement, ref explainedNumber);
		}

		public override float GetLootedNearbyPartySecurityEffect(Town town, float sumOfAttackedPartyStrengths)
		{
			return -1f * sumOfAttackedPartyStrengths * 0.005f;
		}

		public override float GetNearbyBanditPartyDefeatedSecurityEffect(Town town, float sumOfAttackedPartyStrengths)
		{
			return sumOfAttackedPartyStrengths * 0.005f;
		}

		public override void CalculateGoldGainDueToHighSecurity(Town town, ref ExplainedNumber explainedNumber)
		{
			float num = MBMath.Map(town.Security, (float)this.ThresholdForTaxBoost, (float)this.MaximumSecurityInSettlement, 0f, (float)this.SettlementTaxBoostPercentage);
			explainedNumber.AddFactor(num * 0.01f, DefaultSettlementSecurityModel.Security);
		}

		public override void CalculateGoldCutDueToLowSecurity(Town town, ref ExplainedNumber explainedNumber)
		{
			float num = MBMath.Map(town.Security, (float)this.ThresholdForHigherTaxCorruption, (float)this.ThresholdForTaxCorruption, (float)this.SettlementTaxPenaltyPercentage, 0f);
			explainedNumber.AddFactor(-1f * num * 0.01f, DefaultSettlementSecurityModel.CorruptionText);
		}

		private const float GarrisonHighSecurityGain = 3f;

		private const float GarrisonLowSecurityPenalty = -3f;

		private const float NearbyHideoutPenalty = -2f;

		private const float VillageLootedSecurityEffect = -2f;

		private const float UnderSiegeSecurityEffect = -3f;

		private const float MaxProsperityEffect = -5f;

		private const float PerProsperityEffect = -0.0005f;

		private static readonly TextObject GarrisonText = GameTexts.FindText("str_garrison", null);

		private static readonly TextObject LootedVillagesText = GameTexts.FindText("str_looted_villages", null);

		private static readonly TextObject CorruptionText = GameTexts.FindText("str_corruption", null);

		private static readonly TextObject NearbyHideoutText = GameTexts.FindText("str_nearby_hideout", null);

		private static readonly TextObject UnderSiegeText = GameTexts.FindText("str_under_siege", null);

		private static readonly TextObject ProsperityText = GameTexts.FindText("str_prosperity", null);

		private static readonly TextObject Security = GameTexts.FindText("str_security", null);

		private static readonly TextObject SecurityDriftText = GameTexts.FindText("str_security_drift", null);
	}
}
