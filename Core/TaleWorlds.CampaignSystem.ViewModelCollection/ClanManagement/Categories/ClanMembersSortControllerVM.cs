using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.ClanManagement.Categories
{
	public class ClanMembersSortControllerVM : ViewModel
	{
		public ClanMembersSortControllerVM(MBBindingList<MBBindingList<ClanLordItemVM>> listsToControl)
		{
			this._listsToControl = listsToControl;
			this._nameComparer = new ClanMembersSortControllerVM.ItemNameComparer();
			this._locationComparer = new ClanMembersSortControllerVM.ItemLocationComparer();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.NameText = GameTexts.FindText("str_sort_by_name_label", null).ToString();
			this.LocationText = GameTexts.FindText("str_tooltip_label_location", null).ToString();
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
			foreach (MBBindingList<ClanLordItemVM> mbbindingList in this._listsToControl)
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
			foreach (MBBindingList<ClanLordItemVM> mbbindingList in this._listsToControl)
			{
				mbbindingList.Sort(this._locationComparer);
			}
			this.IsLocationSelected = true;
		}

		private void SetAllStates(CampaignUIHelper.SortState state)
		{
			this.NameState = (int)state;
			this.LocationState = (int)state;
			this.IsNameSelected = false;
			this.IsLocationSelected = false;
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

		private readonly MBBindingList<MBBindingList<ClanLordItemVM>> _listsToControl;

		private readonly ClanMembersSortControllerVM.ItemNameComparer _nameComparer;

		private readonly ClanMembersSortControllerVM.ItemLocationComparer _locationComparer;

		private int _nameState;

		private int _locationState;

		private bool _isNameSelected;

		private bool _isLocationSelected;

		private string _nameText;

		private string _locationText;

		public abstract class ItemComparerBase : IComparer<ClanLordItemVM>
		{
			public void SetSortMode(bool isAcending)
			{
				this._isAcending = isAcending;
			}

			public abstract int Compare(ClanLordItemVM x, ClanLordItemVM y);

			protected bool _isAcending;
		}

		public class ItemNameComparer : ClanMembersSortControllerVM.ItemComparerBase
		{
			public override int Compare(ClanLordItemVM x, ClanLordItemVM y)
			{
				if (this._isAcending)
				{
					return y.Name.CompareTo(x.Name) * -1;
				}
				return y.Name.CompareTo(x.Name);
			}
		}

		public class ItemLocationComparer : ClanMembersSortControllerVM.ItemComparerBase
		{
			public override int Compare(ClanLordItemVM x, ClanLordItemVM y)
			{
				if (this._isAcending)
				{
					return y.GetHero().GetTrackDistanceToMainAgent().CompareTo(x.GetHero().GetTrackDistanceToMainAgent()) * -1;
				}
				return y.GetHero().GetTrackDistanceToMainAgent().CompareTo(x.GetHero().GetTrackDistanceToMainAgent());
			}
		}
	}
}
