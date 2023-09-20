using System;
using System.Collections.Generic;
using System.Numerics;
using TaleWorlds.GauntletUI.GamepadNavigation;
using TaleWorlds.InputSystem;

namespace TaleWorlds.GauntletUI.BaseTypes
{
	public class DropdownWidget : Widget
	{
		private Vector2 ListPanelPositionInsideUsableArea
		{
			get
			{
				return this.ListPanel.GlobalPosition - new Vector2(base.EventManager.LeftUsableAreaStart, base.EventManager.TopUsableAreaStart);
			}
		}

		[Editor(false)]
		public RichTextWidget RichTextWidget { get; set; }

		[Editor(false)]
		public bool DoNotHandleDropdownListPanel { get; set; }

		public DropdownWidget(UIContext context)
			: base(context)
		{
			this._clickHandler = new Action<Widget>(this.OnButtonClick);
			this._listSelectionHandler = new Action<Widget>(this.OnSelectionChanged);
			this._listItemRemovedHandler = new Action<Widget, Widget>(this.OnListItemRemoved);
			this._listItemAddedHandler = new Action<Widget, Widget>(this.OnListItemAdded);
			base.UsedNavigationMovements = GamepadNavigationTypes.Horizontal;
		}

		protected override void OnUpdate(float dt)
		{
			base.OnUpdate(dt);
			if (!this.DoNotHandleDropdownListPanel)
			{
				this.UpdateListPanelPosition();
			}
			if (this._buttonClicked)
			{
				if (this.ListPanel != null && !this._changedByControllerNavigation)
				{
					if (this._isOpen)
					{
						this.ClosePanel();
					}
					else
					{
						this.OpenPanel();
					}
				}
				this._buttonClicked = false;
			}
			else if (this._closeNextFrame && this._isOpen)
			{
				this.ClosePanel();
				this._closeNextFrame = false;
			}
			else if (base.EventManager.LatestMouseUpWidget != this._button && this._isOpen)
			{
				if (this.ListPanel.IsVisible)
				{
					this._closeNextFrame = true;
				}
			}
			else if (this._isOpen)
			{
				this._openFrameCounter++;
				if (this._openFrameCounter > 5)
				{
					if (Vector2.Distance(this.ListPanelPositionInsideUsableArea, this._listPanelOpenPosition) > 20f && !this.DoNotHandleDropdownListPanel)
					{
						this._closeNextFrame = true;
					}
				}
				else
				{
					this._listPanelOpenPosition = this.ListPanelPositionInsideUsableArea;
				}
			}
			this.RefreshSelectedItem();
		}

		protected override void OnConnectedToRoot()
		{
			base.OnConnectedToRoot();
			this.ScrollablePanel = this.GetParentScrollablePanelOfWidget(this);
		}

		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (!this.DoNotHandleDropdownListPanel)
			{
				this.UpdateListPanelPosition();
			}
			this.UpdateGamepadNavigationControls();
		}

		private void UpdateGamepadNavigationControls()
		{
			if (this._isOpen && base.EventManager.IsControllerActive && (Input.IsKeyPressed(InputKey.ControllerLBumper) || Input.IsKeyPressed(InputKey.ControllerLTrigger) || Input.IsKeyPressed(InputKey.ControllerRBumper) || Input.IsKeyPressed(InputKey.ControllerRTrigger)))
			{
				this.ClosePanel();
			}
			if (!this._isOpen && (base.IsPressed || this._button.IsPressed) && base.IsRecursivelyVisible() && base.EventManager.GetIsHitThisFrame())
			{
				if (Input.IsKeyReleased(InputKey.ControllerLLeft))
				{
					if (this.CurrentSelectedIndex > 0)
					{
						int num = this.CurrentSelectedIndex;
						this.CurrentSelectedIndex = num - 1;
					}
					else
					{
						this.CurrentSelectedIndex = this.ListPanel.ChildCount - 1;
					}
					this._isSelectedItemDirty = true;
					this._changedByControllerNavigation = true;
				}
				else if (Input.IsKeyReleased(InputKey.ControllerLRight))
				{
					if (this.CurrentSelectedIndex < this.ListPanel.ChildCount - 1)
					{
						int num = this.CurrentSelectedIndex;
						this.CurrentSelectedIndex = num + 1;
					}
					else
					{
						this.CurrentSelectedIndex = 0;
					}
					this._isSelectedItemDirty = true;
					this._changedByControllerNavigation = true;
				}
				base.IsUsingNavigation = true;
				return;
			}
			this._changedByControllerNavigation = false;
			base.IsUsingNavigation = false;
		}

