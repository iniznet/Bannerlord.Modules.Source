using System;
using System.Collections.Generic;
using System.Xml;
using TaleWorlds.Avatar.PlayerServices;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.ModuleManager;
using TaleWorlds.MountAndBlade.Diamond.MultiplayerBadges;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020002DA RID: 730
	public class MultiplayerGame : GameType
	{
		// Token: 0x17000744 RID: 1860
		// (get) Token: 0x06002829 RID: 10281 RVA: 0x0009B716 File Offset: 0x00099916
		public override bool IsCoreOnlyGameMode
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000745 RID: 1861
		// (get) Token: 0x0600282A RID: 10282 RVA: 0x0009B719 File Offset: 0x00099919
		public static MultiplayerGame Current
		{
			get
			{
				return Game.Current.GameType as MultiplayerGame;
			}
		}

		// Token: 0x0600282C RID: 10284 RVA: 0x0009B734 File Offset: 0x00099934
		protected override void OnInitialize()
		{
			Game currentGame = base.CurrentGame;
			IGameStarter gameStarter = new BasicGameStarter();
			this.AddGameModels(gameStarter);
			base.GameManager.InitializeGameStarter(currentGame, gameStarter);
			base.GameManager.OnGameStart(base.CurrentGame, gameStarter);
			currentGame.SetBasicModels(gameStarter.Models);
			currentGame.CreateGameManager();
			base.GameManager.BeginGameStart(base.CurrentGame);
			currentGame.InitializeDefaultGameObjects();
			if (!GameNetwork.IsDedicatedServer)
			{
				currentGame.GameTextManager.LoadGameTexts();
			}
			currentGame.LoadBasicFiles();
			base.ObjectManager.LoadXML("Items", false);
			base.ObjectManager.LoadXML("MPCharacters", false);
			base.ObjectManager.LoadXML("BasicCultures", false);
			base.ObjectManager.LoadXML("MPClassDivisions", false);
			base.ObjectManager.UnregisterNonReadyObjects();
			MultiplayerClassDivisions.Initialize();
			BadgeManager.InitializeWithXML(ModuleHelper.GetModuleFullPath("Native") + "ModuleData/mpbadges.xml");
			base.GameManager.OnNewCampaignStart(base.CurrentGame, null);
			base.GameManager.OnAfterCampaignStart(base.CurrentGame);
			base.GameManager.OnGameInitializationFinished(base.CurrentGame);
			base.CurrentGame.AddGameHandler<ChatBox>();
			if (GameNetwork.IsDedicatedServer)
			{
				base.CurrentGame.AddGameHandler<MultiplayerGameLogger>();
			}
		}

		// Token: 0x0600282D RID: 10285 RVA: 0x0009B878 File Offset: 0x00099A78
		private void AddGameModels(IGameStarter basicGameStarter)
		{
			basicGameStarter.AddModel(new MultiplayerRidingModel());
			basicGameStarter.AddModel(new MultiplayerStrikeMagnitudeModel());
			basicGameStarter.AddModel(new MultiplayerAgentStatCalculateModel());
			basicGameStarter.AddModel(new MultiplayerAgentApplyDamageModel());
			basicGameStarter.AddModel(new MultiplayerBattleMoraleModel());
			basicGameStarter.AddModel(new MultiplayerBattleInitializationModel());
			basicGameStarter.AddModel(new MultiplayerBattleSpawnModel());
			basicGameStarter.AddModel(new MultiplayerBattleBannerBearersModel());
			basicGameStarter.AddModel(new DefaultFormationArrangementModel());
			basicGameStarter.AddModel(new DefaultAgentDecideKilledOrUnconsciousModel());
			basicGameStarter.AddModel(new DefaultDamageParticleModel());
			basicGameStarter.AddModel(new DefaultItemPickupModel());
		}

		// Token: 0x0600282E RID: 10286 RVA: 0x0009B90C File Offset: 0x00099B0C
		public static Dictionary<string, Equipment> ReadDefaultEquipments(string defaultEquipmentsPath)
		{
			Dictionary<string, Equipment> dictionary = new Dictionary<string, Equipment>();
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.Load(defaultEquipmentsPath);
			foreach (object obj in xmlDocument.ChildNodes[0].ChildNodes)
			{
				XmlNode xmlNode = (XmlNode)obj;
				if (xmlNode.NodeType == XmlNodeType.Element)
				{
					string value = xmlNode.Attributes["name"].Value;
					Equipment equipment = new Equipment(false);
					equipment.Deserialize(null, xmlNode);
					dictionary.Add(value, equipment);
				}
			}
			return dictionary;
		}

		// Token: 0x0600282F RID: 10287 RVA: 0x0009B9B8 File Offset: 0x00099BB8
		protected override void BeforeRegisterTypes(MBObjectManager objectManager)
		{
		}

		// Token: 0x06002830 RID: 10288 RVA: 0x0009B9BA File Offset: 0x00099BBA
		protected override void OnRegisterTypes(MBObjectManager objectManager)
		{
			objectManager.RegisterType<BasicCharacterObject>("NPCCharacter", "MPCharacters", 43U, true, false);
			objectManager.RegisterType<BasicCultureObject>("Culture", "BasicCultures", 17U, true, false);
			objectManager.RegisterType<MultiplayerClassDivisions.MPHeroClass>("MPClassDivision", "MPClassDivisions", 45U, true, false);
		}

		// Token: 0x06002831 RID: 10289 RVA: 0x0009B9F8 File Offset: 0x00099BF8
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

		// Token: 0x06002832 RID: 10290 RVA: 0x0009BA2A File Offset: 0x00099C2A
		public override void OnDestroy()
		{
			BadgeManager.OnFinalize();
			MultiplayerOptions.Release();
			InformationManager.ClearAllMessages();
			MultiplayerClassDivisions.Release();
			AvatarServices.ClearAvatarCaches();
		}

		// Token: 0x06002833 RID: 10291 RVA: 0x0009BA45 File Offset: 0x00099C45
		public override void OnStateChanged(GameState oldState)
		{
		}
	}
}
