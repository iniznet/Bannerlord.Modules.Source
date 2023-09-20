using System;
using SandBox.Missions.AgentControllers;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.MissionViews;
using TaleWorlds.MountAndBlade.View.MissionViews.SiegeWeapon;
using TaleWorlds.MountAndBlade.View.MissionViews.Singleplayer;

namespace SandBox.View.Missions
{
	// Token: 0x02000010 RID: 16
	public class MissionAmbushView : MissionView
	{
		// Token: 0x0600005C RID: 92 RVA: 0x000046D0 File Offset: 0x000028D0
		public override void AfterStart()
		{
			base.AfterStart();
			this._ambushMissionController = base.Mission.GetMissionBehavior<AmbushMissionController>();
			this._deploymentBoundaryMarkerHandler = base.Mission.GetMissionBehavior<MissionDeploymentBoundaryMarker>();
			this._ambushMissionController.PlayerDeploymentFinish += this.OnPlayerDeploymentFinish;
			this._ambushMissionController.IntroFinish += this.OnIntroFinish;
		}

		// Token: 0x0600005D RID: 93 RVA: 0x00004734 File Offset: 0x00002934
		public override void OnMissionTick(float dt)
		{
			if (this._firstTick)
			{
				this._firstTick = false;
				if (this._ambushMissionController.IsPlayerAmbusher)
				{
					this._ambushDeploymentView = new MissionAmbushDeploymentView();
					base.MissionScreen.AddMissionView(this._ambushDeploymentView);
					this._ambushDeploymentView.OnBehaviorInitialize();
					this._ambushDeploymentView.EarlyStart();
					this._ambushDeploymentView.AfterStart();
				}
			}
		}

		// Token: 0x0600005E RID: 94 RVA: 0x0000479C File Offset: 0x0000299C
		public void OnIntroFinish()
		{
			if (this._deploymentBoundaryMarkerHandler != null)
			{
				base.Mission.RemoveMissionBehavior(this._deploymentBoundaryMarkerHandler);
			}
			base.MissionScreen.AddMissionView(ViewCreator.CreateMissionAgentStatusUIHandler(null));
			base.MissionScreen.AddMissionView(ViewCreator.CreateMissionMainAgentEquipmentController(null));
			base.MissionScreen.AddMissionView(ViewCreator.CreateMissionMainAgentCheerBarkControllerView(null));
			base.MissionScreen.AddMissionView(ViewCreator.CreateMissionAgentLockVisualizerView(null));
			base.MissionScreen.AddMissionView(ViewCreator.CreateMissionBoundaryCrossingView());
			base.MissionScreen.AddMissionView(new MissionBoundaryWallView());
			base.MissionScreen.AddMissionView(new MissionMainAgentController());
			base.MissionScreen.AddMissionView(new MissionCrosshair());
			base.MissionScreen.AddMissionView(new RangedSiegeWeaponViewController());
		}

		// Token: 0x0600005F RID: 95 RVA: 0x00004856 File Offset: 0x00002A56
		public void OnPlayerDeploymentFinish()
		{
			if (this._ambushMissionController.IsPlayerAmbusher)
			{
				base.Mission.RemoveMissionBehavior(this._ambushDeploymentView);
			}
		}

		// Token: 0x04000026 RID: 38
		private AmbushMissionController _ambushMissionController;

		// Token: 0x04000027 RID: 39
		private MissionDeploymentBoundaryMarker _deploymentBoundaryMarkerHandler;

		// Token: 0x04000028 RID: 40
		private MissionAmbushDeploymentView _ambushDeploymentView;

		// Token: 0x04000029 RID: 41
		private bool _firstTick = true;
	}
}
