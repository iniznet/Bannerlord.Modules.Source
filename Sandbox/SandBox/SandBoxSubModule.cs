using System;
using System.Linq;
using SandBox.AI;
using SandBox.CampaignBehaviors;
using SandBox.GameComponents;
using SandBox.Issues;
using SandBox.Objects;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.MountAndBlade;
using TaleWorlds.ObjectSystem;
using TaleWorlds.SaveSystem;
using TaleWorlds.SaveSystem.Load;

namespace SandBox
{
	// Token: 0x02000017 RID: 23
	public class SandBoxSubModule : MBSubModuleBase
	{
		// Token: 0x060000DD RID: 221 RVA: 0x000070C8 File Offset: 0x000052C8
		protected override void OnSubModuleLoad()
		{
			base.OnSubModuleLoad();
			Module.CurrentModule.SetEditorMissionTester(new SandBoxEditorMissionTester());
		}

		// Token: 0x060000DE RID: 222 RVA: 0x000070E0 File Offset: 0x000052E0
		protected override void InitializeGameStarter(Game game, IGameStarter gameStarterObject)
		{
			if (game.GameType is Campaign)
			{
				DefaultMapWeatherModel defaultMapWeatherModel = gameStarterObject.Models.FirstOrDefault((GameModel model) => model is DefaultMapWeatherModel) as DefaultMapWeatherModel;
				if (defaultMapWeatherModel != null)
				{
					byte[] array = new byte[1048576];
					Utilities.GetSnowAmountData(array);
					defaultMapWeatherModel.InitializeSnowAmountData(array);
				}
				gameStarterObject.AddModel(new SandboxAgentStatCalculateModel());
				gameStarterObject.AddModel(new SandboxAgentApplyDamageModel());
				gameStarterObject.AddModel(new SandboxMissionDifficultyModel());
				gameStarterObject.AddModel(new SandboxApplyWeatherEffectsModel());
				gameStarterObject.AddModel(new SandboxAutoBlockModel());
				gameStarterObject.AddModel(new SandboxAgentDecideKilledOrUnconsciousModel());
				gameStarterObject.AddModel(new SandboxBattleBannerBearersModel());
				gameStarterObject.AddModel(new DefaultFormationArrangementModel());
				gameStarterObject.AddModel(new SandboxBattleMoraleModel());
				gameStarterObject.AddModel(new SandboxBattleInitializationModel());
				gameStarterObject.AddModel(new SandboxBattleSpawnModel());
				gameStarterObject.AddModel(new DefaultDamageParticleModel());
				gameStarterObject.AddModel(new DefaultItemPickupModel());
				CampaignGameStarter campaignGameStarter = gameStarterObject as CampaignGameStarter;
				if (campaignGameStarter != null)
				{
					campaignGameStarter.AddBehavior(new LordConversationsCampaignBehavior());
					campaignGameStarter.AddBehavior(new HideoutConversationsCampaignBehavior());
					campaignGameStarter.AddBehavior(new AlleyCampaignBehavior());
					campaignGameStarter.AddBehavior(new CommonTownsfolkCampaignBehavior());
					campaignGameStarter.AddBehavior(new CompanionRolesCampaignBehavior());
					campaignGameStarter.AddBehavior(new DefaultNotificationsCampaignBehavior());
					campaignGameStarter.AddBehavior(new ClanMemberRolesCampaignBehavior());
					campaignGameStarter.AddBehavior(new GuardsCampaignBehavior());
					campaignGameStarter.AddBehavior(new SettlementMusiciansCampaignBehavior());
					campaignGameStarter.AddBehavior(new BoardGameCampaignBehavior());
					campaignGameStarter.AddBehavior(new WorkshopsCharactersCampaignBehavior());
					campaignGameStarter.AddBehavior(new TradersCampaignBehavior());
					campaignGameStarter.AddBehavior(new ArenaMasterCampaignBehavior());
					campaignGameStarter.AddBehavior(new CommonVillagersCampaignBehavior());
					campaignGameStarter.AddBehavior(new HeirSelectionCampaignBehavior());
					campaignGameStarter.AddBehavior(new DefaultCutscenesCampaignBehavior());
					campaignGameStarter.AddBehavior(new RivalGangMovingInIssueBehavior());
					campaignGameStarter.AddBehavior(new RuralNotableInnAndOutIssueBehavior());
					campaignGameStarter.AddBehavior(new FamilyFeudIssueBehavior());
					campaignGameStarter.AddBehavior(new NotableWantsDaughterFoundIssueBehavior());
					campaignGameStarter.AddBehavior(new TheSpyPartyIssueQuestBehavior());
					campaignGameStarter.AddBehavior(new ProdigalSonIssueBehavior());
					campaignGameStarter.AddBehavior(new BarberCampaignBehavior());
					campaignGameStarter.AddBehavior(new SnareTheWealthyIssueBehavior());
					campaignGameStarter.AddBehavior(new ControllerEffectsCampaignBehavior());
					campaignGameStarter.AddBehavior(new RetirementCampaignBehavior());
					campaignGameStarter.AddBehavior(new StatisticsCampaignBehavior());
				}
			}
		}

