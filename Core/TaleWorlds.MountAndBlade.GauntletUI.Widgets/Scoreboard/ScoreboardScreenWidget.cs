using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.GauntletUI.ExtraWidgets;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Scoreboard
{
	public class ScoreboardScreenWidget : Widget
	{
		public ScoreboardScreenWidget(UIContext context)
			: base(context)
		{
		}

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

		private void UpdateControlButtonsPanel()
		{
			this._controlButtonsPanel.IsVisible = this.ShowScoreboard || this.IsMainCharacterDead;
			this.InputKeysPanel.IsVisible = !this.ShowScoreboard && !this.IsSimulation && this.IsMainCharacterDead;
			this.ShowMouseIconWidget.IsVisible = !this.IsMouseEnabled && this.ShowScoreboard && !this.IsSimulation && !this.IsMainCharacterDead;
		}

		private void UpdateControlButtons()
		{
			this._fastForwardWidget.IsVisible = (this.IsMainCharacterDead || this.IsSimulation) && this.ShowScoreboard;
			this._quitButton.IsVisible = this.ShowScoreboard;
		}

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

		private bool _isAnimationActive;

		private bool _showScoreboard;

		private bool _isOver;

		private int _battleResult;

		private bool _isMainCharacterDead;

		private bool _isSimulation;

		private bool _isMouseEnabled;

		private ScrollablePanel _scrollablePanel;

		private Widget _scrollGradient;

		private Widget _controlButtonsPanel;

		private Widget _showMouseIconWidget;

		private ListPanel _inputKeysPanel;

		private Widget _fastForwardWidget;

		private ButtonWidget _showScoreboardToggle;

		private ButtonWidget _quitButton;

		private ScoreboardBattleRewardsWidget _battleRewardsWidget;

		private DelayedStateChanger _flagsSuccess;

		private DelayedStateChanger _flagsDefeat;

		private DelayedStateChanger _flagsRetreat;

		private DelayedStateChanger _shieldStateChanger;

		private DelayedStateChanger _titleStateChanger;

		private DelayedStateChanger _titleBackgroundStateChanger;
	}
}
