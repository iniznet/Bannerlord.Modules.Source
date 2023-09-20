using System;
using TaleWorlds.Library;

namespace TaleWorlds.Core.ViewModelCollection.Generic
{
	// Token: 0x0200001D RID: 29
	public class BindingListFloatItem : ViewModel
	{
		// Token: 0x06000175 RID: 373 RVA: 0x0000513B File Offset: 0x0000333B
		public BindingListFloatItem(float value)
		{
			this.Item = value;
		}

		// Token: 0x17000076 RID: 118
		// (get) Token: 0x06000176 RID: 374 RVA: 0x0000514A File Offset: 0x0000334A
		// (set) Token: 0x06000177 RID: 375 RVA: 0x00005152 File Offset: 0x00003352
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

		// Token: 0x04000093 RID: 147
		private float _item;
	}
}
