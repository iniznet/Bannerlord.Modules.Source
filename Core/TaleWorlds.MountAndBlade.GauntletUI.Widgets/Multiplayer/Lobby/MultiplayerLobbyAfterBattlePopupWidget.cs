using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer.Lobby
{
	public class MultiplayerLobbyAfterBattlePopupWidget : Widget
	{
		public MultiplayerLobbyAfterBattlePopupWidget(UIContext context)
			: base(context)
		{
		}

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

		private void Reset()
		{
			this.ExperiencePanel.Reset();
		}

		private void IsActiveUpdated()
		{
			if (this.IsActive)
			{
				this.StartAnimation();
				return;
			}
			this.Reset();
		}

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

		private bool _isStarted;

		private bool _isFinished;

		private float _timePassed;

		private int _currentRewardIndex;

		private bool _isActive;

		private float _animationDelay;

		private float _animationDuration;

		private float _rewardRevealDuration;

		private MultiplayerLobbyAfterBattleExperiencePanelWidget _experiencePanel;

		private TextWidget _clickToContinueTextWidget;

		private ListPanel _rewardsListPanel;
	}
}
