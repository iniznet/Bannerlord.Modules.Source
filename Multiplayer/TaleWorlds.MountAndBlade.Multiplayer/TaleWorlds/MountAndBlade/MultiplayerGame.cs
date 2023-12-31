﻿using System;
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
	public class MultiplayerGame : GameType
	{
		public override bool IsCoreOnlyGameMode
		{
			get
			{
				return true;
			}
		}

		public static MultiplayerGame Current
		{
			get
			{
				return Game.Current.GameType as MultiplayerGame;
			}
		}

		public override bool RequiresTutorial
		{
			get
			{
				return false;
			}
		}

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
			MBObjectManagerExtensions.LoadXML(base.ObjectManager, "Items", false);
			MBObjectManagerExtensions.LoadXML(base.ObjectManager, "MPCharacters", false);
			MBObjectManagerExtensions.LoadXML(base.ObjectManager, "BasicCultures", false);
			MBObjectManagerExtensions.LoadXML(base.ObjectManager, "MPClassDivisions", false);
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

		protected override void BeforeRegisterTypes(MBObjectManager objectManager)
		{
		}

		protected override void OnRegisterTypes(MBObjectManager objectManager)
		{
			objectManager.RegisterType<BasicCharacterObject>("NPCCharacter", "MPCharacters", 43U, true, false);
			objectManager.RegisterType<BasicCultureObject>("Culture", "BasicCultures", 17U, true, false);
			objectManager.RegisterType<MultiplayerClassDivisions.MPHeroClass>("MPClassDivision", "MPClassDivisions", 45U, true, false);
		}

		protected override void DoLoadingForGameType(GameTypeLoadingStates gameTypeLoadingState, out GameTypeLoadingStates nextState)
		{
			nextState = -1;
			switch (gameTypeLoadingState)
			{
			case 0:
				base.CurrentGame.Initialize();
				nextState = 1;
				return;
			case 1:
				nextState = 2;
				return;
			case 2:
				nextState = 3;
				break;
			case 3:
				break;
			default:
				return;
			}
		}

		public override void OnDestroy()
		{
			BadgeManager.OnFinalize();
			MultiplayerOptions.Release();
			InformationManager.ClearAllMessages();
			MultiplayerClassDivisions.Release();
			AvatarServices.ClearAvatarCaches();
		}

		public override void OnStateChanged(GameState oldState)
		{
		}
	}
}
