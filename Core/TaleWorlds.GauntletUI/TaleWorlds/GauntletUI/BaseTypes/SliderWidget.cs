using System;
using System.Numerics;
using TaleWorlds.GauntletUI.GamepadNavigation;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.GauntletUI.BaseTypes
{
	// Token: 0x02000067 RID: 103
	public class SliderWidget : ImageWidget
	{
		// Token: 0x170001E7 RID: 487
		// (get) Token: 0x060006A0 RID: 1696 RVA: 0x0001D6DE File Offset: 0x0001B8DE
		// (set) Token: 0x060006A1 RID: 1697 RVA: 0x0001D6E6 File Offset: 0x0001B8E6
		public bool UpdateValueOnScroll { get; set; }

		// Token: 0x170001E8 RID: 488
		// (get) Token: 0x060006A2 RID: 1698 RVA: 0x0001D6EF File Offset: 0x0001B8EF
		// (set) Token: 0x060006A3 RID: 1699 RVA: 0x0001D6F7 File Offset: 0x0001B8F7
		public bool DPadMovementEnabled { get; set; } = true;

		// Token: 0x170001E9 RID: 489
		// (get) Token: 0x060006A4 RID: 1700 RVA: 0x0001D700 File Offset: 0x0001B900
		private float _holdTimeToStartMovement
		{
			get
			{
				return 0.3f;
			}
		}

		// Token: 0x170001EA RID: 490
		// (get) Token: 0x060006A5 RID: 1701 RVA: 0x0001D707 File Offset: 0x0001B907
		private float _dynamicIncrement
		{
			get
			{
				if (this.MaxValueFloat - this.MinValueFloat <= 2f)
				{
					return 0.1f;
				}
				return 1f;
			}
		}

		// Token: 0x060006A6 RID: 1702 RVA: 0x0001D728 File Offset: 0x0001B928
		public SliderWidget(UIContext context)
			: base(context)
		{
			this.SliderArea = this;
			this._firstFrame = true;
			base.FrictionEnabled = true;
			base.UsedNavigationMovements = GamepadNavigationTypes.Horizontal;
		}

		// Token: 0x060006A7 RID: 1703 RVA: 0x0001D768 File Offset: 0x0001B968
		protected override void OnUpdate(float dt)
		{
			base.OnUpdate(dt);
			bool flag = false;
			base.IsUsingNavigation = false;
			if (!base.IsPressed)
			{
				Widget handle = this.Handle;
				if (handle == null || !handle.IsPressed)
				{
					Widget handleExtension = this.HandleExtension;
					if (handleExtension == null || !handleExtension.IsPressed)
					{
						this._downStartTime = -1f;
						this._handleClickOffset = Vector2.Zero;
						this._handleClicked = false;
						this._valueChangedByMouse = false;
						goto IL_1D5;
					}
				}
			}
			if (base.EventManager.IsControllerActive && base.IsRecursivelyVisible() && base.EventManager.GetIsHitThisFrame())
			{
				float num = 0f;
				if (Input.IsKeyDown(InputKey.ControllerLLeft))
				{
					num = -1f;
				}
				else if (Input.IsKeyDown(InputKey.ControllerLRight))
				{
					num = 1f;
				}
				if (num != 0f)
				{
					num *= (this.IsDiscrete ? ((float)this.DiscreteIncrementInterval) : this._dynamicIncrement);
					if (this._downStartTime == -1f)
					{
						this._downStartTime = base.Context.EventManager.Time;
						this.ValueFloat = MathF.Clamp(this._valueFloat + num, this.MinValueFloat, this.MaxValueFloat);
						flag = true;
					}
					else if (this._holdTimeToStartMovement < base.Context.EventManager.Time - this._downStartTime)
					{
						this.ValueFloat = MathF.Clamp(this._valueFloat + num, this.MinValueFloat, this.MaxValueFloat);
						flag = true;
					}
				}
				else
				{
					this._downStartTime = -1f;
				}
				base.IsUsingNavigation = true;
			}
			if (!this._handleClicked)
			{
				this._handleClicked = true;
				this.UpdateLocalClickPosition();
				this._handleClickOffset = base.EventManager.MousePosition - (this.Handle.GlobalPosition + this.Handle.Size * 0.5f);
			}
			this.HandleMouseMove();
			IL_1D5:
			this.UpdateScrollBar();
			this.UpdateHandleLength();
			Widget handle2 = this.Handle;
			if (handle2 != null)
			{
				handle2.SetState(base.CurrentState);
			}
			if (this._snapCursorToHandle)
			{
				Vector2 vector = this.Handle.GlobalPosition + this.Handle.Size / 2f;
				Input.SetMousePosition((int)vector.X, (int)vector.Y);
				base.EventManager.UpdateMousePosition(vector);
				this._snapCursorToHandle = false;
			}
			if (flag && Input.MouseMoveX == 0f && Input.MouseMoveY == 0f)
			{
				this._snapCursorToHandle = true;
			}
			this._firstFrame = false;
		}

		// Token: 0x060006A8 RID: 1704 RVA: 0x0001D9EC File Offset: 0x0001BBEC
		protected override void OnParallelUpdate(float dt)
		{
			base.OnParallelUpdate(dt);
			if (this.Filler != null)
			{
				float num = 1f;
				if (MathF.Abs(this.MaxValueFloat - this.MinValueFloat) > 1E-45f)
				{
					num = (this._valueFloat - this.MinValueFloat) / (this.MaxValueFloat - this.MinValueFloat);
				}
				this.Filler.HorizontalAlignment = HorizontalAlignment.Left;
				if (this.AlignmentAxis == AlignmentAxis.Horizontal)
				{
					this.Filler.WidthSizePolicy = SizePolicy.Fixed;
					this.Filler.ScaledSuggestedWidth = this.SliderArea.Size.X * num;
				}
				else
				{
					this.Filler.HeightSizePolicy = SizePolicy.Fixed;
					this.Filler.ScaledSuggestedHeight = this.SliderArea.Size.Y * num;
				}
				this.Filler.DoNotAcceptEvents = true;
				this.Filler.DoNotPassEventsToChildren = true;
			}
		}

		// Token: 0x060006A9 RID: 1705 RVA: 0x0001DAC6 File Offset: 0x0001BCC6
		protected internal override void OnMousePressed()
		{
			if (this.Handle != null && this.Handle.IsVisible)
			{
				base.IsPressed = true;
				this.UpdateLocalClickPosition();
				base.OnPropertyChanged<string>("MouseDown", "OnMousePressed");
				this.HandleMouseMove();
			}
		}

		// Token: 0x060006AA RID: 1706 RVA: 0x0001DB00 File Offset: 0x0001BD00
		protected internal override void OnMouseReleased()
		{
			if (this.Handle != null)
			{
				base.IsPressed = false;
				if (this.UpdateValueOnRelease)
				{
					base.OnPropertyChanged(this._valueFloat, "ValueFloat");
					base.OnPropertyChanged(this.ValueInt, "ValueInt");
					this.OnValueFloatChanged(this._valueFloat);
				}
			}
		}

		// Token: 0x060006AB RID: 1707 RVA: 0x0001DB52 File Offset: 0x0001BD52
		protected internal override void OnMouseMove()
		{
			base.OnMouseMove();
			if (base.IsPressed)
			{
				this.HandleMouseMove();
			}
		}

		// Token: 0x060006AC RID: 1708 RVA: 0x0001DB68 File Offset: 0x0001BD68
		protected internal virtual void OnValueIntChanged(int value)
		{
		}

		// Token: 0x060006AD RID: 1709 RVA: 0x0001DB6A File Offset: 0x0001BD6A
		protected internal virtual void OnValueFloatChanged(float value)
		{
		}

		// Token: 0x060006AE RID: 1710 RVA: 0x0001DB6C File Offset: 0x0001BD6C
		private void UpdateScrollBar()
		{
			if (!this._firstFrame && base.IsVisible)
			{
				this.UpdateHandleByValue();
			}
		}

		// Token: 0x060006AF RID: 1711 RVA: 0x0001DB84 File Offset: 0x0001BD84
		private void UpdateLocalClickPosition()
		{
			Vector2 mousePosition = base.EventManager.MousePosition;
			this._localClickPos = mousePosition - this.Handle.GlobalPosition;
			if (this._localClickPos.X < 0f || this._localClickPos.X > this.Handle.Size.X)
			{
				this._localClickPos.X = this.Handle.Size.X / 2f;
			}
			if (this._localClickPos.Y < -5f)
			{
				this._localClickPos.Y = -5f;
				return;
			}
			if (this._localClickPos.Y > this.Handle.Size.Y + 5f)
			{
				this._localClickPos.Y = this.Handle.Size.Y + 5f;
			}
		}

		// Token: 0x060006B0 RID: 1712 RVA: 0x0001DC6C File Offset: 0x0001BE6C
		private void HandleMouseMove()
		{
			if (base.EventManager.IsControllerActive && Input.MouseMoveX == 0f && Input.MouseMoveY == 0f)
			{
				return;
			}
			if (this.Handle != null)
			{
				Vector2 vector = base.EventManager.MousePosition - this._localClickPos;
				float num = this.GetValue(vector, this.AlignmentAxis);
				float num2;
				float num3;
				if (this.AlignmentAxis == AlignmentAxis.Horizontal)
				{
					float x = base.ParentWidget.GlobalPosition.X;
					num2 = x + base.Left;
					num3 = x + base.Right;
					num3 -= this.Handle.Size.X;
					Widget handleExtension = this.HandleExtension;
					if (handleExtension != null && handleExtension.IsPressed)
					{
						num -= this._handleClickOffset.X;
					}
				}
				else
				{
					float y = base.ParentWidget.GlobalPosition.Y;
					num2 = y + base.Top;
					num3 = y + base.Bottom;
					num3 -= this.Handle.Size.Y;
				}
				if (Mathf.Abs(num3 - num2) < 1E-05f)
				{
					this.ValueFloat = 0f;
					return;
				}
				if (num < num2)
				{
					num = num2;
				}
				if (num > num3)
				{
					num = num3;
				}
				float num4 = (num - num2) / (num3 - num2);
				this._valueChangedByMouse = true;
				this.ValueFloat = this.MinValueFloat + (this.MaxValueFloat - this.MinValueFloat) * num4;
			}
		}

		// Token: 0x060006B1 RID: 1713 RVA: 0x0001DDB8 File Offset: 0x0001BFB8
		private void UpdateHandleByValue()
		{
			if (this._valueFloat < this.MinValueFloat)
			{
				this.ValueFloat = this.MinValueFloat;
			}
			if (this._valueFloat > this.MaxValueFloat)
			{
				this.ValueFloat = this.MaxValueFloat;
			}
			float num = 1f;
			if (MathF.Abs(this.MaxValueFloat - this.MinValueFloat) > 1E-45f)
			{
				num = (this._valueFloat - this.MinValueFloat) / (this.MaxValueFloat - this.MinValueFloat);
				if (this.ReverseDirection)
				{
					num = 1f - num;
				}
			}
			if (this.Handle != null)
			{
				if (this.AlignmentAxis == AlignmentAxis.Horizontal)
				{
					this.Handle.HorizontalAlignment = HorizontalAlignment.Left;
					this.Handle.VerticalAlignment = VerticalAlignment.Center;
					float num2 = this.SliderArea.Size.X;
					num2 -= this.Handle.Size.X;
					this.Handle.ScaledPositionXOffset = num2 * num;
					this.Handle.ScaledPositionYOffset = 0f;
				}
				else
				{
					this.Handle.HorizontalAlignment = HorizontalAlignment.Center;
					this.Handle.VerticalAlignment = VerticalAlignment.Bottom;
					float num3 = this.SliderArea.Size.Y;
					num3 -= this.Handle.Size.Y;
					this.Handle.ScaledPositionYOffset = -1f * num3 * (1f - num);
					this.Handle.ScaledPositionXOffset = 0f;
				}
				if (this.HandleExtension != null)
				{
					this.HandleExtension.HorizontalAlignment = this.Handle.HorizontalAlignment;
					this.HandleExtension.VerticalAlignment = this.Handle.VerticalAlignment;
					this.HandleExtension.ScaledPositionXOffset = this.Handle.ScaledPositionXOffset;
					this.HandleExtension.ScaledPositionYOffset = this.Handle.ScaledPositionYOffset;
				}
			}
		}

		// Token: 0x060006B2 RID: 1714 RVA: 0x0001DF7C File Offset: 0x0001C17C
		private void UpdateHandleLength()
		{
			if (!this.DoNotUpdateHandleSize && this.IsDiscrete && this.Handle.WidthSizePolicy == SizePolicy.Fixed)
			{
				if (this.AlignmentAxis == AlignmentAxis.Horizontal)
				{
					this.Handle.SuggestedWidth = Mathf.Clamp(base.SuggestedWidth / (this.MaxValueFloat + 1f), 50f, base.SuggestedWidth / 2f);
					return;
				}
				if (this.AlignmentAxis == AlignmentAxis.Vertical)
				{
					this.Handle.SuggestedHeight = Mathf.Clamp(base.SuggestedHeight / (this.MaxValueFloat + 1f), 50f, base.SuggestedHeight / 2f);
				}
			}
		}

		// Token: 0x060006B3 RID: 1715 RVA: 0x0001E026 File Offset: 0x0001C226
		private float GetValue(Vector2 value, AlignmentAxis alignmentAxis)
		{
			if (alignmentAxis == AlignmentAxis.Horizontal)
			{
				return value.X;
			}
			return value.Y;
		}

		// Token: 0x060006B4 RID: 1716 RVA: 0x0001E038 File Offset: 0x0001C238
		protected override bool OnPreviewMouseScroll()
		{
			if (this.UpdateValueOnScroll)
			{
				float num = base.EventManager.DeltaMouseScroll * 0.004f;
				this.ValueFloat = MathF.Clamp(this._valueFloat + this._dynamicIncrement * num, this.MinValueFloat, this.MaxValueFloat);
			}
			return base.OnPreviewMouseScroll();
		}

		// Token: 0x170001EB RID: 491
		// (get) Token: 0x060006B5 RID: 1717 RVA: 0x0001E08B File Offset: 0x0001C28B
		// (set) Token: 0x060006B6 RID: 1718 RVA: 0x0001E093 File Offset: 0x0001C293
		[Editor(false)]
		public bool IsDiscrete
		{
			get
			{
				return this._isDiscrete;
			}
			set
			{
				if (this._isDiscrete != value)
				{
					this._isDiscrete = value;
					base.OnPropertyChanged(this._isDiscrete, "IsDiscrete");
				}
			}
		}

		// Token: 0x170001EC RID: 492
		// (get) Token: 0x060006B7 RID: 1719 RVA: 0x0001E0B6 File Offset: 0x0001C2B6
		// (set) Token: 0x060006B8 RID: 1720 RVA: 0x0001E0BE File Offset: 0x0001C2BE
		[Editor(false)]
		public bool Locked
		{
			get
			{
				return this._locked;
			}
			set
			{
				if (this._locked != value)
				{
					this._locked = value;
					base.OnPropertyChanged(this._locked, "Locked");
				}
			}
		}

		// Token: 0x170001ED RID: 493
		// (get) Token: 0x060006B9 RID: 1721 RVA: 0x0001E0E1 File Offset: 0x0001C2E1
		// (set) Token: 0x060006BA RID: 1722 RVA: 0x0001E0E9 File Offset: 0x0001C2E9
		[Editor(false)]
		public bool UpdateValueOnRelease
		{
			get
			{
				return this._updateValueOnRelease;
			}
			set
			{
				if (this._updateValueOnRelease != value)
				{
					this._updateValueOnRelease = value;
					base.OnPropertyChanged(this._updateValueOnRelease, "UpdateValueOnRelease");
				}
			}
		}

		// Token: 0x170001EE RID: 494
		// (get) Token: 0x060006BB RID: 1723 RVA: 0x0001E10C File Offset: 0x0001C30C
		// (set) Token: 0x060006BC RID: 1724 RVA: 0x0001E117 File Offset: 0x0001C317
		[Editor(false)]
		public bool UpdateValueContinuously
		{
			get
			{
				return !this._updateValueOnRelease;
			}
			set
			{
				if (this.UpdateValueContinuously != value)
				{
					this._updateValueOnRelease = !value;
					base.OnPropertyChanged(this._updateValueOnRelease, "UpdateValueContinuously");
				}
			}
		}

		// Token: 0x170001EF RID: 495
		// (get) Token: 0x060006BD RID: 1725 RVA: 0x0001E13D File Offset: 0x0001C33D
		// (set) Token: 0x060006BE RID: 1726 RVA: 0x0001E145 File Offset: 0x0001C345
		[Editor(false)]
		public AlignmentAxis AlignmentAxis { get; set; }

		// Token: 0x170001F0 RID: 496
		// (get) Token: 0x060006BF RID: 1727 RVA: 0x0001E14E File Offset: 0x0001C34E
		// (set) Token: 0x060006C0 RID: 1728 RVA: 0x0001E156 File Offset: 0x0001C356
		[Editor(false)]
		public bool ReverseDirection { get; set; }

		// Token: 0x170001F1 RID: 497
		// (get) Token: 0x060006C1 RID: 1729 RVA: 0x0001E15F File Offset: 0x0001C35F
		// (set) Token: 0x060006C2 RID: 1730 RVA: 0x0001E167 File Offset: 0x0001C367
		[Editor(false)]
		public Widget Filler { get; set; }

		// Token: 0x170001F2 RID: 498
		// (get) Token: 0x060006C3 RID: 1731 RVA: 0x0001E170 File Offset: 0x0001C370
		// (set) Token: 0x060006C4 RID: 1732 RVA: 0x0001E178 File Offset: 0x0001C378
		[Editor(false)]
		public Widget HandleExtension { get; set; }

		// Token: 0x170001F3 RID: 499
		// (get) Token: 0x060006C5 RID: 1733 RVA: 0x0001E181 File Offset: 0x0001C381
		// (set) Token: 0x060006C6 RID: 1734 RVA: 0x0001E18C File Offset: 0x0001C38C
		[Editor(false)]
		public float ValueFloat
		{
			get
			{
				return this._valueFloat;
			}
			set
			{
				if (!this.Locked && MathF.Abs(this._valueFloat - value) > 1E-05f)
				{
					float valueFloat = this._valueFloat;
					if (this.MinValueFloat <= this.MaxValueFloat)
					{
						if (this._valueFloat < this.MinValueFloat)
						{
							this._valueFloat = this.MinValueFloat;
						}
						if (this._valueFloat > this.MaxValueFloat)
						{
							this._valueFloat = this.MaxValueFloat;
						}
						if (this.IsDiscrete)
						{
							if (value >= (float)this.MaxValueInt)
							{
								this._valueFloat = (float)this.MaxValueInt;
							}
							else
							{
								float num = Mathf.Floor((value - (float)this.MinValueInt) / (float)this.DiscreteIncrementInterval);
								this._valueFloat = num * (float)this.DiscreteIncrementInterval + (float)this.MinValueInt;
							}
						}
						else
						{
							this._valueFloat = value;
						}
						this.UpdateHandleByValue();
						if (MathF.Abs(this._valueFloat - valueFloat) > 1E-05f && ((this.UpdateValueOnRelease && !base.IsPressed) || !this.UpdateValueOnRelease))
						{
							base.OnPropertyChanged(this._valueFloat, "ValueFloat");
							base.OnPropertyChanged(this.ValueInt, "ValueInt");
							this.OnValueFloatChanged(this._valueFloat);
						}
					}
				}
			}
		}

		// Token: 0x170001F4 RID: 500
		// (get) Token: 0x060006C7 RID: 1735 RVA: 0x0001E2BF File Offset: 0x0001C4BF
		// (set) Token: 0x060006C8 RID: 1736 RVA: 0x0001E2CC File Offset: 0x0001C4CC
		[Editor(false)]
		public int ValueInt
		{
			get
			{
				return MathF.Round(this.ValueFloat);
			}
			set
			{
				this.ValueFloat = (float)value;
				this.OnValueIntChanged(this.ValueInt);
			}
		}

		// Token: 0x170001F5 RID: 501
		// (get) Token: 0x060006C9 RID: 1737 RVA: 0x0001E2E2 File Offset: 0x0001C4E2
		// (set) Token: 0x060006CA RID: 1738 RVA: 0x0001E2EA File Offset: 0x0001C4EA
		[Editor(false)]
		public float MinValueFloat { get; set; }

		// Token: 0x170001F6 RID: 502
		// (get) Token: 0x060006CB RID: 1739 RVA: 0x0001E2F3 File Offset: 0x0001C4F3
		// (set) Token: 0x060006CC RID: 1740 RVA: 0x0001E2FB File Offset: 0x0001C4FB
		[Editor(false)]
		public float MaxValueFloat { get; set; }

		// Token: 0x170001F7 RID: 503
		// (get) Token: 0x060006CD RID: 1741 RVA: 0x0001E304 File Offset: 0x0001C504
		// (set) Token: 0x060006CE RID: 1742 RVA: 0x0001E311 File Offset: 0x0001C511
		[Editor(false)]
		public int MinValueInt
		{
			get
			{
				return MathF.Round(this.MinValueFloat);
			}
			set
			{
				this.MinValueFloat = (float)value;
			}
		}

		// Token: 0x170001F8 RID: 504
		// (get) Token: 0x060006CF RID: 1743 RVA: 0x0001E31B File Offset: 0x0001C51B
		// (set) Token: 0x060006D0 RID: 1744 RVA: 0x0001E328 File Offset: 0x0001C528
		[Editor(false)]
		public int MaxValueInt
		{
			get
			{
				return MathF.Round(this.MaxValueFloat);
			}
			set
			{
				this.MaxValueFloat = (float)value;
			}
		}

		// Token: 0x170001F9 RID: 505
		// (get) Token: 0x060006D1 RID: 1745 RVA: 0x0001E332 File Offset: 0x0001C532
		// (set) Token: 0x060006D2 RID: 1746 RVA: 0x0001E33A File Offset: 0x0001C53A
		public int DiscreteIncrementInterval { get; set; } = 1;

		// Token: 0x170001FA RID: 506
		// (get) Token: 0x060006D3 RID: 1747 RVA: 0x0001E343 File Offset: 0x0001C543
		// (set) Token: 0x060006D4 RID: 1748 RVA: 0x0001E34B File Offset: 0x0001C54B
		[Editor(false)]
		public bool DoNotUpdateHandleSize { get; set; }

		// Token: 0x170001FB RID: 507
		// (get) Token: 0x060006D5 RID: 1749 RVA: 0x0001E354 File Offset: 0x0001C554
		// (set) Token: 0x060006D6 RID: 1750 RVA: 0x0001E35C File Offset: 0x0001C55C
		[Editor(false)]
		public Widget Handle
		{
			get
			{
				return this._handle;
			}
			set
			{
				if (this._handle != value)
				{
					this._handle = value;
					this.UpdateHandleByValue();
					if (this._handle != null)
					{
						this._handle.ExtendCursorAreaLeft = 6f;
						this._handle.ExtendCursorAreaRight = 6f;
						this._handle.ExtendCursorAreaTop = 3f;
						this._handle.ExtendCursorAreaBottom = 3f;
					}
				}
			}
		}

		// Token: 0x170001FC RID: 508
		// (get) Token: 0x060006D7 RID: 1751 RVA: 0x0001E3C7 File Offset: 0x0001C5C7
		// (set) Token: 0x060006D8 RID: 1752 RVA: 0x0001E3CF File Offset: 0x0001C5CF
		[Editor(false)]
		public Widget SliderArea { get; set; }

		// Token: 0x04000328 RID: 808
		private bool _firstFrame;

		// Token: 0x04000329 RID: 809
		public float HandleRatio;

		// Token: 0x0400032A RID: 810
		protected bool _handleClicked;

		// Token: 0x0400032B RID: 811
		protected bool _valueChangedByMouse;

		// Token: 0x0400032C RID: 812
		private float _downStartTime = -1f;

		// Token: 0x0400032D RID: 813
		private Vector2 _handleClickOffset;

		// Token: 0x0400032E RID: 814
		private bool _snapCursorToHandle;

		// Token: 0x0400032F RID: 815
		private bool _locked;

		// Token: 0x04000330 RID: 816
		private bool _isDiscrete;

		// Token: 0x04000331 RID: 817
		private bool _updateValueOnRelease;

		// Token: 0x04000332 RID: 818
		private Vector2 _localClickPos;

		// Token: 0x04000333 RID: 819
		private float _valueFloat;

		// Token: 0x04000334 RID: 820
		private Widget _handle;
	}
}
