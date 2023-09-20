using System;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Order
{
	public class OrderTroopItemFilterVM : ViewModel
	{
		public OrderTroopItemFilterVM(int filterTypeValue)
		{
			this.FilterTypeValue = filterTypeValue;
		}

		[DataSourceProperty]
		public int FilterTypeValue
		{
			get
			{
				return this._filterTypeValue;
			}
			set
			{
				if (value != this._filterTypeValue)
				{
					this._filterTypeValue = value;
					base.OnPropertyChangedWithValue(value, "FilterTypeValue");
				}
			}
		}

		private int _filterTypeValue;
	}
}
