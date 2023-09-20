using System;
using TaleWorlds.Library;

namespace TaleWorlds.Core.ViewModelCollection.Tutorial
{
	public class ElementNotificationVM : ViewModel
	{
		[DataSourceProperty]
		public string ElementID
		{
			get
			{
				return this._elementID;
			}
			set
			{
				if (value != this._elementID)
				{
					this._elementID = value;
					base.OnPropertyChangedWithValue<string>(value, "ElementID");
				}
			}
		}

		private string _elementID = string.Empty;
	}
}
