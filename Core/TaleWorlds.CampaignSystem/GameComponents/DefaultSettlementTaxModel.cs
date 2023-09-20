using System;
using Helpers;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Issues;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Buildings;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.GameComponents
{
	// Token: 0x0200013C RID: 316
	public class DefaultSettlementTaxModel : SettlementTaxModel
	{
		// Token: 0x17000645 RID: 1605
		// (get) Token: 0x06001775 RID: 6005 RVA: 0x000741D1 File Offset: 0x000723D1
		public override float SettlementCommissionRateTown
		{
			get
			{
				return 0.06f;
			}
		}

		// Token: 0x17000646 RID: 1606
		// (get) Token: 0x06001776 RID: 6006 RVA: 0x000741D8 File Offset: 0x000723D8
		public override float SettlementCommissionRateVillage
		{
			get
			{
				return 0.7f;
			}
		}

		// Token: 0x17000647 RID: 1607
		// (get) Token: 0x06001777 RID: 6007 RVA: 0x000741DF File Offset: 0x000723DF
		public override int SettlementCommissionDecreaseSecurityThreshold
		{
			get
			{
				return 75;
			}
		}

		// Token: 0x17000648 RID: 1608
		// (get) Token: 0x06001778 RID: 6008 RVA: 0x000741E3 File Offset: 0x000723E3
		public override int MaximumDecreaseBasedOnSecuritySecurity
		{
			get
			{
				return 10;
			}
		}

		// Token: 0x06001779 RID: 6009 RVA: 0x000741E8 File Offset: 0x000723E8
		public override float GetTownTaxRatio(Town town)
		{
			float num = 1f;
			if (town.Settlement.OwnerClan.Kingdom != null && town.Settlement.OwnerClan.Kingdom.ActivePolicies.Contains(DefaultPolicies.CrownDuty))
			{
				num += 0.05f;
			}
			return this.SettlementCommissionRateTown * num;
		}

		// Token: 0x0600177A RID: 6010 RVA: 0x0007423E File Offset: 0x0007243E
		public override float GetVillageTaxRatio()
		{
			return this.SettlementCommissionRateVillage;
		}

		// Token: 0x0600177B RID: 6011 RVA: 0x00074248 File Offset: 0x00072448
		public override float GetTownCommissionChangeBasedOnSecurity(Town town, float commission)
		{
			if (town.Security < (float)this.SettlementCommissionDecreaseSecurityThreshold)
			{
				float num = MBMath.Map((float)this.SettlementCommissionDecreaseSecurityThreshold - town.Security, 0f, (float)this.SettlementCommissionDecreaseSecurityThreshold, (float)this.MaximumDecreaseBasedOnSecuritySecurity, 0f);
				commission -= commission * (num * 0.01f);
				return commission;
			}
			return commission;
		}

		// Token: 0x0600177C RID: 6012 RVA: 0x000742A0 File Offset: 0x000724A0
		public override ExplainedNumber CalculateTownTax(Town town, bool includeDescriptions = false)
		{
			ExplainedNumber explainedNumber = new ExplainedNumber(0f, includeDescriptions, null);
			this.CalculateDailyTaxInternal(town, ref explainedNumber);
			return explainedNumber;
		}

		// Token: 0x0600177D RID: 6013 RVA: 0x000742C8 File Offset: 0x000724C8
		private float CalculateDailyTax(Town town, ref ExplainedNumber explainedNumber)
		{
			float prosperity = town.Prosperity;
			float num = 1f;
			if (town.Settlement.OwnerClan.Kingdom != null && town.Settlement.OwnerClan.Kingdom.ActivePolicies.Contains(DefaultPolicies.CouncilOfTheCommons))
			{
				num -= 0.05f;
			}
			float num2 = 0.35f;
			float num3 = prosperity * num2 * num;
			explainedNumber.Add(num3, DefaultSettlementTaxModel.ProsperityText, null);
			return explainedNumber.ResultNumber;
		}

		// Token: 0x0600177E RID: 6014 RVA: 0x0007433C File Offset: 0x0007253C
		private void CalculateDailyTaxInternal(Town town, ref ExplainedNumber result)
		{
			float num = this.CalculateDailyTax(town, ref result);
			this.CalculatePolicyGoldCut(town, num, ref result);
			if (town.IsTown || town.IsCastle)
			{
				if (PerkHelper.GetPerkValueForTown(DefaultPerks.Bow.QuickDraw, town))
				{
					PerkHelper.AddPerkBonusForTown(DefaultPerks.Bow.QuickDraw, town, ref result);
				}
				Hero governor = town.Governor;
				if (governor != null && governor.GetPerkValue(DefaultPerks.Steward.Logistician))
				{
					PerkHelper.AddPerkBonusForTown(DefaultPerks.Steward.Logistician, town, ref result);
				}
			}
			if (town.IsTown)
			{
				if (PerkHelper.GetPerkValueForTown(DefaultPerks.Scouting.DesertBorn, town))
				{
					PerkHelper.AddPerkBonusForTown(DefaultPerks.Scouting.DesertBorn, town, ref result);
				}
				if (town.Governor != null && town.Governor.GetPerkValue(DefaultPerks.Steward.PriceOfLoyalty))
				{
					int num2 = town.Governor.GetSkillValue(DefaultSkills.Steward) - 250;
					result.AddFactor(DefaultPerks.Steward.PriceOfLoyalty.SecondaryBonus * (float)num2, DefaultPerks.Steward.PriceOfLoyalty.Name);
				}
				if (town.OwnerClan.Culture.HasFeat(DefaultCulturalFeats.KhuzaitDecreasedTaxFeat))
				{
					result.AddFactor(DefaultCulturalFeats.KhuzaitDecreasedTaxFeat.EffectBonus, GameTexts.FindText("str_culture", null));
				}
			}
			this.GetSettlementTaxChangeDueToIssues(town, ref result);
			this.CalculateSettlementTaxDueToSecurity(town, ref result);
			this.CalculateSettlementTaxDueToLoyalty(town, ref result);
			this.CalculateSettlementTaxDueToBuildings(town, ref result);
			result.Clamp(0f, float.MaxValue);
		}

		// Token: 0x0600177F RID: 6015 RVA: 0x00074480 File Offset: 0x00072680
		private void CalculateSettlementTaxDueToSecurity(Town town, ref ExplainedNumber explainedNumber)
		{
			SettlementSecurityModel settlementSecurityModel = Campaign.Current.Models.SettlementSecurityModel;
			if (town.Security >= (float)settlementSecurityModel.ThresholdForTaxBoost)
			{
				settlementSecurityModel.CalculateGoldGainDueToHighSecurity(town, ref explainedNumber);
				return;
			}
			if (town.Security >= (float)settlementSecurityModel.ThresholdForHigherTaxCorruption && town.Security < (float)settlementSecurityModel.ThresholdForTaxCorruption)
			{
				settlementSecurityModel.CalculateGoldCutDueToLowSecurity(town, ref explainedNumber);
			}
		}

		// Token: 0x06001780 RID: 6016 RVA: 0x000744DC File Offset: 0x000726DC
		private void CalculateSettlementTaxDueToLoyalty(Town town, ref ExplainedNumber explainedNumber)
		{
			SettlementLoyaltyModel settlementLoyaltyModel = Campaign.Current.Models.SettlementLoyaltyModel;
			if (town.Loyalty >= (float)settlementLoyaltyModel.ThresholdForTaxBoost)
			{
				settlementLoyaltyModel.CalculateGoldGainDueToHighLoyalty(town, ref explainedNumber);
				return;
			}
			if (town.Loyalty >= (float)settlementLoyaltyModel.ThresholdForHigherTaxCorruption && town.Loyalty <= (float)settlementLoyaltyModel.ThresholdForTaxCorruption)
			{
				settlementLoyaltyModel.CalculateGoldCutDueToLowLoyalty(town, ref explainedNumber);
				return;
			}
			if (town.Loyalty < (float)settlementLoyaltyModel.ThresholdForHigherTaxCorruption)
			{
				explainedNumber.AddFactor(-1f, DefaultSettlementTaxModel.VeryLowLoyalty);
			}
		}

		// Token: 0x06001781 RID: 6017 RVA: 0x00074558 File Offset: 0x00072758
		private void CalculateSettlementTaxDueToBuildings(Town town, ref ExplainedNumber result)
		{
			if (town.IsTown || town.IsCastle)
			{
				foreach (Building building in town.Buildings)
				{
					float buildingEffectAmount = building.GetBuildingEffectAmount(BuildingEffectEnum.Tax);
					result.AddFactor(buildingEffectAmount * 0.01f, building.Name);
				}
			}
		}

		// Token: 0x06001782 RID: 6018 RVA: 0x000745D0 File Offset: 0x000727D0
		private void CalculatePolicyGoldCut(Town town, float rawTax, ref ExplainedNumber explainedNumber)
		{
			if (town.MapFaction.IsKingdomFaction)
			{
				Kingdom kingdom = (Kingdom)town.MapFaction;
				if (kingdom.ActivePolicies.Contains(DefaultPolicies.Magistrates))
				{
					explainedNumber.Add(-0.05f * rawTax, DefaultPolicies.Magistrates.Name, null);
				}
				if (kingdom.ActivePolicies.Contains(DefaultPolicies.Cantons))
				{
					explainedNumber.Add(-0.1f * rawTax, DefaultPolicies.Cantons.Name, null);
				}
				if (kingdom.ActivePolicies.Contains(DefaultPolicies.Bailiffs))
				{
					explainedNumber.Add(-0.05f * rawTax, DefaultPolicies.Bailiffs.Name, null);
				}
				if (kingdom.ActivePolicies.Contains(DefaultPolicies.TribunesOfThePeople))
				{
					explainedNumber.Add(-0.05f * rawTax, DefaultPolicies.TribunesOfThePeople.Name, null);
				}
			}
		}

		// Token: 0x06001783 RID: 6019 RVA: 0x0007469F File Offset: 0x0007289F
		private void GetSettlementTaxChangeDueToIssues(Town center, ref ExplainedNumber result)
		{
			Campaign.Current.Models.IssueModel.GetIssueEffectsOfSettlement(DefaultIssueEffects.SettlementTax, center.Owner.Settlement, ref result);
		}

		// Token: 0x06001784 RID: 6020 RVA: 0x000746C6 File Offset: 0x000728C6
		public override int CalculateVillageTaxFromIncome(Village village, int marketIncome)
		{
			return (int)((float)marketIncome * this.GetVillageTaxRatio());
		}

		// Token: 0x04000860 RID: 2144
		private static readonly TextObject ProsperityText = GameTexts.FindText("str_prosperity", null);

		// Token: 0x04000861 RID: 2145
		private static readonly TextObject VeryLowSecurity = new TextObject("{=IaJ4lhzx}Very Low Security", null);

		// Token: 0x04000862 RID: 2146
		private static readonly TextObject VeryLowLoyalty = new TextObject("{=CcQzFnpN}Very Low Loyalty", null);
	}
}
