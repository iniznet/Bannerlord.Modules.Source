using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using TaleWorlds.Engine;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;

namespace SandBox
{
	public class SettlementVisualizer : ScriptComponentBehavior
	{
		private void CheckNavMeshAux()
		{
			if (this._settlementDatas != null)
			{
				foreach (SettlementVisualizer.SettlementInstance settlementInstance in this._settlementDatas)
				{
					MatrixFrame globalFrame = settlementInstance.ChildEntity.GetGlobalFrame();
					PathFaceRecord nullFaceRecord = PathFaceRecord.NullFaceRecord;
					base.GameEntity.Scene.GetNavMeshFaceIndex(ref nullFaceRecord, globalFrame.origin.AsVec2, false, false);
					if (nullFaceRecord.FaceIndex == -1)
					{
						Debug.Print("Settlement(" + settlementInstance.SettlementName + ") has no nav mesh under", 0, 12, 17592186044416UL);
					}
				}
			}
		}

		private void SnapToTerrainAux()
		{
			foreach (SettlementVisualizer.SettlementInstance settlementInstance in this._settlementDatas)
			{
				MatrixFrame globalFrame = settlementInstance.ChildEntity.GetGlobalFrame();
				float num = 0f;
				settlementInstance.ChildEntity.Scene.GetHeightAtPoint(globalFrame.origin.AsVec2, 2208137, ref num);
				globalFrame.origin.z = num;
				settlementInstance.ChildEntity.SetGlobalFrame(ref globalFrame);
				settlementInstance.ChildEntity.UpdateTriadFrameForEditor();
			}
		}

		protected override void OnEditorVariableChanged(string variableName)
		{
			base.OnEditorVariableChanged(variableName);
			if (variableName == "ReloadXML")
			{
				this.LoadFromXml();
				return;
			}
			if (variableName == "SnapToTerrain")
			{
				this.SnapToTerrainAux();
				return;
			}
			if (variableName == "translateScale")
			{
				this.RepositionAfterScale();
				return;
			}
			if (variableName == "CheckNavMesh")
			{
				this.CheckNavMeshAux();
			}
		}

		private void RepositionAfterScale()
		{
			foreach (SettlementVisualizer.SettlementInstance settlementInstance in this._settlementDatas)
			{
				MatrixFrame globalFrame = settlementInstance.ChildEntity.GetGlobalFrame();
				Vec2 vec = settlementInstance.OriginalPosition * this.translateScale;
				globalFrame.origin.x = vec.x;
				globalFrame.origin.y = vec.y;
				float num = 0f;
				settlementInstance.ChildEntity.Scene.GetHeightAtPoint(globalFrame.origin.AsVec2, 2208137, ref num);
				globalFrame.origin.z = num;
				settlementInstance.ChildEntity.SetGlobalFrame(ref globalFrame);
				settlementInstance.ChildEntity.UpdateTriadFrameForEditor();
			}
		}

		private void LoadFromXml()
		{
			this._settlementDatas = new List<SettlementVisualizer.SettlementInstance>();
			this._doc = this.LoadXmlFile(BasePath.Name + "/Modules/SandBox/ModuleData/settlements.xml");
			base.GameEntity.RemoveAllChildren();
			foreach (object obj in this._doc.DocumentElement.SelectNodes("Settlement"))
			{
				XmlNode xmlNode = (XmlNode)obj;
				if (xmlNode.Attributes["posX"] != null && xmlNode.Attributes["posY"] != null)
				{
					GameEntity gameEntity = GameEntity.CreateEmpty(base.GameEntity.Scene, true);
					MatrixFrame globalFrame = gameEntity.GetGlobalFrame();
					Vec2 vec;
					vec..ctor((float)Convert.ToDouble(xmlNode.Attributes["posX"].Value), (float)Convert.ToDouble(xmlNode.Attributes["posY"].Value));
					string value = xmlNode.Attributes["name"].Value;
					gameEntity.Name = value;
					float num = 0f;
					base.GameEntity.Scene.GetHeightAtPoint(vec, 2208137, ref num);
					globalFrame.origin = new Vec3(vec, num, -1f);
					if (xmlNode.Attributes["culture"] != null)
					{
						string value2 = xmlNode.Attributes["culture"].Value;
						value2.Substring(value2.IndexOf('.') + 1);
						MetaMesh metaMesh = null;
						gameEntity.SetGlobalFrame(ref globalFrame);
						gameEntity.EntityFlags |= 131072;
						base.GameEntity.AddChild(gameEntity, true);
						gameEntity.GetGlobalFrame();
						gameEntity.UpdateTriadFrameForEditor();
						this._settlementDatas.Add(new SettlementVisualizer.SettlementInstance(gameEntity, xmlNode, value, vec));
						if (metaMesh != null)
						{
							gameEntity.AddMultiMesh(metaMesh, true);
						}
						else
						{
							gameEntity.AddMultiMesh(MetaMesh.GetCopy("map_icon_bandit_hideout_a", true, false), true);
						}
					}
					else
					{
						gameEntity.AddMultiMesh(MetaMesh.GetCopy("map_icon_bandit_hideout_a", true, false), true);
					}
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

		protected override void OnEditorTick(float dt)
		{
			if (Input.IsKeyDown(56) && Input.IsKeyDown(29) && Input.IsKeyPressed(30))
			{
				this.SnapToTerrainAux();
			}
			if (this.renderSettlementName && this._settlementDatas != null)
			{
				foreach (SettlementVisualizer.SettlementInstance settlementInstance in this._settlementDatas)
				{
					ref MatrixFrame ptr = ref settlementInstance.ChildEntity.GetGlobalFrame();
					ptr.origin.z = ptr.origin.z + 1.5f;
				}
			}
		}

		protected override bool IsOnlyVisual()
		{
			return true;
		}

		public SimpleButton ReloadXML;

		public SimpleButton SaveXML;

		public SimpleButton SnapToTerrain;

		public SimpleButton CheckNavMesh;

		public bool renderSettlementName;

		public float translateScale = 1f;

		private XmlDocument _doc;

		private List<SettlementVisualizer.SettlementInstance> _settlementDatas;

		private const string settlemensXmlPath = "/Modules/SandBox/ModuleData/settlements.xml";

		private class SettlementInstance
		{
			public SettlementInstance(GameEntity childEntity, XmlNode node, string settlementName, Vec2 originalPosition)
			{
				this.ChildEntity = childEntity;
				this.Node = node;
				this.SettlementName = settlementName;
				this.OriginalPosition = originalPosition;
			}

			public GameEntity ChildEntity;

			public string SettlementName;

			public XmlNode Node;

			public Vec2 OriginalPosition;
		}
	}
}
