using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Tutorial
{
	public class MouseAnimStage
	{
		public bool IsCompleted { get; private set; }

		public float AnimTime { get; private set; }

		public Vec2 Direction { get; private set; }

		public MouseAnimStage.AnimTypes AnimType { get; private set; }

		public Widget WidgetToManipulate { get; private set; }

		private MouseAnimStage()
		{
		}

		internal static MouseAnimStage CreateMovementStage(float movementTime, Vec2 direction, Widget widgetToManipulate)
		{
			return new MouseAnimStage
			{
				AnimTime = movementTime,
				Direction = direction,
				AnimType = MouseAnimStage.AnimTypes.Movement,
				WidgetToManipulate = widgetToManipulate
			};
		}

		internal static MouseAnimStage CreateFadeInStage(float fadeInTime, Widget widgetToManipulate, bool isGlobal)
		{
			return new MouseAnimStage
			{
				AnimTime = fadeInTime,
				AnimType = (isGlobal ? MouseAnimStage.AnimTypes.FadeInGlobal : MouseAnimStage.AnimTypes.FadeInLocal),
				WidgetToManipulate = widgetToManipulate
			};
		}

		internal static MouseAnimStage CreateFadeOutStage(float fadeOutTime, Widget widgetToManipulate, bool isGlobal)
		{
			return new MouseAnimStage
			{
				AnimTime = fadeOutTime,
				AnimType = (isGlobal ? MouseAnimStage.AnimTypes.FadeOutGlobal : MouseAnimStage.AnimTypes.FadeOutLocal),
				WidgetToManipulate = widgetToManipulate
			};
		}

		internal static MouseAnimStage CreateStayStage(float stayTime)
		{
			return new MouseAnimStage
			{
				AnimTime = stayTime,
				AnimType = MouseAnimStage.AnimTypes.Stay,
				WidgetToManipulate = null
			};
		}

		public void Tick(float dt)
		{
			float num = MathF.Clamp(this._totalTime / this.AnimTime, 0f, 1f);
			switch (this.AnimType)
			{
			case MouseAnimStage.AnimTypes.Movement:
				this.WidgetToManipulate.PositionXOffset = ((this.Direction.X != 0f) ? MathF.Lerp(0f, this.Direction.X, num, 1E-05f) : 0f);
				this.WidgetToManipulate.PositionYOffset = ((this.Direction.Y != 0f) ? MathF.Lerp(0f, this.Direction.Y, num, 1E-05f) : 0f);
				this.IsCompleted = this._totalTime > this.AnimTime;
				break;
			case MouseAnimStage.AnimTypes.FadeInLocal:
				this.WidgetToManipulate.AlphaFactor = num;
				this.IsCompleted = this.WidgetToManipulate.AlphaFactor > 0.98f;
				break;
			case MouseAnimStage.AnimTypes.FadeOutLocal:
				this.WidgetToManipulate.AlphaFactor = 1f - num;
				this.IsCompleted = this.WidgetToManipulate.AlphaFactor < 0.02f;
				break;
			case MouseAnimStage.AnimTypes.FadeInGlobal:
				this.WidgetToManipulate.SetGlobalAlphaRecursively(num);
				this.IsCompleted = this.WidgetToManipulate.AlphaFactor > 0.98f;
				break;
			case MouseAnimStage.AnimTypes.FadeOutGlobal:
				this.WidgetToManipulate.SetGlobalAlphaRecursively(1f - num);
				this.IsCompleted = this.WidgetToManipulate.AlphaFactor < 0.02f;
				break;
			case MouseAnimStage.AnimTypes.Stay:
				this.IsCompleted = this._totalTime > this.AnimTime;
				break;
			}
			this._totalTime += dt;
		}

		private float _totalTime;

		public enum AnimTypes
		{
			Movement,
			FadeInLocal,
			FadeOutLocal,
			FadeInGlobal,
			FadeOutGlobal,
			Stay
		}
	}
}
