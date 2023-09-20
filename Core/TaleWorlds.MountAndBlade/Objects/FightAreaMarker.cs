using System;
using System.Collections.Generic;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade.Objects
{
	// Token: 0x020003A0 RID: 928
	public class FightAreaMarker : AreaMarker
	{
		// Token: 0x060032A2 RID: 12962 RVA: 0x000D181C File Offset: 0x000CFA1C
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

		// Token: 0x060032A3 RID: 12963 RVA: 0x000D183A File Offset: 0x000CFA3A
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

		// Token: 0x04001556 RID: 5462
		public int SubAreaIndex = 1;
	}
}
