using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.Encyclopedia;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia.List
{
	public class EncyclopediaListFilterVM : ViewModel
	{
		public EncyclopediaListFilterVM(EncyclopediaFilterItem filter, Action<EncyclopediaListFilterVM> UpdateFilters)
		{
			this.Filter = filter;
			this._isSelected = this.Filter.IsActive;
			this._updateFilters = UpdateFilters;
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.Name = this.Filter.Name.ToString();
		}

		public void CopyFilterFrom(Dictionary<EncyclopediaFilterItem, bool> filters)
		{
			if (filters.ContainsKey(this.Filter))
			{
				this.IsSelected = filters[this.Filter];
			}
		}

		public void ExecuteOnFilterActivated()
		{
			Game.Current.EventManager.TriggerEvent<OnEncyclopediaFilterActivatedEvent>(new OnEncyclopediaFilterActivatedEvent());
		}

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

		public readonly EncyclopediaFilterItem Filter;

		private readonly Action<EncyclopediaListFilterVM> _updateFilters;

		private string _name;

		private bool _isSelected;
	}
}
