using System;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;

namespace TaleWorlds.GauntletUI
{
	public class CircleActionSelectorWidget : Widget
	{
		public float DistanceFromCenterModifier { get; set; } = 300f;

		public Widget DirectionWidget { get; set; }

		public float DirectionWidgetDistanceMultiplier { get; set; } = 0.5f;

		public bool ActivateOnlyWithController { get; set; }

		public CircleActionSelectorWidget(UIContext context)
			: base(context)
		{
		}

		protected override void OnChildAdded(Widget child)
		{
			base.OnChildAdded(child);
			child.boolPropertyChanged += this.OnChildPropertyChanged;
		}

		private void OnChildPropertyChanged(PropertyOwnerObject widget, string propertyName, bool value)
		{
			if (propertyName == "IsSelected" && base.EventManager.IsControllerActive && !this._isRefreshingSelection)
			{
				this._mouseDirection = Vec2.Zero;
				this._mouseMoveAccumulated = Vec2.Zero;
			}
		}

		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (base.IsRecursivelyVisible())
			{
				this.UpdateItemPlacement();
				if (!this.ActivateOnlyWithController || base.EventManager.IsControllerActive)
				{
					this.UpdateAverageMouseDirection();
					this.UpdateCircularInput();
					return;
				}
			}
			else
			{
				if (this._mouseDirection.X != 0f || this._mouseDirection.Y != 0f)
				{
					this._mouseDirection = default(Vec2);
				}
				if (this.DirectionWidget != null)
				{
					this.DirectionWidget.IsVisible = false;
				}
				this._mouseMoveAccumulated = Vec2.Zero;
			}
		}

		private void UpdateAverageMouseDirection()
		{
			IInputContext inputContext = base.EventManager.InputContext;
			bool isMouseActive = inputContext.GetIsMouseActive();
			Vec2 vec = (isMouseActive ? new Vec2(inputContext.GetMouseMoveX(), inputContext.GetMouseMoveY()) : inputContext.GetControllerRightStickState());
			if (isMouseActive)
			{
				this._mouseMoveAccumulated += vec;
				if (this._mouseMoveAccumulated.LengthSquared > 15625f)
				{
					this._mouseMoveAccumulated.Normalize();
					this._mouseMoveAccumulated *= 125f;
				}
				this._mouseDirection = new Vec2(this._mouseMoveAccumulated.X, -this._mouseMoveAccumulated.Y);
				return;
			}
			if (vec.Length > 0.391f)
			{
				this._mouseDirection = new Vec2(vec.X, vec.Y);
			}
		}

		private void UpdateItemPlacement()
		{
			if (base.ChildCount > 0)
			{
				int childCount = base.ChildCount;
				float num = 360f / (float)childCount;
				float num2 = -(num / 2f);
				if (num2 < 0f)
				{
					num2 += 360f;
				}
				for (int i = 0; i < base.ChildCount; i++)
				{
					float num3 = num * (float)i;
					float num4 = this.AddAngle(num2, num3);
					num4 = this.AddAngle(num4, num / 2f);
					Vec2 vec = this.DirFromAngle(num4 * 0.017453292f);
					Widget child = base.GetChild(i);
					child.PositionXOffset = vec.X * this.DistanceFromCenterModifier;
					child.PositionYOffset = vec.Y * this.DistanceFromCenterModifier * -1f;
				}
			}
		}

		private void UpdateCircularInput()
		{
			float num = this.AngleFromDir(this._mouseDirection);
			if (this._mouseDirection.Length > 0.391f && base.ChildCount > 0)
			{
				int indexOfSelectedItemByAngle = this.GetIndexOfSelectedItemByAngle(num);
				if (indexOfSelectedItemByAngle != -1)
				{
					this._isRefreshingSelection = true;
					for (int i = 0; i < base.ChildCount; i++)
					{
						(base.GetChild(i) as ButtonWidget).IsSelected = i == indexOfSelectedItemByAngle;
					}
					this._isRefreshingSelection = false;
				}
			}
			if (this.DirectionWidget != null)
			{
				this.DirectionWidget.IsVisible = true;
				Vec2 vec = this._mouseDirection.Normalized();
				this.DirectionWidget.PositionXOffset = vec.X * (this.DistanceFromCenterModifier * this.DirectionWidgetDistanceMultiplier);
				this.DirectionWidget.PositionYOffset = -vec.Y * (this.DistanceFromCenterModifier * this.DirectionWidgetDistanceMultiplier);
			}
		}

		private int GetIndexOfSelectedItemByAngle(float mouseDirectionAngle)
		{
			int childCount = base.ChildCount;
			float num = 360f / (float)childCount;
			float num2 = -(num / 2f);
			if (num2 < 0f)
			{
				num2 += 360f;
			}
			for (int i = 0; i < childCount; i++)
			{
				float num3 = num * (float)i;
				float num4 = num * (float)(i + 1);
				float num5 = this.AddAngle(num2, num3) * 0.017453292f;
				float num6 = this.AddAngle(num2, num4) * 0.017453292f;
				if (this.IsAngleBetweenAngles(mouseDirectionAngle * 0.017453292f, num5, num6))
				{
					return i;
				}
			}
			return -1;
		}

		private float AddAngle(float angle1, float angle2)
		{
			float num = angle1 + angle2;
			if (num < 0f)
			{
				num += 360f;
			}
			return num % 360f;
		}

		private bool IsAngleBetweenAngles(float angle, float minAngle, float maxAngle)
		{
			float num = angle - 3.1415927f;
			float num2 = minAngle - 3.1415927f;
			float num3 = maxAngle - 3.1415927f;
			if (num2 == num3)
			{
				return true;
			}
			float num4 = MathF.Abs(MBMath.GetSmallestDifferenceBetweenTwoAngles(num3, num2));
			if (num4.ApproximatelyEqualsTo(3.1415927f, 1E-05f))
			{
				return num < num3;
			}
			float num5 = MathF.Abs(MBMath.GetSmallestDifferenceBetweenTwoAngles(num, num2));
			float num6 = MathF.Abs(MBMath.GetSmallestDifferenceBetweenTwoAngles(num, num3));
			return num5 < num4 && num6 < num4;
		}

		private float AngleFromDir(Vec2 directionVector)
		{
			if (directionVector.X < 0f)
			{
				return 360f - (float)Math.Atan2((double)directionVector.X, (double)directionVector.Y) * 57.29578f * -1f;
			}
			return (float)Math.Atan2((double)directionVector.X, (double)directionVector.Y) * 57.29578f;
		}

		private Vec2 DirFromAngle(float angle)
		{
			return new Vec2(MathF.Sin(angle), MathF.Cos(angle));
		}

		private const float _mouseMoveMaxDistance = 125f;

		private const float _gamepadDeadzoneLength = 0.391f;

		private const float _mouseMoveMaxDistanceSquared = 15625f;

		private Vec2 _mouseDirection;

		private Vec2 _mouseMoveAccumulated;

		private bool _isRefreshingSelection;
	}
}
