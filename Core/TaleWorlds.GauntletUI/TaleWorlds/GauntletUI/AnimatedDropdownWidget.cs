using System;
using System.Numerics;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.GauntletUI.GamepadNavigation;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;

namespace TaleWorlds.GauntletUI
{
	public class AnimatedDropdownWidget : Widget
	{
		public AnimatedDropdownWidget(UIContext context)
			: base(context)
		{
			this._clickHandler = new Action<Widget>(this.OnButtonClick);
			this._listSelectionHandler = new Action<Widget>(this.OnSelectionChanged);
			this._listItemRemovedHandler = new Action<Widget, Widget>(this.OnListChanged);
			this._listItemAddedHandler = new Action<Widget, Widget>(this.OnListChanged);
			base.UsedNavigationMovements = GamepadNavigationTypes.Horizontal;
		}

		[Editor(false)]
		public RichTextWidget TextWidget { get; set; }

		public ScrollbarWidget ScrollbarWidget { get; set; }

		protected override void OnUpdate(float dt)
		{
			base.OnUpdate(dt);
			if (!this._initialized)
			{
				this.DropdownClipWidget.ParentWidget = this.FindRelativeRoot(this);
				this._initialized = true;
			}
			if (this._buttonClicked)
			{
				this._buttonClicked = false;
			}
			else if (!this.IsLatestUpOrDown(this._button, false) && !this.IsLatestUpOrDown(this.ScrollbarWidget, true) && this._isOpen && this.DropdownClipWidget.IsVisible)
			{
				this.ClosePanel();
			}
			this.RefreshSelectedItem();
		}

		private bool IsLatestUpOrDown(Widget widget, bool includeChildren)
		{
			if (widget == null)
			{
				return false;
			}
			if (includeChildren)
			{
				return widget.CheckIsMyChildRecursive(base.EventManager.LatestMouseUpWidget) || widget.CheckIsMyChildRecursive(base.EventManager.LatestMouseDownWidget);
			}
			return widget == base.EventManager.LatestMouseUpWidget || widget == base.EventManager.LatestMouseDownWidget;
		}

		protected override void OnDisconnectedFromRoot()
		{
			base.OnDisconnectedFromRoot();
			this.ClosePanelInOneFrame();
		}

