using System;
using System.Collections.Generic;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000362 RID: 866
	public class StandingPointWithAgentLimit : StandingPoint
	{
		// Token: 0x06002F50 RID: 12112 RVA: 0x000C101C File Offset: 0x000BF21C
		public void AddValidAgent(Agent agent)
		{
			if (agent != null)
			{
				this._validAgents.Add(agent);
			}
		}

		// Token: 0x06002F51 RID: 12113 RVA: 0x000C102D File Offset: 0x000BF22D
		public void ClearValidAgents()
		{
			this._validAgents.Clear();
		}

		// Token: 0x06002F52 RID: 12114 RVA: 0x000C103A File Offset: 0x000BF23A
		public override bool IsDisabledForAgent(Agent agent)
		{
			return !this._validAgents.Contains(agent) || base.IsDisabledForAgent(agent);
		}

		// Token: 0x04001366 RID: 4966
		private readonly List<Agent> _validAgents = new List<Agent>();
	}
}
