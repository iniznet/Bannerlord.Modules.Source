using System;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Workshops;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.GameComponents
{
	public class DefaultWorkshopModel : WorkshopModel
	{
		public override int MaxWorkshopLevel
		{
			get
			{
				return 3;
			}
		}

		public override int DaysForPlayerSaveWorkshopFromBankruptcy
		{
			get
			{
				return 3;
			}
		}

		public override int GetInitialCapital(int level)
		{
			return 10000;
		}

		public override int GetDailyExpense(int level)
		{
			return 100;
		}

		public override float GetPolicyEffectToProduction(Town town)
		{
			float num = 1f;
			if (town.Settlement.OwnerClan.Kingdom != null)
			{
				if (town.Settlement.OwnerClan.Kingdom.ActivePolicies.Contains(DefaultPolicies.ForgivenessOfDebts))
				{
					num -= 0.05f;
				}
				if (town.Settlement.OwnerClan.Kingdom.ActivePolicies.Contains(DefaultPolicies.StateMonopolies))
				{
					num -= 0.1f;
				}
			}
			return num;
		}

		public override int GetUpgradeCost(int currentLevel)
		{
			return 5000;
		}

		public override int GetMaxWorkshopCountForTier(int tier)
		{
			return 1 + tier;
		}

		public override int GetBuyingCostForPlayer(Workshop workshop)
		{
			return workshop.WorkshopType.EquipmentCost + (int)workshop.Settlement.Prosperity * 3 + this.GetInitialCapital(workshop.Level);
		}

		public override int GetSellingCost(Workshop workshop)
		{
			return (workshop.WorkshopType.EquipmentCost + (int)workshop.Settlement.Prosperity / 2 + workshop.Capital) / 2;
		}

		public override Hero SelectNextOwnerForWorkshop(Town town, Workshop workshop, Hero excludedHero, int requiredGold = 0)
		{
			Hero hero = null;
			int num = int.MaxValue;
			float num2 = 0f;
			foreach (Hero hero2 in town.Settlement.Notables)
			{
				if (hero2 != excludedHero && hero2.Gold >= requiredGold)
				{
					int count = hero2.OwnedWorkshops.Count;
					float num3 = hero2.Power / (float)count;
					if (num3 > num2 || (MathF.Abs(num3 - num2) < 1E-45f && count < num))
					{
						hero = hero2;
						num = count;
					}
				}
			}
			return hero;
		}

		public override int GetConvertProductionCost(WorkshopType workshopType)
		{
			return 2000;
		}

		public override bool CanPlayerSellWorkshop(Workshop workshop, out TextObject explanation)
		{
			int sellingCost = Campaign.Current.Models.WorkshopModel.GetSellingCost(workshop);
			Hero hero = Campaign.Current.Models.WorkshopModel.SelectNextOwnerForWorkshop(workshop.Settlement.Town, workshop, workshop.Owner, sellingCost);
			explanation = ((hero == null) ? new TextObject("{=oqPf2Gdp}There isn't any prospective buyer in the town.", null) : TextObject.Empty);
			return hero != null;
		}
	}
}
