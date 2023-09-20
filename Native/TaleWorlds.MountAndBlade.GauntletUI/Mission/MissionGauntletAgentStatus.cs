using System;
using TaleWorlds.Core;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.MountAndBlade.Missions.Handlers;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.MissionViews;
using TaleWorlds.MountAndBlade.View.MissionViews.Singleplayer;
using TaleWorlds.MountAndBlade.ViewModelCollection;

namespace TaleWorlds.MountAndBlade.GauntletUI.Mission
{
	// Token: 0x02000022 RID: 34
	[OverrideView(typeof(MissionAgentStatusUIHandler))]
	public class MissionGauntletAgentStatus : MissionGauntletBattleUIBase
	{
		// Token: 0x0600018A RID: 394 RVA: 0x00008AF7 File Offset: 0x00006CF7
		public override void OnMissionStateActivated()
		{
			base.OnMissionStateActivated();
			MissionAgentStatusVM dataSource = this._dataSource;
			if (dataSource == null)
			{
				return;
			}
			dataSource.OnMainAgentWeaponChange();
		}

		// Token: 0x0600018B RID: 395 RVA: 0x00008B10 File Offset: 0x00006D10
		public override void EarlyStart()
		{
			base.EarlyStart();
			this._dataSource = new MissionAgentStatusVM(base.Mission, base.MissionScreen.CombatCamera, new Func<float>(base.MissionScreen.GetCameraToggleProgress));
			this._gauntletLayer = new GauntletLayer(this.ViewOrderPriority, "GauntletLayer", false);
			this._gauntletLayer.LoadMovie("MainAgentHUD", this._dataSource);
			base.MissionScreen.AddLayer(this._gauntletLayer);
			this._dataSource.TakenDamageController.SetIsEnabled(BannerlordConfig.EnableDamageTakenVisuals);
			this.RegisterInteractionEvents();
			CombatLogManager.OnGenerateCombatLog += new CombatLogManager.OnPrintCombatLogHandler(this.OnGenerateCombatLog);
			ManagedOptions.OnManagedOptionChanged = (ManagedOptions.OnManagedOptionChangedDelegate)Delegate.Combine(ManagedOptions.OnManagedOptionChanged, new ManagedOptions.OnManagedOptionChangedDelegate(this.OnManagedOptionChanged));
		}

		// Token: 0x0600018C RID: 396 RVA: 0x00008BDB File Offset: 0x00006DDB
		protected override void OnCreateView()
		{
			this._dataSource.IsAgentStatusAvailable = true;
		}

		// Token: 0x0600018D RID: 397 RVA: 0x00008BE9 File Offset: 0x00006DE9
		protected override void OnDestroyView()
		{
			this._dataSource.IsAgentStatusAvailable = false;
		}

		// Token: 0x0600018E RID: 398 RVA: 0x00008BF7 File Offset: 0x00006DF7
		private void OnManagedOptionChanged(ManagedOptions.ManagedOptionsType changedManagedOptionsType)
		{
			if (changedManagedOptionsType == 21)
			{
				MissionAgentStatusVM dataSource = this._dataSource;
				if (dataSource == null)
				{
					return;
				}
				dataSource.TakenDamageController.SetIsEnabled(BannerlordConfig.EnableDamageTakenVisuals);
			}
		}

		// Token: 0x0600018F RID: 399 RVA: 0x00008C18 File Offset: 0x00006E18
		public override void AfterStart()
		{
			base.AfterStart();
			MissionAgentStatusVM dataSource = this._dataSource;
			if (dataSource == null)
			{
				return;
			}
			dataSource.InitializeMainAgentPropterties();
		}

		// Token: 0x06000190 RID: 400 RVA: 0x00008C30 File Offset: 0x00006E30
		public override void OnMissionScreenInitialize()
		{
			base.OnMissionScreenInitialize();
			this._isInDeployement = base.Mission.GetMissionBehavior<BattleDeploymentHandler>() != null;
			if (this._isInDeployement)
			{
				this._deploymentMissionView = base.Mission.GetMissionBehavior<DeploymentMissionView>();
				if (this._deploymentMissionView != null)
				{
					DeploymentMissionView deploymentMissionView = this._deploymentMissionView;
					deploymentMissionView.OnDeploymentFinish = (OnPlayerDeploymentFinishDelegate)Delegate.Combine(deploymentMissionView.OnDeploymentFinish, new OnPlayerDeploymentFinishDelegate(this.OnDeploymentFinish));
				}
			}
		}

