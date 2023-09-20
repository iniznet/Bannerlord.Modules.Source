using System;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.Core.ViewModelCollection.Generic
{
	public class StringItemWithEnabledAndHintVM : ViewModel
	{
		public StringItemWithEnabledAndHintVM(Action<object> onExecute, string item, bool enabled, object identifier, TextObject hintText = null)
		{
			this._onExecute = onExecute;
			this.Identifier = identifier;
			this.ActionText = item;
			this.IsEnabled = enabled;
			this.Hint = new HintViewModel(hintText ?? TextObject.Empty, null);
		}

		public void ExecuteAction()
		{
			if (this.IsEnabled)
			{
				this._onExecute(this.Identifier);
			}
		}

		[DataSourceProperty]
		public string ActionText
		{
			get
			{
				return this._actionText;
			}
			set
			{
				if (value != this._actionText)
				{
					this._actionText = value;
					base.OnPropertyChangedWithValue<string>(value, "ActionText");
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

		public object Identifier;

		protected Action<object> _onExecute;

		private HintViewModel _hint;

		private string _actionText;

		private bool _isEnabled;
	}
}
