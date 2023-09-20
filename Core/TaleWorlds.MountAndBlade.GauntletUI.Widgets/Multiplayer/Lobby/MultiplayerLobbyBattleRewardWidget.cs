using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.GauntletUI.ExtraWidgets;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer.Lobby
{
	// Token: 0x02000093 RID: 147
	public class MultiplayerLobbyBattleRewardWidget : Widget
	{
		// Token: 0x170002B7 RID: 695
		// (get) Token: 0x060007B5 RID: 1973 RVA: 0x00016A69 File Offset: 0x00014C69
		// (set) Token: 0x060007B6 RID: 1974 RVA: 0x00016A71 File Offset: 0x00014C71
		public float AnimationDuration { get; set; } = 0.1f;

		// Token: 0x170002B8 RID: 696
		// (get) Token: 0x060007B7 RID: 1975 RVA: 0x00016A7A File Offset: 0x00014C7A
		// (set) Token: 0x060007B8 RID: 1976 RVA: 0x00016A82 File Offset: 0x00014C82
		public float TextRevealAnimationDuration { get; set; } = 0.05f;

		// Token: 0x170002B9 RID: 697
		// (get) Token: 0x060007B9 RID: 1977 RVA: 0x00016A8B File Offset: 0x00014C8B
		// (set) Token: 0x060007BA RID: 1978 RVA: 0x00016A93 File Offset: 0x00014C93
		public float AnimationInitialScaleMultiplier { get; set; } = 2f;

		// Token: 0x060007BB RID: 1979 RVA: 0x00016A9C File Offset: 0x00014C9C
		public MultiplayerLobbyBattleRewardWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x060007BC RID: 1980 RVA: 0x00016AC8 File Offset: 0x00014CC8
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

		// Token: 0x060007BD RID: 1981 RVA: 0x00016CE4 File Offset: 0x00014EE4
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

		// Token: 0x060007BE RID: 1982 RVA: 0x00016EAC File Offset: 0x000150AC
		public void StartPreAnimation()
		{
			this._isInPreAnimationState = true;
			base.IsVisible = true;
		}

		// Token: 0x060007BF RID: 1983 RVA: 0x00016EBC File Offset: 0x000150BC
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

		// Token: 0x0400037A RID: 890
		private const string _rewardImpactSoundEventName = "inventory/perk";

		// Token: 0x0400037B RID: 891
		private float _buttonAnimationStartWidth;

		// Token: 0x0400037C RID: 892
		private float _buttonAnimationStartHeight;

		// Token: 0x0400037D RID: 893
		private float _buttonAnimationEndWidth;

		// Token: 0x0400037E RID: 894
		private float _buttonAnimationEndHeight;

		// Token: 0x0400037F RID: 895
		private float _iconAnimationStartWidget;

		// Token: 0x04000380 RID: 896
		private float _iconAnimationStartHeight;

		// Token: 0x04000381 RID: 897
		private float _iconAnimationEndWidth;

		// Token: 0x04000382 RID: 898
		private float _iconAnimationEndHeight;

		// Token: 0x04000386 RID: 902
		private ButtonWidget _rewardIconButton;

		// Token: 0x04000387 RID: 903
		private Widget _rewardIcon;

		// Token: 0x04000388 RID: 904
		private TextWidget _rewardTextDescription;

		// Token: 0x04000389 RID: 905
		private ValueBasedVisibilityWidget _rewardToShow;

		// Token: 0x0400038A RID: 906
		private bool _isAnimationStarted;

		// Token: 0x0400038B RID: 907
		private bool _isTextAnimationStarted;

		// Token: 0x0400038C RID: 908
		private bool _isInPreAnimationState;

		// Token: 0x0400038D RID: 909
		private float _animationStartTime;

		// Token: 0x0400038E RID: 910
		private float _textAnimationStartTime;
	}
}
