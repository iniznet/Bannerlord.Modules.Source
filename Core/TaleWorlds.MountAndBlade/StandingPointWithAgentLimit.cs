using System;
using System.Collections.Generic;

namespace TaleWorlds.MountAndBlade
{
	public class StandingPointWithAgentLimit : StandingPoint
	{
		public void AddValidAgent(Agent agent)
		{
			if (agent != null)
			{
				this._validAgents.Add(agent);
			}
		}

		public void ClearValidAgents()
		{
			this._validAgents.Clear();
		}

		public override bool IsDisabledForAgent(Agent agent)
		{
			return !this._validAgents.Contains(agent) || base.IsDisabledForAgent(agent);
		}

		private readonly List<Agent> _validAgents = new List<Agent>();
	}
}
