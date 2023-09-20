using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets
{
	public class ParallaxItemBrushWidget : BrushWidget
	{
		public bool IsEaseInOutEnabled { get; set; } = true;

		public float OneDirectionDuration { get; set; } = 1f;

		public float OneDirectionDistance { get; set; } = 1f;

		public ParallaxItemBrushWidget.ParallaxMovementDirection InitialDirection { get; set; }

		private float _centerOffset
		{
			get
			{
				return this.OneDirectionDuration / 2f;
			}
		}

		private float _localTime
		{
			get
			{
				return base.Context.EventManager.Time + this._centerOffset;
			}
		}

		public ParallaxItemBrushWidget(UIContext context)
			: base(context)
		{
		}

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

		private float GetCubicEaseInOut(float t)
		{
			if (t < 0.5f)
			{
				return 4f * t * t * t;
			}
			float num = 2f * t - 2f;
			return 0.5f * num * num * num + 1f;
		}

		private float GetElasticEaseInOut(float t)
		{
			if (t < 0.5f)
			{
				return (float)(0.5 * Math.Sin(20.420352248333657 * (double)(2f * t)) * Math.Pow(2.0, (double)(10f * (2f * t - 1f))));
			}
			return (float)(0.5 * (Math.Sin(-20.420352248333657 * (double)(2f * t - 1f + 1f)) * Math.Pow(2.0, (double)(-10f * (2f * t - 1f))) + 2.0));
		}

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

		private float GetQuadEaseInOut(float t)
		{
			if (t < 0.5f)
			{
				return 2f * t * t;
			}
			return -2f * t * t + 4f * t - 1f;
		}

		private bool _initialized;

		public enum ParallaxMovementDirection
		{
			None,
			Left,
			Right,
			Up,
			Down
		}
	}
}
