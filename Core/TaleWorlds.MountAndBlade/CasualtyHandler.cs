using System;
using System.Collections.Generic;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000266 RID: 614
	public class CasualtyHandler : MissionLogic
	{
		// Token: 0x060020DD RID: 8413 RVA: 0x00075D4C File Offset: 0x00073F4C
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

		// Token: 0x060020DE RID: 8414 RVA: 0x00075E20 File Offset: 0x00074020
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

		// Token: 0x060020DF RID: 8415 RVA: 0x00075E50 File Offset: 0x00074050
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

		// Token: 0x04000C1F RID: 3103
		private readonly Dictionary<Formation, int> _casualtyCounts = new Dictionary<Formation, int>();

		// Token: 0x04000C20 RID: 3104
		private readonly Dictionary<Formation, float> _powerLoss = new Dictionary<Formation, float>();
	}
}
