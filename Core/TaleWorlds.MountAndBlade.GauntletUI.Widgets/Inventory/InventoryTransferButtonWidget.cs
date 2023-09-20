using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Inventory
{
	public class InventoryTransferButtonWidget : ButtonWidget
	{
		public InventoryTransferButtonWidget(UIContext context)
			: base(context)
		{
		}

		public void FireClickEvent()
		{
			if (this.IsSell)
			{
				base.EventFired("SellAction", Array.Empty<object>());
				return;
			}
			base.EventFired("BuyAction", Array.Empty<object>());
		}

		private void HandleVisuals()
		{
			int num;
			Brush brush;
			if (this.IsSell)
			{
				num = 0;
				brush = this.SellBrush;
			}
			else
			{
				num = base.ParentWidget.ParentWidget.ChildCount - 1;
				brush = this.BuyBrush;
			}
			if (this.ModifySiblingIndex)
			{
				base.ParentWidget.SetSiblingIndex(num, false);
			}
			base.Brush = brush;
		}

		[Editor(false)]
		public bool IsSell
		{
			get
			{
				return this._isSell;
			}
			set
			{
				if (this._isSell != value)
				{
					this._isSell = value;
					this.HandleVisuals();
					base.OnPropertyChanged(value, "IsSell");
				}
			}
		}

		[Editor(false)]
		public bool ModifySiblingIndex
		{
			get
			{
				return this._modifySiblingIndex;
			}
			set
			{
				if (this._modifySiblingIndex != value)
				{
					this._modifySiblingIndex = value;
					this.HandleVisuals();
					base.OnPropertyChanged(value, "ModifySiblingIndex");
				}
			}
		}

		[Editor(false)]
		public Brush BuyBrush
		{
			get
			{
				return this._buyBrush;
			}
			set
			{
				if (this._buyBrush != value)
				{
					this._buyBrush = value;
					base.OnPropertyChanged<Brush>(value, "BuyBrush");
				}
			}
		}

		[Editor(false)]
		public Brush SellBrush
		{
			get
			{
				return this._sellBrush;
			}
			set
			{
				if (this._sellBrush != value)
				{
					this._sellBrush = value;
					base.OnPropertyChanged<Brush>(value, "SellBrush");
				}
			}
		}

		private bool _isSell;

		private bool _modifySiblingIndex;

		private Brush _buyBrush;

		private Brush _sellBrush;
	}
}
