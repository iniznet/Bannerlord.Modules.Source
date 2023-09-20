using System;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.ViewModelCollection.Input;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Scoreboard
{
	public abstract class ScoreboardBaseVM : ViewModel
	{
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.KillHint = new HintViewModel(GameTexts.FindText("str_battle_result_score_sort_button", "0"), null);
			this.DeadHint = new HintViewModel(GameTexts.FindText("str_battle_result_score_sort_button", "1"), null);
			this.WoundedHint = new HintViewModel(GameTexts.FindText("str_battle_result_score_sort_button", "2"), null);
			this.RoutedHint = new HintViewModel(GameTexts.FindText("str_battle_result_score_sort_button", "3"), null);
			this.RemainingHint = new HintViewModel(GameTexts.FindText("str_battle_result_score_sort_button", "4"), null);
			this.UpgradeHint = new HintViewModel(GameTexts.FindText("str_battle_result_score_sort_button", "5"), null);
			this.QuitText = GameTexts.FindText("str_retreat", null).ToString();
			GameTexts.SetVariable("KEY", Game.Current.GameTextManager.GetHotKeyGameText("Generic", 4));
			this._retreatInquiryData = new InquiryData("", GameTexts.FindText("str_can_not_retreat", null).ToString(), true, false, GameTexts.FindText("str_ok", null).ToString(), "", null, null, "", 0f, null, null, null);
			SPScoreboardSideVM attackers = this.Attackers;
			if (attackers != null)
			{
				attackers.RefreshValues();
			}
			SPScoreboardSideVM defenders = this.Defenders;
			if (defenders != null)
			{
				defenders.RefreshValues();
			}
			this.ShowScoreboardText = new TextObject("{=5Ixsvn3s}Toggle scoreboard", null).ToString();
			this.FastForwardText = new TextObject("{=HH7LDwlK}Toggle Fast Forward", null).ToString();
			InputKeyItemVM showMouseKey = this.ShowMouseKey;
			if (showMouseKey != null)
			{
				showMouseKey.RefreshValues();
			}
			InputKeyItemVM showScoreboardKey = this.ShowScoreboardKey;
			if (showScoreboardKey != null)
			{
				showScoreboardKey.RefreshValues();
			}
			InputKeyItemVM doneInputKey = this.DoneInputKey;
			if (doneInputKey != null)
			{
				doneInputKey.RefreshValues();
			}
			InputKeyItemVM fastForwardKey = this.FastForwardKey;
			if (fastForwardKey == null)
			{
				return;
			}
			fastForwardKey.RefreshValues();
		}

		public void OnMainHeroDeath()
		{
			this.IsMainCharacterDead = true;
			IMissionScreen missionScreen = this._missionScreen;
			if (missionScreen == null)
			{
				return;
			}
			missionScreen.SetOrderFlagVisibility(false);
		}

		public virtual void Initialize(IMissionScreen missionScreen, Mission mission, Action releaseSimulationSources, Action<bool> onToggle)
		{
			this.OnToggle = onToggle;
			this._missionScreen = missionScreen;
			this._mission = mission;
			this._releaseSimulationSources = releaseSimulationSources;
			this.BattleResult = "";
			this.BattleResultIndex = -1;
			this.IsOver = false;
			this.ShowScoreboard = false;
			Action<bool> onToggle2 = this.OnToggle;
			if (onToggle2 != null)
			{
				onToggle2(false);
			}
			if (mission != null)
			{
				this._battleEndLogic = this._mission.GetMissionBehavior<BattleEndLogic>();
			}
			this.PowerComparer = new PowerLevelComparer(1.0, 1.0);
			this.RefreshValues();
		}

		private void UpdateQuitText()
		{
			if (this.IsOver)
			{
				this.QuitText = GameTexts.FindText("str_done", null).ToString();
				return;
			}
			if (this.IsMainCharacterDead && !this.IsSimulation)
			{
				this.QuitText = GameTexts.FindText("str_end_battle", null).ToString();
			}
		}

		public abstract void Tick(float dt);

		protected SPScoreboardSideVM GetSide(BattleSideEnum side)
		{
			if (side != BattleSideEnum.Defender)
			{
				return this.Attackers;
			}
			return this.Defenders;
		}

		public void SetMouseState(bool visible)
		{
			this._mouseState = (visible ? ScoreboardBaseVM.MouseState.Visible : ScoreboardBaseVM.MouseState.NotVisible);
			this.IsMouseEnabled = visible;
		}

		public static string GetFormattedTimeTextFromSeconds(int seconds)
		{
			TimeSpan timeSpan = TimeSpan.FromSeconds((double)seconds);
			string text = "";
			if (timeSpan.Hours > 0)
			{
				text += string.Format("{0:D2}{1}:", timeSpan.Hours, ScoreboardBaseVM._hourAbbrString);
			}
			text += string.Format("{0:D2}{1}:", timeSpan.Minutes, ScoreboardBaseVM._minuteAbbrString);
			return text + string.Format("{0:D2}{1}", timeSpan.Seconds, ScoreboardBaseVM._secondAbbrString);
		}

		public override void OnFinalize()
		{
			base.OnFinalize();
			InputKeyItemVM showMouseKey = this.ShowMouseKey;
			if (showMouseKey != null)
			{
				showMouseKey.OnFinalize();
			}
			InputKeyItemVM showScoreboardKey = this.ShowScoreboardKey;
			if (showScoreboardKey != null)
			{
				showScoreboardKey.OnFinalize();
			}
			InputKeyItemVM doneInputKey = this.DoneInputKey;
			if (doneInputKey != null)
			{
				doneInputKey.OnFinalize();
			}
			InputKeyItemVM fastForwardKey = this.FastForwardKey;
			if (fastForwardKey == null)
			{
				return;
			}
			fastForwardKey.OnFinalize();
		}

		public virtual void ExecuteShowScoreboardAction()
		{
			this.ShowScoreboard = !this.ShowScoreboard;
		}

		public virtual void ExecutePlayAction()
		{
		}

		public virtual void ExecuteFastForwardAction()
		{
		}

		public virtual void ExecuteEndSimulationAction()
		{
		}

		public virtual void ExecuteQuitAction()
		{
		}

		protected int MissionTimeInSeconds
		{
			get
			{
				return this._missionTimeInSeconds;
			}
			set
			{
				if (value != this._missionTimeInSeconds)
				{
					this._missionTimeInSeconds = value;
					this.MissionTimeStr = ScoreboardBaseVM.GetFormattedTimeTextFromSeconds(this._missionTimeInSeconds);
				}
			}
		}

		[DataSourceProperty]
		public string MissionTimeStr
		{
			get
			{
				return this._missionTimeStr;
			}
			set
			{
				if (value != this._missionTimeStr)
				{
					this._missionTimeStr = value;
					base.OnPropertyChangedWithValue<string>(value, "MissionTimeStr");
				}
			}
		}

		[DataSourceProperty]
		public bool IsPowerComparerEnabled
		{
			get
			{
				return this._isPowerComparerEnabled;
			}
			set
			{
				if (value != this._isPowerComparerEnabled)
				{
					this._isPowerComparerEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsPowerComparerEnabled");
				}
			}
		}

		[DataSourceProperty]
		public string QuitText
		{
			get
			{
				return this._quitText;
			}
			set
			{
				if (value != this._quitText)
				{
					this._quitText = value;
					base.OnPropertyChangedWithValue<string>(value, "QuitText");
				}
			}
		}

		[DataSourceProperty]
		public string ShowScoreboardText
		{
			get
			{
				return this._showScoreboardText;
			}
			set
			{
				if (value != this._showScoreboardText)
				{
					this._showScoreboardText = value;
					base.OnPropertyChangedWithValue<string>(value, "ShowScoreboardText");
				}
			}
		}

		[DataSourceProperty]
		public string FastForwardText
		{
			get
			{
				return this._fastForwardText;
			}
			set
			{
				if (value != this._fastForwardText)
				{
					this._fastForwardText = value;
					base.OnPropertyChangedWithValue<string>(value, "FastForwardText");
				}
			}
		}

		[DataSourceProperty]
		public SPScoreboardSideVM Attackers
		{
			get
			{
				return this._attackers;
			}
			set
			{
				if (value != this._attackers)
				{
					this._attackers = value;
					base.OnPropertyChangedWithValue<SPScoreboardSideVM>(value, "Attackers");
				}
			}
		}

		[DataSourceProperty]
		public SPScoreboardSideVM Defenders
		{
			get
			{
				return this._defenders;
			}
			set
			{
				if (value != this._defenders)
				{
					this._defenders = value;
					base.OnPropertyChangedWithValue<SPScoreboardSideVM>(value, "Defenders");
				}
			}
		}

		[DataSourceProperty]
		public HintViewModel KillHint
		{
			get
			{
				return this._killHint;
			}
			set
			{
				if (value != this._killHint)
				{
					this._killHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "KillHint");
				}
			}
		}

		[DataSourceProperty]
		public HintViewModel DeadHint
		{
			get
			{
				return this._deadHint;
			}
			set
			{
				if (value != this._deadHint)
				{
					this._deadHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "DeadHint");
				}
			}
		}

		[DataSourceProperty]
		public HintViewModel UpgradeHint
		{
			get
			{
				return this._upgradeHint;
			}
			set
			{
				if (value != this._upgradeHint)
				{
					this._upgradeHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "UpgradeHint");
				}
			}
		}

		[DataSourceProperty]
		public HintViewModel WoundedHint
		{
			get
			{
				return this._woundedHint;
			}
			set
			{
				if (value != this._woundedHint)
				{
					this._woundedHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "WoundedHint");
				}
			}
		}

		[DataSourceProperty]
		public HintViewModel RoutedHint
		{
			get
			{
				return this._routedHint;
			}
			set
			{
				if (value != this._routedHint)
				{
					this._routedHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "RoutedHint");
				}
			}
		}

		[DataSourceProperty]
		public HintViewModel RemainingHint
		{
			get
			{
				return this._remainingHint;
			}
			set
			{
				if (value != this._remainingHint)
				{
					this._remainingHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "RemainingHint");
				}
			}
		}

		[DataSourceProperty]
		public int BattleResultIndex
		{
			get
			{
				return this._battleResultIndex;
			}
			set
			{
				if (value != this._battleResultIndex)
				{
					this._battleResultIndex = value;
					base.OnPropertyChangedWithValue(value, "BattleResultIndex");
				}
			}
		}

		[DataSourceProperty]
		public string BattleResult
		{
			get
			{
				return this._battleResult;
			}
			set
			{
				if (value != this._battleResult)
				{
					this._battleResult = value;
					base.OnPropertyChangedWithValue<string>(value, "BattleResult");
				}
			}
		}

		[DataSourceProperty]
		public bool IsMouseEnabled
		{
			get
			{
				return this._isMouseEnabled;
			}
			set
			{
				if (value != this._isMouseEnabled)
				{
					this._isMouseEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsMouseEnabled");
				}
			}
		}

		[DataSourceProperty]
		public bool IsOver
		{
			get
			{
				return this._isOver;
			}
			set
			{
				if (value != this._isOver)
				{
					this._isOver = value;
					base.OnPropertyChangedWithValue(value, "IsOver");
					this.UpdateQuitText();
				}
			}
		}

		[DataSourceProperty]
		public bool IsFastForwarding
		{
			get
			{
				return this._isFastForwarding;
			}
			set
			{
				if (value != this._isFastForwarding)
				{
					this._isFastForwarding = value;
					base.OnPropertyChangedWithValue(value, "IsFastForwarding");
				}
			}
		}

		public virtual void SetShortcuts(ScoreboardHotkeys shortcuts)
		{
			this.ShowMouseKey = InputKeyItemVM.CreateFromGameKey(shortcuts.ShowMouseHotkey, false);
			this.ShowScoreboardKey = InputKeyItemVM.CreateFromGameKey(shortcuts.ShowScoreboardHotkey, false);
			this.DoneInputKey = InputKeyItemVM.CreateFromHotKey(shortcuts.DoneInputKey, true);
			this.FastForwardKey = InputKeyItemVM.CreateFromHotKey(shortcuts.FastForwardKey, true);
		}

		[DataSourceProperty]
		public bool IsMainCharacterDead
		{
			get
			{
				return this._isMainCharacterDead;
			}
			set
			{
				if (value != this._isMainCharacterDead)
				{
					this._isMainCharacterDead = value;
					base.OnPropertyChangedWithValue(value, "IsMainCharacterDead");
					this.UpdateQuitText();
				}
			}
		}

		[DataSourceProperty]
		public bool IsSimulation
		{
			get
			{
				return this._isSimulation;
			}
			set
			{
				if (value != this._isSimulation)
				{
					this._isSimulation = value;
					base.OnPropertyChangedWithValue(value, "IsSimulation");
				}
			}
		}

		[DataSourceProperty]
		public PowerLevelComparer PowerComparer
		{
			get
			{
				return this._powerComparer;
			}
			set
			{
				if (value != this._powerComparer)
				{
					this._powerComparer = value;
					base.OnPropertyChangedWithValue<PowerLevelComparer>(value, "PowerComparer");
				}
			}
		}

		[DataSourceProperty]
		public InputKeyItemVM ShowMouseKey
		{
			get
			{
				return this._showMouseKey;
			}
			set
			{
				if (value != this._showMouseKey)
				{
					this._showMouseKey = value;
					base.OnPropertyChangedWithValue<InputKeyItemVM>(value, "ShowMouseKey");
				}
			}
		}

		[DataSourceProperty]
		public bool ShowScoreboard
		{
			get
			{
				return this._showScoreboard;
			}
			set
			{
				if (value != this._showScoreboard)
				{
					this._showScoreboard = value;
					base.OnPropertyChangedWithValue(value, "ShowScoreboard");
				}
			}
		}

		[DataSourceProperty]
		public InputKeyItemVM ShowScoreboardKey
		{
			get
			{
				return this._showScoreboardKey;
			}
			set
			{
				if (value != this._showScoreboardKey)
				{
					this._showScoreboardKey = value;
					base.OnPropertyChangedWithValue<InputKeyItemVM>(value, "ShowScoreboardKey");
				}
			}
		}

		[DataSourceProperty]
		public InputKeyItemVM DoneInputKey
		{
			get
			{
				return this._doneInputKey;
			}
			set
			{
				if (value != this._doneInputKey)
				{
					this._doneInputKey = value;
					base.OnPropertyChangedWithValue<InputKeyItemVM>(value, "DoneInputKey");
				}
			}
		}

		[DataSourceProperty]
		public InputKeyItemVM FastForwardKey
		{
			get
			{
				return this._fastForwardKey;
			}
			set
			{
				if (value != this._fastForwardKey)
				{
					this._fastForwardKey = value;
					base.OnPropertyChangedWithValue<InputKeyItemVM>(value, "FastForwardKey");
				}
			}
		}

		[DataSourceProperty]
		public virtual MBBindingList<BattleResultVM> BattleResults
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		protected Action OnFastForwardIncreaseSpeed;

		protected Action OnFastForwardDecreaseSpeed;

		protected Action OnFastForwardResetSpeed;

		private static readonly TextObject _hourAbbrString = GameTexts.FindText("str_hour_abbr", null);

		private static readonly TextObject _minuteAbbrString = GameTexts.FindText("str_minute_abbr", null);

		private static readonly TextObject _secondAbbrString = GameTexts.FindText("str_second_abbr", null);

		protected BattleSideEnum PlayerSide;

		protected IMissionScreen _missionScreen;

		protected Mission _mission;

		protected BattleEndLogic _battleEndLogic;

		protected InquiryData _retreatInquiryData;

		protected Action _releaseSimulationSources;

		protected Action<bool> OnToggle;

		private ScoreboardBaseVM.MouseState _mouseState;

		protected const float MissionEndScoreboardDelayTime = 1.5f;

		private string _quitText;

		private string _showScoreboardText;

		private string _fastForwardText;

		private bool _isFastForwarding;

		private bool _isMainCharacterDead;

		private bool _showScoreboard;

		private bool _isSimulation = true;

		private bool _isMouseEnabled;

		private PowerLevelComparer _powerComparer;

		private bool _isOver;

		private string _battleResult;

		private int _battleResultIndex = -1;

		private HintViewModel _killHint;

		private HintViewModel _upgradeHint;

		private HintViewModel _deadHint;

		private HintViewModel _woundedHint;

		private HintViewModel _routedHint;

		private HintViewModel _remainingHint;

		private SPScoreboardSideVM _attackers;

		private SPScoreboardSideVM _defenders;

		private bool _isPowerComparerEnabled;

		private int _missionTimeInSeconds;

		private string _missionTimeStr;

		private InputKeyItemVM _showMouseKey;

		private InputKeyItemVM _showScoreboardKey;

		private InputKeyItemVM _doneInputKey;

		private InputKeyItemVM _fastForwardKey;

		internal enum MouseState
		{
			NotVisible,
			Visible
		}

		public enum Categories
		{
			Party,
			Tactical,
			NumOfCategories
		}

		protected enum BattleResultType
		{
			NotOver = -1,
			Defeat,
			Victory,
			Retreat
		}
	}
}
