using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Inventory
{
	public class InventoryEquippedItemControlsBrushWidget : BrushWidget
	{
		public event InventoryEquippedItemControlsBrushWidget.ButtonClickEventHandler OnPreviewClick;

		public event InventoryEquippedItemControlsBrushWidget.ButtonClickEventHandler OnUnequipClick;

		public event InventoryEquippedItemControlsBrushWidget.ButtonClickEventHandler OnSellClick;

		public event Action OnHidePanel;

		public NavigationForcedScopeCollectionTargeter ForcedScopeCollection { get; set; }

		public NavigationScopeTargeter NavigationScope { get; set; }

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

		protected override void OnUpdate(float dt)
		{
			base.OnUpdate(dt);
			InventoryItemButtonWidget itemWidget = this._itemWidget;
		}

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

		private InventoryItemButtonWidget _itemWidget;

		private Action<Widget> _previewClickHandler;

		private Action<Widget> _unequipClickHandler;

		private Action<Widget> _sellClickHandler;

		private float _lastTransitionStartTime;

		private bool _isScopeDirty;

		private bool _isControlsEnabled;

		private InventoryScreenWidget _screenWidget;

		private ButtonWidget _previewButton;

		private ButtonWidget _unequipButton;

		private ButtonWidget _sellButton;

		public delegate void ButtonClickEventHandler(Widget itemWidget);
	}
}
