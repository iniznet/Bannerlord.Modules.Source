using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.MountAndBlade.Diamond.Cosmetics.CosmeticTypes;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection.Lobby
{
	internal static class LobbyTauntHelper
	{
		public static Equipment PrepareForTaunt(Equipment originalEquipment, TauntCosmeticElement taunt, bool doNotAddComplimentaryWeapons = false)
		{
			TauntUsageManager.TauntUsageSet usageSet = TauntUsageManager.GetUsageSet(taunt.Id);
			MBReadOnlyList<TauntUsageManager.TauntUsage> mbreadOnlyList = ((usageSet != null) ? usageSet.GetUsages() : null);
			if (mbreadOnlyList == null || mbreadOnlyList.Count == 0)
			{
				return originalEquipment;
			}
			Equipment equipment = new Equipment(originalEquipment);
			EquipmentIndex equipmentIndex;
			EquipmentIndex equipmentIndex2;
			bool flag;
			equipment.GetInitialWeaponIndicesToEquip(ref equipmentIndex, ref equipmentIndex2, ref flag, 0);
			WeaponComponentData weaponComponentData;
			if (equipmentIndex == -1)
			{
				weaponComponentData = null;
			}
			else
			{
				ItemObject item = equipment[equipmentIndex].Item;
				weaponComponentData = ((item != null) ? item.PrimaryWeapon : null);
			}
			WeaponComponentData weaponComponentData2 = weaponComponentData;
			WeaponComponentData weaponComponentData3 = null;
			if (!flag && equipmentIndex2 != -1)
			{
				ItemObject item2 = equipment[equipmentIndex2].Item;
				weaponComponentData3 = ((item2 != null) ? item2.PrimaryWeapon : null);
			}
			using (List<TauntUsageManager.TauntUsage>.Enumerator enumerator = mbreadOnlyList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.IsSuitable(false, true, weaponComponentData2, weaponComponentData3))
					{
						return equipment;
					}
				}
			}
			TauntUsageManager.TauntUsage tauntUsage = mbreadOnlyList.FirstOrDefault((TauntUsageManager.TauntUsage u) => !Extensions.HasAnyFlag<TauntUsageManager.TauntUsage.TauntUsageFlag>(u.UsageFlag, 3)) ?? mbreadOnlyList[0];
			for (EquipmentIndex equipmentIndex3 = 0; equipmentIndex3 < 5; equipmentIndex3++)
			{
				equipment[equipmentIndex3] = default(EquipmentElement);
			}
			List<ItemObject> list = MBObjectManager.Instance.GetObjectTypeList<ItemObject>().ToList<ItemObject>();
			list.Sort((ItemObject first, ItemObject second) => first.Value.CompareTo(second.Value));
			EquipmentIndex equipmentIndex4 = 0;
			if (Extensions.HasAnyFlag<TauntUsageManager.TauntUsage.TauntUsageFlag>(tauntUsage.UsageFlag, 1))
			{
				ItemObject randomElementWithPredicate = Extensions.GetRandomElementWithPredicate<ItemObject>(list, delegate(ItemObject i)
				{
					WeaponComponentData primaryWeapon = i.PrimaryWeapon;
					return CosmeticsManagerHelper.IsWeaponClassBow((primaryWeapon != null) ? primaryWeapon.WeaponClass : 0);
				});
				if (!equipment.TryAddElement(ref equipmentIndex4, new EquipmentElement(randomElementWithPredicate, null, null, false)))
				{
					return equipment;
				}
				ItemObject randomElementWithPredicate2 = Extensions.GetRandomElementWithPredicate<ItemObject>(list, delegate(ItemObject i)
				{
					WeaponComponentData primaryWeapon2 = i.PrimaryWeapon;
					return primaryWeapon2 != null && primaryWeapon2.WeaponClass == 12;
				});
				equipment.TryAddElement(ref equipmentIndex4, new EquipmentElement(randomElementWithPredicate2, null, null, false));
				return equipment;
			}
			else
			{
				if (Extensions.HasAnyFlag<TauntUsageManager.TauntUsage.TauntUsageFlag>(tauntUsage.UsageFlag, 2))
				{
					ItemObject randomElementWithPredicate3 = Extensions.GetRandomElementWithPredicate<ItemObject>(list, delegate(ItemObject i)
					{
						WeaponComponentData primaryWeapon3 = i.PrimaryWeapon;
						return CosmeticsManagerHelper.IsWeaponClassShield((primaryWeapon3 != null) ? primaryWeapon3.WeaponClass : 0);
					});
					if (!equipment.TryAddElement(ref equipmentIndex4, new EquipmentElement(randomElementWithPredicate3, null, null, false)))
					{
						return equipment;
					}
					if (!Extensions.HasAnyFlag<TauntUsageManager.TauntUsage.TauntUsageFlag>(tauntUsage.UsageFlag, 32))
					{
						ItemObject randomElementWithPredicate4 = Extensions.GetRandomElementWithPredicate<ItemObject>(list, delegate(ItemObject i)
						{
							WeaponComponentData primaryWeapon4 = i.PrimaryWeapon;
							return CosmeticsManagerHelper.IsWeaponClassOneHanded((primaryWeapon4 != null) ? primaryWeapon4.WeaponClass : 0);
						});
						equipment.TryAddElement(ref equipmentIndex4, new EquipmentElement(randomElementWithPredicate4, null, null, false));
						return equipment;
					}
				}
				if (!Extensions.HasAnyFlag<TauntUsageManager.TauntUsage.TauntUsageFlag>(tauntUsage.UsageFlag, 16))
				{
					ItemObject randomElementWithPredicate5 = Extensions.GetRandomElementWithPredicate<ItemObject>(list, delegate(ItemObject i)
					{
						WeaponComponentData primaryWeapon5 = i.PrimaryWeapon;
						return CosmeticsManagerHelper.IsWeaponClassTwoHanded((primaryWeapon5 != null) ? primaryWeapon5.WeaponClass : 0);
					});
					if (!equipment.TryAddElement(ref equipmentIndex4, new EquipmentElement(randomElementWithPredicate5, null, null, false)))
					{
						return equipment;
					}
					if (tauntUsage.IsSuitable(false, true, randomElementWithPredicate5.PrimaryWeapon, null))
					{
						return equipment;
					}
				}
				if (!Extensions.HasAnyFlag<TauntUsageManager.TauntUsage.TauntUsageFlag>(tauntUsage.UsageFlag, 32))
				{
					ItemObject randomElementWithPredicate6 = Extensions.GetRandomElementWithPredicate<ItemObject>(list, delegate(ItemObject i)
					{
						WeaponComponentData primaryWeapon6 = i.PrimaryWeapon;
						return CosmeticsManagerHelper.IsWeaponClassOneHanded((primaryWeapon6 != null) ? primaryWeapon6.WeaponClass : 0);
					});
					if (!equipment.TryAddElement(ref equipmentIndex4, new EquipmentElement(randomElementWithPredicate6, null, null, false)))
					{
						return equipment;
					}
					if (tauntUsage.IsSuitable(false, true, randomElementWithPredicate6.PrimaryWeapon, null))
					{
						return equipment;
					}
				}
				if (!Extensions.HasAnyFlag<TauntUsageManager.TauntUsage.TauntUsageFlag>(tauntUsage.UsageFlag, 64))
				{
					ItemObject randomElementWithPredicate7 = Extensions.GetRandomElementWithPredicate<ItemObject>(list, delegate(ItemObject i)
					{
						WeaponComponentData primaryWeapon7 = i.PrimaryWeapon;
						return CosmeticsManagerHelper.IsWeaponClassShield((primaryWeapon7 != null) ? primaryWeapon7.WeaponClass : 0);
					});
					if (!equipment.TryAddElement(ref equipmentIndex4, new EquipmentElement(randomElementWithPredicate7, null, null, false)))
					{
						return equipment;
					}
				}
				if (!Extensions.HasAnyFlag<TauntUsageManager.TauntUsage.TauntUsageFlag>(tauntUsage.UsageFlag, 128))
				{
					ItemObject randomElementWithPredicate8 = Extensions.GetRandomElementWithPredicate<ItemObject>(list, delegate(ItemObject i)
					{
						WeaponComponentData primaryWeapon8 = i.PrimaryWeapon;
						return CosmeticsManagerHelper.IsWeaponClassBow((primaryWeapon8 != null) ? primaryWeapon8.WeaponClass : 0);
					});
					if (!equipment.TryAddElement(ref equipmentIndex4, new EquipmentElement(randomElementWithPredicate8, null, null, false)))
					{
						return equipment;
					}
					ItemObject randomElementWithPredicate9 = Extensions.GetRandomElementWithPredicate<ItemObject>(list, delegate(ItemObject i)
					{
						WeaponComponentData primaryWeapon9 = i.PrimaryWeapon;
						return primaryWeapon9 != null && primaryWeapon9.WeaponClass == 12;
					});
					equipment.TryAddElement(ref equipmentIndex4, new EquipmentElement(randomElementWithPredicate9, null, null, false));
					return equipment;
				}
				else
				{
					if (Extensions.HasAnyFlag<TauntUsageManager.TauntUsage.TauntUsageFlag>(tauntUsage.UsageFlag, 256))
					{
						return equipment;
					}
					ItemObject randomElementWithPredicate10 = Extensions.GetRandomElementWithPredicate<ItemObject>(list, delegate(ItemObject i)
					{
						WeaponComponentData primaryWeapon10 = i.PrimaryWeapon;
						return CosmeticsManagerHelper.IsWeaponClassCrossbow((primaryWeapon10 != null) ? primaryWeapon10.WeaponClass : 0);
					});
					if (!equipment.TryAddElement(ref equipmentIndex4, new EquipmentElement(randomElementWithPredicate10, null, null, false)))
					{
						return equipment;
					}
					ItemObject randomElementWithPredicate11 = Extensions.GetRandomElementWithPredicate<ItemObject>(list, delegate(ItemObject i)
					{
						WeaponComponentData primaryWeapon11 = i.PrimaryWeapon;
						return primaryWeapon11 != null && primaryWeapon11.WeaponClass == 13;
					});
					equipment.TryAddElement(ref equipmentIndex4, new EquipmentElement(randomElementWithPredicate11, null, null, false));
					return equipment;
				}
			}
			Equipment equipment2;
			return equipment2;
		}

		private static Tuple<EquipmentIndex, EquipmentElement, WeaponComponentData> GetWeaponInfoOfType(this Equipment equipment, WeaponClass type)
		{
			Func<WeaponComponentData, bool> <>9__0;
			for (EquipmentIndex equipmentIndex = 0; equipmentIndex < 5; equipmentIndex++)
			{
				ItemObject item = equipment[equipmentIndex].Item;
				WeaponComponentData weaponComponentData;
				if (item == null)
				{
					weaponComponentData = null;
				}
				else
				{
					MBReadOnlyList<WeaponComponentData> weapons = item.Weapons;
					if (weapons == null)
					{
						weaponComponentData = null;
					}
					else
					{
						Func<WeaponComponentData, bool> func;
						if ((func = <>9__0) == null)
						{
							func = (<>9__0 = (WeaponComponentData w) => w.WeaponClass == type);
						}
						weaponComponentData = weapons.FirstOrDefault(func);
					}
				}
				WeaponComponentData weaponComponentData2 = weaponComponentData;
				if (weaponComponentData2 != null)
				{
					return new Tuple<EquipmentIndex, EquipmentElement, WeaponComponentData>(equipmentIndex, equipment[equipmentIndex], weaponComponentData2);
				}
			}
			return null;
		}

		private static Tuple<EquipmentIndex, EquipmentElement, WeaponComponentData> GetWeaponInfoOfPredicate(this Equipment equipment, Predicate<WeaponComponentData> predicate)
		{
			if (predicate == null)
			{
				return null;
			}
			Func<WeaponComponentData, bool> <>9__0;
			for (EquipmentIndex equipmentIndex = 0; equipmentIndex < 5; equipmentIndex++)
			{
				ItemObject item = equipment[equipmentIndex].Item;
				WeaponComponentData weaponComponentData;
				if (item == null)
				{
					weaponComponentData = null;
				}
				else
				{
					MBReadOnlyList<WeaponComponentData> weapons = item.Weapons;
					if (weapons == null)
					{
						weaponComponentData = null;
					}
					else
					{
						Func<WeaponComponentData, bool> func;
						if ((func = <>9__0) == null)
						{
							func = (<>9__0 = (WeaponComponentData w) => predicate(w));
						}
						weaponComponentData = weapons.FirstOrDefault(func);
					}
				}
				WeaponComponentData weaponComponentData2 = weaponComponentData;
				if (weaponComponentData2 != null)
				{
					return new Tuple<EquipmentIndex, EquipmentElement, WeaponComponentData>(equipmentIndex, equipment[equipmentIndex], weaponComponentData2);
				}
			}
			return null;
		}

		private static Tuple<EquipmentIndex, EquipmentElement, WeaponComponentData> GetTwoHandedWeaponInfo(this Equipment equipment)
		{
			Tuple<EquipmentIndex, EquipmentElement, WeaponComponentData> tuple;
			if ((tuple = equipment.GetWeaponInfoOfType(5)) == null && (tuple = equipment.GetWeaponInfoOfType(3)) == null)
			{
				tuple = equipment.GetWeaponInfoOfType(8) ?? equipment.GetWeaponInfoOfType(10);
			}
			return tuple;
		}

		private static bool TryAddElement(this Equipment equipment, ref EquipmentIndex eqIndex, EquipmentElement element)
		{
			if (eqIndex < 0 || eqIndex > 1)
			{
				return false;
			}
			if (Equipment.IsItemFitsToSlot(eqIndex, element.Item))
			{
				equipment[eqIndex] = element;
				eqIndex++;
			}
			return true;
		}

		private static void SwapItems(this Equipment equipment, EquipmentIndex first, EquipmentIndex second)
		{
			EquipmentElement equipmentElement = equipment[first];
			equipment[first] = equipment[second];
			equipment[second] = equipmentElement;
		}
	}
}
