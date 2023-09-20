using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.TournamentGames;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace SandBox.ViewModelCollection.Tournament
{
	// Token: 0x0200000A RID: 10
	public class TournamentRoundVM : ViewModel
	{
		// Token: 0x1700001B RID: 27
		// (get) Token: 0x06000076 RID: 118 RVA: 0x0000582D File Offset: 0x00003A2D
		// (set) Token: 0x06000077 RID: 119 RVA: 0x00005835 File Offset: 0x00003A35
		public TournamentRound Round { get; private set; }

		// Token: 0x1700001C RID: 28
		// (get) Token: 0x06000078 RID: 120 RVA: 0x0000583E File Offset: 0x00003A3E
		public List<TournamentMatchVM> Matches { get; }

		// Token: 0x06000079 RID: 121 RVA: 0x00005848 File Offset: 0x00003A48
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

		// Token: 0x0600007A RID: 122 RVA: 0x00005925 File Offset: 0x00003B25
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.Matches.ForEach(delegate(TournamentMatchVM x)
			{
				x.RefreshValues();
			});
		}

		// Token: 0x1700001D RID: 29
		// (get) Token: 0x0600007B RID: 123 RVA: 0x00005957 File Offset: 0x00003B57
		// (set) Token: 0x0600007C RID: 124 RVA: 0x0000595F File Offset: 0x00003B5F
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

		// Token: 0x1700001E RID: 30
		// (get) Token: 0x0600007D RID: 125 RVA: 0x0000597D File Offset: 0x00003B7D
		// (set) Token: 0x0600007E RID: 126 RVA: 0x00005985 File Offset: 0x00003B85
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

		// Token: 0x1700001F RID: 31
		// (get) Token: 0x0600007F RID: 127 RVA: 0x000059A8 File Offset: 0x00003BA8
		// (set) Token: 0x06000080 RID: 128 RVA: 0x000059B0 File Offset: 0x00003BB0
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

		// Token: 0x17000020 RID: 32
		// (get) Token: 0x06000081 RID: 129 RVA: 0x000059CE File Offset: 0x00003BCE
		// (set) Token: 0x06000082 RID: 130 RVA: 0x000059D6 File Offset: 0x00003BD6
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

		// Token: 0x17000021 RID: 33
		// (get) Token: 0x06000083 RID: 131 RVA: 0x000059F4 File Offset: 0x00003BF4
		// (set) Token: 0x06000084 RID: 132 RVA: 0x000059FC File Offset: 0x00003BFC
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

		// Token: 0x17000022 RID: 34
		// (get) Token: 0x06000085 RID: 133 RVA: 0x00005A1A File Offset: 0x00003C1A
		// (set) Token: 0x06000086 RID: 134 RVA: 0x00005A22 File Offset: 0x00003C22
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

		// Token: 0x17000023 RID: 35
		// (get) Token: 0x06000087 RID: 135 RVA: 0x00005A40 File Offset: 0x00003C40
		// (set) Token: 0x06000088 RID: 136 RVA: 0x00005A48 File Offset: 0x00003C48
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

		// Token: 0x17000024 RID: 36
		// (get) Token: 0x06000089 RID: 137 RVA: 0x00005A66 File Offset: 0x00003C66
		// (set) Token: 0x0600008A RID: 138 RVA: 0x00005A6E File Offset: 0x00003C6E
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

		// Token: 0x17000025 RID: 37
		// (get) Token: 0x0600008B RID: 139 RVA: 0x00005A8C File Offset: 0x00003C8C
		// (set) Token: 0x0600008C RID: 140 RVA: 0x00005A94 File Offset: 0x00003C94
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

		// Token: 0x17000026 RID: 38
		// (get) Token: 0x0600008D RID: 141 RVA: 0x00005AB2 File Offset: 0x00003CB2
		// (set) Token: 0x0600008E RID: 142 RVA: 0x00005ABA File Offset: 0x00003CBA
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

		// Token: 0x17000027 RID: 39
		// (get) Token: 0x0600008F RID: 143 RVA: 0x00005AD8 File Offset: 0x00003CD8
		// (set) Token: 0x06000090 RID: 144 RVA: 0x00005AE0 File Offset: 0x00003CE0
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

		// Token: 0x06000091 RID: 145 RVA: 0x00005B00 File Offset: 0x00003D00
		public void Initialize()
		{
			for (int i = 0; i < this.Count; i++)
			{
				this.Matches[i].Initialize();
			}
		}

		// Token: 0x06000092 RID: 146 RVA: 0x00005B30 File Offset: 0x00003D30
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

		// Token: 0x06000093 RID: 147 RVA: 0x00005B91 File Offset: 0x00003D91
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

		// Token: 0x0400002A RID: 42
		private TournamentMatchVM _match1;

		// Token: 0x0400002B RID: 43
		private TournamentMatchVM _match2;

		// Token: 0x0400002C RID: 44
		private TournamentMatchVM _match3;

		// Token: 0x0400002D RID: 45
		private TournamentMatchVM _match4;

		// Token: 0x0400002E RID: 46
		private TournamentMatchVM _match5;

		// Token: 0x0400002F RID: 47
		private TournamentMatchVM _match6;

		// Token: 0x04000030 RID: 48
		private TournamentMatchVM _match7;

		// Token: 0x04000031 RID: 49
		private TournamentMatchVM _match8;

		// Token: 0x04000032 RID: 50
		private int _count = -1;

		// Token: 0x04000033 RID: 51
		private string _name;

		// Token: 0x04000034 RID: 52
		private bool _isValid;
	}
}
