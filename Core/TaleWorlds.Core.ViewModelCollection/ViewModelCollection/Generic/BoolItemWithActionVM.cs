using System;
using TaleWorlds.Library;

namespace TaleWorlds.Core.ViewModelCollection.Generic
{
	public class BoolItemWithActionVM : ViewModel
	{
		[DataSourceProperty]
		public bool IsActive
		{
			get
			{
				return this._isActive;
			}
			set
			{
				if (value != this._isActive)
				{
					this._isActive = value;
					base.OnPropertyChangedWithValue(value, "IsActive");
				}
			}
		}

		public void ExecuteAction()
		{
			this._onExecute(this.Identifier);
		}

		public BoolItemWithActionVM(Action<object> onExecute, bool isActive, object identifier)
		{
			this._onExecute = onExecute;
			this.Identifier = identifier;
			this.IsActive = isActive;
		}

		public object Identifier;

		protected Action<object> _onExecute;

		private bool _isActive;
	}
}
