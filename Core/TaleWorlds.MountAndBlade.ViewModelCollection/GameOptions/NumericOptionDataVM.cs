using System;
using TaleWorlds.Engine.Options;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.GameOptions
{
	// Token: 0x020000FA RID: 250
	public class NumericOptionDataVM : GenericOptionDataVM
	{
		// Token: 0x06001604 RID: 5636 RVA: 0x00046BE0 File Offset: 0x00044DE0
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

		// Token: 0x06001605 RID: 5637 RVA: 0x00046C74 File Offset: 0x00044E74
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

		// Token: 0x1700073A RID: 1850
		// (get) Token: 0x06001606 RID: 5638 RVA: 0x00046D01 File Offset: 0x00044F01
		// (set) Token: 0x06001607 RID: 5639 RVA: 0x00046D09 File Offset: 0x00044F09
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

		// Token: 0x1700073B RID: 1851
		// (get) Token: 0x06001608 RID: 5640 RVA: 0x00046D27 File Offset: 0x00044F27
		// (set) Token: 0x06001609 RID: 5641 RVA: 0x00046D2F File Offset: 0x00044F2F
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

		// Token: 0x1700073C RID: 1852
		// (get) Token: 0x0600160A RID: 5642 RVA: 0x00046D4D File Offset: 0x00044F4D
		// (set) Token: 0x0600160B RID: 5643 RVA: 0x00046D55 File Offset: 0x00044F55
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

		// Token: 0x1700073D RID: 1853
		// (get) Token: 0x0600160C RID: 5644 RVA: 0x00046D73 File Offset: 0x00044F73
		// (set) Token: 0x0600160D RID: 5645 RVA: 0x00046D7B File Offset: 0x00044F7B
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

		// Token: 0x1700073E RID: 1854
		// (get) Token: 0x0600160E RID: 5646 RVA: 0x00046DAA File Offset: 0x00044FAA
		// (set) Token: 0x0600160F RID: 5647 RVA: 0x00046DB2 File Offset: 0x00044FB2
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

		// Token: 0x1700073F RID: 1855
		// (get) Token: 0x06001610 RID: 5648 RVA: 0x00046DD0 File Offset: 0x00044FD0
		// (set) Token: 0x06001611 RID: 5649 RVA: 0x00046DD8 File Offset: 0x00044FD8
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

		// Token: 0x17000740 RID: 1856
		// (get) Token: 0x06001612 RID: 5650 RVA: 0x00046DF6 File Offset: 0x00044FF6
		[DataSourceProperty]
		public string OptionValueAsString
		{
			get
			{
				return this.GetValueAsString();
			}
		}

		// Token: 0x06001613 RID: 5651 RVA: 0x00046DFE File Offset: 0x00044FFE
		public override void UpdateValue()
		{
			this.Option.SetValue(this.OptionValue);
			this.Option.Commit();
			this._optionsVM.SetConfig(this.Option, this.OptionValue);
		}

		// Token: 0x06001614 RID: 5652 RVA: 0x00046E33 File Offset: 0x00045033
		public override void Cancel()
		{
			this.OptionValue = this._initialValue;
			this.UpdateValue();
		}

		// Token: 0x06001615 RID: 5653 RVA: 0x00046E47 File Offset: 0x00045047
		public override void SetValue(float value)
		{
			this.OptionValue = value;
		}

		// Token: 0x06001616 RID: 5654 RVA: 0x00046E50 File Offset: 0x00045050
		public override void ResetData()
		{
			this.OptionValue = this.Option.GetDefaultValue();
		}

		// Token: 0x06001617 RID: 5655 RVA: 0x00046E63 File Offset: 0x00045063
		public override bool IsChanged()
		{
			return this._initialValue != this.OptionValue;
		}

		// Token: 0x06001618 RID: 5656 RVA: 0x00046E76 File Offset: 0x00045076
		public override void ApplyValue()
		{
			if (this._initialValue != this.OptionValue)
			{
				this._initialValue = this.OptionValue;
			}
		}

		// Token: 0x04000A7B RID: 2683
		private float _initialValue;

		// Token: 0x04000A7C RID: 2684
		private INumericOptionData _numericOptionData;

		// Token: 0x04000A7D RID: 2685
		private int _discreteIncrementInterval;

		// Token: 0x04000A7E RID: 2686
		private float _min;

		// Token: 0x04000A7F RID: 2687
		private float _max;

		// Token: 0x04000A80 RID: 2688
		private float _optionValue;

		// Token: 0x04000A81 RID: 2689
		private bool _isDiscrete;

		// Token: 0x04000A82 RID: 2690
		private bool _updateContinuously;
	}
}
