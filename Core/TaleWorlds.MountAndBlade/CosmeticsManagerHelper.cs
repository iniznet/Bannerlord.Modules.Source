using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.MountAndBlade.Diamond.Cosmetics;
using TaleWorlds.MountAndBlade.Diamond.Cosmetics.CosmeticTypes;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.MountAndBlade
{
	public static class CosmeticsManagerHelper
	{
		public static Dictionary<int, List<int>> GetUsedIndicesFromIds(Dictionary<string, List<string>> usedCosmetics)
		{
			Dictionary<int, List<int>> dictionary = new Dictionary<int, List<int>>();
			MBReadOnlyList<MultiplayerClassDivisions.MPHeroClass> objectTypeList = MBObjectManager.Instance.GetObjectTypeList<MultiplayerClassDivisions.MPHeroClass>();
			foreach (KeyValuePair<string, List<string>> keyValuePair in usedCosmetics)
			{
				int num = -1;
				for (int i = 0; i < objectTypeList.Count; i++)
				{
					if (objectTypeList[i].StringId == keyValuePair.Key)
					{
						num = i;
						break;
					}
				}
				if (num != -1)
				{
					List<int> list = new List<int>();
					foreach (string text in keyValuePair.Value)
					{
						int num2 = -1;
						for (int j = 0; j < CosmeticsManager.CosmeticElementsList.Count; j++)
						{
							if (CosmeticsManager.CosmeticElementsList[j].Id == text)
							{
								num2 = j;
								break;
							}
						}
						if (num2 >= 0)
						{
							list.Add(num2);
						}
					}
					if (list.Count > 0)
					{
						dictionary.Add(num, list);
					}
				}
			}
			return dictionary;
		}

		public static ActionIndexCache GetSuitableTauntAction(Agent agent, int tauntIndex)
		{
			if (agent.Equipment == null)
			{
				return ActionIndexCache.act_none;
			}
			WeaponComponentData currentUsageItem = agent.WieldedWeapon.CurrentUsageItem;
			WeaponComponentData currentUsageItem2 = agent.WieldedOffhandWeapon.CurrentUsageItem;
			return ActionIndexCache.Create(TauntUsageManager.GetAction(tauntIndex, agent.GetIsLeftStance(), !agent.HasMount, currentUsageItem, currentUsageItem2));
		}

		public static TauntUsageManager.TauntUsage.TauntUsageFlag GetActionNotUsableReason(Agent agent, int tauntIndex)
		{
			WeaponComponentData currentUsageItem = agent.WieldedWeapon.CurrentUsageItem;
			WeaponComponentData currentUsageItem2 = agent.WieldedOffhandWeapon.CurrentUsageItem;
			return TauntUsageManager.GetIsActionNotSuitableReason(tauntIndex, agent.GetIsLeftStance(), !agent.HasMount, currentUsageItem, currentUsageItem2);
		}

		public static string GetSuitableTauntActionForEquipment(Equipment equipment, TauntCosmeticElement taunt)
		{
			if (equipment == null)
			{
				return null;
			}
			EquipmentIndex equipmentIndex;
			EquipmentIndex equipmentIndex2;
			bool flag;
			equipment.GetInitialWeaponIndicesToEquip(out equipmentIndex, out equipmentIndex2, out flag, Equipment.InitialWeaponEquipPreference.Any);
			WeaponComponentData weaponComponentData;
			if (equipmentIndex == EquipmentIndex.None)
			{
				weaponComponentData = null;
			}
			else
			{
				ItemObject item = equipment[equipmentIndex].Item;
				weaponComponentData = ((item != null) ? item.PrimaryWeapon : null);
			}
			WeaponComponentData weaponComponentData2 = weaponComponentData;
			WeaponComponentData weaponComponentData3;
			if (equipmentIndex2 == EquipmentIndex.None)
			{
				weaponComponentData3 = null;
			}
			else
			{
				ItemObject item2 = equipment[equipmentIndex2].Item;
				weaponComponentData3 = ((item2 != null) ? item2.PrimaryWeapon : null);
			}
			WeaponComponentData weaponComponentData4 = weaponComponentData3;
			return TauntUsageManager.GetAction(TauntUsageManager.GetIndexOfAction(taunt.Id), false, true, weaponComponentData2, weaponComponentData4);
		}

		public static bool IsWeaponClassOneHanded(WeaponClass weaponClass)
		{
			return weaponClass == WeaponClass.OneHandedAxe || weaponClass == WeaponClass.OneHandedPolearm || weaponClass == WeaponClass.OneHandedSword;
		}

		public static bool IsWeaponClassTwoHanded(WeaponClass weaponClass)
		{
			return weaponClass == WeaponClass.TwoHandedAxe || weaponClass == WeaponClass.TwoHandedMace || weaponClass == WeaponClass.TwoHandedPolearm || weaponClass == WeaponClass.TwoHandedSword;
		}

		public static bool IsWeaponClassShield(WeaponClass weaponClass)
		{
			return weaponClass == WeaponClass.LargeShield || weaponClass == WeaponClass.SmallShield;
		}

		public static bool IsWeaponClassBow(WeaponClass weaponClass)
		{
			return weaponClass == WeaponClass.Bow;
		}

		public static bool IsWeaponClassCrossbow(WeaponClass weaponClass)
		{
			return weaponClass == WeaponClass.Crossbow;
		}

		public static WeaponClass[] GetComplimentaryWeaponClasses(WeaponClass weaponClass)
		{
			switch (weaponClass)
			{
			case WeaponClass.OneHandedSword:
			case WeaponClass.OneHandedAxe:
			case WeaponClass.Mace:
			case WeaponClass.Pick:
			case WeaponClass.OneHandedPolearm:
			case WeaponClass.LowGripPolearm:
			case WeaponClass.Stone:
			case WeaponClass.ThrowingAxe:
			case WeaponClass.ThrowingKnife:
			case WeaponClass.Javelin:
				return new WeaponClass[]
				{
					WeaponClass.SmallShield,
					WeaponClass.LargeShield
				};
			case WeaponClass.Arrow:
				return new WeaponClass[] { WeaponClass.Bow };
			case WeaponClass.Bolt:
				return new WeaponClass[] { WeaponClass.Crossbow };
			case WeaponClass.Bow:
				return new WeaponClass[] { WeaponClass.Arrow };
			case WeaponClass.Crossbow:
				return new WeaponClass[] { WeaponClass.Bolt };
			case WeaponClass.SmallShield:
			case WeaponClass.LargeShield:
				return new WeaponClass[]
				{
					WeaponClass.OneHandedAxe,
					WeaponClass.OneHandedSword,
					WeaponClass.OneHandedPolearm,
					WeaponClass.Mace
				};
			}
			return new WeaponClass[0];
		}
	}
}
