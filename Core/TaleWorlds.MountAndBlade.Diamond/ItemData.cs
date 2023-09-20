using System;

namespace TaleWorlds.MountAndBlade.Diamond
{
	// Token: 0x0200011B RID: 283
	public class ItemData
	{
		// Token: 0x17000205 RID: 517
		// (get) Token: 0x0600055C RID: 1372 RVA: 0x00007EE8 File Offset: 0x000060E8
		// (set) Token: 0x0600055D RID: 1373 RVA: 0x00007EF0 File Offset: 0x000060F0
		public string TypeId { get; set; }

		// Token: 0x17000206 RID: 518
		// (get) Token: 0x0600055E RID: 1374 RVA: 0x00007EF9 File Offset: 0x000060F9
		// (set) Token: 0x0600055F RID: 1375 RVA: 0x00007F01 File Offset: 0x00006101
		public string ModifierId { get; set; }

		// Token: 0x17000207 RID: 519
		// (get) Token: 0x06000560 RID: 1376 RVA: 0x00007F0A File Offset: 0x0000610A
		// (set) Token: 0x06000561 RID: 1377 RVA: 0x00007F12 File Offset: 0x00006112
		public int? Index { get; set; }

		// Token: 0x06000562 RID: 1378 RVA: 0x00007F1B File Offset: 0x0000611B
		public void CopyItemData(ItemData itemdata)
		{
			this.TypeId = itemdata.TypeId;
			this.ModifierId = itemdata.ModifierId;
			this.Index = itemdata.Index;
		}

		// Token: 0x17000208 RID: 520
		// (get) Token: 0x06000563 RID: 1379 RVA: 0x00007F41 File Offset: 0x00006141
		private ItemType ItemType
		{
			get
			{
				return ItemList.GetItemTypeOf(this.TypeId);
			}
		}

		// Token: 0x06000564 RID: 1380 RVA: 0x00007F50 File Offset: 0x00006150
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

		// Token: 0x06000565 RID: 1381 RVA: 0x00008007 File Offset: 0x00006207
		public bool CanItemToEquipmentDragPossible(int equipmentIndex)
		{
			return ItemData.CanItemToEquipmentDragPossible(this.TypeId, equipmentIndex);
		}

		// Token: 0x06000566 RID: 1382 RVA: 0x00008018 File Offset: 0x00006218
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

		// Token: 0x17000209 RID: 521
		// (get) Token: 0x06000567 RID: 1383 RVA: 0x000080AC File Offset: 0x000062AC
		public int Price
		{
			get
			{
				return ItemData.GetPriceOf(this.TypeId, this.ModifierId);
			}
		}

		// Token: 0x1700020A RID: 522
		// (get) Token: 0x06000568 RID: 1384 RVA: 0x000080BF File Offset: 0x000062BF
		public bool IsValid
		{
			get
			{
				return ItemData.IsItemValid(this.TypeId, this.ModifierId);
			}
		}

		// Token: 0x1700020B RID: 523
		// (get) Token: 0x06000569 RID: 1385 RVA: 0x000080D2 File Offset: 0x000062D2
		public string ItemKey
		{
			get
			{
				return this.TypeId + "|" + this.ModifierId;
			}
		}

		// Token: 0x0600056A RID: 1386 RVA: 0x000080EA File Offset: 0x000062EA
		public static int GetPriceOf(string itemId, string modifierId)
		{
			return ItemList.GetPriceOf(itemId, modifierId);
		}

		// Token: 0x0600056B RID: 1387 RVA: 0x000080F3 File Offset: 0x000062F3
		public static bool IsItemValid(string itemId, string modifierId)
		{
			return ItemList.IsItemValid(itemId, modifierId);
		}
	}
}
