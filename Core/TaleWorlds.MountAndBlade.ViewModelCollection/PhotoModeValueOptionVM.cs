using System;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade.ViewModelCollection
{
	// Token: 0x0200000A RID: 10
	public class PhotoModeValueOptionVM : ViewModel
	{
		// Token: 0x060000BD RID: 189 RVA: 0x0000470C File Offset: 0x0000290C
		public PhotoModeValueOptionVM(TextObject valueNameTextObj, float min, float max, float currentValue, Action<float> onChange)
		{
			this.MinValue = min;
			this.MaxValue = max;
			this._valueNameTextObj = valueNameTextObj;
			this._onChange = onChange;
			this._currentValue = currentValue;
			this.CurrentValueText = currentValue.ToString("0.0");
			this.RefreshValues();
		}

		// Token: 0x060000BE RID: 190 RVA: 0x0000475C File Offset: 0x0000295C
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.ValueName = this._valueNameTextObj.ToString();
		}

		// Token: 0x1700003C RID: 60
		// (get) Token: 0x060000BF RID: 191 RVA: 0x00004775 File Offset: 0x00002975
		// (set) Token: 0x060000C0 RID: 192 RVA: 0x0000477D File Offset: 0x0000297D
		[DataSourceProperty]
		public float MinValue
		{
			get
			{
				return this._minValue;
			}
			set
			{
				if (value != this._minValue)
				{
					this._minValue = value;
					base.OnPropertyChangedWithValue(value, "MinValue");
				}
			}
		}

		// Token: 0x1700003D RID: 61
		// (get) Token: 0x060000C1 RID: 193 RVA: 0x0000479B File Offset: 0x0000299B
		// (set) Token: 0x060000C2 RID: 194 RVA: 0x000047A3 File Offset: 0x000029A3
		[DataSourceProperty]
		public float MaxValue
		{
			get
			{
				return this._maxValue;
			}
			set
			{
				if (value != this._maxValue)
				{
					this._maxValue = value;
					base.OnPropertyChangedWithValue(value, "MaxValue");
				}
			}
		}

		// Token: 0x1700003E RID: 62
		// (get) Token: 0x060000C3 RID: 195 RVA: 0x000047C1 File Offset: 0x000029C1
		// (set) Token: 0x060000C4 RID: 196 RVA: 0x000047CC File Offset: 0x000029CC
		[DataSourceProperty]
		public float CurrentValue
		{
			get
			{
				return this._currentValue;
			}
			set
			{
				if (value != this._currentValue)
				{
					this._currentValue = value;
					base.OnPropertyChangedWithValue(value, "CurrentValue");
					Action<float> onChange = this._onChange;
					if (onChange != null)
					{
						onChange(value);
					}
					this.CurrentValueText = value.ToString("0.0");
				}
			}
		}

		// Token: 0x1700003F RID: 63
		// (get) Token: 0x060000C5 RID: 197 RVA: 0x00004819 File Offset: 0x00002A19
		// (set) Token: 0x060000C6 RID: 198 RVA: 0x00004821 File Offset: 0x00002A21
		[DataSourceProperty]
		public string CurrentValueText
		{
			get
			{
				return this._currentValueText;
			}
			set
			{
				if (value != this._currentValueText)
				{
					this._currentValueText = value;
					base.OnPropertyChangedWithValue<string>(value, "CurrentValueText");
				}
			}
		}

		// Token: 0x17000040 RID: 64
		// (get) Token: 0x060000C7 RID: 199 RVA: 0x00004844 File Offset: 0x00002A44
		// (set) Token: 0x060000C8 RID: 200 RVA: 0x0000484C File Offset: 0x00002A4C
		[DataSourceProperty]
		public string ValueName
		{
			get
			{
				return this._valueName;
			}
			set
			{
				if (value != this._valueName)
				{
					this._valueName = value;
					base.OnPropertyChangedWithValue<string>(value, "ValueName");
				}
			}
		}

		// Token: 0x0400004C RID: 76
		private readonly Action<float> _onChange;

		// Token: 0x0400004D RID: 77
		private readonly TextObject _valueNameTextObj;

		// Token: 0x0400004E RID: 78
		private float _minValue;

		// Token: 0x0400004F RID: 79
		private float _maxValue;

		// Token: 0x04000050 RID: 80
		private float _currentValue;

		// Token: 0x04000051 RID: 81
		private string _currentValueText;

		// Token: 0x04000052 RID: 82
		private string _valueName;
	}
}
