using System;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection.Scoreboard
{
	public class MPEndOfBattleSideVM : ViewModel
	{
		public MissionScoreboardComponent.MissionScoreboardSide Side { get; private set; }

		public MPEndOfBattleSideVM(MissionScoreboardComponent missionScoreboardComponent, MissionScoreboardComponent.MissionScoreboardSide side, BasicCultureObject culture, bool useSecondary)
		{
			this._missionScoreboardComponent = missionScoreboardComponent;
			this.Side = side;
			this._culture = culture;
			if (this.Side != null)
			{
				this.CultureId = culture.StringId;
				this.Score = this.Side.SideScore;
				this.IsRoundWinner = this._missionScoreboardComponent.RoundWinner == side.Side || this._missionScoreboardComponent.RoundWinner == -1;
			}
			this.CultureColor1 = Color.FromUint(useSecondary ? culture.Color2 : culture.Color);
			this.CultureColor2 = Color.FromUint(useSecondary ? culture.Color : culture.Color2);
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			if (this.Side != null)
			{
				this.CultureId = this._culture.StringId;
			}
		}

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

		[DataSourceProperty]
		public Color CultureColor1
		{
			get
			{
				return this._cultureColor1;
			}
			set
			{
				if (value != this._cultureColor1)
				{
					this._cultureColor1 = value;
					base.OnPropertyChangedWithValue(value, "CultureColor1");
				}
			}
		}

		[DataSourceProperty]
		public Color CultureColor2
		{
			get
			{
				return this._cultureColor2;
			}
			set
			{
				if (value != this._cultureColor2)
				{
					this._cultureColor2 = value;
					base.OnPropertyChangedWithValue(value, "CultureColor2");
				}
			}
		}

		private MissionScoreboardComponent _missionScoreboardComponent;

		private BasicCultureObject _culture;

		private string _factionName;

		private string _cultureId;

		private int _score;

		private bool _isRoundWinner;

		private Color _cultureColor1;

		private Color _cultureColor2;
	}
}
