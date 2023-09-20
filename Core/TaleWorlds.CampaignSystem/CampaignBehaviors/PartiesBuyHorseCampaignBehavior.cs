using System;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Extensions;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.CampaignBehaviors
{
	public class PartiesBuyHorseCampaignBehavior : CampaignBehaviorBase
	{
		public override void RegisterEvents()
		{
			CampaignEvents.SettlementEntered.AddNonSerializedListener(this, new Action<MobileParty, Settlement, Hero>(this.OnSettlementEntered));
			CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, new Action(this.OnDailyTick));
			CampaignEvents.OnGameLoadedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnGameStarted));
			CampaignEvents.OnNewGameCreatedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnGameStarted));
		}

		public override void SyncData(IDataStore dataStore)
		{
		}

		private void OnGameStarted(CampaignGameStarter obj)
		{
			this.CalculateAverageHorsePrice();
		}

		private void OnDailyTick()
		{
			this.CalculateAverageHorsePrice();
		}

		private void CalculateAverageHorsePrice()
		{
			int num = 0;
			int num2 = 0;
			for (int i = 0; i < Items.All.Count; i++)
			{
				ItemObject itemObject = Items.All[i];
				if (itemObject.ItemCategory == DefaultItemCategories.Horse)
				{
					num += itemObject.Value;
					num2++;
				}
			}
			if (num2 == 0)
			{
				this._averageHorsePrice = 0f;
				return;
			}
			this._averageHorsePrice = (float)(num / num2);
		}

		public void OnSettlementEntered(MobileParty mobileParty, Settlement settlement, Hero hero)
		{
			if (mobileParty != null && mobileParty.MapFaction != null && !mobileParty.MapFaction.IsAtWarWith(settlement.MapFaction) && mobileParty != MobileParty.MainParty && mobileParty.IsLordParty && mobileParty.LeaderHero != null && !mobileParty.IsDisbanding && settlement.IsTown)
			{
				int num = MathF.Min(100000, mobileParty.LeaderHero.Gold);
				int numberOfMounts = mobileParty.Party.NumberOfMounts;
				if (numberOfMounts > mobileParty.Party.NumberOfRegularMembers)
				{
					return;
				}
				Town town = settlement.Town;
				if (town.MarketData.GetItemCountOfCategory(DefaultItemCategories.Horse) == 0)
				{
					return;
				}
				float num2 = this._averageHorsePrice * (float)numberOfMounts / (float)num;
				if (num2 < 0.08f)
				{
					float randomFloat = MBRandom.RandomFloat;
					float randomFloat2 = MBRandom.RandomFloat;
					float randomFloat3 = MBRandom.RandomFloat;
					float num3 = (0.08f - num2) * (float)num * randomFloat * randomFloat2 * randomFloat3;
					if (num3 > (float)(mobileParty.Party.NumberOfRegularMembers - numberOfMounts) * this._averageHorsePrice)
					{
						num3 = (float)(mobileParty.Party.NumberOfRegularMembers - numberOfMounts) * this._averageHorsePrice;
					}
					this.BuyHorses(mobileParty, town, num3);
				}
			}
			if (mobileParty != null && mobileParty != MobileParty.MainParty && mobileParty.IsLordParty && mobileParty.LeaderHero != null && !mobileParty.IsDisbanding && settlement.IsTown)
			{
				float num4 = 0f;
				for (int i = mobileParty.ItemRoster.Count - 1; i >= 0; i--)
				{
					ItemRosterElement itemRosterElement = mobileParty.ItemRoster[i];
					if (itemRosterElement.EquipmentElement.Item.IsMountable)
					{
						num4 += (float)(itemRosterElement.Amount * itemRosterElement.EquipmentElement.Item.Value);
					}
					else if (!itemRosterElement.EquipmentElement.Item.IsFood)
					{
						SellItemsAction.Apply(mobileParty.Party, settlement.Party, itemRosterElement, itemRosterElement.Amount, settlement);
					}
				}
				int num5 = MathF.Min(100000, mobileParty.LeaderHero.Gold);
				if (num4 > (float)num5 * 0.1f)
				{
					for (int j = 0; j < 10; j++)
					{
						ItemRosterElement itemRosterElement2 = default(ItemRosterElement);
						int num6 = 0;
						for (int k = 0; k < mobileParty.ItemRoster.Count; k++)
						{
							ItemRosterElement itemRosterElement3 = mobileParty.ItemRoster[k];
							if (itemRosterElement3.EquipmentElement.Item.IsMountable && itemRosterElement3.EquipmentElement.Item.Value > num6)
							{
								num6 = itemRosterElement3.EquipmentElement.Item.Value;
								itemRosterElement2 = itemRosterElement3;
							}
						}
						if (num6 <= 0)
						{
							break;
						}
						SellItemsAction.Apply(mobileParty.Party, settlement.Party, itemRosterElement2, 1, settlement);
						num4 -= (float)num6;
						if (num4 < (float)num5 * 0.1f)
						{
							break;
						}
					}
				}
			}
		}

		private void BuyHorses(MobileParty mobileParty, Town town, float budget)
		{
			for (int i = 0; i < 2; i++)
			{
				int num = -1;
				int num2 = 100000;
				ItemRoster itemRoster = town.Owner.ItemRoster;
				for (int j = 0; j < itemRoster.Count; j++)
				{
					if (itemRoster.GetItemAtIndex(j).ItemCategory == DefaultItemCategories.Horse)
					{
						int itemPrice = town.GetItemPrice(itemRoster.GetElementCopyAtIndex(j).EquipmentElement, mobileParty, false);
						if (itemPrice < num2)
						{
							num2 = itemPrice;
							num = j;
						}
					}
				}
				if (num >= 0)
				{
					ItemRosterElement elementCopyAtIndex = itemRoster.GetElementCopyAtIndex(num);
					int num3 = elementCopyAtIndex.Amount;
					if ((float)(num3 * num2) > budget)
					{
						num3 = MathF.Ceiling(budget / (float)num2);
					}
					int numberOfMounts = mobileParty.Party.NumberOfMounts;
					if (num3 > mobileParty.Party.NumberOfRegularMembers - numberOfMounts)
					{
						num3 = mobileParty.Party.NumberOfRegularMembers - numberOfMounts;
					}
					if (num3 > 0)
					{
						SellItemsAction.Apply(town.Owner, mobileParty.Party, elementCopyAtIndex, num3, town.Owner.Settlement);
					}
				}
			}
		}

		private float _averageHorsePrice;
	}
}
