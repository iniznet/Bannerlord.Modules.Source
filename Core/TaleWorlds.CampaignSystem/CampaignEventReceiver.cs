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
	// Token: 0x02000035 RID: 53
	public abstract class CampaignEventReceiver
	{
		// Token: 0x0600044A RID: 1098 RVA: 0x0001BEAF File Offset: 0x0001A0AF
		public virtual void RemoveListeners(object o)
		{
		}

		// Token: 0x0600044B RID: 1099 RVA: 0x0001BEB1 File Offset: 0x0001A0B1
		public virtual void OnCharacterCreationIsOver()
		{
		}

		// Token: 0x0600044C RID: 1100 RVA: 0x0001BEB3 File Offset: 0x0001A0B3
		public virtual void OnHeroLevelledUp(Hero hero, bool shouldNotify = true)
		{
		}

		// Token: 0x0600044D RID: 1101 RVA: 0x0001BEB5 File Offset: 0x0001A0B5
		public virtual void OnHeroGainedSkill(Hero hero, SkillObject skill, int change = 1, bool shouldNotify = true)
		{
		}

		// Token: 0x0600044E RID: 1102 RVA: 0x0001BEB7 File Offset: 0x0001A0B7
		public virtual void OnHeroCreated(Hero hero, bool isBornNaturally = false)
		{
		}

		// Token: 0x0600044F RID: 1103 RVA: 0x0001BEB9 File Offset: 0x0001A0B9
		public virtual void OnHeroWounded(Hero woundedHero)
		{
		}

		// Token: 0x06000450 RID: 1104 RVA: 0x0001BEBB File Offset: 0x0001A0BB
		public virtual void OnHeroRelationChanged(Hero effectiveHero, Hero effectiveHeroGainedRelationWith, int relationChange, bool showNotification, ChangeRelationAction.ChangeRelationDetail detail, Hero originalHero, Hero originalGainedRelationWith)
		{
		}

		// Token: 0x06000451 RID: 1105 RVA: 0x0001BEBD File Offset: 0x0001A0BD
		public virtual void OnQuestLogAdded(QuestBase quest, bool hideInformation)
		{
		}

		// Token: 0x06000452 RID: 1106 RVA: 0x0001BEBF File Offset: 0x0001A0BF
		public virtual void OnIssueLogAdded(IssueBase issue, bool hideInformation)
		{
		}

		// Token: 0x06000453 RID: 1107 RVA: 0x0001BEC1 File Offset: 0x0001A0C1
		public virtual void OnClanTierChanged(Clan clan, bool shouldNotify = true)
		{
		}

		// Token: 0x06000454 RID: 1108 RVA: 0x0001BEC3 File Offset: 0x0001A0C3
		public virtual void OnClanChangedKingdom(Clan clan, Kingdom oldKingdom, Kingdom newKingdom, ChangeKingdomAction.ChangeKingdomActionDetail actionDetail, bool showNotification = true)
		{
		}

		// Token: 0x06000455 RID: 1109 RVA: 0x0001BEC5 File Offset: 0x0001A0C5
		public virtual void OnCompanionClanCreated(Clan clan)
		{
		}

		// Token: 0x06000456 RID: 1110 RVA: 0x0001BEC7 File Offset: 0x0001A0C7
		public virtual void OnHeroJoinedParty(Hero hero, MobileParty mobileParty)
		{
		}

		// Token: 0x06000457 RID: 1111 RVA: 0x0001BEC9 File Offset: 0x0001A0C9
		public virtual void OnKingdomDecisionAdded(KingdomDecision decision, bool isPlayerInvolved)
		{
		}

		// Token: 0x06000458 RID: 1112 RVA: 0x0001BECB File Offset: 0x0001A0CB
		public virtual void OnKingdomDecisionCancelled(KingdomDecision decision, bool isPlayerInvolved)
		{
		}

		// Token: 0x06000459 RID: 1113 RVA: 0x0001BECD File Offset: 0x0001A0CD
		public virtual void OnKingdomDecisionConcluded(KingdomDecision decision, DecisionOutcome chosenOutcome, bool isPlayerInvolved)
		{
		}

		// Token: 0x0600045A RID: 1114 RVA: 0x0001BECF File Offset: 0x0001A0CF
		public virtual void OnHeroOrPartyTradedGold(ValueTuple<Hero, PartyBase> giver, ValueTuple<Hero, PartyBase> recipient, ValueTuple<int, string> goldAmount, bool showNotification)
		{
		}

		// Token: 0x0600045B RID: 1115 RVA: 0x0001BED1 File Offset: 0x0001A0D1
		public virtual void OnHeroOrPartyGaveItem(ValueTuple<Hero, PartyBase> giver, ValueTuple<Hero, PartyBase> receiver, ItemObject item, int count, bool showNotification)
		{
		}

		// Token: 0x0600045C RID: 1116 RVA: 0x0001BED3 File Offset: 0x0001A0D3
		public virtual void OnBanditPartyRecruited(MobileParty banditParty)
		{
		}

		// Token: 0x0600045D RID: 1117 RVA: 0x0001BED5 File Offset: 0x0001A0D5
		public virtual void OnArmyCreated(Army army)
		{
		}

		// Token: 0x0600045E RID: 1118 RVA: 0x0001BED7 File Offset: 0x0001A0D7
		public virtual void OnPartyAttachedAnotherParty(MobileParty mobileParty)
		{
		}

		// Token: 0x0600045F RID: 1119 RVA: 0x0001BED9 File Offset: 0x0001A0D9
		public virtual void OnNearbyPartyAddedToPlayerMapEvent(MobileParty mobileParty)
		{
		}

		// Token: 0x06000460 RID: 1120 RVA: 0x0001BEDB File Offset: 0x0001A0DB
		public virtual void OnArmyDispersed(Army army, Army.ArmyDispersionReason reason, bool isPlayersArmy)
		{
		}

		// Token: 0x06000461 RID: 1121 RVA: 0x0001BEDD File Offset: 0x0001A0DD
		public virtual void OnArmyGathered(Army army, Settlement gatheringSettlement)
		{
		}

		// Token: 0x06000462 RID: 1122 RVA: 0x0001BEDF File Offset: 0x0001A0DF
		public virtual void OnPerkOpened(Hero hero, PerkObject perk)
		{
		}

		// Token: 0x06000463 RID: 1123 RVA: 0x0001BEE1 File Offset: 0x0001A0E1
		public virtual void OnPlayerTraitChanged(TraitObject trait, int previousLevel)
		{
		}

		// Token: 0x06000464 RID: 1124 RVA: 0x0001BEE3 File Offset: 0x0001A0E3
		public virtual void OnVillageStateChanged(Village village, Village.VillageStates oldState, Village.VillageStates newState, MobileParty raiderParty)
		{
		}

		// Token: 0x06000465 RID: 1125 RVA: 0x0001BEE5 File Offset: 0x0001A0E5
		public virtual void OnSettlementEntered(MobileParty party, Settlement settlement, Hero hero)
		{
		}

		// Token: 0x06000466 RID: 1126 RVA: 0x0001BEE7 File Offset: 0x0001A0E7
		public virtual void OnAfterSettlementEntered(MobileParty party, Settlement settlement, Hero hero)
		{
		}

		// Token: 0x06000467 RID: 1127 RVA: 0x0001BEE9 File Offset: 0x0001A0E9
		public virtual void OnMercenaryTroopChangedInTown(Town town, CharacterObject oldTroopType, CharacterObject newTroopType)
		{
		}

		// Token: 0x06000468 RID: 1128 RVA: 0x0001BEEB File Offset: 0x0001A0EB
		public virtual void OnMercenaryNumberChangedInTown(Town town, int oldNumber, int newNumber)
		{
		}

		// Token: 0x06000469 RID: 1129 RVA: 0x0001BEED File Offset: 0x0001A0ED
		public virtual void OnAlleyOwnerChanged(Alley alley, Hero newOwner, Hero oldOwner)
		{
		}

		// Token: 0x0600046A RID: 1130 RVA: 0x0001BEEF File Offset: 0x0001A0EF
		public virtual void OnAlleyClearedByPlayer(Alley alley)
		{
		}

		// Token: 0x0600046B RID: 1131 RVA: 0x0001BEF1 File Offset: 0x0001A0F1
		public virtual void OnAlleyOccupiedByPlayer(Alley alley, TroopRoster troops)
		{
		}

		// Token: 0x0600046C RID: 1132 RVA: 0x0001BEF3 File Offset: 0x0001A0F3
		public virtual void OnRomanticStateChanged(Hero hero1, Hero hero2, Romance.RomanceLevelEnum romanceLevel)
		{
		}

		// Token: 0x0600046D RID: 1133 RVA: 0x0001BEF5 File Offset: 0x0001A0F5
		public virtual void OnHeroesMarried(Hero hero1, Hero hero2, bool showNotification = true)
		{
		}

		// Token: 0x0600046E RID: 1134 RVA: 0x0001BEF7 File Offset: 0x0001A0F7
		public virtual void OnPlayerEliminatedFromTournament(int round, Town town)
		{
		}

		// Token: 0x0600046F RID: 1135 RVA: 0x0001BEF9 File Offset: 0x0001A0F9
		public virtual void OnPlayerStartedTournamentMatch(Town town)
		{
		}

		// Token: 0x06000470 RID: 1136 RVA: 0x0001BEFB File Offset: 0x0001A0FB
		public virtual void OnTournamentStarted(Town town)
		{
		}

		// Token: 0x06000471 RID: 1137 RVA: 0x0001BEFD File Offset: 0x0001A0FD
		public virtual void OnTournamentFinished(CharacterObject winner, MBReadOnlyList<CharacterObject> participants, Town town, ItemObject prize)
		{
		}

		// Token: 0x06000472 RID: 1138 RVA: 0x0001BEFF File Offset: 0x0001A0FF
		public virtual void OnTournamentCancelled(Town town)
		{
		}

		// Token: 0x06000473 RID: 1139 RVA: 0x0001BF01 File Offset: 0x0001A101
		public virtual void OnWarDeclared(IFaction faction1, IFaction faction2, DeclareWarAction.DeclareWarDetail declareWarDetail)
		{
		}

		// Token: 0x06000474 RID: 1140 RVA: 0x0001BF03 File Offset: 0x0001A103
		public virtual void OnMakePeace(IFaction side1Faction, IFaction side2Faction, MakePeaceAction.MakePeaceDetail detail)
		{
		}

		// Token: 0x06000475 RID: 1141 RVA: 0x0001BF05 File Offset: 0x0001A105
		public virtual void OnKingdomCreated(Kingdom createdKingdom)
		{
		}

		// Token: 0x06000476 RID: 1142 RVA: 0x0001BF07 File Offset: 0x0001A107
		public virtual void OnHeroOccupationChanged(Hero hero, Occupation oldOccupation)
		{
		}

		// Token: 0x06000477 RID: 1143 RVA: 0x0001BF09 File Offset: 0x0001A109
		public virtual void OnKingdomDestroyed(Kingdom kingdom)
		{
		}

		// Token: 0x06000478 RID: 1144 RVA: 0x0001BF0B File Offset: 0x0001A10B
		public virtual void OnBarterAccepted(Hero offererHero, Hero otherHero, List<Barterable> barters)
		{
		}

		// Token: 0x06000479 RID: 1145 RVA: 0x0001BF0D File Offset: 0x0001A10D
		public virtual void OnBarterCanceled(Hero offererHero, Hero otherHero, List<Barterable> barters)
		{
		}

		// Token: 0x0600047A RID: 1146 RVA: 0x0001BF0F File Offset: 0x0001A10F
		public virtual void OnStartBattle(PartyBase attackerParty, PartyBase defenderParty, object subject, bool showNotification)
		{
		}

		// Token: 0x0600047B RID: 1147 RVA: 0x0001BF11 File Offset: 0x0001A111
		public virtual void OnRebellionFinished(Settlement settlement, Clan oldOwnerClan)
		{
		}

		// Token: 0x0600047C RID: 1148 RVA: 0x0001BF13 File Offset: 0x0001A113
		public virtual void TownRebelliousStateChanged(Town town, bool rebelliousState)
		{
		}

		// Token: 0x0600047D RID: 1149 RVA: 0x0001BF15 File Offset: 0x0001A115
		public virtual void OnRebelliousClanDisbandedAtSettlement(Settlement settlement, Clan clan)
		{
		}

		// Token: 0x0600047E RID: 1150 RVA: 0x0001BF17 File Offset: 0x0001A117
		public virtual void OnItemsLooted(MobileParty mobileParty, ItemRoster items)
		{
		}

		// Token: 0x0600047F RID: 1151 RVA: 0x0001BF19 File Offset: 0x0001A119
		public virtual void OnMobilePartyDestroyed(MobileParty mobileParty, PartyBase destroyerParty)
		{
		}

		// Token: 0x06000480 RID: 1152 RVA: 0x0001BF1B File Offset: 0x0001A11B
		public virtual void OnMobilePartyCreated(MobileParty party)
		{
		}

		// Token: 0x06000481 RID: 1153 RVA: 0x0001BF1D File Offset: 0x0001A11D
		public virtual void OnHeroKilled(Hero victim, Hero killer, KillCharacterAction.KillCharacterActionDetail detail, bool showNotification = true)
		{
		}

		// Token: 0x06000482 RID: 1154 RVA: 0x0001BF1F File Offset: 0x0001A11F
		public virtual void OnBeforeHeroKilled(Hero victim, Hero killer, KillCharacterAction.KillCharacterActionDetail detail, bool showNotification = true)
		{
		}

		// Token: 0x06000483 RID: 1155 RVA: 0x0001BF21 File Offset: 0x0001A121
		public virtual void OnChildEducationCompleted(Hero hero, int age)
		{
		}

		// Token: 0x06000484 RID: 1156 RVA: 0x0001BF23 File Offset: 0x0001A123
		public virtual void OnHeroComesOfAge(Hero hero)
		{
		}

		// Token: 0x06000485 RID: 1157 RVA: 0x0001BF25 File Offset: 0x0001A125
		public virtual void OnHeroReachesTeenAge(Hero hero)
		{
		}

		// Token: 0x06000486 RID: 1158 RVA: 0x0001BF27 File Offset: 0x0001A127
		public virtual void OnHeroGrowsOutOfInfancy(Hero hero)
		{
		}

		// Token: 0x06000487 RID: 1159 RVA: 0x0001BF29 File Offset: 0x0001A129
		public virtual void OnCharacterDefeated(Hero winner, Hero loser)
		{
		}

		// Token: 0x06000488 RID: 1160 RVA: 0x0001BF2B File Offset: 0x0001A12B
		public virtual void OnHeroPrisonerTaken(PartyBase capturer, Hero prisoner)
		{
		}

		// Token: 0x06000489 RID: 1161 RVA: 0x0001BF2D File Offset: 0x0001A12D
		public virtual void OnHeroPrisonerReleased(Hero prisoner, PartyBase party, IFaction capturerFaction, EndCaptivityDetail detail)
		{
		}

		// Token: 0x0600048A RID: 1162 RVA: 0x0001BF2F File Offset: 0x0001A12F
		public virtual void OnCharacterBecameFugitive(Hero hero)
		{
		}

		// Token: 0x0600048B RID: 1163 RVA: 0x0001BF31 File Offset: 0x0001A131
		public virtual void OnPlayerMetHero(Hero hero)
		{
		}

		// Token: 0x0600048C RID: 1164 RVA: 0x0001BF33 File Offset: 0x0001A133
		public virtual void OnPlayerLearnsAboutHero(Hero hero)
		{
		}

		// Token: 0x0600048D RID: 1165 RVA: 0x0001BF35 File Offset: 0x0001A135
		public virtual void OnRenownGained(Hero hero, int gainedRenown, bool doNotNotify)
		{
		}

		// Token: 0x0600048E RID: 1166 RVA: 0x0001BF37 File Offset: 0x0001A137
		public virtual void OnCrimeRatingChanged(IFaction kingdom, float deltaCrimeAmount)
		{
		}

		// Token: 0x0600048F RID: 1167 RVA: 0x0001BF39 File Offset: 0x0001A139
		public virtual void OnNewCompanionAdded(Hero newCompanion)
		{
		}

		// Token: 0x06000490 RID: 1168 RVA: 0x0001BF3B File Offset: 0x0001A13B
		public virtual void OnAfterMissionStarted(IMission iMission)
		{
		}

		// Token: 0x06000491 RID: 1169 RVA: 0x0001BF3D File Offset: 0x0001A13D
		public virtual void OnGameMenuOpened(MenuCallbackArgs args)
		{
		}

		// Token: 0x06000492 RID: 1170 RVA: 0x0001BF3F File Offset: 0x0001A13F
		public virtual void OnVillageBecomeNormal(Village village)
		{
		}

		// Token: 0x06000493 RID: 1171 RVA: 0x0001BF41 File Offset: 0x0001A141
		public virtual void OnVillageBeingRaided(Village village)
		{
		}

		// Token: 0x06000494 RID: 1172 RVA: 0x0001BF43 File Offset: 0x0001A143
		public virtual void OnVillageLooted(Village village)
		{
		}

		// Token: 0x06000495 RID: 1173 RVA: 0x0001BF45 File Offset: 0x0001A145
		public virtual void OnAgentJoinedConversation(IAgent agent)
		{
		}

		// Token: 0x06000496 RID: 1174 RVA: 0x0001BF47 File Offset: 0x0001A147
		public virtual void OnConversationEnded(IEnumerable<CharacterObject> characters)
		{
		}

		// Token: 0x06000497 RID: 1175 RVA: 0x0001BF49 File Offset: 0x0001A149
		public virtual void OnMapEventEnded(MapEvent mapEvent)
		{
		}

		// Token: 0x06000498 RID: 1176 RVA: 0x0001BF4B File Offset: 0x0001A14B
		public virtual void OnMapEventStarted(MapEvent mapEvent, PartyBase attackerParty, PartyBase defenderParty)
		{
		}

		// Token: 0x06000499 RID: 1177 RVA: 0x0001BF4D File Offset: 0x0001A14D
		public virtual void OnRansomOfferedToPlayer(Hero captiveHero)
		{
		}

		// Token: 0x0600049A RID: 1178 RVA: 0x0001BF4F File Offset: 0x0001A14F
		public virtual void OnPrisonersChangeInSettlement(Settlement settlement, FlattenedTroopRoster prisonerRoster, Hero prisonerHero, bool takenFromDungeon)
		{
		}

		// Token: 0x0600049B RID: 1179 RVA: 0x0001BF51 File Offset: 0x0001A151
		public virtual void OnMissionStarted(IMission mission)
		{
		}

		// Token: 0x0600049C RID: 1180 RVA: 0x0001BF53 File Offset: 0x0001A153
		public virtual void OnRansomOfferCancelled(Hero captiveHero)
		{
		}

		// Token: 0x0600049D RID: 1181 RVA: 0x0001BF55 File Offset: 0x0001A155
		public virtual void OnPeaceOfferedToPlayer(IFaction opponentFaction, int tributeAmount)
		{
		}

		// Token: 0x0600049E RID: 1182 RVA: 0x0001BF57 File Offset: 0x0001A157
		public virtual void OnPeaceOfferCancelled(IFaction opponentFaction)
		{
		}

		// Token: 0x0600049F RID: 1183 RVA: 0x0001BF59 File Offset: 0x0001A159
		public virtual void OnMarriageOfferedToPlayer(Hero suitor, Hero maiden)
		{
		}

		// Token: 0x060004A0 RID: 1184 RVA: 0x0001BF5B File Offset: 0x0001A15B
		public virtual void OnMarriageOfferCanceled(Hero suitor, Hero maiden)
		{
		}

		// Token: 0x060004A1 RID: 1185 RVA: 0x0001BF5D File Offset: 0x0001A15D
		public virtual void OnVassalOrMercenaryServiceOfferedToPlayer(Kingdom offeredKingdom)
		{
		}

		// Token: 0x060004A2 RID: 1186 RVA: 0x0001BF5F File Offset: 0x0001A15F
		public virtual void OnVassalOrMercenaryServiceOfferCanceled(Kingdom offeredKingdom)
		{
		}

		// Token: 0x060004A3 RID: 1187 RVA: 0x0001BF61 File Offset: 0x0001A161
		public virtual void OnPlayerBoardGameOver(Hero opposingHero, BoardGameHelper.BoardGameState state)
		{
		}

		// Token: 0x060004A4 RID: 1188 RVA: 0x0001BF63 File Offset: 0x0001A163
		public virtual void OnCommonAreaStateChanged(Alley alley, Alley.AreaState oldState, Alley.AreaState newState)
		{
		}

		// Token: 0x060004A5 RID: 1189 RVA: 0x0001BF65 File Offset: 0x0001A165
		public virtual void BeforeMissionOpened()
		{
		}

		// Token: 0x060004A6 RID: 1190 RVA: 0x0001BF67 File Offset: 0x0001A167
		public virtual void OnPartyRemoved(PartyBase party)
		{
		}

		// Token: 0x060004A7 RID: 1191 RVA: 0x0001BF69 File Offset: 0x0001A169
		public virtual void OnPartySizeChanged(PartyBase party)
		{
		}

		// Token: 0x060004A8 RID: 1192 RVA: 0x0001BF6B File Offset: 0x0001A16B
		public virtual void OnSettlementOwnerChanged(Settlement settlement, bool openToClaim, Hero newOwner, Hero oldOwner, Hero capturerHero, ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail detail)
		{
		}

		// Token: 0x060004A9 RID: 1193 RVA: 0x0001BF6D File Offset: 0x0001A16D
		public virtual void OnGovernorChanged(Town fortification, Hero oldGovernor, Hero newGovernor)
		{
		}

		// Token: 0x060004AA RID: 1194 RVA: 0x0001BF6F File Offset: 0x0001A16F
		public virtual void OnSettlementLeft(MobileParty party, Settlement settlement)
		{
		}

		// Token: 0x060004AB RID: 1195 RVA: 0x0001BF71 File Offset: 0x0001A171
		public virtual void Tick(float dt)
		{
		}

		// Token: 0x060004AC RID: 1196 RVA: 0x0001BF73 File Offset: 0x0001A173
		public virtual void OnSessionStart(CampaignGameStarter campaignGameStarter)
		{
		}

		// Token: 0x060004AD RID: 1197 RVA: 0x0001BF75 File Offset: 0x0001A175
		public virtual void OnAfterSessionStart(CampaignGameStarter campaignGameStarter)
		{
		}

		// Token: 0x060004AE RID: 1198 RVA: 0x0001BF77 File Offset: 0x0001A177
		public virtual void OnNewGameCreated(CampaignGameStarter campaignGameStarter)
		{
		}

		// Token: 0x060004AF RID: 1199 RVA: 0x0001BF79 File Offset: 0x0001A179
		public virtual void OnGameLoaded(CampaignGameStarter campaignGameStarter)
		{
		}

		// Token: 0x060004B0 RID: 1200 RVA: 0x0001BF7B File Offset: 0x0001A17B
		public virtual void OnGameEarlyLoaded(CampaignGameStarter campaignGameStarter)
		{
		}

		// Token: 0x060004B1 RID: 1201 RVA: 0x0001BF7D File Offset: 0x0001A17D
		public virtual void OnPlayerTradeProfit(int profit)
		{
		}

		// Token: 0x060004B2 RID: 1202 RVA: 0x0001BF7F File Offset: 0x0001A17F
		public virtual void OnRulingClanChanged(Kingdom kingdom, Clan newRulingClan)
		{
		}

		// Token: 0x060004B3 RID: 1203 RVA: 0x0001BF81 File Offset: 0x0001A181
		public virtual void OnPrisonerReleased(FlattenedTroopRoster roster)
		{
		}

		// Token: 0x060004B4 RID: 1204 RVA: 0x0001BF83 File Offset: 0x0001A183
		public virtual void OnGameLoadFinished()
		{
		}

		// Token: 0x060004B5 RID: 1205 RVA: 0x0001BF85 File Offset: 0x0001A185
		public virtual void OnPartyJoinedArmy(MobileParty mobileParty)
		{
		}

		// Token: 0x060004B6 RID: 1206 RVA: 0x0001BF87 File Offset: 0x0001A187
		public virtual void OnPartyRemovedFromArmy(MobileParty mobileParty)
		{
		}

		// Token: 0x060004B7 RID: 1207 RVA: 0x0001BF89 File Offset: 0x0001A189
		public virtual void OnArmyLeaderThink(Hero hero, Army.ArmyLeaderThinkReason reason)
		{
		}

		// Token: 0x060004B8 RID: 1208 RVA: 0x0001BF8B File Offset: 0x0001A18B
		public virtual void OnArmyOverlaySetDirty()
		{
		}

		// Token: 0x060004B9 RID: 1209 RVA: 0x0001BF8D File Offset: 0x0001A18D
		public virtual void OnPlayerDesertedBattle(int sacrificedMenCount)
		{
		}

		// Token: 0x060004BA RID: 1210 RVA: 0x0001BF8F File Offset: 0x0001A18F
		public virtual void MissionTick(float dt)
		{
		}

		// Token: 0x060004BB RID: 1211 RVA: 0x0001BF91 File Offset: 0x0001A191
		public virtual void OnChildConceived(Hero mother)
		{
		}

		// Token: 0x060004BC RID: 1212 RVA: 0x0001BF93 File Offset: 0x0001A193
		public virtual void OnGivenBirth(Hero mother, List<Hero> aliveChildren, int stillbornCount)
		{
		}

		// Token: 0x060004BD RID: 1213 RVA: 0x0001BF95 File Offset: 0x0001A195
		public virtual void OnUnitRecruited(CharacterObject character, int amount)
		{
		}

		// Token: 0x060004BE RID: 1214 RVA: 0x0001BF97 File Offset: 0x0001A197
		public virtual void OnPlayerBattleEnd(MapEvent mapEvent)
		{
		}

		// Token: 0x060004BF RID: 1215 RVA: 0x0001BF99 File Offset: 0x0001A199
		public virtual void OnMissionEnded(IMission mission)
		{
		}

		// Token: 0x060004C0 RID: 1216 RVA: 0x0001BF9B File Offset: 0x0001A19B
		public virtual void TickPartialHourlyAi(MobileParty party)
		{
		}

		// Token: 0x060004C1 RID: 1217 RVA: 0x0001BF9D File Offset: 0x0001A19D
		public virtual void QuarterDailyPartyTick(MobileParty party)
		{
		}

		// Token: 0x060004C2 RID: 1218 RVA: 0x0001BF9F File Offset: 0x0001A19F
		public virtual void AiHourlyTick(MobileParty party, PartyThinkParams partyThinkParams)
		{
		}

		// Token: 0x060004C3 RID: 1219 RVA: 0x0001BFA1 File Offset: 0x0001A1A1
		public virtual void HourlyTick()
		{
		}

		// Token: 0x060004C4 RID: 1220 RVA: 0x0001BFA3 File Offset: 0x0001A1A3
		public virtual void HourlyTickParty(MobileParty mobileParty)
		{
		}

		// Token: 0x060004C5 RID: 1221 RVA: 0x0001BFA5 File Offset: 0x0001A1A5
		public virtual void HourlyTickSettlement(Settlement settlement)
		{
		}

		// Token: 0x060004C6 RID: 1222 RVA: 0x0001BFA7 File Offset: 0x0001A1A7
		public virtual void HourlyTickClan(Clan clan)
		{
		}

		// Token: 0x060004C7 RID: 1223 RVA: 0x0001BFA9 File Offset: 0x0001A1A9
		public virtual void DailyTick()
		{
		}

		// Token: 0x060004C8 RID: 1224 RVA: 0x0001BFAB File Offset: 0x0001A1AB
		public virtual void DailyTickParty(MobileParty mobileParty)
		{
		}

		// Token: 0x060004C9 RID: 1225 RVA: 0x0001BFAD File Offset: 0x0001A1AD
		public virtual void DailyTickTown(Town town)
		{
		}

		// Token: 0x060004CA RID: 1226 RVA: 0x0001BFAF File Offset: 0x0001A1AF
		public virtual void DailyTickSettlement(Settlement settlement)
		{
		}

		// Token: 0x060004CB RID: 1227 RVA: 0x0001BFB1 File Offset: 0x0001A1B1
		public virtual void DailyTickClan(Clan clan)
		{
		}

		// Token: 0x060004CC RID: 1228 RVA: 0x0001BFB3 File Offset: 0x0001A1B3
		public virtual void OnPlayerBodyPropertiesChanged()
		{
		}

		// Token: 0x060004CD RID: 1229 RVA: 0x0001BFB5 File Offset: 0x0001A1B5
		public virtual void WeeklyTick()
		{
		}

		// Token: 0x060004CE RID: 1230 RVA: 0x0001BFB7 File Offset: 0x0001A1B7
		public virtual void CollectAvailableTutorials(ref List<CampaignTutorial> tutorials)
		{
		}

		// Token: 0x060004CF RID: 1231 RVA: 0x0001BFB9 File Offset: 0x0001A1B9
		public virtual void DailyTickHero(Hero hero)
		{
		}

		// Token: 0x060004D0 RID: 1232 RVA: 0x0001BFBB File Offset: 0x0001A1BB
		public virtual void OnTutorialCompleted(string tutorial)
		{
		}

		// Token: 0x060004D1 RID: 1233 RVA: 0x0001BFBD File Offset: 0x0001A1BD
		public virtual void OnBuildingLevelChanged(Town town, Building building, int levelChange)
		{
		}

		// Token: 0x060004D2 RID: 1234 RVA: 0x0001BFBF File Offset: 0x0001A1BF
		public virtual void BeforeGameMenuOpened(MenuCallbackArgs args)
		{
		}

		// Token: 0x060004D3 RID: 1235 RVA: 0x0001BFC1 File Offset: 0x0001A1C1
		public virtual void AfterGameMenuOpened(MenuCallbackArgs args)
		{
		}

		// Token: 0x060004D4 RID: 1236 RVA: 0x0001BFC3 File Offset: 0x0001A1C3
		public virtual void OnBarterablesRequested(BarterData args)
		{
		}

		// Token: 0x060004D5 RID: 1237 RVA: 0x0001BFC5 File Offset: 0x0001A1C5
		public virtual void OnPartyVisibilityChanged(PartyBase party)
		{
		}

		// Token: 0x060004D6 RID: 1238 RVA: 0x0001BFC7 File Offset: 0x0001A1C7
		public virtual void OnCompanionRemoved(Hero companion, RemoveCompanionAction.RemoveCompanionDetail detail)
		{
		}

		// Token: 0x060004D7 RID: 1239 RVA: 0x0001BFC9 File Offset: 0x0001A1C9
		public virtual void TrackDetected(Track track)
		{
		}

		// Token: 0x060004D8 RID: 1240 RVA: 0x0001BFCB File Offset: 0x0001A1CB
		public virtual void TrackLost(Track track)
		{
		}

		// Token: 0x060004D9 RID: 1241 RVA: 0x0001BFCD File Offset: 0x0001A1CD
		public virtual void LocationCharactersAreReadyToSpawn(Dictionary<string, int> unusedUsablePointCount)
		{
		}

		// Token: 0x060004DA RID: 1242 RVA: 0x0001BFCF File Offset: 0x0001A1CF
		public virtual void LocationCharactersSimulated()
		{
		}

		// Token: 0x060004DB RID: 1243 RVA: 0x0001BFD1 File Offset: 0x0001A1D1
		public virtual void OnPlayerUpgradedTroops(CharacterObject upgradeFromTroop, CharacterObject upgradeToTroop, int number)
		{
		}

		// Token: 0x060004DC RID: 1244 RVA: 0x0001BFD3 File Offset: 0x0001A1D3
		public virtual void OnHeroCombatHit(CharacterObject attackerTroop, CharacterObject attackedTroop, PartyBase party, WeaponComponentData usedWeapon, bool isFatal, int xp)
		{
		}

		// Token: 0x060004DD RID: 1245 RVA: 0x0001BFD5 File Offset: 0x0001A1D5
		public virtual void OnCharacterPortraitPopUpOpened(CharacterObject character)
		{
		}

		// Token: 0x060004DE RID: 1246 RVA: 0x0001BFD7 File Offset: 0x0001A1D7
		public virtual void OnCharacterPortraitPopUpClosed()
		{
		}

		// Token: 0x060004DF RID: 1247 RVA: 0x0001BFD9 File Offset: 0x0001A1D9
		public virtual void OnPlayerStartTalkFromMenu(Hero hero)
		{
		}

		// Token: 0x060004E0 RID: 1248 RVA: 0x0001BFDB File Offset: 0x0001A1DB
		public virtual void OnGameMenuOptionSelected(GameMenuOption gameMenuOption)
		{
		}

		// Token: 0x060004E1 RID: 1249 RVA: 0x0001BFDD File Offset: 0x0001A1DD
		public virtual void OnPlayerStartRecruitment(CharacterObject recruitTroopCharacter)
		{
		}

		// Token: 0x060004E2 RID: 1250 RVA: 0x0001BFDF File Offset: 0x0001A1DF
		public virtual void OnBeforePlayerCharacterChanged(Hero oldPlayer, Hero newPlayer)
		{
		}

		// Token: 0x060004E3 RID: 1251 RVA: 0x0001BFE1 File Offset: 0x0001A1E1
		public virtual void OnPlayerCharacterChanged(Hero oldPlayer, Hero newPlayer, MobileParty newMainParty, bool isMainPartyChanged)
		{
		}

		// Token: 0x060004E4 RID: 1252 RVA: 0x0001BFE3 File Offset: 0x0001A1E3
		public virtual void OnClanLeaderChanged(Hero oldLeader, Hero newLeader)
		{
		}

		// Token: 0x060004E5 RID: 1253 RVA: 0x0001BFE5 File Offset: 0x0001A1E5
		public virtual void OnSiegeEventStarted(SiegeEvent siegeEvent)
		{
		}

		// Token: 0x060004E6 RID: 1254 RVA: 0x0001BFE7 File Offset: 0x0001A1E7
		public virtual void OnPlayerSiegeStarted()
		{
		}

		// Token: 0x060004E7 RID: 1255 RVA: 0x0001BFE9 File Offset: 0x0001A1E9
		public virtual void OnSiegeEventEnded(SiegeEvent siegeEvent)
		{
		}

		// Token: 0x060004E8 RID: 1256 RVA: 0x0001BFEB File Offset: 0x0001A1EB
		public virtual void OnSiegeAftermathApplied(MobileParty attackerParty, Settlement settlement, SiegeAftermathAction.SiegeAftermath aftermathType, Clan previousSettlementOwner, Dictionary<MobileParty, float> partyContributions)
		{
		}

		// Token: 0x060004E9 RID: 1257 RVA: 0x0001BFED File Offset: 0x0001A1ED
		public virtual void OnSiegeBombardmentHit(MobileParty besiegerParty, Settlement besiegedSettlement, BattleSideEnum side, SiegeEngineType weapon, SiegeBombardTargets target)
		{
		}

		// Token: 0x060004EA RID: 1258 RVA: 0x0001BFEF File Offset: 0x0001A1EF
		public virtual void OnSiegeBombardmentWallHit(MobileParty besiegerParty, Settlement besiegedSettlement, BattleSideEnum side, SiegeEngineType weapon, bool isWallCracked)
		{
		}

		// Token: 0x060004EB RID: 1259 RVA: 0x0001BFF1 File Offset: 0x0001A1F1
		public virtual void OnSiegeEngineDestroyed(MobileParty besiegerParty, Settlement besiegedSettlement, BattleSideEnum side, SiegeEngineType destroyedEngine)
		{
		}

		// Token: 0x060004EC RID: 1260 RVA: 0x0001BFF3 File Offset: 0x0001A1F3
		public virtual void OnTradeRumorIsTaken(List<TradeRumor> newRumors, Settlement sourceSettlement = null)
		{
		}

		// Token: 0x060004ED RID: 1261 RVA: 0x0001BFF5 File Offset: 0x0001A1F5
		public virtual void OnCheckForIssue(Hero hero)
		{
		}

		// Token: 0x060004EE RID: 1262 RVA: 0x0001BFF7 File Offset: 0x0001A1F7
		public virtual void OnIssueUpdated(IssueBase issue, IssueBase.IssueUpdateDetails details, Hero issueSolver)
		{
		}

		// Token: 0x060004EF RID: 1263 RVA: 0x0001BFF9 File Offset: 0x0001A1F9
		public virtual void OnTroopsDeserted(MobileParty mobileParty, TroopRoster desertedTroops)
		{
		}

		// Token: 0x060004F0 RID: 1264 RVA: 0x0001BFFB File Offset: 0x0001A1FB
		public virtual void OnTroopRecruited(Hero recruiterHero, Settlement recruitmentSettlement, Hero recruitmentSource, CharacterObject troop, int amount)
		{
		}

		// Token: 0x060004F1 RID: 1265 RVA: 0x0001BFFD File Offset: 0x0001A1FD
		public virtual void OnTroopGivenToSettlement(Hero giverHero, Settlement recipientSettlement, TroopRoster roster)
		{
		}

		// Token: 0x060004F2 RID: 1266 RVA: 0x0001BFFF File Offset: 0x0001A1FF
		public virtual void OnItemSold(PartyBase receiverParty, PartyBase payerParty, ItemRosterElement itemRosterElement, int number, Settlement currentSettlement)
		{
		}

		// Token: 0x060004F3 RID: 1267 RVA: 0x0001C001 File Offset: 0x0001A201
		public virtual void OnCaravanTransactionCompleted(MobileParty caravanParty, Town town, List<ValueTuple<EquipmentElement, int>> itemRosterElements)
		{
		}

		// Token: 0x060004F4 RID: 1268 RVA: 0x0001C003 File Offset: 0x0001A203
		public virtual void OnPrisonerSold(MobileParty party, TroopRoster prisoners, Settlement currentSettlement)
		{
		}

		// Token: 0x060004F5 RID: 1269 RVA: 0x0001C005 File Offset: 0x0001A205
		public virtual void OnPartyDisbanded(MobileParty disbandParty, Settlement relatedSettlement)
		{
		}

		// Token: 0x060004F6 RID: 1270 RVA: 0x0001C007 File Offset: 0x0001A207
		public virtual void OnPartyDisbandStarted(MobileParty disbandParty)
		{
		}

		// Token: 0x060004F7 RID: 1271 RVA: 0x0001C009 File Offset: 0x0001A209
		public virtual void OnPartyDisbandCanceled(MobileParty disbandParty)
		{
		}

		// Token: 0x060004F8 RID: 1272 RVA: 0x0001C00B File Offset: 0x0001A20B
		public virtual void OnHideoutSpotted(PartyBase party, PartyBase hideoutParty)
		{
		}

		// Token: 0x060004F9 RID: 1273 RVA: 0x0001C00D File Offset: 0x0001A20D
		public virtual void OnHideoutDeactivated(Settlement hideout)
		{
		}

		// Token: 0x060004FA RID: 1274 RVA: 0x0001C00F File Offset: 0x0001A20F
		public virtual void OnPlayerInventoryExchange(List<ValueTuple<ItemRosterElement, int>> purchasedItems, List<ValueTuple<ItemRosterElement, int>> soldItems, bool isTrading)
		{
		}

		// Token: 0x060004FB RID: 1275 RVA: 0x0001C011 File Offset: 0x0001A211
		public virtual void OnItemsDiscardedByPlayer(ItemRoster roster)
		{
		}

		// Token: 0x060004FC RID: 1276 RVA: 0x0001C013 File Offset: 0x0001A213
		public virtual void OnPersuasionProgressCommitted(Tuple<PersuasionOptionArgs, PersuasionOptionResult> progress)
		{
		}

		// Token: 0x060004FD RID: 1277 RVA: 0x0001C015 File Offset: 0x0001A215
		public virtual void OnHeroSharedFoodWithAnother(Hero supporterHero, Hero supportedHero, float influence)
		{
		}

		// Token: 0x060004FE RID: 1278 RVA: 0x0001C017 File Offset: 0x0001A217
		public virtual void OnQuestCompleted(QuestBase quest, QuestBase.QuestCompleteDetails detail)
		{
		}

		// Token: 0x060004FF RID: 1279 RVA: 0x0001C019 File Offset: 0x0001A219
		public virtual void OnQuestStarted(QuestBase quest)
		{
		}

		// Token: 0x06000500 RID: 1280 RVA: 0x0001C01B File Offset: 0x0001A21B
		public virtual void OnItemProduced(ItemObject itemObject, Settlement settlement, int count)
		{
		}

		// Token: 0x06000501 RID: 1281 RVA: 0x0001C01D File Offset: 0x0001A21D
		public virtual void OnItemConsumed(ItemObject itemObject, Settlement settlement, int count)
		{
		}

		// Token: 0x06000502 RID: 1282 RVA: 0x0001C01F File Offset: 0x0001A21F
		public virtual void OnPartyConsumedFood(MobileParty party)
		{
		}

		// Token: 0x06000503 RID: 1283 RVA: 0x0001C021 File Offset: 0x0001A221
		public virtual void SiegeCompleted(Settlement siegeSettlement, MobileParty attackerParty, bool isWin, MapEvent.BattleTypes battleType)
		{
		}

		// Token: 0x06000504 RID: 1284 RVA: 0x0001C023 File Offset: 0x0001A223
		public virtual void SiegeEngineBuilt(SiegeEvent siegeEvent, BattleSideEnum side, SiegeEngineType siegeEngine)
		{
		}

		// Token: 0x06000505 RID: 1285 RVA: 0x0001C025 File Offset: 0x0001A225
		public virtual void RaidCompleted(BattleSideEnum winnerSide, RaidEventComponent raidEvent)
		{
		}

		// Token: 0x06000506 RID: 1286 RVA: 0x0001C027 File Offset: 0x0001A227
		public virtual void ForceSuppliesCompleted(BattleSideEnum winnerSide, ForceSuppliesEventComponent forceSuppliesEvent)
		{
		}

		// Token: 0x06000507 RID: 1287 RVA: 0x0001C029 File Offset: 0x0001A229
		public virtual void ForceVolunteersCompleted(BattleSideEnum winnerSide, ForceVolunteersEventComponent forceVolunteersEvent)
		{
		}

		// Token: 0x06000508 RID: 1288 RVA: 0x0001C02B File Offset: 0x0001A22B
		public virtual void OnBeforeMainCharacterDied(Hero victim, Hero killer, KillCharacterAction.KillCharacterActionDetail detail, bool showNotification = true)
		{
		}

		// Token: 0x06000509 RID: 1289 RVA: 0x0001C02D File Offset: 0x0001A22D
		public virtual void OnGameOver()
		{
		}

		// Token: 0x0600050A RID: 1290 RVA: 0x0001C02F File Offset: 0x0001A22F
		public virtual void OnClanDestroyed(Clan destroyedClan)
		{
		}

		// Token: 0x0600050B RID: 1291 RVA: 0x0001C031 File Offset: 0x0001A231
		public virtual void OnHideoutBattleCompleted(BattleSideEnum winnerSide, MapEvent mapEvent)
		{
		}

		// Token: 0x0600050C RID: 1292 RVA: 0x0001C033 File Offset: 0x0001A233
		public virtual void OnNewIssueCreated(IssueBase issue)
		{
		}

		// Token: 0x0600050D RID: 1293 RVA: 0x0001C035 File Offset: 0x0001A235
		public virtual void OnIssueOwnerChanged(IssueBase issue, Hero oldOwner)
		{
		}

		// Token: 0x0600050E RID: 1294 RVA: 0x0001C037 File Offset: 0x0001A237
		public virtual void OnNewItemCrafted(ItemObject itemObject, Crafting.OverrideData overrideData)
		{
		}

		// Token: 0x0600050F RID: 1295 RVA: 0x0001C039 File Offset: 0x0001A239
		public virtual void OnWorkshopChanged(Workshop workshop, Hero oldOwner, WorkshopType oldType)
		{
		}

		// Token: 0x06000510 RID: 1296 RVA: 0x0001C03B File Offset: 0x0001A23B
		public virtual void OnEquipmentSmeltedByHero(Hero hero, EquipmentElement equipmentElement)
		{
		}

		// Token: 0x06000511 RID: 1297 RVA: 0x0001C03D File Offset: 0x0001A23D
		public virtual void CraftingPartUnlocked(CraftingPiece craftingPiece)
		{
		}

		// Token: 0x06000512 RID: 1298 RVA: 0x0001C03F File Offset: 0x0001A23F
		public virtual void OnPrisonerTaken(FlattenedTroopRoster roster)
		{
		}

		// Token: 0x06000513 RID: 1299 RVA: 0x0001C041 File Offset: 0x0001A241
		public virtual void OnNewItemCrafted(ItemObject itemObject, Crafting.OverrideData overrideData, bool isCraftingOrderItem)
		{
		}

		// Token: 0x06000514 RID: 1300 RVA: 0x0001C043 File Offset: 0x0001A243
		public virtual void OnBeforeSave()
		{
		}

		// Token: 0x06000515 RID: 1301 RVA: 0x0001C045 File Offset: 0x0001A245
		public virtual void OnMainPartyPrisonerRecruited(FlattenedTroopRoster roster)
		{
		}

		// Token: 0x06000516 RID: 1302 RVA: 0x0001C047 File Offset: 0x0001A247
		public virtual void OnPrisonerDonatedToSettlement(MobileParty donatingParty, FlattenedTroopRoster donatedPrisoners, Settlement donatedSettlement)
		{
		}

		// Token: 0x06000517 RID: 1303 RVA: 0x0001C049 File Offset: 0x0001A249
		public virtual void CanMoveToSettlement(Hero hero, ref bool result)
		{
		}

		// Token: 0x06000518 RID: 1304 RVA: 0x0001C04B File Offset: 0x0001A24B
		public virtual void OnHeroChangedClan(Hero hero, Clan oldClan)
		{
		}

		// Token: 0x06000519 RID: 1305 RVA: 0x0001C04D File Offset: 0x0001A24D
		public virtual void CanHeroDie(Hero hero, KillCharacterAction.KillCharacterActionDetail causeOfDeath, ref bool result)
		{
		}

		// Token: 0x0600051A RID: 1306 RVA: 0x0001C04F File Offset: 0x0001A24F
		public virtual void CanHeroBecomePrisoner(Hero hero, ref bool result)
		{
		}

		// Token: 0x0600051B RID: 1307 RVA: 0x0001C051 File Offset: 0x0001A251
		public virtual void CanBeGovernorOrHavePartyRole(Hero hero, ref bool result)
		{
		}

		// Token: 0x0600051C RID: 1308 RVA: 0x0001C053 File Offset: 0x0001A253
		public virtual void OnSaveOver(bool isSuccessful, string saveName)
		{
		}

		// Token: 0x0600051D RID: 1309 RVA: 0x0001C055 File Offset: 0x0001A255
		public virtual void OnSaveStarted()
		{
		}

		// Token: 0x0600051E RID: 1310 RVA: 0x0001C057 File Offset: 0x0001A257
		public virtual void CanHeroMarry(Hero hero, ref bool result)
		{
		}

		// Token: 0x0600051F RID: 1311 RVA: 0x0001C059 File Offset: 0x0001A259
		public virtual void OnHeroTeleportationRequested(Hero hero, Settlement targetSettlement, MobileParty targetParty, TeleportHeroAction.TeleportationDetail detail)
		{
		}

		// Token: 0x06000520 RID: 1312 RVA: 0x0001C05B File Offset: 0x0001A25B
		public virtual void OnPartyLeaderChangeOfferCanceled(MobileParty party)
		{
		}

		// Token: 0x06000521 RID: 1313 RVA: 0x0001C05D File Offset: 0x0001A25D
		public virtual void OnClanInfluenceChanged(Clan clan, float change)
		{
		}

		// Token: 0x06000522 RID: 1314 RVA: 0x0001C05F File Offset: 0x0001A25F
		public virtual void OnPlayerPartyKnockedOrKilledTroop(CharacterObject strikedTroop)
		{
		}

		// Token: 0x06000523 RID: 1315 RVA: 0x0001C061 File Offset: 0x0001A261
		public virtual void OnPlayerEarnedGoldFromAsset(DefaultClanFinanceModel.AssetIncomeType incomeType, int incomeAmount)
		{
		}

		// Token: 0x06000524 RID: 1316 RVA: 0x0001C063 File Offset: 0x0001A263
		public virtual void CollectLoots(MapEvent mapEvent, PartyBase party, Dictionary<PartyBase, ItemRoster> loot, ItemRoster gainedLoot, MBList<TroopRosterElement> lootedCasualties, float lootAmount)
		{
		}

		// Token: 0x06000525 RID: 1317 RVA: 0x0001C065 File Offset: 0x0001A265
		public virtual void OnLootDistributedToParty(MapEvent mapEvent, PartyBase party, Dictionary<PartyBase, ItemRoster> loot)
		{
		}

		// Token: 0x06000526 RID: 1318 RVA: 0x0001C067 File Offset: 0x0001A267
		public virtual void OnPlayerJoinedTournament(Town town, bool isParticipant)
		{
		}

		// Token: 0x06000527 RID: 1319 RVA: 0x0001C069 File Offset: 0x0001A269
		public virtual void CanHeroLeadParty(Hero hero, ref bool result)
		{
		}

		// Token: 0x06000528 RID: 1320 RVA: 0x0001C06B File Offset: 0x0001A26B
		public virtual void OnMainPartyStarving()
		{
		}

		// Token: 0x06000529 RID: 1321 RVA: 0x0001C06D File Offset: 0x0001A26D
		public virtual void OnHeroGetsBusy(Hero hero, HeroGetsBusyReasons heroGetsBusyReason)
		{
		}

		// Token: 0x0600052A RID: 1322 RVA: 0x0001C06F File Offset: 0x0001A26F
		public virtual void CanHeroEquipmentBeChanged(Hero hero, ref bool result)
		{
		}

		// Token: 0x0600052B RID: 1323 RVA: 0x0001C071 File Offset: 0x0001A271
		public virtual void CanHaveQuestsOrIssues(Hero hero, ref bool result)
		{
		}

		// Token: 0x0600052C RID: 1324 RVA: 0x0001C073 File Offset: 0x0001A273
		public virtual void OnHeroUnregistered(Hero hero)
		{
		}
	}
}
