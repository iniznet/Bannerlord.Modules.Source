using System;
using System.Numerics;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.GauntletUI.GamepadNavigation;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;

namespace TaleWorlds.GauntletUI
{
	// Token: 0x02000006 RID: 6
	public class AnimatedDropdownWidget : Widget
	{
		// Token: 0x0600000B RID: 11 RVA: 0x00002150 File Offset: 0x00000350
		public AnimatedDropdownWidget(UIContext context)
			: base(context)
		{
			this._clickHandler = new Action<Widget>(this.OnButtonClick);
			this._listSelectionHandler = new Action<Widget>(this.OnSelectionChanged);
			this._listItemRemovedHandler = new Action<Widget, Widget>(this.OnListChanged);
			this._listItemAddedHandler = new Action<Widget, Widget>(this.OnListChanged);
			base.UsedNavigationMovements = GamepadNavigationTypes.Horizontal;
		}

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x0600000C RID: 12 RVA: 0x000021C6 File Offset: 0x000003C6
		// (set) Token: 0x0600000D RID: 13 RVA: 0x000021CE File Offset: 0x000003CE
		[Editor(false)]
		public RichTextWidget TextWidget { get; set; }

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x0600000E RID: 14 RVA: 0x000021D7 File Offset: 0x000003D7
		// (set) Token: 0x0600000F RID: 15 RVA: 0x000021DF File Offset: 0x000003DF
		public ScrollbarWidget ScrollbarWidget { get; set; }

		// Token: 0x06000010 RID: 16 RVA: 0x000021E8 File Offset: 0x000003E8
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

		// Token: 0x06000011 RID: 17 RVA: 0x00002270 File Offset: 0x00000470
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

		// Token: 0x06000012 RID: 18 RVA: 0x000022CA File Offset: 0x000004CA
		protected override void OnDisconnectedFromRoot()
		{
			base.OnDisconnectedFromRoot();
			this.ClosePanelInOneFrame();
		}

		// Token: 0x06000013 RID: 19 RVA: 0x000022D8 File Offset: 0x000004D8
		private Widget FindRelativeRoot(Widget widget)
		{
			if (widget.ParentWidget == base.EventManager.Root)
			{
				return widget;
			}
			return this.FindRelativeRoot(widget.ParentWidget);
		}

		// Token: 0x06000014 RID: 20 RVA: 0x000022FC File Offset: 0x000004FC
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

		// Token: 0x06000015 RID: 21 RVA: 0x000024E8 File Offset: 0x000006E8
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

		// Token: 0x06000016 RID: 22 RVA: 0x00002671 File Offset: 0x00000871
		protected virtual void OpenPanel()
		{
			this._isOpen = true;
			this.DropdownClipWidget.IsVisible = true;
			this.CreateNavigationScope();
		}

		// Token: 0x06000017 RID: 23 RVA: 0x0000268C File Offset: 0x0000088C
		protected virtual void ClosePanel()
		{
			this._isOpen = false;
			this.ClearGamepadNavigationScopeData();
		}

		// Token: 0x06000018 RID: 24 RVA: 0x0000269B File Offset: 0x0000089B
		private void ClosePanelInOneFrame()
		{
			this._isOpen = false;
			this.DropdownClipWidget.IsVisible = false;
			this.DropdownClipWidget.ScaledSuggestedHeight = 0f;
			this.ClearGamepadNavigationScopeData();
		}

		// Token: 0x06000019 RID: 25 RVA: 0x000026C8 File Offset: 0x000008C8
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

		// Token: 0x0600001A RID: 26 RVA: 0x0000281C File Offset: 0x00000A1C
		private void OnWidgetGainedNavigationFocus(Widget widget)
		{
			ScrollablePanel parentScrollablePanelOfWidget = this.GetParentScrollablePanelOfWidget(widget);
			if (parentScrollablePanelOfWidget != null)
			{
				parentScrollablePanelOfWidget.ScrollToChild(widget, -1f, -1f, 0, 0, 0f, 0f);
			}
		}

		// Token: 0x0600001B RID: 27 RVA: 0x00002854 File Offset: 0x00000A54
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

		// Token: 0x0600001C RID: 28 RVA: 0x0000287C File Offset: 0x00000A7C
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

		// Token: 0x0600001D RID: 29 RVA: 0x000028BC File Offset: 0x00000ABC
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

		// Token: 0x0600001E RID: 30 RVA: 0x0000299C File Offset: 0x00000B9C
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

