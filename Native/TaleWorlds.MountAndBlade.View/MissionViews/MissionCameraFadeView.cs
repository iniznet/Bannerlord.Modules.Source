using System;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.View.MissionViews
{
	// Token: 0x02000047 RID: 71
	[DefaultView]
	public class MissionCameraFadeView : MissionView
	{
		// Token: 0x17000046 RID: 70
		// (get) Token: 0x06000322 RID: 802 RVA: 0x0001B59F File Offset: 0x0001979F
		// (set) Token: 0x06000323 RID: 803 RVA: 0x0001B5A7 File Offset: 0x000197A7
		public float FadeAlpha { get; private set; }

		// Token: 0x17000047 RID: 71
		// (get) Token: 0x06000324 RID: 804 RVA: 0x0001B5B0 File Offset: 0x000197B0
		// (set) Token: 0x06000325 RID: 805 RVA: 0x0001B5B8 File Offset: 0x000197B8
		public MissionCameraFadeView.CameraFadeState FadeState { get; private set; }

		// Token: 0x17000048 RID: 72
		// (get) Token: 0x06000326 RID: 806 RVA: 0x0001B5C1 File Offset: 0x000197C1
		public bool IsCameraFading
		{
			get
			{
				return this.FadeState == MissionCameraFadeView.CameraFadeState.FadingIn || this.FadeState == MissionCameraFadeView.CameraFadeState.FadingOut;
			}
		}

		// Token: 0x17000049 RID: 73
		// (get) Token: 0x06000327 RID: 807 RVA: 0x0001B5D7 File Offset: 0x000197D7
		public bool HasCameraFadeOut
		{
			get
			{
				return this.FadeState == MissionCameraFadeView.CameraFadeState.Black;
			}
		}

		// Token: 0x1700004A RID: 74
		// (get) Token: 0x06000328 RID: 808 RVA: 0x0001B5E2 File Offset: 0x000197E2
		public bool HasCameraFadeIn
		{
			get
			{
				return this.FadeState == MissionCameraFadeView.CameraFadeState.White;
			}
		}

		// Token: 0x06000329 RID: 809 RVA: 0x0001B5ED File Offset: 0x000197ED
		public override void OnMissionScreenInitialize()
		{
			base.OnMissionScreenInitialize();
			this._stateDuration = 0f;
			this.FadeState = MissionCameraFadeView.CameraFadeState.White;
			this.FadeAlpha = 0f;
		}

		// Token: 0x0600032A RID: 810 RVA: 0x0001B612 File Offset: 0x00019812
		public override void OnMissionScreenTick(float dt)
		{
			base.OnMissionScreenTick(dt);
			if (base.Mission != null && base.MissionScreen.IsMissionTickable)
			{
				this.UpdateFadeState(dt);
			}
		}

		// Token: 0x0600032B RID: 811 RVA: 0x0001B638 File Offset: 0x00019838
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

		// Token: 0x0600032C RID: 812 RVA: 0x0001B73C File Offset: 0x0001993C
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

		// Token: 0x0600032D RID: 813 RVA: 0x0001B7BC File Offset: 0x000199BC
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

		// Token: 0x0600032E RID: 814 RVA: 0x0001B830 File Offset: 0x00019A30
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

		// Token: 0x04000220 RID: 544
		private bool _autoFadeIn;

		// Token: 0x04000221 RID: 545
		private float _fadeInTime = 0.5f;

		// Token: 0x04000222 RID: 546
		private float _blackTime = 0.25f;

		// Token: 0x04000223 RID: 547
		private float _fadeOutTime = 0.5f;

		// Token: 0x04000224 RID: 548
		private float _stateDuration;

		// Token: 0x020000B8 RID: 184
		public enum CameraFadeState
		{
			// Token: 0x0400036A RID: 874
			White,
			// Token: 0x0400036B RID: 875
			FadingOut,
			// Token: 0x0400036C RID: 876
			Black,
			// Token: 0x0400036D RID: 877
			FadingIn
		}
	}
}
