﻿using System;
using System.Collections;
using System.Collections.Generic;
using StoryMode.Quests.FirstPhase;
using StoryMode.Quests.PlayerClanQuests;
using StoryMode.Quests.QuestTasks;
using StoryMode.Quests.SecondPhase;
using StoryMode.Quests.SecondPhase.ConspiracyQuests;
using StoryMode.Quests.ThirdPhase;
using StoryMode.Quests.TutorialPhase;
using StoryMode.StoryModePhases;
using TaleWorlds.Library;
using TaleWorlds.SaveSystem.Definition;

namespace StoryMode
{
	// Token: 0x0200000C RID: 12
	internal class AutoGeneratedSaveManager : IAutoGeneratedSaveManager
	{
		// Token: 0x0600004E RID: 78 RVA: 0x000028E4 File Offset: 0x00000AE4
		public void Initialize(DefinitionContext definitionContext)
		{
			TypeDefinition typeDefinition = (TypeDefinition)definitionContext.TryGetTypeDefinition(new TypeSaveId(320001));
			CollectObjectsDelegate collectObjectsDelegate = new CollectObjectsDelegate(CampaignStoryMode.AutoGeneratedStaticCollectObjectsCampaignStoryMode);
			typeDefinition.InitializeForAutoGeneration(collectObjectsDelegate);
			typeDefinition.GetPropertyDefinitionWithId(new MemberTypeId(4, 9999)).InitializeForAutoGeneration(new GetPropertyValueDelegate(CampaignStoryMode.AutoGeneratedGetMemberValueStoryMode));
			TypeDefinition typeDefinition2 = (TypeDefinition)definitionContext.TryGetTypeDefinition(new TypeSaveId(320002));
			CollectObjectsDelegate collectObjectsDelegate2 = new CollectObjectsDelegate(StoryModeManager.AutoGeneratedStaticCollectObjectsStoryModeManager);
			typeDefinition2.InitializeForAutoGeneration(collectObjectsDelegate2);
			typeDefinition2.GetPropertyDefinitionWithId(new MemberTypeId(2, 1)).InitializeForAutoGeneration(new GetPropertyValueDelegate(StoryModeManager.AutoGeneratedGetMemberValueMainStoryLine));
			TypeDefinition typeDefinition3 = (TypeDefinition)definitionContext.TryGetTypeDefinition(new TypeSaveId(320003));
			CollectObjectsDelegate collectObjectsDelegate3 = new CollectObjectsDelegate(MainStoryLine.AutoGeneratedStaticCollectObjectsMainStoryLine);
			typeDefinition3.InitializeForAutoGeneration(collectObjectsDelegate3);
			typeDefinition3.GetPropertyDefinitionWithId(new MemberTypeId(2, 2)).InitializeForAutoGeneration(new GetPropertyValueDelegate(MainStoryLine.AutoGeneratedGetMemberValueTutorialPhase));
			typeDefinition3.GetPropertyDefinitionWithId(new MemberTypeId(2, 3)).InitializeForAutoGeneration(new GetPropertyValueDelegate(MainStoryLine.AutoGeneratedGetMemberValueFirstPhase));
			typeDefinition3.GetPropertyDefinitionWithId(new MemberTypeId(2, 4)).InitializeForAutoGeneration(new GetPropertyValueDelegate(MainStoryLine.AutoGeneratedGetMemberValueSecondPhase));
			typeDefinition3.GetPropertyDefinitionWithId(new MemberTypeId(2, 5)).InitializeForAutoGeneration(new GetPropertyValueDelegate(MainStoryLine.AutoGeneratedGetMemberValueThirdPhase));
			typeDefinition3.GetPropertyDefinitionWithId(new MemberTypeId(2, 8)).InitializeForAutoGeneration(new GetPropertyValueDelegate(MainStoryLine.AutoGeneratedGetMemberValuePlayerSupportedKingdom));
			typeDefinition3.GetFieldDefinitionWithId(new MemberTypeId(2, 1)).InitializeForAutoGeneration(new GetFieldValueDelegate(MainStoryLine.AutoGeneratedGetMemberValueMainStoryLineSide));
			typeDefinition3.GetFieldDefinitionWithId(new MemberTypeId(2, 6)).InitializeForAutoGeneration(new GetFieldValueDelegate(MainStoryLine.AutoGeneratedGetMemberValueImperialMentorSettlement));
			typeDefinition3.GetFieldDefinitionWithId(new MemberTypeId(2, 7)).InitializeForAutoGeneration(new GetFieldValueDelegate(MainStoryLine.AutoGeneratedGetMemberValueAntiImperialMentorSettlement));
			typeDefinition3.GetFieldDefinitionWithId(new MemberTypeId(2, 10)).InitializeForAutoGeneration(new GetFieldValueDelegate(MainStoryLine.AutoGeneratedGetMemberValueFamilyRescued));
			typeDefinition3.GetFieldDefinitionWithId(new MemberTypeId(2, 9)).InitializeForAutoGeneration(new GetFieldValueDelegate(MainStoryLine.AutoGeneratedGetMemberValue_tutorialScores));
			TypeDefinition typeDefinition4 = (TypeDefinition)definitionContext.TryGetTypeDefinition(new TypeSaveId(320004));
			CollectObjectsDelegate collectObjectsDelegate4 = new CollectObjectsDelegate(TrainingField.AutoGeneratedStaticCollectObjectsTrainingField);
			typeDefinition4.InitializeForAutoGeneration(collectObjectsDelegate4);
			TypeDefinition typeDefinition5 = (TypeDefinition)definitionContext.TryGetTypeDefinition(new TypeSaveId(320005));
			CollectObjectsDelegate collectObjectsDelegate5 = new CollectObjectsDelegate(TrainingFieldEncounter.AutoGeneratedStaticCollectObjectsTrainingFieldEncounter);
			typeDefinition5.InitializeForAutoGeneration(collectObjectsDelegate5);
			TypeDefinition typeDefinition6 = (TypeDefinition)definitionContext.TryGetTypeDefinition(new TypeSaveId(320006));
			CollectObjectsDelegate collectObjectsDelegate6 = new CollectObjectsDelegate(PurchaseItemTutorialQuestTask.AutoGeneratedStaticCollectObjectsPurchaseItemTutorialQuestTask);
			typeDefinition6.InitializeForAutoGeneration(collectObjectsDelegate6);
			typeDefinition6.GetFieldDefinitionWithId(new MemberTypeId(3, 1)).InitializeForAutoGeneration(new GetFieldValueDelegate(PurchaseItemTutorialQuestTask.AutoGeneratedGetMemberValue_progressLog));
			typeDefinition6.GetFieldDefinitionWithId(new MemberTypeId(3, 2)).InitializeForAutoGeneration(new GetFieldValueDelegate(PurchaseItemTutorialQuestTask.AutoGeneratedGetMemberValue_purchasedItemAmount));
			TypeDefinition typeDefinition7 = (TypeDefinition)definitionContext.TryGetTypeDefinition(new TypeSaveId(320007));
			CollectObjectsDelegate collectObjectsDelegate7 = new CollectObjectsDelegate(RecruitTroopTutorialQuestTask.AutoGeneratedStaticCollectObjectsRecruitTroopTutorialQuestTask);
			typeDefinition7.InitializeForAutoGeneration(collectObjectsDelegate7);
			typeDefinition7.GetFieldDefinitionWithId(new MemberTypeId(3, 1)).InitializeForAutoGeneration(new GetFieldValueDelegate(RecruitTroopTutorialQuestTask.AutoGeneratedGetMemberValue_progressLog));
			typeDefinition7.GetFieldDefinitionWithId(new MemberTypeId(3, 2)).InitializeForAutoGeneration(new GetFieldValueDelegate(RecruitTroopTutorialQuestTask.AutoGeneratedGetMemberValue_recruitedTroopAmount));
			TypeDefinition typeDefinition8 = (TypeDefinition)definitionContext.TryGetTypeDefinition(new TypeSaveId(320008));
			CollectObjectsDelegate collectObjectsDelegate8 = new CollectObjectsDelegate(TutorialPhase.AutoGeneratedStaticCollectObjectsTutorialPhase);
			typeDefinition8.InitializeForAutoGeneration(collectObjectsDelegate8);
			typeDefinition8.GetPropertyDefinitionWithId(new MemberTypeId(2, 2)).InitializeForAutoGeneration(new GetPropertyValueDelegate(TutorialPhase.AutoGeneratedGetMemberValueTutorialFocusSettlement));
			typeDefinition8.GetPropertyDefinitionWithId(new MemberTypeId(2, 3)).InitializeForAutoGeneration(new GetPropertyValueDelegate(TutorialPhase.AutoGeneratedGetMemberValueTutorialFocusMobileParty));
			typeDefinition8.GetPropertyDefinitionWithId(new MemberTypeId(2, 5)).InitializeForAutoGeneration(new GetPropertyValueDelegate(TutorialPhase.AutoGeneratedGetMemberValueTalkedWithBrotherForTheFirstTime));
			typeDefinition8.GetPropertyDefinitionWithId(new MemberTypeId(2, 6)).InitializeForAutoGeneration(new GetPropertyValueDelegate(TutorialPhase.AutoGeneratedGetMemberValueLockTutorialVillageEnter));
			typeDefinition8.GetPropertyDefinitionWithId(new MemberTypeId(2, 7)).InitializeForAutoGeneration(new GetPropertyValueDelegate(TutorialPhase.AutoGeneratedGetMemberValueTutorialQuestPhase));
			typeDefinition8.GetPropertyDefinitionWithId(new MemberTypeId(2, 8)).InitializeForAutoGeneration(new GetPropertyValueDelegate(TutorialPhase.AutoGeneratedGetMemberValueIsSkipped));
			typeDefinition8.GetFieldDefinitionWithId(new MemberTypeId(2, 1)).InitializeForAutoGeneration(new GetFieldValueDelegate(TutorialPhase.AutoGeneratedGetMemberValue_tutorialPhaseShoppingRoster));
			TypeDefinition typeDefinition9 = (TypeDefinition)definitionContext.TryGetTypeDefinition(new TypeSaveId(320009));
			CollectObjectsDelegate collectObjectsDelegate9 = new CollectObjectsDelegate(FirstPhase.AutoGeneratedStaticCollectObjectsFirstPhase);
			typeDefinition9.InitializeForAutoGeneration(collectObjectsDelegate9);
			typeDefinition9.GetPropertyDefinitionWithId(new MemberTypeId(2, 1)).InitializeForAutoGeneration(new GetPropertyValueDelegate(FirstPhase.AutoGeneratedGetMemberValueCollectedBannerPieceCount));
			typeDefinition9.GetPropertyDefinitionWithId(new MemberTypeId(2, 2)).InitializeForAutoGeneration(new GetPropertyValueDelegate(FirstPhase.AutoGeneratedGetMemberValueFirstPhaseStartTime));
			TypeDefinition typeDefinition10 = (TypeDefinition)definitionContext.TryGetTypeDefinition(new TypeSaveId(320010));
			CollectObjectsDelegate collectObjectsDelegate10 = new CollectObjectsDelegate(SecondPhase.AutoGeneratedStaticCollectObjectsSecondPhase);
			typeDefinition10.InitializeForAutoGeneration(collectObjectsDelegate10);
			typeDefinition10.GetPropertyDefinitionWithId(new MemberTypeId(2, 1)).InitializeForAutoGeneration(new GetPropertyValueDelegate(SecondPhase.AutoGeneratedGetMemberValueLastConspiracyQuestCreationTime));
			typeDefinition10.GetPropertyDefinitionWithId(new MemberTypeId(2, 5)).InitializeForAutoGeneration(new GetPropertyValueDelegate(SecondPhase.AutoGeneratedGetMemberValueConspiracyStrength));
			typeDefinition10.GetFieldDefinitionWithId(new MemberTypeId(2, 3)).InitializeForAutoGeneration(new GetFieldValueDelegate(SecondPhase.AutoGeneratedGetMemberValue_stopConspiracyAttempts));
			typeDefinition10.GetFieldDefinitionWithId(new MemberTypeId(2, 4)).InitializeForAutoGeneration(new GetFieldValueDelegate(SecondPhase.AutoGeneratedGetMemberValue_lastConspiracyQuest));
			TypeDefinition typeDefinition11 = (TypeDefinition)definitionContext.TryGetTypeDefinition(new TypeSaveId(320011));
			CollectObjectsDelegate collectObjectsDelegate11 = new CollectObjectsDelegate(ThirdPhase.AutoGeneratedStaticCollectObjectsThirdPhase);
			typeDefinition11.InitializeForAutoGeneration(collectObjectsDelegate11);
			typeDefinition11.GetPropertyDefinitionWithId(new MemberTypeId(2, 3)).InitializeForAutoGeneration(new GetPropertyValueDelegate(ThirdPhase.AutoGeneratedGetMemberValueIsCompleted));
			typeDefinition11.GetFieldDefinitionWithId(new MemberTypeId(2, 1)).InitializeForAutoGeneration(new GetFieldValueDelegate(ThirdPhase.AutoGeneratedGetMemberValue_oppositionKingdoms));
			typeDefinition11.GetFieldDefinitionWithId(new MemberTypeId(2, 2)).InitializeForAutoGeneration(new GetFieldValueDelegate(ThirdPhase.AutoGeneratedGetMemberValue_allyKingdoms));
			TypeDefinition typeDefinition12 = (TypeDefinition)definitionContext.TryGetTypeDefinition(new TypeSaveId(320012));
			TypeDefinition typeDefinition13 = (TypeDefinition)definitionContext.TryGetTypeDefinition(new TypeSaveId(320013));
			CollectObjectsDelegate collectObjectsDelegate12 = new CollectObjectsDelegate(ConspiracyQuestMapNotification.AutoGeneratedStaticCollectObjectsConspiracyQuestMapNotification);
			typeDefinition13.InitializeForAutoGeneration(collectObjectsDelegate12);
			typeDefinition13.GetPropertyDefinitionWithId(new MemberTypeId(3, 1)).InitializeForAutoGeneration(new GetPropertyValueDelegate(ConspiracyQuestMapNotification.AutoGeneratedGetMemberValueConspiracyQuest));
			TypeDefinition typeDefinition14 = (TypeDefinition)definitionContext.TryGetTypeDefinition(new TypeSaveId(320014));
			CollectObjectsDelegate collectObjectsDelegate13 = new CollectObjectsDelegate(ConspiracyBaseOfOperationsDiscoveredConspiracyQuest.AutoGeneratedStaticCollectObjectsConspiracyBaseOfOperationsDiscoveredConspiracyQuest);
			typeDefinition14.InitializeForAutoGeneration(collectObjectsDelegate13);
			typeDefinition14.GetFieldDefinitionWithId(new MemberTypeId(5, 1)).InitializeForAutoGeneration(new GetFieldValueDelegate(ConspiracyBaseOfOperationsDiscoveredConspiracyQuest.AutoGeneratedGetMemberValue_hideout));
			typeDefinition14.GetFieldDefinitionWithId(new MemberTypeId(5, 2)).InitializeForAutoGeneration(new GetFieldValueDelegate(ConspiracyBaseOfOperationsDiscoveredConspiracyQuest.AutoGeneratedGetMemberValue_raiderParties));
			TypeDefinition typeDefinition15 = (TypeDefinition)definitionContext.TryGetTypeDefinition(new TypeSaveId(320015));
			CollectObjectsDelegate collectObjectsDelegate14 = new CollectObjectsDelegate(DestroyRaidersConspiracyQuest.AutoGeneratedStaticCollectObjectsDestroyRaidersConspiracyQuest);
			typeDefinition15.InitializeForAutoGeneration(collectObjectsDelegate14);
			typeDefinition15.GetFieldDefinitionWithId(new MemberTypeId(5, 1)).InitializeForAutoGeneration(new GetFieldValueDelegate(DestroyRaidersConspiracyQuest.AutoGeneratedGetMemberValue_targetSettlement));
			typeDefinition15.GetFieldDefinitionWithId(new MemberTypeId(5, 2)).InitializeForAutoGeneration(new GetFieldValueDelegate(DestroyRaidersConspiracyQuest.AutoGeneratedGetMemberValue_regularRaiderParties));
			typeDefinition15.GetFieldDefinitionWithId(new MemberTypeId(5, 3)).InitializeForAutoGeneration(new GetFieldValueDelegate(DestroyRaidersConspiracyQuest.AutoGeneratedGetMemberValue_specialRaiderParty));
			typeDefinition15.GetFieldDefinitionWithId(new MemberTypeId(5, 4)).InitializeForAutoGeneration(new GetFieldValueDelegate(DestroyRaidersConspiracyQuest.AutoGeneratedGetMemberValue_regularPartiesProgressTracker));
			typeDefinition15.GetFieldDefinitionWithId(new MemberTypeId(5, 5)).InitializeForAutoGeneration(new GetFieldValueDelegate(DestroyRaidersConspiracyQuest.AutoGeneratedGetMemberValue_specialPartyProgressTracker));
			typeDefinition15.GetFieldDefinitionWithId(new MemberTypeId(5, 6)).InitializeForAutoGeneration(new GetFieldValueDelegate(DestroyRaidersConspiracyQuest.AutoGeneratedGetMemberValue_banditFaction));
			typeDefinition15.GetFieldDefinitionWithId(new MemberTypeId(5, 7)).InitializeForAutoGeneration(new GetFieldValueDelegate(DestroyRaidersConspiracyQuest.AutoGeneratedGetMemberValue_conspiracyCaptainCharacter));
			typeDefinition15.GetFieldDefinitionWithId(new MemberTypeId(5, 8)).InitializeForAutoGeneration(new GetFieldValueDelegate(DestroyRaidersConspiracyQuest.AutoGeneratedGetMemberValue_closestHideout));
			typeDefinition15.GetFieldDefinitionWithId(new MemberTypeId(5, 9)).InitializeForAutoGeneration(new GetFieldValueDelegate(DestroyRaidersConspiracyQuest.AutoGeneratedGetMemberValue_directedRaidersToEngagePlayer));
			TypeDefinition typeDefinition16 = (TypeDefinition)definitionContext.TryGetTypeDefinition(new TypeSaveId(320017));
			CollectObjectsDelegate collectObjectsDelegate15 = new CollectObjectsDelegate(DisruptSupplyLinesConspiracyQuest.AutoGeneratedStaticCollectObjectsDisruptSupplyLinesConspiracyQuest);
			typeDefinition16.InitializeForAutoGeneration(collectObjectsDelegate15);
			typeDefinition16.GetFieldDefinitionWithId(new MemberTypeId(5, 1)).InitializeForAutoGeneration(new GetFieldValueDelegate(DisruptSupplyLinesConspiracyQuest.AutoGeneratedGetMemberValue_caravanTargetSettlements));
			typeDefinition16.GetFieldDefinitionWithId(new MemberTypeId(5, 2)).InitializeForAutoGeneration(new GetFieldValueDelegate(DisruptSupplyLinesConspiracyQuest.AutoGeneratedGetMemberValue_questCaravanMobileParty));
			typeDefinition16.GetFieldDefinitionWithId(new MemberTypeId(5, 3)).InitializeForAutoGeneration(new GetFieldValueDelegate(DisruptSupplyLinesConspiracyQuest.AutoGeneratedGetMemberValue_questStartTime));
			TypeDefinition typeDefinition17 = (TypeDefinition)definitionContext.TryGetTypeDefinition(new TypeSaveId(1014001));
			CollectObjectsDelegate collectObjectsDelegate16 = new CollectObjectsDelegate(TravelToVillageTutorialQuest.AutoGeneratedStaticCollectObjectsTravelToVillageTutorialQuest);
			typeDefinition17.InitializeForAutoGeneration(collectObjectsDelegate16);
			typeDefinition17.GetFieldDefinitionWithId(new MemberTypeId(5, 1)).InitializeForAutoGeneration(new GetFieldValueDelegate(TravelToVillageTutorialQuest.AutoGeneratedGetMemberValue_questVillage));
			typeDefinition17.GetFieldDefinitionWithId(new MemberTypeId(5, 2)).InitializeForAutoGeneration(new GetFieldValueDelegate(TravelToVillageTutorialQuest.AutoGeneratedGetMemberValue_refugeeParties));
			TypeDefinition typeDefinition18 = (TypeDefinition)definitionContext.TryGetTypeDefinition(new TypeSaveId(1013001));
			CollectObjectsDelegate collectObjectsDelegate17 = new CollectObjectsDelegate(TalkToTheHeadmanTutorialQuest.AutoGeneratedStaticCollectObjectsTalkToTheHeadmanTutorialQuest);
			typeDefinition18.InitializeForAutoGeneration(collectObjectsDelegate17);
			typeDefinition18.GetFieldDefinitionWithId(new MemberTypeId(5, 1)).InitializeForAutoGeneration(new GetFieldValueDelegate(TalkToTheHeadmanTutorialQuest.AutoGeneratedGetMemberValue_headman));
			typeDefinition18.GetFieldDefinitionWithId(new MemberTypeId(5, 2)).InitializeForAutoGeneration(new GetFieldValueDelegate(TalkToTheHeadmanTutorialQuest.AutoGeneratedGetMemberValue_recruitTroopsQuest));
			typeDefinition18.GetFieldDefinitionWithId(new MemberTypeId(5, 3)).InitializeForAutoGeneration(new GetFieldValueDelegate(TalkToTheHeadmanTutorialQuest.AutoGeneratedGetMemberValue_purchaseGrainQuest));
			TypeDefinition typeDefinition19 = (TypeDefinition)definitionContext.TryGetTypeDefinition(new TypeSaveId(1011001));
			CollectObjectsDelegate collectObjectsDelegate18 = new CollectObjectsDelegate(PurchaseGrainTutorialQuest.AutoGeneratedStaticCollectObjectsPurchaseGrainTutorialQuest);
			typeDefinition19.InitializeForAutoGeneration(collectObjectsDelegate18);
			typeDefinition19.GetFieldDefinitionWithId(new MemberTypeId(5, 1)).InitializeForAutoGeneration(new GetFieldValueDelegate(PurchaseGrainTutorialQuest.AutoGeneratedGetMemberValue_purchaseItemTutorialQuestTask));
			TypeDefinition typeDefinition20 = (TypeDefinition)definitionContext.TryGetTypeDefinition(new TypeSaveId(1012001));
			CollectObjectsDelegate collectObjectsDelegate19 = new CollectObjectsDelegate(RecruitTroopsTutorialQuest.AutoGeneratedStaticCollectObjectsRecruitTroopsTutorialQuest);
			typeDefinition20.InitializeForAutoGeneration(collectObjectsDelegate19);
			typeDefinition20.GetFieldDefinitionWithId(new MemberTypeId(5, 1)).InitializeForAutoGeneration(new GetFieldValueDelegate(RecruitTroopsTutorialQuest.AutoGeneratedGetMemberValue_recruitTroopTutorialQuestTask));
			TypeDefinition typeDefinition21 = (TypeDefinition)definitionContext.TryGetTypeDefinition(new TypeSaveId(1008001));
			CollectObjectsDelegate collectObjectsDelegate20 = new CollectObjectsDelegate(LocateAndRescueTravellerTutorialQuest.AutoGeneratedStaticCollectObjectsLocateAndRescueTravellerTutorialQuest);
			typeDefinition21.InitializeForAutoGeneration(collectObjectsDelegate20);
			typeDefinition21.GetFieldDefinitionWithId(new MemberTypeId(5, 1)).InitializeForAutoGeneration(new GetFieldValueDelegate(LocateAndRescueTravellerTutorialQuest.AutoGeneratedGetMemberValue_raiderPartyCount));
			typeDefinition21.GetFieldDefinitionWithId(new MemberTypeId(5, 2)).InitializeForAutoGeneration(new GetFieldValueDelegate(LocateAndRescueTravellerTutorialQuest.AutoGeneratedGetMemberValue_raiderParties));
			typeDefinition21.GetFieldDefinitionWithId(new MemberTypeId(5, 3)).InitializeForAutoGeneration(new GetFieldValueDelegate(LocateAndRescueTravellerTutorialQuest.AutoGeneratedGetMemberValue_defeatedRaiderPartyCount));
			typeDefinition21.GetFieldDefinitionWithId(new MemberTypeId(5, 4)).InitializeForAutoGeneration(new GetFieldValueDelegate(LocateAndRescueTravellerTutorialQuest.AutoGeneratedGetMemberValue_startQuestLog));
			TypeDefinition typeDefinition22 = (TypeDefinition)definitionContext.TryGetTypeDefinition(new TypeSaveId(1006001));
			CollectObjectsDelegate collectObjectsDelegate21 = new CollectObjectsDelegate(FindHideoutTutorialQuest.AutoGeneratedStaticCollectObjectsFindHideoutTutorialQuest);
			typeDefinition22.InitializeForAutoGeneration(collectObjectsDelegate21);
			typeDefinition22.GetFieldDefinitionWithId(new MemberTypeId(5, 1)).InitializeForAutoGeneration(new GetFieldValueDelegate(FindHideoutTutorialQuest.AutoGeneratedGetMemberValue_hideout));
			typeDefinition22.GetFieldDefinitionWithId(new MemberTypeId(5, 2)).InitializeForAutoGeneration(new GetFieldValueDelegate(FindHideoutTutorialQuest.AutoGeneratedGetMemberValue_raiderParties));
			typeDefinition22.GetFieldDefinitionWithId(new MemberTypeId(5, 4)).InitializeForAutoGeneration(new GetFieldValueDelegate(FindHideoutTutorialQuest.AutoGeneratedGetMemberValue_talkedWithRadagos));
			typeDefinition22.GetFieldDefinitionWithId(new MemberTypeId(5, 5)).InitializeForAutoGeneration(new GetFieldValueDelegate(FindHideoutTutorialQuest.AutoGeneratedGetMemberValue_talkedWithBrother));
			typeDefinition22.GetFieldDefinitionWithId(new MemberTypeId(5, 6)).InitializeForAutoGeneration(new GetFieldValueDelegate(FindHideoutTutorialQuest.AutoGeneratedGetMemberValue_hideoutBattleEndState));
			TypeDefinition typeDefinition23 = (TypeDefinition)definitionContext.TryGetTypeDefinition(new TypeSaveId(1004001));
			CollectObjectsDelegate collectObjectsDelegate22 = new CollectObjectsDelegate(BannerInvestigationQuest.AutoGeneratedStaticCollectObjectsBannerInvestigationQuest);
			typeDefinition23.InitializeForAutoGeneration(collectObjectsDelegate22);
			typeDefinition23.GetFieldDefinitionWithId(new MemberTypeId(5, 1)).InitializeForAutoGeneration(new GetFieldValueDelegate(BannerInvestigationQuest.AutoGeneratedGetMemberValue_noblesToTalk));
			typeDefinition23.GetFieldDefinitionWithId(new MemberTypeId(5, 2)).InitializeForAutoGeneration(new GetFieldValueDelegate(BannerInvestigationQuest.AutoGeneratedGetMemberValue_allNoblesDead));
			typeDefinition23.GetFieldDefinitionWithId(new MemberTypeId(5, 3)).InitializeForAutoGeneration(new GetFieldValueDelegate(BannerInvestigationQuest.AutoGeneratedGetMemberValue_battleSummarized));
			typeDefinition23.GetFieldDefinitionWithId(new MemberTypeId(5, 4)).InitializeForAutoGeneration(new GetFieldValueDelegate(BannerInvestigationQuest.AutoGeneratedGetMemberValue_talkedNotablesQuestLog));
			TypeDefinition typeDefinition24 = (TypeDefinition)definitionContext.TryGetTypeDefinition(new TypeSaveId(1003001));
			CollectObjectsDelegate collectObjectsDelegate23 = new CollectObjectsDelegate(AssembleTheBannerQuest.AutoGeneratedStaticCollectObjectsAssembleTheBannerQuest);
			typeDefinition24.InitializeForAutoGeneration(collectObjectsDelegate23);
			typeDefinition24.GetFieldDefinitionWithId(new MemberTypeId(5, 1)).InitializeForAutoGeneration(new GetFieldValueDelegate(AssembleTheBannerQuest.AutoGeneratedGetMemberValue_startLog));
			typeDefinition24.GetFieldDefinitionWithId(new MemberTypeId(5, 2)).InitializeForAutoGeneration(new GetFieldValueDelegate(AssembleTheBannerQuest.AutoGeneratedGetMemberValue_talkedWithImperialMentor));
			typeDefinition24.GetFieldDefinitionWithId(new MemberTypeId(5, 3)).InitializeForAutoGeneration(new GetFieldValueDelegate(AssembleTheBannerQuest.AutoGeneratedGetMemberValue_talkedWithAntiImperialMentor));
			TypeDefinition typeDefinition25 = (TypeDefinition)definitionContext.TryGetTypeDefinition(new TypeSaveId(1010001));
			CollectObjectsDelegate collectObjectsDelegate24 = new CollectObjectsDelegate(MeetWithIstianaQuest.AutoGeneratedStaticCollectObjectsMeetWithIstianaQuest);
			typeDefinition25.InitializeForAutoGeneration(collectObjectsDelegate24);
			typeDefinition25.GetFieldDefinitionWithId(new MemberTypeId(5, 1)).InitializeForAutoGeneration(new GetFieldValueDelegate(MeetWithIstianaQuest.AutoGeneratedGetMemberValue_metImperialMentor));
			TypeDefinition typeDefinition26 = (TypeDefinition)definitionContext.TryGetTypeDefinition(new TypeSaveId(1009001));
			CollectObjectsDelegate collectObjectsDelegate25 = new CollectObjectsDelegate(MeetWithArzagosQuest.AutoGeneratedStaticCollectObjectsMeetWithArzagosQuest);
			typeDefinition26.InitializeForAutoGeneration(collectObjectsDelegate25);
			typeDefinition26.GetFieldDefinitionWithId(new MemberTypeId(5, 1)).InitializeForAutoGeneration(new GetFieldValueDelegate(MeetWithArzagosQuest.AutoGeneratedGetMemberValue_metAntiImperialMentor));
			TypeDefinition typeDefinition27 = (TypeDefinition)definitionContext.TryGetTypeDefinition(new TypeSaveId(1007001));
			CollectObjectsDelegate collectObjectsDelegate26 = new CollectObjectsDelegate(IstianasBannerPieceQuest.AutoGeneratedStaticCollectObjectsIstianasBannerPieceQuest);
			typeDefinition27.InitializeForAutoGeneration(collectObjectsDelegate26);
			typeDefinition27.GetFieldDefinitionWithId(new MemberTypeId(5, 1)).InitializeForAutoGeneration(new GetFieldValueDelegate(IstianasBannerPieceQuest.AutoGeneratedGetMemberValue_hideout));
			typeDefinition27.GetFieldDefinitionWithId(new MemberTypeId(5, 2)).InitializeForAutoGeneration(new GetFieldValueDelegate(IstianasBannerPieceQuest.AutoGeneratedGetMemberValue_raiderParties));
			typeDefinition27.GetFieldDefinitionWithId(new MemberTypeId(5, 3)).InitializeForAutoGeneration(new GetFieldValueDelegate(IstianasBannerPieceQuest.AutoGeneratedGetMemberValue_hideoutBattleEndState));
			TypeDefinition typeDefinition28 = (TypeDefinition)definitionContext.TryGetTypeDefinition(new TypeSaveId(1001001));
			CollectObjectsDelegate collectObjectsDelegate27 = new CollectObjectsDelegate(ArzagosBannerPieceQuest.AutoGeneratedStaticCollectObjectsArzagosBannerPieceQuest);
			typeDefinition28.InitializeForAutoGeneration(collectObjectsDelegate27);
			typeDefinition28.GetFieldDefinitionWithId(new MemberTypeId(5, 1)).InitializeForAutoGeneration(new GetFieldValueDelegate(ArzagosBannerPieceQuest.AutoGeneratedGetMemberValue_hideout));
			typeDefinition28.GetFieldDefinitionWithId(new MemberTypeId(5, 2)).InitializeForAutoGeneration(new GetFieldValueDelegate(ArzagosBannerPieceQuest.AutoGeneratedGetMemberValue_raiderParties));
			typeDefinition28.GetFieldDefinitionWithId(new MemberTypeId(5, 3)).InitializeForAutoGeneration(new GetFieldValueDelegate(ArzagosBannerPieceQuest.AutoGeneratedGetMemberValue_hideoutBattleEndState));
			TypeDefinition typeDefinition29 = (TypeDefinition)definitionContext.TryGetTypeDefinition(new TypeSaveId(1000001));
			CollectObjectsDelegate collectObjectsDelegate28 = new CollectObjectsDelegate(SupportKingdomQuest.AutoGeneratedStaticCollectObjectsSupportKingdomQuest);
			typeDefinition29.InitializeForAutoGeneration(collectObjectsDelegate28);
			typeDefinition29.GetFieldDefinitionWithId(new MemberTypeId(5, 1)).InitializeForAutoGeneration(new GetFieldValueDelegate(SupportKingdomQuest.AutoGeneratedGetMemberValue_isImperial));
			typeDefinition29.GetFieldDefinitionWithId(new MemberTypeId(5, 2)).InitializeForAutoGeneration(new GetFieldValueDelegate(SupportKingdomQuest.AutoGeneratedGetMemberValue_playerRuledKingdom));
			TypeDefinition typeDefinition30 = (TypeDefinition)definitionContext.TryGetTypeDefinition(new TypeSaveId(900001));
			CollectObjectsDelegate collectObjectsDelegate29 = new CollectObjectsDelegate(CreateKingdomQuest.AutoGeneratedStaticCollectObjectsCreateKingdomQuest);
			typeDefinition30.InitializeForAutoGeneration(collectObjectsDelegate29);
			typeDefinition30.GetFieldDefinitionWithId(new MemberTypeId(5, 1)).InitializeForAutoGeneration(new GetFieldValueDelegate(CreateKingdomQuest.AutoGeneratedGetMemberValue_isImperial));
			typeDefinition30.GetFieldDefinitionWithId(new MemberTypeId(5, 2)).InitializeForAutoGeneration(new GetFieldValueDelegate(CreateKingdomQuest.AutoGeneratedGetMemberValue_hasPlayerCreatedKingdom));
			typeDefinition30.GetFieldDefinitionWithId(new MemberTypeId(5, 9)).InitializeForAutoGeneration(new GetFieldValueDelegate(CreateKingdomQuest.AutoGeneratedGetMemberValue_leftKingdomLog));
			typeDefinition30.GetFieldDefinitionWithId(new MemberTypeId(5, 10)).InitializeForAutoGeneration(new GetFieldValueDelegate(CreateKingdomQuest.AutoGeneratedGetMemberValue_playerCreatedKingdom));
			typeDefinition30.GetFieldDefinitionWithId(new MemberTypeId(5, 4)).InitializeForAutoGeneration(new GetFieldValueDelegate(CreateKingdomQuest.AutoGeneratedGetMemberValue_clanTierRequirementLog));
			typeDefinition30.GetFieldDefinitionWithId(new MemberTypeId(5, 5)).InitializeForAutoGeneration(new GetFieldValueDelegate(CreateKingdomQuest.AutoGeneratedGetMemberValue_partySizeRequirementLog));
			typeDefinition30.GetFieldDefinitionWithId(new MemberTypeId(5, 6)).InitializeForAutoGeneration(new GetFieldValueDelegate(CreateKingdomQuest.AutoGeneratedGetMemberValue_settlementOwnershipRequirementLog));
			typeDefinition30.GetFieldDefinitionWithId(new MemberTypeId(5, 7)).InitializeForAutoGeneration(new GetFieldValueDelegate(CreateKingdomQuest.AutoGeneratedGetMemberValue_clanIndependenceRequirementLog));
			TypeDefinition typeDefinition31 = (TypeDefinition)definitionContext.TryGetTypeDefinition(new TypeSaveId(1015001));
			CollectObjectsDelegate collectObjectsDelegate30 = new CollectObjectsDelegate(ConspiracyProgressQuest.AutoGeneratedStaticCollectObjectsConspiracyProgressQuest);
			typeDefinition31.InitializeForAutoGeneration(collectObjectsDelegate30);
			typeDefinition31.GetFieldDefinitionWithId(new MemberTypeId(5, 2)).InitializeForAutoGeneration(new GetFieldValueDelegate(ConspiracyProgressQuest.AutoGeneratedGetMemberValue_startQuestLog));
			TypeDefinition typeDefinition32 = (TypeDefinition)definitionContext.TryGetTypeDefinition(new TypeSaveId(4100001));
			CollectObjectsDelegate collectObjectsDelegate31 = new CollectObjectsDelegate(RebuildPlayerClanQuest.AutoGeneratedStaticCollectObjectsRebuildPlayerClanQuest);
			typeDefinition32.InitializeForAutoGeneration(collectObjectsDelegate31);
			typeDefinition32.GetFieldDefinitionWithId(new MemberTypeId(5, 1)).InitializeForAutoGeneration(new GetFieldValueDelegate(RebuildPlayerClanQuest.AutoGeneratedGetMemberValue_goldGoalLog));
			typeDefinition32.GetFieldDefinitionWithId(new MemberTypeId(5, 2)).InitializeForAutoGeneration(new GetFieldValueDelegate(RebuildPlayerClanQuest.AutoGeneratedGetMemberValue_partySizeGoalLog));
			typeDefinition32.GetFieldDefinitionWithId(new MemberTypeId(5, 3)).InitializeForAutoGeneration(new GetFieldValueDelegate(RebuildPlayerClanQuest.AutoGeneratedGetMemberValue_clanTierGoalLog));
			typeDefinition32.GetFieldDefinitionWithId(new MemberTypeId(5, 4)).InitializeForAutoGeneration(new GetFieldValueDelegate(RebuildPlayerClanQuest.AutoGeneratedGetMemberValue_hireCompanionGoalLog));
			TypeDefinition typeDefinition33 = (TypeDefinition)definitionContext.TryGetTypeDefinition(new TypeSaveId(16001));
			CollectObjectsDelegate collectObjectsDelegate32 = new CollectObjectsDelegate(DefeatTheConspiracyQuestBehavior.OppositionData.AutoGeneratedStaticCollectObjectsOppositionData);
			typeDefinition33.InitializeForAutoGeneration(collectObjectsDelegate32);
			typeDefinition33.GetFieldDefinitionWithId(new MemberTypeId(2, 10)).InitializeForAutoGeneration(new GetFieldValueDelegate(DefeatTheConspiracyQuestBehavior.OppositionData.AutoGeneratedGetMemberValueInitialWarScore));
			typeDefinition33.GetFieldDefinitionWithId(new MemberTypeId(2, 20)).InitializeForAutoGeneration(new GetFieldValueDelegate(DefeatTheConspiracyQuestBehavior.OppositionData.AutoGeneratedGetMemberValueReinforcedWarScore));
			typeDefinition33.GetFieldDefinitionWithId(new MemberTypeId(2, 30)).InitializeForAutoGeneration(new GetFieldValueDelegate(DefeatTheConspiracyQuestBehavior.OppositionData.AutoGeneratedGetMemberValueQuestLog));
			typeDefinition33.GetFieldDefinitionWithId(new MemberTypeId(2, 40)).InitializeForAutoGeneration(new GetFieldValueDelegate(DefeatTheConspiracyQuestBehavior.OppositionData.AutoGeneratedGetMemberValueLastPeaceOfferDate));
			TypeDefinition typeDefinition34 = (TypeDefinition)definitionContext.TryGetTypeDefinition(new TypeSaveId(16002));
			CollectObjectsDelegate collectObjectsDelegate33 = new CollectObjectsDelegate(DefeatTheConspiracyQuestBehavior.DefeatTheConspiracyQuest.AutoGeneratedStaticCollectObjectsDefeatTheConspiracyQuest);
			typeDefinition34.InitializeForAutoGeneration(collectObjectsDelegate33);
			typeDefinition34.GetFieldDefinitionWithId(new MemberTypeId(5, 100)).InitializeForAutoGeneration(new GetFieldValueDelegate(DefeatTheConspiracyQuestBehavior.DefeatTheConspiracyQuest.AutoGeneratedGetMemberValue_oppositionKingdom));
			typeDefinition34.GetFieldDefinitionWithId(new MemberTypeId(5, 110)).InitializeForAutoGeneration(new GetFieldValueDelegate(DefeatTheConspiracyQuestBehavior.DefeatTheConspiracyQuest.AutoGeneratedGetMemberValue_oppositionData));
			TypeDefinition typeDefinition35 = (TypeDefinition)definitionContext.TryGetTypeDefinition(new TypeSaveId(1002001));
			CollectObjectsDelegate collectObjectsDelegate34 = new CollectObjectsDelegate(AssembleEmpireQuestBehavior.AssembleEmpireQuest.AutoGeneratedStaticCollectObjectsAssembleEmpireQuest);
			typeDefinition35.InitializeForAutoGeneration(collectObjectsDelegate34);
			typeDefinition35.GetFieldDefinitionWithId(new MemberTypeId(5, 1)).InitializeForAutoGeneration(new GetFieldValueDelegate(AssembleEmpireQuestBehavior.AssembleEmpireQuest.AutoGeneratedGetMemberValue_numberOfCapturedSettlementsLog));
			TypeDefinition typeDefinition36 = (TypeDefinition)definitionContext.TryGetTypeDefinition(new TypeSaveId(1005001));
			CollectObjectsDelegate collectObjectsDelegate35 = new CollectObjectsDelegate(WeakenEmpireQuestBehavior.WeakenEmpireQuest.AutoGeneratedStaticCollectObjectsWeakenEmpireQuest);
			typeDefinition36.InitializeForAutoGeneration(collectObjectsDelegate35);
			TypeDefinition typeDefinition37 = (TypeDefinition)definitionContext.TryGetTypeDefinition(new TypeSaveId(4140001));
			CollectObjectsDelegate collectObjectsDelegate36 = new CollectObjectsDelegate(RescueFamilyQuestBehavior.RescueFamilyQuest.AutoGeneratedStaticCollectObjectsRescueFamilyQuest);
			typeDefinition37.InitializeForAutoGeneration(collectObjectsDelegate36);
			typeDefinition37.GetFieldDefinitionWithId(new MemberTypeId(5, 1)).InitializeForAutoGeneration(new GetFieldValueDelegate(RescueFamilyQuestBehavior.RescueFamilyQuest.AutoGeneratedGetMemberValue_hideout));
			typeDefinition37.GetFieldDefinitionWithId(new MemberTypeId(5, 2)).InitializeForAutoGeneration(new GetFieldValueDelegate(RescueFamilyQuestBehavior.RescueFamilyQuest.AutoGeneratedGetMemberValue_reunionTalkDone));
			typeDefinition37.GetFieldDefinitionWithId(new MemberTypeId(5, 3)).InitializeForAutoGeneration(new GetFieldValueDelegate(RescueFamilyQuestBehavior.RescueFamilyQuest.AutoGeneratedGetMemberValue_hideoutTalkDone));
			typeDefinition37.GetFieldDefinitionWithId(new MemberTypeId(5, 4)).InitializeForAutoGeneration(new GetFieldValueDelegate(RescueFamilyQuestBehavior.RescueFamilyQuest.AutoGeneratedGetMemberValue_brotherConversationDone));
			typeDefinition37.GetFieldDefinitionWithId(new MemberTypeId(5, 5)).InitializeForAutoGeneration(new GetFieldValueDelegate(RescueFamilyQuestBehavior.RescueFamilyQuest.AutoGeneratedGetMemberValue_radagosGoodByeConversationDone));
			typeDefinition37.GetFieldDefinitionWithId(new MemberTypeId(5, 6)).InitializeForAutoGeneration(new GetFieldValueDelegate(RescueFamilyQuestBehavior.RescueFamilyQuest.AutoGeneratedGetMemberValue_hideoutBattleEndState));
			typeDefinition37.GetFieldDefinitionWithId(new MemberTypeId(5, 7)).InitializeForAutoGeneration(new GetFieldValueDelegate(RescueFamilyQuestBehavior.RescueFamilyQuest.AutoGeneratedGetMemberValue_raiderParties));
			SaveId saveId = SaveId.ReadSaveIdFrom(new StringReader("2 1 0 320004 "));
			ContainerDefinition containerDefinition = (ContainerDefinition)definitionContext.TryGetTypeDefinition(saveId);
			CollectObjectsDelegate collectObjectsDelegate37 = new CollectObjectsDelegate(AutoGeneratedSaveManager.AutoGeneratedStaticCollectObjectsForList0);
			containerDefinition.InitializeForAutoGeneration(collectObjectsDelegate37, false);
			SaveId saveId2 = SaveId.ReadSaveIdFrom(new StringReader("2 5 0 320004 "));
			ContainerDefinition containerDefinition2 = (ContainerDefinition)definitionContext.TryGetTypeDefinition(saveId2);
			CollectObjectsDelegate collectObjectsDelegate38 = new CollectObjectsDelegate(AutoGeneratedSaveManager.AutoGeneratedStaticCollectObjectsForList1);
			containerDefinition2.InitializeForAutoGeneration(collectObjectsDelegate38, false);
			SaveId saveId3 = SaveId.ReadSaveIdFrom(new StringReader("2 6 0 320004 "));
			ContainerDefinition containerDefinition3 = (ContainerDefinition)definitionContext.TryGetTypeDefinition(saveId3);
			CollectObjectsDelegate collectObjectsDelegate39 = new CollectObjectsDelegate(AutoGeneratedSaveManager.AutoGeneratedStaticCollectObjectsForList2);
			containerDefinition3.InitializeForAutoGeneration(collectObjectsDelegate39, false);
			SaveId saveId4 = SaveId.ReadSaveIdFrom(new StringReader("2 2 0 30001 0 331001 "));
			ContainerDefinition containerDefinition4 = (ContainerDefinition)definitionContext.TryGetTypeDefinition(saveId4);
			CollectObjectsDelegate collectObjectsDelegate40 = new CollectObjectsDelegate(AutoGeneratedSaveManager.AutoGeneratedStaticCollectObjectsForDictionary3);
			containerDefinition4.InitializeForAutoGeneration(collectObjectsDelegate40, true);
		}

