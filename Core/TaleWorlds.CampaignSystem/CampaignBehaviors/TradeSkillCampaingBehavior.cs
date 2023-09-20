using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Inventory;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.SaveSystem;

namespace TaleWorlds.CampaignSystem.CampaignBehaviors
{
	public class TradeSkillCampaingBehavior : CampaignBehaviorBase, IPlayerTradeBehavior
	{
		private void RecordPurchases(ItemRosterElement itemRosterElement, int totalPrice)
		{
			TradeSkillCampaingBehavior.ItemTradeData itemTradeData;
			if (!this.ItemsTradeData.TryGetValue(itemRosterElement.EquipmentElement.Item, out itemTradeData))
			{
				itemTradeData = default(TradeSkillCampaingBehavior.ItemTradeData);
			}
			int num = itemTradeData.NumItemsPurchased + itemRosterElement.Amount;
			float num2 = (itemTradeData.AveragePrice * (float)itemTradeData.NumItemsPurchased + (float)totalPrice) / MathF.Max(0.0001f, (float)num);
			this.ItemsTradeData[itemRosterElement.EquipmentElement.Item] = new TradeSkillCampaingBehavior.ItemTradeData(num2, num);
		}

		private int RecordSales(ItemRosterElement itemRosterElement, int totalPrice)
		{
			bool flag = false;
			TradeSkillCampaingBehavior.ItemTradeData itemTradeData;
			if (this.ItemsTradeData.TryGetValue(itemRosterElement.EquipmentElement.Item, out itemTradeData))
			{
				flag = true;
			}
			else
			{
				itemTradeData = default(TradeSkillCampaingBehavior.ItemTradeData);
			}
			int num = MathF.Min(itemTradeData.NumItemsPurchased, itemRosterElement.Amount);
			int num2 = itemTradeData.NumItemsPurchased - num;
			float num3 = (float)num * itemTradeData.AveragePrice;
			float num4 = (float)totalPrice / MathF.Max(0.001f, (float)itemRosterElement.Amount);
			int num5 = MathF.Round((float)num * num4);
			int num6 = MathF.Max(0, num5 - MathF.Floor(num3));
			if (num2 == 0)
			{
				if (flag)
				{
					this.ItemsTradeData.Remove(itemRosterElement.EquipmentElement.Item);
					return num6;
				}
			}
			else
			{
				this.ItemsTradeData[itemRosterElement.EquipmentElement.Item] = new TradeSkillCampaingBehavior.ItemTradeData(itemTradeData.AveragePrice, num2);
			}
			return num6;
		}

		private int GetAveragePriceForItem(ItemRosterElement itemRosterElement)
		{
			TradeSkillCampaingBehavior.ItemTradeData itemTradeData;
			if (!this.ItemsTradeData.TryGetValue(itemRosterElement.EquipmentElement.Item, out itemTradeData))
			{
				return 0;
			}
			return MathF.Round(itemTradeData.AveragePrice);
		}

		public override void RegisterEvents()
		{
			CampaignEvents.PlayerInventoryExchangeEvent.AddNonSerializedListener(this, new Action<List<ValueTuple<ItemRosterElement, int>>, List<ValueTuple<ItemRosterElement, int>>, bool>(this.InventoryUpdated));
		}

		private void InventoryUpdated(List<ValueTuple<ItemRosterElement, int>> purchasedItems, List<ValueTuple<ItemRosterElement, int>> soldItems, bool isTrading)
		{
			if (isTrading)
			{
				foreach (ValueTuple<ItemRosterElement, int> valueTuple in purchasedItems)
				{
					this.ProcessPurchases(valueTuple.Item1, valueTuple.Item2);
				}
				int num = 0;
				foreach (ValueTuple<ItemRosterElement, int> valueTuple2 in soldItems)
				{
					num += this.ProcessSales(valueTuple2.Item1, valueTuple2.Item2);
				}
				SkillLevelingManager.OnTradeProfitMade(PartyBase.MainParty, num);
				CampaignEventDispatcher.Instance.OnPlayerTradeProfit(num);
			}
		}

		private int ProcessSales(ItemRosterElement itemRosterElement, int totalPrice)
		{
			if (itemRosterElement.EquipmentElement.ItemModifier == null)
			{
				return this.RecordSales(itemRosterElement, totalPrice);
			}
			return 0;
		}

		private void ProcessPurchases(ItemRosterElement itemRosterElement, int totalPrice)
		{
			if (itemRosterElement.EquipmentElement.ItemModifier == null)
			{
				this.RecordPurchases(itemRosterElement, totalPrice);
			}
		}

		public override void SyncData(IDataStore dataStore)
		{
			dataStore.SyncData<Dictionary<ItemObject, TradeSkillCampaingBehavior.ItemTradeData>>("ItemsTradeData", ref this.ItemsTradeData);
		}

		public int GetProjectedProfit(ItemRosterElement itemRosterElement, int itemCost)
		{
			if (itemRosterElement.EquipmentElement.ItemModifier != null)
			{
				return 0;
			}
			int averagePriceForItem = this.GetAveragePriceForItem(itemRosterElement);
			return itemCost - averagePriceForItem;
		}

		private Dictionary<ItemObject, TradeSkillCampaingBehavior.ItemTradeData> ItemsTradeData = new Dictionary<ItemObject, TradeSkillCampaingBehavior.ItemTradeData>();

		public class TradeSkillCampaingBehaviorTypeDefiner : CampaignBehaviorBase.SaveableCampaignBehaviorTypeDefiner
		{
			public TradeSkillCampaingBehaviorTypeDefiner()
				: base(150794)
			{
			}

			protected override void DefineStructTypes()
			{
				base.AddStructDefinition(typeof(TradeSkillCampaingBehavior.ItemTradeData), 10, null);
			}

			protected override void DefineContainerDefinitions()
			{
				base.ConstructContainerDefinition(typeof(Dictionary<ItemObject, TradeSkillCampaingBehavior.ItemTradeData>));
			}
		}

		internal struct ItemTradeData
		{
			public ItemTradeData(float averagePrice, int numItemsPurchased)
			{
				this.AveragePrice = averagePrice;
				this.NumItemsPurchased = numItemsPurchased;
			}

			public static void AutoGeneratedStaticCollectObjectsItemTradeData(object o, List<object> collectedObjects)
			{
				((TradeSkillCampaingBehavior.ItemTradeData)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
			}

			private void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
			{
			}

			internal static object AutoGeneratedGetMemberValueAveragePrice(object o)
			{
				return ((TradeSkillCampaingBehavior.ItemTradeData)o).AveragePrice;
			}

			internal static object AutoGeneratedGetMemberValueNumItemsPurchased(object o)
			{
				return ((TradeSkillCampaingBehavior.ItemTradeData)o).NumItemsPurchased;
			}

			[SaveableField(10)]
			public readonly float AveragePrice;

			[SaveableField(20)]
			public readonly int NumItemsPurchased;
		}
	}
}
