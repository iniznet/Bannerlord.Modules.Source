using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Inventory
{
	// Token: 0x02000124 RID: 292
	public class InventoryListPanel : NavigatableListPanel
	{
		// Token: 0x06000F18 RID: 3864 RVA: 0x00029D98 File Offset: 0x00027F98
		public InventoryListPanel(UIContext context)
			: base(context)
		{
			this._sortByTypeClickHandler = new Action<Widget>(this.OnSortByType);
			this._sortByNameClickHandler = new Action<Widget>(this.OnSortByName);
			this._sortByQuantityClickHandler = new Action<Widget>(this.OnSortByQuantity);
			this._sortByCostClickHandler = new Action<Widget>(this.OnSortByCost);
		}

		// Token: 0x06000F19 RID: 3865 RVA: 0x00029DF4 File Offset: 0x00027FF4
		private void OnSortByType(Widget widget)
		{
			base.RefreshChildNavigationIndices();
		}

		// Token: 0x06000F1A RID: 3866 RVA: 0x00029DFC File Offset: 0x00027FFC
		private void OnSortByName(Widget widget)
		{
			base.RefreshChildNavigationIndices();
		}

		// Token: 0x06000F1B RID: 3867 RVA: 0x00029E04 File Offset: 0x00028004
		private void OnSortByQuantity(Widget widget)
		{
			base.RefreshChildNavigationIndices();
		}

		// Token: 0x06000F1C RID: 3868 RVA: 0x00029E0C File Offset: 0x0002800C
		private void OnSortByCost(Widget widget)
		{
			base.RefreshChildNavigationIndices();
		}

		// Token: 0x17000550 RID: 1360
		// (get) Token: 0x06000F1D RID: 3869 RVA: 0x00029E14 File Offset: 0x00028014
		// (set) Token: 0x06000F1E RID: 3870 RVA: 0x00029E1C File Offset: 0x0002801C
		[Editor(false)]
		public ButtonWidget SortByTypeBtn
		{
			get
			{
				return this._sortByTypeBtn;
			}
			set
			{
				if (this._sortByTypeBtn != value)
				{
					if (this._sortByTypeBtn != null)
					{
						this._sortByTypeBtn.ClickEventHandlers.Remove(this._sortByTypeClickHandler);
					}
					this._sortByTypeBtn = value;
					if (this._sortByTypeBtn != null)
					{
						this._sortByTypeBtn.ClickEventHandlers.Add(this._sortByTypeClickHandler);
					}
					base.OnPropertyChanged<ButtonWidget>(value, "SortByTypeBtn");
				}
			}
		}

		// Token: 0x17000551 RID: 1361
		// (get) Token: 0x06000F1F RID: 3871 RVA: 0x00029E82 File Offset: 0x00028082
		// (set) Token: 0x06000F20 RID: 3872 RVA: 0x00029E8C File Offset: 0x0002808C
		[Editor(false)]
		public ButtonWidget SortByNameBtn
		{
			get
			{
				return this._sortByNameBtn;
			}
			set
			{
				if (this._sortByNameBtn != value)
				{
					if (this._sortByNameBtn != null)
					{
						this._sortByNameBtn.ClickEventHandlers.Remove(this._sortByNameClickHandler);
					}
					this._sortByNameBtn = value;
					if (this._sortByNameBtn != null)
					{
						this._sortByNameBtn.ClickEventHandlers.Add(this._sortByNameClickHandler);
					}
					base.OnPropertyChanged<ButtonWidget>(value, "SortByNameBtn");
				}
			}
		}

		// Token: 0x17000552 RID: 1362
		// (get) Token: 0x06000F21 RID: 3873 RVA: 0x00029EF2 File Offset: 0x000280F2
		// (set) Token: 0x06000F22 RID: 3874 RVA: 0x00029EFC File Offset: 0x000280FC
		[Editor(false)]
		public ButtonWidget SortByQuantityBtn
		{
			get
			{
				return this._sortByQuantityBtn;
			}
			set
			{
				if (this._sortByQuantityBtn != value)
				{
					if (this._sortByQuantityBtn != null)
					{
						this._sortByQuantityBtn.ClickEventHandlers.Remove(this._sortByQuantityClickHandler);
					}
					this._sortByQuantityBtn = value;
					if (this._sortByQuantityBtn != null)
					{
						this._sortByQuantityBtn.ClickEventHandlers.Add(this._sortByQuantityClickHandler);
					}
					base.OnPropertyChanged<ButtonWidget>(value, "SortByQuantityBtn");
				}
			}
		}

		// Token: 0x17000553 RID: 1363
		// (get) Token: 0x06000F23 RID: 3875 RVA: 0x00029F62 File Offset: 0x00028162
		// (set) Token: 0x06000F24 RID: 3876 RVA: 0x00029F6C File Offset: 0x0002816C
		[Editor(false)]
		public ButtonWidget SortByCostBtn
		{
			get
			{
				return this._sortByCostBtn;
			}
			set
			{
				if (this._sortByCostBtn != value)
				{
					if (this._sortByCostBtn != null)
					{
						this._sortByCostBtn.ClickEventHandlers.Remove(this._sortByCostClickHandler);
					}
					this._sortByCostBtn = value;
					if (this._sortByCostBtn != null)
					{
						this._sortByCostBtn.ClickEventHandlers.Remove(this._sortByCostClickHandler);
					}
					base.OnPropertyChanged<ButtonWidget>(value, "SortByCostBtn");
				}
			}
		}

		// Token: 0x040006E0 RID: 1760
		private Action<Widget> _sortByTypeClickHandler;

		// Token: 0x040006E1 RID: 1761
		private Action<Widget> _sortByNameClickHandler;

		// Token: 0x040006E2 RID: 1762
		private Action<Widget> _sortByQuantityClickHandler;

		// Token: 0x040006E3 RID: 1763
		private Action<Widget> _sortByCostClickHandler;

		// Token: 0x040006E4 RID: 1764
		private ButtonWidget _sortByTypeBtn;

		// Token: 0x040006E5 RID: 1765
		private ButtonWidget _sortByNameBtn;

		// Token: 0x040006E6 RID: 1766
		private ButtonWidget _sortByQuantityBtn;

		// Token: 0x040006E7 RID: 1767
		private ButtonWidget _sortByCostBtn;
	}
}
