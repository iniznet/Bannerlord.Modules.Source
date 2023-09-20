using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.TournamentGames
{
	// Token: 0x02000281 RID: 641
	public class TournamentMatch
	{
		// Token: 0x17000856 RID: 2134
		// (get) Token: 0x060021A8 RID: 8616 RVA: 0x0008F29B File Offset: 0x0008D49B
		public IEnumerable<TournamentTeam> Teams
		{
			get
			{
				return this._teams.AsEnumerable<TournamentTeam>();
			}
		}

		// Token: 0x17000857 RID: 2135
		// (get) Token: 0x060021A9 RID: 8617 RVA: 0x0008F2A8 File Offset: 0x0008D4A8
		public IEnumerable<TournamentParticipant> Participants
		{
			get
			{
				return this._participants.AsEnumerable<TournamentParticipant>();
			}
		}

		// Token: 0x17000858 RID: 2136
		// (get) Token: 0x060021AA RID: 8618 RVA: 0x0008F2B5 File Offset: 0x0008D4B5
		// (set) Token: 0x060021AB RID: 8619 RVA: 0x0008F2BD File Offset: 0x0008D4BD
		public TournamentMatch.MatchState State { get; private set; }

		// Token: 0x17000859 RID: 2137
		// (get) Token: 0x060021AC RID: 8620 RVA: 0x0008F2C6 File Offset: 0x0008D4C6
		public IEnumerable<TournamentParticipant> Winners
		{
			get
			{
				return this._winners.AsEnumerable<TournamentParticipant>();
			}
		}

		// Token: 0x1700085A RID: 2138
		// (get) Token: 0x060021AD RID: 8621 RVA: 0x0008F2D3 File Offset: 0x0008D4D3
		public bool IsReady
		{
			get
			{
				return this.State == TournamentMatch.MatchState.Ready;
			}
		}

		// Token: 0x060021AE RID: 8622 RVA: 0x0008F2E0 File Offset: 0x0008D4E0
		public TournamentMatch(int participantCount, int numberOfTeamsPerMatch, int numberOfWinnerParticipants, TournamentGame.QualificationMode qualificationMode)
		{
			this._participants = new List<TournamentParticipant>();
			this._participantCount = participantCount;
			this._teams = new TournamentTeam[numberOfTeamsPerMatch];
			this._winners = new List<TournamentParticipant>();
			this._numberOfWinnerParticipants = numberOfWinnerParticipants;
			this.QualificationMode = qualificationMode;
			this._teamSize = participantCount / numberOfTeamsPerMatch;
			int[] array = new int[] { 119, 118, 120, 121 };
			int num = 0;
			for (int i = 0; i < numberOfTeamsPerMatch; i++)
			{
				this._teams[i] = new TournamentTeam(this._teamSize, BannerManager.GetColor(array[num]), Banner.CreateOneColoredEmptyBanner(array[num]));
				num++;
				num %= 4;
			}
			this.State = TournamentMatch.MatchState.Ready;
		}

		// Token: 0x060021AF RID: 8623 RVA: 0x0008F386 File Offset: 0x0008D586
		public void End()
		{
			this.State = TournamentMatch.MatchState.Finished;
			this._winners = this.GetWinners();
		}

		// Token: 0x060021B0 RID: 8624 RVA: 0x0008F39C File Offset: 0x0008D59C
		public void Start()
		{
			if (this.State != TournamentMatch.MatchState.Started)
			{
				this.State = TournamentMatch.MatchState.Started;
				foreach (TournamentParticipant tournamentParticipant in this.Participants)
				{
					tournamentParticipant.ResetScore();
				}
			}
		}

		// Token: 0x060021B1 RID: 8625 RVA: 0x0008F3F8 File Offset: 0x0008D5F8
		public TournamentParticipant GetParticipant(int uniqueSeed)
		{
			return this._participants.FirstOrDefault((TournamentParticipant p) => p.Descriptor.CompareTo(uniqueSeed) == 0);
		}

		// Token: 0x060021B2 RID: 8626 RVA: 0x0008F429 File Offset: 0x0008D629
		public bool IsParticipantRequired()
		{
			return this._participants.Count < this._participantCount;
		}

		// Token: 0x060021B3 RID: 8627 RVA: 0x0008F440 File Offset: 0x0008D640
		public void AddParticipant(TournamentParticipant participant, bool firstTime)
		{
			this._participants.Add(participant);
			foreach (TournamentTeam tournamentTeam in this.Teams)
			{
				if (tournamentTeam.IsParticipantRequired() && ((participant.Team != null && participant.Team.TeamColor == tournamentTeam.TeamColor) || firstTime))
				{
					tournamentTeam.AddParticipant(participant);
					return;
				}
			}
			foreach (TournamentTeam tournamentTeam2 in this.Teams)
			{
				if (tournamentTeam2.IsParticipantRequired())
				{
					tournamentTeam2.AddParticipant(participant);
					break;
				}
			}
		}

		// Token: 0x060021B4 RID: 8628 RVA: 0x0008F50C File Offset: 0x0008D70C
		public bool IsPlayerParticipating()
		{
			return this.Participants.Any((TournamentParticipant x) => x.Character == CharacterObject.PlayerCharacter);
		}

		// Token: 0x060021B5 RID: 8629 RVA: 0x0008F538 File Offset: 0x0008D738
		public bool IsPlayerWinner()
		{
			if (this.IsPlayerParticipating())
			{
				return this.GetWinners().Any((TournamentParticipant x) => x.Character == CharacterObject.PlayerCharacter);
			}
			return false;
		}

		// Token: 0x060021B6 RID: 8630 RVA: 0x0008F570 File Offset: 0x0008D770
		private List<TournamentParticipant> GetWinners()
		{
			List<TournamentParticipant> list = new List<TournamentParticipant>();
			if (this.QualificationMode == TournamentGame.QualificationMode.IndividualScore)
			{
				List<TournamentParticipant> list2 = this._participants.OrderByDescending((TournamentParticipant x) => x.Score).Take(this._numberOfWinnerParticipants).ToList<TournamentParticipant>();
				using (List<TournamentParticipant>.Enumerator enumerator = this._participants.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						TournamentParticipant tournamentParticipant = enumerator.Current;
						if (list2.Contains(tournamentParticipant))
						{
							tournamentParticipant.IsAssigned = false;
							list.Add(tournamentParticipant);
						}
					}
					return list;
				}
			}
			if (this.QualificationMode == TournamentGame.QualificationMode.TeamScore)
			{
				IOrderedEnumerable<TournamentTeam> orderedEnumerable = this._teams.OrderByDescending((TournamentTeam x) => x.Score);
				List<TournamentTeam> list3 = orderedEnumerable.Take(this._numberOfWinnerParticipants / this._teamSize).ToList<TournamentTeam>();
				foreach (TournamentTeam tournamentTeam in this._teams)
				{
					if (list3.Contains(tournamentTeam))
					{
						foreach (TournamentParticipant tournamentParticipant2 in tournamentTeam.Participants)
						{
							tournamentParticipant2.IsAssigned = false;
							list.Add(tournamentParticipant2);
						}
					}
				}
				foreach (TournamentTeam tournamentTeam2 in orderedEnumerable)
				{
					int num = this._numberOfWinnerParticipants - list.Count;
					if (tournamentTeam2.Participants.Count<TournamentParticipant>() >= num)
					{
						IOrderedEnumerable<TournamentParticipant> orderedEnumerable2 = tournamentTeam2.Participants.OrderByDescending((TournamentParticipant x) => x.Score);
						list.AddRange(orderedEnumerable2.Take(num));
						break;
					}
					list.AddRange(tournamentTeam2.Participants);
				}
			}
			return list;
		}

		// Token: 0x04000A75 RID: 2677
		private readonly int _numberOfWinnerParticipants;

		// Token: 0x04000A76 RID: 2678
		public readonly TournamentGame.QualificationMode QualificationMode;

		// Token: 0x04000A77 RID: 2679
		private readonly TournamentTeam[] _teams;

		// Token: 0x04000A78 RID: 2680
		private readonly List<TournamentParticipant> _participants;

		// Token: 0x04000A7A RID: 2682
		private List<TournamentParticipant> _winners;

		// Token: 0x04000A7B RID: 2683
		private readonly int _participantCount;

		// Token: 0x04000A7C RID: 2684
		private int _teamSize;

		// Token: 0x02000596 RID: 1430
		public enum MatchState
		{
			// Token: 0x0400174B RID: 5963
			Ready,
			// Token: 0x0400174C RID: 5964
			Started,
			// Token: 0x0400174D RID: 5965
			Finished
		}
	}
}
