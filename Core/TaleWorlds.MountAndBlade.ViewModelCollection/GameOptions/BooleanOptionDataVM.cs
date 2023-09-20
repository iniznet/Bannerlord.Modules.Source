using System;
using TaleWorlds.Engine.Options;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.GameOptions
{
	// Token: 0x020000F3 RID: 243
	public class BooleanOptionDataVM : GenericOptionDataVM
	{
		// Token: 0x06001572 RID: 5490 RVA: 0x00045890 File Offset: 0x00043A90
		public BooleanOptionDataVM(OptionsVM optionsVM, IBooleanOptionData option, TextObject name, TextObject description)
			: base(optionsVM, option, name, description, OptionsVM.OptionsDataType.BooleanOption)
		{
			this._booleanOptionData = option;
			this._initialValue = option.GetValue(false).Equals(1f);
			this.OptionValueAsBoolean = this._initialValue;
		}

		// Token: 0x1700070C RID: 1804
		// (get) Token: 0x06001573 RID: 5491 RVA: 0x000458D6 File Offset: 0x00043AD6
		// (set) Token: 0x06001574 RID: 5492 RVA: 0x000458DE File Offset: 0x00043ADE
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

		// Token: 0x06001575 RID: 5493 RVA: 0x00045904 File Offset: 0x00043B04
		public override void UpdateValue()
		{
			this.Option.SetValue((float)(this.OptionValueAsBoolean ? 1 : 0));
			this.Option.Commit();
			this._optionsVM.SetConfig(this.Option, (float)(this.OptionValueAsBoolean ? 1 : 0));
		}

		// Token: 0x06001576 RID: 5494 RVA: 0x00045952 File Offset: 0x00043B52
		public override void Cancel()
		{
			this.OptionValueAsBoolean = this._initialValue;
			this.UpdateValue();
		}

		// Token: 0x06001577 RID: 5495 RVA: 0x00045966 File Offset: 0x00043B66
		public override void SetValue(float value)
		{
			this.OptionValueAsBoolean = (int)value == 1;
		}

		// Token: 0x06001578 RID: 5496 RVA: 0x00045973 File Offset: 0x00043B73
		public override void ResetData()
		{
			this.OptionValueAsBoolean = (int)this.Option.GetDefaultValue() == 1;
		}

		// Token: 0x06001579 RID: 5497 RVA: 0x0004598A File Offset: 0x00043B8A
		public override bool IsChanged()
		{
			return this._initialValue != this.OptionValueAsBoolean;
		}

		// Token: 0x0600157A RID: 5498 RVA: 0x0004599D File Offset: 0x00043B9D
		public override void ApplyValue()
		{
			if (this._initialValue != this.OptionValueAsBoolean)
			{
				this._initialValue = this.OptionValueAsBoolean;
			}
		}

		// Token: 0x04000A40 RID: 2624
		private bool _initialValue;

		// Token: 0x04000A41 RID: 2625
		private readonly IBooleanOptionData _booleanOptionData;

		// Token: 0x04000A42 RID: 2626
		private bool _optionValue;
	}
}
