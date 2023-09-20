using System;
using System.Collections.Generic;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Inventory
{
	public class InventoryItemTupleWidget : InventoryItemButtonWidget
	{
		public InventoryImageIdentifierWidget ItemImageIdentifier { get; set; }

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

		private void OnViewClick(Widget widget)
		{
			if (base.ScreenWidget != null)
			{
				base.ScreenWidget.ItemPreviewWidget.SetLastFocusedItem(this);
			}
		}

		private void OnEquipClick(Widget widget)
		{
			base.EquipItem();
		}

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

		private void OnSliderTransferClick(int count)
		{
		}

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

		private void ProcessSelectItem()
		{
			if (base.ScreenWidget != null)
			{
				base.IsSelected = true;
				base.ScreenWidget.SetCurrentTuple(this, !base.IsRightSide);
			}
		}

		protected override void OnMouseReleased()
		{
			base.OnMouseReleased();
			this.ProcessSelectItem();
		}

		protected override void OnMouseAlternateReleased()
		{
			base.OnMouseAlternateReleased();
			base.EventFired("OnAlternateRelease", Array.Empty<object>());
		}

		protected override void OnConnectedToRoot()
		{
			base.OnConnectedToRoot();
			base.ScreenWidget.boolPropertyChanged += this.InventoryScreenWidgetOnPropertyChanged;
		}

		protected override void OnDisconnectedFromRoot()
		{
			if (base.ScreenWidget != null)
			{
				base.ScreenWidget.boolPropertyChanged -= this.InventoryScreenWidgetOnPropertyChanged;
				base.ScreenWidget.SetCurrentTuple(null, false);
			}
		}

		private void SliderIntPropertyChanged(PropertyOwnerObject owner, string propertyName, int value)
		{
			if (propertyName == "ValueInt")
			{
				this.TransactionCount = this._slider.ValueInt;
			}
		}

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

		private void CountTextWidgetOnPropertyChanged(PropertyOwnerObject owner, string propertyName, int value)
		{
			if (propertyName == "IntText")
			{
				this.UpdateCountText();
			}
		}

		private void InventoryScreenWidgetOnPropertyChanged(PropertyOwnerObject owner, string propertyName, bool value)
		{
			if (propertyName == "IsInWarSet")
			{
				this.UpdateCivilianState();
			}
		}

		private void UpdateCountText()
		{
			if (this.SliderTextWidget != null)
			{
				this.SliderTextWidget.IsHidden = this.CountTextWidget.IsHidden;
			}
		}

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

		private void UpdateDragAvailability()
		{
			base.AcceptDrag = this.ItemCount > 0 && (this.IsTransferable || this.IsEquipable);
		}

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

		private readonly Action<Widget> _viewClickHandler;

		private readonly Action<Widget> _equipClickHandler;

		private readonly Action<Widget> _transferClickHandler;

		private readonly Action<Widget> _sliderTransferClickHandler;

		public List<Action<InventoryItemTupleWidget>> TransferRequestHandlers = new List<Action<InventoryItemTupleWidget>>();

		private bool _isCivilianStateSet;

		private float _extendedUpdateTimer;

		private TextWidget _nameTextWidget;

		private TextWidget _countTextWidget;

		private TextWidget _costTextWidget;

		private int _profitState;

		private BrushListPanel _mainContainer;

		private InventoryTupleExtensionControlsWidget _extendedControlsContainer;

		private InventoryTwoWaySliderWidget _slider;

		private Widget _sliderParent;

		private TextWidget _sliderTextWidget;

		private bool _isTransferable;

		private ButtonWidget _equipButton;

		private ButtonWidget _viewButton;

		private InventoryTransferButtonWidget _transferButton;

		private ButtonWidget _sliderTransferButton;

		private int _transactionCount;

		private int _itemCount;

		private bool _isCivilian;

		private bool _isGenderDifferent;

		private bool _isEquipable;

		private bool _canCharacterUseItem;

		private bool _isNewlyAdded;

		private Brush _defaultBrush;

		private Brush _civilianDisabledBrush;

		private Brush _characterCantUseBrush;

		private string _itemID;
	}
}
