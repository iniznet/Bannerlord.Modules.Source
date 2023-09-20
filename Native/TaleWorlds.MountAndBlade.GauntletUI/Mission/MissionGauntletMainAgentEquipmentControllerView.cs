using System;
using System.ComponentModel;
using NetworkMessages.FromClient;
using TaleWorlds.Core;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.MissionViews;
using TaleWorlds.MountAndBlade.View.Screens;
using TaleWorlds.MountAndBlade.ViewModelCollection;
using TaleWorlds.MountAndBlade.ViewModelCollection.HUD;

namespace TaleWorlds.MountAndBlade.GauntletUI.Mission
{
	// Token: 0x0200002B RID: 43
	[OverrideView(typeof(MissionMainAgentEquipmentControllerView))]
	public class MissionGauntletMainAgentEquipmentControllerView : MissionView
	{
		// Token: 0x14000001 RID: 1
		// (add) Token: 0x060001FD RID: 509 RVA: 0x0000AF98 File Offset: 0x00009198
		// (remove) Token: 0x060001FE RID: 510 RVA: 0x0000AFD0 File Offset: 0x000091D0
		public event Action<bool> OnEquipmentDropInteractionViewToggled;

		// Token: 0x14000002 RID: 2
		// (add) Token: 0x060001FF RID: 511 RVA: 0x0000B008 File Offset: 0x00009208
		// (remove) Token: 0x06000200 RID: 512 RVA: 0x0000B040 File Offset: 0x00009240
		public event Action<bool> OnEquipmentEquipInteractionViewToggled;

		// Token: 0x1700004D RID: 77
		// (get) Token: 0x06000201 RID: 513 RVA: 0x0000B075 File Offset: 0x00009275
		private bool IsDisplayingADialog
		{
			get
			{
				IMissionScreen missionScreenAsInterface = this._missionScreenAsInterface;
				return missionScreenAsInterface != null && missionScreenAsInterface.GetDisplayDialog();
			}
		}

		// Token: 0x1700004E RID: 78
		// (get) Token: 0x06000202 RID: 514 RVA: 0x0000B088 File Offset: 0x00009288
		// (set) Token: 0x06000203 RID: 515 RVA: 0x0000B090 File Offset: 0x00009290
		private bool EquipHoldHandled
		{
			get
			{
				return this._equipHoldHandled;
			}
			set
			{
				this._equipHoldHandled = value;
				MissionScreen missionScreen = base.MissionScreen;
				if (missionScreen == null)
				{
					return;
				}
				missionScreen.SetRadialMenuActiveState(value);
			}
		}

		// Token: 0x1700004F RID: 79
		// (get) Token: 0x06000204 RID: 516 RVA: 0x0000B0AA File Offset: 0x000092AA
		// (set) Token: 0x06000205 RID: 517 RVA: 0x0000B0B2 File Offset: 0x000092B2
		private bool DropHoldHandled
		{
			get
			{
				return this._dropHoldHandled;
			}
			set
			{
				this._dropHoldHandled = value;
				MissionScreen missionScreen = base.MissionScreen;
				if (missionScreen == null)
				{
					return;
				}
				missionScreen.SetRadialMenuActiveState(value);
			}
		}

		// Token: 0x06000206 RID: 518 RVA: 0x0000B0CC File Offset: 0x000092CC
		public MissionGauntletMainAgentEquipmentControllerView()
		{
			this._missionScreenAsInterface = base.MissionScreen;
			this.EquipHoldHandled = false;
			this.DropHoldHandled = false;
		}

		// Token: 0x06000207 RID: 519 RVA: 0x0000B0F0 File Offset: 0x000092F0
		public override void OnMissionScreenInitialize()
		{
			base.OnMissionScreenInitialize();
			this._gauntletLayer = new GauntletLayer(2, "GauntletLayer", false);
			this._dataSource = new MissionMainAgentEquipmentControllerVM(new Action<EquipmentIndex>(this.OnDropEquipment), new Action<SpawnedItemEntity, EquipmentIndex>(this.OnEquipItem));
			this._gauntletLayer.LoadMovie("MainAgentEquipmentController", this._dataSource);
			this._gauntletLayer.InputRestrictions.SetInputRestrictions(false, 0);
			base.MissionScreen.AddLayer(this._gauntletLayer);
			base.Mission.OnMainAgentChanged += this.OnMainAgentChanged;
		}

