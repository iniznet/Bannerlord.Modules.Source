using System;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Map.MapBar
{
	public class MapTimeControlVM : ViewModel
	{
		public bool IsInBattleSimulation { get; set; }

		public bool IsInRecruitment { get; set; }

		public bool IsEncyclopediaOpen { get; set; }

		public bool IsInArmyManagement { get; set; }

		public bool IsInTownManagement { get; set; }

		public bool IsInHideoutTroopManage { get; set; }

		public bool IsInMap { get; set; }

		public bool IsInCampaignOptions { get; set; }

		public MapTimeControlVM(Func<MapBarShortcuts> getMapBarShortcuts, Action onTimeFlowStateChange, Action onCameraResetted)
		{
			this._onTimeFlowStateChange = onTimeFlowStateChange;
			this._getMapBarShortcuts = getMapBarShortcuts;
			this._onCameraReset = onCameraResetted;
			this.IsCenterPanelEnabled = false;
			this._lastSetDate = CampaignTime.Zero;
			this.PlayHint = new BasicTooltipViewModel();
			this.FastForwardHint = new BasicTooltipViewModel();
			this.PauseHint = new BasicTooltipViewModel();
			Input.OnGamepadActiveStateChanged = (Action)Delegate.Combine(Input.OnGamepadActiveStateChanged, new Action(this.OnGamepadActiveStateChanged));
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this._shortcuts = this._getMapBarShortcuts();
			if (Input.IsGamepadActive)
			{
				this.PlayHint.SetHintCallback(() => GameTexts.FindText("str_play", null).ToString());
				this.FastForwardHint.SetHintCallback(() => GameTexts.FindText("str_fast_forward", null).ToString());
				this.PauseHint.SetHintCallback(() => GameTexts.FindText("str_pause", null).ToString());
			}
			else
			{
				this.PlayHint.SetHintCallback(delegate
				{
					GameTexts.SetVariable("TEXT", GameTexts.FindText("str_play", null).ToString());
					GameTexts.SetVariable("HOTKEY", this._shortcuts.PlayHotkey);
					return GameTexts.FindText("str_hotkey_with_hint", null).ToString();
				});
				this.FastForwardHint.SetHintCallback(delegate
				{
					GameTexts.SetVariable("TEXT", GameTexts.FindText("str_fast_forward", null).ToString());
					GameTexts.SetVariable("HOTKEY", this._shortcuts.FastForwardHotkey);
					return GameTexts.FindText("str_hotkey_with_hint", null).ToString();
				});
				this.PauseHint.SetHintCallback(delegate
				{
					GameTexts.SetVariable("TEXT", GameTexts.FindText("str_pause", null).ToString());
					GameTexts.SetVariable("HOTKEY", this._shortcuts.PauseHotkey);
					return GameTexts.FindText("str_hotkey_with_hint", null).ToString();
				});
			}
			this.PausedText = GameTexts.FindText("str_paused_capital", null).ToString();
			this.Date = CampaignTime.Now.ToString();
			this._lastSetDate = CampaignTime.Now;
		}

		public override void OnFinalize()
		{
			base.OnFinalize();
			Input.OnGamepadActiveStateChanged = (Action)Delegate.Remove(Input.OnGamepadActiveStateChanged, new Action(this.OnGamepadActiveStateChanged));
			this._onTimeFlowStateChange = null;
			this._getMapBarShortcuts = null;
			this._onCameraReset = null;
		}

		private void OnGamepadActiveStateChanged()
		{
			this.RefreshValues();
		}

		public void Tick()
		{
			this.TimeFlowState = (int)Campaign.Current.GetSimplifiedTimeControlMode();
			this.IsCurrentlyPausedOnMap = (this.TimeFlowState == 0 || this.TimeFlowState == 6) && this.IsCenterPanelEnabled;
			this.IsCenterPanelEnabled = !this.IsInBattleSimulation && !this.IsInRecruitment && !this.IsEncyclopediaOpen && !this.IsInTownManagement && !this.IsInArmyManagement && this.IsInMap && !this.IsInCampaignOptions && !this.IsInHideoutTroopManage;
		}

		public void Refresh()
		{
			if (!this._lastSetDate.StringSameAs(CampaignTime.Now))
			{
				this.Date = CampaignTime.Now.ToString();
				this._lastSetDate = CampaignTime.Now;
			}
			this.Time = CampaignTime.Now.ToHours % 24.0;
			this.TimeOfDayHint = new BasicTooltipViewModel(() => CampaignUIHelper.GetTimeOfDayAndResetCameraTooltip());
		}

		private void SetTimeSpeed(int speed)
		{
			Campaign.Current.SetTimeSpeed(speed);
			this._onTimeFlowStateChange();
		}

		public void ExecuteTimeControlChange(int selectedTimeSpeed)
		{
			if (Campaign.Current.CurrentMenuContext == null || (Campaign.Current.CurrentMenuContext.GameMenu.IsWaitActive && !Campaign.Current.TimeControlModeLock))
			{
				int num = selectedTimeSpeed;
				if (this._timeFlowState == 3 && num == 2)
				{
					num = 4;
				}
				else if (this._timeFlowState == 4 && num == 1)
				{
					num = 3;
				}
				else if (this._timeFlowState == 2 && num == 0)
				{
					num = 6;
				}
				if (num != this._timeFlowState)
				{
					this.TimeFlowState = num;
					this.SetTimeSpeed(selectedTimeSpeed);
				}
			}
		}

		public void ExecuteResetCamera()
		{
			this._onCameraReset();
		}

		[DataSourceProperty]
		public BasicTooltipViewModel TimeOfDayHint
		{
			get
			{
				return this._timeOfDayHint;
			}
			set
			{
				if (value != this._timeOfDayHint)
				{
					this._timeOfDayHint = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "TimeOfDayHint");
				}
			}
		}

		[DataSourceProperty]
		public bool IsCurrentlyPausedOnMap
		{
			get
			{
				return this._isCurrentlyPausedOnMap;
			}
			set
			{
				if (value != this._isCurrentlyPausedOnMap)
				{
					this._isCurrentlyPausedOnMap = value;
					base.OnPropertyChangedWithValue(value, "IsCurrentlyPausedOnMap");
				}
			}
		}

		[DataSourceProperty]
		public bool IsCenterPanelEnabled
		{
			get
			{
				return this._isCenterPanelEnabled;
			}
			set
			{
				if (value != this._isCenterPanelEnabled)
				{
					this._isCenterPanelEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsCenterPanelEnabled");
				}
			}
		}

		[DataSourceProperty]
		public double Time
		{
			get
			{
				return this._time;
			}
			set
			{
				if (this._time != value)
				{
					this._time = value;
					base.OnPropertyChangedWithValue(value, "Time");
				}
			}
		}

		[DataSourceProperty]
		public string PausedText
		{
			get
			{
				return this._pausedText;
			}
			set
			{
				if (this._pausedText != value)
				{
					this._pausedText = value;
					base.OnPropertyChangedWithValue<string>(value, "PausedText");
				}
			}
		}

		[DataSourceProperty]
		public string Date
		{
			get
			{
				return this._date;
			}
			set
			{
				if (value != this._date)
				{
					this._date = value;
					base.OnPropertyChangedWithValue<string>(value, "Date");
				}
			}
		}

		[DataSourceProperty]
		public int TimeFlowState
		{
			get
			{
				return this._timeFlowState;
			}
			set
			{
				if (value != this._timeFlowState)
				{
					this._timeFlowState = value;
					base.OnPropertyChangedWithValue(value, "TimeFlowState");
				}
			}
		}

		[DataSourceProperty]
		public BasicTooltipViewModel PauseHint
		{
			get
			{
				return this._pauseHint;
			}
			set
			{
				if (value != this._pauseHint)
				{
					this._pauseHint = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "PauseHint");
				}
			}
		}

		[DataSourceProperty]
		public BasicTooltipViewModel PlayHint
		{
			get
			{
				return this._playHint;
			}
			set
			{
				if (value != this._playHint)
				{
					this._playHint = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "PlayHint");
				}
			}
		}

		[DataSourceProperty]
		public BasicTooltipViewModel FastForwardHint
		{
			get
			{
				return this._fastForwardHint;
			}
			set
			{
				if (value != this._fastForwardHint)
				{
					this._fastForwardHint = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "FastForwardHint");
				}
			}
		}

		private Action _onTimeFlowStateChange;

		private Func<MapBarShortcuts> _getMapBarShortcuts;

		private MapBarShortcuts _shortcuts;

		private Action _onCameraReset;

		private CampaignTime _lastSetDate;

		private int _timeFlowState = -1;

		private double _time;

		private string _date;

		private string _pausedText;

		private bool _isCurrentlyPausedOnMap;

		private bool _isCenterPanelEnabled;

		private BasicTooltipViewModel _pauseHint;

		private BasicTooltipViewModel _playHint;

		private BasicTooltipViewModel _fastForwardHint;

		private BasicTooltipViewModel _timeOfDayHint;
	}
}
