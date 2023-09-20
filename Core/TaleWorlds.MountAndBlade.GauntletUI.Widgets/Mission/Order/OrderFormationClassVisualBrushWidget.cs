using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Mission.Order
{
	public class OrderFormationClassVisualBrushWidget : BrushWidget
	{
		public OrderFormationClassVisualBrushWidget(UIContext context)
			: base(context)
		{
		}

		private void UpdateVisual()
		{
			switch (this.FormationClassValue)
			{
			case 0:
				this.SetState("Infantry");
				return;
			case 1:
				this.SetState("Ranged");
				return;
			case 2:
				this.SetState("Cavalry");
				return;
			case 3:
				this.SetState("HorseArcher");
				return;
			default:
				this.SetState("Infantry");
				return;
			}
		}

		[Editor(false)]
		public int FormationClassValue
		{
			get
			{
				return this._formationClassValue;
			}
			set
			{
				if (this._formationClassValue != value)
				{
					this._formationClassValue = value;
					base.OnPropertyChanged(value, "FormationClassValue");
					this.UpdateVisual();
				}
			}
		}

		private int _formationClassValue = -1;
	}
}
