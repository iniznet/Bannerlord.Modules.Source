using System;
using System.Numerics;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.GauntletUI.GamepadNavigation;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets
{
	// Token: 0x0200001E RID: 30
	public class GamepadCursorWidget : BrushWidget
	{
		// Token: 0x17000077 RID: 119
		// (get) Token: 0x06000161 RID: 353 RVA: 0x00005D11 File Offset: 0x00003F11
		// (set) Token: 0x06000162 RID: 354 RVA: 0x00005D19 File Offset: 0x00003F19
		private protected float TransitionTimer { protected get; private set; }

		// Token: 0x06000163 RID: 355 RVA: 0x00005D22 File Offset: 0x00003F22
		public GamepadCursorWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000164 RID: 356 RVA: 0x00005D2C File Offset: 0x00003F2C
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

		// Token: 0x06000165 RID: 357 RVA: 0x00005E48 File Offset: 0x00004048
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

		// Token: 0x06000166 RID: 358 RVA: 0x00005F8C File Offset: 0x0000418C
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

		// Token: 0x06000167 RID: 359 RVA: 0x0000625C File Offset: 0x0000445C
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

		// Token: 0x06000168 RID: 360 RVA: 0x00006314 File Offset: 0x00004514
		private void ResetAnimations()
		{
			if (!this._isPressing)
			{
				this.TransitionTimer = 0f;
				this._additionalOffsetBeforeStateChange = this._additionalOffset;
			}
		}

		// Token: 0x17000078 RID: 120
		// (get) Token: 0x06000169 RID: 361 RVA: 0x00006335 File Offset: 0x00004535
		// (set) Token: 0x0600016A RID: 362 RVA: 0x0000633D File Offset: 0x0000453D
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

		// Token: 0x17000079 RID: 121
		// (get) Token: 0x0600016B RID: 363 RVA: 0x0000635B File Offset: 0x0000455B
		// (set) Token: 0x0600016C RID: 364 RVA: 0x00006363 File Offset: 0x00004563
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

		// Token: 0x1700007A RID: 122
		// (get) Token: 0x0600016D RID: 365 RVA: 0x00006381 File Offset: 0x00004581
		// (set) Token: 0x0600016E RID: 366 RVA: 0x00006389 File Offset: 0x00004589
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

		// Token: 0x1700007B RID: 123
		// (get) Token: 0x0600016F RID: 367 RVA: 0x000063A7 File Offset: 0x000045A7
		// (set) Token: 0x06000170 RID: 368 RVA: 0x000063AF File Offset: 0x000045AF
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

		// Token: 0x1700007C RID: 124
		// (get) Token: 0x06000171 RID: 369 RVA: 0x000063CD File Offset: 0x000045CD
		// (set) Token: 0x06000172 RID: 370 RVA: 0x000063D5 File Offset: 0x000045D5
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

		// Token: 0x1700007D RID: 125
		// (get) Token: 0x06000173 RID: 371 RVA: 0x000063F3 File Offset: 0x000045F3
		// (set) Token: 0x06000174 RID: 372 RVA: 0x000063FB File Offset: 0x000045FB
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

		// Token: 0x1700007E RID: 126
		// (get) Token: 0x06000175 RID: 373 RVA: 0x0000642A File Offset: 0x0000462A
		// (set) Token: 0x06000176 RID: 374 RVA: 0x00006432 File Offset: 0x00004632
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

		// Token: 0x1700007F RID: 127
		// (get) Token: 0x06000177 RID: 375 RVA: 0x00006456 File Offset: 0x00004656
		// (set) Token: 0x06000178 RID: 376 RVA: 0x0000645E File Offset: 0x0000465E
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

		// Token: 0x17000080 RID: 128
		// (get) Token: 0x06000179 RID: 377 RVA: 0x00006489 File Offset: 0x00004689
		// (set) Token: 0x0600017A RID: 378 RVA: 0x00006491 File Offset: 0x00004691
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

		// Token: 0x17000081 RID: 129
		// (get) Token: 0x0600017B RID: 379 RVA: 0x000064BC File Offset: 0x000046BC
		// (set) Token: 0x0600017C RID: 380 RVA: 0x000064C4 File Offset: 0x000046C4
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

		// Token: 0x17000082 RID: 130
		// (get) Token: 0x0600017D RID: 381 RVA: 0x000064E8 File Offset: 0x000046E8
		// (set) Token: 0x0600017E RID: 382 RVA: 0x000064F0 File Offset: 0x000046F0
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

		// Token: 0x17000083 RID: 131
		// (get) Token: 0x0600017F RID: 383 RVA: 0x00006514 File Offset: 0x00004714
		// (set) Token: 0x06000180 RID: 384 RVA: 0x0000651C File Offset: 0x0000471C
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

		// Token: 0x17000084 RID: 132
		// (get) Token: 0x06000181 RID: 385 RVA: 0x00006540 File Offset: 0x00004740
		// (set) Token: 0x06000182 RID: 386 RVA: 0x00006548 File Offset: 0x00004748
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

		// Token: 0x17000085 RID: 133
		// (get) Token: 0x06000183 RID: 387 RVA: 0x0000656C File Offset: 0x0000476C
		// (set) Token: 0x06000184 RID: 388 RVA: 0x00006574 File Offset: 0x00004774
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

		// Token: 0x17000086 RID: 134
		// (get) Token: 0x06000185 RID: 389 RVA: 0x00006598 File Offset: 0x00004798
		// (set) Token: 0x06000186 RID: 390 RVA: 0x000065A0 File Offset: 0x000047A0
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

		// Token: 0x17000087 RID: 135
		// (get) Token: 0x06000187 RID: 391 RVA: 0x000065C4 File Offset: 0x000047C4
		// (set) Token: 0x06000188 RID: 392 RVA: 0x000065CC File Offset: 0x000047CC
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

		// Token: 0x040000A8 RID: 168
		private Widget _targetWidget;

		// Token: 0x040000A9 RID: 169
		private bool _targetChangedThisFrame;

		// Token: 0x040000AA RID: 170
		private bool _targetPositionChangedThisFrame;

		// Token: 0x040000AB RID: 171
		private float _animationRatio;

		// Token: 0x040000AC RID: 172
		private float _animationRatioTimer;

		// Token: 0x040000AD RID: 173
		protected bool _isPressing;

		// Token: 0x040000AE RID: 174
		protected bool _areBrushesValidated;

		// Token: 0x040000B0 RID: 176
		protected float _additionalOffset;

		// Token: 0x040000B1 RID: 177
		protected float _additionalOffsetBeforeStateChange;

		// Token: 0x040000B2 RID: 178
		protected float _leftOffset;

		// Token: 0x040000B3 RID: 179
		protected float _rightOffset;

		// Token: 0x040000B4 RID: 180
		protected float _topOffset;

		// Token: 0x040000B5 RID: 181
		protected float _bottomOffset;

		// Token: 0x040000B6 RID: 182
		private GamepadCursorParentWidget _cursorParentWidget;

		// Token: 0x040000B7 RID: 183
		private GamepadCursorMarkerWidget _topLeftMarker;

		// Token: 0x040000B8 RID: 184
		private GamepadCursorMarkerWidget _topRightMarker;

		// Token: 0x040000B9 RID: 185
		private GamepadCursorMarkerWidget _bottomLeftMarker;

		// Token: 0x040000BA RID: 186
		private GamepadCursorMarkerWidget _bottomRightMarker;

		// Token: 0x040000BB RID: 187
		private bool _hasTarget;

		// Token: 0x040000BC RID: 188
		private bool _targetHasAction;

		// Token: 0x040000BD RID: 189
		private float _targetX;

		// Token: 0x040000BE RID: 190
		private float _targetY;

		// Token: 0x040000BF RID: 191
		private float _targetWidth;

		// Token: 0x040000C0 RID: 192
		private float _targetHeight;

		// Token: 0x040000C1 RID: 193
		private float _defaultOffset;

		// Token: 0x040000C2 RID: 194
		private float _hoverOffset;

		// Token: 0x040000C3 RID: 195
		private float _defaultTargetlessOffset;

		// Token: 0x040000C4 RID: 196
		private float _pressOffset;

		// Token: 0x040000C5 RID: 197
		private float _actionAnimationTime;
	}
}