		// Token: 0x06000208 RID: 520 RVA: 0x0000B18C File Offset: 0x0000938C
		public override void OnMissionScreenFinalize()
		{
			base.OnMissionScreenFinalize();
			base.Mission.OnMainAgentChanged -= this.OnMainAgentChanged;
			base.MissionScreen.RemoveLayer(this._gauntletLayer);
			this._gauntletLayer = null;
			this._dataSource.OnFinalize();
			this._dataSource = null;
		}

		// Token: 0x06000209 RID: 521 RVA: 0x0000B1E0 File Offset: 0x000093E0
		public override void OnMissionScreenTick(float dt)
		{
			base.OnMissionScreenTick(dt);
			if (this.IsMainAgentAvailable() && base.Mission.IsMainAgentItemInteractionEnabled)
			{
				this.DropWeaponTick(dt);
				this.EquipWeaponTick(dt);
				return;
			}
			this._prevDropKeyDown = false;
			this._prevEquipKeyDown = false;
		}

		// Token: 0x0600020A RID: 522 RVA: 0x0000B21C File Offset: 0x0000941C
		public override void OnFocusGained(Agent agent, IFocusable focusableObject, bool isInteractable)
		{
			base.OnFocusGained(agent, focusableObject, isInteractable);
			UsableMissionObject usableMissionObject;
			SpawnedItemEntity spawnedItemEntity;
			if ((usableMissionObject = focusableObject as UsableMissionObject) != null && (spawnedItemEntity = usableMissionObject as SpawnedItemEntity) != null)
			{
				this._isCurrentFocusedItemInteractable = isInteractable;
				if (!spawnedItemEntity.WeaponCopy.IsEmpty)
				{
					this._isFocusedOnEquipment = true;
					this._focusedWeaponItem = spawnedItemEntity;
					this._dataSource.SetCurrentFocusedWeaponEntity(this._focusedWeaponItem);
				}
			}
		}

		// Token: 0x0600020B RID: 523 RVA: 0x0000B27C File Offset: 0x0000947C
		public override void OnFocusLost(Agent agent, IFocusable focusableObject)
		{
			base.OnFocusLost(agent, focusableObject);
			this._isCurrentFocusedItemInteractable = false;
			this._isFocusedOnEquipment = false;
			this._focusedWeaponItem = null;
			this._dataSource.SetCurrentFocusedWeaponEntity(this._focusedWeaponItem);
			if (this.EquipHoldHandled)
			{
				this.EquipHoldHandled = false;
				this._equipHoldTime = 0f;
				this._dataSource.OnCancelEquipController();
				this._equipmentWasInFocusFirstFrameOfEquipDown = false;
			}
		}

		// Token: 0x0600020C RID: 524 RVA: 0x0000B2E4 File Offset: 0x000094E4
		private void OnMainAgentChanged(object sender, PropertyChangedEventArgs e)
		{
			if (base.Mission.MainAgent == null)
			{
				if (this.EquipHoldHandled)
				{
					this.EquipHoldHandled = false;
					Action<bool> onEquipmentEquipInteractionViewToggled = this.OnEquipmentEquipInteractionViewToggled;
					if (onEquipmentEquipInteractionViewToggled != null)
					{
						onEquipmentEquipInteractionViewToggled(false);
					}
				}
				this._equipHoldTime = 0f;
				this._dataSource.OnCancelEquipController();
				if (this.DropHoldHandled)
				{
					Action<bool> onEquipmentDropInteractionViewToggled = this.OnEquipmentDropInteractionViewToggled;
					if (onEquipmentDropInteractionViewToggled != null)
					{
						onEquipmentDropInteractionViewToggled(false);
					}
					this.DropHoldHandled = false;
				}
				this._dropHoldTime = 0f;
				this._dataSource.OnCancelDropController();
			}
		}

