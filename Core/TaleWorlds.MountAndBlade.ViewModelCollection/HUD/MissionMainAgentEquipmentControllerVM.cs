using System;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.HUD
{
	// Token: 0x020000E1 RID: 225
	public class MissionMainAgentEquipmentControllerVM : ViewModel
	{
		// Token: 0x06001488 RID: 5256 RVA: 0x00043044 File Offset: 0x00041244
		public MissionMainAgentEquipmentControllerVM(Action<EquipmentIndex> onDropEquipment, Action<SpawnedItemEntity, EquipmentIndex> onEquipItem)
		{
			this._onDropEquipment = onDropEquipment;
			this._onEquipItem = onEquipItem;
			this.DropActions = new MBBindingList<EquipmentActionItemVM>();
			this.EquipActions = new MBBindingList<EquipmentActionItemVM>();
			this.RefreshValues();
		}

		// Token: 0x06001489 RID: 5257 RVA: 0x00043092 File Offset: 0x00041292
		public override void RefreshValues()
		{
			base.RefreshValues();
			this._dropLocalizedText = GameTexts.FindText("str_inventory_drop", null);
			this._replaceWithLocalizedText = GameTexts.FindText("str_replace_with", null);
		}

		// Token: 0x0600148A RID: 5258 RVA: 0x000430BC File Offset: 0x000412BC
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

		// Token: 0x0600148B RID: 5259 RVA: 0x000431D4 File Offset: 0x000413D4
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

		// Token: 0x0600148C RID: 5260 RVA: 0x00043218 File Offset: 0x00041418
		public void SetCurrentFocusedWeaponEntity(SpawnedItemEntity weaponEntity)
		{
			this._focusedWeaponEntity = weaponEntity;
		}

		// Token: 0x0600148D RID: 5261 RVA: 0x00043224 File Offset: 0x00041424
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

		// Token: 0x0600148E RID: 5262 RVA: 0x00043576 File Offset: 0x00041776
		public void OnCancelEquipController()
		{
			this.IsEquipControllerActive = false;
			this.EquipActions.Clear();
		}

		// Token: 0x0600148F RID: 5263 RVA: 0x0004358A File Offset: 0x0004178A
		public void OnCancelDropController()
		{
			this.IsDropControllerActive = false;
			this.DropActions.Clear();
		}

		// Token: 0x06001490 RID: 5264 RVA: 0x000435A0 File Offset: 0x000417A0
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

		// Token: 0x06001491 RID: 5265 RVA: 0x00043610 File Offset: 0x00041810
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

		// Token: 0x06001492 RID: 5266 RVA: 0x0004368C File Offset: 0x0004188C
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

		// Token: 0x170006C2 RID: 1730
		// (get) Token: 0x06001493 RID: 5267 RVA: 0x0004376E File Offset: 0x0004196E
		// (set) Token: 0x06001494 RID: 5268 RVA: 0x00043776 File Offset: 0x00041976
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

		// Token: 0x170006C3 RID: 1731
		// (get) Token: 0x06001495 RID: 5269 RVA: 0x00043794 File Offset: 0x00041994
		// (set) Token: 0x06001496 RID: 5270 RVA: 0x0004379C File Offset: 0x0004199C
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

		// Token: 0x170006C4 RID: 1732
		// (get) Token: 0x06001497 RID: 5271 RVA: 0x000437BA File Offset: 0x000419BA
		// (set) Token: 0x06001498 RID: 5272 RVA: 0x000437C2 File Offset: 0x000419C2
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

		// Token: 0x170006C5 RID: 1733
		// (get) Token: 0x06001499 RID: 5273 RVA: 0x000437E5 File Offset: 0x000419E5
		// (set) Token: 0x0600149A RID: 5274 RVA: 0x000437ED File Offset: 0x000419ED
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

		// Token: 0x170006C6 RID: 1734
		// (get) Token: 0x0600149B RID: 5275 RVA: 0x00043810 File Offset: 0x00041A10
		// (set) Token: 0x0600149C RID: 5276 RVA: 0x00043818 File Offset: 0x00041A18
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

		// Token: 0x170006C7 RID: 1735
		// (get) Token: 0x0600149D RID: 5277 RVA: 0x0004383B File Offset: 0x00041A3B
		// (set) Token: 0x0600149E RID: 5278 RVA: 0x00043843 File Offset: 0x00041A43
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

		// Token: 0x170006C8 RID: 1736
		// (get) Token: 0x0600149F RID: 5279 RVA: 0x00043866 File Offset: 0x00041A66
		// (set) Token: 0x060014A0 RID: 5280 RVA: 0x0004386E File Offset: 0x00041A6E
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

		// Token: 0x170006C9 RID: 1737
		// (get) Token: 0x060014A1 RID: 5281 RVA: 0x0004388C File Offset: 0x00041A8C
		// (set) Token: 0x060014A2 RID: 5282 RVA: 0x00043894 File Offset: 0x00041A94
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

		// Token: 0x060014A3 RID: 5283 RVA: 0x000438B4 File Offset: 0x00041AB4
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

		// Token: 0x060014A4 RID: 5284 RVA: 0x000439C8 File Offset: 0x00041BC8
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

		// Token: 0x060014A5 RID: 5285 RVA: 0x00043A33 File Offset: 0x00041C33
		private bool IsWieldedWeaponAtIndex(EquipmentIndex index)
		{
			return index == Agent.Main.GetWieldedItemIndex(Agent.HandIndex.MainHand) || index == Agent.Main.GetWieldedItemIndex(Agent.HandIndex.OffHand);
		}

		// Token: 0x040009D2 RID: 2514
		private TextObject _replaceWithLocalizedText;

		// Token: 0x040009D3 RID: 2515
		private TextObject _dropLocalizedText;

		// Token: 0x040009D4 RID: 2516
		private SpawnedItemEntity _focusedWeaponEntity;

		// Token: 0x040009D5 RID: 2517
		private readonly Action<EquipmentIndex> _onDropEquipment;

		// Token: 0x040009D6 RID: 2518
		private readonly Action<SpawnedItemEntity, EquipmentIndex> _onEquipItem;

		// Token: 0x040009D7 RID: 2519
		private readonly TextObject _pickText = new TextObject("{=d5SNB0HV}Pick {ITEM_NAME}", null);

		// Token: 0x040009D8 RID: 2520
		private bool _isDropControllerActive;

		// Token: 0x040009D9 RID: 2521
		private bool _isEquipControllerActive;

		// Token: 0x040009DA RID: 2522
		private string _selectedItemText;

		// Token: 0x040009DB RID: 2523
		private string _dropText;

		// Token: 0x040009DC RID: 2524
		private string _equipText;

		// Token: 0x040009DD RID: 2525
		private string _focusedItemText;

		// Token: 0x040009DE RID: 2526
		private MBBindingList<EquipmentActionItemVM> _dropActions;

		// Token: 0x040009DF RID: 2527
		private MBBindingList<EquipmentActionItemVM> _equipActions;

		// Token: 0x02000231 RID: 561
		public enum ItemGroup
		{
			// Token: 0x04000EC3 RID: 3779
			None,
			// Token: 0x04000EC4 RID: 3780
			Spear,
			// Token: 0x04000EC5 RID: 3781
			Javelin,
			// Token: 0x04000EC6 RID: 3782
			Bow,
			// Token: 0x04000EC7 RID: 3783
			Crossbow,
			// Token: 0x04000EC8 RID: 3784
			Sword,
			// Token: 0x04000EC9 RID: 3785
			Axe,
			// Token: 0x04000ECA RID: 3786
			Mace,
			// Token: 0x04000ECB RID: 3787
			ThrowingAxe,
			// Token: 0x04000ECC RID: 3788
			ThrowingKnife,
			// Token: 0x04000ECD RID: 3789
			Ammo,
			// Token: 0x04000ECE RID: 3790
			Shield,
			// Token: 0x04000ECF RID: 3791
			Mount,
			// Token: 0x04000ED0 RID: 3792
			Banner,
			// Token: 0x04000ED1 RID: 3793
			Stone
		}
	}
}
