using System;
using System.Collections.Generic;
using System.Numerics;
using TaleWorlds.GauntletUI.GamepadNavigation;
using TaleWorlds.InputSystem;

namespace TaleWorlds.GauntletUI.BaseTypes
{
	// Token: 0x02000058 RID: 88
	public class DropdownWidget : Widget
	{
		// Token: 0x17000198 RID: 408
		// (get) Token: 0x06000583 RID: 1411 RVA: 0x00017C4E File Offset: 0x00015E4E
		private Vector2 ListPanelPositionInsideUsableArea
		{
			get
			{
				return this.ListPanel.GlobalPosition - new Vector2(base.EventManager.LeftUsableAreaStart, base.EventManager.TopUsableAreaStart);
			}
		}

		// Token: 0x17000199 RID: 409
		// (get) Token: 0x06000584 RID: 1412 RVA: 0x00017C7B File Offset: 0x00015E7B
		// (set) Token: 0x06000585 RID: 1413 RVA: 0x00017C83 File Offset: 0x00015E83
		[Editor(false)]
		public RichTextWidget RichTextWidget { get; set; }

		// Token: 0x1700019A RID: 410
		// (get) Token: 0x06000586 RID: 1414 RVA: 0x00017C8C File Offset: 0x00015E8C
		// (set) Token: 0x06000587 RID: 1415 RVA: 0x00017C94 File Offset: 0x00015E94
		[Editor(false)]
		public bool DoNotHandleDropdownListPanel { get; set; }

		// Token: 0x06000588 RID: 1416 RVA: 0x00017CA0 File Offset: 0x00015EA0
		public DropdownWidget(UIContext context)
			: base(context)
		{
			this._clickHandler = new Action<Widget>(this.OnButtonClick);
			this._listSelectionHandler = new Action<Widget>(this.OnSelectionChanged);
			this._listItemRemovedHandler = new Action<Widget, Widget>(this.OnListItemRemoved);
			this._listItemAddedHandler = new Action<Widget, Widget>(this.OnListItemAdded);
			base.UsedNavigationMovements = GamepadNavigationTypes.Horizontal;
		}

		// Token: 0x06000589 RID: 1417 RVA: 0x00017D0C File Offset: 0x00015F0C
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

		// Token: 0x0600058A RID: 1418 RVA: 0x00017E15 File Offset: 0x00016015
		protected override void OnConnectedToRoot()
		{
			base.OnConnectedToRoot();
			this.ScrollablePanel = this.GetParentScrollablePanelOfWidget(this);
		}

		// Token: 0x0600058B RID: 1419 RVA: 0x00017E2A File Offset: 0x0001602A
		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (!this.DoNotHandleDropdownListPanel)
			{
				this.UpdateListPanelPosition();
			}
			this.UpdateGamepadNavigationControls();
		}

		// Token: 0x0600058C RID: 1420 RVA: 0x00017E48 File Offset: 0x00016048
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

		// Token: 0x0600058D RID: 1421 RVA: 0x00017F88 File Offset: 0x00016188
		private void UpdateListPanelPosition()
		{
			this.ListPanel.HorizontalAlignment = HorizontalAlignment.Left;
			this.ListPanel.VerticalAlignment = VerticalAlignment.Top;
			float num = (base.Size.X - this._listPanel.Size.X) * 0.5f;
			this.ListPanel.MarginTop = (base.GlobalPosition.Y + this.Button.Size.Y - base.EventManager.TopUsableAreaStart) * base._inverseScaleToUse;
			this.ListPanel.MarginLeft = (base.GlobalPosition.X + num - base.EventManager.LeftUsableAreaStart) * base._inverseScaleToUse;
		}

		// Token: 0x0600058E RID: 1422 RVA: 0x00018038 File Offset: 0x00016238
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

		// Token: 0x0600058F RID: 1423 RVA: 0x00018098 File Offset: 0x00016298
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

		// Token: 0x06000590 RID: 1424 RVA: 0x000180EC File Offset: 0x000162EC
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

		// Token: 0x06000591 RID: 1425 RVA: 0x0001823F File Offset: 0x0001643F
		private void OnWidgetGainedNavigationFocus(Widget widget)
		{
			ScrollablePanel scrollablePanel = this.ScrollablePanel;
			if (scrollablePanel == null)
			{
				return;
			}
			scrollablePanel.ScrollToChild(widget, -1f, -1f, 0, 0, 0f, 0f);
		}

		// Token: 0x06000592 RID: 1426 RVA: 0x00018268 File Offset: 0x00016468
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

		// Token: 0x06000593 RID: 1427 RVA: 0x00018290 File Offset: 0x00016490
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

		// Token: 0x06000594 RID: 1428 RVA: 0x000182D0 File Offset: 0x000164D0
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

