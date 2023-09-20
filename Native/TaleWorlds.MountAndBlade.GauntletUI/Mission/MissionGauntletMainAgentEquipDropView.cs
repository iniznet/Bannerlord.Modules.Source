using System;
using System.ComponentModel;
using NetworkMessages.FromClient;
using TaleWorlds.Core;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.InputSystem;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.MissionViews;
using TaleWorlds.MountAndBlade.View.Screens;
using TaleWorlds.MountAndBlade.ViewModelCollection;
using TaleWorlds.MountAndBlade.ViewModelCollection.HUD;
using TaleWorlds.MountAndBlade.ViewModelCollection.Input;

namespace TaleWorlds.MountAndBlade.GauntletUI.Mission
{
	// Token: 0x0200002A RID: 42
	[OverrideView(typeof(MissionMainAgentEquipDropView))]
	public class MissionGauntletMainAgentEquipDropView : MissionView
	{
		// Token: 0x1700004B RID: 75
		// (get) Token: 0x060001E8 RID: 488 RVA: 0x0000A79D File Offset: 0x0000899D
		private bool IsDisplayingADialog
		{
			get
			{
				IMissionScreen missionScreenAsInterface = this._missionScreenAsInterface;
				return (missionScreenAsInterface != null && missionScreenAsInterface.GetDisplayDialog()) || base.MissionScreen.IsRadialMenuActive || base.Mission.IsOrderMenuOpen;
			}
		}

		// Token: 0x1700004C RID: 76
		// (get) Token: 0x060001E9 RID: 489 RVA: 0x0000A7CD File Offset: 0x000089CD
		// (set) Token: 0x060001EA RID: 490 RVA: 0x0000A7D5 File Offset: 0x000089D5
		private bool HoldHandled
		{
			get
			{
				return this._holdHandled;
			}
			set
			{
				this._holdHandled = value;
				MissionScreen missionScreen = base.MissionScreen;
				if (missionScreen == null)
				{
					return;
				}
				missionScreen.SetRadialMenuActiveState(value);
			}
		}

		// Token: 0x060001EB RID: 491 RVA: 0x0000A7EF File Offset: 0x000089EF
		public MissionGauntletMainAgentEquipDropView()
		{
			this._missionScreenAsInterface = base.MissionScreen;
			this.HoldHandled = false;
		}

		// Token: 0x060001EC RID: 492 RVA: 0x0000A80C File Offset: 0x00008A0C
		public override void EarlyStart()
		{
			base.EarlyStart();
			this._gauntletLayer = new GauntletLayer(3, "GauntletLayer", false);
			this._dataSource = new MissionMainAgentControllerEquipDropVM(new Action<EquipmentIndex>(this.OnToggleItem));
			this._missionMainAgentController = base.Mission.GetMissionBehavior<MissionMainAgentController>();
			this._missionControllerLeaveLogic = base.Mission.GetMissionBehavior<EquipmentControllerLeaveLogic>();
			this._gauntletLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("CombatHotKeyCategory"));
			this._gauntletLayer.InputRestrictions.SetInputRestrictions(false, 0);
			this._gauntletLayer.LoadMovie("MainAgentControllerEquipDrop", this._dataSource);
			base.MissionScreen.AddLayer(this._gauntletLayer);
			base.Mission.OnMainAgentChanged += this.OnMainAgentChanged;
			TaleWorlds.InputSystem.Input.OnGamepadActiveStateChanged = (Action)Delegate.Combine(TaleWorlds.InputSystem.Input.OnGamepadActiveStateChanged, new Action(this.OnGamepadActiveChanged));
		}

		// Token: 0x060001ED RID: 493 RVA: 0x0000A8F5 File Offset: 0x00008AF5
		public override void AfterStart()
		{
			base.AfterStart();
			this._dataSource.InitializeMainAgentPropterties();
		}