		// Token: 0x060000DF RID: 223 RVA: 0x00007310 File Offset: 0x00005510
		public override void OnCampaignStart(Game game, object starterObject)
		{
			Campaign campaign = game.GameType as Campaign;
			if (campaign != null)
			{
				SandBoxManager sandBoxManager = campaign.SandBoxManager;
				sandBoxManager.SandBoxMissionManager = new SandBoxMissionManager();
				sandBoxManager.AgentBehaviorManager = new AgentBehaviorManager();
				sandBoxManager.ModuleManager = new ModuleManager();
				sandBoxManager.SandBoxSaveManager = new SandBoxSaveManager();
			}
		}

		// Token: 0x060000E0 RID: 224 RVA: 0x00007360 File Offset: 0x00005560
		private void OnRegisterTypes()
		{
			MBObjectManager.Instance.RegisterType<InstrumentData>("MusicInstrument", "MusicInstruments", 54U, true, false);
			MBObjectManager.Instance.RegisterType<SettlementMusicData>("MusicTrack", "MusicTracks", 55U, true, false);
			new DefaultMusicInstrumentData();
			MBObjectManagerExtensions.LoadXML(MBObjectManager.Instance, "MusicInstruments", false);
			MBObjectManagerExtensions.LoadXML(MBObjectManager.Instance, "MusicTracks", false);
		}

		// Token: 0x060000E1 RID: 225 RVA: 0x000073C4 File Offset: 0x000055C4
		public override void OnGameInitializationFinished(Game game)
		{
			Campaign campaign = game.GameType as Campaign;
			if (campaign != null)
			{
				campaign.CampaignMissionManager = new CampaignMissionManager();
				campaign.MapSceneCreator = new MapSceneCreator();
				campaign.EncyclopediaManager.CreateEncyclopediaPages();
				this.OnRegisterTypes();
			}
		}

		// Token: 0x060000E2 RID: 226 RVA: 0x00007407 File Offset: 0x00005607
		public override void RegisterSubModuleObjects(bool isSavedCampaign)
		{
			Campaign.Current.SandBoxManager.InitializeSandboxXMLs(isSavedCampaign);
		}

		// Token: 0x060000E3 RID: 227 RVA: 0x00007419 File Offset: 0x00005619
		public override void AfterRegisterSubModuleObjects(bool isSavedCampaign)
		{
			Campaign.Current.SandBoxManager.InitializeCharactersAfterLoad(isSavedCampaign);
		}

		// Token: 0x060000E4 RID: 228 RVA: 0x0000742C File Offset: 0x0000562C
		public override void OnInitialState()
		{
			base.OnInitialState();
			if (Module.CurrentModule.StartupInfo.IsContinueGame && !this._latestSaveLoaded)
			{
				this._latestSaveLoaded = true;
				SaveGameFileInfo[] saveFiles = MBSaveLoad.GetSaveFiles(null);
				if (Extensions.IsEmpty<SaveGameFileInfo>(saveFiles))
				{
					return;
				}
				SandBoxSaveHelper.TryLoadSave(Extensions.MaxBy<SaveGameFileInfo, DateTime>(saveFiles, (SaveGameFileInfo s) => MetaDataExtensions.GetCreationTime(s.MetaData)), new Action<LoadResult>(this.StartGame), null);
			}
		}

		// Token: 0x060000E5 RID: 229 RVA: 0x000074A6 File Offset: 0x000056A6
		private void StartGame(LoadResult loadResult)
		{
			MBGameManager.StartNewGame(new SandBoxGameManager(loadResult));
			MouseManager.ShowCursor(false);
		}

		// Token: 0x060000E6 RID: 230 RVA: 0x000074BC File Offset: 0x000056BC
		public override void OnGameLoaded(Game game, object initializerObject)
		{
			Campaign campaign = game.GameType as Campaign;
			if (campaign != null)
			{
				SandBoxManager sandBoxManager = campaign.SandBoxManager;
				sandBoxManager.SandBoxMissionManager = new SandBoxMissionManager();
				sandBoxManager.AgentBehaviorManager = new AgentBehaviorManager();
				sandBoxManager.ModuleManager = new ModuleManager();
				sandBoxManager.SandBoxSaveManager = new SandBoxSaveManager();
			}
		}

		// Token: 0x060000E7 RID: 231 RVA: 0x00007509 File Offset: 0x00005709
		protected override void OnBeforeInitialModuleScreenSetAsRoot()
		{
			base.OnBeforeInitialModuleScreenSetAsRoot();
			if (!this._initialized)
			{
				MBSaveLoad.Initialize(Module.CurrentModule.GlobalTextManager);
				this._initialized = true;
			}
		}

		// Token: 0x04000052 RID: 82
		private bool _initialized;

		// Token: 0x04000053 RID: 83
		private bool _latestSaveLoaded;
	}
}
