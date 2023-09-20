using System;

namespace TaleWorlds.CampaignSystem.TournamentGames
{
	public class TournamentRound
	{
		public TournamentMatch[] Matches { get; private set; }

		public int CurrentMatchIndex { get; private set; }

		public TournamentMatch CurrentMatch
		{
			get
			{
				if (this.CurrentMatchIndex >= this.Matches.Length)
				{
					return null;
				}
				return this.Matches[this.CurrentMatchIndex];
			}
		}

		public TournamentRound(int participantCount, int numberOfMatches, int numberOfTeamsPerMatch, int numberOfWinnerParticipants, TournamentGame.QualificationMode qualificationMode)
		{
			this.Matches = new TournamentMatch[numberOfMatches];
			this.CurrentMatchIndex = 0;
			int num = participantCount / numberOfMatches;
			for (int i = 0; i < numberOfMatches; i++)
			{
				this.Matches[i] = new TournamentMatch(num, numberOfTeamsPerMatch, numberOfWinnerParticipants / numberOfMatches, qualificationMode);
			}
		}

		public void OnMatchEnded()
		{
			int currentMatchIndex = this.CurrentMatchIndex;
			this.CurrentMatchIndex = currentMatchIndex + 1;
		}

		public void EndMatch()
		{
			this.CurrentMatch.End();
			int currentMatchIndex = this.CurrentMatchIndex;
			this.CurrentMatchIndex = currentMatchIndex + 1;
		}

		public void AddParticipant(TournamentParticipant participant, bool firstTime = false)
		{
			foreach (TournamentMatch tournamentMatch in this.Matches)
			{
				if (tournamentMatch.IsParticipantRequired())
				{
					tournamentMatch.AddParticipant(participant, firstTime);
					return;
				}
			}
		}
	}
}
