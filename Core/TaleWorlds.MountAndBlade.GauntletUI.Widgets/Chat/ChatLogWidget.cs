using System;
using System.Collections.Generic;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Chat
{
	// Token: 0x0200015B RID: 347
	public class ChatLogWidget : Widget
	{
		// Token: 0x17000648 RID: 1608
		// (get) Token: 0x060011D2 RID: 4562 RVA: 0x00031252 File Offset: 0x0002F452
		private float _resizeTransitionTime
		{
			get
			{
				return 0.14f;
			}
		}

		// Token: 0x060011D3 RID: 4563 RVA: 0x00031259 File Offset: 0x0002F459
		public ChatLogWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x060011D4 RID: 4564 RVA: 0x00031270 File Offset: 0x0002F470
		protected override void OnUpdate(float dt)
		{
			base.OnUpdate(dt);
			if (!this.IsChatDisabled && this.TextInputWidget != null && this.FullyShowChatWithTyping && this._focusOnNextUpdate)
			{
				base.EventManager.SetWidgetFocused(this.TextInputWidget, false);
				this._focusOnNextUpdate = false;
			}
			if (!this.FullyShowChat)
			{
				this.ScrollablePanel.ResetTweenSpeed();
				this.Scrollbar.ValueFloat = this.Scrollbar.MaxValue;
			}
			base.ParentWidget.DoNotPassEventsToChildren = !this.FullyShowChat;
			if (this.ResizerWidget != null && this.ResizeFrameWidget != null)
			{
				this.UpdateResize(dt);
			}
		}

		// Token: 0x060011D5 RID: 4565 RVA: 0x00031314 File Offset: 0x0002F514
		private void UpdateResize(float dt)
		{
			if (Input.IsKeyPressed(InputKey.LeftMouseButton) && base.EventManager.HoveredView == this.ResizerWidget)
			{
				this._isResizing = true;
				this._resizeStartMousePosition = Input.MousePositionPixel;
				this._resizeOriginalSize = new Vec2(this.SizeX, this.SizeY);
				this.ResizeFrameWidget.IsVisible = true;
				this.ResizeFrameWidget.WidthSizePolicy = SizePolicy.Fixed;
				this.ResizeFrameWidget.HeightSizePolicy = SizePolicy.Fixed;
				this.ResizeFrameWidget.SuggestedHeight = this.SizeY;
				this.ResizeFrameWidget.SuggestedWidth = this.SizeX;
				this._innerPanelDefaultSizePolicies = new ValueTuple<SizePolicy, SizePolicy>(this.ScrollablePanel.InnerPanel.WidthSizePolicy, this.ScrollablePanel.InnerPanel.HeightSizePolicy);
				this.ScrollablePanel.InnerPanel.WidthSizePolicy = SizePolicy.Fixed;
				this.ScrollablePanel.InnerPanel.HeightSizePolicy = SizePolicy.Fixed;
				this.ScrollablePanel.InnerPanel.SuggestedWidth = this.ScrollablePanel.InnerPanel.Size.X;
				this.ScrollablePanel.InnerPanel.SuggestedHeight = this.ScrollablePanel.InnerPanel.Size.Y;
			}
			else if (Input.IsKeyReleased(InputKey.LeftMouseButton))
			{
				if (this._isResizing)
				{
					this.ResizeFrameWidget.IsVisible = false;
					this._resizeActualPanel = true;
					this._lerpRatio = 0f;
				}
				this._isResizing = false;
			}
			if (this._isResizing)
			{
				Vec2 vec = this._resizeOriginalSize + new Vec2((Input.MousePositionPixel - this._resizeStartMousePosition).X, -(Input.MousePositionPixel - this._resizeStartMousePosition).Y);
				this.ResizeFrameWidget.SuggestedWidth = Mathf.Clamp(vec.X, base.MinWidth, base.MaxWidth);
				this.ResizeFrameWidget.SuggestedHeight = Mathf.Clamp(vec.Y, base.MinHeight, base.MaxHeight) - this.ResizeFrameWidget.MarginBottom;
				return;
			}
			if (this._resizeActualPanel)
			{
				this._lerpRatio = Mathf.Clamp(this._lerpRatio + dt / this._resizeTransitionTime, 0f, 1f);
				this.SizeX = Mathf.Lerp(this._resizeOriginalSize.x, this.ResizeFrameWidget.SuggestedWidth, this._lerpRatio);
				this.SizeY = Mathf.Lerp(this._resizeOriginalSize.y, this.ResizeFrameWidget.SuggestedHeight + this.ResizeFrameWidget.MarginBottom, this._lerpRatio);
				if (this.SizeX.ApproximatelyEqualsTo(this.ResizeFrameWidget.SuggestedWidth, 0.01f) && this.SizeY.ApproximatelyEqualsTo(this.ResizeFrameWidget.SuggestedHeight + this.ResizeFrameWidget.MarginBottom, 0.01f))
				{
					this.SizeX = this.ResizeFrameWidget.SuggestedWidth;
					this.SizeY = this.ResizeFrameWidget.SuggestedHeight + this.ResizeFrameWidget.MarginBottom;
					this.ResizeFrameWidget.WidthSizePolicy = SizePolicy.StretchToParent;
					this.ResizeFrameWidget.HeightSizePolicy = SizePolicy.StretchToParent;
					this.ScrollablePanel.InnerPanel.WidthSizePolicy = this._innerPanelDefaultSizePolicies.Item1;
					this.ScrollablePanel.InnerPanel.HeightSizePolicy = this._innerPanelDefaultSizePolicies.Item2;
					this._resizeActualPanel = false;
					base.EventFired("FinishResize", Array.Empty<object>());
					return;
				}
			}
			else if (!this._isInitialized)
			{
				this.SizeX = base.SuggestedWidth;
				this.SizeY = base.SuggestedHeight;
				this._isInitialized = true;
			}
		}

		// Token: 0x060011D6 RID: 4566 RVA: 0x000316B7 File Offset: 0x0002F8B7
		public void RegisterMultiLineElement(ChatCollapsableListPanel element)
		{
			if (!this._registeredMultilineWidgets.Contains(element))
			{
				this._registeredMultilineWidgets.Add(element);
			}
		}

		// Token: 0x060011D7 RID: 4567 RVA: 0x000316D3 File Offset: 0x0002F8D3
		public void RemoveMultiLineElement(ChatCollapsableListPanel element)
		{
			if (this._registeredMultilineWidgets.Contains(element))
			{
				this._registeredMultilineWidgets.Remove(element);
			}
		}

		// Token: 0x17000649 RID: 1609
		// (get) Token: 0x060011D8 RID: 4568 RVA: 0x000316F0 File Offset: 0x0002F8F0
		// (set) Token: 0x060011D9 RID: 4569 RVA: 0x000316F8 File Offset: 0x0002F8F8
		[DataSourceProperty]
		public bool IsChatDisabled
		{
			get
			{
				return this._isChatDisabled;
			}
			set
			{
				if (value != this._isChatDisabled)
				{
					this._isChatDisabled = value;
					base.OnPropertyChanged(value, "IsChatDisabled");
				}
			}
		}

		// Token: 0x1700064A RID: 1610
		// (get) Token: 0x060011DA RID: 4570 RVA: 0x00031716 File Offset: 0x0002F916
		// (set) Token: 0x060011DB RID: 4571 RVA: 0x0003171E File Offset: 0x0002F91E
		[DataSourceProperty]
		public bool FinishedResizing
		{
			get
			{
				return this._finishedResizing;
			}
			set
			{
				if (value != this._finishedResizing)
				{
					this._finishedResizing = value;
					base.OnPropertyChanged(value, "FinishedResizing");
				}
			}
		}

		// Token: 0x1700064B RID: 1611
		// (get) Token: 0x060011DC RID: 4572 RVA: 0x0003173C File Offset: 0x0002F93C
		// (set) Token: 0x060011DD RID: 4573 RVA: 0x00031744 File Offset: 0x0002F944
		[DataSourceProperty]
		public bool FullyShowChat
		{
			get
			{
				return this._fullyShowChat;
			}
			set
			{
				if (value != this._fullyShowChat)
				{
					this._fullyShowChat = value;
				}
			}
		}

		// Token: 0x1700064C RID: 1612
		// (get) Token: 0x060011DE RID: 4574 RVA: 0x00031756 File Offset: 0x0002F956
		// (set) Token: 0x060011DF RID: 4575 RVA: 0x00031760 File Offset: 0x0002F960
		[DataSourceProperty]
		public bool FullyShowChatWithTyping
		{
			get
			{
				return this._fullyShowChatWithTyping;
			}
			set
			{
				if (value != this._fullyShowChatWithTyping)
				{
					this._fullyShowChatWithTyping = value;
					if (!this.IsChatDisabled && this.TextInputWidget != null && this._fullyShowChatWithTyping)
					{
						this._focusOnNextUpdate = true;
					}
					base.EventManager.SetWidgetFocused(null, false);
					base.OnPropertyChanged(value, "FullyShowChatWithTyping");
				}
			}
		}

		// Token: 0x1700064D RID: 1613
		// (get) Token: 0x060011E0 RID: 4576 RVA: 0x000317B5 File Offset: 0x0002F9B5
		// (set) Token: 0x060011E1 RID: 4577 RVA: 0x000317BD File Offset: 0x0002F9BD
		[DataSourceProperty]
		public EditableTextWidget TextInputWidget
		{
			get
			{
				return this._textInputWidget;
			}
			set
			{
				if (value != this._textInputWidget)
				{
					this._textInputWidget = value;
					base.OnPropertyChanged<EditableTextWidget>(value, "TextInputWidget");
				}
			}
		}

		// Token: 0x1700064E RID: 1614
		// (get) Token: 0x060011E2 RID: 4578 RVA: 0x000317DB File Offset: 0x0002F9DB
		// (set) Token: 0x060011E3 RID: 4579 RVA: 0x000317E3 File Offset: 0x0002F9E3
		[DataSourceProperty]
		public ScrollbarWidget Scrollbar
		{
			get
			{
				return this._scrollbar;
			}
			set
			{
				if (value != this._scrollbar)
				{
					this._scrollbar = value;
					base.OnPropertyChanged<ScrollbarWidget>(value, "Scrollbar");
				}
			}
		}

		// Token: 0x1700064F RID: 1615
		// (get) Token: 0x060011E4 RID: 4580 RVA: 0x00031801 File Offset: 0x0002FA01
		// (set) Token: 0x060011E5 RID: 4581 RVA: 0x00031809 File Offset: 0x0002FA09
		[DataSourceProperty]
		public ScrollablePanel ScrollablePanel
		{
			get
			{
				return this._scrollablePanel;
			}
			set
			{
				if (value != this._scrollablePanel)
				{
					this._scrollablePanel = value;
					base.OnPropertyChanged<ScrollablePanel>(value, "ScrollablePanel");
				}
			}
		}

		// Token: 0x17000650 RID: 1616
		// (get) Token: 0x060011E6 RID: 4582 RVA: 0x00031827 File Offset: 0x0002FA27
		// (set) Token: 0x060011E7 RID: 4583 RVA: 0x0003182F File Offset: 0x0002FA2F
		[DataSourceProperty]
		public Widget ResizerWidget
		{
			get
			{
				return this._resizerWidget;
			}
			set
			{
				if (value != this._resizerWidget)
				{
					this._resizerWidget = value;
					base.OnPropertyChanged<Widget>(value, "ResizerWidget");
				}
			}
		}

		// Token: 0x17000651 RID: 1617
		// (get) Token: 0x060011E8 RID: 4584 RVA: 0x0003184D File Offset: 0x0002FA4D
		// (set) Token: 0x060011E9 RID: 4585 RVA: 0x00031855 File Offset: 0x0002FA55
		[DataSourceProperty]
		public Widget ResizeFrameWidget
		{
			get
			{
				return this._resizeFrameWidget;
			}
			set
			{
				if (value != this._resizeFrameWidget)
				{
					this._resizeFrameWidget = value;
					base.OnPropertyChanged<Widget>(value, "ResizeFrameWidget");
				}
			}
		}

		// Token: 0x17000652 RID: 1618
		// (get) Token: 0x060011EA RID: 4586 RVA: 0x00031873 File Offset: 0x0002FA73
		// (set) Token: 0x060011EB RID: 4587 RVA: 0x0003187B File Offset: 0x0002FA7B
		[DataSourceProperty]
		public float SizeX
		{
			get
			{
				return this._sizeX;
			}
			set
			{
				if (value != this._sizeX)
				{
					this._sizeX = value;
					base.SuggestedWidth = value;
					base.OnPropertyChanged(value, "SizeX");
				}
			}
		}

		// Token: 0x17000653 RID: 1619
		// (get) Token: 0x060011EC RID: 4588 RVA: 0x000318A0 File Offset: 0x0002FAA0
		// (set) Token: 0x060011ED RID: 4589 RVA: 0x000318A8 File Offset: 0x0002FAA8
		[DataSourceProperty]
		public float SizeY
		{
			get
			{
				return this._sizeY;
			}
			set
			{
				if (value != this._sizeY)
				{
					this._sizeY = value;
					base.SuggestedHeight = value;
					base.OnPropertyChanged(value, "SizeY");
				}
			}
		}

		// Token: 0x17000654 RID: 1620
		// (get) Token: 0x060011EE RID: 4590 RVA: 0x000318CD File Offset: 0x0002FACD
		// (set) Token: 0x060011EF RID: 4591 RVA: 0x000318D5 File Offset: 0x0002FAD5
		[DataSourceProperty]
		public ListPanel MessageHistoryList
		{
			get
			{
				return this._messageHistoryList;
			}
			set
			{
				if (value != this._messageHistoryList)
				{
					this._messageHistoryList = value;
					base.OnPropertyChanged<ListPanel>(value, "MessageHistoryList");
				}
			}
		}

		// Token: 0x17000655 RID: 1621
		// (get) Token: 0x060011F0 RID: 4592 RVA: 0x000318F3 File Offset: 0x0002FAF3
		// (set) Token: 0x060011F1 RID: 4593 RVA: 0x000318FB File Offset: 0x0002FAFB
		[DataSourceProperty]
		public bool IsMPChatLog
		{
			get
			{
				return this._isMPChatLog;
			}
			set
			{
				if (value != this._isMPChatLog)
				{
					this._isMPChatLog = value;
					base.OnPropertyChanged(value, "IsMPChatLog");
				}
			}
		}

		// Token: 0x04000822 RID: 2082
		private List<ChatCollapsableListPanel> _registeredMultilineWidgets = new List<ChatCollapsableListPanel>();

		// Token: 0x04000823 RID: 2083
		private bool _isInitialized;

		// Token: 0x04000824 RID: 2084
		private float _lerpRatio;

		// Token: 0x04000825 RID: 2085
		private bool _isResizing;

		// Token: 0x04000826 RID: 2086
		private bool _resizeActualPanel;

		// Token: 0x04000827 RID: 2087
		private Vec2 _resizeStartMousePosition;

		// Token: 0x04000828 RID: 2088
		private Vec2 _resizeOriginalSize;

		// Token: 0x04000829 RID: 2089
		private ValueTuple<SizePolicy, SizePolicy> _innerPanelDefaultSizePolicies;

		// Token: 0x0400082A RID: 2090
		private bool _focusOnNextUpdate;

		// Token: 0x0400082B RID: 2091
		private bool _isChatDisabled;

		// Token: 0x0400082C RID: 2092
		private bool _isMPChatLog;

		// Token: 0x0400082D RID: 2093
		private bool _finishedResizing;

		// Token: 0x0400082E RID: 2094
		private bool _fullyShowChat;

		// Token: 0x0400082F RID: 2095
		private bool _fullyShowChatWithTyping;

		// Token: 0x04000830 RID: 2096
		private EditableTextWidget _textInputWidget;

		// Token: 0x04000831 RID: 2097
		private ScrollbarWidget _scrollbar;

		// Token: 0x04000832 RID: 2098
		private ScrollablePanel _scrollablePanel;

		// Token: 0x04000833 RID: 2099
		private Widget _resizerWidget;

		// Token: 0x04000834 RID: 2100
		private Widget _resizeFrameWidget;

		// Token: 0x04000835 RID: 2101
		private float _sizeX;

		// Token: 0x04000836 RID: 2102
		private float _sizeY;

		// Token: 0x04000837 RID: 2103
		private ListPanel _messageHistoryList;
	}
}
