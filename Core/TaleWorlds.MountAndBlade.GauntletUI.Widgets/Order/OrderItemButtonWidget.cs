using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Order
{
	// Token: 0x02000063 RID: 99
	public class OrderItemButtonWidget : ButtonWidget
	{
		// Token: 0x0600053C RID: 1340 RVA: 0x0000FE00 File Offset: 0x0000E000
		public OrderItemButtonWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x0600053D RID: 1341 RVA: 0x0000FE0C File Offset: 0x0000E00C
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

		// Token: 0x0600053E RID: 1342 RVA: 0x0000FE7C File Offset: 0x0000E07C
		private void UpdateIcon()
		{
			if (this.IconWidget == null || string.IsNullOrEmpty(this.OrderIconID))
			{
				return;
			}
			this.IconWidget.Sprite = base.Context.SpriteData.GetSprite("Order\\ItemIcons\\OI" + this.OrderIconID);
		}

		// Token: 0x170001D9 RID: 473
		// (get) Token: 0x0600053F RID: 1343 RVA: 0x0000FECA File Offset: 0x0000E0CA
		// (set) Token: 0x06000540 RID: 1344 RVA: 0x0000FED2 File Offset: 0x0000E0D2
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

		// Token: 0x170001DA RID: 474
		// (get) Token: 0x06000541 RID: 1345 RVA: 0x0000FEF6 File Offset: 0x0000E0F6
		// (set) Token: 0x06000542 RID: 1346 RVA: 0x0000FEFE File Offset: 0x0000E0FE
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

		// Token: 0x170001DB RID: 475
		// (get) Token: 0x06000543 RID: 1347 RVA: 0x0000FF27 File Offset: 0x0000E127
		// (set) Token: 0x06000544 RID: 1348 RVA: 0x0000FF2F File Offset: 0x0000E12F
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

		// Token: 0x170001DC RID: 476
		// (get) Token: 0x06000545 RID: 1349 RVA: 0x0000FF53 File Offset: 0x0000E153
		// (set) Token: 0x06000546 RID: 1350 RVA: 0x0000FF5C File Offset: 0x0000E15C
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

		// Token: 0x04000245 RID: 581
		private int _selectionState;

		// Token: 0x04000246 RID: 582
		private string _orderIconID;

		// Token: 0x04000247 RID: 583
		private Widget _iconWidget;

		// Token: 0x04000248 RID: 584
		private ImageWidget _selectionVisualWidget;
	}
}
