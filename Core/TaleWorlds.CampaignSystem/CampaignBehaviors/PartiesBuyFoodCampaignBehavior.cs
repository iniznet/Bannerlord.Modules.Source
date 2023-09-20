using System;
using System.Collections.Generic;
using Helpers;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.CampaignBehaviors
{
	public class PartiesBuyFoodCampaignBehavior : CampaignBehaviorBase
	{
		public override void RegisterEvents()
		{
			CampaignEvents.SettlementEntered.AddNonSerializedListener(this, new Action<MobileParty, Settlement, Hero>(this.OnSettlementEntered));
			CampaignEvents.HourlyTickPartyEvent.AddNonSerializedListener(this, new Action<MobileParty>(this.HourlyTickParty));
		}

		public override void SyncData(IDataStore dataStore)
		{
		}

		private void TryBuyingFood(MobileParty mobileParty, Settlement settlement)
		{
			if (Campaign.Current.GameStarted && mobileParty.LeaderHero != null && (settlement.IsTown || settlement.IsVillage) && Campaign.Current.Models.MobilePartyFoodConsumptionModel.DoesPartyConsumeFood(mobileParty) && (mobileParty.Army == null || mobileParty.Army.LeaderParty == mobileParty) && (settlement.IsVillage || (mobileParty.MapFaction != null && !mobileParty.MapFaction.IsAtWarWith(settlement.MapFaction))) && settlement.ItemRoster.TotalFood > 0)
			{
				PartyFoodBuyingModel partyFoodBuyingModel = Campaign.Current.Models.PartyFoodBuyingModel;
				float num = (settlement.IsVillage ? partyFoodBuyingModel.MinimumDaysFoodToLastWhileBuyingFoodFromVillage : partyFoodBuyingModel.MinimumDaysFoodToLastWhileBuyingFoodFromTown);
				if (mobileParty.Army == null)
				{
					this.BuyFoodInternal(mobileParty, settlement, this.CalculateFoodCountToBuy(mobileParty, num));
					return;
				}
				this.BuyFoodForArmy(mobileParty, settlement, num);
			}
		}

		private int CalculateFoodCountToBuy(MobileParty mobileParty, float minimumDaysToLast)
		{
			float num = (float)mobileParty.TotalFoodAtInventory / -mobileParty.FoodChange;
			float num2 = minimumDaysToLast - num;
			if (num2 > 0f)
			{
				return (int)(-mobileParty.FoodChange * num2);
			}
			return 0;
		}

		private void BuyFoodInternal(MobileParty mobileParty, Settlement settlement, int numberOfFoodItemsNeededToBuy)
		{
			if (!mobileParty.IsMainParty)
			{
				for (int i = 0; i < numberOfFoodItemsNeededToBuy; i++)
				{
					ItemRosterElement itemRosterElement;
					float num;
					Campaign.Current.Models.PartyFoodBuyingModel.FindItemToBuy(mobileParty, settlement, out itemRosterElement, out num);
					if (itemRosterElement.EquipmentElement.Item == null)
					{
						break;
					}
					if (num <= (float)mobileParty.LeaderHero.Gold)
					{
						SellItemsAction.Apply(settlement.Party, mobileParty.Party, itemRosterElement, 1, null);
					}
					if (itemRosterElement.EquipmentElement.Item.HasHorseComponent && itemRosterElement.EquipmentElement.Item.HorseComponent.IsLiveStock)
					{
						i += itemRosterElement.EquipmentElement.Item.HorseComponent.MeatCount - 1;
					}
				}
			}
		}

		private void BuyFoodForArmy(MobileParty mobileParty, Settlement settlement, float minimumDaysToLast)
		{
			float num = mobileParty.Army.LeaderParty.FoodChange;
			foreach (MobileParty mobileParty2 in mobileParty.Army.LeaderParty.AttachedParties)
			{
				num += mobileParty2.FoodChange;
			}
			List<ValueTuple<int, int>> list = new List<ValueTuple<int, int>>(mobileParty.Army.Parties.Count);
			float num2 = mobileParty.Army.LeaderParty.FoodChange / num;
			int num3 = this.CalculateFoodCountToBuy(mobileParty.Army.LeaderParty, minimumDaysToLast);
			list.Add(new ValueTuple<int, int>((int)((float)settlement.ItemRoster.TotalFood * num2), num3));
			int num4 = num3;
			foreach (MobileParty mobileParty3 in mobileParty.Army.LeaderParty.AttachedParties)
			{
				num2 = mobileParty3.FoodChange / num;
				num3 = this.CalculateFoodCountToBuy(mobileParty3, minimumDaysToLast);
				list.Add(new ValueTuple<int, int>((int)((float)settlement.ItemRoster.TotalFood * num2), num3));
				num4 += num3;
			}
			bool flag = settlement.ItemRoster.TotalFood < num4;
			int num5 = 0;
			foreach (ValueTuple<int, int> valueTuple in list)
			{
				int num6 = (flag ? valueTuple.Item1 : valueTuple.Item2);
				MobileParty mobileParty4 = ((num5 == 0) ? mobileParty.Army.LeaderParty : mobileParty.Army.LeaderParty.AttachedParties[num5 - 1]);
				if (!mobileParty4.IsMainParty)
				{
					this.BuyFoodInternal(mobileParty4, settlement, num6);
				}
				num5++;
			}
		}

		public void HourlyTickParty(MobileParty mobileParty)
		{
			Settlement currentSettlementOfMobilePartyForAICalculation = MobilePartyHelper.GetCurrentSettlementOfMobilePartyForAICalculation(mobileParty);
			if (currentSettlementOfMobilePartyForAICalculation != null)
			{
				this.TryBuyingFood(mobileParty, currentSettlementOfMobilePartyForAICalculation);
			}
		}

		public void OnSettlementEntered(MobileParty mobileParty, Settlement settlement, Hero hero)
		{
			if (mobileParty != null)
			{
				this.TryBuyingFood(mobileParty, settlement);
			}
		}
	}
}
