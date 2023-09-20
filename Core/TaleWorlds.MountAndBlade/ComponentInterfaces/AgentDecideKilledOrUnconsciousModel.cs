using System;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade.ComponentInterfaces
{
	// Token: 0x02000405 RID: 1029
	public abstract class AgentDecideKilledOrUnconsciousModel : GameModel
	{
		// Token: 0x0600355A RID: 13658
		public abstract float GetAgentStateProbability(Agent affectorAgent, Agent effectedAgent, DamageTypes damageType, out float useSurgeryProbability);
	}
}
