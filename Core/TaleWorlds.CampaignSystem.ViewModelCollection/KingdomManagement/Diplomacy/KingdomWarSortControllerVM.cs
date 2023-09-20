using System;
using System.Collections.Generic;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.KingdomManagement.Diplomacy
{
	// Token: 0x02000062 RID: 98
	public class KingdomWarSortControllerVM : ViewModel
	{
		// Token: 0x0600086B RID: 2155 RVA: 0x000239CE File Offset: 0x00021BCE
		public KingdomWarSortControllerVM(ref MBBindingList<KingdomWarItemVM> listToControl)
		{
			this._listToControl = listToControl;
			this._scoreComparer = new KingdomWarSortControllerVM.ItemScoreComparer();
		}

		// Token: 0x0600086C RID: 2156 RVA: 0x000239EC File Offset: 0x00021BEC
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

		// Token: 0x0600086D RID: 2157 RVA: 0x00023A56 File Offset: 0x00021C56
		private void SetAllStates(KingdomWarSortControllerVM.SortState state)
		{
			this.ScoreState = (int)state;
			this.IsScoreSelected = false;
		}

		// Token: 0x17000296 RID: 662
		// (get) Token: 0x0600086E RID: 2158 RVA: 0x00023A66 File Offset: 0x00021C66
		// (set) Token: 0x0600086F RID: 2159 RVA: 0x00023A6E File Offset: 0x00021C6E
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

		// Token: 0x17000297 RID: 663
		// (get) Token: 0x06000870 RID: 2160 RVA: 0x00023A8C File Offset: 0x00021C8C
		// (set) Token: 0x06000871 RID: 2161 RVA: 0x00023A94 File Offset: 0x00021C94
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

		// Token: 0x040003C8 RID: 968
		private readonly MBBindingList<KingdomWarItemVM> _listToControl;

		// Token: 0x040003C9 RID: 969
		private readonly KingdomWarSortControllerVM.ItemScoreComparer _scoreComparer;

		// Token: 0x040003CA RID: 970
		private int _scoreState;

		// Token: 0x040003CB RID: 971
		private bool _isScoreSelected;

		// Token: 0x02000196 RID: 406
		private enum SortState
		{
			// Token: 0x04000F36 RID: 3894
			Default,
			// Token: 0x04000F37 RID: 3895
			Ascending,
			// Token: 0x04000F38 RID: 3896
			Descending
		}

		// Token: 0x02000197 RID: 407
		public abstract class ItemComparerBase : IComparer<KingdomWarItemVM>
		{
			// Token: 0x06001F83 RID: 8067 RVA: 0x0006E9ED File Offset: 0x0006CBED
			public void SetSortMode(bool isAscending)
			{
				this._isAscending = isAscending;
			}

			// Token: 0x06001F84 RID: 8068
			public abstract int Compare(KingdomWarItemVM x, KingdomWarItemVM y);

			// Token: 0x04000F39 RID: 3897
			protected bool _isAscending;
		}

		// Token: 0x02000198 RID: 408
		public class ItemScoreComparer : KingdomWarSortControllerVM.ItemComparerBase
		{
			// Token: 0x06001F86 RID: 8070 RVA: 0x0006EA00 File Offset: 0x0006CC00
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
