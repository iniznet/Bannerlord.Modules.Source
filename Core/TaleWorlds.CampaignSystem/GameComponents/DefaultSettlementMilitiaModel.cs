using System;
using System.Linq;
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
	// Token: 0x02000139 RID: 313
	public class DefaultSettlementMilitiaModel : SettlementMilitiaModel
	{
		// Token: 0x06001742 RID: 5954 RVA: 0x00072B9D File Offset: 0x00070D9D
		public override ExplainedNumber CalculateMilitiaChange(Settlement settlement, bool includeDescriptions = false)
		{
			return DefaultSettlementMilitiaModel.CalculateMilitiaChangeInternal(settlement, includeDescriptions);
		}

		// Token: 0x06001743 RID: 5955 RVA: 0x00072BA8 File Offset: 0x00070DA8
		public override float CalculateEliteMilitiaSpawnChance(Settlement settlement)
		{
			float num = 0f;
			Hero hero = null;
			if (settlement.IsFortification && settlement.Town.Governor != null)
			{
				hero = settlement.Town.Governor;
			}
			else if (settlement.IsVillage)
			{
				Settlement tradeBound = settlement.Village.TradeBound;
				if (((tradeBound != null) ? tradeBound.Town.Governor : null) != null)
				{
					hero = settlement.Village.TradeBound.Town.Governor;
				}
			}
			if (hero != null && hero.GetPerkValue(DefaultPerks.Leadership.CitizenMilitia))
			{
				num += DefaultPerks.Leadership.CitizenMilitia.PrimaryBonus;
			}
			return num;
		}

		// Token: 0x06001744 RID: 5956 RVA: 0x00072C3A File Offset: 0x00070E3A
		public override void CalculateMilitiaSpawnRate(Settlement settlement, out float meleeTroopRate, out float rangedTroopRate)
		{
			meleeTroopRate = 0.5f;
			rangedTroopRate = 1f - meleeTroopRate;
		}

		// Token: 0x06001745 RID: 5957 RVA: 0x00072C50 File Offset: 0x00070E50
		private static ExplainedNumber CalculateMilitiaChangeInternal(Settlement settlement, bool includeDescriptions = false)
		{
			ExplainedNumber explainedNumber = new ExplainedNumber(0f, includeDescriptions, null);
			if (settlement.Party.MapEvent == null)
			{
				float militia = settlement.Militia;
				if (settlement.IsFortification)
				{
					explainedNumber.Add(2f, DefaultSettlementMilitiaModel.BaseText, null);
				}
				float num = -militia * 0.025f;
				explainedNumber.Add(num, DefaultSettlementMilitiaModel.RetiredText, null);
				if (settlement.IsVillage)
				{
					float num2 = settlement.Village.Hearth / 400f;
					explainedNumber.Add(num2, DefaultSettlementMilitiaModel.FromHearthsText, null);
				}
				else if (settlement.IsFortification)
				{
					float num3 = settlement.Town.Prosperity / 1000f;
					explainedNumber.Add(num3, DefaultSettlementMilitiaModel.FromProsperityText, null);
					if (settlement.Town.InRebelliousState)
					{
						float num4 = MBMath.Map(settlement.Town.Loyalty, 0f, (float)Campaign.Current.Models.SettlementLoyaltyModel.RebelliousStateStartLoyaltyThreshold, (float)Campaign.Current.Models.SettlementLoyaltyModel.MilitiaBoostPercentage, 0f);
						float num5 = MathF.Abs(num3 * (num4 * 0.01f));
						explainedNumber.Add(num5, DefaultSettlementMilitiaModel.LowLoyaltyText, null);
					}
				}
				if (settlement.IsTown)
				{
					int num6 = settlement.Town.SoldItems.Sum(delegate(Town.SellLog x)
					{
						if (x.Category.Properties != ItemCategory.Property.BonusToMilitia)
						{
							return 0;
						}
						return x.Number;
					});
					if (num6 > 0)
					{
						explainedNumber.Add(0.2f * (float)num6, DefaultSettlementMilitiaModel.MilitiaFromMarketText, null);
					}
					if (settlement.OwnerClan.Kingdom != null)
					{
						if (settlement.OwnerClan.Kingdom.ActivePolicies.Contains(DefaultPolicies.Serfdom) && settlement.IsTown)
						{
							explainedNumber.Add(-1f, DefaultPolicies.Serfdom.Name, null);
						}
						if (settlement.OwnerClan.Kingdom.ActivePolicies.Contains(DefaultPolicies.Cantons))
						{
							explainedNumber.Add(1f, DefaultPolicies.Cantons.Name, null);
						}
					}
					if (settlement.OwnerClan.Culture.HasFeat(DefaultCulturalFeats.BattanianMilitiaFeat))
					{
						explainedNumber.Add(DefaultCulturalFeats.BattanianMilitiaFeat.EffectBonus, DefaultSettlementMilitiaModel.CultureText, null);
					}
				}
				if (settlement.IsCastle || settlement.IsTown)
				{
					if (settlement.Town.BuildingsInProgress.IsEmpty<Building>())
					{
						BuildingHelper.AddDefaultDailyBonus(settlement.Town, BuildingEffectEnum.MilitiaDaily, ref explainedNumber);
					}
					foreach (Building building in settlement.Town.Buildings)
					{
						if (!building.BuildingType.IsDefaultProject)
						{
							float buildingEffectAmount = building.GetBuildingEffectAmount(BuildingEffectEnum.Militia);
							if (buildingEffectAmount > 0f)
							{
								explainedNumber.Add(buildingEffectAmount, building.Name, null);
							}
						}
					}
					if (settlement.IsCastle && settlement.Town.InRebelliousState)
					{
						float resultNumber = explainedNumber.ResultNumber;
						float num7 = 0f;
						foreach (Building building2 in settlement.Town.Buildings)
						{
							if (num7 < 1f && (!building2.BuildingType.IsDefaultProject || settlement.Town.CurrentBuilding == building2))
							{
								float buildingEffectAmount2 = building2.GetBuildingEffectAmount(BuildingEffectEnum.ReduceMilitia);
								if (buildingEffectAmount2 > 0f)
								{
									float num8 = buildingEffectAmount2 * 0.01f;
									num7 += num8;
									if (num7 > 1f)
									{
										num8 -= num7 - 1f;
									}
									float num9 = resultNumber * -num8;
									explainedNumber.Add(num9, building2.Name, null);
								}
							}
						}
					}
					DefaultSettlementMilitiaModel.GetSettlementMilitiaChangeDueToPolicies(settlement, ref explainedNumber);
					DefaultSettlementMilitiaModel.GetSettlementMilitiaChangeDueToPerks(settlement, ref explainedNumber);
					DefaultSettlementMilitiaModel.GetSettlementMilitiaChangeDueToIssues(settlement, ref explainedNumber);
				}
			}
			return explainedNumber;
		}

		// Token: 0x06001746 RID: 5958 RVA: 0x00073030 File Offset: 0x00071230
		private static void GetSettlementMilitiaChangeDueToPerks(Settlement settlement, ref ExplainedNumber result)
		{
			if (settlement.Town != null && settlement.Town.Governor != null)
			{
				PerkHelper.AddPerkBonusForTown(DefaultPerks.OneHanded.SwiftStrike, settlement.Town, ref result);
				PerkHelper.AddPerkBonusForTown(DefaultPerks.Polearm.KeepAtBay, settlement.Town, ref result);
				PerkHelper.AddPerkBonusForTown(DefaultPerks.Polearm.Drills, settlement.Town, ref result);
				PerkHelper.AddPerkBonusForTown(DefaultPerks.Bow.MerryMen, settlement.Town, ref result);
				PerkHelper.AddPerkBonusForTown(DefaultPerks.Crossbow.LongShots, settlement.Town, ref result);
				PerkHelper.AddPerkBonusForTown(DefaultPerks.Throwing.ThrowingCompetitions, settlement.Town, ref result);
				if (settlement.IsUnderSiege)
				{
					PerkHelper.AddPerkBonusForTown(DefaultPerks.Roguery.ArmsDealer, settlement.Town, ref result);
				}
				PerkHelper.AddPerkBonusForTown(DefaultPerks.Steward.SevenVeterans, settlement.Town, ref result);
			}
		}

		// Token: 0x06001747 RID: 5959 RVA: 0x000730E8 File Offset: 0x000712E8
		private static void GetSettlementMilitiaChangeDueToPolicies(Settlement settlement, ref ExplainedNumber result)
		{
			Kingdom kingdom = settlement.OwnerClan.Kingdom;
			if (kingdom != null && kingdom.ActivePolicies.Contains(DefaultPolicies.Citizenship))
			{
				result.Add(1f, DefaultPolicies.Citizenship.Name, null);
			}
		}

		// Token: 0x06001748 RID: 5960 RVA: 0x0007312C File Offset: 0x0007132C
		private static void GetSettlementMilitiaChangeDueToIssues(Settlement settlement, ref ExplainedNumber result)
		{
			Campaign.Current.Models.IssueModel.GetIssueEffectsOfSettlement(DefaultIssueEffects.SettlementMilitia, settlement, ref result);
		}

		// Token: 0x04000843 RID: 2115
		private static readonly TextObject BaseText = new TextObject("{=militarybase}Base", null);

		// Token: 0x04000844 RID: 2116
		private static readonly TextObject FromHearthsText = new TextObject("{=ecdZglky}From Hearths", null);

		// Token: 0x04000845 RID: 2117
		private static readonly TextObject FromProsperityText = new TextObject("{=cTmiNAlI}From Prosperity", null);

		// Token: 0x04000846 RID: 2118
		private static readonly TextObject RetiredText = new TextObject("{=gHnfFi1s}Retired", null);

		// Token: 0x04000847 RID: 2119
		private static readonly TextObject MilitiaFromMarketText = new TextObject("{=7ve3bQxg}Weapons From Market", null);

		// Token: 0x04000848 RID: 2120
		private static readonly TextObject FoodShortageText = new TextObject("{=qTFKvGSg}Food Shortage", null);

		// Token: 0x04000849 RID: 2121
		private static readonly TextObject LowLoyaltyText = new TextObject("{=SJ2qsRdF}Low Loyalty", null);

		// Token: 0x0400084A RID: 2122
		private static readonly TextObject CultureText = GameTexts.FindText("str_culture", null);
	}
}