		private void UpdateListPanelPosition()
		{
			this.ListPanel.HorizontalAlignment = HorizontalAlignment.Left;
			this.ListPanel.VerticalAlignment = VerticalAlignment.Top;
			float num = (base.Size.X - this._listPanel.Size.X) * 0.5f;
			this.ListPanel.MarginTop = (base.GlobalPosition.Y + this.Button.Size.Y - base.EventManager.TopUsableAreaStart) * base._inverseScaleToUse;
			this.ListPanel.MarginLeft = (base.GlobalPosition.X + num - base.EventManager.LeftUsableAreaStart) * base._inverseScaleToUse;
		}

		protected virtual void OpenPanel()
		{
			if (this.Button != null)
			{
				this.Button.IsSelected = true;
			}
			this.ListPanel.IsVisible = true;
			this._listPanelOpenPosition = this.ListPanelPositionInsideUsableArea;
			this._openFrameCounter = 0;
			this._isOpen = true;
			Action<DropdownWidget> onOpenStateChanged = this.OnOpenStateChanged;
			if (onOpenStateChanged != null)
			{
				onOpenStateChanged(this);
			}
			this.CreateGamepadNavigationScopeData();
		}

		protected virtual void ClosePanel()
		{
			if (this.Button != null)
			{
				this.Button.IsSelected = false;
			}
			this.ListPanel.IsVisible = false;
			this._buttonClicked = false;
			this._isOpen = false;
			Action<DropdownWidget> onOpenStateChanged = this.OnOpenStateChanged;
			if (onOpenStateChanged != null)
			{
				onOpenStateChanged(this);
			}
			this.ClearGamepadScopeData();
		}

		private void CreateGamepadNavigationScopeData()
		{
			if (this._navigationScope != null)
			{
				base.EventManager.RemoveNavigationScope(this._navigationScope);
			}
			this._scopeCollection = new GamepadNavigationForcedScopeCollection();
			this._scopeCollection.ParentWidget = base.ParentWidget ?? this;
			this._scopeCollection.CollectionOrder = 999;
			this._navigationScope = this.BuildGamepadNavigationScopeData();
			base.EventManager.AddNavigationScope(this._navigationScope, true);
			this._button.GamepadNavigationIndex = 0;
			this._navigationScope.AddWidgetAtIndex(this._button, 0);
			ButtonWidget button = this._button;
			button.OnGamepadNavigationFocusGained = (Action<Widget>)Delegate.Combine(button.OnGamepadNavigationFocusGained, new Action<Widget>(this.OnWidgetGainedNavigationFocus));
			for (int i = 0; i < this.ListPanel.Children.Count; i++)
			{
				this.ListPanel.Children[i].GamepadNavigationIndex = i + 1;
				this._navigationScope.AddWidgetAtIndex(this.ListPanel.Children[i], i + 1);
				Widget widget = this.ListPanel.Children[i];
				widget.OnGamepadNavigationFocusGained = (Action<Widget>)Delegate.Combine(widget.OnGamepadNavigationFocusGained, new Action<Widget>(this.OnWidgetGainedNavigationFocus));
			}
			base.EventManager.AddForcedScopeCollection(this._scopeCollection);
		}

