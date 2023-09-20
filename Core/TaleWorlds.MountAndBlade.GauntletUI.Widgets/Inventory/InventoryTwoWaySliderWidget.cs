using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.GauntletUI.ExtraWidgets;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Inventory
{
	// Token: 0x02000127 RID: 295
	public class InventoryTwoWaySliderWidget : TwoWaySliderWidget
	{
		// Token: 0x17000567 RID: 1383
		// (get) Token: 0x06000F63 RID: 3939 RVA: 0x0002B0B6 File Offset: 0x000292B6
		// (set) Token: 0x06000F64 RID: 3940 RVA: 0x0002B0BE File Offset: 0x000292BE
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

		// Token: 0x06000F65 RID: 3941 RVA: 0x0002B0D6 File Offset: 0x000292D6
		public InventoryTwoWaySliderWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000F66 RID: 3942 RVA: 0x0002B0DF File Offset: 0x000292DF
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

		// Token: 0x06000F67 RID: 3943 RVA: 0x0002B112 File Offset: 0x00029312
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

		// Token: 0x06000F68 RID: 3944 RVA: 0x0002B150 File Offset: 0x00029350
		private void OnStockChangeClick(Widget obj)
		{
			this._manuallyIncreased = true;
		}

		// Token: 0x17000568 RID: 1384
		// (get) Token: 0x06000F69 RID: 3945 RVA: 0x0002B159 File Offset: 0x00029359
		// (set) Token: 0x06000F6A RID: 3946 RVA: 0x0002B161 File Offset: 0x00029361
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

		// Token: 0x17000569 RID: 1385
		// (get) Token: 0x06000F6B RID: 3947 RVA: 0x0002B196 File Offset: 0x00029396
		// (set) Token: 0x06000F6C RID: 3948 RVA: 0x0002B19E File Offset: 0x0002939E
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

		// Token: 0x1700056A RID: 1386
		// (get) Token: 0x06000F6D RID: 3949 RVA: 0x0002B1D3 File Offset: 0x000293D3
		// (set) Token: 0x06000F6E RID: 3950 RVA: 0x0002B1DB File Offset: 0x000293DB
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

		// Token: 0x04000707 RID: 1799
		private bool _isExtended;

		// Token: 0x04000708 RID: 1800
		private Widget _initFiller;

		// Token: 0x04000709 RID: 1801
		private ButtonWidget _increaseStockButtonWidget;

		// Token: 0x0400070A RID: 1802
		private ButtonWidget _decreaseStockButtonWidget;

		// Token: 0x0400070B RID: 1803
		private bool _isRightSide;
	}
}