		// Token: 0x060001EE RID: 494 RVA: 0x0000A908 File Offset: 0x00008B08
		public override void OnMissionScreenFinalize()
		{
			base.OnMissionScreenFinalize();
			TaleWorlds.InputSystem.Input.OnGamepadActiveStateChanged = (Action)Delegate.Remove(TaleWorlds.InputSystem.Input.OnGamepadActiveStateChanged, new Action(this.OnGamepadActiveChanged));
			base.Mission.OnMainAgentChanged -= this.OnMainAgentChanged;
			base.MissionScreen.RemoveLayer(this._gauntletLayer);
			this._gauntletLayer = null;
			this._dataSource.OnFinalize();
			this._dataSource = null;
			this._missionMainAgentController = null;
			this._missionControllerLeaveLogic = null;
		}

		// Token: 0x060001EF RID: 495 RVA: 0x0000A98A File Offset: 0x00008B8A
		public override void OnMissionScreenTick(float dt)
		{
			base.OnMissionScreenTick(dt);
			if (this.IsMainAgentAvailable() && (!base.MissionScreen.IsRadialMenuActive || this._dataSource.IsActive))
			{
				this.TickControls(dt);
			}
		}

		// Token: 0x060001F0 RID: 496 RVA: 0x0000A9BC File Offset: 0x00008BBC
		private void OnMainAgentChanged(object sender, PropertyChangedEventArgs e)
		{
			if (base.Mission.MainAgent == null)
			{
				if (this.HoldHandled)
				{
					this.HoldHandled = false;
				}
				this._toggleHoldTime = 0f;
				this._dataSource.OnCancelHoldController();
			}
		}

