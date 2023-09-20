using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.TournamentGames;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace SandBox.ViewModelCollection.Tournament
{
	public class TournamentRoundVM : ViewModel
	{
		public TournamentRound Round { get; private set; }

		public List<TournamentMatchVM> Matches { get; }

		public TournamentRoundVM()
		{
			this.Match1 = new TournamentMatchVM();
			this.Match2 = new TournamentMatchVM();
			this.Match3 = new TournamentMatchVM();
			this.Match4 = new TournamentMatchVM();
			this.Match5 = new TournamentMatchVM();
			this.Match6 = new TournamentMatchVM();
			this.Match7 = new TournamentMatchVM();
			this.Match8 = new TournamentMatchVM();
			this.Matches = new List<TournamentMatchVM> { this.Match1, this.Match2, this.Match3, this.Match4, this.Match5, this.Match6, this.Match7, this.Match8 };
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.Matches.ForEach(delegate(TournamentMatchVM x)
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
		public string Name
		{
			get
			{
				return this._name;
			}
			set
			{
				if (value != this._name)
				{
					this._name = value;
					base.OnPropertyChangedWithValue<string>(value, "Name");
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
		public TournamentMatchVM Match1
		{
			get
			{
				return this._match1;
			}
			set
			{
				if (value != this._match1)
				{
					this._match1 = value;
					base.OnPropertyChangedWithValue<TournamentMatchVM>(value, "Match1");
				}
			}
		}

		[DataSourceProperty]
		public TournamentMatchVM Match2
		{
			get
			{
				return this._match2;
			}
			set
			{
				if (value != this._match2)
				{
					this._match2 = value;
					base.OnPropertyChangedWithValue<TournamentMatchVM>(value, "Match2");
				}
			}
		}

		[DataSourceProperty]
		public TournamentMatchVM Match3
		{
			get
			{
				return this._match3;
			}
			set
			{
				if (value != this._match3)
				{
					this._match3 = value;
					base.OnPropertyChangedWithValue<TournamentMatchVM>(value, "Match3");
				}
			}
		}

		[DataSourceProperty]
		public TournamentMatchVM Match4
		{
			get
			{
				return this._match4;
			}
			set
			{
				if (value != this._match4)
				{
					this._match4 = value;
					base.OnPropertyChangedWithValue<TournamentMatchVM>(value, "Match4");
				}
			}
		}

		[DataSourceProperty]
		public TournamentMatchVM Match5
		{
			get
			{
				return this._match5;
			}
			set
			{
				if (value != this._match5)
				{
					this._match5 = value;
					base.OnPropertyChangedWithValue<TournamentMatchVM>(value, "Match5");
				}
			}
		}

		[DataSourceProperty]
		public TournamentMatchVM Match6
		{
			get
			{
				return this._match6;
			}
			set
			{
				if (value != this._match6)
				{
					this._match6 = value;
					base.OnPropertyChangedWithValue<TournamentMatchVM>(value, "Match6");
				}
			}
		}

		[DataSourceProperty]
		public TournamentMatchVM Match7
		{
			get
			{
				return this._match7;
			}
			set
			{
				if (value != this._match7)
				{
					this._match7 = value;
					base.OnPropertyChangedWithValue<TournamentMatchVM>(value, "Match7");
				}
			}
		}

		[DataSourceProperty]
		public TournamentMatchVM Match8
		{
			get
			{
				return this._match8;
			}
			set
			{
				if (value != this._match8)
				{
					this._match8 = value;
					base.OnPropertyChangedWithValue<TournamentMatchVM>(value, "Match8");
				}
			}
		}

		public void Initialize()
		{
			for (int i = 0; i < this.Count; i++)
			{
				this.Matches[i].Initialize();
			}
		}

		public void Initialize(TournamentRound round, TextObject name)
		{
			this.IsValid = true;
			this.Round = round;
			this.Count = round.Matches.Length;
			for (int i = 0; i < round.Matches.Length; i++)
			{
				this.Matches[i].Initialize(round.Matches[i]);
			}
			this.Name = name.ToString();
		}

		public IEnumerable<TournamentParticipantVM> GetParticipants()
		{
			foreach (TournamentMatchVM tournamentMatchVM in this.Matches)
			{
				if (tournamentMatchVM.IsValid)
				{
					foreach (TournamentParticipantVM tournamentParticipantVM in tournamentMatchVM.GetParticipants())
					{
						yield return tournamentParticipantVM;
					}
					IEnumerator<TournamentParticipantVM> enumerator2 = null;
				}
			}
			List<TournamentMatchVM>.Enumerator enumerator = default(List<TournamentMatchVM>.Enumerator);
			yield break;
			yield break;
		}

		private TournamentMatchVM _match1;

		private TournamentMatchVM _match2;

		private TournamentMatchVM _match3;

		private TournamentMatchVM _match4;

		private TournamentMatchVM _match5;

		private TournamentMatchVM _match6;

		private TournamentMatchVM _match7;

		private TournamentMatchVM _match8;

		private int _count = -1;

		private string _name;

		private bool _isValid;
	}
}
