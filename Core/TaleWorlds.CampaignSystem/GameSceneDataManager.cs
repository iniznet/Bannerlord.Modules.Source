using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem
{
	public class GameSceneDataManager
	{
		public static GameSceneDataManager Instance { get; private set; }

		public MBReadOnlyList<SingleplayerBattleSceneData> SingleplayerBattleScenes
		{
			get
			{
				return this._singleplayerBattleScenes;
			}
		}

		public MBReadOnlyList<ConversationSceneData> ConversationScenes
		{
			get
			{
				return this._conversationScenes;
			}
		}

		public MBReadOnlyList<MeetingSceneData> MeetingScenes
		{
			get
			{
				return this._meetingScenes;
			}
		}

		public GameSceneDataManager()
		{
			this._singleplayerBattleScenes = new MBList<SingleplayerBattleSceneData>();
			this._conversationScenes = new MBList<ConversationSceneData>();
			this._meetingScenes = new MBList<MeetingSceneData>();
		}

		internal static void Initialize()
		{
			GameSceneDataManager.Instance = new GameSceneDataManager();
		}

		internal static void Destroy()
		{
			GameSceneDataManager.Instance = null;
		}

		public void LoadSPBattleScenes(string path)
		{
			XmlDocument xmlDocument = this.LoadXmlFile(path);
			this.LoadSPBattleScenes(xmlDocument);
		}

		public void LoadConversationScenes(string path)
		{
			XmlDocument xmlDocument = this.LoadXmlFile(path);
			this.LoadConversationScenes(xmlDocument);
		}

		public void LoadMeetingScenes(string path)
		{
			XmlDocument xmlDocument = this.LoadXmlFile(path);
			this.LoadMeetingScenes(xmlDocument);
		}

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

		private MBList<SingleplayerBattleSceneData> _singleplayerBattleScenes;

		private MBList<ConversationSceneData> _conversationScenes;

		private MBList<MeetingSceneData> _meetingScenes;

		private const TerrainType DefaultTerrain = TerrainType.Plain;

		private const ForestDensity DefaultForestDensity = ForestDensity.None;
	}
}
