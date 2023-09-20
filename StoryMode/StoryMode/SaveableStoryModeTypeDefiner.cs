using System;
using System.Collections.Generic;
using StoryMode.Quests.FirstPhase;
using StoryMode.Quests.PlayerClanQuests;
using StoryMode.Quests.QuestTasks;
using StoryMode.Quests.SecondPhase;
using StoryMode.Quests.SecondPhase.ConspiracyQuests;
using StoryMode.Quests.TutorialPhase;
using StoryMode.StoryModePhases;
using TaleWorlds.CampaignSystem;
using TaleWorlds.ObjectSystem;
using TaleWorlds.SaveSystem;

namespace StoryMode
{
	// Token: 0x0200000B RID: 11
	public class SaveableStoryModeTypeDefiner : SaveableTypeDefiner
	{
		// Token: 0x06000045 RID: 69 RVA: 0x00002569 File Offset: 0x00000769
		public SaveableStoryModeTypeDefiner()
			: base(320000)
		{
		}

		// Token: 0x06000046 RID: 70 RVA: 0x00002578 File Offset: 0x00000778
		protected override void DefineClassTypes()
		{
			base.AddClassDefinition(typeof(CampaignStoryMode), 1, null);
			base.AddClassDefinition(typeof(StoryModeManager), 2, null);
			base.AddClassDefinition(typeof(MainStoryLine), 3, null);
			base.AddClassDefinition(typeof(TrainingField), 4, null);
			base.AddClassDefinition(typeof(TrainingFieldEncounter), 5, null);
			base.AddClassDefinition(typeof(PurchaseItemTutorialQuestTask), 6, null);
			base.AddClassDefinition(typeof(RecruitTroopTutorialQuestTask), 7, null);
			base.AddClassDefinition(typeof(TutorialPhase), 8, null);
			base.AddClassDefinition(typeof(FirstPhase), 9, null);
			base.AddClassDefinition(typeof(SecondPhase), 10, null);
			base.AddClassDefinition(typeof(ThirdPhase), 11, null);
			base.AddClassDefinition(typeof(ConspiracyQuestBase), 12, null);
			base.AddClassDefinition(typeof(ConspiracyQuestMapNotification), 13, null);
			base.AddClassDefinition(typeof(ConspiracyBaseOfOperationsDiscoveredConspiracyQuest), 14, null);
			base.AddClassDefinition(typeof(DestroyRaidersConspiracyQuest), 15, null);
			base.AddClassDefinition(typeof(DisruptSupplyLinesConspiracyQuest), 17, null);
			base.AddClassDefinition(typeof(TravelToVillageTutorialQuest), 694001, null);
			base.AddClassDefinition(typeof(TalkToTheHeadmanTutorialQuest), 693001, null);
			base.AddClassDefinition(typeof(PurchaseGrainTutorialQuest), 691001, null);
			base.AddClassDefinition(typeof(RecruitTroopsTutorialQuest), 692001, null);
			base.AddClassDefinition(typeof(LocateAndRescueTravellerTutorialQuest), 688001, null);
			base.AddClassDefinition(typeof(FindHideoutTutorialQuest), 686001, null);
			base.AddClassDefinition(typeof(BannerInvestigationQuest), 684001, null);
			base.AddClassDefinition(typeof(AssembleTheBannerQuest), 683001, null);
			base.AddClassDefinition(typeof(MeetWithIstianaQuest), 690001, null);
			base.AddClassDefinition(typeof(MeetWithArzagosQuest), 689001, null);
			base.AddClassDefinition(typeof(IstianasBannerPieceQuest), 687001, null);
			base.AddClassDefinition(typeof(ArzagosBannerPieceQuest), 681001, null);
			base.AddClassDefinition(typeof(SupportKingdomQuest), 680001, null);
			base.AddClassDefinition(typeof(CreateKingdomQuest), 580001, null);
			base.AddClassDefinition(typeof(ConspiracyProgressQuest), 695001, null);
			base.AddClassDefinition(typeof(RebuildPlayerClanQuest), 3780001, null);
		}

		// Token: 0x06000047 RID: 71 RVA: 0x0000280D File Offset: 0x00000A0D
		protected override void DefineStructTypes()
		{
		}

		// Token: 0x06000048 RID: 72 RVA: 0x00002810 File Offset: 0x00000A10
		protected override void DefineEnumTypes()
		{
			base.AddEnumDefinition(typeof(MainStoryLineSide), 2001, null);
			base.AddEnumDefinition(typeof(TutorialQuestPhase), 2002, null);
			base.AddEnumDefinition(typeof(FindHideoutTutorialQuest.HideoutBattleEndState), 686010, null);
			base.AddEnumDefinition(typeof(IstianasBannerPieceQuest.HideoutBattleEndState), 687010, null);
			base.AddEnumDefinition(typeof(ArzagosBannerPieceQuest.HideoutBattleEndState), 681010, null);
		}

		// Token: 0x06000049 RID: 73 RVA: 0x0000288B File Offset: 0x00000A8B
		protected override void DefineInterfaceTypes()
		{
		}

		// Token: 0x0600004A RID: 74 RVA: 0x0000288D File Offset: 0x00000A8D
		protected override void DefineRootClassTypes()
		{
		}

		// Token: 0x0600004B RID: 75 RVA: 0x0000288F File Offset: 0x00000A8F
		protected override void DefineGenericClassDefinitions()
		{
		}

		// Token: 0x0600004C RID: 76 RVA: 0x00002891 File Offset: 0x00000A91
		protected override void DefineGenericStructDefinitions()
		{
		}

		// Token: 0x0600004D RID: 77 RVA: 0x00002894 File Offset: 0x00000A94
		protected override void DefineContainerDefinitions()
		{
			base.ConstructContainerDefinition(typeof(List<TrainingField>));
			base.ConstructContainerDefinition(typeof(Dictionary<string, TrainingField>));
			base.ConstructContainerDefinition(typeof(Dictionary<MBGUID, TrainingField>));
			base.ConstructContainerDefinition(typeof(Dictionary<int, CampaignTime>));
		}
	}
}