		// Token: 0x0600004F RID: 79 RVA: 0x00003C50 File Offset: 0x00001E50
		private static void AutoGeneratedStaticCollectObjectsForList0(object o, List<object> collectedObjects)
		{
			IList list = (IList)o;
			for (int i = 0; i < list.Count; i++)
			{
				TrainingField trainingField = (TrainingField)list[i];
				collectedObjects.Add(trainingField);
			}
		}

		// Token: 0x06000050 RID: 80 RVA: 0x00003C8C File Offset: 0x00001E8C
		private static void AutoGeneratedStaticCollectObjectsForList1(object o, List<object> collectedObjects)
		{
			IList list = (IList)o;
			for (int i = 0; i < list.Count; i++)
			{
				TrainingField trainingField = (TrainingField)list[i];
				collectedObjects.Add(trainingField);
			}
		}

		// Token: 0x06000051 RID: 81 RVA: 0x00003CC8 File Offset: 0x00001EC8
		private static void AutoGeneratedStaticCollectObjectsForList2(object o, List<object> collectedObjects)
		{
			IList list = (IList)o;
			for (int i = 0; i < list.Count; i++)
			{
				TrainingField trainingField = (TrainingField)list[i];
				collectedObjects.Add(trainingField);
			}
		}

		// Token: 0x06000052 RID: 82 RVA: 0x00003D01 File Offset: 0x00001F01
		private static void AutoGeneratedStaticCollectObjectsForDictionary3(object o, List<object> collectedObjects)
		{
		}
	}
}
