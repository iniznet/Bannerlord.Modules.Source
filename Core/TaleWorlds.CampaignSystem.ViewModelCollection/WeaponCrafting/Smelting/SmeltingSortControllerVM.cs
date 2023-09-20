using System;
using System.Collections.Generic;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.WeaponCrafting.Smelting
{
	// Token: 0x020000EE RID: 238
	public class SmeltingSortControllerVM : ViewModel
	{
		// Token: 0x0600164A RID: 5706 RVA: 0x000531CB File Offset: 0x000513CB
		public SmeltingSortControllerVM()
		{
			this._yieldComparer = new SmeltingSortControllerVM.ItemYieldComparer();
			this._typeComparer = new SmeltingSortControllerVM.ItemTypeComparer();
			this._nameComparer = new SmeltingSortControllerVM.ItemNameComparer();
			this.RefreshValues();
		}

		// Token: 0x0600164B RID: 5707 RVA: 0x000531FC File Offset: 0x000513FC
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.SortNameText = new TextObject("{=PDdh1sBj}Name", null).ToString();
			this.SortTypeText = new TextObject("{=zMMqgxb1}Type", null).ToString();
			this.SortYieldText = new TextObject("{=v3OF6vBg}Yield", null).ToString();
		}

		// Token: 0x0600164C RID: 5708 RVA: 0x00053251 File Offset: 0x00051451
		public void SetListToControl(MBBindingList<SmeltingItemVM> listToControl)
		{
			this._listToControl = listToControl;
		}

		// Token: 0x0600164D RID: 5709 RVA: 0x0005325C File Offset: 0x0005145C
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

		// Token: 0x0600164E RID: 5710 RVA: 0x000532B8 File Offset: 0x000514B8
		public void ExecuteSortByName()
		{
			int nameState = this.NameState;
			this.SetAllStates(SmeltingSortControllerVM.SortState.Default);
			this.NameState = (nameState + 1) % 3;
			if (this.NameState == 0)
			{
				this.NameState++;
			}
			this._nameComparer.SetSortMode(this.NameState == 1);
			this._listToControl.Sort(this._nameComparer);
			this.IsNameSelected = true;
		}

		// Token: 0x0600164F RID: 5711 RVA: 0x00053320 File Offset: 0x00051520
		public void ExecuteSortByYield()
		{
			int yieldState = this.YieldState;
			this.SetAllStates(SmeltingSortControllerVM.SortState.Default);
			this.YieldState = (yieldState + 1) % 3;
			if (this.YieldState == 0)
			{
				this.YieldState++;
			}
			this._yieldComparer.SetSortMode(this.YieldState == 1);
			this._listToControl.Sort(this._yieldComparer);
			this.IsYieldSelected = true;
		}

		// Token: 0x06001650 RID: 5712 RVA: 0x00053388 File Offset: 0x00051588
		public void ExecuteSortByType()
		{
			int typeState = this.TypeState;
			this.SetAllStates(SmeltingSortControllerVM.SortState.Default);
			this.TypeState = (typeState + 1) % 3;
			if (this.TypeState == 0)
			{
				this.TypeState++;
			}
			this._typeComparer.SetSortMode(this.TypeState == 1);
			this._listToControl.Sort(this._typeComparer);
			this.IsTypeSelected = true;
		}

		// Token: 0x06001651 RID: 5713 RVA: 0x000533F0 File Offset: 0x000515F0
		private void SetAllStates(SmeltingSortControllerVM.SortState state)
		{
			this.NameState = (int)state;
			this.TypeState = (int)state;
			this.YieldState = (int)state;
			this.IsNameSelected = false;
			this.IsTypeSelected = false;
			this.IsYieldSelected = false;
		}

		// Token: 0x1700078B RID: 1931
		// (get) Token: 0x06001652 RID: 5714 RVA: 0x0005341C File Offset: 0x0005161C
		// (set) Token: 0x06001653 RID: 5715 RVA: 0x00053424 File Offset: 0x00051624
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

		// Token: 0x1700078C RID: 1932
		// (get) Token: 0x06001654 RID: 5716 RVA: 0x00053442 File Offset: 0x00051642
		// (set) Token: 0x06001655 RID: 5717 RVA: 0x0005344A File Offset: 0x0005164A
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

		// Token: 0x1700078D RID: 1933
		// (get) Token: 0x06001656 RID: 5718 RVA: 0x00053468 File Offset: 0x00051668
		// (set) Token: 0x06001657 RID: 5719 RVA: 0x00053470 File Offset: 0x00051670
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

		// Token: 0x1700078E RID: 1934
		// (get) Token: 0x06001658 RID: 5720 RVA: 0x0005348E File Offset: 0x0005168E
		// (set) Token: 0x06001659 RID: 5721 RVA: 0x00053496 File Offset: 0x00051696
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

		// Token: 0x1700078F RID: 1935
		// (get) Token: 0x0600165A RID: 5722 RVA: 0x000534B4 File Offset: 0x000516B4
		// (set) Token: 0x0600165B RID: 5723 RVA: 0x000534BC File Offset: 0x000516BC
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

		// Token: 0x17000790 RID: 1936
		// (get) Token: 0x0600165C RID: 5724 RVA: 0x000534DA File Offset: 0x000516DA
		// (set) Token: 0x0600165D RID: 5725 RVA: 0x000534E2 File Offset: 0x000516E2
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

		// Token: 0x17000791 RID: 1937
		// (get) Token: 0x0600165E RID: 5726 RVA: 0x00053500 File Offset: 0x00051700
		// (set) Token: 0x0600165F RID: 5727 RVA: 0x00053508 File Offset: 0x00051708
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

		// Token: 0x17000792 RID: 1938
		// (get) Token: 0x06001660 RID: 5728 RVA: 0x0005352B File Offset: 0x0005172B
		// (set) Token: 0x06001661 RID: 5729 RVA: 0x00053533 File Offset: 0x00051733
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

		// Token: 0x17000793 RID: 1939
		// (get) Token: 0x06001662 RID: 5730 RVA: 0x00053556 File Offset: 0x00051756
		// (set) Token: 0x06001663 RID: 5731 RVA: 0x0005355E File Offset: 0x0005175E
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

		// Token: 0x04000A71 RID: 2673
		private MBBindingList<SmeltingItemVM> _listToControl;

		// Token: 0x04000A72 RID: 2674
		private readonly SmeltingSortControllerVM.ItemNameComparer _nameComparer;

		// Token: 0x04000A73 RID: 2675
		private readonly SmeltingSortControllerVM.ItemYieldComparer _yieldComparer;

		// Token: 0x04000A74 RID: 2676
		private readonly SmeltingSortControllerVM.ItemTypeComparer _typeComparer;

		// Token: 0x04000A75 RID: 2677
		private int _nameState;

		// Token: 0x04000A76 RID: 2678
		private int _yieldState;

		// Token: 0x04000A77 RID: 2679
		private int _typeState;

		// Token: 0x04000A78 RID: 2680
		private bool _isNameSelected;

		// Token: 0x04000A79 RID: 2681
		private bool _isYieldSelected;

		// Token: 0x04000A7A RID: 2682
		private bool _isTypeSelected;

		// Token: 0x04000A7B RID: 2683
		private string _sortTypeText;

		// Token: 0x04000A7C RID: 2684
		private string _sortNameText;

		// Token: 0x04000A7D RID: 2685
		private string _sortYieldText;

		// Token: 0x02000225 RID: 549
		private enum SortState
		{
			// Token: 0x0400109F RID: 4255
			Default,
			// Token: 0x040010A0 RID: 4256
			Ascending,
			// Token: 0x040010A1 RID: 4257
			Descending
		}

		// Token: 0x02000226 RID: 550
		public abstract class ItemComparerBase : IComparer<SmeltingItemVM>
		{
			// Token: 0x06002122 RID: 8482 RVA: 0x000706CE File Offset: 0x0006E8CE
			public void SetSortMode(bool isAscending)
			{
				this._isAscending = isAscending;
			}

			// Token: 0x06002123 RID: 8483
			public abstract int Compare(SmeltingItemVM x, SmeltingItemVM y);

			// Token: 0x06002124 RID: 8484 RVA: 0x000706D7 File Offset: 0x0006E8D7
			protected int ResolveEquality(SmeltingItemVM x, SmeltingItemVM y)
			{
				return x.Name.CompareTo(y.Name);
			}

			// Token: 0x040010A2 RID: 4258
			protected bool _isAscending;
		}

		// Token: 0x02000227 RID: 551
		public class ItemNameComparer : SmeltingSortControllerVM.ItemComparerBase
		{
			// Token: 0x06002126 RID: 8486 RVA: 0x000706F2 File Offset: 0x0006E8F2
			public override int Compare(SmeltingItemVM x, SmeltingItemVM y)
			{
				if (this._isAscending)
				{
					return y.Name.CompareTo(x.Name) * -1;
				}
				return y.Name.CompareTo(x.Name);
			}
		}

		// Token: 0x02000228 RID: 552
		public class ItemYieldComparer : SmeltingSortControllerVM.ItemComparerBase
		{
			// Token: 0x06002128 RID: 8488 RVA: 0x0007072C File Offset: 0x0006E92C
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

		// Token: 0x02000229 RID: 553
		public class ItemTypeComparer : SmeltingSortControllerVM.ItemComparerBase
		{
			// Token: 0x0600212A RID: 8490 RVA: 0x0007077C File Offset: 0x0006E97C
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
