using System;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ComponentInterfaces
{
	// Token: 0x02000186 RID: 390
	public abstract class KingdomDecisionPermissionModel : GameModel
	{
		// Token: 0x060019AA RID: 6570
		public abstract bool IsPolicyDecisionAllowed(PolicyObject policy);

		// Token: 0x060019AB RID: 6571
		public abstract bool IsWarDecisionAllowedBetweenKingdoms(Kingdom kingdom1, Kingdom kingdom2, out TextObject reason);

		// Token: 0x060019AC RID: 6572
		public abstract bool IsPeaceDecisionAllowedBetweenKingdoms(Kingdom kingdom1, Kingdom kingdom2, out TextObject reason);

		// Token: 0x060019AD RID: 6573
		public abstract bool IsAnnexationDecisionAllowed(Settlement annexedSettlement);

		// Token: 0x060019AE RID: 6574
		public abstract bool IsExpulsionDecisionAllowed(Clan expelledClan);

		// Token: 0x060019AF RID: 6575
		public abstract bool IsKingSelectionDecisionAllowed(Kingdom kingdom);
	}
}
