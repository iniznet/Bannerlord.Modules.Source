using System;
using System.Collections.Generic;
using System.Xml;
using TaleWorlds.Library;
using TaleWorlds.ModuleManager;

namespace TaleWorlds.MountAndBlade
{
	public static class MultiplayerGameTypes
	{
		public static void Initialize()
		{
			MultiplayerGameTypes.CreateGameTypeInformations();
			MultiplayerGameTypes.LoadMultiplayerSceneInformations();
		}

		public static bool CheckGameTypeInfoExists(string gameType)
		{
			return MultiplayerGameTypes._multiplayerGameTypeInfos.ContainsKey(gameType);
		}

		public static MultiplayerGameTypeInfo GetGameTypeInfo(string gameType)
		{
			if (MultiplayerGameTypes._multiplayerGameTypeInfos.ContainsKey(gameType))
			{
				return MultiplayerGameTypes._multiplayerGameTypeInfos[gameType];
			}
			Debug.Print("Cannot find game type:" + gameType, 0, Debug.DebugColor.White, 17592186044416UL);
			return null;
		}

		private static void LoadMultiplayerSceneInformations()
		{
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.Load(ModuleHelper.GetModuleFullPath("Native") + "ModuleData/Multiplayer/MultiplayerScenes.xml");
			foreach (object obj in xmlDocument.FirstChild)
			{
				XmlNode xmlNode = (XmlNode)obj;
				if (xmlNode.NodeType != XmlNodeType.Comment)
				{
					string innerText = xmlNode.Attributes["name"].InnerText;
					foreach (object obj2 in xmlNode.ChildNodes)
					{
						XmlNode xmlNode2 = (XmlNode)obj2;
						if (xmlNode2.NodeType != XmlNodeType.Comment)
						{
							string innerText2 = xmlNode2.Attributes["name"].InnerText;
							if (MultiplayerGameTypes._multiplayerGameTypeInfos.ContainsKey(innerText2))
							{
								MultiplayerGameTypes._multiplayerGameTypeInfos[innerText2].Scenes.Add(innerText);
							}
						}
					}
				}
			}
		}

		private static void CreateGameTypeInformations()
		{
			MultiplayerGameTypes._multiplayerGameTypeInfos = new Dictionary<string, MultiplayerGameTypeInfo>();
			foreach (MultiplayerGameTypeInfo multiplayerGameTypeInfo in Module.CurrentModule.GetMultiplayerGameTypes())
			{
				MultiplayerGameTypes._multiplayerGameTypeInfos.Add(multiplayerGameTypeInfo.GameType, multiplayerGameTypeInfo);
			}
		}

		private static Dictionary<string, MultiplayerGameTypeInfo> _multiplayerGameTypeInfos;
	}
}
