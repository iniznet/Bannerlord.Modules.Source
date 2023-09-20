using System;
using TaleWorlds.Library;

namespace TaleWorlds.Core.ViewModelCollection.Generic
{
	public class StringItemWithActionVM : ViewModel
	{
		public StringItemWithActionVM(Action<object> onExecute, string item, object identifier)
		{
			this._onExecute = onExecute;
			this.Identifier = identifier;
			this.ActionText = item;
		}

		public void ExecuteAction()
		{
			this._onExecute(this.Identifier);
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

		public object Identifier;

		protected Action<object> _onExecute;

		private string _actionText;
	}
}
