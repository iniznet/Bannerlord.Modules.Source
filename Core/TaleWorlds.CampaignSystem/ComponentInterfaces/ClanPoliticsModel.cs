using System;
using TaleWorlds.CampaignSystem.Election;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.ComponentInterfaces
{
	// Token: 0x020001A9 RID: 425
	public abstract class ClanPoliticsModel : GameModel
	{
		// Token: 0x06001AA9 RID: 6825
		public abstract ExplainedNumber CalculateInfluenceChange(Clan clan, bool includeDescriptions = false);

		// Token: 0x06001AAA RID: 6826
		public abstract float CalculateSupportForPolicyInClan(Clan clan, PolicyObject policy);

		// Token: 0x06001AAB RID: 6827
		public abstract float CalculateRelationshipChangeWithSponsor(Clan clan, Clan sponsorClan);

		// Token: 0x06001AAC RID: 6828
		public abstract int GetInfluenceRequiredToOverrideKingdomDecision(DecisionOutcome popularOption, DecisionOutcome overridingOption, KingdomDecision decision);
	}
}
