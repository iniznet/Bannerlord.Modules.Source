using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Inventory
{
	// Token: 0x0200011C RID: 284
	public class InventoryEquippedItemControlsBrushWidget : BrushWidget
	{
		// Token: 0x14000006 RID: 6
		// (add) Token: 0x06000E68 RID: 3688 RVA: 0x00027E88 File Offset: 0x00026088
		// (remove) Token: 0x06000E69 RID: 3689 RVA: 0x00027EC0 File Offset: 0x000260C0
		public event InventoryEquippedItemControlsBrushWidget.ButtonClickEventHandler OnPreviewClick;

		// Token: 0x14000007 RID: 7
		// (add) Token: 0x06000E6A RID: 3690 RVA: 0x00027EF8 File Offset: 0x000260F8
		// (remove) Token: 0x06000E6B RID: 3691 RVA: 0x00027F30 File Offset: 0x00026130
		public event InventoryEquippedItemControlsBrushWidget.ButtonClickEventHandler OnUnequipClick;

		// Token: 0x14000008 RID: 8
		// (add) Token: 0x06000E6C RID: 3692 RVA: 0x00027F68 File Offset: 0x00026168
		// (remove) Token: 0x06000E6D RID: 3693 RVA: 0x00027FA0 File Offset: 0x000261A0
		public event InventoryEquippedItemControlsBrushWidget.ButtonClickEventHandler OnSellClick;

		// Token: 0x14000009 RID: 9
		// (add) Token: 0x06000E6E RID: 3694 RVA: 0x00027FD8 File Offset: 0x000261D8
		// (remove) Token: 0x06000E6F RID: 3695 RVA: 0x00028010 File Offset: 0x00026210
		public event Action OnHidePanel;

		// Token: 0x1700051C RID: 1308
		// (get) Token: 0x06000E70 RID: 3696 RVA: 0x00028045 File Offset: 0x00026245
		// (set) Token: 0x06000E71 RID: 3697 RVA: 0x0002804D File Offset: 0x0002624D
		public NavigationForcedScopeCollectionTargeter ForcedScopeCollection { get; set; }

		// Token: 0x1700051D RID: 1309
		// (get) Token: 0x06000E72 RID: 3698 RVA: 0x00028056 File Offset: 0x00026256
		// (set) Token: 0x06000E73 RID: 3699 RVA: 0x0002805E File Offset: 0x0002625E
		public NavigationScopeTargeter NavigationScope { get; set; }

		// Token: 0x06000E74 RID: 3700 RVA: 0x00028068 File Offset: 0x00026268
		public InventoryEquippedItemControlsBrushWidget(UIContext context)
			: base(context)
		{
			this._previewClickHandler = new Action<Widget>(this.PreviewClicked);
			this._unequipClickHandler = new Action<Widget>(this.UnequipClicked);
			this._sellClickHandler = new Action<Widget>(this.SellClicked);
			base.AddState("LeftHidden");
			base.AddState("LeftVisible");
			base.AddState("RightHidden");
			base.AddState("RightVisible");
		}

		// Token: 0x06000E75 RID: 3701 RVA: 0x000280E0 File Offset: 0x000262E0
		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (this.ScreenWidget == null)
			{
				Widget widget = this;
				while (widget != base.EventManager.Root && this.ScreenWidget == null)
				{
					if (widget is InventoryScreenWidget)
					{
						this.ScreenWidget = (InventoryScreenWidget)widget;
					}
					else
					{
						widget = widget.ParentWidget;
					}
				}
			}
			if (this._isScopeDirty && base.EventManager.Time - this._lastTransitionStartTime > base.VisualDefinition.TransitionDuration)
			{
				this.ForcedScopeCollection.IsCollectionDisabled = base.CurrentState == "RightHidden" || base.CurrentState == "LeftHidden";
				this.NavigationScope.IsScopeDisabled = this.ForcedScopeCollection.IsCollectionDisabled;
				this._isScopeDirty = false;
			}
			if (!this.IsControlsEnabled && this._itemWidget != null)
			{
				this.HidePanel();
			}
		}

		// Token: 0x06000E76 RID: 3702 RVA: 0x000281C0 File Offset: 0x000263C0
		public void ShowPanel(InventoryItemButtonWidget itemWidget)
		{
			if (itemWidget.IsRightSide)
			{
				base.HorizontalAlignment = HorizontalAlignment.Right;
				base.Brush.HorizontalFlip = false;
				this.SetState("RightHidden");
				base.PositionXOffset = base.VisualDefinition.VisualStates["RightHidden"].PositionXOffset;
				this.SetState("RightVisible");
			}
			else
			{
				base.HorizontalAlignment = HorizontalAlignment.Left;
				base.Brush.HorizontalFlip = true;
				this.SetState("LeftHidden");
				base.PositionXOffset = base.VisualDefinition.VisualStates["LeftHidden"].PositionXOffset;
				this.SetState("LeftVisible");
			}
			base.ScaledPositionYOffset = itemWidget.GlobalPosition.Y + itemWidget.Size.Y - 10f * base._scaleToUse - base.EventManager.TopUsableAreaStart;
			base.IsVisible = true;
			this._itemWidget = itemWidget;
			this._isScopeDirty = true;
			this._lastTransitionStartTime = base.Context.EventManager.Time;
			this.IsControlsEnabled = true;
		}

		// Token: 0x06000E77 RID: 3703 RVA: 0x000282D4 File Offset: 0x000264D4
		public void HidePanel()
		{
			if (!base.IsVisible || this._itemWidget == null)
			{
				return;
			}
			if (this._itemWidget.IsRightSide)
			{
				this.SetState("RightHidden");
			}
			else
			{
				this.SetState("LeftHidden");
			}
			this._itemWidget = null;
			Action onHidePanel = this.OnHidePanel;
			if (onHidePanel != null)
			{
				onHidePanel();
			}
			this._isScopeDirty = true;
			this._lastTransitionStartTime = base.Context.EventManager.Time;
			this.IsControlsEnabled = false;
		}

		// Token: 0x06000E78 RID: 3704 RVA: 0x00028353 File Offset: 0x00026553
		private void PreviewClicked(Widget widget)
		{
			if (this._itemWidget == null)
			{
				return;
			}
			InventoryEquippedItemControlsBrushWidget.ButtonClickEventHandler onPreviewClick = this.OnPreviewClick;
			if (onPreviewClick != null)
			{
				onPreviewClick(this._itemWidget);
			}
			this._itemWidget.PreviewItem();
		}

		// Token: 0x06000E79 RID: 3705 RVA: 0x00028380 File Offset: 0x00026580
		private void UnequipClicked(Widget widget)
		{
			if (this._itemWidget == null)
			{
				return;
			}
			InventoryEquippedItemControlsBrushWidget.ButtonClickEventHandler onUnequipClick = this.OnUnequipClick;
			if (onUnequipClick != null)
			{
				onUnequipClick(this._itemWidget);
			}
			this._itemWidget.UnequipItem();
			this.HidePanel();
		}

		// Token: 0x06000E7A RID: 3706 RVA: 0x000283B3 File Offset: 0x000265B3
		private void SellClicked(Widget widget)
		{
			if (this._itemWidget == null)
			{
				return;
			}
			InventoryEquippedItemControlsBrushWidget.ButtonClickEventHandler onSellClick = this.OnSellClick;
			if (onSellClick != null)
			{
				onSellClick(this._itemWidget);
			}
			this.ScreenWidget.TransactionCount = 1;
			this._itemWidget.SellItem();
			this.HidePanel();
		}

		// Token: 0x06000E7B RID: 3707 RVA: 0x000283F2 File Offset: 0x000265F2
		protected override void OnUpdate(float dt)
		{
			base.OnUpdate(dt);
			InventoryItemButtonWidget itemWidget = this._itemWidget;
		}

		// Token: 0x1700051E RID: 1310
		// (get) Token: 0x06000E7C RID: 3708 RVA: 0x00028402 File Offset: 0x00026602
		// (set) Token: 0x06000E7D RID: 3709 RVA: 0x0002840A File Offset: 0x0002660A
		public bool IsControlsEnabled
		{
			get
			{
				return this._isControlsEnabled;
			}
			set
			{
				if (value != this._isControlsEnabled)
				{
					this._isControlsEnabled = value;
					base.OnPropertyChanged(value, "IsControlsEnabled");
				}
			}
		}

		// Token: 0x1700051F RID: 1311
		// (get) Token: 0x06000E7E RID: 3710 RVA: 0x00028428 File Offset: 0x00026628
		// (set) Token: 0x06000E7F RID: 3711 RVA: 0x00028430 File Offset: 0x00026630
		[Editor(false)]
		public InventoryScreenWidget ScreenWidget
		{
			get
			{
				return this._screenWidget;
			}
			set
			{
				if (this._screenWidget != value)
				{
					this._screenWidget = value;
					base.OnPropertyChanged<InventoryScreenWidget>(value, "ScreenWidget");
				}
			}
		}

		// Token: 0x17000520 RID: 1312
		// (get) Token: 0x06000E80 RID: 3712 RVA: 0x0002844E File Offset: 0x0002664E
		// (set) Token: 0x06000E81 RID: 3713 RVA: 0x00028458 File Offset: 0x00026658
		[Editor(false)]
		public ButtonWidget PreviewButton
		{
			get
			{
				return this._previewButton;
			}
			set
			{
				if (this._previewButton != value)
				{
					ButtonWidget previewButton = this._previewButton;
					if (previewButton != null)
					{
						previewButton.ClickEventHandlers.Remove(this._previewClickHandler);
					}
					this._previewButton = value;
					ButtonWidget previewButton2 = this._previewButton;
					if (previewButton2 != null)
					{
						previewButton2.ClickEventHandlers.Add(this._previewClickHandler);
					}
					base.OnPropertyChanged<ButtonWidget>(value, "PreviewButton");
				}
			}
		}

		// Token: 0x17000521 RID: 1313
		// (get) Token: 0x06000E82 RID: 3714 RVA: 0x000284BA File Offset: 0x000266BA
		// (set) Token: 0x06000E83 RID: 3715 RVA: 0x000284C4 File Offset: 0x000266C4
		[Editor(false)]
		public ButtonWidget UnequipButton
		{
			get
			{
				return this._unequipButton;
			}
			set
			{
				if (this._unequipButton != value)
				{
					ButtonWidget unequipButton = this._unequipButton;
					if (unequipButton != null)
					{
						unequipButton.ClickEventHandlers.Remove(this._unequipClickHandler);
					}
					this._unequipButton = value;
					ButtonWidget unequipButton2 = this._unequipButton;
					if (unequipButton2 != null)
					{
						unequipButton2.ClickEventHandlers.Add(this._unequipClickHandler);
					}
					base.OnPropertyChanged<ButtonWidget>(value, "UnequipButton");
				}
			}
		}

		// Token: 0x17000522 RID: 1314
		// (get) Token: 0x06000E84 RID: 3716 RVA: 0x00028526 File Offset: 0x00026726
		// (set) Token: 0x06000E85 RID: 3717 RVA: 0x00028530 File Offset: 0x00026730
		[Editor(false)]
		public ButtonWidget SellButton
		{
			get
			{
				return this._sellButton;
			}
			set
			{
				if (this._sellButton != value)
				{
					ButtonWidget sellButton = this._sellButton;
					if (sellButton != null)
					{
						sellButton.ClickEventHandlers.Remove(this._sellClickHandler);
					}
					this._sellButton = value;
					ButtonWidget sellButton2 = this._sellButton;
					if (sellButton2 != null)
					{
						sellButton2.ClickEventHandlers.Add(this._sellClickHandler);
					}
					base.OnPropertyChanged<ButtonWidget>(value, "SellButton");
				}
			}
		}

		// Token: 0x0400069C RID: 1692
		private InventoryItemButtonWidget _itemWidget;

		// Token: 0x0400069D RID: 1693
		private Action<Widget> _previewClickHandler;

		// Token: 0x0400069E RID: 1694
		private Action<Widget> _unequipClickHandler;

		// Token: 0x0400069F RID: 1695
		private Action<Widget> _sellClickHandler;

		// Token: 0x040006A0 RID: 1696
		private float _lastTransitionStartTime;

		// Token: 0x040006A1 RID: 1697
		private bool _isScopeDirty;

		// Token: 0x040006A4 RID: 1700
		private bool _isControlsEnabled;

		// Token: 0x040006A5 RID: 1701
		private InventoryScreenWidget _screenWidget;

		// Token: 0x040006A6 RID: 1702
		private ButtonWidget _previewButton;

		// Token: 0x040006A7 RID: 1703
		private ButtonWidget _unequipButton;

		// Token: 0x040006A8 RID: 1704
		private ButtonWidget _sellButton;

		// Token: 0x02000199 RID: 409
		// (Invoke) Token: 0x0600131D RID: 4893
		public delegate void ButtonClickEventHandler(Widget itemWidget);
	}
}
