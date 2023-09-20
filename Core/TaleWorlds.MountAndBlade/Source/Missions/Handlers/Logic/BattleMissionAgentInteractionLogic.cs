using System;

namespace TaleWorlds.MountAndBlade.Source.Missions.Handlers.Logic
{
	// Token: 0x020003FE RID: 1022
	public class BattleMissionAgentInteractionLogic : MissionLogic
	{
		// Token: 0x06003515 RID: 13589 RVA: 0x000DD668 File Offset: 0x000DB868
		public override bool IsThereAgentAction(Agent userAgent, Agent otherAgent)
		{
			return !otherAgent.IsEnemyOf(userAgent);
		}
	}
}
