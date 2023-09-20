using System;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.ComponentInterfaces
{
	// Token: 0x02000188 RID: 392
	public abstract class PartyImpairmentModel : GameModel
	{
		// Token: 0x060019B6 RID: 6582
		public abstract float GetDisorganizedStateDuration(MobileParty party);

		// Token: 0x060019B7 RID: 6583
		public abstract float GetVulnerabilityStateDuration(PartyBase party);

		// Token: 0x060019B8 RID: 6584
		public abstract float GetSiegeExpectedVulnerabilityTime();

		// Token: 0x060019B9 RID: 6585
		public abstract bool CanGetDisorganized(PartyBase partyBase);
	}
}
