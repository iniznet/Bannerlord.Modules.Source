using System;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.OrderOfBattle
{
	// Token: 0x0200002C RID: 44
	public class OrderOfBattleFormationFilterSelectorItemVM : ViewModel
	{
		// Token: 0x06000321 RID: 801 RVA: 0x0000E13A File Offset: 0x0000C33A
		public OrderOfBattleFormationFilterSelectorItemVM(FormationFilterType filterType, Action<OrderOfBattleFormationFilterSelectorItemVM> onToggled)
		{
			this.FilterType = filterType;
			this.FilterTypeValue = (int)filterType;
			this._onToggled = onToggled;
			this.RefreshValues();
		}

		// Token: 0x06000322 RID: 802 RVA: 0x0000E15D File Offset: 0x0000C35D
		public override void RefreshValues()
		{
			this.Hint = new HintViewModel(this.FilterType.GetFilterDescription(), null);
		}

		// Token: 0x170000F6 RID: 246
		// (get) Token: 0x06000323 RID: 803 RVA: 0x0000E176 File Offset: 0x0000C376
		// (set) Token: 0x06000324 RID: 804 RVA: 0x0000E17E File Offset: 0x0000C37E
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

		// Token: 0x170000F7 RID: 247
		// (get) Token: 0x06000325 RID: 805 RVA: 0x0000E19C File Offset: 0x0000C39C
		// (set) Token: 0x06000326 RID: 806 RVA: 0x0000E1A4 File Offset: 0x0000C3A4
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

		// Token: 0x170000F8 RID: 248
		// (get) Token: 0x06000327 RID: 807 RVA: 0x0000E1D3 File Offset: 0x0000C3D3
		// (set) Token: 0x06000328 RID: 808 RVA: 0x0000E1DB File Offset: 0x0000C3DB
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

		// Token: 0x04000191 RID: 401
		public readonly FormationFilterType FilterType;

		// Token: 0x04000192 RID: 402
		private Action<OrderOfBattleFormationFilterSelectorItemVM> _onToggled;

		// Token: 0x04000193 RID: 403
		private int _filterType;

		// Token: 0x04000194 RID: 404
		private bool _isActive;

		// Token: 0x04000195 RID: 405
		private HintViewModel _hint;
	}
}
