using System;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.ComponentInterfaces
{
	// Token: 0x02000198 RID: 408
	public abstract class PartyDesertionModel : GameModel
	{
		// Token: 0x06001A22 RID: 6690
		public abstract int GetNumberOfDeserters(MobileParty mobileParty);

		// Token: 0x06001A23 RID: 6691
		public abstract int GetMoraleThresholdForTroopDesertion(MobileParty mobileParty);
	}
}
