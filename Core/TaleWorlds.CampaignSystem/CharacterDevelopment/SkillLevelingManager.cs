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
	// Token: 0x02000355 RID: 853
	public static class SkillLevelingManager
	{
		// Token: 0x17000B8C RID: 2956
		// (get) Token: 0x0600301E RID: 12318 RVA: 0x000CB3C9 File Offset: 0x000C95C9
		private static ISkillLevelingManager Instance
		{
			get
			{
				return Campaign.Current.SkillLevelingManager;
			}
		}

		// Token: 0x0600301F RID: 12319 RVA: 0x000CB3D8 File Offset: 0x000C95D8
		public static void OnCombatHit(CharacterObject affectorCharacter, CharacterObject affectedCharacter, CharacterObject captain, Hero commander, float speedBonusFromMovement, float shotDifficulty, WeaponComponentData affectorWeapon, float hitPointRatio, CombatXpModel.MissionTypeEnum missionType, bool isAffectorMounted, bool isTeamKill, bool isAffectorUnderCommand, float damageAmount, bool isFatal, bool isSiegeEngineHit, bool isHorseCharge)
		{
			SkillLevelingManager.Instance.OnCombatHit(affectorCharacter, affectedCharacter, captain, commander, speedBonusFromMovement, shotDifficulty, affectorWeapon, hitPointRatio, missionType, isAffectorMounted, isTeamKill, isAffectorUnderCommand, damageAmount, isFatal, isSiegeEngineHit, isHorseCharge);
		}

		// Token: 0x06003020 RID: 12320 RVA: 0x000CB40B File Offset: 0x000C960B
		public static void OnSiegeEngineDestroyed(MobileParty party, SiegeEngineType destroyedSiegeEngine)
		{
			SkillLevelingManager.Instance.OnSiegeEngineDestroyed(party, destroyedSiegeEngine);
		}

		// Token: 0x06003021 RID: 12321 RVA: 0x000CB419 File Offset: 0x000C9619
		public static void OnWallBreached(MobileParty party)
		{
			SkillLevelingManager.Instance.OnWallBreached(party);
		}

		// Token: 0x06003022 RID: 12322 RVA: 0x000CB426 File Offset: 0x000C9626
		public static void OnSimulationCombatKill(CharacterObject affectorCharacter, CharacterObject affectedCharacter, PartyBase affectorParty, PartyBase commanderParty)
		{
			SkillLevelingManager.Instance.OnSimulationCombatKill(affectorCharacter, affectedCharacter, affectorParty, commanderParty);
		}

		// Token: 0x06003023 RID: 12323 RVA: 0x000CB436 File Offset: 0x000C9636
		public static void OnTradeProfitMade(PartyBase party, int tradeProfit)
		{
			SkillLevelingManager.Instance.OnTradeProfitMade(party, tradeProfit);
		}

		// Token: 0x06003024 RID: 12324 RVA: 0x000CB444 File Offset: 0x000C9644
		public static void OnTradeProfitMade(Hero hero, int tradeProfit)
		{
			SkillLevelingManager.Instance.OnTradeProfitMade(hero, tradeProfit);
		}

		// Token: 0x06003025 RID: 12325 RVA: 0x000CB452 File Offset: 0x000C9652
		public static void OnSettlementProjectFinished(Settlement settlement)
		{
			SkillLevelingManager.Instance.OnSettlementProjectFinished(settlement);
		}

		// Token: 0x06003026 RID: 12326 RVA: 0x000CB45F File Offset: 0x000C965F
		public static void OnSettlementGoverned(Hero governor, Settlement settlement)
		{
			SkillLevelingManager.Instance.OnSettlementGoverned(governor, settlement);
		}

		// Token: 0x06003027 RID: 12327 RVA: 0x000CB46D File Offset: 0x000C966D
		public static void OnInfluenceSpent(Hero hero, float amountSpent)
		{
			SkillLevelingManager.Instance.OnInfluenceSpent(hero, amountSpent);
		}

		// Token: 0x06003028 RID: 12328 RVA: 0x000CB47B File Offset: 0x000C967B
		public static void OnGainRelation(Hero hero, Hero gainedRelationWith, float relationChange, ChangeRelationAction.ChangeRelationDetail detail = ChangeRelationAction.ChangeRelationDetail.Default)
		{
			SkillLevelingManager.Instance.OnGainRelation(hero, gainedRelationWith, relationChange, detail);
		}

		// Token: 0x06003029 RID: 12329 RVA: 0x000CB48B File Offset: 0x000C968B
		public static void OnTroopRecruited(Hero hero, int amount, int tier)
		{
			SkillLevelingManager.Instance.OnTroopRecruited(hero, amount, tier);
		}

		// Token: 0x0600302A RID: 12330 RVA: 0x000CB49A File Offset: 0x000C969A
		public static void OnBribeGiven(int amount)
		{
			SkillLevelingManager.Instance.OnBribeGiven(amount);
		}

		// Token: 0x0600302B RID: 12331 RVA: 0x000CB4A7 File Offset: 0x000C96A7
		public static void OnBanditsRecruited(MobileParty mobileParty, CharacterObject bandit, int count)
		{
			SkillLevelingManager.Instance.OnBanditsRecruited(mobileParty, bandit, count);
		}

		// Token: 0x0600302C RID: 12332 RVA: 0x000CB4B6 File Offset: 0x000C96B6
		public static void OnMainHeroReleasedFromCaptivity(float captivityTime)
		{
			SkillLevelingManager.Instance.OnMainHeroReleasedFromCaptivity(captivityTime);
		}

		// Token: 0x0600302D RID: 12333 RVA: 0x000CB4C3 File Offset: 0x000C96C3
		public static void OnMainHeroTortured()
		{
			SkillLevelingManager.Instance.OnMainHeroTortured();
		}

		// Token: 0x0600302E RID: 12334 RVA: 0x000CB4CF File Offset: 0x000C96CF
		public static void OnMainHeroDisguised(bool isNotCaught)
		{
			SkillLevelingManager.Instance.OnMainHeroDisguised(isNotCaught);
		}

		// Token: 0x0600302F RID: 12335 RVA: 0x000CB4DC File Offset: 0x000C96DC
		public static void OnRaid(MobileParty attackerParty, ItemRoster lootedItems)
		{
			SkillLevelingManager.Instance.OnRaid(attackerParty, lootedItems);
		}

		// Token: 0x06003030 RID: 12336 RVA: 0x000CB4EA File Offset: 0x000C96EA
		public static void OnLoot(MobileParty attackerParty, MobileParty forcedParty, ItemRoster lootedItems, bool attacked)
		{
			SkillLevelingManager.Instance.OnLoot(attackerParty, forcedParty, lootedItems, attacked);
		}

		// Token: 0x06003031 RID: 12337 RVA: 0x000CB4FA File Offset: 0x000C96FA
		public static void OnForceVolunteers(MobileParty attackerParty, PartyBase forcedParty)
		{
			SkillLevelingManager.Instance.OnForceVolunteers(attackerParty, forcedParty);
		}

		// Token: 0x06003032 RID: 12338 RVA: 0x000CB508 File Offset: 0x000C9708
		public static void OnForceSupplies(MobileParty attackerParty, ItemRoster lootedItems, bool attacked)
		{
			SkillLevelingManager.Instance.OnForceSupplies(attackerParty, lootedItems, attacked);
		}

		// Token: 0x06003033 RID: 12339 RVA: 0x000CB517 File Offset: 0x000C9717
		public static void OnPrisonerSell(MobileParty mobileParty, float count)
		{
			SkillLevelingManager.Instance.OnPrisonerSell(mobileParty, count);
		}

		// Token: 0x06003034 RID: 12340 RVA: 0x000CB525 File Offset: 0x000C9725
		public static void OnSurgeryApplied(MobileParty party, bool surgerySuccess, int troopTier)
		{
			SkillLevelingManager.Instance.OnSurgeryApplied(party, surgerySuccess, troopTier);
		}

		// Token: 0x06003035 RID: 12341 RVA: 0x000CB534 File Offset: 0x000C9734
		public static void OnTacticsUsed(MobileParty party, float xp)
		{
			SkillLevelingManager.Instance.OnTacticsUsed(party, xp);
		}

		// Token: 0x06003036 RID: 12342 RVA: 0x000CB542 File Offset: 0x000C9742
		public static void OnHideoutSpotted(MobileParty party, PartyBase spottedParty)
		{
			SkillLevelingManager.Instance.OnHideoutSpotted(party, spottedParty);
		}

		// Token: 0x06003037 RID: 12343 RVA: 0x000CB550 File Offset: 0x000C9750
		public static void OnTrackDetected(Track track)
		{
			SkillLevelingManager.Instance.OnTrackDetected(track);
		}

		// Token: 0x06003038 RID: 12344 RVA: 0x000CB55D File Offset: 0x000C975D
		public static void OnTravelOnFoot(Hero hero, float speed)
		{
			SkillLevelingManager.Instance.OnTravelOnFoot(hero, speed);
		}

		// Token: 0x06003039 RID: 12345 RVA: 0x000CB56B File Offset: 0x000C976B
		public static void OnTravelOnHorse(Hero hero, float speed)
		{
			SkillLevelingManager.Instance.OnTravelOnHorse(hero, speed);
		}

		// Token: 0x0600303A RID: 12346 RVA: 0x000CB579 File Offset: 0x000C9779
		public static void OnAIPartiesTravel(Hero hero, bool isCaravanParty, TerrainType currentTerrainType)
		{
			SkillLevelingManager.Instance.OnAIPartiesTravel(hero, isCaravanParty, currentTerrainType);
		}

		// Token: 0x0600303B RID: 12347 RVA: 0x000CB588 File Offset: 0x000C9788
		public static void OnTraverseTerrain(MobileParty mobileParty, TerrainType currentTerrainType)
		{
			SkillLevelingManager.Instance.OnTraverseTerrain(mobileParty, currentTerrainType);
		}

		// Token: 0x0600303C RID: 12348 RVA: 0x000CB596 File Offset: 0x000C9796
		public static void OnHeroHealedWhileWaiting(Hero hero, int healingAmount)
		{
			SkillLevelingManager.Instance.OnHeroHealedWhileWaiting(hero, healingAmount);
		}

		// Token: 0x0600303D RID: 12349 RVA: 0x000CB5A4 File Offset: 0x000C97A4
		public static void OnRegularTroopHealedWhileWaiting(MobileParty mobileParty, int healedTroopCount, float averageTier)
		{
			SkillLevelingManager.Instance.OnRegularTroopHealedWhileWaiting(mobileParty, healedTroopCount, averageTier);
		}

		// Token: 0x0600303E RID: 12350 RVA: 0x000CB5B3 File Offset: 0x000C97B3
		public static void OnLeadingArmy(MobileParty mobileParty)
		{
			SkillLevelingManager.Instance.OnLeadingArmy(mobileParty);
		}

		// Token: 0x0600303F RID: 12351 RVA: 0x000CB5C0 File Offset: 0x000C97C0
		public static void OnSieging(MobileParty mobileParty)
		{
			SkillLevelingManager.Instance.OnSieging(mobileParty);
		}

		// Token: 0x06003040 RID: 12352 RVA: 0x000CB5CD File Offset: 0x000C97CD
		public static void OnSiegeEngineBuilt(MobileParty mobileParty, SiegeEngineType siegeEngine)
		{
			SkillLevelingManager.Instance.OnSiegeEngineBuilt(mobileParty, siegeEngine);
		}

		// Token: 0x06003041 RID: 12353 RVA: 0x000CB5DB File Offset: 0x000C97DB
		public static void OnUpgradeTroops(PartyBase party, CharacterObject troop, CharacterObject upgrade, int numberOfTroops)
		{
			SkillLevelingManager.Instance.OnUpgradeTroops(party, troop, upgrade, numberOfTroops);
		}

		// Token: 0x06003042 RID: 12354 RVA: 0x000CB5EB File Offset: 0x000C97EB
		public static void OnBattleEnd(PartyBase party, FlattenedTroopRoster flattenedTroopRoster)
		{
			SkillLevelingManager.Instance.OnBattleEnd(party, flattenedTroopRoster);
		}

		// Token: 0x06003043 RID: 12355 RVA: 0x000CB5F9 File Offset: 0x000C97F9
		public static void OnPersuasionSucceeded(Hero targetHero, SkillObject skill, PersuasionDifficulty difficulty, int argumentDifficultyBonusCoefficient)
		{
			SkillLevelingManager.Instance.OnPersuasionSucceeded(targetHero, skill, difficulty, argumentDifficultyBonusCoefficient);
		}

		// Token: 0x06003044 RID: 12356 RVA: 0x000CB609 File Offset: 0x000C9809
		public static void OnPrisonBreakEnd(Hero prisonerHero, bool isSucceeded)
		{
			SkillLevelingManager.Instance.OnPrisonBreakEnd(prisonerHero, isSucceeded);
		}

		// Token: 0x06003045 RID: 12357 RVA: 0x000CB617 File Offset: 0x000C9817
		public static void OnFoodConsumed(MobileParty mobileParty, bool wasStarving)
		{
			SkillLevelingManager.Instance.OnFoodConsumed(mobileParty, wasStarving);
		}

		// Token: 0x06003046 RID: 12358 RVA: 0x000CB625 File Offset: 0x000C9825
		public static void OnAlleyCleared(Alley alley)
		{
			SkillLevelingManager.Instance.OnAlleyCleared(alley);
		}

		// Token: 0x06003047 RID: 12359 RVA: 0x000CB632 File Offset: 0x000C9832
		public static void OnDailyAlleyTick(Alley alley, Hero alleyLeader)
		{
			SkillLevelingManager.Instance.OnDailyAlleyTick(alley, alleyLeader);
		}

		// Token: 0x06003048 RID: 12360 RVA: 0x000CB640 File Offset: 0x000C9840
		public static void OnBoardGameWonAgainstLord(Hero lord, BoardGameHelper.AIDifficulty difficulty, bool extraXpGain)
		{
			SkillLevelingManager.Instance.OnBoardGameWonAgainstLord(lord, difficulty, extraXpGain);
		}
	}
}
