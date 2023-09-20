using System;
using System.Collections.Generic;
using Helpers;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Extensions;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.CampaignBehaviors
{
	// Token: 0x020003A6 RID: 934
	public class ItemConsumptionBehavior : CampaignBehaviorBase
	{
		// Token: 0x060037B4 RID: 14260 RVA: 0x000FAB58 File Offset: 0x000F8D58
		public override void RegisterEvents()
		{
			CampaignEvents.OnNewGameCreatedPartialFollowUpEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter, int>(this.OnNewGameCreatedFollowUp));
			CampaignEvents.OnNewGameCreatedPartialFollowUpEndEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnNewGameCreatedFollowUpEnd));
			CampaignEvents.DailyTickTownEvent.AddNonSerializedListener(this, new Action<Town>(this.DailyTickTown));
		}

		// Token: 0x060037B5 RID: 14261 RVA: 0x000FABAA File Offset: 0x000F8DAA
		private void OnNewGameCreatedFollowUp(CampaignGameStarter starter, int i)
		{
			if (i < 2)
			{
				this.MakeConsumptionAllTowns();
			}
		}

		// Token: 0x060037B6 RID: 14262 RVA: 0x000FABB8 File Offset: 0x000F8DB8
		private void OnNewGameCreatedFollowUpEnd(CampaignGameStarter starter)
		{
			Dictionary<ItemCategory, float> dictionary = new Dictionary<ItemCategory, float>();
			for (int i = 0; i < 10; i++)
			{
				foreach (Town town in Town.AllTowns)
				{
					ItemConsumptionBehavior.UpdateSupplyAndDemand(town);
					this.UpdateDemandShift(town, dictionary);
				}
			}
		}

		// Token: 0x060037B7 RID: 14263 RVA: 0x000FAC24 File Offset: 0x000F8E24
		public override void SyncData(IDataStore dataStore)
		{
		}

		// Token: 0x060037B8 RID: 14264 RVA: 0x000FAC28 File Offset: 0x000F8E28
		private void DailyTickTown(Town town)
		{
			Dictionary<ItemCategory, int> dictionary = new Dictionary<ItemCategory, int>();
			this.MakeConsumptionInTown(town, dictionary);
		}

		// Token: 0x060037B9 RID: 14265 RVA: 0x000FAC44 File Offset: 0x000F8E44
		private void MakeConsumptionAllTowns()
		{
			foreach (Town town in Town.AllTowns)
			{
				this.DailyTickTown(town);
			}
		}

		// Token: 0x17000CD1 RID: 3281
		// (get) Token: 0x060037BA RID: 14266 RVA: 0x000FAC98 File Offset: 0x000F8E98
		private ItemConsumptionBehavior.CategoryItems CategoryItemsCache
		{
			get
			{
				if (this._categoryItems == null)
				{
					this._categoryItems = new ItemConsumptionBehavior.CategoryItems();
				}
				return this._categoryItems;
			}
		}

		// Token: 0x060037BB RID: 14267 RVA: 0x000FACB4 File Offset: 0x000F8EB4
		private void MakeConsumptionInTown(Town town, Dictionary<ItemCategory, int> saleLog)
		{
			Dictionary<ItemCategory, float> dictionary = new Dictionary<ItemCategory, float>();
			this.DeleteOverproducedItems(town);
			ItemConsumptionBehavior.UpdateSupplyAndDemand(town);
			this.UpdateDemandShift(town, dictionary);
			ItemConsumptionBehavior.MakeConsumption(town, dictionary, saleLog);
			ItemConsumptionBehavior.GetFoodFromMarket(town, saleLog);
			this.UpdateSellLog(town, saleLog);
			this.UpdateTownGold(town);
		}

		// Token: 0x060037BC RID: 14268 RVA: 0x000FACFC File Offset: 0x000F8EFC
		private void UpdateTownGold(Town town)
		{
			int townGoldChange = Campaign.Current.Models.SettlementConsumptionModel.GetTownGoldChange(town);
			town.ChangeGold(townGoldChange);
		}

		// Token: 0x060037BD RID: 14269 RVA: 0x000FAD28 File Offset: 0x000F8F28
		private void DeleteOverproducedItems(Town town)
		{
			ItemRoster itemRoster = town.Owner.ItemRoster;
			for (int i = itemRoster.Count - 1; i >= 0; i--)
			{
				ItemRosterElement elementCopyAtIndex = itemRoster.GetElementCopyAtIndex(i);
				ItemObject item = elementCopyAtIndex.EquipmentElement.Item;
				int amount = elementCopyAtIndex.Amount;
				if (amount > 0 && (item.IsCraftedByPlayer || item.IsBannerItem))
				{
					itemRoster.AddToCounts(elementCopyAtIndex.EquipmentElement, -amount);
				}
				else if (elementCopyAtIndex.EquipmentElement.ItemModifier != null && MBRandom.RandomFloat < 0.5f)
				{
					itemRoster.AddToCounts(elementCopyAtIndex.EquipmentElement, -1);
				}
			}
		}

		// Token: 0x060037BE RID: 14270 RVA: 0x000FADCC File Offset: 0x000F8FCC
		private static void GetFoodFromMarket(Town town, Dictionary<ItemCategory, int> saleLog)
		{
			float foodChange = town.FoodChange;
			ValueTuple<int, int> townFoodAndMarketStocks = TownHelpers.GetTownFoodAndMarketStocks(town);
			int item = townFoodAndMarketStocks.Item1;
			int item2 = townFoodAndMarketStocks.Item2;
			if (town.IsTown && town.IsUnderSiege && foodChange < 0f && item <= 0 && item2 > 0)
			{
				ItemConsumptionBehavior.GetFoodFromMarketInternal(town, Math.Abs(MathF.Floor(foodChange)), saleLog);
			}
		}

		// Token: 0x060037BF RID: 14271 RVA: 0x000FAE28 File Offset: 0x000F9028
		private void UpdateSellLog(Town town, Dictionary<ItemCategory, int> saleLog)
		{
			List<Town.SellLog> list = new List<Town.SellLog>();
			foreach (KeyValuePair<ItemCategory, int> keyValuePair in saleLog)
			{
				if (keyValuePair.Value > 0)
				{
					list.Add(new Town.SellLog(keyValuePair.Key, keyValuePair.Value));
				}
			}
			town.SetSoldItems(list);
		}

		// Token: 0x060037C0 RID: 14272 RVA: 0x000FAEA0 File Offset: 0x000F90A0
		private static void GetFoodFromMarketInternal(Town town, int amount, Dictionary<ItemCategory, int> saleLog)
		{
			ItemRoster itemRoster = town.Owner.ItemRoster;
			int num = itemRoster.Count - 1;
			while (num >= 0 && amount > 0)
			{
				ItemRosterElement elementCopyAtIndex = itemRoster.GetElementCopyAtIndex(num);
				ItemObject item = elementCopyAtIndex.EquipmentElement.Item;
				if (item.ItemCategory.Properties == ItemCategory.Property.BonusToFoodStores)
				{
					int num2 = ((elementCopyAtIndex.Amount >= amount) ? amount : elementCopyAtIndex.Amount);
					amount -= num2;
					itemRoster.AddToCounts(elementCopyAtIndex.EquipmentElement, -num2);
					int num3 = 0;
					saleLog.TryGetValue(item.ItemCategory, out num3);
					saleLog[item.ItemCategory] = num3 + num2;
				}
				num--;
			}
		}

		// Token: 0x060037C1 RID: 14273 RVA: 0x000FAF4C File Offset: 0x000F914C
		private static void MakeConsumption(Town town, Dictionary<ItemCategory, float> categoryDemand, Dictionary<ItemCategory, int> saleLog)
		{
			saleLog.Clear();
			TownMarketData marketData = town.MarketData;
			ItemRoster itemRoster = town.Owner.ItemRoster;
			for (int i = itemRoster.Count - 1; i >= 0; i--)
			{
				ItemRosterElement elementCopyAtIndex = itemRoster.GetElementCopyAtIndex(i);
				ItemObject item = elementCopyAtIndex.EquipmentElement.Item;
				int amount = elementCopyAtIndex.Amount;
				ItemCategory itemCategory = item.GetItemCategory();
				float num = categoryDemand[itemCategory];
				float num2 = ItemConsumptionBehavior.CalculateBudget(town, num, itemCategory);
				if (num2 > 0.01f)
				{
					int price = marketData.GetPrice(item, null, false, null);
					float num3 = num2 / (float)price;
					if (num3 > (float)amount)
					{
						num3 = (float)amount;
					}
					int num4 = MBRandom.RoundRandomized(num3);
					if (num4 > amount)
					{
						num4 = amount;
					}
					itemRoster.AddToCounts(elementCopyAtIndex.EquipmentElement, -num4);
					categoryDemand[itemCategory] = num2 - num3 * (float)price;
					town.ChangeGold(num4 * price);
					int num5 = 0;
					saleLog.TryGetValue(itemCategory, out num5);
					saleLog[itemCategory] = num5 + num4;
				}
			}
		}

		// Token: 0x060037C2 RID: 14274 RVA: 0x000FB052 File Offset: 0x000F9252
		private static float CalculateBudget(Town town, float demand, ItemCategory category)
		{
			return demand * MathF.Pow(town.GetItemCategoryPriceIndex(category), 0.3f);
		}

		// Token: 0x060037C3 RID: 14275 RVA: 0x000FB068 File Offset: 0x000F9268
		private void UpdateDemandShift(Town town, Dictionary<ItemCategory, float> categoryBudget)
		{
			TownMarketData marketData = town.MarketData;
			foreach (ItemCategory itemCategory in ItemCategories.All)
			{
				categoryBudget[itemCategory] = Campaign.Current.Models.SettlementConsumptionModel.GetDailyDemandForCategory(town, itemCategory, 0);
			}
			foreach (ItemCategory itemCategory2 in ItemCategories.All)
			{
				if (itemCategory2.CanSubstitute != null)
				{
					ItemData categoryData = marketData.GetCategoryData(itemCategory2);
					ItemData categoryData2 = marketData.GetCategoryData(itemCategory2.CanSubstitute);
					if (categoryData.Supply / categoryData.Demand > categoryData2.Supply / categoryData2.Demand && categoryData2.Demand > categoryData.Demand)
					{
						float num = (categoryData2.Demand - categoryData.Demand) * itemCategory2.SubstitutionFactor;
						marketData.SetDemand(itemCategory2, categoryData.Demand + num);
						marketData.SetDemand(itemCategory2.CanSubstitute, categoryData2.Demand - num);
						float num2;
						float num3;
						if (categoryBudget.TryGetValue(itemCategory2, out num2) && categoryBudget.TryGetValue(itemCategory2.CanSubstitute, out num3))
						{
							categoryBudget[itemCategory2] = num2 + num;
							categoryBudget[itemCategory2.CanSubstitute] = num3 - num;
						}
					}
				}
			}
		}

		// Token: 0x060037C4 RID: 14276 RVA: 0x000FB1E4 File Offset: 0x000F93E4
		private static void UpdateSupplyAndDemand(Town town)
		{
			TownMarketData marketData = town.MarketData;
			SettlementEconomyModel settlementConsumptionModel = Campaign.Current.Models.SettlementConsumptionModel;
			foreach (ItemCategory itemCategory in ItemCategories.All)
			{
				ItemData categoryData = marketData.GetCategoryData(itemCategory);
				float estimatedDemandForCategory = settlementConsumptionModel.GetEstimatedDemandForCategory(town, categoryData, itemCategory);
				ValueTuple<float, float> supplyDemandForCategory = settlementConsumptionModel.GetSupplyDemandForCategory(town, itemCategory, (float)categoryData.InStoreValue, estimatedDemandForCategory, categoryData.Supply, categoryData.Demand);
				float item = supplyDemandForCategory.Item1;
				float item2 = supplyDemandForCategory.Item2;
				marketData.SetSupplyDemand(itemCategory, item, item2);
			}
		}

		// Token: 0x04001189 RID: 4489
		private ItemConsumptionBehavior.CategoryItems _categoryItems;

		// Token: 0x020006FA RID: 1786
		private class CategoryItems
		{
			// Token: 0x06005542 RID: 21826 RVA: 0x0016CD00 File Offset: 0x0016AF00
			public CategoryItems()
			{
				this.ItemDict = new Dictionary<ItemCategory, List<ItemObject>>();
				foreach (ItemObject itemObject in Items.All)
				{
					List<ItemObject> list;
					this.ItemDict.TryGetValue(itemObject.GetItemCategory(), out list);
					if (list == null)
					{
						list = new List<ItemObject>();
						this.ItemDict[itemObject.GetItemCategory()] = list;
					}
					list.Add(itemObject);
				}
			}

			// Token: 0x06005543 RID: 21827 RVA: 0x0016CD94 File Offset: 0x0016AF94
			public List<ItemObject> GetItemListOfCategory(ItemCategory category)
			{
				List<ItemObject> list;
				this.ItemDict.TryGetValue(category, out list);
				return list;
			}

			// Token: 0x04001CC1 RID: 7361
			private Dictionary<ItemCategory, List<ItemObject>> ItemDict;
		}
	}
}
