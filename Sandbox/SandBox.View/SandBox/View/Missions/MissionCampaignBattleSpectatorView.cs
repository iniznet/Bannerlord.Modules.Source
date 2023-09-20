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
	// Token: 0x02000013 RID: 19
	public class MissionCampaignBattleSpectatorView : MissionView
	{
		// Token: 0x0600006E RID: 110 RVA: 0x00004F76 File Offset: 0x00003176
		public override void AfterStart()
		{
			base.MissionScreen.SetCustomAgentListToSpectateGatherer(new MissionScreen.GatherCustomAgentListToSpectateDelegate(this.SpectateListGatherer));
		}

		// Token: 0x0600006F RID: 111 RVA: 0x00004F90 File Offset: 0x00003190
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

		// Token: 0x06000070 RID: 112 RVA: 0x000050CC File Offset: 0x000032CC
		private List<Agent> SpectateListGatherer(Agent forcedAgentToInclude)
		{
			return LinQuick.WhereQ<Agent>(base.Mission.AllAgents, (Agent x) => x.IsCameraAttachable() || x == forcedAgentToInclude).OrderByDescending(new Func<Agent, int>(this.CalculateAgentScore)).ToList<Agent>();
		}
	}
}
