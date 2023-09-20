using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets
{
	// Token: 0x0200002F RID: 47
	public class ParallaxItemBrushWidget : BrushWidget
	{
		// Token: 0x170000EA RID: 234
		// (get) Token: 0x0600029D RID: 669 RVA: 0x00008AC3 File Offset: 0x00006CC3
		// (set) Token: 0x0600029E RID: 670 RVA: 0x00008ACB File Offset: 0x00006CCB
		public bool IsEaseInOutEnabled { get; set; } = true;

		// Token: 0x170000EB RID: 235
		// (get) Token: 0x0600029F RID: 671 RVA: 0x00008AD4 File Offset: 0x00006CD4
		// (set) Token: 0x060002A0 RID: 672 RVA: 0x00008ADC File Offset: 0x00006CDC
		public float OneDirectionDuration { get; set; } = 1f;

		// Token: 0x170000EC RID: 236
		// (get) Token: 0x060002A1 RID: 673 RVA: 0x00008AE5 File Offset: 0x00006CE5
		// (set) Token: 0x060002A2 RID: 674 RVA: 0x00008AED File Offset: 0x00006CED
		public float OneDirectionDistance { get; set; } = 1f;

		// Token: 0x170000ED RID: 237
		// (get) Token: 0x060002A3 RID: 675 RVA: 0x00008AF6 File Offset: 0x00006CF6
		// (set) Token: 0x060002A4 RID: 676 RVA: 0x00008AFE File Offset: 0x00006CFE
		public ParallaxItemBrushWidget.ParallaxMovementDirection InitialDirection { get; set; }

		// Token: 0x170000EE RID: 238
		// (get) Token: 0x060002A5 RID: 677 RVA: 0x00008B07 File Offset: 0x00006D07
		private float _centerOffset
		{
			get
			{
				return this.OneDirectionDuration / 2f;
			}
		}

		// Token: 0x170000EF RID: 239
		// (get) Token: 0x060002A6 RID: 678 RVA: 0x00008B15 File Offset: 0x00006D15
		private float _localTime
		{
			get
			{
				return base.Context.EventManager.Time + this._centerOffset;
			}
		}

		// Token: 0x060002A7 RID: 679 RVA: 0x00008B2E File Offset: 0x00006D2E
		public ParallaxItemBrushWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x060002A8 RID: 680 RVA: 0x00008B54 File Offset: 0x00006D54
		protected override void OnUpdate(float dt)
		{
			base.OnUpdate(dt);
			if (!this._initialized)
			{
				this.OneDirectionDuration = MathF.Max(float.Epsilon, this.OneDirectionDuration);
				this._initialized = true;
			}
			if (this.InitialDirection != ParallaxItemBrushWidget.ParallaxMovementDirection.None)
			{
				bool flag = this._localTime % (this.OneDirectionDuration * 4f) > this.OneDirectionDuration * 2f;
				float num3;
				if (this.IsEaseInOutEnabled)
				{
					float num = this._localTime % (this.OneDirectionDuration * 4f);
					float oneDirectionDuration = this.OneDirectionDuration;
					float num2 = MathF.PingPong(0f, this.OneDirectionDuration * 4f, this._localTime) / (this.OneDirectionDuration * 4f);
					float quadEaseInOut = this.GetQuadEaseInOut(num2);
					num3 = MathF.Lerp(-this.OneDirectionDistance, this.OneDirectionDistance, quadEaseInOut, 1E-05f);
				}
				else
				{
					float num4 = MathF.PingPong(0f, this.OneDirectionDuration, this._localTime) / this.OneDirectionDuration;
					num3 = this.OneDirectionDistance * num4;
					num3 = (flag ? (-num3) : num3);
				}
				switch (this.InitialDirection)
				{
				case ParallaxItemBrushWidget.ParallaxMovementDirection.Left:
					base.PositionXOffset = num3;
					return;
				case ParallaxItemBrushWidget.ParallaxMovementDirection.Right:
					base.PositionXOffset = -num3;
					return;
				case ParallaxItemBrushWidget.ParallaxMovementDirection.Up:
					base.PositionYOffset = -num3;
					return;
				case ParallaxItemBrushWidget.ParallaxMovementDirection.Down:
					base.PositionYOffset = num3;
					break;
				default:
					return;
				}
			}
		}

		// Token: 0x060002A9 RID: 681 RVA: 0x00008CA8 File Offset: 0x00006EA8
		private float GetCubicEaseInOut(float t)
		{
			if (t < 0.5f)
			{
				return 4f * t * t * t;
			}
			float num = 2f * t - 2f;
			return 0.5f * num * num * num + 1f;
		}

		// Token: 0x060002AA RID: 682 RVA: 0x00008CE8 File Offset: 0x00006EE8
		private float GetElasticEaseInOut(float t)
		{
			if (t < 0.5f)
			{
				return (float)(0.5 * Math.Sin(20.420352248333657 * (double)(2f * t)) * Math.Pow(2.0, (double)(10f * (2f * t - 1f))));
			}
			return (float)(0.5 * (Math.Sin(-20.420352248333657 * (double)(2f * t - 1f + 1f)) * Math.Pow(2.0, (double)(-10f * (2f * t - 1f))) + 2.0));
		}

		// Token: 0x060002AB RID: 683 RVA: 0x00008DA0 File Offset: 0x00006FA0
		private float ExponentialEaseInOut(float t)
		{
			if (t == 0f || t == 1f)
			{
				return t;
			}
			if (t < 0.5f)
			{
				return (float)(0.5 * Math.Pow(2.0, (double)(20f * t - 10f)));
			}
			return (float)(-0.5 * Math.Pow(2.0, (double)(-20f * t + 10f)) + 1.0);
		}

		// Token: 0x060002AC RID: 684 RVA: 0x00008E20 File Offset: 0x00007020
		private float GetQuadEaseInOut(float t)
		{
			if (t < 0.5f)
			{
				return 2f * t * t;
			}
			return -2f * t * t + 4f * t - 1f;
		}

		// Token: 0x04000114 RID: 276
		private bool _initialized;

		// Token: 0x0200017A RID: 378
		public enum ParallaxMovementDirection
		{
			// Token: 0x040008B4 RID: 2228
			None,
			// Token: 0x040008B5 RID: 2229
			Left,
			// Token: 0x040008B6 RID: 2230
			Right,
			// Token: 0x040008B7 RID: 2231
			Up,
			// Token: 0x040008B8 RID: 2232
			Down
		}
	}
}
