using System;
using System.Collections.Generic;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Diamond;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.Clan
{
	// Token: 0x0200009A RID: 154
	public class MPLobbyClanLeaderboardSortControllerVM : ViewModel
	{
		// Token: 0x06000E8C RID: 3724 RVA: 0x00031586 File Offset: 0x0002F786
		public MPLobbyClanLeaderboardSortControllerVM(ref ClanLeaderboardEntry[] listToControl, Action onSorted)
		{
			this._listToControl = listToControl;
			this._winComparer = new MPLobbyClanLeaderboardSortControllerVM.ItemWinComparer();
			this._lossComparer = new MPLobbyClanLeaderboardSortControllerVM.ItemLossComparer();
			this._nameComparer = new MPLobbyClanLeaderboardSortControllerVM.ItemNameComparer();
			this._onSorted = onSorted;
		}

		// Token: 0x06000E8D RID: 3725 RVA: 0x000315C0 File Offset: 0x0002F7C0
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

		// Token: 0x06000E8E RID: 3726 RVA: 0x00031634 File Offset: 0x0002F834
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

		// Token: 0x06000E8F RID: 3727 RVA: 0x000316A8 File Offset: 0x0002F8A8
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

		// Token: 0x06000E90 RID: 3728 RVA: 0x0003171B File Offset: 0x0002F91B
		private void SetAllStates(MPLobbyClanLeaderboardSortControllerVM.SortState state)
		{
			this.NameState = (int)state;
			this.WinState = (int)state;
			this.LossState = (int)state;
			this.IsNameSelected = false;
			this.IsWinSelected = false;
			this.IsLossSelected = false;
		}

		// Token: 0x170004A4 RID: 1188
		// (get) Token: 0x06000E91 RID: 3729 RVA: 0x00031747 File Offset: 0x0002F947
		// (set) Token: 0x06000E92 RID: 3730 RVA: 0x0003174F File Offset: 0x0002F94F
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

		// Token: 0x170004A5 RID: 1189
		// (get) Token: 0x06000E93 RID: 3731 RVA: 0x0003176D File Offset: 0x0002F96D
		// (set) Token: 0x06000E94 RID: 3732 RVA: 0x00031775 File Offset: 0x0002F975
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

		// Token: 0x170004A6 RID: 1190
		// (get) Token: 0x06000E95 RID: 3733 RVA: 0x00031793 File Offset: 0x0002F993
		// (set) Token: 0x06000E96 RID: 3734 RVA: 0x0003179B File Offset: 0x0002F99B
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

		// Token: 0x170004A7 RID: 1191
		// (get) Token: 0x06000E97 RID: 3735 RVA: 0x000317B9 File Offset: 0x0002F9B9
		// (set) Token: 0x06000E98 RID: 3736 RVA: 0x000317C1 File Offset: 0x0002F9C1
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

		// Token: 0x170004A8 RID: 1192
		// (get) Token: 0x06000E99 RID: 3737 RVA: 0x000317DF File Offset: 0x0002F9DF
		// (set) Token: 0x06000E9A RID: 3738 RVA: 0x000317E7 File Offset: 0x0002F9E7
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

		// Token: 0x170004A9 RID: 1193
		// (get) Token: 0x06000E9B RID: 3739 RVA: 0x00031805 File Offset: 0x0002FA05
		// (set) Token: 0x06000E9C RID: 3740 RVA: 0x0003180D File Offset: 0x0002FA0D
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

		// Token: 0x040006E6 RID: 1766
		private readonly ClanLeaderboardEntry[] _listToControl;

		// Token: 0x040006E7 RID: 1767
		private readonly MPLobbyClanLeaderboardSortControllerVM.ItemNameComparer _nameComparer;

		// Token: 0x040006E8 RID: 1768
		private readonly MPLobbyClanLeaderboardSortControllerVM.ItemWinComparer _winComparer;

		// Token: 0x040006E9 RID: 1769
		private readonly MPLobbyClanLeaderboardSortControllerVM.ItemLossComparer _lossComparer;

		// Token: 0x040006EA RID: 1770
		private Action _onSorted;

		// Token: 0x040006EB RID: 1771
		private int _nameState;

		// Token: 0x040006EC RID: 1772
		private int _winState;

		// Token: 0x040006ED RID: 1773
		private int _lossState;

		// Token: 0x040006EE RID: 1774
		private bool _isNameSelected;

		// Token: 0x040006EF RID: 1775
		private bool _isWinSelected;

		// Token: 0x040006F0 RID: 1776
		private bool _isLossSelected;

		// Token: 0x020001ED RID: 493
		private enum SortState
		{
			// Token: 0x04000E1C RID: 3612
			Default,
			// Token: 0x04000E1D RID: 3613
			Ascending,
			// Token: 0x04000E1E RID: 3614
			Descending
		}

		// Token: 0x020001EE RID: 494
		private abstract class ItemComparerBase : IComparer<ClanLeaderboardEntry>
		{
			// Token: 0x06001A95 RID: 6805 RVA: 0x00055EA2 File Offset: 0x000540A2
			public void SetSortMode(bool isAcending)
			{
				this._isAcending = isAcending;
			}

			// Token: 0x06001A96 RID: 6806
			public abstract int Compare(ClanLeaderboardEntry x, ClanLeaderboardEntry y);

			// Token: 0x04000E1F RID: 3615
			protected bool _isAcending;
		}

		// Token: 0x020001EF RID: 495
		private class ItemNameComparer : MPLobbyClanLeaderboardSortControllerVM.ItemComparerBase
		{
			// Token: 0x06001A98 RID: 6808 RVA: 0x00055EB3 File Offset: 0x000540B3
			public override int Compare(ClanLeaderboardEntry x, ClanLeaderboardEntry y)
			{
				if (this._isAcending)
				{
					return y.Name.CompareTo(x.Name) * -1;
				}
				return y.Name.CompareTo(x.Name);
			}
		}

		// Token: 0x020001F0 RID: 496
		private class ItemWinComparer : MPLobbyClanLeaderboardSortControllerVM.ItemComparerBase
		{
			// Token: 0x06001A9A RID: 6810 RVA: 0x00055EEC File Offset: 0x000540EC
			public override int Compare(ClanLeaderboardEntry x, ClanLeaderboardEntry y)
			{
				if (this._isAcending)
				{
					return y.WinCount.CompareTo(x.WinCount) * -1;
				}
				return y.WinCount.CompareTo(x.WinCount);
			}
		}

		// Token: 0x020001F1 RID: 497
		private class ItemLossComparer : MPLobbyClanLeaderboardSortControllerVM.ItemComparerBase
		{
			// Token: 0x06001A9C RID: 6812 RVA: 0x00055F34 File Offset: 0x00054134
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
