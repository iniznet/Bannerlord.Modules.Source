using System;
using Helpers;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Conversation.Persuasion;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.CharacterDevelopment
{
	// Token: 0x02000354 RID: 852
	public interface ISkillLevelingManager
	{
		// Token: 0x06002FF4 RID: 12276
		void OnCombatHit(CharacterObject affectorCharacter, CharacterObject affectedCharacter, CharacterObject captain, Hero commander, float speedBonusFromMovement, float shotDifficulty, WeaponComponentData affectorWeapon, float hitPointRatio, CombatXpModel.MissionTypeEnum missionType, bool isAffectorMounted, bool isTeamKill, bool isAffectorUnderCommand, float damageAmount, bool isFatal, bool isSiegeEngineHit, bool isHorseCharge);

		// Token: 0x06002FF5 RID: 12277
		void OnSiegeEngineDestroyed(MobileParty party, SiegeEngineType destroyedSiegeEngine);

		// Token: 0x06002FF6 RID: 12278
		void OnSimulationCombatKill(CharacterObject affectorCharacter, CharacterObject affectedCharacter, PartyBase affectorParty, PartyBase commanderParty);

		// Token: 0x06002FF7 RID: 12279
		void OnTradeProfitMade(PartyBase party, int tradeProfit);

		// Token: 0x06002FF8 RID: 12280
		void OnTradeProfitMade(Hero hero, int tradeProfit);

		// Token: 0x06002FF9 RID: 12281
		void OnSettlementProjectFinished(Settlement settlement);

		// Token: 0x06002FFA RID: 12282
		void OnSettlementGoverned(Hero governor, Settlement settlement);

		// Token: 0x06002FFB RID: 12283
		void OnInfluenceSpent(Hero hero, float amountSpent);

		// Token: 0x06002FFC RID: 12284
		void OnGainRelation(Hero hero, Hero gainedRelationWith, float relationChange, ChangeRelationAction.ChangeRelationDetail detail = ChangeRelationAction.ChangeRelationDetail.Default);

		// Token: 0x06002FFD RID: 12285
		void OnTroopRecruited(Hero hero, int amount, int tier);

		// Token: 0x06002FFE RID: 12286
		void OnBribeGiven(int amount);

		// Token: 0x06002FFF RID: 12287
		void OnBanditsRecruited(MobileParty mobileParty, CharacterObject bandit, int count);

		// Token: 0x06003000 RID: 12288
		void OnMainHeroReleasedFromCaptivity(float captivityTime);

		// Token: 0x06003001 RID: 12289
		void OnMainHeroTortured();

		// Token: 0x06003002 RID: 12290
		void OnMainHeroDisguised(bool isNotCaught);

		// Token: 0x06003003 RID: 12291
		void OnRaid(MobileParty attackerParty, ItemRoster lootedItems);

		// Token: 0x06003004 RID: 12292
		void OnLoot(MobileParty attackerParty, MobileParty forcedParty, ItemRoster lootedItems, bool attacked);

		// Token: 0x06003005 RID: 12293
		void OnPrisonerSell(MobileParty mobileParty, float count);

		// Token: 0x06003006 RID: 12294
		void OnSurgeryApplied(MobileParty party, bool surgerySuccess, int troopTier);

		// Token: 0x06003007 RID: 12295
		void OnTacticsUsed(MobileParty party, float xp);

		// Token: 0x06003008 RID: 12296
		void OnHideoutSpotted(MobileParty party, PartyBase spottedParty);

		// Token: 0x06003009 RID: 12297
		void OnTrackDetected(Track track);

		// Token: 0x0600300A RID: 12298
		void OnTravelOnFoot(Hero hero, float speed);

		// Token: 0x0600300B RID: 12299
		void OnTravelOnHorse(Hero hero, float speed);

		// Token: 0x0600300C RID: 12300
		void OnHeroHealedWhileWaiting(Hero hero, int healingAmount);

		// Token: 0x0600300D RID: 12301
		void OnRegularTroopHealedWhileWaiting(MobileParty mobileParty, int healedTroopCount, float averageTier);

		// Token: 0x0600300E RID: 12302
		void OnLeadingArmy(MobileParty mobileParty);

		// Token: 0x0600300F RID: 12303
		void OnSieging(MobileParty mobileParty);

		// Token: 0x06003010 RID: 12304
		void OnSiegeEngineBuilt(MobileParty mobileParty, SiegeEngineType siegeEngine);

		// Token: 0x06003011 RID: 12305
		void OnUpgradeTroops(PartyBase party, CharacterObject troop, CharacterObject upgrade, int numberOfTroops);

		// Token: 0x06003012 RID: 12306
		void OnPersuasionSucceeded(Hero targetHero, SkillObject skill, PersuasionDifficulty difficulty, int argumentDifficultyBonusCoefficient);

		// Token: 0x06003013 RID: 12307
		void OnPrisonBreakEnd(Hero prisonerHero, bool isSucceeded);

		// Token: 0x06003014 RID: 12308
		void OnWallBreached(MobileParty party);

		// Token: 0x06003015 RID: 12309
		void OnForceVolunteers(MobileParty attackerParty, PartyBase forcedParty);

		// Token: 0x06003016 RID: 12310
		void OnForceSupplies(MobileParty attackerParty, ItemRoster lootedItems, bool attacked);

		// Token: 0x06003017 RID: 12311
		void OnAIPartiesTravel(Hero hero, bool isCaravanParty, TerrainType currentTerrainType);

		// Token: 0x06003018 RID: 12312
		void OnTraverseTerrain(MobileParty mobileParty, TerrainType currentTerrainType);

		// Token: 0x06003019 RID: 12313
		void OnBattleEnd(PartyBase party, FlattenedTroopRoster flattenedTroopRoster);

		// Token: 0x0600301A RID: 12314
		void OnFoodConsumed(MobileParty mobileParty, bool wasStarving);

		// Token: 0x0600301B RID: 12315
		void OnAlleyCleared(Alley alley);

		// Token: 0x0600301C RID: 12316
		void OnDailyAlleyTick(Alley alley, Hero alleyLeader);

		// Token: 0x0600301D RID: 12317
		void OnBoardGameWonAgainstLord(Hero lord, BoardGameHelper.AIDifficulty difficulty, bool extraXpGain);
	}
}
