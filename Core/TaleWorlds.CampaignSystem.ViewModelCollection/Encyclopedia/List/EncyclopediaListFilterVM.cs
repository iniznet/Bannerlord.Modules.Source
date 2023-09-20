using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.Encyclopedia;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia.List
{
	// Token: 0x020000BC RID: 188
	public class EncyclopediaListFilterVM : ViewModel
	{
		// Token: 0x0600129E RID: 4766 RVA: 0x0004854E File Offset: 0x0004674E
		public EncyclopediaListFilterVM(EncyclopediaFilterItem filter, Action<EncyclopediaListFilterVM> UpdateFilters)
		{
			this.Filter = filter;
			this._isSelected = this.Filter.IsActive;
			this._updateFilters = UpdateFilters;
			this.RefreshValues();
		}

		// Token: 0x0600129F RID: 4767 RVA: 0x0004857B File Offset: 0x0004677B
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.Name = this.Filter.Name.ToString();
		}

		// Token: 0x060012A0 RID: 4768 RVA: 0x00048599 File Offset: 0x00046799
		public void CopyFilterFrom(Dictionary<EncyclopediaFilterItem, bool> filters)
		{
			if (filters.ContainsKey(this.Filter))
			{
				this.IsSelected = filters[this.Filter];
			}
		}

		// Token: 0x060012A1 RID: 4769 RVA: 0x000485BB File Offset: 0x000467BB
		public void ExecuteOnFilterActivated()
		{
			Game.Current.EventManager.TriggerEvent<OnEncyclopediaFilterActivatedEvent>(new OnEncyclopediaFilterActivatedEvent());
		}

		// Token: 0x1700063B RID: 1595
		// (get) Token: 0x060012A2 RID: 4770 RVA: 0x000485D1 File Offset: 0x000467D1
		// (set) Token: 0x060012A3 RID: 4771 RVA: 0x000485D9 File Offset: 0x000467D9
		[DataSourceProperty]
		public bool IsSelected
		{
			get
			{
				return this._isSelected;
			}
			set
			{
				if (value != this._isSelected)
				{
					this._isSelected = value;
					base.OnPropertyChangedWithValue(value, "IsSelected");
					this.Filter.IsActive = value;
					this._updateFilters(this);
				}
			}
		}

		// Token: 0x1700063C RID: 1596
		// (get) Token: 0x060012A4 RID: 4772 RVA: 0x0004860F File Offset: 0x0004680F
		// (set) Token: 0x060012A5 RID: 4773 RVA: 0x00048617 File Offset: 0x00046817
		[DataSourceProperty]
		public string Name
		{
			get
			{
				return this._name;
			}
			set
			{
				if (value != this._name)
				{
					this._name = value;
					base.OnPropertyChangedWithValue<string>(value, "Name");
				}
			}
		}

		// Token: 0x040008A5 RID: 2213
		public readonly EncyclopediaFilterItem Filter;

		// Token: 0x040008A6 RID: 2214
		private readonly Action<EncyclopediaListFilterVM> _updateFilters;

		// Token: 0x040008A7 RID: 2215
		private string _name;

		// Token: 0x040008A8 RID: 2216
		private bool _isSelected;
	}
}
