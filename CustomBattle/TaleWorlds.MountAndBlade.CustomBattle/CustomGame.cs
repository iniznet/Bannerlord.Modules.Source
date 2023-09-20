using System;
using System.Collections.Generic;
using System.Xml;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.CustomBattle.CustomBattleObjects;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.MountAndBlade.CustomBattle
{
	public class CustomGame : GameType
	{
		public IEnumerable<CustomBattleSceneData> CustomBattleScenes
		{
			get
			{
				return this._customBattleScenes;
			}
		}

		public override bool IsCoreOnlyGameMode
		{
			get
			{
				return true;
			}
		}

		public CustomBattleBannerEffects CustomBattleBannerEffects { get; private set; }

		public static CustomGame Current
		{
			get
			{
				return Game.Current.GameType as CustomGame;
			}
		}

		public CustomGame()
		{
			this._customBattleScenes = new List<CustomBattleSceneData>();
		}

		protected override void OnInitialize()
		{
			this.InitializeScenes();
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
			basicGameStarter.AddModel(new CustomBattleAutoBlockModel());
			basicGameStarter.AddModel(new CustomBattleMoraleModel());
			basicGameStarter.AddModel(new CustomBattleInitializationModel());
			basicGameStarter.AddModel(new CustomBattleSpawnModel());
			basicGameStarter.AddModel(new DefaultAgentDecideKilledOrUnconsciousModel());
			basicGameStarter.AddModel(new DefaultMissionDifficultyModel());
			basicGameStarter.AddModel(new DefaultRidingModel());
			basicGameStarter.AddModel(new DefaultStrikeMagnitudeModel());
			basicGameStarter.AddModel(new CustomBattleBannerBearersModel());
			basicGameStarter.AddModel(new DefaultFormationArrangementModel());
			basicGameStarter.AddModel(new DefaultDamageParticleModel());
			basicGameStarter.AddModel(new DefaultItemPickupModel());
			basicGameStarter.AddModel(new DefaultItemValueModel());
		}

		private void InitializeScenes()
		{
			XmlDocument mergedXmlForManaged = MBObjectManager.GetMergedXmlForManaged("Scene", true, true, "");
			this.LoadCustomBattleScenes(mergedXmlForManaged);
		}

		private void LoadCustomGameXmls()
		{
			this.CustomBattleBannerEffects = new CustomBattleBannerEffects();
			MBObjectManagerExtensions.LoadXML(base.ObjectManager, "Items", false);
			MBObjectManagerExtensions.LoadXML(base.ObjectManager, "EquipmentRosters", false);
			MBObjectManagerExtensions.LoadXML(base.ObjectManager, "NPCCharacters", false);
			MBObjectManagerExtensions.LoadXML(base.ObjectManager, "SPCultures", false);
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
		}

		private void LoadCustomBattleScenes(XmlDocument doc)
		{
			if (doc.ChildNodes.Count == 0)
			{
				throw new TWXmlLoadException("Incorrect XML document format. XML document has no nodes.");
			}
			bool flag = doc.ChildNodes[0].Name.ToLower().Equals("xml");
			if (flag && doc.ChildNodes.Count == 1)
			{
				throw new TWXmlLoadException("Incorrect XML document format. XML document must have at least one child node");
			}
			XmlNode xmlNode = (flag ? doc.ChildNodes[1] : doc.ChildNodes[0]);
			if (xmlNode.Name != "CustomBattleScenes")
			{
				throw new TWXmlLoadException("Incorrect XML document format. Root node's name must be CustomBattleScenes.");
			}
			if (xmlNode.Name == "CustomBattleScenes")
			{
				foreach (object obj in xmlNode.ChildNodes)
				{
					XmlNode xmlNode2 = (XmlNode)obj;
					if (xmlNode2.NodeType != XmlNodeType.Comment)
					{
						string text = null;
						TextObject textObject = null;
						TerrainType terrainType = 4;
						ForestDensity forestDensity = 0;
						bool flag2 = false;
						bool flag3 = false;
						bool flag4 = false;
						for (int i = 0; i < xmlNode2.Attributes.Count; i++)
						{
							if (xmlNode2.Attributes[i].Name == "id")
							{
								text = xmlNode2.Attributes[i].InnerText;
							}
							else if (xmlNode2.Attributes[i].Name == "name")
							{
								textObject = new TextObject(xmlNode2.Attributes[i].InnerText, null);
							}
							else if (xmlNode2.Attributes[i].Name == "terrain")
							{
								if (!Enum.TryParse<TerrainType>(xmlNode2.Attributes[i].InnerText, out terrainType))
								{
									terrainType = 4;
								}
							}
							else if (xmlNode2.Attributes[i].Name == "forest_density")
							{
								char[] array = xmlNode2.Attributes[i].InnerText.ToLower().ToCharArray();
								array[0] = char.ToUpper(array[0]);
								if (!Enum.TryParse<ForestDensity>(new string(array), out forestDensity))
								{
									forestDensity = 0;
								}
							}
							else if (xmlNode2.Attributes[i].Name == "is_siege_map")
							{
								bool.TryParse(xmlNode2.Attributes[i].InnerText, out flag2);
							}
							else if (xmlNode2.Attributes[i].Name == "is_village_map")
							{
								bool.TryParse(xmlNode2.Attributes[i].InnerText, out flag3);
							}
							else if (xmlNode2.Attributes[i].Name == "is_lords_hall_map")
							{
								bool.TryParse(xmlNode2.Attributes[i].InnerText, out flag4);
							}
						}
						XmlNodeList childNodes = xmlNode2.ChildNodes;
						List<TerrainType> list = new List<TerrainType>();
						foreach (object obj2 in childNodes)
						{
							XmlNode xmlNode3 = (XmlNode)obj2;
							if (xmlNode3.NodeType != XmlNodeType.Comment && xmlNode3.Name == "flags")
							{
								foreach (object obj3 in xmlNode3.ChildNodes)
								{
									XmlNode xmlNode4 = (XmlNode)obj3;
									TerrainType terrainType2;
									if (xmlNode4.NodeType != XmlNodeType.Comment && xmlNode4.Attributes["name"].InnerText == "TerrainType" && Enum.TryParse<TerrainType>(xmlNode4.Attributes["value"].InnerText, out terrainType2) && !list.Contains(terrainType2))
									{
										list.Add(terrainType2);
									}
								}
							}
						}
						this._customBattleScenes.Add(new CustomBattleSceneData(text, textObject, terrainType, list, forestDensity, flag2, flag3, flag4));
					}
				}
			}
		}

		public override void OnStateChanged(GameState oldState)
		{
		}

		private List<CustomBattleSceneData> _customBattleScenes;

		private const TerrainType DefaultTerrain = 4;

		private const ForestDensity DefaultForestDensity = 0;
	}
}
