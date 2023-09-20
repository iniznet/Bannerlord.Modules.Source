using System;
using SandBox.Missions.AgentControllers;
using TaleWorlds.MountAndBlade;

namespace SandBox.Missions.Handlers
{
	public class AmbushBattleDeploymentHandler : DeploymentHandler
	{
		public AmbushBattleDeploymentHandler(bool isPlayerAttacker)
			: base(isPlayerAttacker)
		{
		}

		public override void FinishDeployment()
		{
			base.FinishDeployment();
			if (base.Mission.GetMissionBehavior<AmbushMissionController>() != null)
			{
				base.Mission.GetMissionBehavior<AmbushMissionController>().OnPlayerDeploymentFinish(false);
			}
		}
	}
}
