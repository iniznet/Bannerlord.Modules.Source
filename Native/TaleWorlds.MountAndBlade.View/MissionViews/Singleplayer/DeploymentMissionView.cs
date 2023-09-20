using System;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade.View.MissionViews.Order;

namespace TaleWorlds.MountAndBlade.View.MissionViews.Singleplayer
{
	public class DeploymentMissionView : MissionView
	{
		public override void AfterStart()
		{
			this._orderTroopPlacer = base.Mission.GetMissionBehavior<OrderTroopPlacer>();
			this._entitySelectionHandler = base.Mission.GetMissionBehavior<MissionEntitySelectionUIHandler>();
			this._deploymentBoundaryMarkerHandler = base.Mission.GetMissionBehavior<MissionDeploymentBoundaryMarker>();
		}

		public override void OnInitialDeploymentPlanMadeForSide(BattleSideEnum side, bool isFirstPlan)
		{
			if (side == base.Mission.PlayerTeam.Side && base.Mission.DeploymentPlan.HasDeploymentBoundaries(base.Mission.PlayerTeam.Side, 0))
			{
				OrderTroopPlacer orderTroopPlacer = this._orderTroopPlacer;
				if (orderTroopPlacer == null)
				{
					return;
				}
				orderTroopPlacer.RestrictOrdersToDeploymentBoundaries(true);
			}
		}

		public override void OnDeploymentFinished()
		{
			this.OnDeploymentFinish();
			if (this._entitySelectionHandler != null)
			{
				base.Mission.RemoveMissionBehavior(this._entitySelectionHandler);
			}
			if (this._deploymentBoundaryMarkerHandler != null)
			{
				if (base.Mission.DeploymentPlan.HasDeploymentBoundaries(base.Mission.PlayerTeam.Side, 0))
				{
					OrderTroopPlacer orderTroopPlacer = this._orderTroopPlacer;
					if (orderTroopPlacer != null)
					{
						orderTroopPlacer.RestrictOrdersToDeploymentBoundaries(false);
					}
				}
				base.Mission.RemoveMissionBehavior(this._deploymentBoundaryMarkerHandler);
			}
			if (!base.Mission.HasMissionBehavior<MissionBoundaryWallView>())
			{
				MissionBoundaryWallView missionBoundaryWallView = new MissionBoundaryWallView();
				base.MissionScreen.AddMissionView(missionBoundaryWallView);
			}
		}

		private OrderTroopPlacer _orderTroopPlacer;

		private MissionDeploymentBoundaryMarker _deploymentBoundaryMarkerHandler;

		private MissionEntitySelectionUIHandler _entitySelectionHandler;

		public OnPlayerDeploymentFinishDelegate OnDeploymentFinish;
	}
}
