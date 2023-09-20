using System;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.OrderOfBattle
{
	public class OrderOfBattleFormationFilterSelectorItemVM : ViewModel
	{
		public OrderOfBattleFormationFilterSelectorItemVM(FormationFilterType filterType, Action<OrderOfBattleFormationFilterSelectorItemVM> onToggled)
		{
			this.FilterType = filterType;
			this.FilterTypeValue = (int)filterType;
			this._onToggled = onToggled;
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			this.Hint = new HintViewModel(this.FilterType.GetFilterDescription(), null);
		}

		[DataSourceProperty]
		public int FilterTypeValue
		{
			get
			{
				return this._filterType;
			}
			set
			{
				if (value != this._filterType)
				{
					this._filterType = value;
					base.OnPropertyChangedWithValue(value, "FilterTypeValue");
				}
			}
		}

		[DataSourceProperty]
		public bool IsActive
		{
			get
			{
				return this._isActive;
			}
			set
			{
				if (value != this._isActive)
				{
					this._isActive = value;
					base.OnPropertyChangedWithValue(value, "IsActive");
					Action<OrderOfBattleFormationFilterSelectorItemVM> onToggled = this._onToggled;
					if (onToggled == null)
					{
						return;
					}
					onToggled(this);
				}
			}
		}

		[DataSourceProperty]
		public HintViewModel Hint
		{
			get
			{
				return this._hint;
			}
			set
			{
				if (value != this._hint)
				{
					this._hint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "Hint");
				}
			}
		}

		public readonly FormationFilterType FilterType;

		private Action<OrderOfBattleFormationFilterSelectorItemVM> _onToggled;

		private int _filterType;

		private bool _isActive;

		private HintViewModel _hint;
	}
}
