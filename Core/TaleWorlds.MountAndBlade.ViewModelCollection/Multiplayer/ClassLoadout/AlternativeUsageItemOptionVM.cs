using System;
using TaleWorlds.Core.ViewModelCollection.Selector;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.ClassLoadout
{
	// Token: 0x020000C7 RID: 199
	public class AlternativeUsageItemOptionVM : SelectorItemVM
	{
		// Token: 0x060012AC RID: 4780 RVA: 0x0003D6F6 File Offset: 0x0003B8F6
		public AlternativeUsageItemOptionVM(string usageType, TextObject s, TextObject hint, SelectorVM<AlternativeUsageItemOptionVM> parentSelector, int index)
			: base(s, hint)
		{
			this.UsageType = usageType;
			this._index = index;
			this._parentSelector = parentSelector;
		}

		// Token: 0x060012AD RID: 4781 RVA: 0x0003D717 File Offset: 0x0003B917
		private void ExecuteSelection()
		{
			this._parentSelector.SelectedIndex = this._index;
		}

		// Token: 0x1700061B RID: 1563
		// (get) Token: 0x060012AE RID: 4782 RVA: 0x0003D72A File Offset: 0x0003B92A
		// (set) Token: 0x060012AF RID: 4783 RVA: 0x0003D732 File Offset: 0x0003B932
		[DataSourceProperty]
		public string UsageType
		{
			get
			{
				return this._usageType;
			}
			set
			{
				if (value != this._usageType)
				{
					this._usageType = value;
					base.OnPropertyChangedWithValue<string>(value, "UsageType");
				}
			}
		}

		// Token: 0x040008F4 RID: 2292
		private int _index;

		// Token: 0x040008F5 RID: 2293
		private SelectorVM<AlternativeUsageItemOptionVM> _parentSelector;

		// Token: 0x040008F6 RID: 2294
		private string _usageType;
	}
}
