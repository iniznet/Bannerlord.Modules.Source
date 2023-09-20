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
	// Token: 0x020003DC RID: 988
	public class TradeSkillCampaingBehavior : CampaignBehaviorBase, IPlayerTradeBehavior
	{
		// Token: 0x06003BBC RID: 15292 RVA: 0x0011AD44 File Offset: 0x00118F44
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

		// Token: 0x06003BBD RID: 15293 RVA: 0x0011ADC8 File Offset: 0x00118FC8
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

		// Token: 0x06003BBE RID: 15294 RVA: 0x0011AEA8 File Offset: 0x001190A8
		private int GetAveragePriceForItem(ItemRosterElement itemRosterElement)
		{
			TradeSkillCampaingBehavior.ItemTradeData itemTradeData;
			if (!this.ItemsTradeData.TryGetValue(itemRosterElement.EquipmentElement.Item, out itemTradeData))
			{
				return 0;
			}
			return MathF.Round(itemTradeData.AveragePrice);
		}

		// Token: 0x06003BBF RID: 15295 RVA: 0x0011AEE0 File Offset: 0x001190E0
		public override void RegisterEvents()
		{
			CampaignEvents.PlayerInventoryExchangeEvent.AddNonSerializedListener(this, new Action<List<ValueTuple<ItemRosterElement, int>>, List<ValueTuple<ItemRosterElement, int>>, bool>(this.InventoryUpdated));
		}

		// Token: 0x06003BC0 RID: 15296 RVA: 0x0011AEFC File Offset: 0x001190FC
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

		// Token: 0x06003BC1 RID: 15297 RVA: 0x0011AFC0 File Offset: 0x001191C0
		private int ProcessSales(ItemRosterElement itemRosterElement, int totalPrice)
		{
			if (itemRosterElement.EquipmentElement.ItemModifier == null)
			{
				return this.RecordSales(itemRosterElement, totalPrice);
			}
			return 0;
		}

		// Token: 0x06003BC2 RID: 15298 RVA: 0x0011AFE8 File Offset: 0x001191E8
		private void ProcessPurchases(ItemRosterElement itemRosterElement, int totalPrice)
		{
			if (itemRosterElement.EquipmentElement.ItemModifier == null)
			{
				this.RecordPurchases(itemRosterElement, totalPrice);
			}
		}

		// Token: 0x06003BC3 RID: 15299 RVA: 0x0011B00E File Offset: 0x0011920E
		public override void SyncData(IDataStore dataStore)
		{
			dataStore.SyncData<Dictionary<ItemObject, TradeSkillCampaingBehavior.ItemTradeData>>("ItemsTradeData", ref this.ItemsTradeData);
		}

		// Token: 0x06003BC4 RID: 15300 RVA: 0x0011B024 File Offset: 0x00119224
		public int GetProjectedProfit(ItemRosterElement itemRosterElement, int itemCost)
		{
			if (itemRosterElement.EquipmentElement.ItemModifier != null)
			{
				return 0;
			}
			int averagePriceForItem = this.GetAveragePriceForItem(itemRosterElement);
			return itemCost - averagePriceForItem;
		}

		// Token: 0x04001231 RID: 4657
		private Dictionary<ItemObject, TradeSkillCampaingBehavior.ItemTradeData> ItemsTradeData = new Dictionary<ItemObject, TradeSkillCampaingBehavior.ItemTradeData>();

		// Token: 0x0200073C RID: 1852
		public class TradeSkillCampaingBehaviorTypeDefiner : CampaignBehaviorBase.SaveableCampaignBehaviorTypeDefiner
		{
			// Token: 0x0600565E RID: 22110 RVA: 0x0016E447 File Offset: 0x0016C647
			public TradeSkillCampaingBehaviorTypeDefiner()
				: base(150794)
			{
			}

			// Token: 0x0600565F RID: 22111 RVA: 0x0016E454 File Offset: 0x0016C654
			protected override void DefineStructTypes()
			{
				base.AddStructDefinition(typeof(TradeSkillCampaingBehavior.ItemTradeData), 10, null);
			}

			// Token: 0x06005660 RID: 22112 RVA: 0x0016E469 File Offset: 0x0016C669
			protected override void DefineContainerDefinitions()
			{
				base.ConstructContainerDefinition(typeof(Dictionary<ItemObject, TradeSkillCampaingBehavior.ItemTradeData>));
			}
		}

		// Token: 0x0200073D RID: 1853
		internal struct ItemTradeData
		{
			// Token: 0x06005661 RID: 22113 RVA: 0x0016E47B File Offset: 0x0016C67B
			public ItemTradeData(float averagePrice, int numItemsPurchased)
			{
				this.AveragePrice = averagePrice;
				this.NumItemsPurchased = numItemsPurchased;
			}

			// Token: 0x06005662 RID: 22114 RVA: 0x0016E48C File Offset: 0x0016C68C
			public static void AutoGeneratedStaticCollectObjectsItemTradeData(object o, List<object> collectedObjects)
			{
				((TradeSkillCampaingBehavior.ItemTradeData)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
			}

			// Token: 0x06005663 RID: 22115 RVA: 0x0016E4A8 File Offset: 0x0016C6A8
			private void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
			{
			}

			// Token: 0x06005664 RID: 22116 RVA: 0x0016E4AA File Offset: 0x0016C6AA
			internal static object AutoGeneratedGetMemberValueAveragePrice(object o)
			{
				return ((TradeSkillCampaingBehavior.ItemTradeData)o).AveragePrice;
			}

			// Token: 0x06005665 RID: 22117 RVA: 0x0016E4BC File Offset: 0x0016C6BC
			internal static object AutoGeneratedGetMemberValueNumItemsPurchased(object o)
			{
				return ((TradeSkillCampaingBehavior.ItemTradeData)o).NumItemsPurchased;
			}

			// Token: 0x04001DE7 RID: 7655
			[SaveableField(10)]
			public readonly float AveragePrice;

			// Token: 0x04001DE8 RID: 7656
			[SaveableField(20)]
			public readonly int NumItemsPurchased;
		}
	}
}
