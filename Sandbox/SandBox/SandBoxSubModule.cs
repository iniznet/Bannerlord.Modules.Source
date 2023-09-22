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
	public class SandBoxSubModule : MBSubModuleBase
	{
		protected override void OnSubModuleLoad()
		{
			base.OnSubModuleLoad();
			Module.CurrentModule.SetEditorMissionTester(new SandBoxEditorMissionTester());
		}

		protected override void InitializeGameStarter(Game game, IGameStarter gameStarterObject)
		{
			if (game.GameType is Campaign)
			{
				DefaultMapWeatherModel defaultMapWeatherModel = gameStarterObject.Models.FirstOrDefault((GameModel model) => model is DefaultMapWeatherModel) as DefaultMapWeatherModel;
				if (defaultMapWeatherModel != null)
				{
					byte[] array = new byte[2097152];
					Utilities.GetSnowAmountData(array);
					defaultMapWeatherModel.InitializeSnowAndRainAmountData(array);
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
					campaignGameStarter.AddBehavior(new RetirementCampaignBehavior());
					campaignGameStarter.AddBehavior(new StatisticsCampaignBehavior());
					campaignGameStarter.AddBehavior(new DumpIntegrityCampaignBehavior());
					campaignGameStarter.AddBehavior(new CaravanConversationsCampaignBehavior());
				}
			}
		}

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

		private void OnRegisterTypes()
		{
			MBObjectManager.Instance.RegisterType<InstrumentData>("MusicInstrument", "MusicInstruments", 54U, true, false);
			MBObjectManager.Instance.RegisterType<SettlementMusicData>("MusicTrack", "MusicTracks", 55U, true, false);
			new DefaultMusicInstrumentData();
			MBObjectManagerExtensions.LoadXML(MBObjectManager.Instance, "MusicInstruments", false);
			MBObjectManagerExtensions.LoadXML(MBObjectManager.Instance, "MusicTracks", false);
		}

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

		public override void RegisterSubModuleObjects(bool isSavedCampaign)
		{
			Campaign.Current.SandBoxManager.InitializeSandboxXMLs(isSavedCampaign);
		}

		public override void AfterRegisterSubModuleObjects(bool isSavedCampaign)
		{
			Campaign.Current.SandBoxManager.InitializeCharactersAfterLoad(isSavedCampaign);
		}

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

		private void StartGame(LoadResult loadResult)
		{
			MBGameManager.StartNewGame(new SandBoxGameManager(loadResult));
			MouseManager.ShowCursor(false);
		}

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

		protected override void OnBeforeInitialModuleScreenSetAsRoot()
		{
			base.OnBeforeInitialModuleScreenSetAsRoot();
			if (!this._initialized)
			{
				MBSaveLoad.Initialize(Module.CurrentModule.GlobalTextManager);
				this._initialized = true;
			}
		}

		public override void OnConfigChanged()
		{
			if (Campaign.Current != null)
			{
				CampaignEventDispatcher.Instance.OnConfigChanged();
			}
		}

		private bool _initialized;

		private bool _latestSaveLoaded;
	}
}
