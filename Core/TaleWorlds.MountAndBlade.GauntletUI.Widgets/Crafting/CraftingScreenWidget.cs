using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Crafting
{
	public class CraftingScreenWidget : Widget
	{
		public CraftingScreenWidget(UIContext context)
			: base(context)
		{
		}

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

		private void OnFinalAction(Widget widget)
		{
			if (this.NewCraftedWeaponPopupWidget != null && this.IsInCraftingMode)
			{
				bool isVisible = this.NewCraftedWeaponPopupWidget.IsVisible;
			}
		}

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

		private ButtonWidget _mainActionButtonWidget;

		private ButtonWidget _finalCraftButtonWidget;

		private bool _isInCraftingMode;

		private bool _isInRefinementMode;

		private bool _isInSmeltingMode;

		private Widget _newCraftedWeaponPopupWidget;

		private Widget _craftingOrdersPopupWidget;
	}
}
