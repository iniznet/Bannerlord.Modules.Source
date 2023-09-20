using System;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade.View.MissionViews.Order;

namespace TaleWorlds.MountAndBlade.View.MissionViews.Singleplayer
{
	// Token: 0x02000061 RID: 97
	public class DeploymentMissionView : MissionView
	{
		// Token: 0x06000431 RID: 1073 RVA: 0x00021855 File Offset: 0x0001FA55
		public override void AfterStart()
		{
			this._orderTroopPlacer = base.Mission.GetMissionBehavior<OrderTroopPlacer>();
			this._entitySelectionHandler = base.Mission.GetMissionBehavior<MissionEntitySelectionUIHandler>();
			this._deploymentBoundaryMarkerHandler = base.Mission.GetMissionBehavior<MissionDeploymentBoundaryMarker>();
		}

		// Token: 0x06000432 RID: 1074 RVA: 0x0002188C File Offset: 0x0001FA8C
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

		// Token: 0x06000433 RID: 1075 RVA: 0x000218E0 File Offset: 0x0001FAE0
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

		// Token: 0x040002A8 RID: 680
		private OrderTroopPlacer _orderTroopPlacer;

		// Token: 0x040002A9 RID: 681
		private MissionDeploymentBoundaryMarker _deploymentBoundaryMarkerHandler;

		// Token: 0x040002AA RID: 682
		private MissionEntitySelectionUIHandler _entitySelectionHandler;

		// Token: 0x040002AB RID: 683
		public OnPlayerDeploymentFinishDelegate OnDeploymentFinish;
	}
}
