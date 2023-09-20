using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Selector;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.ClassLoadout
{
	// Token: 0x020000D0 RID: 208
	public class ShallowItemVM : ViewModel
	{
		// Token: 0x1700066B RID: 1643
		// (get) Token: 0x06001370 RID: 4976 RVA: 0x0003FDEA File Offset: 0x0003DFEA
		// (set) Token: 0x06001371 RID: 4977 RVA: 0x0003FDF2 File Offset: 0x0003DFF2
		public ShallowItemVM.ItemGroup Type { get; private set; }

		// Token: 0x06001372 RID: 4978 RVA: 0x0003FDFB File Offset: 0x0003DFFB
		public ShallowItemVM(Action<ShallowItemVM> onSelect)
		{
			this.PropertyList = new MBBindingList<ShallowItemPropertyVM>();
			this.AlternativeUsageSelector = new SelectorVM<AlternativeUsageItemOptionVM>(new List<string>(), 0, new Action<SelectorVM<AlternativeUsageItemOptionVM>>(this.OnAlternativeUsageChanged));
			this._onSelect = onSelect;
		}

		// Token: 0x06001373 RID: 4979 RVA: 0x0003FE34 File Offset: 0x0003E034
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.RefreshWith(this._equipmentIndex, this._equipment);
			this.PropertyList.ApplyActionOnAllItems(delegate(ShallowItemPropertyVM x)
			{
				x.RefreshValues();
			});
		}

		// Token: 0x06001374 RID: 4980 RVA: 0x0003FE83 File Offset: 0x0003E083
		public override void OnFinalize()
		{
			base.OnFinalize();
			this._equipment = null;
		}

		// Token: 0x06001375 RID: 4981 RVA: 0x0003FE94 File Offset: 0x0003E094
		public void RefreshWith(EquipmentIndex equipmentIndex, Equipment equipment)
		{
			this._equipment = equipment;
			this._equipmentIndex = equipmentIndex;
			ItemObject itemObject = ((equipment != null) ? equipment[equipmentIndex].Item : null);
			if (itemObject == null || (equipmentIndex == EquipmentIndex.ArmorItemEndSlot && !itemObject.HasHorseComponent) || (equipmentIndex != EquipmentIndex.ArmorItemEndSlot && (itemObject.PrimaryWeapon == null || itemObject.PrimaryWeapon.IsAmmo)))
			{
				this.IsValid = false;
				this.Icon = new ImageIdentifierVM(ImageIdentifierType.Null);
				return;
			}
			this.IsValid = true;
			this.Name = itemObject.Name.ToString();
			this.Icon = new ImageIdentifierVM(itemObject, "");
			this.Type = ShallowItemVM.GetItemGroupType(itemObject);
			this.TypeAsString = ((this.Type == ShallowItemVM.ItemGroup.None) ? "" : this.Type.ToString());
			this.HasAnyAlternativeUsage = false;
			this.AlternativeUsageSelector.ItemList.Clear();
			if (itemObject.PrimaryWeapon != null)
			{
				for (int i = 0; i < itemObject.Weapons.Count; i++)
				{
					WeaponComponentData weaponComponentData = itemObject.Weapons[i];
					if (ShallowItemVM.IsItemUsageApplicable(weaponComponentData))
					{
						TextObject textObject = GameTexts.FindText("str_weapon_usage", weaponComponentData.WeaponDescriptionId);
						this.AlternativeUsageSelector.AddItem(new AlternativeUsageItemOptionVM(weaponComponentData.WeaponDescriptionId, textObject, textObject, this.AlternativeUsageSelector, i));
						this.HasAnyAlternativeUsage = true;
					}
				}
			}
			this.AlternativeUsageSelector.SelectedIndex = -1;
			this.AlternativeUsageSelector.SelectedIndex = 0;
			this._latestUsageOption = this.AlternativeUsageSelector.ItemList.FirstOrDefault<AlternativeUsageItemOptionVM>();
			if (this._latestUsageOption != null)
			{
				this._latestUsageOption.IsSelected = true;
			}
			this.AlternativeUsageSelector.SetOnChangeAction(new Action<SelectorVM<AlternativeUsageItemOptionVM>>(this.OnAlternativeUsageChanged));
			this.RefreshItemPropertyList(this._equipmentIndex, this._equipment, this.AlternativeUsageSelector.SelectedIndex);
			this._isInitialized = true;
		}

		// Token: 0x06001376 RID: 4982 RVA: 0x00040068 File Offset: 0x0003E268
		private void OnAlternativeUsageChanged(SelectorVM<AlternativeUsageItemOptionVM> selector)
		{
			if (this._isInitialized && selector.SelectedIndex >= 0)
			{
				if (this._latestUsageOption != null)
				{
					this._latestUsageOption.IsSelected = false;
				}
				this.RefreshItemPropertyList(this._equipmentIndex, this._equipment, selector.SelectedIndex);
				if (selector.SelectedItem != null)
				{
					selector.SelectedItem.IsSelected = true;
				}
			}
		}

		// Token: 0x06001377 RID: 4983 RVA: 0x000400C8 File Offset: 0x0003E2C8
		private void RefreshItemPropertyList(EquipmentIndex equipmentIndex, Equipment equipment, int alternativeIndex)
		{
			ItemObject item = equipment[equipmentIndex].Item;
			ItemModifier itemModifier = equipment[equipmentIndex].ItemModifier;
			this.PropertyList.Clear();
			if (item.PrimaryWeapon != null)
			{
				WeaponComponentData weaponComponentData = item.Weapons[alternativeIndex];
				ItemObject.ItemTypeEnum itemTypeFromWeaponClass = WeaponComponentData.GetItemTypeFromWeaponClass(weaponComponentData.WeaponClass);
				if (itemTypeFromWeaponClass == ItemObject.ItemTypeEnum.OneHandedWeapon || itemTypeFromWeaponClass == ItemObject.ItemTypeEnum.TwoHandedWeapon || itemTypeFromWeaponClass == ItemObject.ItemTypeEnum.Polearm)
				{
					if (weaponComponentData.SwingDamageType != DamageTypes.Invalid)
					{
						this.AddProperty(new TextObject("{=yJsE4Ayo}Swing Spd.", null), (float)weaponComponentData.GetModifiedSwingSpeed(itemModifier) / 145f, weaponComponentData.GetModifiedSwingSpeed(itemModifier));
						this.AddProperty(new TextObject("{=RNgWFLIO}Swing Dmg.", null), (float)weaponComponentData.GetModifiedSwingDamage(itemModifier) / 143f, weaponComponentData.GetModifiedSwingDamage(itemModifier));
					}
					if (weaponComponentData.ThrustDamageType != DamageTypes.Invalid)
					{
						this.AddProperty(new TextObject("{=J0vjDOFO}Thrust Spd.", null), (float)weaponComponentData.GetModifiedThrustSpeed(itemModifier) / 114f, weaponComponentData.GetModifiedThrustSpeed(itemModifier));
						this.AddProperty(new TextObject("{=Ie9I2Bha}Thrust Dmg.", null), (float)weaponComponentData.GetModifiedThrustDamage(itemModifier) / 86f, weaponComponentData.GetModifiedThrustDamage(itemModifier));
					}
					this.AddProperty(new TextObject("{=ftoSCQ0x}Length", null), (float)weaponComponentData.WeaponLength / 315f, weaponComponentData.WeaponLength);
					this.AddProperty(new TextObject("{=oibdTnXP}Handling", null), (float)weaponComponentData.GetModifiedHandling(itemModifier) / 120f, weaponComponentData.GetModifiedHandling(itemModifier));
				}
				if (itemTypeFromWeaponClass == ItemObject.ItemTypeEnum.Thrown)
				{
					this.AddProperty(new TextObject("{=ftoSCQ0x}Length", null), (float)weaponComponentData.WeaponLength / 147f, weaponComponentData.WeaponLength);
					this.AddProperty(new TextObject("{=s31DnnAf}Damage", null), (float)weaponComponentData.GetModifiedThrustDamage(itemModifier) / 94f, weaponComponentData.GetModifiedThrustDamage(itemModifier));
					this.AddProperty(new TextObject("{=QfTt7YRB}Fire Rate", null), (float)weaponComponentData.GetModifiedMissileSpeed(itemModifier) / 115f, weaponComponentData.GetModifiedMissileSpeed(itemModifier));
					this.AddProperty(new TextObject("{=TAnabTdy}Accuracy", null), (float)weaponComponentData.Accuracy / 300f, weaponComponentData.Accuracy);
					this.AddProperty(new TextObject("{=b31ITmm0}Stack Amnt.", null), (float)weaponComponentData.GetModifiedStackCount(itemModifier) / 40f, (int)weaponComponentData.GetModifiedStackCount(itemModifier));
				}
				if (itemTypeFromWeaponClass == ItemObject.ItemTypeEnum.Shield)
				{
					this.AddProperty(new TextObject("{=6GSXsdeX}Speed", null), (float)weaponComponentData.GetModifiedThrustSpeed(itemModifier) / 120f, weaponComponentData.GetModifiedThrustSpeed(itemModifier));
					this.AddProperty(new TextObject("{=GGseMDd3}Durability", null), (float)weaponComponentData.GetModifiedMaximumHitPoints(itemModifier) / 500f, (int)weaponComponentData.GetModifiedMaximumHitPoints(itemModifier));
					this.AddProperty(new TextObject("{=ahiBhAqU}Armor", null), (float)weaponComponentData.GetModifiedArmor(itemModifier) / 40f, weaponComponentData.GetModifiedArmor(itemModifier));
					this.AddProperty(new TextObject("{=4Dd2xgPm}Weight", null), item.Weight / 40f, (int)item.Weight);
				}
				if (itemTypeFromWeaponClass == ItemObject.ItemTypeEnum.Bow || itemTypeFromWeaponClass == ItemObject.ItemTypeEnum.Crossbow)
				{
					int num = 0;
					float num2 = 0f;
					int num3 = 0;
					for (EquipmentIndex equipmentIndex2 = EquipmentIndex.WeaponItemBeginSlot; equipmentIndex2 < EquipmentIndex.ExtraWeaponSlot; equipmentIndex2++)
					{
						ItemObject item2 = equipment[equipmentIndex2].Item;
						ItemModifier itemModifier2 = equipment[equipmentIndex2].ItemModifier;
						if (item2 != null && item2.PrimaryWeapon.IsAmmo)
						{
							num += (int)item2.PrimaryWeapon.GetModifiedStackCount(itemModifier2);
							num3 += item2.PrimaryWeapon.GetModifiedThrustDamage(itemModifier2);
							num2 += 1f;
						}
					}
					num3 = MathF.Round((float)num3 / num2);
					this.AddProperty(new TextObject("{=ftoSCQ0x}Length", null), (float)weaponComponentData.WeaponLength / 123f, weaponComponentData.WeaponLength);
					this.AddProperty(new TextObject("{=s31DnnAf}Damage", null), (float)(weaponComponentData.GetModifiedThrustDamage(itemModifier) + num3) / 70f, weaponComponentData.GetModifiedThrustDamage(itemModifier) + num3);
					this.AddProperty(new TextObject("{=QfTt7YRB}Fire Rate", null), (float)weaponComponentData.GetModifiedSwingSpeed(itemModifier) / 120f, weaponComponentData.GetModifiedSwingSpeed(itemModifier));
					this.AddProperty(new TextObject("{=TAnabTdy}Accuracy", null), (float)weaponComponentData.Accuracy / 105f, weaponComponentData.Accuracy);
					this.AddProperty(new TextObject("{=yUpH2mQ4}Ammo", null), (float)num / 90f, num);
				}
			}
			if (item.HorseComponent != null)
			{
				EquipmentElement equipmentElement = equipment[EquipmentIndex.ArmorItemEndSlot];
				EquipmentElement equipmentElement2 = equipment[EquipmentIndex.HorseHarness];
				int modifiedMountCharge = equipmentElement.GetModifiedMountCharge(equipmentElement2);
				int num4 = (int)(4.33f * (float)equipmentElement.GetModifiedMountSpeed(equipmentElement2));
				int modifiedMountManeuver = equipmentElement.GetModifiedMountManeuver(equipmentElement2);
				int modifiedMountHitPoints = equipmentElement.GetModifiedMountHitPoints();
				int modifiedMountBodyArmor = equipmentElement2.GetModifiedMountBodyArmor();
				this.AddProperty(new TextObject("{=DAVb2Pzg}Charge Dmg.", null), (float)modifiedMountCharge / 35f, modifiedMountCharge);
				this.AddProperty(new TextObject("{=6GSXsdeX}Speed", null), (float)num4 / 303.1f, num4);
				this.AddProperty(new TextObject("{=rg7OuWS2}Maneuver", null), (float)modifiedMountManeuver / 70f, modifiedMountManeuver);
				this.AddProperty(new TextObject("{=oBbiVeKE}Hit Points", null), (float)modifiedMountHitPoints / 300f, modifiedMountHitPoints);
				this.AddProperty(new TextObject("{=kftE5nvv}Horse Armor", null), (float)modifiedMountBodyArmor / 100f, modifiedMountBodyArmor);
			}
		}

		// Token: 0x06001378 RID: 4984 RVA: 0x000405C6 File Offset: 0x0003E7C6
		private void AddProperty(TextObject name, float fraction, int value)
		{
			this.PropertyList.Add(new ShallowItemPropertyVM(name, MathF.Round(fraction * 1000f), value));
		}

		// Token: 0x06001379 RID: 4985 RVA: 0x000405E8 File Offset: 0x0003E7E8
		private static ShallowItemVM.ItemGroup GetItemGroupType(ItemObject item)
		{
			if (item.WeaponComponent != null)
			{
				switch (item.WeaponComponent.PrimaryWeapon.WeaponClass)
				{
				case WeaponClass.OneHandedSword:
				case WeaponClass.TwoHandedSword:
					return ShallowItemVM.ItemGroup.Sword;
				case WeaponClass.OneHandedAxe:
				case WeaponClass.TwoHandedAxe:
					return ShallowItemVM.ItemGroup.Axe;
				case WeaponClass.Mace:
				case WeaponClass.TwoHandedMace:
					return ShallowItemVM.ItemGroup.Mace;
				case WeaponClass.OneHandedPolearm:
				case WeaponClass.TwoHandedPolearm:
				case WeaponClass.LowGripPolearm:
					return ShallowItemVM.ItemGroup.Spear;
				case WeaponClass.Arrow:
				case WeaponClass.Bolt:
				case WeaponClass.Cartridge:
				case WeaponClass.Musket:
					return ShallowItemVM.ItemGroup.Ammo;
				case WeaponClass.Bow:
					return ShallowItemVM.ItemGroup.Bow;
				case WeaponClass.Crossbow:
					return ShallowItemVM.ItemGroup.Crossbow;
				case WeaponClass.Stone:
					return ShallowItemVM.ItemGroup.Stone;
				case WeaponClass.ThrowingAxe:
					return ShallowItemVM.ItemGroup.ThrowingAxe;
				case WeaponClass.ThrowingKnife:
					return ShallowItemVM.ItemGroup.ThrowingKnife;
				case WeaponClass.Javelin:
					return ShallowItemVM.ItemGroup.Javelin;
				case WeaponClass.SmallShield:
				case WeaponClass.LargeShield:
					return ShallowItemVM.ItemGroup.Shield;
				}
				return ShallowItemVM.ItemGroup.None;
			}
			if (item.HasHorseComponent)
			{
				return ShallowItemVM.ItemGroup.Mount;
			}
			return ShallowItemVM.ItemGroup.None;
		}

		// Token: 0x0600137A RID: 4986 RVA: 0x000406A5 File Offset: 0x0003E8A5
		[UsedImplicitly]
		public void OnSelect()
		{
			this._onSelect(this);
		}

		// Token: 0x0600137B RID: 4987 RVA: 0x000406B3 File Offset: 0x0003E8B3
		public static bool IsItemUsageApplicable(WeaponComponentData weapon)
		{
			WeaponDescription weaponDescription = ((weapon != null && weapon.WeaponDescriptionId != null) ? MBObjectManager.Instance.GetObject<WeaponDescription>(weapon.WeaponDescriptionId) : null);
			return weaponDescription != null && !weaponDescription.IsHiddenFromUI;
		}

		// Token: 0x1700066C RID: 1644
		// (get) Token: 0x0600137C RID: 4988 RVA: 0x000406E1 File Offset: 0x0003E8E1
		// (set) Token: 0x0600137D RID: 4989 RVA: 0x000406E9 File Offset: 0x0003E8E9
		[DataSourceProperty]
		public MBBindingList<ShallowItemPropertyVM> PropertyList
		{
			get
			{
				return this._propertyList;
			}
			set
			{
				if (value != this._propertyList)
				{
					this._propertyList = value;
					base.OnPropertyChangedWithValue<MBBindingList<ShallowItemPropertyVM>>(value, "PropertyList");
				}
			}
		}

		// Token: 0x1700066D RID: 1645
		// (get) Token: 0x0600137E RID: 4990 RVA: 0x00040707 File Offset: 0x0003E907
		// (set) Token: 0x0600137F RID: 4991 RVA: 0x0004070F File Offset: 0x0003E90F
		[DataSourceProperty]
		public string Name
		{
			get
			{
				return this._name;
			}
			set
			{
				if (value != this._name)
				{
					this._name = value;
					base.OnPropertyChangedWithValue<string>(value, "Name");
				}
			}
		}

		// Token: 0x1700066E RID: 1646
		// (get) Token: 0x06001380 RID: 4992 RVA: 0x00040732 File Offset: 0x0003E932
		// (set) Token: 0x06001381 RID: 4993 RVA: 0x0004073A File Offset: 0x0003E93A
		[DataSourceProperty]
		public ImageIdentifierVM Icon
		{
			get
			{
				return this._icon;
			}
			set
			{
				if (value != this._icon)
				{
					this._icon = value;
					base.OnPropertyChangedWithValue<ImageIdentifierVM>(value, "Icon");
				}
			}
		}

		// Token: 0x1700066F RID: 1647
		// (get) Token: 0x06001382 RID: 4994 RVA: 0x00040758 File Offset: 0x0003E958
		// (set) Token: 0x06001383 RID: 4995 RVA: 0x00040760 File Offset: 0x0003E960
		[DataSourceProperty]
		public string TypeAsString
		{
			get
			{
				return this._typeAsString;
			}
			set
			{
				if (value != this._typeAsString)
				{
					this._typeAsString = value;
					base.OnPropertyChangedWithValue<string>(value, "TypeAsString");
				}
			}
		}

		// Token: 0x17000670 RID: 1648
		// (get) Token: 0x06001384 RID: 4996 RVA: 0x00040783 File Offset: 0x0003E983
		// (set) Token: 0x06001385 RID: 4997 RVA: 0x0004078B File Offset: 0x0003E98B
		[DataSourceProperty]
		public bool IsSelected
		{
			get
			{
				return this._isSelected;
			}
			set
			{
				if (value != this._isSelected)
				{
					this._isSelected = value;
					base.OnPropertyChangedWithValue(value, "IsSelected");
				}
			}
		}

		// Token: 0x17000671 RID: 1649
		// (get) Token: 0x06001386 RID: 4998 RVA: 0x000407A9 File Offset: 0x0003E9A9
		// (set) Token: 0x06001387 RID: 4999 RVA: 0x000407B1 File Offset: 0x0003E9B1
		[DataSourceProperty]
		public bool HasAnyAlternativeUsage
		{
			get
			{
				return this._hasAnyAlternativeUsage;
			}
			set
			{
				if (value != this._hasAnyAlternativeUsage)
				{
					this._hasAnyAlternativeUsage = value;
					base.OnPropertyChangedWithValue(value, "HasAnyAlternativeUsage");
				}
			}
		}

		// Token: 0x17000672 RID: 1650
		// (get) Token: 0x06001388 RID: 5000 RVA: 0x000407CF File Offset: 0x0003E9CF
		// (set) Token: 0x06001389 RID: 5001 RVA: 0x000407D7 File Offset: 0x0003E9D7
		[DataSourceProperty]
		public bool IsValid
		{
			get
			{
				return this._isValid;
			}
			set
			{
				if (value != this._isValid)
				{
					this._isValid = value;
					base.OnPropertyChangedWithValue(value, "IsValid");
				}
			}
		}

		// Token: 0x17000673 RID: 1651
		// (get) Token: 0x0600138A RID: 5002 RVA: 0x000407F5 File Offset: 0x0003E9F5
		// (set) Token: 0x0600138B RID: 5003 RVA: 0x000407FD File Offset: 0x0003E9FD
		[DataSourceProperty]
		public SelectorVM<AlternativeUsageItemOptionVM> AlternativeUsageSelector
		{
			get
			{
				return this._alternativeUsageSelector;
			}
			set
			{
				if (value != this._alternativeUsageSelector)
				{
					this._alternativeUsageSelector = value;
					base.OnPropertyChangedWithValue<SelectorVM<AlternativeUsageItemOptionVM>>(value, "AlternativeUsageSelector");
				}
			}
		}

		// Token: 0x04000958 RID: 2392
		private readonly Action<ShallowItemVM> _onSelect;

		// Token: 0x0400095A RID: 2394
		private AlternativeUsageItemOptionVM _latestUsageOption;

		// Token: 0x0400095B RID: 2395
		private Equipment _equipment;

		// Token: 0x0400095C RID: 2396
		private EquipmentIndex _equipmentIndex;

		// Token: 0x0400095D RID: 2397
		private bool _isInitialized;

		// Token: 0x0400095E RID: 2398
		private ImageIdentifierVM _icon;

		// Token: 0x0400095F RID: 2399
		private string _name;

		// Token: 0x04000960 RID: 2400
		private string _typeAsString;

		// Token: 0x04000961 RID: 2401
		private bool _isValid;

		// Token: 0x04000962 RID: 2402
		private bool _isSelected;

		// Token: 0x04000963 RID: 2403
		private bool _hasAnyAlternativeUsage;

		// Token: 0x04000964 RID: 2404
		private MBBindingList<ShallowItemPropertyVM> _propertyList;

		// Token: 0x04000965 RID: 2405
		private SelectorVM<AlternativeUsageItemOptionVM> _alternativeUsageSelector;

		// Token: 0x02000228 RID: 552
		public enum ItemGroup
		{
			// Token: 0x04000E9E RID: 3742
			None,
			// Token: 0x04000E9F RID: 3743
			Spear,
			// Token: 0x04000EA0 RID: 3744
			Javelin,
			// Token: 0x04000EA1 RID: 3745
			Bow,
			// Token: 0x04000EA2 RID: 3746
			Crossbow,
			// Token: 0x04000EA3 RID: 3747
			Sword,
			// Token: 0x04000EA4 RID: 3748
			Axe,
			// Token: 0x04000EA5 RID: 3749
			Mace,
			// Token: 0x04000EA6 RID: 3750
			ThrowingAxe,
			// Token: 0x04000EA7 RID: 3751
			ThrowingKnife,
			// Token: 0x04000EA8 RID: 3752
			Ammo,
			// Token: 0x04000EA9 RID: 3753
			Shield,
			// Token: 0x04000EAA RID: 3754
			Mount,
			// Token: 0x04000EAB RID: 3755
			Stone
		}
	}
}
