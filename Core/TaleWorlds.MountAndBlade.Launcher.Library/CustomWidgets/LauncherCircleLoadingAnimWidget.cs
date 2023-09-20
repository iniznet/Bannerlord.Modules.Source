using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.Launcher.Library.CustomWidgets
{
	public class LauncherCircleLoadingAnimWidget : Widget
	{
		public float NumOfCirclesInASecond { get; set; } = 0.5f;

		public float NormalAlpha { get; set; } = 0.5f;

		public float FullAlpha { get; set; } = 1f;

		public float CircleRadius { get; set; } = 50f;

		public float StaySeconds { get; set; } = 2f;

		public bool IsMovementEnabled { get; set; } = true;

		public bool IsReverse { get; set; }

		public float FadeInSeconds { get; set; } = 0.2f;

		public float FadeOutSeconds { get; set; } = 0.2f;

		private float CurrentAlpha
		{
			get
			{
				return base.GetChild(0).AlphaFactor;
			}
		}

		public LauncherCircleLoadingAnimWidget(UIContext context)
			: base(context)
		{
		}

		protected override void OnParallelUpdate(float dt)
		{
			base.OnParallelUpdate(dt);
			this._totalTime += dt;
			if (!this._initialized)
			{
				this._visualState = LauncherCircleLoadingAnimWidget.VisualState.FadeIn;
				this.SetGlobalAlphaRecursively(0f);
				this._initialized = true;
			}
			if (this.IsMovementEnabled && base.IsVisible)
			{
				this.UpdateMovementValues(dt);
				this.UpdateAlphaValues(dt);
			}
		}

		private void UpdateMovementValues(float dt)
		{
			if (this.IsMovementEnabled)
			{
				float num = 360f / (float)base.ChildCount;
				float num2 = this._currentAngle;
				for (int i = 0; i < base.ChildCount; i++)
				{
					float num3 = (float)Math.Cos((double)(num2 * 0.017453292f)) * this.CircleRadius;
					float num4 = (float)Math.Sin((double)(num2 * 0.017453292f)) * this.CircleRadius;
					base.GetChild(i).PositionXOffset = (this.IsReverse ? num4 : num3);
					base.GetChild(i).PositionYOffset = (this.IsReverse ? num3 : num4);
					num2 += num;
					num2 %= 360f;
				}
				this._currentAngle += dt * 360f * this.NumOfCirclesInASecond;
				this._currentAngle %= 360f;
			}
		}

		private void UpdateAlphaValues(float dt)
		{
			float num = 1f;
			if (this._visualState == LauncherCircleLoadingAnimWidget.VisualState.FadeIn)
			{
				num = Mathf.Lerp(this.CurrentAlpha, 1f, dt / this.FadeInSeconds);
				if (this.CurrentAlpha >= 0.9f)
				{
					this._visualState = LauncherCircleLoadingAnimWidget.VisualState.Animating;
					this._stayStartTime = this._totalTime;
				}
			}
			else if (this._visualState == LauncherCircleLoadingAnimWidget.VisualState.Animating)
			{
				num = 1f;
				if (this.StaySeconds != -1f && this._totalTime - this._stayStartTime > this.StaySeconds)
				{
					this._visualState = LauncherCircleLoadingAnimWidget.VisualState.FadeOut;
				}
			}
			else if (this._visualState == LauncherCircleLoadingAnimWidget.VisualState.FadeOut)
			{
				num = Mathf.Lerp(this.CurrentAlpha, 0f, dt / this.FadeOutSeconds);
				if (this.CurrentAlpha <= 0.01f && this._totalTime - (this._stayStartTime + this.StaySeconds + this.FadeOutSeconds) > 3f)
				{
					this._visualState = LauncherCircleLoadingAnimWidget.VisualState.FadeIn;
				}
			}
			else
			{
				Debug.FailedAssert("This visual state is not enabled", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.Launcher.Library\\CustomWidgets\\LauncherCircleLoadingAnimWidget.cs", "UpdateAlphaValues", 113);
			}
			this.SetGlobalAlphaRecursively(num);
		}

		private LauncherCircleLoadingAnimWidget.VisualState _visualState;

		private float _stayStartTime;

		private float _currentAngle;

		private bool _initialized;

		private float _totalTime;

		public enum VisualState
		{
			FadeIn,
			Animating,
			FadeOut
		}
	}
}
