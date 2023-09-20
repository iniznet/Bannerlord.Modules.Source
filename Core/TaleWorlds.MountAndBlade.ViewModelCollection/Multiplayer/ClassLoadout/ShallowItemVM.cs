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
	public class ShallowItemVM : ViewModel
	{
		public ShallowItemVM.ItemGroup Type { get; private set; }

		public ShallowItemVM(Action<ShallowItemVM> onSelect)
		{
			this.PropertyList = new MBBindingList<ShallowItemPropertyVM>();
			this.AlternativeUsageSelector = new SelectorVM<AlternativeUsageItemOptionVM>(new List<string>(), 0, new Action<SelectorVM<AlternativeUsageItemOptionVM>>(this.OnAlternativeUsageChanged));
			this._onSelect = onSelect;
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.RefreshWith(this._equipmentIndex, this._equipment);
			this.PropertyList.ApplyActionOnAllItems(delegate(ShallowItemPropertyVM x)
			{
				x.RefreshValues();
			});
		}

		public override void OnFinalize()
		{
			base.OnFinalize();
			this._equipment = null;
		}

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

		private void AddProperty(TextObject name, float fraction, int value)
		{
			this.PropertyList.Add(new ShallowItemPropertyVM(name, MathF.Round(fraction * 1000f), value));
		}

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

		[UsedImplicitly]
		public void OnSelect()
		{
			this._onSelect(this);
		}

		public static bool IsItemUsageApplicable(WeaponComponentData weapon)
		{
			WeaponDescription weaponDescription = ((weapon != null && weapon.WeaponDescriptionId != null) ? MBObjectManager.Instance.GetObject<WeaponDescription>(weapon.WeaponDescriptionId) : null);
			return weaponDescription != null && !weaponDescription.IsHiddenFromUI;
		}

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

		private readonly Action<ShallowItemVM> _onSelect;

		private AlternativeUsageItemOptionVM _latestUsageOption;

		private Equipment _equipment;

		private EquipmentIndex _equipmentIndex;

		private bool _isInitialized;

		private ImageIdentifierVM _icon;

		private string _name;

		private string _typeAsString;

		private bool _isValid;

		private bool _isSelected;

		private bool _hasAnyAlternativeUsage;

		private MBBindingList<ShallowItemPropertyVM> _propertyList;

		private SelectorVM<AlternativeUsageItemOptionVM> _alternativeUsageSelector;

		public enum ItemGroup
		{
			None,
			Spear,
			Javelin,
			Bow,
			Crossbow,
			Sword,
			Axe,
			Mace,
			ThrowingAxe,
			ThrowingKnife,
			Ammo,
			Shield,
			Mount,
			Stone
		}
	}
}
