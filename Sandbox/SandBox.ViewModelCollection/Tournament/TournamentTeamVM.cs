using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem.TournamentGames;
using TaleWorlds.Library;

namespace SandBox.ViewModelCollection.Tournament
{
	// Token: 0x0200000B RID: 11
	public class TournamentTeamVM : ViewModel
	{
		// Token: 0x17000028 RID: 40
		// (get) Token: 0x06000094 RID: 148 RVA: 0x00005BA1 File Offset: 0x00003DA1
		public List<TournamentParticipantVM> Participants { get; }

		// Token: 0x06000095 RID: 149 RVA: 0x00005BAC File Offset: 0x00003DAC
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

		// Token: 0x06000096 RID: 150 RVA: 0x00005C89 File Offset: 0x00003E89
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.Participants.ForEach(delegate(TournamentParticipantVM x)
			{
				x.RefreshValues();
			});
		}

		// Token: 0x17000029 RID: 41
		// (get) Token: 0x06000097 RID: 151 RVA: 0x00005CBB File Offset: 0x00003EBB
		// (set) Token: 0x06000098 RID: 152 RVA: 0x00005CC3 File Offset: 0x00003EC3
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

		// Token: 0x1700002A RID: 42
		// (get) Token: 0x06000099 RID: 153 RVA: 0x00005CE1 File Offset: 0x00003EE1
		// (set) Token: 0x0600009A RID: 154 RVA: 0x00005CE9 File Offset: 0x00003EE9
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

		// Token: 0x1700002B RID: 43
		// (get) Token: 0x0600009B RID: 155 RVA: 0x00005D07 File Offset: 0x00003F07
		// (set) Token: 0x0600009C RID: 156 RVA: 0x00005D0F File Offset: 0x00003F0F
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

		// Token: 0x1700002C RID: 44
		// (get) Token: 0x0600009D RID: 157 RVA: 0x00005D2D File Offset: 0x00003F2D
		// (set) Token: 0x0600009E RID: 158 RVA: 0x00005D35 File Offset: 0x00003F35
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

		// Token: 0x1700002D RID: 45
		// (get) Token: 0x0600009F RID: 159 RVA: 0x00005D53 File Offset: 0x00003F53
		// (set) Token: 0x060000A0 RID: 160 RVA: 0x00005D5B File Offset: 0x00003F5B
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

		// Token: 0x1700002E RID: 46
		// (get) Token: 0x060000A1 RID: 161 RVA: 0x00005D79 File Offset: 0x00003F79
		// (set) Token: 0x060000A2 RID: 162 RVA: 0x00005D81 File Offset: 0x00003F81
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

		// Token: 0x1700002F RID: 47
		// (get) Token: 0x060000A3 RID: 163 RVA: 0x00005D9F File Offset: 0x00003F9F
		// (set) Token: 0x060000A4 RID: 164 RVA: 0x00005DA7 File Offset: 0x00003FA7
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

		// Token: 0x17000030 RID: 48
		// (get) Token: 0x060000A5 RID: 165 RVA: 0x00005DC5 File Offset: 0x00003FC5
		// (set) Token: 0x060000A6 RID: 166 RVA: 0x00005DCD File Offset: 0x00003FCD
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

		// Token: 0x17000031 RID: 49
		// (get) Token: 0x060000A7 RID: 167 RVA: 0x00005DEB File Offset: 0x00003FEB
		// (set) Token: 0x060000A8 RID: 168 RVA: 0x00005DF3 File Offset: 0x00003FF3
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

		// Token: 0x17000032 RID: 50
		// (get) Token: 0x060000A9 RID: 169 RVA: 0x00005E11 File Offset: 0x00004011
		// (set) Token: 0x060000AA RID: 170 RVA: 0x00005E19 File Offset: 0x00004019
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

		// Token: 0x17000033 RID: 51
		// (get) Token: 0x060000AB RID: 171 RVA: 0x00005E37 File Offset: 0x00004037
		// (set) Token: 0x060000AC RID: 172 RVA: 0x00005E3F File Offset: 0x0000403F
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

		// Token: 0x060000AD RID: 173 RVA: 0x00005E60 File Offset: 0x00004060
		public void Initialize()
		{
			this.IsValid = this._team != null;
			for (int i = 0; i < this.Count; i++)
			{
				TournamentParticipant tournamentParticipant = this._team.Participants.ElementAtOrDefault(i);
				this.Participants[i].Refresh(tournamentParticipant, Color.FromUint(this._team.TeamColor));
			}
		}

		// Token: 0x060000AE RID: 174 RVA: 0x00005EC1 File Offset: 0x000040C1
		public void Initialize(TournamentTeam team)
		{
			this._team = team;
			this.Count = team.TeamSize;
			this.IsValid = this._team != null;
			this.Initialize();
		}

		// Token: 0x060000AF RID: 175 RVA: 0x00005EEC File Offset: 0x000040EC
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

		// Token: 0x060000B0 RID: 176 RVA: 0x00005F90 File Offset: 0x00004190
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

		// Token: 0x04000035 RID: 53
		private TournamentTeam _team;

		// Token: 0x04000037 RID: 55
		private int _count = -1;

		// Token: 0x04000038 RID: 56
		private TournamentParticipantVM _participant1;

		// Token: 0x04000039 RID: 57
		private TournamentParticipantVM _participant2;

		// Token: 0x0400003A RID: 58
		private TournamentParticipantVM _participant3;

		// Token: 0x0400003B RID: 59
		private TournamentParticipantVM _participant4;

		// Token: 0x0400003C RID: 60
		private TournamentParticipantVM _participant5;

		// Token: 0x0400003D RID: 61
		private TournamentParticipantVM _participant6;

		// Token: 0x0400003E RID: 62
		private TournamentParticipantVM _participant7;

		// Token: 0x0400003F RID: 63
		private TournamentParticipantVM _participant8;

		// Token: 0x04000040 RID: 64
		private int _score;

		// Token: 0x04000041 RID: 65
		private bool _isValid;
	}
}
