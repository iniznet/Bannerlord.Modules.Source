using System;
using TaleWorlds.Library;

namespace TaleWorlds.Core
{
	// Token: 0x0200004E RID: 78
	public class DefaultItemValueModel : ItemValueModel
	{
		// Token: 0x060005D1 RID: 1489 RVA: 0x000154EC File Offset: 0x000136EC
		private float CalculateArmorTier(ArmorComponent armorComponent)
		{
			float num = 1.2f * (float)armorComponent.HeadArmor + 1f * (float)armorComponent.BodyArmor + 1f * (float)armorComponent.LegArmor + 1f * (float)armorComponent.ArmArmor;
			if (armorComponent.Item.ItemType == ItemObject.ItemTypeEnum.LegArmor)
			{
				num *= 1.6f;
			}
			else if (armorComponent.Item.ItemType == ItemObject.ItemTypeEnum.HandArmor)
			{
				num *= 1.7f;
			}
			else if (armorComponent.Item.ItemType == ItemObject.ItemTypeEnum.HeadArmor)
			{
				num *= 1.2f;
			}
			else if (armorComponent.Item.ItemType == ItemObject.ItemTypeEnum.Cape)
			{
				num *= 1.8f;
			}
			return num * 0.1f - 0.4f;
		}

		// Token: 0x060005D2 RID: 1490 RVA: 0x000155A0 File Offset: 0x000137A0
		private float CalculateHorseTier(HorseComponent horseComponent)
		{
			return (float)horseComponent.Speed * 0.12f + (float)horseComponent.Maneuver * 0.07f + (float)horseComponent.HitPointBonus * 0.01f + (float)horseComponent.ChargeDamage * 0.15f - 11.5f;
		}

		// Token: 0x060005D3 RID: 1491 RVA: 0x000155DF File Offset: 0x000137DF
		private float CalculateSaddleTier(SaddleComponent saddleComponent)
		{
			return 0f;
		}

		// Token: 0x060005D4 RID: 1492 RVA: 0x000155E8 File Offset: 0x000137E8
		private float CalculateWeaponTier(WeaponComponent weaponComponent)
		{
			ItemObject item = weaponComponent.Item;
			WeaponDesign weaponDesign = ((item != null) ? item.WeaponDesign : null);
			if (weaponDesign != null)
			{
				float num = this.CalculateTierCraftedWeapon(weaponDesign);
				float num2 = this.CalculateTierMeleeWeapon(weaponComponent);
				return 0.6f * num2 + 0.4f * num;
			}
			return this.CalculateTierNonCraftedWeapon(weaponComponent);
		}

		// Token: 0x060005D5 RID: 1493 RVA: 0x00015638 File Offset: 0x00013838
		private float CalculateTierMeleeWeapon(WeaponComponent weaponComponent)
		{
			float num = float.MinValue;
			float num2 = float.MinValue;
			for (int i = 0; i < weaponComponent.Weapons.Count; i++)
			{
				WeaponComponentData weaponComponentData = weaponComponent.Weapons[i];
				float num3 = (float)weaponComponentData.ThrustDamage * this.GetFactor(weaponComponentData.ThrustDamageType) * MathF.Pow((float)weaponComponentData.ThrustSpeed * 0.01f, 1.5f);
				float num4 = (float)weaponComponentData.SwingDamage * this.GetFactor(weaponComponentData.SwingDamageType) * MathF.Pow((float)weaponComponentData.SwingSpeed * 0.01f, 1.5f);
				float num5 = MathF.Max(num3, num4 * 1.1f);
				if (weaponComponentData.WeaponFlags.HasAnyFlag(WeaponFlags.NotUsableWithOneHand))
				{
					num5 *= 0.8f;
				}
				if (weaponComponentData.WeaponClass == WeaponClass.ThrowingKnife || weaponComponentData.WeaponClass == WeaponClass.ThrowingAxe)
				{
					num5 *= 1.2f;
				}
				if (weaponComponentData.WeaponClass == WeaponClass.Javelin)
				{
					num5 *= 0.6f;
				}
				float num6 = (float)weaponComponentData.WeaponLength * 0.01f;
				float num7 = 0.06f * (num5 * (1f + num6)) - 3.5f;
				if (num7 > num2)
				{
					if (num7 >= num)
					{
						num2 = num;
						num = num7;
					}
					else
					{
						num2 = num7;
					}
				}
			}
			num = MathF.Clamp(num, -1.5f, 7.5f);
			if (num2 != -3.4028235E+38f)
			{
				num2 = MathF.Clamp(num2, -1.5f, 7.5f);
			}
			if (weaponComponent.Weapons.Count <= 1)
			{
				return num;
			}
			return num * MathF.Pow(1f + (num2 + 1.5f) / (num + 2.5f), 0.2f);
		}

