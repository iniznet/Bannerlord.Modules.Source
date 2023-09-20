using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Inventory
{
	// Token: 0x02000126 RID: 294
	public class InventoryTransferButtonWidget : ButtonWidget
	{
		// Token: 0x06000F58 RID: 3928 RVA: 0x0002AF86 File Offset: 0x00029186
		public InventoryTransferButtonWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000F59 RID: 3929 RVA: 0x0002AF8F File Offset: 0x0002918F
		public void FireClickEvent()
		{
			if (this.IsSell)
			{
				base.EventFired("SellAction", Array.Empty<object>());
				return;
			}
			base.EventFired("BuyAction", Array.Empty<object>());
		}

		// Token: 0x06000F5A RID: 3930 RVA: 0x0002AFBC File Offset: 0x000291BC
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

		// Token: 0x17000563 RID: 1379
		// (get) Token: 0x06000F5B RID: 3931 RVA: 0x0002B012 File Offset: 0x00029212
		// (set) Token: 0x06000F5C RID: 3932 RVA: 0x0002B01A File Offset: 0x0002921A
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

		// Token: 0x17000564 RID: 1380
		// (get) Token: 0x06000F5D RID: 3933 RVA: 0x0002B03E File Offset: 0x0002923E
		// (set) Token: 0x06000F5E RID: 3934 RVA: 0x0002B046 File Offset: 0x00029246
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

		// Token: 0x17000565 RID: 1381
		// (get) Token: 0x06000F5F RID: 3935 RVA: 0x0002B06A File Offset: 0x0002926A
		// (set) Token: 0x06000F60 RID: 3936 RVA: 0x0002B072 File Offset: 0x00029272
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

		// Token: 0x17000566 RID: 1382
		// (get) Token: 0x06000F61 RID: 3937 RVA: 0x0002B090 File Offset: 0x00029290
		// (set) Token: 0x06000F62 RID: 3938 RVA: 0x0002B098 File Offset: 0x00029298
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

		// Token: 0x04000703 RID: 1795
		private bool _isSell;

		// Token: 0x04000704 RID: 1796
		private bool _modifySiblingIndex;

		// Token: 0x04000705 RID: 1797
		private Brush _buyBrush;

		// Token: 0x04000706 RID: 1798
		private Brush _sellBrush;
	}
}
