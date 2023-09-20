using System;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade.ViewModelCollection
{
	public class PhotoModeValueOptionVM : ViewModel
	{
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

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.ValueName = this._valueNameTextObj.ToString();
		}

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

		private readonly Action<float> _onChange;

		private readonly TextObject _valueNameTextObj;

		private float _minValue;

		private float _maxValue;

		private float _currentValue;

		private string _currentValueText;

		private string _valueName;
	}
}
