using System;
using System.Collections.Generic;
using System.Xml;
using TaleWorlds.Library;
using TaleWorlds.ModuleManager;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000325 RID: 805
	public static class MultiplayerGameTypes
	{
		// Token: 0x06002B86 RID: 11142 RVA: 0x000A9B4D File Offset: 0x000A7D4D
		public static void Initialize()
		{
			MultiplayerGameTypes.CreateGameTypeInformations();
			MultiplayerGameTypes.LoadMultiplayerSceneInformations();
		}

		// Token: 0x06002B87 RID: 11143 RVA: 0x000A9B59 File Offset: 0x000A7D59
		public static bool CheckGameTypeInfoExists(string gameType)
		{
			return MultiplayerGameTypes._multiplayerGameTypeInfos.ContainsKey(gameType);
		}

		// Token: 0x06002B88 RID: 11144 RVA: 0x000A9B66 File Offset: 0x000A7D66
		public static MultiplayerGameTypeInfo GetGameTypeInfo(string gameType)
		{
			if (MultiplayerGameTypes._multiplayerGameTypeInfos.ContainsKey(gameType))
			{
				return MultiplayerGameTypes._multiplayerGameTypeInfos[gameType];
			}
			Debug.Print("Cannot find game type:" + gameType, 0, Debug.DebugColor.White, 17592186044416UL);
			return null;
		}

		// Token: 0x06002B89 RID: 11145 RVA: 0x000A9BA0 File Offset: 0x000A7DA0
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

		// Token: 0x06002B8A RID: 11146 RVA: 0x000A9CC8 File Offset: 0x000A7EC8
		private static void CreateGameTypeInformations()
		{
			MultiplayerGameTypes._multiplayerGameTypeInfos = new Dictionary<string, MultiplayerGameTypeInfo>();
			foreach (MultiplayerGameTypeInfo multiplayerGameTypeInfo in Module.CurrentModule.GetMultiplayerGameTypes())
			{
				MultiplayerGameTypes._multiplayerGameTypeInfos.Add(multiplayerGameTypeInfo.GameType, multiplayerGameTypeInfo);
			}
		}

		// Token: 0x04001078 RID: 4216
		private static Dictionary<string, MultiplayerGameTypeInfo> _multiplayerGameTypeInfos;
	}
}
