using System;
using TaleWorlds.Library;

namespace TaleWorlds.Core.ViewModelCollection.Information.RundownTooltip
{
	public class RundownLineVM : ViewModel
	{
		public RundownLineVM(string name, float value)
		{
			this.Name = name;
			this.ValueAsString = string.Format("{0:0.##}", value);
			this.Value = value;
		}

		[DataSourceProperty]
		public string Name
		{
			get
			{
				return this._name;
			}
			set
			{
				if (value != this._name)
				{
					this._name = value;
					base.OnPropertyChangedWithValue<string>(value, "Name");
				}
			}
		}

		[DataSourceProperty]
		public string ValueAsString
		{
			get
			{
				return this._valueAsString;
			}
			set
			{
				if (value != this._valueAsString)
				{
					this._valueAsString = value;
					base.OnPropertyChangedWithValue<string>(value, "ValueAsString");
				}
			}
		}

		[DataSourceProperty]
		public float Value
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
					base.OnPropertyChangedWithValue(value, "Value");
				}
			}
		}

		private string _name;

		private string _valueAsString;

		private float _value;
	}
}
