using System;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;

namespace TaleWorlds.Core.ViewModelCollection.Generic
{
	public class StringPairItemWithActionVM : ViewModel
	{
		public StringPairItemWithActionVM(Action<object> onExecute, string definition, string value, object identifier)
		{
			this._onExecute = onExecute;
			this.Identifier = identifier;
			this.Definition = definition;
			this.Value = value;
			this.Hint = new HintViewModel();
			this.IsEnabled = true;
		}

		public void ExecuteAction()
		{
			this._onExecute(this.Identifier);
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

		public object Identifier;

		protected Action<object> _onExecute;

		private string _definition;

		private string _value;

		private HintViewModel _hint;

		private bool _isEnabled;
	}
}
