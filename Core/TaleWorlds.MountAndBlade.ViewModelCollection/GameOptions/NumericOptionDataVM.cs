using System;
using TaleWorlds.Engine.Options;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.GameOptions
{
	public class NumericOptionDataVM : GenericOptionDataVM
	{
		public NumericOptionDataVM(OptionsVM optionsVM, INumericOptionData option, TextObject name, TextObject description)
			: base(optionsVM, option, name, description, OptionsVM.OptionsDataType.NumericOption)
		{
			this._numericOptionData = option;
			this._initialValue = this._numericOptionData.GetValue(false);
			this.Min = this._numericOptionData.GetMinValue();
			this.Max = this._numericOptionData.GetMaxValue();
			this.IsDiscrete = this._numericOptionData.GetIsDiscrete();
			this.DiscreteIncrementInterval = this._numericOptionData.GetDiscreteIncrementInterval();
			this.UpdateContinuously = this._numericOptionData.GetShouldUpdateContinuously();
			this.OptionValue = this._initialValue;
		}

		private string GetValueAsString()
		{
			string text = (this.IsDiscrete ? ((int)this._optionValue).ToString() : this._optionValue.ToString("F"));
			if (this._numericOptionData.IsNative() || this._numericOptionData.IsAction())
			{
				return text;
			}
			ManagedOptions.ManagedOptionsType managedOptionsType = (ManagedOptions.ManagedOptionsType)this._numericOptionData.GetOptionType();
			if (managedOptionsType != ManagedOptions.ManagedOptionsType.AutoSaveInterval)
			{
				return text;
			}
			if ((int)this.Min < (int)this._optionValue)
			{
				return text;
			}
			return new TextObject("{=1JlzQIXE}Disabled", null).ToString();
		}

		[DataSourceProperty]
		public int DiscreteIncrementInterval
		{
			get
			{
				return this._discreteIncrementInterval;
			}
			set
			{
				if (value != this._discreteIncrementInterval)
				{
					this._discreteIncrementInterval = value;
					base.OnPropertyChangedWithValue(value, "DiscreteIncrementInterval");
				}
			}
		}

		[DataSourceProperty]
		public float Min
		{
			get
			{
				return this._min;
			}
			set
			{
				if (value != this._min)
				{
					this._min = value;
					base.OnPropertyChangedWithValue(value, "Min");
				}
			}
		}

		[DataSourceProperty]
		public float Max
		{
			get
			{
				return this._max;
			}
			set
			{
				if (value != this._max)
				{
					this._max = value;
					base.OnPropertyChangedWithValue(value, "Max");
				}
			}
		}

		[DataSourceProperty]
		public float OptionValue
		{
			get
			{
				return this._optionValue;
			}
			set
			{
				if (value != this._optionValue)
				{
					this._optionValue = value;
					base.OnPropertyChangedWithValue(value, "OptionValue");
					base.OnPropertyChanged("OptionValueAsString");
					this.UpdateValue();
				}
			}
		}

		[DataSourceProperty]
		public bool IsDiscrete
		{
			get
			{
				return this._isDiscrete;
			}
			set
			{
				if (value != this._isDiscrete)
				{
					this._isDiscrete = value;
					base.OnPropertyChangedWithValue(value, "IsDiscrete");
				}
			}
		}

		[DataSourceProperty]
		public bool UpdateContinuously
		{
			get
			{
				return this._updateContinuously;
			}
			set
			{
				if (value != this._updateContinuously)
				{
					this._updateContinuously = value;
					base.OnPropertyChangedWithValue(value, "UpdateContinuously");
				}
			}
		}

		[DataSourceProperty]
		public string OptionValueAsString
		{
			get
			{
				return this.GetValueAsString();
			}
		}

		public override void UpdateValue()
		{
			this.Option.SetValue(this.OptionValue);
			this.Option.Commit();
			this._optionsVM.SetConfig(this.Option, this.OptionValue);
		}

		public override void Cancel()
		{
			this.OptionValue = this._initialValue;
			this.UpdateValue();
		}

		public override void SetValue(float value)
		{
			this.OptionValue = value;
		}

		public override void ResetData()
		{
			this.OptionValue = this.Option.GetDefaultValue();
		}

		public override bool IsChanged()
		{
			return this._initialValue != this.OptionValue;
		}

		public override void ApplyValue()
		{
			if (this._initialValue != this.OptionValue)
			{
				this._initialValue = this.OptionValue;
			}
		}

		private float _initialValue;

		private INumericOptionData _numericOptionData;

		private int _discreteIncrementInterval;

		private float _min;

		private float _max;

		private float _optionValue;

		private bool _isDiscrete;

		private bool _updateContinuously;
	}
}