		// Token: 0x06000191 RID: 401 RVA: 0x00008C9F File Offset: 0x00006E9F
		private void OnDeploymentFinish()
		{
			this._isInDeployement = false;
			DeploymentMissionView deploymentMissionView = this._deploymentMissionView;
			deploymentMissionView.OnDeploymentFinish = (OnPlayerDeploymentFinishDelegate)Delegate.Remove(deploymentMissionView.OnDeploymentFinish, new OnPlayerDeploymentFinishDelegate(this.OnDeploymentFinish));
		}

		// Token: 0x06000192 RID: 402 RVA: 0x00008CD0 File Offset: 0x00006ED0
		public override void OnMissionScreenFinalize()
		{
			base.OnMissionScreenFinalize();
			this.UnregisterInteractionEvents();
			ManagedOptions.OnManagedOptionChanged = (ManagedOptions.OnManagedOptionChangedDelegate)Delegate.Remove(ManagedOptions.OnManagedOptionChanged, new ManagedOptions.OnManagedOptionChangedDelegate(this.OnManagedOptionChanged));
			CombatLogManager.OnGenerateCombatLog -= new CombatLogManager.OnPrintCombatLogHandler(this.OnGenerateCombatLog);
			base.MissionScreen.RemoveLayer(this._gauntletLayer);
			this._gauntletLayer = null;
			MissionAgentStatusVM dataSource = this._dataSource;
			if (dataSource != null)
			{
				dataSource.OnFinalize();
			}
			this._dataSource = null;
			this._missionMainAgentController = null;
		}

		// Token: 0x06000193 RID: 403 RVA: 0x00008D51 File Offset: 0x00006F51
		public override void OnMissionScreenTick(float dt)
		{
			base.OnMissionScreenTick(dt);
			this._dataSource.IsInDeployement = this._isInDeployement;
			this._dataSource.Tick(dt);
		}

		// Token: 0x06000194 RID: 404 RVA: 0x00008D77 File Offset: 0x00006F77
		public override void OnFocusGained(Agent mainAgent, IFocusable focusableObject, bool isInteractable)
		{
			base.OnFocusGained(mainAgent, focusableObject, isInteractable);
			MissionAgentStatusVM dataSource = this._dataSource;
			if (dataSource == null)
			{
				return;
			}
			dataSource.OnFocusGained(mainAgent, focusableObject, isInteractable);
		}

		// Token: 0x06000195 RID: 405 RVA: 0x00008D95 File Offset: 0x00006F95
		public override void OnAgentInteraction(Agent userAgent, Agent agent)
		{
			base.OnAgentInteraction(userAgent, agent);
			MissionAgentStatusVM dataSource = this._dataSource;
			if (dataSource == null)
			{
				return;
			}
			dataSource.OnAgentInteraction(userAgent, agent);
		}

		// Token: 0x06000196 RID: 406 RVA: 0x00008DB1 File Offset: 0x00006FB1
		public override void OnFocusLost(Agent agent, IFocusable focusableObject)
		{
			base.OnFocusLost(agent, focusableObject);
			MissionAgentStatusVM dataSource = this._dataSource;
			if (dataSource == null)
			{
				return;
			}
			dataSource.OnFocusLost(agent, focusableObject);
		}

		// Token: 0x06000197 RID: 407 RVA: 0x00008DCD File Offset: 0x00006FCD
		public override void OnAgentDeleted(Agent affectedAgent)
		{
			MissionAgentStatusVM dataSource = this._dataSource;
			if (dataSource == null)
			{
				return;
			}
			dataSource.OnAgentDeleted(affectedAgent);
		}

		// Token: 0x06000198 RID: 408 RVA: 0x00008DE0 File Offset: 0x00006FE0
		public override void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow killingBlow)
		{
			MissionAgentStatusVM dataSource = this._dataSource;
			if (dataSource == null)
			{
				return;
			}
			dataSource.OnAgentRemoved(affectedAgent);
		}

