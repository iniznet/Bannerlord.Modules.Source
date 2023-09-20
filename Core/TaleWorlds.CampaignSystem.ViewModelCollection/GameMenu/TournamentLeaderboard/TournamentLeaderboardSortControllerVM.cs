using System;
using System.Collections.Generic;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.GameMenu.TournamentLeaderboard
{
	public class TournamentLeaderboardSortControllerVM : ViewModel
	{
		public TournamentLeaderboardSortControllerVM(ref MBBindingList<TournamentLeaderboardEntryItemVM> listToControl)
		{
			this._listToControl = listToControl;
			this._prizeComparer = new TournamentLeaderboardSortControllerVM.ItemPrizeComparer();
			this._nameComparer = new TournamentLeaderboardSortControllerVM.ItemNameComparer();
			this._placementComparer = new TournamentLeaderboardSortControllerVM.ItemPlacementComparer();
			this._victoriesComparer = new TournamentLeaderboardSortControllerVM.ItemVictoriesComparer();
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
			this._listToControl.Sort(this._nameComparer);
			this.IsNameSelected = true;
		}

		public void ExecuteSortByPrize()
		{
			int prizeState = this.PrizeState;
			this.SetAllStates(CampaignUIHelper.SortState.Default);
			this.PrizeState = (prizeState + 1) % 3;
			if (this.PrizeState == 0)
			{
				int prizeState2 = this.PrizeState;
				this.PrizeState = prizeState2 + 1;
			}
			this._prizeComparer.SetSortMode(this.PrizeState == 1);
			this._listToControl.Sort(this._prizeComparer);
			this.IsPrizeSelected = true;
		}

		public void ExecuteSortByPlacement()
		{
			int placementState = this.PlacementState;
			this.SetAllStates(CampaignUIHelper.SortState.Default);
			this.PlacementState = (placementState + 1) % 3;
			if (this.PlacementState == 0)
			{
				int placementState2 = this.PlacementState;
				this.PlacementState = placementState2 + 1;
			}
			this._placementComparer.SetSortMode(this.PlacementState == 1);
			this._listToControl.Sort(this._placementComparer);
			this.IsPlacementSelected = true;
		}

		public void ExecuteSortByVictories()
		{
			int victoriesState = this.VictoriesState;
			this.SetAllStates(CampaignUIHelper.SortState.Default);
			this.VictoriesState = (victoriesState + 1) % 3;
			if (this.VictoriesState == 0)
			{
				int victoriesState2 = this.VictoriesState;
				this.VictoriesState = victoriesState2 + 1;
			}
			this._victoriesComparer.SetSortMode(this.VictoriesState == 1);
			this._listToControl.Sort(this._victoriesComparer);
			this.IsVictoriesSelected = true;
		}

		private void SetAllStates(CampaignUIHelper.SortState state)
		{
			this.NameState = (int)state;
			this.PrizeState = (int)state;
			this.PlacementState = (int)state;
			this.VictoriesState = (int)state;
			this.IsNameSelected = false;
			this.IsVictoriesSelected = false;
			this.IsPrizeSelected = false;
			this.IsPlacementSelected = false;
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
		public int VictoriesState
		{
			get
			{
				return this._victoriesState;
			}
			set
			{
				if (value != this._victoriesState)
				{
					this._victoriesState = value;
					base.OnPropertyChangedWithValue(value, "VictoriesState");
				}
			}
		}

		[DataSourceProperty]
		public int PrizeState
		{
			get
			{
				return this._prizeState;
			}
			set
			{
				if (value != this._prizeState)
				{
					this._prizeState = value;
					base.OnPropertyChangedWithValue(value, "PrizeState");
				}
			}
		}

		[DataSourceProperty]
		public int PlacementState
		{
			get
			{
				return this._placementState;
			}
			set
			{
				if (value != this._placementState)
				{
					this._placementState = value;
					base.OnPropertyChangedWithValue(value, "PlacementState");
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
		public bool IsPrizeSelected
		{
			get
			{
				return this._isPrizeSelected;
			}
			set
			{
				if (value != this._isPrizeSelected)
				{
					this._isPrizeSelected = value;
					base.OnPropertyChangedWithValue(value, "IsPrizeSelected");
				}
			}
		}

		[DataSourceProperty]
		public bool IsPlacementSelected
		{
			get
			{
				return this._isPlacementSelected;
			}
			set
			{
				if (value != this._isPlacementSelected)
				{
					this._isPlacementSelected = value;
					base.OnPropertyChangedWithValue(value, "IsPlacementSelected");
				}
			}
		}

		[DataSourceProperty]
		public bool IsVictoriesSelected
		{
			get
			{
				return this._isVictoriesSelected;
			}
			set
			{
				if (value != this._isVictoriesSelected)
				{
					this._isVictoriesSelected = value;
					base.OnPropertyChangedWithValue(value, "IsVictoriesSelected");
				}
			}
		}

		private readonly MBBindingList<TournamentLeaderboardEntryItemVM> _listToControl;

		private readonly TournamentLeaderboardSortControllerVM.ItemNameComparer _nameComparer;

		private readonly TournamentLeaderboardSortControllerVM.ItemPrizeComparer _prizeComparer;

		private readonly TournamentLeaderboardSortControllerVM.ItemPlacementComparer _placementComparer;

		private readonly TournamentLeaderboardSortControllerVM.ItemVictoriesComparer _victoriesComparer;

		private int _nameState;

		private int _prizeState;

		private int _placementState;

		private int _victoriesState;

		private bool _isNameSelected;

		private bool _isPrizeSelected;

		private bool _isPlacementSelected;

		private bool _isVictoriesSelected;

		public abstract class ItemComparerBase : IComparer<TournamentLeaderboardEntryItemVM>
		{
			public void SetSortMode(bool isAcending)
			{
				this._isAcending = isAcending;
			}

			public abstract int Compare(TournamentLeaderboardEntryItemVM x, TournamentLeaderboardEntryItemVM y);

			protected bool _isAcending;
		}

		public class ItemNameComparer : TournamentLeaderboardSortControllerVM.ItemComparerBase
		{
			public override int Compare(TournamentLeaderboardEntryItemVM x, TournamentLeaderboardEntryItemVM y)
			{
				if (this._isAcending)
				{
					return y.Name.CompareTo(x.Name) * -1;
				}
				return y.Name.CompareTo(x.Name);
			}
		}

		public class ItemPrizeComparer : TournamentLeaderboardSortControllerVM.ItemComparerBase
		{
			public override int Compare(TournamentLeaderboardEntryItemVM x, TournamentLeaderboardEntryItemVM y)
			{
				if (this._isAcending)
				{
					return y.PrizeValue.CompareTo(x.PrizeValue) * -1;
				}
				return y.PrizeValue.CompareTo(x.PrizeValue);
			}
		}

		public class ItemPlacementComparer : TournamentLeaderboardSortControllerVM.ItemComparerBase
		{
			public override int Compare(TournamentLeaderboardEntryItemVM x, TournamentLeaderboardEntryItemVM y)
			{
				if (this._isAcending)
				{
					return y.PlacementOnLeaderboard.CompareTo(x.PlacementOnLeaderboard) * -1;
				}
				return y.PlacementOnLeaderboard.CompareTo(x.PlacementOnLeaderboard);
			}
		}

		public class ItemVictoriesComparer : TournamentLeaderboardSortControllerVM.ItemComparerBase
		{
			public override int Compare(TournamentLeaderboardEntryItemVM x, TournamentLeaderboardEntryItemVM y)
			{
				if (this._isAcending)
				{
					return y.Victories.CompareTo(x.Victories) * -1;
				}
				return y.Victories.CompareTo(x.Victories);
			}
		}
	}
}
