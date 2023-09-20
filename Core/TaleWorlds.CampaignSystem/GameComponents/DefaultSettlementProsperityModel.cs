using System;
using System.Linq;
using Helpers;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Issues;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Buildings;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.GameComponents
{
	public class DefaultSettlementProsperityModel : SettlementProsperityModel
	{
		public override ExplainedNumber CalculateProsperityChange(Town fortification, bool includeDescriptions = false)
		{
			ExplainedNumber explainedNumber = new ExplainedNumber(0f, includeDescriptions, null);
			this.CalculateProsperityChangeInternal(fortification, ref explainedNumber);
			return explainedNumber;
		}

		public override ExplainedNumber CalculateHearthChange(Village village, bool includeDescriptions = false)
		{
			ExplainedNumber explainedNumber = new ExplainedNumber(0f, includeDescriptions, null);
			this.CalculateHearthChangeInternal(village, ref explainedNumber, includeDescriptions);
			return explainedNumber;
		}

		private void CalculateHearthChangeInternal(Village village, ref ExplainedNumber result, bool includeDescriptions)
		{
			if (village.VillageState == Village.VillageStates.Normal)
			{
				result = new ExplainedNumber((village.Hearth < 300f) ? 0.6f : ((village.Hearth < 600f) ? 0.4f : 0.2f), includeDescriptions, null);
			}
			if (village.VillageState == Village.VillageStates.Looted)
			{
				result.Add(-1f, DefaultSettlementProsperityModel.RaidedText, null);
			}
			if (village.Settlement.OwnerClan != null && village.Settlement.OwnerClan.Kingdom != null && village.Settlement.OwnerClan.Kingdom.ActivePolicies.Contains(DefaultPolicies.GrazingRights))
			{
				result.Add(-0.25f, DefaultPolicies.GrazingRights.Name, null);
			}
			if (village.Bound != null && village.VillageState == Village.VillageStates.Normal)
			{
				if (village.Bound.Town.CurrentDefaultBuilding != null && village.Bound.Town.BuildingsInProgress.IsEmpty<Building>())
				{
					BuildingHelper.AddDefaultDailyBonus(village.Bound.Town, BuildingEffectEnum.VillageDevelopmentDaily, ref result);
				}
				PerkHelper.AddPerkBonusForTown(DefaultPerks.Medicine.BushDoctor, village.Bound.Town, ref result);
				PerkHelper.AddPerkBonusForTown(DefaultPerks.Athletics.Energetic, village.Bound.Town, ref result);
				PerkHelper.AddPerkBonusForTown(DefaultPerks.Steward.AidCorps, village.Bound.Town, ref result);
			}
			if (village.Settlement.OwnerClan.Culture.HasFeat(DefaultCulturalFeats.EmpireVillageHearthFeat) && result.ResultNumber >= 0f)
			{
				result.AddFactor(DefaultCulturalFeats.EmpireVillageHearthFeat.EffectBonus, GameTexts.FindText("str_culture", null));
			}
			Campaign.Current.Models.IssueModel.GetIssueEffectsOfSettlement(DefaultIssueEffects.VillageHearth, village.Settlement, ref result);
		}

		private void CalculateProsperityChangeInternal(Town fortification, ref ExplainedNumber explainedNumber)
		{
			float foodChange = fortification.FoodChange;
			if (fortification.Owner.IsStarving)
			{
				ExplainedNumber explainedNumber2 = new ExplainedNumber((float)((foodChange < 0f) ? ((int)foodChange) : 0), false, null);
				PerkHelper.AddPerkBonusForTown(DefaultPerks.Medicine.HelpingHands, fortification, ref explainedNumber2);
				explainedNumber.Add(explainedNumber2.ResultNumber * 0.5f, DefaultSettlementProsperityModel.FoodShortageText, null);
			}
			if (fortification.IsTown)
			{
				if (fortification.Prosperity < 250f)
				{
					explainedNumber.Add(6f, DefaultSettlementProsperityModel.HousingCostsText, null);
				}
				else if (fortification.Prosperity < 500f)
				{
					explainedNumber.Add(5f, DefaultSettlementProsperityModel.HousingCostsText, null);
				}
				else if (fortification.Prosperity < 750f)
				{
					explainedNumber.Add(4f, DefaultSettlementProsperityModel.HousingCostsText, null);
				}
				else if (fortification.Prosperity < 1000f)
				{
					explainedNumber.Add(3f, DefaultSettlementProsperityModel.HousingCostsText, null);
				}
				else if (fortification.Prosperity < 1250f)
				{
					explainedNumber.Add(2f, DefaultSettlementProsperityModel.HousingCostsText, null);
				}
				else if (fortification.Prosperity < 1500f)
				{
					explainedNumber.Add(1f, DefaultSettlementProsperityModel.HousingCostsText, null);
				}
				if (fortification.Prosperity > 21000f)
				{
					explainedNumber.Add(-6f, DefaultSettlementProsperityModel.HousingCostsText, null);
				}
				else if (fortification.Prosperity > 18000f)
				{
					explainedNumber.Add(-5f, DefaultSettlementProsperityModel.HousingCostsText, null);
				}
				else if (fortification.Prosperity > 15000f)
				{
					explainedNumber.Add(-4f, DefaultSettlementProsperityModel.HousingCostsText, null);
				}
				else if (fortification.Prosperity > 12000f)
				{
					explainedNumber.Add(-3f, DefaultSettlementProsperityModel.HousingCostsText, null);
				}
				else if (fortification.Prosperity > 9000f)
				{
					explainedNumber.Add(-2f, DefaultSettlementProsperityModel.HousingCostsText, null);
				}
				else if (fortification.Prosperity > 6000f)
				{
					explainedNumber.Add(-1f, DefaultSettlementProsperityModel.HousingCostsText, null);
				}
			}
			int num = fortification.FoodStocksUpperLimit();
			int num2 = (int)(fortification.FoodStocks + foodChange) - num;
			if (num2 > 0)
			{
				explainedNumber.Add((float)num2 * 0.1f, DefaultSettlementProsperityModel.SurplusFoodText, null);
			}
			if (fortification.IsTown)
			{
				int num3 = fortification.SoldItems.Sum(delegate(Town.SellLog x)
				{
					if (x.Category.Properties != ItemCategory.Property.BonusToProsperity)
					{
						return 0;
					}
					return x.Number;
				});
				if (num3 > 0)
				{
					explainedNumber.Add((float)num3 * 0.1f, DefaultSettlementProsperityModel.ProsperityFromMarketText, null);
				}
			}
			PerkHelper.AddPerkBonusForTown(DefaultPerks.Medicine.PristineStreets, fortification, ref explainedNumber);
			if (PerkHelper.GetPerkValueForTown(DefaultPerks.Engineering.Apprenticeship, fortification))
			{
				float num4 = 0f;
				foreach (Building building in fortification.Buildings.Where((Building x) => !x.BuildingType.IsDefaultProject && x.CurrentLevel > 0))
				{
					num4 += DefaultPerks.Engineering.Apprenticeship.SecondaryBonus;
				}
				if (num4 > 0f && explainedNumber.ResultNumber > 0f)
				{
					explainedNumber.AddFactor(num4, DefaultPerks.Engineering.Apprenticeship.Name);
				}
			}
			if (fortification.BuildingsInProgress.IsEmpty<Building>())
			{
				BuildingHelper.AddDefaultDailyBonus(fortification, BuildingEffectEnum.ProsperityDaily, ref explainedNumber);
			}
			foreach (Building building2 in fortification.Buildings)
			{
				float buildingEffectAmount = building2.GetBuildingEffectAmount(BuildingEffectEnum.Prosperity);
				if (!building2.BuildingType.IsDefaultProject && buildingEffectAmount > 0f && foodChange > 0f)
				{
					explainedNumber.Add(buildingEffectAmount, building2.Name, null);
				}
				if (building2.CurrentLevel > 0 && (building2.BuildingType == DefaultBuildingTypes.SettlementAquaducts || building2.BuildingType == DefaultBuildingTypes.CastleGranary || building2.BuildingType == DefaultBuildingTypes.SettlementGranary))
				{
					PerkHelper.AddPerkBonusForTown(DefaultPerks.Medicine.CleanInfrastructure, fortification, ref explainedNumber);
				}
			}
			if (fortification.Loyalty > (float)Campaign.Current.Models.SettlementLoyaltyModel.ThresholdForProsperityBoost && foodChange > 0f)
			{
				explainedNumber.Add(Campaign.Current.Models.SettlementLoyaltyModel.HighLoyaltyProsperityEffect, DefaultSettlementProsperityModel.LoyaltyText, null);
			}
			else if (fortification.Loyalty <= (float)Campaign.Current.Models.SettlementLoyaltyModel.ThresholdForProsperityPenalty)
			{
				explainedNumber.Add((float)Campaign.Current.Models.SettlementLoyaltyModel.LowLoyaltyProsperityEffect, DefaultSettlementProsperityModel.LoyaltyText, null);
			}
			if (fortification.IsTown && !fortification.CurrentBuilding.IsCurrentlyDefault && fortification.Governor != null && fortification.Governor.GetPerkValue(DefaultPerks.Trade.TrickleDown))
			{
				explainedNumber.Add(DefaultPerks.Trade.TrickleDown.SecondaryBonus, DefaultPerks.Trade.TrickleDown.Name, null);
			}
			if (fortification.Settlement.OwnerClan.Kingdom != null)
			{
				if (fortification.Settlement.OwnerClan.Kingdom.ActivePolicies.Contains(DefaultPolicies.RoadTolls))
				{
					explainedNumber.Add(-0.2f, DefaultPolicies.RoadTolls.Name, null);
				}
				if (fortification.Settlement.OwnerClan.Kingdom.RulingClan == fortification.Settlement.OwnerClan && fortification.Settlement.OwnerClan.Kingdom.ActivePolicies.Contains(DefaultPolicies.ImperialTowns))
				{
					explainedNumber.Add(1f, DefaultPolicies.ImperialTowns.Name, null);
				}
				if (fortification.Settlement.OwnerClan.Kingdom.ActivePolicies.Contains(DefaultPolicies.CrownDuty))
				{
					explainedNumber.Add(-1f, DefaultPolicies.CrownDuty.Name, null);
				}
				if (fortification.Settlement.OwnerClan.Kingdom.ActivePolicies.Contains(DefaultPolicies.WarTax))
				{
					explainedNumber.Add(-1f, DefaultPolicies.WarTax.Name, null);
				}
			}
			this.GetSettlementProsperityChangeDueToIssues(fortification.Settlement, ref explainedNumber);
		}

		private void GetSettlementProsperityChangeDueToIssues(Settlement settlement, ref ExplainedNumber result)
		{
			Campaign.Current.Models.IssueModel.GetIssueEffectsOfSettlement(DefaultIssueEffects.SettlementProsperity, settlement, ref result);
		}

		private static readonly TextObject LoyaltyText = GameTexts.FindText("str_loyalty", null);

		private static readonly TextObject FoodShortageText = new TextObject("{=qTFKvGSg}Food Shortage", null);

		private static readonly TextObject ProsperityFromMarketText = new TextObject("{=RNT5hMVb}Goods From Market", null);

		private static readonly TextObject SurplusFoodText = GameTexts.FindText("str_surplus_food", null);

		private static readonly TextObject RaidedText = new TextObject("{=RVas572P}Raided", null);

		private static readonly TextObject HousingCostsText = new TextObject("{=ByRAgJy4}Housing Costs", null);
	}
}
