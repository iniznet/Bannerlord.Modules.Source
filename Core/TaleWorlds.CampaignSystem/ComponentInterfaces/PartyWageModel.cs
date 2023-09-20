using System;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.ComponentInterfaces
{
	// Token: 0x02000199 RID: 409
	public abstract class PartyWageModel : GameModel
	{
		// Token: 0x170006B4 RID: 1716
		// (get) Token: 0x06001A25 RID: 6693
		public abstract int MaxWage { get; }

		// Token: 0x06001A26 RID: 6694
		public abstract int GetCharacterWage(CharacterObject character);

		// Token: 0x06001A27 RID: 6695
		public abstract ExplainedNumber GetTotalWage(MobileParty mobileParty, bool includeDescriptions = false);

		// Token: 0x06001A28 RID: 6696
		public abstract int GetTroopRecruitmentCost(CharacterObject troop, Hero buyerHero, bool withoutItemCost = false);
	}
}
