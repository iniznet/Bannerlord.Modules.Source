using System;
using System.Collections.Generic;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.KingdomManagement.Clans
{
	// Token: 0x02000071 RID: 113
	public class KingdomClanSortControllerVM : ViewModel
	{
		// Token: 0x060009E3 RID: 2531 RVA: 0x00027FCC File Offset: 0x000261CC
		public KingdomClanSortControllerVM(ref MBBindingList<KingdomClanItemVM> listToControl)
		{
			this._listToControl = listToControl;
			this._influenceComparer = new KingdomClanSortControllerVM.ItemInfluenceComparer();
			this._membersComparer = new KingdomClanSortControllerVM.ItemMembersComparer();
			this._nameComparer = new KingdomClanSortControllerVM.ItemNameComparer();
			this._fiefsComparer = new KingdomClanSortControllerVM.ItemFiefsComparer();
			this._typeComparer = new KingdomClanSortControllerVM.ItemTypeComparer();
		}

		// Token: 0x060009E4 RID: 2532 RVA: 0x00028020 File Offset: 0x00026220
		public void SortByCurrentState()
		{
			if (this.IsNameSelected)
			{
				this._listToControl.Sort(this._nameComparer);
				return;
			}
			if (this.IsTypeSelected)
			{
				this._listToControl.Sort(this._typeComparer);
				return;
			}
			if (this.IsInfluenceSelected)
			{
				this._listToControl.Sort(this._influenceComparer);
				return;
			}
			if (this.IsMembersSelected)
			{
				this._listToControl.Sort(this._membersComparer);
				return;
			}
			if (this.IsFiefsSelected)
			{
				this._listToControl.Sort(this._fiefsComparer);
			}
		}

		// Token: 0x060009E5 RID: 2533 RVA: 0x000280B0 File Offset: 0x000262B0
		private void ExecuteSortByName()
		{
			int nameState = this.NameState;
			this.SetAllStates(KingdomClanSortControllerVM.SortState.Default);
			this.NameState = (nameState + 1) % 3;
			if (this.NameState == 0)
			{
				this.NameState++;
			}
			this._nameComparer.SetSortMode(this.NameState == 1);
			this._listToControl.Sort(this._nameComparer);
			this.IsNameSelected = true;
		}

		// Token: 0x060009E6 RID: 2534 RVA: 0x00028118 File Offset: 0x00026318
		private void ExecuteSortByType()
		{
			int typeState = this.TypeState;
			this.SetAllStates(KingdomClanSortControllerVM.SortState.Default);
			this.TypeState = (typeState + 1) % 3;
			if (this.TypeState == 0)
			{
				this.TypeState++;
			}
			this._typeComparer.SetSortMode(this.TypeState == 1);
			this._listToControl.Sort(this._typeComparer);
			this.IsTypeSelected = true;
		}

		// Token: 0x060009E7 RID: 2535 RVA: 0x00028180 File Offset: 0x00026380
		private void ExecuteSortByInfluence()
		{
			int influenceState = this.InfluenceState;
			this.SetAllStates(KingdomClanSortControllerVM.SortState.Default);
			this.InfluenceState = (influenceState + 1) % 3;
			if (this.InfluenceState == 0)
			{
				this.InfluenceState++;
			}
			this._influenceComparer.SetSortMode(this.InfluenceState == 1);
			this._listToControl.Sort(this._influenceComparer);
			this.IsInfluenceSelected = true;
		}

		// Token: 0x060009E8 RID: 2536 RVA: 0x000281E8 File Offset: 0x000263E8
		private void ExecuteSortByMembers()
		{
			int membersState = this.MembersState;
			this.SetAllStates(KingdomClanSortControllerVM.SortState.Default);
			this.MembersState = (membersState + 1) % 3;
			if (this.MembersState == 0)
			{
				this.MembersState++;
			}
			this._membersComparer.SetSortMode(this.MembersState == 1);
			this._listToControl.Sort(this._membersComparer);
			this.IsMembersSelected = true;
		}

		// Token: 0x060009E9 RID: 2537 RVA: 0x00028250 File Offset: 0x00026450
		private void ExecuteSortByFiefs()
		{
			int fiefsState = this.FiefsState;
			this.SetAllStates(KingdomClanSortControllerVM.SortState.Default);
			this.FiefsState = (fiefsState + 1) % 3;
			if (this.FiefsState == 0)
			{
				this.FiefsState++;
			}
			this._fiefsComparer.SetSortMode(this.FiefsState == 1);
			this._listToControl.Sort(this._fiefsComparer);
			this.IsFiefsSelected = true;
		}

		// Token: 0x060009EA RID: 2538 RVA: 0x000282B8 File Offset: 0x000264B8
		private void SetAllStates(KingdomClanSortControllerVM.SortState state)
		{
			this.InfluenceState = (int)state;
			this.FiefsState = (int)state;
			this.MembersState = (int)state;
			this.NameState = (int)state;
			this.TypeState = (int)state;
			this.IsInfluenceSelected = false;
			this.IsFiefsSelected = false;
			this.IsNameSelected = false;
			this.IsMembersSelected = false;
			this.IsTypeSelected = false;
		}

		// Token: 0x1700032F RID: 815
		// (get) Token: 0x060009EB RID: 2539 RVA: 0x0002830B File Offset: 0x0002650B
		// (set) Token: 0x060009EC RID: 2540 RVA: 0x00028313 File Offset: 0x00026513
		[DataSourceProperty]
		public int InfluenceState
		{
			get
			{
				return this._influenceState;
			}
			set
			{
				if (value != this._influenceState)
				{
					this._influenceState = value;
					base.OnPropertyChangedWithValue(value, "InfluenceState");
				}
			}
		}

		// Token: 0x17000330 RID: 816
		// (get) Token: 0x060009ED RID: 2541 RVA: 0x00028331 File Offset: 0x00026531
		// (set) Token: 0x060009EE RID: 2542 RVA: 0x00028339 File Offset: 0x00026539
		[DataSourceProperty]
		public int FiefsState
		{
			get
			{
				return this._fiefsState;
			}
			set
			{
				if (value != this._fiefsState)
				{
					this._fiefsState = value;
					base.OnPropertyChangedWithValue(value, "FiefsState");
				}
			}
		}

		// Token: 0x17000331 RID: 817
		// (get) Token: 0x060009EF RID: 2543 RVA: 0x00028357 File Offset: 0x00026557
		// (set) Token: 0x060009F0 RID: 2544 RVA: 0x0002835F File Offset: 0x0002655F
		[DataSourceProperty]
		public int MembersState
		{
			get
			{
				return this._membersState;
			}
			set
			{
				if (value != this._membersState)
				{
					this._membersState = value;
					base.OnPropertyChangedWithValue(value, "MembersState");
				}
			}
		}

		// Token: 0x17000332 RID: 818
		// (get) Token: 0x060009F1 RID: 2545 RVA: 0x0002837D File Offset: 0x0002657D
		// (set) Token: 0x060009F2 RID: 2546 RVA: 0x00028385 File Offset: 0x00026585
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

		// Token: 0x17000333 RID: 819
		// (get) Token: 0x060009F3 RID: 2547 RVA: 0x000283A3 File Offset: 0x000265A3
		// (set) Token: 0x060009F4 RID: 2548 RVA: 0x000283AB File Offset: 0x000265AB
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

		// Token: 0x17000334 RID: 820
		// (get) Token: 0x060009F5 RID: 2549 RVA: 0x000283C9 File Offset: 0x000265C9
		// (set) Token: 0x060009F6 RID: 2550 RVA: 0x000283D1 File Offset: 0x000265D1
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

		// Token: 0x17000335 RID: 821
		// (get) Token: 0x060009F7 RID: 2551 RVA: 0x000283EF File Offset: 0x000265EF
		// (set) Token: 0x060009F8 RID: 2552 RVA: 0x000283F7 File Offset: 0x000265F7
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

		// Token: 0x17000336 RID: 822
		// (get) Token: 0x060009F9 RID: 2553 RVA: 0x00028415 File Offset: 0x00026615
		// (set) Token: 0x060009FA RID: 2554 RVA: 0x0002841D File Offset: 0x0002661D
		[DataSourceProperty]
		public bool IsFiefsSelected
		{
			get
			{
				return this._isFiefsSelected;
			}
			set
			{
				if (value != this._isFiefsSelected)
				{
					this._isFiefsSelected = value;
					base.OnPropertyChangedWithValue(value, "IsFiefsSelected");
				}
			}
		}

		// Token: 0x17000337 RID: 823
		// (get) Token: 0x060009FB RID: 2555 RVA: 0x0002843B File Offset: 0x0002663B
		// (set) Token: 0x060009FC RID: 2556 RVA: 0x00028443 File Offset: 0x00026643
		[DataSourceProperty]
		public bool IsMembersSelected
		{
			get
			{
				return this._isMembersSelected;
			}
			set
			{
				if (value != this._isMembersSelected)
				{
					this._isMembersSelected = value;
					base.OnPropertyChangedWithValue(value, "IsMembersSelected");
				}
			}
		}

		// Token: 0x17000338 RID: 824
		// (get) Token: 0x060009FD RID: 2557 RVA: 0x00028461 File Offset: 0x00026661
		// (set) Token: 0x060009FE RID: 2558 RVA: 0x00028469 File Offset: 0x00026669
		[DataSourceProperty]
		public bool IsInfluenceSelected
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
					base.OnPropertyChangedWithValue(value, "IsInfluenceSelected");
				}
			}
		}

		// Token: 0x04000474 RID: 1140
		private readonly MBBindingList<KingdomClanItemVM> _listToControl;

		// Token: 0x04000475 RID: 1141
		private readonly KingdomClanSortControllerVM.ItemNameComparer _nameComparer;

		// Token: 0x04000476 RID: 1142
		private readonly KingdomClanSortControllerVM.ItemTypeComparer _typeComparer;

		// Token: 0x04000477 RID: 1143
		private readonly KingdomClanSortControllerVM.ItemInfluenceComparer _influenceComparer;

		// Token: 0x04000478 RID: 1144
		private readonly KingdomClanSortControllerVM.ItemMembersComparer _membersComparer;

		// Token: 0x04000479 RID: 1145
		private readonly KingdomClanSortControllerVM.ItemFiefsComparer _fiefsComparer;

		// Token: 0x0400047A RID: 1146
		private int _influenceState;

		// Token: 0x0400047B RID: 1147
		private int _fiefsState;

		// Token: 0x0400047C RID: 1148
		private int _membersState;

		// Token: 0x0400047D RID: 1149
		private int _nameState;

		// Token: 0x0400047E RID: 1150
		private int _typeState;

		// Token: 0x0400047F RID: 1151
		private bool _isNameSelected;

		// Token: 0x04000480 RID: 1152
		private bool _isTypeSelected;

		// Token: 0x04000481 RID: 1153
		private bool _isFiefsSelected;

		// Token: 0x04000482 RID: 1154
		private bool _isMembersSelected;

		// Token: 0x04000483 RID: 1155
		private bool _isDistanceSelected;

		// Token: 0x020001A6 RID: 422
		private enum SortState
		{
			// Token: 0x04000F66 RID: 3942
			Default,
			// Token: 0x04000F67 RID: 3943
			Ascending,
			// Token: 0x04000F68 RID: 3944
			Descending
		}

		// Token: 0x020001A7 RID: 423
		public abstract class ItemComparerBase : IComparer<KingdomClanItemVM>
		{
			// Token: 0x06001FB3 RID: 8115 RVA: 0x0006EC84 File Offset: 0x0006CE84
			public void SetSortMode(bool isAscending)
			{
				this._isAscending = isAscending;
			}

			// Token: 0x06001FB4 RID: 8116
			public abstract int Compare(KingdomClanItemVM x, KingdomClanItemVM y);

			// Token: 0x06001FB5 RID: 8117 RVA: 0x0006EC8D File Offset: 0x0006CE8D
			protected int ResolveEquality(KingdomClanItemVM x, KingdomClanItemVM y)
			{
				return x.Clan.Name.ToString().CompareTo(y.Clan.Name.ToString());
			}

			// Token: 0x04000F69 RID: 3945
			protected bool _isAscending;
		}

		// Token: 0x020001A8 RID: 424
		public class ItemNameComparer : KingdomClanSortControllerVM.ItemComparerBase
		{
			// Token: 0x06001FB7 RID: 8119 RVA: 0x0006ECBC File Offset: 0x0006CEBC
			public override int Compare(KingdomClanItemVM x, KingdomClanItemVM y)
			{
				if (this._isAscending)
				{
					return y.Clan.Name.ToString().CompareTo(x.Clan.Name.ToString()) * -1;
				}
				return y.Clan.Name.ToString().CompareTo(x.Clan.Name.ToString());
			}
		}

		// Token: 0x020001A9 RID: 425
		public class ItemTypeComparer : KingdomClanSortControllerVM.ItemComparerBase
		{
			// Token: 0x06001FB9 RID: 8121 RVA: 0x0006ED28 File Offset: 0x0006CF28
			public override int Compare(KingdomClanItemVM x, KingdomClanItemVM y)
			{
				int num = y.ClanType.CompareTo(x.ClanType);
				if (num != 0)
				{
					return num * (this._isAscending ? (-1) : 1);
				}
				return base.ResolveEquality(x, y);
			}
		}

		// Token: 0x020001AA RID: 426
		public class ItemInfluenceComparer : KingdomClanSortControllerVM.ItemComparerBase
		{
			// Token: 0x06001FBB RID: 8123 RVA: 0x0006ED6C File Offset: 0x0006CF6C
			public override int Compare(KingdomClanItemVM x, KingdomClanItemVM y)
			{
				int num = y.Influence.CompareTo(x.Influence);
				if (num != 0)
				{
					return num * (this._isAscending ? (-1) : 1);
				}
				return base.ResolveEquality(x, y);
			}
		}

		// Token: 0x020001AB RID: 427
		public class ItemMembersComparer : KingdomClanSortControllerVM.ItemComparerBase
		{
			// Token: 0x06001FBD RID: 8125 RVA: 0x0006EDB0 File Offset: 0x0006CFB0
			public override int Compare(KingdomClanItemVM x, KingdomClanItemVM y)
			{
				int num = y.Members.Count.CompareTo(x.Members.Count);
				if (num != 0)
				{
					return num * (this._isAscending ? (-1) : 1);
				}
				return base.ResolveEquality(x, y);
			}
		}

		// Token: 0x020001AC RID: 428
		public class ItemFiefsComparer : KingdomClanSortControllerVM.ItemComparerBase
		{
			// Token: 0x06001FBF RID: 8127 RVA: 0x0006EE00 File Offset: 0x0006D000
			public override int Compare(KingdomClanItemVM x, KingdomClanItemVM y)
			{
				int num = y.Fiefs.Count.CompareTo(x.Fiefs.Count);
				if (num != 0)
				{
					return num * (this._isAscending ? (-1) : 1);
				}
				return base.ResolveEquality(x, y);
			}
		}
	}
}
