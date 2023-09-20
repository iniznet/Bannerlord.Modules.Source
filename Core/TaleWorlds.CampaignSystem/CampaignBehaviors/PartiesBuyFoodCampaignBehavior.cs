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
	// Token: 0x020003B4 RID: 948
	public class PartiesBuyFoodCampaignBehavior : CampaignBehaviorBase
	{
		// Token: 0x06003889 RID: 14473 RVA: 0x001013FE File Offset: 0x000FF5FE
		public override void RegisterEvents()
		{
			CampaignEvents.SettlementEntered.AddNonSerializedListener(this, new Action<MobileParty, Settlement, Hero>(this.OnSettlementEntered));
			CampaignEvents.HourlyTickPartyEvent.AddNonSerializedListener(this, new Action<MobileParty>(this.HourlyTickParty));
		}

		// Token: 0x0600388A RID: 14474 RVA: 0x0010142E File Offset: 0x000FF62E
		public override void SyncData(IDataStore dataStore)
		{
		}

		// Token: 0x0600388B RID: 14475 RVA: 0x00101430 File Offset: 0x000FF630
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

		// Token: 0x0600388C RID: 14476 RVA: 0x00101520 File Offset: 0x000FF720
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

		// Token: 0x0600388D RID: 14477 RVA: 0x00101558 File Offset: 0x000FF758
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

		// Token: 0x0600388E RID: 14478 RVA: 0x00101620 File Offset: 0x000FF820
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

		// Token: 0x0600388F RID: 14479 RVA: 0x00101814 File Offset: 0x000FFA14
		public void HourlyTickParty(MobileParty mobileParty)
		{
			Settlement currentSettlementOfMobilePartyForAICalculation = MobilePartyHelper.GetCurrentSettlementOfMobilePartyForAICalculation(mobileParty);
			if (currentSettlementOfMobilePartyForAICalculation != null)
			{
				this.TryBuyingFood(mobileParty, currentSettlementOfMobilePartyForAICalculation);
			}
		}

		// Token: 0x06003890 RID: 14480 RVA: 0x00101833 File Offset: 0x000FFA33
		public void OnSettlementEntered(MobileParty mobileParty, Settlement settlement, Hero hero)
		{
			if (mobileParty != null)
			{
				this.TryBuyingFood(mobileParty, settlement);
			}
		}
	}
}
