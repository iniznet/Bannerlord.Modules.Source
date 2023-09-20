using System;
using System.Collections.Generic;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.KingdomManagement.Diplomacy
{
	public class KingdomWarSortControllerVM : ViewModel
	{
		public KingdomWarSortControllerVM(ref MBBindingList<KingdomWarItemVM> listToControl)
		{
			this._listToControl = listToControl;
			this._scoreComparer = new KingdomWarSortControllerVM.ItemScoreComparer();
		}

		private void ExecuteSortByScore()
		{
			int scoreState = this.ScoreState;
			this.SetAllStates(KingdomWarSortControllerVM.SortState.Default);
			this.ScoreState = (scoreState + 1) % 3;
			if (this.ScoreState == 0)
			{
				int scoreState2 = this.ScoreState;
				this.ScoreState = scoreState2 + 1;
			}
			this._scoreComparer.SetSortMode(this.ScoreState == 1);
			this._listToControl.Sort(this._scoreComparer);
			this.IsScoreSelected = true;
		}

		private void SetAllStates(KingdomWarSortControllerVM.SortState state)
		{
			this.ScoreState = (int)state;
			this.IsScoreSelected = false;
		}

		[DataSourceProperty]
		public int ScoreState
		{
			get
			{
				return this._scoreState;
			}
			set
			{
				if (value != this._scoreState)
				{
					this._scoreState = value;
					base.OnPropertyChangedWithValue(value, "ScoreState");
				}
			}
		}

		[DataSourceProperty]
		public bool IsScoreSelected
		{
			get
			{
				return this._isScoreSelected;
			}
			set
			{
				if (value != this._isScoreSelected)
				{
					this._isScoreSelected = value;
					base.OnPropertyChangedWithValue(value, "IsScoreSelected");
				}
			}
		}

		private readonly MBBindingList<KingdomWarItemVM> _listToControl;

		private readonly KingdomWarSortControllerVM.ItemScoreComparer _scoreComparer;

		private int _scoreState;

		private bool _isScoreSelected;

		private enum SortState
		{
			Default,
			Ascending,
			Descending
		}

		public abstract class ItemComparerBase : IComparer<KingdomWarItemVM>
		{
			public void SetSortMode(bool isAscending)
			{
				this._isAscending = isAscending;
			}

			public abstract int Compare(KingdomWarItemVM x, KingdomWarItemVM y);

			protected bool _isAscending;
		}

		public class ItemScoreComparer : KingdomWarSortControllerVM.ItemComparerBase
		{
			public override int Compare(KingdomWarItemVM x, KingdomWarItemVM y)
			{
				if (this._isAscending)
				{
					return x.Score.CompareTo(y.Score);
				}
				return x.Score.CompareTo(y.Score) * -1;
			}
		}
	}
}
