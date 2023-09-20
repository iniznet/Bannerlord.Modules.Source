using System;
using System.Collections.Generic;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Inventory
{
	// Token: 0x0200007D RID: 125
	public class SPInventorySortControllerVM : ViewModel
	{
		// Token: 0x170003A6 RID: 934
		// (get) Token: 0x06000B4A RID: 2890 RVA: 0x0002E1B0 File Offset: 0x0002C3B0
		// (set) Token: 0x06000B4B RID: 2891 RVA: 0x0002E1B8 File Offset: 0x0002C3B8
		public SPInventorySortControllerVM.InventoryItemSortOption? CurrentSortOption { get; private set; }

		// Token: 0x170003A7 RID: 935
		// (get) Token: 0x06000B4C RID: 2892 RVA: 0x0002E1C1 File Offset: 0x0002C3C1
		// (set) Token: 0x06000B4D RID: 2893 RVA: 0x0002E1C9 File Offset: 0x0002C3C9
		public SPInventorySortControllerVM.InventoryItemSortState? CurrentSortState { get; private set; }

		// Token: 0x06000B4E RID: 2894 RVA: 0x0002E1D4 File Offset: 0x0002C3D4
		public SPInventorySortControllerVM(ref MBBindingList<SPItemVM> listToControl)
		{
			this._listToControl = listToControl;
			this._typeComparer = new SPInventorySortControllerVM.ItemTypeComparer();
			this._nameComparer = new SPInventorySortControllerVM.ItemNameComparer();
			this._quantityComparer = new SPInventorySortControllerVM.ItemQuantityComparer();
			this._costComparer = new SPInventorySortControllerVM.ItemCostComparer();
			this.RefreshValues();
		}

		// Token: 0x06000B4F RID: 2895 RVA: 0x0002E221 File Offset: 0x0002C421
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

		// Token: 0x06000B50 RID: 2896 RVA: 0x0002E25B File Offset: 0x0002C45B
		public void SortByDefaultState()
		{
			this.ExecuteSortByType();
		}

		// Token: 0x06000B51 RID: 2897 RVA: 0x0002E264 File Offset: 0x0002C464
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

		// Token: 0x06000B52 RID: 2898 RVA: 0x0002E308 File Offset: 0x0002C508
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

		// Token: 0x06000B53 RID: 2899 RVA: 0x0002E394 File Offset: 0x0002C594
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

		// Token: 0x06000B54 RID: 2900 RVA: 0x0002E420 File Offset: 0x0002C620
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

		// Token: 0x06000B55 RID: 2901 RVA: 0x0002E4AC File Offset: 0x0002C6AC
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

		// Token: 0x06000B56 RID: 2902 RVA: 0x0002E538 File Offset: 0x0002C738
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

		// Token: 0x170003A8 RID: 936
		// (get) Token: 0x06000B57 RID: 2903 RVA: 0x0002E589 File Offset: 0x0002C789
		// (set) Token: 0x06000B58 RID: 2904 RVA: 0x0002E591 File Offset: 0x0002C791
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

		// Token: 0x170003A9 RID: 937
		// (get) Token: 0x06000B59 RID: 2905 RVA: 0x0002E5AF File Offset: 0x0002C7AF
		// (set) Token: 0x06000B5A RID: 2906 RVA: 0x0002E5B7 File Offset: 0x0002C7B7
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

		// Token: 0x170003AA RID: 938
		// (get) Token: 0x06000B5B RID: 2907 RVA: 0x0002E5D5 File Offset: 0x0002C7D5
		// (set) Token: 0x06000B5C RID: 2908 RVA: 0x0002E5DD File Offset: 0x0002C7DD
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

		// Token: 0x170003AB RID: 939
		// (get) Token: 0x06000B5D RID: 2909 RVA: 0x0002E5FB File Offset: 0x0002C7FB
		// (set) Token: 0x06000B5E RID: 2910 RVA: 0x0002E603 File Offset: 0x0002C803
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

		// Token: 0x170003AC RID: 940
		// (get) Token: 0x06000B5F RID: 2911 RVA: 0x0002E621 File Offset: 0x0002C821
		// (set) Token: 0x06000B60 RID: 2912 RVA: 0x0002E629 File Offset: 0x0002C829
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

		// Token: 0x170003AD RID: 941
		// (get) Token: 0x06000B61 RID: 2913 RVA: 0x0002E647 File Offset: 0x0002C847
		// (set) Token: 0x06000B62 RID: 2914 RVA: 0x0002E64F File Offset: 0x0002C84F
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

		// Token: 0x170003AE RID: 942
		// (get) Token: 0x06000B63 RID: 2915 RVA: 0x0002E66D File Offset: 0x0002C86D
		// (set) Token: 0x06000B64 RID: 2916 RVA: 0x0002E675 File Offset: 0x0002C875
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

		// Token: 0x170003AF RID: 943
		// (get) Token: 0x06000B65 RID: 2917 RVA: 0x0002E693 File Offset: 0x0002C893
		// (set) Token: 0x06000B66 RID: 2918 RVA: 0x0002E69B File Offset: 0x0002C89B
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

		// Token: 0x04000536 RID: 1334
		private MBBindingList<SPItemVM> _listToControl;

		// Token: 0x04000537 RID: 1335
		private SPInventorySortControllerVM.ItemTypeComparer _typeComparer;

		// Token: 0x04000538 RID: 1336
		private SPInventorySortControllerVM.ItemNameComparer _nameComparer;

		// Token: 0x04000539 RID: 1337
		private SPInventorySortControllerVM.ItemQuantityComparer _quantityComparer;

		// Token: 0x0400053A RID: 1338
		private SPInventorySortControllerVM.ItemCostComparer _costComparer;

		// Token: 0x0400053D RID: 1341
		private int _typeState;

		// Token: 0x0400053E RID: 1342
		private int _nameState;

		// Token: 0x0400053F RID: 1343
		private int _quantityState;

		// Token: 0x04000540 RID: 1344
		private int _costState;

		// Token: 0x04000541 RID: 1345
		private bool _isTypeSelected;

		// Token: 0x04000542 RID: 1346
		private bool _isNameSelected;

		// Token: 0x04000543 RID: 1347
		private bool _isQuantitySelected;

		// Token: 0x04000544 RID: 1348
		private bool _isCostSelected;

		// Token: 0x020001B8 RID: 440
		public enum InventoryItemSortState
		{
			// Token: 0x04000F7B RID: 3963
			Default,
			// Token: 0x04000F7C RID: 3964
			Ascending,
			// Token: 0x04000F7D RID: 3965
			Descending
		}

		// Token: 0x020001B9 RID: 441
		public enum InventoryItemSortOption
		{
			// Token: 0x04000F7F RID: 3967
			Type,
			// Token: 0x04000F80 RID: 3968
			Name,
			// Token: 0x04000F81 RID: 3969
			Quantity,
			// Token: 0x04000F82 RID: 3970
			Cost
		}

		// Token: 0x020001BA RID: 442
		public abstract class ItemComparer : IComparer<SPItemVM>
		{
			// Token: 0x06001FDE RID: 8158 RVA: 0x0006F077 File Offset: 0x0006D277
			public void SetSortMode(bool isAscending)
			{
				this._isAscending = isAscending;
			}

			// Token: 0x06001FDF RID: 8159
			public abstract int Compare(SPItemVM x, SPItemVM y);

			// Token: 0x06001FE0 RID: 8160 RVA: 0x0006F080 File Offset: 0x0006D280
			protected int ResolveEquality(SPItemVM x, SPItemVM y)
			{
				return x.ItemDescription.CompareTo(y.ItemDescription);
			}

			// Token: 0x04000F83 RID: 3971
			protected bool _isAscending;
		}

		// Token: 0x020001BB RID: 443
		public class ItemTypeComparer : SPInventorySortControllerVM.ItemComparer
		{
			// Token: 0x06001FE2 RID: 8162 RVA: 0x0006F09C File Offset: 0x0006D29C
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

		// Token: 0x020001BC RID: 444
		public class ItemNameComparer : SPInventorySortControllerVM.ItemComparer
		{
			// Token: 0x06001FE4 RID: 8164 RVA: 0x0006F121 File Offset: 0x0006D321
			public override int Compare(SPItemVM x, SPItemVM y)
			{
				if (this._isAscending)
				{
					return y.ItemDescription.CompareTo(x.ItemDescription) * -1;
				}
				return y.ItemDescription.CompareTo(x.ItemDescription);
			}
		}

		// Token: 0x020001BD RID: 445
		public class ItemQuantityComparer : SPInventorySortControllerVM.ItemComparer
		{
			// Token: 0x06001FE6 RID: 8166 RVA: 0x0006F158 File Offset: 0x0006D358
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

		// Token: 0x020001BE RID: 446
		public class ItemCostComparer : SPInventorySortControllerVM.ItemComparer
		{
			// Token: 0x06001FE8 RID: 8168 RVA: 0x0006F19C File Offset: 0x0006D39C
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
