using System;
using System.Collections.Generic;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.KingdomManagement.Clans
{
	public class KingdomClanSortControllerVM : ViewModel
	{
		public KingdomClanSortControllerVM(ref MBBindingList<KingdomClanItemVM> listToControl)
		{
			this._listToControl = listToControl;
			this._influenceComparer = new KingdomClanSortControllerVM.ItemInfluenceComparer();
			this._membersComparer = new KingdomClanSortControllerVM.ItemMembersComparer();
			this._nameComparer = new KingdomClanSortControllerVM.ItemNameComparer();
			this._fiefsComparer = new KingdomClanSortControllerVM.ItemFiefsComparer();
			this._typeComparer = new KingdomClanSortControllerVM.ItemTypeComparer();
		}

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

		private readonly MBBindingList<KingdomClanItemVM> _listToControl;

		private readonly KingdomClanSortControllerVM.ItemNameComparer _nameComparer;

		private readonly KingdomClanSortControllerVM.ItemTypeComparer _typeComparer;

		private readonly KingdomClanSortControllerVM.ItemInfluenceComparer _influenceComparer;

		private readonly KingdomClanSortControllerVM.ItemMembersComparer _membersComparer;

		private readonly KingdomClanSortControllerVM.ItemFiefsComparer _fiefsComparer;

		private int _influenceState;

		private int _fiefsState;

		private int _membersState;

		private int _nameState;

		private int _typeState;

		private bool _isNameSelected;

		private bool _isTypeSelected;

		private bool _isFiefsSelected;

		private bool _isMembersSelected;

		private bool _isDistanceSelected;

		private enum SortState
		{
			Default,
			Ascending,
			Descending
		}

		public abstract class ItemComparerBase : IComparer<KingdomClanItemVM>
		{
			public void SetSortMode(bool isAscending)
			{
				this._isAscending = isAscending;
			}

			public abstract int Compare(KingdomClanItemVM x, KingdomClanItemVM y);

			protected int ResolveEquality(KingdomClanItemVM x, KingdomClanItemVM y)
			{
				return x.Clan.Name.ToString().CompareTo(y.Clan.Name.ToString());
			}

			protected bool _isAscending;
		}

		public class ItemNameComparer : KingdomClanSortControllerVM.ItemComparerBase
		{
			public override int Compare(KingdomClanItemVM x, KingdomClanItemVM y)
			{
				if (this._isAscending)
				{
					return y.Clan.Name.ToString().CompareTo(x.Clan.Name.ToString()) * -1;
				}
				return y.Clan.Name.ToString().CompareTo(x.Clan.Name.ToString());
			}
		}

		public class ItemTypeComparer : KingdomClanSortControllerVM.ItemComparerBase
		{
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

		public class ItemInfluenceComparer : KingdomClanSortControllerVM.ItemComparerBase
		{
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

		public class ItemMembersComparer : KingdomClanSortControllerVM.ItemComparerBase
		{
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

		public class ItemFiefsComparer : KingdomClanSortControllerVM.ItemComparerBase
		{
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
