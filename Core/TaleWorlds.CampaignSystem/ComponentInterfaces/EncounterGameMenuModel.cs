using System;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.ComponentInterfaces
{
	// Token: 0x02000194 RID: 404
	public abstract class EncounterGameMenuModel : GameModel
	{
		// Token: 0x06001A11 RID: 6673
		public abstract string GetEncounterMenu(PartyBase attackerParty, PartyBase defenderParty, out bool startBattle, out bool joinBattle);

		// Token: 0x06001A12 RID: 6674
		public abstract string GetRaidCompleteMenu();

		// Token: 0x06001A13 RID: 6675
		public abstract string GetNewPartyJoinMenu(MobileParty newParty);

		// Token: 0x06001A14 RID: 6676
		public abstract string GetGenericStateMenu();
	}
}
