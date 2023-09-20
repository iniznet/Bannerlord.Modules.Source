using System;
using System.ComponentModel;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.HUD
{
	// Token: 0x020000DF RID: 223
	public class MissionMainAgentControllerEquipDropVM : ViewModel
	{
		// Token: 0x06001468 RID: 5224 RVA: 0x000429FE File Offset: 0x00040BFE
		public MissionMainAgentControllerEquipDropVM(Action<EquipmentIndex> toggleItem)
		{
			this._toggleItem = toggleItem;
			this.EquippedWeapons = new MBBindingList<ControllerEquippedItemVM>();
			this.RefreshValues();
		}

		// Token: 0x06001469 RID: 5225 RVA: 0x00042A2F File Offset: 0x00040C2F
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.PressToEquipText = new TextObject("{=HEEZhL90}Press to Equip", null).ToString();
		}

		// Token: 0x0600146A RID: 5226 RVA: 0x00042A4D File Offset: 0x00040C4D
		public void InitializeMainAgentPropterties()
		{
			Mission.Current.OnMainAgentChanged += this.OnMainAgentChanged;
			this.OnMainAgentChanged(null, null);
		}

		// Token: 0x0600146B RID: 5227 RVA: 0x00042A6D File Offset: 0x00040C6D
		private void OnMainAgentChanged(object sender, PropertyChangedEventArgs e)
		{
			if (Agent.Main != null)
			{
				Agent main = Agent.Main;
				main.OnMainAgentWieldedItemChange = (Agent.OnMainAgentWieldedItemChangeDelegate)Delegate.Combine(main.OnMainAgentWieldedItemChange, new Agent.OnMainAgentWieldedItemChangeDelegate(this.OnMainAgentWeaponChange));
			}
		}

		// Token: 0x0600146C RID: 5228 RVA: 0x00042A9C File Offset: 0x00040C9C
		private void OnMainAgentWeaponChange()
		{
			this.UpdateItemsWieldStatus();
		}

		// Token: 0x0600146D RID: 5229 RVA: 0x00042AA4 File Offset: 0x00040CA4
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

		// Token: 0x0600146E RID: 5230 RVA: 0x00042BE7 File Offset: 0x00040DE7
		private void OnItemSelected(EquipmentActionItemVM selectedItem)
		{
			if (this._lastSelectedItem != selectedItem)
			{
				this._lastSelectedItem = selectedItem;
			}
		}

		// Token: 0x0600146F RID: 5231 RVA: 0x00042BF9 File Offset: 0x00040DF9
		public void OnCancelHoldController()
		{
		}

		// Token: 0x06001470 RID: 5232 RVA: 0x00042BFB File Offset: 0x00040DFB
		public void OnWeaponDroppedAtIndex(int droppedWeaponIndex)
		{
			this.OnToggle(true);
		}

		// Token: 0x06001471 RID: 5233 RVA: 0x00042C04 File Offset: 0x00040E04
		private bool IsWieldedWeaponAtIndex(EquipmentIndex index)
		{
			return index == Agent.Main.GetWieldedItemIndex(Agent.HandIndex.MainHand) || index == Agent.Main.GetWieldedItemIndex(Agent.HandIndex.OffHand);
		}

		// Token: 0x06001472 RID: 5234 RVA: 0x00042C24 File Offset: 0x00040E24
		public void OnWeaponEquippedAtIndex(int equippedWeaponIndex)
		{
			this.UpdateItemsWieldStatus();
		}

		// Token: 0x06001473 RID: 5235 RVA: 0x00042C2C File Offset: 0x00040E2C
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

		// Token: 0x06001474 RID: 5236 RVA: 0x00042C98 File Offset: 0x00040E98
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

		// Token: 0x06001475 RID: 5237 RVA: 0x00042CF4 File Offset: 0x00040EF4
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

		// Token: 0x06001476 RID: 5238 RVA: 0x00042DD6 File Offset: 0x00040FD6
		public override void OnFinalize()
		{
			base.OnFinalize();
			this.EquippedWeapons.ApplyActionOnAllItems(delegate(ControllerEquippedItemVM o)
			{
				o.OnFinalize();
			});
			this.EquippedWeapons.Clear();
		}

		// Token: 0x06001477 RID: 5239 RVA: 0x00042E14 File Offset: 0x00041014
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
			}
			return null;
		}

		// Token: 0x06001478 RID: 5240 RVA: 0x00042EBF File Offset: 0x000410BF
		public void OnGamepadActiveChanged(bool isActive)
		{
			this.HoldToDropText = (isActive ? this._dropTextObject.ToString() : string.Empty);
		}

		// Token: 0x06001479 RID: 5241 RVA: 0x00042EDC File Offset: 0x000410DC
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

		// Token: 0x170006BC RID: 1724
		// (get) Token: 0x0600147A RID: 5242 RVA: 0x00042F16 File Offset: 0x00041116
		// (set) Token: 0x0600147B RID: 5243 RVA: 0x00042F1E File Offset: 0x0004111E
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

		// Token: 0x170006BD RID: 1725
		// (get) Token: 0x0600147C RID: 5244 RVA: 0x00042F3C File Offset: 0x0004113C
		// (set) Token: 0x0600147D RID: 5245 RVA: 0x00042F44 File Offset: 0x00041144
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

		// Token: 0x170006BE RID: 1726
		// (get) Token: 0x0600147E RID: 5246 RVA: 0x00042F67 File Offset: 0x00041167
		// (set) Token: 0x0600147F RID: 5247 RVA: 0x00042F6F File Offset: 0x0004116F
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

		// Token: 0x170006BF RID: 1727
		// (get) Token: 0x06001480 RID: 5248 RVA: 0x00042F92 File Offset: 0x00041192
		// (set) Token: 0x06001481 RID: 5249 RVA: 0x00042F9A File Offset: 0x0004119A
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

		// Token: 0x040009C9 RID: 2505
		private EquipmentActionItemVM _lastSelectedItem;

		// Token: 0x040009CA RID: 2506
		private Action<EquipmentIndex> _toggleItem;

		// Token: 0x040009CB RID: 2507
		private TextObject _dropTextObject = new TextObject("{=d1tCz15N}Hold to Drop", null);

		// Token: 0x040009CC RID: 2508
		private MBBindingList<ControllerEquippedItemVM> _equipActions;

		// Token: 0x040009CD RID: 2509
		private bool _isActive;

		// Token: 0x040009CE RID: 2510
		private string _holdToDropText;

		// Token: 0x040009CF RID: 2511
		private string _pressToEquipText;
	}
}
