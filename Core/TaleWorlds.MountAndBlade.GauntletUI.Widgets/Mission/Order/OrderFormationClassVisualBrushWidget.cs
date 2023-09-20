using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Mission.Order
{
	// Token: 0x020000CE RID: 206
	public class OrderFormationClassVisualBrushWidget : BrushWidget
	{
		// Token: 0x06000A84 RID: 2692 RVA: 0x0001D8F3 File Offset: 0x0001BAF3
		public OrderFormationClassVisualBrushWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000A85 RID: 2693 RVA: 0x0001D904 File Offset: 0x0001BB04
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

		// Token: 0x170003B5 RID: 949
		// (get) Token: 0x06000A86 RID: 2694 RVA: 0x0001D96B File Offset: 0x0001BB6B
		// (set) Token: 0x06000A87 RID: 2695 RVA: 0x0001D973 File Offset: 0x0001BB73
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

		// Token: 0x040004CC RID: 1228
		private int _formationClassValue = -1;
	}
}
