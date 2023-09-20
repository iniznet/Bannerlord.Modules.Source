using System;
using System.Linq;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Issues;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Buildings;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.GameComponents
{
	public class DefaultSettlementFoodModel : SettlementFoodModel
	{
		public override int FoodStocksUpperLimit
		{
			get
			{
				return 100;
			}
		}

		public override int NumberOfProsperityToEatOneFood
		{
			get
			{
				return 40;
			}
		}

		public override int NumberOfMenOnGarrisonToEatOneFood
		{
			get
			{
				return 20;
			}
		}

		public override int CastleFoodStockUpperLimitBonus
		{
			get
			{
				return 150;
			}
		}

		public override ExplainedNumber CalculateTownFoodStocksChange(Town town, bool includeMarketStocks = true, bool includeDescriptions = false)
		{
			return this.CalculateTownFoodChangeInternal(town, includeMarketStocks, includeDescriptions);
		}

		private ExplainedNumber CalculateTownFoodChangeInternal(Town town, bool includeMarketStocks, bool includeDescriptions)
		{
			ExplainedNumber explainedNumber = new ExplainedNumber(0f, includeDescriptions, null);
			float num2;
			if (!town.IsUnderSiege)
			{
				int num = (town.IsTown ? 15 : 10);
				explainedNumber.Add((float)num, DefaultSettlementFoodModel.LandsAroundSettlementText, null);
				num2 = -town.Prosperity / (float)this.NumberOfProsperityToEatOneFood;
			}
			else
			{
				num2 = -town.Prosperity / (float)this.NumberOfProsperityToEatOneFood;
			}
			MobileParty garrisonParty = town.GarrisonParty;
			int num3 = ((garrisonParty != null) ? garrisonParty.Party.NumberOfAllMembers : 0);
			num3 = -num3 / this.NumberOfMenOnGarrisonToEatOneFood;
			float num4 = 0f;
			float num5 = 0f;
			if (town.Governor != null)
			{
				if (town.IsUnderSiege)
				{
					if (town.Governor.GetPerkValue(DefaultPerks.Steward.Gourmet))
					{
						num5 += DefaultPerks.Steward.Gourmet.SecondaryBonus;
					}
					if (town.Governor.GetPerkValue(DefaultPerks.Medicine.TriageTent))
					{
						num4 += DefaultPerks.Medicine.TriageTent.SecondaryBonus;
					}
				}
				if (town.Governor.GetPerkValue(DefaultPerks.Steward.MasterOfWarcraft))
				{
					num4 += DefaultPerks.Steward.MasterOfWarcraft.SecondaryBonus;
				}
			}
			num2 += num2 * num4;
			num3 += (int)((float)num3 * (num4 + num5));
			explainedNumber.Add(num2, DefaultSettlementFoodModel.ProsperityText, null);
			explainedNumber.Add((float)num3, DefaultSettlementFoodModel.GarrisonText, null);
			Clan ownerClan = town.Settlement.OwnerClan;
			if (((ownerClan != null) ? ownerClan.Kingdom : null) != null && town.Settlement.OwnerClan.Kingdom.ActivePolicies.Contains(DefaultPolicies.HuntingRights))
			{
				explainedNumber.Add(2f, DefaultPolicies.HuntingRights.Name, null);
			}
			if (!town.IsUnderSiege)
			{
				foreach (Village village in town.Owner.Settlement.BoundVillages)
				{
					if (village.VillageState == Village.VillageStates.Normal)
					{
						int num6 = (village.GetHearthLevel() + 1) * 6;
						explainedNumber.Add((float)num6, village.Name, null);
					}
					else
					{
						int num6 = 0;
						explainedNumber.Add((float)num6, village.Name, null);
					}
				}
				float effectOfBuildings = town.GetEffectOfBuildings(BuildingEffectEnum.FoodProduction);
				if (effectOfBuildings > 0f)
				{
					explainedNumber.Add(effectOfBuildings, includeDescriptions ? GameTexts.FindText("str_building_bonus", null) : TextObject.Empty, null);
				}
			}
			else if (town.Governor != null && town.Governor.GetPerkValue(DefaultPerks.Roguery.DirtyFighting))
			{
				explainedNumber.Add(DefaultPerks.Roguery.DirtyFighting.SecondaryBonus, DefaultPerks.Roguery.DirtyFighting.Name, null);
			}
			else
			{
				explainedNumber.Add(0f, DefaultSettlementFoodModel.VillagesUnderSiegeText, null);
			}
			if (includeMarketStocks)
			{
				foreach (Town.SellLog sellLog in town.SoldItems)
				{
					if (sellLog.Category.Properties == ItemCategory.Property.BonusToFoodStores)
					{
						explainedNumber.Add((float)sellLog.Number, includeDescriptions ? sellLog.Category.GetName() : TextObject.Empty, null);
					}
				}
			}
			DefaultSettlementFoodModel.GetSettlementFoodChangeDueToIssues(town, ref explainedNumber);
			return explainedNumber;
		}

		private int CalculateFoodPurchasedFromMarket(Town town)
		{
			return town.SoldItems.Sum(delegate(Town.SellLog x)
			{
				if (x.Category.Properties != ItemCategory.Property.BonusToFoodStores)
				{
					return 0;
				}
				return x.Number;
			});
		}

		private static void GetSettlementFoodChangeDueToIssues(Town town, ref ExplainedNumber explainedNumber)
		{
			Campaign.Current.Models.IssueModel.GetIssueEffectsOfSettlement(DefaultIssueEffects.SettlementFood, town.Settlement, ref explainedNumber);
		}

		private static readonly TextObject ProsperityText = GameTexts.FindText("str_prosperity", null);

		private static readonly TextObject GarrisonText = GameTexts.FindText("str_garrison", null);

		private static readonly TextObject LandsAroundSettlementText = GameTexts.FindText("str_lands_around_settlement", null);

		private static readonly TextObject NormalVillagesText = GameTexts.FindText("str_normal_villages", null);

		private static readonly TextObject RaidedVillagesText = GameTexts.FindText("str_raided_villages", null);

		private static readonly TextObject VillagesUnderSiegeText = GameTexts.FindText("str_villages_under_siege", null);

		private static readonly TextObject FoodBoughtByCiviliansText = GameTexts.FindText("str_food_bought_by_civilians", null);

		private const int FoodProductionPerVillage = 10;
	}
}
