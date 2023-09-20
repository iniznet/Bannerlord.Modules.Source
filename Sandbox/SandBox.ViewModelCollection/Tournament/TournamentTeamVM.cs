using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem.TournamentGames;
using TaleWorlds.Library;

namespace SandBox.ViewModelCollection.Tournament
{
	public class TournamentTeamVM : ViewModel
	{
		public List<TournamentParticipantVM> Participants { get; }

		public TournamentTeamVM()
		{
			this.Participant1 = new TournamentParticipantVM();
			this.Participant2 = new TournamentParticipantVM();
			this.Participant3 = new TournamentParticipantVM();
			this.Participant4 = new TournamentParticipantVM();
			this.Participant5 = new TournamentParticipantVM();
			this.Participant6 = new TournamentParticipantVM();
			this.Participant7 = new TournamentParticipantVM();
			this.Participant8 = new TournamentParticipantVM();
			this.Participants = new List<TournamentParticipantVM> { this.Participant1, this.Participant2, this.Participant3, this.Participant4, this.Participant5, this.Participant6, this.Participant7, this.Participant8 };
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.Participants.ForEach(delegate(TournamentParticipantVM x)
			{
				x.RefreshValues();
			});
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
		public int Score
		{
			get
			{
				return this._score;
			}
			set
			{
				if (value != this._score)
				{
					this._score = value;
					base.OnPropertyChangedWithValue(value, "Score");
				}
			}
		}

		[DataSourceProperty]
		public TournamentParticipantVM Participant1
		{
			get
			{
				return this._participant1;
			}
			set
			{
				if (value != this._participant1)
				{
					this._participant1 = value;
					base.OnPropertyChangedWithValue<TournamentParticipantVM>(value, "Participant1");
				}
			}
		}

		[DataSourceProperty]
		public TournamentParticipantVM Participant2
		{
			get
			{
				return this._participant2;
			}
			set
			{
				if (value != this._participant2)
				{
					this._participant2 = value;
					base.OnPropertyChangedWithValue<TournamentParticipantVM>(value, "Participant2");
				}
			}
		}

		[DataSourceProperty]
		public TournamentParticipantVM Participant3
		{
			get
			{
				return this._participant3;
			}
			set
			{
				if (value != this._participant3)
				{
					this._participant3 = value;
					base.OnPropertyChangedWithValue<TournamentParticipantVM>(value, "Participant3");
				}
			}
		}

		[DataSourceProperty]
		public TournamentParticipantVM Participant4
		{
			get
			{
				return this._participant4;
			}
			set
			{
				if (value != this._participant4)
				{
					this._participant4 = value;
					base.OnPropertyChangedWithValue<TournamentParticipantVM>(value, "Participant4");
				}
			}
		}

		[DataSourceProperty]
		public TournamentParticipantVM Participant5
		{
			get
			{
				return this._participant5;
			}
			set
			{
				if (value != this._participant5)
				{
					this._participant5 = value;
					base.OnPropertyChangedWithValue<TournamentParticipantVM>(value, "Participant5");
				}
			}
		}

		[DataSourceProperty]
		public TournamentParticipantVM Participant6
		{
			get
			{
				return this._participant6;
			}
			set
			{
				if (value != this._participant6)
				{
					this._participant6 = value;
					base.OnPropertyChangedWithValue<TournamentParticipantVM>(value, "Participant6");
				}
			}
		}

		[DataSourceProperty]
		public TournamentParticipantVM Participant7
		{
			get
			{
				return this._participant7;
			}
			set
			{
				if (value != this._participant7)
				{
					this._participant7 = value;
					base.OnPropertyChangedWithValue<TournamentParticipantVM>(value, "Participant7");
				}
			}
		}

		[DataSourceProperty]
		public TournamentParticipantVM Participant8
		{
			get
			{
				return this._participant8;
			}
			set
			{
				if (value != this._participant8)
				{
					this._participant8 = value;
					base.OnPropertyChangedWithValue<TournamentParticipantVM>(value, "Participant8");
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

		public void Initialize()
		{
			this.IsValid = this._team != null;
			for (int i = 0; i < this.Count; i++)
			{
				TournamentParticipant tournamentParticipant = this._team.Participants.ElementAtOrDefault(i);
				this.Participants[i].Refresh(tournamentParticipant, Color.FromUint(this._team.TeamColor));
			}
		}

		public void Initialize(TournamentTeam team)
		{
			this._team = team;
			this.Count = team.TeamSize;
			this.IsValid = this._team != null;
			this.Initialize();
		}

		public void Refresh()
		{
			this.IsValid = this._team != null;
			base.OnPropertyChanged("Count");
			int num = 0;
			foreach (TournamentParticipantVM tournamentParticipantVM in this.Participants.Where((TournamentParticipantVM p) => p.IsValid))
			{
				base.OnPropertyChanged("Participant" + num);
				tournamentParticipantVM.Refresh();
				num++;
			}
		}

		public IEnumerable<TournamentParticipantVM> GetParticipants()
		{
			if (this.Participant1.IsValid)
			{
				yield return this.Participant1;
			}
			if (this.Participant2.IsValid)
			{
				yield return this.Participant2;
			}
			if (this.Participant3.IsValid)
			{
				yield return this.Participant3;
			}
			if (this.Participant4.IsValid)
			{
				yield return this.Participant4;
			}
			if (this.Participant5.IsValid)
			{
				yield return this.Participant5;
			}
			if (this.Participant6.IsValid)
			{
				yield return this.Participant6;
			}
			if (this.Participant7.IsValid)
			{
				yield return this.Participant7;
			}
			if (this.Participant8.IsValid)
			{
				yield return this.Participant8;
			}
			yield break;
		}

		private TournamentTeam _team;

		private int _count = -1;

		private TournamentParticipantVM _participant1;

		private TournamentParticipantVM _participant2;

		private TournamentParticipantVM _participant3;

		private TournamentParticipantVM _participant4;

		private TournamentParticipantVM _participant5;

		private TournamentParticipantVM _participant6;

		private TournamentParticipantVM _participant7;

		private TournamentParticipantVM _participant8;

		private int _score;

		private bool _isValid;
	}
}
