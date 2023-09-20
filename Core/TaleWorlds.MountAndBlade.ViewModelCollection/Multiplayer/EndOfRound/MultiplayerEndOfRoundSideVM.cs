using System;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.EndOfRound
{
	public class MultiplayerEndOfRoundSideVM : ViewModel
	{
		public void SetData(BasicCultureObject culture, int score, bool isWinner, bool useSecondary)
		{
			this._culture = culture;
			this.CultureID = culture.StringId;
			this.Score = score;
			this.IsWinner = isWinner;
			this.UseSecondary = useSecondary;
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.CultureName = this._culture.Name.ToString();
		}

		[DataSourceProperty]
		public bool IsWinner
		{
			get
			{
				return this._isWinner;
			}
			set
			{
				if (value != this._isWinner)
				{
					this._isWinner = value;
					base.OnPropertyChangedWithValue(value, "IsWinner");
				}
			}
		}

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

		[DataSourceProperty]
		public string CultureID
		{
			get
			{
				return this._cultureID;
			}
			set
			{
				if (value != this._cultureID)
				{
					this._cultureID = value;
					base.OnPropertyChangedWithValue<string>(value, "CultureID");
				}
			}
		}

		[DataSourceProperty]
		public string CultureName
		{
			get
			{
				return this._cultureName;
			}
			set
			{
				if (value != this._cultureName)
				{
					this._cultureName = value;
					base.OnPropertyChangedWithValue<string>(value, "CultureName");
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

		private BasicCultureObject _culture;

		private bool _isWinner;

		private bool _useSecondary;

		private string _cultureID;

		private string _cultureName;

		private int _score;
	}
}
