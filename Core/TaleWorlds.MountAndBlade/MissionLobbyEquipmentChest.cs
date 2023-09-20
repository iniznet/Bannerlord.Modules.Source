using System;
using System.Collections;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade.Diamond;

namespace TaleWorlds.MountAndBlade
{
	public class MissionLobbyEquipmentChest : IEnumerable<KeyValuePair<int, ItemObject>>, IEnumerable
	{
		public MissionLobbyEquipmentChest(InventoryData chest)
		{
			this._items = new Dictionary<int, ItemObject>();
			foreach (ItemData itemData in chest.Items)
			{
				ItemObject itemObject = itemData.GetItemObject();
				this.AddItem(itemObject);
			}
		}

		public ItemObject GetItem(int itemIndex)
		{
			return this._items[itemIndex];
		}

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

		public int AddItem(ItemObject itemObject)
		{
			int num = this.FindAvaliableItemNo();
			this._items.Add(num, itemObject);
			return num;
		}

		private int FindAvaliableItemNo()
		{
			int num = 0;
			while (this._items.ContainsKey(num))
			{
				num++;
			}
			return num;
		}

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

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		private Dictionary<int, ItemObject> _items;
	}
}
