using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.TournamentGames
{
	public class TournamentMatch
	{
		public IEnumerable<TournamentTeam> Teams
		{
			get
			{
				return this._teams.AsEnumerable<TournamentTeam>();
			}
		}

		public IEnumerable<TournamentParticipant> Participants
		{
			get
			{
				return this._participants.AsEnumerable<TournamentParticipant>();
			}
		}

		public TournamentMatch.MatchState State { get; private set; }

		public IEnumerable<TournamentParticipant> Winners
		{
			get
			{
				return this._winners.AsEnumerable<TournamentParticipant>();
			}
		}

		public bool IsReady
		{
			get
			{
				return this.State == TournamentMatch.MatchState.Ready;
			}
		}

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

		public void End()
		{
			this.State = TournamentMatch.MatchState.Finished;
			this._winners = this.GetWinners();
		}

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

		public TournamentParticipant GetParticipant(int uniqueSeed)
		{
			return this._participants.FirstOrDefault((TournamentParticipant p) => p.Descriptor.CompareTo(uniqueSeed) == 0);
		}

		public bool IsParticipantRequired()
		{
			return this._participants.Count < this._participantCount;
		}

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

		public bool IsPlayerParticipating()
		{
			return this.Participants.Any((TournamentParticipant x) => x.Character == CharacterObject.PlayerCharacter);
		}

		public bool IsPlayerWinner()
		{
			if (this.IsPlayerParticipating())
			{
				return this.GetWinners().Any((TournamentParticipant x) => x.Character == CharacterObject.PlayerCharacter);
			}
			return false;
		}

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

		private readonly int _numberOfWinnerParticipants;

		public readonly TournamentGame.QualificationMode QualificationMode;

		private readonly TournamentTeam[] _teams;

		private readonly List<TournamentParticipant> _participants;

		private List<TournamentParticipant> _winners;

		private readonly int _participantCount;

		private int _teamSize;

		public enum MatchState
		{
			Ready,
			Started,
			Finished
		}
	}
}
