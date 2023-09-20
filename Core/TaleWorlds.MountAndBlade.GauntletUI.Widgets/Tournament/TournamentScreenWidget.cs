using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.GauntletUI.ExtraWidgets;
using TaleWorlds.MountAndBlade.GauntletUI.Widgets.Scoreboard;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Tournament
{
	// Token: 0x02000048 RID: 72
	public class TournamentScreenWidget : Widget
	{
		// Token: 0x060003CC RID: 972 RVA: 0x0000C96A File Offset: 0x0000AB6A
		public TournamentScreenWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x060003CD RID: 973 RVA: 0x0000C973 File Offset: 0x0000AB73
		protected override void OnUpdate(float dt)
		{
			base.OnUpdate(dt);
			if (!this._isAnimationActive && this.IsOver)
			{
				this.StartBattleResultAnimation();
			}
		}

		// Token: 0x060003CE RID: 974 RVA: 0x0000C994 File Offset: 0x0000AB94
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

		// Token: 0x17000153 RID: 339
		// (get) Token: 0x060003CF RID: 975 RVA: 0x0000CA0D File Offset: 0x0000AC0D
		// (set) Token: 0x060003D0 RID: 976 RVA: 0x0000CA15 File Offset: 0x0000AC15
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

		// Token: 0x17000154 RID: 340
		// (get) Token: 0x060003D1 RID: 977 RVA: 0x0000CA33 File Offset: 0x0000AC33
		// (set) Token: 0x060003D2 RID: 978 RVA: 0x0000CA3B File Offset: 0x0000AC3B
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

		// Token: 0x17000155 RID: 341
		// (get) Token: 0x060003D3 RID: 979 RVA: 0x0000CA59 File Offset: 0x0000AC59
		// (set) Token: 0x060003D4 RID: 980 RVA: 0x0000CA61 File Offset: 0x0000AC61
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

		// Token: 0x17000156 RID: 342
		// (get) Token: 0x060003D5 RID: 981 RVA: 0x0000CA7F File Offset: 0x0000AC7F
		// (set) Token: 0x060003D6 RID: 982 RVA: 0x0000CA87 File Offset: 0x0000AC87
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

		// Token: 0x17000157 RID: 343
		// (get) Token: 0x060003D7 RID: 983 RVA: 0x0000CAA5 File Offset: 0x0000ACA5
		// (set) Token: 0x060003D8 RID: 984 RVA: 0x0000CAAD File Offset: 0x0000ACAD
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

		// Token: 0x17000158 RID: 344
		// (get) Token: 0x060003D9 RID: 985 RVA: 0x0000CACB File Offset: 0x0000ACCB
		// (set) Token: 0x060003DA RID: 986 RVA: 0x0000CAD3 File Offset: 0x0000ACD3
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

		// Token: 0x17000159 RID: 345
		// (get) Token: 0x060003DB RID: 987 RVA: 0x0000CAF1 File Offset: 0x0000ACF1
		// (set) Token: 0x060003DC RID: 988 RVA: 0x0000CAF9 File Offset: 0x0000ACF9
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

		// Token: 0x040001A5 RID: 421
		private bool _isAnimationActive;

		// Token: 0x040001A6 RID: 422
		private bool _isOver;

		// Token: 0x040001A7 RID: 423
		private DelayedStateChanger _flagsSuccess;

		// Token: 0x040001A8 RID: 424
		private DelayedStateChanger _shieldStateChanger;

		// Token: 0x040001A9 RID: 425
		private DelayedStateChanger _winnerTextContainer1;

		// Token: 0x040001AA RID: 426
		private DelayedStateChanger _characterContainer;

		// Token: 0x040001AB RID: 427
		private DelayedStateChanger _rewardsContainer;

		// Token: 0x040001AC RID: 428
		private ScoreboardBattleRewardsWidget _scoreboardBattleRewardsWidget;
	}
}
