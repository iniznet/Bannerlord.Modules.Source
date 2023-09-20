using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Issues;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.GameComponents
{
	// Token: 0x0200013B RID: 315
	public class DefaultSettlementSecurityModel : SettlementSecurityModel
	{
		// Token: 0x17000635 RID: 1589
		// (get) Token: 0x06001752 RID: 5970 RVA: 0x00073A51 File Offset: 0x00071C51
		public override int MaximumSecurityInSettlement
		{
			get
			{
				return 100;
			}
		}

		// Token: 0x17000636 RID: 1590
		// (get) Token: 0x06001753 RID: 5971 RVA: 0x00073A55 File Offset: 0x00071C55
		public override int SecurityDriftMedium
		{
			get
			{
				return 50;
			}
		}

		// Token: 0x17000637 RID: 1591
		// (get) Token: 0x06001754 RID: 5972 RVA: 0x00073A59 File Offset: 0x00071C59
		public override float MapEventSecurityEffectRadius
		{
			get
			{
				return 50f;
			}
		}

		// Token: 0x17000638 RID: 1592
		// (get) Token: 0x06001755 RID: 5973 RVA: 0x00073A60 File Offset: 0x00071C60
		public override float HideoutClearedSecurityEffectRadius
		{
			get
			{
				return 100f;
			}
		}

		// Token: 0x17000639 RID: 1593
		// (get) Token: 0x06001756 RID: 5974 RVA: 0x00073A67 File Offset: 0x00071C67
		public override int HideoutClearedSecurityGain
		{
			get
			{
				return 6;
			}
		}

		// Token: 0x1700063A RID: 1594
		// (get) Token: 0x06001757 RID: 5975 RVA: 0x00073A6A File Offset: 0x00071C6A
		public override int ThresholdForTaxCorruption
		{
			get
			{
				return 50;
			}
		}

		// Token: 0x1700063B RID: 1595
		// (get) Token: 0x06001758 RID: 5976 RVA: 0x00073A6E File Offset: 0x00071C6E
		public override int ThresholdForHigherTaxCorruption
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x1700063C RID: 1596
		// (get) Token: 0x06001759 RID: 5977 RVA: 0x00073A71 File Offset: 0x00071C71
		public override int ThresholdForTaxBoost
		{
			get
			{
				return 75;
			}
		}

		// Token: 0x1700063D RID: 1597
		// (get) Token: 0x0600175A RID: 5978 RVA: 0x00073A75 File Offset: 0x00071C75
		public override int SettlementTaxBoostPercentage
		{
			get
			{
				return 5;
			}
		}

		// Token: 0x1700063E RID: 1598
		// (get) Token: 0x0600175B RID: 5979 RVA: 0x00073A78 File Offset: 0x00071C78
		public override int SettlementTaxPenaltyPercentage
		{
			get
			{
				return 10;
			}
		}

		// Token: 0x1700063F RID: 1599
		// (get) Token: 0x0600175C RID: 5980 RVA: 0x00073A7C File Offset: 0x00071C7C
		public override int ThresholdForNotableRelationBonus
		{
			get
			{
				return 75;
			}
		}

		// Token: 0x17000640 RID: 1600
		// (get) Token: 0x0600175D RID: 5981 RVA: 0x00073A80 File Offset: 0x00071C80
		public override int ThresholdForNotableRelationPenalty
		{
			get
			{
				return 50;
			}
		}

		// Token: 0x17000641 RID: 1601
		// (get) Token: 0x0600175E RID: 5982 RVA: 0x00073A84 File Offset: 0x00071C84
		public override int DailyNotableRelationBonus
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x17000642 RID: 1602
		// (get) Token: 0x0600175F RID: 5983 RVA: 0x00073A87 File Offset: 0x00071C87
		public override int DailyNotableRelationPenalty
		{
			get
			{
				return -1;
			}
		}

		// Token: 0x17000643 RID: 1603
		// (get) Token: 0x06001760 RID: 5984 RVA: 0x00073A8A File Offset: 0x00071C8A
		public override int DailyNotablePowerBonus
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x17000644 RID: 1604
		// (get) Token: 0x06001761 RID: 5985 RVA: 0x00073A8D File Offset: 0x00071C8D
		public override int DailyNotablePowerPenalty
		{
			get
			{
				return -1;
			}
		}

		// Token: 0x06001762 RID: 5986 RVA: 0x00073A90 File Offset: 0x00071C90
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

		// Token: 0x06001763 RID: 5987 RVA: 0x00073B0F File Offset: 0x00071D0F
		private void CalculateProsperityEffectOnSecurity(Town town, ref ExplainedNumber explainedNumber)
		{
			explainedNumber.Add(MathF.Max(-5f, -0.0005f * town.Settlement.Prosperity), DefaultSettlementSecurityModel.ProsperityText, null);
		}

		// Token: 0x06001764 RID: 5988 RVA: 0x00073B38 File Offset: 0x00071D38
		private void CalculateUnderSiegeEffectsOnSecurity(Town town, ref ExplainedNumber explainedNumber)
		{
			if (town.Settlement.IsUnderSiege)
			{
				explainedNumber.Add(-3f, DefaultSettlementSecurityModel.UnderSiegeText, null);
			}
		}

		// Token: 0x06001765 RID: 5989 RVA: 0x00073B58 File Offset: 0x00071D58
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

		// Token: 0x06001766 RID: 5990 RVA: 0x00073BD0 File Offset: 0x00071DD0
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

		// Token: 0x06001767 RID: 5991 RVA: 0x00073C6C File Offset: 0x00071E6C
		private void CalculateSecurityDrift(Town town, ref ExplainedNumber explainedNumber)
		{
			explainedNumber.Add(-1f * (town.Security - (float)this.SecurityDriftMedium) / 15f, DefaultSettlementSecurityModel.SecurityDriftText, null);
		}

		// Token: 0x06001768 RID: 5992 RVA: 0x00073C94 File Offset: 0x00071E94
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

		// Token: 0x06001769 RID: 5993 RVA: 0x00073D60 File Offset: 0x00071F60
		private void CalculateGovernorEffectsOnSecurity(Town town, ref ExplainedNumber explainedNumber)
		{
		}

		// Token: 0x0600176A RID: 5994 RVA: 0x00073D64 File Offset: 0x00071F64
		private void CalculateGarrisonEffectsOnSecurity(Town town, ref ExplainedNumber result)
		{
			if (town.GarrisonParty != null && town.GarrisonParty.MemberRoster.Count != 0 && town.GarrisonParty.MemberRoster.TotalHealthyCount != 0)
			{
				ExplainedNumber explainedNumber = new ExplainedNumber(0.01f, false, null);
				PerkHelper.AddPerkBonusForTown(DefaultPerks.OneHanded.StandUnited, town, ref explainedNumber);
				float num;
				float num2;
				float num3;
				this.CalculateStrength(town.GarrisonParty.Party, out num, out num2, out num3);
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
				if (PerkHelper.GetPerkValueForTown(DefaultPerks.Bow.NockingPoint, town))
				{
					result.Add(num4 * num6 * DefaultPerks.Bow.NockingPoint.SecondaryBonus, DefaultPerks.Bow.NockingPoint.Name, null);
				}
				if (PerkHelper.GetPerkValueForTown(DefaultPerks.Crossbow.RenownMarksmen, town))
				{
					result.Add(num4 * num6 * DefaultPerks.Crossbow.RenownMarksmen.SecondaryBonus, DefaultPerks.Crossbow.RenownMarksmen.Name, null);
				}
			}
		}

		// Token: 0x0600176B RID: 5995 RVA: 0x00073EE0 File Offset: 0x000720E0
		public void CalculateStrength(PartyBase party, out float totalStrength, out float archerStrength, out float cavalryStrength)
		{
			totalStrength = 0f;
			archerStrength = 0f;
			cavalryStrength = 0f;
			for (int i = 0; i < party.MemberRoster.Count; i++)
			{
				TroopRosterElement elementCopyAtIndex = party.MemberRoster.GetElementCopyAtIndex(i);
				if (elementCopyAtIndex.Character != null)
				{
					float num = (float)(elementCopyAtIndex.Number - elementCopyAtIndex.WoundedNumber) * Campaign.Current.Models.MilitaryPowerModel.GetTroopPowerToCalculateSecurity(elementCopyAtIndex.Character);
					if (elementCopyAtIndex.Character.IsMounted)
					{
						cavalryStrength += num;
					}
					if (elementCopyAtIndex.Character.IsRanged)
					{
						archerStrength += num;
					}
					totalStrength += num;
				}
			}
		}

		// Token: 0x0600176C RID: 5996 RVA: 0x00073F8C File Offset: 0x0007218C
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

		// Token: 0x0600176D RID: 5997 RVA: 0x0007406C File Offset: 0x0007226C
		private void CalculateProjectEffectsOnSecurity(Town town, ref ExplainedNumber explainedNumber)
		{
		}

		// Token: 0x0600176E RID: 5998 RVA: 0x0007406E File Offset: 0x0007226E
		private void CalculateIssueEffectsOnSecurity(Town town, ref ExplainedNumber explainedNumber)
		{
			Campaign.Current.Models.IssueModel.GetIssueEffectsOfSettlement(DefaultIssueEffects.SettlementSecurity, town.Settlement, ref explainedNumber);
		}

		// Token: 0x0600176F RID: 5999 RVA: 0x00074090 File Offset: 0x00072290
		public override float GetLootedNearbyPartySecurityEffect(Town town, float sumOfAttackedPartyStrengths)
		{
			return -1f * sumOfAttackedPartyStrengths * 0.005f;
		}

		// Token: 0x06001770 RID: 6000 RVA: 0x0007409F File Offset: 0x0007229F
		public override float GetNearbyBanditPartyDefeatedSecurityEffect(Town town, float sumOfAttackedPartyStrengths)
		{
			return sumOfAttackedPartyStrengths * 0.005f;
		}

		// Token: 0x06001771 RID: 6001 RVA: 0x000740A8 File Offset: 0x000722A8
		public override void CalculateGoldGainDueToHighSecurity(Town town, ref ExplainedNumber explainedNumber)
		{
			float num = MBMath.Map(town.Security, (float)this.ThresholdForTaxBoost, (float)this.MaximumSecurityInSettlement, 0f, (float)this.SettlementTaxBoostPercentage);
			explainedNumber.AddFactor(num * 0.01f, DefaultSettlementSecurityModel.Security);
		}

		// Token: 0x06001772 RID: 6002 RVA: 0x000740F0 File Offset: 0x000722F0
		public override void CalculateGoldCutDueToLowSecurity(Town town, ref ExplainedNumber explainedNumber)
		{
			float num = MBMath.Map(town.Security, (float)this.ThresholdForHigherTaxCorruption, (float)this.ThresholdForTaxCorruption, (float)this.SettlementTaxPenaltyPercentage, 0f);
			explainedNumber.AddFactor(-1f * num * 0.01f, DefaultSettlementSecurityModel.CorruptionText);
		}

		// Token: 0x04000851 RID: 2129
		private const float GarrisonHighSecurityGain = 3f;

		// Token: 0x04000852 RID: 2130
		private const float GarrisonLowSecurityPenalty = -3f;

		// Token: 0x04000853 RID: 2131
		private const float NearbyHideoutPenalty = -2f;

		// Token: 0x04000854 RID: 2132
		private const float VillageLootedSecurityEffect = -2f;

		// Token: 0x04000855 RID: 2133
		private const float UnderSiegeSecurityEffect = -3f;

		// Token: 0x04000856 RID: 2134
		private const float MaxProsperityEffect = -5f;

		// Token: 0x04000857 RID: 2135
		private const float PerProsperityEffect = -0.0005f;

		// Token: 0x04000858 RID: 2136
		private static readonly TextObject GarrisonText = GameTexts.FindText("str_garrison", null);

		// Token: 0x04000859 RID: 2137
		private static readonly TextObject LootedVillagesText = GameTexts.FindText("str_looted_villages", null);

		// Token: 0x0400085A RID: 2138
		private static readonly TextObject CorruptionText = GameTexts.FindText("str_corruption", null);

		// Token: 0x0400085B RID: 2139
		private static readonly TextObject NearbyHideoutText = GameTexts.FindText("str_nearby_hideout", null);

		// Token: 0x0400085C RID: 2140
		private static readonly TextObject UnderSiegeText = GameTexts.FindText("str_under_siege", null);

		// Token: 0x0400085D RID: 2141
		private static readonly TextObject ProsperityText = GameTexts.FindText("str_prosperity", null);

		// Token: 0x0400085E RID: 2142
		private static readonly TextObject Security = GameTexts.FindText("str_security", null);

		// Token: 0x0400085F RID: 2143
		private static readonly TextObject SecurityDriftText = GameTexts.FindText("str_security_drift", null);
	}
}
