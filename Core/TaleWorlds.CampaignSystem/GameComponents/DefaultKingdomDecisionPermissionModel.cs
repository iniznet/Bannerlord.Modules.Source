using System;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.GameComponents
{
	// Token: 0x02000115 RID: 277
	public class DefaultKingdomDecisionPermissionModel : KingdomDecisionPermissionModel
	{
		// Token: 0x060015DA RID: 5594 RVA: 0x0006775F File Offset: 0x0006595F
		public override bool IsPolicyDecisionAllowed(PolicyObject policy)
		{
			return true;
		}

		// Token: 0x060015DB RID: 5595 RVA: 0x00067762 File Offset: 0x00065962
		public override bool IsWarDecisionAllowedBetweenKingdoms(Kingdom kingdom1, Kingdom kingdom2, out TextObject reason)
		{
			reason = TextObject.Empty;
			return true;
		}

		// Token: 0x060015DC RID: 5596 RVA: 0x0006776C File Offset: 0x0006596C
		public override bool IsPeaceDecisionAllowedBetweenKingdoms(Kingdom kingdom1, Kingdom kingdom2, out TextObject reason)
		{
			reason = TextObject.Empty;
			return true;
		}

		// Token: 0x060015DD RID: 5597 RVA: 0x00067776 File Offset: 0x00065976
		public override bool IsAnnexationDecisionAllowed(Settlement annexedSettlement)
		{
			return true;
		}

		// Token: 0x060015DE RID: 5598 RVA: 0x00067779 File Offset: 0x00065979
		public override bool IsExpulsionDecisionAllowed(Clan expelledClan)
		{
			return true;
		}

		// Token: 0x060015DF RID: 5599 RVA: 0x0006777C File Offset: 0x0006597C
		public override bool IsKingSelectionDecisionAllowed(Kingdom kingdom)
		{
			return true;
		}
	}
}
