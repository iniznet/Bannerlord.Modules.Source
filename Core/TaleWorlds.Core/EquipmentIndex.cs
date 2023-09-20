using System;

namespace TaleWorlds.Core
{
	public enum EquipmentIndex
	{
		None = -1,
		WeaponItemBeginSlot,
		Weapon0 = 0,
		Weapon1,
		Weapon2,
		Weapon3,
		ExtraWeaponSlot,
		NumAllWeaponSlots,
		NumPrimaryWeaponSlots = 4,
		NonWeaponItemBeginSlot,
		ArmorItemBeginSlot = 5,
		Head = 5,
		Body,
		Leg,
		Gloves,
		Cape,
		ArmorItemEndSlot,
		NumAllArmorSlots = 5,
		Horse = 10,
		HorseHarness,
		NumEquipmentSetSlots
	}
}
