using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer.Scoreboard;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer.Lobby
{
	public class MultiplayerLobbyAfterBattleExperiencePanelWidget : Widget
	{
		public MultiplayerLobbyAfterBattleExperiencePanelWidget(UIContext context)
			: base(context)
		{
		}

		public void StartAnimation()
		{
			this.ExperienceFillBar.StartAnimation();
			this.EarnedExperienceCounterTextWidget.IntTarget = this.GainedExperience;
		}

		public void Reset()
		{
			MultiplayerScoreboardAnimatedFillBarWidget experienceFillBar = this.ExperienceFillBar;
			if (experienceFillBar != null)
			{
				experienceFillBar.Reset();
			}
			CounterTextBrushWidget earnedExperienceCounterTextWidget = this.EarnedExperienceCounterTextWidget;
			if (earnedExperienceCounterTextWidget == null)
			{
				return;
			}
			earnedExperienceCounterTextWidget.SetInitialValue(0f);
		}

		private void OnFillBarFill()
		{
			this.CurrentLevelTextWidget.IntText++;
			this.NextLevelTextWidget.IntText++;
		}

		protected override void RefreshState()
		{
			if (base.IsHidden)
			{
				MultiplayerScoreboardAnimatedFillBarWidget experienceFillBar = this.ExperienceFillBar;
				if (experienceFillBar == null)
				{
					return;
				}
				experienceFillBar.Reset();
			}
		}

		[Editor(false)]
		public int GainedExperience
		{
			get
			{
				return this._gainedExperience;
			}
			set
			{
				if (value != this._gainedExperience)
				{
					this._gainedExperience = value;
					base.OnPropertyChanged(value, "GainedExperience");
				}
			}
		}

		[Editor(false)]
		public MultiplayerScoreboardAnimatedFillBarWidget ExperienceFillBar
		{
			get
			{
				return this._experienceFillBar;
			}
			set
			{
				if (value != this._experienceFillBar)
				{
					if (this._experienceFillBar != null)
					{
						this._experienceFillBar.OnFullFillFinished -= this.OnFillBarFill;
					}
					this._experienceFillBar = value;
					if (this._experienceFillBar != null)
					{
						this._experienceFillBar.OnFullFillFinished += this.OnFillBarFill;
					}
					base.OnPropertyChanged<MultiplayerScoreboardAnimatedFillBarWidget>(value, "ExperienceFillBar");
					this.Reset();
				}
			}
		}

		[Editor(false)]
		public CounterTextBrushWidget EarnedExperienceCounterTextWidget
		{
			get
			{
				return this._earnedExperienceCounterTextWidget;
			}
			set
			{
				if (value != this._earnedExperienceCounterTextWidget)
				{
					this._earnedExperienceCounterTextWidget = value;
					base.OnPropertyChanged<CounterTextBrushWidget>(value, "EarnedExperienceCounterTextWidget");
					this.Reset();
				}
			}
		}

		[Editor(false)]
		public TextWidget CurrentLevelTextWidget
		{
			get
			{
				return this._currentLevelTextWidget;
			}
			set
			{
				if (value != this._currentLevelTextWidget)
				{
					this._currentLevelTextWidget = value;
					base.OnPropertyChanged<TextWidget>(value, "CurrentLevelTextWidget");
				}
			}
		}

		public TextWidget NextLevelTextWidget
		{
			get
			{
				return this._nextLevelTextWidget;
			}
			set
			{
				if (value != this._nextLevelTextWidget)
				{
					this._nextLevelTextWidget = value;
					base.OnPropertyChanged<TextWidget>(value, "NextLevelTextWidget");
				}
			}
		}

		private int _gainedExperience;

		private MultiplayerScoreboardAnimatedFillBarWidget _experienceFillBar;

		private CounterTextBrushWidget _earnedExperienceCounterTextWidget;

		private TextWidget _currentLevelTextWidget;

		private TextWidget _nextLevelTextWidget;
	}
}
