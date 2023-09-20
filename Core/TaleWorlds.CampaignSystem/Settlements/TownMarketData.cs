﻿using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Extensions;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.Core;
using TaleWorlds.SaveSystem;

namespace TaleWorlds.CampaignSystem.Settlements
{
	public class TownMarketData : IMarketData
	{
		internal static void AutoGeneratedStaticCollectObjectsTownMarketData(object o, List<object> collectedObjects)
		{
			((TownMarketData)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		protected virtual void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
		{
			collectedObjects.Add(this._itemDict);
			collectedObjects.Add(this._town);
		}

		internal static object AutoGeneratedGetMemberValue_itemDict(object o)
		{
			return ((TownMarketData)o)._itemDict;
		}

		internal static object AutoGeneratedGetMemberValue_town(object o)
		{
			return ((TownMarketData)o)._town;
		}

		public TownMarketData(Town town)
		{
			this._town = town;
		}

		public ItemData GetCategoryData(ItemCategory itemCategory)
		{
			ItemData itemData;
			if (!this._itemDict.TryGetValue(itemCategory, out itemData))
			{
				itemData = default(ItemData);
			}
			return itemData;
		}

		public int GetItemCountOfCategory(ItemCategory itemCategory)
		{
			ItemData itemData;
			if (!this._itemDict.TryGetValue(itemCategory, out itemData))
			{
				return 0;
			}
			return itemData.InStore;
		}

		private void SetItemData(ItemCategory itemCategory, ItemData itemData)
		{
			this._itemDict[itemCategory] = itemData;
		}

		public void OnTownInventoryUpdated(ItemRosterElement item, int count)
		{
			if (item.EquipmentElement.Item == null)
			{
				this.ClearStores();
				return;
			}
			this.AddNumberInStore(item.EquipmentElement.Item.GetItemCategory(), count, item.EquipmentElement.Item.Value);
		}

		public void AddDemand(ItemCategory itemCategory, float demandAmount)
		{
			SettlementEconomyModel settlementConsumptionModel = Campaign.Current.Models.SettlementConsumptionModel;
			this.SetItemData(itemCategory, this.GetCategoryData(itemCategory).AddDemand(settlementConsumptionModel.GetDemandChangeFromValue(demandAmount)));
		}

		public void AddSupply(ItemCategory itemCategory, float supplyAmount)
		{
			this.SetItemData(itemCategory, this.GetCategoryData(itemCategory).AddSupply(supplyAmount));
		}

		public void AddNumberInStore(ItemCategory itemCategory, int number, int value)
		{
			this.SetItemData(itemCategory, this.GetCategoryData(itemCategory).AddInStore(number, value));
		}

		public void SetSupplyDemand(ItemCategory itemCategory, float supply, float demand)
		{
			ItemData categoryData = this.GetCategoryData(itemCategory);
			this.SetItemData(itemCategory, new ItemData(supply, demand, categoryData.InStore, categoryData.InStoreValue));
		}

		public void SetDemand(ItemCategory itemCategory, float demand)
		{
			ItemData categoryData = this.GetCategoryData(itemCategory);
			this.SetItemData(itemCategory, new ItemData(categoryData.Supply, demand, categoryData.InStore, categoryData.InStoreValue));
		}

		public float GetDemand(ItemCategory itemCategory)
		{
			return this.GetCategoryData(itemCategory).Demand;
		}

		public float GetSupply(ItemCategory itemCategory)
		{
			return this.GetCategoryData(itemCategory).Supply;
		}

		public float GetPriceFactor(ItemCategory itemCategory)
		{
			ItemData categoryData = this.GetCategoryData(itemCategory);
			return Campaign.Current.Models.TradeItemPriceFactorModel.GetBasePriceFactor(itemCategory, (float)categoryData.InStoreValue, categoryData.Supply, categoryData.Demand, false, 0);
		}

		public int GetPrice(ItemObject item, MobileParty tradingParty = null, bool isSelling = false, PartyBase merchantParty = null)
		{
			return this.GetPrice(new EquipmentElement(item, null, null, false), tradingParty, isSelling, null);
		}

		public int GetPrice(EquipmentElement itemRosterElement, MobileParty tradingParty = null, bool isSelling = false, PartyBase merchantParty = null)
		{
			ItemData categoryData = this.GetCategoryData(itemRosterElement.Item.GetItemCategory());
			return Campaign.Current.Models.TradeItemPriceFactorModel.GetPrice(itemRosterElement, tradingParty, merchantParty, isSelling, (float)categoryData.InStoreValue, categoryData.Supply, categoryData.Demand);
		}

		public void UpdateStores()
		{
			this.ClearStores();
			ItemRoster itemRoster = this._town.Owner.ItemRoster;
			for (int i = 0; i < itemRoster.Count; i++)
			{
				ItemRosterElement itemRosterElement = itemRoster[i];
				if (itemRosterElement.EquipmentElement.Item.ItemCategory != null)
				{
					ItemData categoryData = this.GetCategoryData(itemRosterElement.EquipmentElement.Item.GetItemCategory());
					this.SetItemData(itemRosterElement.EquipmentElement.Item.GetItemCategory(), categoryData.AddInStore(itemRosterElement.Amount, itemRosterElement.EquipmentElement.Item.Value));
				}
			}
		}

		private void ClearStores()
		{
			foreach (ItemCategory itemCategory in ItemCategories.All)
			{
				ItemData categoryData = this.GetCategoryData(itemCategory);
				this.SetItemData(itemCategory, new ItemData(categoryData.Supply, categoryData.Demand, 0, 0));
			}
		}

		[SaveableField(1)]
		private Dictionary<ItemCategory, ItemData> _itemDict = new Dictionary<ItemCategory, ItemData>();

		[SaveableField(2)]
		private Town _town;
	}
}