using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.Diamond
{
	// Token: 0x02000118 RID: 280
	public class InventoryData
	{
		// Token: 0x06000557 RID: 1367 RVA: 0x00007DF3 File Offset: 0x00005FF3
		public InventoryData()
		{
			this.Items = new List<ItemData>();
		}

		// Token: 0x06000558 RID: 1368 RVA: 0x00007E08 File Offset: 0x00006008
		public ItemData GetItemWithIndex(int itemIndex)
		{
			return this.Items.SingleOrDefault(delegate(ItemData q)
			{
				int? index = q.Index;
				int itemIndex2 = itemIndex;
				return (index.GetValueOrDefault() == itemIndex2) & (index != null);
			});
		}

		// Token: 0x17000204 RID: 516
		// (get) Token: 0x06000559 RID: 1369 RVA: 0x00007E39 File Offset: 0x00006039
		// (set) Token: 0x0600055A RID: 1370 RVA: 0x00007E41 File Offset: 0x00006041
		public List<ItemData> Items { get; private set; }

		// Token: 0x0600055B RID: 1371 RVA: 0x00007E4C File Offset: 0x0000604C
		public void DebugPrint()
		{
			string text = "";
			foreach (ItemData itemData in this.Items)
			{
				text = string.Concat(new object[] { text, itemData.Index, " ", itemData.TypeId, "\n" });
			}
			Debug.Print(text, 0, Debug.DebugColor.White, 17592186044416UL);
		}
	}
}
