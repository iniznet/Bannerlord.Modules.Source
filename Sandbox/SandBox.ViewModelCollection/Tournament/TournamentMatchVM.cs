using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem.TournamentGames;
using TaleWorlds.Library;

namespace SandBox.ViewModelCollection.Tournament
{
	// Token: 0x02000008 RID: 8
	public class TournamentMatchVM : ViewModel
	{
		// Token: 0x17000006 RID: 6
		// (get) Token: 0x06000040 RID: 64 RVA: 0x00004FB1 File Offset: 0x000031B1
		// (set) Token: 0x06000041 RID: 65 RVA: 0x00004FB9 File Offset: 0x000031B9
		public TournamentMatch Match { get; private set; }

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x06000042 RID: 66 RVA: 0x00004FC2 File Offset: 0x000031C2
		public List<TournamentTeamVM> Teams { get; }

		// Token: 0x06000043 RID: 67 RVA: 0x00004FCC File Offset: 0x000031CC
		public TournamentMatchVM()
		{
			this.Team1 = new TournamentTeamVM();
			this.Team2 = new TournamentTeamVM();
			this.Team3 = new TournamentTeamVM();
			this.Team4 = new TournamentTeamVM();
			this.Teams = new List<TournamentTeamVM> { this.Team1, this.Team2, this.Team3, this.Team4 };
		}

		// Token: 0x06000044 RID: 68 RVA: 0x00005054 File Offset: 0x00003254
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.Teams.ForEach(delegate(TournamentTeamVM x)
			{
				x.RefreshValues();
			});
		}

		// Token: 0x06000045 RID: 69 RVA: 0x00005088 File Offset: 0x00003288
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

		// Token: 0x06000046 RID: 70 RVA: 0x000050E4 File Offset: 0x000032E4
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

		// Token: 0x06000047 RID: 71 RVA: 0x00005170 File Offset: 0x00003370
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

		// Token: 0x06000048 RID: 72 RVA: 0x00005228 File Offset: 0x00003428
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

		// Token: 0x06000049 RID: 73 RVA: 0x00005288 File Offset: 0x00003488
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

		// Token: 0x0600004A RID: 74 RVA: 0x00005324 File Offset: 0x00003524
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

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x0600004B RID: 75 RVA: 0x000053B0 File Offset: 0x000035B0
		// (set) Token: 0x0600004C RID: 76 RVA: 0x000053B8 File Offset: 0x000035B8
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

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x0600004D RID: 77 RVA: 0x000053D6 File Offset: 0x000035D6
		// (set) Token: 0x0600004E RID: 78 RVA: 0x000053DE File Offset: 0x000035DE
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

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x0600004F RID: 79 RVA: 0x000053FC File Offset: 0x000035FC
		// (set) Token: 0x06000050 RID: 80 RVA: 0x00005404 File Offset: 0x00003604
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

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x06000051 RID: 81 RVA: 0x00005422 File Offset: 0x00003622
		// (set) Token: 0x06000052 RID: 82 RVA: 0x0000542A File Offset: 0x0000362A
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

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x06000053 RID: 83 RVA: 0x00005448 File Offset: 0x00003648
		// (set) Token: 0x06000054 RID: 84 RVA: 0x00005450 File Offset: 0x00003650
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

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x06000055 RID: 85 RVA: 0x0000546E File Offset: 0x0000366E
		// (set) Token: 0x06000056 RID: 86 RVA: 0x00005476 File Offset: 0x00003676
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

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x06000057 RID: 87 RVA: 0x00005494 File Offset: 0x00003694
		// (set) Token: 0x06000058 RID: 88 RVA: 0x0000549C File Offset: 0x0000369C
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

		// Token: 0x04000014 RID: 20
		private TournamentTeamVM _team1;

		// Token: 0x04000015 RID: 21
		private TournamentTeamVM _team2;

		// Token: 0x04000016 RID: 22
		private TournamentTeamVM _team3;

		// Token: 0x04000017 RID: 23
		private TournamentTeamVM _team4;

		// Token: 0x04000018 RID: 24
		private int _count = -1;

		// Token: 0x04000019 RID: 25
		private int _state = -1;

		// Token: 0x0400001A RID: 26
		private bool _isValid;

		// Token: 0x0200004B RID: 75
		public enum TournamentMatchState
		{
			// Token: 0x04000279 RID: 633
			Unfinished,
			// Token: 0x0400027A RID: 634
			Current,
			// Token: 0x0400027B RID: 635
			Over,
			// Token: 0x0400027C RID: 636
			Active
		}
	}
}
