using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using TaleWorlds.Engine;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;

namespace SandBox
{
	// Token: 0x02000005 RID: 5
	public class SettlementVisualizer : ScriptComponentBehavior
	{
		// Token: 0x0600000F RID: 15 RVA: 0x00002B2C File Offset: 0x00000D2C
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

		// Token: 0x06000010 RID: 16 RVA: 0x00002BE8 File Offset: 0x00000DE8
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

		// Token: 0x06000011 RID: 17 RVA: 0x00002C90 File Offset: 0x00000E90
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

		// Token: 0x06000012 RID: 18 RVA: 0x00002CF4 File Offset: 0x00000EF4
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

		// Token: 0x06000013 RID: 19 RVA: 0x00002DD8 File Offset: 0x00000FD8
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

		// Token: 0x06000014 RID: 20 RVA: 0x00003018 File Offset: 0x00001218
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

		// Token: 0x06000015 RID: 21 RVA: 0x00003064 File Offset: 0x00001264
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

		// Token: 0x06000016 RID: 22 RVA: 0x00003100 File Offset: 0x00001300
		protected override bool IsOnlyVisual()
		{
			return true;
		}

		// Token: 0x04000004 RID: 4
		public SimpleButton ReloadXML;

		// Token: 0x04000005 RID: 5
		public SimpleButton SaveXML;

		// Token: 0x04000006 RID: 6
		public SimpleButton SnapToTerrain;

		// Token: 0x04000007 RID: 7
		public SimpleButton CheckNavMesh;

		// Token: 0x04000008 RID: 8
		public bool renderSettlementName;

		// Token: 0x04000009 RID: 9
		public float translateScale = 1f;

		// Token: 0x0400000A RID: 10
		private XmlDocument _doc;

		// Token: 0x0400000B RID: 11
		private List<SettlementVisualizer.SettlementInstance> _settlementDatas;

		// Token: 0x0400000C RID: 12
		private const string settlemensXmlPath = "/Modules/SandBox/ModuleData/settlements.xml";

		// Token: 0x0200005F RID: 95
		private class SettlementInstance
		{
			// Token: 0x0600040F RID: 1039 RVA: 0x000229A6 File Offset: 0x00020BA6
			public SettlementInstance(GameEntity childEntity, XmlNode node, string settlementName, Vec2 originalPosition)
			{
				this.ChildEntity = childEntity;
				this.Node = node;
				this.SettlementName = settlementName;
				this.OriginalPosition = originalPosition;
			}

			// Token: 0x04000232 RID: 562
			public GameEntity ChildEntity;

			// Token: 0x04000233 RID: 563
			public string SettlementName;

			// Token: 0x04000234 RID: 564
			public XmlNode Node;

			// Token: 0x04000235 RID: 565
			public Vec2 OriginalPosition;
		}
	}
}
