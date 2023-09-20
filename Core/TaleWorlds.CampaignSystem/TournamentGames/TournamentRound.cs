using System;

namespace TaleWorlds.CampaignSystem.TournamentGames
{
	// Token: 0x02000283 RID: 643
	public class TournamentRound
	{
		// Token: 0x17000862 RID: 2146
		// (get) Token: 0x060021C8 RID: 8648 RVA: 0x0008F86F File Offset: 0x0008DA6F
		// (set) Token: 0x060021C9 RID: 8649 RVA: 0x0008F877 File Offset: 0x0008DA77
		public TournamentMatch[] Matches { get; private set; }

		// Token: 0x17000863 RID: 2147
		// (get) Token: 0x060021CA RID: 8650 RVA: 0x0008F880 File Offset: 0x0008DA80
		// (set) Token: 0x060021CB RID: 8651 RVA: 0x0008F888 File Offset: 0x0008DA88
		public int CurrentMatchIndex { get; private set; }

		// Token: 0x17000864 RID: 2148
		// (get) Token: 0x060021CC RID: 8652 RVA: 0x0008F891 File Offset: 0x0008DA91
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

		// Token: 0x060021CD RID: 8653 RVA: 0x0008F8B4 File Offset: 0x0008DAB4
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

		// Token: 0x060021CE RID: 8654 RVA: 0x0008F900 File Offset: 0x0008DB00
		public void OnMatchEnded()
		{
			int currentMatchIndex = this.CurrentMatchIndex;
			this.CurrentMatchIndex = currentMatchIndex + 1;
		}

		// Token: 0x060021CF RID: 8655 RVA: 0x0008F920 File Offset: 0x0008DB20
		public void EndMatch()
		{
			this.CurrentMatch.End();
			int currentMatchIndex = this.CurrentMatchIndex;
			this.CurrentMatchIndex = currentMatchIndex + 1;
		}

		// Token: 0x060021D0 RID: 8656 RVA: 0x0008F948 File Offset: 0x0008DB48
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
