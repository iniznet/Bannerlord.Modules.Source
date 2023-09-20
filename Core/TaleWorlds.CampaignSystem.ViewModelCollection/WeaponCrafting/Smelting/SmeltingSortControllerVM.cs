using System;
using System.Collections.Generic;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.WeaponCrafting.Smelting
{
	public class SmeltingSortControllerVM : ViewModel
	{
		public SmeltingSortControllerVM()
		{
			this._yieldComparer = new SmeltingSortControllerVM.ItemYieldComparer();
			this._typeComparer = new SmeltingSortControllerVM.ItemTypeComparer();
			this._nameComparer = new SmeltingSortControllerVM.ItemNameComparer();
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.SortNameText = new TextObject("{=PDdh1sBj}Name", null).ToString();
			this.SortTypeText = new TextObject("{=zMMqgxb1}Type", null).ToString();
			this.SortYieldText = new TextObject("{=v3OF6vBg}Yield", null).ToString();
		}

		public void SetListToControl(MBBindingList<SmeltingItemVM> listToControl)
		{
			this._listToControl = listToControl;
		}

		public void SortByCurrentState()
		{
			if (this.IsNameSelected)
			{
				this._listToControl.Sort(this._nameComparer);
				return;
			}
			if (this.IsYieldSelected)
			{
				this._listToControl.Sort(this._yieldComparer);
				return;
			}
			if (this.IsTypeSelected)
			{
				this._listToControl.Sort(this._typeComparer);
			}
		}

		public void ExecuteSortByName()
		{
			int nameState = this.NameState;
			this.SetAllStates(CampaignUIHelper.SortState.Default);
			this.NameState = (nameState + 1) % 3;
			if (this.NameState == 0)
			{
				this.NameState++;
			}
			this._nameComparer.SetSortMode(this.NameState == 1);
			this._listToControl.Sort(this._nameComparer);
			this.IsNameSelected = true;
		}

		public void ExecuteSortByYield()
		{
			int yieldState = this.YieldState;
			this.SetAllStates(CampaignUIHelper.SortState.Default);
			this.YieldState = (yieldState + 1) % 3;
			if (this.YieldState == 0)
			{
				this.YieldState++;
			}
			this._yieldComparer.SetSortMode(this.YieldState == 1);
			this._listToControl.Sort(this._yieldComparer);
			this.IsYieldSelected = true;
		}

		public void ExecuteSortByType()
		{
			int typeState = this.TypeState;
			this.SetAllStates(CampaignUIHelper.SortState.Default);
			this.TypeState = (typeState + 1) % 3;
			if (this.TypeState == 0)
			{
				this.TypeState++;
			}
			this._typeComparer.SetSortMode(this.TypeState == 1);
			this._listToControl.Sort(this._typeComparer);
			this.IsTypeSelected = true;
		}

		private void SetAllStates(CampaignUIHelper.SortState state)
		{
			this.NameState = (int)state;
			this.TypeState = (int)state;
			this.YieldState = (int)state;
			this.IsNameSelected = false;
			this.IsTypeSelected = false;
			this.IsYieldSelected = false;
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
		public int YieldState
		{
			get
			{
				return this._yieldState;
			}
			set
			{
				if (value != this._yieldState)
				{
					this._yieldState = value;
					base.OnPropertyChangedWithValue(value, "YieldState");
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
		public bool IsYieldSelected
		{
			get
			{
				return this._isYieldSelected;
			}
			set
			{
				if (value != this._isYieldSelected)
				{
					this._isYieldSelected = value;
					base.OnPropertyChangedWithValue(value, "IsYieldSelected");
				}
			}
		}

		[DataSourceProperty]
		public string SortTypeText
		{
			get
			{
				return this._sortTypeText;
			}
			set
			{
				if (value != this._sortTypeText)
				{
					this._sortTypeText = value;
					base.OnPropertyChangedWithValue<string>(value, "SortTypeText");
				}
			}
		}

		[DataSourceProperty]
		public string SortNameText
		{
			get
			{
				return this._sortNameText;
			}
			set
			{
				if (value != this._sortNameText)
				{
					this._sortNameText = value;
					base.OnPropertyChangedWithValue<string>(value, "SortNameText");
				}
			}
		}

		[DataSourceProperty]
		public string SortYieldText
		{
			get
			{
				return this._sortYieldText;
			}
			set
			{
				if (value != this._sortYieldText)
				{
					this._sortYieldText = value;
					base.OnPropertyChangedWithValue<string>(value, "SortYieldText");
				}
			}
		}

		private MBBindingList<SmeltingItemVM> _listToControl;

		private readonly SmeltingSortControllerVM.ItemNameComparer _nameComparer;

		private readonly SmeltingSortControllerVM.ItemYieldComparer _yieldComparer;

		private readonly SmeltingSortControllerVM.ItemTypeComparer _typeComparer;

		private int _nameState;

		private int _yieldState;

		private int _typeState;

		private bool _isNameSelected;

		private bool _isYieldSelected;

		private bool _isTypeSelected;

		private string _sortTypeText;

		private string _sortNameText;

		private string _sortYieldText;

		public abstract class ItemComparerBase : IComparer<SmeltingItemVM>
		{
			public void SetSortMode(bool isAscending)
			{
				this._isAscending = isAscending;
			}

			public abstract int Compare(SmeltingItemVM x, SmeltingItemVM y);

			protected int ResolveEquality(SmeltingItemVM x, SmeltingItemVM y)
			{
				return x.Name.CompareTo(y.Name);
			}

			protected bool _isAscending;
		}

		public class ItemNameComparer : SmeltingSortControllerVM.ItemComparerBase
		{
			public override int Compare(SmeltingItemVM x, SmeltingItemVM y)
			{
				if (this._isAscending)
				{
					return y.Name.CompareTo(x.Name) * -1;
				}
				return y.Name.CompareTo(x.Name);
			}
		}

		public class ItemYieldComparer : SmeltingSortControllerVM.ItemComparerBase
		{
			public override int Compare(SmeltingItemVM x, SmeltingItemVM y)
			{
				int num = y.Yield.Count.CompareTo(x.Yield.Count);
				if (num != 0)
				{
					return num * (this._isAscending ? (-1) : 1);
				}
				return base.ResolveEquality(x, y);
			}
		}

		public class ItemTypeComparer : SmeltingSortControllerVM.ItemComparerBase
		{
			public override int Compare(SmeltingItemVM x, SmeltingItemVM y)
			{
				int itemObjectTypeSortIndex = CampaignUIHelper.GetItemObjectTypeSortIndex(x.EquipmentElement.Item);
				int num = CampaignUIHelper.GetItemObjectTypeSortIndex(y.EquipmentElement.Item).CompareTo(itemObjectTypeSortIndex);
				if (num != 0)
				{
					return num * (this._isAscending ? (-1) : 1);
				}
				return base.ResolveEquality(x, y);
			}
		}
	}
}
