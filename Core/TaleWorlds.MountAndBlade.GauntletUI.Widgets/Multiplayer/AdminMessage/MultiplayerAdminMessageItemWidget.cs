using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer.AdminMessage
{
	// Token: 0x020000BF RID: 191
	public class MultiplayerAdminMessageItemWidget : Widget
	{
		// Token: 0x060009BD RID: 2493 RVA: 0x0001BD32 File Offset: 0x00019F32
		public MultiplayerAdminMessageItemWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x060009BE RID: 2494 RVA: 0x0001BD3B File Offset: 0x00019F3B
		public void Remove()
		{
			base.EventFired("Remove", Array.Empty<object>());
		}
	}
}
