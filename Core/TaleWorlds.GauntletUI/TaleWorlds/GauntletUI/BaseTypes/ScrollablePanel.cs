using System;
using System.Collections.Generic;
using TaleWorlds.GauntletUI.GamepadNavigation;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.GauntletUI.BaseTypes
{
	// Token: 0x02000063 RID: 99
	public class ScrollablePanel : Widget
	{
		// Token: 0x1400000F RID: 15
		// (add) Token: 0x0600063C RID: 1596 RVA: 0x0001BED4 File Offset: 0x0001A0D4
		// (remove) Token: 0x0600063D RID: 1597 RVA: 0x0001BF0C File Offset: 0x0001A10C
		public event Action<float> OnScroll;

		// Token: 0x170001C8 RID: 456
		// (get) Token: 0x0600063E RID: 1598 RVA: 0x0001BF41 File Offset: 0x0001A141
		// (set) Token: 0x0600063F RID: 1599 RVA: 0x0001BF49 File Offset: 0x0001A149
		public Widget ClipRect { get; set; }

		// Token: 0x170001C9 RID: 457
		// (get) Token: 0x06000640 RID: 1600 RVA: 0x0001BF52 File Offset: 0x0001A152
		// (set) Token: 0x06000641 RID: 1601 RVA: 0x0001BF5A File Offset: 0x0001A15A
		public Widget InnerPanel
		{
			get
			{
				return this._innerPanel;
			}
			set
			{
				if (value != this._innerPanel)
				{
					this._innerPanel = value;
					this.OnInnerPanelValueChanged();
				}
			}
		}

		// Token: 0x170001CA RID: 458
		// (get) Token: 0x06000642 RID: 1602 RVA: 0x0001BF72 File Offset: 0x0001A172
		public ScrollbarWidget ActiveScrollbar
		{
			get
			{
				return this.VerticalScrollbar ?? this.HorizontalScrollbar;
			}
		}

		// Token: 0x170001CB RID: 459
		// (get) Token: 0x06000643 RID: 1603 RVA: 0x0001BF84 File Offset: 0x0001A184
		// (set) Token: 0x06000644 RID: 1604 RVA: 0x0001BF8C File Offset: 0x0001A18C
		public bool UpdateScrollbarVisibility { get; set; } = true;

		// Token: 0x170001CC RID: 460
		// (get) Token: 0x06000645 RID: 1605 RVA: 0x0001BF95 File Offset: 0x0001A195
		// (set) Token: 0x06000646 RID: 1606 RVA: 0x0001BF9D File Offset: 0x0001A19D
		public Widget FixedHeader { get; set; }

		// Token: 0x170001CD RID: 461
		// (get) Token: 0x06000647 RID: 1607 RVA: 0x0001BFA6 File Offset: 0x0001A1A6
		// (set) Token: 0x06000648 RID: 1608 RVA: 0x0001BFAE File Offset: 0x0001A1AE
		public Widget ScrolledHeader { get; set; }

		// Token: 0x06000649 RID: 1609 RVA: 0x0001BFB7 File Offset: 0x0001A1B7
		public ScrollablePanel(UIContext context)
			: base(context)
		{
			this._verticalScrollbarInterpolationController = new ScrollablePanel.ScrollbarInterpolationController();
			this._horizontalScrollbarInterpolationController = new ScrollablePanel.ScrollbarInterpolationController();
		}

		// Token: 0x0600064A RID: 1610 RVA: 0x0001BFEF File Offset: 0x0001A1EF
		public void ResetTweenSpeed()
		{
			this._verticalScrollVelocity = 0f;
			this._horizontalScrollVelocity = 0f;
		}

		// Token: 0x0600064B RID: 1611 RVA: 0x0001C007 File Offset: 0x0001A207
		protected override bool OnPreviewMouseScroll()
		{
			return !this.OnlyAcceptScrollEventIfCanScroll || this._canScrollHorizontal || this._canScrollVertical;
		}

		// Token: 0x0600064C RID: 1612 RVA: 0x0001C021 File Offset: 0x0001A221
		protected override bool OnPreviewRightStickMovement()
		{
			return (!this.OnlyAcceptScrollEventIfCanScroll || this._canScrollHorizontal || this._canScrollVertical) && !GauntletGamepadNavigationManager.Instance.IsCursorMovingForNavigation && !GauntletGamepadNavigationManager.Instance.AnyWidgetUsingNavigation;
		}

		// Token: 0x0600064D RID: 1613 RVA: 0x0001C058 File Offset: 0x0001A258
		protected internal override void OnMouseScroll()
		{
			float num = base.EventManager.DeltaMouseScroll * 0.2f;
			if ((Input.IsKeyDown(InputKey.LeftShift) || Input.IsKeyDown(InputKey.RightShift) || this.VerticalScrollbar == null) && this.HorizontalScrollbar != null)
			{
				this._horizontalScrollVelocity += num;
			}
			else if (this.VerticalScrollbar != null)
			{
				this._verticalScrollVelocity += num;
			}
			this.StopAllInterpolations();
			Action<float> onScroll = this.OnScroll;
			if (onScroll == null)
			{
				return;
			}
			onScroll(num);
		}

		// Token: 0x0600064E RID: 1614 RVA: 0x0001C0D8 File Offset: 0x0001A2D8
		protected internal override void OnRightStickMovement()
		{
			float num = -base.EventManager.RightStickHorizontalScrollAmount * 0.2f;
			float num2 = base.EventManager.RightStickVerticalScrollAmount * 0.2f;
			this._horizontalScrollVelocity += num;
			this._verticalScrollVelocity += num2;
			this.StopAllInterpolations();
			Action<float> onScroll = this.OnScroll;
			if (onScroll == null)
			{
				return;
			}
			onScroll(Mathf.Max(num, num2));
		}

		// Token: 0x0600064F RID: 1615 RVA: 0x0001C143 File Offset: 0x0001A343
		private void StopAllInterpolations()
		{
			this._verticalScrollbarInterpolationController.StopInterpolation();
			this._horizontalScrollbarInterpolationController.StopInterpolation();
		}

		// Token: 0x06000650 RID: 1616 RVA: 0x0001C15B File Offset: 0x0001A35B
		private void OnInnerPanelChildAddedEventFire(Widget widget, string eventName, object[] eventArgs)
		{
			if ((eventName == "ItemAdd" || eventName == "AfterItemRemove") && eventArgs.Length != 0 && eventArgs[0] is ScrollablePanelFixedHeaderWidget)
			{
				this.RefreshFixedHeaders();
				this.StopAllInterpolations();
			}
		}

		// Token: 0x06000651 RID: 1617 RVA: 0x0001C191 File Offset: 0x0001A391
		private void OnInnerPanelValueChanged()
		{
			if (this.InnerPanel != null)
			{
				this.InnerPanel.EventFire += this.OnInnerPanelChildAddedEventFire;
				this.RefreshFixedHeaders();
				this.StopAllInterpolations();
			}
		}

		// Token: 0x06000652 RID: 1618 RVA: 0x0001C1BE File Offset: 0x0001A3BE
		private void OnFixedHeaderPropertyChangedEventFire(Widget widget, string eventName, object[] eventArgs)
		{
			if (eventName == "FixedHeaderPropertyChanged")
			{
				this.RefreshFixedHeaders();
				this.StopAllInterpolations();
			}
		}

		// Token: 0x06000653 RID: 1619 RVA: 0x0001C1DC File Offset: 0x0001A3DC
		private void RefreshFixedHeaders()
		{
			foreach (ScrollablePanelFixedHeaderWidget scrollablePanelFixedHeaderWidget in this._fixedHeaders)
			{
				scrollablePanelFixedHeaderWidget.EventFire -= this.OnFixedHeaderPropertyChangedEventFire;
			}
			this._fixedHeaders.Clear();
			float num = 0f;
			for (int i = 0; i < this.InnerPanel.ChildCount; i++)
			{
				ScrollablePanelFixedHeaderWidget scrollablePanelFixedHeaderWidget2;
				if ((scrollablePanelFixedHeaderWidget2 = this.InnerPanel.GetChild(i) as ScrollablePanelFixedHeaderWidget) != null && scrollablePanelFixedHeaderWidget2.IsRelevant)
				{
					num += scrollablePanelFixedHeaderWidget2.AdditionalTopOffset;
					scrollablePanelFixedHeaderWidget2.TopOffset = num;
					num += scrollablePanelFixedHeaderWidget2.SuggestedHeight;
					this._fixedHeaders.Add(scrollablePanelFixedHeaderWidget2);
					scrollablePanelFixedHeaderWidget2.EventFire += this.OnFixedHeaderPropertyChangedEventFire;
				}
			}
			float num2 = 0f;
			for (int j = this._fixedHeaders.Count - 1; j >= 0; j--)
			{
				num2 += this._fixedHeaders[j].AdditionalBottomOffset;
				this._fixedHeaders[j].BottomOffset = num2;
				num2 += this._fixedHeaders[j].SuggestedHeight;
			}
		}

		// Token: 0x06000654 RID: 1620 RVA: 0x0001C31C File Offset: 0x0001A51C
		private void AdjustVerticalScrollBar()
		{
			if (this.VerticalScrollbar != null)
			{
				if (this.InnerPanel.VerticalAlignment == VerticalAlignment.Bottom)
				{
					this.VerticalScrollbar.ValueFloat = this.VerticalScrollbar.MaxValue - this.InnerPanel.ScaledPositionOffset.Y;
					return;
				}
				this.VerticalScrollbar.ValueFloat = -this.InnerPanel.ScaledPositionOffset.Y;
			}
		}

		// Token: 0x06000655 RID: 1621 RVA: 0x0001C383 File Offset: 0x0001A583
		private void AdjustHorizontalScrollBar()
		{
			if (this.HorizontalScrollbar != null)
			{
				this.HorizontalScrollbar.ValueFloat = -this.InnerPanel.ScaledPositionOffset.X;
			}
		}

		// Token: 0x06000656 RID: 1622 RVA: 0x0001C3A9 File Offset: 0x0001A5A9
		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			this.UpdateScrollInterpolation(dt);
			this.UpdateScrollablePanel(dt);
		}

		// Token: 0x06000657 RID: 1623 RVA: 0x0001C3C0 File Offset: 0x0001A5C0
		protected void SetActiveCursor(UIContext.MouseCursors cursor)
		{
			base.Context.ActiveCursorOfContext = cursor;
		}

		// Token: 0x06000658 RID: 1624 RVA: 0x0001C3CE File Offset: 0x0001A5CE
		private void UpdateScrollInterpolation(float dt)
		{
			this._verticalScrollbarInterpolationController.Tick(dt);
			this._horizontalScrollbarInterpolationController.Tick(dt);
		}

		// Token: 0x06000659 RID: 1625 RVA: 0x0001C3E8 File Offset: 0x0001A5E8
		private void UpdateScrollablePanel(float dt)
		{
			if (this.InnerPanel != null && this.ClipRect != null)
			{
				this._canScrollHorizontal = false;
				this._canScrollVertical = false;
				if (this.HorizontalScrollbar != null)
				{
					bool flag = base.IsVisible;
					bool flag2 = base.IsVisible;
					float num = this.InnerPanel.ScaledPositionXOffset - this.InnerPanel.Left;
					float valueFloat = this.HorizontalScrollbar.ValueFloat;
					this.InnerPanel.ScaledPositionXOffset = -valueFloat;
					this._scrollOffset = this.InnerPanel.ScaledPositionOffset.X;
					this.HorizontalScrollbar.ReverseDirection = false;
					this.HorizontalScrollbar.MinValue = 0f;
					if (this.FixedHeader != null && this.ScrolledHeader != null)
					{
						if (this.FixedHeader.GlobalPosition.Y > this.ScrolledHeader.GlobalPosition.Y)
						{
							this.FixedHeader.IsVisible = true;
						}
						else
						{
							this.FixedHeader.IsVisible = false;
						}
					}
					if (this.InnerPanel.Size.X > this.ClipRect.Size.X)
					{
						this._canScrollHorizontal = true;
						this.HorizontalScrollbar.MaxValue = MathF.Max(1f, this.InnerPanel.Size.X - this.ClipRect.Size.X);
						if (this.AutoAdjustScrollbarHandleSize && this.HorizontalScrollbar.Handle != null)
						{
							this.HorizontalScrollbar.Handle.ScaledSuggestedWidth = this.HorizontalScrollbar.Size.X * (this.ClipRect.Size.X / this.InnerPanel.Size.X);
						}
						this._scrollOffset += this._horizontalScrollVelocity;
						this._horizontalScrollVelocity *= MathF.Max(0f, 1f - 8f * dt);
						this.InnerPanel.ScaledPositionXOffset = this._scrollOffset;
						this.AdjustHorizontalScrollBar();
						if (this.InnerPanel.HorizontalAlignment == HorizontalAlignment.Center)
						{
							this.InnerPanel.ScaledPositionXOffset += num;
						}
					}
					else
					{
						this.HorizontalScrollbar.Handle.ScaledSuggestedWidth = this.HorizontalScrollbar.Size.X;
						this.InnerPanel.ScaledPositionXOffset = 0f;
						this.HorizontalScrollbar.ValueFloat = 0f;
						this._horizontalScrollVelocity = 0f;
						this._scrollOffset = 0f;
						if (this.AutoHideScrollBars)
						{
							flag = false;
						}
						if (this.AutoHideScrollBarHandle)
						{
							flag2 = false;
						}
					}
					if (this.UpdateScrollbarVisibility)
					{
						this.HorizontalScrollbar.IsVisible = flag;
						this.HorizontalScrollbar.Handle.IsVisible = flag2 && flag;
					}
				}
				if (this.VerticalScrollbar != null)
				{
					float valueFloat2 = this.VerticalScrollbar.ValueFloat;
					bool flag3 = base.IsVisible;
					bool flag4 = base.IsVisible;
					this.InnerPanel.ScaledPositionYOffset = -valueFloat2;
					this._scrollOffset = this.InnerPanel.ScaledPositionOffset.Y;
					this.VerticalScrollbar.ReverseDirection = false;
					this.VerticalScrollbar.MinValue = 0f;
					if (this.FixedHeader != null && this.ScrolledHeader != null)
					{
						if (this.FixedHeader.GlobalPosition.Y >= this.ScrolledHeader.GlobalPosition.Y)
						{
							this.FixedHeader.IsVisible = true;
						}
						else
						{
							this.FixedHeader.IsVisible = false;
						}
					}
					if (this.InnerPanel.Size.Y > this.ClipRect.Size.Y)
					{
						this._canScrollVertical = true;
						this.VerticalScrollbar.MaxValue = MathF.Max(1f, this.InnerPanel.Size.Y - this.ClipRect.Size.Y);
						if (this.InnerPanel.VerticalAlignment == VerticalAlignment.Bottom)
						{
							this._scrollOffset = this.VerticalScrollbar.MaxValue - valueFloat2;
						}
						if (this.AutoAdjustScrollbarHandleSize && this.VerticalScrollbar.Handle != null)
						{
							this.VerticalScrollbar.Handle.ScaledSuggestedHeight = this.VerticalScrollbar.Size.Y * (this.ClipRect.Size.Y / this.InnerPanel.Size.Y);
						}
						this._scrollOffset += this._verticalScrollVelocity;
						this._verticalScrollVelocity *= MathF.Max(0f, 1f - 8f * dt);
						this.InnerPanel.ScaledPositionYOffset = this._scrollOffset;
						this.AdjustVerticalScrollBar();
					}
					else
					{
						if (this.AutoAdjustScrollbarHandleSize && this.VerticalScrollbar.Handle != null)
						{
							this.VerticalScrollbar.Handle.ScaledSuggestedHeight = this.VerticalScrollbar.Size.Y;
						}
						this.InnerPanel.ScaledPositionYOffset = 0f;
						this.VerticalScrollbar.ValueFloat = 0f;
						this._verticalScrollVelocity = 0f;
						this._scrollOffset = 0f;
						if (this.AutoHideScrollBars)
						{
							flag3 = false;
						}
						if (this.AutoHideScrollBarHandle)
						{
							flag4 = false;
						}
					}
					foreach (ScrollablePanelFixedHeaderWidget scrollablePanelFixedHeaderWidget in this._fixedHeaders)
					{
						if (scrollablePanelFixedHeaderWidget != null && scrollablePanelFixedHeaderWidget.FixedHeader != null)
						{
							scrollablePanelFixedHeaderWidget.FixedHeader.ScaledPositionYOffset = MathF.Clamp(scrollablePanelFixedHeaderWidget.LocalPosition.Y + this._scrollOffset, scrollablePanelFixedHeaderWidget.TopOffset * base._scaleToUse, base.MeasuredSize.Y - scrollablePanelFixedHeaderWidget.BottomOffset * base._scaleToUse);
						}
					}
					if (this.UpdateScrollbarVisibility)
					{
						this.VerticalScrollbar.IsVisible = flag3;
						this.VerticalScrollbar.Handle.IsVisible = flag4 && flag3;
					}
				}
			}
		}

		// Token: 0x0600065A RID: 1626 RVA: 0x0001C9C4 File Offset: 0x0001ABC4
		protected float GetScrollYValueForWidget(Widget widget, float widgetTargetYValue, float offset)
		{
			float num = MBMath.ClampFloat(widgetTargetYValue, 0f, 1f);
			float num2 = Mathf.Lerp(widget.GlobalPosition.Y + offset, widget.GlobalPosition.Y - this.ClipRect.Size.Y + widget.Size.Y + offset, num);
			float num3 = this.InverseLerp(this.InnerPanel.GlobalPosition.Y, this.InnerPanel.GlobalPosition.Y + this.InnerPanel.Size.Y - this.ClipRect.Size.Y, num2);
			num3 = MathF.Clamp(num3, 0f, 1f);
			return MathF.Lerp(this.VerticalScrollbar.MinValue, this.VerticalScrollbar.MaxValue, num3, 1E-05f);
		}

		// Token: 0x0600065B RID: 1627 RVA: 0x0001CA9C File Offset: 0x0001AC9C
		protected float GetScrollXValueForWidget(Widget widget, float widgetTargetXValue, float offset)
		{
			float num = MBMath.ClampFloat(widgetTargetXValue, 0f, 1f);
			float num2 = Mathf.Lerp(widget.GlobalPosition.X + offset, widget.GlobalPosition.X - this.ClipRect.Size.X + widget.Size.X + offset, num);
			float num3 = this.InverseLerp(this.InnerPanel.GlobalPosition.X, this.InnerPanel.GlobalPosition.X + this.InnerPanel.Size.X - this.ClipRect.Size.X, num2);
			num3 = MathF.Clamp(num3, 0f, 1f);
			return MathF.Lerp(this.HorizontalScrollbar.MinValue, this.HorizontalScrollbar.MaxValue, num3, 1E-05f);
		}

		// Token: 0x0600065C RID: 1628 RVA: 0x0001CB74 File Offset: 0x0001AD74
		private float InverseLerp(float fromValue, float toValue, float value)
		{
			if (fromValue == toValue)
			{
				return 0f;
			}
			return (value - fromValue) / (toValue - fromValue);
		}

		// Token: 0x0600065D RID: 1629 RVA: 0x0001CB88 File Offset: 0x0001AD88
		public void ScrollToChild(Widget targetWidget, float horizontalTargetValue = -1f, float verticalTargetValue = -1f, int horizontalOffsetInPixels = 0, int verticalOffsetInPixels = 0, float verticalInterpolationTime = 0f, float horizontalInterpolationTime = 0f)
		{
			if (this.ClipRect != null && this.InnerPanel != null && base.CheckIsMyChildRecursive(targetWidget))
			{
				if (this.VerticalScrollbar != null)
				{
					bool flag = targetWidget.GlobalPosition.Y - (float)verticalOffsetInPixels < this.ClipRect.GlobalPosition.Y;
					bool flag2 = targetWidget.GlobalPosition.Y + targetWidget.Size.Y + (float)verticalOffsetInPixels > this.ClipRect.GlobalPosition.Y + this.ClipRect.Size.Y;
					if (flag || flag2)
					{
						if (verticalTargetValue == -1f)
						{
							verticalTargetValue = (flag ? 0f : 1f);
						}
						float scrollYValueForWidget = this.GetScrollYValueForWidget(targetWidget, verticalTargetValue, (float)(flag ? (-(float)verticalOffsetInPixels) : verticalOffsetInPixels));
						if (verticalInterpolationTime <= 1E-45f)
						{
							this.VerticalScrollbar.ValueFloat = scrollYValueForWidget;
						}
						else
						{
							this._verticalScrollbarInterpolationController.StartInterpolation(scrollYValueForWidget, verticalInterpolationTime);
						}
					}
				}
				if (this.HorizontalScrollbar != null)
				{
					bool flag3 = targetWidget.GlobalPosition.X - (float)horizontalOffsetInPixels < this.ClipRect.GlobalPosition.X;
					bool flag4 = targetWidget.GlobalPosition.X + targetWidget.Size.X + (float)horizontalOffsetInPixels > this.ClipRect.GlobalPosition.X + this.ClipRect.Size.X;
					if (flag3 || flag4)
					{
						if (horizontalTargetValue == -1f)
						{
							horizontalTargetValue = (flag3 ? 0f : 1f);
						}
						float scrollXValueForWidget = this.GetScrollXValueForWidget(targetWidget, horizontalTargetValue, (float)(flag3 ? (-(float)horizontalOffsetInPixels) : horizontalOffsetInPixels));
						if (horizontalInterpolationTime <= 1E-45f)
						{
							this.HorizontalScrollbar.ValueFloat = scrollXValueForWidget;
							return;
						}
						this._horizontalScrollbarInterpolationController.StartInterpolation(scrollXValueForWidget, horizontalInterpolationTime);
					}
				}
			}
		}

		// Token: 0x0600065E RID: 1630 RVA: 0x0001CD41 File Offset: 0x0001AF41
		public void SetVerticalScrollTarget(float targetValue, float interpolationDuration)
		{
			this._verticalScrollbarInterpolationController.StartInterpolation(targetValue, interpolationDuration);
		}

		// Token: 0x0600065F RID: 1631 RVA: 0x0001CD50 File Offset: 0x0001AF50
		public void SetHorizontalScrollTarget(float targetValue, float interpolationDuration)
		{
			this._horizontalScrollbarInterpolationController.StartInterpolation(targetValue, interpolationDuration);
		}

		// Token: 0x170001CE RID: 462
		// (get) Token: 0x06000660 RID: 1632 RVA: 0x0001CD5F File Offset: 0x0001AF5F
		// (set) Token: 0x06000661 RID: 1633 RVA: 0x0001CD67 File Offset: 0x0001AF67
		[Editor(false)]
		public bool AutoHideScrollBars
		{
			get
			{
				return this._autoHideScrollBars;
			}
			set
			{
				if (this._autoHideScrollBars != value)
				{
					this._autoHideScrollBars = value;
					base.OnPropertyChanged(value, "AutoHideScrollBars");
				}
			}
		}

		// Token: 0x170001CF RID: 463
		// (get) Token: 0x06000662 RID: 1634 RVA: 0x0001CD85 File Offset: 0x0001AF85
		// (set) Token: 0x06000663 RID: 1635 RVA: 0x0001CD8D File Offset: 0x0001AF8D
		[Editor(false)]
		public bool AutoHideScrollBarHandle
		{
			get
			{
				return this._autoHideScrollBarHandle;
			}
			set
			{
				if (this._autoHideScrollBarHandle != value)
				{
					this._autoHideScrollBarHandle = value;
					base.OnPropertyChanged(value, "AutoHideScrollBarHandle");
				}
			}
		}

		// Token: 0x170001D0 RID: 464
		// (get) Token: 0x06000664 RID: 1636 RVA: 0x0001CDAB File Offset: 0x0001AFAB
		// (set) Token: 0x06000665 RID: 1637 RVA: 0x0001CDB3 File Offset: 0x0001AFB3
		[Editor(false)]
		public bool AutoAdjustScrollbarHandleSize
		{
			get
			{
				return this._autoAdjustScrollbarHandleSize;
			}
			set
			{
				if (this._autoAdjustScrollbarHandleSize != value)
				{
					this._autoAdjustScrollbarHandleSize = value;
					base.OnPropertyChanged(value, "AutoAdjustScrollbarHandleSize");
				}
			}
		}

		// Token: 0x170001D1 RID: 465
		// (get) Token: 0x06000666 RID: 1638 RVA: 0x0001CDD1 File Offset: 0x0001AFD1
		// (set) Token: 0x06000667 RID: 1639 RVA: 0x0001CDD9 File Offset: 0x0001AFD9
		[Editor(false)]
		public bool OnlyAcceptScrollEventIfCanScroll
		{
			get
			{
				return this._onlyAcceptScrollEventIfCanScroll;
			}
			set
			{
				if (this._onlyAcceptScrollEventIfCanScroll != value)
				{
					this._onlyAcceptScrollEventIfCanScroll = value;
					base.OnPropertyChanged(value, "OnlyAcceptScrollEventIfCanScroll");
				}
			}
		}

		// Token: 0x170001D2 RID: 466
		// (get) Token: 0x06000668 RID: 1640 RVA: 0x0001CDF7 File Offset: 0x0001AFF7
		// (set) Token: 0x06000669 RID: 1641 RVA: 0x0001CDFF File Offset: 0x0001AFFF
		public ScrollbarWidget HorizontalScrollbar
		{
			get
			{
				return this._horizontalScrollbar;
			}
			set
			{
				if (value != this._horizontalScrollbar)
				{
					this._horizontalScrollbar = value;
					this._horizontalScrollbarInterpolationController.SetControlledScrollbar(value);
					base.OnPropertyChanged<ScrollbarWidget>(value, "HorizontalScrollbar");
				}
			}
		}

		// Token: 0x170001D3 RID: 467
		// (get) Token: 0x0600066A RID: 1642 RVA: 0x0001CE29 File Offset: 0x0001B029
		// (set) Token: 0x0600066B RID: 1643 RVA: 0x0001CE31 File Offset: 0x0001B031
		public ScrollbarWidget VerticalScrollbar
		{
			get
			{
				return this._verticalScrollbar;
			}
			set
			{
				if (value != this._verticalScrollbar)
				{
					this._verticalScrollbar = value;
					this._verticalScrollbarInterpolationController.SetControlledScrollbar(value);
					base.OnPropertyChanged<ScrollbarWidget>(value, "VerticalScrollbar");
				}
			}
		}

		// Token: 0x040002F9 RID: 761
		private Widget _innerPanel;

		// Token: 0x040002FD RID: 765
		protected bool _canScrollHorizontal;

		// Token: 0x040002FE RID: 766
		protected bool _canScrollVertical;

		// Token: 0x040002FF RID: 767
		public int MouseScrollSpeed;

		// Token: 0x04000300 RID: 768
		public AlignmentAxis MouseScrollAxis;

		// Token: 0x04000301 RID: 769
		private float _verticalScrollVelocity;

		// Token: 0x04000302 RID: 770
		private float _horizontalScrollVelocity;

		// Token: 0x04000303 RID: 771
		private float _scrollOffset;

		// Token: 0x04000304 RID: 772
		private ScrollablePanel.ScrollbarInterpolationController _verticalScrollbarInterpolationController;

		// Token: 0x04000305 RID: 773
		private ScrollablePanel.ScrollbarInterpolationController _horizontalScrollbarInterpolationController;

		// Token: 0x04000306 RID: 774
		private List<ScrollablePanelFixedHeaderWidget> _fixedHeaders = new List<ScrollablePanelFixedHeaderWidget>();

		// Token: 0x04000307 RID: 775
		private bool _autoHideScrollBars;

		// Token: 0x04000308 RID: 776
		private bool _autoHideScrollBarHandle;

		// Token: 0x04000309 RID: 777
		private bool _autoAdjustScrollbarHandleSize = true;

		// Token: 0x0400030A RID: 778
		private bool _onlyAcceptScrollEventIfCanScroll;

		// Token: 0x0400030B RID: 779
		private ScrollbarWidget _horizontalScrollbar;

		// Token: 0x0400030C RID: 780
		private ScrollbarWidget _verticalScrollbar;

		// Token: 0x0200008F RID: 143
		private class ScrollbarInterpolationController
		{
			// Token: 0x060008BA RID: 2234 RVA: 0x00022DE0 File Offset: 0x00020FE0
			public void SetControlledScrollbar(ScrollbarWidget scrollbar)
			{
				this._scrollbar = scrollbar;
			}

			// Token: 0x060008BB RID: 2235 RVA: 0x00022DE9 File Offset: 0x00020FE9
			public void StartInterpolation(float targetValue, float duration)
			{
				this._targetValue = targetValue;
				this._duration = duration;
				this._timer = 0f;
				this._isInterpolating = true;
			}

			// Token: 0x060008BC RID: 2236 RVA: 0x00022E0B File Offset: 0x0002100B
			public void StopInterpolation()
			{
				this._isInterpolating = false;
				this._targetValue = 0f;
				this._duration = 0f;
				this._timer = 0f;
				this._isInterpolating = false;
			}

			// Token: 0x060008BD RID: 2237 RVA: 0x00022E3C File Offset: 0x0002103C
			public void Tick(float dt)
			{
				if (this._isInterpolating && this._scrollbar != null)
				{
					if (this._duration == 0f || this._timer > this._duration)
					{
						this._scrollbar.ValueFloat = this._targetValue;
						this.StopInterpolation();
						return;
					}
					float num = MathF.Clamp(this._timer / this._duration, 0f, 1f);
					this._scrollbar.ValueFloat = MathF.Lerp(this._scrollbar.ValueFloat, this._targetValue, num, 1E-05f);
					this._timer += dt;
				}
			}

			// Token: 0x0400046A RID: 1130
			private ScrollbarWidget _scrollbar;

			// Token: 0x0400046B RID: 1131
			private bool _isInterpolating;

			// Token: 0x0400046C RID: 1132
			private float _targetValue;

			// Token: 0x0400046D RID: 1133
			private float _duration;

			// Token: 0x0400046E RID: 1134
			private float _timer;
		}
	}
}
