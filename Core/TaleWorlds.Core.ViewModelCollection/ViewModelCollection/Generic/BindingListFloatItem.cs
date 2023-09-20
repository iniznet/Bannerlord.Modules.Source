using System;
using TaleWorlds.Library;

namespace TaleWorlds.Core.ViewModelCollection.Generic
{
	public class BindingListFloatItem : ViewModel
	{
		public BindingListFloatItem(float value)
		{
			this.Item = value;
		}

		[DataSourceProperty]
		public float Item
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
					base.OnPropertyChangedWithValue(value, "Item");
				}
			}
		}

		private float _item;
	}
}
