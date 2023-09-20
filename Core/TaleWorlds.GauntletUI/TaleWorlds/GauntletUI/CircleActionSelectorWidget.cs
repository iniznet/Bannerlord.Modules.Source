using System;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;

namespace TaleWorlds.GauntletUI
{
	public class CircleActionSelectorWidget : Widget
	{
		public CircleActionSelectorWidget(UIContext context)
			: base(context)
		{
			this._activateOnlyWithController = false;
			this._distanceFromCenterModifier = 300f;
			this._directionWidgetDistanceMultiplier = 0.5f;
			this._centerDistanceAnimationTimer = -1f;
			this._centerDistanceAnimationDuration = -1f;
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
			if (!this.AllowInvalidSelection)
			{
				this._currentSelectedIndex = -1;
			}
			if (base.IsRecursivelyVisible())
			{
				this.UpdateItemPlacement();
				this.AnimateDistanceFromCenter(dt);
				bool flag = this.IsCircularInputEnabled && (!this.ActivateOnlyWithController || base.EventManager.IsControllerActive);
				if (this.DirectionWidget != null)
				{
					this.DirectionWidget.IsVisible = flag;
				}
				if (flag)
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

		private void AnimateDistanceFromCenter(float dt)
		{
			if (this._centerDistanceAnimationTimer == -1f || this._centerDistanceAnimationDuration == -1f || this._centerDistanceAnimationTarget == -1f)
			{
				return;
			}
			if (this._centerDistanceAnimationTimer < this._centerDistanceAnimationDuration)
			{
				this.DistanceFromCenterModifier = MathF.Lerp(this._centerDistanceAnimationInitialValue, this._centerDistanceAnimationTarget, this._centerDistanceAnimationTimer / this._centerDistanceAnimationDuration, 1E-05f);
				this._centerDistanceAnimationTimer += dt;
				return;
			}
			this.DistanceFromCenterModifier = this._centerDistanceAnimationTarget;
			this._centerDistanceAnimationTimer = -1f;
			this._centerDistanceAnimationDuration = -1f;
			this._centerDistanceAnimationTarget = -1f;
		}

		public void AnimateDistanceFromCenterTo(float distanceFromCenter, float animationDuration)
		{
			this._centerDistanceAnimationTimer = 0f;
			this._centerDistanceAnimationInitialValue = this.DistanceFromCenterModifier;
			this._centerDistanceAnimationDuration = animationDuration;
			this._centerDistanceAnimationTarget = distanceFromCenter;
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
			this._mouseDirection = new Vec2(vec.X, vec.Y);
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

		public bool TrySetSelectedIndex(int index)
		{
			if (index >= 0 && index < base.ChildCount)
			{
				this.OnSelectedIndexChanged(index);
				return true;
			}
			return false;
		}

		protected virtual void OnSelectedIndexChanged(int selectedIndex)
		{
			for (int i = 0; i < base.ChildCount; i++)
			{
				Widget child = base.GetChild(i);
				ButtonWidget buttonWidget;
				if ((buttonWidget = child as ButtonWidget) != null)
				{
					buttonWidget.IsSelected = !child.IsDisabled && i == selectedIndex;
				}
			}
		}

		private void UpdateCircularInput()
		{
			int currentSelectedIndex = this._currentSelectedIndex;
			if (this._mouseDirection.Length > 0.391f)
			{
				if (base.ChildCount > 0)
				{
					float num = this.AngleFromDir(this._mouseDirection);
					this._currentSelectedIndex = this.GetIndexOfSelectedItemByAngle(num);
				}
			}
			else if (this.AllowInvalidSelection)
			{
				this._currentSelectedIndex = -1;
			}
			if (currentSelectedIndex != this._currentSelectedIndex)
			{
				this._isRefreshingSelection = true;
				this.OnSelectedIndexChanged(this._currentSelectedIndex);
				this._isRefreshingSelection = false;
			}
			if (this.DirectionWidget != null)
			{
				if (this._mouseDirection.LengthSquared > 0f)
				{
					Vec2 vec = this._mouseDirection.Normalized();
					this.DirectionWidget.PositionXOffset = vec.X * (this.DistanceFromCenterModifier * this.DirectionWidgetDistanceMultiplier);
					this.DirectionWidget.PositionYOffset = -vec.Y * (this.DistanceFromCenterModifier * this.DirectionWidgetDistanceMultiplier);
					return;
				}
				this.DirectionWidget.PositionXOffset = 0f;
				this.DirectionWidget.PositionYOffset = 0f;
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

		public bool AllowInvalidSelection
		{
			get
			{
				return this._allowInvalidSelection;
			}
			set
			{
				if (value != this._allowInvalidSelection)
				{
					this._allowInvalidSelection = value;
					base.OnPropertyChanged(value, "AllowInvalidSelection");
				}
			}
		}

		public bool ActivateOnlyWithController
		{
			get
			{
				return this._activateOnlyWithController;
			}
			set
			{
				if (value != this._activateOnlyWithController)
				{
					this._activateOnlyWithController = value;
					base.OnPropertyChanged(value, "ActivateOnlyWithController");
				}
			}
		}

		public bool IsCircularInputEnabled
		{
			get
			{
				return !this.IsCircularInputDisabled;
			}
			set
			{
				if (value == this.IsCircularInputDisabled)
				{
					this.IsCircularInputDisabled = !value;
					base.OnPropertyChanged(!value, "IsCircularInputEnabled");
				}
			}
		}

		public bool IsCircularInputDisabled
		{
			get
			{
				return this._isCircularInputDisabled;
			}
			set
			{
				if (value != this._isCircularInputDisabled)
				{
					this._isCircularInputDisabled = value;
					base.OnPropertyChanged(value, "IsCircularInputDisabled");
					if (value)
					{
						this.OnSelectedIndexChanged(-1);
					}
				}
			}
		}

		public float DistanceFromCenterModifier
		{
			get
			{
				return this._distanceFromCenterModifier;
			}
			set
			{
				if (value != this._distanceFromCenterModifier)
				{
					this._distanceFromCenterModifier = value;
					base.OnPropertyChanged(value, "DistanceFromCenterModifier");
				}
			}
		}

		public float DirectionWidgetDistanceMultiplier
		{
			get
			{
				return this._directionWidgetDistanceMultiplier;
			}
			set
			{
				if (value != this._directionWidgetDistanceMultiplier)
				{
					this._directionWidgetDistanceMultiplier = value;
					base.OnPropertyChanged(value, "DirectionWidgetDistanceMultiplier");
				}
			}
		}

		public Widget DirectionWidget
		{
			get
			{
				return this._directionWidget;
			}
			set
			{
				if (value != this._directionWidget)
				{
					this._directionWidget = value;
					base.OnPropertyChanged<Widget>(value, "DirectionWidget");
				}
			}
		}

		private int _currentSelectedIndex;

		private const float _mouseMoveMaxDistance = 125f;

		private const float _gamepadDeadzoneLength = 0.391f;

		private const float _mouseMoveMaxDistanceSquared = 15625f;

		private float _centerDistanceAnimationTimer;

		private float _centerDistanceAnimationDuration;

		private float _centerDistanceAnimationInitialValue;

		private float _centerDistanceAnimationTarget;

		private Vec2 _mouseDirection;

		private Vec2 _mouseMoveAccumulated;

		private bool _isRefreshingSelection;

		private bool _allowInvalidSelection;

		private bool _activateOnlyWithController;

		private bool _isCircularInputDisabled;

		private float _distanceFromCenterModifier;

		private float _directionWidgetDistanceMultiplier;

		private Widget _directionWidget;
	}
}
