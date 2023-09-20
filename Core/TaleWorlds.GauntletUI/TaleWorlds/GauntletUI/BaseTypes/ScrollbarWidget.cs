using System;
using System.Numerics;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.GauntletUI.BaseTypes
{
	// Token: 0x02000065 RID: 101
	public class ScrollbarWidget : ImageWidget
	{
		// Token: 0x170001DB RID: 475
		// (get) Token: 0x0600067C RID: 1660 RVA: 0x0001CF74 File Offset: 0x0001B174
		// (set) Token: 0x0600067D RID: 1661 RVA: 0x0001CF7C File Offset: 0x0001B17C
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

		// Token: 0x170001DC RID: 476
		// (get) Token: 0x0600067E RID: 1662 RVA: 0x0001CF9F File Offset: 0x0001B19F
		// (set) Token: 0x0600067F RID: 1663 RVA: 0x0001CFA7 File Offset: 0x0001B1A7
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

		// Token: 0x170001DD RID: 477
		// (get) Token: 0x06000680 RID: 1664 RVA: 0x0001CFCA File Offset: 0x0001B1CA
		// (set) Token: 0x06000681 RID: 1665 RVA: 0x0001CFD2 File Offset: 0x0001B1D2
		[Editor(false)]
		public AlignmentAxis AlignmentAxis { get; set; }

		// Token: 0x170001DE RID: 478
		// (get) Token: 0x06000682 RID: 1666 RVA: 0x0001CFDB File Offset: 0x0001B1DB
		// (set) Token: 0x06000683 RID: 1667 RVA: 0x0001CFE3 File Offset: 0x0001B1E3
		[Editor(false)]
		public bool ReverseDirection { get; set; }

		// Token: 0x170001DF RID: 479
		// (get) Token: 0x06000684 RID: 1668 RVA: 0x0001CFEC File Offset: 0x0001B1EC
		// (set) Token: 0x06000685 RID: 1669 RVA: 0x0001CFF4 File Offset: 0x0001B1F4
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

		// Token: 0x170001E0 RID: 480
		// (get) Token: 0x06000686 RID: 1670 RVA: 0x0001D0C9 File Offset: 0x0001B2C9
		// (set) Token: 0x06000687 RID: 1671 RVA: 0x0001D0D6 File Offset: 0x0001B2D6
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

		// Token: 0x170001E1 RID: 481
		// (get) Token: 0x06000688 RID: 1672 RVA: 0x0001D0E0 File Offset: 0x0001B2E0
		// (set) Token: 0x06000689 RID: 1673 RVA: 0x0001D0E8 File Offset: 0x0001B2E8
		[Editor(false)]
		public float MinValue { get; set; }

		// Token: 0x170001E2 RID: 482
		// (get) Token: 0x0600068A RID: 1674 RVA: 0x0001D0F1 File Offset: 0x0001B2F1
		// (set) Token: 0x0600068B RID: 1675 RVA: 0x0001D0F9 File Offset: 0x0001B2F9
		[Editor(false)]
		public float MaxValue { get; set; }

		// Token: 0x170001E3 RID: 483
		// (get) Token: 0x0600068C RID: 1676 RVA: 0x0001D102 File Offset: 0x0001B302
		// (set) Token: 0x0600068D RID: 1677 RVA: 0x0001D10A File Offset: 0x0001B30A
		[Editor(false)]
		public bool DoNotUpdateHandleSize { get; set; }

		// Token: 0x170001E4 RID: 484
		// (get) Token: 0x0600068E RID: 1678 RVA: 0x0001D113 File Offset: 0x0001B313
		// (set) Token: 0x0600068F RID: 1679 RVA: 0x0001D11B File Offset: 0x0001B31B
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

		// Token: 0x170001E5 RID: 485
		// (get) Token: 0x06000690 RID: 1680 RVA: 0x0001D133 File Offset: 0x0001B333
		// (set) Token: 0x06000691 RID: 1681 RVA: 0x0001D13B File Offset: 0x0001B33B
		[Editor(false)]
		public Widget ScrollbarArea { get; set; }

		// Token: 0x06000692 RID: 1682 RVA: 0x0001D144 File Offset: 0x0001B344
		public ScrollbarWidget(UIContext context)
			: base(context)
		{
			this.ScrollbarArea = this;
			this._firstFrame = true;
		}

		// Token: 0x06000693 RID: 1683 RVA: 0x0001D15C File Offset: 0x0001B35C
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

		// Token: 0x06000694 RID: 1684 RVA: 0x0001D1D0 File Offset: 0x0001B3D0
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

		// Token: 0x06000695 RID: 1685 RVA: 0x0001D2E8 File Offset: 0x0001B4E8
		protected internal override void OnMouseReleased()
		{
			if (this.Handle != null)
			{
				base.IsPressed = false;
			}
		}

		// Token: 0x06000696 RID: 1686 RVA: 0x0001D2F9 File Offset: 0x0001B4F9
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

		// Token: 0x06000697 RID: 1687 RVA: 0x0001D324 File Offset: 0x0001B524
		private void UpdateScrollBar()
		{
			if (!this._firstFrame)
			{
				this.UpdateHandleByValue();
			}
		}

		// Token: 0x06000698 RID: 1688 RVA: 0x0001D334 File Offset: 0x0001B534
		private float GetValue(Vector2 value, AlignmentAxis alignmentAxis)
		{
			if (alignmentAxis == AlignmentAxis.Horizontal)
			{
				return value.X;
			}
			return value.Y;
		}

		// Token: 0x06000699 RID: 1689 RVA: 0x0001D348 File Offset: 0x0001B548
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

		// Token: 0x0600069A RID: 1690 RVA: 0x0001D44C File Offset: 0x0001B64C
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

		// Token: 0x0600069B RID: 1691 RVA: 0x0001D4F8 File Offset: 0x0001B6F8
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

		// Token: 0x04000315 RID: 789
		private bool _locked;

		// Token: 0x04000316 RID: 790
		private bool _isDiscrete;

		// Token: 0x04000319 RID: 793
		private float _valueFloat;

		// Token: 0x0400031D RID: 797
		public float HandleRatio;

		// Token: 0x0400031E RID: 798
		private Widget _handle;

		// Token: 0x04000320 RID: 800
		private bool _firstFrame;

		// Token: 0x04000321 RID: 801
		private Vector2 _localClickPos;

		// Token: 0x04000322 RID: 802
		private bool _handleClicked;
	}
}
