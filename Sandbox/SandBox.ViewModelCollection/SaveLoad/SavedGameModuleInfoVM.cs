using System;
using TaleWorlds.Library;

namespace SandBox.ViewModelCollection.SaveLoad
{
	public class SavedGameModuleInfoVM : ViewModel
	{
		public SavedGameModuleInfoVM(string definition, string seperator, string value)
		{
			this.Definition = definition;
			this.Seperator = seperator;
			this.Value = value;
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
		public string Seperator
		{
			get
			{
				return this._seperator;
			}
			set
			{
				if (value != this._seperator)
				{
					this._seperator = value;
					base.OnPropertyChangedWithValue<string>(value, "Seperator");
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

		private string _definition;

		private string _seperator;

		private string _value;
	}
}