		// Token: 0x060001F1 RID: 497 RVA: 0x0000A9F0 File Offset: 0x00008BF0
		public override void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow blow)
		{
			if (affectedAgent == Agent.Main)
			{
				this.HandleClosingHold();
			}
		}

		// Token: 0x060001F2 RID: 498 RVA: 0x0000AA00 File Offset: 0x00008C00
		private void TickControls(float dt)
		{
			if (base.MissionScreen.SceneLayer.Input.IsGameKeyDown(34) && !this.IsDisplayingADialog && this.IsMainAgentAvailable() && base.Mission.Mode != 6 && base.Mission.Mode != 9 && !base.MissionScreen.IsRadialMenuActive)
			{
				if (this._toggleHoldTime > 0.3f && !this.HoldHandled)
				{
					this.HandleOpeningHold();
					this.HoldHandled = true;
				}
				this._toggleHoldTime += dt;
				this._prevKeyDown = true;
			}
			else if (this._prevKeyDown && !base.MissionScreen.SceneLayer.Input.IsGameKeyDown(34))
			{
				if (this._toggleHoldTime < 0.3f)
				{
					this.HandleQuickRelease();
				}
				else
				{
					this.HandleClosingHold();
				}
				this.HoldHandled = false;
				this._toggleHoldTime = 0f;
				this._weaponDropHoldTime = 0f;
				this._prevKeyDown = false;
				this._weaponDropHandled = false;
			}
			if (this.HoldHandled)
			{
				int keyWeaponIndex = this.GetKeyWeaponIndex(false);
				int keyWeaponIndex2 = this.GetKeyWeaponIndex(true);
				this._dataSource.SetDropProgressForIndex(-1, this._weaponDropHoldTime / 0.5f);
				if (keyWeaponIndex != -1)
				{
					if (!this._weaponDropHandled)
					{
						int num = keyWeaponIndex;
						if (this._weaponDropHoldTime > 0.5f && !Agent.Main.Equipment[num].IsEmpty)
						{
							this.OnDropEquipment(num);
							this._dataSource.OnWeaponDroppedAtIndex(keyWeaponIndex);
							this._weaponDropHandled = true;
						}
						this._dataSource.SetDropProgressForIndex(num, this._weaponDropHoldTime / 0.5f);
					}
					this._weaponDropHoldTime += dt;
					return;
				}
				if (keyWeaponIndex2 != -1)
				{
					if (!this._weaponDropHandled)
					{
						int num2 = keyWeaponIndex2;
						if (!Agent.Main.Equipment[num2].IsEmpty)
						{
							this.OnToggleItem(num2);
							this._dataSource.OnWeaponEquippedAtIndex(keyWeaponIndex2);
							this._weaponDropHandled = true;
						}
					}
					this._weaponDropHoldTime = 0f;
					return;
				}
				this._weaponDropHoldTime = 0f;
				this._weaponDropHandled = false;
			}
		}

		// Token: 0x060001F3 RID: 499 RVA: 0x0000AC0C File Offset: 0x00008E0C
		private void HandleOpeningHold()
		{
			MissionMainAgentControllerEquipDropVM dataSource = this._dataSource;
			if (dataSource != null)
			{
				dataSource.OnToggle(true);
			}
			base.MissionScreen.SetRadialMenuActiveState(true);
			EquipmentControllerLeaveLogic missionControllerLeaveLogic = this._missionControllerLeaveLogic;
			if (missionControllerLeaveLogic != null)
			{
				missionControllerLeaveLogic.SetIsEquipmentSelectionActive(true);
			}
			if (!GameNetwork.IsMultiplayer && !this._isSlowDownApplied)
			{
				base.Mission.AddTimeSpeedRequest(new Mission.TimeSpeedRequest(0.25f, 624));
				this._isSlowDownApplied = true;
			}
		}

		// Token: 0x060001F4 RID: 500 RVA: 0x0000AC7C File Offset: 0x00008E7C
		private void HandleClosingHold()
		{
			MissionMainAgentControllerEquipDropVM dataSource = this._dataSource;
			if (dataSource != null)
			{
				dataSource.OnToggle(false);
			}
			base.MissionScreen.SetRadialMenuActiveState(false);
			EquipmentControllerLeaveLogic missionControllerLeaveLogic = this._missionControllerLeaveLogic;
			if (missionControllerLeaveLogic != null)
			{
				missionControllerLeaveLogic.SetIsEquipmentSelectionActive(false);
			}
			if (!GameNetwork.IsMultiplayer && this._isSlowDownApplied)
			{
				base.Mission.RemoveTimeSpeedRequest(624);
				this._isSlowDownApplied = false;
			}
		}

		// Token: 0x060001F5 RID: 501 RVA: 0x0000ACDF File Offset: 0x00008EDF
		private void HandleQuickRelease()
		{
			this._missionMainAgentController.OnWeaponUsageToggleRequested();
			MissionMainAgentControllerEquipDropVM dataSource = this._dataSource;
			if (dataSource != null)
			{
				dataSource.OnToggle(false);
			}
			base.MissionScreen.SetRadialMenuActiveState(false);
			EquipmentControllerLeaveLogic missionControllerLeaveLogic = this._missionControllerLeaveLogic;
			if (missionControllerLeaveLogic == null)
			{
				return;
			}
			missionControllerLeaveLogic.SetIsEquipmentSelectionActive(false);
		}

		// Token: 0x060001F6 RID: 502 RVA: 0x0000AD1C File Offset: 0x00008F1C
		private void OnToggleItem(EquipmentIndex indexToToggle)
		{
			bool flag = indexToToggle == Agent.Main.GetWieldedItemIndex(0);
			bool flag2 = indexToToggle == Agent.Main.GetWieldedItemIndex(1);
			if (flag || flag2)
			{
				Agent.Main.TryToSheathWeaponInHand(flag ? 0 : 1, 0);
				return;
			}
			Agent.Main.TryToWieldWeaponInSlot(indexToToggle, 0, false);
		}

		// Token: 0x060001F7 RID: 503 RVA: 0x0000AD6C File Offset: 0x00008F6C
		private void OnDropEquipment(EquipmentIndex indexToDrop)
		{
			if (GameNetwork.IsClient)
			{
				GameNetwork.BeginModuleEventAsClient();
				GameNetwork.WriteMessage(new DropWeapon(base.Input.IsGameKeyDown(10), indexToDrop));
				GameNetwork.EndModuleEventAsClient();
				return;
			}
			Agent.Main.HandleDropWeapon(base.Input.IsGameKeyDown(10), indexToDrop);
		}

		// Token: 0x060001F8 RID: 504 RVA: 0x0000ADBB File Offset: 0x00008FBB
		private bool IsMainAgentAvailable()
		{
			Agent main = Agent.Main;
			return main != null && main.IsActive();
		}

		// Token: 0x060001F9 RID: 505 RVA: 0x0000ADCD File Offset: 0x00008FCD
		public override void OnPhotoModeActivated()
		{
			base.OnPhotoModeActivated();
			this._gauntletLayer._gauntletUIContext.ContextAlpha = 0f;
		}

		// Token: 0x060001FA RID: 506 RVA: 0x0000ADEA File Offset: 0x00008FEA
		public override void OnPhotoModeDeactivated()
		{
			base.OnPhotoModeDeactivated();
			this._gauntletLayer._gauntletUIContext.ContextAlpha = 1f;
		}

		// Token: 0x060001FB RID: 507 RVA: 0x0000AE07 File Offset: 0x00009007
		private void OnGamepadActiveChanged()
		{
			this._dataSource.OnGamepadActiveChanged(TaleWorlds.InputSystem.Input.IsGamepadActive);
		}

		// Token: 0x060001FC RID: 508 RVA: 0x0000AE1C File Offset: 0x0000901C
		private int GetKeyWeaponIndex(bool isReleased)
		{
			Func<string, bool> func;
			Func<string, bool> func2;
			if (isReleased)
			{
				func = new Func<string, bool>(base.MissionScreen.SceneLayer.Input.IsHotKeyReleased);
				func2 = new Func<string, bool>(this._gauntletLayer.Input.IsHotKeyReleased);
			}
			else
			{
				func = new Func<string, bool>(base.MissionScreen.SceneLayer.Input.IsHotKeyDown);
				func2 = new Func<string, bool>(this._gauntletLayer.Input.IsHotKeyDown);
			}
			string text = string.Empty;
			if (func("ControllerEquipDropWeapon1") || func2("ControllerEquipDropWeapon1"))
			{
				text = "ControllerEquipDropWeapon1";
			}
			else if (func("ControllerEquipDropWeapon2") || func2("ControllerEquipDropWeapon2"))
			{
				text = "ControllerEquipDropWeapon2";
			}
			else if (func("ControllerEquipDropWeapon3") || func2("ControllerEquipDropWeapon3"))
			{
				text = "ControllerEquipDropWeapon3";
			}
			else if (func("ControllerEquipDropWeapon4") || func2("ControllerEquipDropWeapon4"))
			{
				text = "ControllerEquipDropWeapon4";
			}
			if (!string.IsNullOrEmpty(text))
			{
				for (int i = 0; i < this._dataSource.EquippedWeapons.Count; i++)
				{
					InputKeyItemVM shortcutKey = this._dataSource.EquippedWeapons[i].ShortcutKey;
					if (((shortcutKey != null) ? shortcutKey.HotKey.Id : null) == text)
					{
						return (int)this._dataSource.EquippedWeapons[i].Identifier;
					}
				}
			}
			return -1;
		}

		// Token: 0x040000E9 RID: 233
		private const int _missionTimeSpeedRequestID = 624;

		// Token: 0x040000EA RID: 234
		private const float _slowDownAmountWhileRadialIsOpen = 0.25f;

		// Token: 0x040000EB RID: 235
		private bool _isSlowDownApplied;

		// Token: 0x040000EC RID: 236
		private GauntletLayer _gauntletLayer;

		// Token: 0x040000ED RID: 237
		private MissionMainAgentControllerEquipDropVM _dataSource;

		// Token: 0x040000EE RID: 238
		private MissionMainAgentController _missionMainAgentController;

		// Token: 0x040000EF RID: 239
		private EquipmentControllerLeaveLogic _missionControllerLeaveLogic;

		// Token: 0x040000F0 RID: 240
		private const float _minOpenHoldTime = 0.3f;

		// Token: 0x040000F1 RID: 241
		private const float _minDropHoldTime = 0.5f;

		// Token: 0x040000F2 RID: 242
		private readonly IMissionScreen _missionScreenAsInterface;

		// Token: 0x040000F3 RID: 243
		private bool _holdHandled;

		// Token: 0x040000F4 RID: 244
		private float _toggleHoldTime;

		// Token: 0x040000F5 RID: 245
		private float _weaponDropHoldTime;

		// Token: 0x040000F6 RID: 246
		private bool _prevKeyDown;

		// Token: 0x040000F7 RID: 247
		private bool _weaponDropHandled;
	}
}
