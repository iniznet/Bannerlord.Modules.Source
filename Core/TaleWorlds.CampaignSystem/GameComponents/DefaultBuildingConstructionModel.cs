using System;
using System.Linq;
using Helpers;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Buildings;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.GameComponents
{
	public class DefaultBuildingConstructionModel : BuildingConstructionModel
	{
		public override int TownBoostCost
		{
			get
			{
				return 500;
			}
		}

		public override int TownBoostBonus
		{
			get
			{
				return 50;
			}
		}

		public override int CastleBoostCost
		{
			get
			{
				return 250;
			}
		}

		public override int CastleBoostBonus
		{
			get
			{
				return 20;
			}
		}

		public override ExplainedNumber CalculateDailyConstructionPower(Town town, bool includeDescriptions = false)
		{
			ExplainedNumber explainedNumber = new ExplainedNumber(0f, includeDescriptions, null);
			this.CalculateDailyConstructionPowerInternal(town, ref explainedNumber, false);
			return explainedNumber;
		}

		public override int CalculateDailyConstructionPowerWithoutBoost(Town town)
		{
			ExplainedNumber explainedNumber = new ExplainedNumber(0f, false, null);
			return this.CalculateDailyConstructionPowerInternal(town, ref explainedNumber, true);
		}

		public override int GetBoostAmount(Town town)
		{
			object obj = (town.IsCastle ? this.CastleBoostBonus : this.TownBoostBonus);
			float num = 0f;
			if (town.Governor != null && town.Governor.GetPerkValue(DefaultPerks.Steward.Relocation))
			{
				num += DefaultPerks.Steward.Relocation.SecondaryBonus;
			}
			if (town.Governor != null && town.Governor.GetPerkValue(DefaultPerks.Trade.SpringOfGold))
			{
				num += DefaultPerks.Trade.SpringOfGold.SecondaryBonus;
			}
			object obj2 = obj;
			return obj2 + (int)(obj2 * num);
		}

		public override int GetBoostCost(Town town)
		{
			if (!town.IsCastle)
			{
				return this.TownBoostCost;
			}
			return this.CastleBoostCost;
		}

		private int CalculateDailyConstructionPowerInternal(Town town, ref ExplainedNumber result, bool omitBoost = false)
		{
			float num = town.Prosperity * 0.01f;
			result.Add(num, GameTexts.FindText("str_prosperity", null), null);
			if (!omitBoost && town.BoostBuildingProcess > 0)
			{
				int num2 = (town.IsCastle ? this.CastleBoostCost : this.TownBoostCost);
				int num3 = this.GetBoostAmount(town);
				float num4 = MathF.Min(1f, (float)town.BoostBuildingProcess / (float)num2);
				float num5 = 0f;
				if (town.IsTown && town.Governor != null && town.Governor.GetPerkValue(DefaultPerks.Engineering.Clockwork))
				{
					num5 += DefaultPerks.Engineering.Clockwork.SecondaryBonus;
				}
				num3 += MathF.Round((float)num3 * num5);
				result.Add((float)num3 * num4, DefaultBuildingConstructionModel.BoostText, null);
			}
			if (town.Governor != null)
			{
				Settlement currentSettlement = town.Governor.CurrentSettlement;
				if (((currentSettlement != null) ? currentSettlement.Town : null) == town)
				{
					SkillHelper.AddSkillBonusForTown(DefaultSkills.Engineering, DefaultSkillEffects.TownProjectBuildingBonus, town, ref result);
					PerkHelper.AddPerkBonusForTown(DefaultPerks.Steward.ForcedLabor, town, ref result);
				}
			}
			if (town.Governor != null)
			{
				Settlement currentSettlement2 = town.Governor.CurrentSettlement;
				if (((currentSettlement2 != null) ? currentSettlement2.Town : null) == town && !town.BuildingsInProgress.IsEmpty<Building>())
				{
					if (town.Governor.GetPerkValue(DefaultPerks.Steward.ForcedLabor) && town.Settlement.Party.PrisonRoster.TotalManCount > 0)
					{
						float num6 = MathF.Min(0.3f, (float)(town.Settlement.Party.PrisonRoster.TotalManCount / 3) * DefaultPerks.Steward.ForcedLabor.SecondaryBonus);
						result.AddFactor(num6, DefaultPerks.Steward.ForcedLabor.Name);
					}
					if (town.IsCastle && town.Governor.GetPerkValue(DefaultPerks.Engineering.MilitaryPlanner))
					{
						result.AddFactor(DefaultPerks.Engineering.MilitaryPlanner.SecondaryBonus, DefaultPerks.Engineering.MilitaryPlanner.Name);
					}
					else if (town.IsTown && town.Governor.GetPerkValue(DefaultPerks.Engineering.Carpenters))
					{
						result.AddFactor(DefaultPerks.Engineering.Carpenters.SecondaryBonus, DefaultPerks.Engineering.Carpenters.Name);
					}
					Building building = town.BuildingsInProgress.Peek();
					if ((building.BuildingType == DefaultBuildingTypes.Fortifications || building.BuildingType == DefaultBuildingTypes.CastleBarracks || building.BuildingType == DefaultBuildingTypes.CastleMilitiaBarracks || building.BuildingType == DefaultBuildingTypes.SettlementGarrisonBarracks || building.BuildingType == DefaultBuildingTypes.SettlementMilitiaBarracks || building.BuildingType == DefaultBuildingTypes.SettlementAquaducts) && town.Governor.GetPerkValue(DefaultPerks.Engineering.Stonecutters))
					{
						result.AddFactor(DefaultPerks.Engineering.Stonecutters.PrimaryBonus, DefaultPerks.Engineering.Stonecutters.Name);
					}
				}
			}
			SettlementLoyaltyModel settlementLoyaltyModel = Campaign.Current.Models.SettlementLoyaltyModel;
			int num7 = town.SoldItems.Sum(delegate(Town.SellLog x)
			{
				if (x.Category.Properties != ItemCategory.Property.BonusToProduction)
				{
					return 0;
				}
				return x.Number;
			});
			if (num7 > 0)
			{
				result.Add(0.25f * (float)num7, DefaultBuildingConstructionModel.ProductionFromMarketText, null);
			}
			BuildingType buildingType = (town.BuildingsInProgress.IsEmpty<Building>() ? null : town.BuildingsInProgress.Peek().BuildingType);
			if (DefaultBuildingTypes.MilitaryBuildings.Contains(buildingType))
			{
				PerkHelper.AddPerkBonusForTown(DefaultPerks.TwoHanded.Confidence, town, ref result);
			}
			if (buildingType == DefaultBuildingTypes.SettlementMarketplace || buildingType == DefaultBuildingTypes.SettlementAquaducts || buildingType == DefaultBuildingTypes.SettlementLimeKilns)
			{
				PerkHelper.AddPerkBonusForTown(DefaultPerks.Trade.SelfMadeMan, town, ref result);
			}
			float effectOfBuildings = town.GetEffectOfBuildings(BuildingEffectEnum.Construction);
			if (effectOfBuildings > 0f)
			{
				result.Add(effectOfBuildings, GameTexts.FindText("str_building_bonus", null), null);
			}
			if (town.Loyalty >= 75f)
			{
				float num8 = MBMath.Map(town.Loyalty, 75f, 100f, 0f, 20f);
				float num9 = result.ResultNumber * (num8 / 100f);
				result.Add(num9, DefaultBuildingConstructionModel.HighLoyaltyBonusText, null);
			}
			else if (town.Loyalty > 25f && town.Loyalty <= 50f)
			{
				float num10 = MBMath.Map(town.Loyalty, 25f, 50f, 50f, 0f);
				float num11 = result.ResultNumber * (num10 / 100f);
				result.Add(-num11, DefaultBuildingConstructionModel.LowLoyaltyPenaltyText, null);
			}
			else if (town.Loyalty <= 25f)
			{
				result.Add(-result.ResultNumber, DefaultBuildingConstructionModel.VeryLowLoyaltyPenaltyText, null);
				result.LimitMax(0f);
			}
			if (town.Loyalty > 25f && town.OwnerClan.Culture.HasFeat(DefaultCulturalFeats.BattanianConstructionFeat))
			{
				result.AddFactor(DefaultCulturalFeats.BattanianConstructionFeat.EffectBonus, DefaultBuildingConstructionModel.CultureText);
			}
			result.LimitMin(0f);
			return (int)result.ResultNumber;
		}

		private const float HammerMultiplier = 0.01f;

		private const int VeryLowLoyaltyValue = 25;

		private const float MediumLoyaltyValue = 50f;

		private const float HighLoyaltyValue = 75f;

		private const float HighestLoyaltyValue = 100f;

		private static readonly TextObject ProductionFromMarketText = new TextObject("{=vaZDJGMx}Construction from Market", null);

		private static readonly TextObject BoostText = new TextObject("{=yX1RycON}Boost from Reserve", null);

		private static readonly TextObject HighLoyaltyBonusText = new TextObject("{=aSniKUJv}High Loyalty", null);

		private static readonly TextObject LowLoyaltyPenaltyText = new TextObject("{=SJ2qsRdF}Low Loyalty", null);

		private static readonly TextObject VeryLowLoyaltyPenaltyText = new TextObject("{=CcQzFnpN}Very Low Loyalty", null);

		private static readonly TextObject CultureText = GameTexts.FindText("str_culture", null);
	}
}
