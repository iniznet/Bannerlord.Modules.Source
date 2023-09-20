using System;
using System.Collections.Generic;
using NetworkMessages.FromServer;
using TaleWorlds.Core;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.MissionViews;
using TaleWorlds.MountAndBlade.View.MissionViews.Multiplayer;
using TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.KillFeed;

namespace TaleWorlds.MountAndBlade.GauntletUI.Mission.Multiplayer
{
	// Token: 0x0200003F RID: 63
	[OverrideView(typeof(MissionMultiplayerKillNotificationUIHandler))]
	public class MissionGauntletKillNotificationUIHandler : MissionView
	{
		// Token: 0x060002F6 RID: 758 RVA: 0x000107FC File Offset: 0x0000E9FC
		public override void OnMissionScreenInitialize()
		{
			base.OnMissionScreenInitialize();
			this.ViewOrderPriority = 2;
			this._isGeneralFeedEnabled = this._doesGameModeAllowGeneralFeed && BannerlordConfig.ReportCasualtiesType < 2;
			this._isPersonalFeedEnabled = BannerlordConfig.ReportPersonalDamage;
			this._dataSource = new MPKillFeedVM();
			this._gauntletLayer = new GauntletLayer(this.ViewOrderPriority, "GauntletLayer", false);
			this._gauntletLayer.LoadMovie("MultiplayerKillFeed", this._dataSource);
			base.MissionScreen.AddLayer(this._gauntletLayer);
			CombatLogManager.OnGenerateCombatLog += new CombatLogManager.OnPrintCombatLogHandler(this.OnCombatLogManagerOnPrintCombatLog);
			ManagedOptions.OnManagedOptionChanged = (ManagedOptions.OnManagedOptionChangedDelegate)Delegate.Combine(ManagedOptions.OnManagedOptionChanged, new ManagedOptions.OnManagedOptionChangedDelegate(this.OnOptionChange));
		}

		// Token: 0x060002F7 RID: 759 RVA: 0x000108B5 File Offset: 0x0000EAB5
		private void OnOptionChange(ManagedOptions.ManagedOptionsType changedManagedOptionsType)
		{
			if (changedManagedOptionsType == 16)
			{
				this._isGeneralFeedEnabled = this._doesGameModeAllowGeneralFeed && BannerlordConfig.ReportCasualtiesType < 2;
				return;
			}
			if (changedManagedOptionsType == 18)
			{
				this._isPersonalFeedEnabled = BannerlordConfig.ReportPersonalDamage;
			}
		}

		// Token: 0x060002F8 RID: 760 RVA: 0x000108E8 File Offset: 0x0000EAE8
		public override void AfterStart()
		{
			base.AfterStart();
			this._tdmClient = base.Mission.GetMissionBehavior<MissionMultiplayerTeamDeathmatchClient>();
			if (this._tdmClient != null)
			{
				this._tdmClient.OnGoldGainEvent += this.OnGoldGain;
			}
			this._siegeClient = base.Mission.GetMissionBehavior<MissionMultiplayerSiegeClient>();
			if (this._siegeClient != null)
			{
				this._siegeClient.OnGoldGainEvent += this.OnGoldGain;
			}
			this._flagDominationClient = base.Mission.GetMissionBehavior<MissionMultiplayerGameModeFlagDominationClient>();
			if (this._flagDominationClient != null)
			{
				this._flagDominationClient.OnGoldGainEvent += this.OnGoldGain;
			}
			this._duelClient = base.Mission.GetMissionBehavior<MissionMultiplayerGameModeDuelClient>();
			if (this._duelClient != null)
			{
				this._doesGameModeAllowGeneralFeed = false;
			}
		}

		// Token: 0x060002F9 RID: 761 RVA: 0x000109AC File Offset: 0x0000EBAC
		public override void OnMissionScreenFinalize()
		{
			base.OnMissionScreenFinalize();
			CombatLogManager.OnGenerateCombatLog -= new CombatLogManager.OnPrintCombatLogHandler(this.OnCombatLogManagerOnPrintCombatLog);
			ManagedOptions.OnManagedOptionChanged = (ManagedOptions.OnManagedOptionChangedDelegate)Delegate.Remove(ManagedOptions.OnManagedOptionChanged, new ManagedOptions.OnManagedOptionChangedDelegate(this.OnOptionChange));
			if (this._tdmClient != null)
			{
				this._tdmClient.OnGoldGainEvent -= this.OnGoldGain;
			}
			if (this._siegeClient != null)
			{
				this._siegeClient.OnGoldGainEvent -= this.OnGoldGain;
			}
			if (this._flagDominationClient != null)
			{
				this._flagDominationClient.OnGoldGainEvent -= this.OnGoldGain;
			}
			base.MissionScreen.RemoveLayer(this._gauntletLayer);
			this._gauntletLayer = null;
			this._dataSource.OnFinalize();
			this._dataSource = null;
		}

		// Token: 0x060002FA RID: 762 RVA: 0x00010A78 File Offset: 0x0000EC78
		private void OnGoldGain(GoldGain goldGainMessage)
		{
			if (this._isPersonalFeedEnabled)
			{
				foreach (KeyValuePair<ushort, int> keyValuePair in goldGainMessage.GoldChangeEventList)
				{
					this._dataSource.PersonalCasualty.OnGoldChange(keyValuePair.Value, keyValuePair.Key);
				}
			}
		}

		// Token: 0x060002FB RID: 763 RVA: 0x00010AEC File Offset: 0x0000ECEC
		private void OnCombatLogManagerOnPrintCombatLog(CombatLogData logData)
		{
			if (this._isPersonalFeedEnabled && (logData.IsAttackerAgentMine || logData.IsAttackerAgentRiderAgentMine) && logData.TotalDamage > 0 && !logData.IsVictimAgentSameAsAttackerAgent)
			{
				this._dataSource.OnPersonalDamage(logData.TotalDamage, logData.IsFatalDamage, logData.IsVictimAgentMount, logData.IsFriendlyFire, logData.BodyPartHit == 0, logData.VictimAgentName);
			}
		}

		// Token: 0x060002FC RID: 764 RVA: 0x00010B58 File Offset: 0x0000ED58
		public override void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow killingBlow)
		{
			base.OnAgentRemoved(affectedAgent, affectorAgent, agentState, killingBlow);
			if (!this._isGeneralFeedEnabled || GameNetwork.IsDedicatedServer || affectorAgent == null || !affectedAgent.IsHuman || (agentState != 4 && agentState != 3))
			{
				return;
			}
			this._dataSource.OnAgentRemoved(affectedAgent, affectorAgent, this._isPersonalFeedEnabled);
		}

		// Token: 0x0400018A RID: 394
		private MPKillFeedVM _dataSource;

		// Token: 0x0400018B RID: 395
		private GauntletLayer _gauntletLayer;

		// Token: 0x0400018C RID: 396
		private MissionMultiplayerTeamDeathmatchClient _tdmClient;

		// Token: 0x0400018D RID: 397
		private MissionMultiplayerSiegeClient _siegeClient;

		// Token: 0x0400018E RID: 398
		private MissionMultiplayerGameModeDuelClient _duelClient;

		// Token: 0x0400018F RID: 399
		private MissionMultiplayerGameModeFlagDominationClient _flagDominationClient;

		// Token: 0x04000190 RID: 400
		private bool _isGeneralFeedEnabled;

		// Token: 0x04000191 RID: 401
		private bool _doesGameModeAllowGeneralFeed = true;

		// Token: 0x04000192 RID: 402
		private bool _isPersonalFeedEnabled;
	}
}
