using System;
using TaleWorlds.CampaignSystem.Party;

namespace TaleWorlds.CampaignSystem.CampaignBehaviors
{
	// Token: 0x0200039C RID: 924
	public interface IDisbandPartyCampaignBehavior : ICampaignBehavior
	{
		// Token: 0x0600372F RID: 14127
		bool IsPartyWaitingForDisband(MobileParty party);
	}
}