		private Widget FindRelativeRoot(Widget widget)
		{
			if (widget.ParentWidget == base.EventManager.Root)
			{
				return widget;
			}
			return this.FindRelativeRoot(widget.ParentWidget);
		}

		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (this._previousOpenState && this._isOpen && Vector2.Distance(this.DropdownClipWidget.GlobalPosition, this._dropdownOpenPosition) > 5f)
			{
				this.ClosePanelInOneFrame();
			}
			this.UpdateListPanelPosition(dt);
			if (this._isOpen && base.EventManager.IsControllerActive && (Input.IsKeyPressed(InputKey.ControllerLBumper) || Input.IsKeyPressed(InputKey.ControllerLTrigger) || Input.IsKeyPressed(InputKey.ControllerRBumper) || Input.IsKeyPressed(InputKey.ControllerRTrigger)))
			{
				this.ClosePanelInOneFrame();
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
					this.RefreshSelectedItem();
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
					this.RefreshSelectedItem();
					this._changedByControllerNavigation = true;
				}
				base.IsUsingNavigation = true;
			}
			else
			{
				this._changedByControllerNavigation = false;
				base.IsUsingNavigation = false;
			}
			if (!this._previousOpenState && this._isOpen)
			{
				this._dropdownOpenPosition = this.Button.GlobalPosition + new Vector2((this.Button.Size.X - this.DropdownClipWidget.Size.X) / 2f, this.Button.Size.Y);
			}
			this._previousOpenState = this._isOpen;
		}

		private void UpdateListPanelPosition(float dt)
		{
			this.DropdownClipWidget.HorizontalAlignment = HorizontalAlignment.Left;
			this.DropdownClipWidget.VerticalAlignment = VerticalAlignment.Top;
			Vector2 vector = Vector2.One;
			float num;
			if (this._isOpen)
			{
				Widget child = this.DropdownContainerWidget.GetChild(0);
				num = child.Size.Y + child.MarginBottom * base._scaleToUse;
			}
			else
			{
				num = 0f;
			}
			vector = this.Button.GlobalPosition + new Vector2((this.Button.Size.X - this.DropdownClipWidget.Size.X) / 2f, this.Button.Size.Y) - new Vector2(base.EventManager.LeftUsableAreaStart, base.EventManager.TopUsableAreaStart);
			this.DropdownClipWidget.ScaledPositionXOffset = vector.X;
			float num2 = MathF.Clamp(dt * this._animationSpeedModifier, 0f, 1f);
			this.DropdownClipWidget.ScaledSuggestedHeight = MathF.Lerp(this.DropdownClipWidget.ScaledSuggestedHeight, num, num2, 1E-05f);
			this.DropdownClipWidget.ScaledPositionYOffset = MathF.Lerp(this.DropdownClipWidget.ScaledPositionYOffset, vector.Y, num2, 1E-05f);
			if (!this._isOpen && MathF.Abs(this.DropdownClipWidget.ScaledSuggestedHeight - num) < 0.5f)
			{
				this.DropdownClipWidget.IsVisible = false;
				return;
			}
			if (this._isOpen)
			{
				this.DropdownClipWidget.IsVisible = true;
			}
		}

		protected virtual void OpenPanel()
		{
			this._isOpen = true;
			this.DropdownClipWidget.IsVisible = true;
			this.CreateNavigationScope();
		}

		protected virtual void ClosePanel()
		{
			this._isOpen = false;
			this.ClearGamepadNavigationScopeData();
		}

		private void ClosePanelInOneFrame()
		{
			this._isOpen = false;
			this.DropdownClipWidget.IsVisible = false;
			this.DropdownClipWidget.ScaledSuggestedHeight = 0f;
			this.ClearGamepadNavigationScopeData();
		}

		private void CreateNavigationScope()
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
			ScrollablePanel parentScrollablePanelOfWidget = this.GetParentScrollablePanelOfWidget(widget);
			if (parentScrollablePanelOfWidget != null)
			{
				parentScrollablePanelOfWidget.ScrollToChild(widget, -1f, -1f, 0, 0, 0f, 0f);
			}
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

		private void ClearGamepadNavigationScopeData()
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
			if (!this._changedByControllerNavigation)
			{
				this._buttonClicked = true;
				if (this._isOpen)
				{
					this.ClosePanel();
				}
				else
				{
					this.OpenPanel();
				}
			}
			base.EventFired("OnDropdownClick", Array.Empty<object>());
		}

		public void UpdateButtonText(string text)
		{
			if (this.TextWidget == null)
			{
				return;
			}
			if (text != null)
			{
				this.TextWidget.Text = text;
				return;
			}
			this.TextWidget.Text = " ";
		}

		public void OnListChanged(Widget widget)
		{
			this.RefreshSelectedItem();
			this.DropdownContainerWidget.IsVisible = widget.ChildCount > 1;
		}

		public void OnListChanged(Widget parentWidget, Widget addedWidget)
		{
			this.RefreshSelectedItem();
			this.DropdownContainerWidget.IsVisible = parentWidget.ChildCount > 0;
		}

		public void OnSelectionChanged(Widget widget)
		{
			if (this.UpdateSelectedItem)
			{
				this.CurrentSelectedIndex = this.ListPanelValue;
				this.RefreshSelectedItem();
			}
		}

		private void RefreshSelectedItem()
		{
			if (this.UpdateSelectedItem)
			{
				this.ListPanelValue = this.CurrentSelectedIndex;
				string text = "";
				if (this.ListPanelValue >= 0 && this.ListPanel != null)
				{
					Widget child = this.ListPanel.GetChild(this.ListPanelValue);
					if (child != null)
					{
						foreach (Widget widget in child.AllChildren)
						{
							RichTextWidget richTextWidget = widget as RichTextWidget;
							if (richTextWidget != null)
							{
								text = richTextWidget.Text;
							}
						}
					}
				}
				this.UpdateButtonText(text);
				if (this.ListPanel != null)
				{
					for (int i = 0; i < this.ListPanel.ChildCount; i++)
					{
						Widget child2 = this.ListPanel.GetChild(i);
						if (this.CurrentSelectedIndex == i)
						{
							if (child2.CurrentState != "Selected")
							{
								child2.SetState("Selected");
							}
							if (child2 is ButtonWidget)
							{
								(child2 as ButtonWidget).IsSelected = this.CurrentSelectedIndex == i;
							}
						}
					}
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
				if (this._button != null)
				{
					this._button.ClickEventHandlers.Remove(this._clickHandler);
				}
				this._button = value;
				if (this._button != null)
				{
					this._button.ClickEventHandlers.Add(this._clickHandler);
				}
				this.RefreshSelectedItem();
			}
		}

		[Editor(false)]
		public Widget DropdownContainerWidget
		{
			get
			{
				return this._dropdownContainerWidget;
			}
			set
			{
				this._dropdownContainerWidget = value;
			}
		}

		[Editor(false)]
		public Widget DropdownClipWidget
		{
			get
			{
				return this._dropdownClipWidget;
			}
			set
			{
				this._dropdownClipWidget = value;
				this._dropdownClipWidget.HorizontalAlignment = HorizontalAlignment.Left;
				this._dropdownClipWidget.VerticalAlignment = VerticalAlignment.Top;
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
					this._listPanel.SelectEventHandlers.Add(this._listSelectionHandler);
					this._listPanel.ItemAddEventHandlers.Add(this._listItemAddedHandler);
					this._listPanel.ItemRemoveEventHandlers.Add(this._listItemRemovedHandler);
				}
				this.RefreshSelectedItem();
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
					base.OnPropertyChanged(this.CurrentSelectedIndex, "CurrentSelectedIndex");
				}
			}
		}

		[Editor(false)]
		public bool UpdateSelectedItem
		{
			get
			{
				return this._updateSelectedItem;
			}
			set
			{
				if (this._updateSelectedItem != value)
				{
					this._updateSelectedItem = value;
				}
			}
		}

		private Action<Widget> _clickHandler;

		private Action<Widget> _listSelectionHandler;

		private Action<Widget, Widget> _listItemRemovedHandler;

		private Action<Widget, Widget> _listItemAddedHandler;

		private Vector2 _dropdownOpenPosition;

		private float _animationSpeedModifier = 15f;

		private bool _initialized;

		private bool _changedByControllerNavigation;

		private GamepadNavigationScope _navigationScope;

		private GamepadNavigationForcedScopeCollection _scopeCollection;

		private bool _previousOpenState;

		private ButtonWidget _button;

		private ListPanel _listPanel;

		private int _currentSelectedIndex;

		private Widget _dropdownContainerWidget;

		private Widget _dropdownClipWidget;

		private bool _isOpen;

		private bool _buttonClicked;

		private bool _updateSelectedItem = true;
	}
}
