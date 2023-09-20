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
	[OverrideView(typeof(MissionOrderUIHandler))]
	public class MissionGauntletSingleplayerOrderUIHandler : MissionView, ISiegeDeploymentView
	{
		private float _minHoldTimeForActivation
		{
			get
			{
				return 0f;
			}
		}

		public bool IsSiegeDeployment { get; private set; }

		public bool IsBattleDeployment { get; private set; }

		private bool _isAnyDeployment
		{
			get
			{
				return this.IsSiegeDeployment || this.IsBattleDeployment;
			}
		}

		public bool IsOrderMenuActive
		{
			get
			{
				MissionOrderVM dataSource = this._dataSource;
				return dataSource != null && dataSource.IsToggleOrderShown;
			}
		}

		public event Action<bool> OnCameraControlsToggled;

		public MissionGauntletSingleplayerOrderUIHandler()
		{
			this.ViewOrderPriority = 14;
		}

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
					if (this._targetFormationOrderGivenWithActionButton)
					{
						this.SetSuspendTroopPlacer(false);
						this._targetFormationOrderGivenWithActionButton = false;
					}
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
						flag = this._dataSource.IsHolding || base.Mission.Mode == 6;
						goto IL_348;
					}
				}
				flag = false;
				IL_348:
				bool flag2 = flag;
				if (flag2 != base.MissionScreen.IsRadialMenuActive)
				{
					base.MissionScreen.SetRadialMenuActiveState(flag2);
				}
			}
			this._targetFormationOrderGivenWithActionButton = false;
			this._dataSource.CanUseShortcuts = this._isReceivingInput;
		}

		public override bool OnEscape()
		{
			bool isToggleOrderShown = this._dataSource.IsToggleOrderShown;
			this._dataSource.OnEscape();
			return isToggleOrderShown;
		}

		public override void OnMissionScreenActivate()
		{
			base.OnMissionScreenActivate();
			this._dataSource.AfterInitialize();
			this._isInitialized = true;
		}

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

		public override void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow killingBlow)
		{
			base.OnAgentRemoved(affectedAgent, affectorAgent, agentState, killingBlow);
			if (affectedAgent.IsHuman)
			{
				this._dataSource.TroopController.RemoveTroops(affectedAgent);
			}
		}

		public override void OnMissionScreenInitialize()
		{
			base.OnMissionScreenInitialize();
			base.MissionScreen.SceneLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("MissionOrderHotkeyCategory"));
			base.MissionScreen.OrderFlag = new OrderFlag(base.Mission, base.MissionScreen);
			this._orderTroopPlacer = base.Mission.GetMissionBehavior<OrderTroopPlacer>();
			base.MissionScreen.SetOrderFlagVisibility(false);
			this._siegeDeploymentHandler = base.Mission.GetMissionBehavior<SiegeDeploymentHandler>();
			this._battleDeploymentHandler = base.Mission.GetMissionBehavior<BattleDeploymentHandler>();
			this._formationTargetHandler = base.Mission.GetMissionBehavior<MissionFormationTargetSelectionHandler>();
			if (this._formationTargetHandler != null)
			{
				this._formationTargetHandler.OnFormationFocused += this.OnFormationFocused;
			}
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
			this._dataSource.TroopController.SetDoneInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Confirm"));
			this._dataSource.TroopController.SetCancelInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Exit"));
			this._dataSource.TroopController.SetResetInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Reset"));
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
			if (!this._isAnyDeployment && BannerlordConfig.HideBattleUI)
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

		public override bool IsReady()
		{
			return this._spriteCategory.IsCategoryFullyLoaded();
		}

		private void OnManagedOptionChanged(ManagedOptions.ManagedOptionsType changedManagedOptionsType)
		{
			if (changedManagedOptionsType == 33)
			{
				this._gauntletLayer.ReleaseMovie(this._movie);
				string text = ((BannerlordConfig.OrderType == 0) ? "OrderBar" : "OrderRadial");
				this._movie = this._gauntletLayer.LoadMovie(text, this._dataSource);
				return;
			}
			if (changedManagedOptionsType != 34)
			{
				if (changedManagedOptionsType == 43)
				{
					if (!this._isAnyDeployment)
					{
						this._gauntletLayer._gauntletUIContext.ContextAlpha = (BannerlordConfig.HideBattleUI ? 0f : 1f);
						return;
					}
				}
				else if (changedManagedOptionsType == 37 && !BannerlordConfig.SlowDownOnOrder && this._slowedDownMission)
				{
					base.Mission.RemoveTimeSpeedRequest(864);
				}
				return;
			}
			MissionOrderVM dataSource = this._dataSource;
			if (dataSource == null)
			{
				return;
			}
			dataSource.OnOrderLayoutTypeChanged();
		}

		public override void OnMissionScreenFinalize()
		{
			base.OnMissionScreenFinalize();
			ManagedOptions.OnManagedOptionChanged = (ManagedOptions.OnManagedOptionChangedDelegate)Delegate.Remove(ManagedOptions.OnManagedOptionChanged, new ManagedOptions.OnManagedOptionChangedDelegate(this.OnManagedOptionChanged));
			if (this._formationTargetHandler != null)
			{
				this._formationTargetHandler.OnFormationFocused -= this.OnFormationFocused;
			}
			this._deploymentPointDataSources = null;
			this._orderTroopPlacer = null;
			this._movie = null;
			this._gauntletLayer = null;
			this._dataSource.OnFinalize();
			this._dataSource = null;
			this._siegeDeploymentHandler = null;
			this._spriteCategory.Unload();
			this._battleDeploymentHandler = null;
			this._formationTargetHandler = null;
		}

		public OrderSetVM GetLastSelectedOrderSet()
		{
			return this._dataSource.LastSelectedOrderSet;
		}

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

		public void OnActivateToggleOrder()
		{
			this.SetLayerEnabled(true);
		}

		public void OnDeactivateToggleOrder()
		{
			if (!this._dataSource.TroopController.IsTransferActive)
			{
				this.SetLayerEnabled(false);
			}
		}

		private void OnTransferFinished()
		{
			if (!this._isAnyDeployment)
			{
				this.SetLayerEnabled(false);
			}
		}

		public void OnAutoDeploy()
		{
			this._dataSource.DeploymentController.ExecuteAutoDeploy();
		}

		public void OnBeginMission()
		{
			this._dataSource.DeploymentController.ExecuteBeginSiege();
		}

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
				this.SetSuspendTroopPlacer(true);
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

		private void OnDeploymentFinish()
		{
			this.IsSiegeDeployment = false;
			this.IsBattleDeployment = false;
			this._dataSource.OnDeploymentFinished();
			this._deploymentPointDataSources.Clear();
			this.SetSuspendTroopPlacer(true);
			this._gauntletLayer.IsFocusLayer = false;
			ScreenManager.TryLoseFocus(this._gauntletLayer);
			base.MissionScreen.SetRadialMenuActiveState(false);
			this._gauntletLayer._gauntletUIContext.ContextAlpha = (BannerlordConfig.HideBattleUI ? 0f : 1f);
			if (this._deploymentMissionView != null)
			{
				DeploymentMissionView deploymentMissionView = this._deploymentMissionView;
				deploymentMissionView.OnDeploymentFinish = (OnPlayerDeploymentFinishDelegate)Delegate.Remove(deploymentMissionView.OnDeploymentFinish, new OnPlayerDeploymentFinishDelegate(this.OnDeploymentFinish));
			}
		}

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

		private IOrderable GetFocusedOrderableObject()
		{
			return base.MissionScreen.OrderFlag.FocusedOrderableObject;
		}

		private void SetSuspendTroopPlacer(bool value)
		{
			this._orderTroopPlacer.SuspendTroopPlacer = value;
			base.MissionScreen.SetOrderFlagVisibility(!value);
		}

		public void SelectFormationAtIndex(int index)
		{
			this._dataSource.OnSelect(index);
		}

		public void DeselectFormationAtIndex(int index)
		{
			this._dataSource.TroopController.OnDeselectFormation(index);
		}

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

		public void OnFiltersSet(List<ValueTuple<int, List<int>>> filterData)
		{
			this._dataSource.OnFiltersSet(filterData);
		}

		public void SetIsOrderPreconfigured(bool isOrderPreconfigured)
		{
			this._dataSource.DeploymentController.SetIsOrderPreconfigured(isOrderPreconfigured);
		}

		private void OnBeforeOrder()
		{
			this.TickOrderFlag(this._latestDt, true);
		}

		private void TickOrderFlag(float dt, bool forceUpdate)
		{
			if ((base.MissionScreen.OrderFlag.IsVisible || forceUpdate) && Utilities.EngineFrameNo != base.MissionScreen.OrderFlag.LatestUpdateFrameNo)
			{
				base.MissionScreen.OrderFlag.Tick(this._latestDt);
			}
		}

		private void OnFormationFocused(MBReadOnlyList<Formation> focusedFormations)
		{
			this._focusedFormationsCache = focusedFormations;
			this._dataSource.SetFocusedFormations(this._focusedFormationsCache);
		}

		void ISiegeDeploymentView.OnEntityHover(GameEntity hoveredEntity)
		{
			if (!this._gauntletLayer.IsHitThisFrame)
			{
				this._dataSource.DeploymentController.OnEntityHover(hoveredEntity);
			}
		}

		void ISiegeDeploymentView.OnEntitySelection(GameEntity selectedEntity)
		{
			this._dataSource.DeploymentController.OnEntitySelect(selectedEntity);
		}

		private void ToggleScreenRotation(bool isLocked)
		{
			MissionScreen.SetFixedMissionCameraActive(isLocked);
		}

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
						this._dataSource.OpenToggleOrder(true, !this._dataSource.IsHolding);
						this._dataSource.IsHolding = true;
					}
				}
				else if (!base.Input.IsGameKeyDown(86))
				{
					if (this._dataSource.IsHolding && this._dataSource.IsToggleOrderShown)
					{
						this._dataSource.TryCloseToggleOrder(false);
					}
					this._dataSource.IsHolding = false;
					this._holdTime = 0f;
				}
			}
			if (this._dataSource.IsToggleOrderShown)
			{
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
							MBReadOnlyList<Formation> focusedFormationsCache = this._focusedFormationsCache;
							if (focusedFormationsCache != null && focusedFormationsCache.Count > 0)
							{
								this._dataSource.OrderController.SetOrderWithFormation(4, this._focusedFormationsCache[0]);
								this.SetSuspendTroopPlacer(true);
								this._targetFormationOrderGivenWithActionButton = true;
							}
							else
							{
								IOrderable focusedOrderableObject = this.GetFocusedOrderableObject();
								if (focusedOrderableObject != null && OrderFlag.IsOrderPositionValid(ModuleExtensions.ToWorldPosition(base.MissionScreen.GetOrderFlagPosition())))
								{
									this._dataSource.OrderController.SetOrderWithOrderableObject(focusedOrderableObject);
								}
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
							Debug.FailedAssert("false", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.GauntletUI\\Mission\\Singleplayer\\MissionGauntletSingleplayerOrderUIHandler.cs", "TickInput", 686);
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
					this._gauntletLayer._gauntletUIContext.ContextAlpha = (BannerlordConfig.HideBattleUI ? 0f : 1f);
					this._gauntletLayer.IsFocusLayer = false;
					ScreenManager.TryLoseFocus(this._gauntletLayer);
				}
				else
				{
					this._gauntletLayer._gauntletUIContext.ContextAlpha = 1f;
					this._gauntletLayer.IsFocusLayer = true;
					ScreenManager.TrySetFocus(this._gauntletLayer);
				}
			}
			else if (this._dataSource.TroopController.IsTransferActive)
			{
				if (this._gauntletLayer.Input.IsHotKeyReleased("Exit"))
				{
					UISoundsHelper.PlayUISound("event:/ui/default");
					this._dataSource.TroopController.ExecuteCancelTransfer();
				}
				else if (this._gauntletLayer.Input.IsHotKeyReleased("Confirm"))
				{
					UISoundsHelper.PlayUISound("event:/ui/default");
					this._dataSource.TroopController.ExecuteConfirmTransfer();
				}
				else if (this._gauntletLayer.Input.IsHotKeyReleased("Reset"))
				{
					UISoundsHelper.PlayUISound("event:/ui/default");
					this._dataSource.TroopController.ExecuteReset();
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

		private const string _radialOrderMovieName = "OrderRadial";

		private const string _barOrderMovieName = "OrderBar";

		private const float _slowDownAmountWhileOrderIsOpen = 0.25f;

		private const int _missionTimeSpeedRequestID = 864;

		private float _holdTime;

		private DeploymentMissionView _deploymentMissionView;

		private List<DeploymentSiegeMachineVM> _deploymentPointDataSources;

		private OrderTroopPlacer _orderTroopPlacer;

		private GauntletLayer _gauntletLayer;

		private IGauntletMovie _movie;

		private SpriteCategory _spriteCategory;

		private MissionOrderVM _dataSource;

		private SiegeDeploymentHandler _siegeDeploymentHandler;

		private BattleDeploymentHandler _battleDeploymentHandler;

		private MissionFormationTargetSelectionHandler _formationTargetHandler;

		private MBReadOnlyList<Formation> _focusedFormationsCache;

		private bool _isReceivingInput;

		private bool _isInitialized;

		private bool _slowedDownMission;

		private float _latestDt;

		private bool _targetFormationOrderGivenWithActionButton;

		private bool _isTransferEnabled;
	}
}
