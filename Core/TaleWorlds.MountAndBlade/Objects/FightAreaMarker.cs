using System;
using System.Collections.Generic;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade.Objects
{
	public class FightAreaMarker : AreaMarker
	{
		public IEnumerable<Agent> GetAgentsInRange(Team team, bool humanOnly = true)
		{
			foreach (Agent agent in team.ActiveAgents)
			{
				if ((!humanOnly || agent.IsHuman) && base.IsPositionInRange(agent.Position))
				{
					yield return agent;
				}
			}
			List<Agent>.Enumerator enumerator = default(List<Agent>.Enumerator);
			yield break;
			yield break;
		}

		public IEnumerable<Agent> GetAgentsInRange(BattleSideEnum side, bool humanOnly = true)
		{
			foreach (Team team in Mission.Current.Teams)
			{
				if (team.Side == side)
				{
					foreach (Agent agent in this.GetAgentsInRange(team, humanOnly))
					{
						yield return agent;
					}
					IEnumerator<Agent> enumerator2 = null;
				}
			}
			List<Team>.Enumerator enumerator = default(List<Team>.Enumerator);
			yield break;
			yield break;
		}

		public int SubAreaIndex = 1;
	}
}
