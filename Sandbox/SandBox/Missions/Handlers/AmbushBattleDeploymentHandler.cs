using System;
using SandBox.Missions.AgentControllers;
using TaleWorlds.MountAndBlade;

namespace SandBox.Missions.Handlers
{
	// Token: 0x02000066 RID: 102
	public class AmbushBattleDeploymentHandler : DeploymentHandler
	{
		// Token: 0x06000467 RID: 1127 RVA: 0x0002078D File Offset: 0x0001E98D
		public AmbushBattleDeploymentHandler(bool isPlayerAttacker)
			: base(isPlayerAttacker)
		{
		}

		// Token: 0x06000468 RID: 1128 RVA: 0x00020796 File Offset: 0x0001E996
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
