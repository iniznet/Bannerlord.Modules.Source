using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer.Lobby
{
	public class MultiplayerLobbyCosmeticAnimationPartWidget : Widget
	{
		public MultiplayerLobbyCosmeticAnimationPartWidget(UIContext context)
			: base(context)
		{
			this.StopAnimation();
		}

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

		public void InitializeAnimationParameters()
		{
			this._currentAlpha = this._minAlpha;
			this._targetAlpha = this._maxAlpha;
			this._alphaChangeTimeElapsed = 0f;
			base.AlphaFactor = this._currentAlpha;
		}

		private void InvertAnimationDirection()
		{
			float minAlpha = this._minAlpha;
			this._minAlpha = this._maxAlpha;
			this._maxAlpha = minAlpha;
		}

		public void StartAnimation(float alphaChangeDuration, float minAlpha, float maxAlpha)
		{
			this._alphaChangeDuration = alphaChangeDuration;
			this._minAlpha = minAlpha;
			this._maxAlpha = maxAlpha;
			this.InitializeAnimationParameters();
			this._isAnimationPlaying = true;
			base.IsVisible = true;
		}

		public void StopAnimation()
		{
			this.InitializeAnimationParameters();
			this._isAnimationPlaying = false;
			base.IsVisible = false;
		}

		private float _alphaChangeDuration;

		private float _minAlpha;

		private float _maxAlpha;

		private float _currentAlpha;

		private float _targetAlpha;

		private float _alphaChangeTimeElapsed;

		private bool _isAnimationPlaying;
	}
}
