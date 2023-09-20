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
	// Token: 0x02000032 RID: 50
	[OverrideView(typeof(MissionOrderOfBattleUIHandler))]
	public class MissionGauntletOrderOfBattleUIHandler : MissionView
	{
		// Token: 0x06000258 RID: 600 RVA: 0x0000CE4C File Offset: 0x0000B04C
		public MissionGauntletOrderOfBattleUIHandler(OrderOfBattleVM dataSource)
		{
			this._dataSource = dataSource;
			this.ViewOrderPriority = 13;
		}

		// Token: 0x06000259 RID: 601 RVA: 0x0000CE64 File Offset: 0x0000B064
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

		// Token: 0x0600025A RID: 602 RVA: 0x0000CFF3 File Offset: 0x0000B1F3
		public override bool IsReady()
		{
			return this._isDeploymentFinished || this._orderOfBattleCategory.IsLoaded;
		}

		// Token: 0x0600025B RID: 603 RVA: 0x0000D00A File Offset: 0x0000B20A
		public override void OnMissionTick(float dt)
		{
			base.OnMissionTick(dt);
			if (this._isActive)
			{
				this.TickInput();
				this._dataSource.Tick();
			}
		}

		// Token: 0x0600025C RID: 604 RVA: 0x0000D02C File Offset: 0x0000B22C
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

		// Token: 0x0600025D RID: 605 RVA: 0x0000D0E8 File Offset: 0x0000B2E8
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

		// Token: 0x0600025E RID: 606 RVA: 0x0000D178 File Offset: 0x0000B378
		public override void OnMissionScreenFinalize()
		{
			this._dataSource.OnFinalize();
			this._dataSource = null;
			this._gauntletLayer.ReleaseMovie(this._movie);
			base.MissionScreen.RemoveLayer(this._gauntletLayer);
			this._orderOfBattleCategory.Unload();
			base.OnMissionScreenFinalize();
		}

		// Token: 0x0600025F RID: 607 RVA: 0x0000D1CC File Offset: 0x0000B3CC
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

		// Token: 0x06000260 RID: 608 RVA: 0x0000D20C File Offset: 0x0000B40C
		public override void OnPhotoModeActivated()
		{
			base.OnPhotoModeActivated();
			this._gauntletLayer._gauntletUIContext.ContextAlpha = 0f;
		}

		// Token: 0x06000261 RID: 609 RVA: 0x0000D229 File Offset: 0x0000B429
		public override void OnPhotoModeDeactivated()
		{
			base.OnPhotoModeDeactivated();
			this._gauntletLayer._gauntletUIContext.ContextAlpha = 1f;
		}

		// Token: 0x06000262 RID: 610 RVA: 0x0000D246 File Offset: 0x0000B446
		public override bool IsOpeningEscapeMenuOnFocusChangeAllowed()
		{
			return !this._isActive;
		}

		// Token: 0x06000263 RID: 611 RVA: 0x0000D254 File Offset: 0x0000B454
		private void OnPlayerTurnToChooseFormationToLead(Dictionary<int, Agent> lockedFormationIndicesAndSergeants, List<int> remainingFormationIndices)
		{
			this._cachedOrderTypeSetting = ManagedOptions.GetConfig(31);
			ManagedOptions.SetConfig(31, 1f);
			this._dataSource.Initialize(base.Mission, base.MissionScreen.CombatCamera, new Action<int>(this.SelectFormationAtIndex), new Action<int>(this.DeselectFormationAtIndex), new Action(this.ClearFormationSelection), new Action(this.OnAutoDeploy), new Action(this.OnBeginMission), lockedFormationIndicesAndSergeants);
			this._orderUIHandler.SetIsOrderPreconfigured(this._dataSource.IsOrderPreconfigured);
			this._gauntletLayer.InputRestrictions.SetInputRestrictions(true, 7);
			this._isActive = true;
		}

		// Token: 0x06000264 RID: 612 RVA: 0x0000D302 File Offset: 0x0000B502
		private void OnAllFormationsAssignedSergeants(Dictionary<int, Agent> formationsWithLooselyAssignedSergeants)
		{
			this._dataSource.OnAllFormationsAssignedSergeants(formationsWithLooselyAssignedSergeants);
		}

		// Token: 0x06000265 RID: 613 RVA: 0x0000D310 File Offset: 0x0000B510
		private void OnDeploymentFinish()
		{
			bool flag = MissionGameModels.Current.BattleInitializationModel.CanPlayerSideDeployWithOrderOfBattle();
			this._dataSource.OnDeploymentFinalized(flag);
			if (this._isActive)
			{
				ManagedOptions.SetConfig(31, this._cachedOrderTypeSetting);
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

		// Token: 0x06000266 RID: 614 RVA: 0x0000D421 File Offset: 0x0000B621
		private void OnCameraControlsToggled(bool isEnabled)
		{
			this._dataSource.AreCameraControlsEnabled = isEnabled;
		}

		// Token: 0x06000267 RID: 615 RVA: 0x0000D42F File Offset: 0x0000B62F
		private void SelectFormationAtIndex(int index)
		{
			MissionGauntletSingleplayerOrderUIHandler orderUIHandler = this._orderUIHandler;
			if (orderUIHandler == null)
			{
				return;
			}
			orderUIHandler.SelectFormationAtIndex(index);
		}

		// Token: 0x06000268 RID: 616 RVA: 0x0000D442 File Offset: 0x0000B642
		private void DeselectFormationAtIndex(int index)
		{
			MissionGauntletSingleplayerOrderUIHandler orderUIHandler = this._orderUIHandler;
			if (orderUIHandler == null)
			{
				return;
			}
			orderUIHandler.DeselectFormationAtIndex(index);
		}

		// Token: 0x06000269 RID: 617 RVA: 0x0000D455 File Offset: 0x0000B655
		private void ClearFormationSelection()
		{
			MissionGauntletSingleplayerOrderUIHandler orderUIHandler = this._orderUIHandler;
			if (orderUIHandler == null)
			{
				return;
			}
			orderUIHandler.ClearFormationSelection();
		}

		// Token: 0x0600026A RID: 618 RVA: 0x0000D467 File Offset: 0x0000B667
		private void OnAutoDeploy()
		{
			this._orderUIHandler.OnAutoDeploy();
		}

		// Token: 0x0600026B RID: 619 RVA: 0x0000D474 File Offset: 0x0000B674
		private void OnBeginMission()
		{
			this._orderUIHandler.OnBeginMission();
			this._orderUIHandler.OnFiltersSet(this._dataSource.CurrentConfiguration);
		}

		// Token: 0x0600026C RID: 620 RVA: 0x0000D497 File Offset: 0x0000B697
		private void OnUnitDeployed()
		{
			this._dataSource.OnUnitDeployed();
		}

		// Token: 0x04000128 RID: 296
		private OrderOfBattleVM _dataSource;

		// Token: 0x04000129 RID: 297
		private GauntletLayer _gauntletLayer;

		// Token: 0x0400012A RID: 298
		private IGauntletMovie _movie;

		// Token: 0x0400012B RID: 299
		private SpriteCategory _orderOfBattleCategory;

		// Token: 0x0400012C RID: 300
		private DeploymentMissionView _deploymentMissionView;

		// Token: 0x0400012D RID: 301
		private MissionGauntletSingleplayerOrderUIHandler _orderUIHandler;

		// Token: 0x0400012E RID: 302
		private AssignPlayerRoleInTeamMissionController _playerRoleMissionController;

		// Token: 0x0400012F RID: 303
		private OrderTroopPlacer _orderTroopPlacer;

		// Token: 0x04000130 RID: 304
		private bool _isActive;

		// Token: 0x04000131 RID: 305
		private bool _isDeploymentFinished;

		// Token: 0x04000132 RID: 306
		private float _cachedOrderTypeSetting;
	}
}
