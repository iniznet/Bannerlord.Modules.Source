using System;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000256 RID: 598
	public class AgentCommonAILogic : MissionLogic
	{
		// Token: 0x0600205A RID: 8282 RVA: 0x00072FAB File Offset: 0x000711AB
		public override void OnAgentCreated(Agent agent)
		{
			base.OnAgentCreated(agent);
			if (agent.IsAIControlled)
			{
				agent.AddComponent(new CommonAIComponent(agent));
			}
		}

		// Token: 0x0600205B RID: 8283 RVA: 0x00072FC8 File Offset: 0x000711C8
		protected internal override void OnAgentControllerChanged(Agent agent, Agent.ControllerType oldController)
		{
			base.OnAgentControllerChanged(agent, oldController);
			if (agent.Controller == Agent.ControllerType.AI)
			{
				agent.AddComponent(new CommonAIComponent(agent));
				return;
			}
			if (oldController == Agent.ControllerType.AI && agent.CommonAIComponent != null)
			{
				agent.RemoveComponent(agent.CommonAIComponent);
			}
		}
	}
}
