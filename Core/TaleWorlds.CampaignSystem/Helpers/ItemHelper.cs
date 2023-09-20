using System;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace Helpers
{
	// Token: 0x0200000A RID: 10
	public static class ItemHelper
	{
		// Token: 0x0600003C RID: 60 RVA: 0x000049D8 File Offset: 0x00002BD8
		public static bool IsWeaponComparableWithUsage(ItemObject item, string comparedUsageId)
		{
			for (int i = 0; i < item.Weapons.Count; i++)
			{
				if (item.Weapons[i].WeaponDescriptionId == comparedUsageId || (comparedUsageId == "OneHandedBastardSword" && item.Weapons[i].WeaponDescriptionId == "OneHandedSword") || (comparedUsageId == "OneHandedSword" && item.Weapons[i].WeaponDescriptionId == "OneHandedBastardSword"))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600003D RID: 61 RVA: 0x00004A70 File Offset: 0x00002C70
		public static bool IsWeaponComparableWithUsage(ItemObject item, string comparedUsageId, out int comparableUsageIndex)
		{
			comparableUsageIndex = -1;
			for (int i = 0; i < item.Weapons.Count; i++)
			{
				if (item.Weapons[i].WeaponDescriptionId == comparedUsageId || (comparedUsageId == "OneHandedBastardSword" && item.Weapons[i].WeaponDescriptionId == "OneHandedSword") || (comparedUsageId == "OneHandedSword" && item.Weapons[i].WeaponDescriptionId == "OneHandedBastardSword"))
				{
					comparableUsageIndex = i;
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600003E RID: 62 RVA: 0x00004B0C File Offset: 0x00002D0C
		public static bool CheckComparability(ItemObject item, ItemObject comparedItem)
		{
			if (item == null || comparedItem == null)
			{
				return false;
			}
			if (item.PrimaryWeapon != null && comparedItem.PrimaryWeapon != null && ((item.PrimaryWeapon.IsMeleeWeapon && comparedItem.PrimaryWeapon.IsMeleeWeapon) || (item.PrimaryWeapon.IsRangedWeapon && item.PrimaryWeapon.IsConsumable && comparedItem.PrimaryWeapon.IsRangedWeapon && comparedItem.PrimaryWeapon.IsConsumable) || (!item.PrimaryWeapon.IsRangedWeapon && item.PrimaryWeapon.IsConsumable && !comparedItem.PrimaryWeapon.IsRangedWeapon && comparedItem.PrimaryWeapon.IsConsumable) || (item.PrimaryWeapon.IsShield && comparedItem.PrimaryWeapon.IsShield)))
			{
				WeaponComponentData primaryWeapon = item.PrimaryWeapon;
				return ItemHelper.IsWeaponComparableWithUsage(comparedItem, primaryWeapon.WeaponDescriptionId);
			}
			return item.Type == comparedItem.Type;
		}

		// Token: 0x0600003F RID: 63 RVA: 0x00004BF8 File Offset: 0x00002DF8
		public static bool CheckComparability(ItemObject item, ItemObject comparedItem, int usageIndex)
		{
			if (item == null || comparedItem == null)
			{
				return false;
			}
			if (item.PrimaryWeapon != null && ((item.PrimaryWeapon.IsMeleeWeapon && comparedItem.PrimaryWeapon.IsMeleeWeapon) || (item.PrimaryWeapon.IsRangedWeapon && item.PrimaryWeapon.IsConsumable && comparedItem.PrimaryWeapon.IsRangedWeapon && comparedItem.PrimaryWeapon.IsConsumable) || (!item.PrimaryWeapon.IsRangedWeapon && item.PrimaryWeapon.IsConsumable && !comparedItem.PrimaryWeapon.IsRangedWeapon && comparedItem.PrimaryWeapon.IsConsumable) || (item.PrimaryWeapon.IsShield && comparedItem.PrimaryWeapon.IsShield)))
			{
				WeaponComponentData weaponComponentData = item.Weapons[usageIndex];
				return ItemHelper.IsWeaponComparableWithUsage(comparedItem, weaponComponentData.WeaponDescriptionId);
			}
			return item.Type == comparedItem.Type;
		}

		// Token: 0x06000040 RID: 64 RVA: 0x00004CDF File Offset: 0x00002EDF
		private static TextObject GetDamageDescription(int damage, DamageTypes damageType)
		{
			TextObject textObject = new TextObject("{=vvCwVo7i}{DAMAGE} {DAMAGE_TYPE}", null);
			textObject.SetTextVariable("DAMAGE", damage);
			textObject.SetTextVariable("DAMAGE_TYPE", GameTexts.FindText("str_damage_types", damageType.ToString()));
			return textObject;
		}

		// Token: 0x06000041 RID: 65 RVA: 0x00004D1C File Offset: 0x00002F1C
		public static TextObject GetSwingDamageText(WeaponComponentData weapon, ItemModifier itemModifier)
		{
			int modifiedSwingDamage = weapon.GetModifiedSwingDamage(itemModifier);
			DamageTypes swingDamageType = weapon.SwingDamageType;
			return ItemHelper.GetDamageDescription(modifiedSwingDamage, swingDamageType);
		}

		// Token: 0x06000042 RID: 66 RVA: 0x00004D40 File Offset: 0x00002F40
		public static TextObject GetMissileDamageText(WeaponComponentData weapon, ItemModifier itemModifier)
		{
			int modifiedMissileDamage = weapon.GetModifiedMissileDamage(itemModifier);
			DamageTypes damageTypes = ((weapon.WeaponClass == WeaponClass.ThrowingAxe) ? weapon.SwingDamageType : weapon.ThrustDamageType);
			return ItemHelper.GetDamageDescription(modifiedMissileDamage, damageTypes);
		}

		// Token: 0x06000043 RID: 67 RVA: 0x00004D74 File Offset: 0x00002F74
		public static TextObject GetThrustDamageText(WeaponComponentData weapon, ItemModifier itemModifier)
		{
			int modifiedThrustDamage = weapon.GetModifiedThrustDamage(itemModifier);
			DamageTypes thrustDamageType = weapon.ThrustDamageType;
			return ItemHelper.GetDamageDescription(modifiedThrustDamage, thrustDamageType);
		}

		// Token: 0x06000044 RID: 68 RVA: 0x00004D95 File Offset: 0x00002F95
		public static TextObject NumberOfItems(int number, ItemObject item)
		{
			TextObject textObject = new TextObject("{=siWNDxgo}{.%}{?NUMBER_OF_ITEM > 1}{NUMBER_OF_ITEM} {PLURAL(ITEM)}{?}one {ITEM}{\\?}{.%}", null);
			textObject.SetTextVariable("ITEM", item.Name);
			textObject.SetTextVariable("NUMBER_OF_ITEM", number);
			return textObject;
		}
	}
}
