using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Inventory
{
	// Token: 0x02000121 RID: 289
	public class InventoryItemPreviewWidget : Widget
	{
		// Token: 0x06000EC1 RID: 3777 RVA: 0x00028F48 File Offset: 0x00027148
		public InventoryItemPreviewWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000EC2 RID: 3778 RVA: 0x00028F51 File Offset: 0x00027151
		public void SetLastFocusedItem(Widget lastFocusedWidget)
		{
			this._lastFocusedWidget = lastFocusedWidget;
		}

		// Token: 0x17000533 RID: 1331
		// (get) Token: 0x06000EC3 RID: 3779 RVA: 0x00028F5A File Offset: 0x0002715A
		// (set) Token: 0x06000EC4 RID: 3780 RVA: 0x00028F64 File Offset: 0x00027164
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

		// Token: 0x17000534 RID: 1332
		// (get) Token: 0x06000EC5 RID: 3781 RVA: 0x00028FB0 File Offset: 0x000271B0
		// (set) Token: 0x06000EC6 RID: 3782 RVA: 0x00028FB8 File Offset: 0x000271B8
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

		// Token: 0x040006BA RID: 1722
		private Widget _lastFocusedWidget;

		// Token: 0x040006BB RID: 1723
		private ItemTableauWidget _itemTableau;

		// Token: 0x040006BC RID: 1724
		private bool _isPreviewOpen;
	}
}
