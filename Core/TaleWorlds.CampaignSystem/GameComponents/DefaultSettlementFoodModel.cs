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
	// Token: 0x02000136 RID: 310
	public class DefaultSettlementFoodModel : SettlementFoodModel
	{
		// Token: 0x1700061A RID: 1562
		// (get) Token: 0x06001708 RID: 5896 RVA: 0x000717E0 File Offset: 0x0006F9E0
		public override int FoodStocksUpperLimit
		{
			get
			{
				return 100;
			}
		}

		// Token: 0x1700061B RID: 1563
		// (get) Token: 0x06001709 RID: 5897 RVA: 0x000717E4 File Offset: 0x0006F9E4
		public override int NumberOfProsperityToEatOneFood
		{
			get
			{
				return 40;
			}
		}

		// Token: 0x1700061C RID: 1564
		// (get) Token: 0x0600170A RID: 5898 RVA: 0x000717E8 File Offset: 0x0006F9E8
		public override int NumberOfMenOnGarrisonToEatOneFood
		{
			get
			{
				return 20;
			}
		}

		// Token: 0x1700061D RID: 1565
		// (get) Token: 0x0600170B RID: 5899 RVA: 0x000717EC File Offset: 0x0006F9EC
		public override int CastleFoodStockUpperLimitBonus
		{
			get
			{
				return 150;
			}
		}

		// Token: 0x0600170C RID: 5900 RVA: 0x000717F3 File Offset: 0x0006F9F3
		public override ExplainedNumber CalculateTownFoodStocksChange(Town town, bool includeMarketStocks = true, bool includeDescriptions = false)
		{
			return this.CalculateTownFoodChangeInternal(town, includeMarketStocks, includeDescriptions);
		}

		// Token: 0x0600170D RID: 5901 RVA: 0x00071800 File Offset: 0x0006FA00
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

		// Token: 0x0600170E RID: 5902 RVA: 0x00071B1C File Offset: 0x0006FD1C
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

		// Token: 0x0600170F RID: 5903 RVA: 0x00071B48 File Offset: 0x0006FD48
		private static void GetSettlementFoodChangeDueToIssues(Town town, ref ExplainedNumber explainedNumber)
		{
			Campaign.Current.Models.IssueModel.GetIssueEffectsOfSettlement(DefaultIssueEffects.SettlementFood, town.Settlement, ref explainedNumber);
		}

		// Token: 0x0400081F RID: 2079
		private static readonly TextObject ProsperityText = GameTexts.FindText("str_prosperity", null);

		// Token: 0x04000820 RID: 2080
		private static readonly TextObject GarrisonText = GameTexts.FindText("str_garrison", null);

		// Token: 0x04000821 RID: 2081
		private static readonly TextObject LandsAroundSettlementText = GameTexts.FindText("str_lands_around_settlement", null);

		// Token: 0x04000822 RID: 2082
		private static readonly TextObject NormalVillagesText = GameTexts.FindText("str_normal_villages", null);

		// Token: 0x04000823 RID: 2083
		private static readonly TextObject RaidedVillagesText = GameTexts.FindText("str_raided_villages", null);

		// Token: 0x04000824 RID: 2084
		private static readonly TextObject VillagesUnderSiegeText = GameTexts.FindText("str_villages_under_siege", null);

		// Token: 0x04000825 RID: 2085
		private static readonly TextObject FoodBoughtByCiviliansText = GameTexts.FindText("str_food_bought_by_civilians", null);

		// Token: 0x04000826 RID: 2086
		private const int FoodProductionPerVillage = 10;
	}
}
