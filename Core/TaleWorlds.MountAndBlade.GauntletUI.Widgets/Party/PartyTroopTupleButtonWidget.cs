using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.MountAndBlade.GauntletUI.Widgets.Inventory;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Party
{
	public class PartyTroopTupleButtonWidget : ButtonWidget
	{
		public string CharacterID { get; set; }

		public PartyTroopTupleButtonWidget(UIContext context)
			: base(context)
		{
			base.OverrideDefaultStateSwitchingEnabled = true;
			base.AddState("Selected");
		}

		private void SetWidgetsState(string state)
		{
			this.SetState(state);
			string currentState = this._extendedControlsContainer.CurrentState;
			this._extendedControlsContainer.SetState(base.IsSelected ? "Selected" : "Default");
			this._main.SetState(state);
			if (currentState == "Default" && base.IsSelected)
			{
				base.EventFired("Opened", Array.Empty<object>());
				this.TransferSlider.IsExtended = true;
				this._extendedControlsContainer.IsExtended = true;
				return;
			}
			if (currentState == "Selected" && !base.IsSelected)
			{
				base.EventFired("Closed", Array.Empty<object>());
				this.TransferSlider.IsExtended = false;
				this._extendedControlsContainer.IsExtended = false;
			}
		}

		protected override void OnDisconnectedFromRoot()
		{
			base.OnDisconnectedFromRoot();
			if (this.ScreenWidget.CurrentMainTuple == this)
			{
				this.ScreenWidget.SetCurrentTuple(null, false);
			}
		}

		protected override void RefreshState()
		{
			base.RefreshState();
			this._extendedControlsContainer.IsEnabled = base.IsSelected;
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

		private void AssignScreenWidget()
		{
			Widget widget = this;
			while (widget != base.EventManager.Root && this._screenWidget == null)
			{
				PartyScreenWidget partyScreenWidget;
				if ((partyScreenWidget = widget as PartyScreenWidget) != null)
				{
					this._screenWidget = partyScreenWidget;
				}
				else
				{
					widget = widget.ParentWidget;
				}
			}
		}

		protected override void OnMouseReleased()
		{
			base.OnMouseReleased();
			PartyScreenWidget screenWidget = this.ScreenWidget;
			if (screenWidget == null)
			{
				return;
			}
			screenWidget.SetCurrentTuple(this, this.IsTupleLeftSide);
		}

		public void ResetIsSelected()
		{
			base.IsSelected = false;
		}

		private void OnValueChanged(PropertyOwnerObject arg1, string arg2, int arg3)
		{
			if (arg2 == "ValueInt")
			{
				base.AcceptDrag = arg3 > 0;
			}
		}

		public PartyScreenWidget ScreenWidget
		{
			get
			{
				if (this._screenWidget == null)
				{
					this.AssignScreenWidget();
				}
				return this._screenWidget;
			}
		}

		[Editor(false)]
		public bool IsTupleLeftSide
		{
			get
			{
				return this._isTupleLeftSide;
			}
			set
			{
				if (this._isTupleLeftSide != value)
				{
					this._isTupleLeftSide = value;
					base.OnPropertyChanged(value, "IsTupleLeftSide");
				}
			}
		}

		[Editor(false)]
		public InventoryTwoWaySliderWidget TransferSlider
		{
			get
			{
				return this._transferSlider;
			}
			set
			{
				if (this._transferSlider != value)
				{
					this._transferSlider = value;
					base.OnPropertyChanged<InventoryTwoWaySliderWidget>(value, "TransferSlider");
					value.intPropertyChanged += this.OnValueChanged;
					this._transferSlider.AddState("Selected");
					this._transferSlider.OverrideDefaultStateSwitchingEnabled = true;
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
				}
			}
		}

		[Editor(false)]
		public bool IsMainHero
		{
			get
			{
				return this._isMainHero;
			}
			set
			{
				if (this._isMainHero != value)
				{
					base.AcceptDrag = !value;
					this._isMainHero = value;
					base.OnPropertyChanged(value, "IsMainHero");
				}
			}
		}

		[Editor(false)]
		public bool IsPrisoner
		{
			get
			{
				return this._isPrisoner;
			}
			set
			{
				if (this._isPrisoner != value)
				{
					this._isPrisoner = value;
					base.OnPropertyChanged(value, "IsPrisoner");
				}
			}
		}

		[Editor(false)]
		public int TransferAmount
		{
			get
			{
				return this._transferAmount;
			}
			set
			{
				if (this._transferAmount != value)
				{
					this._transferAmount = value;
					base.OnPropertyChanged(value, "TransferAmount");
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
		public Widget Main
		{
			get
			{
				return this._main;
			}
			set
			{
				if (this._main != value)
				{
					this._main = value;
					base.OnPropertyChanged<Widget>(value, "Main");
				}
			}
		}

		[Editor(false)]
		public Widget UpgradesPanel
		{
			get
			{
				return this._upgradesPanel;
			}
			set
			{
				if (this._upgradesPanel != value)
				{
					this._upgradesPanel = value;
					base.OnPropertyChanged<Widget>(value, "UpgradesPanel");
				}
			}
		}

		private PartyScreenWidget _screenWidget;

		public InventoryTwoWaySliderWidget _transferSlider;

		private bool _isTupleLeftSide;

		private bool _isTransferable;

		private bool _isMainHero;

		private bool _isPrisoner;

		private int _transferAmount;

		private InventoryTupleExtensionControlsWidget _extendedControlsContainer;

		private Widget _main;

		private Widget _upgradesPanel;
	}
}
