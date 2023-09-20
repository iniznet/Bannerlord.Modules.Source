using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace SandBox
{
	// Token: 0x020000CA RID: 202
	public class MultiplayerItemTestMissionController : MissionLogic
	{
		// Token: 0x06000850 RID: 2128 RVA: 0x0000ED68 File Offset: 0x0000CF68
		public MultiplayerItemTestMissionController(BasicCultureObject culture)
		{
			this._culture = culture;
			if (!MultiplayerItemTestMissionController._initializeFlag)
			{
				Game.Current.ObjectManager.LoadXML("MPCharacters", false);
				MultiplayerItemTestMissionController._initializeFlag = true;
			}
		}

		// Token: 0x06000851 RID: 2129 RVA: 0x0000EDD9 File Offset: 0x0000CFD9
		public override void AfterStart()
		{
			this.GetAllTroops();
			this.SpawnMainAgent();
			this.SpawnMultiplayerTroops();
		}

		// Token: 0x06000852 RID: 2130 RVA: 0x0000EDF0 File Offset: 0x0000CFF0
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

		// Token: 0x06000853 RID: 2131 RVA: 0x0000EED8 File Offset: 0x0000D0D8
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

		// Token: 0x06000854 RID: 2132 RVA: 0x0000EF68 File Offset: 0x0000D168
		private XmlDocument LoadXmlFile(string path)
		{
			Debug.Print("opening " + path, 0, Debug.DebugColor.White, 17592186044416UL);
			XmlDocument xmlDocument = new XmlDocument();
			string text = new StreamReader(path).ReadToEnd();
			xmlDocument.LoadXml(text);
			return xmlDocument;
		}

		// Token: 0x06000855 RID: 2133 RVA: 0x0000EFAC File Offset: 0x0000D1AC
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

		// Token: 0x06000856 RID: 2134 RVA: 0x0000F058 File Offset: 0x0000D258
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

		// Token: 0x040001E7 RID: 487
		private Agent mainAgent;

		// Token: 0x040001E8 RID: 488
		private BasicCultureObject _culture;

		// Token: 0x040001E9 RID: 489
		private List<BasicCharacterObject> _troops = new List<BasicCharacterObject>();

		// Token: 0x040001EA RID: 490
		private const float HorizontalGap = 3f;

		// Token: 0x040001EB RID: 491
		private const float VerticalGap = 3f;

		// Token: 0x040001EC RID: 492
		private Vec3 _coordinate = new Vec3(200f, 200f, 0f, -1f);

		// Token: 0x040001ED RID: 493
		private int _mapHorizontalEndCoordinate = 800;

		// Token: 0x040001EE RID: 494
		private static bool _initializeFlag;
	}
}
