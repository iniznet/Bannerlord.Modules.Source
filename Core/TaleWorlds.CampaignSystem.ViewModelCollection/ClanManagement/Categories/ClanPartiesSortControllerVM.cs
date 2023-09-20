using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.ClanManagement.Categories
{
	public class ClanPartiesSortControllerVM : ViewModel
	{
		public ClanPartiesSortControllerVM(MBBindingList<MBBindingList<ClanPartyItemVM>> listsToControl)
		{
			this._listsToControl = listsToControl;
			this._nameComparer = new ClanPartiesSortControllerVM.ItemNameComparer();
			this._locationComparer = new ClanPartiesSortControllerVM.ItemLocationComparer();
			this._sizeComparer = new ClanPartiesSortControllerVM.ItemSizeComparer();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.NameText = GameTexts.FindText("str_sort_by_name_label", null).ToString();
			this.LocationText = GameTexts.FindText("str_tooltip_label_location", null).ToString();
			this.SizeText = GameTexts.FindText("str_clan_party_size", null).ToString();
		}

		public void ExecuteSortByName()
		{
			int nameState = this.NameState;
			this.SetAllStates(CampaignUIHelper.SortState.Default);
			this.NameState = (nameState + 1) % 3;
			if (this.NameState == 0)
			{
				int nameState2 = this.NameState;
				this.NameState = nameState2 + 1;
			}
			this._nameComparer.SetSortMode(this.NameState == 1);
			foreach (MBBindingList<ClanPartyItemVM> mbbindingList in this._listsToControl)
			{
				mbbindingList.Sort(this._nameComparer);
			}
			this.IsNameSelected = true;
		}

		public void ExecuteSortByLocation()
		{
			int locationState = this.LocationState;
			this.SetAllStates(CampaignUIHelper.SortState.Default);
			this.LocationState = (locationState + 1) % 3;
			if (this.LocationState == 0)
			{
				int locationState2 = this.LocationState;
				this.LocationState = locationState2 + 1;
			}
			this._locationComparer.SetSortMode(this.LocationState == 1);
			foreach (MBBindingList<ClanPartyItemVM> mbbindingList in this._listsToControl)
			{
				mbbindingList.Sort(this._locationComparer);
			}
			this.IsLocationSelected = true;
		}

		public void ExecuteSortBySize()
		{
			int sizeState = this.SizeState;
			this.SetAllStates(CampaignUIHelper.SortState.Default);
			this.SizeState = (sizeState + 1) % 3;
			if (this.SizeState == 0)
			{
				int sizeState2 = this.SizeState;
				this.SizeState = sizeState2 + 1;
			}
			this._sizeComparer.SetSortMode(this.SizeState == 1);
			foreach (MBBindingList<ClanPartyItemVM> mbbindingList in this._listsToControl)
			{
				mbbindingList.Sort(this._sizeComparer);
			}
			this.IsSizeSelected = true;
		}

		private void SetAllStates(CampaignUIHelper.SortState state)
		{
			this.NameState = (int)state;
			this.LocationState = (int)state;
			this.SizeState = (int)state;
			this.IsNameSelected = false;
			this.IsLocationSelected = false;
			this.IsSizeSelected = false;
		}

		public void ResetAllStates()
		{
			this.SetAllStates(CampaignUIHelper.SortState.Default);
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
		public int LocationState
		{
			get
			{
				return this._locationState;
			}
			set
			{
				if (value != this._locationState)
				{
					this._locationState = value;
					base.OnPropertyChangedWithValue(value, "LocationState");
				}
			}
		}

		[DataSourceProperty]
		public int SizeState
		{
			get
			{
				return this._sizeState;
			}
			set
			{
				if (value != this._sizeState)
				{
					this._sizeState = value;
					base.OnPropertyChangedWithValue(value, "SizeState");
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
		public bool IsLocationSelected
		{
			get
			{
				return this._isLocationSelected;
			}
			set
			{
				if (value != this._isLocationSelected)
				{
					this._isLocationSelected = value;
					base.OnPropertyChangedWithValue(value, "IsLocationSelected");
				}
			}
		}

		[DataSourceProperty]
		public bool IsSizeSelected
		{
			get
			{
				return this._isSizeSelected;
			}
			set
			{
				if (value != this._isSizeSelected)
				{
					this._isSizeSelected = value;
					base.OnPropertyChangedWithValue(value, "IsSizeSelected");
				}
			}
		}

		[DataSourceProperty]
		public string NameText
		{
			get
			{
				return this._nameText;
			}
			set
			{
				if (value != this._nameText)
				{
					this._nameText = value;
					base.OnPropertyChangedWithValue<string>(value, "NameText");
				}
			}
		}

		[DataSourceProperty]
		public string LocationText
		{
			get
			{
				return this._locationText;
			}
			set
			{
				if (value != this._locationText)
				{
					this._locationText = value;
					base.OnPropertyChangedWithValue<string>(value, "LocationText");
				}
			}
		}

		[DataSourceProperty]
		public string SizeText
		{
			get
			{
				return this._sizeText;
			}
			set
			{
				if (value != this._sizeText)
				{
					this._sizeText = value;
					base.OnPropertyChangedWithValue<string>(value, "SizeText");
				}
			}
		}

		private readonly MBBindingList<MBBindingList<ClanPartyItemVM>> _listsToControl;

		private readonly ClanPartiesSortControllerVM.ItemNameComparer _nameComparer;

		private readonly ClanPartiesSortControllerVM.ItemLocationComparer _locationComparer;

		private readonly ClanPartiesSortControllerVM.ItemSizeComparer _sizeComparer;

		private int _nameState;

		private int _locationState;

		private int _sizeState;

		private bool _isNameSelected;

		private bool _isLocationSelected;

		private bool _isSizeSelected;

		private string _nameText;

		private string _locationText;

		private string _sizeText;

		public abstract class ItemComparerBase : IComparer<ClanPartyItemVM>
		{
			public void SetSortMode(bool isAcending)
			{
				this._isAcending = isAcending;
			}

			public abstract int Compare(ClanPartyItemVM x, ClanPartyItemVM y);

			protected bool _isAcending;
		}

		public class ItemNameComparer : ClanPartiesSortControllerVM.ItemComparerBase
		{
			public override int Compare(ClanPartyItemVM x, ClanPartyItemVM y)
			{
				if (this._isAcending)
				{
					return y.Name.CompareTo(x.Name) * -1;
				}
				return y.Name.CompareTo(x.Name);
			}
		}

		public class ItemLocationComparer : ClanPartiesSortControllerVM.ItemComparerBase
		{
			public override int Compare(ClanPartyItemVM x, ClanPartyItemVM y)
			{
				if (this._isAcending)
				{
					return y.Party.MobileParty.GetTrackDistanceToMainAgent().CompareTo(x.Party.MobileParty.GetTrackDistanceToMainAgent()) * -1;
				}
				return y.Party.MobileParty.GetTrackDistanceToMainAgent().CompareTo(x.Party.MobileParty.GetTrackDistanceToMainAgent());
			}
		}

		public class ItemSizeComparer : ClanPartiesSortControllerVM.ItemComparerBase
		{
			public override int Compare(ClanPartyItemVM x, ClanPartyItemVM y)
			{
				if (this._isAcending)
				{
					return y.Party.MobileParty.MemberRoster.TotalManCount.CompareTo(x.Party.MobileParty.MemberRoster.TotalManCount) * -1;
				}
				return y.Party.MobileParty.MemberRoster.TotalManCount.CompareTo(x.Party.MobileParty.MemberRoster.TotalManCount);
			}
		}
	}
}
