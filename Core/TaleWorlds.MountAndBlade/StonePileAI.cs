using System;
using System.Collections.Generic;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000149 RID: 329
	public class StonePileAI : UsableMachineAIBase
	{
		// Token: 0x060010D0 RID: 4304 RVA: 0x00037032 File Offset: 0x00035232
		public StonePileAI(StonePile stonePile)
			: base(stonePile)
		{
		}

		// Token: 0x060010D1 RID: 4305 RVA: 0x0003703C File Offset: 0x0003523C
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

		// Token: 0x060010D2 RID: 4306 RVA: 0x000370B4 File Offset: 0x000352B4
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

		// Token: 0x060010D3 RID: 4307 RVA: 0x00037130 File Offset: 0x00035330
		public static bool IsAgentAssignable(Agent agent)
		{
			return agent != null && agent.IsAIControlled && agent.IsActive() && !agent.IsRunningAway && !agent.InteractingWithAnyGameObject() && (agent.Formation == null || !agent.IsDetachedFromFormation);
		}

		// Token: 0x060010D4 RID: 4308 RVA: 0x0003716A File Offset: 0x0003536A
		protected override void HandleAgentStopUsingStandingPoint(Agent agent, StandingPoint standingPoint)
		{
			agent.DisableScriptedCombatMovement();
			base.HandleAgentStopUsingStandingPoint(agent, standingPoint);
		}
	}
}
