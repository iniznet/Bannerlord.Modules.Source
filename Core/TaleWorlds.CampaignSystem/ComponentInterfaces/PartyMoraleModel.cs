using System;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.ComponentInterfaces
{
	// Token: 0x02000181 RID: 385
	public abstract class PartyMoraleModel : GameModel
	{
		// Token: 0x17000689 RID: 1673
		// (get) Token: 0x0600195C RID: 6492
		public abstract float HighMoraleValue { get; }

		// Token: 0x0600195D RID: 6493
		public abstract int GetDailyStarvationMoralePenalty(PartyBase party);

		// Token: 0x0600195E RID: 6494
		public abstract int GetDailyNoWageMoralePenalty(MobileParty party);

		// Token: 0x0600195F RID: 6495
		public abstract float GetStandardBaseMorale(PartyBase party);

		// Token: 0x06001960 RID: 6496
		public abstract float GetVictoryMoraleChange(PartyBase party);

		// Token: 0x06001961 RID: 6497
		public abstract float GetDefeatMoraleChange(PartyBase party);

		// Token: 0x06001962 RID: 6498
		public abstract ExplainedNumber GetEffectivePartyMorale(MobileParty party, bool includeDescription = false);
	}
}
