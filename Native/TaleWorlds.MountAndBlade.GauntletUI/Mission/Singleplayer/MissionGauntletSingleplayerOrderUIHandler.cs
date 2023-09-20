using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Missions.Handlers;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.MissionViews;
using TaleWorlds.MountAndBlade.View.MissionViews.Order;
using TaleWorlds.MountAndBlade.View.MissionViews.Singleplayer;
using TaleWorlds.MountAndBlade.View.Screens;
using TaleWorlds.MountAndBlade.ViewModelCollection.Order;
using TaleWorlds.ScreenSystem;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Mission.Singleplayer
{
	// Token: 0x02000036 RID: 54
	[OverrideView(typeof(MissionOrderUIHandler))]
	public class MissionGauntletSingleplayerOrderUIHandler : MissionView, ISiegeDeploymentView
	{
		// Token: 0x17000054 RID: 84
		// (get) Token: 0x06000289 RID: 649 RVA: 0x0000E237 File Offset: 0x0000C437
		private float _minHoldTimeForActivation
		{
			get
			{
				return 0f;
			}
		}

		// Token: 0x17000055 RID: 85
		// (get) Token: 0x0600028A RID: 650 RVA: 0x0000E23E File Offset: 0x0000C43E
		// (set) Token: 0x0600028B RID: 651 RVA: 0x0000E246 File Offset: 0x0000C446
		public bool IsSiegeDeployment { get; private set; }

		// Token: 0x17000056 RID: 86
		// (get) Token: 0x0600028C RID: 652 RVA: 0x0000E24F File Offset: 0x0000C44F
		// (set) Token: 0x0600028D RID: 653 RVA: 0x0000E257 File Offset: 0x0000C457
		public bool IsBattleDeployment { get; private set; }

		// Token: 0x17000057 RID: 87
		// (get) Token: 0x0600028E RID: 654 RVA: 0x0000E260 File Offset: 0x0000C460
		private bool _isAnyDeployment
		{
			get
			{
				return this.IsSiegeDeployment || this.IsBattleDeployment;
			}
		}

		// Token: 0x17000058 RID: 88
		// (get) Token: 0x0600028F RID: 655 RVA: 0x0000E272 File Offset: 0x0000C472
		public bool IsOrderMenuActive
		{
			get
			{
				MissionOrderVM dataSource = this._dataSource;
				return dataSource != null && dataSource.IsToggleOrderShown;
			}
		}

		// Token: 0x14000003 RID: 3
		// (add) Token: 0x06000290 RID: 656 RVA: 0x0000E288 File Offset: 0x0000C488
		// (remove) Token: 0x06000291 RID: 657 RVA: 0x0000E2C0 File Offset: 0x0000C4C0
		public event Action<bool> OnCameraControlsToggled;

		// Token: 0x06000292 RID: 658 RVA: 0x0000E2F5 File Offset: 0x0000C4F5
		public MissionGauntletSingleplayerOrderUIHandler()
		{
			this.ViewOrderPriority = 14;
		}

		// Token: 0x06000293 RID: 659 RVA: 0x0000E308 File Offset: 0x0000C508
		public override void OnMissionScreenTick(float dt)
		{
			base.OnMissionScreenTick(dt);
			this._latestDt = dt;
			this._isReceivingInput = false;
			if (!base.MissionScreen.IsPhotoModeEnabled && (!base.MissionScreen.IsRadialMenuActive || this._dataSource.IsToggleOrderShown))
			{
				this.TickInput(dt);
				this._dataSource.Update();
				if (this._dataSource.IsToggleOrderShown)
				{
					this._orderTroopPlacer.IsDrawingForced = this._dataSource.IsMovementSubOrdersShown;
					this._orderTroopPlacer.IsDrawingFacing = this._dataSource.IsFacingSubOrdersShown;
					this._orderTroopPlacer.IsDrawingForming = false;
					if (this.cursorState == 1)
					{
						Vec2 orderLookAtDirection = OrderController.GetOrderLookAtDirection(base.Mission.MainAgent.Team.PlayerOrderController.SelectedFormations, base.MissionScreen.OrderFlag.Position.AsVec2);
						base.MissionScreen.OrderFlag.SetArrowVisibility(true, orderLookAtDirection);
					}
					else
					{
						base.MissionScreen.OrderFlag.SetArrowVisibility(false, Vec2.Invalid);
					}
					if (this.cursorState == 2)
					{
						float orderFormCustomWidth = OrderController.GetOrderFormCustomWidth(base.Mission.MainAgent.Team.PlayerOrderController.SelectedFormations, base.MissionScreen.OrderFlag.Position);
						base.MissionScreen.OrderFlag.SetWidthVisibility(true, orderFormCustomWidth);
					}
					else
					{
						base.MissionScreen.OrderFlag.SetWidthVisibility(false, -1f);
					}
					if (TaleWorlds.InputSystem.Input.IsGamepadActive)
					{
						OrderItemVM lastSelectedOrderItem = this._dataSource.LastSelectedOrderItem;
						if (lastSelectedOrderItem == null || lastSelectedOrderItem.IsTitle)
						{
							if (this._orderTroopPlacer.SuspendTroopPlacer && this._dataSource.ActiveTargetState == 0)
							{
								this._orderTroopPlacer.SuspendTroopPlacer = false;
							}
						}
						else if (!this._orderTroopPlacer.SuspendTroopPlacer)
						{
							this._orderTroopPlacer.SuspendTroopPlacer = true;
						}
					}
				}
				else if (this._dataSource.TroopController.IsTransferActive || this._isAnyDeployment)
				{
					this._gauntletLayer.InputRestrictions.SetInputRestrictions(true, 7);
				}
				else
				{
					if (!this._dataSource.TroopController.IsTransferActive && !this._orderTroopPlacer.SuspendTroopPlacer)
					{
						this._orderTroopPlacer.SuspendTroopPlacer = true;
					}
					this._gauntletLayer.InputRestrictions.ResetInputRestrictions();
				}
				if (this._isAnyDeployment)
				{
					if (base.MissionScreen.SceneLayer.Input.IsKeyDown(225) || base.MissionScreen.SceneLayer.Input.IsKeyDown(254))
					{
						Action<bool> onCameraControlsToggled = this.OnCameraControlsToggled;
						if (onCameraControlsToggled != null)
						{
							onCameraControlsToggled(true);
						}
					}
					else
					{
						Action<bool> onCameraControlsToggled2 = this.OnCameraControlsToggled;
						if (onCameraControlsToggled2 != null)
						{
							onCameraControlsToggled2(false);
						}
					}
				}
				base.MissionScreen.OrderFlag.IsTroop = this._dataSource.ActiveTargetState == 0;
				this.TickOrderFlag(this._latestDt, false);
				bool flag;
				if (this._dataSource.IsToggleOrderShown)
				{
					if (this._dataSource.OrderSets.Any((OrderSetVM x) => x.ShowOrders))
					{
						flag = this._holdExecuted || base.Mission.Mode == 6;
						goto IL_32D;
					}
				}
				flag = false;
				IL_32D:
				bool flag2 = flag;
				if (flag2 != base.MissionScreen.IsRadialMenuActive)
				{
					base.MissionScreen.SetRadialMenuActiveState(flag2);
				}
			}
			this._dataSource.IsHolding = this._holdExecuted;
			this._dataSource.CanUseShortcuts = this._isReceivingInput;
		}

		// Token: 0x06000294 RID: 660 RVA: 0x0000E67F File Offset: 0x0000C87F
		public override bool OnEscape()
		{
			bool isToggleOrderShown = this._dataSource.IsToggleOrderShown;
			this._dataSource.OnEscape();
			return isToggleOrderShown;
		}

		// Token: 0x06000295 RID: 661 RVA: 0x0000E697 File Offset: 0x0000C897
		public override void OnMissionScreenActivate()
		{
			base.OnMissionScreenActivate();
			this._dataSource.AfterInitialize();
			this._isInitialized = true;
		}

		// Token: 0x06000296 RID: 662 RVA: 0x0000E6B1 File Offset: 0x0000C8B1
		public override void OnAgentBuild(Agent agent, Banner banner)
		{
			if (!this._isInitialized)
			{
				return;
			}
			if (agent.IsHuman)
			{
				this._dataSource.TroopController.AddTroops(agent);
			}
		}

		// Token: 0x06000297 RID: 663 RVA: 0x0000E6D5 File Offset: 0x0000C8D5
		public override void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow killingBlow)
		{
			base.OnAgentRemoved(affectedAgent, affectorAgent, agentState, killingBlow);
			if (affectedAgent.IsHuman)
			{
				this._dataSource.TroopController.RemoveTroops(affectedAgent);
			}
		}

		// Token: 0x06000298 RID: 664 RVA: 0x0000E6FC File Offset: 0x0000C8FC
		public override void OnMissionScreenInitialize()
		{
			base.OnMissionScreenInitialize();
			base.MissionScreen.SceneLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("MissionOrderHotkeyCategory"));
			base.MissionScreen.OrderFlag = new OrderFlag(base.Mission, base.MissionScreen);
			this._orderTroopPlacer = base.Mission.GetMissionBehavior<OrderTroopPlacer>();
			base.MissionScreen.SetOrderFlagVisibility(false);
			this._siegeDeploymentHandler = base.Mission.GetMissionBehavior<SiegeDeploymentHandler>();
			this._battleDeploymentHandler = base.Mission.GetMissionBehavior<BattleDeploymentHandler>();
			this.IsSiegeDeployment = this._siegeDeploymentHandler != null;
			this.IsBattleDeployment = this._battleDeploymentHandler != null;
			if (this._isAnyDeployment)
			{
				this._deploymentMissionView = base.Mission.GetMissionBehavior<DeploymentMissionView>();
				if (this._deploymentMissionView != null)
				{
					DeploymentMissionView deploymentMissionView = this._deploymentMissionView;
					deploymentMissionView.OnDeploymentFinish = (OnPlayerDeploymentFinishDelegate)Delegate.Combine(deploymentMissionView.OnDeploymentFinish, new OnPlayerDeploymentFinishDelegate(this.OnDeploymentFinish));
				}
				this._deploymentPointDataSources = new List<DeploymentSiegeMachineVM>();
			}
			this._dataSource = new MissionOrderVM(base.MissionScreen.CombatCamera, this.IsSiegeDeployment ? this._siegeDeploymentHandler.PlayerDeploymentPoints.ToList<DeploymentPoint>() : new List<DeploymentPoint>(), new Action<bool>(this.ToggleScreenRotation), this._isAnyDeployment, new GetOrderFlagPositionDelegate(base.MissionScreen.GetOrderFlagPosition), new OnRefreshVisualsDelegate(this.RefreshVisuals), new ToggleOrderPositionVisibilityDelegate(this.SetSuspendTroopPlacer), new OnToggleActivateOrderStateDelegate(this.OnActivateToggleOrder), new OnToggleActivateOrderStateDelegate(this.OnDeactivateToggleOrder), new OnToggleActivateOrderStateDelegate(this.OnTransferFinished), new OnBeforeOrderDelegate(this.OnBeforeOrder), false);
			this._dataSource.SetCancelInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("ToggleEscapeMenu"));
			if (this.IsSiegeDeployment)
			{
				foreach (DeploymentPoint deploymentPoint in this._siegeDeploymentHandler.PlayerDeploymentPoints)
				{
					DeploymentSiegeMachineVM deploymentSiegeMachineVM = new DeploymentSiegeMachineVM(deploymentPoint, null, base.MissionScreen.CombatCamera, new Action<DeploymentSiegeMachineVM>(this._dataSource.DeploymentController.OnRefreshSelectedDeploymentPoint), new Action<DeploymentPoint>(this._dataSource.DeploymentController.OnEntityHover), false);
					Vec3 vec = deploymentPoint.GameEntity.GetFrame().origin;
					for (int i = 0; i < deploymentPoint.GameEntity.ChildCount; i++)
					{
						if (deploymentPoint.GameEntity.GetChild(i).HasTag("deployment_point_icon_target"))
						{
							vec += deploymentPoint.GameEntity.GetChild(i).GetFrame().origin;
							break;
						}
					}
					this._deploymentPointDataSources.Add(deploymentSiegeMachineVM);
					deploymentSiegeMachineVM.RemainingCount = 0;
				}
			}
			this._gauntletLayer = new GauntletLayer(this.ViewOrderPriority, "GauntletLayer", false);
			this._gauntletLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericPanelGameKeyCategory"));
			string text = ((BannerlordConfig.OrderType == 0) ? "OrderBar" : "OrderRadial");
			SpriteData spriteData = UIResourceManager.SpriteData;
			TwoDimensionEngineResourceContext resourceContext = UIResourceManager.ResourceContext;
			ResourceDepot uiresourceDepot = UIResourceManager.UIResourceDepot;
			this._spriteCategory = spriteData.SpriteCategories["ui_order"];
			this._spriteCategory.Load(resourceContext, uiresourceDepot);
			this._movie = this._gauntletLayer.LoadMovie(text, this._dataSource);
			base.MissionScreen.AddLayer(this._gauntletLayer);
			if (BannerlordConfig.HideBattleUI)
			{
				this._gauntletLayer._gauntletUIContext.ContextAlpha = 0f;
			}
			if (!this._isAnyDeployment && !this._dataSource.IsToggleOrderShown)
			{
				ScreenManager.SetSuspendLayer(this._gauntletLayer, true);
			}
			this._dataSource.InputRestrictions = this._gauntletLayer.InputRestrictions;
			ManagedOptions.OnManagedOptionChanged = (ManagedOptions.OnManagedOptionChangedDelegate)Delegate.Combine(ManagedOptions.OnManagedOptionChanged, new ManagedOptions.OnManagedOptionChangedDelegate(this.OnManagedOptionChanged));
		}

		// Token: 0x06000299 RID: 665 RVA: 0x0000EAE8 File Offset: 0x0000CCE8
		public override bool IsReady()
		{
			return this._spriteCategory.IsCategoryFullyLoaded();
		}

		// Token: 0x0600029A RID: 666 RVA: 0x0000EAF8 File Offset: 0x0000CCF8
		private void OnManagedOptionChanged(ManagedOptions.ManagedOptionsType changedManagedOptionsType)
		{
			if (changedManagedOptionsType == 31)
			{
				this._gauntletLayer.ReleaseMovie(this._movie);
				string text = ((BannerlordConfig.OrderType == 0) ? "OrderBar" : "OrderRadial");
				this._movie = this._gauntletLayer.LoadMovie(text, this._dataSource);
				return;
			}
			if (changedManagedOptionsType == 32)
			{
				MissionOrderVM dataSource = this._dataSource;
				if (dataSource == null)
				{
					return;
				}
				dataSource.OnOrderLayoutTypeChanged();
				return;
			}
			else
			{
				if (changedManagedOptionsType == 41)
				{
					this._gauntletLayer._gauntletUIContext.ContextAlpha = (BannerlordConfig.HideBattleUI ? 0f : 1f);
					return;
				}
				if (changedManagedOptionsType == 35 && !BannerlordConfig.SlowDownOnOrder && this._slowedDownMission)
				{
					base.Mission.RemoveTimeSpeedRequest(864);
				}
				return;
			}
		}

		// Token: 0x0600029B RID: 667 RVA: 0x0000EBAC File Offset: 0x0000CDAC
		public override void OnMissionScreenFinalize()
		{
			base.OnMissionScreenFinalize();
			ManagedOptions.OnManagedOptionChanged = (ManagedOptions.OnManagedOptionChangedDelegate)Delegate.Remove(ManagedOptions.OnManagedOptionChanged, new ManagedOptions.OnManagedOptionChangedDelegate(this.OnManagedOptionChanged));
			this._deploymentPointDataSources = null;
			this._orderTroopPlacer = null;
			this._movie = null;
			this._gauntletLayer = null;
			this._dataSource.OnFinalize();
			this._dataSource = null;
			this._siegeDeploymentHandler = null;
			this._spriteCategory.Unload();
			this._battleDeploymentHandler = null;
		}

		// Token: 0x0600029C RID: 668 RVA: 0x0000EC26 File Offset: 0x0000CE26
		public override void OnConversationBegin()
		{
			base.OnConversationBegin();
			MissionOrderVM dataSource = this._dataSource;
			if (dataSource == null)
			{
				return;
			}
			dataSource.TryCloseToggleOrder(true);
		}

		// Token: 0x0600029D RID: 669 RVA: 0x0000EC40 File Offset: 0x0000CE40
		public void OnActivateToggleOrder()
		{
			this.SetLayerEnabled(true);
		}

		// Token: 0x0600029E RID: 670 RVA: 0x0000EC49 File Offset: 0x0000CE49
		public void OnDeactivateToggleOrder()
		{
			if (!this._dataSource.TroopController.IsTransferActive)
			{
				this.SetLayerEnabled(false);
			}
		}

		// Token: 0x0600029F RID: 671 RVA: 0x0000EC64 File Offset: 0x0000CE64
		private void OnTransferFinished()
		{
			if (!this._isAnyDeployment)
			{
				this.SetLayerEnabled(false);
			}
		}

		// Token: 0x060002A0 RID: 672 RVA: 0x0000EC75 File Offset: 0x0000CE75
		public void OnAutoDeploy()
		{
			this._dataSource.DeploymentController.ExecuteAutoDeploy();
		}

		// Token: 0x060002A1 RID: 673 RVA: 0x0000EC87 File Offset: 0x0000CE87
		public void OnBeginMission()
		{
			this._dataSource.DeploymentController.ExecuteBeginSiege();
		}

		// Token: 0x060002A2 RID: 674 RVA: 0x0000EC9C File Offset: 0x0000CE9C
		private void SetLayerEnabled(bool isEnabled)
		{
			if (isEnabled)
			{
				if (!base.MissionScreen.IsRadialMenuActive)
				{
					if (this._dataSource == null || this._dataSource.ActiveTargetState == 0)
					{
						this._orderTroopPlacer.SuspendTroopPlacer = false;
					}
					if (!this._slowedDownMission && BannerlordConfig.SlowDownOnOrder)
					{
						base.Mission.AddTimeSpeedRequest(new Mission.TimeSpeedRequest(0.25f, 864));
						this._slowedDownMission = true;
					}
					base.MissionScreen.SetOrderFlagVisibility(true);
					if (this._gauntletLayer != null)
					{
						ScreenManager.SetSuspendLayer(this._gauntletLayer, false);
					}
					Game.Current.EventManager.TriggerEvent<MissionPlayerToggledOrderViewEvent>(new MissionPlayerToggledOrderViewEvent(true));
					return;
				}
			}
			else
			{
				this._orderTroopPlacer.SuspendTroopPlacer = true;
				base.MissionScreen.SetOrderFlagVisibility(false);
				if (this._gauntletLayer != null)
				{
					ScreenManager.SetSuspendLayer(this._gauntletLayer, true);
				}
				if (this._slowedDownMission)
				{
					base.Mission.RemoveTimeSpeedRequest(864);
					this._slowedDownMission = false;
				}
				Game.Current.EventManager.TriggerEvent<MissionPlayerToggledOrderViewEvent>(new MissionPlayerToggledOrderViewEvent(false));
			}
		}

		// Token: 0x060002A3 RID: 675 RVA: 0x0000EDA8 File Offset: 0x0000CFA8
		private void OnDeploymentFinish()
		{
			this.IsSiegeDeployment = false;
			this.IsBattleDeployment = false;
			this._dataSource.OnDeploymentFinished();
			this._deploymentPointDataSources.Clear();
			this._orderTroopPlacer.SuspendTroopPlacer = true;
			base.MissionScreen.SetOrderFlagVisibility(false);
			this._gauntletLayer.IsFocusLayer = false;
			ScreenManager.TryLoseFocus(this._gauntletLayer);
			base.MissionScreen.SetRadialMenuActiveState(false);
			if (this._deploymentMissionView != null)
			{
				DeploymentMissionView deploymentMissionView = this._deploymentMissionView;
				deploymentMissionView.OnDeploymentFinish = (OnPlayerDeploymentFinishDelegate)Delegate.Remove(deploymentMissionView.OnDeploymentFinish, new OnPlayerDeploymentFinishDelegate(this.OnDeploymentFinish));
			}
		}

		// Token: 0x060002A4 RID: 676 RVA: 0x0000EE44 File Offset: 0x0000D044
		private void RefreshVisuals()
		{
			if (this.IsSiegeDeployment)
			{
				foreach (DeploymentSiegeMachineVM deploymentSiegeMachineVM in this._deploymentPointDataSources)
				{
					deploymentSiegeMachineVM.RefreshWithDeployedWeapon();
				}
			}
		}

		// Token: 0x060002A5 RID: 677 RVA: 0x0000EE9C File Offset: 0x0000D09C
		private IOrderable GetFocusedOrderableObject()
		{
			return base.MissionScreen.OrderFlag.FocusedOrderableObject;
		}

		// Token: 0x060002A6 RID: 678 RVA: 0x0000EEAE File Offset: 0x0000D0AE
		private void SetSuspendTroopPlacer(bool value)
		{
			this._orderTroopPlacer.SuspendTroopPlacer = value;
			base.MissionScreen.SetOrderFlagVisibility(!value);
		}

		// Token: 0x060002A7 RID: 679 RVA: 0x0000EECB File Offset: 0x0000D0CB
		public void SelectFormationAtIndex(int index)
		{
			this._dataSource.OnSelect(index);
		}

		// Token: 0x060002A8 RID: 680 RVA: 0x0000EED9 File Offset: 0x0000D0D9
		public void DeselectFormationAtIndex(int index)
		{
			this._dataSource.TroopController.OnDeselectFormation(index);
		}

		// Token: 0x060002A9 RID: 681 RVA: 0x0000EEEC File Offset: 0x0000D0EC
		public void ClearFormationSelection()
		{
			MissionOrderVM dataSource = this._dataSource;
			if (dataSource != null)
			{
				dataSource.DeploymentController.ExecuteCancelSelectedDeploymentPoint();
			}
			MissionOrderVM dataSource2 = this._dataSource;
			if (dataSource2 != null)
			{
				dataSource2.OrderController.ClearSelectedFormations();
			}
			MissionOrderVM dataSource3 = this._dataSource;
			if (dataSource3 == null)
			{
				return;
			}
			dataSource3.TryCloseToggleOrder(false);
		}

		// Token: 0x060002AA RID: 682 RVA: 0x0000EF2C File Offset: 0x0000D12C
		public void OnFiltersSet(List<ValueTuple<int, List<int>>> filterData)
		{
			this._dataSource.OnFiltersSet(filterData);
		}

		// Token: 0x060002AB RID: 683 RVA: 0x0000EF3A File Offset: 0x0000D13A
		public void SetIsOrderPreconfigured(bool isOrderPreconfigured)
		{
			this._dataSource.DeploymentController.SetIsOrderPreconfigured(isOrderPreconfigured);
		}

		// Token: 0x060002AC RID: 684 RVA: 0x0000EF4D File Offset: 0x0000D14D
		private void OnBeforeOrder()
		{
			this.TickOrderFlag(this._latestDt, true);
		}

		// Token: 0x060002AD RID: 685 RVA: 0x0000EF5C File Offset: 0x0000D15C
		private void TickOrderFlag(float dt, bool forceUpdate)
		{
			if ((base.MissionScreen.OrderFlag.IsVisible || forceUpdate) && Utilities.EngineFrameNo != base.MissionScreen.OrderFlag.LatestUpdateFrameNo)
			{
				base.MissionScreen.OrderFlag.Tick(this._latestDt);
			}
		}

		// Token: 0x060002AE RID: 686 RVA: 0x0000EFAA File Offset: 0x0000D1AA
		void ISiegeDeploymentView.OnEntityHover(GameEntity hoveredEntity)
		{
			if (!this._gauntletLayer.IsHitThisFrame)
			{
				this._dataSource.DeploymentController.OnEntityHover(hoveredEntity);
			}
		}

		// Token: 0x060002AF RID: 687 RVA: 0x0000EFCA File Offset: 0x0000D1CA
		void ISiegeDeploymentView.OnEntitySelection(GameEntity selectedEntity)
		{
			this._dataSource.DeploymentController.OnEntitySelect(selectedEntity);
		}

		// Token: 0x060002B0 RID: 688 RVA: 0x0000EFDD File Offset: 0x0000D1DD
		private void ToggleScreenRotation(bool isLocked)
		{
			MissionScreen.SetFixedMissionCameraActive(isLocked);
		}

		// Token: 0x17000059 RID: 89
		// (get) Token: 0x060002B1 RID: 689 RVA: 0x0000EFE5 File Offset: 0x0000D1E5
		public MissionOrderVM.CursorState cursorState
		{
			get
			{
				if (this._dataSource.IsFacingSubOrdersShown)
				{
					return 1;
				}
				return 0;
			}
		}

		// Token: 0x060002B2 RID: 690 RVA: 0x0000EFF8 File Offset: 0x0000D1F8
		private void TickInput(float dt)
		{
			bool displayDialog = base.MissionScreen.GetDisplayDialog();
			bool flag = base.MissionScreen.SceneLayer.IsHitThisFrame || this._gauntletLayer.IsHitThisFrame;
			if (displayDialog || (TaleWorlds.InputSystem.Input.IsGamepadActive && !flag))
			{
				this._isReceivingInput = false;
				return;
			}
			this._isReceivingInput = true;
			if (!this.IsSiegeDeployment && !this.IsBattleDeployment)
			{
				if (base.Input.IsGameKeyDown(86) && !this._dataSource.IsToggleOrderShown)
				{
					this._holdTime += dt;
					if (this._holdTime >= this._minHoldTimeForActivation)
					{
						this._dataSource.OpenToggleOrder(true, !this._holdExecuted);
						this._holdExecuted = true;
					}
				}
				else if (!base.Input.IsGameKeyDown(86))
				{
					if (this._holdExecuted && this._dataSource.IsToggleOrderShown)
					{
						this._dataSource.TryCloseToggleOrder(false);
					}
					this._holdExecuted = false;
					this._holdTime = 0f;
				}
			}
			if (this._dataSource.IsToggleOrderShown)
			{
				if (this._dataSource.TroopController.IsTransferActive && this._gauntletLayer.Input.IsHotKeyPressed("Exit"))
				{
					this._dataSource.TroopController.IsTransferActive = false;
				}
				if (this._dataSource.ActiveTargetState == 0 && (base.Input.IsKeyReleased(224) || base.Input.IsKeyReleased(255)))
				{
					OrderItemVM lastSelectedOrderItem = this._dataSource.LastSelectedOrderItem;
					if (lastSelectedOrderItem != null && !lastSelectedOrderItem.IsTitle && TaleWorlds.InputSystem.Input.IsGamepadActive)
					{
						this._dataSource.ApplySelectedOrder();
					}
					else
					{
						switch (this.cursorState)
						{
						case 0:
						{
							IOrderable focusedOrderableObject = this.GetFocusedOrderableObject();
							if (focusedOrderableObject != null && OrderFlag.IsOrderPositionValid(ModuleExtensions.ToWorldPosition(base.MissionScreen.GetOrderFlagPosition())))
							{
								this._dataSource.OrderController.SetOrderWithOrderableObject(focusedOrderableObject);
							}
							break;
						}
						case 1:
							this._dataSource.OrderController.SetOrderWithPosition(16, new WorldPosition(Mission.Current.Scene, UIntPtr.Zero, base.MissionScreen.GetOrderFlagPosition(), false));
							break;
						case 2:
							this._dataSource.OrderController.SetOrderWithPosition(25, new WorldPosition(Mission.Current.Scene, UIntPtr.Zero, base.MissionScreen.GetOrderFlagPosition(), false));
							break;
						default:
							Debug.FailedAssert("false", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.GauntletUI\\Mission\\Singleplayer\\MissionGauntletSingleplayerOrderUIHandler.cs", "TickInput", 636);
							break;
						}
					}
				}
				if (base.Input.IsKeyReleased(225) && !this._isAnyDeployment)
				{
					this._dataSource.OnEscape();
				}
			}
			else if (this._dataSource.TroopController.IsTransferActive != this._isTransferEnabled)
			{
				this._isTransferEnabled = this._dataSource.TroopController.IsTransferActive;
				if (!this._isTransferEnabled)
				{
					this._gauntletLayer.IsFocusLayer = false;
					ScreenManager.TryLoseFocus(this._gauntletLayer);
				}
				else
				{
					this._gauntletLayer.IsFocusLayer = true;
					ScreenManager.TrySetFocus(this._gauntletLayer);
				}
			}
			int num = -1;
			if ((!TaleWorlds.InputSystem.Input.IsGamepadActive || this._dataSource.IsToggleOrderShown) && !base.DebugInput.IsControlDown())
			{
				if (base.Input.IsGameKeyPressed(68))
				{
					num = 0;
				}
				else if (base.Input.IsGameKeyPressed(69))
				{
					num = 1;
				}
				else if (base.Input.IsGameKeyPressed(70))
				{
					num = 2;
				}
				else if (base.Input.IsGameKeyPressed(71))
				{
					num = 3;
				}
				else if (base.Input.IsGameKeyPressed(72))
				{
					num = 4;
				}
				else if (base.Input.IsGameKeyPressed(73))
				{
					num = 5;
				}
				else if (base.Input.IsGameKeyPressed(74))
				{
					num = 6;
				}
				else if (base.Input.IsGameKeyPressed(75))
				{
					num = 7;
				}
				else if (base.Input.IsGameKeyPressed(76))
				{
					num = 8;
				}
			}
			if (num > -1)
			{
				this._dataSource.OnGiveOrder(num);
			}
			int num2 = -1;
			if (base.Input.IsGameKeyPressed(77))
			{
				num2 = 100;
			}
			else if (base.Input.IsGameKeyPressed(78))
			{
				num2 = 0;
			}
			else if (base.Input.IsGameKeyPressed(79))
			{
				num2 = 1;
			}
			else if (base.Input.IsGameKeyPressed(80))
			{
				num2 = 2;
			}
			else if (base.Input.IsGameKeyPressed(81))
			{
				num2 = 3;
			}
			else if (base.Input.IsGameKeyPressed(82))
			{
				num2 = 4;
			}
			else if (base.Input.IsGameKeyPressed(83))
			{
				num2 = 5;
			}
			else if (base.Input.IsGameKeyPressed(84))
			{
				num2 = 6;
			}
			else if (base.Input.IsGameKeyPressed(85))
			{
				num2 = 7;
			}
			if (!this.IsBattleDeployment && !this.IsSiegeDeployment && this._dataSource.IsToggleOrderShown)
			{
				if (base.Input.IsGameKeyPressed(87))
				{
					this._dataSource.SelectNextTroop(1);
				}
				else if (base.Input.IsGameKeyPressed(88))
				{
					this._dataSource.SelectNextTroop(-1);
				}
				else if (base.Input.IsGameKeyPressed(89))
				{
					this._dataSource.ToggleSelectionForCurrentTroop();
				}
			}
			if (num2 != -1)
			{
				this._dataSource.OnSelect(num2);
			}
			if (base.Input.IsGameKeyPressed(67))
			{
				this._dataSource.ViewOrders();
			}
		}

		// Token: 0x0400014B RID: 331
		private const string _radialOrderMovieName = "OrderRadial";

		// Token: 0x0400014C RID: 332
		private const string _barOrderMovieName = "OrderBar";

		// Token: 0x0400014D RID: 333
		private const float _slowDownAmountWhileOrderIsOpen = 0.25f;

		// Token: 0x0400014E RID: 334
		private const int _missionTimeSpeedRequestID = 864;

		// Token: 0x0400014F RID: 335
		private float _holdTime;

		// Token: 0x04000150 RID: 336
		private bool _holdExecuted;

		// Token: 0x04000151 RID: 337
		private DeploymentMissionView _deploymentMissionView;

		// Token: 0x04000152 RID: 338
		private List<DeploymentSiegeMachineVM> _deploymentPointDataSources;

		// Token: 0x04000153 RID: 339
		private OrderTroopPlacer _orderTroopPlacer;

		// Token: 0x04000154 RID: 340
		private GauntletLayer _gauntletLayer;

		// Token: 0x04000155 RID: 341
		private IGauntletMovie _movie;

		// Token: 0x04000156 RID: 342
		private SpriteCategory _spriteCategory;

		// Token: 0x04000157 RID: 343
		private MissionOrderVM _dataSource;

		// Token: 0x04000158 RID: 344
		private SiegeDeploymentHandler _siegeDeploymentHandler;

		// Token: 0x04000159 RID: 345
		private BattleDeploymentHandler _battleDeploymentHandler;

		// Token: 0x0400015C RID: 348
		private bool _isReceivingInput;

		// Token: 0x0400015D RID: 349
		private bool _isInitialized;

		// Token: 0x0400015E RID: 350
		private bool _slowedDownMission;

		// Token: 0x0400015F RID: 351
		private float _latestDt;

		// Token: 0x04000161 RID: 353
		private bool _isTransferEnabled;
	}
}
