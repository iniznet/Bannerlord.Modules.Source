using System;
using StoryMode.GameComponents;
using StoryMode.GameComponents.CampaignBehaviors;
using StoryMode.Quests.PlayerClanQuests;
using StoryMode.Quests.SecondPhase;
using StoryMode.Quests.ThirdPhase;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.ModuleManager;
using TaleWorlds.MountAndBlade;
using TaleWorlds.ObjectSystem;

namespace StoryMode
{
	// Token: 0x02000015 RID: 21
	public class StoryModeSubModule : MBSubModuleBase
	{
		// Token: 0x06000096 RID: 150 RVA: 0x00004CB0 File Offset: 0x00002EB0
		protected override void InitializeGameStarter(Game game, IGameStarter gameStarterObject)
		{
			CampaignStoryMode campaignStoryMode = game.GameType as CampaignStoryMode;
			if (campaignStoryMode != null)
			{
				CampaignGameStarter campaignGameStarter = (CampaignGameStarter)gameStarterObject;
				campaignStoryMode.AddCampaignEventReceiver(StoryModeEvents.Instance);
				this.AddGameMenus(campaignGameStarter);
				this.AddModels(campaignGameStarter);
				this.AddBehaviors(campaignGameStarter);
			}
		}

		// Token: 0x06000097 RID: 151 RVA: 0x00004CF3 File Offset: 0x00002EF3
		public override void OnGameEnd(Game game)
		{
			base.OnGameEnd(game);
			if (game.GameType is CampaignStoryMode && StoryModeManager.Current != null)
			{
				StoryModeManager.Current.Destroy();
			}
		}

		// Token: 0x06000098 RID: 152 RVA: 0x00004D1C File Offset: 0x00002F1C
		private void AddGameMenus(CampaignGameStarter campaignGameStarter)
		{
			campaignGameStarter.AddGameMenu("menu_story_mode_welcome", "{=GGfM1HKn}Welcome to MBII Bannerlord", null, 0, 0, null);
			campaignGameStarter.AddGameMenuOption("menu_story_mode_welcome", "mno_continue", "{=str_continue}Continue...", delegate(MenuCallbackArgs args)
			{
				args.optionLeaveType = 17;
				return true;
			}, null, false, -1, false, null);
		}

		// Token: 0x06000099 RID: 153 RVA: 0x00004D78 File Offset: 0x00002F78
		private void AddBehaviors(CampaignGameStarter campaignGameStarter)
		{
			campaignGameStarter.AddBehavior(new LordConversationsStoryModeBehavior());
			campaignGameStarter.AddBehavior(new MainStorylineCampaignBehavior());
			if (!StoryModeManager.Current.MainStoryLine.IsCompleted)
			{
				if (!StoryModeManager.Current.MainStoryLine.TutorialPhase.IsCompleted)
				{
					campaignGameStarter.AddBehavior(new TutorialPhaseCampaignBehavior());
				}
				if (!StoryModeManager.Current.MainStoryLine.IsFirstPhaseCompleted)
				{
					campaignGameStarter.AddBehavior(new FirstPhaseCampaignBehavior());
				}
				if (!StoryModeManager.Current.MainStoryLine.IsSecondPhaseCompleted)
				{
					campaignGameStarter.AddBehavior(new SecondPhaseCampaignBehavior());
				}
				campaignGameStarter.AddBehavior(new ThirdPhaseCampaignBehavior());
			}
			campaignGameStarter.AddBehavior(new TrainingFieldCampaignBehavior());
			campaignGameStarter.AddBehavior(new StoryModeTutorialBoxCampaignBehavior());
			Debug.Print("campaignGameStarter.AddBehavior(AchievementsCampaignBehavior)", 0, 12, 17592186044416UL);
			campaignGameStarter.AddBehavior(new AchievementsCampaignBehavior());
			campaignGameStarter.AddBehavior(new WeakenEmpireQuestBehavior());
			campaignGameStarter.AddBehavior(new AssembleEmpireQuestBehavior());
			campaignGameStarter.AddBehavior(new DefeatTheConspiracyQuestBehavior());
			campaignGameStarter.AddBehavior(new RescueFamilyQuestBehavior());
		}

		// Token: 0x0600009A RID: 154 RVA: 0x00004E74 File Offset: 0x00003074
		private void AddModels(CampaignGameStarter campaignGameStarter)
		{
			campaignGameStarter.AddModel(new StoryModeBanditDensityModel());
			campaignGameStarter.AddModel(new StoryModeEncounterGameMenuModel());
			campaignGameStarter.AddModel(new StoryModeBattleRewardModel());
			campaignGameStarter.AddModel(new StoryModeTargetScoreCalculatingModel());
			campaignGameStarter.AddModel(new StoryModePartyWageModel());
			campaignGameStarter.AddModel(new StoryModeKingdomDecisionPermissionModel());
			campaignGameStarter.AddModel(new StoryModeCombatXpModel());
			campaignGameStarter.AddModel(new StoryModeGenericXpModel());
			campaignGameStarter.AddModel(new StoryModeNotableSpawnModel());
			campaignGameStarter.AddModel(new StoryModeHeroDeathProbabilityCalculationModel());
			campaignGameStarter.AddModel(new StoryModeAgentDecideKilledOrUnconsciousModel());
			campaignGameStarter.AddModel(new StoryModePartySizeLimitModel());
			campaignGameStarter.AddModel(new StoryModeBannerItemModel());
			campaignGameStarter.AddModel(new StoryModePrisonerRecruitmentCalculationModel());
			campaignGameStarter.AddModel(new StoryModeTroopSupplierProbabilityModel());
			campaignGameStarter.AddModel(new StoryModeCutsceneSelectionModel());
			campaignGameStarter.AddModel(new StoryModeVoiceOverModel());
		}

		// Token: 0x0600009B RID: 155 RVA: 0x00004F3C File Offset: 0x0000313C
		public override void RegisterSubModuleObjects(bool isSavedCampaign)
		{
			if (StoryModeManager.Current != null)
			{
				MBObjectManager.Instance.LoadOneXmlFromFile(ModuleHelper.GetModuleFullPath("StoryMode") + "ModuleData/story_mode_settlements.xml", null, true);
			}
		}

		// Token: 0x0600009C RID: 156 RVA: 0x00004F65 File Offset: 0x00003165
		protected override void OnApplicationTick(float dt)
		{
			base.OnApplicationTick(dt);
			if (StoryModeManager.Current != null)
			{
				StoryModeManager.Current.TickRealTime(dt);
			}
		}

		// Token: 0x0600009D RID: 157 RVA: 0x00004F80 File Offset: 0x00003180
		public override void OnConfigChanged()
		{
			if (StoryModeManager.Current != null)
			{
				StoryModeManager.Current.StoryModeEvents.OnConfigChanged();
			}
		}
	}
}
