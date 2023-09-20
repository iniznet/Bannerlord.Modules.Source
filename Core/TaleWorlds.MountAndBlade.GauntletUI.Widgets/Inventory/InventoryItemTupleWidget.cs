using System;
using System.Collections.Generic;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Inventory
{
	// Token: 0x02000122 RID: 290
	public class InventoryItemTupleWidget : InventoryItemButtonWidget
	{
		// Token: 0x17000535 RID: 1333
		// (get) Token: 0x06000EC7 RID: 3783 RVA: 0x00028FD6 File Offset: 0x000271D6
		// (set) Token: 0x06000EC8 RID: 3784 RVA: 0x00028FDE File Offset: 0x000271DE
		public InventoryImageIdentifierWidget ItemImageIdentifier { get; set; }

		// Token: 0x06000EC9 RID: 3785 RVA: 0x00028FE8 File Offset: 0x000271E8
		public InventoryItemTupleWidget(UIContext context)
			: base(context)
		{
			this._viewClickHandler = new Action<Widget>(this.OnViewClick);
			this._equipClickHandler = new Action<Widget>(this.OnEquipClick);
			this._transferClickHandler = delegate(Widget widget)
			{
				this.OnTransferClick(widget, 1);
			};
			this._sliderTransferClickHandler = delegate(Widget widget)
			{
				this.OnSliderTransferClick(this.TransactionCount);
			};
			base.OverrideDefaultStateSwitchingEnabled = false;
			base.AddState("Selected");
		}

		// Token: 0x06000ECA RID: 3786 RVA: 0x00029064 File Offset: 0x00027264
		private void SetWidgetsState(string state)
		{
			this.SetState(state);
			string currentState = this.ExtendedControlsContainer.CurrentState;
			this.ExtendedControlsContainer.SetState(base.IsSelected ? "Selected" : "Default");
			this.MainContainer.SetState(state);
			this.NameTextWidget.SetState((state == "Pressed") ? state : "Default");
			if (currentState == "Default" && base.IsSelected)
			{
				base.EventFired("Opened", Array.Empty<object>());
				this.Slider.IsExtended = true;
				return;
			}
			if (currentState == "Selected" && !base.IsSelected)
			{
				base.EventFired("Closed", Array.Empty<object>());
				this.Slider.IsExtended = false;
			}
		}

		// Token: 0x06000ECB RID: 3787 RVA: 0x00029134 File Offset: 0x00027334
		private void OnExtendedHiddenUpdate(float dt)
		{
			if (!base.IsSelected)
			{
				this._extendedUpdateTimer += dt;
				if (this._extendedUpdateTimer > 2f)
				{
					this.ExtendedControlsContainer.IsVisible = false;
					return;
				}
				base.EventManager.AddLateUpdateAction(this, new Action<float>(this.OnExtendedHiddenUpdate), 1);
			}
		}

		// Token: 0x06000ECC RID: 3788 RVA: 0x0002918C File Offset: 0x0002738C
		protected override void RefreshState()
		{
			base.RefreshState();
			bool isVisible = this.ExtendedControlsContainer.IsVisible;
			this.ExtendedControlsContainer.IsExtended = base.IsSelected;
			if (base.IsSelected)
			{
				this.ExtendedControlsContainer.IsVisible = true;
			}
			else if (this.ExtendedControlsContainer.IsVisible)
			{
				this._extendedUpdateTimer = 0f;
				base.EventManager.AddLateUpdateAction(this, new Action<float>(this.OnExtendedHiddenUpdate), 1);
			}
			if (base.IsDisabled)
			{
				this.SetWidgetsState("Disabled");
				return;
			}
			if (base.IsPressed)
			{
				this.SetWidgetsState("Pressed");
				return;
			}
			if (base.IsHovered)
			{
				this.SetWidgetsState("Hovered");
				return;
			}
			if (base.IsSelected)
			{
				this.SetWidgetsState("Selected");
				return;
			}
			this.SetWidgetsState("Default");
		}

		// Token: 0x06000ECD RID: 3789 RVA: 0x00029260 File Offset: 0x00027460
		private void UpdateCivilianState()
		{
			if (base.ScreenWidget != null)
			{
				bool flag = !base.ScreenWidget.IsInWarSet && !this.IsCivilian;
				if (!this.CanCharacterUseItem)
				{
					if (!this.MainContainer.Brush.IsCloneRelated(this.CharacterCantUseBrush))
					{
						this.MainContainer.Brush = this.CharacterCantUseBrush;
						this.EquipButton.IsVisible = true;
						this.EquipButton.IsEnabled = false;
						return;
					}
				}
				else if (flag)
				{
					if (!this.MainContainer.Brush.IsCloneRelated(this.CivilianDisabledBrush))
					{
						this.MainContainer.Brush = this.CivilianDisabledBrush;
						this.EquipButton.IsVisible = true;
						this.EquipButton.IsEnabled = false;
						return;
					}
				}
				else if (!this.MainContainer.Brush.IsCloneRelated(this.DefaultBrush))
				{
					this.MainContainer.Brush = this.DefaultBrush;
					this.EquipButton.IsVisible = this.IsEquipable;
					this.EquipButton.IsEnabled = this.IsEquipable;
				}
			}
		}

		// Token: 0x06000ECE RID: 3790 RVA: 0x0002936F File Offset: 0x0002756F
		private void OnViewClick(Widget widget)
		{
			if (base.ScreenWidget != null)
			{
				base.ScreenWidget.ItemPreviewWidget.SetLastFocusedItem(this);
			}
		}

		// Token: 0x06000ECF RID: 3791 RVA: 0x0002938A File Offset: 0x0002758A
		private void OnEquipClick(Widget widget)
		{
			base.EquipItem();
		}

		// Token: 0x06000ED0 RID: 3792 RVA: 0x00029394 File Offset: 0x00027594
		private void OnTransferClick(Widget widget, int count)
		{
			foreach (Action<InventoryItemTupleWidget> action in this.TransferRequestHandlers)
			{
				action(this);
			}
			if (base.IsRightSide)
			{
				this.ProcessBuyItem(true, count);
				return;
			}
			this.ProcessSellItem(true, count);
		}

		// Token: 0x06000ED1 RID: 3793 RVA: 0x00029400 File Offset: 0x00027600
		private void OnSliderTransferClick(int count)
		{
		}

		// Token: 0x06000ED2 RID: 3794 RVA: 0x00029402 File Offset: 0x00027602
		public void ProcessBuyItem(bool playSound, int count = -1)
		{
			if (count == -1)
			{
				count = this.TransactionCount;
			}
			this.TransactionCount = count;
			base.ScreenWidget.TransactionCount = count;
			base.ScreenWidget.TargetEquipmentIndex = -1;
			this.TransferButton.FireClickEvent();
		}

		// Token: 0x06000ED3 RID: 3795 RVA: 0x0002943C File Offset: 0x0002763C
		public void ProcessSellItem(bool playSound, int count = -1)
		{
			if (count == -1)
			{
				count = this.TransactionCount;
			}
			this.TransactionCount = count;
			base.ScreenWidget.TransactionCount = count;
			base.ScreenWidget.TargetEquipmentIndex = -1;
			this.TransferButton.FireClickEvent();
		}

		// Token: 0x06000ED4 RID: 3796 RVA: 0x00029476 File Offset: 0x00027676
		private void ProcessSelectItem()
		{
			if (base.ScreenWidget != null)
			{
				base.IsSelected = true;
				base.ScreenWidget.SetCurrentTuple(this, !base.IsRightSide);
			}
		}

		// Token: 0x06000ED5 RID: 3797 RVA: 0x0002949C File Offset: 0x0002769C
		protected override void OnMouseReleased()
		{
			base.OnMouseReleased();
			this.ProcessSelectItem();
		}

		// Token: 0x06000ED6 RID: 3798 RVA: 0x000294AA File Offset: 0x000276AA
		protected override void OnMouseAlternateReleased()
		{
			base.OnMouseAlternateReleased();
			base.EventFired("OnAlternateRelease", Array.Empty<object>());
		}

		// Token: 0x06000ED7 RID: 3799 RVA: 0x000294C2 File Offset: 0x000276C2
		protected override void OnConnectedToRoot()
		{
			base.OnConnectedToRoot();
			base.ScreenWidget.boolPropertyChanged += this.InventoryScreenWidgetOnPropertyChanged;
		}

		// Token: 0x06000ED8 RID: 3800 RVA: 0x000294E1 File Offset: 0x000276E1
		protected override void OnDisconnectedFromRoot()
		{
			if (base.ScreenWidget != null)
			{
				base.ScreenWidget.boolPropertyChanged -= this.InventoryScreenWidgetOnPropertyChanged;
				base.ScreenWidget.SetCurrentTuple(null, false);
			}
		}

		// Token: 0x06000ED9 RID: 3801 RVA: 0x0002950F File Offset: 0x0002770F
		private void SliderIntPropertyChanged(PropertyOwnerObject owner, string propertyName, int value)
		{
			if (propertyName == "ValueInt")
			{
				this.TransactionCount = this._slider.ValueInt;
			}
		}

		// Token: 0x06000EDA RID: 3802 RVA: 0x00029530 File Offset: 0x00027730
		private void SliderValuePropertyChanged(PropertyOwnerObject owner, string propertyName, object value)
		{
			if (propertyName == "OnMousePressed")
			{
				foreach (Action<InventoryItemTupleWidget> action in this.TransferRequestHandlers)
				{
					action(this);
				}
			}
		}

		// Token: 0x06000EDB RID: 3803 RVA: 0x00029590 File Offset: 0x00027790
		private void CountTextWidgetOnPropertyChanged(PropertyOwnerObject owner, string propertyName, int value)
		{
			if (propertyName == "IntText")
			{
				this.UpdateCountText();
			}
		}

		// Token: 0x06000EDC RID: 3804 RVA: 0x000295A5 File Offset: 0x000277A5
		private void InventoryScreenWidgetOnPropertyChanged(PropertyOwnerObject owner, string propertyName, bool value)
		{
			if (propertyName == "IsInWarSet")
			{
				this.UpdateCivilianState();
			}
		}

		// Token: 0x06000EDD RID: 3805 RVA: 0x000295BA File Offset: 0x000277BA
		private void UpdateCountText()
		{
			if (this.SliderTextWidget != null)
			{
				this.SliderTextWidget.IsHidden = this.CountTextWidget.IsHidden;
			}
		}

		// Token: 0x06000EDE RID: 3806 RVA: 0x000295DC File Offset: 0x000277DC
		private void UpdateCostText()
		{
			if (this.CostTextWidget == null)
			{
				return;
			}
			switch (this.ProfitState)
			{
			case -2:
				this.CostTextWidget.SetState("VeryBad");
				return;
			case -1:
				this.CostTextWidget.SetState("Bad");
				return;
			case 0:
				this.CostTextWidget.SetState("Default");
				return;
			case 1:
				this.CostTextWidget.SetState("Good");
				return;
			case 2:
				this.CostTextWidget.SetState("VeryGood");
				return;
			default:
				return;
			}
		}

		// Token: 0x06000EDF RID: 3807 RVA: 0x0002966B File Offset: 0x0002786B
		private void UpdateDragAvailability()
		{
			base.AcceptDrag = this.ItemCount > 0 && (this.IsTransferable || this.IsEquipable);
		}

		// Token: 0x17000536 RID: 1334
		// (get) Token: 0x06000EE0 RID: 3808 RVA: 0x00029690 File Offset: 0x00027890
		// (set) Token: 0x06000EE1 RID: 3809 RVA: 0x00029698 File Offset: 0x00027898
		[Editor(false)]
		public string ItemID
		{
			get
			{
				return this._itemID;
			}
			set
			{
				if (this._itemID != value)
				{
					this._itemID = value;
					base.OnPropertyChanged<string>(value, "ItemID");
				}
			}
		}

		// Token: 0x17000537 RID: 1335
		// (get) Token: 0x06000EE2 RID: 3810 RVA: 0x000296BB File Offset: 0x000278BB
		// (set) Token: 0x06000EE3 RID: 3811 RVA: 0x000296C3 File Offset: 0x000278C3
		[Editor(false)]
		public TextWidget NameTextWidget
		{
			get
			{
				return this._nameTextWidget;
			}
			set
			{
				if (this._nameTextWidget != value)
				{
					this._nameTextWidget = value;
					base.OnPropertyChanged<TextWidget>(value, "NameTextWidget");
					this.NameTextWidget.AddState("Pressed");
				}
			}
		}

		// Token: 0x17000538 RID: 1336
		// (get) Token: 0x06000EE4 RID: 3812 RVA: 0x000296F1 File Offset: 0x000278F1
		// (set) Token: 0x06000EE5 RID: 3813 RVA: 0x000296FC File Offset: 0x000278FC
		[Editor(false)]
		public TextWidget CountTextWidget
		{
			get
			{
				return this._countTextWidget;
			}
			set
			{
				if (this._countTextWidget != value)
				{
					if (this._countTextWidget != null)
					{
						this._countTextWidget.intPropertyChanged -= this.CountTextWidgetOnPropertyChanged;
					}
					this._countTextWidget = value;
					if (this._countTextWidget != null)
					{
						this._countTextWidget.intPropertyChanged += this.CountTextWidgetOnPropertyChanged;
					}
					base.OnPropertyChanged<TextWidget>(value, "CountTextWidget");
					this.UpdateCountText();
				}
			}
		}

		// Token: 0x17000539 RID: 1337
		// (get) Token: 0x06000EE6 RID: 3814 RVA: 0x00029769 File Offset: 0x00027969
		// (set) Token: 0x06000EE7 RID: 3815 RVA: 0x00029771 File Offset: 0x00027971
		[Editor(false)]
		public TextWidget CostTextWidget
		{
			get
			{
				return this._costTextWidget;
			}
			set
			{
				if (this._costTextWidget != value)
				{
					this._costTextWidget = value;
					this.UpdateCostText();
					base.OnPropertyChanged<TextWidget>(value, "CostTextWidget");
				}
			}
		}

		// Token: 0x1700053A RID: 1338
		// (get) Token: 0x06000EE8 RID: 3816 RVA: 0x00029795 File Offset: 0x00027995
		// (set) Token: 0x06000EE9 RID: 3817 RVA: 0x0002979D File Offset: 0x0002799D
		public int ProfitState
		{
			get
			{
				return this._profitState;
			}
			set
			{
				if (value != this._profitState)
				{
					this._profitState = value;
					this.UpdateCostText();
					base.OnPropertyChanged(value, "ProfitState");
				}
			}
		}

		// Token: 0x1700053B RID: 1339
		// (get) Token: 0x06000EEA RID: 3818 RVA: 0x000297C1 File Offset: 0x000279C1
		// (set) Token: 0x06000EEB RID: 3819 RVA: 0x000297C9 File Offset: 0x000279C9
		[Editor(false)]
		public BrushListPanel MainContainer
		{
			get
			{
				return this._mainContainer;
			}
			set
			{
				if (this._mainContainer != value)
				{
					this._mainContainer = value;
					base.OnPropertyChanged<BrushListPanel>(value, "MainContainer");
				}
			}
		}

		// Token: 0x1700053C RID: 1340
		// (get) Token: 0x06000EEC RID: 3820 RVA: 0x000297E7 File Offset: 0x000279E7
		// (set) Token: 0x06000EED RID: 3821 RVA: 0x000297EF File Offset: 0x000279EF
		[Editor(false)]
		public InventoryTupleExtensionControlsWidget ExtendedControlsContainer
		{
			get
			{
				return this._extendedControlsContainer;
			}
			set
			{
				if (this._extendedControlsContainer != value)
				{
					this._extendedControlsContainer = value;
					base.OnPropertyChanged<InventoryTupleExtensionControlsWidget>(value, "ExtendedControlsContainer");
				}
			}
		}

		// Token: 0x1700053D RID: 1341
		// (get) Token: 0x06000EEE RID: 3822 RVA: 0x0002980D File Offset: 0x00027A0D
		// (set) Token: 0x06000EEF RID: 3823 RVA: 0x00029818 File Offset: 0x00027A18
		[Editor(false)]
		public InventoryTwoWaySliderWidget Slider
		{
			get
			{
				return this._slider;
			}
			set
			{
				if (this._slider != value)
				{
					if (this._slider != null)
					{
						this._slider.intPropertyChanged -= this.SliderIntPropertyChanged;
						this._slider.PropertyChanged -= this.SliderValuePropertyChanged;
					}
					this._slider = value;
					if (this._slider != null)
					{
						this._slider.intPropertyChanged += this.SliderIntPropertyChanged;
						this._slider.PropertyChanged += this.SliderValuePropertyChanged;
					}
					base.OnPropertyChanged<InventoryTwoWaySliderWidget>(value, "Slider");
					this.Slider.AddState("Selected");
					this.Slider.OverrideDefaultStateSwitchingEnabled = true;
				}
			}
		}

		// Token: 0x1700053E RID: 1342
		// (get) Token: 0x06000EF0 RID: 3824 RVA: 0x000298CC File Offset: 0x00027ACC
		// (set) Token: 0x06000EF1 RID: 3825 RVA: 0x000298D4 File Offset: 0x00027AD4
		[Editor(false)]
		public Widget SliderParent
		{
			get
			{
				return this._sliderParent;
			}
			set
			{
				if (this._sliderParent != value)
				{
					this._sliderParent = value;
					base.OnPropertyChanged<Widget>(value, "SliderParent");
					this.SliderParent.AddState("Selected");
				}
			}
		}

		// Token: 0x1700053F RID: 1343
		// (get) Token: 0x06000EF2 RID: 3826 RVA: 0x00029902 File Offset: 0x00027B02
		// (set) Token: 0x06000EF3 RID: 3827 RVA: 0x0002990A File Offset: 0x00027B0A
		[Editor(false)]
		public TextWidget SliderTextWidget
		{
			get
			{
				return this._sliderTextWidget;
			}
			set
			{
				if (this._sliderTextWidget != value)
				{
					this._sliderTextWidget = value;
					base.OnPropertyChanged<TextWidget>(value, "SliderTextWidget");
					this.SliderTextWidget.AddState("Selected");
				}
			}
		}

		// Token: 0x17000540 RID: 1344
		// (get) Token: 0x06000EF4 RID: 3828 RVA: 0x00029938 File Offset: 0x00027B38
		// (set) Token: 0x06000EF5 RID: 3829 RVA: 0x00029940 File Offset: 0x00027B40
		[Editor(false)]
		public bool IsTransferable
		{
			get
			{
				return this._isTransferable;
			}
			set
			{
				if (this._isTransferable != value)
				{
					this._isTransferable = value;
					base.OnPropertyChanged(value, "IsTransferable");
					this.UpdateDragAvailability();
				}
			}
		}

		// Token: 0x17000541 RID: 1345
		// (get) Token: 0x06000EF6 RID: 3830 RVA: 0x00029964 File Offset: 0x00027B64
		// (set) Token: 0x06000EF7 RID: 3831 RVA: 0x0002996C File Offset: 0x00027B6C
		[Editor(false)]
		public ButtonWidget EquipButton
		{
			get
			{
				return this._equipButton;
			}
			set
			{
				if (this._equipButton != value)
				{
					ButtonWidget equipButton = this._equipButton;
					if (equipButton != null)
					{
						equipButton.ClickEventHandlers.Remove(this._equipClickHandler);
					}
					this._equipButton = value;
					ButtonWidget equipButton2 = this._equipButton;
					if (equipButton2 != null)
					{
						equipButton2.ClickEventHandlers.Add(this._equipClickHandler);
					}
					base.OnPropertyChanged<ButtonWidget>(value, "EquipButton");
				}
			}
		}

		// Token: 0x17000542 RID: 1346
		// (get) Token: 0x06000EF8 RID: 3832 RVA: 0x000299CE File Offset: 0x00027BCE
		// (set) Token: 0x06000EF9 RID: 3833 RVA: 0x000299D8 File Offset: 0x00027BD8
		[Editor(false)]
		public ButtonWidget ViewButton
		{
			get
			{
				return this._viewButton;
			}
			set
			{
				if (this._viewButton != value)
				{
					ButtonWidget viewButton = this._viewButton;
					if (viewButton != null)
					{
						viewButton.ClickEventHandlers.Remove(this._viewClickHandler);
					}
					this._viewButton = value;
					ButtonWidget viewButton2 = this._viewButton;
					if (viewButton2 != null)
					{
						viewButton2.ClickEventHandlers.Add(this._viewClickHandler);
					}
					base.OnPropertyChanged<ButtonWidget>(value, "ViewButton");
				}
			}
		}

		// Token: 0x17000543 RID: 1347
		// (get) Token: 0x06000EFA RID: 3834 RVA: 0x00029A3A File Offset: 0x00027C3A
		// (set) Token: 0x06000EFB RID: 3835 RVA: 0x00029A44 File Offset: 0x00027C44
		[Editor(false)]
		public InventoryTransferButtonWidget TransferButton
		{
			get
			{
				return this._transferButton;
			}
			set
			{
				if (this._transferButton != value)
				{
					InventoryTransferButtonWidget transferButton = this._transferButton;
					if (transferButton != null)
					{
						transferButton.ClickEventHandlers.Remove(this._transferClickHandler);
					}
					this._transferButton = value;
					InventoryTransferButtonWidget transferButton2 = this._transferButton;
					if (transferButton2 != null)
					{
						transferButton2.ClickEventHandlers.Add(this._transferClickHandler);
					}
					base.OnPropertyChanged<InventoryTransferButtonWidget>(value, "TransferButton");
				}
			}
		}

		// Token: 0x17000544 RID: 1348
		// (get) Token: 0x06000EFC RID: 3836 RVA: 0x00029AA6 File Offset: 0x00027CA6
		// (set) Token: 0x06000EFD RID: 3837 RVA: 0x00029AB0 File Offset: 0x00027CB0
		[Editor(false)]
		public ButtonWidget SliderTransferButton
		{
			get
			{
				return this._sliderTransferButton;
			}
			set
			{
				if (this._sliderTransferButton != value)
				{
					ButtonWidget sliderTransferButton = this._sliderTransferButton;
					if (sliderTransferButton != null)
					{
						sliderTransferButton.ClickEventHandlers.Remove(this._sliderTransferClickHandler);
					}
					this._sliderTransferButton = value;
					ButtonWidget sliderTransferButton2 = this._sliderTransferButton;
					if (sliderTransferButton2 != null)
					{
						sliderTransferButton2.ClickEventHandlers.Add(this._sliderTransferClickHandler);
					}
					base.OnPropertyChanged<ButtonWidget>(value, "SliderTransferButton");
				}
			}
		}

		// Token: 0x17000545 RID: 1349
		// (get) Token: 0x06000EFE RID: 3838 RVA: 0x00029B12 File Offset: 0x00027D12
		// (set) Token: 0x06000EFF RID: 3839 RVA: 0x00029B1A File Offset: 0x00027D1A
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

		// Token: 0x17000546 RID: 1350
		// (get) Token: 0x06000F00 RID: 3840 RVA: 0x00029B38 File Offset: 0x00027D38
		// (set) Token: 0x06000F01 RID: 3841 RVA: 0x00029B40 File Offset: 0x00027D40
		[Editor(false)]
		public int ItemCount
		{
			get
			{
				return this._itemCount;
			}
			set
			{
				if (this._itemCount != value)
				{
					this._itemCount = value;
					base.OnPropertyChanged(value, "ItemCount");
					this.UpdateDragAvailability();
				}
			}
		}

		// Token: 0x17000547 RID: 1351
		// (get) Token: 0x06000F02 RID: 3842 RVA: 0x00029B64 File Offset: 0x00027D64
		// (set) Token: 0x06000F03 RID: 3843 RVA: 0x00029B6C File Offset: 0x00027D6C
		[Editor(false)]
		public bool IsCivilian
		{
			get
			{
				return this._isCivilian;
			}
			set
			{
				if (this._isCivilian != value || !this._isCivilianStateSet)
				{
					this._isCivilian = value;
					base.OnPropertyChanged(value, "IsCivilian");
					this._isCivilianStateSet = true;
					this.UpdateCivilianState();
				}
			}
		}

		// Token: 0x17000548 RID: 1352
		// (get) Token: 0x06000F04 RID: 3844 RVA: 0x00029B9F File Offset: 0x00027D9F
		// (set) Token: 0x06000F05 RID: 3845 RVA: 0x00029BA7 File Offset: 0x00027DA7
		[Editor(false)]
		public bool IsGenderDifferent
		{
			get
			{
				return this._isGenderDifferent;
			}
			set
			{
				if (this._isGenderDifferent != value)
				{
					this._isGenderDifferent = value;
					base.OnPropertyChanged(value, "IsGenderDifferent");
					this.UpdateCivilianState();
				}
			}
		}

		// Token: 0x17000549 RID: 1353
		// (get) Token: 0x06000F06 RID: 3846 RVA: 0x00029BCB File Offset: 0x00027DCB
		// (set) Token: 0x06000F07 RID: 3847 RVA: 0x00029BD3 File Offset: 0x00027DD3
		[Editor(false)]
		public bool IsEquipable
		{
			get
			{
				return this._isEquipable;
			}
			set
			{
				if (this._isEquipable != value)
				{
					this._isEquipable = value;
					base.OnPropertyChanged(value, "IsEquipable");
					this.UpdateDragAvailability();
				}
			}
		}

		// Token: 0x1700054A RID: 1354
		// (get) Token: 0x06000F08 RID: 3848 RVA: 0x00029BF7 File Offset: 0x00027DF7
		// (set) Token: 0x06000F09 RID: 3849 RVA: 0x00029BFF File Offset: 0x00027DFF
		[Editor(false)]
		public bool IsNewlyAdded
		{
			get
			{
				return this._isNewlyAdded;
			}
			set
			{
				if (this._isNewlyAdded != value)
				{
					this._isNewlyAdded = value;
					base.OnPropertyChanged(value, "IsNewlyAdded");
					this.ItemImageIdentifier.SetRenderRequestedPreviousFrame(value);
				}
			}
		}

		// Token: 0x1700054B RID: 1355
		// (get) Token: 0x06000F0A RID: 3850 RVA: 0x00029C29 File Offset: 0x00027E29
		// (set) Token: 0x06000F0B RID: 3851 RVA: 0x00029C31 File Offset: 0x00027E31
		[Editor(false)]
		public bool CanCharacterUseItem
		{
			get
			{
				return this._canCharacterUseItem;
			}
			set
			{
				if (this._canCharacterUseItem != value)
				{
					this._canCharacterUseItem = value;
					base.OnPropertyChanged(value, "CanCharacterUseItem");
					this.UpdateCivilianState();
				}
			}
		}

		// Token: 0x1700054C RID: 1356
		// (get) Token: 0x06000F0C RID: 3852 RVA: 0x00029C55 File Offset: 0x00027E55
		// (set) Token: 0x06000F0D RID: 3853 RVA: 0x00029C5D File Offset: 0x00027E5D
		[Editor(false)]
		public Brush DefaultBrush
		{
			get
			{
				return this._defaultBrush;
			}
			set
			{
				if (this._defaultBrush != value)
				{
					this._defaultBrush = value;
					base.OnPropertyChanged<Brush>(value, "DefaultBrush");
				}
			}
		}

		// Token: 0x1700054D RID: 1357
		// (get) Token: 0x06000F0E RID: 3854 RVA: 0x00029C7B File Offset: 0x00027E7B
		// (set) Token: 0x06000F0F RID: 3855 RVA: 0x00029C83 File Offset: 0x00027E83
		[Editor(false)]
		public Brush CivilianDisabledBrush
		{
			get
			{
				return this._civilianDisabledBrush;
			}
			set
			{
				if (this._civilianDisabledBrush != value)
				{
					this._civilianDisabledBrush = value;
					base.OnPropertyChanged<Brush>(value, "CivilianDisabledBrush");
				}
			}
		}

		// Token: 0x1700054E RID: 1358
		// (get) Token: 0x06000F10 RID: 3856 RVA: 0x00029CA1 File Offset: 0x00027EA1
		// (set) Token: 0x06000F11 RID: 3857 RVA: 0x00029CA9 File Offset: 0x00027EA9
		[Editor(false)]
		public Brush CharacterCantUseBrush
		{
			get
			{
				return this._characterCantUseBrush;
			}
			set
			{
				if (this._characterCantUseBrush != value)
				{
					this._characterCantUseBrush = value;
					base.OnPropertyChanged<Brush>(value, "CharacterCantUseBrush");
				}
			}
		}

		// Token: 0x040006BE RID: 1726
		private readonly Action<Widget> _viewClickHandler;

		// Token: 0x040006BF RID: 1727
		private readonly Action<Widget> _equipClickHandler;

		// Token: 0x040006C0 RID: 1728
		private readonly Action<Widget> _transferClickHandler;

		// Token: 0x040006C1 RID: 1729
		private readonly Action<Widget> _sliderTransferClickHandler;

		// Token: 0x040006C2 RID: 1730
		public List<Action<InventoryItemTupleWidget>> TransferRequestHandlers = new List<Action<InventoryItemTupleWidget>>();

		// Token: 0x040006C3 RID: 1731
		private bool _isCivilianStateSet;

		// Token: 0x040006C4 RID: 1732
		private float _extendedUpdateTimer;

		// Token: 0x040006C5 RID: 1733
		private TextWidget _nameTextWidget;

		// Token: 0x040006C6 RID: 1734
		private TextWidget _countTextWidget;

		// Token: 0x040006C7 RID: 1735
		private TextWidget _costTextWidget;

		// Token: 0x040006C8 RID: 1736
		private int _profitState;

		// Token: 0x040006C9 RID: 1737
		private BrushListPanel _mainContainer;

		// Token: 0x040006CA RID: 1738
		private InventoryTupleExtensionControlsWidget _extendedControlsContainer;

		// Token: 0x040006CB RID: 1739
		private InventoryTwoWaySliderWidget _slider;

		// Token: 0x040006CC RID: 1740
		private Widget _sliderParent;

		// Token: 0x040006CD RID: 1741
		private TextWidget _sliderTextWidget;

		// Token: 0x040006CE RID: 1742
		private bool _isTransferable;

		// Token: 0x040006CF RID: 1743
		private ButtonWidget _equipButton;

		// Token: 0x040006D0 RID: 1744
		private ButtonWidget _viewButton;

		// Token: 0x040006D1 RID: 1745
		private InventoryTransferButtonWidget _transferButton;

		// Token: 0x040006D2 RID: 1746
		private ButtonWidget _sliderTransferButton;

		// Token: 0x040006D3 RID: 1747
		private int _transactionCount;

		// Token: 0x040006D4 RID: 1748
		private int _itemCount;

		// Token: 0x040006D5 RID: 1749
		private bool _isCivilian;

		// Token: 0x040006D6 RID: 1750
		private bool _isGenderDifferent;

		// Token: 0x040006D7 RID: 1751
		private bool _isEquipable;

		// Token: 0x040006D8 RID: 1752
		private bool _canCharacterUseItem;

		// Token: 0x040006D9 RID: 1753
		private bool _isNewlyAdded;

		// Token: 0x040006DA RID: 1754
		private Brush _defaultBrush;

		// Token: 0x040006DB RID: 1755
		private Brush _civilianDisabledBrush;

		// Token: 0x040006DC RID: 1756
		private Brush _characterCantUseBrush;

		// Token: 0x040006DD RID: 1757
		private string _itemID;
	}
}
