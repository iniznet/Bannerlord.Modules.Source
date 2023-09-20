using System;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.View.MissionViews
{
	[DefaultView]
	public class MissionCameraFadeView : MissionView
	{
		public float FadeAlpha { get; private set; }

		public MissionCameraFadeView.CameraFadeState FadeState { get; private set; }

		public bool IsCameraFading
		{
			get
			{
				return this.FadeState == MissionCameraFadeView.CameraFadeState.FadingIn || this.FadeState == MissionCameraFadeView.CameraFadeState.FadingOut;
			}
		}

		public bool HasCameraFadeOut
		{
			get
			{
				return this.FadeState == MissionCameraFadeView.CameraFadeState.Black;
			}
		}

		public bool HasCameraFadeIn
		{
			get
			{
				return this.FadeState == MissionCameraFadeView.CameraFadeState.White;
			}
		}

		public override void OnMissionScreenInitialize()
		{
			base.OnMissionScreenInitialize();
			this._stateDuration = 0f;
			this.FadeState = MissionCameraFadeView.CameraFadeState.White;
			this.FadeAlpha = 0f;
		}

		public override void OnMissionScreenTick(float dt)
		{
			base.OnMissionScreenTick(dt);
			if (base.Mission != null && base.MissionScreen.IsMissionTickable)
			{
				this.UpdateFadeState(dt);
			}
		}

		protected void UpdateFadeState(float dt)
		{
			if (this.IsCameraFading)
			{
				this._stateDuration -= dt;
				if (this.FadeState == MissionCameraFadeView.CameraFadeState.FadingOut)
				{
					this.FadeAlpha = MathF.Min(1f - this._stateDuration / this._fadeOutTime, 1f);
					if (this._stateDuration < 0f)
					{
						this._stateDuration = this._blackTime;
						this.FadeState = MissionCameraFadeView.CameraFadeState.Black;
						return;
					}
				}
				else if (this.FadeState == MissionCameraFadeView.CameraFadeState.FadingIn)
				{
					this.FadeAlpha = MathF.Max(this._stateDuration / this._fadeInTime, 0f);
					if (this._stateDuration < 0f)
					{
						this._stateDuration = 0f;
						this.FadeState = MissionCameraFadeView.CameraFadeState.White;
						return;
					}
				}
			}
			else if (this.HasCameraFadeOut && this._autoFadeIn)
			{
				this._stateDuration -= dt;
				if (this._stateDuration < 0f)
				{
					this._stateDuration = this._fadeInTime;
					this.FadeState = MissionCameraFadeView.CameraFadeState.FadingIn;
					this._autoFadeIn = false;
				}
			}
		}

		public void BeginFadeOutAndIn(float fadeOutTime, float blackTime, float fadeInTime)
		{
			if (base.Mission != null && base.MissionScreen.IsMissionTickable && this.FadeState == MissionCameraFadeView.CameraFadeState.White)
			{
				this._autoFadeIn = true;
				this._fadeOutTime = MathF.Max(fadeOutTime, 1E-05f);
				this._blackTime = MathF.Max(blackTime, 1E-05f);
				this._fadeInTime = MathF.Max(fadeInTime, 1E-05f);
				this._stateDuration = fadeOutTime;
				this.FadeAlpha = 0f;
				this.FadeState = MissionCameraFadeView.CameraFadeState.FadingOut;
			}
		}

		public void BeginFadeOut(float fadeOutTime)
		{
			if (base.Mission != null && base.MissionScreen.IsMissionTickable && this.FadeState == MissionCameraFadeView.CameraFadeState.White)
			{
				this._autoFadeIn = false;
				this._fadeOutTime = MathF.Max(fadeOutTime, 1E-05f);
				this._blackTime = 0f;
				this._fadeInTime = 0f;
				this._stateDuration = fadeOutTime;
				this.FadeAlpha = 0f;
				this.FadeState = MissionCameraFadeView.CameraFadeState.FadingOut;
			}
		}

		public void BeginFadeIn(float fadeInTime)
		{
			if (base.Mission != null && base.MissionScreen.IsMissionTickable && this.FadeState == MissionCameraFadeView.CameraFadeState.Black && !this._autoFadeIn)
			{
				this._fadeOutTime = 0f;
				this._blackTime = 0f;
				this._fadeInTime = MathF.Max(fadeInTime, 1E-05f);
				this._stateDuration = fadeInTime;
				this.FadeAlpha = 1f;
				this.FadeState = MissionCameraFadeView.CameraFadeState.FadingIn;
			}
		}

		private bool _autoFadeIn;

		private float _fadeInTime = 0.5f;

		private float _blackTime = 0.25f;

		private float _fadeOutTime = 0.5f;

		private float _stateDuration;

		public enum CameraFadeState
		{
			White,
			FadingOut,
			Black,
			FadingIn
		}
	}
}
