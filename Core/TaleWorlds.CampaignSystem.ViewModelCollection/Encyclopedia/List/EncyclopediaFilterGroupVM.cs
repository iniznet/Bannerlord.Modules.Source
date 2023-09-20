using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.Encyclopedia;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia.List
{
	// Token: 0x020000BB RID: 187
	public class EncyclopediaFilterGroupVM : ViewModel
	{
		// Token: 0x06001297 RID: 4759 RVA: 0x000483FC File Offset: 0x000465FC
		public EncyclopediaFilterGroupVM(EncyclopediaFilterGroup filterGroup, Action<EncyclopediaListFilterVM> UpdateFilters)
		{
			this.FilterGroup = filterGroup;
			this.Filters = new MBBindingList<EncyclopediaListFilterVM>();
			foreach (EncyclopediaFilterItem encyclopediaFilterItem in filterGroup.Filters)
			{
				this.Filters.Add(new EncyclopediaListFilterVM(encyclopediaFilterItem, UpdateFilters));
			}
			this.RefreshValues();
		}

		// Token: 0x06001298 RID: 4760 RVA: 0x00048478 File Offset: 0x00046678
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.Filters.ApplyActionOnAllItems(delegate(EncyclopediaListFilterVM x)
			{
				x.RefreshValues();
			});
			this.FilterName = this.FilterGroup.Name.ToString();
		}

		// Token: 0x06001299 RID: 4761 RVA: 0x000484CC File Offset: 0x000466CC
		public void CopyFiltersFrom(Dictionary<EncyclopediaFilterItem, bool> filters)
		{
			this.Filters.ApplyActionOnAllItems(delegate(EncyclopediaListFilterVM x)
			{
				x.CopyFilterFrom(filters);
			});
		}

		// Token: 0x17000639 RID: 1593
		// (get) Token: 0x0600129A RID: 4762 RVA: 0x000484FD File Offset: 0x000466FD
		// (set) Token: 0x0600129B RID: 4763 RVA: 0x00048505 File Offset: 0x00046705
		[DataSourceProperty]
		public string FilterName
		{
			get
			{
				return this._filterName;
			}
			set
			{
				if (value != this._filterName)
				{
					this._filterName = value;
					base.OnPropertyChangedWithValue<string>(value, "FilterName");
				}
			}
		}

		// Token: 0x1700063A RID: 1594
		// (get) Token: 0x0600129C RID: 4764 RVA: 0x00048528 File Offset: 0x00046728
		// (set) Token: 0x0600129D RID: 4765 RVA: 0x00048530 File Offset: 0x00046730
		[DataSourceProperty]
		public MBBindingList<EncyclopediaListFilterVM> Filters
		{
			get
			{
				return this._filters;
			}
			set
			{
				if (value != this._filters)
				{
					this._filters = value;
					base.OnPropertyChangedWithValue<MBBindingList<EncyclopediaListFilterVM>>(value, "Filters");
				}
			}
		}

		// Token: 0x040008A2 RID: 2210
		public readonly EncyclopediaFilterGroup FilterGroup;

		// Token: 0x040008A3 RID: 2211
		private MBBindingList<EncyclopediaListFilterVM> _filters;

		// Token: 0x040008A4 RID: 2212
		private string _filterName;
	}
}
