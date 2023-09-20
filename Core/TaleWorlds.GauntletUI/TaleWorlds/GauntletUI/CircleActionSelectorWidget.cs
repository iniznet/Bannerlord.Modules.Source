using System;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;

namespace TaleWorlds.GauntletUI
{
	// Token: 0x02000018 RID: 24
	public class CircleActionSelectorWidget : Widget
	{
		// Token: 0x17000097 RID: 151
		// (get) Token: 0x060001C4 RID: 452 RVA: 0x0000A7C9 File Offset: 0x000089C9
		// (set) Token: 0x060001C5 RID: 453 RVA: 0x0000A7D1 File Offset: 0x000089D1
		public float DistanceFromCenterModifier { get; set; } = 300f;

		// Token: 0x17000098 RID: 152
		// (get) Token: 0x060001C6 RID: 454 RVA: 0x0000A7DA File Offset: 0x000089DA
		// (set) Token: 0x060001C7 RID: 455 RVA: 0x0000A7E2 File Offset: 0x000089E2
		public Widget DirectionWidget { get; set; }

		// Token: 0x17000099 RID: 153
		// (get) Token: 0x060001C8 RID: 456 RVA: 0x0000A7EB File Offset: 0x000089EB
		// (set) Token: 0x060001C9 RID: 457 RVA: 0x0000A7F3 File Offset: 0x000089F3
		public float DirectionWidgetDistanceMultiplier { get; set; } = 0.5f;

		// Token: 0x1700009A RID: 154
		// (get) Token: 0x060001CA RID: 458 RVA: 0x0000A7FC File Offset: 0x000089FC
		// (set) Token: 0x060001CB RID: 459 RVA: 0x0000A804 File Offset: 0x00008A04
		public bool ActivateOnlyWithController { get; set; }

		// Token: 0x060001CC RID: 460 RVA: 0x0000A80D File Offset: 0x00008A0D
		public CircleActionSelectorWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x060001CD RID: 461 RVA: 0x0000A82C File Offset: 0x00008A2C
		protected override void OnChildAdded(Widget child)
		{
			base.OnChildAdded(child);
			child.boolPropertyChanged += this.OnChildPropertyChanged;
		}

		// Token: 0x060001CE RID: 462 RVA: 0x0000A847 File Offset: 0x00008A47
		private void OnChildPropertyChanged(PropertyOwnerObject widget, string propertyName, bool value)
		{
			if (propertyName == "IsSelected" && base.EventManager.IsControllerActive && !this._isRefreshingSelection)
			{
				this._mouseDirection = Vec2.Zero;
				this._mouseMoveAccumulated = Vec2.Zero;
			}
		}

		// Token: 0x060001CF RID: 463 RVA: 0x0000A884 File Offset: 0x00008A84
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

		// Token: 0x060001D0 RID: 464 RVA: 0x0000A918 File Offset: 0x00008B18
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

		// Token: 0x060001D1 RID: 465 RVA: 0x0000A9E8 File Offset: 0x00008BE8
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

		// Token: 0x060001D2 RID: 466 RVA: 0x0000AAA4 File Offset: 0x00008CA4
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

		// Token: 0x060001D3 RID: 467 RVA: 0x0000AB7C File Offset: 0x00008D7C
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

		// Token: 0x060001D4 RID: 468 RVA: 0x0000AC04 File Offset: 0x00008E04
		private float AddAngle(float angle1, float angle2)
		{
			float num = angle1 + angle2;
			if (num < 0f)
			{
				num += 360f;
			}
			return num % 360f;
		}

		// Token: 0x060001D5 RID: 469 RVA: 0x0000AC30 File Offset: 0x00008E30
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

		// Token: 0x060001D6 RID: 470 RVA: 0x0000ACA8 File Offset: 0x00008EA8
		private float AngleFromDir(Vec2 directionVector)
		{
			if (directionVector.X < 0f)
			{
				return 360f - (float)Math.Atan2((double)directionVector.X, (double)directionVector.Y) * 57.29578f * -1f;
			}
			return (float)Math.Atan2((double)directionVector.X, (double)directionVector.Y) * 57.29578f;
		}

		// Token: 0x060001D7 RID: 471 RVA: 0x0000AD08 File Offset: 0x00008F08
		private Vec2 DirFromAngle(float angle)
		{
			return new Vec2(MathF.Sin(angle), MathF.Cos(angle));
		}

		// Token: 0x040000FA RID: 250
		private const float _mouseMoveMaxDistance = 125f;

		// Token: 0x040000FB RID: 251
		private const float _gamepadDeadzoneLength = 0.391f;

		// Token: 0x040000FC RID: 252
		private const float _mouseMoveMaxDistanceSquared = 15625f;

		// Token: 0x040000FD RID: 253
		private Vec2 _mouseDirection;

		// Token: 0x040000FE RID: 254
		private Vec2 _mouseMoveAccumulated;

		// Token: 0x040000FF RID: 255
		private bool _isRefreshingSelection;
	}
}
