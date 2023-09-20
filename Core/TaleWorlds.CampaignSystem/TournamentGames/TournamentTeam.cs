using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.TournamentGames
{
	public class TournamentTeam
	{
		public int TeamSize { get; private set; }

		public uint TeamColor { get; private set; }

		public Banner TeamBanner { get; private set; }

		public bool IsPlayerTeam { get; private set; }

		public IEnumerable<TournamentParticipant> Participants
		{
			get
			{
				return this._participants.AsEnumerable<TournamentParticipant>();
			}
		}

		public int Score
		{
			get
			{
				int num = 0;
				foreach (TournamentParticipant tournamentParticipant in this._participants)
				{
					num += tournamentParticipant.Score;
				}
				return num;
			}
		}

		public TournamentTeam(int teamSize, uint teamColor, Banner teamBanner)
		{
			this.TeamColor = teamColor;
			this.TeamBanner = teamBanner;
			this.TeamSize = teamSize;
			this._participants = new List<TournamentParticipant>();
		}

		public bool IsParticipantRequired()
		{
			return this._participants.Count < this.TeamSize;
		}

		public void AddParticipant(TournamentParticipant participant)
		{
			participant.IsAssigned = true;
			this._participants.Add(participant);
			participant.SetTeam(this);
			if (participant.IsPlayer)
			{
				this.IsPlayerTeam = true;
			}
		}

		private List<TournamentParticipant> _participants;
	}
}
