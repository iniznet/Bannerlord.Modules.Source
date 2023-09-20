using System;
using System.Collections.Generic;
using System.Numerics;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.GauntletUI.GamepadNavigation;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets
{
	public class ContextMenuBrushWidget : BrushWidget
	{
		public float HorizontalPadding { get; set; } = 10f;

		public float VerticalPadding { get; set; } = 10f;

		private bool _isClickedOnOtherWidget
		{
			get
			{
				return this._isPrimaryClickedOnOtherWidget || this._isAlternateClickedOnOtherWidget;
			}
		}

		private bool _isPrimaryClickedOnOtherWidget
		{
			get
			{
				return this._latestMouseUpWidgetWhenActivated != base.EventManager.LatestMouseDownWidget && !base.CheckIsMyChildRecursive(base.EventManager.LatestMouseDownWidget);
			}
		}

		private bool _isAlternateClickedOnOtherWidget
		{
			get
			{
				return this._latestAltMouseUpWidgetWhenActivated != base.EventManager.LatestMouseAlternateDownWidget && !base.CheckIsMyChildRecursive(base.EventManager.LatestMouseAlternateDownWidget);
			}
		}

		public ContextMenuBrushWidget(UIContext context)
			: base(context)
		{
			this._newlyAddedItemList = new List<ContextMenuItemWidget>();
			this._newlyRemovedItemList = new List<ContextMenuItemWidget>();
			base.EventManager.AddLateUpdateAction(this, new Action<float>(this.CustomLateUpdate), 1);
		}

		private void CustomLateUpdate(float dt)
		{
			if (!this._isDestroyed)
			{
				base.EventManager.AddLateUpdateAction(this, new Action<float>(this.CustomLateUpdate), 1);
				if (base.IsVisible && !base.IsRecursivelyVisible())
				{
					this.Deactivate();
				}
				if (base.IsVisible && !this._isActivatedThisFrame && this._isClickedOnOtherWidget)
				{
					this.Deactivate();
				}
				if (this._isActivatedThisFrame)
				{
					Vector2 vector = this.DetermineMenuPositionFromMousePosition(base.EventManager.MousePosition);
					this._targetPosition = base.ParentWidget.GetLocalPoint(vector);
					this._isActivatedThisFrame = false;
				}
				base.ScaledPositionXOffset = MathF.Clamp(this._targetPosition.X, 0f, base.EventManager.PageSize.X - base.Size.X);
				base.ScaledPositionYOffset = MathF.Clamp(this._targetPosition.Y, 0f, base.EventManager.PageSize.Y - base.Size.Y);
				this.HandleNewlyAddedRemovedList();
			}
		}

		private void HandleNewlyAddedRemovedList()
		{
			foreach (ContextMenuItemWidget contextMenuItemWidget in this._newlyAddedItemList)
			{
				contextMenuItemWidget.ActionButtonWidget.ClickEventHandlers.Add(new Action<Widget>(this.OnAnyAction));
			}
			this._newlyAddedItemList.Clear();
			foreach (ContextMenuItemWidget contextMenuItemWidget2 in this._newlyRemovedItemList)
			{
				contextMenuItemWidget2.ActionButtonWidget.ClickEventHandlers.Remove(new Action<Widget>(this.OnAnyAction));
			}
			this._newlyRemovedItemList.Clear();
		}

		protected override void OnDisconnectedFromRoot()
		{
			base.OnDisconnectedFromRoot();
			this._isDestroyed = true;
		}

		private void Activate()
		{
			this._isActivatedThisFrame = true;
			this._latestMouseUpWidgetWhenActivated = base.EventManager.LatestMouseDownWidget;
			this._latestAltMouseUpWidgetWhenActivated = base.EventManager.LatestMouseAlternateDownWidget;
			base.IsVisible = true;
			this.AddGamepadNavigation();
		}

		private void Deactivate()
		{
			base.ScaledPositionXOffset = base.EventManager.PageSize.X;
			base.ScaledPositionYOffset = base.EventManager.PageSize.Y;
			base.IsVisible = false;
			this.DestroyGamepadNavigation();
		}

		private void AddGamepadNavigation()
		{
			if (this._navigationScope == null && this._scopeCollection == null)
			{
				this._navigationScope = new GamepadNavigationScope
				{
					ScopeID = "ContextMenuScope",
					ScopeMovements = GamepadNavigationTypes.Vertical,
					ParentWidget = this,
					DoNotAutomaticallyFindChildren = true,
					HasCircularMovement = true
				};
				base.EventManager.AddNavigationScope(this._navigationScope, false);
				for (int i = 0; i < this.ActionListPanel.Children.Count; i++)
				{
					ContextMenuItemWidget contextMenuItemWidget;
					if ((contextMenuItemWidget = this.ActionListPanel.Children[i] as ContextMenuItemWidget) != null)
					{
						this._navigationScope.AddWidgetAtIndex(contextMenuItemWidget, i);
					}
				}
				this._scopeCollection = new GamepadNavigationForcedScopeCollection
				{
					CollectionID = "ContextMenuCollection",
					CollectionOrder = 999,
					ParentWidget = this
				};
				base.EventManager.AddForcedScopeCollection(this._scopeCollection);
			}
		}

		private void DestroyGamepadNavigation()
		{
			if (this._navigationScope != null && this._scopeCollection != null)
			{
				this._navigationScope.ClearNavigatableWidgets();
				this._scopeCollection.ClearScopes();
				base.EventManager.RemoveNavigationScope(this._navigationScope);
				base.EventManager.RemoveForcedScopeCollection(this._scopeCollection);
				this._navigationScope = null;
				this._scopeCollection = null;
			}
		}

		private void OnScrollOfContextItem(float scrollAmount)
		{
			this.Deactivate();
		}

		private void OnAnyAction(Widget obj)
		{
			this.Deactivate();
		}

		private void OnNewActionButtonRemoved(Widget obj, Widget child)
		{
			ContextMenuItemWidget contextMenuItemWidget;
			if ((contextMenuItemWidget = child as ContextMenuItemWidget) != null)
			{
				this._newlyRemovedItemList.Add(contextMenuItemWidget);
			}
		}

		private void OnNewActionButtonAdded(Widget listPanel, Widget child)
		{
			ContextMenuItemWidget contextMenuItemWidget;
			if ((contextMenuItemWidget = child as ContextMenuItemWidget) != null)
			{
				this._newlyAddedItemList.Add(contextMenuItemWidget);
			}
		}

		private Vector2 DetermineMenuPositionFromMousePosition(Vector2 mousePosition)
		{
			bool flag = mousePosition.X > base.EventManager.PageSize.X / 2f;
			bool flag2 = mousePosition.Y > base.EventManager.PageSize.Y / 2f;
			float num = (flag ? (mousePosition.X - base.Size.X) : mousePosition.X);
			float num2 = (flag2 ? (mousePosition.Y - base.Size.Y) : mousePosition.Y);
			float num3 = num + (flag ? (-this.HorizontalPadding) : this.HorizontalPadding);
			num2 += (flag2 ? (-this.VerticalPadding) : this.VerticalPadding);
			return new Vector2(num3, num2);
		}

		[Editor(false)]
		public bool IsActivated
		{
			get
			{
				return this._isActivated;
			}
			set
			{
				if (this._isActivated != value)
				{
					this._isActivated = value;
					base.OnPropertyChanged(value, "IsActivated");
					if (this._isActivated)
					{
						this.Activate();
						return;
					}
					this.Deactivate();
				}
			}
		}

		[Editor(false)]
		public ListPanel ActionListPanel
		{
			get
			{
				return this._actionListPanel;
			}
			set
			{
				if (this._actionListPanel != value)
				{
					this._actionListPanel = value;
					this._actionListPanel.ItemAddEventHandlers.Add(new Action<Widget, Widget>(this.OnNewActionButtonAdded));
					this._actionListPanel.ItemRemoveEventHandlers.Add(new Action<Widget, Widget>(this.OnNewActionButtonRemoved));
					base.OnPropertyChanged<ListPanel>(value, "ActionListPanel");
				}
			}
		}

		[Editor(false)]
		public ScrollablePanel ScrollPanelToWatch
		{
			get
			{
				return this._scrollPanelToWatch;
			}
			set
			{
				if (this._scrollPanelToWatch != value)
				{
					this._scrollPanelToWatch = value;
					this._scrollPanelToWatch.OnScroll += this.OnScrollOfContextItem;
					base.OnPropertyChanged<ScrollablePanel>(value, "ScrollPanelToWatch");
				}
			}
		}

		private Vector2 _targetPosition;

		private Widget _latestMouseUpWidgetWhenActivated;

		private Widget _latestAltMouseUpWidgetWhenActivated;

		private bool _isDestroyed;

		private bool _isActivatedThisFrame;

		private List<ContextMenuItemWidget> _newlyAddedItemList;

		private List<ContextMenuItemWidget> _newlyRemovedItemList;

		private GamepadNavigationScope _navigationScope;

		private GamepadNavigationForcedScopeCollection _scopeCollection;

		private bool _isActivated;

		public ScrollablePanel _scrollPanelToWatch;

		public ListPanel _actionListPanel;
	}
}
