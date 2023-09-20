using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer.Scoreboard;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer.Lobby
{
	// Token: 0x0200008E RID: 142
	public class MultiplayerLobbyAfterBattleExperiencePanelWidget : Widget
	{
		// Token: 0x06000775 RID: 1909 RVA: 0x000160AF File Offset: 0x000142AF
		public MultiplayerLobbyAfterBattleExperiencePanelWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000776 RID: 1910 RVA: 0x000160B8 File Offset: 0x000142B8
		public void StartAnimation()
		{
			this.ExperienceFillBar.StartAnimation();
			this.EarnedExperienceCounterTextWidget.IntTarget = this.GainedExperience;
		}

		// Token: 0x06000777 RID: 1911 RVA: 0x000160D6 File Offset: 0x000142D6
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

		// Token: 0x06000778 RID: 1912 RVA: 0x000160FE File Offset: 0x000142FE
		private void OnFillBarFill()
		{
			this.CurrentLevelTextWidget.IntText++;
			this.NextLevelTextWidget.IntText++;
		}

		// Token: 0x06000779 RID: 1913 RVA: 0x00016126 File Offset: 0x00014326
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

		// Token: 0x170002A0 RID: 672
		// (get) Token: 0x0600077A RID: 1914 RVA: 0x00016140 File Offset: 0x00014340
		// (set) Token: 0x0600077B RID: 1915 RVA: 0x00016148 File Offset: 0x00014348
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

		// Token: 0x170002A1 RID: 673
		// (get) Token: 0x0600077C RID: 1916 RVA: 0x00016166 File Offset: 0x00014366
		// (set) Token: 0x0600077D RID: 1917 RVA: 0x00016170 File Offset: 0x00014370
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

		// Token: 0x170002A2 RID: 674
		// (get) Token: 0x0600077E RID: 1918 RVA: 0x000161DD File Offset: 0x000143DD
		// (set) Token: 0x0600077F RID: 1919 RVA: 0x000161E5 File Offset: 0x000143E5
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

		// Token: 0x170002A3 RID: 675
		// (get) Token: 0x06000780 RID: 1920 RVA: 0x00016209 File Offset: 0x00014409
		// (set) Token: 0x06000781 RID: 1921 RVA: 0x00016211 File Offset: 0x00014411
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

		// Token: 0x170002A4 RID: 676
		// (get) Token: 0x06000782 RID: 1922 RVA: 0x0001622F File Offset: 0x0001442F
		// (set) Token: 0x06000783 RID: 1923 RVA: 0x00016237 File Offset: 0x00014437
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

		// Token: 0x0400035B RID: 859
		private int _gainedExperience;

		// Token: 0x0400035C RID: 860
		private MultiplayerScoreboardAnimatedFillBarWidget _experienceFillBar;

		// Token: 0x0400035D RID: 861
		private CounterTextBrushWidget _earnedExperienceCounterTextWidget;

		// Token: 0x0400035E RID: 862
		private TextWidget _currentLevelTextWidget;

		// Token: 0x0400035F RID: 863
		private TextWidget _nextLevelTextWidget;
	}
}
