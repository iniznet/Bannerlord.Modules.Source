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
	[OverrideView(typeof(MissionMainAgentEquipDropView))]
	public class MissionGauntletMainAgentEquipDropView : MissionView
	{
		private bool IsDisplayingADialog
		{
			get
			{
				IMissionScreen missionScreenAsInterface = this._missionScreenAsInterface;
				return (missionScreenAsInterface != null && missionScreenAsInterface.GetDisplayDialog()) || base.MissionScreen.IsRadialMenuActive || base.Mission.IsOrderMenuOpen;
			}
		}

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

		public MissionGauntletMainAgentEquipDropView()
		{
			this._missionScreenAsInterface = base.MissionScreen;
			this.HoldHandled = false;
		}

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

		public override void AfterStart()
		{
			base.AfterStart();
			this._dataSource.InitializeMainAgentPropterties();
		}

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

		public override void OnMissionScreenTick(float dt)
		{
			base.OnMissionScreenTick(dt);
			if (this._dataSource.IsActive && !this.IsMainAgentAvailable())
			{
				this.HandleClosingHold();
			}
			if (this.IsMainAgentAvailable() && (!base.MissionScreen.IsRadialMenuActive || this._dataSource.IsActive))
			{
				this.TickControls(dt);
			}
		}

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

		public override void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow blow)
		{
			if (affectedAgent == Agent.Main)
			{
				this.HandleClosingHold();
			}
		}

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

		private bool IsMainAgentAvailable()
		{
			Agent main = Agent.Main;
			return main != null && main.IsActive();
		}

		public override void OnPhotoModeActivated()
		{
			base.OnPhotoModeActivated();
			this._gauntletLayer._gauntletUIContext.ContextAlpha = 0f;
		}

		public override void OnPhotoModeDeactivated()
		{
			base.OnPhotoModeDeactivated();
			this._gauntletLayer._gauntletUIContext.ContextAlpha = 1f;
		}

		private void OnGamepadActiveChanged()
		{
			this._dataSource.OnGamepadActiveChanged(TaleWorlds.InputSystem.Input.IsGamepadActive);
		}

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

		private const int _missionTimeSpeedRequestID = 624;

		private const float _slowDownAmountWhileRadialIsOpen = 0.25f;

		private bool _isSlowDownApplied;

		private GauntletLayer _gauntletLayer;

		private MissionMainAgentControllerEquipDropVM _dataSource;

		private MissionMainAgentController _missionMainAgentController;

		private EquipmentControllerLeaveLogic _missionControllerLeaveLogic;

		private const float _minOpenHoldTime = 0.3f;

		private const float _minDropHoldTime = 0.5f;

		private readonly IMissionScreen _missionScreenAsInterface;

		private bool _holdHandled;

		private float _toggleHoldTime;

		private float _weaponDropHoldTime;

		private bool _prevKeyDown;

		private bool _weaponDropHandled;
	}
}
