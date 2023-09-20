using System;
using System.Collections.Generic;
using SandBox.Tournaments.MissionLogics;
using TaleWorlds.CampaignSystem.TournamentGames;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace SandBox.Missions.MissionLogics
{
	// Token: 0x02000050 RID: 80
	public class SandboxHighlightsController : MissionLogic
	{
		// Token: 0x060003B0 RID: 944 RVA: 0x0001B2C0 File Offset: 0x000194C0
		public override void AfterStart()
		{
			this._highlightsController = Mission.Current.GetMissionBehavior<HighlightsController>();
			foreach (HighlightsController.HighlightType highlightType in this._highlightTypes)
			{
				HighlightsController.AddHighlightType(highlightType);
			}
		}

		// Token: 0x060003B1 RID: 945 RVA: 0x0001B320 File Offset: 0x00019520
		public override void OnAgentRemoved(Agent affectedAgentBase, Agent affectorAgentBase, AgentState agentState, KillingBlow killingBlow)
		{
			if (affectorAgentBase != null && affectorAgentBase.IsMainAgent && affectedAgentBase != null && affectedAgentBase.IsHuman)
			{
				TournamentBehavior missionBehavior = Mission.Current.GetMissionBehavior<TournamentBehavior>();
				if (missionBehavior != null && missionBehavior.CurrentMatch != null && missionBehavior.NextRound == null)
				{
					foreach (TournamentParticipant tournamentParticipant in missionBehavior.CurrentMatch.Participants)
					{
						if (affectorAgentBase.Character == tournamentParticipant.Character && affectedAgentBase.Character != tournamentParticipant.Character)
						{
							HighlightsController.Highlight highlight = default(HighlightsController.Highlight);
							highlight.Start = Mission.Current.CurrentTime;
							highlight.End = Mission.Current.CurrentTime;
							highlight.HighlightType = this._highlightsController.GetHighlightTypeWithId("hlid_tournament_last_match_kill");
							this._highlightsController.SaveHighlight(highlight, affectedAgentBase.Position);
							break;
						}
					}
				}
			}
		}

		// Token: 0x040001BE RID: 446
		private List<HighlightsController.HighlightType> _highlightTypes = new List<HighlightsController.HighlightType>
		{
			new HighlightsController.HighlightType("hlid_tournament_last_match_kill", "Champion of the Arena", "grpid_incidents", -5000, 3000, 0f, float.MaxValue, true)
		};

		// Token: 0x040001BF RID: 447
		private HighlightsController _highlightsController;
	}
}
