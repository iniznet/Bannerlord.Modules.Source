using System;
using System.Collections.Generic;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade
{
	public class CasualtyHandler : MissionLogic
	{
		public override void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow killingBlow)
		{
			this.RegisterCasualty(affectedAgent);
		}

		public override void OnAgentFleeing(Agent affectedAgent)
		{
			this.RegisterCasualty(affectedAgent);
		}

		public int GetCasualtyCountOfFormation(Formation formation)
		{
			int num;
			if (!this._casualtyCounts.TryGetValue(formation, out num))
			{
				num = 0;
				this._casualtyCounts[formation] = 0;
			}
			return num;
		}

		public float GetCasualtyPowerLossOfFormation(Formation formation)
		{
			float num;
			if (!this._powerLoss.TryGetValue(formation, out num))
			{
				num = 0f;
				this._powerLoss[formation] = 0f;
			}
			return num;
		}

		private void RegisterCasualty(Agent agent)
		{
			Formation formation = agent.Formation;
			if (formation != null)
			{
				if (this._casualtyCounts.ContainsKey(formation))
				{
					Dictionary<Formation, int> casualtyCounts = this._casualtyCounts;
					Formation formation2 = formation;
					int num = casualtyCounts[formation2];
					casualtyCounts[formation2] = num + 1;
				}
				else
				{
					this._casualtyCounts[formation] = 1;
				}
				if (this._powerLoss.ContainsKey(formation))
				{
					Dictionary<Formation, float> powerLoss = this._powerLoss;
					Formation formation2 = formation;
					powerLoss[formation2] += agent.Character.GetPower();
					return;
				}
				this._powerLoss[formation] = agent.Character.GetPower();
			}
		}

		private readonly Dictionary<Formation, int> _casualtyCounts = new Dictionary<Formation, int>();

		private readonly Dictionary<Formation, float> _powerLoss = new Dictionary<Formation, float>();
	}
}
