using System;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.ComponentInterfaces
{
	// Token: 0x02000191 RID: 401
	public abstract class BattleRewardModel : GameModel
	{
		// Token: 0x170006AE RID: 1710
		// (get) Token: 0x060019F0 RID: 6640
		public abstract float DestroyHideoutBannerLootChance { get; }

		// Token: 0x170006AF RID: 1711
		// (get) Token: 0x060019F1 RID: 6641
		public abstract float CaptureSettlementBannerLootChance { get; }

		// Token: 0x170006B0 RID: 1712
		// (get) Token: 0x060019F2 RID: 6642
		public abstract float DefeatRegularHeroBannerLootChance { get; }

		// Token: 0x170006B1 RID: 1713
		// (get) Token: 0x060019F3 RID: 6643
		public abstract float DefeatClanLeaderBannerLootChance { get; }

		// Token: 0x170006B2 RID: 1714
		// (get) Token: 0x060019F4 RID: 6644
		public abstract float DefeatKingdomRulerBannerLootChance { get; }

		// Token: 0x060019F5 RID: 6645
		public abstract int GetPlayerGainedRelationAmount(MapEvent mapEvent, Hero hero);

		// Token: 0x060019F6 RID: 6646
		public abstract ExplainedNumber CalculateRenownGain(PartyBase party, float renownValueOfBattle, float contributionShare);

		// Token: 0x060019F7 RID: 6647
		public abstract ExplainedNumber CalculateInfluenceGain(PartyBase party, float influenceValueOfBattle, float contributionShare);

		// Token: 0x060019F8 RID: 6648
		public abstract ExplainedNumber CalculateMoraleGainVictory(PartyBase party, float renownValueOfBattle, float contributionShare);

		// Token: 0x060019F9 RID: 6649
		public abstract int CalculateGoldLossAfterDefeat(Hero partyLeaderHero);

		// Token: 0x060019FA RID: 6650
		public abstract EquipmentElement GetLootedItemFromTroop(CharacterObject character, float targetValue);

		// Token: 0x060019FB RID: 6651
		public abstract float GetPartySavePrisonerAsMemberShareProbability(PartyBase winnerParty, float lootAmount);

		// Token: 0x060019FC RID: 6652
		public abstract float GetExpectedLootedItemValue(CharacterObject character);

		// Token: 0x060019FD RID: 6653
		public abstract float GetAITradePenalty();
	}
}
