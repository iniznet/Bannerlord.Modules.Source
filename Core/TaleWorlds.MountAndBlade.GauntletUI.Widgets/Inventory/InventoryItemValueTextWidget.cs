using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Inventory
{
	public class InventoryItemValueTextWidget : TextWidget
	{
		public InventoryItemValueTextWidget(UIContext context)
			: base(context)
		{
		}

		private void HandleVisuals()
		{
			if (!this._firstHandled)
			{
				this.RegisterBrushStatesOfWidget();
				this._firstHandled = true;
			}
			switch (this.ProfitType)
			{
			case -2:
				this.SetState("VeryBad");
				return;
			case -1:
				this.SetState("Bad");
				return;
			case 0:
				this.SetState("Default");
				return;
			case 1:
				this.SetState("Good");
				return;
			case 2:
				this.SetState("VeryGood");
				return;
			default:
				return;
			}
		}

		[Editor(false)]
		public int ProfitType
		{
			get
			{
				return this._profitType;
			}
			set
			{
				if (this._profitType != value)
				{
					this._profitType = value;
					base.OnPropertyChanged(value, "ProfitType");
					this.HandleVisuals();
				}
			}
		}

		private bool _firstHandled;

		private int _profitType;
	}
}
