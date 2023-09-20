using System;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Library.EventSystem;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia
{
	// Token: 0x020000AB RID: 171
	public class PlayerToggleTrackSettlementFromEncyclopediaEvent : EventBase
	{
		// Token: 0x1700058F RID: 1423
		// (get) Token: 0x060010DB RID: 4315 RVA: 0x00043107 File Offset: 0x00041307
		// (set) Token: 0x060010DC RID: 4316 RVA: 0x0004310F File Offset: 0x0004130F
		public bool IsCurrentlyTracked { get; private set; }

		// Token: 0x17000590 RID: 1424
		// (get) Token: 0x060010DD RID: 4317 RVA: 0x00043118 File Offset: 0x00041318
		// (set) Token: 0x060010DE RID: 4318 RVA: 0x00043120 File Offset: 0x00041320
		public Settlement ToggledTrackedSettlement { get; private set; }

		// Token: 0x060010DF RID: 4319 RVA: 0x00043129 File Offset: 0x00041329
		public PlayerToggleTrackSettlementFromEncyclopediaEvent(Settlement toggleTrackedSettlement, bool isCurrentlyTracked)
		{
			this.ToggledTrackedSettlement = toggleTrackedSettlement;
			this.IsCurrentlyTracked = isCurrentlyTracked;
		}
	}
}