		// Token: 0x060005D6 RID: 1494 RVA: 0x000157C6 File Offset: 0x000139C6
		private float GetFactor(DamageTypes swingDamageType)
		{
			if (swingDamageType == DamageTypes.Blunt)
			{
				return 1.45f;
			}
			if (swingDamageType != DamageTypes.Pierce)
			{
				return 1f;
			}
			return 1.15f;
		}

		// Token: 0x060005D7 RID: 1495 RVA: 0x000157E4 File Offset: 0x000139E4
		private float CalculateTierNonCraftedWeapon(WeaponComponent weaponComponent)
		{
			ItemObject item = weaponComponent.Item;
			ItemObject.ItemTypeEnum itemTypeEnum = ((item != null) ? item.ItemType : ItemObject.ItemTypeEnum.Invalid);
			if (itemTypeEnum == ItemObject.ItemTypeEnum.Crossbow || itemTypeEnum == ItemObject.ItemTypeEnum.Bow || itemTypeEnum == ItemObject.ItemTypeEnum.Musket || itemTypeEnum == ItemObject.ItemTypeEnum.Pistol)
			{
				return this.CalculateRangedWeaponTier(weaponComponent);
			}
			if (itemTypeEnum == ItemObject.ItemTypeEnum.Arrows || itemTypeEnum == ItemObject.ItemTypeEnum.Bolts || itemTypeEnum == ItemObject.ItemTypeEnum.Bullets)
			{
				return this.CalculateAmmoTier(weaponComponent);
			}
			if (itemTypeEnum == ItemObject.ItemTypeEnum.Shield)
			{
				return this.CalculateShieldTier(weaponComponent);
			}
			return 0f;
		}

		// Token: 0x060005D8 RID: 1496 RVA: 0x00015848 File Offset: 0x00013A48
		private float CalculateRangedWeaponTier(WeaponComponent weaponComponent)
		{
			WeaponComponentData weaponComponentData = weaponComponent.Weapons[0];
			ItemObject item = weaponComponent.Item;
			int num = (int)((item != null) ? item.ItemType : ItemObject.ItemTypeEnum.Invalid);
			float num2 = 0f;
			if (num == 9)
			{
				num2 += -1.5f;
			}
			if (weaponComponentData.ItemUsage.Contains("light"))
			{
				num2 += 1.25f;
			}
			if (!weaponComponent.PrimaryWeapon.ItemUsage.Contains("long_bow") && !weaponComponent.PrimaryWeapon.WeaponFlags.HasAnyFlag(WeaponFlags.CantReloadOnHorseback))
			{
				num2 += 0.5f;
			}
			int thrustDamage = weaponComponentData.ThrustDamage;
			int missileSpeed = weaponComponentData.MissileSpeed;
			int accuracy = weaponComponentData.Accuracy;
			return (float)thrustDamage * 0.1f + (float)missileSpeed * 0.02f + (float)accuracy * 0.05f - 9.25f + num2;
		}

		// Token: 0x060005D9 RID: 1497 RVA: 0x0001590C File Offset: 0x00013B0C
		private float CalculateShieldTier(WeaponComponent weaponComponent)
		{
			WeaponComponentData weaponComponentData = weaponComponent.Weapons[0];
			return ((float)weaponComponentData.MaxDataValue + 3f * (float)weaponComponentData.BodyArmor + (float)weaponComponentData.ThrustSpeed) / (6f + weaponComponent.Item.Weight) * 0.13f - 3f;
		}

		// Token: 0x060005DA RID: 1498 RVA: 0x00015964 File Offset: 0x00013B64
		private float CalculateAmmoTier(WeaponComponent weaponComponent)
		{
			WeaponComponentData weaponComponentData = weaponComponent.Weapons[0];
			float missileDamage = (float)weaponComponentData.MissileDamage;
			int num = MathF.Max(0, (int)(weaponComponentData.MaxDataValue - 20));
			return missileDamage + (float)num * 0.1f;
		}

