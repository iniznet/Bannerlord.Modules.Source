using System;
using System.Collections.Generic;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.KingdomManagement.Armies
{
	public class KingdomArmySortControllerVM : ViewModel
	{
		public KingdomArmySortControllerVM(ref MBBindingList<KingdomArmyItemVM> listToControl)
		{
			this._listToControl = listToControl;
			this._ownerComparer = new KingdomArmySortControllerVM.ItemOwnerComparer();
			this._strengthComparer = new KingdomArmySortControllerVM.ItemStrengthComparer();
			this._nameComparer = new KingdomArmySortControllerVM.ItemNameComparer();
			this._partiesComparer = new KingdomArmySortControllerVM.ItemPartiesComparer();
			this._distanceComparer = new KingdomArmySortControllerVM.ItemDistanceComparer();
		}

		private void ExecuteSortByName()
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

		private void ExecuteSortByOwner()
		{
			int ownerState = this.OwnerState;
			this.SetAllStates(CampaignUIHelper.SortState.Default);
			this.OwnerState = (ownerState + 1) % 3;
			if (this.OwnerState == 0)
			{
				this.OwnerState++;
			}
			this._ownerComparer.SetSortMode(this.OwnerState == 1);
			this._listToControl.Sort(this._ownerComparer);
			this.IsOwnerSelected = true;
		}

		private void ExecuteSortByStrength()
		{
			int strengthState = this.StrengthState;
			this.SetAllStates(CampaignUIHelper.SortState.Default);
			this.StrengthState = (strengthState + 1) % 3;
			if (this.StrengthState == 0)
			{
				this.StrengthState++;
			}
			this._strengthComparer.SetSortMode(this.StrengthState == 1);
			this._listToControl.Sort(this._strengthComparer);
			this.IsStrengthSelected = true;
		}

		private void ExecuteSortByParties()
		{
			int partiesState = this.PartiesState;
			this.SetAllStates(CampaignUIHelper.SortState.Default);
			this.PartiesState = (partiesState + 1) % 3;
			if (this.PartiesState == 0)
			{
				this.PartiesState++;
			}
			this._partiesComparer.SetSortMode(this.PartiesState == 1);
			this._listToControl.Sort(this._partiesComparer);
			this.IsPartiesSelected = true;
		}

		private void ExecuteSortByDistance()
		{
			int distanceState = this.DistanceState;
			this.SetAllStates(CampaignUIHelper.SortState.Default);
			this.DistanceState = (distanceState + 1) % 3;
			if (this.DistanceState == 0)
			{
				this.DistanceState++;
			}
			this._distanceComparer.SetSortMode(this.DistanceState == 1);
			this._listToControl.Sort(this._distanceComparer);
			this.IsDistanceSelected = true;
		}

		private void SetAllStates(CampaignUIHelper.SortState state)
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

		private readonly MBBindingList<KingdomArmyItemVM> _listToControl;

		private readonly KingdomArmySortControllerVM.ItemNameComparer _nameComparer;

		private readonly KingdomArmySortControllerVM.ItemOwnerComparer _ownerComparer;

		private readonly KingdomArmySortControllerVM.ItemStrengthComparer _strengthComparer;

		private readonly KingdomArmySortControllerVM.ItemPartiesComparer _partiesComparer;

		private readonly KingdomArmySortControllerVM.ItemDistanceComparer _distanceComparer;

		private int _nameState;

		private int _ownerState;

		private int _strengthState;

		private int _partiesState;

		private int _distanceState;

		private bool _isNameSelected;

		private bool _isOwnerSelected;

		private bool _isStrengthSelected;

		private bool _isPartiesSelected;

		private bool _isDistanceSelected;

		public abstract class ItemComparerBase : IComparer<KingdomArmyItemVM>
		{
			public void SetSortMode(bool isAscending)
			{
				this._isAscending = isAscending;
			}

			public abstract int Compare(KingdomArmyItemVM x, KingdomArmyItemVM y);

			protected int ResolveEquality(KingdomArmyItemVM x, KingdomArmyItemVM y)
			{
				return x.ArmyName.CompareTo(y.ArmyName);
			}

			protected bool _isAscending;
		}

		public class ItemNameComparer : KingdomArmySortControllerVM.ItemComparerBase
		{
			public override int Compare(KingdomArmyItemVM x, KingdomArmyItemVM y)
			{
				if (this._isAscending)
				{
					return y.ArmyName.CompareTo(x.ArmyName) * -1;
				}
				return y.ArmyName.CompareTo(x.ArmyName);
			}
		}

		public class ItemOwnerComparer : KingdomArmySortControllerVM.ItemComparerBase
		{
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

		public class ItemStrengthComparer : KingdomArmySortControllerVM.ItemComparerBase
		{
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

		public class ItemPartiesComparer : KingdomArmySortControllerVM.ItemComparerBase
		{
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

		public class ItemDistanceComparer : KingdomArmySortControllerVM.ItemComparerBase
		{
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
