using System;
using System.Numerics;
using TaleWorlds.GauntletUI.GamepadNavigation;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.GauntletUI.BaseTypes
{
	public class SliderWidget : ImageWidget
	{
		public bool UpdateValueOnScroll { get; set; }

		public bool DPadMovementEnabled { get; set; } = true;

		private float _holdTimeToStartMovement
		{
			get
			{
				return 0.3f;
			}
		}

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

		public SliderWidget(UIContext context)
			: base(context)
		{
			this.SliderArea = this;
			this._firstFrame = true;
			base.FrictionEnabled = true;
			base.UsedNavigationMovements = GamepadNavigationTypes.Horizontal;
		}

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

		protected internal override void OnMouseMove()
		{
			base.OnMouseMove();
			if (base.IsPressed)
			{
				this.HandleMouseMove();
			}
		}

		protected internal virtual void OnValueIntChanged(int value)
		{
		}

		protected internal virtual void OnValueFloatChanged(float value)
		{
		}

		private void UpdateScrollBar()
		{
			if (!this._firstFrame && base.IsVisible)
			{
				this.UpdateHandleByValue();
			}
		}

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

		private float GetValue(Vector2 value, AlignmentAxis alignmentAxis)
		{
			if (alignmentAxis == AlignmentAxis.Horizontal)
			{
				return value.X;
			}
			return value.Y;
		}

		protected override bool OnPreviewMouseScroll()
		{
			if (this.UpdateValueOnScroll)
			{
				float num = base.EventManager.DeltaMouseScroll * 0.004f;
				this.ValueFloat = MathF.Clamp(this._valueFloat + this._dynamicIncrement * num, this.MinValueFloat, this.MaxValueFloat);
			}
			return base.OnPreviewMouseScroll();
		}

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

		[Editor(false)]
		public AlignmentAxis AlignmentAxis { get; set; }

		[Editor(false)]
		public bool ReverseDirection { get; set; }

		[Editor(false)]
		public Widget Filler { get; set; }

		[Editor(false)]
		public Widget HandleExtension { get; set; }

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

		[Editor(false)]
		public float MinValueFloat { get; set; }

		[Editor(false)]
		public float MaxValueFloat { get; set; }

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

		public int DiscreteIncrementInterval { get; set; } = 1;

		[Editor(false)]
		public bool DoNotUpdateHandleSize { get; set; }

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

		[Editor(false)]
		public Widget SliderArea { get; set; }

		private bool _firstFrame;

		public float HandleRatio;

		protected bool _handleClicked;

		protected bool _valueChangedByMouse;

		private float _downStartTime = -1f;

		private Vector2 _handleClickOffset;

		private bool _snapCursorToHandle;

		private bool _locked;

		private bool _isDiscrete;

		private bool _updateValueOnRelease;

		private Vector2 _localClickPos;

		private float _valueFloat;

		private Widget _handle;
	}
}
