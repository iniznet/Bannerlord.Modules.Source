using System;
using TaleWorlds.Library;

namespace TaleWorlds.Core.ViewModelCollection.Generic
{
	public class BindingListStringItem : ViewModel
	{
		public BindingListStringItem(string value)
		{
			this.Item = value;
		}

		[DataSourceProperty]
		public string Item
		{
			get
			{
				return this._item;
			}
			set
			{
				if (value != this._item)
				{
					this._item = value;
					base.OnPropertyChangedWithValue<string>(value, "Item");
				}
			}
		}

		private string _item;
	}
}
