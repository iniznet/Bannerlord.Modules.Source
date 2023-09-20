using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Missions.Handlers;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.MissionViews;
using TaleWorlds.MountAndBlade.View.MissionViews.Multiplayer;
using TaleWorlds.MountAndBlade.View.MissionViews.Order;
using TaleWorlds.MountAndBlade.View.Screens;
using TaleWorlds.MountAndBlade.ViewModelCollection.Order;
using TaleWorlds.ScreenSystem;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Mission.Multiplayer
{
	[OverrideView(typeof(MultiplayerMissionOrderUIHandler))]
	public class MissionGauntletMultiplayerOrderUIHandler : MissionView
	{
		private float _minHoldTimeForActivation
		{
			get
			{
				return 0f;
			}
		}

		public MissionGauntletMultiplayerOrderUIHandler()
		{
			this.ViewOrderPriority = 19;
		}

		public override void AfterStart()
		{
			base.AfterStart();
			int num;
			MultiplayerOptions.Instance.GetOptionFromOptionType(20, 0).GetValue(ref num);
			this._shouldTick = num > 0;
		}

		public override void OnMissionScreenTick(float dt)
		{
			base.OnMissionScreenTick(dt);
			this._latestDt = dt;
			if (this._shouldTick && (!base.MissionScreen.IsRadialMenuActive || this._dataSource.IsToggleOrderShown))
			{
				if (!this._isInitialized)
				{
					Team team = (GameNetwork.IsMyPeerReady ? PeerExtensions.GetComponent<MissionPeer>(GameNetwork.MyPeer).Team : null);
					if (team != null && (team == base.Mission.AttackerTeam || team == base.Mission.DefenderTeam))
					{
						this.InitializeInADisgustingManner();
					}
				}
				if (!this._isValid)
				{
					Team team2 = (GameNetwork.IsMyPeerReady ? PeerExtensions.GetComponent<MissionPeer>(GameNetwork.MyPeer).Team : null);
					if (team2 != null && (team2 == base.Mission.AttackerTeam || team2 == base.Mission.DefenderTeam))
					{
						this.ValidateInADisgustingManner();
					}
					return;
				}
				if (this._shouldInitializeFormationInfo)
				{
					Team team3 = (GameNetwork.IsMyPeerReady ? PeerExtensions.GetComponent<MissionPeer>(GameNetwork.MyPeer).Team : null);
					if (this._dataSource != null && team3 != null)
					{
						this._dataSource.AfterInitialize();
						this._shouldInitializeFormationInfo = false;
					}
				}
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
							base.MissionScreen.SetRadialMenuActiveState(false);
							if (this._orderTroopPlacer.SuspendTroopPlacer && this._dataSource.ActiveTargetState == 0)
							{
								this._orderTroopPlacer.SuspendTroopPlacer = false;
							}
						}
						else
						{
							base.MissionScreen.SetRadialMenuActiveState(true);
							if (!this._orderTroopPlacer.SuspendTroopPlacer)
							{
								this._orderTroopPlacer.SuspendTroopPlacer = true;
							}
						}
					}
				}
				else if (this._dataSource.TroopController.IsTransferActive)
				{
					this._gauntletLayer.InputRestrictions.SetInputRestrictions(true, 7);
				}
				else
				{
					if (!this._orderTroopPlacer.SuspendTroopPlacer)
					{
						this._orderTroopPlacer.SuspendTroopPlacer = true;
					}
					this._gauntletLayer.InputRestrictions.ResetInputRestrictions();
				}
				base.MissionScreen.OrderFlag.IsTroop = this._dataSource.ActiveTargetState == 0;
				this.TickOrderFlag(dt, false);
			}
		}

		public override bool OnEscape()
		{
			if (this._dataSource != null)
			{
				this._dataSource.OnEscape();
				return this._dataSource.IsToggleOrderShown;
			}
			return false;
		}

		public override void OnMissionScreenActivate()
		{
			base.OnMissionScreenActivate();
			if (this._dataSource != null)
			{
				this._dataSource.AfterInitialize();
				this._isInitialized = true;
			}
		}

		public override void OnAgentBuild(Agent agent, Banner banner)
		{
			if (!this._isValid)
			{
				return;
			}
			if (agent.IsHuman)
			{
				MissionOrderVM dataSource = this._dataSource;
				if (dataSource == null)
				{
					return;
				}
				dataSource.TroopController.AddTroops(agent);
			}
		}

		public override void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow killingBlow)
		{
			base.OnAgentRemoved(affectedAgent, affectorAgent, agentState, killingBlow);
			if (affectedAgent.IsHuman)
			{
				MissionOrderVM dataSource = this._dataSource;
				if (dataSource == null)
				{
					return;
				}
				dataSource.TroopController.RemoveTroops(affectedAgent);
			}
		}

		public override void OnMissionScreenInitialize()
		{
			base.OnMissionScreenInitialize();
			base.MissionScreen.SceneLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("MissionOrderHotkeyCategory"));
			this._siegeDeploymentHandler = null;
			this.IsDeployment = false;
			ManagedOptions.OnManagedOptionChanged = (ManagedOptions.OnManagedOptionChangedDelegate)Delegate.Combine(ManagedOptions.OnManagedOptionChanged, new ManagedOptions.OnManagedOptionChangedDelegate(this.OnManagedOptionChanged));
			MissionMultiplayerGameModeBaseClient missionBehavior = base.Mission.GetMissionBehavior<MissionMultiplayerGameModeBaseClient>();
			this._roundComponent = ((missionBehavior != null) ? missionBehavior.RoundComponent : null);
			if (this._roundComponent != null)
			{
				this._roundComponent.OnRoundStarted += this.OnRoundStarted;
				this._roundComponent.OnPreparationEnded += this.OnPreparationEnded;
			}
		}

		private void OnRoundStarted()
		{
			MissionOrderVM dataSource = this._dataSource;
			if (dataSource == null)
			{
				return;
			}
			dataSource.AfterInitialize();
		}

		private void OnPreparationEnded()
		{
			this._shouldInitializeFormationInfo = true;
		}

		private void OnManagedOptionChanged(ManagedOptions.ManagedOptionsType changedManagedOptionsType)
		{
			if (changedManagedOptionsType == 31)
			{
				if (this._gauntletLayer != null && this._viewMovie != null)
				{
					this._gauntletLayer.ReleaseMovie(this._viewMovie);
					string text = ((BannerlordConfig.OrderType == 0) ? "OrderBar" : "OrderRadial");
					this._viewMovie = this._gauntletLayer.LoadMovie(text, this._dataSource);
					return;
				}
			}
			else if (changedManagedOptionsType == 32)
			{
				MissionOrderVM dataSource = this._dataSource;
				if (dataSource == null)
				{
					return;
				}
				dataSource.OnOrderLayoutTypeChanged();
			}
		}

		public override void OnMissionScreenFinalize()
		{
			this.Clear();
			this._orderTroopPlacer = null;
			MissionPeer.OnTeamChanged -= new MissionPeer.OnTeamChangedDelegate(this.TeamChange);
			ManagedOptions.OnManagedOptionChanged = (ManagedOptions.OnManagedOptionChangedDelegate)Delegate.Remove(ManagedOptions.OnManagedOptionChanged, new ManagedOptions.OnManagedOptionChangedDelegate(this.OnManagedOptionChanged));
			if (this._roundComponent != null)
			{
				this._roundComponent.OnRoundStarted -= this.OnRoundStarted;
				this._roundComponent.OnPreparationEnded -= this.OnPreparationEnded;
			}
			base.OnMissionScreenFinalize();
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
		}

		private void SetLayerEnabled(bool isEnabled)
		{
			if (isEnabled)
			{
				if (this._dataSource == null || this._dataSource.ActiveTargetState == 0)
				{
					this._orderTroopPlacer.SuspendTroopPlacer = false;
				}
				base.MissionScreen.SetOrderFlagVisibility(true);
				if (this._gauntletLayer != null)
				{
					ScreenManager.SetSuspendLayer(this._gauntletLayer, false);
				}
				Game.Current.EventManager.TriggerEvent<MissionPlayerToggledOrderViewEvent>(new MissionPlayerToggledOrderViewEvent(true));
				return;
			}
			this._orderTroopPlacer.SuspendTroopPlacer = true;
			base.MissionScreen.SetOrderFlagVisibility(false);
			if (this._gauntletLayer != null)
			{
				ScreenManager.SetSuspendLayer(this._gauntletLayer, true);
			}
			base.MissionScreen.SetRadialMenuActiveState(false);
			Game.Current.EventManager.TriggerEvent<MissionPlayerToggledOrderViewEvent>(new MissionPlayerToggledOrderViewEvent(false));
		}

		public void InitializeInADisgustingManner()
		{
			base.AfterStart();
			base.MissionScreen.OrderFlag = new OrderFlag(base.Mission, base.MissionScreen);
			this._orderTroopPlacer = base.Mission.GetMissionBehavior<OrderTroopPlacer>();
			base.MissionScreen.SetOrderFlagVisibility(false);
			MissionPeer.OnTeamChanged += new MissionPeer.OnTeamChangedDelegate(this.TeamChange);
			this._isInitialized = true;
		}

		public void ValidateInADisgustingManner()
		{
			this._dataSource = new MissionOrderVM(base.MissionScreen.CombatCamera, this.IsDeployment ? this._siegeDeploymentHandler.PlayerDeploymentPoints.ToList<DeploymentPoint>() : new List<DeploymentPoint>(), new Action<bool>(this.ToggleScreenRotation), this.IsDeployment, new GetOrderFlagPositionDelegate(base.MissionScreen.GetOrderFlagPosition), new OnRefreshVisualsDelegate(this.RefreshVisuals), new ToggleOrderPositionVisibilityDelegate(this.SetSuspendTroopPlacer), new OnToggleActivateOrderStateDelegate(this.OnActivateToggleOrder), new OnToggleActivateOrderStateDelegate(this.OnDeactivateToggleOrder), new OnToggleActivateOrderStateDelegate(this.OnTransferFinished), new OnBeforeOrderDelegate(this.OnBeforeOrder), true);
			this._gauntletLayer = new GauntletLayer(this.ViewOrderPriority, "GauntletLayer", false);
			SpriteData spriteData = UIResourceManager.SpriteData;
			TwoDimensionEngineResourceContext resourceContext = UIResourceManager.ResourceContext;
			ResourceDepot uiresourceDepot = UIResourceManager.UIResourceDepot;
			this._spriteCategory = spriteData.SpriteCategories["ui_order"];
			this._spriteCategory.Load(resourceContext, uiresourceDepot);
			string text = ((BannerlordConfig.OrderType == 0) ? "OrderBar" : "OrderRadial");
			this._viewMovie = this._gauntletLayer.LoadMovie(text, this._dataSource);
			this._dataSource.InputRestrictions = this._gauntletLayer.InputRestrictions;
			base.MissionScreen.AddLayer(this._gauntletLayer);
			this._dataSource.AfterInitialize();
			this._isValid = true;
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

		private void RefreshVisuals()
		{
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

		private void ToggleScreenRotation(bool isLocked)
		{
			MissionScreen.SetFixedMissionCameraActive(isLocked);
		}

		private void Clear()
		{
			if (this._gauntletLayer != null)
			{
				base.MissionScreen.RemoveLayer(this._gauntletLayer);
			}
			if (this._dataSource != null)
			{
				this._dataSource.OnFinalize();
			}
			this._gauntletLayer = null;
			this._dataSource = null;
			this._viewMovie = null;
			if (this._isValid)
			{
				this._spriteCategory.Unload();
			}
		}

		private void TeamChange(NetworkCommunicator peer, Team previousTeam, Team newTeam)
		{
			if (peer.IsMine)
			{
				this.Clear();
				this._isValid = false;
			}
		}

		[Conditional("DEBUG")]
		private void TickInputDebug()
		{
			if (this._dataSource.IsToggleOrderShown && base.DebugInput.IsKeyPressed(87) && !base.DebugInput.IsKeyPressed(29) && !base.DebugInput.IsKeyPressed(157))
			{
				OrderType orderType = OrderController.GetActiveArrangementOrderOf(this._dataSource.OrderController.SelectedFormations[0]);
				OrderType orderType2 = 17;
				OrderType orderType3 = 24;
				orderType++;
				if (orderType > orderType3)
				{
					orderType = orderType2;
				}
				this._dataSource.OrderController.SetOrder(orderType);
				MBTextManager.SetTextVariable("ARRANGEMENT_ORDER", orderType.ToString(), false);
				InformationManager.DisplayMessage(new InformationMessage(new TextObject("{=RGzyUTzm}Formation arrangement switching to {ARRANGEMENT_ORDER}", null).ToString()));
			}
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
			if (this._dataSource.IsToggleOrderShown)
			{
				if (this._dataSource.TroopController.IsTransferActive && this._gauntletLayer.Input.IsHotKeyPressed("Exit"))
				{
					this._dataSource.TroopController.IsTransferActive = false;
				}
				if (this._dataSource.TroopController.IsTransferActive != this._isTransferEnabled)
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
							if (focusedOrderableObject != null)
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
							Debug.FailedAssert("false", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.GauntletUI\\Mission\\Multiplayer\\MissionGauntletMultiplayerOrderUIHandler.cs", "TickInput", 578);
							break;
						}
					}
				}
				if (base.Input.IsKeyReleased(225))
				{
					this._dataSource.OnEscape();
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

		private float _holdTime;

		private bool _holdExecuted;

		private OrderTroopPlacer _orderTroopPlacer;

		private GauntletLayer _gauntletLayer;

		private MissionOrderVM _dataSource;

		private IGauntletMovie _viewMovie;

		private IRoundComponent _roundComponent;

		private SpriteCategory _spriteCategory;

		private SiegeDeploymentHandler _siegeDeploymentHandler;

		private bool _isValid;

		private bool _isInitialized;

		private bool IsDeployment;

		private bool _shouldTick;

		private bool _shouldInitializeFormationInfo;

		private float _latestDt;

		private bool _isTransferEnabled;
	}
}
