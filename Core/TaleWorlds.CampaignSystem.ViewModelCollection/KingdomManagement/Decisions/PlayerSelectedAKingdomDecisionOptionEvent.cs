using System;
using TaleWorlds.CampaignSystem.Election;
using TaleWorlds.Library.EventSystem;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.KingdomManagement.Decisions
{
	// Token: 0x02000064 RID: 100
	public class PlayerSelectedAKingdomDecisionOptionEvent : EventBase
	{
		// Token: 0x170002B2 RID: 690
		// (get) Token: 0x060008AF RID: 2223 RVA: 0x0002442C File Offset: 0x0002262C
		// (set) Token: 0x060008B0 RID: 2224 RVA: 0x00024434 File Offset: 0x00022634
		public DecisionOutcome Option { get; private set; }

		// Token: 0x060008B1 RID: 2225 RVA: 0x0002443D File Offset: 0x0002263D
		public PlayerSelectedAKingdomDecisionOptionEvent(DecisionOutcome option)
		{
			this.Option = option;
		}
	}
}