		// Token: 0x06000199 RID: 409 RVA: 0x00008DF4 File Offset: 0x00006FF4
		private void OnGenerateCombatLog(CombatLogData logData)
		{
			if (logData.IsVictimAgentMine && logData.TotalDamage > 0 && logData.BodyPartHit != -1)
			{
				MissionAgentStatusVM dataSource = this._dataSource;
				if (dataSource == null)
				{
					return;
				}
				dataSource.OnMainAgentHit(logData.TotalDamage, (float)(logData.IsRangedAttack ? 1 : 0));
			}
		}

		// Token: 0x0600019A RID: 410 RVA: 0x00008E40 File Offset: 0x00007040
		private void RegisterInteractionEvents()
		{
			this._missionMainAgentController = base.Mission.GetMissionBehavior<MissionMainAgentController>();
			if (this._missionMainAgentController != null)
			{
				this._missionMainAgentController.InteractionComponent.OnFocusGained += this._dataSource.OnSecondaryFocusGained;
				this._missionMainAgentController.InteractionComponent.OnFocusLost += this._dataSource.OnSecondaryFocusLost;
				this._missionMainAgentController.InteractionComponent.OnFocusHealthChanged += this._dataSource.InteractionInterface.OnFocusedHealthChanged;
			}
			this._missionMainAgentEquipmentControllerView = base.Mission.GetMissionBehavior<MissionGauntletMainAgentEquipmentControllerView>();
			if (this._missionMainAgentEquipmentControllerView != null)
			{
				this._missionMainAgentEquipmentControllerView.OnEquipmentDropInteractionViewToggled += this._dataSource.OnEquipmentInteractionViewToggled;
				this._missionMainAgentEquipmentControllerView.OnEquipmentEquipInteractionViewToggled += this._dataSource.OnEquipmentInteractionViewToggled;
			}
		}

		// Token: 0x0600019B RID: 411 RVA: 0x00008F20 File Offset: 0x00007120
		private void UnregisterInteractionEvents()
		{
			if (this._missionMainAgentController != null)
			{
				this._missionMainAgentController.InteractionComponent.OnFocusGained -= this._dataSource.OnSecondaryFocusGained;
				this._missionMainAgentController.InteractionComponent.OnFocusLost -= this._dataSource.OnSecondaryFocusLost;
				this._missionMainAgentController.InteractionComponent.OnFocusHealthChanged -= this._dataSource.InteractionInterface.OnFocusedHealthChanged;
			}
			if (this._missionMainAgentEquipmentControllerView != null)
			{
				this._missionMainAgentEquipmentControllerView.OnEquipmentDropInteractionViewToggled -= this._dataSource.OnEquipmentInteractionViewToggled;
				this._missionMainAgentEquipmentControllerView.OnEquipmentEquipInteractionViewToggled -= this._dataSource.OnEquipmentInteractionViewToggled;
			}
		}

		// Token: 0x0600019C RID: 412 RVA: 0x00008FDD File Offset: 0x000071DD
		public override void OnPhotoModeActivated()
		{
			base.OnPhotoModeActivated();
			this._gauntletLayer._gauntletUIContext.ContextAlpha = 0f;
		}

		// Token: 0x0600019D RID: 413 RVA: 0x00008FFA File Offset: 0x000071FA
		public override void OnPhotoModeDeactivated()
		{
			base.OnPhotoModeDeactivated();
			this._gauntletLayer._gauntletUIContext.ContextAlpha = 1f;
		}

		// Token: 0x040000C0 RID: 192
		private GauntletLayer _gauntletLayer;

		// Token: 0x040000C1 RID: 193
		private MissionAgentStatusVM _dataSource;

		// Token: 0x040000C2 RID: 194
		private MissionMainAgentController _missionMainAgentController;

		// Token: 0x040000C3 RID: 195
		private MissionGauntletMainAgentEquipmentControllerView _missionMainAgentEquipmentControllerView;

		// Token: 0x040000C4 RID: 196
		private DeploymentMissionView _deploymentMissionView;

		// Token: 0x040000C5 RID: 197
		private bool _isInDeployement;
	}
}
