using System;
using TaleWorlds.Core;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.MissionViews.Singleplayer;
using TaleWorlds.MountAndBlade.ViewModelCollection.HUD.KillFeed;

namespace TaleWorlds.MountAndBlade.GauntletUI.Mission.Singleplayer
{
	// Token: 0x02000030 RID: 48
	[OverrideView(typeof(MissionSingleplayerKillNotificationUIHandler))]
	public class MissionGauntletKillNotificationSingleplayerUIHandler : MissionGauntletBattleUIBase
	{
		// Token: 0x06000247 RID: 583 RVA: 0x0000CA7C File Offset: 0x0000AC7C
		public override void OnMissionScreenInitialize()
		{
			base.OnMissionScreenInitialize();
			this.ViewOrderPriority = 17;
			this._isGeneralFeedEnabled = BannerlordConfig.ReportCasualtiesType < 2;
			this._isPersonalFeedEnabled = BannerlordConfig.ReportPersonalDamage;
			ManagedOptions.OnManagedOptionChanged = (ManagedOptions.OnManagedOptionChangedDelegate)Delegate.Combine(ManagedOptions.OnManagedOptionChanged, new ManagedOptions.OnManagedOptionChangedDelegate(this.OnOptionChange));
		}

		// Token: 0x06000248 RID: 584 RVA: 0x0000CAD0 File Offset: 0x0000ACD0
		public override void OnMissionScreenFinalize()
		{
			base.OnMissionScreenFinalize();
			ManagedOptions.OnManagedOptionChanged = (ManagedOptions.OnManagedOptionChangedDelegate)Delegate.Remove(ManagedOptions.OnManagedOptionChanged, new ManagedOptions.OnManagedOptionChangedDelegate(this.OnOptionChange));
		}

		// Token: 0x06000249 RID: 585 RVA: 0x0000CAF8 File Offset: 0x0000ACF8
		protected override void OnCreateView()
		{
			this._dataSource = new SPKillFeedVM();
			this._gauntletLayer = new GauntletLayer(this.ViewOrderPriority, "GauntletLayer", false);
			this._gauntletLayer.LoadMovie("SingleplayerKillfeed", this._dataSource);
			base.MissionScreen.AddLayer(this._gauntletLayer);
			CombatLogManager.OnGenerateCombatLog += new CombatLogManager.OnPrintCombatLogHandler(this.OnCombatLogManagerOnPrintCombatLog);
		}

		// Token: 0x0600024A RID: 586 RVA: 0x0000CB60 File Offset: 0x0000AD60
		protected override void OnDestroyView()
		{
			CombatLogManager.OnGenerateCombatLog -= new CombatLogManager.OnPrintCombatLogHandler(this.OnCombatLogManagerOnPrintCombatLog);
			base.MissionScreen.RemoveLayer(this._gauntletLayer);
			this._gauntletLayer = null;
			this._dataSource.OnFinalize();
			this._dataSource = null;
		}

		// Token: 0x0600024B RID: 587 RVA: 0x0000CB9D File Offset: 0x0000AD9D
		private void OnOptionChange(ManagedOptions.ManagedOptionsType changedManagedOptionsType)
		{
			if (changedManagedOptionsType == 16)
			{
				this._isGeneralFeedEnabled = BannerlordConfig.ReportCasualtiesType < 2;
				return;
			}
			if (changedManagedOptionsType == 18)
			{
				this._isPersonalFeedEnabled = BannerlordConfig.ReportPersonalDamage;
			}
		}

		// Token: 0x0600024C RID: 588 RVA: 0x0000CBC4 File Offset: 0x0000ADC4
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

		// Token: 0x0600024D RID: 589 RVA: 0x0000CC84 File Offset: 0x0000AE84
		private void OnCombatLogManagerOnPrintCombatLog(CombatLogData logData)
		{
			if (this._isPersonalFeedEnabled && !logData.IsVictimAgentMine && (logData.IsAttackerAgentMine || logData.IsAttackerAgentRiderAgentMine) && logData.TotalDamage > 0 && !logData.IsFatalDamage)
			{
				this._dataSource.OnPersonalDamage(logData.TotalDamage, logData.IsVictimAgentMount, logData.IsFriendlyFire, logData.VictimAgentName);
			}
		}

		// Token: 0x0600024E RID: 590 RVA: 0x0000CCE7 File Offset: 0x0000AEE7
		public override void OnPhotoModeActivated()
		{
			base.OnPhotoModeActivated();
			if (base.IsViewActive)
			{
				this._gauntletLayer._gauntletUIContext.ContextAlpha = 0f;
			}
		}

		// Token: 0x0600024F RID: 591 RVA: 0x0000CD0C File Offset: 0x0000AF0C
		public override void OnPhotoModeDeactivated()
		{
			base.OnPhotoModeDeactivated();
			if (base.IsViewActive)
			{
				this._gauntletLayer._gauntletUIContext.ContextAlpha = 1f;
			}
		}

		// Token: 0x04000122 RID: 290
		private SPKillFeedVM _dataSource;

		// Token: 0x04000123 RID: 291
		private GauntletLayer _gauntletLayer;

		// Token: 0x04000124 RID: 292
		private bool _isGeneralFeedEnabled = true;

		// Token: 0x04000125 RID: 293
		private bool _isPersonalFeedEnabled = true;
	}
}
