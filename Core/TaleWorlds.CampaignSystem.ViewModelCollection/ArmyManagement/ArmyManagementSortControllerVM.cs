using System;
using System.Collections.Generic;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.ArmyManagement
{
	// Token: 0x02000133 RID: 307
	public class ArmyManagementSortControllerVM : ViewModel
	{
		// Token: 0x06001D9A RID: 7578 RVA: 0x00069904 File Offset: 0x00067B04
		public ArmyManagementSortControllerVM(ref MBBindingList<ArmyManagementItemVM> listToControl)
		{
			this._listToControl = listToControl;
			this._distanceComparer = new ArmyManagementSortControllerVM.ItemDistanceComparer();
			this._costComparer = new ArmyManagementSortControllerVM.ItemCostComparer();
			this._strengthComparer = new ArmyManagementSortControllerVM.ItemStrengthComparer();
			this._nameComparer = new ArmyManagementSortControllerVM.ItemNameComparer();
			this._clanComparer = new ArmyManagementSortControllerVM.ItemClanComparer();
		}

		// Token: 0x06001D9B RID: 7579 RVA: 0x00069958 File Offset: 0x00067B58
		public void ExecuteSortByDistance()
		{
			int distanceState = this.DistanceState;
			this.SetAllStates(ArmyManagementSortControllerVM.SortState.Default);
			this.DistanceState = (distanceState + 1) % 3;
			if (this.DistanceState == 0)
			{
				int distanceState2 = this.DistanceState;
				this.DistanceState = distanceState2 + 1;
			}
			this._distanceComparer.SetSortMode(this.DistanceState == 1);
			this._listToControl.Sort(this._distanceComparer);
			this.IsDistanceSelected = true;
		}

		// Token: 0x06001D9C RID: 7580 RVA: 0x000699C4 File Offset: 0x00067BC4
		public void ExecuteSortByCost()
		{
			int costState = this.CostState;
			this.SetAllStates(ArmyManagementSortControllerVM.SortState.Default);
			this.CostState = (costState + 1) % 3;
			if (this.CostState == 0)
			{
				int costState2 = this.CostState;
				this.CostState = costState2 + 1;
			}
			this._costComparer.SetSortMode(this.CostState == 1);
			this._listToControl.Sort(this._costComparer);
			this.IsCostSelected = true;
		}

		// Token: 0x06001D9D RID: 7581 RVA: 0x00069A30 File Offset: 0x00067C30
		public void ExecuteSortByStrength()
		{
			int strengthState = this.StrengthState;
			this.SetAllStates(ArmyManagementSortControllerVM.SortState.Default);
			this.StrengthState = (strengthState + 1) % 3;
			if (this.StrengthState == 0)
			{
				int strengthState2 = this.StrengthState;
				this.StrengthState = strengthState2 + 1;
			}
			this._strengthComparer.SetSortMode(this.StrengthState == 1);
			this._listToControl.Sort(this._strengthComparer);
			this.IsStrengthSelected = true;
		}

		// Token: 0x06001D9E RID: 7582 RVA: 0x00069A9C File Offset: 0x00067C9C
		public void ExecuteSortByName()
		{
			int nameState = this.NameState;
			this.SetAllStates(ArmyManagementSortControllerVM.SortState.Default);
			this.NameState = (nameState + 1) % 3;
			if (this.NameState == 0)
			{
				int nameState2 = this.NameState;
				this.NameState = nameState2 + 1;
			}
			this._nameComparer.SetSortMode(this.NameState == 1);
			this._listToControl.Sort(this._nameComparer);
			this.IsNameSelected = true;
		}

		// Token: 0x06001D9F RID: 7583 RVA: 0x00069B08 File Offset: 0x00067D08
		public void ExecuteSortByClan()
		{
			int clanState = this.ClanState;
			this.SetAllStates(ArmyManagementSortControllerVM.SortState.Default);
			this.ClanState = (clanState + 1) % 3;
			if (this.ClanState == 0)
			{
				int clanState2 = this.ClanState;
				this.ClanState = clanState2 + 1;
			}
			this._clanComparer.SetSortMode(this.ClanState == 1);
			this._listToControl.Sort(this._clanComparer);
			this.IsClanSelected = true;
		}

		// Token: 0x06001DA0 RID: 7584 RVA: 0x00069B74 File Offset: 0x00067D74
		private void SetAllStates(ArmyManagementSortControllerVM.SortState state)
		{
			this.DistanceState = (int)state;
			this.CostState = (int)state;
			this.StrengthState = (int)state;
			this.NameState = (int)state;
			this.ClanState = (int)state;
			this.IsDistanceSelected = false;
			this.IsCostSelected = false;
			this.IsNameSelected = false;
			this.IsClanSelected = false;
			this.IsStrengthSelected = false;
		}

		// Token: 0x17000A28 RID: 2600
		// (get) Token: 0x06001DA1 RID: 7585 RVA: 0x00069BC7 File Offset: 0x00067DC7
		// (set) Token: 0x06001DA2 RID: 7586 RVA: 0x00069BCF File Offset: 0x00067DCF
		[DataSourceProperty]
		public int DistanceState
		{
			get
			{
				return this._distanceState;
			}
			set
			{
				if (value != this._distanceState)
				{
					this._distanceState = value;
					base.OnPropertyChangedWithValue(value, "DistanceState");
				}
			}
		}

		// Token: 0x17000A29 RID: 2601
		// (get) Token: 0x06001DA3 RID: 7587 RVA: 0x00069BED File Offset: 0x00067DED
		// (set) Token: 0x06001DA4 RID: 7588 RVA: 0x00069BF5 File Offset: 0x00067DF5
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

		// Token: 0x17000A2A RID: 2602
		// (get) Token: 0x06001DA5 RID: 7589 RVA: 0x00069C13 File Offset: 0x00067E13
		// (set) Token: 0x06001DA6 RID: 7590 RVA: 0x00069C1B File Offset: 0x00067E1B
		[DataSourceProperty]
		public int StrengthState
		{
			get
			{
				return this._strengthState;
			}
			set
			{
				if (value != this._strengthState)
				{
					this._strengthState = value;
					base.OnPropertyChangedWithValue(value, "StrengthState");
				}
			}
		}

		// Token: 0x17000A2B RID: 2603
		// (get) Token: 0x06001DA7 RID: 7591 RVA: 0x00069C39 File Offset: 0x00067E39
		// (set) Token: 0x06001DA8 RID: 7592 RVA: 0x00069C41 File Offset: 0x00067E41
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

		// Token: 0x17000A2C RID: 2604
		// (get) Token: 0x06001DA9 RID: 7593 RVA: 0x00069C5F File Offset: 0x00067E5F
		// (set) Token: 0x06001DAA RID: 7594 RVA: 0x00069C67 File Offset: 0x00067E67
		[DataSourceProperty]
		public int ClanState
		{
			get
			{
				return this._clanState;
			}
			set
			{
				if (value != this._clanState)
				{
					this._clanState = value;
					base.OnPropertyChangedWithValue(value, "ClanState");
				}
			}
		}

		// Token: 0x17000A2D RID: 2605
		// (get) Token: 0x06001DAB RID: 7595 RVA: 0x00069C85 File Offset: 0x00067E85
		// (set) Token: 0x06001DAC RID: 7596 RVA: 0x00069C8D File Offset: 0x00067E8D
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

		// Token: 0x17000A2E RID: 2606
		// (get) Token: 0x06001DAD RID: 7597 RVA: 0x00069CAB File Offset: 0x00067EAB
		// (set) Token: 0x06001DAE RID: 7598 RVA: 0x00069CB3 File Offset: 0x00067EB3
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

		// Token: 0x17000A2F RID: 2607
		// (get) Token: 0x06001DAF RID: 7599 RVA: 0x00069CD1 File Offset: 0x00067ED1
		// (set) Token: 0x06001DB0 RID: 7600 RVA: 0x00069CD9 File Offset: 0x00067ED9
		[DataSourceProperty]
		public bool IsStrengthSelected
		{
			get
			{
				return this._isStrengthSelected;
			}
			set
			{
				if (value != this._isStrengthSelected)
				{
					this._isStrengthSelected = value;
					base.OnPropertyChangedWithValue(value, "IsStrengthSelected");
				}
			}
		}

		// Token: 0x17000A30 RID: 2608
		// (get) Token: 0x06001DB1 RID: 7601 RVA: 0x00069CF7 File Offset: 0x00067EF7
		// (set) Token: 0x06001DB2 RID: 7602 RVA: 0x00069CFF File Offset: 0x00067EFF
		[DataSourceProperty]
		public bool IsDistanceSelected
		{
			get
			{
				return this._isDistanceSelected;
			}
			set
			{
				if (value != this._isDistanceSelected)
				{
					this._isDistanceSelected = value;
					base.OnPropertyChangedWithValue(value, "IsDistanceSelected");
				}
			}
		}

		// Token: 0x17000A31 RID: 2609
		// (get) Token: 0x06001DB3 RID: 7603 RVA: 0x00069D1D File Offset: 0x00067F1D
		// (set) Token: 0x06001DB4 RID: 7604 RVA: 0x00069D25 File Offset: 0x00067F25
		[DataSourceProperty]
		public bool IsClanSelected
		{
			get
			{
				return this._isClanSelected;
			}
			set
			{
				if (value != this._isClanSelected)
				{
					this._isClanSelected = value;
					base.OnPropertyChangedWithValue(value, "IsClanSelected");
				}
			}
		}

		// Token: 0x04000DF2 RID: 3570
		private readonly MBBindingList<ArmyManagementItemVM> _listToControl;

		// Token: 0x04000DF3 RID: 3571
		private readonly ArmyManagementSortControllerVM.ItemDistanceComparer _distanceComparer;

		// Token: 0x04000DF4 RID: 3572
		private readonly ArmyManagementSortControllerVM.ItemCostComparer _costComparer;

		// Token: 0x04000DF5 RID: 3573
		private readonly ArmyManagementSortControllerVM.ItemStrengthComparer _strengthComparer;

		// Token: 0x04000DF6 RID: 3574
		private readonly ArmyManagementSortControllerVM.ItemNameComparer _nameComparer;

		// Token: 0x04000DF7 RID: 3575
		private readonly ArmyManagementSortControllerVM.ItemClanComparer _clanComparer;

		// Token: 0x04000DF8 RID: 3576
		private int _distanceState;

		// Token: 0x04000DF9 RID: 3577
		private int _costState;

		// Token: 0x04000DFA RID: 3578
		private int _strengthState;

		// Token: 0x04000DFB RID: 3579
		private int _nameState;

		// Token: 0x04000DFC RID: 3580
		private int _clanState;

		// Token: 0x04000DFD RID: 3581
		private bool _isNameSelected;

		// Token: 0x04000DFE RID: 3582
		private bool _isCostSelected;

		// Token: 0x04000DFF RID: 3583
		private bool _isStrengthSelected;

		// Token: 0x04000E00 RID: 3584
		private bool _isDistanceSelected;

		// Token: 0x04000E01 RID: 3585
		private bool _isClanSelected;

		// Token: 0x0200027F RID: 639
		private enum SortState
		{
			// Token: 0x040011C2 RID: 4546
			Default,
			// Token: 0x040011C3 RID: 4547
			Ascending,
			// Token: 0x040011C4 RID: 4548
			Descending
		}

		// Token: 0x02000280 RID: 640
		public abstract class ItemComparerBase : IComparer<ArmyManagementItemVM>
		{
			// Token: 0x06002285 RID: 8837 RVA: 0x00072E5B File Offset: 0x0007105B
			public void SetSortMode(bool isAscending)
			{
				this._isAscending = isAscending;
			}

			// Token: 0x06002286 RID: 8838
			public abstract int Compare(ArmyManagementItemVM x, ArmyManagementItemVM y);

			// Token: 0x06002287 RID: 8839 RVA: 0x00072E64 File Offset: 0x00071064
			protected int ResolveEquality(ArmyManagementItemVM x, ArmyManagementItemVM y)
			{
				return x.LeaderNameText.CompareTo(y.LeaderNameText);
			}

			// Token: 0x040011C5 RID: 4549
			protected bool _isAscending;
		}

		// Token: 0x02000281 RID: 641
		public class ItemDistanceComparer : ArmyManagementSortControllerVM.ItemComparerBase
		{
			// Token: 0x06002289 RID: 8841 RVA: 0x00072E80 File Offset: 0x00071080
			public override int Compare(ArmyManagementItemVM x, ArmyManagementItemVM y)
			{
				int num = y.DistInTime.CompareTo(x.DistInTime);
				if (num != 0)
				{
					return num * (this._isAscending ? (-1) : 1);
				}
				return base.ResolveEquality(x, y);
			}
		}

		// Token: 0x02000282 RID: 642
		public class ItemCostComparer : ArmyManagementSortControllerVM.ItemComparerBase
		{
			// Token: 0x0600228B RID: 8843 RVA: 0x00072EC4 File Offset: 0x000710C4
			public override int Compare(ArmyManagementItemVM x, ArmyManagementItemVM y)
			{
				int num = y.Cost.CompareTo(x.Cost);
				if (num != 0)
				{
					return num * (this._isAscending ? (-1) : 1);
				}
				return base.ResolveEquality(x, y);
			}
		}

		// Token: 0x02000283 RID: 643
		public class ItemStrengthComparer : ArmyManagementSortControllerVM.ItemComparerBase
		{
			// Token: 0x0600228D RID: 8845 RVA: 0x00072F08 File Offset: 0x00071108
			public override int Compare(ArmyManagementItemVM x, ArmyManagementItemVM y)
			{
				int num = y.Strength.CompareTo(x.Strength);
				if (num != 0)
				{
					return num * (this._isAscending ? (-1) : 1);
				}
				return base.ResolveEquality(x, y);
			}
		}

		// Token: 0x02000284 RID: 644
		public class ItemNameComparer : ArmyManagementSortControllerVM.ItemComparerBase
		{
			// Token: 0x0600228F RID: 8847 RVA: 0x00072F4C File Offset: 0x0007114C
			public override int Compare(ArmyManagementItemVM x, ArmyManagementItemVM y)
			{
				if (this._isAscending)
				{
					return y.LeaderNameText.CompareTo(x.LeaderNameText) * -1;
				}
				return y.LeaderNameText.CompareTo(x.LeaderNameText);
			}
		}

		// Token: 0x02000285 RID: 645
		public class ItemClanComparer : ArmyManagementSortControllerVM.ItemComparerBase
		{
			// Token: 0x06002291 RID: 8849 RVA: 0x00072F84 File Offset: 0x00071184
			public override int Compare(ArmyManagementItemVM x, ArmyManagementItemVM y)
			{
				int num = y.Clan.Name.ToString().CompareTo(x.Clan.Name.ToString());
				if (num != 0)
				{
					return num * (this._isAscending ? (-1) : 1);
				}
				return base.ResolveEquality(x, y);
			}
		}
	}
}
