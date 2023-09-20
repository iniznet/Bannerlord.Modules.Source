using System;
using System.Collections.Generic;

namespace TaleWorlds.MountAndBlade
{
	public class MissionAgentPanicHandler : MissionLogic
	{
		public MissionAgentPanicHandler()
		{
			this._panickedAgents = new List<Agent>(256);
			this._panickedFormations = new List<Formation>(24);
			this._panickedTeams = new List<Team>(2);
		}

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

		public override void OnRemoveBehavior()
		{
			this._panickedAgents.Clear();
			this._panickedFormations.Clear();
			this._panickedTeams.Clear();
			base.OnRemoveBehavior();
		}

		private readonly List<Agent> _panickedAgents;

		private readonly List<Formation> _panickedFormations;

		private readonly List<Team> _panickedTeams;
	}
}
