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
	public static class SkillLevelingManager
	{
		private static ISkillLevelingManager Instance
		{
			get
			{
				return Campaign.Current.SkillLevelingManager;
			}
		}

		public static void OnCombatHit(CharacterObject affectorCharacter, CharacterObject affectedCharacter, CharacterObject captain, Hero commander, float speedBonusFromMovement, float shotDifficulty, WeaponComponentData affectorWeapon, float hitPointRatio, CombatXpModel.MissionTypeEnum missionType, bool isAffectorMounted, bool isTeamKill, bool isAffectorUnderCommand, float damageAmount, bool isFatal, bool isSiegeEngineHit, bool isHorseCharge)
		{
			SkillLevelingManager.Instance.OnCombatHit(affectorCharacter, affectedCharacter, captain, commander, speedBonusFromMovement, shotDifficulty, affectorWeapon, hitPointRatio, missionType, isAffectorMounted, isTeamKill, isAffectorUnderCommand, damageAmount, isFatal, isSiegeEngineHit, isHorseCharge);
		}

		public static void OnSiegeEngineDestroyed(MobileParty party, SiegeEngineType destroyedSiegeEngine)
		{
			SkillLevelingManager.Instance.OnSiegeEngineDestroyed(party, destroyedSiegeEngine);
		}

		public static void OnWallBreached(MobileParty party)
		{
			SkillLevelingManager.Instance.OnWallBreached(party);
		}

		public static void OnSimulationCombatKill(CharacterObject affectorCharacter, CharacterObject affectedCharacter, PartyBase affectorParty, PartyBase commanderParty)
		{
			SkillLevelingManager.Instance.OnSimulationCombatKill(affectorCharacter, affectedCharacter, affectorParty, commanderParty);
		}

		public static void OnTradeProfitMade(PartyBase party, int tradeProfit)
		{
			SkillLevelingManager.Instance.OnTradeProfitMade(party, tradeProfit);
		}

		public static void OnTradeProfitMade(Hero hero, int tradeProfit)
		{
			SkillLevelingManager.Instance.OnTradeProfitMade(hero, tradeProfit);
		}

		public static void OnSettlementProjectFinished(Settlement settlement)
		{
			SkillLevelingManager.Instance.OnSettlementProjectFinished(settlement);
		}

		public static void OnSettlementGoverned(Hero governor, Settlement settlement)
		{
			SkillLevelingManager.Instance.OnSettlementGoverned(governor, settlement);
		}

		public static void OnInfluenceSpent(Hero hero, float amountSpent)
		{
			SkillLevelingManager.Instance.OnInfluenceSpent(hero, amountSpent);
		}

		public static void OnGainRelation(Hero hero, Hero gainedRelationWith, float relationChange, ChangeRelationAction.ChangeRelationDetail detail = ChangeRelationAction.ChangeRelationDetail.Default)
		{
			SkillLevelingManager.Instance.OnGainRelation(hero, gainedRelationWith, relationChange, detail);
		}

		public static void OnTroopRecruited(Hero hero, int amount, int tier)
		{
			SkillLevelingManager.Instance.OnTroopRecruited(hero, amount, tier);
		}

		public static void OnBribeGiven(int amount)
		{
			SkillLevelingManager.Instance.OnBribeGiven(amount);
		}

		public static void OnBanditsRecruited(MobileParty mobileParty, CharacterObject bandit, int count)
		{
			SkillLevelingManager.Instance.OnBanditsRecruited(mobileParty, bandit, count);
		}

		public static void OnMainHeroReleasedFromCaptivity(float captivityTime)
		{
			SkillLevelingManager.Instance.OnMainHeroReleasedFromCaptivity(captivityTime);
		}

		public static void OnMainHeroTortured()
		{
			SkillLevelingManager.Instance.OnMainHeroTortured();
		}

		public static void OnMainHeroDisguised(bool isNotCaught)
		{
			SkillLevelingManager.Instance.OnMainHeroDisguised(isNotCaught);
		}

		public static void OnRaid(MobileParty attackerParty, ItemRoster lootedItems)
		{
			SkillLevelingManager.Instance.OnRaid(attackerParty, lootedItems);
		}

		public static void OnLoot(MobileParty attackerParty, MobileParty forcedParty, ItemRoster lootedItems, bool attacked)
		{
			SkillLevelingManager.Instance.OnLoot(attackerParty, forcedParty, lootedItems, attacked);
		}

		public static void OnForceVolunteers(MobileParty attackerParty, PartyBase forcedParty)
		{
			SkillLevelingManager.Instance.OnForceVolunteers(attackerParty, forcedParty);
		}

		public static void OnForceSupplies(MobileParty attackerParty, ItemRoster lootedItems, bool attacked)
		{
			SkillLevelingManager.Instance.OnForceSupplies(attackerParty, lootedItems, attacked);
		}

		public static void OnPrisonerSell(MobileParty mobileParty, in TroopRoster prisonerRoster)
		{
			SkillLevelingManager.Instance.OnPrisonerSell(mobileParty, prisonerRoster);
		}

		public static void OnSurgeryApplied(MobileParty party, bool surgerySuccess, int troopTier)
		{
			SkillLevelingManager.Instance.OnSurgeryApplied(party, surgerySuccess, troopTier);
		}

		public static void OnTacticsUsed(MobileParty party, float xp)
		{
			SkillLevelingManager.Instance.OnTacticsUsed(party, xp);
		}

		public static void OnHideoutSpotted(MobileParty party, PartyBase spottedParty)
		{
			SkillLevelingManager.Instance.OnHideoutSpotted(party, spottedParty);
		}

		public static void OnTrackDetected(Track track)
		{
			SkillLevelingManager.Instance.OnTrackDetected(track);
		}

		public static void OnTravelOnFoot(Hero hero, float speed)
		{
			SkillLevelingManager.Instance.OnTravelOnFoot(hero, speed);
		}

		public static void OnTravelOnHorse(Hero hero, float speed)
		{
			SkillLevelingManager.Instance.OnTravelOnHorse(hero, speed);
		}

		public static void OnAIPartiesTravel(Hero hero, bool isCaravanParty, TerrainType currentTerrainType)
		{
			SkillLevelingManager.Instance.OnAIPartiesTravel(hero, isCaravanParty, currentTerrainType);
		}

		public static void OnTraverseTerrain(MobileParty mobileParty, TerrainType currentTerrainType)
		{
			SkillLevelingManager.Instance.OnTraverseTerrain(mobileParty, currentTerrainType);
		}

		public static void OnHeroHealedWhileWaiting(Hero hero, int healingAmount)
		{
			SkillLevelingManager.Instance.OnHeroHealedWhileWaiting(hero, healingAmount);
		}

		public static void OnRegularTroopHealedWhileWaiting(MobileParty mobileParty, int healedTroopCount, float averageTier)
		{
			SkillLevelingManager.Instance.OnRegularTroopHealedWhileWaiting(mobileParty, healedTroopCount, averageTier);
		}

		public static void OnLeadingArmy(MobileParty mobileParty)
		{
			SkillLevelingManager.Instance.OnLeadingArmy(mobileParty);
		}

		public static void OnSieging(MobileParty mobileParty)
		{
			SkillLevelingManager.Instance.OnSieging(mobileParty);
		}

		public static void OnSiegeEngineBuilt(MobileParty mobileParty, SiegeEngineType siegeEngine)
		{
			SkillLevelingManager.Instance.OnSiegeEngineBuilt(mobileParty, siegeEngine);
		}

		public static void OnUpgradeTroops(PartyBase party, CharacterObject troop, CharacterObject upgrade, int numberOfTroops)
		{
			SkillLevelingManager.Instance.OnUpgradeTroops(party, troop, upgrade, numberOfTroops);
		}

		public static void OnBattleEnd(PartyBase party, FlattenedTroopRoster flattenedTroopRoster)
		{
			SkillLevelingManager.Instance.OnBattleEnd(party, flattenedTroopRoster);
		}

		public static void OnPersuasionSucceeded(Hero targetHero, SkillObject skill, PersuasionDifficulty difficulty, int argumentDifficultyBonusCoefficient)
		{
			SkillLevelingManager.Instance.OnPersuasionSucceeded(targetHero, skill, difficulty, argumentDifficultyBonusCoefficient);
		}

		public static void OnPrisonBreakEnd(Hero prisonerHero, bool isSucceeded)
		{
			SkillLevelingManager.Instance.OnPrisonBreakEnd(prisonerHero, isSucceeded);
		}

		public static void OnFoodConsumed(MobileParty mobileParty, bool wasStarving)
		{
			SkillLevelingManager.Instance.OnFoodConsumed(mobileParty, wasStarving);
		}

		public static void OnAlleyCleared(Alley alley)
		{
			SkillLevelingManager.Instance.OnAlleyCleared(alley);
		}

		public static void OnDailyAlleyTick(Alley alley, Hero alleyLeader)
		{
			SkillLevelingManager.Instance.OnDailyAlleyTick(alley, alleyLeader);
		}

		public static void OnBoardGameWonAgainstLord(Hero lord, BoardGameHelper.AIDifficulty difficulty, bool extraXpGain)
		{
			SkillLevelingManager.Instance.OnBoardGameWonAgainstLord(lord, difficulty, extraXpGain);
		}

		public static void OnProductionProducedToWarehouse(EquipmentElement production)
		{
			SkillLevelingManager.Instance.OnWarehouseProduction(production);
		}
	}
}
