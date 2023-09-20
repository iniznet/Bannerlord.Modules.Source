using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Inventory
{
	public class InventoryBooleanRadioListPanel : ListPanel
	{
		public InventoryBooleanRadioListPanel(UIContext context)
			: base(context)
		{
		}

		private void UpdateChildSelectedState()
		{
			if (base.ChildCount < 2)
			{
				return;
			}
			ButtonWidget buttonWidget = base.GetChild(1) as ButtonWidget;
			ButtonWidget buttonWidget2 = base.GetChild(0) as ButtonWidget;
			if (buttonWidget == null || buttonWidget2 == null)
			{
				return;
			}
			buttonWidget.IsSelected = this.IsFirstSelected;
			buttonWidget2.IsSelected = !this.IsFirstSelected;
		}

		public override void OnChildSelected(Widget widget)
		{
			base.OnChildSelected(widget);
			int childIndex = base.GetChildIndex(widget);
			this.IsFirstSelected = childIndex == 1;
		}

		[Editor(false)]
		public bool IsFirstSelected
		{
			get
			{
				return this._isFirstSelected;
			}
			set
			{
				if (this._isFirstSelected != value || !this._isSelectedStateSet)
				{
					this._isFirstSelected = value;
					base.OnPropertyChanged(value, "IsFirstSelected");
					this._isSelectedStateSet = true;
					this.UpdateChildSelectedState();
				}
			}
		}

		[Editor(false)]
		public bool IsSecondSelected
		{
			get
			{
				return !this._isFirstSelected;
			}
			set
			{
				if (this._isFirstSelected != !value)
				{
					this.IsFirstSelected = !value;
					base.OnPropertyChanged(!value, "IsSecondSelected");
				}
			}
		}

		private bool _isSelectedStateSet;

		private bool _isFirstSelected;
	}
}
