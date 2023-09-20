using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Crafting
{
	// Token: 0x02000148 RID: 328
	public class CraftingScreenWidget : Widget
	{
		// Token: 0x0600112C RID: 4396 RVA: 0x0002F9AD File Offset: 0x0002DBAD
		public CraftingScreenWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x0600112D RID: 4397 RVA: 0x0002F9B8 File Offset: 0x0002DBB8
		private void OnMainAction(Widget widget)
		{
			if (this.IsInCraftingMode)
			{
				base.Context.TwoDimensionContext.PlaySound("crafting/craft_success");
				return;
			}
			if (this.IsInRefinementMode)
			{
				base.Context.TwoDimensionContext.PlaySound("crafting/refine_success");
				return;
			}
			if (this.IsInSmeltingMode)
			{
				base.Context.TwoDimensionContext.PlaySound("crafting/smelt_success");
			}
		}

		// Token: 0x0600112E RID: 4398 RVA: 0x0002FA1E File Offset: 0x0002DC1E
		private void OnFinalAction(Widget widget)
		{
			if (this.NewCraftedWeaponPopupWidget != null && this.IsInCraftingMode)
			{
				bool isVisible = this.NewCraftedWeaponPopupWidget.IsVisible;
			}
		}

		// Token: 0x17000614 RID: 1556
		// (get) Token: 0x0600112F RID: 4399 RVA: 0x0002FA3C File Offset: 0x0002DC3C
		// (set) Token: 0x06001130 RID: 4400 RVA: 0x0002FA44 File Offset: 0x0002DC44
		[Editor(false)]
		public bool IsInCraftingMode
		{
			get
			{
				return this._isInCraftingMode;
			}
			set
			{
				if (this._isInCraftingMode != value)
				{
					this._isInCraftingMode = value;
					base.OnPropertyChanged(value, "IsInCraftingMode");
				}
			}
		}

		// Token: 0x17000615 RID: 1557
		// (get) Token: 0x06001131 RID: 4401 RVA: 0x0002FA62 File Offset: 0x0002DC62
		// (set) Token: 0x06001132 RID: 4402 RVA: 0x0002FA6A File Offset: 0x0002DC6A
		[Editor(false)]
		public bool IsInRefinementMode
		{
			get
			{
				return this._isInRefinementMode;
			}
			set
			{
				if (this._isInRefinementMode != value)
				{
					this._isInRefinementMode = value;
					base.OnPropertyChanged(value, "IsInRefinementMode");
				}
			}
		}

		// Token: 0x17000616 RID: 1558
		// (get) Token: 0x06001133 RID: 4403 RVA: 0x0002FA88 File Offset: 0x0002DC88
		// (set) Token: 0x06001134 RID: 4404 RVA: 0x0002FA90 File Offset: 0x0002DC90
		[Editor(false)]
		public bool IsInSmeltingMode
		{
			get
			{
				return this._isInSmeltingMode;
			}
			set
			{
				if (this._isInSmeltingMode != value)
				{
					this._isInSmeltingMode = value;
					base.OnPropertyChanged(value, "IsInSmeltingMode");
				}
			}
		}

		// Token: 0x17000617 RID: 1559
		// (get) Token: 0x06001135 RID: 4405 RVA: 0x0002FAAE File Offset: 0x0002DCAE
		// (set) Token: 0x06001136 RID: 4406 RVA: 0x0002FAB8 File Offset: 0x0002DCB8
		[Editor(false)]
		public ButtonWidget MainActionButtonWidget
		{
			get
			{
				return this._mainActionButtonWidget;
			}
			set
			{
				if (this._mainActionButtonWidget != value)
				{
					this._mainActionButtonWidget = value;
					base.OnPropertyChanged<ButtonWidget>(value, "MainActionButtonWidget");
					if (!value.ClickEventHandlers.Contains(new Action<Widget>(this.OnMainAction)))
					{
						value.ClickEventHandlers.Add(new Action<Widget>(this.OnMainAction));
					}
				}
			}
		}

		// Token: 0x17000618 RID: 1560
		// (get) Token: 0x06001137 RID: 4407 RVA: 0x0002FB11 File Offset: 0x0002DD11
		// (set) Token: 0x06001138 RID: 4408 RVA: 0x0002FB1C File Offset: 0x0002DD1C
		[Editor(false)]
		public ButtonWidget FinalCraftButtonWidget
		{
			get
			{
				return this._mainActionButtonWidget;
			}
			set
			{
				if (this._finalCraftButtonWidget != value)
				{
					this._finalCraftButtonWidget = value;
					base.OnPropertyChanged<ButtonWidget>(value, "FinalCraftButtonWidget");
					if (!value.ClickEventHandlers.Contains(new Action<Widget>(this.OnFinalAction)))
					{
						value.ClickEventHandlers.Add(new Action<Widget>(this.OnFinalAction));
					}
				}
			}
		}

		// Token: 0x17000619 RID: 1561
		// (get) Token: 0x06001139 RID: 4409 RVA: 0x0002FB75 File Offset: 0x0002DD75
		// (set) Token: 0x0600113A RID: 4410 RVA: 0x0002FB7D File Offset: 0x0002DD7D
		[Editor(false)]
		public Widget NewCraftedWeaponPopupWidget
		{
			get
			{
				return this._newCraftedWeaponPopupWidget;
			}
			set
			{
				if (this._newCraftedWeaponPopupWidget != value)
				{
					this._newCraftedWeaponPopupWidget = value;
					base.OnPropertyChanged<Widget>(value, "NewCraftedWeaponPopupWidget");
				}
			}
		}

		// Token: 0x1700061A RID: 1562
		// (get) Token: 0x0600113B RID: 4411 RVA: 0x0002FB9B File Offset: 0x0002DD9B
		// (set) Token: 0x0600113C RID: 4412 RVA: 0x0002FBA3 File Offset: 0x0002DDA3
		[Editor(false)]
		public Widget CraftingOrderPopupWidget
		{
			get
			{
				return this._craftingOrdersPopupWidget;
			}
			set
			{
				if (this._craftingOrdersPopupWidget != value)
				{
					this._craftingOrdersPopupWidget = value;
					base.OnPropertyChanged<Widget>(value, "CraftingOrderPopupWidget");
				}
			}
		}

		// Token: 0x040007E0 RID: 2016
		private ButtonWidget _mainActionButtonWidget;

		// Token: 0x040007E1 RID: 2017
		private ButtonWidget _finalCraftButtonWidget;

		// Token: 0x040007E2 RID: 2018
		private bool _isInCraftingMode;

		// Token: 0x040007E3 RID: 2019
		private bool _isInRefinementMode;

		// Token: 0x040007E4 RID: 2020
		private bool _isInSmeltingMode;

		// Token: 0x040007E5 RID: 2021
		private Widget _newCraftedWeaponPopupWidget;

		// Token: 0x040007E6 RID: 2022
		private Widget _craftingOrdersPopupWidget;
	}
}
