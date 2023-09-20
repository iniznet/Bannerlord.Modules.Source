using System;
using System.Collections.Generic;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Chat
{
	public class ChatLogWidget : Widget
	{
		private float _resizeTransitionTime
		{
			get
			{
				return 0.14f;
			}
		}

		public ChatLogWidget(UIContext context)
			: base(context)
		{
		}

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

		public void RegisterMultiLineElement(ChatCollapsableListPanel element)
		{
			if (!this._registeredMultilineWidgets.Contains(element))
			{
				this._registeredMultilineWidgets.Add(element);
			}
		}

		public void RemoveMultiLineElement(ChatCollapsableListPanel element)
		{
			if (this._registeredMultilineWidgets.Contains(element))
			{
				this._registeredMultilineWidgets.Remove(element);
			}
		}

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

		private List<ChatCollapsableListPanel> _registeredMultilineWidgets = new List<ChatCollapsableListPanel>();

		private bool _isInitialized;

		private float _lerpRatio;

		private bool _isResizing;

		private bool _resizeActualPanel;

		private Vec2 _resizeStartMousePosition;

		private Vec2 _resizeOriginalSize;

		private ValueTuple<SizePolicy, SizePolicy> _innerPanelDefaultSizePolicies;

		private bool _focusOnNextUpdate;

		private bool _isChatDisabled;

		private bool _isMPChatLog;

		private bool _finishedResizing;

		private bool _fullyShowChat;

		private bool _fullyShowChatWithTyping;

		private EditableTextWidget _textInputWidget;

		private ScrollbarWidget _scrollbar;

		private ScrollablePanel _scrollablePanel;

		private Widget _resizerWidget;

		private Widget _resizeFrameWidget;

		private float _sizeX;

		private float _sizeY;

		private ListPanel _messageHistoryList;
	}
}