		// Token: 0x0600020D RID: 525 RVA: 0x0000B36C File Offset: 0x0000956C
		private void EquipWeaponTick(float dt)
		{
			if (base.MissionScreen.SceneLayer.Input.IsGameKeyDown(13) && !this._prevDropKeyDown && !this.IsDisplayingADialog && this.IsMainAgentAvailable() && !base.MissionScreen.Mission.IsOrderMenuOpen)
			{
				if (!this._firstFrameOfEquipDownHandled)
				{
					this._equipmentWasInFocusFirstFrameOfEquipDown = this._isFocusedOnEquipment;
					this._firstFrameOfEquipDownHandled = true;
				}
				if (this._equipmentWasInFocusFirstFrameOfEquipDown)
				{
					this._equipHoldTime += dt;
					if (this._equipHoldTime > 0.5f && !this.EquipHoldHandled && this._isFocusedOnEquipment && this._isCurrentFocusedItemInteractable)
					{
						this.HandleOpeningHoldEquip();
						this.EquipHoldHandled = true;
					}
				}
				this._prevEquipKeyDown = true;
				return;
			}
			if (this._prevEquipKeyDown && !base.MissionScreen.SceneLayer.Input.IsGameKeyDown(13))
			{
				if (this._equipmentWasInFocusFirstFrameOfEquipDown)
				{
					if (this._equipHoldTime < 0.5f)
					{
						if (this._focusedWeaponItem != null)
						{
							Agent main = Agent.Main;
							if (main != null && main.CanQuickPickUp(this._focusedWeaponItem))
							{
								this.HandleQuickReleaseEquip();
							}
						}
					}
					else
					{
						this.HandleClosingHoldEquip();
					}
				}
				if (this.EquipHoldHandled)
				{
					this.EquipHoldHandled = false;
				}
				this._equipHoldTime = 0f;
				this._firstFrameOfEquipDownHandled = false;
				this._prevEquipKeyDown = false;
			}
		}

		// Token: 0x0600020E RID: 526 RVA: 0x0000B4C0 File Offset: 0x000096C0
		private void DropWeaponTick(float dt)
		{
			if (base.MissionScreen.SceneLayer.Input.IsGameKeyDown(22) && !this._prevEquipKeyDown && !this.IsDisplayingADialog && this.IsMainAgentAvailable() && this.IsMainAgentHasAtLeastOneItem() && !base.MissionScreen.Mission.IsOrderMenuOpen)
			{
				this._dropHoldTime += dt;
				if (this._dropHoldTime > 0.5f && !this.DropHoldHandled)
				{
					this.HandleOpeningHoldDrop();
					this.DropHoldHandled = true;
				}
				this._prevDropKeyDown = true;
				return;
			}
			if (this._prevDropKeyDown && !base.MissionScreen.SceneLayer.Input.IsGameKeyDown(22))
			{
				if (this._dropHoldTime < 0.5f)
				{
					this.HandleQuickReleaseDrop();
				}
				else
				{
					this.HandleClosingHoldDrop();
				}
				this.DropHoldHandled = false;
				this._dropHoldTime = 0f;
				this._prevDropKeyDown = false;
			}
		}

		// Token: 0x0600020F RID: 527 RVA: 0x0000B5A5 File Offset: 0x000097A5
		private void HandleOpeningHoldEquip()
		{
			MissionMainAgentEquipmentControllerVM dataSource = this._dataSource;
			if (dataSource != null)
			{
				dataSource.OnEquipControllerToggle(true);
			}
			Action<bool> onEquipmentEquipInteractionViewToggled = this.OnEquipmentEquipInteractionViewToggled;
			if (onEquipmentEquipInteractionViewToggled == null)
			{
				return;
			}
			onEquipmentEquipInteractionViewToggled(true);
		}

		// Token: 0x06000210 RID: 528 RVA: 0x0000B5CA File Offset: 0x000097CA
		private void HandleClosingHoldEquip()
		{
			MissionMainAgentEquipmentControllerVM dataSource = this._dataSource;
			if (dataSource != null)
			{
				dataSource.OnEquipControllerToggle(false);
			}
			Action<bool> onEquipmentEquipInteractionViewToggled = this.OnEquipmentEquipInteractionViewToggled;
			if (onEquipmentEquipInteractionViewToggled == null)
			{
				return;
			}
			onEquipmentEquipInteractionViewToggled(false);
		}

		// Token: 0x06000211 RID: 529 RVA: 0x0000B5EF File Offset: 0x000097EF
		private void HandleQuickReleaseEquip()
		{
			this.OnEquipItem(this._focusedWeaponItem, -1);
		}

