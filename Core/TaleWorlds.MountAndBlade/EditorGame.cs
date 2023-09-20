using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.MountAndBlade
{
	public class EditorGame : GameType
	{
		public static EditorGame Current
		{
			get
			{
				return Game.Current.GameType as EditorGame;
			}
		}

		protected override void OnInitialize()
		{
			Game currentGame = base.CurrentGame;
			IGameStarter gameStarter = new BasicGameStarter();
			this.InitializeGameModels(gameStarter);
			base.GameManager.InitializeGameStarter(currentGame, gameStarter);
			base.GameManager.OnGameStart(base.CurrentGame, gameStarter);
			MBObjectManager objectManager = currentGame.ObjectManager;
			currentGame.SetBasicModels(gameStarter.Models);
			currentGame.CreateGameManager();
			base.GameManager.BeginGameStart(base.CurrentGame);
			currentGame.InitializeDefaultGameObjects();
			currentGame.LoadBasicFiles();
			this.LoadCustomGameXmls();
			objectManager.UnregisterNonReadyObjects();
			currentGame.SetDefaultEquipments(new Dictionary<string, Equipment>());
			objectManager.UnregisterNonReadyObjects();
			base.GameManager.OnNewCampaignStart(base.CurrentGame, null);
			base.GameManager.OnAfterCampaignStart(base.CurrentGame);
			base.GameManager.OnGameInitializationFinished(base.CurrentGame);
		}

		private void InitializeGameModels(IGameStarter basicGameStarter)
		{
			basicGameStarter.AddModel(new CustomBattleAgentStatCalculateModel());
			basicGameStarter.AddModel(new CustomAgentApplyDamageModel());
			basicGameStarter.AddModel(new CustomBattleApplyWeatherEffectsModel());
			basicGameStarter.AddModel(new CustomBattleMoraleModel());
			basicGameStarter.AddModel(new CustomBattleInitializationModel());
			basicGameStarter.AddModel(new CustomBattleSpawnModel());
			basicGameStarter.AddModel(new DefaultAgentDecideKilledOrUnconsciousModel());
			basicGameStarter.AddModel(new DefaultRidingModel());
			basicGameStarter.AddModel(new DefaultStrikeMagnitudeModel());
			basicGameStarter.AddModel(new CustomBattleBannerBearersModel());
			basicGameStarter.AddModel(new DefaultFormationArrangementModel());
			basicGameStarter.AddModel(new DefaultDamageParticleModel());
			basicGameStarter.AddModel(new DefaultItemPickupModel());
		}

		private void LoadCustomGameXmls()
		{
			base.ObjectManager.LoadXML("Items", false);
			base.ObjectManager.LoadXML("EquipmentRosters", false);
			base.ObjectManager.LoadXML("NPCCharacters", false);
			base.ObjectManager.LoadXML("SPCultures", false);
		}

		protected override void BeforeRegisterTypes(MBObjectManager objectManager)
		{
		}

		protected override void OnRegisterTypes(MBObjectManager objectManager)
		{
			objectManager.RegisterType<BasicCharacterObject>("NPCCharacter", "NPCCharacters", 43U, true, false);
			objectManager.RegisterType<BasicCultureObject>("Culture", "SPCultures", 17U, true, false);
		}

		protected override void DoLoadingForGameType(GameTypeLoadingStates gameTypeLoadingState, out GameTypeLoadingStates nextState)
		{
			nextState = GameTypeLoadingStates.None;
			switch (gameTypeLoadingState)
			{
			case GameTypeLoadingStates.InitializeFirstStep:
				base.CurrentGame.Initialize();
				nextState = GameTypeLoadingStates.WaitSecondStep;
				return;
			case GameTypeLoadingStates.WaitSecondStep:
				nextState = GameTypeLoadingStates.LoadVisualsThirdState;
				return;
			case GameTypeLoadingStates.LoadVisualsThirdState:
				nextState = GameTypeLoadingStates.PostInitializeFourthState;
				break;
			case GameTypeLoadingStates.PostInitializeFourthState:
				break;
			default:
				return;
			}
		}

		public override void OnDestroy()
		{
		}

		public override void OnStateChanged(GameState oldState)
		{
		}
	}
}
