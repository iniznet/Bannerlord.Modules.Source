using System;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.MissionViews;
using TaleWorlds.MountAndBlade.View.MissionViews.Singleplayer;
using TaleWorlds.MountAndBlade.ViewModelCollection.HUD;

namespace TaleWorlds.MountAndBlade.GauntletUI.Mission.Singleplayer
{
	[OverrideView(typeof(MissionAgentLockVisualizerView))]
	public class MissionGauntletAgentLockVisualizerView : MissionGauntletBattleUIBase
	{
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

		protected override void OnDestroyView()
		{
			base.MissionScreen.RemoveLayer(this._layer);
			this._dataSource.OnFinalize();
			this._dataSource = null;
			this._layer = null;
			this._missionMainAgentController = null;
		}

		private void OnPotentialLockedAgentChanged(Agent newPotentialAgent)
		{
			MissionAgentLockVisualizerVM dataSource = this._dataSource;
			if (dataSource != null && dataSource.IsEnabled)
			{
				this._dataSource.OnPossibleLockAgentChange(this._latestPotentialLockedAgent, newPotentialAgent);
				this._latestPotentialLockedAgent = newPotentialAgent;
			}
		}

		private void OnLockedAgentChanged(Agent newAgent)
		{
			MissionAgentLockVisualizerVM dataSource = this._dataSource;
			if (dataSource != null && dataSource.IsEnabled)
			{
				this._dataSource.OnActiveLockAgentChange(this._latestLockedAgent, newAgent);
				this._latestLockedAgent = newAgent;
			}
		}

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

		private bool IsMainAgentAvailable()
		{
			Agent main = Agent.Main;
			return main != null && main.IsActive();
		}

		private GauntletLayer _layer;

		private MissionAgentLockVisualizerVM _dataSource;

		private MissionMainAgentController _missionMainAgentController;

		private Agent _latestLockedAgent;

		private Agent _latestPotentialLockedAgent;
	}
}
