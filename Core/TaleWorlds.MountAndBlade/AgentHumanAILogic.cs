using System;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000257 RID: 599
	public class AgentHumanAILogic : MissionLogic
	{
		// Token: 0x0600205D RID: 8285 RVA: 0x00073009 File Offset: 0x00071209
		public override void OnAgentCreated(Agent agent)
		{
			base.OnAgentCreated(agent);
			if (agent.IsAIControlled && agent.IsHuman)
			{
				agent.AddComponent(new HumanAIComponent(agent));
			}
		}

		// Token: 0x0600205E RID: 8286 RVA: 0x00073030 File Offset: 0x00071230
		protected internal override void OnAgentControllerChanged(Agent agent, Agent.ControllerType oldController)
		{
			base.OnAgentControllerChanged(agent, oldController);
			if (agent.IsHuman)
			{
				if (agent.Controller == Agent.ControllerType.AI)
				{
					agent.AddComponent(new HumanAIComponent(agent));
					return;
				}
				if (oldController == Agent.ControllerType.AI && agent.HumanAIComponent != null)
				{
					agent.RemoveComponent(agent.HumanAIComponent);
				}
			}
		}
	}
}
