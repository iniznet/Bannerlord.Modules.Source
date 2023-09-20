using System;
using System.Collections.Generic;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.KingdomManagement.Armies
{
	// Token: 0x02000075 RID: 117
	public class KingdomArmySortControllerVM : ViewModel
	{
		// Token: 0x06000A5A RID: 2650 RVA: 0x0002946C File Offset: 0x0002766C
		public KingdomArmySortControllerVM(ref MBBindingList<KingdomArmyItemVM> listToControl)
		{
			this._listToControl = listToControl;
			this._ownerComparer = new KingdomArmySortControllerVM.ItemOwnerComparer();
			this._strengthComparer = new KingdomArmySortControllerVM.ItemStrengthComparer();
			this._nameComparer = new KingdomArmySortControllerVM.ItemNameComparer();
			this._partiesComparer = new KingdomArmySortControllerVM.ItemPartiesComparer();
			this._distanceComparer = new KingdomArmySortControllerVM.ItemDistanceComparer();
		}

		// Token: 0x06000A5B RID: 2651 RVA: 0x000294C0 File Offset: 0x000276C0
		private void ExecuteSortByName()
		{
			int nameState = this.NameState;
			this.SetAllStates(KingdomArmySortControllerVM.SortState.Default);
			this.NameState = (nameState + 1) % 3;
			if (this.NameState == 0)
			{
				this.NameState++;
			}
			this._nameComparer.SetSortMode(this.NameState == 1);
			this._listToControl.Sort(this._nameComparer);
			this.IsNameSelected = true;
		}

		// Token: 0x06000A5C RID: 2652 RVA: 0x00029528 File Offset: 0x00027728
		private void ExecuteSortByOwner()
		{
			int ownerState = this.OwnerState;
			this.SetAllStates(KingdomArmySortControllerVM.SortState.Default);
			this.OwnerState = (ownerState + 1) % 3;
			if (this.OwnerState == 0)
			{
				this.OwnerState++;
			}
			this._ownerComparer.SetSortMode(this.OwnerState == 1);
			this._listToControl.Sort(this._ownerComparer);
			this.IsOwnerSelected = true;
		}

		// Token: 0x06000A5D RID: 2653 RVA: 0x00029590 File Offset: 0x00027790
		private void ExecuteSortByStrength()
		{
			int strengthState = this.StrengthState;
			this.SetAllStates(KingdomArmySortControllerVM.SortState.Default);
			this.StrengthState = (strengthState + 1) % 3;
			if (this.StrengthState == 0)
			{
				this.StrengthState++;
			}
			this._strengthComparer.SetSortMode(this.StrengthState == 1);
			this._listToControl.Sort(this._strengthComparer);
			this.IsStrengthSelected = true;
		}

		// Token: 0x06000A5E RID: 2654 RVA: 0x000295F8 File Offset: 0x000277F8
		private void ExecuteSortByParties()
		{
			int partiesState = this.PartiesState;
			this.SetAllStates(KingdomArmySortControllerVM.SortState.Default);
			this.PartiesState = (partiesState + 1) % 3;
			if (this.PartiesState == 0)
			{
				this.PartiesState++;
			}
			this._partiesComparer.SetSortMode(this.PartiesState == 1);
			this._listToControl.Sort(this._partiesComparer);
			this.IsPartiesSelected = true;
		}

		// Token: 0x06000A5F RID: 2655 RVA: 0x00029660 File Offset: 0x00027860
		private void ExecuteSortByDistance()
		{
			int distanceState = this.DistanceState;
			this.SetAllStates(KingdomArmySortControllerVM.SortState.Default);
			this.DistanceState = (distanceState + 1) % 3;
			if (this.DistanceState == 0)
			{
				this.DistanceState++;
			}
			this._distanceComparer.SetSortMode(this.DistanceState == 1);
			this._listToControl.Sort(this._distanceComparer);
			this.IsDistanceSelected = true;
		}

		// Token: 0x06000A60 RID: 2656 RVA: 0x000296C8 File Offset: 0x000278C8
		private void SetAllStates(KingdomArmySortControllerVM.SortState state)
		{
			this.NameState = (int)state;
			this.OwnerState = (int)state;
			this.StrengthState = (int)state;
			this.PartiesState = (int)state;
			this.DistanceState = (int)state;
			this.IsNameSelected = false;
			this.IsOwnerSelected = false;
			this.IsStrengthSelected = false;
			this.IsPartiesSelected = false;
			this.IsDistanceSelected = false;
		}

		// Token: 0x17000359 RID: 857
		// (get) Token: 0x06000A61 RID: 2657 RVA: 0x0002971B File Offset: 0x0002791B
		// (set) Token: 0x06000A62 RID: 2658 RVA: 0x00029723 File Offset: 0x00027923
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

		// Token: 0x1700035A RID: 858
		// (get) Token: 0x06000A63 RID: 2659 RVA: 0x00029741 File Offset: 0x00027941
		// (set) Token: 0x06000A64 RID: 2660 RVA: 0x00029749 File Offset: 0x00027949
		[DataSourceProperty]
		public int PartiesState
		{
			get
			{
				return this._partiesState;
			}
			set
			{
				if (value != this._partiesState)
				{
					this._partiesState = value;
					base.OnPropertyChangedWithValue(value, "PartiesState");
				}
			}
		}

		// Token: 0x1700035B RID: 859
		// (get) Token: 0x06000A65 RID: 2661 RVA: 0x00029767 File Offset: 0x00027967
		// (set) Token: 0x06000A66 RID: 2662 RVA: 0x0002976F File Offset: 0x0002796F
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

		// Token: 0x1700035C RID: 860
		// (get) Token: 0x06000A67 RID: 2663 RVA: 0x0002978D File Offset: 0x0002798D
		// (set) Token: 0x06000A68 RID: 2664 RVA: 0x00029795 File Offset: 0x00027995
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

		// Token: 0x1700035D RID: 861
		// (get) Token: 0x06000A69 RID: 2665 RVA: 0x000297B3 File Offset: 0x000279B3
		// (set) Token: 0x06000A6A RID: 2666 RVA: 0x000297BB File Offset: 0x000279BB
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

		// Token: 0x1700035E RID: 862
		// (get) Token: 0x06000A6B RID: 2667 RVA: 0x000297D9 File Offset: 0x000279D9
		// (set) Token: 0x06000A6C RID: 2668 RVA: 0x000297E1 File Offset: 0x000279E1
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

		// Token: 0x1700035F RID: 863
		// (get) Token: 0x06000A6D RID: 2669 RVA: 0x000297FF File Offset: 0x000279FF
		// (set) Token: 0x06000A6E RID: 2670 RVA: 0x00029807 File Offset: 0x00027A07
		[DataSourceProperty]
		public bool IsPartiesSelected
		{
			get
			{
				return this._isPartiesSelected;
			}
			set
			{
				if (value != this._isPartiesSelected)
				{
					this._isPartiesSelected = value;
					base.OnPropertyChangedWithValue(value, "IsPartiesSelected");
				}
			}
		}

		// Token: 0x17000360 RID: 864
		// (get) Token: 0x06000A6F RID: 2671 RVA: 0x00029825 File Offset: 0x00027A25
		// (set) Token: 0x06000A70 RID: 2672 RVA: 0x0002982D File Offset: 0x00027A2D
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

		// Token: 0x17000361 RID: 865
		// (get) Token: 0x06000A71 RID: 2673 RVA: 0x0002984B File Offset: 0x00027A4B
		// (set) Token: 0x06000A72 RID: 2674 RVA: 0x00029853 File Offset: 0x00027A53
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

		// Token: 0x17000362 RID: 866
		// (get) Token: 0x06000A73 RID: 2675 RVA: 0x00029871 File Offset: 0x00027A71
		// (set) Token: 0x06000A74 RID: 2676 RVA: 0x00029879 File Offset: 0x00027A79
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

		// Token: 0x040004A9 RID: 1193
		private readonly MBBindingList<KingdomArmyItemVM> _listToControl;

		// Token: 0x040004AA RID: 1194
		private readonly KingdomArmySortControllerVM.ItemNameComparer _nameComparer;

		// Token: 0x040004AB RID: 1195
		private readonly KingdomArmySortControllerVM.ItemOwnerComparer _ownerComparer;

		// Token: 0x040004AC RID: 1196
		private readonly KingdomArmySortControllerVM.ItemStrengthComparer _strengthComparer;

		// Token: 0x040004AD RID: 1197
		private readonly KingdomArmySortControllerVM.ItemPartiesComparer _partiesComparer;

		// Token: 0x040004AE RID: 1198
		private readonly KingdomArmySortControllerVM.ItemDistanceComparer _distanceComparer;

		// Token: 0x040004AF RID: 1199
		private int _nameState;

		// Token: 0x040004B0 RID: 1200
		private int _ownerState;

		// Token: 0x040004B1 RID: 1201
		private int _strengthState;

		// Token: 0x040004B2 RID: 1202
		private int _partiesState;

		// Token: 0x040004B3 RID: 1203
		private int _distanceState;

		// Token: 0x040004B4 RID: 1204
		private bool _isNameSelected;

		// Token: 0x040004B5 RID: 1205
		private bool _isOwnerSelected;

		// Token: 0x040004B6 RID: 1206
		private bool _isStrengthSelected;

		// Token: 0x040004B7 RID: 1207
		private bool _isPartiesSelected;

		// Token: 0x040004B8 RID: 1208
		private bool _isDistanceSelected;

		// Token: 0x020001AE RID: 430
		private enum SortState
		{
			// Token: 0x04000F6E RID: 3950
			Default,
			// Token: 0x04000F6F RID: 3951
			Ascending,
			// Token: 0x04000F70 RID: 3952
			Descending
		}

		// Token: 0x020001AF RID: 431
		public abstract class ItemComparerBase : IComparer<KingdomArmyItemVM>
		{
			// Token: 0x06001FC5 RID: 8133 RVA: 0x0006EE77 File Offset: 0x0006D077
			public void SetSortMode(bool isAscending)
			{
				this._isAscending = isAscending;
			}

			// Token: 0x06001FC6 RID: 8134
			public abstract int Compare(KingdomArmyItemVM x, KingdomArmyItemVM y);

			// Token: 0x06001FC7 RID: 8135 RVA: 0x0006EE80 File Offset: 0x0006D080
			protected int ResolveEquality(KingdomArmyItemVM x, KingdomArmyItemVM y)
			{
				return x.ArmyName.CompareTo(y.ArmyName);
			}

			// Token: 0x04000F71 RID: 3953
			protected bool _isAscending;
		}

		// Token: 0x020001B0 RID: 432
		public class ItemNameComparer : KingdomArmySortControllerVM.ItemComparerBase
		{
			// Token: 0x06001FC9 RID: 8137 RVA: 0x0006EE9B File Offset: 0x0006D09B
			public override int Compare(KingdomArmyItemVM x, KingdomArmyItemVM y)
			{
				if (this._isAscending)
				{
					return y.ArmyName.CompareTo(x.ArmyName) * -1;
				}
				return y.ArmyName.CompareTo(x.ArmyName);
			}
		}

		// Token: 0x020001B1 RID: 433
		public class ItemOwnerComparer : KingdomArmySortControllerVM.ItemComparerBase
		{
			// Token: 0x06001FCB RID: 8139 RVA: 0x0006EED4 File Offset: 0x0006D0D4
			public override int Compare(KingdomArmyItemVM x, KingdomArmyItemVM y)
			{
				int num = y.Leader.NameText.ToString().CompareTo(x.Leader.NameText.ToString());
				if (num != 0)
				{
					return num * (this._isAscending ? (-1) : 1);
				}
				return base.ResolveEquality(x, y);
			}
		}

		// Token: 0x020001B2 RID: 434
		public class ItemStrengthComparer : KingdomArmySortControllerVM.ItemComparerBase
		{
			// Token: 0x06001FCD RID: 8141 RVA: 0x0006EF2C File Offset: 0x0006D12C
			public override int Compare(KingdomArmyItemVM x, KingdomArmyItemVM y)
			{
				int num = y.Strength.CompareTo(x.Strength);
				if (num != 0)
				{
					return num * (this._isAscending ? (-1) : 1);
				}
				return base.ResolveEquality(x, y);
			}
		}

		// Token: 0x020001B3 RID: 435
		public class ItemPartiesComparer : KingdomArmySortControllerVM.ItemComparerBase
		{
			// Token: 0x06001FCF RID: 8143 RVA: 0x0006EF70 File Offset: 0x0006D170
			public override int Compare(KingdomArmyItemVM x, KingdomArmyItemVM y)
			{
				int num = y.Parties.Count.CompareTo(x.Parties.Count);
				if (num != 0)
				{
					return num * (this._isAscending ? (-1) : 1);
				}
				return base.ResolveEquality(x, y);
			}
		}

		// Token: 0x020001B4 RID: 436
		public class ItemDistanceComparer : KingdomArmySortControllerVM.ItemComparerBase
		{
			// Token: 0x06001FD1 RID: 8145 RVA: 0x0006EFC0 File Offset: 0x0006D1C0
			public override int Compare(KingdomArmyItemVM x, KingdomArmyItemVM y)
			{
				int num = y.DistanceToMainParty.CompareTo(x.DistanceToMainParty);
				if (num != 0)
				{
					return num * (this._isAscending ? (-1) : 1);
				}
				return base.ResolveEquality(x, y);
			}
		}
	}
}
