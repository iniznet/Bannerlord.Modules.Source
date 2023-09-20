using System;
using System.Collections.Generic;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.KingdomManagement.Settlements
{
	// Token: 0x02000057 RID: 87
	public class KingdomSettlementSortControllerVM : ViewModel
	{
		// Token: 0x0600072A RID: 1834 RVA: 0x0001F748 File Offset: 0x0001D948
		public KingdomSettlementSortControllerVM(ref MBBindingList<KingdomSettlementItemVM> listToControl)
		{
			this._listToControl = listToControl;
			this._typeComparer = new KingdomSettlementSortControllerVM.ItemTypeComparer();
			this._prosperityComparer = new KingdomSettlementSortControllerVM.ItemProsperityComparer();
			this._defendersComparer = new KingdomSettlementSortControllerVM.ItemDefendersComparer();
			this._ownerComparer = new KingdomSettlementSortControllerVM.ItemOwnerComparer();
			this._nameComparer = new KingdomSettlementSortControllerVM.ItemNameComparer();
		}

		// Token: 0x0600072B RID: 1835 RVA: 0x0001F79C File Offset: 0x0001D99C
		private void ExecuteSortByType()
		{
			int typeState = this.TypeState;
			this.SetAllStates(KingdomSettlementSortControllerVM.SortState.Default);
			this.TypeState = (typeState + 1) % 3;
			if (this.TypeState == 0)
			{
				this.TypeState++;
			}
			this._typeComparer.SetSortMode(this.TypeState == 1);
			this._listToControl.Sort(this._typeComparer);
			this.IsTypeSelected = true;
		}

		// Token: 0x0600072C RID: 1836 RVA: 0x0001F804 File Offset: 0x0001DA04
		private void ExecuteSortByName()
		{
			int nameState = this.NameState;
			this.SetAllStates(KingdomSettlementSortControllerVM.SortState.Default);
			this.NameState = (nameState + 1) % 3;
			if (this.NameState == 0)
			{
				this.NameState++;
			}
			this._nameComparer.SetSortMode(this.NameState == 1);
			this._listToControl.Sort(this._nameComparer);
			this.IsNameSelected = true;
		}

		// Token: 0x0600072D RID: 1837 RVA: 0x0001F86C File Offset: 0x0001DA6C
		private void ExecuteSortByOwner()
		{
			int ownerState = this.OwnerState;
			this.SetAllStates(KingdomSettlementSortControllerVM.SortState.Default);
			this.OwnerState = (ownerState + 1) % 3;
			if (this.OwnerState == 0)
			{
				this.OwnerState++;
			}
			this._ownerComparer.SetSortMode(this.OwnerState == 1);
			this._listToControl.Sort(this._ownerComparer);
			this.IsOwnerSelected = true;
		}

		// Token: 0x0600072E RID: 1838 RVA: 0x0001F8D4 File Offset: 0x0001DAD4
		private void ExecuteSortByProsperity()
		{
			int prosperityState = this.ProsperityState;
			this.SetAllStates(KingdomSettlementSortControllerVM.SortState.Default);
			this.ProsperityState = (prosperityState + 1) % 3;
			if (this.ProsperityState == 0)
			{
				this.ProsperityState++;
			}
			this._prosperityComparer.SetSortMode(this.ProsperityState == 1);
			this._listToControl.Sort(this._prosperityComparer);
			this.IsProsperitySelected = true;
		}

		// Token: 0x0600072F RID: 1839 RVA: 0x0001F93C File Offset: 0x0001DB3C
		private void ExecuteSortByDefenders()
		{
			int defendersState = this.DefendersState;
			this.SetAllStates(KingdomSettlementSortControllerVM.SortState.Default);
			this.DefendersState = (defendersState + 1) % 3;
			if (this.DefendersState == 0)
			{
				int defendersState2 = this.DefendersState;
				this.DefendersState = defendersState2 + 1;
			}
			this._defendersComparer.SetSortMode(this.DefendersState == 1);
			this._listToControl.Sort(this._defendersComparer);
			this.IsDefendersSelected = true;
		}

		// Token: 0x06000730 RID: 1840 RVA: 0x0001F9A8 File Offset: 0x0001DBA8
		private void SetAllStates(KingdomSettlementSortControllerVM.SortState state)
		{
			this.TypeState = (int)state;
			this.NameState = (int)state;
			this.OwnerState = (int)state;
			this.ProsperityState = (int)state;
			this.DefendersState = (int)state;
			this.IsTypeSelected = false;
			this.IsNameSelected = false;
			this.IsProsperitySelected = false;
			this.IsOwnerSelected = false;
			this.IsDefendersSelected = false;
		}

		// Token: 0x17000220 RID: 544
		// (get) Token: 0x06000731 RID: 1841 RVA: 0x0001F9FB File Offset: 0x0001DBFB
		// (set) Token: 0x06000732 RID: 1842 RVA: 0x0001FA03 File Offset: 0x0001DC03
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

		// Token: 0x17000221 RID: 545
		// (get) Token: 0x06000733 RID: 1843 RVA: 0x0001FA21 File Offset: 0x0001DC21
		// (set) Token: 0x06000734 RID: 1844 RVA: 0x0001FA29 File Offset: 0x0001DC29
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

		// Token: 0x17000222 RID: 546
		// (get) Token: 0x06000735 RID: 1845 RVA: 0x0001FA47 File Offset: 0x0001DC47
		// (set) Token: 0x06000736 RID: 1846 RVA: 0x0001FA4F File Offset: 0x0001DC4F
		[DataSourceProperty]
		public int OwnerState
		{
			get
			{
				return this._ownerState;
			}
			set
			{
				if (value != this._ownerState)
				{
					this._ownerState = value;
					base.OnPropertyChangedWithValue(value, "OwnerState");
				}
			}
		}

		// Token: 0x17000223 RID: 547
		// (get) Token: 0x06000737 RID: 1847 RVA: 0x0001FA6D File Offset: 0x0001DC6D
		// (set) Token: 0x06000738 RID: 1848 RVA: 0x0001FA75 File Offset: 0x0001DC75
		[DataSourceProperty]
		public int ProsperityState
		{
			get
			{
				return this._prosperityState;
			}
			set
			{
				if (value != this._prosperityState)
				{
					this._prosperityState = value;
					base.OnPropertyChangedWithValue(value, "ProsperityState");
				}
			}
		}

		// Token: 0x17000224 RID: 548
		// (get) Token: 0x06000739 RID: 1849 RVA: 0x0001FA93 File Offset: 0x0001DC93
		// (set) Token: 0x0600073A RID: 1850 RVA: 0x0001FA9B File Offset: 0x0001DC9B
		[DataSourceProperty]
		public int DefendersState
		{
			get
			{
				return this._defendersState;
			}
			set
			{
				if (value != this._defendersState)
				{
					this._defendersState = value;
					base.OnPropertyChangedWithValue(value, "DefendersState");
				}
			}
		}

		// Token: 0x17000225 RID: 549
		// (get) Token: 0x0600073B RID: 1851 RVA: 0x0001FAB9 File Offset: 0x0001DCB9
		// (set) Token: 0x0600073C RID: 1852 RVA: 0x0001FAC1 File Offset: 0x0001DCC1
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

		// Token: 0x17000226 RID: 550
		// (get) Token: 0x0600073D RID: 1853 RVA: 0x0001FADF File Offset: 0x0001DCDF
		// (set) Token: 0x0600073E RID: 1854 RVA: 0x0001FAE7 File Offset: 0x0001DCE7
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

		// Token: 0x17000227 RID: 551
		// (get) Token: 0x0600073F RID: 1855 RVA: 0x0001FB05 File Offset: 0x0001DD05
		// (set) Token: 0x06000740 RID: 1856 RVA: 0x0001FB0D File Offset: 0x0001DD0D
		[DataSourceProperty]
		public bool IsDefendersSelected
		{
			get
			{
				return this._isDefendersSelected;
			}
			set
			{
				if (value != this._isDefendersSelected)
				{
					this._isDefendersSelected = value;
					base.OnPropertyChangedWithValue(value, "IsDefendersSelected");
				}
			}
		}

		// Token: 0x17000228 RID: 552
		// (get) Token: 0x06000741 RID: 1857 RVA: 0x0001FB2B File Offset: 0x0001DD2B
		// (set) Token: 0x06000742 RID: 1858 RVA: 0x0001FB33 File Offset: 0x0001DD33
		[DataSourceProperty]
		public bool IsOwnerSelected
		{
			get
			{
				return this._isOwnerSelected;
			}
			set
			{
				if (value != this._isOwnerSelected)
				{
					this._isOwnerSelected = value;
					base.OnPropertyChangedWithValue(value, "IsOwnerSelected");
				}
			}
		}

		// Token: 0x17000229 RID: 553
		// (get) Token: 0x06000743 RID: 1859 RVA: 0x0001FB51 File Offset: 0x0001DD51
		// (set) Token: 0x06000744 RID: 1860 RVA: 0x0001FB59 File Offset: 0x0001DD59
		[DataSourceProperty]
		public bool IsProsperitySelected
		{
			get
			{
				return this._isProsperitySelected;
			}
			set
			{
				if (value != this._isProsperitySelected)
				{
					this._isProsperitySelected = value;
					base.OnPropertyChangedWithValue(value, "IsProsperitySelected");
				}
			}
		}

		// Token: 0x04000324 RID: 804
		private readonly MBBindingList<KingdomSettlementItemVM> _listToControl;

		// Token: 0x04000325 RID: 805
		private readonly KingdomSettlementSortControllerVM.ItemTypeComparer _typeComparer;

		// Token: 0x04000326 RID: 806
		private readonly KingdomSettlementSortControllerVM.ItemProsperityComparer _prosperityComparer;

		// Token: 0x04000327 RID: 807
		private readonly KingdomSettlementSortControllerVM.ItemDefendersComparer _defendersComparer;

		// Token: 0x04000328 RID: 808
		private readonly KingdomSettlementSortControllerVM.ItemNameComparer _nameComparer;

		// Token: 0x04000329 RID: 809
		private readonly KingdomSettlementSortControllerVM.ItemOwnerComparer _ownerComparer;

		// Token: 0x0400032A RID: 810
		private int _typeState;

		// Token: 0x0400032B RID: 811
		private int _nameState;

		// Token: 0x0400032C RID: 812
		private int _ownerState;

		// Token: 0x0400032D RID: 813
		private int _prosperityState;

		// Token: 0x0400032E RID: 814
		private int _defendersState;

		// Token: 0x0400032F RID: 815
		private bool _isTypeSelected;

		// Token: 0x04000330 RID: 816
		private bool _isNameSelected;

		// Token: 0x04000331 RID: 817
		private bool _isOwnerSelected;

		// Token: 0x04000332 RID: 818
		private bool _isProsperitySelected;

		// Token: 0x04000333 RID: 819
		private bool _isDefendersSelected;

		// Token: 0x02000184 RID: 388
		private enum SortState
		{
			// Token: 0x04000F1E RID: 3870
			Default,
			// Token: 0x04000F1F RID: 3871
			Ascending,
			// Token: 0x04000F20 RID: 3872
			Descending
		}

		// Token: 0x02000185 RID: 389
		public abstract class ItemComparerBase : IComparer<KingdomSettlementItemVM>
		{
			// Token: 0x06001F52 RID: 8018 RVA: 0x0006E4E2 File Offset: 0x0006C6E2
			public void SetSortMode(bool isAscending)
			{
				this._isAscending = isAscending;
			}

			// Token: 0x06001F53 RID: 8019
			public abstract int Compare(KingdomSettlementItemVM x, KingdomSettlementItemVM y);

			// Token: 0x06001F54 RID: 8020 RVA: 0x0006E4EB File Offset: 0x0006C6EB
			protected int ResolveEquality(KingdomSettlementItemVM x, KingdomSettlementItemVM y)
			{
				return x.Settlement.Name.ToString().CompareTo(y.Settlement.Name.ToString());
			}

			// Token: 0x04000F21 RID: 3873
			protected bool _isAscending;
		}

		// Token: 0x02000186 RID: 390
		public class ItemNameComparer : KingdomSettlementSortControllerVM.ItemComparerBase
		{
			// Token: 0x06001F56 RID: 8022 RVA: 0x0006E51C File Offset: 0x0006C71C
			public override int Compare(KingdomSettlementItemVM x, KingdomSettlementItemVM y)
			{
				if (this._isAscending)
				{
					return y.Settlement.Name.ToString().CompareTo(x.Settlement.Name.ToString()) * -1;
				}
				return y.Settlement.Name.ToString().CompareTo(x.Settlement.Name.ToString());
			}
		}

		// Token: 0x02000187 RID: 391
		public class ItemClanComparer : KingdomSettlementSortControllerVM.ItemComparerBase
		{
			// Token: 0x06001F58 RID: 8024 RVA: 0x0006E588 File Offset: 0x0006C788
			public override int Compare(KingdomSettlementItemVM x, KingdomSettlementItemVM y)
			{
				int num = y.Settlement.OwnerClan.Name.ToString().CompareTo(x.Settlement.OwnerClan.Name.ToString());
				if (num != 0)
				{
					return num * (this._isAscending ? (-1) : 1);
				}
				return base.ResolveEquality(x, y);
			}
		}

		// Token: 0x02000188 RID: 392
		public class ItemOwnerComparer : KingdomSettlementSortControllerVM.ItemComparerBase
		{
			// Token: 0x06001F5A RID: 8026 RVA: 0x0006E5E8 File Offset: 0x0006C7E8
			public override int Compare(KingdomSettlementItemVM x, KingdomSettlementItemVM y)
			{
				int num = y.Owner.NameText.CompareTo(x.Owner.NameText);
				if (num != 0)
				{
					return num * (this._isAscending ? (-1) : 1);
				}
				return base.ResolveEquality(x, y);
			}
		}

		// Token: 0x02000189 RID: 393
		public class ItemVillagesComparer : KingdomSettlementSortControllerVM.ItemComparerBase
		{
			// Token: 0x06001F5C RID: 8028 RVA: 0x0006E634 File Offset: 0x0006C834
			public override int Compare(KingdomSettlementItemVM x, KingdomSettlementItemVM y)
			{
				int num = y.Villages.Count.CompareTo(x.Villages.Count);
				if (num != 0)
				{
					return num * (this._isAscending ? (-1) : 1);
				}
				return base.ResolveEquality(x, y);
			}
		}

		// Token: 0x0200018A RID: 394
		public class ItemTypeComparer : KingdomSettlementSortControllerVM.ItemComparerBase
		{
			// Token: 0x06001F5E RID: 8030 RVA: 0x0006E684 File Offset: 0x0006C884
			public override int Compare(KingdomSettlementItemVM x, KingdomSettlementItemVM y)
			{
				int num = y.Settlement.IsCastle.CompareTo(x.Settlement.IsCastle);
				if (num != 0)
				{
					return num * (this._isAscending ? (-1) : 1);
				}
				return base.ResolveEquality(x, y);
			}
		}

		// Token: 0x0200018B RID: 395
		public class ItemProsperityComparer : KingdomSettlementSortControllerVM.ItemComparerBase
		{
			// Token: 0x06001F60 RID: 8032 RVA: 0x0006E6D4 File Offset: 0x0006C8D4
			public override int Compare(KingdomSettlementItemVM x, KingdomSettlementItemVM y)
			{
				int num = y.Prosperity.CompareTo(x.Prosperity);
				if (num != 0)
				{
					return num * (this._isAscending ? (-1) : 1);
				}
				return base.ResolveEquality(x, y);
			}
		}

		// Token: 0x0200018C RID: 396
		public class ItemFoodComparer : KingdomSettlementSortControllerVM.ItemComparerBase
		{
			// Token: 0x06001F62 RID: 8034 RVA: 0x0006E718 File Offset: 0x0006C918
			public override int Compare(KingdomSettlementItemVM x, KingdomSettlementItemVM y)
			{
				float num = ((y.Settlement.Town != null) ? y.Settlement.Town.FoodStocks : 0f);
				float num2 = ((x.Settlement.Town != null) ? x.Settlement.Town.FoodStocks : 0f);
				int num3 = num.CompareTo(num2);
				if (num3 != 0)
				{
					return num3 * (this._isAscending ? (-1) : 1);
				}
				return base.ResolveEquality(x, y);
			}
		}

		// Token: 0x0200018D RID: 397
		public class ItemGarrisonComparer : KingdomSettlementSortControllerVM.ItemComparerBase
		{
			// Token: 0x06001F64 RID: 8036 RVA: 0x0006E79C File Offset: 0x0006C99C
			public override int Compare(KingdomSettlementItemVM x, KingdomSettlementItemVM y)
			{
				int num = y.Garrison.CompareTo(x.Garrison);
				if (num != 0)
				{
					return num * (this._isAscending ? (-1) : 1);
				}
				return base.ResolveEquality(x, y);
			}
		}

		// Token: 0x0200018E RID: 398
		private class ItemDefendersComparer : KingdomSettlementSortControllerVM.ItemComparerBase
		{
			// Token: 0x06001F66 RID: 8038 RVA: 0x0006E7E0 File Offset: 0x0006C9E0
			public override int Compare(KingdomSettlementItemVM x, KingdomSettlementItemVM y)
			{
				int num = y.Defenders.CompareTo(x.Defenders);
				if (num != 0)
				{
					return num * (this._isAscending ? (-1) : 1);
				}
				return base.ResolveEquality(x, y);
			}
		}
	}
}
