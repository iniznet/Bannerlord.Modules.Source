using System;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.ComponentInterfaces
{
	// Token: 0x020001A4 RID: 420
	public abstract class SettlementGarrisonModel : GameModel
	{
		// Token: 0x06001A83 RID: 6787
		public abstract ExplainedNumber CalculateGarrisonChange(Settlement settlement, bool includeDescriptions = false);

		// Token: 0x06001A84 RID: 6788
		public abstract ExplainedNumber CalculateGarrisonChangeAutoRecruitment(Settlement settlement, bool includeDescriptions = false);

		// Token: 0x06001A85 RID: 6789
		public abstract int FindNumberOfTroopsToTakeFromGarrison(MobileParty mobileParty, Settlement settlement, float idealGarrisonStrengthPerWalledCenter = 0f);

		// Token: 0x06001A86 RID: 6790
		public abstract int FindNumberOfTroopsToLeaveToGarrison(MobileParty mobileParty, Settlement settlement);
	}
}