		// Token: 0x0600001F RID: 31 RVA: 0x000029D3 File Offset: 0x00000BD3
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

		// Token: 0x06000020 RID: 32 RVA: 0x000029FE File Offset: 0x00000BFE
		public void OnListChanged(Widget widget)
		{
			this.RefreshSelectedItem();
			this.DropdownContainerWidget.IsVisible = widget.ChildCount > 1;
		}

		// Token: 0x06000021 RID: 33 RVA: 0x00002A1A File Offset: 0x00000C1A
		public void OnListChanged(Widget parentWidget, Widget addedWidget)
		{
			this.RefreshSelectedItem();
			this.DropdownContainerWidget.IsVisible = parentWidget.ChildCount > 0;
		}

		// Token: 0x06000022 RID: 34 RVA: 0x00002A36 File Offset: 0x00000C36
		public void OnSelectionChanged(Widget widget)
		{
			if (this.UpdateSelectedItem)
			{
				this.CurrentSelectedIndex = this.ListPanelValue;
				this.RefreshSelectedItem();
			}
		}

		// Token: 0x06000023 RID: 35 RVA: 0x00002A54 File Offset: 0x00000C54
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

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x06000024 RID: 36 RVA: 0x00002B6C File Offset: 0x00000D6C
		// (set) Token: 0x06000025 RID: 37 RVA: 0x00002B74 File Offset: 0x00000D74
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

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x06000026 RID: 38 RVA: 0x00002BCB File Offset: 0x00000DCB
		// (set) Token: 0x06000027 RID: 39 RVA: 0x00002BD3 File Offset: 0x00000DD3
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

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x06000028 RID: 40 RVA: 0x00002BDC File Offset: 0x00000DDC
		// (set) Token: 0x06000029 RID: 41 RVA: 0x00002BE4 File Offset: 0x00000DE4
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

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x0600002A RID: 42 RVA: 0x00002C05 File Offset: 0x00000E05
		// (set) Token: 0x0600002B RID: 43 RVA: 0x00002C10 File Offset: 0x00000E10
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

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x0600002C RID: 44 RVA: 0x00002CC1 File Offset: 0x00000EC1
		// (set) Token: 0x0600002D RID: 45 RVA: 0x00002CD8 File Offset: 0x00000ED8
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

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x0600002E RID: 46 RVA: 0x00002CFC File Offset: 0x00000EFC
		// (set) Token: 0x0600002F RID: 47 RVA: 0x00002D04 File Offset: 0x00000F04
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

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x06000030 RID: 48 RVA: 0x00002D27 File Offset: 0x00000F27
		// (set) Token: 0x06000031 RID: 49 RVA: 0x00002D2F File Offset: 0x00000F2F
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

		// Token: 0x04000007 RID: 7
		private Action<Widget> _clickHandler;

		// Token: 0x04000008 RID: 8
		private Action<Widget> _listSelectionHandler;

		// Token: 0x04000009 RID: 9
		private Action<Widget, Widget> _listItemRemovedHandler;

		// Token: 0x0400000A RID: 10
		private Action<Widget, Widget> _listItemAddedHandler;

		// Token: 0x0400000B RID: 11
		private Vector2 _dropdownOpenPosition;

		// Token: 0x0400000C RID: 12
		private float _animationSpeedModifier = 15f;

		// Token: 0x0400000D RID: 13
		private bool _initialized;

		// Token: 0x0400000E RID: 14
		private bool _changedByControllerNavigation;

		// Token: 0x0400000F RID: 15
		private GamepadNavigationScope _navigationScope;

		// Token: 0x04000010 RID: 16
		private GamepadNavigationForcedScopeCollection _scopeCollection;

		// Token: 0x04000011 RID: 17
		private bool _previousOpenState;

		// Token: 0x04000014 RID: 20
		private ButtonWidget _button;

		// Token: 0x04000015 RID: 21
		private ListPanel _listPanel;

		// Token: 0x04000016 RID: 22
		private int _currentSelectedIndex;

		// Token: 0x04000017 RID: 23
		private Widget _dropdownContainerWidget;

		// Token: 0x04000018 RID: 24
		private Widget _dropdownClipWidget;

		// Token: 0x04000019 RID: 25
		private bool _isOpen;

		// Token: 0x0400001A RID: 26
		private bool _buttonClicked;

		// Token: 0x0400001B RID: 27
		private bool _updateSelectedItem = true;
	}
}
