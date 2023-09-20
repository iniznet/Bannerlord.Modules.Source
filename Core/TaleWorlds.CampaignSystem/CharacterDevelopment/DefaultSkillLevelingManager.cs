using System;
using System.Collections.Generic;
using Helpers;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Conversation.Persuasion;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.CharacterDevelopment
{
	public class DefaultSkillLevelingManager : ISkillLevelingManager
	{
		public void OnCombatHit(CharacterObject affectorCharacter, CharacterObject affectedCharacter, CharacterObject captain, Hero commander, float speedBonusFromMovement, float shotDifficulty, WeaponComponentData affectorWeapon, float hitPointRatio, CombatXpModel.MissionTypeEnum missionType, bool isAffectorMounted, bool isTeamKill, bool isAffectorUnderCommand, float damageAmount, bool isFatal, bool isSiegeEngineHit, bool isHorseCharge)
		{
			if (isTeamKill)
			{
				return;
			}
			float num = 1f;
			if (affectorCharacter.IsHero)
			{
				Hero heroObject = affectorCharacter.HeroObject;
				CombatXpModel combatXpModel = Campaign.Current.Models.CombatXpModel;
				CharacterObject characterObject = heroObject.CharacterObject;
				MobileParty partyBelongedTo = heroObject.PartyBelongedTo;
				int num2;
				combatXpModel.GetXpFromHit(characterObject, captain, affectedCharacter, (partyBelongedTo != null) ? partyBelongedTo.Party : null, (int)damageAmount, isFatal, missionType, out num2);
				num = (float)num2;
				SkillObject skillObject;
				if (affectorWeapon != null)
				{
					skillObject = Campaign.Current.Models.CombatXpModel.GetSkillForWeapon(affectorWeapon, isSiegeEngineHit);
					float num3 = ((skillObject == DefaultSkills.Bow) ? 0.5f : 1f);
					if (shotDifficulty > 0f)
					{
						num += (float)MathF.Floor(num * num3 * Campaign.Current.Models.CombatXpModel.GetXpMultiplierFromShotDifficulty(shotDifficulty));
					}
				}
				else
				{
					skillObject = (isHorseCharge ? DefaultSkills.Riding : DefaultSkills.Athletics);
				}
				heroObject.AddSkillXp(skillObject, (float)MBRandom.RoundRandomized(num));
				if (!isSiegeEngineHit && !isHorseCharge)
				{
					float num4 = shotDifficulty * 0.15f;
					if (isAffectorMounted)
					{
						float num5 = 0.5f;
						if (num4 > 0f)
						{
							num5 += num4;
						}
						if (speedBonusFromMovement > 0f)
						{
							num5 *= 1f + speedBonusFromMovement;
						}
						if (num5 > 0f)
						{
							DefaultSkillLevelingManager.OnGainingRidingExperience(heroObject, (float)MBRandom.RoundRandomized(num5 * num), heroObject.CharacterObject.Equipment.Horse.Item);
						}
					}
					else
					{
						float num6 = 1f;
						if (num4 > 0f)
						{
							num6 += num4;
						}
						if (speedBonusFromMovement > 0f)
						{
							num6 += 1.5f * speedBonusFromMovement;
						}
						if (num6 > 0f)
						{
							heroObject.AddSkillXp(DefaultSkills.Athletics, (float)MBRandom.RoundRandomized(num6 * num));
						}
					}
				}
			}
			if (commander != null && commander != affectorCharacter.HeroObject && commander.PartyBelongedTo != null)
			{
				this.OnTacticsUsed(commander.PartyBelongedTo, (float)MathF.Ceiling(0.02f * num));
			}
		}

		public void OnSiegeEngineDestroyed(MobileParty party, SiegeEngineType destroyedSiegeEngine)
		{
			if (((party != null) ? party.EffectiveEngineer : null) != null)
			{
				float num = (float)destroyedSiegeEngine.ManDayCost * 20f;
				DefaultSkillLevelingManager.OnPartySkillExercised(party, DefaultSkills.Engineering, num, SkillEffect.PerkRole.Engineer);
			}
		}

		public void OnSimulationCombatKill(CharacterObject affectorCharacter, CharacterObject affectedCharacter, PartyBase affectorParty, PartyBase commanderParty)
		{
			int xpReward = Campaign.Current.Models.PartyTrainingModel.GetXpReward(affectedCharacter);
			if (affectorCharacter.IsHero)
			{
				ItemObject defaultWeapon = CharacterHelper.GetDefaultWeapon(affectorCharacter);
				Hero heroObject = affectorCharacter.HeroObject;
				if (defaultWeapon != null)
				{
					SkillObject skillForWeapon = Campaign.Current.Models.CombatXpModel.GetSkillForWeapon(defaultWeapon.GetWeaponWithUsageIndex(0), false);
					heroObject.AddSkillXp(skillForWeapon, (float)xpReward);
				}
				if (affectorCharacter.IsMounted)
				{
					float num = (float)xpReward * 0.3f;
					DefaultSkillLevelingManager.OnGainingRidingExperience(heroObject, (float)MBRandom.RoundRandomized(num), heroObject.CharacterObject.Equipment.Horse.Item);
				}
				else
				{
					float num2 = (float)xpReward * 0.3f;
					heroObject.AddSkillXp(DefaultSkills.Athletics, (float)MBRandom.RoundRandomized(num2));
				}
			}
			if (commanderParty != null && commanderParty.IsMobile && commanderParty.LeaderHero != null && commanderParty.LeaderHero != affectedCharacter.HeroObject)
			{
				this.OnTacticsUsed(commanderParty.MobileParty, (float)MathF.Ceiling(0.02f * (float)xpReward));
			}
		}

		public void OnTradeProfitMade(PartyBase party, int tradeProfit)
		{
			if (tradeProfit > 0)
			{
				float num = (float)tradeProfit * 0.5f;
				DefaultSkillLevelingManager.OnPartySkillExercised(party.MobileParty, DefaultSkills.Trade, num, SkillEffect.PerkRole.PartyLeader);
			}
		}

		public void OnTradeProfitMade(Hero hero, int tradeProfit)
		{
			if (tradeProfit > 0)
			{
				float num = (float)tradeProfit * 0.5f;
				DefaultSkillLevelingManager.OnPersonalSkillExercised(hero, DefaultSkills.Trade, num, hero == Hero.MainHero);
			}
		}

		public void OnSettlementProjectFinished(Settlement settlement)
		{
			DefaultSkillLevelingManager.OnSettlementSkillExercised(settlement, DefaultSkills.Steward, 1000f);
		}

		public void OnSettlementGoverned(Hero governor, Settlement settlement)
		{
			float prosperityChange = settlement.Town.ProsperityChange;
			if (prosperityChange > 0f)
			{
				float num = prosperityChange * 30f;
				DefaultSkillLevelingManager.OnPersonalSkillExercised(governor, DefaultSkills.Steward, num, true);
			}
		}

		public void OnInfluenceSpent(Hero hero, float amountSpent)
		{
			if (hero.PartyBelongedTo != null)
			{
				float num = 10f * amountSpent;
				DefaultSkillLevelingManager.OnPartySkillExercised(hero.PartyBelongedTo, DefaultSkills.Steward, num, SkillEffect.PerkRole.PartyLeader);
			}
		}

		public void OnGainRelation(Hero hero, Hero gainedRelationWith, float relationChange, ChangeRelationAction.ChangeRelationDetail detail = ChangeRelationAction.ChangeRelationDetail.Default)
		{
			if ((hero.PartyBelongedTo == null && detail != ChangeRelationAction.ChangeRelationDetail.Emissary) || relationChange <= 0f)
			{
				return;
			}
			int charmExperienceFromRelationGain = Campaign.Current.Models.DiplomacyModel.GetCharmExperienceFromRelationGain(gainedRelationWith, relationChange, detail);
			if (hero.PartyBelongedTo != null)
			{
				DefaultSkillLevelingManager.OnPartySkillExercised(hero.PartyBelongedTo, DefaultSkills.Charm, (float)charmExperienceFromRelationGain, SkillEffect.PerkRole.PartyLeader);
				return;
			}
			DefaultSkillLevelingManager.OnPersonalSkillExercised(hero, DefaultSkills.Charm, (float)charmExperienceFromRelationGain, true);
		}

		public void OnTroopRecruited(Hero hero, int amount, int tier)
		{
			if (amount > 0)
			{
				int num = amount * tier * 2;
				DefaultSkillLevelingManager.OnPersonalSkillExercised(hero, DefaultSkills.Leadership, (float)num, true);
			}
		}

		public void OnBribeGiven(int amount)
		{
			if (amount > 0)
			{
				float num = (float)amount * 0.1f;
				DefaultSkillLevelingManager.OnPartySkillExercised(MobileParty.MainParty, DefaultSkills.Roguery, num, SkillEffect.PerkRole.PartyLeader);
			}
		}

		public void OnBanditsRecruited(MobileParty mobileParty, CharacterObject bandit, int count)
		{
			if (count > 0)
			{
				DefaultSkillLevelingManager.OnPersonalSkillExercised(mobileParty.LeaderHero, DefaultSkills.Roguery, (float)(count * 2 * bandit.Tier), true);
			}
		}

		public void OnMainHeroReleasedFromCaptivity(float captivityTime)
		{
			float num = captivityTime * 0.5f;
			DefaultSkillLevelingManager.OnPersonalSkillExercised(Hero.MainHero, DefaultSkills.Roguery, num, true);
		}

		public void OnMainHeroTortured()
		{
			float num = MBRandom.RandomFloatRanged(50f, 100f);
			DefaultSkillLevelingManager.OnPersonalSkillExercised(Hero.MainHero, DefaultSkills.Roguery, num, true);
		}

		public void OnMainHeroDisguised(bool isNotCaught)
		{
			float num = (isNotCaught ? MBRandom.RandomFloatRanged(10f, 25f) : MBRandom.RandomFloatRanged(1f, 10f));
			DefaultSkillLevelingManager.OnPartySkillExercised(MobileParty.MainParty, DefaultSkills.Roguery, num, SkillEffect.PerkRole.PartyLeader);
		}

		public void OnRaid(MobileParty attackerParty, ItemRoster lootedItems)
		{
			if (attackerParty.LeaderHero != null)
			{
				float num = (float)lootedItems.TradeGoodsTotalValue * 0.5f;
				DefaultSkillLevelingManager.OnPersonalSkillExercised(attackerParty.LeaderHero, DefaultSkills.Roguery, num, true);
			}
		}

		public void OnLoot(MobileParty attackerParty, MobileParty forcedParty, ItemRoster lootedItems, bool attacked)
		{
			if (attackerParty.LeaderHero != null)
			{
				float num = 0f;
				if (forcedParty.IsVillager)
				{
					num = (attacked ? 0.75f : 0.5f);
				}
				else if (forcedParty.IsCaravan)
				{
					num = (attacked ? 0.15f : 0.1f);
				}
				float num2 = (float)lootedItems.TradeGoodsTotalValue * num;
				DefaultSkillLevelingManager.OnPersonalSkillExercised(attackerParty.LeaderHero, DefaultSkills.Roguery, num2, true);
			}
		}

		public void OnPrisonerSell(MobileParty mobileParty, float count)
		{
			float num = 6f * count;
			DefaultSkillLevelingManager.OnPartySkillExercised(mobileParty, DefaultSkills.Roguery, num, SkillEffect.PerkRole.PartyLeader);
		}

		public void OnSurgeryApplied(MobileParty party, bool surgerySuccess, int troopTier)
		{
			float num = (float)(surgerySuccess ? (10 * troopTier) : (5 * troopTier));
			DefaultSkillLevelingManager.OnPartySkillExercised(party, DefaultSkills.Medicine, num, SkillEffect.PerkRole.Surgeon);
		}

		public void OnTacticsUsed(MobileParty party, float xp)
		{
			if (xp > 0f)
			{
				DefaultSkillLevelingManager.OnPartySkillExercised(party, DefaultSkills.Tactics, xp, SkillEffect.PerkRole.PartyLeader);
			}
		}

		public void OnHideoutSpotted(MobileParty party, PartyBase spottedParty)
		{
			DefaultSkillLevelingManager.OnPartySkillExercised(party, DefaultSkills.Scouting, 100f, SkillEffect.PerkRole.Scout);
		}

		public void OnTrackDetected(Track track)
		{
			float skillFromTrackDetected = Campaign.Current.Models.MapTrackModel.GetSkillFromTrackDetected(track);
			DefaultSkillLevelingManager.OnPartySkillExercised(MobileParty.MainParty, DefaultSkills.Scouting, skillFromTrackDetected, SkillEffect.PerkRole.Scout);
		}

		public void OnTravelOnFoot(Hero hero, float speed)
		{
			hero.AddSkillXp(DefaultSkills.Athletics, (float)(MBRandom.RoundRandomized(0.2f * speed) + 1));
		}

		public void OnTravelOnHorse(Hero hero, float speed)
		{
			ItemObject item = hero.CharacterObject.Equipment.Horse.Item;
			DefaultSkillLevelingManager.OnGainingRidingExperience(hero, (float)MBRandom.RoundRandomized(0.3f * speed), item);
		}

		public void OnHeroHealedWhileWaiting(Hero hero, int healingAmount)
		{
			if (hero.PartyBelongedTo != null && hero.PartyBelongedTo.EffectiveSurgeon != null)
			{
				float num = (float)Campaign.Current.Models.PartyHealingModel.GetSkillXpFromHealingTroop(hero.PartyBelongedTo.Party);
				float num2 = ((hero.PartyBelongedTo.CurrentSettlement != null && !hero.PartyBelongedTo.CurrentSettlement.IsCastle) ? 0.2f : 0.1f);
				num *= (float)healingAmount * num2 * (1f + (float)hero.PartyBelongedTo.EffectiveSurgeon.Level * 0.1f);
				DefaultSkillLevelingManager.OnPartySkillExercised(hero.PartyBelongedTo, DefaultSkills.Medicine, num, SkillEffect.PerkRole.Surgeon);
			}
		}

		public void OnRegularTroopHealedWhileWaiting(MobileParty mobileParty, int healedTroopCount, float averageTier)
		{
			float num = (float)(Campaign.Current.Models.PartyHealingModel.GetSkillXpFromHealingTroop(mobileParty.Party) * healedTroopCount) * averageTier;
			float num2 = ((mobileParty.CurrentSettlement != null && !mobileParty.CurrentSettlement.IsCastle) ? 2f : 1f);
			num *= num2;
			DefaultSkillLevelingManager.OnPartySkillExercised(mobileParty, DefaultSkills.Medicine, num, SkillEffect.PerkRole.Surgeon);
		}

		public void OnLeadingArmy(MobileParty mobileParty)
		{
			float num = mobileParty.GetTotalStrengthWithFollowers(true) * 0.0004f * mobileParty.Army.Morale;
			DefaultSkillLevelingManager.OnPartySkillExercised(mobileParty, DefaultSkills.Leadership, num, SkillEffect.PerkRole.PartyLeader);
		}

		public void OnSieging(MobileParty mobileParty)
		{
			int num = mobileParty.MemberRoster.TotalManCount;
			if (mobileParty.Army != null && mobileParty.Army.LeaderParty == mobileParty)
			{
				foreach (MobileParty mobileParty2 in mobileParty.Army.Parties)
				{
					if (mobileParty2 != mobileParty)
					{
						num += mobileParty2.MemberRoster.TotalManCount;
					}
				}
			}
			float num2 = 0.25f * MathF.Sqrt((float)num);
			DefaultSkillLevelingManager.OnPartySkillExercised(mobileParty, DefaultSkills.Engineering, num2, SkillEffect.PerkRole.Engineer);
		}

		public void OnSiegeEngineBuilt(MobileParty mobileParty, SiegeEngineType siegeEngine)
		{
			float num = 30f + 2f * (float)siegeEngine.Difficulty;
			DefaultSkillLevelingManager.OnPartySkillExercised(mobileParty, DefaultSkills.Engineering, num, SkillEffect.PerkRole.Engineer);
		}

		public void OnUpgradeTroops(PartyBase party, CharacterObject troop, CharacterObject upgrade, int numberOfTroops)
		{
			Hero hero = party.LeaderHero ?? party.Owner;
			if (hero != null)
			{
				SkillObject skillObject = DefaultSkills.Leadership;
				float num = 0.025f;
				if (troop.Occupation == Occupation.Bandit)
				{
					skillObject = DefaultSkills.Roguery;
					num = 0.05f;
				}
				float num2 = (float)Campaign.Current.Models.PartyTroopUpgradeModel.GetXpCostForUpgrade(party, troop, upgrade) * num * (float)numberOfTroops;
				hero.AddSkillXp(skillObject, num2);
			}
		}

		public void OnPersuasionSucceeded(Hero targetHero, SkillObject skill, PersuasionDifficulty difficulty, int argumentDifficultyBonusCoefficient)
		{
			float num = (float)Campaign.Current.Models.PersuasionModel.GetSkillXpFromPersuasion(difficulty, argumentDifficultyBonusCoefficient);
			if (num > 0f)
			{
				targetHero.AddSkillXp(skill, num);
			}
		}

		public void OnPrisonBreakEnd(Hero prisonerHero, bool isSucceeded)
		{
			float rogueryRewardOnPrisonBreak = Campaign.Current.Models.PrisonBreakModel.GetRogueryRewardOnPrisonBreak(prisonerHero, isSucceeded);
			if (rogueryRewardOnPrisonBreak > 0f)
			{
				Hero.MainHero.AddSkillXp(DefaultSkills.Roguery, rogueryRewardOnPrisonBreak);
			}
		}

		public void OnWallBreached(MobileParty party)
		{
			if (((party != null) ? party.EffectiveEngineer : null) != null)
			{
				DefaultSkillLevelingManager.OnPartySkillExercised(party, DefaultSkills.Engineering, 250f, SkillEffect.PerkRole.Engineer);
			}
		}

		public void OnForceVolunteers(MobileParty attackerParty, PartyBase forcedParty)
		{
			if (attackerParty.LeaderHero != null)
			{
				int num = MathF.Ceiling(forcedParty.Settlement.Village.Hearth / 10f);
				DefaultSkillLevelingManager.OnPersonalSkillExercised(attackerParty.LeaderHero, DefaultSkills.Roguery, (float)num, true);
			}
		}

		public void OnForceSupplies(MobileParty attackerParty, ItemRoster lootedItems, bool attacked)
		{
			if (attackerParty.LeaderHero != null)
			{
				float num = (attacked ? 0.75f : 0.5f);
				float num2 = (float)lootedItems.TradeGoodsTotalValue * num;
				DefaultSkillLevelingManager.OnPersonalSkillExercised(attackerParty.LeaderHero, DefaultSkills.Roguery, num2, true);
			}
		}

		public void OnAIPartiesTravel(Hero hero, bool isCaravanParty, TerrainType currentTerrainType)
		{
			int num = ((currentTerrainType == TerrainType.Forest) ? MBRandom.RoundRandomized(5f) : MBRandom.RoundRandomized(3f));
			hero.AddSkillXp(DefaultSkills.Scouting, isCaravanParty ? ((float)num / 2f) : ((float)num));
		}

		public void OnTraverseTerrain(MobileParty mobileParty, TerrainType currentTerrainType)
		{
			float num = 0f;
			float speed = mobileParty.Speed;
			if (speed > 1f)
			{
				bool flag = currentTerrainType == TerrainType.Desert || currentTerrainType == TerrainType.Dune || currentTerrainType == TerrainType.Forest || currentTerrainType == TerrainType.Snow;
				num = speed * (1f + MathF.Pow((float)mobileParty.MemberRoster.TotalManCount, 0.66f)) * (flag ? 0.25f : 0.15f);
			}
			if (mobileParty.IsCaravan)
			{
				num *= 0.5f;
			}
			if (num >= 5f)
			{
				DefaultSkillLevelingManager.OnPartySkillExercised(mobileParty, DefaultSkills.Scouting, num, SkillEffect.PerkRole.Scout);
			}
		}

		public void OnBattleEnd(PartyBase party, FlattenedTroopRoster flattenedTroopRoster)
		{
			Hero hero = party.LeaderHero ?? party.Owner;
			if (hero != null && hero.IsAlive)
			{
				Dictionary<SkillObject, float> dictionary = new Dictionary<SkillObject, float>();
				foreach (FlattenedTroopRosterElement flattenedTroopRosterElement in flattenedTroopRoster)
				{
					CharacterObject troop = flattenedTroopRosterElement.Troop;
					bool flag = Campaign.Current.Models.PartyTroopUpgradeModel.CanTroopGainXp(party, troop);
					if (!flattenedTroopRosterElement.IsKilled && flattenedTroopRosterElement.XpGained > 0 && !flag)
					{
						float num = ((troop.Occupation == Occupation.Bandit) ? 0.05f : 0.025f);
						float num2 = (float)flattenedTroopRosterElement.XpGained * num;
						SkillObject skillObject = ((troop.Occupation == Occupation.Bandit) ? DefaultSkills.Roguery : DefaultSkills.Leadership);
						float num3;
						if (dictionary.TryGetValue(skillObject, out num3))
						{
							dictionary[skillObject] = num3 + num2;
						}
						else
						{
							dictionary[skillObject] = num2;
						}
					}
				}
				foreach (SkillObject skillObject2 in dictionary.Keys)
				{
					if (dictionary[skillObject2] > 0f)
					{
						hero.AddSkillXp(skillObject2, dictionary[skillObject2]);
					}
				}
			}
		}

		public void OnFoodConsumed(MobileParty mobileParty, bool wasStarving)
		{
			if (!wasStarving && mobileParty.ItemRoster.FoodVariety > 3 && mobileParty.EffectiveQuartermaster != null)
			{
				float num = (float)(MathF.Round(-mobileParty.BaseFoodChange * 100f) * (mobileParty.ItemRoster.FoodVariety - 2) / 3);
				DefaultSkillLevelingManager.OnPartySkillExercised(mobileParty, DefaultSkills.Steward, num, SkillEffect.PerkRole.Quartermaster);
			}
		}

		public void OnAlleyCleared(Alley alley)
		{
			Hero.MainHero.AddSkillXp(DefaultSkills.Roguery, Campaign.Current.Models.AlleyModel.GetInitialXpGainForMainHero());
		}

		public void OnDailyAlleyTick(Alley alley, Hero alleyLeader)
		{
			Hero.MainHero.AddSkillXp(DefaultSkills.Roguery, Campaign.Current.Models.AlleyModel.GetDailyXpGainForMainHero());
			if (alleyLeader != null && !alleyLeader.IsDead)
			{
				alleyLeader.AddSkillXp(DefaultSkills.Roguery, Campaign.Current.Models.AlleyModel.GetDailyXpGainForAssignedClanMember(alleyLeader));
			}
		}

		public void OnBoardGameWonAgainstLord(Hero lord, BoardGameHelper.AIDifficulty difficulty, bool extraXpGain)
		{
			switch (difficulty)
			{
			case BoardGameHelper.AIDifficulty.Easy:
				Hero.MainHero.AddSkillXp(DefaultSkills.Steward, 20f);
				break;
			case BoardGameHelper.AIDifficulty.Normal:
				Hero.MainHero.AddSkillXp(DefaultSkills.Steward, 50f);
				break;
			case BoardGameHelper.AIDifficulty.Hard:
				Hero.MainHero.AddSkillXp(DefaultSkills.Steward, 100f);
				break;
			}
			if (extraXpGain)
			{
				lord.AddSkillXp(DefaultSkills.Steward, 100f);
			}
		}

		private static void OnPersonalSkillExercised(Hero hero, SkillObject skill, float skillXp, bool shouldNotify = true)
		{
			if (hero != null)
			{
				hero.HeroDeveloper.AddSkillXp(skill, skillXp, true, shouldNotify);
			}
		}

		private static void OnSettlementSkillExercised(Settlement settlement, SkillObject skill, float skillXp)
		{
			Town town = settlement.Town;
			Hero hero = ((town != null) ? town.Governor : null) ?? ((settlement.OwnerClan.Leader.CurrentSettlement == settlement) ? settlement.OwnerClan.Leader : null);
			if (hero == null)
			{
				return;
			}
			hero.AddSkillXp(skill, skillXp);
		}

		private static void OnGainingRidingExperience(Hero hero, float baseXpAmount, ItemObject horse)
		{
			if (horse != null)
			{
				float num = 1f + (float)horse.Difficulty * 0.02f;
				hero.AddSkillXp(DefaultSkills.Riding, baseXpAmount * num);
			}
		}

		private static void OnPartySkillExercised(MobileParty party, SkillObject skill, float skillXp, SkillEffect.PerkRole perkRole = SkillEffect.PerkRole.PartyLeader)
		{
			Hero effectiveRoleHolder = party.GetEffectiveRoleHolder(perkRole);
			if (effectiveRoleHolder == null)
			{
				return;
			}
			effectiveRoleHolder.AddSkillXp(skill, skillXp);
		}

		private const float TacticsXpCoefficient = 0.02f;
	}
}
