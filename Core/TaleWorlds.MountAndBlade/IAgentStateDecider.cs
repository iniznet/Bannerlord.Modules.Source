using System;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200024F RID: 591
	public interface IAgentStateDecider : IMissionBehavior
	{
		// Token: 0x06001FF7 RID: 8183
		AgentState GetAgentState(Agent affectedAgent, float deathProbability, out bool usedSurgery);
	}
}
