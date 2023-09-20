using System;
using TaleWorlds.Library;

namespace TaleWorlds.Core.ViewModelCollection.Generic
{
	// Token: 0x0200001E RID: 30
	public class BindingListStringItem : ViewModel
	{
		// Token: 0x06000178 RID: 376 RVA: 0x00005170 File Offset: 0x00003370
		public BindingListStringItem(string value)
		{
			this.Item = value;
		}

		// Token: 0x17000077 RID: 119
		// (get) Token: 0x06000179 RID: 377 RVA: 0x0000517F File Offset: 0x0000337F
		// (set) Token: 0x0600017A RID: 378 RVA: 0x00005187 File Offset: 0x00003387
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

		// Token: 0x04000094 RID: 148
		private string _item;
	}
}
