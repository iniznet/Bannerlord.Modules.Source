using System;
using System.Collections.Generic;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.GameMenu.TournamentLeaderboard
{
	// Token: 0x02000098 RID: 152
	public class TournamentLeaderboardSortControllerVM : ViewModel
	{
		// Token: 0x06000EC1 RID: 3777 RVA: 0x0003A43C File Offset: 0x0003863C
		public TournamentLeaderboardSortControllerVM(ref MBBindingList<TournamentLeaderboardEntryItemVM> listToControl)
		{
			this._listToControl = listToControl;
			this._prizeComparer = new TournamentLeaderboardSortControllerVM.ItemPrizeComparer();
			this._nameComparer = new TournamentLeaderboardSortControllerVM.ItemNameComparer();
			this._placementComparer = new TournamentLeaderboardSortControllerVM.ItemPlacementComparer();
			this._victoriesComparer = new TournamentLeaderboardSortControllerVM.ItemVictoriesComparer();
		}

		// Token: 0x06000EC2 RID: 3778 RVA: 0x0003A478 File Offset: 0x00038678
		public void ExecuteSortByName()
		{
			int nameState = this.NameState;
			this.SetAllStates(TournamentLeaderboardSortControllerVM.SortState.Default);
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

		// Token: 0x06000EC3 RID: 3779 RVA: 0x0003A4E4 File Offset: 0x000386E4
		public void ExecuteSortByPrize()
		{
			int prizeState = this.PrizeState;
			this.SetAllStates(TournamentLeaderboardSortControllerVM.SortState.Default);
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

		// Token: 0x06000EC4 RID: 3780 RVA: 0x0003A550 File Offset: 0x00038750
		public void ExecuteSortByPlacement()
		{
			int placementState = this.PlacementState;
			this.SetAllStates(TournamentLeaderboardSortControllerVM.SortState.Default);
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

		// Token: 0x06000EC5 RID: 3781 RVA: 0x0003A5BC File Offset: 0x000387BC
		public void ExecuteSortByVictories()
		{
			int victoriesState = this.VictoriesState;
			this.SetAllStates(TournamentLeaderboardSortControllerVM.SortState.Default);
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

		// Token: 0x06000EC6 RID: 3782 RVA: 0x0003A626 File Offset: 0x00038826
		private void SetAllStates(TournamentLeaderboardSortControllerVM.SortState state)
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

		// Token: 0x170004D5 RID: 1237
		// (get) Token: 0x06000EC7 RID: 3783 RVA: 0x0003A660 File Offset: 0x00038860
		// (set) Token: 0x06000EC8 RID: 3784 RVA: 0x0003A668 File Offset: 0x00038868
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

		// Token: 0x170004D6 RID: 1238
		// (get) Token: 0x06000EC9 RID: 3785 RVA: 0x0003A686 File Offset: 0x00038886
		// (set) Token: 0x06000ECA RID: 3786 RVA: 0x0003A68E File Offset: 0x0003888E
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

		// Token: 0x170004D7 RID: 1239
		// (get) Token: 0x06000ECB RID: 3787 RVA: 0x0003A6AC File Offset: 0x000388AC
		// (set) Token: 0x06000ECC RID: 3788 RVA: 0x0003A6B4 File Offset: 0x000388B4
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

		// Token: 0x170004D8 RID: 1240
		// (get) Token: 0x06000ECD RID: 3789 RVA: 0x0003A6D2 File Offset: 0x000388D2
		// (set) Token: 0x06000ECE RID: 3790 RVA: 0x0003A6DA File Offset: 0x000388DA
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

		// Token: 0x170004D9 RID: 1241
		// (get) Token: 0x06000ECF RID: 3791 RVA: 0x0003A6F8 File Offset: 0x000388F8
		// (set) Token: 0x06000ED0 RID: 3792 RVA: 0x0003A700 File Offset: 0x00038900
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

		// Token: 0x170004DA RID: 1242
		// (get) Token: 0x06000ED1 RID: 3793 RVA: 0x0003A71E File Offset: 0x0003891E
		// (set) Token: 0x06000ED2 RID: 3794 RVA: 0x0003A726 File Offset: 0x00038926
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

		// Token: 0x170004DB RID: 1243
		// (get) Token: 0x06000ED3 RID: 3795 RVA: 0x0003A744 File Offset: 0x00038944
		// (set) Token: 0x06000ED4 RID: 3796 RVA: 0x0003A74C File Offset: 0x0003894C
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

		// Token: 0x170004DC RID: 1244
		// (get) Token: 0x06000ED5 RID: 3797 RVA: 0x0003A76A File Offset: 0x0003896A
		// (set) Token: 0x06000ED6 RID: 3798 RVA: 0x0003A772 File Offset: 0x00038972
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

		// Token: 0x040006D8 RID: 1752
		private readonly MBBindingList<TournamentLeaderboardEntryItemVM> _listToControl;

		// Token: 0x040006D9 RID: 1753
		private readonly TournamentLeaderboardSortControllerVM.ItemNameComparer _nameComparer;

		// Token: 0x040006DA RID: 1754
		private readonly TournamentLeaderboardSortControllerVM.ItemPrizeComparer _prizeComparer;

		// Token: 0x040006DB RID: 1755
		private readonly TournamentLeaderboardSortControllerVM.ItemPlacementComparer _placementComparer;

		// Token: 0x040006DC RID: 1756
		private readonly TournamentLeaderboardSortControllerVM.ItemVictoriesComparer _victoriesComparer;

		// Token: 0x040006DD RID: 1757
		private int _nameState;

		// Token: 0x040006DE RID: 1758
		private int _prizeState;

		// Token: 0x040006DF RID: 1759
		private int _placementState;

		// Token: 0x040006E0 RID: 1760
		private int _victoriesState;

		// Token: 0x040006E1 RID: 1761
		private bool _isNameSelected;

		// Token: 0x040006E2 RID: 1762
		private bool _isPrizeSelected;

		// Token: 0x040006E3 RID: 1763
		private bool _isPlacementSelected;

		// Token: 0x040006E4 RID: 1764
		private bool _isVictoriesSelected;

		// Token: 0x020001D3 RID: 467
		private enum SortState
		{
			// Token: 0x04000FE4 RID: 4068
			Default,
			// Token: 0x04000FE5 RID: 4069
			Ascending,
			// Token: 0x04000FE6 RID: 4070
			Descending
		}

		// Token: 0x020001D4 RID: 468
		public abstract class ItemComparerBase : IComparer<TournamentLeaderboardEntryItemVM>
		{
			// Token: 0x0600202C RID: 8236 RVA: 0x0006F644 File Offset: 0x0006D844
			public void SetSortMode(bool isAcending)
			{
				this._isAcending = isAcending;
			}

			// Token: 0x0600202D RID: 8237
			public abstract int Compare(TournamentLeaderboardEntryItemVM x, TournamentLeaderboardEntryItemVM y);

			// Token: 0x04000FE7 RID: 4071
			protected bool _isAcending;
		}

		// Token: 0x020001D5 RID: 469
		public class ItemNameComparer : TournamentLeaderboardSortControllerVM.ItemComparerBase
		{
			// Token: 0x0600202F RID: 8239 RVA: 0x0006F655 File Offset: 0x0006D855
			public override int Compare(TournamentLeaderboardEntryItemVM x, TournamentLeaderboardEntryItemVM y)
			{
				if (this._isAcending)
				{
					return y.Name.CompareTo(x.Name) * -1;
				}
				return y.Name.CompareTo(x.Name);
			}
		}

		// Token: 0x020001D6 RID: 470
		public class ItemPrizeComparer : TournamentLeaderboardSortControllerVM.ItemComparerBase
		{
			// Token: 0x06002031 RID: 8241 RVA: 0x0006F68C File Offset: 0x0006D88C
			public override int Compare(TournamentLeaderboardEntryItemVM x, TournamentLeaderboardEntryItemVM y)
			{
				if (this._isAcending)
				{
					return y.PrizeValue.CompareTo(x.PrizeValue) * -1;
				}
				return y.PrizeValue.CompareTo(x.PrizeValue);
			}
		}

		// Token: 0x020001D7 RID: 471
		public class ItemPlacementComparer : TournamentLeaderboardSortControllerVM.ItemComparerBase
		{
			// Token: 0x06002033 RID: 8243 RVA: 0x0006F6D4 File Offset: 0x0006D8D4
			public override int Compare(TournamentLeaderboardEntryItemVM x, TournamentLeaderboardEntryItemVM y)
			{
				if (this._isAcending)
				{
					return y.PlacementOnLeaderboard.CompareTo(x.PlacementOnLeaderboard) * -1;
				}
				return y.PlacementOnLeaderboard.CompareTo(x.PlacementOnLeaderboard);
			}
		}

		// Token: 0x020001D8 RID: 472
		public class ItemVictoriesComparer : TournamentLeaderboardSortControllerVM.ItemComparerBase
		{
			// Token: 0x06002035 RID: 8245 RVA: 0x0006F71C File Offset: 0x0006D91C
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
