using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.GauntletUI.ExtraWidgets;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Inventory
{
	public class InventoryTwoWaySliderWidget : TwoWaySliderWidget
	{
		public bool IsExtended
		{
			get
			{
				return this._isExtended;
			}
			set
			{
				if (this._isExtended != value)
				{
					this.CheckFillerState();
					this._isExtended = value;
				}
			}
		}

		public InventoryTwoWaySliderWidget(UIContext context)
			: base(context)
		{
		}

		protected override void OnParallelUpdate(float dt)
		{
			if (this._initFiller == null && base.Filler != null)
			{
				this._initFiller = base.Filler;
			}
			if (this.IsExtended)
			{
				base.OnParallelUpdate(dt);
				this.CheckFillerState();
			}
		}

		private void CheckFillerState()
		{
			if (this._initFiller != null)
			{
				if (this.IsExtended && base.Filler == null)
				{
					base.Filler = this._initFiller;
					return;
				}
				if (!this.IsExtended && base.Filler != null)
				{
					base.Filler = null;
				}
			}
		}

		private void OnStockChangeClick(Widget obj)
		{
			this._manuallyIncreased = true;
		}

		[Editor(false)]
		public ButtonWidget IncreaseStockButtonWidget
		{
			get
			{
				return this._increaseStockButtonWidget;
			}
			set
			{
				if (this._increaseStockButtonWidget != value)
				{
					this._increaseStockButtonWidget = value;
					base.OnPropertyChanged<ButtonWidget>(value, "IncreaseStockButtonWidget");
					value.ClickEventHandlers.Add(new Action<Widget>(this.OnStockChangeClick));
				}
			}
		}

		[Editor(false)]
		public ButtonWidget DecreaseStockButtonWidget
		{
			get
			{
				return this._decreaseStockButtonWidget;
			}
			set
			{
				if (this._decreaseStockButtonWidget != value)
				{
					this._decreaseStockButtonWidget = value;
					base.OnPropertyChanged<ButtonWidget>(value, "DecreaseStockButtonWidget");
					value.ClickEventHandlers.Add(new Action<Widget>(this.OnStockChangeClick));
				}
			}
		}

		[Editor(false)]
		public bool IsRightSide
		{
			get
			{
				return this._isRightSide;
			}
			set
			{
				if (this._isRightSide != value)
				{
					this._isRightSide = value;
					base.OnPropertyChanged(value, "IsRightSide");
				}
			}
		}

		private bool _isExtended;

		private Widget _initFiller;

		private ButtonWidget _increaseStockButtonWidget;

		private ButtonWidget _decreaseStockButtonWidget;

		private bool _isRightSide;
	}
}
