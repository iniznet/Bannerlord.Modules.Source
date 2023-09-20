using System;
using System.Collections.Generic;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.MissionViews;
using TaleWorlds.MountAndBlade.View.MissionViews.Order;
using TaleWorlds.MountAndBlade.View.MissionViews.Singleplayer;
using TaleWorlds.MountAndBlade.ViewModelCollection.OrderOfBattle;
using TaleWorlds.ScreenSystem;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Mission.Singleplayer
{
	[OverrideView(typeof(MissionOrderOfBattleUIHandler))]
	public class MissionGauntletOrderOfBattleUIHandler : MissionView
	{
		public MissionGauntletOrderOfBattleUIHandler(OrderOfBattleVM dataSource)
		{
			this._dataSource = dataSource;
			this.ViewOrderPriority = 13;
		}

		public override void OnMissionScreenInitialize()
		{
			base.OnMissionScreenInitialize();
			this._deploymentMissionView = base.Mission.GetMissionBehavior<DeploymentMissionView>();
			DeploymentMissionView deploymentMissionView = this._deploymentMissionView;
			deploymentMissionView.OnDeploymentFinish = (OnPlayerDeploymentFinishDelegate)Delegate.Combine(deploymentMissionView.OnDeploymentFinish, new OnPlayerDeploymentFinishDelegate(this.OnDeploymentFinish));
			this._playerRoleMissionController = base.Mission.GetMissionBehavior<AssignPlayerRoleInTeamMissionController>();
			this._playerRoleMissionController.OnPlayerTurnToChooseFormationToLead += new PlayerTurnToChooseFormationToLeadEvent(this.OnPlayerTurnToChooseFormationToLead);
			this._playerRoleMissionController.OnAllFormationsAssignedSergeants += new AllFormationsAssignedSergeantsEvent(this.OnAllFormationsAssignedSergeants);
			this._orderUIHandler = base.Mission.GetMissionBehavior<MissionGauntletSingleplayerOrderUIHandler>();
			this._orderUIHandler.OnCameraControlsToggled += this.OnCameraControlsToggled;
			this._orderTroopPlacer = base.Mission.GetMissionBehavior<OrderTroopPlacer>();
			OrderTroopPlacer orderTroopPlacer = this._orderTroopPlacer;
			orderTroopPlacer.OnUnitDeployed = (Action)Delegate.Combine(orderTroopPlacer.OnUnitDeployed, new Action(this.OnUnitDeployed));
			this._gauntletLayer = new GauntletLayer(this.ViewOrderPriority, "GauntletLayer", false);
			this._movie = this._gauntletLayer.LoadMovie("OrderOfBattle", this._dataSource);
			SpriteData spriteData = UIResourceManager.SpriteData;
			TwoDimensionEngineResourceContext resourceContext = UIResourceManager.ResourceContext;
			ResourceDepot uiresourceDepot = UIResourceManager.UIResourceDepot;
			this._orderOfBattleCategory = spriteData.SpriteCategories["ui_order_of_battle"];
			this._orderOfBattleCategory.Load(resourceContext, uiresourceDepot);
			base.MissionScreen.AddLayer(this._gauntletLayer);
			this._gauntletLayer.InputRestrictions.SetInputRestrictions(false, 7);
			this._gauntletLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericPanelGameKeyCategory"));
		}

		public override bool IsReady()
		{
			return this._isDeploymentFinished || this._orderOfBattleCategory.IsLoaded;
		}

		public override void OnMissionTick(float dt)
		{
			base.OnMissionTick(dt);
			if (this._isActive)
			{
				this.TickInput();
				this._dataSource.Tick();
			}
		}

		private void TickInput()
		{
			bool flag;
			bool flag2;
			bool flag3;
			this.HandleLayerFocus(out flag, out flag2, out flag3);
			if (this._gauntletLayer.Input.IsHotKeyReleased("Exit"))
			{
				if (flag2)
				{
					this._dataSource.ExecuteDisableAllClassSelections();
				}
				else if (flag3)
				{
					this._dataSource.ExecuteDisableAllFilterSelections();
				}
				else if (flag)
				{
					this._dataSource.ExecuteClearHeroSelection();
				}
			}
			if (base.MissionScreen.SceneLayer.Input.IsKeyDown(225) || base.MissionScreen.SceneLayer.Input.IsKeyDown(254))
			{
				this._gauntletLayer.InputRestrictions.SetMouseVisibility(false);
				return;
			}
			this._gauntletLayer.InputRestrictions.SetInputRestrictions(true, 7);
		}

		private void HandleLayerFocus(out bool isAnyHeroSelected, out bool isClassSelectionEnabled, out bool isFilterSelectionEnabled)
		{
			isAnyHeroSelected = this._dataSource.HasSelectedHeroes;
			isClassSelectionEnabled = this._dataSource.IsAnyClassSelectionEnabled();
			isFilterSelectionEnabled = this._dataSource.IsAnyFilterSelectionEnabled();
			bool flag = isAnyHeroSelected | isClassSelectionEnabled | isFilterSelectionEnabled;
			if (this._gauntletLayer.IsFocusLayer && !flag)
			{
				this._gauntletLayer.IsFocusLayer = false;
				ScreenManager.TryLoseFocus(this._gauntletLayer);
				return;
			}
			if (!this._gauntletLayer.IsFocusLayer && flag)
			{
				this._gauntletLayer.IsFocusLayer = true;
				ScreenManager.TrySetFocus(this._gauntletLayer);
			}
		}

		public override void OnMissionScreenFinalize()
		{
			this._dataSource.OnFinalize();
			this._dataSource = null;
			this._gauntletLayer.ReleaseMovie(this._movie);
			base.MissionScreen.RemoveLayer(this._gauntletLayer);
			this._gauntletLayer = null;
			this._orderOfBattleCategory.Unload();
			base.OnMissionScreenFinalize();
		}

		public override bool OnEscape()
		{
			bool flag = false;
			if (this._orderUIHandler != null && this._orderUIHandler.IsOrderMenuActive)
			{
				flag = this._orderUIHandler.OnEscape();
			}
			if (!flag)
			{
				flag = this._dataSource.OnEscape();
			}
			return flag;
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

		public override bool IsOpeningEscapeMenuOnFocusChangeAllowed()
		{
			return !this._isActive;
		}

		private void OnPlayerTurnToChooseFormationToLead(Dictionary<int, Agent> lockedFormationIndicesAndSergeants, List<int> remainingFormationIndices)
		{
			this._cachedOrderTypeSetting = ManagedOptions.GetConfig(33);
			ManagedOptions.SetConfig(33, 1f);
			this._dataSource.Initialize(base.Mission, base.MissionScreen.CombatCamera, new Action<int>(this.SelectFormationAtIndex), new Action<int>(this.DeselectFormationAtIndex), new Action(this.ClearFormationSelection), new Action(this.OnAutoDeploy), new Action(this.OnBeginMission), lockedFormationIndicesAndSergeants);
			this._orderUIHandler.SetIsOrderPreconfigured(this._dataSource.IsOrderPreconfigured);
			this._gauntletLayer.InputRestrictions.SetInputRestrictions(true, 7);
			this._isActive = true;
		}

		private void OnAllFormationsAssignedSergeants(Dictionary<int, Agent> formationsWithLooselyAssignedSergeants)
		{
			this._dataSource.OnAllFormationsAssignedSergeants(formationsWithLooselyAssignedSergeants);
		}

		private void OnDeploymentFinish()
		{
			bool flag = MissionGameModels.Current.BattleInitializationModel.CanPlayerSideDeployWithOrderOfBattle();
			this._dataSource.OnDeploymentFinalized(flag);
			if (this._isActive)
			{
				ManagedOptions.SetConfig(33, this._cachedOrderTypeSetting);
				this._isActive = false;
				this._gauntletLayer.InputRestrictions.ResetInputRestrictions();
			}
			this._isDeploymentFinished = true;
			DeploymentMissionView deploymentMissionView = this._deploymentMissionView;
			deploymentMissionView.OnDeploymentFinish = (OnPlayerDeploymentFinishDelegate)Delegate.Remove(deploymentMissionView.OnDeploymentFinish, new OnPlayerDeploymentFinishDelegate(this.OnDeploymentFinish));
			this._playerRoleMissionController.OnPlayerTurnToChooseFormationToLead -= new PlayerTurnToChooseFormationToLeadEvent(this.OnPlayerTurnToChooseFormationToLead);
			this._playerRoleMissionController.OnAllFormationsAssignedSergeants -= new AllFormationsAssignedSergeantsEvent(this.OnAllFormationsAssignedSergeants);
			OrderTroopPlacer orderTroopPlacer = this._orderTroopPlacer;
			orderTroopPlacer.OnUnitDeployed = (Action)Delegate.Remove(orderTroopPlacer.OnUnitDeployed, new Action(this.OnUnitDeployed));
			this._orderUIHandler.OnCameraControlsToggled -= this.OnCameraControlsToggled;
			this._orderOfBattleCategory.Unload();
			ScreenManager.TryLoseFocus(this._gauntletLayer);
			this._gauntletLayer.IsFocusLayer = false;
		}

		private void OnCameraControlsToggled(bool isEnabled)
		{
			this._dataSource.AreCameraControlsEnabled = isEnabled;
		}

		private void SelectFormationAtIndex(int index)
		{
			MissionGauntletSingleplayerOrderUIHandler orderUIHandler = this._orderUIHandler;
			if (orderUIHandler == null)
			{
				return;
			}
			orderUIHandler.SelectFormationAtIndex(index);
		}

		private void DeselectFormationAtIndex(int index)
		{
			MissionGauntletSingleplayerOrderUIHandler orderUIHandler = this._orderUIHandler;
			if (orderUIHandler == null)
			{
				return;
			}
			orderUIHandler.DeselectFormationAtIndex(index);
		}

		private void ClearFormationSelection()
		{
			MissionGauntletSingleplayerOrderUIHandler orderUIHandler = this._orderUIHandler;
			if (orderUIHandler == null)
			{
				return;
			}
			orderUIHandler.ClearFormationSelection();
		}

		private void OnAutoDeploy()
		{
			this._orderUIHandler.OnAutoDeploy();
		}

		private void OnBeginMission()
		{
			this._orderUIHandler.OnBeginMission();
			this._orderUIHandler.OnFiltersSet(this._dataSource.CurrentConfiguration);
		}

		private void OnUnitDeployed()
		{
			this._dataSource.OnUnitDeployed();
		}

		private OrderOfBattleVM _dataSource;

		private GauntletLayer _gauntletLayer;

		private IGauntletMovie _movie;

		private SpriteCategory _orderOfBattleCategory;

		private DeploymentMissionView _deploymentMissionView;

		private MissionGauntletSingleplayerOrderUIHandler _orderUIHandler;

		private AssignPlayerRoleInTeamMissionController _playerRoleMissionController;

		private OrderTroopPlacer _orderTroopPlacer;

		private bool _isActive;

		private bool _isDeploymentFinished;

		private float _cachedOrderTypeSetting;
	}
}
