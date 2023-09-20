using System;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Scoreboard
{
	// Token: 0x02000054 RID: 84
	public class MPEndOfBattleSideVM : ViewModel
	{
		// Token: 0x17000227 RID: 551
		// (get) Token: 0x06000717 RID: 1815 RVA: 0x0001C987 File Offset: 0x0001AB87
		// (set) Token: 0x06000718 RID: 1816 RVA: 0x0001C98F File Offset: 0x0001AB8F
		public MissionScoreboardComponent.MissionScoreboardSide Side { get; private set; }

		// Token: 0x06000719 RID: 1817 RVA: 0x0001C998 File Offset: 0x0001AB98
		public MPEndOfBattleSideVM(MissionScoreboardComponent missionScoreboardComponent, MissionScoreboardComponent.MissionScoreboardSide side, BasicCultureObject culture)
		{
			this._missionScoreboardComponent = missionScoreboardComponent;
			this.Side = side;
			this._culture = culture;
			if (this.Side != null)
			{
				this.CultureId = culture.StringId;
				this.Score = this.Side.SideScore;
				this.IsRoundWinner = this._missionScoreboardComponent.RoundWinner == side.Side || this._missionScoreboardComponent.RoundWinner == BattleSideEnum.None;
			}
			this.RefreshValues();
		}

		// Token: 0x0600071A RID: 1818 RVA: 0x0001CA15 File Offset: 0x0001AC15
		public override void RefreshValues()
		{
			base.RefreshValues();
			if (this.Side != null)
			{
				this.CultureId = this._culture.StringId;
			}
		}

		// Token: 0x17000228 RID: 552
		// (get) Token: 0x0600071B RID: 1819 RVA: 0x0001CA36 File Offset: 0x0001AC36
		// (set) Token: 0x0600071C RID: 1820 RVA: 0x0001CA3E File Offset: 0x0001AC3E
		[DataSourceProperty]
		public string FactionName
		{
			get
			{
				return this._factionName;
			}
			set
			{
				if (value != this._factionName)
				{
					this._factionName = value;
					base.OnPropertyChangedWithValue<string>(value, "FactionName");
				}
			}
		}

		// Token: 0x17000229 RID: 553
		// (get) Token: 0x0600071D RID: 1821 RVA: 0x0001CA61 File Offset: 0x0001AC61
		// (set) Token: 0x0600071E RID: 1822 RVA: 0x0001CA69 File Offset: 0x0001AC69
		[DataSourceProperty]
		public string CultureId
		{
			get
			{
				return this._cultureId;
			}
			set
			{
				if (value != this._cultureId)
				{
					this._cultureId = value;
					base.OnPropertyChangedWithValue<string>(value, "CultureId");
				}
			}
		}

		// Token: 0x1700022A RID: 554
		// (get) Token: 0x0600071F RID: 1823 RVA: 0x0001CA8C File Offset: 0x0001AC8C
		// (set) Token: 0x06000720 RID: 1824 RVA: 0x0001CA94 File Offset: 0x0001AC94
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

		// Token: 0x1700022B RID: 555
		// (get) Token: 0x06000721 RID: 1825 RVA: 0x0001CAB2 File Offset: 0x0001ACB2
		// (set) Token: 0x06000722 RID: 1826 RVA: 0x0001CABA File Offset: 0x0001ACBA
		[DataSourceProperty]
		public bool IsRoundWinner
		{
			get
			{
				return this._isRoundWinner;
			}
			set
			{
				if (value != this._isRoundWinner)
				{
					this._isRoundWinner = value;
					base.OnPropertyChangedWithValue(value, "IsRoundWinner");
				}
			}
		}

		// Token: 0x1700022C RID: 556
		// (get) Token: 0x06000723 RID: 1827 RVA: 0x0001CAD8 File Offset: 0x0001ACD8
		// (set) Token: 0x06000724 RID: 1828 RVA: 0x0001CAE0 File Offset: 0x0001ACE0
		[DataSourceProperty]
		public bool UseSecondary
		{
			get
			{
				return this._useSecondary;
			}
			set
			{
				if (value != this._useSecondary)
				{
					this._useSecondary = value;
					base.OnPropertyChangedWithValue(value, "UseSecondary");
				}
			}
		}

		// Token: 0x0400039B RID: 923
		private MissionScoreboardComponent _missionScoreboardComponent;

		// Token: 0x0400039C RID: 924
		private BasicCultureObject _culture;

		// Token: 0x0400039D RID: 925
		private string _factionName;

		// Token: 0x0400039E RID: 926
		private string _cultureId;

		// Token: 0x0400039F RID: 927
		private int _score;

		// Token: 0x040003A0 RID: 928
		private bool _isRoundWinner;

		// Token: 0x040003A1 RID: 929
		private bool _useSecondary;
	}
}
