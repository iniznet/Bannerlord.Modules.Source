using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Siege;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.ComponentInterfaces
{
	// Token: 0x0200016D RID: 365
	public abstract class EncounterModel : GameModel
	{
		// Token: 0x17000673 RID: 1651
		// (get) Token: 0x060018CD RID: 6349
		public abstract float EstimatedMaximumMobilePartySpeedExceptPlayer { get; }

		// Token: 0x17000674 RID: 1652
		// (get) Token: 0x060018CE RID: 6350
		public abstract float NeededMaximumDistanceForEncounteringMobileParty { get; }

		// Token: 0x17000675 RID: 1653
		// (get) Token: 0x060018CF RID: 6351
		public abstract float MaximumAllowedDistanceForEncounteringMobilePartyInArmy { get; }

		// Token: 0x17000676 RID: 1654
		// (get) Token: 0x060018D0 RID: 6352
		public abstract float NeededMaximumDistanceForEncounteringTown { get; }

		// Token: 0x17000677 RID: 1655
		// (get) Token: 0x060018D1 RID: 6353
		public abstract float NeededMaximumDistanceForEncounteringVillage { get; }

		// Token: 0x060018D2 RID: 6354
		public abstract bool IsEncounterExemptFromHostileActions(PartyBase side1, PartyBase side2);

		// Token: 0x060018D3 RID: 6355
		public abstract Hero GetLeaderOfSiegeEvent(SiegeEvent siegeEvent, BattleSideEnum side);

		// Token: 0x060018D4 RID: 6356
		public abstract Hero GetLeaderOfMapEvent(MapEvent mapEvent, BattleSideEnum side);

		// Token: 0x060018D5 RID: 6357
		public abstract int GetCharacterSergeantScore(Hero hero);

		// Token: 0x060018D6 RID: 6358
		public abstract IEnumerable<PartyBase> GetDefenderPartiesOfSettlement(Settlement settlement, MapEvent.BattleTypes mapEventType);

		// Token: 0x060018D7 RID: 6359
		public abstract PartyBase GetNextDefenderPartyOfSettlement(Settlement settlement, ref int partyIndex, MapEvent.BattleTypes mapEventType);
	}
}
