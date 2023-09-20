using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem
{
	// Token: 0x02000084 RID: 132
	public class GameSceneDataManager
	{
		// Token: 0x1700047C RID: 1148
		// (get) Token: 0x0600104B RID: 4171 RVA: 0x00048FDC File Offset: 0x000471DC
		// (set) Token: 0x0600104C RID: 4172 RVA: 0x00048FE3 File Offset: 0x000471E3
		public static GameSceneDataManager Instance { get; private set; }

		// Token: 0x1700047D RID: 1149
		// (get) Token: 0x0600104D RID: 4173 RVA: 0x00048FEB File Offset: 0x000471EB
		public MBReadOnlyList<SingleplayerBattleSceneData> SingleplayerBattleScenes
		{
			get
			{
				return this._singleplayerBattleScenes;
			}
		}

		// Token: 0x1700047E RID: 1150
		// (get) Token: 0x0600104E RID: 4174 RVA: 0x00048FF3 File Offset: 0x000471F3
		public MBReadOnlyList<ConversationSceneData> ConversationScenes
		{
			get
			{
				return this._conversationScenes;
			}
		}

		// Token: 0x1700047F RID: 1151
		// (get) Token: 0x0600104F RID: 4175 RVA: 0x00048FFB File Offset: 0x000471FB
		public MBReadOnlyList<MeetingSceneData> MeetingScenes
		{
			get
			{
				return this._meetingScenes;
			}
		}

		// Token: 0x06001050 RID: 4176 RVA: 0x00049003 File Offset: 0x00047203
		public GameSceneDataManager()
		{
			this._singleplayerBattleScenes = new MBList<SingleplayerBattleSceneData>();
			this._conversationScenes = new MBList<ConversationSceneData>();
			this._meetingScenes = new MBList<MeetingSceneData>();
		}

		// Token: 0x06001051 RID: 4177 RVA: 0x0004902C File Offset: 0x0004722C
		internal static void Initialize()
		{
			GameSceneDataManager.Instance = new GameSceneDataManager();
		}

		// Token: 0x06001052 RID: 4178 RVA: 0x00049038 File Offset: 0x00047238
		internal static void Destroy()
		{
			GameSceneDataManager.Instance = null;
		}

		// Token: 0x06001053 RID: 4179 RVA: 0x00049040 File Offset: 0x00047240
		public void LoadSPBattleScenes(string path)
		{
			XmlDocument xmlDocument = this.LoadXmlFile(path);
			this.LoadSPBattleScenes(xmlDocument);
		}

		// Token: 0x06001054 RID: 4180 RVA: 0x0004905C File Offset: 0x0004725C
		public void LoadConversationScenes(string path)
		{
			XmlDocument xmlDocument = this.LoadXmlFile(path);
			this.LoadConversationScenes(xmlDocument);
		}

		// Token: 0x06001055 RID: 4181 RVA: 0x00049078 File Offset: 0x00047278
		public void LoadMeetingScenes(string path)
		{
			XmlDocument xmlDocument = this.LoadXmlFile(path);
			this.LoadMeetingScenes(xmlDocument);
		}

		// Token: 0x06001056 RID: 4182 RVA: 0x00049094 File Offset: 0x00047294
		private XmlDocument LoadXmlFile(string path)
		{
			Debug.Print("opening " + path, 0, Debug.DebugColor.White, 17592186044416UL);
			XmlDocument xmlDocument = new XmlDocument();
			StreamReader streamReader = new StreamReader(path);
			string text = streamReader.ReadToEnd();
			xmlDocument.LoadXml(text);
			streamReader.Close();
			return xmlDocument;
		}

		// Token: 0x06001057 RID: 4183 RVA: 0x000490E0 File Offset: 0x000472E0
		private void LoadSPBattleScenes(XmlDocument doc)
		{
			this._singleplayerBattleScenes.Clear();
			Debug.Print("loading sp_battles.xml", 0, Debug.DebugColor.White, 17592186044416UL);
			if (doc.ChildNodes.Count <= 1)
			{
				throw new TWXmlLoadException("Incorrect XML document format. XML document must have at least 2 child nodes.");
			}
			XmlNode xmlNode = doc.ChildNodes[1];
			if (xmlNode.Name != "SPBattleScenes")
			{
				throw new TWXmlLoadException("Incorrect XML document format. Root node's name must be SPBattleScenes.");
			}
			if (xmlNode.Name == "SPBattleScenes")
			{
				foreach (object obj in xmlNode.ChildNodes)
				{
					XmlNode xmlNode2 = (XmlNode)obj;
					if (xmlNode2.NodeType != XmlNodeType.Comment)
					{
						string text = null;
						List<int> list = new List<int>();
						TerrainType terrainType = TerrainType.Plain;
						ForestDensity forestDensity = ForestDensity.None;
						for (int i = 0; i < xmlNode2.Attributes.Count; i++)
						{
							if (xmlNode2.Attributes[i].Name == "id")
							{
								text = xmlNode2.Attributes[i].InnerText;
							}
							else if (xmlNode2.Attributes[i].Name == "map_indices")
							{
								foreach (string text2 in xmlNode2.Attributes[i].InnerText.Replace(" ", "").Split(new char[] { ',' }))
								{
									list.Add(int.Parse(text2));
								}
							}
							else if (xmlNode2.Attributes[i].Name == "terrain")
							{
								if (!Enum.TryParse<TerrainType>(xmlNode2.Attributes[i].InnerText, out terrainType))
								{
									terrainType = TerrainType.Plain;
								}
							}
							else if (xmlNode2.Attributes[i].Name == "forest_density")
							{
								char[] array2 = xmlNode2.Attributes[i].InnerText.ToLower().ToCharArray();
								array2[0] = char.ToUpper(array2[0]);
								if (!Enum.TryParse<ForestDensity>(new string(array2), out forestDensity))
								{
									forestDensity = ForestDensity.None;
								}
							}
						}
						XmlNodeList childNodes = xmlNode2.ChildNodes;
						List<TerrainType> list2 = new List<TerrainType>();
						foreach (object obj2 in childNodes)
						{
							XmlNode xmlNode3 = (XmlNode)obj2;
							if (xmlNode3.NodeType != XmlNodeType.Comment && xmlNode3.Name == "TerrainTypes")
							{
								foreach (object obj3 in xmlNode3.ChildNodes)
								{
									XmlNode xmlNode4 = (XmlNode)obj3;
									TerrainType terrainType2;
									if (xmlNode4.Name == "TerrainType" && Enum.TryParse<TerrainType>(xmlNode4.Attributes["name"].InnerText, out terrainType2) && !list2.Contains(terrainType2))
									{
										list2.Add(terrainType2);
									}
								}
							}
						}
						this._singleplayerBattleScenes.Add(new SingleplayerBattleSceneData(text, terrainType, list2, forestDensity, list));
					}
				}
			}
		}

		// Token: 0x06001058 RID: 4184 RVA: 0x00049484 File Offset: 0x00047684
		private void LoadConversationScenes(XmlDocument doc)
		{
			Debug.Print("loading conversation_scenes.xml", 0, Debug.DebugColor.White, 17592186044416UL);
			if (doc.ChildNodes.Count <= 1)
			{
				throw new TWXmlLoadException("Incorrect XML document format. XML document must have at least 2 child nodes.");
			}
			XmlNode xmlNode = doc.ChildNodes[1];
			if (xmlNode.Name != "ConversationScenes")
			{
				throw new TWXmlLoadException("Incorrect XML document format. Root node's name must be ConversationScenes.");
			}
			if (xmlNode.Name == "ConversationScenes")
			{
				foreach (object obj in xmlNode.ChildNodes)
				{
					XmlNode xmlNode2 = (XmlNode)obj;
					if (xmlNode2.NodeType != XmlNodeType.Comment)
					{
						string text = null;
						TerrainType terrainType = TerrainType.Plain;
						ForestDensity forestDensity = ForestDensity.None;
						for (int i = 0; i < xmlNode2.Attributes.Count; i++)
						{
							if (xmlNode2.Attributes[i].Name == "id")
							{
								text = xmlNode2.Attributes[i].InnerText;
							}
							else if (xmlNode2.Attributes[i].Name == "terrain")
							{
								if (!Enum.TryParse<TerrainType>(xmlNode2.Attributes[i].InnerText, out terrainType))
								{
									terrainType = TerrainType.Plain;
								}
							}
							else if (xmlNode2.Attributes[i].Name == "forest_density")
							{
								char[] array = xmlNode2.Attributes[i].InnerText.ToLower().ToCharArray();
								array[0] = char.ToUpper(array[0]);
								if (!Enum.TryParse<ForestDensity>(new string(array), out forestDensity))
								{
									forestDensity = ForestDensity.None;
								}
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
						this._conversationScenes.Add(new ConversationSceneData(text, terrainType, list, forestDensity));
					}
				}
			}
		}

		// Token: 0x06001059 RID: 4185 RVA: 0x000497B0 File Offset: 0x000479B0
		private void LoadMeetingScenes(XmlDocument doc)
		{
			Debug.Print("loading meeting_scenes.xml", 0, Debug.DebugColor.White, 17592186044416UL);
			if (doc.ChildNodes.Count <= 1)
			{
				throw new TWXmlLoadException("Incorrect XML document format. XML document must have at least 2 child nodes.");
			}
			XmlNode xmlNode = doc.ChildNodes[1];
			if (xmlNode.Name != "MeetingScenes")
			{
				throw new TWXmlLoadException("Incorrect XML document format. Root node's name must be MeetingScenes.");
			}
			if (xmlNode.Name == "MeetingScenes")
			{
				foreach (object obj in xmlNode.ChildNodes)
				{
					XmlNode xmlNode2 = (XmlNode)obj;
					if (xmlNode2.NodeType != XmlNodeType.Comment)
					{
						string text = null;
						string text2 = null;
						for (int i = 0; i < xmlNode2.Attributes.Count; i++)
						{
							if (xmlNode2.Attributes[i].Name == "id")
							{
								text = xmlNode2.Attributes[i].InnerText;
							}
							if (xmlNode2.Attributes[i].Name == "culture")
							{
								text2 = xmlNode2.Attributes[i].InnerText.Split(new char[] { '.' })[1];
							}
						}
						this._meetingScenes.Add(new MeetingSceneData(text, text2));
					}
				}
			}
		}

		// Token: 0x040005D1 RID: 1489
		private MBList<SingleplayerBattleSceneData> _singleplayerBattleScenes;

		// Token: 0x040005D2 RID: 1490
		private MBList<ConversationSceneData> _conversationScenes;

		// Token: 0x040005D3 RID: 1491
		private MBList<MeetingSceneData> _meetingScenes;

		// Token: 0x040005D4 RID: 1492
		private const TerrainType DefaultTerrain = TerrainType.Plain;

		// Token: 0x040005D5 RID: 1493
		private const ForestDensity DefaultForestDensity = ForestDensity.None;
	}
}
