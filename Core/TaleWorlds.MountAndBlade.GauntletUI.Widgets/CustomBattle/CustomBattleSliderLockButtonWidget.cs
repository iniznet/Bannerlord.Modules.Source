using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.CustomBattle
{
	// Token: 0x0200013D RID: 317
	public class CustomBattleSliderLockButtonWidget : ButtonWidget
	{
		// Token: 0x170005E3 RID: 1507
		// (get) Token: 0x060010AF RID: 4271 RVA: 0x0002EC2F File Offset: 0x0002CE2F
		// (set) Token: 0x060010B0 RID: 4272 RVA: 0x0002EC37 File Offset: 0x0002CE37
		public Brush LockOpenedBrush { get; set; }

		// Token: 0x170005E4 RID: 1508
		// (get) Token: 0x060010B1 RID: 4273 RVA: 0x0002EC40 File Offset: 0x0002CE40
		// (set) Token: 0x060010B2 RID: 4274 RVA: 0x0002EC48 File Offset: 0x0002CE48
		public Brush LockClosedBrush { get; set; }

		// Token: 0x060010B3 RID: 4275 RVA: 0x0002EC51 File Offset: 0x0002CE51
		public CustomBattleSliderLockButtonWidget(UIContext context)
			: base(context)
		{
			base.boolPropertyChanged += this.CustomBattleSliderLockButtonWidget_PropertyChanged;
		}

		// Token: 0x060010B4 RID: 4276 RVA: 0x0002EC6C File Offset: 0x0002CE6C
		private void CustomBattleSliderLockButtonWidget_PropertyChanged(PropertyOwnerObject widget, string propertyName, bool propertyValue)
		{
			if (propertyName == "IsSelected")
			{
				base.Brush = (propertyValue ? this.LockClosedBrush : this.LockOpenedBrush);
			}
		}
	}
}
