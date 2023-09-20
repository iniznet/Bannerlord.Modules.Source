using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Scoreboard
{
	// Token: 0x0200004F RID: 79
	public class MissionScoreboardPlayerSortControllerVM : ViewModel
	{
		// Token: 0x0600066E RID: 1646 RVA: 0x0001A448 File Offset: 0x00018648
		public MissionScoreboardPlayerSortControllerVM(ref MBBindingList<MissionScoreboardPlayerVM> listToControl)
		{
			this._listToControl = listToControl;
			this._nameComparer = new MissionScoreboardPlayerSortControllerVM.ItemNameComparer();
			this._scoreComparer = new MissionScoreboardPlayerSortControllerVM.ItemScoreComparer();
			this._killComparer = new MissionScoreboardPlayerSortControllerVM.ItemKillComparer();
			this._assistComparer = new MissionScoreboardPlayerSortControllerVM.ItemAssistComparer();
			this.ExecuteSortByScore();
			this.RefreshValues();
		}

		// Token: 0x0600066F RID: 1647 RVA: 0x0001A4B8 File Offset: 0x000186B8
		public override void RefreshValues()
		{
			this.NameText = GameTexts.FindText("str_scoreboard_header", "name").ToString();
			this.ScoreText = GameTexts.FindText("str_scoreboard_header", "score").ToString();
			this.KillText = GameTexts.FindText("str_scoreboard_header", "kill").ToString();
			this.AssistText = GameTexts.FindText("str_scoreboard_header", "assist").ToString();
		}

		// Token: 0x06000670 RID: 1648 RVA: 0x0001A530 File Offset: 0x00018730
		public void SortByCurrentState()
		{
			if (this.IsNameSelected)
			{
				this._listToControl.Sort(this._nameComparer);
				return;
			}
			if (this.IsScoreSelected)
			{
				this._listToControl.Sort(this._scoreComparer);
				return;
			}
			if (this.IsKillSelected)
			{
				this._listToControl.Sort(this._killComparer);
				return;
			}
			if (this.IsAssistSelected)
			{
				this._listToControl.Sort(this._assistComparer);
			}
		}

		// Token: 0x06000671 RID: 1649 RVA: 0x0001A5A4 File Offset: 0x000187A4
		public void ExecuteSortByName()
		{
			int nameState = this.NameState;
			this.SetAllStates(MissionScoreboardPlayerSortControllerVM.SortState.Default);
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

		// Token: 0x06000672 RID: 1650 RVA: 0x0001A610 File Offset: 0x00018810
		public void ExecuteSortByScore()
		{
			int scoreState = this.ScoreState;
			this.SetAllStates(MissionScoreboardPlayerSortControllerVM.SortState.Default);
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

		// Token: 0x06000673 RID: 1651 RVA: 0x0001A67C File Offset: 0x0001887C
		public void ExecuteSortByKill()
		{
			int killState = this.KillState;
			this.SetAllStates(MissionScoreboardPlayerSortControllerVM.SortState.Default);
			this.KillState = (killState + 1) % 3;
			if (this.KillState == 0)
			{
				int killState2 = this.KillState;
				this.KillState = killState2 + 1;
			}
			this._killComparer.SetSortMode(this.KillState == 1);
			this._listToControl.Sort(this._killComparer);
			this.IsKillSelected = true;
		}

		// Token: 0x06000674 RID: 1652 RVA: 0x0001A6E8 File Offset: 0x000188E8
		public void ExecuteSortByAssist()
		{
			int assistState = this.AssistState;
			this.SetAllStates(MissionScoreboardPlayerSortControllerVM.SortState.Default);
			this.AssistState = (assistState + 1) % 3;
			if (this.AssistState == 0)
			{
				int assistState2 = this.AssistState;
				this.AssistState = assistState2 + 1;
			}
			this._assistComparer.SetSortMode(this.AssistState == 1);
			this._listToControl.Sort(this._assistComparer);
			this.IsAssistSelected = true;
		}

		// Token: 0x06000675 RID: 1653 RVA: 0x0001A752 File Offset: 0x00018952
		private void SetAllStates(MissionScoreboardPlayerSortControllerVM.SortState state)
		{
			this.NameState = (int)state;
			this.ScoreState = (int)state;
			this.KillState = (int)state;
			this.AssistState = (int)state;
			this.IsNameSelected = false;
			this.IsScoreSelected = false;
			this.IsKillSelected = false;
			this.IsAssistSelected = false;
		}

		// Token: 0x170001ED RID: 493
		// (get) Token: 0x06000676 RID: 1654 RVA: 0x0001A78C File Offset: 0x0001898C
		// (set) Token: 0x06000677 RID: 1655 RVA: 0x0001A794 File Offset: 0x00018994
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

		// Token: 0x170001EE RID: 494
		// (get) Token: 0x06000678 RID: 1656 RVA: 0x0001A7B7 File Offset: 0x000189B7
		// (set) Token: 0x06000679 RID: 1657 RVA: 0x0001A7BF File Offset: 0x000189BF
		[DataSourceProperty]
		public string ScoreText
		{
			get
			{
				return this._scoreText;
			}
			set
			{
				if (value != this._scoreText)
				{
					this._scoreText = value;
					base.OnPropertyChangedWithValue<string>(value, "ScoreText");
				}
			}
		}

		// Token: 0x170001EF RID: 495
		// (get) Token: 0x0600067A RID: 1658 RVA: 0x0001A7E2 File Offset: 0x000189E2
		// (set) Token: 0x0600067B RID: 1659 RVA: 0x0001A7EA File Offset: 0x000189EA
		[DataSourceProperty]
		public string KillText
		{
			get
			{
				return this._killText;
			}
			set
			{
				if (value != this._killText)
				{
					this._killText = value;
					base.OnPropertyChangedWithValue<string>(value, "KillText");
				}
			}
		}

		// Token: 0x170001F0 RID: 496
		// (get) Token: 0x0600067C RID: 1660 RVA: 0x0001A80D File Offset: 0x00018A0D
		// (set) Token: 0x0600067D RID: 1661 RVA: 0x0001A815 File Offset: 0x00018A15
		[DataSourceProperty]
		public string AssistText
		{
			get
			{
				return this._assistText;
			}
			set
			{
				if (value != this._assistText)
				{
					this._assistText = value;
					base.OnPropertyChangedWithValue<string>(value, "AssistText");
				}
			}
		}

		// Token: 0x170001F1 RID: 497
		// (get) Token: 0x0600067E RID: 1662 RVA: 0x0001A838 File Offset: 0x00018A38
		// (set) Token: 0x0600067F RID: 1663 RVA: 0x0001A840 File Offset: 0x00018A40
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

		// Token: 0x170001F2 RID: 498
		// (get) Token: 0x06000680 RID: 1664 RVA: 0x0001A85E File Offset: 0x00018A5E
		// (set) Token: 0x06000681 RID: 1665 RVA: 0x0001A866 File Offset: 0x00018A66
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

		// Token: 0x170001F3 RID: 499
		// (get) Token: 0x06000682 RID: 1666 RVA: 0x0001A884 File Offset: 0x00018A84
		// (set) Token: 0x06000683 RID: 1667 RVA: 0x0001A88C File Offset: 0x00018A8C
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
					base.OnPropertyChangedWithValue(value, "KillState");
				}
			}
		}

		// Token: 0x170001F4 RID: 500
		// (get) Token: 0x06000684 RID: 1668 RVA: 0x0001A8AA File Offset: 0x00018AAA
		// (set) Token: 0x06000685 RID: 1669 RVA: 0x0001A8B2 File Offset: 0x00018AB2
		[DataSourceProperty]
		public int AssistState
		{
			get
			{
				return this._assistState;
			}
			set
			{
				if (value != this._assistState)
				{
					this._assistState = value;
					base.OnPropertyChangedWithValue(value, "AssistState");
				}
			}
		}

		// Token: 0x170001F5 RID: 501
		// (get) Token: 0x06000686 RID: 1670 RVA: 0x0001A8D0 File Offset: 0x00018AD0
		// (set) Token: 0x06000687 RID: 1671 RVA: 0x0001A8D8 File Offset: 0x00018AD8
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

		// Token: 0x170001F6 RID: 502
		// (get) Token: 0x06000688 RID: 1672 RVA: 0x0001A8F6 File Offset: 0x00018AF6
		// (set) Token: 0x06000689 RID: 1673 RVA: 0x0001A8FE File Offset: 0x00018AFE
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

		// Token: 0x170001F7 RID: 503
		// (get) Token: 0x0600068A RID: 1674 RVA: 0x0001A91C File Offset: 0x00018B1C
		// (set) Token: 0x0600068B RID: 1675 RVA: 0x0001A924 File Offset: 0x00018B24
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
					base.OnPropertyChangedWithValue(value, "IsKillSelected");
				}
			}
		}

		// Token: 0x170001F8 RID: 504
		// (get) Token: 0x0600068C RID: 1676 RVA: 0x0001A942 File Offset: 0x00018B42
		// (set) Token: 0x0600068D RID: 1677 RVA: 0x0001A94A File Offset: 0x00018B4A
		[DataSourceProperty]
		public bool IsAssistSelected
		{
			get
			{
				return this._isAssistSelected;
			}
			set
			{
				if (value != this._isAssistSelected)
				{
					this._isAssistSelected = value;
					base.OnPropertyChangedWithValue(value, "IsAssistSelected");
				}
			}
		}

		// Token: 0x04000343 RID: 835
		private const string _nameHeaderID = "name";

		// Token: 0x04000344 RID: 836
		private const string _scoreHeaderID = "score";

		// Token: 0x04000345 RID: 837
		private const string _killHeaderID = "kill";

		// Token: 0x04000346 RID: 838
		private const string _assistHeaderID = "assist";

		// Token: 0x04000347 RID: 839
		private readonly MBBindingList<MissionScoreboardPlayerVM> _listToControl;

		// Token: 0x04000348 RID: 840
		private readonly MissionScoreboardPlayerSortControllerVM.ItemNameComparer _nameComparer;

		// Token: 0x04000349 RID: 841
		private readonly MissionScoreboardPlayerSortControllerVM.ItemScoreComparer _scoreComparer;

		// Token: 0x0400034A RID: 842
		private readonly MissionScoreboardPlayerSortControllerVM.ItemKillComparer _killComparer;

		// Token: 0x0400034B RID: 843
		private readonly MissionScoreboardPlayerSortControllerVM.ItemAssistComparer _assistComparer;

		// Token: 0x0400034C RID: 844
		private string _nameText;

		// Token: 0x0400034D RID: 845
		private string _scoreText;

		// Token: 0x0400034E RID: 846
		private string _killText;

		// Token: 0x0400034F RID: 847
		private string _assistText;

		// Token: 0x04000350 RID: 848
		private int _nameState = 1;

		// Token: 0x04000351 RID: 849
		private int _scoreState = 1;

		// Token: 0x04000352 RID: 850
		private int _killState = 1;

		// Token: 0x04000353 RID: 851
		private int _assistState = 1;

		// Token: 0x04000354 RID: 852
		private bool _isNameSelected;

		// Token: 0x04000355 RID: 853
		private bool _isScoreSelected;

		// Token: 0x04000356 RID: 854
		private bool _isKillSelected;

		// Token: 0x04000357 RID: 855
		private bool _isAssistSelected;

		// Token: 0x0200016D RID: 365
		private enum SortState
		{
			// Token: 0x04000C96 RID: 3222
			Default,
			// Token: 0x04000C97 RID: 3223
			Ascending,
			// Token: 0x04000C98 RID: 3224
			Descending
		}

		// Token: 0x0200016E RID: 366
		public abstract class ItemComparerBase : IComparer<MissionScoreboardPlayerVM>
		{
			// Token: 0x06001948 RID: 6472 RVA: 0x0005130D File Offset: 0x0004F50D
			public void SetSortMode(bool isAscending)
			{
				this._isAscending = isAscending;
			}

			// Token: 0x06001949 RID: 6473
			public abstract int Compare(MissionScoreboardPlayerVM x, MissionScoreboardPlayerVM y);

			// Token: 0x04000C99 RID: 3225
			protected bool _isAscending;
		}

		// Token: 0x0200016F RID: 367
		public class ItemNameComparer : MissionScoreboardPlayerSortControllerVM.ItemComparerBase
		{
			// Token: 0x0600194B RID: 6475 RVA: 0x0005131E File Offset: 0x0004F51E
			public override int Compare(MissionScoreboardPlayerVM x, MissionScoreboardPlayerVM y)
			{
				return y.Name.CompareTo(x.Name) * (this._isAscending ? (-1) : 1);
			}
		}

		// Token: 0x02000170 RID: 368
		public class ItemScoreComparer : MissionScoreboardPlayerSortControllerVM.ItemComparerBase
		{
			// Token: 0x0600194D RID: 6477 RVA: 0x00051348 File Offset: 0x0004F548
			public override int Compare(MissionScoreboardPlayerVM x, MissionScoreboardPlayerVM y)
			{
				return y.Score.CompareTo(x.Score) * (this._isAscending ? (-1) : 1);
			}
		}

		// Token: 0x02000171 RID: 369
		public class ItemKillComparer : MissionScoreboardPlayerSortControllerVM.ItemComparerBase
		{
			// Token: 0x0600194F RID: 6479 RVA: 0x00051380 File Offset: 0x0004F580
			public override int Compare(MissionScoreboardPlayerVM x, MissionScoreboardPlayerVM y)
			{
				MissionScoreboardStatItemVM missionScoreboardStatItemVM = x.Stats.FirstOrDefault((MissionScoreboardStatItemVM s) => s.HeaderID == "kill");
				MissionScoreboardStatItemVM missionScoreboardStatItemVM2 = y.Stats.FirstOrDefault((MissionScoreboardStatItemVM s) => s.HeaderID == "kill");
				if (missionScoreboardStatItemVM != null && missionScoreboardStatItemVM2 != null)
				{
					return int.Parse(missionScoreboardStatItemVM2.Item).CompareTo(int.Parse(missionScoreboardStatItemVM.Item)) * (this._isAscending ? (-1) : 1);
				}
				return 0;
			}
		}

		// Token: 0x02000172 RID: 370
		public class ItemAssistComparer : MissionScoreboardPlayerSortControllerVM.ItemComparerBase
		{
			// Token: 0x06001951 RID: 6481 RVA: 0x00051420 File Offset: 0x0004F620
			public override int Compare(MissionScoreboardPlayerVM x, MissionScoreboardPlayerVM y)
			{
				MissionScoreboardStatItemVM missionScoreboardStatItemVM = x.Stats.FirstOrDefault((MissionScoreboardStatItemVM s) => s.HeaderID == "assist");
				MissionScoreboardStatItemVM missionScoreboardStatItemVM2 = y.Stats.FirstOrDefault((MissionScoreboardStatItemVM s) => s.HeaderID == "assist");
				if (missionScoreboardStatItemVM != null && missionScoreboardStatItemVM2 != null)
				{
					return int.Parse(missionScoreboardStatItemVM2.Item).CompareTo(int.Parse(missionScoreboardStatItemVM.Item)) * (this._isAscending ? (-1) : 1);
				}
				return 0;
			}
		}
	}
}
