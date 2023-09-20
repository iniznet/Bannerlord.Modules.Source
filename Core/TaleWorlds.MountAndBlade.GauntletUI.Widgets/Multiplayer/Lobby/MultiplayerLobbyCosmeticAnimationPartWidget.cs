using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer.Lobby
{
	// Token: 0x02000095 RID: 149
	public class MultiplayerLobbyCosmeticAnimationPartWidget : Widget
	{
		// Token: 0x060007E2 RID: 2018 RVA: 0x00017381 File Offset: 0x00015581
		public MultiplayerLobbyCosmeticAnimationPartWidget(UIContext context)
			: base(context)
		{
			this.StopAnimation();
		}

		// Token: 0x060007E3 RID: 2019 RVA: 0x00017390 File Offset: 0x00015590
		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (!this._isAnimationPlaying)
			{
				return;
			}
			if (this._alphaChangeTimeElapsed >= this._alphaChangeDuration)
			{
				this.InvertAnimationDirection();
				this.InitializeAnimationParameters();
			}
			this._currentAlpha = MathF.Lerp(this._currentAlpha, this._targetAlpha, this._alphaChangeTimeElapsed / this._alphaChangeDuration, 1E-05f);
			base.AlphaFactor = this._currentAlpha;
			this._alphaChangeTimeElapsed += dt;
		}

		// Token: 0x060007E4 RID: 2020 RVA: 0x0001740A File Offset: 0x0001560A
		public void InitializeAnimationParameters()
		{
			this._currentAlpha = this._minAlpha;
			this._targetAlpha = this._maxAlpha;
			this._alphaChangeTimeElapsed = 0f;
			base.AlphaFactor = this._currentAlpha;
		}

		// Token: 0x060007E5 RID: 2021 RVA: 0x0001743C File Offset: 0x0001563C
		private void InvertAnimationDirection()
		{
			float minAlpha = this._minAlpha;
			this._minAlpha = this._maxAlpha;
			this._maxAlpha = minAlpha;
		}

		// Token: 0x060007E6 RID: 2022 RVA: 0x00017463 File Offset: 0x00015663
		public void StartAnimation(float alphaChangeDuration, float minAlpha, float maxAlpha)
		{
			this._alphaChangeDuration = alphaChangeDuration;
			this._minAlpha = minAlpha;
			this._maxAlpha = maxAlpha;
			this.InitializeAnimationParameters();
			this._isAnimationPlaying = true;
			base.IsVisible = true;
		}

		// Token: 0x060007E7 RID: 2023 RVA: 0x0001748E File Offset: 0x0001568E
		public void StopAnimation()
		{
			this.InitializeAnimationParameters();
			this._isAnimationPlaying = false;
			base.IsVisible = false;
		}

		// Token: 0x0400039B RID: 923
		private float _alphaChangeDuration;

		// Token: 0x0400039C RID: 924
		private float _minAlpha;

		// Token: 0x0400039D RID: 925
		private float _maxAlpha;

		// Token: 0x0400039E RID: 926
		private float _currentAlpha;

		// Token: 0x0400039F RID: 927
		private float _targetAlpha;

		// Token: 0x040003A0 RID: 928
		private float _alphaChangeTimeElapsed;

		// Token: 0x040003A1 RID: 929
		private bool _isAnimationPlaying;
	}
}
