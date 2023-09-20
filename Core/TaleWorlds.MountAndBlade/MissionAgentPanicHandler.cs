using System;
using System.Collections.Generic;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200026E RID: 622
	public class MissionAgentPanicHandler : MissionLogic
	{
		// Token: 0x06002129 RID: 8489 RVA: 0x00078442 File Offset: 0x00076642
		public MissionAgentPanicHandler()
		{
			this._panickedAgents = new List<Agent>(256);
			this._panickedFormations = new List<Formation>(24);
			this._panickedTeams = new List<Team>(2);
		}

		// Token: 0x0600212A RID: 8490 RVA: 0x00078474 File Offset: 0x00076674
		public override void OnAgentPanicked(Agent agent)
		{
			this._panickedAgents.Add(agent);
			if (agent.Formation != null && agent.Team != null)
			{
				if (!this._panickedFormations.Contains(agent.Formation))
				{
					this._panickedFormations.Add(agent.Formation);
				}
				if (!this._panickedTeams.Contains(agent.Team))
				{
					this._panickedTeams.Add(agent.Team);
				}
			}
		}

		// Token: 0x0600212B RID: 8491 RVA: 0x000784E8 File Offset: 0x000766E8
		public override void OnPreMissionTick(float dt)
		{
			if (this._panickedAgents.Count > 0)
			{
				foreach (Team team in this._panickedTeams)
				{
					team.UpdateCachedEnemyDataForFleeing();
				}
				foreach (Formation formation in this._panickedFormations)
				{
					formation.OnBatchUnitRemovalStart();
				}
				foreach (Agent agent in this._panickedAgents)
				{
					CommonAIComponent commonAIComponent = agent.CommonAIComponent;
					if (commonAIComponent != null)
					{
						commonAIComponent.Retreat();
					}
					Mission.Current.OnAgentFleeing(agent);
				}
				foreach (Formation formation2 in this._panickedFormations)
				{
					formation2.OnBatchUnitRemovalEnd();
				}
				this._panickedAgents.Clear();
				this._panickedFormations.Clear();
				this._panickedTeams.Clear();
			}
		}

		// Token: 0x0600212C RID: 8492 RVA: 0x00078640 File Offset: 0x00076840
		public override void OnRemoveBehavior()
		{
			this._panickedAgents.Clear();
			this._panickedFormations.Clear();
			this._panickedTeams.Clear();
			base.OnRemoveBehavior();
		}

		// Token: 0x04000C47 RID: 3143
		private readonly List<Agent> _panickedAgents;

		// Token: 0x04000C48 RID: 3144
		private readonly List<Formation> _panickedFormations;

		// Token: 0x04000C49 RID: 3145
		private readonly List<Team> _panickedTeams;
	}
}
