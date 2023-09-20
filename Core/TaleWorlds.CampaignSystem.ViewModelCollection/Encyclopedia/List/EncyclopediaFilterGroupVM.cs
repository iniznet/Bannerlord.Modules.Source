using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.Encyclopedia;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia.List
{
	public class EncyclopediaFilterGroupVM : ViewModel
	{
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

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.Filters.ApplyActionOnAllItems(delegate(EncyclopediaListFilterVM x)
			{
				x.RefreshValues();
			});
			this.FilterName = this.FilterGroup.Name.ToString();
		}

		public void CopyFiltersFrom(Dictionary<EncyclopediaFilterItem, bool> filters)
		{
			this.Filters.ApplyActionOnAllItems(delegate(EncyclopediaListFilterVM x)
			{
				x.CopyFilterFrom(filters);
			});
		}

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

		public readonly EncyclopediaFilterGroup FilterGroup;

		private MBBindingList<EncyclopediaListFilterVM> _filters;

		private string _filterName;
	}
}
