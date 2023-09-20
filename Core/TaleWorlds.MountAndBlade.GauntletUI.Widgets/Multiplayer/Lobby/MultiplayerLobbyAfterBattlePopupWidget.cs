using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer.Lobby
{
	// Token: 0x0200008F RID: 143
	public class MultiplayerLobbyAfterBattlePopupWidget : Widget
	{
		// Token: 0x06000784 RID: 1924 RVA: 0x00016255 File Offset: 0x00014455
		public MultiplayerLobbyAfterBattlePopupWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000785 RID: 1925 RVA: 0x00016260 File Offset: 0x00014460
		protected override void OnUpdate(float dt)
		{
			base.OnUpdate(dt);
			this.IsActive = base.IsVisible;
			if (this._isActive)
			{
				this._timePassed += dt;
			}
			if (this._isFinished)
			{
				return;
			}
			if (this._timePassed >= this.AnimationDuration + this.AnimationDelay + (float)this._currentRewardIndex * this.RewardRevealDuration && this._currentRewardIndex < this.RewardsListPanel.Children.Count)
			{
				(this.RewardsListPanel.Children[this._currentRewardIndex] as MultiplayerLobbyBattleRewardWidget).StartAnimation();
				this._currentRewardIndex++;
			}
			if (this._timePassed >= this.AnimationDelay + this.AnimationDuration + (float)this.RewardsListPanel.Children.Count * this.RewardRevealDuration)
			{
				this._isFinished = true;
				this.ClickToContinueTextWidget.IsVisible = true;
			}
			if (!this._isStarted)
			{
				return;
			}
			if (this._timePassed >= this.AnimationDelay)
			{
				this._isStarted = false;
				this.ExperiencePanel.StartAnimation();
			}
		}

		// Token: 0x06000786 RID: 1926 RVA: 0x00016374 File Offset: 0x00014574
		public void StartAnimation()
		{
			foreach (Widget widget in this.RewardsListPanel.Children)
			{
				(widget as MultiplayerLobbyBattleRewardWidget).StartPreAnimation();
			}
			this._isStarted = true;
			this._isFinished = false;
			this._timePassed = 0f;
			this._currentRewardIndex = 0;
			this.ClickToContinueTextWidget.IsVisible = false;
		}

		// Token: 0x06000787 RID: 1927 RVA: 0x000163FC File Offset: 0x000145FC
		private void Reset()
		{
			this.ExperiencePanel.Reset();
		}

		// Token: 0x06000788 RID: 1928 RVA: 0x00016409 File Offset: 0x00014609
		private void IsActiveUpdated()
		{
			if (this.IsActive)
			{
				this.StartAnimation();
				return;
			}
			this.Reset();
		}

		// Token: 0x170002A5 RID: 677
		// (get) Token: 0x06000789 RID: 1929 RVA: 0x00016420 File Offset: 0x00014620
		// (set) Token: 0x0600078A RID: 1930 RVA: 0x00016428 File Offset: 0x00014628
		[Editor(false)]
		public bool IsActive
		{
			get
			{
				return this._isActive;
			}
			set
			{
				if (value != this._isActive)
				{
					this._isActive = value;
					base.OnPropertyChanged(value, "IsActive");
					this.IsActiveUpdated();
				}
			}
		}

		// Token: 0x170002A6 RID: 678
		// (get) Token: 0x0600078B RID: 1931 RVA: 0x0001644C File Offset: 0x0001464C
		// (set) Token: 0x0600078C RID: 1932 RVA: 0x00016454 File Offset: 0x00014654
		[Editor(false)]
		public float AnimationDelay
		{
			get
			{
				return this._animationDelay;
			}
			set
			{
				if (value != this._animationDelay)
				{
					this._animationDelay = value;
					base.OnPropertyChanged(value, "AnimationDelay");
				}
			}
		}

		// Token: 0x170002A7 RID: 679
		// (get) Token: 0x0600078D RID: 1933 RVA: 0x00016472 File Offset: 0x00014672
		// (set) Token: 0x0600078E RID: 1934 RVA: 0x0001647A File Offset: 0x0001467A
		[Editor(false)]
		public float AnimationDuration
		{
			get
			{
				return this._animationDuration;
			}
			set
			{
				if (value != this._animationDuration)
				{
					this._animationDuration = value;
					base.OnPropertyChanged(value, "AnimationDuration");
				}
			}
		}

		// Token: 0x170002A8 RID: 680
		// (get) Token: 0x0600078F RID: 1935 RVA: 0x00016498 File Offset: 0x00014698
		// (set) Token: 0x06000790 RID: 1936 RVA: 0x000164A0 File Offset: 0x000146A0
		[Editor(false)]
		public float RewardRevealDuration
		{
			get
			{
				return this._rewardRevealDuration;
			}
			set
			{
				if (value != this._rewardRevealDuration)
				{
					this._rewardRevealDuration = value;
					base.OnPropertyChanged(value, "RewardRevealDuration");
				}
			}
		}

		// Token: 0x170002A9 RID: 681
		// (get) Token: 0x06000791 RID: 1937 RVA: 0x000164BE File Offset: 0x000146BE
		// (set) Token: 0x06000792 RID: 1938 RVA: 0x000164C6 File Offset: 0x000146C6
		[Editor(false)]
		public MultiplayerLobbyAfterBattleExperiencePanelWidget ExperiencePanel
		{
			get
			{
				return this._experiencePanel;
			}
			set
			{
				if (value != this._experiencePanel)
				{
					this._experiencePanel = value;
					base.OnPropertyChanged<MultiplayerLobbyAfterBattleExperiencePanelWidget>(value, "ExperiencePanel");
				}
			}
		}

		// Token: 0x170002AA RID: 682
		// (get) Token: 0x06000793 RID: 1939 RVA: 0x000164E4 File Offset: 0x000146E4
		// (set) Token: 0x06000794 RID: 1940 RVA: 0x000164EC File Offset: 0x000146EC
		[Editor(false)]
		public TextWidget ClickToContinueTextWidget
		{
			get
			{
				return this._clickToContinueTextWidget;
			}
			set
			{
				if (value != this._clickToContinueTextWidget)
				{
					this._clickToContinueTextWidget = value;
					base.OnPropertyChanged<TextWidget>(value, "ClickToContinueTextWidget");
				}
			}
		}

		// Token: 0x170002AB RID: 683
		// (get) Token: 0x06000795 RID: 1941 RVA: 0x0001650A File Offset: 0x0001470A
		// (set) Token: 0x06000796 RID: 1942 RVA: 0x00016512 File Offset: 0x00014712
		[Editor(false)]
		public ListPanel RewardsListPanel
		{
			get
			{
				return this._rewardsListPanel;
			}
			set
			{
				if (value != this._rewardsListPanel)
				{
					this._rewardsListPanel = value;
					base.OnPropertyChanged<ListPanel>(value, "RewardsListPanel");
				}
			}
		}

		// Token: 0x04000360 RID: 864
		private bool _isStarted;

		// Token: 0x04000361 RID: 865
		private bool _isFinished;

		// Token: 0x04000362 RID: 866
		private float _timePassed;

		// Token: 0x04000363 RID: 867
		private int _currentRewardIndex;

		// Token: 0x04000364 RID: 868
		private bool _isActive;

		// Token: 0x04000365 RID: 869
		private float _animationDelay;

		// Token: 0x04000366 RID: 870
		private float _animationDuration;

		// Token: 0x04000367 RID: 871
		private float _rewardRevealDuration;

		// Token: 0x04000368 RID: 872
		private MultiplayerLobbyAfterBattleExperiencePanelWidget _experiencePanel;

		// Token: 0x04000369 RID: 873
		private TextWidget _clickToContinueTextWidget;

		// Token: 0x0400036A RID: 874
		private ListPanel _rewardsListPanel;
	}
}
