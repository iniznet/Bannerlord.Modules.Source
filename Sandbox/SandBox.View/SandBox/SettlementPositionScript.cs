using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace SandBox
{
	public class SettlementPositionScript : ScriptComponentBehavior
	{
		private string SettlementsXmlPath
		{
			get
			{
				string text = base.Scene.GetModulePath();
				text = text.Remove(0, 6);
				return BasePath.Name + text + "ModuleData/settlements.xml";
			}
		}

		private string SettlementsDistanceCacheFilePath
		{
			get
			{
				string text = base.Scene.GetModulePath();
				text = text.Remove(0, 6);
				return BasePath.Name + text + "ModuleData/settlements_distance_cache.bin";
			}
		}

		protected override void OnEditorVariableChanged(string variableName)
		{
			base.OnEditorVariableChanged(variableName);
			if (variableName == "SavePositions")
			{
				this.SaveSettlementPositions();
			}
			if (variableName == "ComputeAndSaveSettlementDistanceCache")
			{
				this.SaveSettlementDistanceCache();
			}
			if (variableName == "CheckPositions")
			{
				this.CheckSettlementPositions();
			}
		}

		protected override void OnSceneSave(string saveFolder)
		{
			base.OnSceneSave(saveFolder);
			this.SaveSettlementPositions();
		}

		private void CheckSettlementPositions()
		{
			XmlDocument xmlDocument = this.LoadXmlFile(this.SettlementsXmlPath);
			base.GameEntity.RemoveAllChildren();
			foreach (object obj in xmlDocument.DocumentElement.SelectNodes("Settlement"))
			{
				string value = ((XmlNode)obj).Attributes["id"].Value;
				GameEntity campaignEntityWithName = base.Scene.GetCampaignEntityWithName(value);
				Vec3 origin = campaignEntityWithName.GetGlobalFrame().origin;
				Vec3 vec = default(Vec3);
				List<GameEntity> list = new List<GameEntity>();
				campaignEntityWithName.GetChildrenRecursive(ref list);
				bool flag = false;
				foreach (GameEntity gameEntity in list)
				{
					if (gameEntity.HasTag("main_map_city_gate"))
					{
						vec = gameEntity.GetGlobalFrame().origin;
						flag = true;
						break;
					}
				}
				Vec3 vec2 = origin;
				if (flag)
				{
					vec2 = vec;
				}
				PathFaceRecord pathFaceRecord;
				pathFaceRecord..ctor(-1, -1, -1);
				base.GameEntity.Scene.GetNavMeshFaceIndex(ref pathFaceRecord, vec2.AsVec2, true, false);
				int num = 0;
				if (pathFaceRecord.IsValid())
				{
					num = pathFaceRecord.FaceGroupIndex;
				}
				if (num == 0 || num == 7 || num == 8 || num == 10 || num == 11 || num == 13 || num == 14)
				{
					MBEditor.ZoomToPosition(vec2);
					break;
				}
			}
		}

		protected override void OnInit()
		{
			try
			{
				Debug.Print("SettlementsDistanceCacheFilePath: " + this.SettlementsDistanceCacheFilePath, 0, 12, 17592186044416UL);
				BinaryReader binaryReader = new BinaryReader(File.Open(this.SettlementsDistanceCacheFilePath, FileMode.Open, FileAccess.Read));
				if (Campaign.Current.Models.MapDistanceModel is DefaultMapDistanceModel)
				{
					((DefaultMapDistanceModel)Campaign.Current.Models.MapDistanceModel).LoadCacheFromFile(binaryReader);
				}
				binaryReader.Close();
			}
			catch
			{
				Debug.FailedAssert("SettlementsDistanceCacheFilePath could not be read!. Campaign performance will be affected very badly.", "C:\\Develop\\MB3\\Source\\Bannerlord\\SandBox.View\\Map\\SettlementPositionScript.cs", "OnInit", 165);
				Debug.Print("SettlementsDistanceCacheFilePath could not be read!. Campaign performance will be affected very badly.", 0, 12, 17592186044416UL);
			}
		}

		private List<SettlementPositionScript.SettlementRecord> LoadSettlementData(XmlDocument settlementDocument)
		{
			List<SettlementPositionScript.SettlementRecord> list = new List<SettlementPositionScript.SettlementRecord>();
			base.GameEntity.RemoveAllChildren();
			foreach (object obj in settlementDocument.DocumentElement.SelectNodes("Settlement"))
			{
				XmlNode xmlNode = (XmlNode)obj;
				string value = xmlNode.Attributes["name"].Value;
				string value2 = xmlNode.Attributes["id"].Value;
				GameEntity campaignEntityWithName = base.Scene.GetCampaignEntityWithName(value2);
				if (!(campaignEntityWithName == null))
				{
					Vec2 asVec = campaignEntityWithName.GetGlobalFrame().origin.AsVec2;
					Vec2 vec = default(Vec2);
					List<GameEntity> list2 = new List<GameEntity>();
					campaignEntityWithName.GetChildrenRecursive(ref list2);
					bool flag = false;
					foreach (GameEntity gameEntity in list2)
					{
						if (gameEntity.HasTag("main_map_city_gate"))
						{
							vec = gameEntity.GetGlobalFrame().origin.AsVec2;
							flag = true;
						}
					}
					list.Add(new SettlementPositionScript.SettlementRecord(value, value2, asVec, flag ? vec : asVec, xmlNode, flag));
				}
			}
			return list;
		}

		private void SaveSettlementPositions()
		{
			XmlDocument xmlDocument = this.LoadXmlFile(this.SettlementsXmlPath);
			foreach (SettlementPositionScript.SettlementRecord settlementRecord in this.LoadSettlementData(xmlDocument))
			{
				if (settlementRecord.Node.Attributes["posX"] == null)
				{
					XmlAttribute xmlAttribute = xmlDocument.CreateAttribute("posX");
					settlementRecord.Node.Attributes.Append(xmlAttribute);
				}
				settlementRecord.Node.Attributes["posX"].Value = settlementRecord.Position.X.ToString();
				if (settlementRecord.Node.Attributes["posY"] == null)
				{
					XmlAttribute xmlAttribute2 = xmlDocument.CreateAttribute("posY");
					settlementRecord.Node.Attributes.Append(xmlAttribute2);
				}
				settlementRecord.Node.Attributes["posY"].Value = settlementRecord.Position.Y.ToString();
				if (settlementRecord.HasGate)
				{
					if (settlementRecord.Node.Attributes["gate_posX"] == null)
					{
						XmlAttribute xmlAttribute3 = xmlDocument.CreateAttribute("gate_posX");
						settlementRecord.Node.Attributes.Append(xmlAttribute3);
					}
					settlementRecord.Node.Attributes["gate_posX"].Value = settlementRecord.GatePosition.X.ToString();
					if (settlementRecord.Node.Attributes["gate_posY"] == null)
					{
						XmlAttribute xmlAttribute4 = xmlDocument.CreateAttribute("gate_posY");
						settlementRecord.Node.Attributes.Append(xmlAttribute4);
					}
					settlementRecord.Node.Attributes["gate_posY"].Value = settlementRecord.GatePosition.Y.ToString();
				}
			}
			xmlDocument.Save(this.SettlementsXmlPath);
		}

		private void SaveSettlementDistanceCache()
		{
			BinaryWriter binaryWriter = null;
			try
			{
				XmlDocument xmlDocument = this.LoadXmlFile(this.SettlementsXmlPath);
				List<SettlementPositionScript.SettlementRecord> list = this.LoadSettlementData(xmlDocument);
				int navigationMeshIndexOfTerrainType = MapScene.GetNavigationMeshIndexOfTerrainType(1);
				int navigationMeshIndexOfTerrainType2 = MapScene.GetNavigationMeshIndexOfTerrainType(12);
				int navigationMeshIndexOfTerrainType3 = MapScene.GetNavigationMeshIndexOfTerrainType(0);
				int navigationMeshIndexOfTerrainType4 = MapScene.GetNavigationMeshIndexOfTerrainType(9);
				int navigationMeshIndexOfTerrainType5 = MapScene.GetNavigationMeshIndexOfTerrainType(13);
				int navigationMeshIndexOfTerrainType6 = MapScene.GetNavigationMeshIndexOfTerrainType(14);
				base.Scene.SetAbilityOfFacesWithId(navigationMeshIndexOfTerrainType, false);
				base.Scene.SetAbilityOfFacesWithId(navigationMeshIndexOfTerrainType2, false);
				base.Scene.SetAbilityOfFacesWithId(navigationMeshIndexOfTerrainType3, false);
				base.Scene.SetAbilityOfFacesWithId(navigationMeshIndexOfTerrainType4, false);
				base.Scene.SetAbilityOfFacesWithId(navigationMeshIndexOfTerrainType5, false);
				base.Scene.SetAbilityOfFacesWithId(navigationMeshIndexOfTerrainType6, false);
				binaryWriter = new BinaryWriter(File.Open(this.SettlementsDistanceCacheFilePath, FileMode.Create));
				binaryWriter.Write(list.Count);
				for (int i = 0; i < list.Count; i++)
				{
					binaryWriter.Write(list[i].SettlementId);
					Vec2 gatePosition = list[i].GatePosition;
					PathFaceRecord pathFaceRecord;
					pathFaceRecord..ctor(-1, -1, -1);
					base.Scene.GetNavMeshFaceIndex(ref pathFaceRecord, gatePosition, false, false);
					for (int j = i + 1; j < list.Count; j++)
					{
						binaryWriter.Write(list[j].SettlementId);
						Vec2 gatePosition2 = list[j].GatePosition;
						PathFaceRecord pathFaceRecord2;
						pathFaceRecord2..ctor(-1, -1, -1);
						base.Scene.GetNavMeshFaceIndex(ref pathFaceRecord2, gatePosition2, false, false);
						float num;
						base.Scene.GetPathDistanceBetweenAIFaces(pathFaceRecord.FaceIndex, pathFaceRecord2.FaceIndex, gatePosition, gatePosition2, 0.1f, float.MaxValue, ref num);
						binaryWriter.Write(num);
					}
				}
				int navMeshFaceCount = base.Scene.GetNavMeshFaceCount();
				for (int k = 0; k < navMeshFaceCount; k++)
				{
					int idOfNavMeshFace = base.Scene.GetIdOfNavMeshFace(k);
					if (idOfNavMeshFace != navigationMeshIndexOfTerrainType && idOfNavMeshFace != navigationMeshIndexOfTerrainType2 && idOfNavMeshFace != navigationMeshIndexOfTerrainType3 && idOfNavMeshFace != navigationMeshIndexOfTerrainType4 && idOfNavMeshFace != navigationMeshIndexOfTerrainType5 && idOfNavMeshFace != navigationMeshIndexOfTerrainType6)
					{
						Vec3 zero = Vec3.Zero;
						base.Scene.GetNavMeshCenterPosition(k, ref zero);
						Vec2 asVec = zero.AsVec2;
						float num2 = float.MaxValue;
						string text = "";
						for (int l = 0; l < list.Count; l++)
						{
							Vec2 gatePosition3 = list[l].GatePosition;
							PathFaceRecord pathFaceRecord3;
							pathFaceRecord3..ctor(-1, -1, -1);
							base.Scene.GetNavMeshFaceIndex(ref pathFaceRecord3, gatePosition3, false, false);
							float num3;
							if ((num2 == 3.4028235E+38f || asVec.DistanceSquared(gatePosition3) < num2 * num2) && base.Scene.GetPathDistanceBetweenAIFaces(k, pathFaceRecord3.FaceIndex, asVec, gatePosition3, 0.1f, num2, ref num3) && num3 < num2)
							{
								num2 = num3;
								text = list[l].SettlementId;
							}
						}
						if (!string.IsNullOrEmpty(text))
						{
							binaryWriter.Write(k);
							binaryWriter.Write(text);
						}
					}
				}
				binaryWriter.Write(-1);
			}
			catch
			{
			}
			finally
			{
				base.Scene.SetAbilityOfFacesWithId(MapScene.GetNavigationMeshIndexOfTerrainType(1), true);
				base.Scene.SetAbilityOfFacesWithId(MapScene.GetNavigationMeshIndexOfTerrainType(12), true);
				base.Scene.SetAbilityOfFacesWithId(MapScene.GetNavigationMeshIndexOfTerrainType(0), true);
				base.Scene.SetAbilityOfFacesWithId(MapScene.GetNavigationMeshIndexOfTerrainType(9), true);
				base.Scene.SetAbilityOfFacesWithId(MapScene.GetNavigationMeshIndexOfTerrainType(13), true);
				base.Scene.SetAbilityOfFacesWithId(MapScene.GetNavigationMeshIndexOfTerrainType(14), true);
				if (binaryWriter != null)
				{
					binaryWriter.Close();
				}
			}
		}

		private XmlDocument LoadXmlFile(string path)
		{
			Debug.Print("opening " + path, 0, 12, 17592186044416UL);
			XmlDocument xmlDocument = new XmlDocument();
			StreamReader streamReader = new StreamReader(path);
			string text = streamReader.ReadToEnd();
			xmlDocument.LoadXml(text);
			streamReader.Close();
			return xmlDocument;
		}

		protected override bool IsOnlyVisual()
		{
			return true;
		}

		public SimpleButton CheckPositions;

		public SimpleButton SavePositions;

		public SimpleButton ComputeAndSaveSettlementDistanceCache;

		private struct SettlementRecord
		{
			public SettlementRecord(string settlementName, string settlementId, Vec2 position, Vec2 gatePosition, XmlNode node, bool hasGate)
			{
				this.SettlementName = settlementName;
				this.SettlementId = settlementId;
				this.Position = position;
				this.GatePosition = gatePosition;
				this.Node = node;
				this.HasGate = hasGate;
			}

			public readonly string SettlementName;

			public readonly string SettlementId;

			public readonly XmlNode Node;

			public readonly Vec2 Position;

			public readonly Vec2 GatePosition;

			public readonly bool HasGate;
		}
	}
}
