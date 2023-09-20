using System;
using TaleWorlds.Core;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.MissionViews.Singleplayer;
using TaleWorlds.MountAndBlade.ViewModelCollection.HUD.KillFeed;

namespace TaleWorlds.MountAndBlade.GauntletUI.Mission.Singleplayer
{
	[OverrideView(typeof(MissionSingleplayerKillNotificationUIHandler))]
	public class MissionGauntletKillNotificationSingleplayerUIHandler : MissionGauntletBattleUIBase
	{
		public override void OnMissionScreenInitialize()
		{
			base.OnMissionScreenInitialize();
			this.ViewOrderPriority = 17;
			this._isGeneralFeedEnabled = BannerlordConfig.ReportCasualtiesType < 2;
			this._isPersonalFeedEnabled = BannerlordConfig.ReportPersonalDamage;
			ManagedOptions.OnManagedOptionChanged = (ManagedOptions.OnManagedOptionChangedDelegate)Delegate.Combine(ManagedOptions.OnManagedOptionChanged, new ManagedOptions.OnManagedOptionChangedDelegate(this.OnOptionChange));
		}

		public override void OnMissionScreenFinalize()
		{
			base.OnMissionScreenFinalize();
			ManagedOptions.OnManagedOptionChanged = (ManagedOptions.OnManagedOptionChangedDelegate)Delegate.Remove(ManagedOptions.OnManagedOptionChanged, new ManagedOptions.OnManagedOptionChangedDelegate(this.OnOptionChange));
		}

		protected override void OnCreateView()
		{
			this._dataSource = new SPKillFeedVM();
			this._gauntletLayer = new GauntletLayer(this.ViewOrderPriority, "GauntletLayer", false);
			this._gauntletLayer.LoadMovie("SingleplayerKillfeed", this._dataSource);
			base.MissionScreen.AddLayer(this._gauntletLayer);
			CombatLogManager.OnGenerateCombatLog += new CombatLogManager.OnPrintCombatLogHandler(this.OnCombatLogManagerOnPrintCombatLog);
		}

		protected override void OnDestroyView()
		{
			CombatLogManager.OnGenerateCombatLog -= new CombatLogManager.OnPrintCombatLogHandler(this.OnCombatLogManagerOnPrintCombatLog);
			base.MissionScreen.RemoveLayer(this._gauntletLayer);
			this._gauntletLayer = null;
			this._dataSource.OnFinalize();
			this._dataSource = null;
		}

		private void OnOptionChange(ManagedOptions.ManagedOptionsType changedManagedOptionsType)
		{
			if (changedManagedOptionsType == 17)
			{
				this._isGeneralFeedEnabled = BannerlordConfig.ReportCasualtiesType < 2;
				return;
			}
			if (changedManagedOptionsType == 19)
			{
				this._isPersonalFeedEnabled = BannerlordConfig.ReportPersonalDamage;
			}
		}

		public override void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow killingBlow)
		{
			base.OnAgentRemoved(affectedAgent, affectorAgent, agentState, killingBlow);
			if (!base.IsViewActive || affectorAgent == null || (agentState != 4 && agentState != 3))
			{
				return;
			}
			bool flag = killingBlow.IsHeadShot();
			if (this._isPersonalFeedEnabled && affectorAgent == Agent.Main && (affectedAgent.IsHuman || affectedAgent.IsMount))
			{
				bool flag2 = affectedAgent.Team == affectorAgent.Team || affectedAgent.IsFriendOf(affectorAgent);
				SPKillFeedVM dataSource = this._dataSource;
				int inflictedDamage = killingBlow.InflictedDamage;
				bool isMount = affectedAgent.IsMount;
				bool flag3 = flag2;
				bool flag4 = flag;
				BasicCharacterObject character = affectedAgent.Character;
				dataSource.OnPersonalKill(inflictedDamage, isMount, flag3, flag4, (character != null) ? character.Name.ToString() : null, agentState == 3);
			}
			if (this._isGeneralFeedEnabled && affectedAgent.IsHuman)
			{
				this._dataSource.OnAgentRemoved(affectedAgent, affectorAgent, flag);
			}
		}

		private void OnCombatLogManagerOnPrintCombatLog(CombatLogData logData)
		{
			if (this._isPersonalFeedEnabled && !logData.IsVictimAgentMine && (logData.IsAttackerAgentMine || logData.IsAttackerAgentRiderAgentMine) && logData.TotalDamage > 0 && !logData.IsFatalDamage)
			{
				this._dataSource.OnPersonalDamage(logData.TotalDamage, logData.IsVictimAgentMount, logData.IsFriendlyFire, logData.VictimAgentName);
			}
		}

		public override void OnPhotoModeActivated()
		{
			base.OnPhotoModeActivated();
			if (base.IsViewActive)
			{
				this._gauntletLayer._gauntletUIContext.ContextAlpha = 0f;
			}
		}

		public override void OnPhotoModeDeactivated()
		{
			base.OnPhotoModeDeactivated();
			if (base.IsViewActive)
			{
				this._gauntletLayer._gauntletUIContext.ContextAlpha = 1f;
			}
		}

		private SPKillFeedVM _dataSource;

		private GauntletLayer _gauntletLayer;

		private bool _isGeneralFeedEnabled = true;

		private bool _isPersonalFeedEnabled = true;
	}
}
