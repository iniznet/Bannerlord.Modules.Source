using System;
using System.Collections.Generic;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Inventory
{
	public class SPInventorySortControllerVM : ViewModel
	{
		public SPInventorySortControllerVM.InventoryItemSortOption? CurrentSortOption { get; private set; }

		public SPInventorySortControllerVM.InventoryItemSortState? CurrentSortState { get; private set; }

		public SPInventorySortControllerVM(ref MBBindingList<SPItemVM> listToControl)
		{
			this._listToControl = listToControl;
			this._typeComparer = new SPInventorySortControllerVM.ItemTypeComparer();
			this._nameComparer = new SPInventorySortControllerVM.ItemNameComparer();
			this._quantityComparer = new SPInventorySortControllerVM.ItemQuantityComparer();
			this._costComparer = new SPInventorySortControllerVM.ItemCostComparer();
			this.RefreshValues();
		}

		public void SortByOption(SPInventorySortControllerVM.InventoryItemSortOption sortOption, SPInventorySortControllerVM.InventoryItemSortState sortState)
		{
			this.SetAllStates((sortState == SPInventorySortControllerVM.InventoryItemSortState.Ascending) ? SPInventorySortControllerVM.InventoryItemSortState.Descending : SPInventorySortControllerVM.InventoryItemSortState.Ascending);
			if (sortOption == SPInventorySortControllerVM.InventoryItemSortOption.Type)
			{
				this.ExecuteSortByType();
				return;
			}
			if (sortOption == SPInventorySortControllerVM.InventoryItemSortOption.Name)
			{
				this.ExecuteSortByName();
				return;
			}
			if (sortOption == SPInventorySortControllerVM.InventoryItemSortOption.Quantity)
			{
				this.ExecuteSortByQuantity();
				return;
			}
			if (sortOption == SPInventorySortControllerVM.InventoryItemSortOption.Cost)
			{
				this.ExecuteSortByCost();
			}
		}

		public void SortByDefaultState()
		{
			this.ExecuteSortByType();
		}

		public void SortByCurrentState()
		{
			if (this.IsTypeSelected)
			{
				this._listToControl.Sort(this._typeComparer);
				this.CurrentSortOption = new SPInventorySortControllerVM.InventoryItemSortOption?(SPInventorySortControllerVM.InventoryItemSortOption.Type);
				return;
			}
			if (this.IsNameSelected)
			{
				this._listToControl.Sort(this._nameComparer);
				this.CurrentSortOption = new SPInventorySortControllerVM.InventoryItemSortOption?(SPInventorySortControllerVM.InventoryItemSortOption.Name);
				return;
			}
			if (this.IsQuantitySelected)
			{
				this._listToControl.Sort(this._quantityComparer);
				this.CurrentSortOption = new SPInventorySortControllerVM.InventoryItemSortOption?(SPInventorySortControllerVM.InventoryItemSortOption.Quantity);
				return;
			}
			if (this.IsCostSelected)
			{
				this._listToControl.Sort(this._costComparer);
				this.CurrentSortOption = new SPInventorySortControllerVM.InventoryItemSortOption?(SPInventorySortControllerVM.InventoryItemSortOption.Cost);
			}
		}

		public void ExecuteSortByName()
		{
			int nameState = this.NameState;
			this.SetAllStates(SPInventorySortControllerVM.InventoryItemSortState.Default);
			this.NameState = (nameState + 1) % 3;
			if (this.NameState == 0)
			{
				this.NameState++;
			}
			this._nameComparer.SetSortMode(this.NameState == 1);
			this.CurrentSortState = new SPInventorySortControllerVM.InventoryItemSortState?((this.NameState == 1) ? SPInventorySortControllerVM.InventoryItemSortState.Ascending : SPInventorySortControllerVM.InventoryItemSortState.Descending);
			this._listToControl.Sort(this._nameComparer);
			this.IsNameSelected = true;
			this.CurrentSortOption = new SPInventorySortControllerVM.InventoryItemSortOption?(SPInventorySortControllerVM.InventoryItemSortOption.Name);
		}

		public void ExecuteSortByType()
		{
			int typeState = this.TypeState;
			this.SetAllStates(SPInventorySortControllerVM.InventoryItemSortState.Default);
			this.TypeState = (typeState + 1) % 3;
			if (this.TypeState == 0)
			{
				this.TypeState++;
			}
			this._typeComparer.SetSortMode(this.TypeState == 1);
			this.CurrentSortState = new SPInventorySortControllerVM.InventoryItemSortState?((this.TypeState == 1) ? SPInventorySortControllerVM.InventoryItemSortState.Ascending : SPInventorySortControllerVM.InventoryItemSortState.Descending);
			this._listToControl.Sort(this._typeComparer);
			this.IsTypeSelected = true;
			this.CurrentSortOption = new SPInventorySortControllerVM.InventoryItemSortOption?(SPInventorySortControllerVM.InventoryItemSortOption.Type);
		}

		public void ExecuteSortByQuantity()
		{
			int quantityState = this.QuantityState;
			this.SetAllStates(SPInventorySortControllerVM.InventoryItemSortState.Default);
			this.QuantityState = (quantityState + 1) % 3;
			if (this.QuantityState == 0)
			{
				this.QuantityState++;
			}
			this._quantityComparer.SetSortMode(this.QuantityState == 1);
			this.CurrentSortState = new SPInventorySortControllerVM.InventoryItemSortState?((this.QuantityState == 1) ? SPInventorySortControllerVM.InventoryItemSortState.Ascending : SPInventorySortControllerVM.InventoryItemSortState.Descending);
			this._listToControl.Sort(this._quantityComparer);
			this.IsQuantitySelected = true;
			this.CurrentSortOption = new SPInventorySortControllerVM.InventoryItemSortOption?(SPInventorySortControllerVM.InventoryItemSortOption.Quantity);
		}

		public void ExecuteSortByCost()
		{
			int costState = this.CostState;
			this.SetAllStates(SPInventorySortControllerVM.InventoryItemSortState.Default);
			this.CostState = (costState + 1) % 3;
			if (this.CostState == 0)
			{
				this.CostState++;
			}
			this._costComparer.SetSortMode(this.CostState == 1);
			this.CurrentSortState = new SPInventorySortControllerVM.InventoryItemSortState?((this.CostState == 1) ? SPInventorySortControllerVM.InventoryItemSortState.Ascending : SPInventorySortControllerVM.InventoryItemSortState.Descending);
			this._listToControl.Sort(this._costComparer);
			this.IsCostSelected = true;
			this.CurrentSortOption = new SPInventorySortControllerVM.InventoryItemSortOption?(SPInventorySortControllerVM.InventoryItemSortOption.Cost);
		}

		private void SetAllStates(SPInventorySortControllerVM.InventoryItemSortState state)
		{
			this.TypeState = (int)state;
			this.NameState = (int)state;
			this.QuantityState = (int)state;
			this.CostState = (int)state;
			this.IsTypeSelected = false;
			this.IsNameSelected = false;
			this.IsQuantitySelected = false;
			this.IsCostSelected = false;
			this.CurrentSortState = new SPInventorySortControllerVM.InventoryItemSortState?(state);
		}

		[DataSourceProperty]
		public int TypeState
		{
			get
			{
				return this._typeState;
			}
			set
			{
				if (value != this._typeState)
				{
					this._typeState = value;
					base.OnPropertyChangedWithValue(value, "TypeState");
				}
			}
		}

		[DataSourceProperty]
		public int NameState
		{
			get
			{
				return this._nameState;
			}
			set
			{
				if (value != this._nameState)
				{
					this._nameState = value;
					base.OnPropertyChangedWithValue(value, "NameState");
				}
			}
		}

		[DataSourceProperty]
		public int QuantityState
		{
			get
			{
				return this._quantityState;
			}
			set
			{
				if (value != this._quantityState)
				{
					this._quantityState = value;
					base.OnPropertyChangedWithValue(value, "QuantityState");
				}
			}
		}

		[DataSourceProperty]
		public int CostState
		{
			get
			{
				return this._costState;
			}
			set
			{
				if (value != this._costState)
				{
					this._costState = value;
					base.OnPropertyChangedWithValue(value, "CostState");
				}
			}
		}

		[DataSourceProperty]
		public bool IsTypeSelected
		{
			get
			{
				return this._isTypeSelected;
			}
			set
			{
				if (value != this._isTypeSelected)
				{
					this._isTypeSelected = value;
					base.OnPropertyChangedWithValue(value, "IsTypeSelected");
				}
			}
		}

		[DataSourceProperty]
		public bool IsNameSelected
		{
			get
			{
				return this._isNameSelected;
			}
			set
			{
				if (value != this._isNameSelected)
				{
					this._isNameSelected = value;
					base.OnPropertyChangedWithValue(value, "IsNameSelected");
				}
			}
		}

		[DataSourceProperty]
		public bool IsQuantitySelected
		{
			get
			{
				return this._isQuantitySelected;
			}
			set
			{
				if (value != this._isQuantitySelected)
				{
					this._isQuantitySelected = value;
					base.OnPropertyChangedWithValue(value, "IsQuantitySelected");
				}
			}
		}

		[DataSourceProperty]
		public bool IsCostSelected
		{
			get
			{
				return this._isCostSelected;
			}
			set
			{
				if (value != this._isCostSelected)
				{
					this._isCostSelected = value;
					base.OnPropertyChangedWithValue(value, "IsCostSelected");
				}
			}
		}

		private MBBindingList<SPItemVM> _listToControl;

		private SPInventorySortControllerVM.ItemTypeComparer _typeComparer;

		private SPInventorySortControllerVM.ItemNameComparer _nameComparer;

		private SPInventorySortControllerVM.ItemQuantityComparer _quantityComparer;

		private SPInventorySortControllerVM.ItemCostComparer _costComparer;

		private int _typeState;

		private int _nameState;

		private int _quantityState;

		private int _costState;

		private bool _isTypeSelected;

		private bool _isNameSelected;

		private bool _isQuantitySelected;

		private bool _isCostSelected;

		public enum InventoryItemSortState
		{
			Default,
			Ascending,
			Descending
		}

		public enum InventoryItemSortOption
		{
			Type,
			Name,
			Quantity,
			Cost
		}

		public abstract class ItemComparer : IComparer<SPItemVM>
		{
			public void SetSortMode(bool isAscending)
			{
				this._isAscending = isAscending;
			}

			public abstract int Compare(SPItemVM x, SPItemVM y);

			protected int ResolveEquality(SPItemVM x, SPItemVM y)
			{
				return x.ItemDescription.CompareTo(y.ItemDescription);
			}

			protected bool _isAscending;
		}

		public class ItemTypeComparer : SPInventorySortControllerVM.ItemComparer
		{
			public override int Compare(SPItemVM x, SPItemVM y)
			{
				int itemObjectTypeSortIndex = CampaignUIHelper.GetItemObjectTypeSortIndex(x.ItemRosterElement.EquipmentElement.Item);
				int num = CampaignUIHelper.GetItemObjectTypeSortIndex(y.ItemRosterElement.EquipmentElement.Item).CompareTo(itemObjectTypeSortIndex);
				if (num != 0)
				{
					return num * (this._isAscending ? (-1) : 1);
				}
				num = x.ItemCost.CompareTo(y.ItemCost);
				if (num != 0)
				{
					return num;
				}
				return base.ResolveEquality(x, y);
			}
		}

		public class ItemNameComparer : SPInventorySortControllerVM.ItemComparer
		{
			public override int Compare(SPItemVM x, SPItemVM y)
			{
				if (this._isAscending)
				{
					return y.ItemDescription.CompareTo(x.ItemDescription) * -1;
				}
				return y.ItemDescription.CompareTo(x.ItemDescription);
			}
		}

		public class ItemQuantityComparer : SPInventorySortControllerVM.ItemComparer
		{
			public override int Compare(SPItemVM x, SPItemVM y)
			{
				int num = y.ItemCount.CompareTo(x.ItemCount);
				if (num != 0)
				{
					return num * (this._isAscending ? (-1) : 1);
				}
				return base.ResolveEquality(x, y);
			}
		}

		public class ItemCostComparer : SPInventorySortControllerVM.ItemComparer
		{
			public override int Compare(SPItemVM x, SPItemVM y)
			{
				int num = y.ItemCost.CompareTo(x.ItemCost);
				if (num != 0)
				{
					return num * (this._isAscending ? (-1) : 1);
				}
				return base.ResolveEquality(x, y);
			}
		}
	}
}
