using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem.TournamentGames;
using TaleWorlds.Library;

namespace SandBox.ViewModelCollection.Tournament
{
	public class TournamentMatchVM : ViewModel
	{
		public TournamentMatch Match { get; private set; }

		public List<TournamentTeamVM> Teams { get; }

		public TournamentMatchVM()
		{
			this.Team1 = new TournamentTeamVM();
			this.Team2 = new TournamentTeamVM();
			this.Team3 = new TournamentTeamVM();
			this.Team4 = new TournamentTeamVM();
			this.Teams = new List<TournamentTeamVM> { this.Team1, this.Team2, this.Team3, this.Team4 };
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.Teams.ForEach(delegate(TournamentTeamVM x)
			{
				x.RefreshValues();
			});
		}

		public void Initialize()
		{
			foreach (TournamentTeamVM tournamentTeamVM in this.Teams)
			{
				if (tournamentTeamVM.IsValid)
				{
					tournamentTeamVM.Initialize();
				}
			}
		}

		public void Initialize(TournamentMatch match)
		{
			int num = 0;
			this.Match = match;
			this.IsValid = this.Match != null;
			this.Count = match.Teams.Count<TournamentTeam>();
			foreach (TournamentTeam tournamentTeam in match.Teams)
			{
				this.Teams[num].Initialize(tournamentTeam);
				num++;
			}
			this.State = 0;
		}

		public void Refresh(bool forceRefresh)
		{
			if (forceRefresh)
			{
				base.OnPropertyChanged("Count");
			}
			for (int i = 0; i < this.Count; i++)
			{
				TournamentTeamVM tournamentTeamVM = this.Teams[i];
				if (forceRefresh)
				{
					base.OnPropertyChanged("Team" + i + 1);
				}
				tournamentTeamVM.Refresh();
				for (int j = 0; j < tournamentTeamVM.Count; j++)
				{
					TournamentParticipantVM tournamentParticipantVM = tournamentTeamVM.Participants[j];
					tournamentParticipantVM.Score = tournamentParticipantVM.Participant.Score.ToString();
					tournamentParticipantVM.IsQualifiedForNextRound = this.Match.Winners.Contains(tournamentParticipantVM.Participant);
				}
			}
		}

		public void RefreshActiveMatch()
		{
			for (int i = 0; i < this.Count; i++)
			{
				TournamentTeamVM tournamentTeamVM = this.Teams[i];
				for (int j = 0; j < tournamentTeamVM.Count; j++)
				{
					TournamentParticipantVM tournamentParticipantVM = tournamentTeamVM.Participants[j];
					tournamentParticipantVM.Score = tournamentParticipantVM.Participant.Score.ToString();
				}
			}
		}

		public void Refresh(TournamentMatchVM target)
		{
			base.OnPropertyChanged("Count");
			int num = 0;
			foreach (TournamentTeamVM tournamentTeamVM in this.Teams.Where((TournamentTeamVM t) => t.IsValid))
			{
				base.OnPropertyChanged("Team" + num + 1);
				tournamentTeamVM.Refresh();
				num++;
			}
		}

		public IEnumerable<TournamentParticipantVM> GetParticipants()
		{
			List<TournamentParticipantVM> list = new List<TournamentParticipantVM>();
			if (this.Team1.IsValid)
			{
				list.AddRange(this.Team1.GetParticipants());
			}
			if (this.Team2.IsValid)
			{
				list.AddRange(this.Team2.GetParticipants());
			}
			if (this.Team3.IsValid)
			{
				list.AddRange(this.Team3.GetParticipants());
			}
			if (this.Team4.IsValid)
			{
				list.AddRange(this.Team4.GetParticipants());
			}
			return list;
		}

		[DataSourceProperty]
		public bool IsValid
		{
			get
			{
				return this._isValid;
			}
			set
			{
				if (value != this._isValid)
				{
					this._isValid = value;
					base.OnPropertyChangedWithValue(value, "IsValid");
				}
			}
		}

		[DataSourceProperty]
		public int State
		{
			get
			{
				return this._state;
			}
			set
			{
				if (value != this._state)
				{
					this._state = value;
					base.OnPropertyChangedWithValue(value, "State");
				}
			}
		}

		[DataSourceProperty]
		public int Count
		{
			get
			{
				return this._count;
			}
			set
			{
				if (value != this._count)
				{
					this._count = value;
					base.OnPropertyChangedWithValue(value, "Count");
				}
			}
		}

		[DataSourceProperty]
		public TournamentTeamVM Team1
		{
			get
			{
				return this._team1;
			}
			set
			{
				if (value != this._team1)
				{
					this._team1 = value;
					base.OnPropertyChangedWithValue<TournamentTeamVM>(value, "Team1");
				}
			}
		}

		[DataSourceProperty]
		public TournamentTeamVM Team2
		{
			get
			{
				return this._team2;
			}
			set
			{
				if (value != this._team2)
				{
					this._team2 = value;
					base.OnPropertyChangedWithValue<TournamentTeamVM>(value, "Team2");
				}
			}
		}

		[DataSourceProperty]
		public TournamentTeamVM Team3
		{
			get
			{
				return this._team3;
			}
			set
			{
				if (value != this._team3)
				{
					this._team3 = value;
					base.OnPropertyChangedWithValue<TournamentTeamVM>(value, "Team3");
				}
			}
		}

		[DataSourceProperty]
		public TournamentTeamVM Team4
		{
			get
			{
				return this._team4;
			}
			set
			{
				if (value != this._team4)
				{
					this._team4 = value;
					base.OnPropertyChangedWithValue<TournamentTeamVM>(value, "Team4");
				}
			}
		}

		private TournamentTeamVM _team1;

		private TournamentTeamVM _team2;

		private TournamentTeamVM _team3;

		private TournamentTeamVM _team4;

		private int _count = -1;

		private int _state = -1;

		private bool _isValid;

		public enum TournamentMatchState
		{
			Unfinished,
			Current,
			Over,
			Active
		}
	}
}
