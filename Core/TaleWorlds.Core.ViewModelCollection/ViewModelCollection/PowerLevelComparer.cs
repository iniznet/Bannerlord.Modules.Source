using System;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;

namespace TaleWorlds.Core.ViewModelCollection
{
	// Token: 0x0200000E RID: 14
	public class PowerLevelComparer : ViewModel
	{
		// Token: 0x06000094 RID: 148 RVA: 0x00002E8C File Offset: 0x0000108C
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

		// Token: 0x06000095 RID: 149 RVA: 0x00002F1A File Offset: 0x0000111A
		public void SetColors(string defenderColor, string attackerColor)
		{
			this.DefenderColor = defenderColor;
			this.AttackerColor = attackerColor;
		}

		// Token: 0x06000096 RID: 150 RVA: 0x00002F2A File Offset: 0x0000112A
		public void Update(double defenderPower, double attackerPower)
		{
			this.Update(defenderPower, attackerPower, this.InitialDefenderBattlePowerValue, this.InitialAttackerBattlePowerValue);
		}

		// Token: 0x06000097 RID: 151 RVA: 0x00002F40 File Offset: 0x00001140
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

		// Token: 0x17000031 RID: 49
		// (get) Token: 0x06000098 RID: 152 RVA: 0x00002FEE File Offset: 0x000011EE
		// (set) Token: 0x06000099 RID: 153 RVA: 0x00002FF6 File Offset: 0x000011F6
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

		// Token: 0x17000032 RID: 50
		// (get) Token: 0x0600009A RID: 154 RVA: 0x00003014 File Offset: 0x00001214
		// (set) Token: 0x0600009B RID: 155 RVA: 0x0000301C File Offset: 0x0000121C
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

		// Token: 0x17000033 RID: 51
		// (get) Token: 0x0600009C RID: 156 RVA: 0x0000303A File Offset: 0x0000123A
		// (set) Token: 0x0600009D RID: 157 RVA: 0x00003042 File Offset: 0x00001242
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

		// Token: 0x17000034 RID: 52
		// (get) Token: 0x0600009E RID: 158 RVA: 0x00003060 File Offset: 0x00001260
		// (set) Token: 0x0600009F RID: 159 RVA: 0x00003068 File Offset: 0x00001268
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

		// Token: 0x17000035 RID: 53
		// (get) Token: 0x060000A0 RID: 160 RVA: 0x00003086 File Offset: 0x00001286
		// (set) Token: 0x060000A1 RID: 161 RVA: 0x0000308E File Offset: 0x0000128E
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

		// Token: 0x17000036 RID: 54
		// (get) Token: 0x060000A2 RID: 162 RVA: 0x000030AC File Offset: 0x000012AC
		// (set) Token: 0x060000A3 RID: 163 RVA: 0x000030B4 File Offset: 0x000012B4
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

		// Token: 0x17000037 RID: 55
		// (get) Token: 0x060000A4 RID: 164 RVA: 0x000030D2 File Offset: 0x000012D2
		// (set) Token: 0x060000A5 RID: 165 RVA: 0x000030DA File Offset: 0x000012DA
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

		// Token: 0x17000038 RID: 56
		// (get) Token: 0x060000A6 RID: 166 RVA: 0x000030F8 File Offset: 0x000012F8
		// (set) Token: 0x060000A7 RID: 167 RVA: 0x00003100 File Offset: 0x00001300
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

		// Token: 0x17000039 RID: 57
		// (get) Token: 0x060000A8 RID: 168 RVA: 0x0000311E File Offset: 0x0000131E
		// (set) Token: 0x060000A9 RID: 169 RVA: 0x00003126 File Offset: 0x00001326
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

		// Token: 0x1700003A RID: 58
		// (get) Token: 0x060000AA RID: 170 RVA: 0x00003144 File Offset: 0x00001344
		// (set) Token: 0x060000AB RID: 171 RVA: 0x0000314C File Offset: 0x0000134C
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

		// Token: 0x1700003B RID: 59
		// (get) Token: 0x060000AC RID: 172 RVA: 0x0000316A File Offset: 0x0000136A
		// (set) Token: 0x060000AD RID: 173 RVA: 0x00003172 File Offset: 0x00001372
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

		// Token: 0x1700003C RID: 60
		// (get) Token: 0x060000AE RID: 174 RVA: 0x00003190 File Offset: 0x00001390
		// (set) Token: 0x060000AF RID: 175 RVA: 0x00003198 File Offset: 0x00001398
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

		// Token: 0x1700003D RID: 61
		// (get) Token: 0x060000B0 RID: 176 RVA: 0x000031BB File Offset: 0x000013BB
		// (set) Token: 0x060000B1 RID: 177 RVA: 0x000031C3 File Offset: 0x000013C3
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

		// Token: 0x1700003E RID: 62
		// (get) Token: 0x060000B2 RID: 178 RVA: 0x000031E6 File Offset: 0x000013E6
		// (set) Token: 0x060000B3 RID: 179 RVA: 0x000031EE File Offset: 0x000013EE
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

		// Token: 0x04000037 RID: 55
		private double _totalStrength;

		// Token: 0x04000038 RID: 56
		private double _totalInitialStrength;

		// Token: 0x04000039 RID: 57
		private double _defenderBattlePower;

		// Token: 0x0400003A RID: 58
		private double _attackerBattlePower;

		// Token: 0x0400003B RID: 59
		private double _defenderBattlePowerValue;

		// Token: 0x0400003C RID: 60
		private double _attackerBattlePowerValue;

		// Token: 0x0400003D RID: 61
		private double _initialDefenderBattlePower;

		// Token: 0x0400003E RID: 62
		private double _initialAttackerBattlePower;

		// Token: 0x0400003F RID: 63
		private double _initialDefenderBattlePowerValue;

		// Token: 0x04000040 RID: 64
		private double _initialAttackerBattlePowerValue;

		// Token: 0x04000041 RID: 65
		private float _defenderRelativePower;

		// Token: 0x04000042 RID: 66
		private float _attackerRelativePower;

		// Token: 0x04000043 RID: 67
		private string _defenderColor = "#5E8C23FF";

		// Token: 0x04000044 RID: 68
		private string _attackerColor = "#A0341EFF";

		// Token: 0x04000045 RID: 69
		private bool _isEnabled = true;

		// Token: 0x04000046 RID: 70
		private HintViewModel _hint;
	}
}
