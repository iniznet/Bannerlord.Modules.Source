using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.GauntletUI.ExtraWidgets;
using TaleWorlds.GauntletUI.GamepadNavigation;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Inventory
{
	// Token: 0x02000125 RID: 293
	public class InventoryScreenWidget : Widget
	{
		// Token: 0x06000F25 RID: 3877 RVA: 0x00029FD3 File Offset: 0x000281D3
		public InventoryScreenWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000F26 RID: 3878 RVA: 0x00029FE4 File Offset: 0x000281E4
		private T IsWidgetChildOfType<T>(Widget currentWidget) where T : Widget
		{
			while (currentWidget != null)
			{
				if (currentWidget is T)
				{
					return (T)((object)currentWidget);
				}
				currentWidget = currentWidget.ParentWidget;
			}
			return default(T);
		}

		// Token: 0x06000F27 RID: 3879 RVA: 0x0002A016 File Offset: 0x00028216
		private bool IsWidgetChildOf(Widget parentWidget, Widget currentWidget)
		{
			while (currentWidget != null)
			{
				if (currentWidget == parentWidget)
				{
					return true;
				}
				currentWidget = currentWidget.ParentWidget;
			}
			return false;
		}

		// Token: 0x06000F28 RID: 3880 RVA: 0x0002A02C File Offset: 0x0002822C
		private bool IsWidgetChildOfId(string parentId, Widget currentWidget)
		{
			while (currentWidget != null)
			{
				if (currentWidget.Id == parentId)
				{
					return true;
				}
				currentWidget = currentWidget.ParentWidget;
			}
			return false;
		}

		// Token: 0x06000F29 RID: 3881 RVA: 0x0002A04C File Offset: 0x0002824C
		private InventoryListPanel GetCurrentHoveredListPanel()
		{
			for (int i = 0; i < base.EventManager.MouseOveredViews.Count; i++)
			{
				InventoryListPanel inventoryListPanel;
				if ((inventoryListPanel = base.EventManager.MouseOveredViews[i] as InventoryListPanel) != null)
				{
					return inventoryListPanel;
				}
			}
			return null;
		}

		// Token: 0x06000F2A RID: 3882 RVA: 0x0002A094 File Offset: 0x00028294
		protected override void OnUpdate(float dt)
		{
			base.OnUpdate(dt);
			if (!this._eventsRegistered)
			{
				ListPanel listPanel = this.OtherInventoryListWidget.InnerPanel as ListPanel;
				ListPanel listPanel2 = ((listPanel != null) ? listPanel.GetChild(0) : null) as ListPanel;
				if (listPanel2 != null)
				{
					listPanel2.ItemAddEventHandlers.Add(new Action<Widget, Widget>(this.OnNewInventoryItemAdded));
				}
				ListPanel listPanel3 = this.PlayerInventoryListWidget.InnerPanel as ListPanel;
				ListPanel listPanel4 = ((listPanel3 != null) ? listPanel3.GetChild(0) : null) as ListPanel;
				if (listPanel4 != null)
				{
					listPanel4.ItemAddEventHandlers.Add(new Action<Widget, Widget>(this.OnNewInventoryItemAdded));
				}
				this._eventsRegistered = true;
			}
			if (base.EventManager.DraggedWidget == null)
			{
				this.TargetEquipmentIndex = -1;
				this._currentDraggedItemWidget = null;
			}
			Widget latestMouseDownWidget = base.EventManager.LatestMouseDownWidget;
			bool flag;
			if (latestMouseDownWidget != null)
			{
				if (!(latestMouseDownWidget is InventoryItemButtonWidget))
				{
					InventoryEquippedItemControlsBrushWidget equippedItemControls = this.EquippedItemControls;
					if (equippedItemControls == null || !equippedItemControls.CheckIsMyChildRecursive(latestMouseDownWidget))
					{
						InventoryItemButtonWidget currentSelectedItemWidget = this._currentSelectedItemWidget;
						if (currentSelectedItemWidget == null || !currentSelectedItemWidget.CheckIsMyChildRecursive(latestMouseDownWidget))
						{
							InventoryItemButtonWidget currentSelectedOtherItemWidget = this._currentSelectedOtherItemWidget;
							flag = currentSelectedOtherItemWidget != null && currentSelectedOtherItemWidget.CheckIsMyChildRecursive(latestMouseDownWidget);
							goto IL_10A;
						}
					}
				}
				flag = true;
			}
			else
			{
				flag = false;
			}
			IL_10A:
			bool flag2 = flag;
			bool flag3 = this.IsWidgetChildOf(this.InventoryTooltip, latestMouseDownWidget);
			if (latestMouseDownWidget == null || (this._currentSelectedItemWidget != null && !flag2 && !flag3 && !this.ItemPreviewWidget.IsVisible))
			{
				this.SetCurrentTuple(null, false);
			}
			Widget hoveredView = base.EventManager.HoveredView;
			if (hoveredView != null)
			{
				InventoryItemButtonWidget inventoryItemButtonWidget = this.IsWidgetChildOfType<InventoryItemButtonWidget>(hoveredView);
				bool flag4 = this.IsWidgetChildOfId("InventoryTooltip", hoveredView);
				if (inventoryItemButtonWidget != null)
				{
					this.ItemWidgetHoverBegin(inventoryItemButtonWidget);
				}
				else if (flag4 && GauntletGamepadNavigationManager.Instance.IsCursorMovingForNavigation)
				{
					this.ItemWidgetHoverEnd(null);
				}
				else if (!flag4 && hoveredView.ParentWidget != null)
				{
					this.ItemWidgetHoverEnd(null);
				}
			}
			else
			{
				this.ItemWidgetHoverEnd(null);
			}
			this.UpdateControllerTransferKeyVisuals();
		}

		// Token: 0x06000F2B RID: 3883 RVA: 0x0002A250 File Offset: 0x00028450
		private void UpdateControllerTransferKeyVisuals()
		{
			InventoryListPanel currentHoveredListPanel = this.GetCurrentHoveredListPanel();
			this.IsFocusedOnItemList = currentHoveredListPanel != null;
			if (!base.EventManager.IsControllerActive || !this.IsFocusedOnItemList)
			{
				this.PreviousCharacterInputVisualParent.IsVisible = true;
				this.NextCharacterInputVisualParent.IsVisible = true;
				this.TransferInputKeyVisualWidget.IsVisible = false;
				return;
			}
			this.PreviousCharacterInputVisualParent.IsVisible = false;
			this.NextCharacterInputVisualParent.IsVisible = false;
			InventoryItemTupleWidget inventoryItemTupleWidget;
			if ((inventoryItemTupleWidget = this._currentHoveredItemWidget as InventoryItemTupleWidget) != null && inventoryItemTupleWidget.IsHovered && inventoryItemTupleWidget.IsTransferable)
			{
				this.TransferInputKeyVisualWidget.IsVisible = true;
				Vector2 vector;
				if (inventoryItemTupleWidget.IsRightSide)
				{
					InputKeyVisualWidget transferInputKeyVisualWidget = this.TransferInputKeyVisualWidget;
					InputKeyVisualWidget nextCharacterInputKeyVisual = this._nextCharacterInputKeyVisual;
					transferInputKeyVisualWidget.KeyID = ((nextCharacterInputKeyVisual != null) ? nextCharacterInputKeyVisual.KeyID : null) ?? "";
					vector = this._currentHoveredItemWidget.GlobalPosition - new Vector2(base.EventManager.LeftUsableAreaStart, base.EventManager.TopUsableAreaStart + 20f * base._scaleToUse);
				}
				else
				{
					InputKeyVisualWidget transferInputKeyVisualWidget2 = this.TransferInputKeyVisualWidget;
					InputKeyVisualWidget previousCharacterInputKeyVisual = this._previousCharacterInputKeyVisual;
					transferInputKeyVisualWidget2.KeyID = ((previousCharacterInputKeyVisual != null) ? previousCharacterInputKeyVisual.KeyID : null) ?? "";
					vector = this._currentHoveredItemWidget.GlobalPosition - new Vector2(base.EventManager.LeftUsableAreaStart + 60f * base._scaleToUse - this._currentHoveredItemWidget.Size.X, base.EventManager.TopUsableAreaStart + 20f * base._scaleToUse);
				}
				this.TransferInputKeyVisualWidget.ScaledPositionXOffset = vector.X;
				this.TransferInputKeyVisualWidget.ScaledPositionYOffset = vector.Y;
				return;
			}
			this.TransferInputKeyVisualWidget.IsVisible = false;
		}

		// Token: 0x06000F2C RID: 3884 RVA: 0x0002A410 File Offset: 0x00028610
		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (this._newAddedItem != null)
			{
				string itemID = this._newAddedItem.ItemID;
				InventoryItemTupleWidget inventoryItemTupleWidget = this._currentSelectedItemWidget as InventoryItemTupleWidget;
				if (itemID == ((inventoryItemTupleWidget != null) ? inventoryItemTupleWidget.ItemID : null))
				{
					this._currentSelectedOtherItemWidget = this._newAddedItem;
					(this._currentSelectedOtherItemWidget as InventoryItemTupleWidget).TransferRequestHandlers.Add(new Action<InventoryItemTupleWidget>(this.OnTransferItemRequested));
					this._newAddedItem.IsSelected = true;
					this.UpdateScrollTarget(this._newAddedItem.IsRightSide);
				}
				this._newAddedItem = null;
			}
			if (this._focusLostThisFrame)
			{
				base.EventFired("OnFocusLose", Array.Empty<object>());
				this._focusLostThisFrame = false;
			}
			this.UpdateTooltipPosition();
		}

		// Token: 0x06000F2D RID: 3885 RVA: 0x0002A4CC File Offset: 0x000286CC
		private void UpdateTooltipPosition()
		{
			if (base.EventManager.DraggedWidget != null)
			{
				this.InventoryTooltip.IsHidden = true;
			}
			InventoryItemButtonWidget currentHoveredItemWidget = this._currentHoveredItemWidget;
			if (((currentHoveredItemWidget != null) ? currentHoveredItemWidget.ParentWidget : null) == null)
			{
				this._lastDisplayedTooltipItem = null;
				return;
			}
			if (this._tooltipHiddenFrameCount < this.TooltipHideFrameLength)
			{
				this._tooltipHiddenFrameCount++;
				this.InventoryTooltip.PositionXOffset = 5000f;
				this.InventoryTooltip.PositionYOffset = 5000f;
				return;
			}
			if (this._currentHoveredItemWidget.IsRightSide)
			{
				this.InventoryTooltip.ScaledPositionXOffset = this._currentHoveredItemWidget.ParentWidget.GlobalPosition.X - this.InventoryTooltip.Size.X + 10f * base._scaleToUse - base.EventManager.LeftUsableAreaStart;
			}
			else
			{
				this.InventoryTooltip.ScaledPositionXOffset = this._currentHoveredItemWidget.ParentWidget.GlobalPosition.X + this._currentHoveredItemWidget.ParentWidget.Size.X - 10f * base._scaleToUse - base.EventManager.LeftUsableAreaStart;
			}
			float num = base.EventManager.PageSize.Y - this.InventoryTooltip.MeasuredSize.Y;
			this.InventoryTooltip.ScaledPositionYOffset = Mathf.Clamp(this._currentHoveredItemWidget.GlobalPosition.Y - base.EventManager.TopUsableAreaStart, 0f, num);
			this._lastDisplayedTooltipItem = this._currentHoveredItemWidget;
		}

		// Token: 0x06000F2E RID: 3886 RVA: 0x0002A658 File Offset: 0x00028858
		public void SetCurrentTuple(InventoryItemButtonWidget itemWidget, bool isLeftSide)
		{
			this._focusLostThisFrame = itemWidget == null;
			if (this._currentSelectedItemWidget != null && this._currentSelectedItemWidget != itemWidget)
			{
				this._currentSelectedItemWidget.IsSelected = false;
				InventoryItemTupleWidget inventoryItemTupleWidget;
				if ((inventoryItemTupleWidget = this._currentSelectedItemWidget as InventoryItemTupleWidget) != null)
				{
					inventoryItemTupleWidget.TransferRequestHandlers.Remove(new Action<InventoryItemTupleWidget>(this.OnTransferItemRequested));
				}
				if (this._currentSelectedOtherItemWidget != null)
				{
					this._currentSelectedOtherItemWidget.IsSelected = false;
				}
			}
			InventoryItemTupleWidget inventoryItemTupleWidget2;
			InventoryItemTupleWidget inventoryItemTupleWidget3;
			if (itemWidget == null || ((inventoryItemTupleWidget2 = itemWidget as InventoryItemTupleWidget) != null && (inventoryItemTupleWidget3 = this._currentSelectedOtherItemWidget as InventoryItemTupleWidget) != null && inventoryItemTupleWidget2.ItemID == inventoryItemTupleWidget3.ItemID))
			{
				this._equippedItemControls.HidePanel();
				if (this._currentSelectedItemWidget != null)
				{
					this._currentSelectedItemWidget.IsSelected = false;
				}
				this._currentSelectedItemWidget = null;
				if (this._currentSelectedOtherItemWidget != null)
				{
					this._currentSelectedOtherItemWidget.IsSelected = false;
					(this._currentSelectedOtherItemWidget as InventoryItemTupleWidget).TransferRequestHandlers.Remove(new Action<InventoryItemTupleWidget>(this.OnTransferItemRequested));
				}
				this._currentSelectedOtherItemWidget = null;
				return;
			}
			if (this._currentSelectedItemWidget == itemWidget)
			{
				this.SetCurrentTuple(null, false);
				if (this._currentSelectedOtherItemWidget != null)
				{
					this._currentSelectedOtherItemWidget.IsSelected = false;
				}
				this._currentSelectedOtherItemWidget = null;
				return;
			}
			this._currentSelectedItemWidget = itemWidget;
			this.TargetEquipmentIndex = -1;
			this.TransactionCount = 1;
			if (this._currentSelectedItemWidget is InventoryEquippedItemSlotWidget)
			{
				this._equippedItemControls.ShowPanel(itemWidget);
				this._currentSelectedOtherItemWidget = null;
			}
			else
			{
				this._equippedItemControls.HidePanel();
				InventoryItemTupleWidget inventoryItemTupleWidget4;
				if ((inventoryItemTupleWidget4 = this._currentSelectedItemWidget as InventoryItemTupleWidget) != null)
				{
					inventoryItemTupleWidget4.TransferRequestHandlers.Add(new Action<InventoryItemTupleWidget>(this.OnTransferItemRequested));
					if (isLeftSide)
					{
						using (IEnumerator<Widget> enumerator = this.PlayerInventoryListWidget.AllChildren.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								InventoryItemTupleWidget inventoryItemTupleWidget5;
								if ((inventoryItemTupleWidget5 = enumerator.Current as InventoryItemTupleWidget) != null && inventoryItemTupleWidget5.ItemID == inventoryItemTupleWidget4.ItemID)
								{
									this._currentSelectedOtherItemWidget = inventoryItemTupleWidget5;
									this._currentSelectedOtherItemWidget.IsSelected = true;
									(this._currentSelectedOtherItemWidget as InventoryItemTupleWidget).TransferRequestHandlers.Add(new Action<InventoryItemTupleWidget>(this.OnTransferItemRequested));
									break;
								}
							}
							goto IL_2AD;
						}
					}
					using (IEnumerator<Widget> enumerator = this.OtherInventoryListWidget.AllChildren.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							InventoryItemTupleWidget inventoryItemTupleWidget6;
							if ((inventoryItemTupleWidget6 = enumerator.Current as InventoryItemTupleWidget) != null && inventoryItemTupleWidget6.ItemID == inventoryItemTupleWidget4.ItemID)
							{
								this._currentSelectedOtherItemWidget = inventoryItemTupleWidget6;
								this._currentSelectedOtherItemWidget.IsSelected = true;
								(this._currentSelectedOtherItemWidget as InventoryItemTupleWidget).TransferRequestHandlers.Add(new Action<InventoryItemTupleWidget>(this.OnTransferItemRequested));
								break;
							}
						}
					}
				}
			}
			IL_2AD:
			this.UpdateScrollTarget(isLeftSide);
		}

		// Token: 0x06000F2F RID: 3887 RVA: 0x0002A938 File Offset: 0x00028B38
		private void OnEquipmentControlsHidden()
		{
			this._currentSelectedItemWidget = null;
			if (this._currentSelectedOtherItemWidget != null)
			{
				this._currentSelectedOtherItemWidget.IsSelected = false;
				(this._currentSelectedOtherItemWidget as InventoryItemTupleWidget).TransferRequestHandlers.Remove(new Action<InventoryItemTupleWidget>(this.OnTransferItemRequested));
			}
			this._currentSelectedOtherItemWidget = null;
		}

		// Token: 0x06000F30 RID: 3888 RVA: 0x0002A989 File Offset: 0x00028B89
		private void OnTransferItemRequested(InventoryItemTupleWidget owner)
		{
			this.UpdateScrollTarget(!owner.IsRightSide);
		}

		// Token: 0x06000F31 RID: 3889 RVA: 0x0002A99A File Offset: 0x00028B9A
		private void TradeLabelOnPropertyChanged(PropertyOwnerObject owner, string propertyName, object value)
		{
			if (propertyName == "Text")
			{
				this.TradeLabel.IsDisabled = string.IsNullOrEmpty(this.TradeLabel.Text);
			}
		}

		// Token: 0x06000F32 RID: 3890 RVA: 0x0002A9C4 File Offset: 0x00028BC4
		private void EquippedItemControlsOnPreviewClick(Widget itemwidget)
		{
			this.ItemPreviewWidget.SetLastFocusedItem(null);
		}

		// Token: 0x06000F33 RID: 3891 RVA: 0x0002A9D4 File Offset: 0x00028BD4
		private void ItemWidgetHoverBegin(InventoryItemButtonWidget itemWidget)
		{
			if (this._currentHoveredItemWidget != itemWidget)
			{
				this._currentHoveredItemWidget = itemWidget;
				this._tooltipHiddenFrameCount = 0;
				Widget widget = this.InventoryTooltip.FindChild("TargetItemTooltip");
				if (this._currentHoveredItemWidget.IsRightSide)
				{
					widget.SetSiblingIndex(1, false);
				}
				else
				{
					widget.SetSiblingIndex(0, false);
				}
				this.InventoryTooltip.IsHidden = false;
				base.EventFired("ItemHoverBegin", new object[] { itemWidget });
			}
		}

		// Token: 0x06000F34 RID: 3892 RVA: 0x0002AA49 File Offset: 0x00028C49
		private void ItemWidgetHoverEnd(InventoryItemButtonWidget itemWidget)
		{
			if (this._currentHoveredItemWidget != null && itemWidget == null)
			{
				this._currentHoveredItemWidget = null;
				this.InventoryTooltip.IsHidden = true;
				base.EventFired("ItemHoverEnd", Array.Empty<object>());
			}
		}

		// Token: 0x06000F35 RID: 3893 RVA: 0x0002AA7C File Offset: 0x00028C7C
		public void ItemWidgetDragBegin(InventoryItemButtonWidget itemWidget)
		{
			InventoryEquippedItemControlsBrushWidget equippedItemControls = this.EquippedItemControls;
			if (equippedItemControls != null)
			{
				equippedItemControls.HidePanel();
			}
			this._currentDraggedItemWidget = itemWidget;
			InventoryEquippedItemSlotWidget inventoryEquippedItemSlotWidget = itemWidget as InventoryEquippedItemSlotWidget;
			if (inventoryEquippedItemSlotWidget != null)
			{
				this.TargetEquipmentIndex = inventoryEquippedItemSlotWidget.TargetEquipmentIndex;
				return;
			}
			this.TargetEquipmentIndex = itemWidget.EquipmentIndex;
		}

		// Token: 0x06000F36 RID: 3894 RVA: 0x0002AAC4 File Offset: 0x00028CC4
		public void ItemWidgetDrop(InventoryItemButtonWidget itemWidget)
		{
			if (this._currentDraggedItemWidget == itemWidget)
			{
				this._currentDraggedItemWidget = null;
				this.TargetEquipmentIndex = -1;
			}
		}

		// Token: 0x06000F37 RID: 3895 RVA: 0x0002AAE0 File Offset: 0x00028CE0
		private void OtherInventoryGoldTextOnPropertyChanged(PropertyOwnerObject owner, string propertyName, int value)
		{
			if (propertyName == "IntText")
			{
				bool flag = this.OtherInventoryGoldText.IntText > 0;
				this.OtherInventoryGoldText.IsVisible = flag;
				this.OtherInventoryGoldImage.IsVisible = flag;
			}
		}

		// Token: 0x06000F38 RID: 3896 RVA: 0x0002AB24 File Offset: 0x00028D24
		private void UpdateScrollTarget(bool isLeftSide)
		{
			if (this._currentSelectedOtherItemWidget != null)
			{
				if (isLeftSide)
				{
					this.PlayerInventoryListWidget.ScrollToChild(this._currentSelectedOtherItemWidget, -1f, 1f, 0, 400, 0.35f, 0f);
					return;
				}
				this.OtherInventoryListWidget.ScrollToChild(this._currentSelectedOtherItemWidget, -1f, 1f, 0, 400, 0.35f, 0f);
			}
		}

		// Token: 0x17000554 RID: 1364
		// (get) Token: 0x06000F39 RID: 3897 RVA: 0x0002AB93 File Offset: 0x00028D93
		// (set) Token: 0x06000F3A RID: 3898 RVA: 0x0002AB9B File Offset: 0x00028D9B
		[Editor(false)]
		public InputKeyVisualWidget TransferInputKeyVisualWidget
		{
			get
			{
				return this._transferInputKeyVisualWidget;
			}
			set
			{
				if (this._transferInputKeyVisualWidget != value)
				{
					this._transferInputKeyVisualWidget = value;
					base.OnPropertyChanged<InputKeyVisualWidget>(value, "TransferInputKeyVisualWidget");
				}
			}
		}

		// Token: 0x17000555 RID: 1365
		// (get) Token: 0x06000F3B RID: 3899 RVA: 0x0002ABB9 File Offset: 0x00028DB9
		// (set) Token: 0x06000F3C RID: 3900 RVA: 0x0002ABC4 File Offset: 0x00028DC4
		public Widget PreviousCharacterInputVisualParent
		{
			get
			{
				return this._previousCharacterInputVisualParent;
			}
			set
			{
				if (value != this._previousCharacterInputVisualParent)
				{
					this._previousCharacterInputVisualParent = value;
					if (this._previousCharacterInputVisualParent != null)
					{
						this._previousCharacterInputKeyVisual = this._previousCharacterInputVisualParent.Children.FirstOrDefault((Widget x) => x is InputKeyVisualWidget) as InputKeyVisualWidget;
					}
				}
			}
		}

		// Token: 0x17000556 RID: 1366
		// (get) Token: 0x06000F3D RID: 3901 RVA: 0x0002AC23 File Offset: 0x00028E23
		// (set) Token: 0x06000F3E RID: 3902 RVA: 0x0002AC2C File Offset: 0x00028E2C
		public Widget NextCharacterInputVisualParent
		{
			get
			{
				return this._nextCharacterInputVisualParent;
			}
			set
			{
				if (value != this._nextCharacterInputVisualParent)
				{
					this._nextCharacterInputVisualParent = value;
					if (this._nextCharacterInputVisualParent != null)
					{
						this._nextCharacterInputKeyVisual = this._nextCharacterInputVisualParent.Children.FirstOrDefault((Widget x) => x is InputKeyVisualWidget) as InputKeyVisualWidget;
					}
				}
			}
		}

		// Token: 0x17000557 RID: 1367
		// (get) Token: 0x06000F3F RID: 3903 RVA: 0x0002AC8B File Offset: 0x00028E8B
		// (set) Token: 0x06000F40 RID: 3904 RVA: 0x0002AC94 File Offset: 0x00028E94
		[Editor(false)]
		public RichTextWidget TradeLabel
		{
			get
			{
				return this._tradeLabel;
			}
			set
			{
				if (this._tradeLabel != value)
				{
					if (this._tradeLabel != null)
					{
						this._tradeLabel.PropertyChanged -= this.TradeLabelOnPropertyChanged;
					}
					this._tradeLabel = value;
					if (this._tradeLabel != null)
					{
						this._tradeLabel.PropertyChanged += this.TradeLabelOnPropertyChanged;
					}
					base.OnPropertyChanged<RichTextWidget>(value, "TradeLabel");
				}
			}
		}

		// Token: 0x17000558 RID: 1368
		// (get) Token: 0x06000F41 RID: 3905 RVA: 0x0002ACFB File Offset: 0x00028EFB
		// (set) Token: 0x06000F42 RID: 3906 RVA: 0x0002AD04 File Offset: 0x00028F04
		[Editor(false)]
		public InventoryEquippedItemControlsBrushWidget EquippedItemControls
		{
			get
			{
				return this._equippedItemControls;
			}
			set
			{
				if (this._equippedItemControls != value)
				{
					if (this._equippedItemControls != null)
					{
						this._equippedItemControls.OnPreviewClick -= this.EquippedItemControlsOnPreviewClick;
						this._equippedItemControls.OnHidePanel -= this.OnEquipmentControlsHidden;
					}
					this._equippedItemControls = value;
					if (this._equippedItemControls != null)
					{
						this._equippedItemControls.OnPreviewClick += this.EquippedItemControlsOnPreviewClick;
						this._equippedItemControls.OnHidePanel += this.OnEquipmentControlsHidden;
					}
					base.OnPropertyChanged<InventoryEquippedItemControlsBrushWidget>(value, "EquippedItemControls");
				}
			}
		}

		// Token: 0x17000559 RID: 1369
		// (get) Token: 0x06000F43 RID: 3907 RVA: 0x0002AD99 File Offset: 0x00028F99
		// (set) Token: 0x06000F44 RID: 3908 RVA: 0x0002ADA1 File Offset: 0x00028FA1
		[Editor(false)]
		public Widget InventoryTooltip
		{
			get
			{
				return this._inventoryTooltip;
			}
			set
			{
				if (this._inventoryTooltip != value)
				{
					this._inventoryTooltip = value;
					base.OnPropertyChanged<Widget>(value, "InventoryTooltip");
				}
			}
		}

		// Token: 0x1700055A RID: 1370
		// (get) Token: 0x06000F45 RID: 3909 RVA: 0x0002ADBF File Offset: 0x00028FBF
		// (set) Token: 0x06000F46 RID: 3910 RVA: 0x0002ADC7 File Offset: 0x00028FC7
		[Editor(false)]
		public InventoryItemPreviewWidget ItemPreviewWidget
		{
			get
			{
				return this._itemPreviewWidget;
			}
			set
			{
				if (this._itemPreviewWidget != value)
				{
					this._itemPreviewWidget = value;
					base.OnPropertyChanged<InventoryItemPreviewWidget>(value, "ItemPreviewWidget");
				}
			}
		}

		// Token: 0x1700055B RID: 1371
		// (get) Token: 0x06000F47 RID: 3911 RVA: 0x0002ADE5 File Offset: 0x00028FE5
		// (set) Token: 0x06000F48 RID: 3912 RVA: 0x0002ADED File Offset: 0x00028FED
		[Editor(false)]
		public int TransactionCount
		{
			get
			{
				return this._transactionCount;
			}
			set
			{
				if (this._transactionCount != value)
				{
					this._transactionCount = value;
					base.OnPropertyChanged(value, "TransactionCount");
				}
			}
		}

		// Token: 0x1700055C RID: 1372
		// (get) Token: 0x06000F49 RID: 3913 RVA: 0x0002AE0B File Offset: 0x0002900B
		// (set) Token: 0x06000F4A RID: 3914 RVA: 0x0002AE13 File Offset: 0x00029013
		[Editor(false)]
		public bool IsInWarSet
		{
			get
			{
				return this._isInWarSet;
			}
			set
			{
				if (this._isInWarSet != value)
				{
					this._isInWarSet = value;
					base.OnPropertyChanged(value, "IsInWarSet");
				}
			}
		}

		// Token: 0x1700055D RID: 1373
		// (get) Token: 0x06000F4B RID: 3915 RVA: 0x0002AE31 File Offset: 0x00029031
		// (set) Token: 0x06000F4C RID: 3916 RVA: 0x0002AE39 File Offset: 0x00029039
		[Editor(false)]
		public int TargetEquipmentIndex
		{
			get
			{
				return this._targetEquipmentIndex;
			}
			set
			{
				if (this._targetEquipmentIndex != value)
				{
					this._targetEquipmentIndex = value;
					base.OnPropertyChanged(value, "TargetEquipmentIndex");
				}
			}
		}

		// Token: 0x1700055E RID: 1374
		// (get) Token: 0x06000F4D RID: 3917 RVA: 0x0002AE57 File Offset: 0x00029057
		// (set) Token: 0x06000F4E RID: 3918 RVA: 0x0002AE60 File Offset: 0x00029060
		[Editor(false)]
		public TextWidget OtherInventoryGoldText
		{
			get
			{
				return this._otherInventoryGoldText;
			}
			set
			{
				if (value != this._otherInventoryGoldText)
				{
					if (this._otherInventoryGoldText != null)
					{
						this._otherInventoryGoldText.intPropertyChanged -= this.OtherInventoryGoldTextOnPropertyChanged;
					}
					this._otherInventoryGoldText = value;
					if (this._otherInventoryGoldText != null)
					{
						this._otherInventoryGoldText.intPropertyChanged += this.OtherInventoryGoldTextOnPropertyChanged;
					}
					base.OnPropertyChanged<TextWidget>(value, "OtherInventoryGoldText");
				}
			}
		}

		// Token: 0x1700055F RID: 1375
		// (get) Token: 0x06000F4F RID: 3919 RVA: 0x0002AEC7 File Offset: 0x000290C7
		// (set) Token: 0x06000F50 RID: 3920 RVA: 0x0002AECF File Offset: 0x000290CF
		[Editor(false)]
		public Widget OtherInventoryGoldImage
		{
			get
			{
				return this._otherInventoryGoldImage;
			}
			set
			{
				if (value != this._otherInventoryGoldImage)
				{
					this._otherInventoryGoldImage = value;
					base.OnPropertyChanged<Widget>(value, "OtherInventoryGoldImage");
				}
			}
		}

		// Token: 0x17000560 RID: 1376
		// (get) Token: 0x06000F51 RID: 3921 RVA: 0x0002AEED File Offset: 0x000290ED
		// (set) Token: 0x06000F52 RID: 3922 RVA: 0x0002AEF5 File Offset: 0x000290F5
		[Editor(false)]
		public ScrollablePanel OtherInventoryListWidget
		{
			get
			{
				return this._otherInventoryListWidget;
			}
			set
			{
				if (value != this._otherInventoryListWidget)
				{
					this._otherInventoryListWidget = value;
					base.OnPropertyChanged<ScrollablePanel>(value, "OtherInventoryListWidget");
				}
			}
		}

		// Token: 0x17000561 RID: 1377
		// (get) Token: 0x06000F53 RID: 3923 RVA: 0x0002AF13 File Offset: 0x00029113
		// (set) Token: 0x06000F54 RID: 3924 RVA: 0x0002AF1B File Offset: 0x0002911B
		[Editor(false)]
		public ScrollablePanel PlayerInventoryListWidget
		{
			get
			{
				return this._playerInventoryListWidget;
			}
			set
			{
				if (value != this._playerInventoryListWidget)
				{
					this._playerInventoryListWidget = value;
					base.OnPropertyChanged<ScrollablePanel>(value, "PlayerInventoryListWidget");
				}
			}
		}

		// Token: 0x17000562 RID: 1378
		// (get) Token: 0x06000F55 RID: 3925 RVA: 0x0002AF39 File Offset: 0x00029139
		// (set) Token: 0x06000F56 RID: 3926 RVA: 0x0002AF41 File Offset: 0x00029141
		[Editor(false)]
		public bool IsFocusedOnItemList
		{
			get
			{
				return this._isFocusedOnItemList;
			}
			set
			{
				if (value != this._isFocusedOnItemList)
				{
					this._isFocusedOnItemList = value;
					base.OnPropertyChanged(value, "IsFocusedOnItemList");
				}
			}
		}

		// Token: 0x06000F57 RID: 3927 RVA: 0x0002AF60 File Offset: 0x00029160
		private void OnNewInventoryItemAdded(Widget parentWidget, Widget addedWidget)
		{
			InventoryItemTupleWidget inventoryItemTupleWidget;
			if (this._currentSelectedItemWidget != null && (inventoryItemTupleWidget = addedWidget as InventoryItemTupleWidget) != null)
			{
				this._newAddedItem = inventoryItemTupleWidget;
			}
		}

		// Token: 0x040006E8 RID: 1768
		private readonly int TooltipHideFrameLength = 2;

		// Token: 0x040006E9 RID: 1769
		private InventoryItemButtonWidget _currentSelectedItemWidget;

		// Token: 0x040006EA RID: 1770
		private InventoryItemButtonWidget _currentSelectedOtherItemWidget;

		// Token: 0x040006EB RID: 1771
		private InventoryItemButtonWidget _currentHoveredItemWidget;

		// Token: 0x040006EC RID: 1772
		private InventoryItemButtonWidget _currentDraggedItemWidget;

		// Token: 0x040006ED RID: 1773
		private InventoryItemButtonWidget _lastDisplayedTooltipItem;

		// Token: 0x040006EE RID: 1774
		private int _tooltipHiddenFrameCount;

		// Token: 0x040006EF RID: 1775
		private bool _eventsRegistered;

		// Token: 0x040006F0 RID: 1776
		private InputKeyVisualWidget _previousCharacterInputKeyVisual;

		// Token: 0x040006F1 RID: 1777
		private InputKeyVisualWidget _nextCharacterInputKeyVisual;

		// Token: 0x040006F2 RID: 1778
		private InventoryItemTupleWidget _newAddedItem;

		// Token: 0x040006F3 RID: 1779
		private Widget _previousCharacterInputVisualParent;

		// Token: 0x040006F4 RID: 1780
		private Widget _nextCharacterInputVisualParent;

		// Token: 0x040006F5 RID: 1781
		private InputKeyVisualWidget _transferInputKeyVisualWidget;

		// Token: 0x040006F6 RID: 1782
		private RichTextWidget _tradeLabel;

		// Token: 0x040006F7 RID: 1783
		private Widget _inventoryTooltip;

		// Token: 0x040006F8 RID: 1784
		private InventoryEquippedItemControlsBrushWidget _equippedItemControls;

		// Token: 0x040006F9 RID: 1785
		private InventoryItemPreviewWidget _itemPreviewWidget;

		// Token: 0x040006FA RID: 1786
		private int _transactionCount;

		// Token: 0x040006FB RID: 1787
		private bool _isInWarSet;

		// Token: 0x040006FC RID: 1788
		private int _targetEquipmentIndex;

		// Token: 0x040006FD RID: 1789
		private TextWidget _otherInventoryGoldText;

		// Token: 0x040006FE RID: 1790
		private Widget _otherInventoryGoldImage;

		// Token: 0x040006FF RID: 1791
		private ScrollablePanel _otherInventoryListWidget;

		// Token: 0x04000700 RID: 1792
		private ScrollablePanel _playerInventoryListWidget;

		// Token: 0x04000701 RID: 1793
		private bool _focusLostThisFrame;

		// Token: 0x04000702 RID: 1794
		private bool _isFocusedOnItemList;
	}
}
