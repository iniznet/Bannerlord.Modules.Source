using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.GatherArmy
{
	public class BoostItemButtonWidget : ButtonWidget
	{
		public BoostCohesionPopupWidget ParentPopupWidget { get; private set; }

		public BoostItemButtonWidget(UIContext context)
			: base(context)
		{
		}

		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (this.BoostCurrencyIconWidget != null)
			{
				int boostCurrencyType = this.BoostCurrencyType;
				if (boostCurrencyType != 0)
				{
					if (boostCurrencyType == 1)
					{
						this.BoostCurrencyIconWidget.SetState("Influence");
					}
				}
				else
				{
					this.BoostCurrencyIconWidget.SetState("Gold");
				}
			}
			if (this.ParentPopupWidget == null)
			{
				this.ParentPopupWidget = this.FindParentPopupWidget();
				if (this.ParentPopupWidget != null)
				{
					this.ClickEventHandlers.Add(new Action<Widget>(this.ParentPopupWidget.ClosePopup));
				}
			}
		}

		private BoostCohesionPopupWidget FindParentPopupWidget()
		{
			Widget widget = this;
			while (widget != base.EventManager.Root && this.ParentPopupWidget == null)
			{
				if (widget is BoostCohesionPopupWidget)
				{
					return widget as BoostCohesionPopupWidget;
				}
				widget = widget.ParentWidget;
			}
			return null;
		}

		[Editor(false)]
		public int BoostCurrencyType
		{
			get
			{
				return this._boostCurrencyType;
			}
			set
			{
				if (this._boostCurrencyType != value)
				{
					this._boostCurrencyType = value;
					base.OnPropertyChanged(value, "BoostCurrencyType");
				}
			}
		}

		[Editor(false)]
		public Widget BoostCurrencyIconWidget
		{
			get
			{
				return this._boostCurrencyIconWidget;
			}
			set
			{
				if (this._boostCurrencyIconWidget != value)
				{
					this._boostCurrencyIconWidget = value;
					base.OnPropertyChanged<Widget>(value, "BoostCurrencyIconWidget");
				}
			}
		}

		private int _boostCurrencyType = -1;

		private Widget _boostCurrencyIconWidget;
	}
}
