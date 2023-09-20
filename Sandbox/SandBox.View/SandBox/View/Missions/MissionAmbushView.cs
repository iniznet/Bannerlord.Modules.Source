using System;
using SandBox.Missions.AgentControllers;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.MissionViews;
using TaleWorlds.MountAndBlade.View.MissionViews.SiegeWeapon;
using TaleWorlds.MountAndBlade.View.MissionViews.Singleplayer;

namespace SandBox.View.Missions
{
	public class MissionAmbushView : MissionView
	{
		public override void AfterStart()
		{
			base.AfterStart();
			this._ambushMissionController = base.Mission.GetMissionBehavior<AmbushMissionController>();
			this._deploymentBoundaryMarkerHandler = base.Mission.GetMissionBehavior<MissionDeploymentBoundaryMarker>();
			this._ambushMissionController.PlayerDeploymentFinish += this.OnPlayerDeploymentFinish;
			this._ambushMissionController.IntroFinish += this.OnIntroFinish;
		}

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

		public void OnPlayerDeploymentFinish()
		{
			if (this._ambushMissionController.IsPlayerAmbusher)
			{
				base.Mission.RemoveMissionBehavior(this._ambushDeploymentView);
			}
		}

		private AmbushMissionController _ambushMissionController;

		private MissionDeploymentBoundaryMarker _deploymentBoundaryMarkerHandler;

		private MissionAmbushDeploymentView _ambushDeploymentView;

		private bool _firstTick = true;
	}
}
