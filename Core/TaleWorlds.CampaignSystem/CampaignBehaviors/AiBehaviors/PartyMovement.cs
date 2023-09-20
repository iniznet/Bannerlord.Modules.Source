using System;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;

namespace TaleWorlds.CampaignSystem.CampaignBehaviors.AiBehaviors
{
	// Token: 0x02000402 RID: 1026
	public class PartyMovement
	{
		// Token: 0x17000CF1 RID: 3313
		// (get) Token: 0x06003D29 RID: 15657 RVA: 0x001235E8 File Offset: 0x001217E8
		// (set) Token: 0x06003D2A RID: 15658 RVA: 0x001235F0 File Offset: 0x001217F0
		public PartyMovementType MoveType { get; set; }

		// Token: 0x17000CF2 RID: 3314
		// (get) Token: 0x06003D2B RID: 15659 RVA: 0x001235F9 File Offset: 0x001217F9
		// (set) Token: 0x06003D2C RID: 15660 RVA: 0x00123601 File Offset: 0x00121801
		public MobileParty Party { get; set; }

		// Token: 0x17000CF3 RID: 3315
		// (get) Token: 0x06003D2D RID: 15661 RVA: 0x0012360A File Offset: 0x0012180A
		// (set) Token: 0x06003D2E RID: 15662 RVA: 0x00123612 File Offset: 0x00121812
		public Settlement Settlement { get; set; }

		// Token: 0x17000CF4 RID: 3316
		// (get) Token: 0x06003D2F RID: 15663 RVA: 0x0012361B File Offset: 0x0012181B
		// (set) Token: 0x06003D30 RID: 15664 RVA: 0x00123623 File Offset: 0x00121823
		public float Score { get; set; }

		// Token: 0x06003D31 RID: 15665 RVA: 0x0012362C File Offset: 0x0012182C
		private PartyMovement()
		{
			this.MoveType = PartyMovementType.None;
			this.Settlement = null;
			this.Party = null;
			this.Score = 0f;
		}
	}
}
