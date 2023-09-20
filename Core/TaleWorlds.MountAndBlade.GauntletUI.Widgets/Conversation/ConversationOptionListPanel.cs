using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Conversation
{
	// Token: 0x0200014E RID: 334
	public class ConversationOptionListPanel : ListPanel
	{
		// Token: 0x17000626 RID: 1574
		// (get) Token: 0x06001161 RID: 4449 RVA: 0x0002FF73 File Offset: 0x0002E173
		// (set) Token: 0x06001162 RID: 4450 RVA: 0x0002FF7B File Offset: 0x0002E17B
		public ButtonWidget OptionButtonWidget { get; set; }

		// Token: 0x06001163 RID: 4451 RVA: 0x0002FF84 File Offset: 0x0002E184
		public ConversationOptionListPanel(UIContext context)
			: base(context)
		{
		}
	}
}
