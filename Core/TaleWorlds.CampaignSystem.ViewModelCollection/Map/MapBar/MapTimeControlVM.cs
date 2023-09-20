using System;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Map.MapBar
{
	// Token: 0x02000051 RID: 81
	public class MapTimeControlVM : ViewModel
	{
		// Token: 0x170001CE RID: 462
		// (get) Token: 0x06000645 RID: 1605 RVA: 0x0001D340 File Offset: 0x0001B540
		// (set) Token: 0x06000646 RID: 1606 RVA: 0x0001D348 File Offset: 0x0001B548
		public bool IsInBattleSimulation { get; set; }

		// Token: 0x170001CF RID: 463
		// (get) Token: 0x06000647 RID: 1607 RVA: 0x0001D351 File Offset: 0x0001B551
		// (set) Token: 0x06000648 RID: 1608 RVA: 0x0001D359 File Offset: 0x0001B559
		public bool IsInRecruitment { get; set; }

		// Token: 0x170001D0 RID: 464
		// (get) Token: 0x06000649 RID: 1609 RVA: 0x0001D362 File Offset: 0x0001B562
		// (set) Token: 0x0600064A RID: 1610 RVA: 0x0001D36A File Offset: 0x0001B56A
		public bool IsEncyclopediaOpen { get; set; }

		// Token: 0x170001D1 RID: 465
		// (get) Token: 0x0600064B RID: 1611 RVA: 0x0001D373 File Offset: 0x0001B573
		// (set) Token: 0x0600064C RID: 1612 RVA: 0x0001D37B File Offset: 0x0001B57B
		public bool IsInArmyManagement { get; set; }

		// Token: 0x170001D2 RID: 466
		// (get) Token: 0x0600064D RID: 1613 RVA: 0x0001D384 File Offset: 0x0001B584
		// (set) Token: 0x0600064E RID: 1614 RVA: 0x0001D38C File Offset: 0x0001B58C
		public bool IsInTownManagement { get; set; }

		// Token: 0x170001D3 RID: 467
		// (get) Token: 0x0600064F RID: 1615 RVA: 0x0001D395 File Offset: 0x0001B595
		// (set) Token: 0x06000650 RID: 1616 RVA: 0x0001D39D File Offset: 0x0001B59D
		public bool IsInHideoutTroopManage { get; set; }

		// Token: 0x170001D4 RID: 468
		// (get) Token: 0x06000651 RID: 1617 RVA: 0x0001D3A6 File Offset: 0x0001B5A6
		// (set) Token: 0x06000652 RID: 1618 RVA: 0x0001D3AE File Offset: 0x0001B5AE
		public bool IsInMap { get; set; }

		// Token: 0x170001D5 RID: 469
		// (get) Token: 0x06000653 RID: 1619 RVA: 0x0001D3B7 File Offset: 0x0001B5B7
		// (set) Token: 0x06000654 RID: 1620 RVA: 0x0001D3BF File Offset: 0x0001B5BF
		public bool IsInCampaignOptions { get; set; }

		// Token: 0x06000655 RID: 1621 RVA: 0x0001D3C8 File Offset: 0x0001B5C8
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

		// Token: 0x06000656 RID: 1622 RVA: 0x0001D450 File Offset: 0x0001B650
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

		// Token: 0x06000657 RID: 1623 RVA: 0x0001D57D File Offset: 0x0001B77D
		public override void OnFinalize()
		{
			base.OnFinalize();
			Input.OnGamepadActiveStateChanged = (Action)Delegate.Remove(Input.OnGamepadActiveStateChanged, new Action(this.OnGamepadActiveStateChanged));
			this._onTimeFlowStateChange = null;
			this._getMapBarShortcuts = null;
			this._onCameraReset = null;
		}

		// Token: 0x06000658 RID: 1624 RVA: 0x0001D5BA File Offset: 0x0001B7BA
		private void OnGamepadActiveStateChanged()
		{
			this.RefreshValues();
		}

		// Token: 0x06000659 RID: 1625 RVA: 0x0001D5C4 File Offset: 0x0001B7C4
		public void Tick()
		{
			this.TimeFlowState = (int)Campaign.Current.GetSimplifiedTimeControlMode();
			this.IsCurrentlyPausedOnMap = (this.TimeFlowState == 0 || this.TimeFlowState == 6) && this.IsCenterPanelEnabled;
			this.IsCenterPanelEnabled = !this.IsInBattleSimulation && !this.IsInRecruitment && !this.IsEncyclopediaOpen && !this.IsInTownManagement && !this.IsInArmyManagement && this.IsInMap && !this.IsInCampaignOptions && !this.IsInHideoutTroopManage;
		}

		// Token: 0x0600065A RID: 1626 RVA: 0x0001D64C File Offset: 0x0001B84C
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

		// Token: 0x0600065B RID: 1627 RVA: 0x0001D6D6 File Offset: 0x0001B8D6
		private void SetTimeSpeed(int speed)
		{
			Campaign.Current.SetTimeSpeed(speed);
			this._onTimeFlowStateChange();
		}

		// Token: 0x0600065C RID: 1628 RVA: 0x0001D6F0 File Offset: 0x0001B8F0
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

		// Token: 0x0600065D RID: 1629 RVA: 0x0001D774 File Offset: 0x0001B974
		public void ExecuteResetCamera()
		{
			this._onCameraReset();
		}

		// Token: 0x170001D6 RID: 470
		// (get) Token: 0x0600065E RID: 1630 RVA: 0x0001D781 File Offset: 0x0001B981
		// (set) Token: 0x0600065F RID: 1631 RVA: 0x0001D789 File Offset: 0x0001B989
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

		// Token: 0x170001D7 RID: 471
		// (get) Token: 0x06000660 RID: 1632 RVA: 0x0001D7A7 File Offset: 0x0001B9A7
		// (set) Token: 0x06000661 RID: 1633 RVA: 0x0001D7AF File Offset: 0x0001B9AF
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

		// Token: 0x170001D8 RID: 472
		// (get) Token: 0x06000662 RID: 1634 RVA: 0x0001D7CD File Offset: 0x0001B9CD
		// (set) Token: 0x06000663 RID: 1635 RVA: 0x0001D7D5 File Offset: 0x0001B9D5
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

		// Token: 0x170001D9 RID: 473
		// (get) Token: 0x06000664 RID: 1636 RVA: 0x0001D7F3 File Offset: 0x0001B9F3
		// (set) Token: 0x06000665 RID: 1637 RVA: 0x0001D7FB File Offset: 0x0001B9FB
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

		// Token: 0x170001DA RID: 474
		// (get) Token: 0x06000666 RID: 1638 RVA: 0x0001D819 File Offset: 0x0001BA19
		// (set) Token: 0x06000667 RID: 1639 RVA: 0x0001D821 File Offset: 0x0001BA21
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

		// Token: 0x170001DB RID: 475
		// (get) Token: 0x06000668 RID: 1640 RVA: 0x0001D844 File Offset: 0x0001BA44
		// (set) Token: 0x06000669 RID: 1641 RVA: 0x0001D84C File Offset: 0x0001BA4C
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

		// Token: 0x170001DC RID: 476
		// (get) Token: 0x0600066A RID: 1642 RVA: 0x0001D86F File Offset: 0x0001BA6F
		// (set) Token: 0x0600066B RID: 1643 RVA: 0x0001D877 File Offset: 0x0001BA77
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

		// Token: 0x170001DD RID: 477
		// (get) Token: 0x0600066C RID: 1644 RVA: 0x0001D895 File Offset: 0x0001BA95
		// (set) Token: 0x0600066D RID: 1645 RVA: 0x0001D89D File Offset: 0x0001BA9D
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

		// Token: 0x170001DE RID: 478
		// (get) Token: 0x0600066E RID: 1646 RVA: 0x0001D8BB File Offset: 0x0001BABB
		// (set) Token: 0x0600066F RID: 1647 RVA: 0x0001D8C3 File Offset: 0x0001BAC3
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

		// Token: 0x170001DF RID: 479
		// (get) Token: 0x06000670 RID: 1648 RVA: 0x0001D8E1 File Offset: 0x0001BAE1
		// (set) Token: 0x06000671 RID: 1649 RVA: 0x0001D8E9 File Offset: 0x0001BAE9
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

		// Token: 0x040002CC RID: 716
		private Action _onTimeFlowStateChange;

		// Token: 0x040002CD RID: 717
		private Func<MapBarShortcuts> _getMapBarShortcuts;

		// Token: 0x040002CE RID: 718
		private MapBarShortcuts _shortcuts;

		// Token: 0x040002CF RID: 719
		private Action _onCameraReset;

		// Token: 0x040002D0 RID: 720
		private CampaignTime _lastSetDate;

		// Token: 0x040002D1 RID: 721
		private int _timeFlowState = -1;

		// Token: 0x040002D2 RID: 722
		private double _time;

		// Token: 0x040002D3 RID: 723
		private string _date;

		// Token: 0x040002D4 RID: 724
		private string _pausedText;

		// Token: 0x040002D5 RID: 725
		private bool _isCurrentlyPausedOnMap;

		// Token: 0x040002D6 RID: 726
		private bool _isCenterPanelEnabled;

		// Token: 0x040002D7 RID: 727
		private BasicTooltipViewModel _pauseHint;

		// Token: 0x040002D8 RID: 728
		private BasicTooltipViewModel _playHint;

		// Token: 0x040002D9 RID: 729
		private BasicTooltipViewModel _fastForwardHint;

		// Token: 0x040002DA RID: 730
		private BasicTooltipViewModel _timeOfDayHint;
	}
}
