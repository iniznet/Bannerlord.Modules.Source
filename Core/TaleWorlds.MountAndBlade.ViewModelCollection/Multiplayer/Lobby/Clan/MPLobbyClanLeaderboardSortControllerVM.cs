using System;
using System.Collections.Generic;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Diamond;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.Clan
{
	public class MPLobbyClanLeaderboardSortControllerVM : ViewModel
	{
		public MPLobbyClanLeaderboardSortControllerVM(ref ClanLeaderboardEntry[] listToControl, Action onSorted)
		{
			this._listToControl = listToControl;
			this._winComparer = new MPLobbyClanLeaderboardSortControllerVM.ItemWinComparer();
			this._lossComparer = new MPLobbyClanLeaderboardSortControllerVM.ItemLossComparer();
			this._nameComparer = new MPLobbyClanLeaderboardSortControllerVM.ItemNameComparer();
			this._onSorted = onSorted;
		}

		private void ExecuteSortByName()
		{
			int nameState = this.NameState;
			this.SetAllStates(MPLobbyClanLeaderboardSortControllerVM.SortState.Default);
			this.NameState = (nameState + 1) % 3;
			if (this.NameState == 0)
			{
				this.NameState++;
			}
			this._nameComparer.SetSortMode(this.NameState == 1);
			Array.Sort<ClanLeaderboardEntry>(this._listToControl, this._nameComparer);
			this.IsNameSelected = true;
			this._onSorted();
		}

		private void ExecuteSortByWin()
		{
			int winState = this.WinState;
			this.SetAllStates(MPLobbyClanLeaderboardSortControllerVM.SortState.Default);
			this.WinState = (winState + 1) % 3;
			if (this.WinState == 0)
			{
				this.WinState++;
			}
			this._winComparer.SetSortMode(this.WinState == 1);
			Array.Sort<ClanLeaderboardEntry>(this._listToControl, this._winComparer);
			this.IsWinSelected = true;
			this._onSorted();
		}

		private void ExecuteSortByLoss()
		{
			int lossState = this.LossState;
			this.SetAllStates(MPLobbyClanLeaderboardSortControllerVM.SortState.Default);
			this.LossState = (lossState + 1) % 3;
			if (this.LossState == 0)
			{
				this.LossState++;
			}
			this._lossComparer.SetSortMode(this.LossState == 1);
			Array.Sort<ClanLeaderboardEntry>(this._listToControl, this._lossComparer);
			this.IsLossSelected = true;
			this._onSorted();
		}

		private void SetAllStates(MPLobbyClanLeaderboardSortControllerVM.SortState state)
		{
			this.NameState = (int)state;
			this.WinState = (int)state;
			this.LossState = (int)state;
			this.IsNameSelected = false;
			this.IsWinSelected = false;
			this.IsLossSelected = false;
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
		public int WinState
		{
			get
			{
				return this._winState;
			}
			set
			{
				if (value != this._winState)
				{
					this._winState = value;
					base.OnPropertyChangedWithValue(value, "WinState");
				}
			}
		}

		[DataSourceProperty]
		public int LossState
		{
			get
			{
				return this._lossState;
			}
			set
			{
				if (value != this._lossState)
				{
					this._lossState = value;
					base.OnPropertyChangedWithValue(value, "LossState");
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
		public bool IsWinSelected
		{
			get
			{
				return this._isWinSelected;
			}
			set
			{
				if (value != this._isWinSelected)
				{
					this._isWinSelected = value;
					base.OnPropertyChangedWithValue(value, "IsWinSelected");
				}
			}
		}

		[DataSourceProperty]
		public bool IsLossSelected
		{
			get
			{
				return this._isLossSelected;
			}
			set
			{
				if (value != this._isLossSelected)
				{
					this._isLossSelected = value;
					base.OnPropertyChangedWithValue(value, "IsLossSelected");
				}
			}
		}

		private readonly ClanLeaderboardEntry[] _listToControl;

		private readonly MPLobbyClanLeaderboardSortControllerVM.ItemNameComparer _nameComparer;

		private readonly MPLobbyClanLeaderboardSortControllerVM.ItemWinComparer _winComparer;

		private readonly MPLobbyClanLeaderboardSortControllerVM.ItemLossComparer _lossComparer;

		private Action _onSorted;

		private int _nameState;

		private int _winState;

		private int _lossState;

		private bool _isNameSelected;

		private bool _isWinSelected;

		private bool _isLossSelected;

		private enum SortState
		{
			Default,
			Ascending,
			Descending
		}

		private abstract class ItemComparerBase : IComparer<ClanLeaderboardEntry>
		{
			public void SetSortMode(bool isAcending)
			{
				this._isAcending = isAcending;
			}

			public abstract int Compare(ClanLeaderboardEntry x, ClanLeaderboardEntry y);

			protected bool _isAcending;
		}

		private class ItemNameComparer : MPLobbyClanLeaderboardSortControllerVM.ItemComparerBase
		{
			public override int Compare(ClanLeaderboardEntry x, ClanLeaderboardEntry y)
			{
				if (this._isAcending)
				{
					return y.Name.CompareTo(x.Name) * -1;
				}
				return y.Name.CompareTo(x.Name);
			}
		}

		private class ItemWinComparer : MPLobbyClanLeaderboardSortControllerVM.ItemComparerBase
		{
			public override int Compare(ClanLeaderboardEntry x, ClanLeaderboardEntry y)
			{
				if (this._isAcending)
				{
					return y.WinCount.CompareTo(x.WinCount) * -1;
				}
				return y.WinCount.CompareTo(x.WinCount);
			}
		}

		private class ItemLossComparer : MPLobbyClanLeaderboardSortControllerVM.ItemComparerBase
		{
			public override int Compare(ClanLeaderboardEntry x, ClanLeaderboardEntry y)
			{
				if (this._isAcending)
				{
					return y.LossCount.CompareTo(x.LossCount) * -1;
				}
				return y.LossCount.CompareTo(x.LossCount);
			}
		}
	}
}
