using System;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;

namespace TaleWorlds.Core.ViewModelCollection
{
	public class PowerLevelComparer : ViewModel
	{
		public PowerLevelComparer(double defenderPower, double attackerPower)
		{
			this._totalStrength = defenderPower + attackerPower;
			this._totalInitialStrength = this._totalStrength;
			this.InitialDefenderBattlePowerValue = defenderPower;
			this.InitialAttackerBattlePowerValue = attackerPower;
			this.InitialDefenderBattlePower = defenderPower / this._totalStrength;
			this.InitialAttackerBattlePower = attackerPower / this._totalStrength;
			this.Update(defenderPower, attackerPower);
			this.Hint = new HintViewModel(GameTexts.FindText("str_power_levels", null), null);
		}

		public void SetColors(string defenderColor, string attackerColor)
		{
			this.DefenderColor = defenderColor;
			this.AttackerColor = attackerColor;
		}

		public void Update(double defenderPower, double attackerPower)
		{
			this.Update(defenderPower, attackerPower, this.InitialDefenderBattlePowerValue, this.InitialAttackerBattlePowerValue);
		}

		public void Update(double defenderPower, double attackerPower, double initialDefenderPower, double initialAttackerPower)
		{
			this._totalStrength = defenderPower + attackerPower;
			this._totalInitialStrength = initialDefenderPower + initialAttackerPower;
			this.InitialDefenderBattlePower = initialDefenderPower / (initialDefenderPower + initialAttackerPower);
			this.InitialAttackerBattlePower = initialAttackerPower / (initialDefenderPower + initialAttackerPower);
			this.InitialDefenderBattlePowerValue = initialDefenderPower;
			this.InitialAttackerBattlePowerValue = initialAttackerPower;
			this.DefenderBattlePower = defenderPower / this._totalStrength;
			this.AttackerBattlePower = attackerPower / this._totalStrength;
			this.DefenderBattlePowerValue = defenderPower;
			this.AttackerBattlePowerValue = attackerPower;
			this.DefenderRelativePower = ((initialDefenderPower == 0.0) ? 0f : ((float)(defenderPower / initialDefenderPower)));
			this.AttackerRelativePower = ((initialAttackerPower == 0.0) ? 0f : ((float)(attackerPower / initialAttackerPower)));
		}

		[DataSourceProperty]
		public bool IsEnabled
		{
			get
			{
				return this._isEnabled;
			}
			set
			{
				if (value != this._isEnabled)
				{
					this._isEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsEnabled");
				}
			}
		}

		[DataSourceProperty]
		public double DefenderBattlePower
		{
			get
			{
				return this._defenderBattlePower;
			}
			set
			{
				if (value != this._defenderBattlePower)
				{
					this._defenderBattlePower = value;
					base.OnPropertyChangedWithValue(value, "DefenderBattlePower");
				}
			}
		}

		[DataSourceProperty]
		public double DefenderBattlePowerValue
		{
			get
			{
				return this._defenderBattlePowerValue;
			}
			set
			{
				if (value != this._defenderBattlePowerValue)
				{
					this._defenderBattlePowerValue = value;
					base.OnPropertyChangedWithValue(value, "DefenderBattlePowerValue");
				}
			}
		}

		[DataSourceProperty]
		public double AttackerBattlePower
		{
			get
			{
				return this._attackerBattlePower;
			}
			set
			{
				if (value != this._attackerBattlePower)
				{
					this._attackerBattlePower = value;
					base.OnPropertyChangedWithValue(value, "AttackerBattlePower");
				}
			}
		}

		[DataSourceProperty]
		public double AttackerBattlePowerValue
		{
			get
			{
				return this._attackerBattlePowerValue;
			}
			set
			{
				if (value != this._attackerBattlePowerValue)
				{
					this._attackerBattlePowerValue = value;
					base.OnPropertyChangedWithValue(value, "AttackerBattlePowerValue");
				}
			}
		}

		[DataSourceProperty]
		public double InitialDefenderBattlePower
		{
			get
			{
				return this._initialDefenderBattlePower;
			}
			set
			{
				if (value != this._initialDefenderBattlePower)
				{
					this._initialDefenderBattlePower = value;
					base.OnPropertyChangedWithValue(value, "InitialDefenderBattlePower");
				}
			}
		}

		[DataSourceProperty]
		public double InitialAttackerBattlePower
		{
			get
			{
				return this._initialAttackerBattlePower;
			}
			set
			{
				if (value != this._initialAttackerBattlePower)
				{
					this._initialAttackerBattlePower = value;
					base.OnPropertyChangedWithValue(value, "InitialAttackerBattlePower");
				}
			}
		}

		[DataSourceProperty]
		public double InitialDefenderBattlePowerValue
		{
			get
			{
				return this._initialDefenderBattlePowerValue;
			}
			set
			{
				if (value != this._initialDefenderBattlePowerValue)
				{
					this._initialDefenderBattlePowerValue = value;
					base.OnPropertyChangedWithValue(value, "InitialDefenderBattlePowerValue");
				}
			}
		}

		[DataSourceProperty]
		public double InitialAttackerBattlePowerValue
		{
			get
			{
				return this._initialAttackerBattlePowerValue;
			}
			set
			{
				if (value != this._initialAttackerBattlePowerValue)
				{
					this._initialAttackerBattlePowerValue = value;
					base.OnPropertyChangedWithValue(value, "InitialAttackerBattlePowerValue");
				}
			}
		}

		[DataSourceProperty]
		public float DefenderRelativePower
		{
			get
			{
				return this._defenderRelativePower;
			}
			set
			{
				if (value != this._defenderRelativePower)
				{
					this._defenderRelativePower = value;
					base.OnPropertyChangedWithValue(value, "DefenderRelativePower");
				}
			}
		}

		[DataSourceProperty]
		public float AttackerRelativePower
		{
			get
			{
				return this._attackerRelativePower;
			}
			set
			{
				if (value != this._attackerRelativePower)
				{
					this._attackerRelativePower = value;
					base.OnPropertyChangedWithValue(value, "AttackerRelativePower");
				}
			}
		}

		[DataSourceProperty]
		public string DefenderColor
		{
			get
			{
				return this._defenderColor;
			}
			set
			{
				if (value != this._defenderColor)
				{
					this._defenderColor = value;
					base.OnPropertyChangedWithValue<string>(value, "DefenderColor");
				}
			}
		}

		[DataSourceProperty]
		public string AttackerColor
		{
			get
			{
				return this._attackerColor;
			}
			set
			{
				if (value != this._attackerColor)
				{
					this._attackerColor = value;
					base.OnPropertyChangedWithValue<string>(value, "AttackerColor");
				}
			}
		}

		[DataSourceProperty]
		public HintViewModel Hint
		{
			get
			{
				return this._hint;
			}
			set
			{
				if (value != this._hint)
				{
					this._hint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "Hint");
				}
			}
		}

		private double _totalStrength;

		private double _totalInitialStrength;

		private double _defenderBattlePower;

		private double _attackerBattlePower;

		private double _defenderBattlePowerValue;

		private double _attackerBattlePowerValue;

		private double _initialDefenderBattlePower;

		private double _initialAttackerBattlePower;

		private double _initialDefenderBattlePowerValue;

		private double _initialAttackerBattlePowerValue;

		private float _defenderRelativePower;

		private float _attackerRelativePower;

		private string _defenderColor = "#5E8C23FF";

		private string _attackerColor = "#A0341EFF";

		private bool _isEnabled = true;

		private HintViewModel _hint;
	}
}