		private void OnWidgetGainedNavigationFocus(Widget widget)
		{
			ScrollablePanel scrollablePanel = this.ScrollablePanel;
			if (scrollablePanel == null)
			{
				return;
			}
			scrollablePanel.ScrollToChild(widget, -1f, -1f, 0, 0, 0f, 0f);
		}

		private ScrollablePanel GetParentScrollablePanelOfWidget(Widget widget)
		{
			for (Widget widget2 = widget; widget2 != null; widget2 = widget2.ParentWidget)
			{
				ScrollablePanel scrollablePanel;
				if ((scrollablePanel = widget2 as ScrollablePanel) != null)
				{
					return scrollablePanel;
				}
			}
			return null;
		}

		private GamepadNavigationScope BuildGamepadNavigationScopeData()
		{
			return new GamepadNavigationScope
			{
				ScopeMovements = GamepadNavigationTypes.Vertical,
				DoNotAutomaticallyFindChildren = true,
				DoNotAutoNavigateAfterSort = true,
				HasCircularMovement = true,
				ParentWidget = (base.ParentWidget ?? this),
				ScopeID = "DropdownScope"
			};
		}

		private void ClearGamepadScopeData()
		{
			if (this._navigationScope != null)
			{
				base.EventManager.RemoveNavigationScope(this._navigationScope);
				for (int i = 0; i < this.ListPanel.Children.Count; i++)
				{
					this.ListPanel.Children[i].GamepadNavigationIndex = -1;
					Widget widget = this.ListPanel.Children[i];
					widget.OnGamepadNavigationFocusGained = (Action<Widget>)Delegate.Remove(widget.OnGamepadNavigationFocusGained, new Action<Widget>(this.OnWidgetGainedNavigationFocus));
				}
				this._button.GamepadNavigationIndex = -1;
				ButtonWidget button = this._button;
				button.OnGamepadNavigationFocusGained = (Action<Widget>)Delegate.Remove(button.OnGamepadNavigationFocusGained, new Action<Widget>(this.OnWidgetGainedNavigationFocus));
				this._navigationScope = null;
			}
			if (this._scopeCollection != null)
			{
				base.EventManager.RemoveForcedScopeCollection(this._scopeCollection);
			}
		}

		public void OnButtonClick(Widget widget)
		{
			this._buttonClicked = true;
			this._closeNextFrame = false;
		}

		public void UpdateButtonText(string text)
		{
			if (this.RichTextWidget != null)
			{
				this.RichTextWidget.Text = ((!string.IsNullOrEmpty(text)) ? text : " ");
			}
		}

		public void OnListItemAdded(Widget parentWidget, Widget newChild)
		{
			this._isSelectedItemDirty = true;
		}

		public void OnListItemRemoved(Widget removedItem, Widget removedChild)
		{
			this._isSelectedItemDirty = true;
		}

		public void OnSelectionChanged(Widget widget)
		{
			this.CurrentSelectedIndex = this.ListPanelValue;
			this._isSelectedItemDirty = true;
			base.OnPropertyChanged(this.CurrentSelectedIndex, "CurrentSelectedIndex");
		}