		// Token: 0x06000212 RID: 530 RVA: 0x0000B5FE File Offset: 0x000097FE
		private void HandleOpeningHoldDrop()
		{
			MissionMainAgentEquipmentControllerVM dataSource = this._dataSource;
			if (dataSource != null)
			{
				dataSource.OnDropControllerToggle(true);
			}
			Action<bool> onEquipmentDropInteractionViewToggled = this.OnEquipmentDropInteractionViewToggled;
			if (onEquipmentDropInteractionViewToggled == null)
			{
				return;
			}
			onEquipmentDropInteractionViewToggled(true);
		}

		// Token: 0x06000213 RID: 531 RVA: 0x0000B623 File Offset: 0x00009823
		private void HandleClosingHoldDrop()
		{
			MissionMainAgentEquipmentControllerVM dataSource = this._dataSource;
			if (dataSource != null)
			{
				dataSource.OnDropControllerToggle(false);
			}
			Action<bool> onEquipmentDropInteractionViewToggled = this.OnEquipmentDropInteractionViewToggled;
			if (onEquipmentDropInteractionViewToggled == null)
			{
				return;
			}
			onEquipmentDropInteractionViewToggled(false);
		}

		// Token: 0x06000214 RID: 532 RVA: 0x0000B648 File Offset: 0x00009848
		private void HandleQuickReleaseDrop()
		{
			this.OnDropEquipment(-1);
		}

		// Token: 0x06000215 RID: 533 RVA: 0x0000B651 File Offset: 0x00009851
		private void OnEquipItem(SpawnedItemEntity itemToEquip, EquipmentIndex indexToEquipItTo)
		{
			if (itemToEquip.GameEntity != null)
			{
				Agent main = Agent.Main;
				if (main == null)
				{
					return;
				}
				main.HandleStartUsingAction(itemToEquip, indexToEquipItTo);
			}
		}

		// Token: 0x06000216 RID: 534 RVA: 0x0000B674 File Offset: 0x00009874
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

		// Token: 0x06000217 RID: 535 RVA: 0x0000B6C3 File Offset: 0x000098C3
		private bool IsMainAgentAvailable()
		{
			Agent main = Agent.Main;
			return main != null && main.IsActive();
		}

		// Token: 0x06000218 RID: 536 RVA: 0x0000B6D8 File Offset: 0x000098D8
		private bool IsMainAgentHasAtLeastOneItem()
		{
			for (EquipmentIndex equipmentIndex = 0; equipmentIndex < 5; equipmentIndex++)
			{
				if (!Agent.Main.Equipment[equipmentIndex].IsEmpty)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000219 RID: 537 RVA: 0x0000B70E File Offset: 0x0000990E
		public override void OnPhotoModeActivated()
		{
			base.OnPhotoModeActivated();
			this._gauntletLayer._gauntletUIContext.ContextAlpha = 0f;
		}

		// Token: 0x0600021A RID: 538 RVA: 0x0000B72B File Offset: 0x0000992B
		public override void OnPhotoModeDeactivated()
		{
			base.OnPhotoModeDeactivated();
			this._gauntletLayer._gauntletUIContext.ContextAlpha = 1f;
		}

		// Token: 0x040000F8 RID: 248
		private const float _minHoldTime = 0.5f;

		// Token: 0x040000FB RID: 251
		private readonly IMissionScreen _missionScreenAsInterface;

		// Token: 0x040000FC RID: 252
		private bool _equipmentWasInFocusFirstFrameOfEquipDown;

		// Token: 0x040000FD RID: 253
		private bool _firstFrameOfEquipDownHandled;

		// Token: 0x040000FE RID: 254
		private bool _equipHoldHandled;

		// Token: 0x040000FF RID: 255
		private bool _isFocusedOnEquipment;

		// Token: 0x04000100 RID: 256
		private float _equipHoldTime;

		// Token: 0x04000101 RID: 257
		private bool _prevEquipKeyDown;

		// Token: 0x04000102 RID: 258
		private SpawnedItemEntity _focusedWeaponItem;

		// Token: 0x04000103 RID: 259
		private bool _dropHoldHandled;

		// Token: 0x04000104 RID: 260
		private float _dropHoldTime;

		// Token: 0x04000105 RID: 261
		private bool _prevDropKeyDown;

		// Token: 0x04000106 RID: 262
		private bool _isCurrentFocusedItemInteractable;

		// Token: 0x04000107 RID: 263
		private GauntletLayer _gauntletLayer;

		// Token: 0x04000108 RID: 264
		private MissionMainAgentEquipmentControllerVM _dataSource;
	}
}
