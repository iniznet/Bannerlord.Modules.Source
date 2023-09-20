using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Inventory
{
	public class InventoryListPanel : NavigatableListPanel
	{
		public InventoryListPanel(UIContext context)
			: base(context)
		{
			this._sortByTypeClickHandler = new Action<Widget>(this.OnSortByType);
			this._sortByNameClickHandler = new Action<Widget>(this.OnSortByName);
			this._sortByQuantityClickHandler = new Action<Widget>(this.OnSortByQuantity);
			this._sortByCostClickHandler = new Action<Widget>(this.OnSortByCost);
		}

		private void OnSortByType(Widget widget)
		{
			base.RefreshChildNavigationIndices();
		}

		private void OnSortByName(Widget widget)
		{
			base.RefreshChildNavigationIndices();
		}

		private void OnSortByQuantity(Widget widget)
		{
			base.RefreshChildNavigationIndices();
		}

		private void OnSortByCost(Widget widget)
		{
			base.RefreshChildNavigationIndices();
		}

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

		private Action<Widget> _sortByTypeClickHandler;

		private Action<Widget> _sortByNameClickHandler;

		private Action<Widget> _sortByQuantityClickHandler;

		private Action<Widget> _sortByCostClickHandler;

		private ButtonWidget _sortByTypeBtn;

		private ButtonWidget _sortByNameBtn;

		private ButtonWidget _sortByQuantityBtn;

		private ButtonWidget _sortByCostBtn;
	}
}
