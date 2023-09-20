using System;
using System.Collections.Generic;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade
{
	public class CasualtyHandler : MissionLogic
	{
		public override void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow killingBlow)
		{
			if (base.Mission.Mode != MissionMode.Deployment && affectedAgent.IsHuman && affectedAgent.Formation != null)
			{
				if (this._casualtyCounts.ContainsKey(affectedAgent.Formation))
				{
					Dictionary<Formation, int> casualtyCounts = this._casualtyCounts;
					Formation formation = affectedAgent.Formation;
					int num = casualtyCounts[formation];
					casualtyCounts[formation] = num + 1;
				}
				else
				{
					this._casualtyCounts[affectedAgent.Formation] = 1;
				}
				if (this._powerLoss.ContainsKey(affectedAgent.Formation))
				{
					Dictionary<Formation, float> powerLoss = this._powerLoss;
					Formation formation = affectedAgent.Formation;
					powerLoss[formation] += affectedAgent.Character.GetPower();
					return;
				}
				this._powerLoss[affectedAgent.Formation] = affectedAgent.Character.GetPower();
			}
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

		private readonly Dictionary<Formation, int> _casualtyCounts = new Dictionary<Formation, int>();

		private readonly Dictionary<Formation, float> _powerLoss = new Dictionary<Formation, float>();
	}
}
