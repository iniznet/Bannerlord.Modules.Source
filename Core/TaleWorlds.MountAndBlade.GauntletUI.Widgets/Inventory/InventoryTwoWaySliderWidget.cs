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

		protected override void OnUpdate(float dt)
		{
			base.OnUpdate(dt);
			if (this._isBeingDragged && !base.IsPressed)
			{
				Widget handle = base.Handle;
				if (handle != null && !handle.IsPressed)
				{
					this._shouldRemoveZeroCounts = true;
				}
			}
			bool flag;
			if (!base.IsPressed)
			{
				Widget handle2 = base.Handle;
				flag = handle2 != null && handle2.IsPressed;
			}
			else
			{
				flag = true;
			}
			this._isBeingDragged = flag;
			if (this._shouldRemoveZeroCounts)
			{
				base.EventFired("RemoveZeroCounts", Array.Empty<object>());
				this._shouldRemoveZeroCounts = false;
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
			this._shouldRemoveZeroCounts = true;
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

		private bool _isBeingDragged;

		private bool _shouldRemoveZeroCounts;

		private ButtonWidget _increaseStockButtonWidget;

		private ButtonWidget _decreaseStockButtonWidget;

		private bool _isRightSide;
	}
}
