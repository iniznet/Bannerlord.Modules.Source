using System;
using System.Collections.Generic;
using TaleWorlds.GauntletUI.GamepadNavigation;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.GauntletUI.BaseTypes
{
	public class ScrollablePanel : Widget
	{
		public event Action<float> OnScroll;

		public Widget ClipRect { get; set; }

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

		public ScrollbarWidget ActiveScrollbar
		{
			get
			{
				return this.VerticalScrollbar ?? this.HorizontalScrollbar;
			}
		}

		public bool UpdateScrollbarVisibility { get; set; } = true;

		public Widget FixedHeader { get; set; }

		public Widget ScrolledHeader { get; set; }

		public ScrollablePanel(UIContext context)
			: base(context)
		{
			this._verticalScrollbarInterpolationController = new ScrollablePanel.ScrollbarInterpolationController();
			this._horizontalScrollbarInterpolationController = new ScrollablePanel.ScrollbarInterpolationController();
		}

		public void ResetTweenSpeed()
		{
			this._verticalScrollVelocity = 0f;
			this._horizontalScrollVelocity = 0f;
		}

		protected override bool OnPreviewMouseScroll()
		{
			return !this.OnlyAcceptScrollEventIfCanScroll || this._canScrollHorizontal || this._canScrollVertical;
		}

		protected override bool OnPreviewRightStickMovement()
		{
			return (!this.OnlyAcceptScrollEventIfCanScroll || this._canScrollHorizontal || this._canScrollVertical) && !GauntletGamepadNavigationManager.Instance.IsCursorMovingForNavigation && !GauntletGamepadNavigationManager.Instance.AnyWidgetUsingNavigation;
		}

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

		private void StopAllInterpolations()
		{
			this._verticalScrollbarInterpolationController.StopInterpolation();
			this._horizontalScrollbarInterpolationController.StopInterpolation();
		}

		private void OnInnerPanelChildAddedEventFire(Widget widget, string eventName, object[] eventArgs)
		{
			if ((eventName == "ItemAdd" || eventName == "AfterItemRemove") && eventArgs.Length != 0 && eventArgs[0] is ScrollablePanelFixedHeaderWidget)
			{
				this.RefreshFixedHeaders();
				this.StopAllInterpolations();
			}
		}

		private void OnInnerPanelValueChanged()
		{
			if (this.InnerPanel != null)
			{
				this.InnerPanel.EventFire += this.OnInnerPanelChildAddedEventFire;
				this.RefreshFixedHeaders();
				this.StopAllInterpolations();
			}
		}

		private void OnFixedHeaderPropertyChangedEventFire(Widget widget, string eventName, object[] eventArgs)
		{
			if (eventName == "FixedHeaderPropertyChanged")
			{
				this.RefreshFixedHeaders();
				this.StopAllInterpolations();
			}
		}

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

		private void AdjustHorizontalScrollBar()
		{
			if (this.HorizontalScrollbar != null)
			{
				this.HorizontalScrollbar.ValueFloat = -this.InnerPanel.ScaledPositionOffset.X;
			}
		}

		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			this.UpdateScrollInterpolation(dt);
			this.UpdateScrollablePanel(dt);
		}

		protected void SetActiveCursor(UIContext.MouseCursors cursor)
		{
			base.Context.ActiveCursorOfContext = cursor;
		}

		private void UpdateScrollInterpolation(float dt)
		{
			this._verticalScrollbarInterpolationController.Tick(dt);
			this._horizontalScrollbarInterpolationController.Tick(dt);
		}

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

		protected float GetScrollYValueForWidget(Widget widget, float widgetTargetYValue, float offset)
		{
			float num = MBMath.ClampFloat(widgetTargetYValue, 0f, 1f);
			float num2 = Mathf.Lerp(widget.GlobalPosition.Y + offset, widget.GlobalPosition.Y - this.ClipRect.Size.Y + widget.Size.Y + offset, num);
			float num3 = this.InverseLerp(this.InnerPanel.GlobalPosition.Y, this.InnerPanel.GlobalPosition.Y + this.InnerPanel.Size.Y - this.ClipRect.Size.Y, num2);
			num3 = MathF.Clamp(num3, 0f, 1f);
			return MathF.Lerp(this.VerticalScrollbar.MinValue, this.VerticalScrollbar.MaxValue, num3, 1E-05f);
		}

		protected float GetScrollXValueForWidget(Widget widget, float widgetTargetXValue, float offset)
		{
			float num = MBMath.ClampFloat(widgetTargetXValue, 0f, 1f);
			float num2 = Mathf.Lerp(widget.GlobalPosition.X + offset, widget.GlobalPosition.X - this.ClipRect.Size.X + widget.Size.X + offset, num);
			float num3 = this.InverseLerp(this.InnerPanel.GlobalPosition.X, this.InnerPanel.GlobalPosition.X + this.InnerPanel.Size.X - this.ClipRect.Size.X, num2);
			num3 = MathF.Clamp(num3, 0f, 1f);
			return MathF.Lerp(this.HorizontalScrollbar.MinValue, this.HorizontalScrollbar.MaxValue, num3, 1E-05f);
		}

		private float InverseLerp(float fromValue, float toValue, float value)
		{
			if (fromValue == toValue)
			{
				return 0f;
			}
			return (value - fromValue) / (toValue - fromValue);
		}

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

		public void SetVerticalScrollTarget(float targetValue, float interpolationDuration)
		{
			this._verticalScrollbarInterpolationController.StartInterpolation(targetValue, interpolationDuration);
		}

		public void SetHorizontalScrollTarget(float targetValue, float interpolationDuration)
		{
			this._horizontalScrollbarInterpolationController.StartInterpolation(targetValue, interpolationDuration);
		}

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

		private Widget _innerPanel;

		protected bool _canScrollHorizontal;

		protected bool _canScrollVertical;

		public int MouseScrollSpeed;

		public AlignmentAxis MouseScrollAxis;

		private float _verticalScrollVelocity;

		private float _horizontalScrollVelocity;

		private float _scrollOffset;

		private ScrollablePanel.ScrollbarInterpolationController _verticalScrollbarInterpolationController;

		private ScrollablePanel.ScrollbarInterpolationController _horizontalScrollbarInterpolationController;

		private List<ScrollablePanelFixedHeaderWidget> _fixedHeaders = new List<ScrollablePanelFixedHeaderWidget>();

		private bool _autoHideScrollBars;

		private bool _autoHideScrollBarHandle;

		private bool _autoAdjustScrollbarHandleSize = true;

		private bool _onlyAcceptScrollEventIfCanScroll;

		private ScrollbarWidget _horizontalScrollbar;

		private ScrollbarWidget _verticalScrollbar;

		private class ScrollbarInterpolationController
		{
			public void SetControlledScrollbar(ScrollbarWidget scrollbar)
			{
				this._scrollbar = scrollbar;
			}

			public void StartInterpolation(float targetValue, float duration)
			{
				this._targetValue = targetValue;
				this._duration = duration;
				this._timer = 0f;
				this._isInterpolating = true;
			}

			public void StopInterpolation()
			{
				this._isInterpolating = false;
				this._targetValue = 0f;
				this._duration = 0f;
				this._timer = 0f;
				this._isInterpolating = false;
			}

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

			private ScrollbarWidget _scrollbar;

			private bool _isInterpolating;

			private float _targetValue;

			private float _duration;

			private float _timer;
		}
	}
}
