using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets
{
	// Token: 0x0200000C RID: 12
	public class CircleLoadingAnimWidget : Widget
	{
		// Token: 0x17000024 RID: 36
		// (get) Token: 0x0600005F RID: 95 RVA: 0x00002DA4 File Offset: 0x00000FA4
		// (set) Token: 0x06000060 RID: 96 RVA: 0x00002DAC File Offset: 0x00000FAC
		public float NumOfCirclesInASecond { get; set; } = 0.5f;

		// Token: 0x17000025 RID: 37
		// (get) Token: 0x06000061 RID: 97 RVA: 0x00002DB5 File Offset: 0x00000FB5
		// (set) Token: 0x06000062 RID: 98 RVA: 0x00002DBD File Offset: 0x00000FBD
		public float NormalAlpha { get; set; } = 0.5f;

		// Token: 0x17000026 RID: 38
		// (get) Token: 0x06000063 RID: 99 RVA: 0x00002DC6 File Offset: 0x00000FC6
		// (set) Token: 0x06000064 RID: 100 RVA: 0x00002DCE File Offset: 0x00000FCE
		public float FullAlpha { get; set; } = 1f;

		// Token: 0x17000027 RID: 39
		// (get) Token: 0x06000065 RID: 101 RVA: 0x00002DD7 File Offset: 0x00000FD7
		// (set) Token: 0x06000066 RID: 102 RVA: 0x00002DDF File Offset: 0x00000FDF
		public float CircleRadius { get; set; } = 50f;

		// Token: 0x17000028 RID: 40
		// (get) Token: 0x06000067 RID: 103 RVA: 0x00002DE8 File Offset: 0x00000FE8
		// (set) Token: 0x06000068 RID: 104 RVA: 0x00002DF0 File Offset: 0x00000FF0
		public float StaySeconds { get; set; } = 2f;

		// Token: 0x17000029 RID: 41
		// (get) Token: 0x06000069 RID: 105 RVA: 0x00002DF9 File Offset: 0x00000FF9
		// (set) Token: 0x0600006A RID: 106 RVA: 0x00002E01 File Offset: 0x00001001
		public bool IsMovementEnabled { get; set; } = true;

		// Token: 0x1700002A RID: 42
		// (get) Token: 0x0600006B RID: 107 RVA: 0x00002E0A File Offset: 0x0000100A
		// (set) Token: 0x0600006C RID: 108 RVA: 0x00002E12 File Offset: 0x00001012
		public bool IsReverse { get; set; }

		// Token: 0x1700002B RID: 43
		// (get) Token: 0x0600006D RID: 109 RVA: 0x00002E1B File Offset: 0x0000101B
		// (set) Token: 0x0600006E RID: 110 RVA: 0x00002E23 File Offset: 0x00001023
		public float FadeInSeconds { get; set; } = 0.2f;

		// Token: 0x1700002C RID: 44
		// (get) Token: 0x0600006F RID: 111 RVA: 0x00002E2C File Offset: 0x0000102C
		// (set) Token: 0x06000070 RID: 112 RVA: 0x00002E34 File Offset: 0x00001034
		public float FadeOutSeconds { get; set; } = 0.2f;

		// Token: 0x1700002D RID: 45
		// (get) Token: 0x06000071 RID: 113 RVA: 0x00002E3D File Offset: 0x0000103D
		private float CurrentAlpha
		{
			get
			{
				return base.GetChild(0).AlphaFactor;
			}
		}

		// Token: 0x06000072 RID: 114 RVA: 0x00002E4C File Offset: 0x0000104C
		public CircleLoadingAnimWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000073 RID: 115 RVA: 0x00002EB4 File Offset: 0x000010B4
		protected override void OnParallelUpdate(float dt)
		{
			base.OnParallelUpdate(dt);
			this._totalTime += dt;
			if (!this._initialized)
			{
				this._visualState = CircleLoadingAnimWidget.VisualState.FadeIn;
				this.SetGlobalAlphaRecursively(0f);
				this._initialized = true;
			}
			if (this.IsMovementEnabled && base.IsVisible)
			{
				Widget parentWidget = base.ParentWidget;
				if (parentWidget != null && parentWidget.IsVisible)
				{
					this.UpdateMovementValues(dt);
					this.UpdateAlphaValues(dt);
				}
			}
		}

		// Token: 0x06000074 RID: 116 RVA: 0x00002F2C File Offset: 0x0000112C
		private void UpdateMovementValues(float dt)
		{
			if (this.IsMovementEnabled)
			{
				float num = 360f / (float)base.ChildCount;
				float num2 = this._currentAngle;
				for (int i = 0; i < base.ChildCount; i++)
				{
					float num3 = MathF.Cos(num2 * 0.017453292f) * this.CircleRadius;
					float num4 = MathF.Sin(num2 * 0.017453292f) * this.CircleRadius;
					base.GetChild(i).PositionXOffset = (this.IsReverse ? num4 : num3);
					base.GetChild(i).PositionYOffset = (this.IsReverse ? num3 : num4);
					num2 += num;
					num2 %= 360f;
				}
				this._currentAngle += dt * 360f * this.NumOfCirclesInASecond;
				this._currentAngle %= 360f;
			}
		}

		// Token: 0x06000075 RID: 117 RVA: 0x00003000 File Offset: 0x00001200
		private void UpdateAlphaValues(float dt)
		{
			float num = 1f;
			if (this._visualState == CircleLoadingAnimWidget.VisualState.FadeIn)
			{
				num = Mathf.Lerp(this.CurrentAlpha, 1f, dt / this.FadeInSeconds);
				if (this.CurrentAlpha >= 0.9f)
				{
					this._visualState = CircleLoadingAnimWidget.VisualState.Animating;
					this._stayStartTime = this._totalTime;
				}
			}
			else if (this._visualState == CircleLoadingAnimWidget.VisualState.Animating)
			{
				num = 1f;
				if (this.StaySeconds != -1f && this._totalTime - this._stayStartTime > this.StaySeconds)
				{
					this._visualState = CircleLoadingAnimWidget.VisualState.FadeOut;
				}
			}
			else if (this._visualState == CircleLoadingAnimWidget.VisualState.FadeOut)
			{
				num = Mathf.Lerp(this.CurrentAlpha, 0f, dt / this.FadeOutSeconds);
				if (this.CurrentAlpha <= 0.01f && this._totalTime - (this._stayStartTime + this.StaySeconds + this.FadeOutSeconds) > 3f)
				{
					this._visualState = CircleLoadingAnimWidget.VisualState.FadeIn;
				}
			}
			else
			{
				Debug.FailedAssert("This visual state is not enabled", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.GauntletUI.Widgets\\CircleLoadingAnimWidget.cs", "UpdateAlphaValues", 115);
			}
			this.SetGlobalAlphaRecursively(num);
		}

		// Token: 0x04000032 RID: 50
		private CircleLoadingAnimWidget.VisualState _visualState;

		// Token: 0x04000033 RID: 51
		private float _stayStartTime;

		// Token: 0x04000034 RID: 52
		private float _currentAngle;

		// Token: 0x04000035 RID: 53
		private bool _initialized;

		// Token: 0x04000036 RID: 54
		private float _totalTime;

		// Token: 0x02000175 RID: 373
		public enum VisualState
		{
			// Token: 0x040008A7 RID: 2215
			FadeIn,
			// Token: 0x040008A8 RID: 2216
			Animating,
			// Token: 0x040008A9 RID: 2217
			FadeOut
		}
	}
}
