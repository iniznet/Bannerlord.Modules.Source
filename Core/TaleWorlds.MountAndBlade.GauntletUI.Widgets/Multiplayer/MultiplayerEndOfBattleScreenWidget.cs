using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer
{
	public class MultiplayerEndOfBattleScreenWidget : Widget
	{
		public MultiplayerEndOfBattleScreenWidget(UIContext context)
			: base(context)
		{
		}

		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (this._isAnimationStarted)
			{
				this.SetGlobalAlphaRecursively(MathF.Lerp(this._initialAlpha, this._targetAlpha, this._fadeInTimeElapsed / this.FadeInDuration, 1E-05f));
				this._fadeInTimeElapsed += dt;
				if (this._fadeInTimeElapsed >= this.FadeInDuration)
				{
					this._isAnimationStarted = false;
				}
			}
		}

		[Editor(false)]
		public bool IsShown
		{
			get
			{
				return this._isShown;
			}
			set
			{
				if (value != this._isShown)
				{
					this._isShown = value;
					base.OnPropertyChanged(value, "IsShown");
					this.SetGlobalAlphaRecursively(0f);
					this._isAnimationStarted = value;
					base.IsVisible = value;
					this._fadeInTimeElapsed = 0f;
				}
			}
		}

		[Editor(false)]
		public float FadeInDuration
		{
			get
			{
				return this._fadeInDuration;
			}
			set
			{
				if (value != this._fadeInDuration)
				{
					this._fadeInDuration = value;
					base.OnPropertyChanged(value, "FadeInDuration");
				}
			}
		}

		private float _initialAlpha;

		private float _targetAlpha = 1f;

		private bool _isAnimationStarted;

		private float _fadeInTimeElapsed;

		private bool _isShown;

		private float _fadeInDuration = 0.3f;
	}
}
