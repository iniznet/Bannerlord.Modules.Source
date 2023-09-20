using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Inventory
{
	// Token: 0x02000123 RID: 291
	public class InventoryItemValueTextWidget : TextWidget
	{
		// Token: 0x06000F14 RID: 3860 RVA: 0x00029CDF File Offset: 0x00027EDF
		public InventoryItemValueTextWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000F15 RID: 3861 RVA: 0x00029CE8 File Offset: 0x00027EE8
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

		// Token: 0x1700054F RID: 1359
		// (get) Token: 0x06000F16 RID: 3862 RVA: 0x00029D6A File Offset: 0x00027F6A
		// (set) Token: 0x06000F17 RID: 3863 RVA: 0x00029D72 File Offset: 0x00027F72
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

		// Token: 0x040006DE RID: 1758
		private bool _firstHandled;

		// Token: 0x040006DF RID: 1759
		private int _profitType;
	}
}
