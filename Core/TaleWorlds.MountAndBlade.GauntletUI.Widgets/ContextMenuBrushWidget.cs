using System;
using System.Collections.Generic;
using System.Numerics;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.GauntletUI.GamepadNavigation;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets
{
	// Token: 0x02000012 RID: 18
	public class ContextMenuBrushWidget : BrushWidget
	{
		// Token: 0x17000042 RID: 66
		// (get) Token: 0x060000C4 RID: 196 RVA: 0x00003F18 File Offset: 0x00002118
		// (set) Token: 0x060000C5 RID: 197 RVA: 0x00003F20 File Offset: 0x00002120
		public float HorizontalPadding { get; set; } = 10f;

		// Token: 0x17000043 RID: 67
		// (get) Token: 0x060000C6 RID: 198 RVA: 0x00003F29 File Offset: 0x00002129
		// (set) Token: 0x060000C7 RID: 199 RVA: 0x00003F31 File Offset: 0x00002131
		public float VerticalPadding { get; set; } = 10f;

		// Token: 0x17000044 RID: 68
		// (get) Token: 0x060000C8 RID: 200 RVA: 0x00003F3A File Offset: 0x0000213A
		private bool _isClickedOnOtherWidget
		{
			get
			{
				return this._isPrimaryClickedOnOtherWidget || this._isAlternateClickedOnOtherWidget;
			}
		}

		// Token: 0x17000045 RID: 69
		// (get) Token: 0x060000C9 RID: 201 RVA: 0x00003F4C File Offset: 0x0000214C
		private bool _isPrimaryClickedOnOtherWidget
		{
			get
			{
				return this._latestMouseUpWidgetWhenActivated != base.EventManager.LatestMouseDownWidget && !base.CheckIsMyChildRecursive(base.EventManager.LatestMouseDownWidget);
			}
		}

		// Token: 0x17000046 RID: 70
		// (get) Token: 0x060000CA RID: 202 RVA: 0x00003F77 File Offset: 0x00002177
		private bool _isAlternateClickedOnOtherWidget
		{
			get
			{
				return this._latestAltMouseUpWidgetWhenActivated != base.EventManager.LatestMouseAlternateDownWidget && !base.CheckIsMyChildRecursive(base.EventManager.LatestMouseAlternateDownWidget);
			}
		}

		// Token: 0x060000CB RID: 203 RVA: 0x00003FA4 File Offset: 0x000021A4
		public ContextMenuBrushWidget(UIContext context)
			: base(context)
		{
			this._newlyAddedItemList = new List<ContextMenuItemWidget>();
			this._newlyRemovedItemList = new List<ContextMenuItemWidget>();
			base.EventManager.AddLateUpdateAction(this, new Action<float>(this.CustomLateUpdate), 1);
		}

		// Token: 0x060000CC RID: 204 RVA: 0x00004000 File Offset: 0x00002200
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

		// Token: 0x060000CD RID: 205 RVA: 0x0000410C File Offset: 0x0000230C
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

		// Token: 0x060000CE RID: 206 RVA: 0x000041E0 File Offset: 0x000023E0
		protected override void OnDisconnectedFromRoot()
		{
			base.OnDisconnectedFromRoot();
			this._isDestroyed = true;
		}

		// Token: 0x060000CF RID: 207 RVA: 0x000041EF File Offset: 0x000023EF
		private void Activate()
		{
			this._isActivatedThisFrame = true;
			this._latestMouseUpWidgetWhenActivated = base.EventManager.LatestMouseDownWidget;
			this._latestAltMouseUpWidgetWhenActivated = base.EventManager.LatestMouseAlternateDownWidget;
			base.IsVisible = true;
			this.AddGamepadNavigation();
		}

		// Token: 0x060000D0 RID: 208 RVA: 0x00004227 File Offset: 0x00002427
		private void Deactivate()
		{
			base.ScaledPositionXOffset = base.EventManager.PageSize.X;
			base.ScaledPositionYOffset = base.EventManager.PageSize.Y;
			base.IsVisible = false;
			this.DestroyGamepadNavigation();
		}

		// Token: 0x060000D1 RID: 209 RVA: 0x00004264 File Offset: 0x00002464
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

		// Token: 0x060000D2 RID: 210 RVA: 0x00004348 File Offset: 0x00002548
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

		// Token: 0x060000D3 RID: 211 RVA: 0x000043AB File Offset: 0x000025AB
		private void OnScrollOfContextItem(float scrollAmount)
		{
			this.Deactivate();
		}

		// Token: 0x060000D4 RID: 212 RVA: 0x000043B3 File Offset: 0x000025B3
		private void OnAnyAction(Widget obj)
		{
			this.Deactivate();
		}

		// Token: 0x060000D5 RID: 213 RVA: 0x000043BC File Offset: 0x000025BC
		private void OnNewActionButtonRemoved(Widget obj, Widget child)
		{
			ContextMenuItemWidget contextMenuItemWidget;
			if ((contextMenuItemWidget = child as ContextMenuItemWidget) != null)
			{
				this._newlyRemovedItemList.Add(contextMenuItemWidget);
			}
		}

		// Token: 0x060000D6 RID: 214 RVA: 0x000043E0 File Offset: 0x000025E0
		private void OnNewActionButtonAdded(Widget listPanel, Widget child)
		{
			ContextMenuItemWidget contextMenuItemWidget;
			if ((contextMenuItemWidget = child as ContextMenuItemWidget) != null)
			{
				this._newlyAddedItemList.Add(contextMenuItemWidget);
			}
		}

		// Token: 0x060000D7 RID: 215 RVA: 0x00004404 File Offset: 0x00002604
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

		// Token: 0x17000047 RID: 71
		// (get) Token: 0x060000D8 RID: 216 RVA: 0x000044B8 File Offset: 0x000026B8
		// (set) Token: 0x060000D9 RID: 217 RVA: 0x000044C0 File Offset: 0x000026C0
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

		// Token: 0x17000048 RID: 72
		// (get) Token: 0x060000DA RID: 218 RVA: 0x000044F3 File Offset: 0x000026F3
		// (set) Token: 0x060000DB RID: 219 RVA: 0x000044FC File Offset: 0x000026FC
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

		// Token: 0x17000049 RID: 73
		// (get) Token: 0x060000DC RID: 220 RVA: 0x0000455D File Offset: 0x0000275D
		// (set) Token: 0x060000DD RID: 221 RVA: 0x00004565 File Offset: 0x00002765
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

		// Token: 0x04000061 RID: 97
		private Vector2 _targetPosition;

		// Token: 0x04000062 RID: 98
		private Widget _latestMouseUpWidgetWhenActivated;

		// Token: 0x04000063 RID: 99
		private Widget _latestAltMouseUpWidgetWhenActivated;

		// Token: 0x04000064 RID: 100
		private bool _isDestroyed;

		// Token: 0x04000065 RID: 101
		private bool _isActivatedThisFrame;

		// Token: 0x04000066 RID: 102
		private List<ContextMenuItemWidget> _newlyAddedItemList;

		// Token: 0x04000067 RID: 103
		private List<ContextMenuItemWidget> _newlyRemovedItemList;

		// Token: 0x04000068 RID: 104
		private GamepadNavigationScope _navigationScope;

		// Token: 0x04000069 RID: 105
		private GamepadNavigationForcedScopeCollection _scopeCollection;

		// Token: 0x0400006A RID: 106
		private bool _isActivated;

		// Token: 0x0400006B RID: 107
		public ScrollablePanel _scrollPanelToWatch;

		// Token: 0x0400006C RID: 108
		public ListPanel _actionListPanel;
	}
}
