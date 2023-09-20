﻿using System;
using System.Collections.Generic;
using TaleWorlds.SaveSystem;

namespace TaleWorlds.CampaignSystem.Settlements
{
	// Token: 0x02000364 RID: 868
	public struct ItemData
	{
		// Token: 0x06003231 RID: 12849 RVA: 0x000D0A20 File Offset: 0x000CEC20
		public static void AutoGeneratedStaticCollectObjectsItemData(object o, List<object> collectedObjects)
		{
			((ItemData)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		// Token: 0x06003232 RID: 12850 RVA: 0x000D0A3C File Offset: 0x000CEC3C
		private void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
		{
		}

		// Token: 0x06003233 RID: 12851 RVA: 0x000D0A3E File Offset: 0x000CEC3E
		internal static object AutoGeneratedGetMemberValueSupply(object o)
		{
			return ((ItemData)o).Supply;
		}

		// Token: 0x06003234 RID: 12852 RVA: 0x000D0A50 File Offset: 0x000CEC50
		internal static object AutoGeneratedGetMemberValueDemand(object o)
		{
			return ((ItemData)o).Demand;
		}

		// Token: 0x06003235 RID: 12853 RVA: 0x000D0A62 File Offset: 0x000CEC62
		internal static object AutoGeneratedGetMemberValueInStore(object o)
		{
			return ((ItemData)o).InStore;
		}

		// Token: 0x06003236 RID: 12854 RVA: 0x000D0A74 File Offset: 0x000CEC74
		internal static object AutoGeneratedGetMemberValueInStoreValue(object o)
		{
			return ((ItemData)o).InStoreValue;
		}

		// Token: 0x06003237 RID: 12855 RVA: 0x000D0A86 File Offset: 0x000CEC86
		public ItemData(float supply, float demand, int inStore, int inStoreValue)
		{
			this.Supply = supply;
			this.Demand = demand;
			this.InStore = inStore;
			this.InStoreValue = inStoreValue;
		}

		// Token: 0x06003238 RID: 12856 RVA: 0x000D0AA5 File Offset: 0x000CECA5
		public ItemData Add(ItemData other)
		{
			return new ItemData(this.Supply + other.Supply, this.Demand + other.Demand, this.InStore + other.InStore, this.InStoreValue + other.InStoreValue);
		}

		// Token: 0x06003239 RID: 12857 RVA: 0x000D0AE0 File Offset: 0x000CECE0
		internal ItemData AddDemand(float demandAmount)
		{
			return new ItemData(this.Supply, this.Demand + demandAmount, this.InStore, this.InStoreValue);
		}

		// Token: 0x0600323A RID: 12858 RVA: 0x000D0B01 File Offset: 0x000CED01
		internal ItemData AddSupply(float supplyAmount)
		{
			return new ItemData(this.Supply + supplyAmount, this.Demand, this.InStore, this.InStoreValue);
		}

		// Token: 0x0600323B RID: 12859 RVA: 0x000D0B22 File Offset: 0x000CED22
		internal ItemData AddInStore(int inStoreAmount, int value)
		{
			return new ItemData(this.Supply, this.Demand, this.InStore + inStoreAmount, this.InStoreValue + inStoreAmount * value);
		}

		// Token: 0x0600323C RID: 12860 RVA: 0x000D0B47 File Offset: 0x000CED47
		internal ItemData AddSupplyDemand(float supply, float demand)
		{
			return new ItemData(this.Supply + supply, this.Demand + demand, this.InStore, this.InStoreValue);
		}

		// Token: 0x04001054 RID: 4180
		[SaveableField(1)]
		public readonly float Supply;

		// Token: 0x04001055 RID: 4181
		[SaveableField(2)]
		public readonly float Demand;

		// Token: 0x04001056 RID: 4182
		[SaveableField(3)]
		public readonly int InStore;

		// Token: 0x04001057 RID: 4183
		[SaveableField(4)]
		public readonly int InStoreValue;
	}
}
