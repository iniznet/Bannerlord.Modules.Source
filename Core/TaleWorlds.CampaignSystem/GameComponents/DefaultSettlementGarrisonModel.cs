using System;
using Helpers;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Issues;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.GameComponents
{
	// Token: 0x02000137 RID: 311
	public class DefaultSettlementGarrisonModel : SettlementGarrisonModel
	{
		// Token: 0x06001712 RID: 5906 RVA: 0x00071BF4 File Offset: 0x0006FDF4
		public override ExplainedNumber CalculateGarrisonChange(Settlement settlement, bool includeDescriptions = false)
		{
			ExplainedNumber explainedNumber = new ExplainedNumber(0f, includeDescriptions, null);
			DefaultSettlementGarrisonModel.CalculateGarrisonChangeInternal(settlement, ref explainedNumber);
			return explainedNumber;
		}

		// Token: 0x06001713 RID: 5907 RVA: 0x00071C18 File Offset: 0x0006FE18
		private static void CalculateGarrisonChangeInternal(Settlement settlement, ref ExplainedNumber result)
		{
			if (settlement.IsTown || settlement.IsCastle)
			{
				if (settlement.Town.GarrisonParty != null && settlement.Town.GarrisonParty.HasUnpaidWages > 0f)
				{
					int num = MathF.Min(settlement.Town.GarrisonParty.Party.NumberOfHealthyMembers, 5);
					result.Add((float)(-(float)num), DefaultSettlementGarrisonModel.UnpaidWagesText, null);
				}
				if (settlement.Town.GarrisonParty != null && ((float)settlement.Town.GarrisonParty.Party.NumberOfHealthyMembers + result.ResultNumber > (float)settlement.Town.GarrisonParty.LimitedPartySize || settlement.Town.GarrisonParty.IsWageLimitExceeded()))
				{
					int num2 = MathF.Max(settlement.Town.GarrisonParty.IsWageLimitExceeded() ? MathF.Min(20, MathF.Max(1, (int)((float)(settlement.Town.GarrisonParty.TotalWage - settlement.Town.GarrisonParty.PaymentLimit) / Campaign.Current.AverageWage / 5f))) : 0, Campaign.Current.Models.PartyDesertionModel.GetNumberOfDeserters(settlement.Town.GarrisonParty));
					result.Add((float)(-(float)num2), DefaultSettlementGarrisonModel.PaymentIsLessText, null);
				}
				if (settlement.OwnerClan.IsRebelClan && (settlement.OwnerClan.MapFaction == null || !settlement.OwnerClan.MapFaction.IsKingdomFaction))
				{
					result.Add(2f, DefaultSettlementGarrisonModel.RebellionText, null);
				}
				if (settlement.IsFortification && settlement.Town.GarrisonAutoRecruitmentIsEnabled)
				{
					DefaultSettlementGarrisonModel.GetSettlementGarrisonDueToAutoRecruitment(settlement, ref result);
				}
			}
			DefaultSettlementGarrisonModel.GetSettlementGarrisonChangeDueToIssues(settlement, ref result);
		}

		// Token: 0x06001714 RID: 5908 RVA: 0x00071DC8 File Offset: 0x0006FFC8
		public override ExplainedNumber CalculateGarrisonChangeAutoRecruitment(Settlement settlement, bool includeDescriptions = false)
		{
			ExplainedNumber explainedNumber = new ExplainedNumber(0f, includeDescriptions, null);
			DefaultSettlementGarrisonModel.GetSettlementGarrisonDueToAutoRecruitment(settlement, ref explainedNumber);
			return explainedNumber;
		}

		// Token: 0x06001715 RID: 5909 RVA: 0x00071DEC File Offset: 0x0006FFEC
		private static void GetSettlementGarrisonDueToAutoRecruitment(Settlement settlement, ref ExplainedNumber result)
		{
			if (settlement.SiegeEvent == null && settlement.OwnerClan != null && settlement.IsFortification && settlement.Town.FoodChange > 0f && settlement.OwnerClan.Leader.PartyBelongedTo != null && settlement.Town.GarrisonParty != null && settlement.Town.GarrisonParty.CanPayMoreWage() && settlement.Town.GarrisonParty.Party.MemberRoster.TotalManCount < settlement.Town.GarrisonParty.LimitedPartySize && SettlementHelper.IsThereAnyVolunteerCanBeRecruitedForGarrison(settlement))
			{
				result.Add(1f, DefaultSettlementGarrisonModel.RecruitFromCenterNotablesText, null);
			}
		}

		// Token: 0x06001716 RID: 5910 RVA: 0x00071EA2 File Offset: 0x000700A2
		private static void GetSettlementGarrisonChangeDueToIssues(Settlement settlement, ref ExplainedNumber result)
		{
			Campaign.Current.Models.IssueModel.GetIssueEffectsOfSettlement(DefaultIssueEffects.SettlementGarrison, settlement, ref result);
		}

		// Token: 0x06001717 RID: 5911 RVA: 0x00071EC0 File Offset: 0x000700C0
		public override int FindNumberOfTroopsToTakeFromGarrison(MobileParty mobileParty, Settlement settlement, float defaultIdealGarrisonStrengthPerWalledCenter = 0f)
		{
			MobileParty garrisonParty = settlement.Town.GarrisonParty;
			if (garrisonParty != null)
			{
				float totalStrength = garrisonParty.Party.TotalStrength;
				float num;
				if (garrisonParty.HasLimitedWage())
				{
					num = (float)garrisonParty.PaymentLimit / Campaign.Current.AverageWage;
					num /= 1.5f;
				}
				else
				{
					num = ((defaultIdealGarrisonStrengthPerWalledCenter > 0.1f) ? defaultIdealGarrisonStrengthPerWalledCenter : FactionHelper.FindIdealGarrisonStrengthPerWalledCenter(mobileParty.MapFaction as Kingdom, settlement.OwnerClan));
					float num2 = FactionHelper.OwnerClanEconomyEffectOnGarrisonSizeConstant(settlement.OwnerClan);
					num *= num2;
					num *= (settlement.IsTown ? 2f : 1f);
				}
				float limitedPartySize = (float)mobileParty.LimitedPartySize;
				int numberOfAllMembers = mobileParty.Party.NumberOfAllMembers;
				float num3 = limitedPartySize / (float)numberOfAllMembers;
				float num4 = MathF.Min(11f, num3 * MathF.Sqrt(num3)) - 1f;
				float num5 = MathF.Pow(totalStrength / num, 1.5f);
				float num6 = ((mobileParty.LeaderHero.Clan.Leader == mobileParty.LeaderHero) ? 2f : 1f);
				int num7 = 0;
				if (num4 * num5 * num6 > 1f)
				{
					num7 = MBRandom.RoundRandomized(num4 * num5 * num6);
				}
				int num8 = 25;
				num8 *= (settlement.IsTown ? 2 : 1);
				if (num7 > garrisonParty.Party.MemberRoster.TotalRegulars - num8)
				{
					num7 = garrisonParty.Party.MemberRoster.TotalRegulars - num8;
				}
				return num7;
			}
			return 0;
		}

		// Token: 0x06001718 RID: 5912 RVA: 0x00072038 File Offset: 0x00070238
		public override int FindNumberOfTroopsToLeaveToGarrison(MobileParty mobileParty, Settlement settlement)
		{
			MobileParty garrisonParty = settlement.Town.GarrisonParty;
			float num = 0f;
			if (garrisonParty != null)
			{
				num = garrisonParty.Party.TotalStrength;
			}
			float num2;
			if (garrisonParty != null && garrisonParty.HasLimitedWage())
			{
				num2 = (float)garrisonParty.PaymentLimit / Campaign.Current.AverageWage;
			}
			else
			{
				num2 = FactionHelper.FindIdealGarrisonStrengthPerWalledCenter(mobileParty.MapFaction as Kingdom, settlement.OwnerClan);
				float num3 = FactionHelper.OwnerClanEconomyEffectOnGarrisonSizeConstant(settlement.OwnerClan);
				float num4 = FactionHelper.SettlementProsperityEffectOnGarrisonSizeConstant(settlement);
				float num5 = FactionHelper.SettlementFoodPotentialEffectOnGarrisonSizeConstant(settlement);
				num2 *= num3;
				num2 *= num4;
				num2 *= num5;
			}
			if ((settlement.OwnerClan.Leader != Hero.MainHero || (mobileParty.LeaderHero != null && mobileParty.LeaderHero.Clan == Clan.PlayerClan)) && num < num2)
			{
				int numberOfRegularMembers = mobileParty.Party.NumberOfRegularMembers;
				float num6 = 1f + (float)mobileParty.Party.MemberRoster.TotalWoundedRegulars / (float)mobileParty.Party.NumberOfRegularMembers;
				int limitedPartySize = mobileParty.LimitedPartySize;
				float num7 = MathF.Pow(MathF.Min(2f, (float)numberOfRegularMembers / (float)limitedPartySize), 1.2f) * 0.75f;
				float num8 = (1f - num / num2) * (1f - num / num2);
				float num9 = 1f;
				if (mobileParty.Army != null)
				{
					num8 = MathF.Min(num8, 0.5f);
					num9 = 0.3f + mobileParty.Army.TotalStrength / mobileParty.Party.TotalStrength * 0.015f;
				}
				float num10 = (settlement.Town.IsOwnerUnassigned ? 0.75f : 0.5f);
				if (settlement.OwnerClan == mobileParty.LeaderHero.Clan || settlement.OwnerClan == mobileParty.Party.Owner.MapFaction.Leader.Clan)
				{
					num10 = 1f;
				}
				float num11 = MathF.Min(0.7f, num7 * num8 * num10 * num6 * num9);
				if ((float)numberOfRegularMembers * num11 > 1f)
				{
					return MBRandom.RoundRandomized((float)numberOfRegularMembers * num11);
				}
			}
			return 0;
		}

		// Token: 0x04000827 RID: 2087
		private static readonly TextObject TownWallsText = new TextObject("{=SlmhqqH8}Town Walls", null);

		// Token: 0x04000828 RID: 2088
		private static readonly TextObject MoraleText = new TextObject("{=UjL7jVYF}Morale", null);

		// Token: 0x04000829 RID: 2089
		private static readonly TextObject FoodShortageText = new TextObject("{=qTFKvGSg}Food Shortage", null);

		// Token: 0x0400082A RID: 2090
		private static readonly TextObject SurplusFoodText = GameTexts.FindText("str_surplus_food", null);

		// Token: 0x0400082B RID: 2091
		private static readonly TextObject RecruitFromCenterNotablesText = GameTexts.FindText("str_center_notables", null);

		// Token: 0x0400082C RID: 2092
		private static readonly TextObject RecruitFromVillageNotablesText = GameTexts.FindText("str_village_notables", null);

		// Token: 0x0400082D RID: 2093
		private static readonly TextObject VillageBeingRaided = GameTexts.FindText("str_village_being_raided", null);

		// Token: 0x0400082E RID: 2094
		private static readonly TextObject VillageLooted = GameTexts.FindText("str_village_looted", null);

		// Token: 0x0400082F RID: 2095
		private static readonly TextObject TownIsUnderSiege = GameTexts.FindText("str_villages_under_siege", null);

		// Token: 0x04000830 RID: 2096
		private static readonly TextObject RetiredText = GameTexts.FindText("str_retired", null);

		// Token: 0x04000831 RID: 2097
		private static readonly TextObject PaymentIsLessText = GameTexts.FindText("str_payment_is_less", null);

		// Token: 0x04000832 RID: 2098
		private static readonly TextObject UnpaidWagesText = GameTexts.FindText("str_unpaid_wages", null);

		// Token: 0x04000833 RID: 2099
		private static readonly TextObject RebellionText = GameTexts.FindText("str_rebel_settlement", null);
	}
}
