using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.Diamond
{
	public class InventoryData
	{
		public InventoryData()
		{
			this.Items = new List<ItemData>();
		}

		public ItemData GetItemWithIndex(int itemIndex)
		{
			return this.Items.SingleOrDefault(delegate(ItemData q)
			{
				int? index = q.Index;
				int itemIndex2 = itemIndex;
				return (index.GetValueOrDefault() == itemIndex2) & (index != null);
			});
		}

		public List<ItemData> Items { get; private set; }

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
