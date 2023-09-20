using System;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;

namespace TaleWorlds.Core.ViewModelCollection.Generic
{
	public class StringPairItemVM : ViewModel
	{
		public StringPairItemVM(string definition, string value, BasicTooltipViewModel hint = null)
		{
			this.Definition = definition;
			this.Value = value;
			this.Hint = hint;
		}

		[DataSourceProperty]
		public string Definition
		{
			get
			{
				return this._definition;
			}
			set
			{
				if (value != this._definition)
				{
					this._definition = value;
					base.OnPropertyChangedWithValue<string>(value, "Definition");
				}
			}
		}

		[DataSourceProperty]
		public string Value
		{
			get
			{
				return this._value;
			}
			set
			{
				if (value != this._value)
				{
					this._value = value;
					base.OnPropertyChangedWithValue<string>(value, "Value");
				}
			}
		}

		[DataSourceProperty]
		public BasicTooltipViewModel Hint
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
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "Hint");
				}
			}
		}

		private string _definition;

		private string _value;

		private BasicTooltipViewModel _hint;
	}
}