		// Token: 0x060005DB RID: 1499 RVA: 0x000159A0 File Offset: 0x00013BA0
		private float CalculateTierCraftedWeapon(WeaponDesign craftingData)
		{
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			foreach (WeaponDesignElement weaponDesignElement in craftingData.UsedPieces)
			{
				if (weaponDesignElement.CraftingPiece.IsValid)
				{
					num += weaponDesignElement.CraftingPiece.PieceTier;
					num2++;
					foreach (ValueTuple<CraftingMaterials, int> valueTuple in weaponDesignElement.CraftingPiece.MaterialsUsed)
					{
						CraftingMaterials item = valueTuple.Item1;
						int item2 = valueTuple.Item2;
						int num5 = ((item == CraftingMaterials.Wood) ? (-1) : ((item == CraftingMaterials.Iron1) ? 1 : ((item == CraftingMaterials.Iron2) ? 2 : ((item == CraftingMaterials.Iron3) ? 3 : ((item == CraftingMaterials.Iron4) ? 4 : ((item == CraftingMaterials.Iron5) ? 5 : ((item == CraftingMaterials.Iron6) ? 6 : (-1))))))));
						if (num5 >= 0)
						{
							num3 += item2 * num5;
							num4 += item2;
						}
					}
				}
			}
			if (num4 > 0 && num2 > 0)
			{
				return 0.4f * (1.25f * (float)num / (float)num2) + 0.6f * ((float)num3 * 1.3f / ((float)num4 + 0.6f) - 1.3f);
			}
			if (num2 > 0)
			{
				return (float)num / (float)num2;
			}
			return 0.1f;
		}

		// Token: 0x060005DC RID: 1500 RVA: 0x00015AEC File Offset: 0x00013CEC
		public override int CalculateValue(ItemObject item)
		{
			float num = 1f;
			if (item.ItemComponent != null)
			{
				num = this.GetEquipmentValueFromTier(item.Tierf);
			}
			float num2 = 1f;
			if (item.ItemComponent is ArmorComponent)
			{
				num2 = (float)((item.ItemType == ItemObject.ItemTypeEnum.BodyArmor) ? 120 : ((item.ItemType == ItemObject.ItemTypeEnum.HandArmor) ? 120 : ((item.ItemType == ItemObject.ItemTypeEnum.LegArmor) ? 120 : 100)));
			}
			else if (item.ItemComponent is WeaponComponent)
			{
				num2 = 100f;
			}
			else if (item.ItemComponent is HorseComponent)
			{
				num2 = 100f;
			}
			else if (item.ItemComponent is SaddleComponent)
			{
				num2 = 100f;
			}
			else if (item.ItemComponent is TradeItemComponent)
			{
				num2 = 100f;
			}
			else if (item.ItemComponent is BannerComponent)
			{
				num2 = 100f;
			}
			return (int)(num2 * num * (1f + 0.2f * (item.Appearance - 1f)) + 100f * MathF.Max(0f, item.Appearance - 1f));
		}

		// Token: 0x060005DD RID: 1501 RVA: 0x00015BF8 File Offset: 0x00013DF8
		private float GetWeaponPriceFactor(ItemObject item)
		{
			return 100f;
		}

		// Token: 0x060005DE RID: 1502 RVA: 0x00015BFF File Offset: 0x00013DFF
		public override float GetEquipmentValueFromTier(float itemTierf)
		{
			return MathF.Pow(2.75f, MathF.Clamp(itemTierf, -1f, 7.5f));
		}

		// Token: 0x060005DF RID: 1503 RVA: 0x00015C1C File Offset: 0x00013E1C
		public override float CalculateTier(ItemObject item)
		{
			if (item.ItemComponent is ArmorComponent)
			{
				return this.CalculateArmorTier(item.ItemComponent as ArmorComponent);
			}
			if (item.ItemComponent is BannerComponent)
			{
				return this.CalculateBannerTier(item, item.ItemComponent as BannerComponent);
			}
			if (item.ItemComponent is WeaponComponent)
			{
				return this.CalculateWeaponTier(item.ItemComponent as WeaponComponent);
			}
			if (item.ItemComponent is HorseComponent)
			{
				return this.CalculateHorseTier(item.ItemComponent as HorseComponent);
			}
			if (item.ItemComponent is SaddleComponent)
			{
				return this.CalculateSaddleTier(item.ItemComponent as SaddleComponent);
			}
			return 0f;
		}

		// Token: 0x060005E0 RID: 1504 RVA: 0x00015CCA File Offset: 0x00013ECA
		private float CalculateBannerTier(ItemObject item, BannerComponent bannerComponent)
		{
			return this.GetBannerItemCultureBonus(item.Culture) + this.GetBannerItemLevelBonus(bannerComponent.BannerLevel);
		}

		// Token: 0x060005E1 RID: 1505 RVA: 0x00015CE5 File Offset: 0x00013EE5
		private float GetBannerItemCultureBonus(BasicCultureObject culture)
		{
			if (culture == null)
			{
				return 0f;
			}
			return 1f;
		}

		// Token: 0x060005E2 RID: 1506 RVA: 0x00015CF5 File Offset: 0x00013EF5
		private float GetBannerItemLevelBonus(int bannerLevel)
		{
			if (bannerLevel == 3)
			{
				return 5f;
			}
			if (bannerLevel == 2)
			{
				return 3f;
			}
			return 1f;
		}
	}
}
