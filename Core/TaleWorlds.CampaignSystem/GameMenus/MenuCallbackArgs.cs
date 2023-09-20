using System;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.GameMenus
{
	// Token: 0x020000E8 RID: 232
	public class MenuCallbackArgs
	{
		// Token: 0x170005A3 RID: 1443
		// (get) Token: 0x0600140C RID: 5132 RVA: 0x00058703 File Offset: 0x00056903
		// (set) Token: 0x0600140D RID: 5133 RVA: 0x0005870B File Offset: 0x0005690B
		public MenuContext MenuContext { get; private set; }

		// Token: 0x170005A4 RID: 1444
		// (get) Token: 0x0600140E RID: 5134 RVA: 0x00058714 File Offset: 0x00056914
		// (set) Token: 0x0600140F RID: 5135 RVA: 0x0005871C File Offset: 0x0005691C
		public MapState MapState { get; private set; }

		// Token: 0x06001410 RID: 5136 RVA: 0x00058725 File Offset: 0x00056925
		public MenuCallbackArgs(MenuContext menuContext, TextObject text)
		{
			this.MenuContext = menuContext;
			this.Text = text;
		}

		// Token: 0x06001411 RID: 5137 RVA: 0x00058758 File Offset: 0x00056958
		public MenuCallbackArgs(MapState mapState, TextObject text)
		{
			this.MapState = mapState;
			this.Text = text;
		}

		// Token: 0x06001412 RID: 5138 RVA: 0x0005878B File Offset: 0x0005698B
		public MenuCallbackArgs(MapState mapState, TextObject text, float dt)
		{
			this.MapState = mapState;
			this.Text = text;
			this.DeltaTime = dt;
		}

		// Token: 0x04000706 RID: 1798
		public float DeltaTime;

		// Token: 0x04000707 RID: 1799
		public bool IsEnabled = true;

		// Token: 0x04000708 RID: 1800
		public TextObject Text;

		// Token: 0x04000709 RID: 1801
		public TextObject Tooltip = TextObject.Empty;

		// Token: 0x0400070A RID: 1802
		public GameMenuOption.IssueQuestFlags OptionQuestData;

		// Token: 0x0400070B RID: 1803
		public GameMenuOption.LeaveType optionLeaveType;

		// Token: 0x0400070C RID: 1804
		public TextObject MenuTitle = TextObject.Empty;
	}
}
