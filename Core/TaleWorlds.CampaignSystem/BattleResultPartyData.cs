using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.Party;

namespace TaleWorlds.CampaignSystem
{
	// Token: 0x02000021 RID: 33
	public struct BattleResultPartyData
	{
		// Token: 0x0600014C RID: 332 RVA: 0x0000E9FF File Offset: 0x0000CBFF
		public BattleResultPartyData(PartyBase party)
		{
			this.Party = party;
			this.Characters = new List<CharacterObject>();
		}

		// Token: 0x04000033 RID: 51
		public readonly PartyBase Party;

		// Token: 0x04000034 RID: 52
		public readonly List<CharacterObject> Characters;
	}
}
