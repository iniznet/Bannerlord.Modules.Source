using System;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.HUD
{
	public class MissionMainAgentEquipmentControllerVM : ViewModel
	{
		public MissionMainAgentEquipmentControllerVM(Action<EquipmentIndex> onDropEquipment, Action<SpawnedItemEntity, EquipmentIndex> onEquipItem)
		{
			this._onDropEquipment = onDropEquipment;
			this._onEquipItem = onEquipItem;
			this.DropActions = new MBBindingList<EquipmentActionItemVM>();
			this.EquipActions = new MBBindingList<EquipmentActionItemVM>();
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this._dropLocalizedText = GameTexts.FindText("str_inventory_drop", null);
			this._replaceWithLocalizedText = GameTexts.FindText("str_replace_with", null);
		}

		public void OnDropControllerToggle(bool isActive)
		{
			this.SelectedItemText = "";
			if (isActive && Agent.Main != null)
			{
				this.DropActions.Clear();
				this.DropActions.Add(new EquipmentActionItemVM(GameTexts.FindText("str_cancel", null).ToString(), "None", null, new Action<EquipmentActionItemVM>(this.OnItemSelected), false));
				for (EquipmentIndex equipmentIndex = EquipmentIndex.WeaponItemBeginSlot; equipmentIndex < EquipmentIndex.NumAllWeaponSlots; equipmentIndex++)
				{
					MissionWeapon missionWeapon = Agent.Main.Equipment[equipmentIndex];
					if (!missionWeapon.IsEmpty)
					{
						string itemTypeAsString = MissionMainAgentEquipmentControllerVM.GetItemTypeAsString(missionWeapon.Item);
						bool flag = this.IsWieldedWeaponAtIndex(equipmentIndex);
						string weaponName = this.GetWeaponName(missionWeapon);
						this.DropActions.Add(new EquipmentActionItemVM(weaponName, itemTypeAsString, equipmentIndex, new Action<EquipmentActionItemVM>(this.OnItemSelected), flag));
					}
				}
			}
			else
			{
				EquipmentActionItemVM equipmentActionItemVM = this.DropActions.SingleOrDefault((EquipmentActionItemVM a) => a.IsSelected);
				if (equipmentActionItemVM != null)
				{
					this.HandleDropItemActionSelection(equipmentActionItemVM.Identifier);
				}
			}
			this.IsDropControllerActive = isActive;
		}

		private void HandleDropItemActionSelection(object selectedItem)
		{
			if (selectedItem is EquipmentIndex)
			{
				EquipmentIndex equipmentIndex = (EquipmentIndex)selectedItem;
				this._onDropEquipment(equipmentIndex);
				return;
			}
			if (selectedItem != null)
			{
				Debug.FailedAssert("Unidentified action on drop wheel", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.ViewModelCollection\\HUD\\MissionMainAgentEquipmentControllerVM.cs", "HandleDropItemActionSelection", 106);
			}
		}

		public void SetCurrentFocusedWeaponEntity(SpawnedItemEntity weaponEntity)
		{
			this._focusedWeaponEntity = weaponEntity;
		}

		public void OnEquipControllerToggle(bool isActive)
		{
			this.SelectedItemText = "";
			this.FocusedItemText = "";
			if (isActive && Agent.Main != null)
			{
				this.EquipActions.Clear();
				this.EquipActions.Add(new EquipmentActionItemVM(GameTexts.FindText("str_cancel", null).ToString(), "None", null, new Action<EquipmentActionItemVM>(this.OnItemSelected), false));
				if (this._focusedWeaponEntity.WeaponCopy.Item.Type == ItemObject.ItemTypeEnum.Shield && this.DoesPlayerHaveAtLeastOneShield())
				{
					this._pickText.SetTextVariable("ITEM_NAME", this._focusedWeaponEntity.WeaponCopy.Item.Name.ToString());
					this.FocusedItemText = this._pickText.ToString();
					for (EquipmentIndex equipmentIndex = EquipmentIndex.WeaponItemBeginSlot; equipmentIndex < EquipmentIndex.ExtraWeaponSlot; equipmentIndex++)
					{
						MissionWeapon missionWeapon = Agent.Main.Equipment[equipmentIndex];
						if (!missionWeapon.IsEmpty && missionWeapon.Item.Type == ItemObject.ItemTypeEnum.Shield)
						{
							string itemTypeAsString = MissionMainAgentEquipmentControllerVM.GetItemTypeAsString(missionWeapon.Item);
							bool flag = this.IsWieldedWeaponAtIndex(equipmentIndex);
							string weaponName = this.GetWeaponName(missionWeapon);
							this.EquipActions.Add(new EquipmentActionItemVM(weaponName, itemTypeAsString, equipmentIndex, new Action<EquipmentActionItemVM>(this.OnItemSelected), flag));
						}
					}
				}
				else
				{
					Agent main = Agent.Main;
					if (main != null && main.CanInteractableWeaponBePickedUp(this._focusedWeaponEntity))
					{
						this._pickText.SetTextVariable("ITEM_NAME", this._focusedWeaponEntity.WeaponCopy.Item.Name.ToString());
						this.FocusedItemText = this._pickText.ToString();
						bool flag2 = Agent.Main.WillDropWieldedShield(this._focusedWeaponEntity);
						for (EquipmentIndex equipmentIndex2 = EquipmentIndex.WeaponItemBeginSlot; equipmentIndex2 < EquipmentIndex.ExtraWeaponSlot; equipmentIndex2++)
						{
							MissionWeapon missionWeapon2 = Mission.Current.MainAgent.Equipment[equipmentIndex2];
							if (!missionWeapon2.IsEmpty && (!flag2 || missionWeapon2.IsShield()))
							{
								string itemTypeAsString2 = MissionMainAgentEquipmentControllerVM.GetItemTypeAsString(missionWeapon2.Item);
								bool flag3 = this.IsWieldedWeaponAtIndex(equipmentIndex2);
								string weaponName2 = this.GetWeaponName(missionWeapon2);
								this.EquipActions.Add(new EquipmentActionItemVM(weaponName2, itemTypeAsString2, equipmentIndex2, new Action<EquipmentActionItemVM>(this.OnItemSelected), flag3));
							}
						}
					}
					else
					{
						this.FocusedItemText = this._focusedWeaponEntity.WeaponCopy.Item.Name.ToString();
						EquipmentActionItemVM equipmentActionItemVM = new EquipmentActionItemVM(GameTexts.FindText("str_pickup_to_equip", null).ToString(), "PickUp", this._focusedWeaponEntity, new Action<EquipmentActionItemVM>(this.OnItemSelected), false)
						{
							IsSelected = true
						};
						this.EquipActions.Add(equipmentActionItemVM);
					}
				}
				EquipmentIndex itemIndexThatQuickPickUpWouldReplace = MissionEquipment.SelectWeaponPickUpSlot(Agent.Main, this._focusedWeaponEntity.WeaponCopy, this._focusedWeaponEntity.IsStuckMissile());
				EquipmentActionItemVM equipmentActionItemVM2 = this.EquipActions.SingleOrDefault(delegate(EquipmentActionItemVM a)
				{
					object identifier;
					if ((identifier = a.Identifier) is EquipmentIndex)
					{
						EquipmentIndex equipmentIndex3 = (EquipmentIndex)identifier;
						return equipmentIndex3 == itemIndexThatQuickPickUpWouldReplace;
					}
					return false;
				});
				if (equipmentActionItemVM2 != null)
				{
					equipmentActionItemVM2.IsSelected = true;
				}
			}
			else
			{
				EquipmentActionItemVM equipmentActionItemVM3 = this.EquipActions.SingleOrDefault((EquipmentActionItemVM a) => a.IsSelected);
				if (equipmentActionItemVM3 != null)
				{
					this.HandleEquipItemActionSelection(equipmentActionItemVM3.Identifier);
				}
			}
			this.IsEquipControllerActive = isActive;
		}

		public void OnCancelEquipController()
		{
			this.IsEquipControllerActive = false;
			this.EquipActions.Clear();
		}

		public void OnCancelDropController()
		{
			this.IsDropControllerActive = false;
			this.DropActions.Clear();
		}

		private void HandleEquipItemActionSelection(object selectedItem)
		{
			if (selectedItem is EquipmentIndex)
			{
				EquipmentIndex equipmentIndex = (EquipmentIndex)selectedItem;
				if (this._focusedWeaponEntity != null)
				{
					this._onEquipItem(this._focusedWeaponEntity, equipmentIndex);
					return;
				}
			}
			SpawnedItemEntity spawnedItemEntity;
			if ((spawnedItemEntity = selectedItem as SpawnedItemEntity) != null)
			{
				this._onEquipItem(spawnedItemEntity, EquipmentIndex.None);
				return;
			}
			if (selectedItem != null)
			{
				Debug.FailedAssert("Unidentified action on drop wheel", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.ViewModelCollection\\HUD\\MissionMainAgentEquipmentControllerVM.cs", "HandleEquipItemActionSelection", 223);
			}
		}

		private void OnItemSelected(EquipmentActionItemVM item)
		{
			if (this.IsEquipControllerActive)
			{
				if (item.Identifier == null || item.Identifier is SpawnedItemEntity)
				{
					this.EquipText = "";
				}
				else
				{
					this.EquipText = this._replaceWithLocalizedText.ToString();
				}
			}
			else if (item.Identifier == null)
			{
				this.DropText = "";
			}
			else
			{
				this.DropText = this._dropLocalizedText.ToString();
			}
			this.SelectedItemText = item.ActionText;
		}

		private string GetWeaponName(MissionWeapon weapon)
		{
			string text = weapon.Item.Name.ToString();
			WeaponComponentData currentUsageItem = weapon.CurrentUsageItem;
			if (currentUsageItem != null && currentUsageItem.IsShield)
			{
				text = string.Concat(new object[] { text, " (", weapon.HitPoints, " / ", weapon.ModifiedMaxHitPoints, ")" });
			}
			else
			{
				WeaponComponentData currentUsageItem2 = weapon.CurrentUsageItem;
				if (currentUsageItem2 != null && currentUsageItem2.IsConsumable && weapon.ModifiedMaxAmount > 1)
				{
					text = string.Concat(new object[] { text, " (", weapon.Amount, " / ", weapon.ModifiedMaxAmount, ")" });
				}
			}
			return text;
		}

		[DataSourceProperty]
		public bool IsDropControllerActive
		{
			get
			{
				return this._isDropControllerActive;
			}
			set
			{
				if (value != this._isDropControllerActive)
				{
					this._isDropControllerActive = value;
					base.OnPropertyChangedWithValue(value, "IsDropControllerActive");
				}
			}
		}

		[DataSourceProperty]
		public bool IsEquipControllerActive
		{
			get
			{
				return this._isEquipControllerActive;
			}
			set
			{
				if (value != this._isEquipControllerActive)
				{
					this._isEquipControllerActive = value;
					base.OnPropertyChangedWithValue(value, "IsEquipControllerActive");
				}
			}
		}

		[DataSourceProperty]
		public string DropText
		{
			get
			{
				return this._dropText;
			}
			set
			{
				if (value != this._dropText)
				{
					this._dropText = value;
					base.OnPropertyChangedWithValue<string>(value, "DropText");
				}
			}
		}

		[DataSourceProperty]
		public string EquipText
		{
			get
			{
				return this._equipText;
			}
			set
			{
				if (value != this._equipText)
				{
					this._equipText = value;
					base.OnPropertyChangedWithValue<string>(value, "EquipText");
				}
			}
		}

		[DataSourceProperty]
		public string FocusedItemText
		{
			get
			{
				return this._focusedItemText;
			}
			set
			{
				if (value != this._focusedItemText)
				{
					this._focusedItemText = value;
					base.OnPropertyChangedWithValue<string>(value, "FocusedItemText");
				}
			}
		}

		[DataSourceProperty]
		public string SelectedItemText
		{
			get
			{
				return this._selectedItemText;
			}
			set
			{
				if (value != this._selectedItemText)
				{
					this._selectedItemText = value;
					base.OnPropertyChangedWithValue<string>(value, "SelectedItemText");
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<EquipmentActionItemVM> DropActions
		{
			get
			{
				return this._dropActions;
			}
			set
			{
				if (value != this._dropActions)
				{
					this._dropActions = value;
					base.OnPropertyChangedWithValue<MBBindingList<EquipmentActionItemVM>>(value, "DropActions");
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<EquipmentActionItemVM> EquipActions
		{
			get
			{
				return this._equipActions;
			}
			set
			{
				if (value != this._equipActions)
				{
					this._equipActions = value;
					base.OnPropertyChangedWithValue<MBBindingList<EquipmentActionItemVM>>(value, "EquipActions");
				}
			}
		}

		public static string GetItemTypeAsString(ItemObject item)
		{
			if (item.ItemComponent is WeaponComponent)
			{
				switch ((item.ItemComponent as WeaponComponent).PrimaryWeapon.WeaponClass)
				{
				case WeaponClass.Dagger:
				case WeaponClass.OneHandedSword:
				case WeaponClass.TwoHandedSword:
					return "Sword";
				case WeaponClass.OneHandedAxe:
				case WeaponClass.TwoHandedAxe:
					return "Axe";
				case WeaponClass.Mace:
				case WeaponClass.TwoHandedMace:
					return "Mace";
				case WeaponClass.OneHandedPolearm:
				case WeaponClass.TwoHandedPolearm:
				case WeaponClass.LowGripPolearm:
					return "Spear";
				case WeaponClass.Arrow:
				case WeaponClass.Bolt:
				case WeaponClass.Cartridge:
				case WeaponClass.Musket:
					return "Ammo";
				case WeaponClass.Bow:
					return "Bow";
				case WeaponClass.Crossbow:
					return "Crossbow";
				case WeaponClass.Stone:
					return "Stone";
				case WeaponClass.ThrowingAxe:
					return "ThrowingAxe";
				case WeaponClass.ThrowingKnife:
					return "ThrowingKnife";
				case WeaponClass.Javelin:
					return "Javelin";
				case WeaponClass.SmallShield:
				case WeaponClass.LargeShield:
					return "Shield";
				case WeaponClass.Banner:
					return "Banner";
				}
				return "None";
			}
			if (item.ItemComponent is HorseComponent)
			{
				return "Mount";
			}
			return "None";
		}

		private bool DoesPlayerHaveAtLeastOneShield()
		{
			EquipmentIndex wieldedItemIndex = Agent.Main.GetWieldedItemIndex(Agent.HandIndex.OffHand);
			for (EquipmentIndex equipmentIndex = EquipmentIndex.WeaponItemBeginSlot; equipmentIndex < EquipmentIndex.NumAllWeaponSlots; equipmentIndex++)
			{
				if (equipmentIndex != wieldedItemIndex && !Agent.Main.Equipment[equipmentIndex].IsEmpty && Mission.Current.MainAgent.Equipment[equipmentIndex].Item.Type == ItemObject.ItemTypeEnum.Shield)
				{
					return true;
				}
			}
			return false;
		}

		private bool IsWieldedWeaponAtIndex(EquipmentIndex index)
		{
			return index == Agent.Main.GetWieldedItemIndex(Agent.HandIndex.MainHand) || index == Agent.Main.GetWieldedItemIndex(Agent.HandIndex.OffHand);
		}

		private TextObject _replaceWithLocalizedText;

		private TextObject _dropLocalizedText;

		private SpawnedItemEntity _focusedWeaponEntity;

		private readonly Action<EquipmentIndex> _onDropEquipment;

		private readonly Action<SpawnedItemEntity, EquipmentIndex> _onEquipItem;

		private readonly TextObject _pickText = new TextObject("{=d5SNB0HV}Pick {ITEM_NAME}", null);

		private bool _isDropControllerActive;

		private bool _isEquipControllerActive;

		private string _selectedItemText;

		private string _dropText;

		private string _equipText;

		private string _focusedItemText;

		private MBBindingList<EquipmentActionItemVM> _dropActions;

		private MBBindingList<EquipmentActionItemVM> _equipActions;

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
			Banner,
			Stone
		}
	}
}
