using System;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Order
{
	// Token: 0x02000027 RID: 39
	public class OrderTroopItemFilterVM : ViewModel
	{
		// Token: 0x060002DE RID: 734 RVA: 0x0000D517 File Offset: 0x0000B717
		public OrderTroopItemFilterVM(int filterTypeValue)
		{
			this.FilterTypeValue = filterTypeValue;
		}

		// Token: 0x170000DF RID: 223
		// (get) Token: 0x060002DF RID: 735 RVA: 0x0000D526 File Offset: 0x0000B726
		// (set) Token: 0x060002E0 RID: 736 RVA: 0x0000D52E File Offset: 0x0000B72E
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

		// Token: 0x04000166 RID: 358
		private int _filterTypeValue;
	}
}
