using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Mission.OrderOfBattle
{
	// Token: 0x020000D7 RID: 215
	public class OrderOfBattleHeroDropWidget : ButtonWidget
	{
		// Token: 0x06000AED RID: 2797 RVA: 0x0001E638 File Offset: 0x0001C838
		public OrderOfBattleHeroDropWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000AEE RID: 2798 RVA: 0x0001E641 File Offset: 0x0001C841
		protected override bool OnPreviewDrop()
		{
			return true;
		}

		// Token: 0x06000AEF RID: 2799 RVA: 0x0001E644 File Offset: 0x0001C844
		protected override bool OnPreviewDragHover()
		{
			return true;
		}
	}
}
