using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.GauntletUI.ExtraWidgets;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Scoreboard
{
	// Token: 0x0200004C RID: 76
	public class ScoreboardScreenWidget : Widget
	{
		// Token: 0x060003F8 RID: 1016 RVA: 0x0000D0C4 File Offset: 0x0000B2C4
		public ScoreboardScreenWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x060003F9 RID: 1017 RVA: 0x0000D0D0 File Offset: 0x0000B2D0
		protected override void OnUpdate(float dt)
		{
			base.OnUpdate(dt);
			if (this.ScrollablePanel != null && this.ScrollGradient != null)
			{
				this.ScrollGradient.IsVisible = this.ScrollablePanel.InnerPanel.Size.Y > this.ScrollablePanel.ClipRect.Size.Y;
			}
			if (!this._isAnimationActive && this.ShowScoreboard && this.IsOver)
			{
				this.StartBattleResultAnimation();
			}
		}

		// Token: 0x060003FA RID: 1018 RVA: 0x0000D14C File Offset: 0x0000B34C
		private void UpdateControlButtonsPanel()
		{
			this._controlButtonsPanel.IsVisible = this.ShowScoreboard || this.IsMainCharacterDead;
			this.InputKeysPanel.IsVisible = !this.ShowScoreboard && !this.IsSimulation && this.IsMainCharacterDead;
			this.ShowMouseIconWidget.IsVisible = !this.IsMouseEnabled && this.ShowScoreboard && !this.IsSimulation && !this.IsMainCharacterDead;
		}

		// Token: 0x060003FB RID: 1019 RVA: 0x0000D1C8 File Offset: 0x0000B3C8
		private void UpdateControlButtons()
		{
			this._fastForwardWidget.IsVisible = (this.IsMainCharacterDead || this.IsSimulation) && this.ShowScoreboard;
			this._quitButton.IsVisible = this.ShowScoreboard;
		}

		// Token: 0x060003FC RID: 1020 RVA: 0x0000D200 File Offset: 0x0000B400
		private void StartBattleResultAnimation()
		{
			this._isAnimationActive = true;
			ScoreboardBattleRewardsWidget battleRewardsWidget = this.BattleRewardsWidget;
			if (battleRewardsWidget != null)
			{
				battleRewardsWidget.StartAnimation();
			}
			DelayedStateChanger shieldStateChanger = this.ShieldStateChanger;
			if (shieldStateChanger != null)
			{
				shieldStateChanger.Start();
			}
			DelayedStateChanger titleStateChanger = this.TitleStateChanger;
			if (titleStateChanger != null)
			{
				titleStateChanger.Start();
			}
			DelayedStateChanger titleBackgroundStateChanger = this.TitleBackgroundStateChanger;
			if (titleBackgroundStateChanger != null)
			{
				titleBackgroundStateChanger.Start();
			}
			if (this.BattleResult == 0)
			{
				DelayedStateChanger flagsDefeat = this.FlagsDefeat;
				if (flagsDefeat == null)
				{
					return;
				}
				flagsDefeat.Start();
				return;
			}
			else
			{
				if (this.BattleResult != 1)
				{
					if (this.BattleResult == 2)
					{
						DelayedStateChanger flagsRetreat = this.FlagsRetreat;
						if (flagsRetreat == null)
						{
							return;
						}
						flagsRetreat.Start();
					}
					return;
				}
				DelayedStateChanger flagsSuccess = this.FlagsSuccess;
				if (flagsSuccess == null)
				{
					return;
				}
				flagsSuccess.Start();
				return;
			}
		}

		// Token: 0x17000161 RID: 353
		// (get) Token: 0x060003FD RID: 1021 RVA: 0x0000D2A4 File Offset: 0x0000B4A4
		// (set) Token: 0x060003FE RID: 1022 RVA: 0x0000D2AC File Offset: 0x0000B4AC
		[Editor(false)]
		public bool ShowScoreboard
		{
			get
			{
				return this._showScoreboard;
			}
			set
			{
				if (this._showScoreboard != value)
				{
					this._showScoreboard = value;
					base.OnPropertyChanged(value, "ShowScoreboard");
					this.UpdateControlButtonsPanel();
					this.UpdateControlButtons();
				}
			}
		}

		// Token: 0x17000162 RID: 354
		// (get) Token: 0x060003FF RID: 1023 RVA: 0x0000D2D6 File Offset: 0x0000B4D6
		// (set) Token: 0x06000400 RID: 1024 RVA: 0x0000D2DE File Offset: 0x0000B4DE
		[Editor(false)]
		public bool IsOver
		{
			get
			{
				return this._isOver;
			}
			set
			{
				if (this._isOver != value)
				{
					this._isOver = value;
					base.OnPropertyChanged(value, "IsOver");
					this.UpdateControlButtons();
				}
			}
		}

		// Token: 0x17000163 RID: 355
		// (get) Token: 0x06000401 RID: 1025 RVA: 0x0000D302 File Offset: 0x0000B502
		// (set) Token: 0x06000402 RID: 1026 RVA: 0x0000D30A File Offset: 0x0000B50A
		[Editor(false)]
		public int BattleResult
		{
			get
			{
				return this._battleResult;
			}
			set
			{
				if (this._battleResult != value)
				{
					this._battleResult = value;
					base.OnPropertyChanged(value, "BattleResult");
				}
			}
		}

		// Token: 0x17000164 RID: 356
		// (get) Token: 0x06000403 RID: 1027 RVA: 0x0000D328 File Offset: 0x0000B528
		// (set) Token: 0x06000404 RID: 1028 RVA: 0x0000D330 File Offset: 0x0000B530
		[Editor(false)]
		public bool IsMainCharacterDead
		{
			get
			{
				return this._isMainCharacterDead;
			}
			set
			{
				if (this._isMainCharacterDead != value)
				{
					this._isMainCharacterDead = value;
					base.OnPropertyChanged(value, "IsMainCharacterDead");
					this.UpdateControlButtonsPanel();
					this.UpdateControlButtons();
				}
			}
		}

		// Token: 0x17000165 RID: 357
		// (get) Token: 0x06000405 RID: 1029 RVA: 0x0000D35A File Offset: 0x0000B55A
		// (set) Token: 0x06000406 RID: 1030 RVA: 0x0000D362 File Offset: 0x0000B562
		[Editor(false)]
		public bool IsSimulation
		{
			get
			{
				return this._isSimulation;
			}
			set
			{
				if (this._isSimulation != value)
				{
					this._isSimulation = value;
					base.OnPropertyChanged(value, "IsSimulation");
					this.UpdateControlButtonsPanel();
					this.UpdateControlButtons();
				}
			}
		}

		// Token: 0x17000166 RID: 358
		// (get) Token: 0x06000407 RID: 1031 RVA: 0x0000D38C File Offset: 0x0000B58C
		// (set) Token: 0x06000408 RID: 1032 RVA: 0x0000D394 File Offset: 0x0000B594
		[Editor(false)]
		public bool IsMouseEnabled
		{
			get
			{
				return this._isMouseEnabled;
			}
			set
			{
				if (this._isMouseEnabled != value)
				{
					this._isMouseEnabled = value;
					base.OnPropertyChanged(value, "IsMouseEnabled");
					this.UpdateControlButtonsPanel();
				}
			}
		}

		// Token: 0x17000167 RID: 359
		// (get) Token: 0x06000409 RID: 1033 RVA: 0x0000D3B8 File Offset: 0x0000B5B8
		// (set) Token: 0x0600040A RID: 1034 RVA: 0x0000D3C0 File Offset: 0x0000B5C0
		[Editor(false)]
		public ScrollablePanel ScrollablePanel
		{
			get
			{
				return this._scrollablePanel;
			}
			set
			{
				if (this._scrollablePanel != value)
				{
					this._scrollablePanel = value;
					base.OnPropertyChanged<ScrollablePanel>(value, "ScrollablePanel");
				}
			}
		}

		// Token: 0x17000168 RID: 360
		// (get) Token: 0x0600040B RID: 1035 RVA: 0x0000D3DE File Offset: 0x0000B5DE
		// (set) Token: 0x0600040C RID: 1036 RVA: 0x0000D3E6 File Offset: 0x0000B5E6
		[Editor(false)]
		public Widget ScrollGradient
		{
			get
			{
				return this._scrollGradient;
			}
			set
			{
				if (this._scrollGradient != value)
				{
					this._scrollGradient = value;
					base.OnPropertyChanged<Widget>(value, "ScrollGradient");
				}
			}
		}

		// Token: 0x17000169 RID: 361
		// (get) Token: 0x0600040D RID: 1037 RVA: 0x0000D404 File Offset: 0x0000B604
		// (set) Token: 0x0600040E RID: 1038 RVA: 0x0000D40C File Offset: 0x0000B60C
		[Editor(false)]
		public Widget ControlButtonsPanel
		{
			get
			{
				return this._controlButtonsPanel;
			}
			set
			{
				if (this._controlButtonsPanel != value)
				{
					this._controlButtonsPanel = value;
					base.OnPropertyChanged<Widget>(value, "ControlButtonsPanel");
				}
			}
		}

		// Token: 0x1700016A RID: 362
		// (get) Token: 0x0600040F RID: 1039 RVA: 0x0000D42A File Offset: 0x0000B62A
		// (set) Token: 0x06000410 RID: 1040 RVA: 0x0000D432 File Offset: 0x0000B632
		[Editor(false)]
		public ListPanel InputKeysPanel
		{
			get
			{
				return this._inputKeysPanel;
			}
			set
			{
				if (this._inputKeysPanel != value)
				{
					this._inputKeysPanel = value;
					base.OnPropertyChanged<ListPanel>(value, "InputKeysPanel");
				}
			}
		}

		// Token: 0x1700016B RID: 363
		// (get) Token: 0x06000411 RID: 1041 RVA: 0x0000D450 File Offset: 0x0000B650
		// (set) Token: 0x06000412 RID: 1042 RVA: 0x0000D458 File Offset: 0x0000B658
		[Editor(false)]
		public Widget ShowMouseIconWidget
		{
			get
			{
				return this._showMouseIconWidget;
			}
			set
			{
				if (this._showMouseIconWidget != value)
				{
					this._showMouseIconWidget = value;
					base.OnPropertyChanged<Widget>(value, "ShowMouseIconWidget");
				}
			}
		}

		// Token: 0x1700016C RID: 364
		// (get) Token: 0x06000413 RID: 1043 RVA: 0x0000D476 File Offset: 0x0000B676
		// (set) Token: 0x06000414 RID: 1044 RVA: 0x0000D47E File Offset: 0x0000B67E
		[Editor(false)]
		public Widget FastForwardWidget
		{
			get
			{
				return this._fastForwardWidget;
			}
			set
			{
				if (this._fastForwardWidget != value)
				{
					this._fastForwardWidget = value;
					base.OnPropertyChanged<Widget>(value, "FastForwardWidget");
				}
			}
		}

		// Token: 0x1700016D RID: 365
		// (get) Token: 0x06000415 RID: 1045 RVA: 0x0000D49C File Offset: 0x0000B69C
		// (set) Token: 0x06000416 RID: 1046 RVA: 0x0000D4A4 File Offset: 0x0000B6A4
		[Editor(false)]
		public ButtonWidget QuitButton
		{
			get
			{
				return this._quitButton;
			}
			set
			{
				if (this._quitButton != value)
				{
					this._quitButton = value;
					base.OnPropertyChanged<ButtonWidget>(value, "QuitButton");
				}
			}
		}

		// Token: 0x1700016E RID: 366
		// (get) Token: 0x06000417 RID: 1047 RVA: 0x0000D4C2 File Offset: 0x0000B6C2
		// (set) Token: 0x06000418 RID: 1048 RVA: 0x0000D4CA File Offset: 0x0000B6CA
		[Editor(false)]
		public ButtonWidget ShowScoreboardToggle
		{
			get
			{
				return this._showScoreboardToggle;
			}
			set
			{
				if (this._showScoreboardToggle != value)
				{
					this._showScoreboardToggle = value;
					base.OnPropertyChanged<ButtonWidget>(value, "ShowScoreboardToggle");
				}
			}
		}

		// Token: 0x1700016F RID: 367
		// (get) Token: 0x06000419 RID: 1049 RVA: 0x0000D4E8 File Offset: 0x0000B6E8
		// (set) Token: 0x0600041A RID: 1050 RVA: 0x0000D4F0 File Offset: 0x0000B6F0
		[Editor(false)]
		public ScoreboardBattleRewardsWidget BattleRewardsWidget
		{
			get
			{
				return this._battleRewardsWidget;
			}
			set
			{
				if (this._battleRewardsWidget != value)
				{
					this._battleRewardsWidget = value;
					base.OnPropertyChanged<ScoreboardBattleRewardsWidget>(value, "BattleRewardsWidget");
				}
			}
		}

		// Token: 0x17000170 RID: 368
		// (get) Token: 0x0600041B RID: 1051 RVA: 0x0000D50E File Offset: 0x0000B70E
		// (set) Token: 0x0600041C RID: 1052 RVA: 0x0000D516 File Offset: 0x0000B716
		[Editor(false)]
		public DelayedStateChanger FlagsSuccess
		{
			get
			{
				return this._flagsSuccess;
			}
			set
			{
				if (this._flagsSuccess != value)
				{
					this._flagsSuccess = value;
					base.OnPropertyChanged<DelayedStateChanger>(value, "FlagsSuccess");
				}
			}
		}

		// Token: 0x17000171 RID: 369
		// (get) Token: 0x0600041D RID: 1053 RVA: 0x0000D534 File Offset: 0x0000B734
		// (set) Token: 0x0600041E RID: 1054 RVA: 0x0000D53C File Offset: 0x0000B73C
		[Editor(false)]
		public DelayedStateChanger FlagsRetreat
		{
			get
			{
				return this._flagsRetreat;
			}
			set
			{
				if (this._flagsRetreat != value)
				{
					this._flagsRetreat = value;
					base.OnPropertyChanged<DelayedStateChanger>(value, "FlagsRetreat");
				}
			}
		}

		// Token: 0x17000172 RID: 370
		// (get) Token: 0x0600041F RID: 1055 RVA: 0x0000D55A File Offset: 0x0000B75A
		// (set) Token: 0x06000420 RID: 1056 RVA: 0x0000D562 File Offset: 0x0000B762
		[Editor(false)]
		public DelayedStateChanger FlagsDefeat
		{
			get
			{
				return this._flagsDefeat;
			}
			set
			{
				if (this._flagsDefeat != value)
				{
					this._flagsDefeat = value;
					base.OnPropertyChanged<DelayedStateChanger>(value, "FlagsDefeat");
				}
			}
		}

		// Token: 0x17000173 RID: 371
		// (get) Token: 0x06000421 RID: 1057 RVA: 0x0000D580 File Offset: 0x0000B780
		// (set) Token: 0x06000422 RID: 1058 RVA: 0x0000D588 File Offset: 0x0000B788
		[Editor(false)]
		public DelayedStateChanger ShieldStateChanger
		{
			get
			{
				return this._shieldStateChanger;
			}
			set
			{
				if (this._shieldStateChanger != value)
				{
					this._shieldStateChanger = value;
					base.OnPropertyChanged<DelayedStateChanger>(value, "ShieldStateChanger");
				}
			}
		}

		// Token: 0x17000174 RID: 372
		// (get) Token: 0x06000423 RID: 1059 RVA: 0x0000D5A6 File Offset: 0x0000B7A6
		// (set) Token: 0x06000424 RID: 1060 RVA: 0x0000D5AE File Offset: 0x0000B7AE
		[Editor(false)]
		public DelayedStateChanger TitleStateChanger
		{
			get
			{
				return this._titleStateChanger;
			}
			set
			{
				if (this._titleStateChanger != value)
				{
					this._titleStateChanger = value;
					base.OnPropertyChanged<DelayedStateChanger>(value, "TitleStateChanger");
				}
			}
		}

		// Token: 0x17000175 RID: 373
		// (get) Token: 0x06000425 RID: 1061 RVA: 0x0000D5CC File Offset: 0x0000B7CC
		// (set) Token: 0x06000426 RID: 1062 RVA: 0x0000D5D4 File Offset: 0x0000B7D4
		[Editor(false)]
		public DelayedStateChanger TitleBackgroundStateChanger
		{
			get
			{
				return this._titleBackgroundStateChanger;
			}
			set
			{
				if (this._titleBackgroundStateChanger != value)
				{
					this._titleBackgroundStateChanger = value;
					base.OnPropertyChanged<DelayedStateChanger>(value, "TitleBackgroundStateChanger");
				}
			}
		}

		// Token: 0x040001B9 RID: 441
		private bool _isAnimationActive;

		// Token: 0x040001BA RID: 442
		private bool _showScoreboard;

		// Token: 0x040001BB RID: 443
		private bool _isOver;

		// Token: 0x040001BC RID: 444
		private int _battleResult;

		// Token: 0x040001BD RID: 445
		private bool _isMainCharacterDead;

		// Token: 0x040001BE RID: 446
		private bool _isSimulation;

		// Token: 0x040001BF RID: 447
		private bool _isMouseEnabled;

		// Token: 0x040001C0 RID: 448
		private ScrollablePanel _scrollablePanel;

		// Token: 0x040001C1 RID: 449
		private Widget _scrollGradient;

		// Token: 0x040001C2 RID: 450
		private Widget _controlButtonsPanel;

		// Token: 0x040001C3 RID: 451
		private Widget _showMouseIconWidget;

		// Token: 0x040001C4 RID: 452
		private ListPanel _inputKeysPanel;

		// Token: 0x040001C5 RID: 453
		private Widget _fastForwardWidget;

		// Token: 0x040001C6 RID: 454
		private ButtonWidget _showScoreboardToggle;

		// Token: 0x040001C7 RID: 455
		private ButtonWidget _quitButton;

		// Token: 0x040001C8 RID: 456
		private ScoreboardBattleRewardsWidget _battleRewardsWidget;

		// Token: 0x040001C9 RID: 457
		private DelayedStateChanger _flagsSuccess;

		// Token: 0x040001CA RID: 458
		private DelayedStateChanger _flagsDefeat;

		// Token: 0x040001CB RID: 459
		private DelayedStateChanger _flagsRetreat;

		// Token: 0x040001CC RID: 460
		private DelayedStateChanger _shieldStateChanger;

		// Token: 0x040001CD RID: 461
		private DelayedStateChanger _titleStateChanger;

		// Token: 0x040001CE RID: 462
		private DelayedStateChanger _titleBackgroundStateChanger;
	}
}
