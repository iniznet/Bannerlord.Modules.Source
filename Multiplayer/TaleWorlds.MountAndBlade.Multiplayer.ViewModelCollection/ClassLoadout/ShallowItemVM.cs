using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Selector;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection.ClassLoadout
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
			if (itemObject == null || (equipmentIndex == 10 && !itemObject.HasHorseComponent) || (equipmentIndex != 10 && (itemObject.PrimaryWeapon == null || itemObject.PrimaryWeapon.IsAmmo)))
			{
				this.IsValid = false;
				this.Icon = new ImageIdentifierVM(0);
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
				if (itemTypeFromWeaponClass == 2 || itemTypeFromWeaponClass == 3 || itemTypeFromWeaponClass == 4)
				{
					if (weaponComponentData.SwingDamageType != -1)
					{
						this.AddProperty(new TextObject("{=yJsE4Ayo}Swing Spd.", null), (float)WeaponComponentDataExtensions.GetModifiedSwingSpeed(weaponComponentData, itemModifier) / 145f, WeaponComponentDataExtensions.GetModifiedSwingSpeed(weaponComponentData, itemModifier));
						this.AddProperty(new TextObject("{=RNgWFLIO}Swing Dmg.", null), (float)WeaponComponentDataExtensions.GetModifiedSwingDamage(weaponComponentData, itemModifier) / 143f, WeaponComponentDataExtensions.GetModifiedSwingDamage(weaponComponentData, itemModifier));
					}
					if (weaponComponentData.ThrustDamageType != -1)
					{
						this.AddProperty(new TextObject("{=J0vjDOFO}Thrust Spd.", null), (float)WeaponComponentDataExtensions.GetModifiedThrustSpeed(weaponComponentData, itemModifier) / 114f, WeaponComponentDataExtensions.GetModifiedThrustSpeed(weaponComponentData, itemModifier));
						this.AddProperty(new TextObject("{=Ie9I2Bha}Thrust Dmg.", null), (float)WeaponComponentDataExtensions.GetModifiedThrustDamage(weaponComponentData, itemModifier) / 86f, WeaponComponentDataExtensions.GetModifiedThrustDamage(weaponComponentData, itemModifier));
					}
					this.AddProperty(new TextObject("{=ftoSCQ0x}Length", null), (float)weaponComponentData.WeaponLength / 315f, weaponComponentData.WeaponLength);
					this.AddProperty(new TextObject("{=oibdTnXP}Handling", null), (float)WeaponComponentDataExtensions.GetModifiedHandling(weaponComponentData, itemModifier) / 120f, WeaponComponentDataExtensions.GetModifiedHandling(weaponComponentData, itemModifier));
				}
				if (itemTypeFromWeaponClass == 10)
				{
					this.AddProperty(new TextObject("{=ftoSCQ0x}Length", null), (float)weaponComponentData.WeaponLength / 147f, weaponComponentData.WeaponLength);
					this.AddProperty(new TextObject("{=s31DnnAf}Damage", null), (float)WeaponComponentDataExtensions.GetModifiedThrustDamage(weaponComponentData, itemModifier) / 94f, WeaponComponentDataExtensions.GetModifiedThrustDamage(weaponComponentData, itemModifier));
					this.AddProperty(new TextObject("{=QfTt7YRB}Fire Rate", null), (float)WeaponComponentDataExtensions.GetModifiedMissileSpeed(weaponComponentData, itemModifier) / 115f, WeaponComponentDataExtensions.GetModifiedMissileSpeed(weaponComponentData, itemModifier));
					this.AddProperty(new TextObject("{=TAnabTdy}Accuracy", null), (float)weaponComponentData.Accuracy / 300f, weaponComponentData.Accuracy);
					this.AddProperty(new TextObject("{=b31ITmm0}Stack Amnt.", null), (float)WeaponComponentDataExtensions.GetModifiedStackCount(weaponComponentData, itemModifier) / 40f, (int)WeaponComponentDataExtensions.GetModifiedStackCount(weaponComponentData, itemModifier));
				}
				if (itemTypeFromWeaponClass == 7)
				{
					this.AddProperty(new TextObject("{=6GSXsdeX}Speed", null), (float)WeaponComponentDataExtensions.GetModifiedThrustSpeed(weaponComponentData, itemModifier) / 120f, WeaponComponentDataExtensions.GetModifiedThrustSpeed(weaponComponentData, itemModifier));
					this.AddProperty(new TextObject("{=GGseMDd3}Durability", null), (float)WeaponComponentDataExtensions.GetModifiedMaximumHitPoints(weaponComponentData, itemModifier) / 500f, (int)WeaponComponentDataExtensions.GetModifiedMaximumHitPoints(weaponComponentData, itemModifier));
					this.AddProperty(new TextObject("{=ahiBhAqU}Armor", null), (float)WeaponComponentDataExtensions.GetModifiedArmor(weaponComponentData, itemModifier) / 40f, WeaponComponentDataExtensions.GetModifiedArmor(weaponComponentData, itemModifier));
					this.AddProperty(new TextObject("{=4Dd2xgPm}Weight", null), item.Weight / 40f, (int)item.Weight);
				}
				if (itemTypeFromWeaponClass == 8 || itemTypeFromWeaponClass == 9)
				{
					int num = 0;
					float num2 = 0f;
					int num3 = 0;
					for (EquipmentIndex equipmentIndex2 = 0; equipmentIndex2 < 4; equipmentIndex2++)
					{
						ItemObject item2 = equipment[equipmentIndex2].Item;
						ItemModifier itemModifier2 = equipment[equipmentIndex2].ItemModifier;
						if (item2 != null && item2.PrimaryWeapon.IsAmmo)
						{
							num += (int)WeaponComponentDataExtensions.GetModifiedStackCount(item2.PrimaryWeapon, itemModifier2);
							num3 += WeaponComponentDataExtensions.GetModifiedThrustDamage(item2.PrimaryWeapon, itemModifier2);
							num2 += 1f;
						}
					}
					num3 = MathF.Round((float)num3 / num2);
					this.AddProperty(new TextObject("{=ftoSCQ0x}Length", null), (float)weaponComponentData.WeaponLength / 123f, weaponComponentData.WeaponLength);
					this.AddProperty(new TextObject("{=s31DnnAf}Damage", null), (float)(WeaponComponentDataExtensions.GetModifiedThrustDamage(weaponComponentData, itemModifier) + num3) / 70f, WeaponComponentDataExtensions.GetModifiedThrustDamage(weaponComponentData, itemModifier) + num3);
					this.AddProperty(new TextObject("{=QfTt7YRB}Fire Rate", null), (float)WeaponComponentDataExtensions.GetModifiedSwingSpeed(weaponComponentData, itemModifier) / 120f, WeaponComponentDataExtensions.GetModifiedSwingSpeed(weaponComponentData, itemModifier));
					this.AddProperty(new TextObject("{=TAnabTdy}Accuracy", null), (float)weaponComponentData.Accuracy / 105f, weaponComponentData.Accuracy);
					this.AddProperty(new TextObject("{=yUpH2mQ4}Ammo", null), (float)num / 90f, num);
				}
			}
			if (item.HorseComponent != null)
			{
				EquipmentElement equipmentElement = equipment[10];
				EquipmentElement equipmentElement2 = equipment[11];
				int modifiedMountCharge = equipmentElement.GetModifiedMountCharge(ref equipmentElement2);
				int num4 = (int)(4.33f * (float)equipmentElement.GetModifiedMountSpeed(ref equipmentElement2));
				int modifiedMountManeuver = equipmentElement.GetModifiedMountManeuver(ref equipmentElement2);
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
				case 2:
				case 3:
					return ShallowItemVM.ItemGroup.Sword;
				case 4:
				case 5:
					return ShallowItemVM.ItemGroup.Axe;
				case 6:
				case 8:
					return ShallowItemVM.ItemGroup.Mace;
				case 9:
				case 10:
				case 11:
					return ShallowItemVM.ItemGroup.Spear;
				case 12:
				case 13:
				case 14:
				case 23:
					return ShallowItemVM.ItemGroup.Ammo;
				case 15:
					return ShallowItemVM.ItemGroup.Bow;
				case 16:
					return ShallowItemVM.ItemGroup.Crossbow;
				case 17:
					return ShallowItemVM.ItemGroup.Stone;
				case 19:
					return ShallowItemVM.ItemGroup.ThrowingAxe;
				case 20:
					return ShallowItemVM.ItemGroup.ThrowingKnife;
				case 21:
					return ShallowItemVM.ItemGroup.Javelin;
				case 24:
				case 25:
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
