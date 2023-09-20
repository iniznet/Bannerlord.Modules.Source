using System;
using System.Numerics;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.GauntletUI.BaseTypes
{
	public class ScrollbarWidget : ImageWidget
	{
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
		public AlignmentAxis AlignmentAxis { get; set; }

		[Editor(false)]
		public bool ReverseDirection { get; set; }

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
					if (this.MinValue <= this.MaxValue)
					{
						if (this._valueFloat < this.MinValue)
						{
							this._valueFloat = this.MinValue;
						}
						if (this._valueFloat > this.MaxValue)
						{
							this._valueFloat = this.MaxValue;
						}
						if (this.IsDiscrete)
						{
							this._valueFloat = (float)MathF.Round(value);
						}
						else
						{
							this._valueFloat = value;
						}
						this.UpdateHandleByValue();
						if (MathF.Abs(this._valueFloat - valueFloat) > 1E-05f)
						{
							base.OnPropertyChanged(this._valueFloat, "ValueFloat");
							base.OnPropertyChanged(this.ValueInt, "ValueInt");
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
			}
		}

		[Editor(false)]
		public float MinValue { get; set; }

		[Editor(false)]
		public float MaxValue { get; set; }

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
				}
			}
		}

		[Editor(false)]
		public Widget ScrollbarArea { get; set; }

		public ScrollbarWidget(UIContext context)
			: base(context)
		{
			this.ScrollbarArea = this;
			this._firstFrame = true;
		}

		protected override void OnLateUpdate(float dt)
		{
			base.OnUpdate(dt);
			if (this.Handle.IsPressed)
			{
				if (!this._handleClicked)
				{
					this._handleClicked = true;
					this._localClickPos = base.EventManager.MousePosition - this.Handle.GlobalPosition;
				}
				this.HandleMouseMove();
			}
			else
			{
				this._handleClicked = false;
			}
			this.UpdateScrollBar();
			this.UpdateHandleLength();
			this._firstFrame = false;
		}

		protected internal override void OnMousePressed()
		{
			if (this.Handle != null)
			{
				base.IsPressed = true;
				Vector2 mousePosition = base.EventManager.MousePosition;
				this._localClickPos = mousePosition - this.Handle.GlobalPosition;
				if (this._localClickPos.X < -5f)
				{
					this._localClickPos.X = -5f;
				}
				else if (this._localClickPos.X > this.Handle.Size.X + 5f)
				{
					this._localClickPos.X = this.Handle.Size.X + 5f;
				}
				if (this._localClickPos.Y < -5f)
				{
					this._localClickPos.Y = -5f;
				}
				else if (this._localClickPos.Y > this.Handle.Size.Y + 5f)
				{
					this._localClickPos.Y = this.Handle.Size.Y + 5f;
				}
				this.HandleMouseMove();
			}
		}

		protected internal override void OnMouseReleased()
		{
			if (this.Handle != null)
			{
				base.IsPressed = false;
			}
		}

		public void SetValueForced(float value)
		{
			if (value > this.MaxValue)
			{
				this.MaxValue = value;
			}
			else if (value < this.MinValue)
			{
				this.MinValue = value;
			}
			this.ValueFloat = value;
		}

		private void UpdateScrollBar()
		{
			if (!this._firstFrame)
			{
				this.UpdateHandleByValue();
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

		private void HandleMouseMove()
		{
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
				}
				else
				{
					if (num < num2)
					{
						num = num2;
					}
					if (num > num3)
					{
						num = num3;
					}
					float num4 = (num - num2) / (num3 - num2);
					this.ValueFloat = this.MinValue + (this.MaxValue - this.MinValue) * num4;
				}
				this.UpdateHandleByValue();
			}
		}

		private void UpdateHandleLength()
		{
			if (!this.DoNotUpdateHandleSize && this.IsDiscrete && this.Handle.WidthSizePolicy == SizePolicy.Fixed)
			{
				if (this.AlignmentAxis == AlignmentAxis.Horizontal)
				{
					this.Handle.SuggestedWidth = Mathf.Clamp(base.SuggestedWidth / (this.MaxValue + 1f), 50f, base.SuggestedWidth / 2f);
					return;
				}
				if (this.AlignmentAxis == AlignmentAxis.Vertical)
				{
					this.Handle.SuggestedHeight = Mathf.Clamp(base.SuggestedHeight / (this.MaxValue + 1f), 50f, base.SuggestedHeight / 2f);
				}
			}
		}

		private void UpdateHandleByValue()
		{
			if (this._valueFloat < this.MinValue)
			{
				this.ValueFloat = this.MinValue;
			}
			if (this._valueFloat > this.MaxValue)
			{
				this.ValueFloat = this.MaxValue;
			}
			float num = 0f;
			if (MathF.Abs(this.MaxValue - this.MinValue) > 1E-45f)
			{
				num = (this._valueFloat - this.MinValue) / (this.MaxValue - this.MinValue);
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
					float num2 = this.ScrollbarArea.Size.X;
					num2 -= this.Handle.Size.X;
					this.Handle.ScaledPositionXOffset = num2 * num;
					this.Handle.ScaledPositionYOffset = 0f;
					return;
				}
				this.Handle.HorizontalAlignment = HorizontalAlignment.Center;
				this.Handle.VerticalAlignment = VerticalAlignment.Bottom;
				float num3 = this.ScrollbarArea.Size.Y;
				num3 -= this.Handle.Size.Y;
				this.Handle.ScaledPositionYOffset = -1f * num3 * (1f - num);
				this.Handle.ScaledPositionXOffset = 0f;
			}
		}

		private bool _locked;

		private bool _isDiscrete;

		private float _valueFloat;

		public float HandleRatio;

		private Widget _handle;

		private bool _firstFrame;

		private Vector2 _localClickPos;

		private bool _handleClicked;
	}
}
