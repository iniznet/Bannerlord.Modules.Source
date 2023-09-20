using System;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.MissionViews;
using TaleWorlds.MountAndBlade.View.MissionViews.Singleplayer;
using TaleWorlds.MountAndBlade.ViewModelCollection.HUD;

namespace TaleWorlds.MountAndBlade.GauntletUI.Mission.Singleplayer
{
	// Token: 0x0200002D RID: 45
	[OverrideView(typeof(MissionAgentLockVisualizerView))]
	public class MissionGauntletAgentLockVisualizerView : MissionGauntletBattleUIBase
	{
		// Token: 0x06000228 RID: 552 RVA: 0x0000BF10 File Offset: 0x0000A110
		protected override void OnCreateView()
		{
			this._missionMainAgentController = base.Mission.GetMissionBehavior<MissionMainAgentController>();
			this._missionMainAgentController.OnLockedAgentChanged += this.OnLockedAgentChanged;
			this._missionMainAgentController.OnPotentialLockedAgentChanged += this.OnPotentialLockedAgentChanged;
			this._dataSource = new MissionAgentLockVisualizerVM();
			this._layer = new GauntletLayer(10, "GauntletLayer", false);
			this._layer.LoadMovie("AgentLockTargets", this._dataSource);
			base.MissionScreen.AddLayer(this._layer);
		}

		// Token: 0x06000229 RID: 553 RVA: 0x0000BFA2 File Offset: 0x0000A1A2
		protected override void OnDestroyView()
		{
			base.MissionScreen.RemoveLayer(this._layer);
			this._dataSource.OnFinalize();
			this._dataSource = null;
			this._layer = null;
			this._missionMainAgentController = null;
		}

		// Token: 0x0600022A RID: 554 RVA: 0x0000BFD5 File Offset: 0x0000A1D5
		private void OnPotentialLockedAgentChanged(Agent newPotentialAgent)
		{
			MissionAgentLockVisualizerVM dataSource = this._dataSource;
			if (dataSource != null && dataSource.IsEnabled)
			{
				this._dataSource.OnPossibleLockAgentChange(this._latestPotentialLockedAgent, newPotentialAgent);
				this._latestPotentialLockedAgent = newPotentialAgent;
			}
		}

		// Token: 0x0600022B RID: 555 RVA: 0x0000C004 File Offset: 0x0000A204
		private void OnLockedAgentChanged(Agent newAgent)
		{
			MissionAgentLockVisualizerVM dataSource = this._dataSource;
			if (dataSource != null && dataSource.IsEnabled)
			{
				this._dataSource.OnActiveLockAgentChange(this._latestLockedAgent, newAgent);
				this._latestLockedAgent = newAgent;
			}
		}

		// Token: 0x0600022C RID: 556 RVA: 0x0000C034 File Offset: 0x0000A234
		public override void OnMissionScreenTick(float dt)
		{
			base.OnMissionScreenTick(dt);
			if (base.IsViewActive && this._dataSource != null)
			{
				this._dataSource.IsEnabled = this.IsMainAgentAvailable();
				if (this._dataSource.IsEnabled)
				{
					for (int i = 0; i < this._dataSource.AllTrackedAgents.Count; i++)
					{
						MissionAgentLockItemVM missionAgentLockItemVM = this._dataSource.AllTrackedAgents[i];
						float num = 0f;
						float num2 = 0f;
						float num3 = 0f;
						MBWindowManager.WorldToScreenInsideUsableArea(base.MissionScreen.CombatCamera, missionAgentLockItemVM.TrackedAgent.GetChestGlobalPosition(), ref num, ref num2, ref num3);
						missionAgentLockItemVM.Position = new Vec2(num, num2);
					}
				}
			}
		}

		// Token: 0x0600022D RID: 557 RVA: 0x0000C0EB File Offset: 0x0000A2EB
		private bool IsMainAgentAvailable()
		{
			Agent main = Agent.Main;
			return main != null && main.IsActive();
		}

		// Token: 0x04000112 RID: 274
		private GauntletLayer _layer;

		// Token: 0x04000113 RID: 275
		private MissionAgentLockVisualizerVM _dataSource;

		// Token: 0x04000114 RID: 276
		private MissionMainAgentController _missionMainAgentController;

		// Token: 0x04000115 RID: 277
		private Agent _latestLockedAgent;

		// Token: 0x04000116 RID: 278
		private Agent _latestPotentialLockedAgent;
	}
}
