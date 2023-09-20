using System;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade.ComponentInterfaces;

namespace TaleWorlds.MountAndBlade
{
	public class DefaultAgentDecideKilledOrUnconsciousModel : AgentDecideKilledOrUnconsciousModel
	{
		public override float GetAgentStateProbability(Agent affectorAgent, Agent effectedAgent, DamageTypes damageType, out float useSurgeryProbability)
		{
			useSurgeryProbability = 0f;
			return 1f;
		}
	}
}
