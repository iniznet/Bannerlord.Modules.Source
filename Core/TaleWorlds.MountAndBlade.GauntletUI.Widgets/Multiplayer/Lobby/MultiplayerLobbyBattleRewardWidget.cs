using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.GauntletUI.ExtraWidgets;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer.Lobby
{
	public class MultiplayerLobbyBattleRewardWidget : Widget
	{
		public float AnimationDuration { get; set; } = 0.1f;

		public float TextRevealAnimationDuration { get; set; } = 0.05f;

		public float AnimationInitialScaleMultiplier { get; set; } = 2f;

		public MultiplayerLobbyBattleRewardWidget(UIContext context)
			: base(context)
		{
		}

		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (this._isInPreAnimationState)
			{
				foreach (Widget widget in base.Children)
				{
					if (widget is ValueBasedVisibilityWidget)
					{
						widget.IsVisible = false;
					}
				}
			}
			bool flag = false;
			if (this._isAnimationStarted && base.EventManager.Time - this._animationStartTime < this.AnimationDuration)
			{
				float num = (base.EventManager.Time - this._animationStartTime) / this.AnimationDuration;
				this._rewardIconButton.SuggestedWidth = Mathf.Lerp(this._buttonAnimationStartWidth, this._buttonAnimationEndWidth, num);
				this._rewardIconButton.SuggestedHeight = Mathf.Lerp(this._buttonAnimationStartHeight, this._buttonAnimationEndHeight, num);
				this._rewardIcon.SuggestedWidth = Mathf.Lerp(this._iconAnimationStartWidget, this._iconAnimationEndWidth, num);
				this._rewardIcon.SuggestedHeight = Mathf.Lerp(this._iconAnimationStartHeight, this._iconAnimationEndHeight, num);
				this._rewardIconButton.SetGlobalAlphaRecursively(Mathf.Lerp(0f, 1f, num));
				this._rewardIcon.SetGlobalAlphaRecursively(Mathf.Lerp(0f, 1f, num));
				this._rewardToShow.IsVisible = true;
				flag = true;
			}
			if (!this._isTextAnimationStarted && this._isAnimationStarted && base.EventManager.Time - this._animationStartTime >= this.AnimationDuration)
			{
				this._textAnimationStartTime = base.EventManager.Time;
				this._isTextAnimationStarted = true;
			}
			if (this._isTextAnimationStarted && base.EventManager.Time - this._textAnimationStartTime < this.TextRevealAnimationDuration)
			{
				float num2 = (base.EventManager.Time - this._textAnimationStartTime) / this.TextRevealAnimationDuration;
				this._rewardTextDescription.SetGlobalAlphaRecursively(Mathf.Lerp(0f, 1f, num2));
				flag = true;
			}
			if (this._isAnimationStarted && this._isTextAnimationStarted && !flag)
			{
				this.EndAnimation();
			}
		}

		public void StartAnimation()
		{
			this._isInPreAnimationState = false;
			foreach (Widget widget in base.Children)
			{
				ValueBasedVisibilityWidget valueBasedVisibilityWidget;
				if ((valueBasedVisibilityWidget = widget as ValueBasedVisibilityWidget) != null && valueBasedVisibilityWidget.IndexToWatch == valueBasedVisibilityWidget.IndexToBeVisible)
				{
					this._rewardToShow = valueBasedVisibilityWidget;
					this._rewardIconButton = widget.Children[0].Children[0] as ButtonWidget;
					this._rewardIcon = this._rewardIconButton.Children[0];
					this._rewardTextDescription = widget.Children[0].Children[1] as TextWidget;
					this._buttonAnimationStartWidth = this._rewardIconButton.SuggestedWidth * this.AnimationInitialScaleMultiplier;
					this._buttonAnimationStartHeight = this._rewardIconButton.SuggestedHeight * this.AnimationInitialScaleMultiplier;
					this._buttonAnimationEndWidth = this._rewardIconButton.SuggestedWidth;
					this._buttonAnimationEndHeight = this._rewardIconButton.SuggestedHeight;
					this._iconAnimationStartWidget = this._rewardIcon.SuggestedWidth * this.AnimationInitialScaleMultiplier;
					this._iconAnimationStartHeight = this._rewardIcon.SuggestedHeight * this.AnimationInitialScaleMultiplier;
					this._iconAnimationEndWidth = this._rewardIcon.SuggestedWidth;
					this._iconAnimationEndHeight = this._rewardIcon.SuggestedHeight;
					this._rewardTextDescription.SetGlobalAlphaRecursively(0f);
				}
			}
			this._isAnimationStarted = true;
			this._animationStartTime = base.EventManager.Time;
			base.Context.TwoDimensionContext.PlaySound("inventory/perk");
		}

		public void StartPreAnimation()
		{
			this._isInPreAnimationState = true;
			base.IsVisible = true;
		}

		public void EndAnimation()
		{
			this._rewardIconButton.SetGlobalAlphaRecursively(1f);
			this._rewardIcon.SetGlobalAlphaRecursively(1f);
			this._rewardTextDescription.SetGlobalAlphaRecursively(1f);
			this._rewardIconButton.SuggestedWidth = this._buttonAnimationEndWidth;
			this._rewardIconButton.SuggestedHeight = this._buttonAnimationEndHeight;
			this._rewardIcon.SuggestedWidth = this._iconAnimationEndWidth;
			this._rewardIcon.SuggestedHeight = this._iconAnimationEndHeight;
			this._isAnimationStarted = false;
			this._isTextAnimationStarted = false;
		}

		private const string _rewardImpactSoundEventName = "inventory/perk";

		private float _buttonAnimationStartWidth;

		private float _buttonAnimationStartHeight;

		private float _buttonAnimationEndWidth;

		private float _buttonAnimationEndHeight;

		private float _iconAnimationStartWidget;

		private float _iconAnimationStartHeight;

		private float _iconAnimationEndWidth;

		private float _iconAnimationEndHeight;

		private ButtonWidget _rewardIconButton;

		private Widget _rewardIcon;

		private TextWidget _rewardTextDescription;

		private ValueBasedVisibilityWidget _rewardToShow;

		private bool _isAnimationStarted;

		private bool _isTextAnimationStarted;

		private bool _isInPreAnimationState;

		private float _animationStartTime;

		private float _textAnimationStartTime;
	}
}
