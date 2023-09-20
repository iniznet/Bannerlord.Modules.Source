using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Scoreboard
{
	public class MissionScoreboardPlayerSortControllerVM : ViewModel
	{
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

		public override void RefreshValues()
		{
			this.NameText = GameTexts.FindText("str_scoreboard_header", "name").ToString();
			this.ScoreText = GameTexts.FindText("str_scoreboard_header", "score").ToString();
			this.KillText = GameTexts.FindText("str_scoreboard_header", "kill").ToString();
			this.AssistText = GameTexts.FindText("str_scoreboard_header", "assist").ToString();
		}

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

		private const string _nameHeaderID = "name";

		private const string _scoreHeaderID = "score";

		private const string _killHeaderID = "kill";

		private const string _assistHeaderID = "assist";

		private readonly MBBindingList<MissionScoreboardPlayerVM> _listToControl;

		private readonly MissionScoreboardPlayerSortControllerVM.ItemNameComparer _nameComparer;

		private readonly MissionScoreboardPlayerSortControllerVM.ItemScoreComparer _scoreComparer;

		private readonly MissionScoreboardPlayerSortControllerVM.ItemKillComparer _killComparer;

		private readonly MissionScoreboardPlayerSortControllerVM.ItemAssistComparer _assistComparer;

		private string _nameText;

		private string _scoreText;

		private string _killText;

		private string _assistText;

		private int _nameState = 1;

		private int _scoreState = 1;

		private int _killState = 1;

		private int _assistState = 1;

		private bool _isNameSelected;

		private bool _isScoreSelected;

		private bool _isKillSelected;

		private bool _isAssistSelected;

		private enum SortState
		{
			Default,
			Ascending,
			Descending
		}

		public abstract class ItemComparerBase : IComparer<MissionScoreboardPlayerVM>
		{
			public void SetSortMode(bool isAscending)
			{
				this._isAscending = isAscending;
			}

			public abstract int Compare(MissionScoreboardPlayerVM x, MissionScoreboardPlayerVM y);

			protected bool _isAscending;
		}

		public class ItemNameComparer : MissionScoreboardPlayerSortControllerVM.ItemComparerBase
		{
			public override int Compare(MissionScoreboardPlayerVM x, MissionScoreboardPlayerVM y)
			{
				return y.Name.CompareTo(x.Name) * (this._isAscending ? (-1) : 1);
			}
		}

		public class ItemScoreComparer : MissionScoreboardPlayerSortControllerVM.ItemComparerBase
		{
			public override int Compare(MissionScoreboardPlayerVM x, MissionScoreboardPlayerVM y)
			{
				return y.Score.CompareTo(x.Score) * (this._isAscending ? (-1) : 1);
			}
		}

		public class ItemKillComparer : MissionScoreboardPlayerSortControllerVM.ItemComparerBase
		{
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

		public class ItemAssistComparer : MissionScoreboardPlayerSortControllerVM.ItemComparerBase
		{
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
