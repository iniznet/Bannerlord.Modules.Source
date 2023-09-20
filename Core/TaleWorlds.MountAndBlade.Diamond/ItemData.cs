using System;

namespace TaleWorlds.MountAndBlade.Diamond
{
	public class ItemData
	{
		public string TypeId { get; set; }

		public string ModifierId { get; set; }

		public int? Index { get; set; }

		public void CopyItemData(ItemData itemdata)
		{
			this.TypeId = itemdata.TypeId;
			this.ModifierId = itemdata.ModifierId;
			this.Index = itemdata.Index;
		}

		private ItemType ItemType
		{
			get
			{
				return ItemList.GetItemTypeOf(this.TypeId);
			}
		}

		private static int GetInventoryItemTypeOfItem(ItemType itemType)
		{
			switch (itemType)
			{
			case ItemType.Horse:
				return 64;
			case ItemType.OneHandedWeapon:
				return 1;
			case ItemType.TwoHandedWeapon:
				return 1;
			case ItemType.Polearm:
				return 1;
			case ItemType.Arrows:
				return 1;
			case ItemType.Bolts:
				return 1;
			case ItemType.Shield:
				return 2;
			case ItemType.Bow:
				return 1;
			case ItemType.Crossbow:
				return 1;
			case ItemType.Thrown:
				return 1;
			case ItemType.Goods:
				return 256;
			case ItemType.HeadArmor:
				return 4;
			case ItemType.BodyArmor:
				return 8;
			case ItemType.LegArmor:
				return 16;
			case ItemType.HandArmor:
				return 32;
			case ItemType.Pistol:
				return 1;
			case ItemType.Musket:
				return 1;
			case ItemType.Bullets:
				return 1;
			case ItemType.Animal:
				return 1024;
			case ItemType.Book:
				return 512;
			case ItemType.Cape:
				return 2048;
			case ItemType.HorseHarness:
				return 128;
			}
			return 0;
		}

		public bool CanItemToEquipmentDragPossible(int equipmentIndex)
		{
			return ItemData.CanItemToEquipmentDragPossible(this.TypeId, equipmentIndex);
		}

		public static bool CanItemToEquipmentDragPossible(string itemTypeId, int equipmentIndex)
		{
			InventoryItemType inventoryItemTypeOfItem = (InventoryItemType)ItemData.GetInventoryItemTypeOfItem(ItemList.GetItemTypeOf(itemTypeId));
			bool flag = false;
			if (equipmentIndex == 0 || equipmentIndex == 1 || equipmentIndex == 2 || equipmentIndex == 3)
			{
				flag = inventoryItemTypeOfItem == InventoryItemType.Weapon || inventoryItemTypeOfItem == InventoryItemType.Shield;
			}
			else if (equipmentIndex == 5)
			{
				flag = inventoryItemTypeOfItem == InventoryItemType.HeadArmor;
			}
			else if (equipmentIndex == 6)
			{
				flag = inventoryItemTypeOfItem == InventoryItemType.BodyArmor;
			}
			else if (equipmentIndex == 7)
			{
				flag = inventoryItemTypeOfItem == InventoryItemType.LegArmor;
			}
			else if (equipmentIndex == 8)
			{
				flag = inventoryItemTypeOfItem == InventoryItemType.HandArmor;
			}
			else if (equipmentIndex == 9)
			{
				flag = inventoryItemTypeOfItem == InventoryItemType.Cape;
			}
			else if (equipmentIndex == 10)
			{
				flag = inventoryItemTypeOfItem == InventoryItemType.Horse;
			}
			else if (equipmentIndex == 11)
			{
				flag = inventoryItemTypeOfItem == InventoryItemType.HorseHarness;
			}
			return flag;
		}

		public int Price
		{
			get
			{
				return ItemData.GetPriceOf(this.TypeId, this.ModifierId);
			}
		}

		public bool IsValid
		{
			get
			{
				return ItemData.IsItemValid(this.TypeId, this.ModifierId);
			}
		}

		public string ItemKey
		{
			get
			{
				return this.TypeId + "|" + this.ModifierId;
			}
		}

		public static int GetPriceOf(string itemId, string modifierId)
		{
			return ItemList.GetPriceOf(itemId, modifierId);
		}

		public static bool IsItemValid(string itemId, string modifierId)
		{
			return ItemList.IsItemValid(itemId, modifierId);
		}
	}
}
