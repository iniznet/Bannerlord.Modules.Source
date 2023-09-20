using System;
using TaleWorlds.GauntletUI.Layout;

namespace TaleWorlds.GauntletUI.BaseTypes
{
	// Token: 0x02000057 RID: 87
	public class DragCarrierWidget : Widget
	{
		// Token: 0x06000582 RID: 1410 RVA: 0x00017C25 File Offset: 0x00015E25
		public DragCarrierWidget(UIContext context)
			: base(context)
		{
			base.LayoutImp = new DragCarrierLayout();
			base.DoNotAcceptEvents = true;
			base.DoNotPassEventsToChildren = true;
			base.IsDisabled = true;
		}
	}
}
