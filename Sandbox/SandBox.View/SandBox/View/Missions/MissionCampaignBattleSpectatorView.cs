using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.LinQuick;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View.MissionViews;
using TaleWorlds.MountAndBlade.View.Screens;

namespace SandBox.View.Missions
{
	public class MissionCampaignBattleSpectatorView : MissionView
	{
		public override void AfterStart()
		{
			base.MissionScreen.SetCustomAgentListToSpectateGatherer(new MissionScreen.GatherCustomAgentListToSpectateDelegate(this.SpectateListGatherer));
		}

		private int CalculateAgentScore(Agent agent)
		{
			Mission mission = agent.Mission;
			CharacterObject characterObject = (CharacterObject)agent.Character;
			int num = (agent.IsPlayerControlled ? 2000000 : 0);
			if (agent.Team != null && agent.Team.IsValid)
			{
				num += ((mission.PlayerTeam != null && mission.PlayerTeam.IsValid && agent.Team.IsEnemyOf(mission.PlayerTeam)) ? 0 : 1000000);
				if (agent.Team.GeneralAgent == agent)
				{
					num += 500000;
				}
				else if (characterObject.IsHero)
				{
					if (characterObject.HeroObject.IsLord)
					{
						num += 125000;
					}
					else
					{
						num += 250000;
					}
					using (List<Formation>.Enumerator enumerator = agent.Team.FormationsIncludingEmpty.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							if (enumerator.Current.Captain == agent)
							{
								num += 100000;
							}
						}
					}
				}
				if (characterObject.IsMounted)
				{
					num += 50000;
				}
				if (!characterObject.IsRanged)
				{
					num += 25000;
				}
				num += (int)agent.Health;
			}
			return num;
		}

		private List<Agent> SpectateListGatherer(Agent forcedAgentToInclude)
		{
			return LinQuick.WhereQ<Agent>(base.Mission.AllAgents, (Agent x) => x.IsCameraAttachable() || x == forcedAgentToInclude).OrderByDescending(new Func<Agent, int>(this.CalculateAgentScore)).ToList<Agent>();
		}
	}
}
