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
	public class InventoryScreenWidget : Widget
	{
		public InventoryScreenWidget(UIContext context)
			: base(context)
		{
		}

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

		private Widget GetFirstBannerItem()
		{
			ListPanel listPanel = this.OtherInventoryListWidget.InnerPanel as ListPanel;
			ListPanel listPanel2 = ((listPanel != null) ? listPanel.GetChild(0) : null) as ListPanel;
			if (listPanel2 == null)
			{
				return null;
			}
			return listPanel2.FindChild((Widget x) => (x as InventoryItemTupleWidget).ItemType == this.BannerTypeCode);
		}

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
			if (this._scrollToBannersInFrames > -1)
			{
				if (this._scrollToBannersInFrames == 0)
				{
					this.OtherInventoryListWidget.ScrollToChild(this.GetFirstBannerItem(), -1f, 0.2f, 0, 0, 0.35f, 0f);
				}
				this._scrollToBannersInFrames--;
			}
			if (this._focusLostThisFrame)
			{
				base.EventFired("OnFocusLose", Array.Empty<object>());
				this._focusLostThisFrame = false;
			}
			this.UpdateTooltipPosition();
		}

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

		private void OnTransferItemRequested(InventoryItemTupleWidget owner)
		{
			this.UpdateScrollTarget(!owner.IsRightSide);
		}

		private void TradeLabelOnPropertyChanged(PropertyOwnerObject owner, string propertyName, object value)
		{
			if (propertyName == "Text")
			{
				this.TradeLabel.IsDisabled = string.IsNullOrEmpty(this.TradeLabel.Text);
			}
		}

		private void EquippedItemControlsOnPreviewClick(Widget itemwidget)
		{
			this.ItemPreviewWidget.SetLastFocusedItem(null);
		}

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

		private void ItemWidgetHoverEnd(InventoryItemButtonWidget itemWidget)
		{
			if (this._currentHoveredItemWidget != null && itemWidget == null)
			{
				this._currentHoveredItemWidget = null;
				this.InventoryTooltip.IsHidden = true;
				base.EventFired("ItemHoverEnd", Array.Empty<object>());
			}
		}

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

		public void ItemWidgetDrop(InventoryItemButtonWidget itemWidget)
		{
			if (this._currentDraggedItemWidget == itemWidget)
			{
				this._currentDraggedItemWidget = null;
				this.TargetEquipmentIndex = -1;
			}
		}

		private void OtherInventoryGoldTextOnPropertyChanged(PropertyOwnerObject owner, string propertyName, int value)
		{
			if (propertyName == "IntText")
			{
				bool flag = this.OtherInventoryGoldText.IntText > 0;
				this.OtherInventoryGoldText.IsVisible = flag;
				this.OtherInventoryGoldImage.IsVisible = flag;
			}
		}

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

		[Editor(false)]
		public bool IsBannerTutorialActive
		{
			get
			{
				return this._isBannerTutorialActive;
			}
			set
			{
				if (value != this._isBannerTutorialActive)
				{
					this._isBannerTutorialActive = value;
					base.OnPropertyChanged(value, "IsBannerTutorialActive");
					if (value)
					{
						this._scrollToBannersInFrames = 1;
					}
				}
			}
		}

		[Editor(false)]
		public int BannerTypeCode
		{
			get
			{
				return this._bannerTypeCode;
			}
			set
			{
				if (value != this._bannerTypeCode)
				{
					this._bannerTypeCode = value;
					base.OnPropertyChanged(value, "BannerTypeCode");
				}
			}
		}

		private void OnNewInventoryItemAdded(Widget parentWidget, Widget addedWidget)
		{
			InventoryItemTupleWidget inventoryItemTupleWidget;
			if (this._currentSelectedItemWidget != null && (inventoryItemTupleWidget = addedWidget as InventoryItemTupleWidget) != null)
			{
				this._newAddedItem = inventoryItemTupleWidget;
			}
		}

		private readonly int TooltipHideFrameLength = 2;

		private InventoryItemButtonWidget _currentSelectedItemWidget;

		private InventoryItemButtonWidget _currentSelectedOtherItemWidget;

		private InventoryItemButtonWidget _currentHoveredItemWidget;

		private InventoryItemButtonWidget _currentDraggedItemWidget;

		private InventoryItemButtonWidget _lastDisplayedTooltipItem;

		private int _tooltipHiddenFrameCount;

		private bool _eventsRegistered;

		private int _scrollToBannersInFrames = -1;

		private InputKeyVisualWidget _previousCharacterInputKeyVisual;

		private InputKeyVisualWidget _nextCharacterInputKeyVisual;

		private InventoryItemTupleWidget _newAddedItem;

		private Widget _previousCharacterInputVisualParent;

		private Widget _nextCharacterInputVisualParent;

		private InputKeyVisualWidget _transferInputKeyVisualWidget;

		private RichTextWidget _tradeLabel;

		private Widget _inventoryTooltip;

		private InventoryEquippedItemControlsBrushWidget _equippedItemControls;

		private InventoryItemPreviewWidget _itemPreviewWidget;

		private int _transactionCount;

		private bool _isInWarSet;

		private int _targetEquipmentIndex;

		private TextWidget _otherInventoryGoldText;

		private Widget _otherInventoryGoldImage;

		private ScrollablePanel _otherInventoryListWidget;

		private ScrollablePanel _playerInventoryListWidget;

		private bool _focusLostThisFrame;

		private bool _isFocusedOnItemList;

		private bool _isBannerTutorialActive;

		private int _bannerTypeCode;
	}
}
