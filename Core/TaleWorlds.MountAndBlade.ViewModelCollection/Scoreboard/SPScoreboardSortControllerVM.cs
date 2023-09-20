using System;
using System.Collections.Generic;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Scoreboard
{
	public class SPScoreboardSortControllerVM : ViewModel
	{
		public SPScoreboardSortControllerVM(ref MBBindingList<SPScoreboardPartyVM> listToControl)
		{
			this._listToControl = listToControl;
			this._remainingComparer = new SPScoreboardSortControllerVM.ItemRemainingComparer();
			this._killComparer = new SPScoreboardSortControllerVM.ItemKillComparer();
			this._upgradeComparer = new SPScoreboardSortControllerVM.ItemUpgradeComparer();
			this._deadComparer = new SPScoreboardSortControllerVM.ItemDeadComparer();
			this._woundedComparer = new SPScoreboardSortControllerVM.ItemWoundedComparer();
			this._routedComparer = new SPScoreboardSortControllerVM.ItemRoutedComparer();
			this._memberComparer = new SPScoreboardSortControllerVM.ItemMemberComparer();
		}

		public void ExecuteSortByRemaining()
		{
			int remainingState = this.RemainingState;
			this.SetAllStates(SPScoreboardSortControllerVM.SortState.Default);
			this.RemainingState = (remainingState + 1) % 3;
			this._remainingComparer.SetSortMode(this.RemainingState == 1);
			SPScoreboardSortControllerVM.ScoreboardUnitItemComparerBase scoreboardUnitItemComparerBase = this._remainingComparer;
			if (this.RemainingState == 0)
			{
				scoreboardUnitItemComparerBase = this._memberComparer;
			}
			foreach (SPScoreboardPartyVM spscoreboardPartyVM in this._listToControl)
			{
				spscoreboardPartyVM.Members.Sort(scoreboardUnitItemComparerBase);
			}
			this.IsRemainingSelected = true;
		}

		public void ExecuteSortByKill()
		{
			int killState = this.KillState;
			this.SetAllStates(SPScoreboardSortControllerVM.SortState.Default);
			this.KillState = (killState + 1) % 3;
			SPScoreboardSortControllerVM.ScoreboardUnitItemComparerBase scoreboardUnitItemComparerBase = this._killComparer;
			if (this.KillState == 0)
			{
				scoreboardUnitItemComparerBase = this._memberComparer;
			}
			this._killComparer.SetSortMode(this.KillState == 1);
			foreach (SPScoreboardPartyVM spscoreboardPartyVM in this._listToControl)
			{
				spscoreboardPartyVM.Members.Sort(scoreboardUnitItemComparerBase);
			}
			this.IsKillSelected = true;
		}

		public void ExecuteSortByUpgrade()
		{
			int upgradeState = this.UpgradeState;
			this.SetAllStates(SPScoreboardSortControllerVM.SortState.Default);
			this.UpgradeState = (upgradeState + 1) % 3;
			SPScoreboardSortControllerVM.ScoreboardUnitItemComparerBase scoreboardUnitItemComparerBase = this._upgradeComparer;
			if (this.UpgradeState == 0)
			{
				scoreboardUnitItemComparerBase = this._memberComparer;
			}
			this._upgradeComparer.SetSortMode(this.UpgradeState == 1);
			foreach (SPScoreboardPartyVM spscoreboardPartyVM in this._listToControl)
			{
				spscoreboardPartyVM.Members.Sort(scoreboardUnitItemComparerBase);
			}
			this.IsUpgradeSelected = true;
		}

		public void ExecuteSortByDead()
		{
			int deadState = this.DeadState;
			this.SetAllStates(SPScoreboardSortControllerVM.SortState.Default);
			this.DeadState = (deadState + 1) % 3;
			SPScoreboardSortControllerVM.ScoreboardUnitItemComparerBase scoreboardUnitItemComparerBase = this._deadComparer;
			if (this.DeadState == 0)
			{
				scoreboardUnitItemComparerBase = this._memberComparer;
			}
			this._deadComparer.SetSortMode(this.DeadState == 1);
			foreach (SPScoreboardPartyVM spscoreboardPartyVM in this._listToControl)
			{
				spscoreboardPartyVM.Members.Sort(scoreboardUnitItemComparerBase);
			}
			this.IsDeadSelected = true;
		}

		public void ExecuteSortByWounded()
		{
			int woundedState = this.WoundedState;
			this.SetAllStates(SPScoreboardSortControllerVM.SortState.Default);
			this.WoundedState = (woundedState + 1) % 3;
			SPScoreboardSortControllerVM.ScoreboardUnitItemComparerBase scoreboardUnitItemComparerBase = this._woundedComparer;
			if (this.WoundedState == 0)
			{
				scoreboardUnitItemComparerBase = this._memberComparer;
			}
			this._woundedComparer.SetSortMode(this.WoundedState == 1);
			foreach (SPScoreboardPartyVM spscoreboardPartyVM in this._listToControl)
			{
				spscoreboardPartyVM.Members.Sort(scoreboardUnitItemComparerBase);
			}
			this.IsWoundedSelected = true;
		}

		public void ExecuteSortByRouted()
		{
			int routedState = this.RoutedState;
			this.SetAllStates(SPScoreboardSortControllerVM.SortState.Default);
			this.RoutedState = (routedState + 1) % 3;
			SPScoreboardSortControllerVM.ScoreboardUnitItemComparerBase scoreboardUnitItemComparerBase = this._routedComparer;
			if (this.RoutedState == 0)
			{
				scoreboardUnitItemComparerBase = this._memberComparer;
			}
			this._routedComparer.SetSortMode(this.RoutedState == 1);
			foreach (SPScoreboardPartyVM spscoreboardPartyVM in this._listToControl)
			{
				spscoreboardPartyVM.Members.Sort(scoreboardUnitItemComparerBase);
			}
			this.IsRoutedSelected = true;
		}

		private void SetAllStates(SPScoreboardSortControllerVM.SortState state)
		{
			this.RemainingState = (int)state;
			this.KillState = (int)state;
			this.UpgradeState = (int)state;
			this.DeadState = (int)state;
			this.WoundedState = (int)state;
			this.RoutedState = (int)state;
			this.IsRemainingSelected = false;
			this.IsKillSelected = false;
			this.IsUpgradeSelected = false;
			this.IsDeadSelected = false;
			this.IsWoundedSelected = false;
			this.IsRoutedSelected = false;
		}

		[DataSourceProperty]
		public int RemainingState
		{
			get
			{
				return this._remainingState;
			}
			set
			{
				if (value != this._remainingState)
				{
					this._remainingState = value;
					base.OnPropertyChanged("RemainingState");
				}
			}
		}

		[DataSourceProperty]
		public bool IsRemainingSelected
		{
			get
			{
				return this._isRemainingSelected;
			}
			set
			{
				if (value != this._isRemainingSelected)
				{
					this._isRemainingSelected = value;
					base.OnPropertyChanged("IsRemainingSelected");
				}
			}
		}

		[DataSourceProperty]
		public int KillState
		{
			get
			{
				return this._killState;
			}
			set
			{
				if (value != this._killState)
				{
					this._killState = value;
					base.OnPropertyChanged("KillState");
				}
			}
		}

		[DataSourceProperty]
		public bool IsKillSelected
		{
			get
			{
				return this._isKillSelected;
			}
			set
			{
				if (value != this._isKillSelected)
				{
					this._isKillSelected = value;
					base.OnPropertyChanged("IsKillSelected");
				}
			}
		}

		[DataSourceProperty]
		public int UpgradeState
		{
			get
			{
				return this._upgradeState;
			}
			set
			{
				if (value != this._upgradeState)
				{
					this._upgradeState = value;
					base.OnPropertyChanged("UpgradeState");
				}
			}
		}

		[DataSourceProperty]
		public bool IsUpgradeSelected
		{
			get
			{
				return this._isUpgradeSelected;
			}
			set
			{
				if (value != this._isUpgradeSelected)
				{
					this._isUpgradeSelected = value;
					base.OnPropertyChanged("IsUpgradeSelected");
				}
			}
		}

		[DataSourceProperty]
		public int DeadState
		{
			get
			{
				return this._deadState;
			}
			set
			{
				if (value != this._deadState)
				{
					this._deadState = value;
					base.OnPropertyChanged("DeadState");
				}
			}
		}

		[DataSourceProperty]
		public bool IsDeadSelected
		{
			get
			{
				return this._isDeadSelected;
			}
			set
			{
				if (value != this._isDeadSelected)
				{
					this._isDeadSelected = value;
					base.OnPropertyChanged("IsDeadSelected");
				}
			}
		}

		[DataSourceProperty]
		public int WoundedState
		{
			get
			{
				return this._woundedState;
			}
			set
			{
				if (value != this._woundedState)
				{
					this._woundedState = value;
					base.OnPropertyChanged("WoundedState");
				}
			}
		}

		[DataSourceProperty]
		public bool IsWoundedSelected
		{
			get
			{
				return this._isWoundedSelected;
			}
			set
			{
				if (value != this._isWoundedSelected)
				{
					this._isWoundedSelected = value;
					base.OnPropertyChanged("IsWoundedSelected");
				}
			}
		}

		[DataSourceProperty]
		public int RoutedState
		{
			get
			{
				return this._routedState;
			}
			set
			{
				if (value != this._routedState)
				{
					this._routedState = value;
					base.OnPropertyChanged("RoutedState");
				}
			}
		}

		[DataSourceProperty]
		public bool IsRoutedSelected
		{
			get
			{
				return this._isRoutedSelected;
			}
			set
			{
				if (value != this._isRoutedSelected)
				{
					this._isRoutedSelected = value;
					base.OnPropertyChanged("IsRoutedSelected");
				}
			}
		}

		private readonly MBBindingList<SPScoreboardPartyVM> _listToControl;

		private readonly SPScoreboardSortControllerVM.ItemRemainingComparer _remainingComparer;

		private readonly SPScoreboardSortControllerVM.ItemKillComparer _killComparer;

		private readonly SPScoreboardSortControllerVM.ItemUpgradeComparer _upgradeComparer;

		private readonly SPScoreboardSortControllerVM.ItemDeadComparer _deadComparer;

		private readonly SPScoreboardSortControllerVM.ItemWoundedComparer _woundedComparer;

		private readonly SPScoreboardSortControllerVM.ItemRoutedComparer _routedComparer;

		private readonly SPScoreboardSortControllerVM.ItemMemberComparer _memberComparer;

		private int _remainingState;

		private bool _isRemainingSelected;

		private int _killState;

		private bool _isKillSelected;

		private int _upgradeState;

		private bool _isUpgradeSelected;

		private int _deadState;

		private bool _isDeadSelected;

		private int _woundedState;

		private bool _isWoundedSelected;

		private int _routedState;

		private bool _isRoutedSelected;

		public enum SortState
		{
			Default,
			Ascending,
			Descending
		}

		public abstract class ScoreboardUnitItemComparerBase : IComparer<SPScoreboardUnitVM>
		{
			public void SetSortMode(bool isAscending)
			{
				this._isAscending = isAscending;
			}

			public abstract int Compare(SPScoreboardUnitVM x, SPScoreboardUnitVM y);

			protected bool _isAscending;
		}

		public class ItemRemainingComparer : SPScoreboardSortControllerVM.ScoreboardUnitItemComparerBase
		{
			public override int Compare(SPScoreboardUnitVM x, SPScoreboardUnitVM y)
			{
				if (this._isAscending)
				{
					return y.Score.Remaining.CompareTo(x.Score.Remaining) * -1;
				}
				return y.Score.Remaining.CompareTo(x.Score.Remaining);
			}
		}

		public class ItemKillComparer : SPScoreboardSortControllerVM.ScoreboardUnitItemComparerBase
		{
			public override int Compare(SPScoreboardUnitVM x, SPScoreboardUnitVM y)
			{
				if (this._isAscending)
				{
					return y.Score.Kill.CompareTo(x.Score.Kill) * -1;
				}
				return y.Score.Kill.CompareTo(x.Score.Kill);
			}
		}

		public class ItemUpgradeComparer : SPScoreboardSortControllerVM.ScoreboardUnitItemComparerBase
		{
			public override int Compare(SPScoreboardUnitVM x, SPScoreboardUnitVM y)
			{
				if (this._isAscending)
				{
					return y.Score.ReadyToUpgrade.CompareTo(x.Score.ReadyToUpgrade) * -1;
				}
				return y.Score.ReadyToUpgrade.CompareTo(x.Score.ReadyToUpgrade);
			}
		}

		public class ItemDeadComparer : SPScoreboardSortControllerVM.ScoreboardUnitItemComparerBase
		{
			public override int Compare(SPScoreboardUnitVM x, SPScoreboardUnitVM y)
			{
				if (this._isAscending)
				{
					return y.Score.Dead.CompareTo(x.Score.Dead) * -1;
				}
				return y.Score.Dead.CompareTo(x.Score.Dead);
			}
		}

		public class ItemWoundedComparer : SPScoreboardSortControllerVM.ScoreboardUnitItemComparerBase
		{
			public override int Compare(SPScoreboardUnitVM x, SPScoreboardUnitVM y)
			{
				if (this._isAscending)
				{
					return y.Score.Wounded.CompareTo(x.Score.Wounded) * -1;
				}
				return y.Score.Wounded.CompareTo(x.Score.Wounded);
			}
		}

		public class ItemRoutedComparer : SPScoreboardSortControllerVM.ScoreboardUnitItemComparerBase
		{
			public override int Compare(SPScoreboardUnitVM x, SPScoreboardUnitVM y)
			{
				if (this._isAscending)
				{
					return y.Score.Routed.CompareTo(x.Score.Routed) * -1;
				}
				return y.Score.Routed.CompareTo(x.Score.Routed);
			}
		}

		public class ItemMemberComparer : SPScoreboardSortControllerVM.ScoreboardUnitItemComparerBase
		{
			public override int Compare(SPScoreboardUnitVM x, SPScoreboardUnitVM y)
			{
				if (x.Character.IsPlayerCharacter && !y.Character.IsPlayerCharacter)
				{
					return -1;
				}
				if (!x.Character.IsPlayerCharacter && y.Character.IsPlayerCharacter)
				{
					return 1;
				}
				if (x.IsHero && !y.IsHero)
				{
					return -1;
				}
				if (!x.IsHero && y.IsHero)
				{
					return 1;
				}
				return x.Character.Name.ToString().CompareTo(y.Character.Name.ToString());
			}
		}
	}
}