		private void RefreshSelectedItem()
		{
			if (this._isSelectedItemDirty)
			{
				this.ListPanelValue = this.CurrentSelectedIndex;
				if (this.ListPanelValue >= 0)
				{
					string text = "";
					ListPanel listPanel = this.ListPanel;
					Widget widget = ((listPanel != null) ? listPanel.GetChild(this.ListPanelValue) : null);
					if (widget != null)
					{
						using (IEnumerator<Widget> enumerator = widget.AllChildren.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								RichTextWidget richTextWidget;
								if ((richTextWidget = enumerator.Current as RichTextWidget) != null)
								{
									text = richTextWidget.Text;
								}
							}
						}
					}
					this.UpdateButtonText(text);
				}
				if (this.ListPanel != null)
				{
					for (int i = 0; i < this.ListPanel.ChildCount; i++)
					{
						ButtonWidget buttonWidget;
						if ((buttonWidget = this.ListPanel.GetChild(i) as ButtonWidget) != null)
						{
							buttonWidget.IsSelected = this.CurrentSelectedIndex == i;
						}
					}
				}
				this._isSelectedItemDirty = false;
			}
		}

		[Editor(false)]
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

		[Editor(false)]
		public ButtonWidget Button
		{
			get
			{
				return this._button;
			}
			set
			{
				ButtonWidget button = this._button;
				if (button != null)
				{
					button.ClickEventHandlers.Remove(this._clickHandler);
				}
				this._button = value;
				ButtonWidget button2 = this._button;
				if (button2 != null)
				{
					button2.ClickEventHandlers.Add(this._clickHandler);
				}
				this._isSelectedItemDirty = true;
			}
		}

		[Editor(false)]
		public ListPanel ListPanel
		{
			get
			{
				return this._listPanel;
			}
			set
			{
				if (this._listPanel != null)
				{
					this._listPanel.SelectEventHandlers.Remove(this._listSelectionHandler);
					this._listPanel.ItemAddEventHandlers.Remove(this._listItemAddedHandler);
					this._listPanel.ItemRemoveEventHandlers.Remove(this._listItemRemovedHandler);
				}
				this._listPanel = value;
				if (this._listPanel != null)
				{
					if (!this.DoNotHandleDropdownListPanel)
					{
						this._listPanel.ParentWidget = base.EventManager.Root;
						this._listPanel.HorizontalAlignment = HorizontalAlignment.Left;
						this._listPanel.VerticalAlignment = VerticalAlignment.Top;
					}
					this._listPanel.SelectEventHandlers.Add(this._listSelectionHandler);
					this._listPanel.ItemAddEventHandlers.Add(this._listItemAddedHandler);
					this._listPanel.ItemRemoveEventHandlers.Add(this._listItemRemovedHandler);
				}
				this._isSelectedItemDirty = true;
			}
		}

		public bool IsOpen
		{
			get
			{
				return this._isOpen;
			}
			set
			{
				if (value != this._isOpen && !this._buttonClicked)
				{
					if (this._isOpen)
					{
						this.ClosePanel();
						return;
					}
					this.OpenPanel();
				}
			}
		}

		[Editor(false)]
		public int ListPanelValue
		{
			get
			{
				if (this.ListPanel != null)
				{
					return this.ListPanel.IntValue;
				}
				return -1;
			}
			set
			{
				if (this.ListPanel != null && this.ListPanel.IntValue != value)
				{
					this.ListPanel.IntValue = value;
				}
			}
		}

		[Editor(false)]
		public int CurrentSelectedIndex
		{
			get
			{
				return this._currentSelectedIndex;
			}
			set
			{
				if (this._currentSelectedIndex != value)
				{
					this._currentSelectedIndex = value;
					this._isSelectedItemDirty = true;
				}
			}
		}

		public Action<DropdownWidget> OnOpenStateChanged;

		private readonly Action<Widget> _clickHandler;

		private readonly Action<Widget> _listSelectionHandler;

		private readonly Action<Widget, Widget> _listItemRemovedHandler;

		private readonly Action<Widget, Widget> _listItemAddedHandler;

		private Vector2 _listPanelOpenPosition;

		private int _openFrameCounter;

		private bool _isSelectedItemDirty = true;

		private bool _changedByControllerNavigation;

		private GamepadNavigationScope _navigationScope;

		private GamepadNavigationForcedScopeCollection _scopeCollection;

		private ScrollablePanel _scrollablePanel;

		private ButtonWidget _button;

		private ListPanel _listPanel;

		private int _currentSelectedIndex;

		private bool _closeNextFrame;

		private bool _isOpen;

		private bool _buttonClicked;
	}
}
