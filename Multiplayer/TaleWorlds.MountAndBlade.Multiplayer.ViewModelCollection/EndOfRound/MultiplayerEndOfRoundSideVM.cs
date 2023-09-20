using System;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection.EndOfRound
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
			this.CultureColor1 = Color.FromUint(this.UseSecondary ? culture.Color2 : culture.Color);
			this.CultureColor2 = Color.FromUint(this.UseSecondary ? culture.Color : culture.Color2);
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

		private Color _cultureColor1;

		private Color _cultureColor2;

		private string _cultureName;

		private int _score;
	}
}
