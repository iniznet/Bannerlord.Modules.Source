using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.Party;

namespace TaleWorlds.CampaignSystem
{
	public struct BattleResultPartyData
	{
		public BattleResultPartyData(PartyBase party)
		{
			this.Party = party;
			this.Characters = new List<CharacterObject>();
		}

		public readonly PartyBase Party;

		public readonly List<CharacterObject> Characters;
	}
}
