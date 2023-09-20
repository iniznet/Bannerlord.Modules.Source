using System;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.ComponentInterfaces
{
	// Token: 0x02000196 RID: 406
	public abstract class PartySizeLimitModel : GameModel
	{
		// Token: 0x06001A1A RID: 6682
		public abstract ExplainedNumber GetPartyMemberSizeLimit(PartyBase party, bool includeDescriptions = false);

		// Token: 0x06001A1B RID: 6683
		public abstract ExplainedNumber GetPartyPrisonerSizeLimit(PartyBase party, bool includeDescriptions = false);

		// Token: 0x06001A1C RID: 6684
		public abstract int GetTierPartySizeEffect(int tier);

		// Token: 0x06001A1D RID: 6685
		public abstract int GetAssumedPartySizeForLordParty(Hero leaderHero, IFaction partyMapFaction, Clan actualClan);
	}
}
