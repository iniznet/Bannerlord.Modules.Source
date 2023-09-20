using System;
using Helpers;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Workshops;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.GameComponents
{
	public class DefaultWorkshopModel : WorkshopModel
	{
		public override int WarehouseCapacity
		{
			get
			{
				return 6000;
			}
		}

		public override int DaysForPlayerSaveWorkshopFromBankruptcy
		{
			get
			{
				return 3;
			}
		}

		public override int CapitalLowLimit
		{
			get
			{
				return 5000;
			}
		}

		public override int InitialCapital
		{
			get
			{
				return 10000;
			}
		}

		public override int DailyExpense
		{
			get
			{
				return 100;
			}
		}

		public override int DefaultWorkshopCountInSettlement
		{
			get
			{
				return 4;
			}
		}

		public override int MaximumWorkshopsPlayerCanHave
		{
			get
			{
				return this.GetMaxWorkshopCountForClanTier(Campaign.Current.Models.ClanTierModel.MaxClanTier);
			}
		}

		public override ExplainedNumber GetEffectiveConversionSpeedOfProduction(Workshop workshop, float speed, bool includeDescription)
		{
			ExplainedNumber explainedNumber = new ExplainedNumber(speed, includeDescription, new TextObject("{=basevalue}Base", null));
			Settlement settlement = workshop.Settlement;
			if (settlement.OwnerClan.Kingdom != null)
			{
				if (settlement.OwnerClan.Kingdom.ActivePolicies.Contains(DefaultPolicies.ForgivenessOfDebts))
				{
					explainedNumber.AddFactor(-0.05f, DefaultPolicies.ForgivenessOfDebts.Name);
				}
				if (settlement.OwnerClan.Kingdom.ActivePolicies.Contains(DefaultPolicies.StateMonopolies))
				{
					explainedNumber.AddFactor(-0.1f, DefaultPolicies.StateMonopolies.Name);
				}
			}
			PerkHelper.AddPerkBonusForTown(DefaultPerks.Trade.MercenaryConnections, settlement.Town, ref explainedNumber);
			PerkHelper.AddPerkBonusForCharacter(DefaultPerks.Steward.Sweatshops, workshop.Owner.CharacterObject, true, ref explainedNumber);
			return explainedNumber;
		}

		public override int GetMaxWorkshopCountForClanTier(int tier)
		{
			return tier + 1;
		}

		public override int GetCostForPlayer(Workshop workshop)
		{
			return workshop.WorkshopType.EquipmentCost + (int)workshop.Settlement.Town.Prosperity * 3 + this.InitialCapital;
		}

		public override int GetCostForNotable(Workshop workshop)
		{
			return (workshop.WorkshopType.EquipmentCost + (int)workshop.Settlement.Town.Prosperity / 2 + workshop.Capital) / 2;
		}

		public override Hero GetNotableOwnerForWorkshop(Settlement settlement)
		{
			Hero hero = null;
			float num = 0f;
			foreach (Hero hero2 in settlement.Notables)
			{
				int count = hero2.OwnedWorkshops.Count;
				float num2 = hero2.Power / MathF.Pow(10f, (float)count);
				num += num2;
			}
			num *= MBRandom.RandomFloat;
			foreach (Hero hero3 in settlement.Notables)
			{
				int count2 = hero3.OwnedWorkshops.Count;
				float num3 = hero3.Power / MathF.Pow(10f, (float)count2);
				num -= num3;
				if (num < 0f)
				{
					hero = hero3;
					break;
				}
			}
			return hero;
		}

		public override int GetConvertProductionCost(WorkshopType workshopType)
		{
			return workshopType.EquipmentCost;
		}

		public override bool CanPlayerSellWorkshop(Workshop workshop, out TextObject explanation)
		{
			Campaign.Current.Models.WorkshopModel.GetCostForNotable(workshop);
			Hero notableOwnerForWorkshop = Campaign.Current.Models.WorkshopModel.GetNotableOwnerForWorkshop(workshop.Settlement);
			explanation = ((notableOwnerForWorkshop == null) ? new TextObject("{=oqPf2Gdp}There isn't any prospective buyer in the town.", null) : TextObject.Empty);
			return notableOwnerForWorkshop != null;
		}

		public override float GetTradeXpPerWarehouseProduction(EquipmentElement production)
		{
			return (float)production.GetBaseValue() * 0.1f;
		}
	}
}
