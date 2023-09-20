using System;
using System.Collections.Generic;
using Helpers;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.BarterSystem;
using TaleWorlds.CampaignSystem.BarterSystem.Barterables;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Conversation.Persuasion;
using TaleWorlds.CampaignSystem.Election;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.Issues;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Buildings;
using TaleWorlds.CampaignSystem.Settlements.Workshops;
using TaleWorlds.CampaignSystem.Siege;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem
{
	// Token: 0x02000036 RID: 54
	public class CampaignEvents : CampaignEventReceiver
	{
		// Token: 0x170000AB RID: 171
		// (get) Token: 0x0600052E RID: 1326 RVA: 0x0001C07D File Offset: 0x0001A27D
		private static CampaignEvents Instance
		{
			get
			{
				return Campaign.Current.CampaignEvents;
			}
		}

		// Token: 0x0600052F RID: 1327 RVA: 0x0001C08C File Offset: 0x0001A28C
		public override void RemoveListeners(object obj)
		{
			this._heroLevelledUp.ClearListeners(obj);
			this._heroGainedSkill.ClearListeners(obj);
			this._heroRelationChanged.ClearListeners(obj);
			this._questLogAddedEvent.ClearListeners(obj);
			this._issueLogAddedEvent.ClearListeners(obj);
			this._onCharacterCreationIsOverEvent.ClearListeners(obj);
			this._clanChangedKingdom.ClearListeners(obj);
			this._onCompanionClanCreatedEvent.ClearListeners(obj);
			this._onHeroJoinedPartyEvent.ClearListeners(obj);
			this._partyAttachedParty.ClearListeners(obj);
			this._nearbyPartyAddedToPlayerMapEvent.ClearListeners(obj);
			this._armyCreated.ClearListeners(obj);
			this._armyGathered.ClearListeners(obj);
			this._armyDispersed.ClearListeners(obj);
			this._villageStateChanged.ClearListeners(obj);
			this._settlementEntered.ClearListeners(obj);
			this._afterSettlementEntered.ClearListeners(obj);
			this._mercenaryTroopChangedInTown.ClearListeners(obj);
			this._mercenaryNumberChangedInTown.ClearListeners(obj);
			this._alleyOwnerChanged.ClearListeners(obj);
			this._alleyOccupiedByPlayer.ClearListeners(obj);
			this._alleyClearedByPlayer.ClearListeners(obj);
			this._romanticStateChanged.ClearListeners(obj);
			this._warDeclared.ClearListeners(obj);
			this._battleStarted.ClearListeners(obj);
			this._rebellionFinished.ClearListeners(obj);
			this._townRebelliousStateChanged.ClearListeners(obj);
			this._rebelliousClanDisbandedAtSettlement.ClearListeners(obj);
			this._mobilePartyDestroyed.ClearListeners(obj);
			this._mobilePartyCreated.ClearListeners(obj);
			this._heroKilled.ClearListeners(obj);
			this._characterDefeated.ClearListeners(obj);
			this._heroPrisonerTaken.ClearListeners(obj);
			this._onPartySizeChangedEvent.ClearListeners(obj);
			this._characterBecameFugitive.ClearListeners(obj);
			this._playerMetHero.ClearListeners(obj);
			this._playerLearnsAboutHero.ClearListeners(obj);
			this._renownGained.ClearListeners(obj);
			this._barterablesRequested.ClearListeners(obj);
			this._crimeRatingChanged.ClearListeners(obj);
			this._newCompanionAdded.ClearListeners(obj);
			this._afterMissionStarted.ClearListeners(obj);
			this._gameMenuOpened.ClearListeners(obj);
			this._makePeace.ClearListeners(obj);
			this._kingdomCreated.ClearListeners(obj);
			this._kingdomDestroyed.ClearListeners(obj);
			this._villageBeingRaided.ClearListeners(obj);
			this._villageLooted.ClearListeners(obj);
			this._mapEventEnded.ClearListeners(obj);
			this._mapEventStarted.ClearListeners(obj);
			this._prisonersChangeInSettlement.ClearListeners(obj);
			this._onMissionStartedEvent.ClearListeners(obj);
			this._beforeMissionOpenedEvent.ClearListeners(obj);
			this._onPartyRemovedEvent.ClearListeners(obj);
			this._banditPartyRecruited.ClearListeners(obj);
			this._onSettlementOwnerChangedEvent.ClearListeners(obj);
			this._onGovernorChangedEvent.ClearListeners(obj);
			this._onSettlementLeftEvent.ClearListeners(obj);
			this._weeklyTickEvent.ClearListeners(obj);
			this._dailyTickEvent.ClearListeners(obj);
			this._dailyTickPartyEvent.ClearListeners(obj);
			this._hourlyTickEvent.ClearListeners(obj);
			this._tickEvent.ClearListeners(obj);
			this._onSessionLaunchedEvent.ClearListeners(obj);
			this._onAfterSessionLaunchedEvent.ClearListeners(obj);
			this._onNewGameCreatedPartialFollowUpEvent.ClearListeners(obj);
			this._onNewGameCreatedPartialFollowUpEndEvent.ClearListeners(obj);
			this._onNewGameCreatedEvent.ClearListeners(obj);
			this._onGameLoadedEvent.ClearListeners(obj);
			this._onBarterAcceptedEvent.ClearListeners(obj);
			this._onBarterCanceledEvent.ClearListeners(obj);
			this._onGameEarlyLoadedEvent.ClearListeners(obj);
			this._onGameLoadFinishedEvent.ClearListeners(obj);
			this._aiHourlyTickEvent.ClearListeners(obj);
			this._tickPartialHourlyAiEvent.ClearListeners(obj);
			this._onPartyJoinedArmyEvent.ClearListeners(obj);
			this._onPartyRemovedFromArmyEvent.ClearListeners(obj);
			this._onMissionEndedEvent.ClearListeners(obj);
			this._onPlayerBattleEndEvent.ClearListeners(obj);
			this._onPlayerBoardGameOver.ClearListeners(obj);
			this._onRansomOfferedToPlayer.ClearListeners(obj);
			this._onRansomOfferCancelled.ClearListeners(obj);
			this._onPeaceOfferedToPlayer.ClearListeners(obj);
			this._onPeaceOfferCancelled.ClearListeners(obj);
			this._onMarriageOfferedToPlayerEvent.ClearListeners(obj);
			this._onMarriageOfferCanceledEvent.ClearListeners(obj);
			this._onVassalOrMercenaryServiceOfferedToPlayerEvent.ClearListeners(obj);
			this._onVassalOrMercenaryServiceOfferCanceledEvent.ClearListeners(obj);
			this._afterGameMenuOpenedEvent.ClearListeners(obj);
			this._beforeGameMenuOpenedEvent.ClearListeners(obj);
			this._onChildConceived.ClearListeners(obj);
			this._onGivenBirthEvent.ClearListeners(obj);
			this._missionTickEvent.ClearListeners(obj);
			this._armyOverlaySetDirty.ClearListeners(obj);
			this._onArmyLeaderThinkEvent.ClearListeners(obj);
			this._partyVisibilityChanged.ClearListeners(obj);
			this._onHeroCreated.ClearListeners(obj);
			this._heroOccupationChangedEvent.ClearListeners(obj);
			this._onHeroWounded.ClearListeners(obj);
			this._playerDesertedBattle.ClearListeners(obj);
			this._companionRemoved.ClearListeners(obj);
			this._setupPreConversationEvent.ClearListeners(obj);
			this._trackLostEvent.ClearListeners(obj);
			this._trackDetectedEvent.ClearListeners(obj);
			this._locationCharactersAreReadyToSpawn.ClearListeners(obj);
			this._locationCharactersSimulatedSpawned.ClearListeners(obj);
			this._playerUpgradedTroopsEvent.ClearListeners(obj);
			this._onHeroCombatHitEvent.ClearListeners(obj);
			this._characterPortraitPopUpOpenedEvent.ClearListeners(obj);
			this._characterPortraitPopUpClosedEvent.ClearListeners(obj);
			this._playerStartTalkFromMenu.ClearListeners(obj);
			this._gameMenuOptionSelectedEvent.ClearListeners(obj);
			this._playerStartRecruitmentEvent.ClearListeners(obj);
			this._onAgentJoinedConversationEvent.ClearListeners(obj);
			this._onConversationEnded.ClearListeners(obj);
			this._heroesMarried.ClearListeners(obj);
			this._onTroopsDesertedEvent.ClearListeners(obj);
			this._onBeforePlayerCharacterChangedEvent.ClearListeners(obj);
			this._onPlayerCharacterChangedEvent.ClearListeners(obj);
			this._onClanLeaderChangedEvent.ClearListeners(obj);
			this._onSiegeEventStartedEvent.ClearListeners(obj);
			this._onPlayerSiegeStartedEvent.ClearListeners(obj);
			this._onSiegeEventEndedEvent.ClearListeners(obj);
			this._siegeAftermathAppliedEvent.ClearListeners(obj);
			this._onSiegeBombardmentHitEvent.ClearListeners(obj);
			this._onSiegeBombardmentWallHitEvent.ClearListeners(obj);
			this._onSiegeEngineDestroyedEvent.ClearListeners(obj);
			this._kingdomDecisionAdded.ClearListeners(obj);
			this._kingdomDecisionCancelled.ClearListeners(obj);
			this._kingdomDecisionConcluded.ClearListeners(obj);
			this._childEducationCompleted.ClearListeners(obj);
			this._heroComesOfAge.ClearListeners(obj);
			this._heroGrowsOutOfInfancyEvent.ClearListeners(obj);
			this._heroReachesTeenAgeEvent.ClearListeners(obj);
			this._onCheckForIssueEvent.ClearListeners(obj);
			this._onIssueUpdatedEvent.ClearListeners(obj);
			this._onTroopRecruitedEvent.ClearListeners(obj);
			this._onTroopGivenToSettlementEvent.ClearListeners(obj);
			this._onItemSoldEvent.ClearListeners(obj);
			this._onCaravanTransactionCompletedEvent.ClearListeners(obj);
			this._onPrisonerSoldEvent.ClearListeners(obj);
			this._heroPrisonerReleased.ClearListeners(obj);
			this._heroOrPartyTradedGold.ClearListeners(obj);
			this._heroOrPartyGaveItem.ClearListeners(obj);
			this._perkOpenedEvent.ClearListeners(obj);
			this._playerTraitChangedEvent.ClearListeners(obj);
			this._onPartyDisbandedEvent.ClearListeners(obj);
			this._onPartyDisbandStartedEvent.ClearListeners(obj);
			this._onPartyDisbandCanceledEvent.ClearListeners(obj);
			this._itemsLooted.ClearListeners(obj);
			this._hideoutSpottedEvent.ClearListeners(obj);
			this._hideoutBattleCompletedEvent.ClearListeners(obj);
			this._hideoutDeactivatedEvent.ClearListeners(obj);
			this._heroSharedFoodWithAnotherHeroEvent.ClearListeners(obj);
			this._onQuestCompletedEvent.ClearListeners(obj);
			this._itemProducedEvent.ClearListeners(obj);
			this._itemConsumedEvent.ClearListeners(obj);
			this._onQuestStartedEvent.ClearListeners(obj);
			this._onPartyConsumedFoodEvent.ClearListeners(obj);
			this._siegeCompletedEvent.ClearListeners(obj);
			this._raidCompletedEvent.ClearListeners(obj);
			this._forceVolunteersCompletedEvent.ClearListeners(obj);
			this._forceSuppliesCompletedEvent.ClearListeners(obj);
			this._onBeforeMainCharacterDiedEvent.ClearListeners(obj);
			this._onGameOverEvent.ClearListeners(obj);
			this._onClanDestroyedEvent.ClearListeners(obj);
			this._onNewIssueCreatedEvent.ClearListeners(obj);
			this._onIssueOwnerChangedEvent.ClearListeners(obj);
			this._onTutorialCompletedEvent.ClearListeners(obj);
			this._collectAvailableTutorialsEvent.ClearListeners(obj);
			this._playerEliminatedFromTournament.ClearListeners(obj);
			this._playerStartedTournamentMatch.ClearListeners(obj);
			this._tournamentStarted.ClearListeners(obj);
			this._tournamentFinished.ClearListeners(obj);
			this._tournamentCancelled.ClearListeners(obj);
			this._playerInventoryExchangeEvent.ClearListeners(obj);
			this._onItemsDiscardedByPlayerEvent.ClearListeners(obj);
			this._onNewItemCraftedEvent.ClearListeners(obj);
			this._craftingPartUnlockedEvent.ClearListeners(obj);
			this._onWorkshopChangedEvent.ClearListeners(obj);
			this._persuasionProgressCommittedEvent.ClearListeners(obj);
			this._onBeforeSaveEvent.ClearListeners(obj);
			this._onPrisonerTakenEvent.ClearListeners(obj);
			this._onPrisonerReleasedEvent.ClearListeners(obj);
			this._onMainPartyPrisonerRecruitedEvent.ClearListeners(obj);
			this._onPrisonerDonatedToSettlementEvent.ClearListeners(obj);
			this._onEquipmentSmeltedByHero.ClearListeners(obj);
			this._onPlayerTradeProfit.ClearListeners(obj);
			this._onBeforeHeroKilled.ClearListeners(obj);
			this._onBuildingLevelChangedEvent.ClearListeners(obj);
			this._hourlyTickSettlementEvent.ClearListeners(obj);
			this._hourlyTickClanEvent.ClearListeners(obj);
			this._onUnitRecruitedEvent.ClearListeners(obj);
			this._trackDetectedEvent.ClearListeners(obj);
			this._trackLostEvent.ClearListeners(obj);
			this._onTradeRumorIsTakenEvent.ClearListeners(obj);
			this._siegeEngineBuiltEvent.ClearListeners(obj);
			this._dailyTickHeroEvent.ClearListeners(obj);
			this._dailyTickSettlementEvent.ClearListeners(obj);
			this._hourlyTickPartyEvent.ClearListeners(obj);
			this._dailyTickClanEvent.ClearListeners(obj);
			this._villageBecomeNormal.ClearListeners(obj);
			this._clanTierIncrease.ClearListeners(obj);
			this._dailyTickTownEvent.ClearListeners(obj);
			this._onHeroChangedClan.ClearListeners(obj);
			this._onHeroGetsBusy.ClearListeners(obj);
			this._onSaveStartedEvent.ClearListeners(obj);
			this._onSaveOverEvent.ClearListeners(obj);
			this._onPlayerBodyPropertiesChangedEvent.ClearListeners(obj);
			this._rulingClanChanged.ClearListeners(obj);
			this._collectLootsEvent.ClearListeners(obj);
			this._distributeLootToPartyEvent.ClearListeners(obj);
			this._onHeroTeleportationRequestedEvent.ClearListeners(obj);
			this._onPartyLeaderChangeOfferCanceledEvent.ClearListeners(obj);
			this._canBeGovernorOrHavePartyRoleEvent.ClearListeners(obj);
			this._canHeroLeadPartyEvent.ClearListeners(obj);
			this._canMarryEvent.ClearListeners(obj);
			this._canHeroDieEvent.ClearListeners(obj);
			this._canHeroBecomePrisonerEvent.ClearListeners(obj);
			this._canHeroEquipmentBeChangedEvent.ClearListeners(obj);
			this._canHaveQuestsOrIssues.ClearListeners(obj);
			this._canMoveToSettlementEvent.ClearListeners(obj);
			this._onQuarterDailyPartyTick.ClearListeners(obj);
			this._onMainPartyStarving.ClearListeners(obj);
			this._onClanInfluenceChangedEvent.ClearListeners(obj);
			this._onPlayerPartyKnockedOrKilledTroopEvent.ClearListeners(obj);
			this._onPlayerEarnedGoldFromAssetEvent.ClearListeners(obj);
			this._onPlayerJoinedTournamentEvent.ClearListeners(obj);
			this._onHeroUnregisteredEvent.ClearListeners(obj);
		}

		// Token: 0x170000AC RID: 172
		// (get) Token: 0x06000530 RID: 1328 RVA: 0x0001CB55 File Offset: 0x0001AD55
		public static IMbEvent OnPlayerBodyPropertiesChangedEvent
		{
			get
			{
				return CampaignEvents.Instance._onPlayerBodyPropertiesChangedEvent;
			}
		}

		// Token: 0x06000531 RID: 1329 RVA: 0x0001CB61 File Offset: 0x0001AD61
		public override void OnPlayerBodyPropertiesChanged()
		{
			CampaignEvents.Instance._onPlayerBodyPropertiesChangedEvent.Invoke();
		}

		// Token: 0x170000AD RID: 173
		// (get) Token: 0x06000532 RID: 1330 RVA: 0x0001CB72 File Offset: 0x0001AD72
		public static IMbEvent<BarterData> BarterablesRequested
		{
			get
			{
				return CampaignEvents.Instance._barterablesRequested;
			}
		}

		// Token: 0x06000533 RID: 1331 RVA: 0x0001CB7E File Offset: 0x0001AD7E
		public override void OnBarterablesRequested(BarterData args)
		{
			CampaignEvents.Instance._barterablesRequested.Invoke(args);
		}

		// Token: 0x170000AE RID: 174
		// (get) Token: 0x06000534 RID: 1332 RVA: 0x0001CB90 File Offset: 0x0001AD90
		public static IMbEvent<Hero, bool> HeroLevelledUp
		{
			get
			{
				return CampaignEvents.Instance._heroLevelledUp;
			}
		}

		// Token: 0x06000535 RID: 1333 RVA: 0x0001CB9C File Offset: 0x0001AD9C
		public override void OnHeroLevelledUp(Hero hero, bool shouldNotify = true)
		{
			this._heroLevelledUp.Invoke(hero, shouldNotify);
		}

		// Token: 0x170000AF RID: 175
		// (get) Token: 0x06000536 RID: 1334 RVA: 0x0001CBAB File Offset: 0x0001ADAB
		public static IMbEvent<Hero, SkillObject, int, bool> HeroGainedSkill
		{
			get
			{
				return CampaignEvents.Instance._heroGainedSkill;
			}
		}

		// Token: 0x06000537 RID: 1335 RVA: 0x0001CBB7 File Offset: 0x0001ADB7
		public override void OnHeroGainedSkill(Hero hero, SkillObject skill, int change = 1, bool shouldNotify = true)
		{
			this._heroGainedSkill.Invoke(hero, skill, change, shouldNotify);
		}

		// Token: 0x170000B0 RID: 176
		// (get) Token: 0x06000538 RID: 1336 RVA: 0x0001CBC9 File Offset: 0x0001ADC9
		public static IMbEvent OnCharacterCreationIsOverEvent
		{
			get
			{
				return CampaignEvents.Instance._onCharacterCreationIsOverEvent;
			}
		}

		// Token: 0x06000539 RID: 1337 RVA: 0x0001CBD5 File Offset: 0x0001ADD5
		public override void OnCharacterCreationIsOver()
		{
			CampaignEvents.Instance._onCharacterCreationIsOverEvent.Invoke();
		}

		// Token: 0x170000B1 RID: 177
		// (get) Token: 0x0600053A RID: 1338 RVA: 0x0001CBE6 File Offset: 0x0001ADE6
		public static IMbEvent<Hero, bool> HeroCreated
		{
			get
			{
				return CampaignEvents.Instance._onHeroCreated;
			}
		}

		// Token: 0x0600053B RID: 1339 RVA: 0x0001CBF2 File Offset: 0x0001ADF2
		public override void OnHeroCreated(Hero hero, bool isBornNaturally = false)
		{
			this._onHeroCreated.Invoke(hero, isBornNaturally);
		}

		// Token: 0x170000B2 RID: 178
		// (get) Token: 0x0600053C RID: 1340 RVA: 0x0001CC01 File Offset: 0x0001AE01
		public static IMbEvent<Hero, Occupation> HeroOccupationChangedEvent
		{
			get
			{
				return CampaignEvents.Instance._heroOccupationChangedEvent;
			}
		}

		// Token: 0x0600053D RID: 1341 RVA: 0x0001CC0D File Offset: 0x0001AE0D
		public override void OnHeroOccupationChanged(Hero hero, Occupation oldOccupation)
		{
			this._heroOccupationChangedEvent.Invoke(hero, oldOccupation);
		}

		// Token: 0x170000B3 RID: 179
		// (get) Token: 0x0600053E RID: 1342 RVA: 0x0001CC1C File Offset: 0x0001AE1C
		public static IMbEvent<Hero> HeroWounded
		{
			get
			{
				return CampaignEvents.Instance._onHeroWounded;
			}
		}

		// Token: 0x0600053F RID: 1343 RVA: 0x0001CC28 File Offset: 0x0001AE28
		public override void OnHeroWounded(Hero woundedHero)
		{
			this._onHeroWounded.Invoke(woundedHero);
		}

		// Token: 0x170000B4 RID: 180
		// (get) Token: 0x06000540 RID: 1344 RVA: 0x0001CC36 File Offset: 0x0001AE36
		public static IMbEvent<Hero, Hero, List<Barterable>> OnBarterAcceptedEvent
		{
			get
			{
				return CampaignEvents.Instance._onBarterAcceptedEvent;
			}
		}

		// Token: 0x06000541 RID: 1345 RVA: 0x0001CC42 File Offset: 0x0001AE42
		public override void OnBarterAccepted(Hero offererHero, Hero otherHero, List<Barterable> barters)
		{
			CampaignEvents.Instance._onBarterAcceptedEvent.Invoke(offererHero, otherHero, barters);
		}

		// Token: 0x170000B5 RID: 181
		// (get) Token: 0x06000542 RID: 1346 RVA: 0x0001CC56 File Offset: 0x0001AE56
		public static IMbEvent<Hero, Hero, List<Barterable>> OnBarterCanceledEvent
		{
			get
			{
				return CampaignEvents.Instance._onBarterCanceledEvent;
			}
		}

		// Token: 0x06000543 RID: 1347 RVA: 0x0001CC62 File Offset: 0x0001AE62
		public override void OnBarterCanceled(Hero offererHero, Hero otherHero, List<Barterable> barters)
		{
			CampaignEvents.Instance._onBarterCanceledEvent.Invoke(offererHero, otherHero, barters);
		}

		// Token: 0x170000B6 RID: 182
		// (get) Token: 0x06000544 RID: 1348 RVA: 0x0001CC76 File Offset: 0x0001AE76
		public static IMbEvent<Hero, Hero, int, bool, ChangeRelationAction.ChangeRelationDetail, Hero, Hero> HeroRelationChanged
		{
			get
			{
				return CampaignEvents.Instance._heroRelationChanged;
			}
		}

		// Token: 0x06000545 RID: 1349 RVA: 0x0001CC82 File Offset: 0x0001AE82
		public override void OnHeroRelationChanged(Hero effectiveHero, Hero effectiveHeroGainedRelationWith, int relationChange, bool showNotification, ChangeRelationAction.ChangeRelationDetail detail, Hero originalHero, Hero originalGainedRelationWith)
		{
			CampaignEvents.Instance._heroRelationChanged.Invoke(effectiveHero, effectiveHeroGainedRelationWith, relationChange, showNotification, detail, originalHero, originalGainedRelationWith);
		}

		// Token: 0x170000B7 RID: 183
		// (get) Token: 0x06000546 RID: 1350 RVA: 0x0001CC9E File Offset: 0x0001AE9E
		public static IMbEvent<QuestBase, bool> QuestLogAddedEvent
		{
			get
			{
				return CampaignEvents.Instance._questLogAddedEvent;
			}
		}

		// Token: 0x06000547 RID: 1351 RVA: 0x0001CCAA File Offset: 0x0001AEAA
		public override void OnQuestLogAdded(QuestBase quest, bool hideInformation)
		{
			CampaignEvents.Instance._questLogAddedEvent.Invoke(quest, hideInformation);
		}

		// Token: 0x170000B8 RID: 184
		// (get) Token: 0x06000548 RID: 1352 RVA: 0x0001CCBD File Offset: 0x0001AEBD
		public static IMbEvent<IssueBase, bool> IssueLogAddedEvent
		{
			get
			{
				return CampaignEvents.Instance._issueLogAddedEvent;
			}
		}

		// Token: 0x06000549 RID: 1353 RVA: 0x0001CCC9 File Offset: 0x0001AEC9
		public override void OnIssueLogAdded(IssueBase issue, bool hideInformation)
		{
			CampaignEvents.Instance._issueLogAddedEvent.Invoke(issue, hideInformation);
		}

		// Token: 0x170000B9 RID: 185
		// (get) Token: 0x0600054A RID: 1354 RVA: 0x0001CCDC File Offset: 0x0001AEDC
		public static IMbEvent<Clan, bool> ClanTierIncrease
		{
			get
			{
				return CampaignEvents.Instance._clanTierIncrease;
			}
		}

		// Token: 0x0600054B RID: 1355 RVA: 0x0001CCE8 File Offset: 0x0001AEE8
		public override void OnClanTierChanged(Clan clan, bool shouldNotify = true)
		{
			CampaignEvents.Instance._clanTierIncrease.Invoke(clan, shouldNotify);
		}

		// Token: 0x170000BA RID: 186
		// (get) Token: 0x0600054C RID: 1356 RVA: 0x0001CCFB File Offset: 0x0001AEFB
		public static IMbEvent<Clan, Kingdom, Kingdom, ChangeKingdomAction.ChangeKingdomActionDetail, bool> ClanChangedKingdom
		{
			get
			{
				return CampaignEvents.Instance._clanChangedKingdom;
			}
		}

		// Token: 0x0600054D RID: 1357 RVA: 0x0001CD07 File Offset: 0x0001AF07
		public override void OnClanChangedKingdom(Clan clan, Kingdom oldKingdom, Kingdom newKingdom, ChangeKingdomAction.ChangeKingdomActionDetail detail, bool showNotification = true)
		{
			CampaignEvents.Instance._clanChangedKingdom.Invoke(clan, oldKingdom, newKingdom, detail, showNotification);
		}

		// Token: 0x170000BB RID: 187
		// (get) Token: 0x0600054E RID: 1358 RVA: 0x0001CD1F File Offset: 0x0001AF1F
		public static IMbEvent<Clan> OnCompanionClanCreatedEvent
		{
			get
			{
				return CampaignEvents.Instance._onCompanionClanCreatedEvent;
			}
		}

		// Token: 0x0600054F RID: 1359 RVA: 0x0001CD2B File Offset: 0x0001AF2B
		public override void OnCompanionClanCreated(Clan clan)
		{
			CampaignEvents.Instance._onCompanionClanCreatedEvent.Invoke(clan);
		}

		// Token: 0x170000BC RID: 188
		// (get) Token: 0x06000550 RID: 1360 RVA: 0x0001CD3D File Offset: 0x0001AF3D
		public static IMbEvent<Hero, MobileParty> OnHeroJoinedPartyEvent
		{
			get
			{
				return CampaignEvents.Instance._onHeroJoinedPartyEvent;
			}
		}

		// Token: 0x06000551 RID: 1361 RVA: 0x0001CD49 File Offset: 0x0001AF49
		public override void OnHeroJoinedParty(Hero hero, MobileParty mobileParty)
		{
			CampaignEvents.Instance._onHeroJoinedPartyEvent.Invoke(hero, mobileParty);
		}

		// Token: 0x170000BD RID: 189
		// (get) Token: 0x06000552 RID: 1362 RVA: 0x0001CD5C File Offset: 0x0001AF5C
		public static IMbEvent<ValueTuple<Hero, PartyBase>, ValueTuple<Hero, PartyBase>, ValueTuple<int, string>, bool> HeroOrPartyTradedGold
		{
			get
			{
				return CampaignEvents.Instance._heroOrPartyTradedGold;
			}
		}

		// Token: 0x06000553 RID: 1363 RVA: 0x0001CD68 File Offset: 0x0001AF68
		public override void OnHeroOrPartyTradedGold(ValueTuple<Hero, PartyBase> giver, ValueTuple<Hero, PartyBase> recipient, ValueTuple<int, string> goldAmount, bool showNotification)
		{
			CampaignEvents.Instance._heroOrPartyTradedGold.Invoke(giver, recipient, goldAmount, showNotification);
		}

		// Token: 0x170000BE RID: 190
		// (get) Token: 0x06000554 RID: 1364 RVA: 0x0001CD7E File Offset: 0x0001AF7E
		public static IMbEvent<ValueTuple<Hero, PartyBase>, ValueTuple<Hero, PartyBase>, ItemObject, int, bool> HeroOrPartyGaveItem
		{
			get
			{
				return CampaignEvents.Instance._heroOrPartyGaveItem;
			}
		}

		// Token: 0x06000555 RID: 1365 RVA: 0x0001CD8A File Offset: 0x0001AF8A
		public override void OnHeroOrPartyGaveItem(ValueTuple<Hero, PartyBase> giver, ValueTuple<Hero, PartyBase> receiver, ItemObject item, int count, bool showNotification)
		{
			CampaignEvents.Instance._heroOrPartyGaveItem.Invoke(giver, receiver, item, count, showNotification);
		}

		// Token: 0x170000BF RID: 191
		// (get) Token: 0x06000556 RID: 1366 RVA: 0x0001CDA2 File Offset: 0x0001AFA2
		public static IMbEvent<MobileParty> BanditPartyRecruited
		{
			get
			{
				return CampaignEvents.Instance._banditPartyRecruited;
			}
		}

		// Token: 0x06000557 RID: 1367 RVA: 0x0001CDAE File Offset: 0x0001AFAE
		public override void OnBanditPartyRecruited(MobileParty banditParty)
		{
			CampaignEvents.Instance._banditPartyRecruited.Invoke(banditParty);
		}

		// Token: 0x170000C0 RID: 192
		// (get) Token: 0x06000558 RID: 1368 RVA: 0x0001CDC0 File Offset: 0x0001AFC0
		public static IMbEvent<KingdomDecision, bool> KingdomDecisionAdded
		{
			get
			{
				return CampaignEvents.Instance._kingdomDecisionAdded;
			}
		}

		// Token: 0x06000559 RID: 1369 RVA: 0x0001CDCC File Offset: 0x0001AFCC
		public override void OnKingdomDecisionAdded(KingdomDecision decision, bool isPlayerInvolved)
		{
			CampaignEvents.Instance._kingdomDecisionAdded.Invoke(decision, isPlayerInvolved);
		}

		// Token: 0x170000C1 RID: 193
		// (get) Token: 0x0600055A RID: 1370 RVA: 0x0001CDDF File Offset: 0x0001AFDF
		public static IMbEvent<KingdomDecision, bool> KingdomDecisionCancelled
		{
			get
			{
				return CampaignEvents.Instance._kingdomDecisionCancelled;
			}
		}

		// Token: 0x0600055B RID: 1371 RVA: 0x0001CDEB File Offset: 0x0001AFEB
		public override void OnKingdomDecisionCancelled(KingdomDecision decision, bool isPlayerInvolved)
		{
			CampaignEvents.Instance._kingdomDecisionCancelled.Invoke(decision, isPlayerInvolved);
		}

		// Token: 0x170000C2 RID: 194
		// (get) Token: 0x0600055C RID: 1372 RVA: 0x0001CDFE File Offset: 0x0001AFFE
		public static IMbEvent<KingdomDecision, DecisionOutcome, bool> KingdomDecisionConcluded
		{
			get
			{
				return CampaignEvents.Instance._kingdomDecisionConcluded;
			}
		}

		// Token: 0x0600055D RID: 1373 RVA: 0x0001CE0A File Offset: 0x0001B00A
		public override void OnKingdomDecisionConcluded(KingdomDecision decision, DecisionOutcome chosenOutcome, bool isPlayerInvolved)
		{
			CampaignEvents.Instance._kingdomDecisionConcluded.Invoke(decision, chosenOutcome, isPlayerInvolved);
		}

		// Token: 0x170000C3 RID: 195
		// (get) Token: 0x0600055E RID: 1374 RVA: 0x0001CE1E File Offset: 0x0001B01E
		public static IMbEvent<MobileParty> PartyAttachedAnotherParty
		{
			get
			{
				return CampaignEvents.Instance._partyAttachedParty;
			}
		}

		// Token: 0x0600055F RID: 1375 RVA: 0x0001CE2A File Offset: 0x0001B02A
		public override void OnPartyAttachedAnotherParty(MobileParty mobileParty)
		{
			CampaignEvents.Instance._partyAttachedParty.Invoke(mobileParty);
		}

		// Token: 0x170000C4 RID: 196
		// (get) Token: 0x06000560 RID: 1376 RVA: 0x0001CE3C File Offset: 0x0001B03C
		public static IMbEvent<MobileParty> NearbyPartyAddedToPlayerMapEvent
		{
			get
			{
				return CampaignEvents.Instance._nearbyPartyAddedToPlayerMapEvent;
			}
		}

		// Token: 0x06000561 RID: 1377 RVA: 0x0001CE48 File Offset: 0x0001B048
		public override void OnNearbyPartyAddedToPlayerMapEvent(MobileParty mobileParty)
		{
			CampaignEvents.Instance._nearbyPartyAddedToPlayerMapEvent.Invoke(mobileParty);
		}

		// Token: 0x170000C5 RID: 197
		// (get) Token: 0x06000562 RID: 1378 RVA: 0x0001CE5A File Offset: 0x0001B05A
		public static IMbEvent<Army> ArmyCreated
		{
			get
			{
				return CampaignEvents.Instance._armyCreated;
			}
		}

		// Token: 0x06000563 RID: 1379 RVA: 0x0001CE66 File Offset: 0x0001B066
		public override void OnArmyCreated(Army army)
		{
			CampaignEvents.Instance._armyCreated.Invoke(army);
		}

		// Token: 0x170000C6 RID: 198
		// (get) Token: 0x06000564 RID: 1380 RVA: 0x0001CE78 File Offset: 0x0001B078
		public static IMbEvent<Army, Army.ArmyDispersionReason, bool> ArmyDispersed
		{
			get
			{
				return CampaignEvents.Instance._armyDispersed;
			}
		}

		// Token: 0x06000565 RID: 1381 RVA: 0x0001CE84 File Offset: 0x0001B084
		public override void OnArmyDispersed(Army army, Army.ArmyDispersionReason reason, bool isPlayersArmy)
		{
			CampaignEvents.Instance._armyDispersed.Invoke(army, reason, isPlayersArmy);
		}

		// Token: 0x170000C7 RID: 199
		// (get) Token: 0x06000566 RID: 1382 RVA: 0x0001CE98 File Offset: 0x0001B098
		public static IMbEvent<Army, Settlement> ArmyGathered
		{
			get
			{
				return CampaignEvents.Instance._armyGathered;
			}
		}

		// Token: 0x06000567 RID: 1383 RVA: 0x0001CEA4 File Offset: 0x0001B0A4
		public override void OnArmyGathered(Army army, Settlement gatheringSettlement)
		{
			CampaignEvents.Instance._armyGathered.Invoke(army, gatheringSettlement);
		}

		// Token: 0x170000C8 RID: 200
		// (get) Token: 0x06000568 RID: 1384 RVA: 0x0001CEB7 File Offset: 0x0001B0B7
		public static IMbEvent<Hero, PerkObject> PerkOpenedEvent
		{
			get
			{
				return CampaignEvents.Instance._perkOpenedEvent;
			}
		}

		// Token: 0x06000569 RID: 1385 RVA: 0x0001CEC3 File Offset: 0x0001B0C3
		public override void OnPerkOpened(Hero hero, PerkObject perk)
		{
			CampaignEvents.Instance._perkOpenedEvent.Invoke(hero, perk);
		}

		// Token: 0x170000C9 RID: 201
		// (get) Token: 0x0600056A RID: 1386 RVA: 0x0001CED6 File Offset: 0x0001B0D6
		public static IMbEvent<TraitObject, int> PlayerTraitChangedEvent
		{
			get
			{
				return CampaignEvents.Instance._playerTraitChangedEvent;
			}
		}

		// Token: 0x0600056B RID: 1387 RVA: 0x0001CEE2 File Offset: 0x0001B0E2
		public override void OnPlayerTraitChanged(TraitObject trait, int previousLevel)
		{
			CampaignEvents.Instance._playerTraitChangedEvent.Invoke(trait, previousLevel);
		}

		// Token: 0x170000CA RID: 202
		// (get) Token: 0x0600056C RID: 1388 RVA: 0x0001CEF5 File Offset: 0x0001B0F5
		public static IMbEvent<Village, Village.VillageStates, Village.VillageStates, MobileParty> VillageStateChanged
		{
			get
			{
				return CampaignEvents.Instance._villageStateChanged;
			}
		}

		// Token: 0x0600056D RID: 1389 RVA: 0x0001CF01 File Offset: 0x0001B101
		public override void OnVillageStateChanged(Village village, Village.VillageStates oldState, Village.VillageStates newState, MobileParty raiderParty)
		{
			CampaignEvents.Instance._villageStateChanged.Invoke(village, oldState, newState, raiderParty);
		}

		// Token: 0x170000CB RID: 203
		// (get) Token: 0x0600056E RID: 1390 RVA: 0x0001CF17 File Offset: 0x0001B117
		public static IMbEvent<MobileParty, Settlement, Hero> SettlementEntered
		{
			get
			{
				return CampaignEvents.Instance._settlementEntered;
			}
		}

		// Token: 0x0600056F RID: 1391 RVA: 0x0001CF23 File Offset: 0x0001B123
		public override void OnSettlementEntered(MobileParty party, Settlement settlement, Hero hero)
		{
			CampaignEvents.Instance._settlementEntered.Invoke(party, settlement, hero);
		}

		// Token: 0x170000CC RID: 204
		// (get) Token: 0x06000570 RID: 1392 RVA: 0x0001CF37 File Offset: 0x0001B137
		public static IMbEvent<MobileParty, Settlement, Hero> AfterSettlementEntered
		{
			get
			{
				return CampaignEvents.Instance._afterSettlementEntered;
			}
		}

		// Token: 0x06000571 RID: 1393 RVA: 0x0001CF43 File Offset: 0x0001B143
		public override void OnAfterSettlementEntered(MobileParty party, Settlement settlement, Hero hero)
		{
			CampaignEvents.Instance._afterSettlementEntered.Invoke(party, settlement, hero);
		}

		// Token: 0x170000CD RID: 205
		// (get) Token: 0x06000572 RID: 1394 RVA: 0x0001CF57 File Offset: 0x0001B157
		public static IMbEvent<Town, CharacterObject, CharacterObject> MercenaryTroopChangedInTown
		{
			get
			{
				return CampaignEvents.Instance._mercenaryTroopChangedInTown;
			}
		}

		// Token: 0x06000573 RID: 1395 RVA: 0x0001CF63 File Offset: 0x0001B163
		public override void OnMercenaryTroopChangedInTown(Town town, CharacterObject oldTroopType, CharacterObject newTroopType)
		{
			CampaignEvents.Instance._mercenaryTroopChangedInTown.Invoke(town, oldTroopType, newTroopType);
		}

		// Token: 0x170000CE RID: 206
		// (get) Token: 0x06000574 RID: 1396 RVA: 0x0001CF77 File Offset: 0x0001B177
		public static IMbEvent<Town, int, int> MercenaryNumberChangedInTown
		{
			get
			{
				return CampaignEvents.Instance._mercenaryNumberChangedInTown;
			}
		}

		// Token: 0x06000575 RID: 1397 RVA: 0x0001CF83 File Offset: 0x0001B183
		public override void OnMercenaryNumberChangedInTown(Town town, int oldNumber, int newNumber)
		{
			CampaignEvents.Instance._mercenaryNumberChangedInTown.Invoke(town, oldNumber, newNumber);
		}

		// Token: 0x170000CF RID: 207
		// (get) Token: 0x06000576 RID: 1398 RVA: 0x0001CF97 File Offset: 0x0001B197
		public static IMbEvent<Alley, Hero, Hero> AlleyOwnerChanged
		{
			get
			{
				return CampaignEvents.Instance._alleyOwnerChanged;
			}
		}

		// Token: 0x170000D0 RID: 208
		// (get) Token: 0x06000577 RID: 1399 RVA: 0x0001CFA3 File Offset: 0x0001B1A3
		public static IMbEvent<Alley, TroopRoster> AlleyOccupiedByPlayer
		{
			get
			{
				return CampaignEvents.Instance._alleyOccupiedByPlayer;
			}
		}

		// Token: 0x06000578 RID: 1400 RVA: 0x0001CFAF File Offset: 0x0001B1AF
		public override void OnAlleyOccupiedByPlayer(Alley alley, TroopRoster troops)
		{
			CampaignEvents.Instance._alleyOccupiedByPlayer.Invoke(alley, troops);
		}

		// Token: 0x06000579 RID: 1401 RVA: 0x0001CFC2 File Offset: 0x0001B1C2
		public override void OnAlleyOwnerChanged(Alley alley, Hero newOwner, Hero oldOwner)
		{
			CampaignEvents.Instance._alleyOwnerChanged.Invoke(alley, newOwner, oldOwner);
		}

		// Token: 0x170000D1 RID: 209
		// (get) Token: 0x0600057A RID: 1402 RVA: 0x0001CFD6 File Offset: 0x0001B1D6
		public static IMbEvent<Alley> AlleyClearedByPlayer
		{
			get
			{
				return CampaignEvents.Instance._alleyClearedByPlayer;
			}
		}

		// Token: 0x0600057B RID: 1403 RVA: 0x0001CFE2 File Offset: 0x0001B1E2
		public override void OnAlleyClearedByPlayer(Alley alley)
		{
			CampaignEvents.Instance._alleyClearedByPlayer.Invoke(alley);
		}

		// Token: 0x170000D2 RID: 210
		// (get) Token: 0x0600057C RID: 1404 RVA: 0x0001CFF4 File Offset: 0x0001B1F4
		public static IMbEvent<Hero, Hero, Romance.RomanceLevelEnum> RomanticStateChanged
		{
			get
			{
				return CampaignEvents.Instance._romanticStateChanged;
			}
		}

		// Token: 0x0600057D RID: 1405 RVA: 0x0001D000 File Offset: 0x0001B200
		public override void OnRomanticStateChanged(Hero hero1, Hero hero2, Romance.RomanceLevelEnum romanceLevel)
		{
			CampaignEvents.Instance._romanticStateChanged.Invoke(hero1, hero2, romanceLevel);
		}

		// Token: 0x170000D3 RID: 211
		// (get) Token: 0x0600057E RID: 1406 RVA: 0x0001D014 File Offset: 0x0001B214
		public static IMbEvent<Hero, Hero, bool> HeroesMarried
		{
			get
			{
				return CampaignEvents.Instance._heroesMarried;
			}
		}

		// Token: 0x0600057F RID: 1407 RVA: 0x0001D020 File Offset: 0x0001B220
		public override void OnHeroesMarried(Hero hero1, Hero hero2, bool showNotification = true)
		{
			CampaignEvents.Instance._heroesMarried.Invoke(hero1, hero2, showNotification);
		}

		// Token: 0x170000D4 RID: 212
		// (get) Token: 0x06000580 RID: 1408 RVA: 0x0001D034 File Offset: 0x0001B234
		public static IMbEvent<int, Town> PlayerEliminatedFromTournament
		{
			get
			{
				return CampaignEvents.Instance._playerEliminatedFromTournament;
			}
		}

		// Token: 0x06000581 RID: 1409 RVA: 0x0001D040 File Offset: 0x0001B240
		public override void OnPlayerEliminatedFromTournament(int round, Town town)
		{
			CampaignEvents.Instance._playerEliminatedFromTournament.Invoke(round, town);
		}

		// Token: 0x170000D5 RID: 213
		// (get) Token: 0x06000582 RID: 1410 RVA: 0x0001D053 File Offset: 0x0001B253
		public static IMbEvent<Town> PlayerStartedTournamentMatch
		{
			get
			{
				return CampaignEvents.Instance._playerStartedTournamentMatch;
			}
		}

		// Token: 0x06000583 RID: 1411 RVA: 0x0001D05F File Offset: 0x0001B25F
		public override void OnPlayerStartedTournamentMatch(Town town)
		{
			CampaignEvents.Instance._playerStartedTournamentMatch.Invoke(town);
		}

		// Token: 0x170000D6 RID: 214
		// (get) Token: 0x06000584 RID: 1412 RVA: 0x0001D071 File Offset: 0x0001B271
		public static IMbEvent<Town> TournamentStarted
		{
			get
			{
				return CampaignEvents.Instance._tournamentStarted;
			}
		}

		// Token: 0x06000585 RID: 1413 RVA: 0x0001D07D File Offset: 0x0001B27D
		public override void OnTournamentStarted(Town town)
		{
			CampaignEvents.Instance._tournamentStarted.Invoke(town);
		}

		// Token: 0x170000D7 RID: 215
		// (get) Token: 0x06000586 RID: 1414 RVA: 0x0001D08F File Offset: 0x0001B28F
		public static IMbEvent<IFaction, IFaction, DeclareWarAction.DeclareWarDetail> WarDeclared
		{
			get
			{
				return CampaignEvents.Instance._warDeclared;
			}
		}

		// Token: 0x06000587 RID: 1415 RVA: 0x0001D09B File Offset: 0x0001B29B
		public override void OnWarDeclared(IFaction faction1, IFaction faction2, DeclareWarAction.DeclareWarDetail declareWarDetail)
		{
			CampaignEvents.Instance._warDeclared.Invoke(faction1, faction2, declareWarDetail);
		}

		// Token: 0x170000D8 RID: 216
		// (get) Token: 0x06000588 RID: 1416 RVA: 0x0001D0AF File Offset: 0x0001B2AF
		public static IMbEvent<CharacterObject, MBReadOnlyList<CharacterObject>, Town, ItemObject> TournamentFinished
		{
			get
			{
				return CampaignEvents.Instance._tournamentFinished;
			}
		}

		// Token: 0x06000589 RID: 1417 RVA: 0x0001D0BB File Offset: 0x0001B2BB
		public override void OnTournamentFinished(CharacterObject winner, MBReadOnlyList<CharacterObject> participants, Town town, ItemObject prize)
		{
			CampaignEvents.Instance._tournamentFinished.Invoke(winner, participants, town, prize);
		}

		// Token: 0x170000D9 RID: 217
		// (get) Token: 0x0600058A RID: 1418 RVA: 0x0001D0D1 File Offset: 0x0001B2D1
		public static IMbEvent<Town> TournamentCancelled
		{
			get
			{
				return CampaignEvents.Instance._tournamentCancelled;
			}
		}

		// Token: 0x0600058B RID: 1419 RVA: 0x0001D0DD File Offset: 0x0001B2DD
		public override void OnTournamentCancelled(Town town)
		{
			CampaignEvents.Instance._tournamentCancelled.Invoke(town);
		}

		// Token: 0x170000DA RID: 218
		// (get) Token: 0x0600058C RID: 1420 RVA: 0x0001D0EF File Offset: 0x0001B2EF
		public static IMbEvent<PartyBase, PartyBase, object, bool> BattleStarted
		{
			get
			{
				return CampaignEvents.Instance._battleStarted;
			}
		}

		// Token: 0x0600058D RID: 1421 RVA: 0x0001D0FB File Offset: 0x0001B2FB
		public override void OnStartBattle(PartyBase attackerParty, PartyBase defenderParty, object subject, bool showNotification)
		{
			CampaignEvents.Instance._battleStarted.Invoke(attackerParty, defenderParty, subject, showNotification);
		}

		// Token: 0x170000DB RID: 219
		// (get) Token: 0x0600058E RID: 1422 RVA: 0x0001D111 File Offset: 0x0001B311
		public static IMbEvent<Settlement, Clan> RebellionFinished
		{
			get
			{
				return CampaignEvents.Instance._rebellionFinished;
			}
		}

		// Token: 0x0600058F RID: 1423 RVA: 0x0001D11D File Offset: 0x0001B31D
		public override void OnRebellionFinished(Settlement settlement, Clan oldOwnerClan)
		{
			CampaignEvents.Instance._rebellionFinished.Invoke(settlement, oldOwnerClan);
		}

		// Token: 0x170000DC RID: 220
		// (get) Token: 0x06000590 RID: 1424 RVA: 0x0001D130 File Offset: 0x0001B330
		public static IMbEvent<Town, bool> TownRebelliosStateChanged
		{
			get
			{
				return CampaignEvents.Instance._townRebelliousStateChanged;
			}
		}

		// Token: 0x06000591 RID: 1425 RVA: 0x0001D13C File Offset: 0x0001B33C
		public override void TownRebelliousStateChanged(Town town, bool rebelliousState)
		{
			CampaignEvents.Instance._townRebelliousStateChanged.Invoke(town, rebelliousState);
		}

		// Token: 0x170000DD RID: 221
		// (get) Token: 0x06000592 RID: 1426 RVA: 0x0001D14F File Offset: 0x0001B34F
		public static IMbEvent<Settlement, Clan> RebelliousClanDisbandedAtSettlement
		{
			get
			{
				return CampaignEvents.Instance._rebelliousClanDisbandedAtSettlement;
			}
		}

		// Token: 0x06000593 RID: 1427 RVA: 0x0001D15B File Offset: 0x0001B35B
		public override void OnRebelliousClanDisbandedAtSettlement(Settlement settlement, Clan clan)
		{
			CampaignEvents.Instance._rebelliousClanDisbandedAtSettlement.Invoke(settlement, clan);
		}

		// Token: 0x170000DE RID: 222
		// (get) Token: 0x06000594 RID: 1428 RVA: 0x0001D16E File Offset: 0x0001B36E
		public static IMbEvent<MobileParty, ItemRoster> ItemsLooted
		{
			get
			{
				return CampaignEvents.Instance._itemsLooted;
			}
		}

		// Token: 0x06000595 RID: 1429 RVA: 0x0001D17A File Offset: 0x0001B37A
		public override void OnItemsLooted(MobileParty mobileParty, ItemRoster items)
		{
			CampaignEvents.Instance._itemsLooted.Invoke(mobileParty, items);
		}

		// Token: 0x170000DF RID: 223
		// (get) Token: 0x06000596 RID: 1430 RVA: 0x0001D18D File Offset: 0x0001B38D
		public static IMbEvent<MobileParty, PartyBase> MobilePartyDestroyed
		{
			get
			{
				return CampaignEvents.Instance._mobilePartyDestroyed;
			}
		}

		// Token: 0x06000597 RID: 1431 RVA: 0x0001D199 File Offset: 0x0001B399
		public override void OnMobilePartyDestroyed(MobileParty mobileParty, PartyBase destroyerParty)
		{
			CampaignEvents.Instance._mobilePartyDestroyed.Invoke(mobileParty, destroyerParty);
		}

		// Token: 0x170000E0 RID: 224
		// (get) Token: 0x06000598 RID: 1432 RVA: 0x0001D1AC File Offset: 0x0001B3AC
		public static IMbEvent<MobileParty> MobilePartyCreated
		{
			get
			{
				return CampaignEvents.Instance._mobilePartyCreated;
			}
		}

		// Token: 0x06000599 RID: 1433 RVA: 0x0001D1B8 File Offset: 0x0001B3B8
		public override void OnMobilePartyCreated(MobileParty party)
		{
			CampaignEvents.Instance._mobilePartyCreated.Invoke(party);
		}

		// Token: 0x170000E1 RID: 225
		// (get) Token: 0x0600059A RID: 1434 RVA: 0x0001D1CA File Offset: 0x0001B3CA
		public static IMbEvent<Hero, Hero, KillCharacterAction.KillCharacterActionDetail, bool> HeroKilledEvent
		{
			get
			{
				return CampaignEvents.Instance._heroKilled;
			}
		}

		// Token: 0x0600059B RID: 1435 RVA: 0x0001D1D6 File Offset: 0x0001B3D6
		public override void OnHeroKilled(Hero victim, Hero killer, KillCharacterAction.KillCharacterActionDetail detail, bool showNotification = true)
		{
			CampaignEvents.Instance._heroKilled.Invoke(victim, killer, detail, showNotification);
		}

		// Token: 0x170000E2 RID: 226
		// (get) Token: 0x0600059C RID: 1436 RVA: 0x0001D1EC File Offset: 0x0001B3EC
		public static IMbEvent<Hero, Hero, KillCharacterAction.KillCharacterActionDetail, bool> BeforeHeroKilledEvent
		{
			get
			{
				return CampaignEvents.Instance._onBeforeHeroKilled;
			}
		}

		// Token: 0x0600059D RID: 1437 RVA: 0x0001D1F8 File Offset: 0x0001B3F8
		public override void OnBeforeHeroKilled(Hero victim, Hero killer, KillCharacterAction.KillCharacterActionDetail detail, bool showNotification = true)
		{
			CampaignEvents.Instance._onBeforeHeroKilled.Invoke(victim, killer, detail, showNotification);
		}

		// Token: 0x170000E3 RID: 227
		// (get) Token: 0x0600059E RID: 1438 RVA: 0x0001D20E File Offset: 0x0001B40E
		public static IMbEvent<Hero, int> ChildEducationCompletedEvent
		{
			get
			{
				return CampaignEvents.Instance._childEducationCompleted;
			}
		}

		// Token: 0x0600059F RID: 1439 RVA: 0x0001D21A File Offset: 0x0001B41A
		public override void OnChildEducationCompleted(Hero hero, int age)
		{
			CampaignEvents.Instance._childEducationCompleted.Invoke(hero, age);
		}

		// Token: 0x170000E4 RID: 228
		// (get) Token: 0x060005A0 RID: 1440 RVA: 0x0001D22D File Offset: 0x0001B42D
		public static IMbEvent<Hero> HeroComesOfAgeEvent
		{
			get
			{
				return CampaignEvents.Instance._heroComesOfAge;
			}
		}

		// Token: 0x060005A1 RID: 1441 RVA: 0x0001D239 File Offset: 0x0001B439
		public override void OnHeroComesOfAge(Hero hero)
		{
			CampaignEvents.Instance._heroComesOfAge.Invoke(hero);
		}

		// Token: 0x170000E5 RID: 229
		// (get) Token: 0x060005A2 RID: 1442 RVA: 0x0001D24B File Offset: 0x0001B44B
		public static IMbEvent<Hero> HeroGrowsOutOfInfancyEvent
		{
			get
			{
				return CampaignEvents.Instance._heroGrowsOutOfInfancyEvent;
			}
		}

		// Token: 0x060005A3 RID: 1443 RVA: 0x0001D257 File Offset: 0x0001B457
		public override void OnHeroGrowsOutOfInfancy(Hero hero)
		{
			CampaignEvents.Instance._heroGrowsOutOfInfancyEvent.Invoke(hero);
		}

		// Token: 0x170000E6 RID: 230
		// (get) Token: 0x060005A4 RID: 1444 RVA: 0x0001D269 File Offset: 0x0001B469
		public static IMbEvent<Hero> HeroReachesTeenAgeEvent
		{
			get
			{
				return CampaignEvents.Instance._heroReachesTeenAgeEvent;
			}
		}

		// Token: 0x060005A5 RID: 1445 RVA: 0x0001D275 File Offset: 0x0001B475
		public override void OnHeroReachesTeenAge(Hero hero)
		{
			CampaignEvents.Instance._heroReachesTeenAgeEvent.Invoke(hero);
		}

		// Token: 0x170000E7 RID: 231
		// (get) Token: 0x060005A6 RID: 1446 RVA: 0x0001D287 File Offset: 0x0001B487
		public static IMbEvent<Hero, Hero> CharacterDefeated
		{
			get
			{
				return CampaignEvents.Instance._characterDefeated;
			}
		}

		// Token: 0x060005A7 RID: 1447 RVA: 0x0001D293 File Offset: 0x0001B493
		public override void OnCharacterDefeated(Hero winner, Hero loser)
		{
			CampaignEvents.Instance._characterDefeated.Invoke(winner, loser);
		}

		// Token: 0x170000E8 RID: 232
		// (get) Token: 0x060005A8 RID: 1448 RVA: 0x0001D2A6 File Offset: 0x0001B4A6
		public static IMbEvent<Kingdom, Clan> RulingClanChanged
		{
			get
			{
				return CampaignEvents.Instance._rulingClanChanged;
			}
		}

		// Token: 0x060005A9 RID: 1449 RVA: 0x0001D2B2 File Offset: 0x0001B4B2
		public override void OnRulingClanChanged(Kingdom kingdom, Clan newRulingClan)
		{
			CampaignEvents.Instance._rulingClanChanged.Invoke(kingdom, newRulingClan);
		}

		// Token: 0x170000E9 RID: 233
		// (get) Token: 0x060005AA RID: 1450 RVA: 0x0001D2C5 File Offset: 0x0001B4C5
		public static IMbEvent<PartyBase, Hero> HeroPrisonerTaken
		{
			get
			{
				return CampaignEvents.Instance._heroPrisonerTaken;
			}
		}

		// Token: 0x060005AB RID: 1451 RVA: 0x0001D2D1 File Offset: 0x0001B4D1
		public override void OnHeroPrisonerTaken(PartyBase capturer, Hero prisoner)
		{
			CampaignEvents.Instance._heroPrisonerTaken.Invoke(capturer, prisoner);
		}

		// Token: 0x170000EA RID: 234
		// (get) Token: 0x060005AC RID: 1452 RVA: 0x0001D2E4 File Offset: 0x0001B4E4
		public static IMbEvent<Hero, PartyBase, IFaction, EndCaptivityDetail> HeroPrisonerReleased
		{
			get
			{
				return CampaignEvents.Instance._heroPrisonerReleased;
			}
		}

		// Token: 0x060005AD RID: 1453 RVA: 0x0001D2F0 File Offset: 0x0001B4F0
		public override void OnHeroPrisonerReleased(Hero prisoner, PartyBase party, IFaction capturerFaction, EndCaptivityDetail detail)
		{
			CampaignEvents.Instance._heroPrisonerReleased.Invoke(prisoner, party, capturerFaction, detail);
		}

		// Token: 0x170000EB RID: 235
		// (get) Token: 0x060005AE RID: 1454 RVA: 0x0001D306 File Offset: 0x0001B506
		public static IMbEvent<Hero> CharacterBecameFugitive
		{
			get
			{
				return CampaignEvents.Instance._characterBecameFugitive;
			}
		}

		// Token: 0x060005AF RID: 1455 RVA: 0x0001D312 File Offset: 0x0001B512
		public override void OnCharacterBecameFugitive(Hero hero)
		{
			CampaignEvents.Instance._characterBecameFugitive.Invoke(hero);
		}

		// Token: 0x170000EC RID: 236
		// (get) Token: 0x060005B0 RID: 1456 RVA: 0x0001D324 File Offset: 0x0001B524
		public static IMbEvent<Hero> OnPlayerMetHeroEvent
		{
			get
			{
				return CampaignEvents.Instance._playerMetHero;
			}
		}

		// Token: 0x060005B1 RID: 1457 RVA: 0x0001D330 File Offset: 0x0001B530
		public override void OnPlayerMetHero(Hero hero)
		{
			CampaignEvents.Instance._playerMetHero.Invoke(hero);
		}

		// Token: 0x170000ED RID: 237
		// (get) Token: 0x060005B2 RID: 1458 RVA: 0x0001D342 File Offset: 0x0001B542
		public static IMbEvent<Hero> OnPlayerLearnsAboutHeroEvent
		{
			get
			{
				return CampaignEvents.Instance._playerLearnsAboutHero;
			}
		}

		// Token: 0x060005B3 RID: 1459 RVA: 0x0001D34E File Offset: 0x0001B54E
		public override void OnPlayerLearnsAboutHero(Hero hero)
		{
			CampaignEvents.Instance._playerLearnsAboutHero.Invoke(hero);
		}

		// Token: 0x170000EE RID: 238
		// (get) Token: 0x060005B4 RID: 1460 RVA: 0x0001D360 File Offset: 0x0001B560
		public static IMbEvent<Hero, int, bool> RenownGained
		{
			get
			{
				return CampaignEvents.Instance._renownGained;
			}
		}

		// Token: 0x060005B5 RID: 1461 RVA: 0x0001D36C File Offset: 0x0001B56C
		public override void OnRenownGained(Hero hero, int gainedRenown, bool doNotNotify)
		{
			CampaignEvents.Instance._renownGained.Invoke(hero, gainedRenown, doNotNotify);
		}

		// Token: 0x170000EF RID: 239
		// (get) Token: 0x060005B6 RID: 1462 RVA: 0x0001D380 File Offset: 0x0001B580
		public static IMbEvent<IFaction, float> CrimeRatingChanged
		{
			get
			{
				return CampaignEvents.Instance._crimeRatingChanged;
			}
		}

		// Token: 0x060005B7 RID: 1463 RVA: 0x0001D38C File Offset: 0x0001B58C
		public override void OnCrimeRatingChanged(IFaction kingdom, float deltaCrimeAmount)
		{
			CampaignEvents.Instance._crimeRatingChanged.Invoke(kingdom, deltaCrimeAmount);
		}

		// Token: 0x170000F0 RID: 240
		// (get) Token: 0x060005B8 RID: 1464 RVA: 0x0001D39F File Offset: 0x0001B59F
		public static IMbEvent<Hero> NewCompanionAdded
		{
			get
			{
				return CampaignEvents.Instance._newCompanionAdded;
			}
		}

		// Token: 0x060005B9 RID: 1465 RVA: 0x0001D3AB File Offset: 0x0001B5AB
		public override void OnNewCompanionAdded(Hero newCompanion)
		{
			CampaignEvents.Instance._newCompanionAdded.Invoke(newCompanion);
		}

		// Token: 0x170000F1 RID: 241
		// (get) Token: 0x060005BA RID: 1466 RVA: 0x0001D3BD File Offset: 0x0001B5BD
		public static IMbEvent<IMission> AfterMissionStarted
		{
			get
			{
				return CampaignEvents.Instance._afterMissionStarted;
			}
		}

		// Token: 0x060005BB RID: 1467 RVA: 0x0001D3C9 File Offset: 0x0001B5C9
		public override void OnAfterMissionStarted(IMission iMission)
		{
			CampaignEvents.Instance._afterMissionStarted.Invoke(iMission);
		}

		// Token: 0x170000F2 RID: 242
		// (get) Token: 0x060005BC RID: 1468 RVA: 0x0001D3DB File Offset: 0x0001B5DB
		public static IMbEvent<MenuCallbackArgs> GameMenuOpened
		{
			get
			{
				return CampaignEvents.Instance._gameMenuOpened;
			}
		}

		// Token: 0x060005BD RID: 1469 RVA: 0x0001D3E7 File Offset: 0x0001B5E7
		public override void OnGameMenuOpened(MenuCallbackArgs args)
		{
			CampaignEvents.Instance._gameMenuOpened.Invoke(args);
		}

		// Token: 0x170000F3 RID: 243
		// (get) Token: 0x060005BE RID: 1470 RVA: 0x0001D3F9 File Offset: 0x0001B5F9
		public static IMbEvent<MenuCallbackArgs> AfterGameMenuOpenedEvent
		{
			get
			{
				return CampaignEvents.Instance._afterGameMenuOpenedEvent;
			}
		}

		// Token: 0x060005BF RID: 1471 RVA: 0x0001D405 File Offset: 0x0001B605
		public override void AfterGameMenuOpened(MenuCallbackArgs args)
		{
			CampaignEvents.Instance._afterGameMenuOpenedEvent.Invoke(args);
		}

		// Token: 0x170000F4 RID: 244
		// (get) Token: 0x060005C0 RID: 1472 RVA: 0x0001D417 File Offset: 0x0001B617
		public static IMbEvent<MenuCallbackArgs> BeforeGameMenuOpenedEvent
		{
			get
			{
				return CampaignEvents.Instance._beforeGameMenuOpenedEvent;
			}
		}

		// Token: 0x060005C1 RID: 1473 RVA: 0x0001D423 File Offset: 0x0001B623
		public override void BeforeGameMenuOpened(MenuCallbackArgs args)
		{
			CampaignEvents.Instance._beforeGameMenuOpenedEvent.Invoke(args);
		}

		// Token: 0x170000F5 RID: 245
		// (get) Token: 0x060005C2 RID: 1474 RVA: 0x0001D435 File Offset: 0x0001B635
		public static IMbEvent<IFaction, IFaction, MakePeaceAction.MakePeaceDetail> MakePeace
		{
			get
			{
				return CampaignEvents.Instance._makePeace;
			}
		}

		// Token: 0x060005C3 RID: 1475 RVA: 0x0001D441 File Offset: 0x0001B641
		public override void OnMakePeace(IFaction side1Faction, IFaction side2Faction, MakePeaceAction.MakePeaceDetail detail)
		{
			CampaignEvents.Instance._makePeace.Invoke(side1Faction, side2Faction, detail);
		}

		// Token: 0x170000F6 RID: 246
		// (get) Token: 0x060005C4 RID: 1476 RVA: 0x0001D455 File Offset: 0x0001B655
		public static IMbEvent<Kingdom> KingdomDestroyedEvent
		{
			get
			{
				return CampaignEvents.Instance._kingdomDestroyed;
			}
		}

		// Token: 0x060005C5 RID: 1477 RVA: 0x0001D461 File Offset: 0x0001B661
		public override void OnKingdomDestroyed(Kingdom destroyedKingdom)
		{
			CampaignEvents.Instance._kingdomDestroyed.Invoke(destroyedKingdom);
		}

		// Token: 0x170000F7 RID: 247
		// (get) Token: 0x060005C6 RID: 1478 RVA: 0x0001D473 File Offset: 0x0001B673
		public static IMbEvent<Kingdom> KingdomCreatedEvent
		{
			get
			{
				return CampaignEvents.Instance._kingdomCreated;
			}
		}

		// Token: 0x060005C7 RID: 1479 RVA: 0x0001D47F File Offset: 0x0001B67F
		public override void OnKingdomCreated(Kingdom createdKingdom)
		{
			CampaignEvents.Instance._kingdomCreated.Invoke(createdKingdom);
		}

		// Token: 0x170000F8 RID: 248
		// (get) Token: 0x060005C8 RID: 1480 RVA: 0x0001D491 File Offset: 0x0001B691
		public static IMbEvent<Village> VillageBecomeNormal
		{
			get
			{
				return CampaignEvents.Instance._villageBecomeNormal;
			}
		}

		// Token: 0x060005C9 RID: 1481 RVA: 0x0001D49D File Offset: 0x0001B69D
		public override void OnVillageBecomeNormal(Village village)
		{
			CampaignEvents.Instance._villageBecomeNormal.Invoke(village);
		}

		// Token: 0x170000F9 RID: 249
		// (get) Token: 0x060005CA RID: 1482 RVA: 0x0001D4AF File Offset: 0x0001B6AF
		public static IMbEvent<Village> VillageBeingRaided
		{
			get
			{
				return CampaignEvents.Instance._villageBeingRaided;
			}
		}

		// Token: 0x060005CB RID: 1483 RVA: 0x0001D4BB File Offset: 0x0001B6BB
		public override void OnVillageBeingRaided(Village village)
		{
			CampaignEvents.Instance._villageBeingRaided.Invoke(village);
		}

		// Token: 0x170000FA RID: 250
		// (get) Token: 0x060005CC RID: 1484 RVA: 0x0001D4CD File Offset: 0x0001B6CD
		public static IMbEvent<Village> VillageLooted
		{
			get
			{
				return CampaignEvents.Instance._villageLooted;
			}
		}

		// Token: 0x060005CD RID: 1485 RVA: 0x0001D4D9 File Offset: 0x0001B6D9
		public override void OnVillageLooted(Village village)
		{
			CampaignEvents.Instance._villageLooted.Invoke(village);
		}

		// Token: 0x170000FB RID: 251
		// (get) Token: 0x060005CE RID: 1486 RVA: 0x0001D4EB File Offset: 0x0001B6EB
		public static IMbEvent<Hero, RemoveCompanionAction.RemoveCompanionDetail> CompanionRemoved
		{
			get
			{
				return CampaignEvents.Instance._companionRemoved;
			}
		}

		// Token: 0x060005CF RID: 1487 RVA: 0x0001D4F7 File Offset: 0x0001B6F7
		public override void OnCompanionRemoved(Hero companion, RemoveCompanionAction.RemoveCompanionDetail detail)
		{
			CampaignEvents.Instance._companionRemoved.Invoke(companion, detail);
		}

		// Token: 0x170000FC RID: 252
		// (get) Token: 0x060005D0 RID: 1488 RVA: 0x0001D50A File Offset: 0x0001B70A
		public static IMbEvent<IAgent> OnAgentJoinedConversationEvent
		{
			get
			{
				return CampaignEvents.Instance._onAgentJoinedConversationEvent;
			}
		}

		// Token: 0x060005D1 RID: 1489 RVA: 0x0001D516 File Offset: 0x0001B716
		public override void OnAgentJoinedConversation(IAgent agent)
		{
			CampaignEvents.Instance._onAgentJoinedConversationEvent.Invoke(agent);
		}

		// Token: 0x170000FD RID: 253
		// (get) Token: 0x060005D2 RID: 1490 RVA: 0x0001D528 File Offset: 0x0001B728
		public static IMbEvent<IEnumerable<CharacterObject>> ConversationEnded
		{
			get
			{
				return CampaignEvents.Instance._onConversationEnded;
			}
		}

		// Token: 0x060005D3 RID: 1491 RVA: 0x0001D534 File Offset: 0x0001B734
		public override void OnConversationEnded(IEnumerable<CharacterObject> characters)
		{
			CampaignEvents.Instance._onConversationEnded.Invoke(characters);
		}

		// Token: 0x170000FE RID: 254
		// (get) Token: 0x060005D4 RID: 1492 RVA: 0x0001D546 File Offset: 0x0001B746
		public static IMbEvent<MapEvent> MapEventEnded
		{
			get
			{
				return CampaignEvents.Instance._mapEventEnded;
			}
		}

		// Token: 0x060005D5 RID: 1493 RVA: 0x0001D552 File Offset: 0x0001B752
		public override void OnMapEventEnded(MapEvent mapEvent)
		{
			CampaignEvents.Instance._mapEventEnded.Invoke(mapEvent);
		}

		// Token: 0x170000FF RID: 255
		// (get) Token: 0x060005D6 RID: 1494 RVA: 0x0001D564 File Offset: 0x0001B764
		public static IMbEvent<MapEvent, PartyBase, PartyBase> MapEventStarted
		{
			get
			{
				return CampaignEvents.Instance._mapEventStarted;
			}
		}

		// Token: 0x060005D7 RID: 1495 RVA: 0x0001D570 File Offset: 0x0001B770
		public override void OnMapEventStarted(MapEvent mapEvent, PartyBase attackerParty, PartyBase defenderParty)
		{
			CampaignEvents.Instance._mapEventStarted.Invoke(mapEvent, attackerParty, defenderParty);
		}

		// Token: 0x17000100 RID: 256
		// (get) Token: 0x060005D8 RID: 1496 RVA: 0x0001D584 File Offset: 0x0001B784
		public static IMbEvent<Settlement, FlattenedTroopRoster, Hero, bool> PrisonersChangeInSettlement
		{
			get
			{
				return CampaignEvents.Instance._prisonersChangeInSettlement;
			}
		}

		// Token: 0x060005D9 RID: 1497 RVA: 0x0001D590 File Offset: 0x0001B790
		public override void OnPrisonersChangeInSettlement(Settlement settlement, FlattenedTroopRoster prisonerRoster, Hero prisonerHero, bool takenFromDungeon)
		{
			CampaignEvents.Instance._prisonersChangeInSettlement.Invoke(settlement, prisonerRoster, prisonerHero, takenFromDungeon);
		}

		// Token: 0x17000101 RID: 257
		// (get) Token: 0x060005DA RID: 1498 RVA: 0x0001D5A6 File Offset: 0x0001B7A6
		public static IMbEvent<Hero, BoardGameHelper.BoardGameState> OnPlayerBoardGameOverEvent
		{
			get
			{
				return CampaignEvents.Instance._onPlayerBoardGameOver;
			}
		}

		// Token: 0x060005DB RID: 1499 RVA: 0x0001D5B2 File Offset: 0x0001B7B2
		public override void OnPlayerBoardGameOver(Hero opposingHero, BoardGameHelper.BoardGameState state)
		{
			CampaignEvents.Instance._onPlayerBoardGameOver.Invoke(opposingHero, state);
		}

		// Token: 0x17000102 RID: 258
		// (get) Token: 0x060005DC RID: 1500 RVA: 0x0001D5C5 File Offset: 0x0001B7C5
		public static IMbEvent<Hero> OnRansomOfferedToPlayerEvent
		{
			get
			{
				return CampaignEvents.Instance._onRansomOfferedToPlayer;
			}
		}

		// Token: 0x060005DD RID: 1501 RVA: 0x0001D5D1 File Offset: 0x0001B7D1
		public override void OnRansomOfferedToPlayer(Hero captiveHero)
		{
			CampaignEvents.Instance._onRansomOfferedToPlayer.Invoke(captiveHero);
		}

		// Token: 0x17000103 RID: 259
		// (get) Token: 0x060005DE RID: 1502 RVA: 0x0001D5E3 File Offset: 0x0001B7E3
		public static IMbEvent<Hero> OnRansomOfferCancelledEvent
		{
			get
			{
				return CampaignEvents.Instance._onRansomOfferCancelled;
			}
		}

		// Token: 0x060005DF RID: 1503 RVA: 0x0001D5EF File Offset: 0x0001B7EF
		public override void OnRansomOfferCancelled(Hero captiveHero)
		{
			CampaignEvents.Instance._onRansomOfferCancelled.Invoke(captiveHero);
		}

		// Token: 0x17000104 RID: 260
		// (get) Token: 0x060005E0 RID: 1504 RVA: 0x0001D601 File Offset: 0x0001B801
		public static IMbEvent<IFaction, int> OnPeaceOfferedToPlayerEvent
		{
			get
			{
				return CampaignEvents.Instance._onPeaceOfferedToPlayer;
			}
		}

		// Token: 0x060005E1 RID: 1505 RVA: 0x0001D60D File Offset: 0x0001B80D
		public override void OnPeaceOfferedToPlayer(IFaction opponentFaction, int tributeAmount)
		{
			CampaignEvents.Instance._onPeaceOfferedToPlayer.Invoke(opponentFaction, tributeAmount);
		}

		// Token: 0x17000105 RID: 261
		// (get) Token: 0x060005E2 RID: 1506 RVA: 0x0001D620 File Offset: 0x0001B820
		public static IMbEvent<IFaction> OnPeaceOfferCancelledEvent
		{
			get
			{
				return CampaignEvents.Instance._onPeaceOfferCancelled;
			}
		}

		// Token: 0x060005E3 RID: 1507 RVA: 0x0001D62C File Offset: 0x0001B82C
		public override void OnPeaceOfferCancelled(IFaction opponentFaction)
		{
			CampaignEvents.Instance._onPeaceOfferCancelled.Invoke(opponentFaction);
		}

		// Token: 0x17000106 RID: 262
		// (get) Token: 0x060005E4 RID: 1508 RVA: 0x0001D63E File Offset: 0x0001B83E
		public static IMbEvent<Hero, Hero> OnMarriageOfferedToPlayerEvent
		{
			get
			{
				return CampaignEvents.Instance._onMarriageOfferedToPlayerEvent;
			}
		}

		// Token: 0x060005E5 RID: 1509 RVA: 0x0001D64A File Offset: 0x0001B84A
		public override void OnMarriageOfferedToPlayer(Hero suitor, Hero maiden)
		{
			CampaignEvents.Instance._onMarriageOfferedToPlayerEvent.Invoke(suitor, maiden);
		}

		// Token: 0x17000107 RID: 263
		// (get) Token: 0x060005E6 RID: 1510 RVA: 0x0001D65D File Offset: 0x0001B85D
		public static IMbEvent<Hero, Hero> OnMarriageOfferCanceledEvent
		{
			get
			{
				return CampaignEvents.Instance._onMarriageOfferCanceledEvent;
			}
		}

		// Token: 0x060005E7 RID: 1511 RVA: 0x0001D669 File Offset: 0x0001B869
		public override void OnMarriageOfferCanceled(Hero suitor, Hero maiden)
		{
			CampaignEvents.Instance._onMarriageOfferCanceledEvent.Invoke(suitor, maiden);
		}

		// Token: 0x17000108 RID: 264
		// (get) Token: 0x060005E8 RID: 1512 RVA: 0x0001D67C File Offset: 0x0001B87C
		public static IMbEvent<Kingdom> OnVassalOrMercenaryServiceOfferedToPlayerEvent
		{
			get
			{
				return CampaignEvents.Instance._onVassalOrMercenaryServiceOfferedToPlayerEvent;
			}
		}

		// Token: 0x060005E9 RID: 1513 RVA: 0x0001D688 File Offset: 0x0001B888
		public override void OnVassalOrMercenaryServiceOfferedToPlayer(Kingdom offeredKingdom)
		{
			CampaignEvents.Instance._onVassalOrMercenaryServiceOfferedToPlayerEvent.Invoke(offeredKingdom);
		}

		// Token: 0x17000109 RID: 265
		// (get) Token: 0x060005EA RID: 1514 RVA: 0x0001D69A File Offset: 0x0001B89A
		public static IMbEvent<Kingdom> OnVassalOrMercenaryServiceOfferCanceledEvent
		{
			get
			{
				return CampaignEvents.Instance._onVassalOrMercenaryServiceOfferCanceledEvent;
			}
		}

		// Token: 0x060005EB RID: 1515 RVA: 0x0001D6A6 File Offset: 0x0001B8A6
		public override void OnVassalOrMercenaryServiceOfferCanceled(Kingdom offeredKingdom)
		{
			CampaignEvents.Instance._onVassalOrMercenaryServiceOfferCanceledEvent.Invoke(offeredKingdom);
		}

		// Token: 0x1700010A RID: 266
		// (get) Token: 0x060005EC RID: 1516 RVA: 0x0001D6B8 File Offset: 0x0001B8B8
		public static IMbEvent<IMission> OnMissionStartedEvent
		{
			get
			{
				return CampaignEvents.Instance._onMissionStartedEvent;
			}
		}

		// Token: 0x060005ED RID: 1517 RVA: 0x0001D6C4 File Offset: 0x0001B8C4
		public override void OnMissionStarted(IMission mission)
		{
			CampaignEvents.Instance._onMissionStartedEvent.Invoke(mission);
		}

		// Token: 0x1700010B RID: 267
		// (get) Token: 0x060005EE RID: 1518 RVA: 0x0001D6D6 File Offset: 0x0001B8D6
		public static IMbEvent BeforeMissionOpenedEvent
		{
			get
			{
				return CampaignEvents.Instance._beforeMissionOpenedEvent;
			}
		}

		// Token: 0x060005EF RID: 1519 RVA: 0x0001D6E2 File Offset: 0x0001B8E2
		public override void BeforeMissionOpened()
		{
			CampaignEvents.Instance._beforeMissionOpenedEvent.Invoke();
		}

		// Token: 0x1700010C RID: 268
		// (get) Token: 0x060005F0 RID: 1520 RVA: 0x0001D6F3 File Offset: 0x0001B8F3
		public static IMbEvent<PartyBase> OnPartyRemovedEvent
		{
			get
			{
				return CampaignEvents.Instance._onPartyRemovedEvent;
			}
		}

		// Token: 0x060005F1 RID: 1521 RVA: 0x0001D6FF File Offset: 0x0001B8FF
		public override void OnPartyRemoved(PartyBase party)
		{
			CampaignEvents.Instance._onPartyRemovedEvent.Invoke(party);
		}

		// Token: 0x1700010D RID: 269
		// (get) Token: 0x060005F2 RID: 1522 RVA: 0x0001D711 File Offset: 0x0001B911
		public static IMbEvent<PartyBase> OnPartySizeChangedEvent
		{
			get
			{
				return CampaignEvents.Instance._onPartySizeChangedEvent;
			}
		}

		// Token: 0x060005F3 RID: 1523 RVA: 0x0001D71D File Offset: 0x0001B91D
		public override void OnPartySizeChanged(PartyBase party)
		{
			CampaignEvents.Instance._onPartySizeChangedEvent.Invoke(party);
		}

		// Token: 0x1700010E RID: 270
		// (get) Token: 0x060005F4 RID: 1524 RVA: 0x0001D72F File Offset: 0x0001B92F
		public static IMbEvent<Settlement, bool, Hero, Hero, Hero, ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail> OnSettlementOwnerChangedEvent
		{
			get
			{
				return CampaignEvents.Instance._onSettlementOwnerChangedEvent;
			}
		}

		// Token: 0x060005F5 RID: 1525 RVA: 0x0001D73B File Offset: 0x0001B93B
		public override void OnSettlementOwnerChanged(Settlement settlement, bool openToClaim, Hero newOwner, Hero oldOwner, Hero capturerHero, ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail detail)
		{
			CampaignEvents.Instance._onSettlementOwnerChangedEvent.Invoke(settlement, openToClaim, newOwner, oldOwner, capturerHero, detail);
		}

		// Token: 0x1700010F RID: 271
		// (get) Token: 0x060005F6 RID: 1526 RVA: 0x0001D755 File Offset: 0x0001B955
		public static IMbEvent<Town, Hero, Hero> OnGovernorChangedEvent
		{
			get
			{
				return CampaignEvents.Instance._onGovernorChangedEvent;
			}
		}

		// Token: 0x060005F7 RID: 1527 RVA: 0x0001D761 File Offset: 0x0001B961
		public override void OnGovernorChanged(Town fortification, Hero oldGovernor, Hero newGovernor)
		{
			CampaignEvents.Instance._onGovernorChangedEvent.Invoke(fortification, oldGovernor, newGovernor);
		}

		// Token: 0x17000110 RID: 272
		// (get) Token: 0x060005F8 RID: 1528 RVA: 0x0001D775 File Offset: 0x0001B975
		public static IMbEvent<MobileParty, Settlement> OnSettlementLeftEvent
		{
			get
			{
				return CampaignEvents.Instance._onSettlementLeftEvent;
			}
		}

		// Token: 0x060005F9 RID: 1529 RVA: 0x0001D781 File Offset: 0x0001B981
		public override void OnSettlementLeft(MobileParty party, Settlement settlement)
		{
			CampaignEvents.Instance._onSettlementLeftEvent.Invoke(party, settlement);
		}

		// Token: 0x17000111 RID: 273
		// (get) Token: 0x060005FA RID: 1530 RVA: 0x0001D794 File Offset: 0x0001B994
		public static IMbEvent WeeklyTickEvent
		{
			get
			{
				return CampaignEvents.Instance._weeklyTickEvent;
			}
		}

		// Token: 0x060005FB RID: 1531 RVA: 0x0001D7A0 File Offset: 0x0001B9A0
		public override void WeeklyTick()
		{
			CampaignEvents.Instance._weeklyTickEvent.Invoke();
		}

		// Token: 0x17000112 RID: 274
		// (get) Token: 0x060005FC RID: 1532 RVA: 0x0001D7B1 File Offset: 0x0001B9B1
		public static IMbEvent DailyTickEvent
		{
			get
			{
				return CampaignEvents.Instance._dailyTickEvent;
			}
		}

		// Token: 0x060005FD RID: 1533 RVA: 0x0001D7BD File Offset: 0x0001B9BD
		public override void DailyTick()
		{
			CampaignEvents.Instance._dailyTickEvent.Invoke();
		}

		// Token: 0x17000113 RID: 275
		// (get) Token: 0x060005FE RID: 1534 RVA: 0x0001D7CE File Offset: 0x0001B9CE
		public static IMbEvent<MobileParty> DailyTickPartyEvent
		{
			get
			{
				return CampaignEvents.Instance._dailyTickPartyEvent;
			}
		}

		// Token: 0x060005FF RID: 1535 RVA: 0x0001D7DA File Offset: 0x0001B9DA
		public override void DailyTickParty(MobileParty mobileParty)
		{
			CampaignEvents.Instance._dailyTickPartyEvent.Invoke(mobileParty);
		}

		// Token: 0x17000114 RID: 276
		// (get) Token: 0x06000600 RID: 1536 RVA: 0x0001D7EC File Offset: 0x0001B9EC
		public static IMbEvent<Town> DailyTickTownEvent
		{
			get
			{
				return CampaignEvents.Instance._dailyTickTownEvent;
			}
		}

		// Token: 0x06000601 RID: 1537 RVA: 0x0001D7F8 File Offset: 0x0001B9F8
		public override void DailyTickTown(Town town)
		{
			CampaignEvents.Instance._dailyTickTownEvent.Invoke(town);
		}

		// Token: 0x17000115 RID: 277
		// (get) Token: 0x06000602 RID: 1538 RVA: 0x0001D80A File Offset: 0x0001BA0A
		public static IMbEvent<Settlement> DailyTickSettlementEvent
		{
			get
			{
				return CampaignEvents.Instance._dailyTickSettlementEvent;
			}
		}

		// Token: 0x06000603 RID: 1539 RVA: 0x0001D816 File Offset: 0x0001BA16
		public override void DailyTickSettlement(Settlement settlement)
		{
			CampaignEvents.Instance._dailyTickSettlementEvent.Invoke(settlement);
		}

		// Token: 0x17000116 RID: 278
		// (get) Token: 0x06000604 RID: 1540 RVA: 0x0001D828 File Offset: 0x0001BA28
		public static IMbEvent<Hero> DailyTickHeroEvent
		{
			get
			{
				return CampaignEvents.Instance._dailyTickHeroEvent;
			}
		}

		// Token: 0x06000605 RID: 1541 RVA: 0x0001D834 File Offset: 0x0001BA34
		public override void DailyTickHero(Hero hero)
		{
			CampaignEvents.Instance._dailyTickHeroEvent.Invoke(hero);
		}

		// Token: 0x17000117 RID: 279
		// (get) Token: 0x06000606 RID: 1542 RVA: 0x0001D846 File Offset: 0x0001BA46
		public static IMbEvent<Clan> DailyTickClanEvent
		{
			get
			{
				return CampaignEvents.Instance._dailyTickClanEvent;
			}
		}

		// Token: 0x06000607 RID: 1543 RVA: 0x0001D852 File Offset: 0x0001BA52
		public override void DailyTickClan(Clan clan)
		{
			CampaignEvents.Instance._dailyTickClanEvent.Invoke(clan);
		}

		// Token: 0x17000118 RID: 280
		// (get) Token: 0x06000608 RID: 1544 RVA: 0x0001D864 File Offset: 0x0001BA64
		public static IMbEvent<List<CampaignTutorial>> CollectAvailableTutorialsEvent
		{
			get
			{
				return CampaignEvents.Instance._collectAvailableTutorialsEvent;
			}
		}

		// Token: 0x06000609 RID: 1545 RVA: 0x0001D870 File Offset: 0x0001BA70
		public override void CollectAvailableTutorials(ref List<CampaignTutorial> tutorials)
		{
			CampaignEvents.Instance._collectAvailableTutorialsEvent.Invoke(tutorials);
		}

		// Token: 0x17000119 RID: 281
		// (get) Token: 0x0600060A RID: 1546 RVA: 0x0001D883 File Offset: 0x0001BA83
		public static IMbEvent<string> OnTutorialCompletedEvent
		{
			get
			{
				return CampaignEvents.Instance._onTutorialCompletedEvent;
			}
		}

		// Token: 0x0600060B RID: 1547 RVA: 0x0001D88F File Offset: 0x0001BA8F
		public override void OnTutorialCompleted(string tutorial)
		{
			CampaignEvents.Instance._onTutorialCompletedEvent.Invoke(tutorial);
		}

		// Token: 0x1700011A RID: 282
		// (get) Token: 0x0600060C RID: 1548 RVA: 0x0001D8A1 File Offset: 0x0001BAA1
		public static IMbEvent<Town, Building, int> OnBuildingLevelChangedEvent
		{
			get
			{
				return CampaignEvents.Instance._onBuildingLevelChangedEvent;
			}
		}

		// Token: 0x0600060D RID: 1549 RVA: 0x0001D8AD File Offset: 0x0001BAAD
		public override void OnBuildingLevelChanged(Town town, Building building, int levelChange)
		{
			CampaignEvents.Instance._onBuildingLevelChangedEvent.Invoke(town, building, levelChange);
		}

		// Token: 0x1700011B RID: 283
		// (get) Token: 0x0600060E RID: 1550 RVA: 0x0001D8C1 File Offset: 0x0001BAC1
		public static IMbEvent HourlyTickEvent
		{
			get
			{
				return CampaignEvents.Instance._hourlyTickEvent;
			}
		}

		// Token: 0x0600060F RID: 1551 RVA: 0x0001D8CD File Offset: 0x0001BACD
		public override void HourlyTick()
		{
			CampaignEvents.Instance._hourlyTickEvent.Invoke();
		}

		// Token: 0x1700011C RID: 284
		// (get) Token: 0x06000610 RID: 1552 RVA: 0x0001D8DE File Offset: 0x0001BADE
		public static IMbEvent<MobileParty> HourlyTickPartyEvent
		{
			get
			{
				return CampaignEvents.Instance._hourlyTickPartyEvent;
			}
		}

		// Token: 0x06000611 RID: 1553 RVA: 0x0001D8EA File Offset: 0x0001BAEA
		public override void HourlyTickParty(MobileParty mobileParty)
		{
			CampaignEvents.Instance._hourlyTickPartyEvent.Invoke(mobileParty);
		}

		// Token: 0x1700011D RID: 285
		// (get) Token: 0x06000612 RID: 1554 RVA: 0x0001D8FC File Offset: 0x0001BAFC
		public static IMbEvent<Settlement> HourlyTickSettlementEvent
		{
			get
			{
				return CampaignEvents.Instance._hourlyTickSettlementEvent;
			}
		}

		// Token: 0x06000613 RID: 1555 RVA: 0x0001D908 File Offset: 0x0001BB08
		public override void HourlyTickSettlement(Settlement settlement)
		{
			CampaignEvents.Instance._hourlyTickSettlementEvent.Invoke(settlement);
		}

		// Token: 0x1700011E RID: 286
		// (get) Token: 0x06000614 RID: 1556 RVA: 0x0001D91A File Offset: 0x0001BB1A
		public static IMbEvent<Clan> HourlyTickClanEvent
		{
			get
			{
				return CampaignEvents.Instance._hourlyTickClanEvent;
			}
		}

		// Token: 0x06000615 RID: 1557 RVA: 0x0001D926 File Offset: 0x0001BB26
		public override void HourlyTickClan(Clan clan)
		{
			CampaignEvents.Instance._hourlyTickClanEvent.Invoke(clan);
		}

		// Token: 0x1700011F RID: 287
		// (get) Token: 0x06000616 RID: 1558 RVA: 0x0001D938 File Offset: 0x0001BB38
		public static IMbEvent<float> TickEvent
		{
			get
			{
				return CampaignEvents.Instance._tickEvent;
			}
		}

		// Token: 0x06000617 RID: 1559 RVA: 0x0001D944 File Offset: 0x0001BB44
		public override void Tick(float dt)
		{
			CampaignEvents.Instance._tickEvent.Invoke(dt);
		}

		// Token: 0x17000120 RID: 288
		// (get) Token: 0x06000618 RID: 1560 RVA: 0x0001D956 File Offset: 0x0001BB56
		public static IMbEvent<CampaignGameStarter> OnSessionLaunchedEvent
		{
			get
			{
				return CampaignEvents.Instance._onSessionLaunchedEvent;
			}
		}

		// Token: 0x06000619 RID: 1561 RVA: 0x0001D962 File Offset: 0x0001BB62
		public override void OnSessionStart(CampaignGameStarter campaignGameStarter)
		{
			CampaignEvents.Instance._onSessionLaunchedEvent.Invoke(campaignGameStarter);
		}

		// Token: 0x17000121 RID: 289
		// (get) Token: 0x0600061A RID: 1562 RVA: 0x0001D974 File Offset: 0x0001BB74
		public static IMbEvent<CampaignGameStarter> OnAfterSessionLaunchedEvent
		{
			get
			{
				return CampaignEvents.Instance._onAfterSessionLaunchedEvent;
			}
		}

		// Token: 0x0600061B RID: 1563 RVA: 0x0001D980 File Offset: 0x0001BB80
		public override void OnAfterSessionStart(CampaignGameStarter campaignGameStarter)
		{
			CampaignEvents.Instance._onAfterSessionLaunchedEvent.Invoke(campaignGameStarter);
		}

		// Token: 0x17000122 RID: 290
		// (get) Token: 0x0600061C RID: 1564 RVA: 0x0001D992 File Offset: 0x0001BB92
		public static IMbEvent<CampaignGameStarter> OnNewGameCreatedEvent
		{
			get
			{
				return CampaignEvents.Instance._onNewGameCreatedEvent;
			}
		}

		// Token: 0x17000123 RID: 291
		// (get) Token: 0x0600061D RID: 1565 RVA: 0x0001D99E File Offset: 0x0001BB9E
		public static IMbEvent<CampaignGameStarter, int> OnNewGameCreatedPartialFollowUpEvent
		{
			get
			{
				return CampaignEvents.Instance._onNewGameCreatedPartialFollowUpEvent;
			}
		}

		// Token: 0x0600061E RID: 1566 RVA: 0x0001D9AC File Offset: 0x0001BBAC
		public override void OnNewGameCreated(CampaignGameStarter campaignGameStarter)
		{
			CampaignEvents.Instance._onNewGameCreatedEvent.Invoke(campaignGameStarter);
			for (int i = 0; i < 100; i++)
			{
				CampaignEvents.Instance._onNewGameCreatedPartialFollowUpEvent.Invoke(campaignGameStarter, i);
			}
			CampaignEvents.Instance._onNewGameCreatedPartialFollowUpEndEvent.Invoke(campaignGameStarter);
		}

		// Token: 0x17000124 RID: 292
		// (get) Token: 0x0600061F RID: 1567 RVA: 0x0001D9F7 File Offset: 0x0001BBF7
		public static IMbEvent<CampaignGameStarter> OnNewGameCreatedPartialFollowUpEndEvent
		{
			get
			{
				return CampaignEvents.Instance._onNewGameCreatedPartialFollowUpEndEvent;
			}
		}

		// Token: 0x17000125 RID: 293
		// (get) Token: 0x06000620 RID: 1568 RVA: 0x0001DA03 File Offset: 0x0001BC03
		public static IMbEvent<CampaignGameStarter> OnGameEarlyLoadedEvent
		{
			get
			{
				return CampaignEvents.Instance._onGameEarlyLoadedEvent;
			}
		}

		// Token: 0x06000621 RID: 1569 RVA: 0x0001DA0F File Offset: 0x0001BC0F
		public override void OnGameEarlyLoaded(CampaignGameStarter campaignGameStarter)
		{
			CampaignEvents.Instance._onGameEarlyLoadedEvent.Invoke(campaignGameStarter);
		}

		// Token: 0x17000126 RID: 294
		// (get) Token: 0x06000622 RID: 1570 RVA: 0x0001DA21 File Offset: 0x0001BC21
		public static IMbEvent<CampaignGameStarter> OnGameLoadedEvent
		{
			get
			{
				return CampaignEvents.Instance._onGameLoadedEvent;
			}
		}

		// Token: 0x06000623 RID: 1571 RVA: 0x0001DA2D File Offset: 0x0001BC2D
		public override void OnGameLoaded(CampaignGameStarter campaignGameStarter)
		{
			CampaignEvents.Instance._onGameLoadedEvent.Invoke(campaignGameStarter);
		}

		// Token: 0x17000127 RID: 295
		// (get) Token: 0x06000624 RID: 1572 RVA: 0x0001DA3F File Offset: 0x0001BC3F
		public static IMbEvent OnGameLoadFinishedEvent
		{
			get
			{
				return CampaignEvents.Instance._onGameLoadFinishedEvent;
			}
		}

		// Token: 0x06000625 RID: 1573 RVA: 0x0001DA4B File Offset: 0x0001BC4B
		public override void OnGameLoadFinished()
		{
			CampaignEvents.Instance._onGameLoadFinishedEvent.Invoke();
		}

		// Token: 0x17000128 RID: 296
		// (get) Token: 0x06000626 RID: 1574 RVA: 0x0001DA5C File Offset: 0x0001BC5C
		public static IMbEvent<MobileParty, PartyThinkParams> AiHourlyTickEvent
		{
			get
			{
				return CampaignEvents.Instance._aiHourlyTickEvent;
			}
		}

		// Token: 0x06000627 RID: 1575 RVA: 0x0001DA68 File Offset: 0x0001BC68
		public override void AiHourlyTick(MobileParty party, PartyThinkParams partyThinkParams)
		{
			CampaignEvents.Instance._aiHourlyTickEvent.Invoke(party, partyThinkParams);
		}

		// Token: 0x17000129 RID: 297
		// (get) Token: 0x06000628 RID: 1576 RVA: 0x0001DA7B File Offset: 0x0001BC7B
		public static IMbEvent<MobileParty> TickPartialHourlyAiEvent
		{
			get
			{
				return CampaignEvents.Instance._tickPartialHourlyAiEvent;
			}
		}

		// Token: 0x06000629 RID: 1577 RVA: 0x0001DA87 File Offset: 0x0001BC87
		public override void TickPartialHourlyAi(MobileParty party)
		{
			CampaignEvents.Instance._tickPartialHourlyAiEvent.Invoke(party);
		}

		// Token: 0x1700012A RID: 298
		// (get) Token: 0x0600062A RID: 1578 RVA: 0x0001DA99 File Offset: 0x0001BC99
		public static IMbEvent<MobileParty> OnPartyJoinedArmyEvent
		{
			get
			{
				return CampaignEvents.Instance._onPartyJoinedArmyEvent;
			}
		}

		// Token: 0x0600062B RID: 1579 RVA: 0x0001DAA5 File Offset: 0x0001BCA5
		public override void OnPartyJoinedArmy(MobileParty mobileParty)
		{
			CampaignEvents.Instance._onPartyJoinedArmyEvent.Invoke(mobileParty);
		}

		// Token: 0x1700012B RID: 299
		// (get) Token: 0x0600062C RID: 1580 RVA: 0x0001DAB7 File Offset: 0x0001BCB7
		public static IMbEvent<MobileParty> PartyRemovedFromArmyEvent
		{
			get
			{
				return CampaignEvents.Instance._onPartyRemovedFromArmyEvent;
			}
		}

		// Token: 0x0600062D RID: 1581 RVA: 0x0001DAC3 File Offset: 0x0001BCC3
		public override void OnPartyRemovedFromArmy(MobileParty mobileParty)
		{
			CampaignEvents.Instance._onPartyRemovedFromArmyEvent.Invoke(mobileParty);
		}

		// Token: 0x1700012C RID: 300
		// (get) Token: 0x0600062E RID: 1582 RVA: 0x0001DAD5 File Offset: 0x0001BCD5
		public static IMbEvent<Hero, Army.ArmyLeaderThinkReason> OnArmyLeaderThinkEvent
		{
			get
			{
				return CampaignEvents.Instance._onArmyLeaderThinkEvent;
			}
		}

		// Token: 0x0600062F RID: 1583 RVA: 0x0001DAE1 File Offset: 0x0001BCE1
		public override void OnArmyLeaderThink(Hero hero, Army.ArmyLeaderThinkReason reason)
		{
			CampaignEvents.Instance._onArmyLeaderThinkEvent.Invoke(hero, reason);
		}

		// Token: 0x1700012D RID: 301
		// (get) Token: 0x06000630 RID: 1584 RVA: 0x0001DAF4 File Offset: 0x0001BCF4
		public static IMbEvent<IMission> OnMissionEndedEvent
		{
			get
			{
				return CampaignEvents.Instance._onMissionEndedEvent;
			}
		}

		// Token: 0x06000631 RID: 1585 RVA: 0x0001DB00 File Offset: 0x0001BD00
		public override void OnMissionEnded(IMission mission)
		{
			CampaignEvents.Instance._onMissionEndedEvent.Invoke(mission);
		}

		// Token: 0x1700012E RID: 302
		// (get) Token: 0x06000632 RID: 1586 RVA: 0x0001DB12 File Offset: 0x0001BD12
		public static IMbEvent<MobileParty> OnQuarterDailyPartyTick
		{
			get
			{
				return CampaignEvents.Instance._onQuarterDailyPartyTick;
			}
		}

		// Token: 0x06000633 RID: 1587 RVA: 0x0001DB1E File Offset: 0x0001BD1E
		public override void QuarterDailyPartyTick(MobileParty mobileParty)
		{
			CampaignEvents.Instance._onQuarterDailyPartyTick.Invoke(mobileParty);
		}

		// Token: 0x1700012F RID: 303
		// (get) Token: 0x06000634 RID: 1588 RVA: 0x0001DB30 File Offset: 0x0001BD30
		public static IMbEvent<MapEvent> OnPlayerBattleEndEvent
		{
			get
			{
				return CampaignEvents.Instance._onPlayerBattleEndEvent;
			}
		}

		// Token: 0x06000635 RID: 1589 RVA: 0x0001DB3C File Offset: 0x0001BD3C
		public override void OnPlayerBattleEnd(MapEvent mapEvent)
		{
			CampaignEvents.Instance._onPlayerBattleEndEvent.Invoke(mapEvent);
		}

		// Token: 0x17000130 RID: 304
		// (get) Token: 0x06000636 RID: 1590 RVA: 0x0001DB4E File Offset: 0x0001BD4E
		public static IMbEvent<CharacterObject, int> OnUnitRecruitedEvent
		{
			get
			{
				return CampaignEvents.Instance._onUnitRecruitedEvent;
			}
		}

		// Token: 0x06000637 RID: 1591 RVA: 0x0001DB5A File Offset: 0x0001BD5A
		public override void OnUnitRecruited(CharacterObject character, int amount)
		{
			CampaignEvents.Instance._onUnitRecruitedEvent.Invoke(character, amount);
		}

		// Token: 0x17000131 RID: 305
		// (get) Token: 0x06000638 RID: 1592 RVA: 0x0001DB6D File Offset: 0x0001BD6D
		public static IMbEvent<Hero> OnChildConceivedEvent
		{
			get
			{
				return CampaignEvents.Instance._onChildConceived;
			}
		}

		// Token: 0x06000639 RID: 1593 RVA: 0x0001DB79 File Offset: 0x0001BD79
		public override void OnChildConceived(Hero mother)
		{
			CampaignEvents.Instance._onChildConceived.Invoke(mother);
		}

		// Token: 0x17000132 RID: 306
		// (get) Token: 0x0600063A RID: 1594 RVA: 0x0001DB8B File Offset: 0x0001BD8B
		public static IMbEvent<Hero, List<Hero>, int> OnGivenBirthEvent
		{
			get
			{
				return CampaignEvents.Instance._onGivenBirthEvent;
			}
		}

		// Token: 0x0600063B RID: 1595 RVA: 0x0001DB97 File Offset: 0x0001BD97
		public override void OnGivenBirth(Hero mother, List<Hero> aliveChildren, int stillbornCount)
		{
			CampaignEvents.Instance._onGivenBirthEvent.Invoke(mother, aliveChildren, stillbornCount);
		}

		// Token: 0x17000133 RID: 307
		// (get) Token: 0x0600063C RID: 1596 RVA: 0x0001DBAB File Offset: 0x0001BDAB
		public static IMbEvent<float> MissionTickEvent
		{
			get
			{
				return CampaignEvents.Instance._missionTickEvent;
			}
		}

		// Token: 0x0600063D RID: 1597 RVA: 0x0001DBB7 File Offset: 0x0001BDB7
		public override void MissionTick(float dt)
		{
			CampaignEvents.Instance._missionTickEvent.Invoke(dt);
		}

		// Token: 0x17000134 RID: 308
		// (get) Token: 0x0600063E RID: 1598 RVA: 0x0001DBC9 File Offset: 0x0001BDC9
		public static IMbEvent SetupPreConversationEvent
		{
			get
			{
				return CampaignEvents.Instance._setupPreConversationEvent;
			}
		}

		// Token: 0x0600063F RID: 1599 RVA: 0x0001DBD5 File Offset: 0x0001BDD5
		public static void SetupPreConversation()
		{
			CampaignEvents.Instance._setupPreConversationEvent.Invoke();
		}

		// Token: 0x17000135 RID: 309
		// (get) Token: 0x06000640 RID: 1600 RVA: 0x0001DBE8 File Offset: 0x0001BDE8
		public static IMbEvent ArmyOverlaySetDirtyEvent
		{
			get
			{
				MbEvent mbEvent;
				if ((mbEvent = CampaignEvents.Instance._armyOverlaySetDirty) == null)
				{
					mbEvent = (CampaignEvents.Instance._armyOverlaySetDirty = new MbEvent());
				}
				return mbEvent;
			}
		}

		// Token: 0x06000641 RID: 1601 RVA: 0x0001DC15 File Offset: 0x0001BE15
		public override void OnArmyOverlaySetDirty()
		{
			if (CampaignEvents.Instance._armyOverlaySetDirty == null)
			{
				CampaignEvents.Instance._armyOverlaySetDirty = new MbEvent();
			}
			CampaignEvents.Instance._armyOverlaySetDirty.Invoke();
		}

		// Token: 0x17000136 RID: 310
		// (get) Token: 0x06000642 RID: 1602 RVA: 0x0001DC41 File Offset: 0x0001BE41
		public static IMbEvent<int> PlayerDesertedBattleEvent
		{
			get
			{
				return CampaignEvents.Instance._playerDesertedBattle;
			}
		}

		// Token: 0x06000643 RID: 1603 RVA: 0x0001DC4D File Offset: 0x0001BE4D
		public override void OnPlayerDesertedBattle(int sacrificedMenCount)
		{
			CampaignEvents.Instance._playerDesertedBattle.Invoke(sacrificedMenCount);
		}

		// Token: 0x17000137 RID: 311
		// (get) Token: 0x06000644 RID: 1604 RVA: 0x0001DC60 File Offset: 0x0001BE60
		public static IMbEvent<PartyBase> PartyVisibilityChangedEvent
		{
			get
			{
				MbEvent<PartyBase> mbEvent;
				if ((mbEvent = CampaignEvents.Instance._partyVisibilityChanged) == null)
				{
					mbEvent = (CampaignEvents.Instance._partyVisibilityChanged = new MbEvent<PartyBase>());
				}
				return mbEvent;
			}
		}

		// Token: 0x06000645 RID: 1605 RVA: 0x0001DC8D File Offset: 0x0001BE8D
		public override void OnPartyVisibilityChanged(PartyBase party)
		{
			if (CampaignEvents.Instance._partyVisibilityChanged == null)
			{
				CampaignEvents.Instance._partyVisibilityChanged = new MbEvent<PartyBase>();
			}
			CampaignEvents.Instance._partyVisibilityChanged.Invoke(party);
		}

		// Token: 0x17000138 RID: 312
		// (get) Token: 0x06000646 RID: 1606 RVA: 0x0001DCBA File Offset: 0x0001BEBA
		public static IMbEvent<Track> TrackDetectedEvent
		{
			get
			{
				return CampaignEvents.Instance._trackDetectedEvent;
			}
		}

		// Token: 0x06000647 RID: 1607 RVA: 0x0001DCC6 File Offset: 0x0001BEC6
		public override void TrackDetected(Track track)
		{
			CampaignEvents.Instance._trackDetectedEvent.Invoke(track);
		}

		// Token: 0x17000139 RID: 313
		// (get) Token: 0x06000648 RID: 1608 RVA: 0x0001DCD8 File Offset: 0x0001BED8
		public static IMbEvent<Track> TrackLostEvent
		{
			get
			{
				return CampaignEvents.Instance._trackLostEvent;
			}
		}

		// Token: 0x06000649 RID: 1609 RVA: 0x0001DCE4 File Offset: 0x0001BEE4
		public override void TrackLost(Track track)
		{
			CampaignEvents.Instance._trackLostEvent.Invoke(track);
		}

		// Token: 0x1700013A RID: 314
		// (get) Token: 0x0600064A RID: 1610 RVA: 0x0001DCF6 File Offset: 0x0001BEF6
		public static IMbEvent<Dictionary<string, int>> LocationCharactersAreReadyToSpawnEvent
		{
			get
			{
				return CampaignEvents.Instance._locationCharactersAreReadyToSpawn;
			}
		}

		// Token: 0x0600064B RID: 1611 RVA: 0x0001DD04 File Offset: 0x0001BF04
		public override void LocationCharactersAreReadyToSpawn(Dictionary<string, int> unusedUsablePointCount)
		{
			foreach (KeyValuePair<string, int> keyValuePair in unusedUsablePointCount)
			{
			}
			CampaignEvents.Instance._locationCharactersAreReadyToSpawn.Invoke(unusedUsablePointCount);
		}

		// Token: 0x1700013B RID: 315
		// (get) Token: 0x0600064C RID: 1612 RVA: 0x0001DD5C File Offset: 0x0001BF5C
		public static IMbEvent LocationCharactersSimulatedEvent
		{
			get
			{
				return CampaignEvents.Instance._locationCharactersSimulatedSpawned;
			}
		}

		// Token: 0x0600064D RID: 1613 RVA: 0x0001DD68 File Offset: 0x0001BF68
		public override void LocationCharactersSimulated()
		{
			CampaignEvents.Instance._locationCharactersSimulatedSpawned.Invoke();
		}

		// Token: 0x1700013C RID: 316
		// (get) Token: 0x0600064E RID: 1614 RVA: 0x0001DD79 File Offset: 0x0001BF79
		public static IMbEvent<CharacterObject, CharacterObject, int> PlayerUpgradedTroopsEvent
		{
			get
			{
				return CampaignEvents.Instance._playerUpgradedTroopsEvent;
			}
		}

		// Token: 0x0600064F RID: 1615 RVA: 0x0001DD85 File Offset: 0x0001BF85
		public override void OnPlayerUpgradedTroops(CharacterObject upgradeFromTroop, CharacterObject upgradeToTroop, int number)
		{
			CampaignEvents.Instance._playerUpgradedTroopsEvent.Invoke(upgradeFromTroop, upgradeToTroop, number);
		}

		// Token: 0x1700013D RID: 317
		// (get) Token: 0x06000650 RID: 1616 RVA: 0x0001DD99 File Offset: 0x0001BF99
		public static IMbEvent<CharacterObject, CharacterObject, PartyBase, WeaponComponentData, bool, int> OnHeroCombatHitEvent
		{
			get
			{
				return CampaignEvents.Instance._onHeroCombatHitEvent;
			}
		}

		// Token: 0x06000651 RID: 1617 RVA: 0x0001DDA5 File Offset: 0x0001BFA5
		public override void OnHeroCombatHit(CharacterObject attackerTroop, CharacterObject attackedTroop, PartyBase party, WeaponComponentData usedWeapon, bool isFatal, int xp)
		{
			CampaignEvents.Instance._onHeroCombatHitEvent.Invoke(attackerTroop, attackedTroop, party, usedWeapon, isFatal, xp);
		}

		// Token: 0x1700013E RID: 318
		// (get) Token: 0x06000652 RID: 1618 RVA: 0x0001DDBF File Offset: 0x0001BFBF
		public static IMbEvent<CharacterObject> CharacterPortraitPopUpOpenedEvent
		{
			get
			{
				return CampaignEvents.Instance._characterPortraitPopUpOpenedEvent;
			}
		}

		// Token: 0x06000653 RID: 1619 RVA: 0x0001DDCB File Offset: 0x0001BFCB
		public override void OnCharacterPortraitPopUpOpened(CharacterObject character)
		{
			this._timeControlModeBeforePopUpOpened = Campaign.Current.TimeControlMode;
			Campaign.Current.TimeControlMode = CampaignTimeControlMode.Stop;
			Campaign.Current.SetTimeControlModeLock(true);
			CampaignEvents.Instance._characterPortraitPopUpOpenedEvent.Invoke(character);
		}

		// Token: 0x1700013F RID: 319
		// (get) Token: 0x06000654 RID: 1620 RVA: 0x0001DE03 File Offset: 0x0001C003
		public static IMbEvent CharacterPortraitPopUpClosedEvent
		{
			get
			{
				return CampaignEvents.Instance._characterPortraitPopUpClosedEvent;
			}
		}

		// Token: 0x06000655 RID: 1621 RVA: 0x0001DE0F File Offset: 0x0001C00F
		public override void OnCharacterPortraitPopUpClosed()
		{
			Campaign.Current.SetTimeControlModeLock(false);
			Campaign.Current.TimeControlMode = this._timeControlModeBeforePopUpOpened;
			this._timeControlModeBeforePopUpOpened = CampaignTimeControlMode.Stop;
			CampaignEvents.Instance._characterPortraitPopUpClosedEvent.Invoke();
		}

		// Token: 0x17000140 RID: 320
		// (get) Token: 0x06000656 RID: 1622 RVA: 0x0001DE42 File Offset: 0x0001C042
		public static IMbEvent<Hero> PlayerStartTalkFromMenu
		{
			get
			{
				return CampaignEvents.Instance._playerStartTalkFromMenu;
			}
		}

		// Token: 0x06000657 RID: 1623 RVA: 0x0001DE4E File Offset: 0x0001C04E
		public override void OnPlayerStartTalkFromMenu(Hero hero)
		{
			CampaignEvents.Instance._playerStartTalkFromMenu.Invoke(hero);
		}

		// Token: 0x17000141 RID: 321
		// (get) Token: 0x06000658 RID: 1624 RVA: 0x0001DE60 File Offset: 0x0001C060
		public static IMbEvent<GameMenuOption> GameMenuOptionSelectedEvent
		{
			get
			{
				return CampaignEvents.Instance._gameMenuOptionSelectedEvent;
			}
		}

		// Token: 0x06000659 RID: 1625 RVA: 0x0001DE6C File Offset: 0x0001C06C
		public override void OnGameMenuOptionSelected(GameMenuOption gameMenuOption)
		{
			CampaignEvents.Instance._gameMenuOptionSelectedEvent.Invoke(gameMenuOption);
		}

		// Token: 0x17000142 RID: 322
		// (get) Token: 0x0600065A RID: 1626 RVA: 0x0001DE7E File Offset: 0x0001C07E
		public static IMbEvent<CharacterObject> PlayerStartRecruitmentEvent
		{
			get
			{
				return CampaignEvents.Instance._playerStartRecruitmentEvent;
			}
		}

		// Token: 0x0600065B RID: 1627 RVA: 0x0001DE8A File Offset: 0x0001C08A
		public override void OnPlayerStartRecruitment(CharacterObject recruitTroopCharacter)
		{
			CampaignEvents.Instance._playerStartRecruitmentEvent.Invoke(recruitTroopCharacter);
		}

		// Token: 0x17000143 RID: 323
		// (get) Token: 0x0600065C RID: 1628 RVA: 0x0001DE9C File Offset: 0x0001C09C
		public static IMbEvent<Hero, Hero> OnBeforePlayerCharacterChangedEvent
		{
			get
			{
				return CampaignEvents.Instance._onBeforePlayerCharacterChangedEvent;
			}
		}

		// Token: 0x0600065D RID: 1629 RVA: 0x0001DEA8 File Offset: 0x0001C0A8
		public override void OnBeforePlayerCharacterChanged(Hero oldPlayer, Hero newPlayer)
		{
			CampaignEvents.Instance._onBeforePlayerCharacterChangedEvent.Invoke(oldPlayer, newPlayer);
		}

		// Token: 0x17000144 RID: 324
		// (get) Token: 0x0600065E RID: 1630 RVA: 0x0001DEBB File Offset: 0x0001C0BB
		public static IMbEvent<Hero, Hero, MobileParty, bool> OnPlayerCharacterChangedEvent
		{
			get
			{
				return CampaignEvents.Instance._onPlayerCharacterChangedEvent;
			}
		}

		// Token: 0x0600065F RID: 1631 RVA: 0x0001DEC7 File Offset: 0x0001C0C7
		public override void OnPlayerCharacterChanged(Hero oldPlayer, Hero newPlayer, MobileParty newMainParty, bool isMainPartyChanged)
		{
			CampaignEvents.Instance._onPlayerCharacterChangedEvent.Invoke(oldPlayer, newPlayer, newMainParty, isMainPartyChanged);
		}

		// Token: 0x17000145 RID: 325
		// (get) Token: 0x06000660 RID: 1632 RVA: 0x0001DEDD File Offset: 0x0001C0DD
		public static IMbEvent<Hero, Hero> OnClanLeaderChangedEvent
		{
			get
			{
				return CampaignEvents.Instance._onClanLeaderChangedEvent;
			}
		}

		// Token: 0x06000661 RID: 1633 RVA: 0x0001DEE9 File Offset: 0x0001C0E9
		public override void OnClanLeaderChanged(Hero oldLeader, Hero newLeader)
		{
			CampaignEvents.Instance._onClanLeaderChangedEvent.Invoke(oldLeader, newLeader);
		}

		// Token: 0x17000146 RID: 326
		// (get) Token: 0x06000662 RID: 1634 RVA: 0x0001DEFC File Offset: 0x0001C0FC
		public static IMbEvent<SiegeEvent> OnSiegeEventStartedEvent
		{
			get
			{
				return CampaignEvents.Instance._onSiegeEventStartedEvent;
			}
		}

		// Token: 0x06000663 RID: 1635 RVA: 0x0001DF08 File Offset: 0x0001C108
		public override void OnSiegeEventStarted(SiegeEvent siegeEvent)
		{
			CampaignEvents.Instance._onSiegeEventStartedEvent.Invoke(siegeEvent);
		}

		// Token: 0x17000147 RID: 327
		// (get) Token: 0x06000664 RID: 1636 RVA: 0x0001DF1A File Offset: 0x0001C11A
		public static IMbEvent OnPlayerSiegeStartedEvent
		{
			get
			{
				return CampaignEvents.Instance._onPlayerSiegeStartedEvent;
			}
		}

		// Token: 0x06000665 RID: 1637 RVA: 0x0001DF26 File Offset: 0x0001C126
		public override void OnPlayerSiegeStarted()
		{
			CampaignEvents.Instance._onPlayerSiegeStartedEvent.Invoke();
		}

		// Token: 0x17000148 RID: 328
		// (get) Token: 0x06000666 RID: 1638 RVA: 0x0001DF37 File Offset: 0x0001C137
		public static IMbEvent<SiegeEvent> OnSiegeEventEndedEvent
		{
			get
			{
				return CampaignEvents.Instance._onSiegeEventEndedEvent;
			}
		}

		// Token: 0x06000667 RID: 1639 RVA: 0x0001DF43 File Offset: 0x0001C143
		public override void OnSiegeEventEnded(SiegeEvent siegeEvent)
		{
			CampaignEvents.Instance._onSiegeEventEndedEvent.Invoke(siegeEvent);
		}

		// Token: 0x17000149 RID: 329
		// (get) Token: 0x06000668 RID: 1640 RVA: 0x0001DF55 File Offset: 0x0001C155
		public static IMbEvent<MobileParty, Settlement, SiegeAftermathAction.SiegeAftermath, Clan, Dictionary<MobileParty, float>> OnSiegeAftermathAppliedEvent
		{
			get
			{
				return CampaignEvents.Instance._siegeAftermathAppliedEvent;
			}
		}

		// Token: 0x06000669 RID: 1641 RVA: 0x0001DF61 File Offset: 0x0001C161
		public override void OnSiegeAftermathApplied(MobileParty attackerParty, Settlement settlement, SiegeAftermathAction.SiegeAftermath aftermathType, Clan previousSettlementOwner, Dictionary<MobileParty, float> partyContributions)
		{
			CampaignEvents.Instance._siegeAftermathAppliedEvent.Invoke(attackerParty, settlement, aftermathType, previousSettlementOwner, partyContributions);
		}

		// Token: 0x1700014A RID: 330
		// (get) Token: 0x0600066A RID: 1642 RVA: 0x0001DF79 File Offset: 0x0001C179
		public static IMbEvent<MobileParty, Settlement, BattleSideEnum, SiegeEngineType, SiegeBombardTargets> OnSiegeBombardmentHitEvent
		{
			get
			{
				return CampaignEvents.Instance._onSiegeBombardmentHitEvent;
			}
		}

		// Token: 0x0600066B RID: 1643 RVA: 0x0001DF85 File Offset: 0x0001C185
		public override void OnSiegeBombardmentHit(MobileParty besiegerParty, Settlement besiegedSettlement, BattleSideEnum side, SiegeEngineType weapon, SiegeBombardTargets target)
		{
			CampaignEvents.Instance._onSiegeBombardmentHitEvent.Invoke(besiegerParty, besiegedSettlement, side, weapon, target);
		}

		// Token: 0x1700014B RID: 331
		// (get) Token: 0x0600066C RID: 1644 RVA: 0x0001DF9D File Offset: 0x0001C19D
		public static IMbEvent<MobileParty, Settlement, BattleSideEnum, SiegeEngineType, bool> OnSiegeBombardmentWallHitEvent
		{
			get
			{
				return CampaignEvents.Instance._onSiegeBombardmentWallHitEvent;
			}
		}

		// Token: 0x0600066D RID: 1645 RVA: 0x0001DFA9 File Offset: 0x0001C1A9
		public override void OnSiegeBombardmentWallHit(MobileParty besiegerParty, Settlement besiegedSettlement, BattleSideEnum side, SiegeEngineType weapon, bool isWallCracked)
		{
			CampaignEvents.Instance._onSiegeBombardmentWallHitEvent.Invoke(besiegerParty, besiegedSettlement, side, weapon, isWallCracked);
		}

		// Token: 0x1700014C RID: 332
		// (get) Token: 0x0600066E RID: 1646 RVA: 0x0001DFC1 File Offset: 0x0001C1C1
		public static IMbEvent<MobileParty, Settlement, BattleSideEnum, SiegeEngineType> OnSiegeEngineDestroyedEvent
		{
			get
			{
				return CampaignEvents.Instance._onSiegeEngineDestroyedEvent;
			}
		}

		// Token: 0x0600066F RID: 1647 RVA: 0x0001DFCD File Offset: 0x0001C1CD
		public override void OnSiegeEngineDestroyed(MobileParty besiegerParty, Settlement besiegedSettlement, BattleSideEnum side, SiegeEngineType destroyedEngine)
		{
			CampaignEvents.Instance._onSiegeEngineDestroyedEvent.Invoke(besiegerParty, besiegedSettlement, side, destroyedEngine);
		}

		// Token: 0x1700014D RID: 333
		// (get) Token: 0x06000670 RID: 1648 RVA: 0x0001DFE3 File Offset: 0x0001C1E3
		public static IMbEvent<List<TradeRumor>, Settlement> OnTradeRumorIsTakenEvent
		{
			get
			{
				return CampaignEvents.Instance._onTradeRumorIsTakenEvent;
			}
		}

		// Token: 0x06000671 RID: 1649 RVA: 0x0001DFEF File Offset: 0x0001C1EF
		public override void OnTradeRumorIsTaken(List<TradeRumor> newRumors, Settlement sourceSettlement = null)
		{
			CampaignEvents.Instance._onTradeRumorIsTakenEvent.Invoke(newRumors, sourceSettlement);
		}

		// Token: 0x1700014E RID: 334
		// (get) Token: 0x06000672 RID: 1650 RVA: 0x0001E002 File Offset: 0x0001C202
		public static IMbEvent<Hero> OnCheckForIssueEvent
		{
			get
			{
				return CampaignEvents.Instance._onCheckForIssueEvent;
			}
		}

		// Token: 0x06000673 RID: 1651 RVA: 0x0001E00E File Offset: 0x0001C20E
		public override void OnCheckForIssue(Hero hero)
		{
			CampaignEvents.Instance._onCheckForIssueEvent.Invoke(hero);
		}

		// Token: 0x1700014F RID: 335
		// (get) Token: 0x06000674 RID: 1652 RVA: 0x0001E020 File Offset: 0x0001C220
		public static IMbEvent<IssueBase, IssueBase.IssueUpdateDetails, Hero> OnIssueUpdatedEvent
		{
			get
			{
				return CampaignEvents.Instance._onIssueUpdatedEvent;
			}
		}

		// Token: 0x06000675 RID: 1653 RVA: 0x0001E02C File Offset: 0x0001C22C
		public override void OnIssueUpdated(IssueBase issue, IssueBase.IssueUpdateDetails details, Hero issueSolver = null)
		{
			CampaignEvents.Instance._onIssueUpdatedEvent.Invoke(issue, details, issueSolver);
		}

		// Token: 0x17000150 RID: 336
		// (get) Token: 0x06000676 RID: 1654 RVA: 0x0001E040 File Offset: 0x0001C240
		public static IMbEvent<MobileParty, TroopRoster> OnTroopsDesertedEvent
		{
			get
			{
				return CampaignEvents.Instance._onTroopsDesertedEvent;
			}
		}

		// Token: 0x06000677 RID: 1655 RVA: 0x0001E04C File Offset: 0x0001C24C
		public override void OnTroopsDeserted(MobileParty mobileParty, TroopRoster desertedTroops)
		{
			CampaignEvents.Instance._onTroopsDesertedEvent.Invoke(mobileParty, desertedTroops);
		}

		// Token: 0x17000151 RID: 337
		// (get) Token: 0x06000678 RID: 1656 RVA: 0x0001E05F File Offset: 0x0001C25F
		public static IMbEvent<Hero, Settlement, Hero, CharacterObject, int> OnTroopRecruitedEvent
		{
			get
			{
				return CampaignEvents.Instance._onTroopRecruitedEvent;
			}
		}

		// Token: 0x06000679 RID: 1657 RVA: 0x0001E06B File Offset: 0x0001C26B
		public override void OnTroopRecruited(Hero recruiterHero, Settlement recruitmentSettlement, Hero recruitmentSource, CharacterObject troop, int amount)
		{
			CampaignEvents.Instance._onTroopRecruitedEvent.Invoke(recruiterHero, recruitmentSettlement, recruitmentSource, troop, amount);
		}

		// Token: 0x17000152 RID: 338
		// (get) Token: 0x0600067A RID: 1658 RVA: 0x0001E083 File Offset: 0x0001C283
		public static IMbEvent<Hero, Settlement, TroopRoster> OnTroopGivenToSettlementEvent
		{
			get
			{
				return CampaignEvents.Instance._onTroopGivenToSettlementEvent;
			}
		}

		// Token: 0x0600067B RID: 1659 RVA: 0x0001E08F File Offset: 0x0001C28F
		public override void OnTroopGivenToSettlement(Hero giverHero, Settlement recipientSettlement, TroopRoster roster)
		{
			CampaignEvents.Instance._onTroopGivenToSettlementEvent.Invoke(giverHero, recipientSettlement, roster);
		}

		// Token: 0x17000153 RID: 339
		// (get) Token: 0x0600067C RID: 1660 RVA: 0x0001E0A3 File Offset: 0x0001C2A3
		public static IMbEvent<PartyBase, PartyBase, ItemRosterElement, int, Settlement> OnItemSoldEvent
		{
			get
			{
				return CampaignEvents.Instance._onItemSoldEvent;
			}
		}

		// Token: 0x0600067D RID: 1661 RVA: 0x0001E0AF File Offset: 0x0001C2AF
		public override void OnItemSold(PartyBase receiverParty, PartyBase payerParty, ItemRosterElement itemRosterElement, int number, Settlement currentSettlement)
		{
			CampaignEvents.Instance._onItemSoldEvent.Invoke(receiverParty, payerParty, itemRosterElement, number, currentSettlement);
		}

		// Token: 0x17000154 RID: 340
		// (get) Token: 0x0600067E RID: 1662 RVA: 0x0001E0C7 File Offset: 0x0001C2C7
		public static IMbEvent<MobileParty, Town, List<ValueTuple<EquipmentElement, int>>> OnCaravanTransactionCompletedEvent
		{
			get
			{
				return CampaignEvents.Instance._onCaravanTransactionCompletedEvent;
			}
		}

		// Token: 0x0600067F RID: 1663 RVA: 0x0001E0D3 File Offset: 0x0001C2D3
		public override void OnCaravanTransactionCompleted(MobileParty caravanParty, Town town, List<ValueTuple<EquipmentElement, int>> itemRosterElements)
		{
			CampaignEvents.Instance._onCaravanTransactionCompletedEvent.Invoke(caravanParty, town, itemRosterElements);
		}

		// Token: 0x17000155 RID: 341
		// (get) Token: 0x06000680 RID: 1664 RVA: 0x0001E0E7 File Offset: 0x0001C2E7
		public static IMbEvent<MobileParty, TroopRoster, Settlement> OnPrisonerSoldEvent
		{
			get
			{
				return CampaignEvents.Instance._onPrisonerSoldEvent;
			}
		}

		// Token: 0x06000681 RID: 1665 RVA: 0x0001E0F3 File Offset: 0x0001C2F3
		public override void OnPrisonerSold(MobileParty party, TroopRoster prisoners, Settlement currentSettlement)
		{
			CampaignEvents.Instance._onPrisonerSoldEvent.Invoke(party, prisoners, currentSettlement);
		}

		// Token: 0x17000156 RID: 342
		// (get) Token: 0x06000682 RID: 1666 RVA: 0x0001E107 File Offset: 0x0001C307
		public static IMbEvent<MobileParty> OnPartyDisbandStartedEvent
		{
			get
			{
				return CampaignEvents.Instance._onPartyDisbandStartedEvent;
			}
		}

		// Token: 0x06000683 RID: 1667 RVA: 0x0001E113 File Offset: 0x0001C313
		public override void OnPartyDisbandStarted(MobileParty disbandParty)
		{
			CampaignEvents.Instance._onPartyDisbandStartedEvent.Invoke(disbandParty);
		}

		// Token: 0x17000157 RID: 343
		// (get) Token: 0x06000684 RID: 1668 RVA: 0x0001E125 File Offset: 0x0001C325
		public static IMbEvent<MobileParty, Settlement> OnPartyDisbandedEvent
		{
			get
			{
				return CampaignEvents.Instance._onPartyDisbandedEvent;
			}
		}

		// Token: 0x06000685 RID: 1669 RVA: 0x0001E131 File Offset: 0x0001C331
		public override void OnPartyDisbanded(MobileParty disbandParty, Settlement relatedSettlement)
		{
			CampaignEvents.Instance._onPartyDisbandedEvent.Invoke(disbandParty, relatedSettlement);
		}

		// Token: 0x17000158 RID: 344
		// (get) Token: 0x06000686 RID: 1670 RVA: 0x0001E144 File Offset: 0x0001C344
		public static IMbEvent<MobileParty> OnPartyDisbandCanceledEvent
		{
			get
			{
				return CampaignEvents.Instance._onPartyDisbandCanceledEvent;
			}
		}

		// Token: 0x06000687 RID: 1671 RVA: 0x0001E150 File Offset: 0x0001C350
		public override void OnPartyDisbandCanceled(MobileParty disbandParty)
		{
			CampaignEvents.Instance._onPartyDisbandCanceledEvent.Invoke(disbandParty);
		}

		// Token: 0x17000159 RID: 345
		// (get) Token: 0x06000688 RID: 1672 RVA: 0x0001E162 File Offset: 0x0001C362
		public static IMbEvent<PartyBase, PartyBase> OnHideoutSpottedEvent
		{
			get
			{
				return CampaignEvents.Instance._hideoutSpottedEvent;
			}
		}

		// Token: 0x06000689 RID: 1673 RVA: 0x0001E16E File Offset: 0x0001C36E
		public override void OnHideoutSpotted(PartyBase party, PartyBase hideoutParty)
		{
			CampaignEvents.Instance._hideoutSpottedEvent.Invoke(party, hideoutParty);
		}

		// Token: 0x1700015A RID: 346
		// (get) Token: 0x0600068A RID: 1674 RVA: 0x0001E181 File Offset: 0x0001C381
		public static IMbEvent<Settlement> OnHideoutDeactivatedEvent
		{
			get
			{
				return CampaignEvents.Instance._hideoutDeactivatedEvent;
			}
		}

		// Token: 0x0600068B RID: 1675 RVA: 0x0001E18D File Offset: 0x0001C38D
		public override void OnHideoutDeactivated(Settlement hideout)
		{
			CampaignEvents.Instance._hideoutDeactivatedEvent.Invoke(hideout);
		}

		// Token: 0x1700015B RID: 347
		// (get) Token: 0x0600068C RID: 1676 RVA: 0x0001E19F File Offset: 0x0001C39F
		public static IMbEvent<Hero, Hero, float> OnHeroSharedFoodWithAnotherHeroEvent
		{
			get
			{
				return CampaignEvents.Instance._heroSharedFoodWithAnotherHeroEvent;
			}
		}

		// Token: 0x0600068D RID: 1677 RVA: 0x0001E1AB File Offset: 0x0001C3AB
		public override void OnHeroSharedFoodWithAnother(Hero supporterHero, Hero supportedHero, float influence)
		{
			CampaignEvents.Instance._heroSharedFoodWithAnotherHeroEvent.Invoke(supporterHero, supportedHero, influence);
		}

		// Token: 0x1700015C RID: 348
		// (get) Token: 0x0600068E RID: 1678 RVA: 0x0001E1BF File Offset: 0x0001C3BF
		public static IMbEvent<List<ValueTuple<ItemRosterElement, int>>, List<ValueTuple<ItemRosterElement, int>>, bool> PlayerInventoryExchangeEvent
		{
			get
			{
				return CampaignEvents.Instance._playerInventoryExchangeEvent;
			}
		}

		// Token: 0x0600068F RID: 1679 RVA: 0x0001E1CB File Offset: 0x0001C3CB
		public override void OnPlayerInventoryExchange(List<ValueTuple<ItemRosterElement, int>> purchasedItems, List<ValueTuple<ItemRosterElement, int>> soldItems, bool isTrading)
		{
			CampaignEvents.Instance._playerInventoryExchangeEvent.Invoke(purchasedItems, soldItems, isTrading);
		}

		// Token: 0x1700015D RID: 349
		// (get) Token: 0x06000690 RID: 1680 RVA: 0x0001E1DF File Offset: 0x0001C3DF
		public static IMbEvent<ItemRoster> OnItemsDiscardedByPlayerEvent
		{
			get
			{
				return CampaignEvents.Instance._onItemsDiscardedByPlayerEvent;
			}
		}

		// Token: 0x06000691 RID: 1681 RVA: 0x0001E1EB File Offset: 0x0001C3EB
		public override void OnItemsDiscardedByPlayer(ItemRoster discardedItems)
		{
			CampaignEvents.Instance._onItemsDiscardedByPlayerEvent.Invoke(discardedItems);
		}

		// Token: 0x1700015E RID: 350
		// (get) Token: 0x06000692 RID: 1682 RVA: 0x0001E1FD File Offset: 0x0001C3FD
		public static IMbEvent<Tuple<PersuasionOptionArgs, PersuasionOptionResult>> PersuasionProgressCommittedEvent
		{
			get
			{
				return CampaignEvents.Instance._persuasionProgressCommittedEvent;
			}
		}

		// Token: 0x06000693 RID: 1683 RVA: 0x0001E209 File Offset: 0x0001C409
		public override void OnPersuasionProgressCommitted(Tuple<PersuasionOptionArgs, PersuasionOptionResult> progress)
		{
			CampaignEvents.Instance._persuasionProgressCommittedEvent.Invoke(progress);
		}

		// Token: 0x1700015F RID: 351
		// (get) Token: 0x06000694 RID: 1684 RVA: 0x0001E21B File Offset: 0x0001C41B
		public static IMbEvent<QuestBase, QuestBase.QuestCompleteDetails> OnQuestCompletedEvent
		{
			get
			{
				return CampaignEvents.Instance._onQuestCompletedEvent;
			}
		}

		// Token: 0x06000695 RID: 1685 RVA: 0x0001E227 File Offset: 0x0001C427
		public override void OnQuestCompleted(QuestBase quest, QuestBase.QuestCompleteDetails detail)
		{
			CampaignEvents.Instance._onQuestCompletedEvent.Invoke(quest, detail);
		}

		// Token: 0x17000160 RID: 352
		// (get) Token: 0x06000696 RID: 1686 RVA: 0x0001E23A File Offset: 0x0001C43A
		public static IMbEvent<QuestBase> OnQuestStartedEvent
		{
			get
			{
				return CampaignEvents.Instance._onQuestStartedEvent;
			}
		}

		// Token: 0x06000697 RID: 1687 RVA: 0x0001E246 File Offset: 0x0001C446
		public override void OnQuestStarted(QuestBase quest)
		{
			CampaignEvents.Instance._onQuestStartedEvent.Invoke(quest);
		}

		// Token: 0x17000161 RID: 353
		// (get) Token: 0x06000698 RID: 1688 RVA: 0x0001E258 File Offset: 0x0001C458
		public static IMbEvent<ItemObject, Settlement, int> OnItemProducedEvent
		{
			get
			{
				return CampaignEvents.Instance._itemProducedEvent;
			}
		}

		// Token: 0x06000699 RID: 1689 RVA: 0x0001E264 File Offset: 0x0001C464
		public override void OnItemProduced(ItemObject itemObject, Settlement settlement, int count)
		{
			CampaignEvents.Instance._itemProducedEvent.Invoke(itemObject, settlement, count);
		}

		// Token: 0x17000162 RID: 354
		// (get) Token: 0x0600069A RID: 1690 RVA: 0x0001E278 File Offset: 0x0001C478
		public static IMbEvent<ItemObject, Settlement, int> OnItemConsumedEvent
		{
			get
			{
				return CampaignEvents.Instance._itemConsumedEvent;
			}
		}

		// Token: 0x0600069B RID: 1691 RVA: 0x0001E284 File Offset: 0x0001C484
		public override void OnItemConsumed(ItemObject itemObject, Settlement settlement, int count)
		{
			CampaignEvents.Instance._itemConsumedEvent.Invoke(itemObject, settlement, count);
		}

		// Token: 0x17000163 RID: 355
		// (get) Token: 0x0600069C RID: 1692 RVA: 0x0001E298 File Offset: 0x0001C498
		public static IMbEvent<MobileParty> OnPartyConsumedFoodEvent
		{
			get
			{
				return CampaignEvents.Instance._onPartyConsumedFoodEvent;
			}
		}

		// Token: 0x0600069D RID: 1693 RVA: 0x0001E2A4 File Offset: 0x0001C4A4
		public override void OnPartyConsumedFood(MobileParty party)
		{
			CampaignEvents.Instance._onPartyConsumedFoodEvent.Invoke(party);
		}

		// Token: 0x17000164 RID: 356
		// (get) Token: 0x0600069E RID: 1694 RVA: 0x0001E2B6 File Offset: 0x0001C4B6
		public static IMbEvent<Hero, Hero, KillCharacterAction.KillCharacterActionDetail, bool> OnBeforeMainCharacterDiedEvent
		{
			get
			{
				return CampaignEvents.Instance._onBeforeMainCharacterDiedEvent;
			}
		}

		// Token: 0x0600069F RID: 1695 RVA: 0x0001E2C2 File Offset: 0x0001C4C2
		public override void OnBeforeMainCharacterDied(Hero victim, Hero killer, KillCharacterAction.KillCharacterActionDetail detail, bool showNotification = true)
		{
			CampaignEvents.Instance._onBeforeMainCharacterDiedEvent.Invoke(victim, killer, detail, showNotification);
		}

		// Token: 0x17000165 RID: 357
		// (get) Token: 0x060006A0 RID: 1696 RVA: 0x0001E2D8 File Offset: 0x0001C4D8
		public static IMbEvent<IssueBase> OnNewIssueCreatedEvent
		{
			get
			{
				return CampaignEvents.Instance._onNewIssueCreatedEvent;
			}
		}

		// Token: 0x060006A1 RID: 1697 RVA: 0x0001E2E4 File Offset: 0x0001C4E4
		public override void OnNewIssueCreated(IssueBase issue)
		{
			CampaignEvents.Instance._onNewIssueCreatedEvent.Invoke(issue);
		}

		// Token: 0x17000166 RID: 358
		// (get) Token: 0x060006A2 RID: 1698 RVA: 0x0001E2F6 File Offset: 0x0001C4F6
		public static IMbEvent<IssueBase, Hero> OnIssueOwnerChangedEvent
		{
			get
			{
				return CampaignEvents.Instance._onIssueOwnerChangedEvent;
			}
		}

		// Token: 0x060006A3 RID: 1699 RVA: 0x0001E302 File Offset: 0x0001C502
		public override void OnIssueOwnerChanged(IssueBase issue, Hero oldOwner)
		{
			CampaignEvents.Instance._onIssueOwnerChangedEvent.Invoke(issue, oldOwner);
		}

		// Token: 0x060006A4 RID: 1700 RVA: 0x0001E315 File Offset: 0x0001C515
		public override void OnGameOver()
		{
			CampaignEvents.Instance._onGameOverEvent.Invoke();
		}

		// Token: 0x17000167 RID: 359
		// (get) Token: 0x060006A5 RID: 1701 RVA: 0x0001E326 File Offset: 0x0001C526
		public static IMbEvent OnGameOverEvent
		{
			get
			{
				return CampaignEvents.Instance._onGameOverEvent;
			}
		}

		// Token: 0x17000168 RID: 360
		// (get) Token: 0x060006A6 RID: 1702 RVA: 0x0001E332 File Offset: 0x0001C532
		public static IMbEvent<Settlement, MobileParty, bool, MapEvent.BattleTypes> SiegeCompletedEvent
		{
			get
			{
				return CampaignEvents.Instance._siegeCompletedEvent;
			}
		}

		// Token: 0x060006A7 RID: 1703 RVA: 0x0001E33E File Offset: 0x0001C53E
		public override void SiegeCompleted(Settlement siegeSettlement, MobileParty attackerParty, bool isWin, MapEvent.BattleTypes battleType)
		{
			CampaignEvents.Instance._siegeCompletedEvent.Invoke(siegeSettlement, attackerParty, isWin, battleType);
		}

		// Token: 0x17000169 RID: 361
		// (get) Token: 0x060006A8 RID: 1704 RVA: 0x0001E354 File Offset: 0x0001C554
		public static IMbEvent<SiegeEvent, BattleSideEnum, SiegeEngineType> SiegeEngineBuiltEvent
		{
			get
			{
				return CampaignEvents.Instance._siegeEngineBuiltEvent;
			}
		}

		// Token: 0x060006A9 RID: 1705 RVA: 0x0001E360 File Offset: 0x0001C560
		public override void SiegeEngineBuilt(SiegeEvent siegeEvent, BattleSideEnum side, SiegeEngineType siegeEngineType)
		{
			CampaignEvents.Instance._siegeEngineBuiltEvent.Invoke(siegeEvent, side, siegeEngineType);
		}

		// Token: 0x1700016A RID: 362
		// (get) Token: 0x060006AA RID: 1706 RVA: 0x0001E374 File Offset: 0x0001C574
		public static IMbEvent<BattleSideEnum, RaidEventComponent> RaidCompletedEvent
		{
			get
			{
				return CampaignEvents.Instance._raidCompletedEvent;
			}
		}

		// Token: 0x060006AB RID: 1707 RVA: 0x0001E380 File Offset: 0x0001C580
		public override void RaidCompleted(BattleSideEnum winnerSide, RaidEventComponent raidEvent)
		{
			CampaignEvents.Instance._raidCompletedEvent.Invoke(winnerSide, raidEvent);
		}

		// Token: 0x1700016B RID: 363
		// (get) Token: 0x060006AC RID: 1708 RVA: 0x0001E393 File Offset: 0x0001C593
		public static IMbEvent<BattleSideEnum, ForceVolunteersEventComponent> ForceVolunteersCompletedEvent
		{
			get
			{
				return CampaignEvents.Instance._forceVolunteersCompletedEvent;
			}
		}

		// Token: 0x060006AD RID: 1709 RVA: 0x0001E39F File Offset: 0x0001C59F
		public override void ForceVolunteersCompleted(BattleSideEnum winnerSide, ForceVolunteersEventComponent forceVolunteersEvent)
		{
			CampaignEvents.Instance._forceVolunteersCompletedEvent.Invoke(winnerSide, forceVolunteersEvent);
		}

		// Token: 0x1700016C RID: 364
		// (get) Token: 0x060006AE RID: 1710 RVA: 0x0001E3B2 File Offset: 0x0001C5B2
		public static IMbEvent<BattleSideEnum, ForceSuppliesEventComponent> ForceSuppliesCompletedEvent
		{
			get
			{
				return CampaignEvents.Instance._forceSuppliesCompletedEvent;
			}
		}

		// Token: 0x060006AF RID: 1711 RVA: 0x0001E3BE File Offset: 0x0001C5BE
		public override void ForceSuppliesCompleted(BattleSideEnum winnerSide, ForceSuppliesEventComponent forceSuppliesEvent)
		{
			CampaignEvents.Instance._forceSuppliesCompletedEvent.Invoke(winnerSide, forceSuppliesEvent);
		}

		// Token: 0x1700016D RID: 365
		// (get) Token: 0x060006B0 RID: 1712 RVA: 0x0001E3D1 File Offset: 0x0001C5D1
		public static MbEvent<BattleSideEnum, MapEvent> OnHideoutBattleCompletedEvent
		{
			get
			{
				return CampaignEvents.Instance._hideoutBattleCompletedEvent;
			}
		}

		// Token: 0x060006B1 RID: 1713 RVA: 0x0001E3DD File Offset: 0x0001C5DD
		public override void OnHideoutBattleCompleted(BattleSideEnum winnerSide, MapEvent mapEvent)
		{
			CampaignEvents.Instance._hideoutBattleCompletedEvent.Invoke(winnerSide, mapEvent);
		}

		// Token: 0x1700016E RID: 366
		// (get) Token: 0x060006B2 RID: 1714 RVA: 0x0001E3F0 File Offset: 0x0001C5F0
		public static IMbEvent<Clan> OnClanDestroyedEvent
		{
			get
			{
				return CampaignEvents.Instance._onClanDestroyedEvent;
			}
		}

		// Token: 0x060006B3 RID: 1715 RVA: 0x0001E3FC File Offset: 0x0001C5FC
		public override void OnClanDestroyed(Clan destroyedClan)
		{
			CampaignEvents.Instance._onClanDestroyedEvent.Invoke(destroyedClan);
		}

		// Token: 0x1700016F RID: 367
		// (get) Token: 0x060006B4 RID: 1716 RVA: 0x0001E40E File Offset: 0x0001C60E
		public static IMbEvent<ItemObject, Crafting.OverrideData, bool> OnNewItemCraftedEvent
		{
			get
			{
				return CampaignEvents.Instance._onNewItemCraftedEvent;
			}
		}

		// Token: 0x060006B5 RID: 1717 RVA: 0x0001E41A File Offset: 0x0001C61A
		public override void OnNewItemCrafted(ItemObject itemObject, Crafting.OverrideData overrideData, bool isCraftingOrderItem)
		{
			CampaignEvents.Instance._onNewItemCraftedEvent.Invoke(itemObject, overrideData, isCraftingOrderItem);
		}

		// Token: 0x17000170 RID: 368
		// (get) Token: 0x060006B6 RID: 1718 RVA: 0x0001E42E File Offset: 0x0001C62E
		public static IMbEvent<CraftingPiece> CraftingPartUnlockedEvent
		{
			get
			{
				return CampaignEvents.Instance._craftingPartUnlockedEvent;
			}
		}

		// Token: 0x060006B7 RID: 1719 RVA: 0x0001E43A File Offset: 0x0001C63A
		public override void CraftingPartUnlocked(CraftingPiece craftingPiece)
		{
			CampaignEvents.Instance._craftingPartUnlockedEvent.Invoke(craftingPiece);
		}

		// Token: 0x17000171 RID: 369
		// (get) Token: 0x060006B8 RID: 1720 RVA: 0x0001E44C File Offset: 0x0001C64C
		public static IMbEvent<Workshop, Hero, WorkshopType> OnWorkshopChangedEvent
		{
			get
			{
				return CampaignEvents.Instance._onWorkshopChangedEvent;
			}
		}

		// Token: 0x060006B9 RID: 1721 RVA: 0x0001E458 File Offset: 0x0001C658
		public override void OnWorkshopChanged(Workshop workshop, Hero oldOwner, WorkshopType oldType)
		{
			CampaignEvents.Instance._onWorkshopChangedEvent.Invoke(workshop, oldOwner, oldType);
		}

		// Token: 0x17000172 RID: 370
		// (get) Token: 0x060006BA RID: 1722 RVA: 0x0001E46C File Offset: 0x0001C66C
		public static IMbEvent OnBeforeSaveEvent
		{
			get
			{
				return CampaignEvents.Instance._onBeforeSaveEvent;
			}
		}

		// Token: 0x060006BB RID: 1723 RVA: 0x0001E478 File Offset: 0x0001C678
		public override void OnBeforeSave()
		{
			CampaignEvents.Instance._onBeforeSaveEvent.Invoke();
		}

		// Token: 0x17000173 RID: 371
		// (get) Token: 0x060006BC RID: 1724 RVA: 0x0001E489 File Offset: 0x0001C689
		public static IMbEvent OnSaveStartedEvent
		{
			get
			{
				return CampaignEvents.Instance._onSaveStartedEvent;
			}
		}

		// Token: 0x060006BD RID: 1725 RVA: 0x0001E495 File Offset: 0x0001C695
		public override void OnSaveStarted()
		{
			CampaignEvents.Instance._onSaveStartedEvent.Invoke();
		}

		// Token: 0x17000174 RID: 372
		// (get) Token: 0x060006BE RID: 1726 RVA: 0x0001E4A6 File Offset: 0x0001C6A6
		public static IMbEvent<bool, string> OnSaveOverEvent
		{
			get
			{
				return CampaignEvents.Instance._onSaveOverEvent;
			}
		}

		// Token: 0x060006BF RID: 1727 RVA: 0x0001E4B2 File Offset: 0x0001C6B2
		public override void OnSaveOver(bool isSuccessful, string saveName)
		{
			CampaignEvents.Instance._onSaveOverEvent.Invoke(isSuccessful, saveName);
		}

		// Token: 0x17000175 RID: 373
		// (get) Token: 0x060006C0 RID: 1728 RVA: 0x0001E4C5 File Offset: 0x0001C6C5
		public static IMbEvent<FlattenedTroopRoster> OnPrisonerTakenEvent
		{
			get
			{
				return CampaignEvents.Instance._onPrisonerTakenEvent;
			}
		}

		// Token: 0x060006C1 RID: 1729 RVA: 0x0001E4D1 File Offset: 0x0001C6D1
		public override void OnPrisonerTaken(FlattenedTroopRoster roster)
		{
			CampaignEvents.Instance._onPrisonerTakenEvent.Invoke(roster);
		}

		// Token: 0x17000176 RID: 374
		// (get) Token: 0x060006C2 RID: 1730 RVA: 0x0001E4E3 File Offset: 0x0001C6E3
		public static IMbEvent<FlattenedTroopRoster> OnPrisonerReleasedEvent
		{
			get
			{
				return CampaignEvents.Instance._onPrisonerReleasedEvent;
			}
		}

		// Token: 0x060006C3 RID: 1731 RVA: 0x0001E4EF File Offset: 0x0001C6EF
		public override void OnPrisonerReleased(FlattenedTroopRoster roster)
		{
			CampaignEvents.Instance._onPrisonerReleasedEvent.Invoke(roster);
		}

		// Token: 0x17000177 RID: 375
		// (get) Token: 0x060006C4 RID: 1732 RVA: 0x0001E501 File Offset: 0x0001C701
		public static IMbEvent<FlattenedTroopRoster> OnMainPartyPrisonerRecruitedEvent
		{
			get
			{
				return CampaignEvents.Instance._onMainPartyPrisonerRecruitedEvent;
			}
		}

		// Token: 0x060006C5 RID: 1733 RVA: 0x0001E50D File Offset: 0x0001C70D
		public override void OnMainPartyPrisonerRecruited(FlattenedTroopRoster roster)
		{
			CampaignEvents.Instance._onMainPartyPrisonerRecruitedEvent.Invoke(roster);
		}

		// Token: 0x17000178 RID: 376
		// (get) Token: 0x060006C6 RID: 1734 RVA: 0x0001E51F File Offset: 0x0001C71F
		public static IMbEvent<MobileParty, FlattenedTroopRoster, Settlement> OnPrisonerDonatedToSettlementEvent
		{
			get
			{
				return CampaignEvents.Instance._onPrisonerDonatedToSettlementEvent;
			}
		}

		// Token: 0x060006C7 RID: 1735 RVA: 0x0001E52B File Offset: 0x0001C72B
		public override void OnPrisonerDonatedToSettlement(MobileParty donatingParty, FlattenedTroopRoster donatedPrisoners, Settlement donatedSettlement)
		{
			CampaignEvents.Instance._onPrisonerDonatedToSettlementEvent.Invoke(donatingParty, donatedPrisoners, donatedSettlement);
		}

		// Token: 0x17000179 RID: 377
		// (get) Token: 0x060006C8 RID: 1736 RVA: 0x0001E53F File Offset: 0x0001C73F
		public static IMbEvent<Hero, EquipmentElement> OnEquipmentSmeltedByHeroEvent
		{
			get
			{
				return CampaignEvents.Instance._onEquipmentSmeltedByHero;
			}
		}

		// Token: 0x060006C9 RID: 1737 RVA: 0x0001E54B File Offset: 0x0001C74B
		public override void OnEquipmentSmeltedByHero(Hero hero, EquipmentElement smeltedEquipmentElement)
		{
			CampaignEvents.Instance._onEquipmentSmeltedByHero.Invoke(hero, smeltedEquipmentElement);
		}

		// Token: 0x1700017A RID: 378
		// (get) Token: 0x060006CA RID: 1738 RVA: 0x0001E55E File Offset: 0x0001C75E
		public static IMbEvent<int> OnPlayerTradeProfitEvent
		{
			get
			{
				return CampaignEvents.Instance._onPlayerTradeProfit;
			}
		}

		// Token: 0x060006CB RID: 1739 RVA: 0x0001E56A File Offset: 0x0001C76A
		public override void OnPlayerTradeProfit(int profit)
		{
			CampaignEvents.Instance._onPlayerTradeProfit.Invoke(profit);
		}

		// Token: 0x1700017B RID: 379
		// (get) Token: 0x060006CC RID: 1740 RVA: 0x0001E57C File Offset: 0x0001C77C
		public static IMbEvent<Hero, Clan> OnHeroChangedClanEvent
		{
			get
			{
				return CampaignEvents.Instance._onHeroChangedClan;
			}
		}

		// Token: 0x060006CD RID: 1741 RVA: 0x0001E588 File Offset: 0x0001C788
		public override void OnHeroChangedClan(Hero hero, Clan oldClan)
		{
			CampaignEvents.Instance._onHeroChangedClan.Invoke(hero, oldClan);
		}

		// Token: 0x1700017C RID: 380
		// (get) Token: 0x060006CE RID: 1742 RVA: 0x0001E59B File Offset: 0x0001C79B
		public static IMbEvent<Hero, HeroGetsBusyReasons> OnHeroGetsBusyEvent
		{
			get
			{
				return CampaignEvents.Instance._onHeroGetsBusy;
			}
		}

		// Token: 0x060006CF RID: 1743 RVA: 0x0001E5A7 File Offset: 0x0001C7A7
		public override void OnHeroGetsBusy(Hero hero, HeroGetsBusyReasons heroGetsBusyReason)
		{
			CampaignEvents.Instance._onHeroGetsBusy.Invoke(hero, heroGetsBusyReason);
		}

		// Token: 0x1700017D RID: 381
		// (get) Token: 0x060006D0 RID: 1744 RVA: 0x0001E5BA File Offset: 0x0001C7BA
		public static IMbEvent<MapEvent, PartyBase, Dictionary<PartyBase, ItemRoster>, ItemRoster, MBList<TroopRosterElement>, float> CollectLootsEvent
		{
			get
			{
				return CampaignEvents.Instance._collectLootsEvent;
			}
		}

		// Token: 0x060006D1 RID: 1745 RVA: 0x0001E5C6 File Offset: 0x0001C7C6
		public override void CollectLoots(MapEvent mapEvent, PartyBase party, Dictionary<PartyBase, ItemRoster> loot, ItemRoster rosterToReceiveLoot, MBList<TroopRosterElement> lootedCasualties, float lootAmount)
		{
			CampaignEvents.Instance._collectLootsEvent.Invoke(mapEvent, party, loot, rosterToReceiveLoot, lootedCasualties, lootAmount);
		}

		// Token: 0x1700017E RID: 382
		// (get) Token: 0x060006D2 RID: 1746 RVA: 0x0001E5E0 File Offset: 0x0001C7E0
		public static IMbEvent<MapEvent, PartyBase, Dictionary<PartyBase, ItemRoster>> DistributeLootToPartyEvent
		{
			get
			{
				return CampaignEvents.Instance._distributeLootToPartyEvent;
			}
		}

		// Token: 0x060006D3 RID: 1747 RVA: 0x0001E5EC File Offset: 0x0001C7EC
		public override void OnLootDistributedToParty(MapEvent mapEvent, PartyBase party, Dictionary<PartyBase, ItemRoster> loot)
		{
			CampaignEvents.Instance._distributeLootToPartyEvent.Invoke(mapEvent, party, loot);
		}

		// Token: 0x1700017F RID: 383
		// (get) Token: 0x060006D4 RID: 1748 RVA: 0x0001E600 File Offset: 0x0001C800
		public static IMbEvent<Hero, Settlement, MobileParty, TeleportHeroAction.TeleportationDetail> OnHeroTeleportationRequestedEvent
		{
			get
			{
				return CampaignEvents.Instance._onHeroTeleportationRequestedEvent;
			}
		}

		// Token: 0x060006D5 RID: 1749 RVA: 0x0001E60C File Offset: 0x0001C80C
		public override void OnHeroTeleportationRequested(Hero hero, Settlement targetSettlement, MobileParty targetParty, TeleportHeroAction.TeleportationDetail detail)
		{
			CampaignEvents.Instance._onHeroTeleportationRequestedEvent.Invoke(hero, targetSettlement, targetParty, detail);
		}

		// Token: 0x17000180 RID: 384
		// (get) Token: 0x060006D6 RID: 1750 RVA: 0x0001E622 File Offset: 0x0001C822
		public static IMbEvent<MobileParty> OnPartyLeaderChangeOfferCanceledEvent
		{
			get
			{
				return CampaignEvents.Instance._onPartyLeaderChangeOfferCanceledEvent;
			}
		}

		// Token: 0x060006D7 RID: 1751 RVA: 0x0001E62E File Offset: 0x0001C82E
		public override void OnPartyLeaderChangeOfferCanceled(MobileParty party)
		{
			CampaignEvents.Instance._onPartyLeaderChangeOfferCanceledEvent.Invoke(party);
		}

		// Token: 0x17000181 RID: 385
		// (get) Token: 0x060006D8 RID: 1752 RVA: 0x0001E640 File Offset: 0x0001C840
		public static IMbEvent<Clan, float> OnClanInfluenceChangedEvent
		{
			get
			{
				return CampaignEvents.Instance._onClanInfluenceChangedEvent;
			}
		}

		// Token: 0x060006D9 RID: 1753 RVA: 0x0001E64C File Offset: 0x0001C84C
		public override void OnClanInfluenceChanged(Clan clan, float change)
		{
			CampaignEvents.Instance._onClanInfluenceChangedEvent.Invoke(clan, change);
		}

		// Token: 0x17000182 RID: 386
		// (get) Token: 0x060006DA RID: 1754 RVA: 0x0001E65F File Offset: 0x0001C85F
		public static IMbEvent<CharacterObject> OnPlayerPartyKnockedOrKilledTroopEvent
		{
			get
			{
				return CampaignEvents.Instance._onPlayerPartyKnockedOrKilledTroopEvent;
			}
		}

		// Token: 0x060006DB RID: 1755 RVA: 0x0001E66B File Offset: 0x0001C86B
		public override void OnPlayerPartyKnockedOrKilledTroop(CharacterObject strikedTroop)
		{
			CampaignEvents.Instance._onPlayerPartyKnockedOrKilledTroopEvent.Invoke(strikedTroop);
		}

		// Token: 0x17000183 RID: 387
		// (get) Token: 0x060006DC RID: 1756 RVA: 0x0001E67D File Offset: 0x0001C87D
		public static IMbEvent<DefaultClanFinanceModel.AssetIncomeType, int> OnPlayerEarnedGoldFromAssetEvent
		{
			get
			{
				return CampaignEvents.Instance._onPlayerEarnedGoldFromAssetEvent;
			}
		}

		// Token: 0x060006DD RID: 1757 RVA: 0x0001E689 File Offset: 0x0001C889
		public override void OnPlayerEarnedGoldFromAsset(DefaultClanFinanceModel.AssetIncomeType incomeType, int incomeAmount)
		{
			CampaignEvents.Instance._onPlayerEarnedGoldFromAssetEvent.Invoke(incomeType, incomeAmount);
		}

		// Token: 0x17000184 RID: 388
		// (get) Token: 0x060006DE RID: 1758 RVA: 0x0001E69C File Offset: 0x0001C89C
		public static IMbEvent OnMainPartyStarvingEvent
		{
			get
			{
				return CampaignEvents.Instance._onMainPartyStarving;
			}
		}

		// Token: 0x060006DF RID: 1759 RVA: 0x0001E6A8 File Offset: 0x0001C8A8
		public override void OnMainPartyStarving()
		{
			CampaignEvents.Instance._onMainPartyStarving.Invoke();
		}

		// Token: 0x17000185 RID: 389
		// (get) Token: 0x060006E0 RID: 1760 RVA: 0x0001E6B9 File Offset: 0x0001C8B9
		public static IMbEvent<Town, bool> OnPlayerJoinedTournamentEvent
		{
			get
			{
				return CampaignEvents.Instance._onPlayerJoinedTournamentEvent;
			}
		}

		// Token: 0x060006E1 RID: 1761 RVA: 0x0001E6C5 File Offset: 0x0001C8C5
		public override void OnPlayerJoinedTournament(Town town, bool isParticipant)
		{
			CampaignEvents.Instance._onPlayerJoinedTournamentEvent.Invoke(town, isParticipant);
		}

		// Token: 0x17000186 RID: 390
		// (get) Token: 0x060006E2 RID: 1762 RVA: 0x0001E6D8 File Offset: 0x0001C8D8
		public static ReferenceIMBEvent<Hero, bool> CanHeroLeadPartyEvent
		{
			get
			{
				return CampaignEvents.Instance._canHeroLeadPartyEvent;
			}
		}

		// Token: 0x060006E3 RID: 1763 RVA: 0x0001E6E4 File Offset: 0x0001C8E4
		public override void CanHeroLeadParty(Hero hero, ref bool result)
		{
			CampaignEvents.Instance._canHeroLeadPartyEvent.Invoke(hero, ref result);
		}

		// Token: 0x17000187 RID: 391
		// (get) Token: 0x060006E4 RID: 1764 RVA: 0x0001E6F7 File Offset: 0x0001C8F7
		public static ReferenceIMBEvent<Hero, bool> CanHeroMarryEvent
		{
			get
			{
				return CampaignEvents.Instance._canMarryEvent;
			}
		}

		// Token: 0x060006E5 RID: 1765 RVA: 0x0001E703 File Offset: 0x0001C903
		public override void CanHeroMarry(Hero hero, ref bool result)
		{
			CampaignEvents.Instance._canMarryEvent.Invoke(hero, ref result);
		}

		// Token: 0x17000188 RID: 392
		// (get) Token: 0x060006E6 RID: 1766 RVA: 0x0001E716 File Offset: 0x0001C916
		public static ReferenceIMBEvent<Hero, bool> CanHeroEquipmentBeChangedEvent
		{
			get
			{
				return CampaignEvents.Instance._canHeroEquipmentBeChangedEvent;
			}
		}

		// Token: 0x060006E7 RID: 1767 RVA: 0x0001E722 File Offset: 0x0001C922
		public override void CanHeroEquipmentBeChanged(Hero hero, ref bool result)
		{
			CampaignEvents.Instance._canHeroEquipmentBeChangedEvent.Invoke(hero, ref result);
		}

		// Token: 0x17000189 RID: 393
		// (get) Token: 0x060006E8 RID: 1768 RVA: 0x0001E735 File Offset: 0x0001C935
		public static ReferenceIMBEvent<Hero, bool> CanBeGovernorOrHavePartyRoleEvent
		{
			get
			{
				return CampaignEvents.Instance._canBeGovernorOrHavePartyRoleEvent;
			}
		}

		// Token: 0x060006E9 RID: 1769 RVA: 0x0001E741 File Offset: 0x0001C941
		public override void CanBeGovernorOrHavePartyRole(Hero hero, ref bool result)
		{
			CampaignEvents.Instance._canBeGovernorOrHavePartyRoleEvent.Invoke(hero, ref result);
		}

		// Token: 0x1700018A RID: 394
		// (get) Token: 0x060006EA RID: 1770 RVA: 0x0001E754 File Offset: 0x0001C954
		public static ReferenceIMBEvent<Hero, KillCharacterAction.KillCharacterActionDetail, bool> CanHeroDieEvent
		{
			get
			{
				return CampaignEvents.Instance._canHeroDieEvent;
			}
		}

		// Token: 0x060006EB RID: 1771 RVA: 0x0001E760 File Offset: 0x0001C960
		public override void CanHeroDie(Hero hero, KillCharacterAction.KillCharacterActionDetail causeOfDeath, ref bool result)
		{
			CampaignEvents.Instance._canHeroDieEvent.Invoke(hero, causeOfDeath, ref result);
		}

		// Token: 0x1700018B RID: 395
		// (get) Token: 0x060006EC RID: 1772 RVA: 0x0001E774 File Offset: 0x0001C974
		public static ReferenceIMBEvent<Hero, bool> CanHeroBecomePrisonerEvent
		{
			get
			{
				return CampaignEvents.Instance._canHeroBecomePrisonerEvent;
			}
		}

		// Token: 0x060006ED RID: 1773 RVA: 0x0001E780 File Offset: 0x0001C980
		public override void CanHeroBecomePrisoner(Hero hero, ref bool result)
		{
			CampaignEvents.Instance._canHeroBecomePrisonerEvent.Invoke(hero, ref result);
		}

		// Token: 0x1700018C RID: 396
		// (get) Token: 0x060006EE RID: 1774 RVA: 0x0001E793 File Offset: 0x0001C993
		public static ReferenceIMBEvent<Hero, bool> CanMoveToSettlementEvent
		{
			get
			{
				return CampaignEvents.Instance._canMoveToSettlementEvent;
			}
		}

		// Token: 0x060006EF RID: 1775 RVA: 0x0001E79F File Offset: 0x0001C99F
		public override void CanMoveToSettlement(Hero hero, ref bool result)
		{
			CampaignEvents.Instance._canMoveToSettlementEvent.Invoke(hero, ref result);
		}

		// Token: 0x1700018D RID: 397
		// (get) Token: 0x060006F0 RID: 1776 RVA: 0x0001E7B2 File Offset: 0x0001C9B2
		public static ReferenceIMBEvent<Hero, bool> CanHaveQuestsOrIssuesEvent
		{
			get
			{
				return CampaignEvents.Instance._canHaveQuestsOrIssues;
			}
		}

		// Token: 0x060006F1 RID: 1777 RVA: 0x0001E7BE File Offset: 0x0001C9BE
		public override void CanHaveQuestsOrIssues(Hero hero, ref bool result)
		{
			CampaignEvents.Instance._canHaveQuestsOrIssues.Invoke(hero, ref result);
		}

		// Token: 0x1700018E RID: 398
		// (get) Token: 0x060006F2 RID: 1778 RVA: 0x0001E7D1 File Offset: 0x0001C9D1
		public static IMbEvent<Hero> OnHeroUnregisteredEvent
		{
			get
			{
				return CampaignEvents.Instance._onHeroUnregisteredEvent;
			}
		}

		// Token: 0x060006F3 RID: 1779 RVA: 0x0001E7DD File Offset: 0x0001C9DD
		public override void OnHeroUnregistered(Hero hero)
		{
			CampaignEvents.Instance._onHeroUnregisteredEvent.Invoke(hero);
		}

		// Token: 0x04000189 RID: 393
		private readonly MbEvent _onPlayerBodyPropertiesChangedEvent = new MbEvent();

		// Token: 0x0400018A RID: 394
		private readonly MbEvent<BarterData> _barterablesRequested = new MbEvent<BarterData>();

		// Token: 0x0400018B RID: 395
		private readonly MbEvent<Hero, bool> _heroLevelledUp = new MbEvent<Hero, bool>();

		// Token: 0x0400018C RID: 396
		private readonly MbEvent<Hero, SkillObject, int, bool> _heroGainedSkill = new MbEvent<Hero, SkillObject, int, bool>();

		// Token: 0x0400018D RID: 397
		private readonly MbEvent _onCharacterCreationIsOverEvent = new MbEvent();

		// Token: 0x0400018E RID: 398
		private readonly MbEvent<Hero, bool> _onHeroCreated = new MbEvent<Hero, bool>();

		// Token: 0x0400018F RID: 399
		private readonly MbEvent<Hero, Occupation> _heroOccupationChangedEvent = new MbEvent<Hero, Occupation>();

		// Token: 0x04000190 RID: 400
		private readonly MbEvent<Hero> _onHeroWounded = new MbEvent<Hero>();

		// Token: 0x04000191 RID: 401
		private readonly MbEvent<Hero, Hero, List<Barterable>> _onBarterAcceptedEvent = new MbEvent<Hero, Hero, List<Barterable>>();

		// Token: 0x04000192 RID: 402
		private readonly MbEvent<Hero, Hero, List<Barterable>> _onBarterCanceledEvent = new MbEvent<Hero, Hero, List<Barterable>>();

		// Token: 0x04000193 RID: 403
		private readonly MbEvent<Hero, Hero, int, bool, ChangeRelationAction.ChangeRelationDetail, Hero, Hero> _heroRelationChanged = new MbEvent<Hero, Hero, int, bool, ChangeRelationAction.ChangeRelationDetail, Hero, Hero>();

		// Token: 0x04000194 RID: 404
		private readonly MbEvent<QuestBase, bool> _questLogAddedEvent = new MbEvent<QuestBase, bool>();

		// Token: 0x04000195 RID: 405
		private readonly MbEvent<IssueBase, bool> _issueLogAddedEvent = new MbEvent<IssueBase, bool>();

		// Token: 0x04000196 RID: 406
		private readonly MbEvent<Clan, bool> _clanTierIncrease = new MbEvent<Clan, bool>();

		// Token: 0x04000197 RID: 407
		private readonly MbEvent<Clan, Kingdom, Kingdom, ChangeKingdomAction.ChangeKingdomActionDetail, bool> _clanChangedKingdom = new MbEvent<Clan, Kingdom, Kingdom, ChangeKingdomAction.ChangeKingdomActionDetail, bool>();

		// Token: 0x04000198 RID: 408
		private readonly MbEvent<Clan> _onCompanionClanCreatedEvent = new MbEvent<Clan>();

		// Token: 0x04000199 RID: 409
		private readonly MbEvent<Hero, MobileParty> _onHeroJoinedPartyEvent = new MbEvent<Hero, MobileParty>();

		// Token: 0x0400019A RID: 410
		private readonly MbEvent<ValueTuple<Hero, PartyBase>, ValueTuple<Hero, PartyBase>, ValueTuple<int, string>, bool> _heroOrPartyTradedGold = new MbEvent<ValueTuple<Hero, PartyBase>, ValueTuple<Hero, PartyBase>, ValueTuple<int, string>, bool>();

		// Token: 0x0400019B RID: 411
		private readonly MbEvent<ValueTuple<Hero, PartyBase>, ValueTuple<Hero, PartyBase>, ItemObject, int, bool> _heroOrPartyGaveItem = new MbEvent<ValueTuple<Hero, PartyBase>, ValueTuple<Hero, PartyBase>, ItemObject, int, bool>();

		// Token: 0x0400019C RID: 412
		private readonly MbEvent<MobileParty> _banditPartyRecruited = new MbEvent<MobileParty>();

		// Token: 0x0400019D RID: 413
		private readonly MbEvent<KingdomDecision, bool> _kingdomDecisionAdded = new MbEvent<KingdomDecision, bool>();

		// Token: 0x0400019E RID: 414
		private readonly MbEvent<KingdomDecision, bool> _kingdomDecisionCancelled = new MbEvent<KingdomDecision, bool>();

		// Token: 0x0400019F RID: 415
		private readonly MbEvent<KingdomDecision, DecisionOutcome, bool> _kingdomDecisionConcluded = new MbEvent<KingdomDecision, DecisionOutcome, bool>();

		// Token: 0x040001A0 RID: 416
		private readonly MbEvent<MobileParty> _partyAttachedParty = new MbEvent<MobileParty>();

		// Token: 0x040001A1 RID: 417
		private readonly MbEvent<MobileParty> _nearbyPartyAddedToPlayerMapEvent = new MbEvent<MobileParty>();

		// Token: 0x040001A2 RID: 418
		private readonly MbEvent<Army> _armyCreated = new MbEvent<Army>();

		// Token: 0x040001A3 RID: 419
		private readonly MbEvent<Army, Army.ArmyDispersionReason, bool> _armyDispersed = new MbEvent<Army, Army.ArmyDispersionReason, bool>();

		// Token: 0x040001A4 RID: 420
		private readonly MbEvent<Army, Settlement> _armyGathered = new MbEvent<Army, Settlement>();

		// Token: 0x040001A5 RID: 421
		private readonly MbEvent<Hero, PerkObject> _perkOpenedEvent = new MbEvent<Hero, PerkObject>();

		// Token: 0x040001A6 RID: 422
		private readonly MbEvent<TraitObject, int> _playerTraitChangedEvent = new MbEvent<TraitObject, int>();

		// Token: 0x040001A7 RID: 423
		private readonly MbEvent<Village, Village.VillageStates, Village.VillageStates, MobileParty> _villageStateChanged = new MbEvent<Village, Village.VillageStates, Village.VillageStates, MobileParty>();

		// Token: 0x040001A8 RID: 424
		private readonly MbEvent<MobileParty, Settlement, Hero> _settlementEntered = new MbEvent<MobileParty, Settlement, Hero>();

		// Token: 0x040001A9 RID: 425
		private readonly MbEvent<MobileParty, Settlement, Hero> _afterSettlementEntered = new MbEvent<MobileParty, Settlement, Hero>();

		// Token: 0x040001AA RID: 426
		private readonly MbEvent<Town, CharacterObject, CharacterObject> _mercenaryTroopChangedInTown = new MbEvent<Town, CharacterObject, CharacterObject>();

		// Token: 0x040001AB RID: 427
		private readonly MbEvent<Town, int, int> _mercenaryNumberChangedInTown = new MbEvent<Town, int, int>();

		// Token: 0x040001AC RID: 428
		private readonly MbEvent<Alley, Hero, Hero> _alleyOwnerChanged = new MbEvent<Alley, Hero, Hero>();

		// Token: 0x040001AD RID: 429
		private readonly MbEvent<Alley, TroopRoster> _alleyOccupiedByPlayer = new MbEvent<Alley, TroopRoster>();

		// Token: 0x040001AE RID: 430
		private readonly MbEvent<Alley> _alleyClearedByPlayer = new MbEvent<Alley>();

		// Token: 0x040001AF RID: 431
		private readonly MbEvent<Hero, Hero, Romance.RomanceLevelEnum> _romanticStateChanged = new MbEvent<Hero, Hero, Romance.RomanceLevelEnum>();

		// Token: 0x040001B0 RID: 432
		private readonly MbEvent<Hero, Hero, bool> _heroesMarried = new MbEvent<Hero, Hero, bool>();

		// Token: 0x040001B1 RID: 433
		private readonly MbEvent<int, Town> _playerEliminatedFromTournament = new MbEvent<int, Town>();

		// Token: 0x040001B2 RID: 434
		private readonly MbEvent<Town> _playerStartedTournamentMatch = new MbEvent<Town>();

		// Token: 0x040001B3 RID: 435
		private readonly MbEvent<Town> _tournamentStarted = new MbEvent<Town>();

		// Token: 0x040001B4 RID: 436
		private readonly MbEvent<IFaction, IFaction, DeclareWarAction.DeclareWarDetail> _warDeclared = new MbEvent<IFaction, IFaction, DeclareWarAction.DeclareWarDetail>();

		// Token: 0x040001B5 RID: 437
		private readonly MbEvent<CharacterObject, MBReadOnlyList<CharacterObject>, Town, ItemObject> _tournamentFinished = new MbEvent<CharacterObject, MBReadOnlyList<CharacterObject>, Town, ItemObject>();

		// Token: 0x040001B6 RID: 438
		private readonly MbEvent<Town> _tournamentCancelled = new MbEvent<Town>();

		// Token: 0x040001B7 RID: 439
		private readonly MbEvent<PartyBase, PartyBase, object, bool> _battleStarted = new MbEvent<PartyBase, PartyBase, object, bool>();

		// Token: 0x040001B8 RID: 440
		private readonly MbEvent<Settlement, Clan> _rebellionFinished = new MbEvent<Settlement, Clan>();

		// Token: 0x040001B9 RID: 441
		private readonly MbEvent<Town, bool> _townRebelliousStateChanged = new MbEvent<Town, bool>();

		// Token: 0x040001BA RID: 442
		private readonly MbEvent<Settlement, Clan> _rebelliousClanDisbandedAtSettlement = new MbEvent<Settlement, Clan>();

		// Token: 0x040001BB RID: 443
		private readonly MbEvent<MobileParty, ItemRoster> _itemsLooted = new MbEvent<MobileParty, ItemRoster>();

		// Token: 0x040001BC RID: 444
		private readonly MbEvent<MobileParty, PartyBase> _mobilePartyDestroyed = new MbEvent<MobileParty, PartyBase>();

		// Token: 0x040001BD RID: 445
		private readonly MbEvent<MobileParty> _mobilePartyCreated = new MbEvent<MobileParty>();

		// Token: 0x040001BE RID: 446
		private readonly MbEvent<Hero, Hero, KillCharacterAction.KillCharacterActionDetail, bool> _heroKilled = new MbEvent<Hero, Hero, KillCharacterAction.KillCharacterActionDetail, bool>();

		// Token: 0x040001BF RID: 447
		private readonly MbEvent<Hero, Hero, KillCharacterAction.KillCharacterActionDetail, bool> _onBeforeHeroKilled = new MbEvent<Hero, Hero, KillCharacterAction.KillCharacterActionDetail, bool>();

		// Token: 0x040001C0 RID: 448
		private readonly MbEvent<Hero, int> _childEducationCompleted = new MbEvent<Hero, int>();

		// Token: 0x040001C1 RID: 449
		private readonly MbEvent<Hero> _heroComesOfAge = new MbEvent<Hero>();

		// Token: 0x040001C2 RID: 450
		private readonly MbEvent<Hero> _heroGrowsOutOfInfancyEvent = new MbEvent<Hero>();

		// Token: 0x040001C3 RID: 451
		private readonly MbEvent<Hero> _heroReachesTeenAgeEvent = new MbEvent<Hero>();

		// Token: 0x040001C4 RID: 452
		private readonly MbEvent<Hero, Hero> _characterDefeated = new MbEvent<Hero, Hero>();

		// Token: 0x040001C5 RID: 453
		private readonly MbEvent<Kingdom, Clan> _rulingClanChanged = new MbEvent<Kingdom, Clan>();

		// Token: 0x040001C6 RID: 454
		private readonly MbEvent<PartyBase, Hero> _heroPrisonerTaken = new MbEvent<PartyBase, Hero>();

		// Token: 0x040001C7 RID: 455
		private readonly MbEvent<Hero, PartyBase, IFaction, EndCaptivityDetail> _heroPrisonerReleased = new MbEvent<Hero, PartyBase, IFaction, EndCaptivityDetail>();

		// Token: 0x040001C8 RID: 456
		private readonly MbEvent<Hero> _characterBecameFugitive = new MbEvent<Hero>();

		// Token: 0x040001C9 RID: 457
		private readonly MbEvent<Hero> _playerMetHero = new MbEvent<Hero>();

		// Token: 0x040001CA RID: 458
		private readonly MbEvent<Hero> _playerLearnsAboutHero = new MbEvent<Hero>();

		// Token: 0x040001CB RID: 459
		private readonly MbEvent<Hero, int, bool> _renownGained = new MbEvent<Hero, int, bool>();

		// Token: 0x040001CC RID: 460
		private readonly MbEvent<IFaction, float> _crimeRatingChanged = new MbEvent<IFaction, float>();

		// Token: 0x040001CD RID: 461
		private readonly MbEvent<Hero> _newCompanionAdded = new MbEvent<Hero>();

		// Token: 0x040001CE RID: 462
		private readonly MbEvent<IMission> _afterMissionStarted = new MbEvent<IMission>();

		// Token: 0x040001CF RID: 463
		private readonly MbEvent<MenuCallbackArgs> _gameMenuOpened = new MbEvent<MenuCallbackArgs>();

		// Token: 0x040001D0 RID: 464
		private readonly MbEvent<MenuCallbackArgs> _afterGameMenuOpenedEvent = new MbEvent<MenuCallbackArgs>();

		// Token: 0x040001D1 RID: 465
		private readonly MbEvent<MenuCallbackArgs> _beforeGameMenuOpenedEvent = new MbEvent<MenuCallbackArgs>();

		// Token: 0x040001D2 RID: 466
		private readonly MbEvent<IFaction, IFaction, MakePeaceAction.MakePeaceDetail> _makePeace = new MbEvent<IFaction, IFaction, MakePeaceAction.MakePeaceDetail>();

		// Token: 0x040001D3 RID: 467
		private readonly MbEvent<Kingdom> _kingdomDestroyed = new MbEvent<Kingdom>();

		// Token: 0x040001D4 RID: 468
		private readonly MbEvent<Kingdom> _kingdomCreated = new MbEvent<Kingdom>();

		// Token: 0x040001D5 RID: 469
		private readonly MbEvent<Village> _villageBecomeNormal = new MbEvent<Village>();

		// Token: 0x040001D6 RID: 470
		private readonly MbEvent<Village> _villageBeingRaided = new MbEvent<Village>();

		// Token: 0x040001D7 RID: 471
		private readonly MbEvent<Village> _villageLooted = new MbEvent<Village>();

		// Token: 0x040001D8 RID: 472
		private readonly MbEvent<Hero, RemoveCompanionAction.RemoveCompanionDetail> _companionRemoved = new MbEvent<Hero, RemoveCompanionAction.RemoveCompanionDetail>();

		// Token: 0x040001D9 RID: 473
		private readonly MbEvent<IAgent> _onAgentJoinedConversationEvent = new MbEvent<IAgent>();

		// Token: 0x040001DA RID: 474
		private readonly MbEvent<IEnumerable<CharacterObject>> _onConversationEnded = new MbEvent<IEnumerable<CharacterObject>>();

		// Token: 0x040001DB RID: 475
		private readonly MbEvent<MapEvent> _mapEventEnded = new MbEvent<MapEvent>();

		// Token: 0x040001DC RID: 476
		private readonly MbEvent<MapEvent, PartyBase, PartyBase> _mapEventStarted = new MbEvent<MapEvent, PartyBase, PartyBase>();

		// Token: 0x040001DD RID: 477
		private readonly MbEvent<Settlement, FlattenedTroopRoster, Hero, bool> _prisonersChangeInSettlement = new MbEvent<Settlement, FlattenedTroopRoster, Hero, bool>();

		// Token: 0x040001DE RID: 478
		private readonly MbEvent<Hero, BoardGameHelper.BoardGameState> _onPlayerBoardGameOver = new MbEvent<Hero, BoardGameHelper.BoardGameState>();

		// Token: 0x040001DF RID: 479
		private readonly MbEvent<Hero> _onRansomOfferedToPlayer = new MbEvent<Hero>();

		// Token: 0x040001E0 RID: 480
		private readonly MbEvent<Hero> _onRansomOfferCancelled = new MbEvent<Hero>();

		// Token: 0x040001E1 RID: 481
		private readonly MbEvent<IFaction, int> _onPeaceOfferedToPlayer = new MbEvent<IFaction, int>();

		// Token: 0x040001E2 RID: 482
		private readonly MbEvent<IFaction> _onPeaceOfferCancelled = new MbEvent<IFaction>();

		// Token: 0x040001E3 RID: 483
		private readonly MbEvent<Hero, Hero> _onMarriageOfferedToPlayerEvent = new MbEvent<Hero, Hero>();

		// Token: 0x040001E4 RID: 484
		private readonly MbEvent<Hero, Hero> _onMarriageOfferCanceledEvent = new MbEvent<Hero, Hero>();

		// Token: 0x040001E5 RID: 485
		private readonly MbEvent<Kingdom> _onVassalOrMercenaryServiceOfferedToPlayerEvent = new MbEvent<Kingdom>();

		// Token: 0x040001E6 RID: 486
		private readonly MbEvent<Kingdom> _onVassalOrMercenaryServiceOfferCanceledEvent = new MbEvent<Kingdom>();

		// Token: 0x040001E7 RID: 487
		private readonly MbEvent<IMission> _onMissionStartedEvent = new MbEvent<IMission>();

		// Token: 0x040001E8 RID: 488
		private readonly MbEvent _beforeMissionOpenedEvent = new MbEvent();

		// Token: 0x040001E9 RID: 489
		private readonly MbEvent<PartyBase> _onPartyRemovedEvent = new MbEvent<PartyBase>();

		// Token: 0x040001EA RID: 490
		private readonly MbEvent<PartyBase> _onPartySizeChangedEvent = new MbEvent<PartyBase>();

		// Token: 0x040001EB RID: 491
		private readonly MbEvent<Settlement, bool, Hero, Hero, Hero, ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail> _onSettlementOwnerChangedEvent = new MbEvent<Settlement, bool, Hero, Hero, Hero, ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail>();

		// Token: 0x040001EC RID: 492
		private readonly MbEvent<Town, Hero, Hero> _onGovernorChangedEvent = new MbEvent<Town, Hero, Hero>();

		// Token: 0x040001ED RID: 493
		private readonly MbEvent<MobileParty, Settlement> _onSettlementLeftEvent = new MbEvent<MobileParty, Settlement>();

		// Token: 0x040001EE RID: 494
		private readonly MbEvent _weeklyTickEvent = new MbEvent();

		// Token: 0x040001EF RID: 495
		private readonly MbEvent _dailyTickEvent = new MbEvent();

		// Token: 0x040001F0 RID: 496
		private readonly MbEvent<MobileParty> _dailyTickPartyEvent = new MbEvent<MobileParty>();

		// Token: 0x040001F1 RID: 497
		private readonly MbEvent<Town> _dailyTickTownEvent = new MbEvent<Town>();

		// Token: 0x040001F2 RID: 498
		private readonly MbEvent<Settlement> _dailyTickSettlementEvent = new MbEvent<Settlement>();

		// Token: 0x040001F3 RID: 499
		private readonly MbEvent<Hero> _dailyTickHeroEvent = new MbEvent<Hero>();

		// Token: 0x040001F4 RID: 500
		private readonly MbEvent<Clan> _dailyTickClanEvent = new MbEvent<Clan>();

		// Token: 0x040001F5 RID: 501
		private readonly MbEvent<List<CampaignTutorial>> _collectAvailableTutorialsEvent = new MbEvent<List<CampaignTutorial>>();

		// Token: 0x040001F6 RID: 502
		private readonly MbEvent<string> _onTutorialCompletedEvent = new MbEvent<string>();

		// Token: 0x040001F7 RID: 503
		private readonly MbEvent<Town, Building, int> _onBuildingLevelChangedEvent = new MbEvent<Town, Building, int>();

		// Token: 0x040001F8 RID: 504
		private readonly MbEvent _hourlyTickEvent = new MbEvent();

		// Token: 0x040001F9 RID: 505
		private readonly MbEvent<MobileParty> _hourlyTickPartyEvent = new MbEvent<MobileParty>();

		// Token: 0x040001FA RID: 506
		private readonly MbEvent<Settlement> _hourlyTickSettlementEvent = new MbEvent<Settlement>();

		// Token: 0x040001FB RID: 507
		private readonly MbEvent<Clan> _hourlyTickClanEvent = new MbEvent<Clan>();

		// Token: 0x040001FC RID: 508
		private readonly MbEvent<float> _tickEvent = new MbEvent<float>();

		// Token: 0x040001FD RID: 509
		private readonly MbEvent<CampaignGameStarter> _onSessionLaunchedEvent = new MbEvent<CampaignGameStarter>();

		// Token: 0x040001FE RID: 510
		private readonly MbEvent<CampaignGameStarter> _onAfterSessionLaunchedEvent = new MbEvent<CampaignGameStarter>();

		// Token: 0x040001FF RID: 511
		public const int OnNewGameCreatedPartialFollowUpEventMaxIndex = 100;

		// Token: 0x04000200 RID: 512
		private readonly MbEvent<CampaignGameStarter> _onNewGameCreatedEvent = new MbEvent<CampaignGameStarter>();

		// Token: 0x04000201 RID: 513
		private readonly MbEvent<CampaignGameStarter, int> _onNewGameCreatedPartialFollowUpEvent = new MbEvent<CampaignGameStarter, int>();

		// Token: 0x04000202 RID: 514
		private readonly MbEvent<CampaignGameStarter> _onNewGameCreatedPartialFollowUpEndEvent = new MbEvent<CampaignGameStarter>();

		// Token: 0x04000203 RID: 515
		private readonly MbEvent<CampaignGameStarter> _onGameEarlyLoadedEvent = new MbEvent<CampaignGameStarter>();

		// Token: 0x04000204 RID: 516
		private readonly MbEvent<CampaignGameStarter> _onGameLoadedEvent = new MbEvent<CampaignGameStarter>();

		// Token: 0x04000205 RID: 517
		private readonly MbEvent _onGameLoadFinishedEvent = new MbEvent();

		// Token: 0x04000206 RID: 518
		private readonly MbEvent<MobileParty, PartyThinkParams> _aiHourlyTickEvent = new MbEvent<MobileParty, PartyThinkParams>();

		// Token: 0x04000207 RID: 519
		private readonly MbEvent<MobileParty> _tickPartialHourlyAiEvent = new MbEvent<MobileParty>();

		// Token: 0x04000208 RID: 520
		private readonly MbEvent<MobileParty> _onPartyJoinedArmyEvent = new MbEvent<MobileParty>();

		// Token: 0x04000209 RID: 521
		private readonly MbEvent<MobileParty> _onPartyRemovedFromArmyEvent = new MbEvent<MobileParty>();

		// Token: 0x0400020A RID: 522
		private readonly MbEvent<Hero, Army.ArmyLeaderThinkReason> _onArmyLeaderThinkEvent = new MbEvent<Hero, Army.ArmyLeaderThinkReason>();

		// Token: 0x0400020B RID: 523
		private readonly MbEvent<IMission> _onMissionEndedEvent = new MbEvent<IMission>();

		// Token: 0x0400020C RID: 524
		private readonly MbEvent<MobileParty> _onQuarterDailyPartyTick = new MbEvent<MobileParty>();

		// Token: 0x0400020D RID: 525
		private readonly MbEvent<MapEvent> _onPlayerBattleEndEvent = new MbEvent<MapEvent>();

		// Token: 0x0400020E RID: 526
		private readonly MbEvent<CharacterObject, int> _onUnitRecruitedEvent = new MbEvent<CharacterObject, int>();

		// Token: 0x0400020F RID: 527
		private readonly MbEvent<Hero> _onChildConceived = new MbEvent<Hero>();

		// Token: 0x04000210 RID: 528
		private readonly MbEvent<Hero, List<Hero>, int> _onGivenBirthEvent = new MbEvent<Hero, List<Hero>, int>();

		// Token: 0x04000211 RID: 529
		private readonly MbEvent<float> _missionTickEvent = new MbEvent<float>();

		// Token: 0x04000212 RID: 530
		private readonly MbEvent _setupPreConversationEvent = new MbEvent();

		// Token: 0x04000213 RID: 531
		private MbEvent _armyOverlaySetDirty = new MbEvent();

		// Token: 0x04000214 RID: 532
		private readonly MbEvent<int> _playerDesertedBattle = new MbEvent<int>();

		// Token: 0x04000215 RID: 533
		private MbEvent<PartyBase> _partyVisibilityChanged = new MbEvent<PartyBase>();

		// Token: 0x04000216 RID: 534
		private readonly MbEvent<Track> _trackDetectedEvent = new MbEvent<Track>();

		// Token: 0x04000217 RID: 535
		private readonly MbEvent<Track> _trackLostEvent = new MbEvent<Track>();

		// Token: 0x04000218 RID: 536
		private readonly MbEvent<Dictionary<string, int>> _locationCharactersAreReadyToSpawn = new MbEvent<Dictionary<string, int>>();

		// Token: 0x04000219 RID: 537
		private readonly MbEvent _locationCharactersSimulatedSpawned = new MbEvent();

		// Token: 0x0400021A RID: 538
		private readonly MbEvent<CharacterObject, CharacterObject, int> _playerUpgradedTroopsEvent = new MbEvent<CharacterObject, CharacterObject, int>();

		// Token: 0x0400021B RID: 539
		private readonly MbEvent<CharacterObject, CharacterObject, PartyBase, WeaponComponentData, bool, int> _onHeroCombatHitEvent = new MbEvent<CharacterObject, CharacterObject, PartyBase, WeaponComponentData, bool, int>();

		// Token: 0x0400021C RID: 540
		private readonly MbEvent<CharacterObject> _characterPortraitPopUpOpenedEvent = new MbEvent<CharacterObject>();

		// Token: 0x0400021D RID: 541
		private CampaignTimeControlMode _timeControlModeBeforePopUpOpened;

		// Token: 0x0400021E RID: 542
		private readonly MbEvent _characterPortraitPopUpClosedEvent = new MbEvent();

		// Token: 0x0400021F RID: 543
		private readonly MbEvent<Hero> _playerStartTalkFromMenu = new MbEvent<Hero>();

		// Token: 0x04000220 RID: 544
		private readonly MbEvent<GameMenuOption> _gameMenuOptionSelectedEvent = new MbEvent<GameMenuOption>();

		// Token: 0x04000221 RID: 545
		private readonly MbEvent<CharacterObject> _playerStartRecruitmentEvent = new MbEvent<CharacterObject>();

		// Token: 0x04000222 RID: 546
		private readonly MbEvent<Hero, Hero> _onBeforePlayerCharacterChangedEvent = new MbEvent<Hero, Hero>();

		// Token: 0x04000223 RID: 547
		private readonly MbEvent<Hero, Hero, MobileParty, bool> _onPlayerCharacterChangedEvent = new MbEvent<Hero, Hero, MobileParty, bool>();

		// Token: 0x04000224 RID: 548
		private readonly MbEvent<Hero, Hero> _onClanLeaderChangedEvent = new MbEvent<Hero, Hero>();

		// Token: 0x04000225 RID: 549
		private readonly MbEvent<SiegeEvent> _onSiegeEventStartedEvent = new MbEvent<SiegeEvent>();

		// Token: 0x04000226 RID: 550
		private readonly MbEvent _onPlayerSiegeStartedEvent = new MbEvent();

		// Token: 0x04000227 RID: 551
		private readonly MbEvent<SiegeEvent> _onSiegeEventEndedEvent = new MbEvent<SiegeEvent>();

		// Token: 0x04000228 RID: 552
		private readonly MbEvent<MobileParty, Settlement, SiegeAftermathAction.SiegeAftermath, Clan, Dictionary<MobileParty, float>> _siegeAftermathAppliedEvent = new MbEvent<MobileParty, Settlement, SiegeAftermathAction.SiegeAftermath, Clan, Dictionary<MobileParty, float>>();

		// Token: 0x04000229 RID: 553
		private readonly MbEvent<MobileParty, Settlement, BattleSideEnum, SiegeEngineType, SiegeBombardTargets> _onSiegeBombardmentHitEvent = new MbEvent<MobileParty, Settlement, BattleSideEnum, SiegeEngineType, SiegeBombardTargets>();

		// Token: 0x0400022A RID: 554
		private readonly MbEvent<MobileParty, Settlement, BattleSideEnum, SiegeEngineType, bool> _onSiegeBombardmentWallHitEvent = new MbEvent<MobileParty, Settlement, BattleSideEnum, SiegeEngineType, bool>();

		// Token: 0x0400022B RID: 555
		private readonly MbEvent<MobileParty, Settlement, BattleSideEnum, SiegeEngineType> _onSiegeEngineDestroyedEvent = new MbEvent<MobileParty, Settlement, BattleSideEnum, SiegeEngineType>();

		// Token: 0x0400022C RID: 556
		private readonly MbEvent<List<TradeRumor>, Settlement> _onTradeRumorIsTakenEvent = new MbEvent<List<TradeRumor>, Settlement>();

		// Token: 0x0400022D RID: 557
		private readonly MbEvent<Hero> _onCheckForIssueEvent = new MbEvent<Hero>();

		// Token: 0x0400022E RID: 558
		private readonly MbEvent<IssueBase, IssueBase.IssueUpdateDetails, Hero> _onIssueUpdatedEvent = new MbEvent<IssueBase, IssueBase.IssueUpdateDetails, Hero>();

		// Token: 0x0400022F RID: 559
		private readonly MbEvent<MobileParty, TroopRoster> _onTroopsDesertedEvent = new MbEvent<MobileParty, TroopRoster>();

		// Token: 0x04000230 RID: 560
		private readonly MbEvent<Hero, Settlement, Hero, CharacterObject, int> _onTroopRecruitedEvent = new MbEvent<Hero, Settlement, Hero, CharacterObject, int>();

		// Token: 0x04000231 RID: 561
		private readonly MbEvent<Hero, Settlement, TroopRoster> _onTroopGivenToSettlementEvent = new MbEvent<Hero, Settlement, TroopRoster>();

		// Token: 0x04000232 RID: 562
		private readonly MbEvent<PartyBase, PartyBase, ItemRosterElement, int, Settlement> _onItemSoldEvent = new MbEvent<PartyBase, PartyBase, ItemRosterElement, int, Settlement>();

		// Token: 0x04000233 RID: 563
		private readonly MbEvent<MobileParty, Town, List<ValueTuple<EquipmentElement, int>>> _onCaravanTransactionCompletedEvent = new MbEvent<MobileParty, Town, List<ValueTuple<EquipmentElement, int>>>();

		// Token: 0x04000234 RID: 564
		private readonly MbEvent<MobileParty, TroopRoster, Settlement> _onPrisonerSoldEvent = new MbEvent<MobileParty, TroopRoster, Settlement>();

		// Token: 0x04000235 RID: 565
		private readonly MbEvent<MobileParty> _onPartyDisbandStartedEvent = new MbEvent<MobileParty>();

		// Token: 0x04000236 RID: 566
		private readonly MbEvent<MobileParty, Settlement> _onPartyDisbandedEvent = new MbEvent<MobileParty, Settlement>();

		// Token: 0x04000237 RID: 567
		private readonly MbEvent<MobileParty> _onPartyDisbandCanceledEvent = new MbEvent<MobileParty>();

		// Token: 0x04000238 RID: 568
		private readonly MbEvent<PartyBase, PartyBase> _hideoutSpottedEvent = new MbEvent<PartyBase, PartyBase>();

		// Token: 0x04000239 RID: 569
		private readonly MbEvent<Settlement> _hideoutDeactivatedEvent = new MbEvent<Settlement>();

		// Token: 0x0400023A RID: 570
		private readonly MbEvent<Hero, Hero, float> _heroSharedFoodWithAnotherHeroEvent = new MbEvent<Hero, Hero, float>();

		// Token: 0x0400023B RID: 571
		private readonly MbEvent<List<ValueTuple<ItemRosterElement, int>>, List<ValueTuple<ItemRosterElement, int>>, bool> _playerInventoryExchangeEvent = new MbEvent<List<ValueTuple<ItemRosterElement, int>>, List<ValueTuple<ItemRosterElement, int>>, bool>();

		// Token: 0x0400023C RID: 572
		private readonly MbEvent<ItemRoster> _onItemsDiscardedByPlayerEvent = new MbEvent<ItemRoster>();

		// Token: 0x0400023D RID: 573
		private readonly MbEvent<Tuple<PersuasionOptionArgs, PersuasionOptionResult>> _persuasionProgressCommittedEvent = new MbEvent<Tuple<PersuasionOptionArgs, PersuasionOptionResult>>();

		// Token: 0x0400023E RID: 574
		private readonly MbEvent<QuestBase, QuestBase.QuestCompleteDetails> _onQuestCompletedEvent = new MbEvent<QuestBase, QuestBase.QuestCompleteDetails>();

		// Token: 0x0400023F RID: 575
		private readonly MbEvent<QuestBase> _onQuestStartedEvent = new MbEvent<QuestBase>();

		// Token: 0x04000240 RID: 576
		private readonly MbEvent<ItemObject, Settlement, int> _itemProducedEvent = new MbEvent<ItemObject, Settlement, int>();

		// Token: 0x04000241 RID: 577
		private readonly MbEvent<ItemObject, Settlement, int> _itemConsumedEvent = new MbEvent<ItemObject, Settlement, int>();

		// Token: 0x04000242 RID: 578
		private readonly MbEvent<MobileParty> _onPartyConsumedFoodEvent = new MbEvent<MobileParty>();

		// Token: 0x04000243 RID: 579
		private readonly MbEvent<Hero, Hero, KillCharacterAction.KillCharacterActionDetail, bool> _onBeforeMainCharacterDiedEvent = new MbEvent<Hero, Hero, KillCharacterAction.KillCharacterActionDetail, bool>();

		// Token: 0x04000244 RID: 580
		private readonly MbEvent<IssueBase> _onNewIssueCreatedEvent = new MbEvent<IssueBase>();

		// Token: 0x04000245 RID: 581
		private readonly MbEvent<IssueBase, Hero> _onIssueOwnerChangedEvent = new MbEvent<IssueBase, Hero>();

		// Token: 0x04000246 RID: 582
		private readonly MbEvent _onGameOverEvent = new MbEvent();

		// Token: 0x04000247 RID: 583
		private readonly MbEvent<Settlement, MobileParty, bool, MapEvent.BattleTypes> _siegeCompletedEvent = new MbEvent<Settlement, MobileParty, bool, MapEvent.BattleTypes>();

		// Token: 0x04000248 RID: 584
		private readonly MbEvent<SiegeEvent, BattleSideEnum, SiegeEngineType> _siegeEngineBuiltEvent = new MbEvent<SiegeEvent, BattleSideEnum, SiegeEngineType>();

		// Token: 0x04000249 RID: 585
		private readonly MbEvent<BattleSideEnum, RaidEventComponent> _raidCompletedEvent = new MbEvent<BattleSideEnum, RaidEventComponent>();

		// Token: 0x0400024A RID: 586
		private readonly MbEvent<BattleSideEnum, ForceVolunteersEventComponent> _forceVolunteersCompletedEvent = new MbEvent<BattleSideEnum, ForceVolunteersEventComponent>();

		// Token: 0x0400024B RID: 587
		private readonly MbEvent<BattleSideEnum, ForceSuppliesEventComponent> _forceSuppliesCompletedEvent = new MbEvent<BattleSideEnum, ForceSuppliesEventComponent>();

		// Token: 0x0400024C RID: 588
		private readonly MbEvent<BattleSideEnum, MapEvent> _hideoutBattleCompletedEvent = new MbEvent<BattleSideEnum, MapEvent>();

		// Token: 0x0400024D RID: 589
		private readonly MbEvent<Clan> _onClanDestroyedEvent = new MbEvent<Clan>();

		// Token: 0x0400024E RID: 590
		private readonly MbEvent<ItemObject, Crafting.OverrideData, bool> _onNewItemCraftedEvent = new MbEvent<ItemObject, Crafting.OverrideData, bool>();

		// Token: 0x0400024F RID: 591
		private readonly MbEvent<CraftingPiece> _craftingPartUnlockedEvent = new MbEvent<CraftingPiece>();

		// Token: 0x04000250 RID: 592
		private readonly MbEvent<Workshop, Hero, WorkshopType> _onWorkshopChangedEvent = new MbEvent<Workshop, Hero, WorkshopType>();

		// Token: 0x04000251 RID: 593
		private readonly MbEvent _onBeforeSaveEvent = new MbEvent();

		// Token: 0x04000252 RID: 594
		private readonly MbEvent _onSaveStartedEvent = new MbEvent();

		// Token: 0x04000253 RID: 595
		private readonly MbEvent<bool, string> _onSaveOverEvent = new MbEvent<bool, string>();

		// Token: 0x04000254 RID: 596
		private readonly MbEvent<FlattenedTroopRoster> _onPrisonerTakenEvent = new MbEvent<FlattenedTroopRoster>();

		// Token: 0x04000255 RID: 597
		private readonly MbEvent<FlattenedTroopRoster> _onPrisonerReleasedEvent = new MbEvent<FlattenedTroopRoster>();

		// Token: 0x04000256 RID: 598
		private readonly MbEvent<FlattenedTroopRoster> _onMainPartyPrisonerRecruitedEvent = new MbEvent<FlattenedTroopRoster>();

		// Token: 0x04000257 RID: 599
		private readonly MbEvent<MobileParty, FlattenedTroopRoster, Settlement> _onPrisonerDonatedToSettlementEvent = new MbEvent<MobileParty, FlattenedTroopRoster, Settlement>();

		// Token: 0x04000258 RID: 600
		private readonly MbEvent<Hero, EquipmentElement> _onEquipmentSmeltedByHero = new MbEvent<Hero, EquipmentElement>();

		// Token: 0x04000259 RID: 601
		private readonly MbEvent<int> _onPlayerTradeProfit = new MbEvent<int>();

		// Token: 0x0400025A RID: 602
		private readonly MbEvent<Hero, Clan> _onHeroChangedClan = new MbEvent<Hero, Clan>();

		// Token: 0x0400025B RID: 603
		private readonly MbEvent<Hero, HeroGetsBusyReasons> _onHeroGetsBusy = new MbEvent<Hero, HeroGetsBusyReasons>();

		// Token: 0x0400025C RID: 604
		private readonly MbEvent<MapEvent, PartyBase, Dictionary<PartyBase, ItemRoster>, ItemRoster, MBList<TroopRosterElement>, float> _collectLootsEvent = new MbEvent<MapEvent, PartyBase, Dictionary<PartyBase, ItemRoster>, ItemRoster, MBList<TroopRosterElement>, float>();

		// Token: 0x0400025D RID: 605
		private readonly MbEvent<MapEvent, PartyBase, Dictionary<PartyBase, ItemRoster>> _distributeLootToPartyEvent = new MbEvent<MapEvent, PartyBase, Dictionary<PartyBase, ItemRoster>>();

		// Token: 0x0400025E RID: 606
		private readonly MbEvent<Hero, Settlement, MobileParty, TeleportHeroAction.TeleportationDetail> _onHeroTeleportationRequestedEvent = new MbEvent<Hero, Settlement, MobileParty, TeleportHeroAction.TeleportationDetail>();

		// Token: 0x0400025F RID: 607
		private readonly MbEvent<MobileParty> _onPartyLeaderChangeOfferCanceledEvent = new MbEvent<MobileParty>();

		// Token: 0x04000260 RID: 608
		private readonly MbEvent<Clan, float> _onClanInfluenceChangedEvent = new MbEvent<Clan, float>();

		// Token: 0x04000261 RID: 609
		private readonly MbEvent<CharacterObject> _onPlayerPartyKnockedOrKilledTroopEvent = new MbEvent<CharacterObject>();

		// Token: 0x04000262 RID: 610
		private readonly MbEvent<DefaultClanFinanceModel.AssetIncomeType, int> _onPlayerEarnedGoldFromAssetEvent = new MbEvent<DefaultClanFinanceModel.AssetIncomeType, int>();

		// Token: 0x04000263 RID: 611
		private readonly MbEvent _onMainPartyStarving = new MbEvent();

		// Token: 0x04000264 RID: 612
		private readonly MbEvent<Town, bool> _onPlayerJoinedTournamentEvent = new MbEvent<Town, bool>();

		// Token: 0x04000265 RID: 613
		private readonly ReferenceMBEvent<Hero, bool> _canHeroLeadPartyEvent = new ReferenceMBEvent<Hero, bool>();

		// Token: 0x04000266 RID: 614
		private readonly ReferenceMBEvent<Hero, bool> _canMarryEvent = new ReferenceMBEvent<Hero, bool>();

		// Token: 0x04000267 RID: 615
		private readonly ReferenceMBEvent<Hero, bool> _canHeroEquipmentBeChangedEvent = new ReferenceMBEvent<Hero, bool>();

		// Token: 0x04000268 RID: 616
		private readonly ReferenceMBEvent<Hero, bool> _canBeGovernorOrHavePartyRoleEvent = new ReferenceMBEvent<Hero, bool>();

		// Token: 0x04000269 RID: 617
		private readonly ReferenceMBEvent<Hero, KillCharacterAction.KillCharacterActionDetail, bool> _canHeroDieEvent = new ReferenceMBEvent<Hero, KillCharacterAction.KillCharacterActionDetail, bool>();

		// Token: 0x0400026A RID: 618
		private readonly ReferenceMBEvent<Hero, bool> _canHeroBecomePrisonerEvent = new ReferenceMBEvent<Hero, bool>();

		// Token: 0x0400026B RID: 619
		private readonly ReferenceMBEvent<Hero, bool> _canMoveToSettlementEvent = new ReferenceMBEvent<Hero, bool>();

		// Token: 0x0400026C RID: 620
		private readonly ReferenceMBEvent<Hero, bool> _canHaveQuestsOrIssues = new ReferenceMBEvent<Hero, bool>();

		// Token: 0x0400026D RID: 621
		private readonly MbEvent<Hero> _onHeroUnregisteredEvent = new MbEvent<Hero>();
	}
}
