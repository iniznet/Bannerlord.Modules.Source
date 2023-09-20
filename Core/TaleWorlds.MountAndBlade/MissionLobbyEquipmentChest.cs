using System;
using System.Collections;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade.Diamond;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000290 RID: 656
	public class MissionLobbyEquipmentChest : IEnumerable<KeyValuePair<int, ItemObject>>, IEnumerable
	{
		// Token: 0x060022D6 RID: 8918 RVA: 0x0007F384 File Offset: 0x0007D584
		public MissionLobbyEquipmentChest(InventoryData chest)
		{
			this._items = new Dictionary<int, ItemObject>();
			foreach (ItemData itemData in chest.Items)
			{
				ItemObject itemObject = itemData.GetItemObject();
				this.AddItem(itemObject);
			}
		}

		// Token: 0x060022D7 RID: 8919 RVA: 0x0007F3F0 File Offset: 0x0007D5F0
		public ItemObject GetItem(int itemIndex)
		{
			return this._items[itemIndex];
		}

		// Token: 0x060022D8 RID: 8920 RVA: 0x0007F400 File Offset: 0x0007D600
		public int GetItemIndex(ItemObject item)
		{
			foreach (KeyValuePair<int, ItemObject> keyValuePair in this._items)
			{
				if (keyValuePair.Value == item)
				{
					return keyValuePair.Key;
				}
			}
			return -1;
		}

		// Token: 0x060022D9 RID: 8921 RVA: 0x0007F464 File Offset: 0x0007D664
		public int AddItem(ItemObject itemObject)
		{
			int num = this.FindAvaliableItemNo();
			this._items.Add(num, itemObject);
			return num;
		}

		// Token: 0x060022DA RID: 8922 RVA: 0x0007F488 File Offset: 0x0007D688
		private int FindAvaliableItemNo()
		{
			int num = 0;
			while (this._items.ContainsKey(num))
			{
				num++;
			}
			return num;
		}

		// Token: 0x060022DB RID: 8923 RVA: 0x0007F4AC File Offset: 0x0007D6AC
		public IEnumerator<KeyValuePair<int, ItemObject>> GetEnumerator()
		{
			foreach (KeyValuePair<int, ItemObject> keyValuePair in this._items)
			{
				yield return keyValuePair;
			}
			Dictionary<int, ItemObject>.Enumerator enumerator = default(Dictionary<int, ItemObject>.Enumerator);
			yield break;
			yield break;
		}

		// Token: 0x060022DC RID: 8924 RVA: 0x0007F4BB File Offset: 0x0007D6BB
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		// Token: 0x04000CF9 RID: 3321
		private Dictionary<int, ItemObject> _items;
	}
}
