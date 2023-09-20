using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Inventory
{
	public class InventoryItemPreviewWidget : Widget
	{
		public InventoryItemPreviewWidget(UIContext context)
			: base(context)
		{
		}

		public void SetLastFocusedItem(Widget lastFocusedWidget)
		{
			this._lastFocusedWidget = lastFocusedWidget;
		}

		[Editor(false)]
		public bool IsPreviewOpen
		{
			get
			{
				return this._isPreviewOpen;
			}
			set
			{
				if (this._isPreviewOpen != value)
				{
					if (!value)
					{
						base.EventManager.SetWidgetFocused(this._lastFocusedWidget, false);
						this._lastFocusedWidget = null;
					}
					this._isPreviewOpen = value;
					base.IsVisible = value;
					base.OnPropertyChanged(value, "IsPreviewOpen");
				}
			}
		}

		[Editor(false)]
		public ItemTableauWidget ItemTableau
		{
			get
			{
				return this._itemTableau;
			}
			set
			{
				if (this._itemTableau != value)
				{
					this._itemTableau = value;
					base.OnPropertyChanged<ItemTableauWidget>(value, "ItemTableau");
				}
			}
		}

		private Widget _lastFocusedWidget;

		private ItemTableauWidget _itemTableau;

		private bool _isPreviewOpen;
	}
}
