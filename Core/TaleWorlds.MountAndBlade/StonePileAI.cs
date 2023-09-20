using System;
using System.Collections.Generic;

namespace TaleWorlds.MountAndBlade
{
	public class StonePileAI : UsableMachineAIBase
	{
		public StonePileAI(StonePile stonePile)
			: base(stonePile)
		{
		}

		public static Agent GetSuitableAgentForStandingPoint(StonePile usableMachine, StandingPoint standingPoint, List<Agent> agents, List<Agent> usedAgents)
		{
			float num = float.MinValue;
			Agent agent = null;
			foreach (Agent agent2 in agents)
			{
				if (StonePileAI.IsAgentAssignable(agent2) && !standingPoint.IsDisabledForAgent(agent2) && standingPoint.GetUsageScoreForAgent(agent2) > num)
				{
					num = standingPoint.GetUsageScoreForAgent(agent2);
					agent = agent2;
				}
			}
			return agent;
		}

		public static Agent GetSuitableAgentForStandingPoint(StonePile stonePile, StandingPoint standingPoint, List<ValueTuple<Agent, float>> agents, List<Agent> usedAgents, float weight)
		{
			float num = float.MinValue;
			Agent agent = null;
			foreach (ValueTuple<Agent, float> valueTuple in agents)
			{
				Agent item = valueTuple.Item1;
				if (StonePileAI.IsAgentAssignable(item) && !standingPoint.IsDisabledForAgent(item) && standingPoint.GetUsageScoreForAgent(item) > num)
				{
					num = standingPoint.GetUsageScoreForAgent(item);
					agent = item;
				}
			}
			return agent;
		}

		public static bool IsAgentAssignable(Agent agent)
		{
			return agent != null && agent.IsAIControlled && agent.IsActive() && !agent.IsRunningAway && !agent.InteractingWithAnyGameObject() && (agent.Formation == null || !agent.IsDetachedFromFormation);
		}

		protected override void HandleAgentStopUsingStandingPoint(Agent agent, StandingPoint standingPoint)
		{
			agent.DisableScriptedCombatMovement();
			base.HandleAgentStopUsingStandingPoint(agent, standingPoint);
		}
	}
}