		// Token: 0x06000595 RID: 1429 RVA: 0x000183B0 File Offset: 0x000165B0
		public void OnButtonClick(Widget widget)
		{
			this._buttonClicked = true;
			this._closeNextFrame = false;
		}

		// Token: 0x06000596 RID: 1430 RVA: 0x000183C0 File Offset: 0x000165C0
		public void UpdateButtonText(string text)
		{
			if (this.RichTextWidget != null)
			{
				this.RichTextWidget.Text = ((!string.IsNullOrEmpty(text)) ? text : " ");
			}
		}

		// Token: 0x06000597 RID: 1431 RVA: 0x000183E5 File Offset: 0x000165E5
		public void OnListItemAdded(Widget parentWidget, Widget newChild)
		{
			this._isSelectedItemDirty = true;
		}

		// Token: 0x06000598 RID: 1432 RVA: 0x000183EE File Offset: 0x000165EE
		public void OnListItemRemoved(Widget removedItem, Widget removedChild)
		{
			this._isSelectedItemDirty = true;
		}

		// Token: 0x06000599 RID: 1433 RVA: 0x000183F7 File Offset: 0x000165F7
		public void OnSelectionChanged(Widget widget)
		{
			this.CurrentSelectedIndex = this.ListPanelValue;
			this._isSelectedItemDirty = true;
			base.OnPropertyChanged(this.CurrentSelectedIndex, "CurrentSelectedIndex");
		}

		// Token: 0x0600059A RID: 1434 RVA: 0x00018420 File Offset: 0x00016620
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

		// Token: 0x1700019B RID: 411
		// (get) Token: 0x0600059B RID: 1435 RVA: 0x00018510 File Offset: 0x00016710
		// (set) Token: 0x0600059C RID: 1436 RVA: 0x00018518 File Offset: 0x00016718
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

		// Token: 0x1700019C RID: 412
		// (get) Token: 0x0600059D RID: 1437 RVA: 0x00018536 File Offset: 0x00016736
		// (set) Token: 0x0600059E RID: 1438 RVA: 0x00018540 File Offset: 0x00016740
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

		// Token: 0x1700019D RID: 413
		// (get) Token: 0x0600059F RID: 1439 RVA: 0x00018594 File Offset: 0x00016794
		// (set) Token: 0x060005A0 RID: 1440 RVA: 0x0001859C File Offset: 0x0001679C
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

		// Token: 0x1700019E RID: 414
		// (get) Token: 0x060005A1 RID: 1441 RVA: 0x00018684 File Offset: 0x00016884
		// (set) Token: 0x060005A2 RID: 1442 RVA: 0x0001868C File Offset: 0x0001688C
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

		// Token: 0x1700019F RID: 415
		// (get) Token: 0x060005A3 RID: 1443 RVA: 0x000186B4 File Offset: 0x000168B4
		// (set) Token: 0x060005A4 RID: 1444 RVA: 0x000186CB File Offset: 0x000168CB
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

		// Token: 0x170001A0 RID: 416
		// (get) Token: 0x060005A5 RID: 1445 RVA: 0x000186EF File Offset: 0x000168EF
		// (set) Token: 0x060005A6 RID: 1446 RVA: 0x000186F7 File Offset: 0x000168F7
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

		// Token: 0x040002A9 RID: 681
		public Action<DropdownWidget> OnOpenStateChanged;

		// Token: 0x040002AA RID: 682
		private readonly Action<Widget> _clickHandler;

		// Token: 0x040002AB RID: 683
		private readonly Action<Widget> _listSelectionHandler;

		// Token: 0x040002AC RID: 684
		private readonly Action<Widget, Widget> _listItemRemovedHandler;

		// Token: 0x040002AD RID: 685
		private readonly Action<Widget, Widget> _listItemAddedHandler;

		// Token: 0x040002AE RID: 686
		private Vector2 _listPanelOpenPosition;

		// Token: 0x040002AF RID: 687
		private int _openFrameCounter;

		// Token: 0x040002B0 RID: 688
		private bool _isSelectedItemDirty = true;

		// Token: 0x040002B1 RID: 689
		private bool _changedByControllerNavigation;

		// Token: 0x040002B2 RID: 690
		private GamepadNavigationScope _navigationScope;

		// Token: 0x040002B3 RID: 691
		private GamepadNavigationForcedScopeCollection _scopeCollection;

		// Token: 0x040002B6 RID: 694
		private ScrollablePanel _scrollablePanel;

		// Token: 0x040002B7 RID: 695
		private ButtonWidget _button;

		// Token: 0x040002B8 RID: 696
		private ListPanel _listPanel;

		// Token: 0x040002B9 RID: 697
		private int _currentSelectedIndex;

		// Token: 0x040002BA RID: 698
		private bool _closeNextFrame;

		// Token: 0x040002BB RID: 699
		private bool _isOpen;

		// Token: 0x040002BC RID: 700
		private bool _buttonClicked;
	}
}
