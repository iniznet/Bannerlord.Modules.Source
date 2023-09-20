using System;
using System.Numerics;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.GauntletUI.GamepadNavigation;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets
{
	public class GamepadCursorWidget : BrushWidget
	{
		private protected float TransitionTimer { protected get; private set; }

		public GamepadCursorWidget(UIContext context)
			: base(context)
		{
		}

		protected override void OnLateUpdate(float dt)
		{
			if (base.IsVisible)
			{
				this.RefreshTarget();
				bool flag = Input.IsKeyDown(InputKey.ControllerRDown);
				if (flag != this._isPressing)
				{
					this._animationRatioTimer = 0f;
					this.TransitionTimer = 0f;
					this._additionalOffsetBeforeStateChange = this._additionalOffset;
				}
				this._isPressing = flag;
				if (this._animationRatioTimer < 1.4f)
				{
					this._animationRatioTimer = MathF.Min(this._animationRatioTimer + dt, 1.4f);
				}
				bool flag2 = !this._targetChangedThisFrame && this._targetPositionChangedThisFrame;
				this._animationRatio = ((this.HasTarget && !this._isPressing) ? MathF.Clamp(17f * dt, 0f, 1f) : MathF.Lerp(this._animationRatio, 1f, this._animationRatioTimer / 1.4f, 1E-05f));
				this.UpdateAdditionalOffsets();
				this.UpdateTargetOffsets(flag2 ? 1f : this._animationRatio);
				if (!flag2)
				{
					this.TransitionTimer += dt;
				}
			}
			this._targetChangedThisFrame = false;
			this._targetPositionChangedThisFrame = false;
		}

		private void RefreshTarget()
		{
			GauntletGamepadNavigationManager instance = GauntletGamepadNavigationManager.Instance;
			Widget widget = ((instance != null) ? instance.LastTargetedWidget : null);
			this._targetChangedThisFrame = this._targetWidget != widget;
			this._targetWidget = widget;
			this.TargetHasAction = GauntletGamepadNavigationManager.Instance.TargetedWidgetHasAction;
			this.HasTarget = this._targetWidget != null;
			this.CursorParentWidget.HasTarget = this.HasTarget;
			if (this.HasTarget)
			{
				Vector2 globalPosition = this._targetWidget.GlobalPosition;
				Vector2 size = this._targetWidget.Size;
				float num = (this._targetWidget.DoNotUseCustomScale ? this._targetWidget.EventManager.Context.Scale : this._targetWidget.EventManager.Context.CustomScale);
				float num2 = this._targetWidget.ExtendCursorAreaLeft * num;
				float num3 = this._targetWidget.ExtendCursorAreaTop * num;
				float num4 = this._targetWidget.ExtendCursorAreaRight * num;
				float num5 = this._targetWidget.ExtendCursorAreaBottom * num;
				this.TargetX = globalPosition.X - num2;
				this.TargetY = globalPosition.Y - num3;
				this.TargetWidth = size.X + num2 + num4;
				this.TargetHeight = size.Y + num3 + num5;
			}
		}

		private void UpdateTargetOffsets(float ratio)
		{
			Vector2 vector = new Vector2(base.EventManager.LeftUsableAreaStart, base.EventManager.TopUsableAreaStart);
			float num;
			float num2;
			float num3;
			float num4;
			if (this.HasTarget)
			{
				num = this.TargetX - vector.X;
				num2 = this.TargetY - vector.Y;
				num3 = this.TargetWidth;
				num4 = this.TargetHeight;
			}
			else
			{
				num = this.CursorParentWidget.XOffset - (float)MathF.Floor(this.DefaultTargetlessOffset / 2f);
				num2 = this.CursorParentWidget.YOffset - (float)MathF.Floor(this.DefaultTargetlessOffset / 2f);
				num3 = this.DefaultTargetlessOffset;
				num4 = this.DefaultTargetlessOffset;
			}
			num -= this._additionalOffset;
			num2 -= this._additionalOffset;
			float num5 = 45f * base._scaleToUse;
			if (num3 < num5)
			{
				float num6 = num5;
				num += (num3 - num6) / 2f;
				num3 = num6;
			}
			if (num4 < num5)
			{
				float num7 = num5;
				num2 += (num4 - num7) / 2f;
				num4 = num7;
			}
			num3 += this._additionalOffset * 2f;
			num4 += this._additionalOffset * 2f;
			base.ScaledSuggestedWidth = MathF.Lerp(base.ScaledSuggestedWidth, num3, ratio, 1E-05f);
			base.ScaledSuggestedHeight = MathF.Lerp(base.ScaledSuggestedHeight, num4, ratio, 1E-05f);
			if (GauntletGamepadNavigationManager.Instance.IsCursorMovingForNavigation)
			{
				Vector2 vector2 = this.CursorParentWidget.CenterWidget.GlobalPosition + this.CursorParentWidget.CenterWidget.Size / 2f;
				base.ScaledPositionXOffset = vector2.X - base.ScaledSuggestedWidth / 2f - vector.X;
				base.ScaledPositionYOffset = vector2.Y - base.ScaledSuggestedHeight / 2f - vector.Y;
			}
			else
			{
				base.ScaledPositionXOffset = MathF.Lerp(base.ScaledPositionXOffset, num, ratio, 1E-05f);
				base.ScaledPositionYOffset = MathF.Lerp(base.ScaledPositionYOffset, num2, ratio, 1E-05f);
			}
			base.ScaledPositionXOffset = MathF.Clamp(base.ScaledPositionXOffset, 0f, Input.Resolution.X - num3 - vector.X * 2f);
			base.ScaledPositionYOffset = MathF.Clamp(base.ScaledPositionYOffset, 0f, Input.Resolution.Y - num4 - vector.Y * 2f);
			base.ScaledSuggestedWidth = MathF.Min(base.ScaledSuggestedWidth, Input.Resolution.X - base.ScaledPositionXOffset - vector.X * 2f);
			base.ScaledSuggestedHeight = MathF.Min(base.ScaledSuggestedHeight, Input.Resolution.Y - base.ScaledPositionYOffset - vector.Y * 2f);
		}

		private void UpdateAdditionalOffsets()
		{
			float num2;
			if (this.TargetHasAction && !this._isPressing)
			{
				float num = (MathF.Sin(this.TransitionTimer / this.ActionAnimationTime * 1.6f) + 1f) / 2f;
				num2 = MathF.Lerp(this.DefaultOffset, this.HoverOffset, num, 1E-05f) - this.DefaultOffset;
			}
			else
			{
				num2 = 0f;
			}
			float num3;
			if (this._isPressing)
			{
				num3 = (this.HasTarget ? this.PressOffset : (this.DefaultTargetlessOffset * 0.7f));
			}
			else if (this.HasTarget)
			{
				num3 = this.DefaultOffset;
			}
			else
			{
				num3 = this.DefaultTargetlessOffset;
			}
			this._additionalOffset = (num3 + num2) * base._scaleToUse;
		}

		private void ResetAnimations()
		{
			if (!this._isPressing)
			{
				this.TransitionTimer = 0f;
				this._additionalOffsetBeforeStateChange = this._additionalOffset;
			}
		}

		public GamepadCursorParentWidget CursorParentWidget
		{
			get
			{
				return this._cursorParentWidget;
			}
			set
			{
				if (value != this._cursorParentWidget)
				{
					this._cursorParentWidget = value;
					base.OnPropertyChanged<GamepadCursorParentWidget>(value, "CursorParentWidget");
				}
			}
		}

		public GamepadCursorMarkerWidget TopLeftMarker
		{
			get
			{
				return this._topLeftMarker;
			}
			set
			{
				if (value != this._topLeftMarker)
				{
					this._topLeftMarker = value;
					base.OnPropertyChanged<GamepadCursorMarkerWidget>(value, "TopLeftMarker");
				}
			}
		}

		public GamepadCursorMarkerWidget TopRightMarker
		{
			get
			{
				return this._topRightMarker;
			}
			set
			{
				if (value != this._topRightMarker)
				{
					this._topRightMarker = value;
					base.OnPropertyChanged<GamepadCursorMarkerWidget>(value, "TopRightMarker");
				}
			}
		}

		public GamepadCursorMarkerWidget BottomLeftMarker
		{
			get
			{
				return this._bottomLeftMarker;
			}
			set
			{
				if (value != this._bottomLeftMarker)
				{
					this._bottomLeftMarker = value;
					base.OnPropertyChanged<GamepadCursorMarkerWidget>(value, "BottomLeftMarker");
				}
			}
		}

		public GamepadCursorMarkerWidget BottomRightMarker
		{
			get
			{
				return this._bottomRightMarker;
			}
			set
			{
				if (value != this._bottomRightMarker)
				{
					this._bottomRightMarker = value;
					base.OnPropertyChanged<GamepadCursorMarkerWidget>(value, "BottomRightMarker");
				}
			}
		}

		public bool HasTarget
		{
			get
			{
				return this._hasTarget;
			}
			set
			{
				if (value != this._hasTarget)
				{
					this._hasTarget = value;
					base.OnPropertyChanged(value, "HasTarget");
					this.ResetAnimations();
					this._animationRatioTimer = 0f;
				}
			}
		}

		public bool TargetHasAction
		{
			get
			{
				return this._targetHasAction;
			}
			set
			{
				if (value != this._targetHasAction)
				{
					this._targetHasAction = value;
					base.OnPropertyChanged(value, "TargetHasAction");
					this.ResetAnimations();
				}
			}
		}

		public float TargetX
		{
			get
			{
				return this._targetX;
			}
			set
			{
				if (value != this._targetX)
				{
					this._targetX = value;
					base.OnPropertyChanged(value, "TargetX");
					this.ResetAnimations();
					this._targetPositionChangedThisFrame = true;
				}
			}
		}

		public float TargetY
		{
			get
			{
				return this._targetY;
			}
			set
			{
				if (value != this._targetY)
				{
					this._targetY = value;
					base.OnPropertyChanged(value, "TargetY");
					this.ResetAnimations();
					this._targetPositionChangedThisFrame = true;
				}
			}
		}

		public float TargetWidth
		{
			get
			{
				return this._targetWidth;
			}
			set
			{
				if (value != this._targetWidth)
				{
					this._targetWidth = value;
					base.OnPropertyChanged(value, "TargetWidth");
					this.ResetAnimations();
				}
			}
		}

		public float TargetHeight
		{
			get
			{
				return this._targetHeight;
			}
			set
			{
				if (value != this._targetHeight)
				{
					this._targetHeight = value;
					base.OnPropertyChanged(value, "TargetHeight");
					this.ResetAnimations();
				}
			}
		}

		public float DefaultOffset
		{
			get
			{
				return this._defaultOffset;
			}
			set
			{
				if (value != this._defaultOffset)
				{
					this._defaultOffset = value;
					base.OnPropertyChanged(value, "DefaultOffset");
					this.ResetAnimations();
				}
			}
		}

		public float HoverOffset
		{
			get
			{
				return this._hoverOffset;
			}
			set
			{
				if (value != this._hoverOffset)
				{
					this._hoverOffset = value;
					base.OnPropertyChanged(value, "HoverOffset");
					this.ResetAnimations();
				}
			}
		}

		public float DefaultTargetlessOffset
		{
			get
			{
				return this._defaultTargetlessOffset;
			}
			set
			{
				if (value != this._defaultTargetlessOffset)
				{
					this._defaultTargetlessOffset = value;
					base.OnPropertyChanged(value, "DefaultTargetlessOffset");
					this.ResetAnimations();
				}
			}
		}

		public float PressOffset
		{
			get
			{
				return this._pressOffset;
			}
			set
			{
				if (value != this._pressOffset)
				{
					this._pressOffset = value;
					base.OnPropertyChanged(value, "PressOffset");
					this.ResetAnimations();
				}
			}
		}

		public float ActionAnimationTime
		{
			get
			{
				return this._actionAnimationTime;
			}
			set
			{
				if (value != this._actionAnimationTime)
				{
					this._actionAnimationTime = value;
					base.OnPropertyChanged(value, "ActionAnimationTime");
					this.ResetAnimations();
				}
			}
		}

		private Widget _targetWidget;

		private bool _targetChangedThisFrame;

		private bool _targetPositionChangedThisFrame;

		private float _animationRatio;

		private float _animationRatioTimer;

		protected bool _isPressing;

		protected bool _areBrushesValidated;

		protected float _additionalOffset;

		protected float _additionalOffsetBeforeStateChange;

		protected float _leftOffset;

		protected float _rightOffset;

		protected float _topOffset;

		protected float _bottomOffset;

		private GamepadCursorParentWidget _cursorParentWidget;

		private GamepadCursorMarkerWidget _topLeftMarker;

		private GamepadCursorMarkerWidget _topRightMarker;

		private GamepadCursorMarkerWidget _bottomLeftMarker;

		private GamepadCursorMarkerWidget _bottomRightMarker;

		private bool _hasTarget;

		private bool _targetHasAction;

		private float _targetX;

		private float _targetY;

		private float _targetWidth;

		private float _targetHeight;

		private float _defaultOffset;

		private float _hoverOffset;

		private float _defaultTargetlessOffset;

		private float _pressOffset;

		private float _actionAnimationTime;
	}
}
