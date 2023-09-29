using System;
using System.ComponentModel;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.HUD
{
	public class MissionMainAgentControllerEquipDropVM : ViewModel
	{
		public MissionMainAgentControllerEquipDropVM(Action<EquipmentIndex> toggleItem)
		{
			this._toggleItem = toggleItem;
			this.EquippedWeapons = new MBBindingList<ControllerEquippedItemVM>();
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.PressToEquipText = new TextObject("{=HEEZhL90}Press to Equip", null).ToString();
		}

		public void InitializeMainAgentPropterties()
		{
			Mission.Current.OnMainAgentChanged += this.OnMainAgentChanged;
			this.OnMainAgentChanged(null, null);
		}

		private void OnMainAgentChanged(object sender, PropertyChangedEventArgs e)
		{
			if (Agent.Main != null)
			{
				Agent main = Agent.Main;
				main.OnMainAgentWieldedItemChange = (Agent.OnMainAgentWieldedItemChangeDelegate)Delegate.Combine(main.OnMainAgentWieldedItemChange, new Agent.OnMainAgentWieldedItemChangeDelegate(this.OnMainAgentWeaponChange));
			}
		}

		private void OnMainAgentWeaponChange()
		{
			this.UpdateItemsWieldStatus();
		}

		public void OnToggle(bool isEnabled)
		{
			this.EquippedWeapons.ApplyActionOnAllItems(delegate(ControllerEquippedItemVM o)
			{
				o.OnFinalize();
			});
			this.EquippedWeapons.Clear();
			if (isEnabled)
			{
				this.EquippedWeapons.Add(new ControllerEquippedItemVM(GameTexts.FindText("str_cancel", null).ToString(), null, "None", null, new Action<EquipmentActionItemVM>(this.OnItemSelected)));
				int num = 0;
				int totalNumberOfWeaponsOnMainAgent = this.GetTotalNumberOfWeaponsOnMainAgent();
				for (EquipmentIndex equipmentIndex = EquipmentIndex.WeaponItemBeginSlot; equipmentIndex < EquipmentIndex.ExtraWeaponSlot; equipmentIndex++)
				{
					MissionWeapon missionWeapon = Agent.Main.Equipment[equipmentIndex];
					if (!missionWeapon.IsEmpty)
					{
						string itemTypeAsString = MissionMainAgentEquipmentControllerVM.GetItemTypeAsString(missionWeapon.Item);
						string weaponName = this.GetWeaponName(missionWeapon);
						this.EquippedWeapons.Add(new ControllerEquippedItemVM(weaponName, itemTypeAsString, equipmentIndex, MissionMainAgentControllerEquipDropVM.GetWeaponHotKey(num, totalNumberOfWeaponsOnMainAgent), new Action<EquipmentActionItemVM>(this.OnItemSelected)));
						num++;
					}
				}
				this.UpdateItemsWieldStatus();
			}
			else
			{
				if (this._lastSelectedItem != null && this._lastSelectedItem.Identifier is EquipmentIndex)
				{
					Action<EquipmentIndex> toggleItem = this._toggleItem;
					if (toggleItem != null)
					{
						toggleItem((EquipmentIndex)this._lastSelectedItem.Identifier);
					}
				}
				this._lastSelectedItem = null;
			}
			this.IsActive = isEnabled;
		}

		private void OnItemSelected(EquipmentActionItemVM selectedItem)
		{
			if (this._lastSelectedItem != selectedItem)
			{
				this._lastSelectedItem = selectedItem;
			}
		}

		public void OnCancelHoldController()
		{
		}

		public void OnWeaponDroppedAtIndex(int droppedWeaponIndex)
		{
			this.OnToggle(true);
		}

		private bool IsWieldedWeaponAtIndex(EquipmentIndex index)
		{
			return index == Agent.Main.GetWieldedItemIndex(Agent.HandIndex.MainHand) || index == Agent.Main.GetWieldedItemIndex(Agent.HandIndex.OffHand);
		}

		public void OnWeaponEquippedAtIndex(int equippedWeaponIndex)
		{
			this.UpdateItemsWieldStatus();
		}

		public void SetDropProgressForIndex(EquipmentIndex eqIndex, float progress)
		{
			int i = 0;
			while (i < this.EquippedWeapons.Count)
			{
				object identifier;
				if (!((identifier = this.EquippedWeapons[i].Identifier) is EquipmentIndex))
				{
					goto IL_31;
				}
				EquipmentIndex equipmentIndex = (EquipmentIndex)identifier;
				if (equipmentIndex != eqIndex || progress <= 0.2f)
				{
					goto IL_31;
				}
				float num = progress;
				IL_39:
				float num2 = num;
				this.EquippedWeapons[i].DropProgress = num2;
				i++;
				continue;
				IL_31:
				num = 0f;
				goto IL_39;
			}
		}

		private void UpdateItemsWieldStatus()
		{
			for (int i = 0; i < this.EquippedWeapons.Count; i++)
			{
				object identifier;
				if ((identifier = this.EquippedWeapons[i].Identifier) is EquipmentIndex)
				{
					EquipmentIndex equipmentIndex = (EquipmentIndex)identifier;
					this.EquippedWeapons[i].IsWielded = this.IsWieldedWeaponAtIndex(equipmentIndex);
				}
			}
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

		public override void OnFinalize()
		{
			base.OnFinalize();
			this.EquippedWeapons.ApplyActionOnAllItems(delegate(ControllerEquippedItemVM o)
			{
				o.OnFinalize();
			});
			this.EquippedWeapons.Clear();
		}

		private static HotKey GetWeaponHotKey(int currentIndexOfWeapon, int totalNumOfWeapons)
		{
			if (currentIndexOfWeapon == 0)
			{
				if (totalNumOfWeapons == 1)
				{
					return HotKeyManager.GetCategory("CombatHotKeyCategory").GetHotKey("ControllerEquipDropWeapon4");
				}
				if (totalNumOfWeapons > 1)
				{
					return HotKeyManager.GetCategory("CombatHotKeyCategory").GetHotKey("ControllerEquipDropWeapon1");
				}
				Debug.FailedAssert("Wrong number of total weapons!", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.ViewModelCollection\\HUD\\MissionMainAgentControllerEquipDropVM.cs", "GetWeaponHotKey", 182);
			}
			else if (currentIndexOfWeapon == 1)
			{
				if (totalNumOfWeapons == 2)
				{
					return HotKeyManager.GetCategory("CombatHotKeyCategory").GetHotKey("ControllerEquipDropWeapon3");
				}
				if (totalNumOfWeapons > 2)
				{
					return HotKeyManager.GetCategory("CombatHotKeyCategory").GetHotKey("ControllerEquipDropWeapon4");
				}
			}
			else
			{
				if (currentIndexOfWeapon == 2)
				{
					return HotKeyManager.GetCategory("CombatHotKeyCategory").GetHotKey("ControllerEquipDropWeapon3");
				}
				if (currentIndexOfWeapon == 3)
				{
					return HotKeyManager.GetCategory("CombatHotKeyCategory").GetHotKey("ControllerEquipDropWeapon2");
				}
				Debug.FailedAssert("Wrong index of current weapon. Cannot be higher than 3", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.ViewModelCollection\\HUD\\MissionMainAgentControllerEquipDropVM.cs", "GetWeaponHotKey", 206);
			}
			return null;
		}

		public void OnGamepadActiveChanged(bool isActive)
		{
			this.HoldToDropText = (isActive ? this._dropTextObject.ToString() : string.Empty);
		}

		private int GetTotalNumberOfWeaponsOnMainAgent()
		{
			int num = 0;
			for (EquipmentIndex equipmentIndex = EquipmentIndex.WeaponItemBeginSlot; equipmentIndex < EquipmentIndex.ExtraWeaponSlot; equipmentIndex++)
			{
				if (!Agent.Main.Equipment[equipmentIndex].IsEmpty)
				{
					num++;
				}
			}
			return num;
		}

		[DataSourceProperty]
		public MBBindingList<ControllerEquippedItemVM> EquippedWeapons
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
					base.OnPropertyChangedWithValue<MBBindingList<ControllerEquippedItemVM>>(value, "EquippedWeapons");
				}
			}
		}

		[DataSourceProperty]
		public string HoldToDropText
		{
			get
			{
				return this._holdToDropText;
			}
			set
			{
				if (value != this._holdToDropText)
				{
					this._holdToDropText = value;
					base.OnPropertyChangedWithValue<string>(value, "HoldToDropText");
				}
			}
		}

		[DataSourceProperty]
		public string PressToEquipText
		{
			get
			{
				return this._pressToEquipText;
			}
			set
			{
				if (value != this._pressToEquipText)
				{
					this._pressToEquipText = value;
					base.OnPropertyChangedWithValue<string>(value, "PressToEquipText");
				}
			}
		}

		[DataSourceProperty]
		public bool IsActive
		{
			get
			{
				return this._isActive;
			}
			set
			{
				if (value != this._isActive)
				{
					this._isActive = value;
					base.OnPropertyChangedWithValue(value, "IsActive");
				}
			}
		}

		private EquipmentActionItemVM _lastSelectedItem;

		private Action<EquipmentIndex> _toggleItem;

		private TextObject _dropTextObject = new TextObject("{=d1tCz15N}Hold to Drop", null);

		private MBBindingList<ControllerEquippedItemVM> _equipActions;

		private bool _isActive;

		private string _holdToDropText;

		private string _pressToEquipText;
	}
}
