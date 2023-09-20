using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.GauntletUI.ExtraWidgets;
using TaleWorlds.MountAndBlade.GauntletUI.Widgets.Scoreboard;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Tournament
{
	public class TournamentScreenWidget : Widget
	{
		public TournamentScreenWidget(UIContext context)
			: base(context)
		{
		}

		protected override void OnUpdate(float dt)
		{
			base.OnUpdate(dt);
			if (!this._isAnimationActive && this.IsOver)
			{
				this.StartBattleResultAnimation();
			}
		}

		private void StartBattleResultAnimation()
		{
			this._isAnimationActive = true;
			DelayedStateChanger shieldStateChanger = this.ShieldStateChanger;
			if (shieldStateChanger != null)
			{
				shieldStateChanger.Start();
			}
			DelayedStateChanger winnerTextContainer = this.WinnerTextContainer1;
			if (winnerTextContainer != null)
			{
				winnerTextContainer.Start();
			}
			DelayedStateChanger characterContainer = this.CharacterContainer;
			if (characterContainer != null)
			{
				characterContainer.Start();
			}
			DelayedStateChanger rewardsContainer = this.RewardsContainer;
			if (rewardsContainer != null)
			{
				rewardsContainer.Start();
			}
			DelayedStateChanger flagsSuccess = this.FlagsSuccess;
			if (flagsSuccess != null)
			{
				flagsSuccess.Start();
			}
			ScoreboardBattleRewardsWidget scoreboardBattleRewardsWidget = this.ScoreboardBattleRewardsWidget;
			if (scoreboardBattleRewardsWidget == null)
			{
				return;
			}
			scoreboardBattleRewardsWidget.StartAnimation();
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
		public DelayedStateChanger WinnerTextContainer1
		{
			get
			{
				return this._winnerTextContainer1;
			}
			set
			{
				if (this._winnerTextContainer1 != value)
				{
					this._winnerTextContainer1 = value;
					base.OnPropertyChanged<DelayedStateChanger>(value, "WinnerTextContainer1");
				}
			}
		}

		[Editor(false)]
		public DelayedStateChanger CharacterContainer
		{
			get
			{
				return this._characterContainer;
			}
			set
			{
				if (this._characterContainer != value)
				{
					this._characterContainer = value;
					base.OnPropertyChanged<DelayedStateChanger>(value, "CharacterContainer");
				}
			}
		}

		[Editor(false)]
		public DelayedStateChanger RewardsContainer
		{
			get
			{
				return this._rewardsContainer;
			}
			set
			{
				if (this._rewardsContainer != value)
				{
					this._rewardsContainer = value;
					base.OnPropertyChanged<DelayedStateChanger>(value, "RewardsContainer");
				}
			}
		}

		[Editor(false)]
		public ScoreboardBattleRewardsWidget ScoreboardBattleRewardsWidget
		{
			get
			{
				return this._scoreboardBattleRewardsWidget;
			}
			set
			{
				if (this._scoreboardBattleRewardsWidget != value)
				{
					this._scoreboardBattleRewardsWidget = value;
					base.OnPropertyChanged<ScoreboardBattleRewardsWidget>(value, "ScoreboardBattleRewardsWidget");
				}
			}
		}

		private bool _isAnimationActive;

		private bool _isOver;

		private DelayedStateChanger _flagsSuccess;

		private DelayedStateChanger _shieldStateChanger;

		private DelayedStateChanger _winnerTextContainer1;

		private DelayedStateChanger _characterContainer;

		private DelayedStateChanger _rewardsContainer;

		private ScoreboardBattleRewardsWidget _scoreboardBattleRewardsWidget;
	}
}
