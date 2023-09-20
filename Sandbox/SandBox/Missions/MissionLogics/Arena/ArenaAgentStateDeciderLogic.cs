using System;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace SandBox.Missions.MissionLogics.Arena
{
	// Token: 0x02000062 RID: 98
	public class ArenaAgentStateDeciderLogic : MissionLogic, IAgentStateDecider, IMissionBehavior
	{
		// Token: 0x06000435 RID: 1077 RVA: 0x0001F1B0 File Offset: 0x0001D3B0
		public AgentState GetAgentState(Agent effectedAgent, float deathProbability, out bool usedSurgery)
		{
			usedSurgery = false;
			return 3;
		}
	}
}
