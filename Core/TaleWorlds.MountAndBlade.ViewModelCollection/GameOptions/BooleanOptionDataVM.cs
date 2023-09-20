using System;
using TaleWorlds.Engine.Options;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.GameOptions
{
	public class BooleanOptionDataVM : GenericOptionDataVM
	{
		public BooleanOptionDataVM(OptionsVM optionsVM, IBooleanOptionData option, TextObject name, TextObject description)
			: base(optionsVM, option, name, description, OptionsVM.OptionsDataType.BooleanOption)
		{
			this._booleanOptionData = option;
			this._initialValue = option.GetValue(false).Equals(1f);
			this.OptionValueAsBoolean = this._initialValue;
		}

		[DataSourceProperty]
		public bool OptionValueAsBoolean
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
					base.OnPropertyChangedWithValue(value, "OptionValueAsBoolean");
					this.UpdateValue();
				}
			}
		}

		public override void UpdateValue()
		{
			this.Option.SetValue((float)(this.OptionValueAsBoolean ? 1 : 0));
			this.Option.Commit();
			this._optionsVM.SetConfig(this.Option, (float)(this.OptionValueAsBoolean ? 1 : 0));
		}

		public override void Cancel()
		{
			this.OptionValueAsBoolean = this._initialValue;
			this.UpdateValue();
		}

		public override void SetValue(float value)
		{
			this.OptionValueAsBoolean = (int)value == 1;
		}

		public override void ResetData()
		{
			this.OptionValueAsBoolean = (int)this.Option.GetDefaultValue() == 1;
		}

		public override bool IsChanged()
		{
			return this._initialValue != this.OptionValueAsBoolean;
		}

		public override void ApplyValue()
		{
			if (this._initialValue != this.OptionValueAsBoolean)
			{
				this._initialValue = this.OptionValueAsBoolean;
			}
		}

		private bool _initialValue;

		private readonly IBooleanOptionData _booleanOptionData;

		private bool _optionValue;
	}
}
