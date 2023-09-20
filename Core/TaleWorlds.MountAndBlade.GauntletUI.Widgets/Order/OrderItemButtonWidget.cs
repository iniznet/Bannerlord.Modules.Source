using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Order
{
	public class OrderItemButtonWidget : ButtonWidget
	{
		public OrderItemButtonWidget(UIContext context)
			: base(context)
		{
		}

		private void SelectionStateChanged()
		{
			switch (this.SelectionState)
			{
			case 0:
				this.SelectionVisualWidget.SetState("Disabled");
				return;
			case 2:
				this.SelectionVisualWidget.SetState("PartiallyActive");
				return;
			case 3:
				this.SelectionVisualWidget.SetState("Active");
				return;
			}
			this.SelectionVisualWidget.SetState("Default");
		}

		private void UpdateIcon()
		{
			if (this.IconWidget == null || string.IsNullOrEmpty(this.OrderIconID))
			{
				return;
			}
			this.IconWidget.Sprite = base.Context.SpriteData.GetSprite("Order\\ItemIcons\\OI" + this.OrderIconID);
		}

		[Editor(false)]
		public int SelectionState
		{
			get
			{
				return this._selectionState;
			}
			set
			{
				if (this._selectionState != value)
				{
					this._selectionState = value;
					base.OnPropertyChanged(value, "SelectionState");
					this.SelectionStateChanged();
				}
			}
		}

		[Editor(false)]
		public string OrderIconID
		{
			get
			{
				return this._orderIconID;
			}
			set
			{
				if (this._orderIconID != value)
				{
					this._orderIconID = value;
					base.OnPropertyChanged<string>(value, "OrderIconID");
					this.UpdateIcon();
				}
			}
		}

		[Editor(false)]
		public Widget IconWidget
		{
			get
			{
				return this._iconWidget;
			}
			set
			{
				if (this._iconWidget != value)
				{
					this._iconWidget = value;
					base.OnPropertyChanged<Widget>(value, "IconWidget");
					this.UpdateIcon();
				}
			}
		}

		[Editor(false)]
		public ImageWidget SelectionVisualWidget
		{
			get
			{
				return this._selectionVisualWidget;
			}
			set
			{
				if (this._selectionVisualWidget != value)
				{
					this._selectionVisualWidget = value;
					base.OnPropertyChanged<ImageWidget>(value, "SelectionVisualWidget");
					if (value != null)
					{
						value.AddState("Disabled");
						value.AddState("PartiallyActive");
						value.AddState("Active");
					}
				}
			}
		}

		private int _selectionState;

		private string _orderIconID;

		private Widget _iconWidget;

		private ImageWidget _selectionVisualWidget;
	}
}
