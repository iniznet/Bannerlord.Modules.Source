using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace SandBox
{
	public class MultiplayerItemTestMissionController : MissionLogic
	{
		public MultiplayerItemTestMissionController(BasicCultureObject culture)
		{
			this._culture = culture;
			if (!MultiplayerItemTestMissionController._initializeFlag)
			{
				Game.Current.ObjectManager.LoadXML("MPCharacters", false);
				MultiplayerItemTestMissionController._initializeFlag = true;
			}
		}

		public override void AfterStart()
		{
			this.GetAllTroops();
			this.SpawnMainAgent();
			this.SpawnMultiplayerTroops();
		}

		private void SpawnMultiplayerTroops()
		{
			foreach (BasicCharacterObject basicCharacterObject in this._troops)
			{
				Vec3 vec;
				Vec2 vec2;
				this.GetNextSpawnFrame(out vec, out vec2);
				foreach (Equipment equipment in basicCharacterObject.AllEquipments)
				{
					base.Mission.SpawnAgent(new AgentBuildData(new BasicBattleAgentOrigin(basicCharacterObject)).Equipment(equipment).InitialPosition(vec).InitialDirection(vec2), false);
					vec += new Vec3(0f, 2f, 0f, -1f);
				}
			}
		}

		private void GetNextSpawnFrame(out Vec3 position, out Vec2 direction)
		{
			this._coordinate += new Vec3(3f, 0f, 0f, -1f);
			if (this._coordinate.x > (float)this._mapHorizontalEndCoordinate)
			{
				this._coordinate.x = 3f;
				this._coordinate.y = this._coordinate.y + 3f;
			}
			position = this._coordinate;
			direction = new Vec2(0f, -1f);
		}

		private XmlDocument LoadXmlFile(string path)
		{
			Debug.Print("opening " + path, 0, Debug.DebugColor.White, 17592186044416UL);
			XmlDocument xmlDocument = new XmlDocument();
			string text = new StreamReader(path).ReadToEnd();
			xmlDocument.LoadXml(text);
			return xmlDocument;
		}

		private void SpawnMainAgent()
		{
			if (this.mainAgent == null || this.mainAgent.State != AgentState.Active)
			{
				BasicCharacterObject @object = Game.Current.ObjectManager.GetObject<BasicCharacterObject>("main_hero");
				Mission mission = base.Mission;
				AgentBuildData agentBuildData = new AgentBuildData(new BasicBattleAgentOrigin(@object)).Team(base.Mission.DefenderTeam);
				Vec3 vec = new Vec3(200f + (float)MBRandom.RandomInt(15), 200f + (float)MBRandom.RandomInt(15), 1f, -1f);
				this.mainAgent = mission.SpawnAgent(agentBuildData.InitialPosition(vec).InitialDirection(Vec2.Forward).Controller(Agent.ControllerType.Player), false);
			}
		}

		private void GetAllTroops()
		{
			foreach (object obj in this.LoadXmlFile(BasePath.Name + "/Modules/Native/ModuleData/mpcharacters.xml").DocumentElement.SelectNodes("NPCCharacter"))
			{
				XmlNode xmlNode = (XmlNode)obj;
				XmlAttributeCollection attributes = xmlNode.Attributes;
				if (((attributes != null) ? attributes["occupation"] : null) != null && xmlNode.Attributes["occupation"].InnerText == "Soldier")
				{
					string innerText = xmlNode.Attributes["id"].InnerText;
					BasicCharacterObject @object = Game.Current.ObjectManager.GetObject<BasicCharacterObject>(innerText);
					if (@object != null && @object.Culture == this._culture)
					{
						this._troops.Add(@object);
					}
				}
			}
		}

		private Agent mainAgent;

		private BasicCultureObject _culture;

		private List<BasicCharacterObject> _troops = new List<BasicCharacterObject>();

		private const float HorizontalGap = 3f;

		private const float VerticalGap = 3f;

		private Vec3 _coordinate = new Vec3(200f, 200f, 0f, -1f);

		private int _mapHorizontalEndCoordinate = 800;

		private static bool _initializeFlag;
	}
}
