using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using TaleWorlds.Library;
using TaleWorlds.ModuleManager;

namespace TaleWorlds.CampaignSystem.Conversation
{
	// Token: 0x020001EB RID: 491
	public class ConversationAnimationManager
	{
		// Token: 0x17000756 RID: 1878
		// (get) Token: 0x06001D39 RID: 7481 RVA: 0x0008383E File Offset: 0x00081A3E
		// (set) Token: 0x06001D3A RID: 7482 RVA: 0x00083846 File Offset: 0x00081A46
		public Dictionary<string, ConversationAnimData> ConversationAnims { get; private set; }

		// Token: 0x06001D3B RID: 7483 RVA: 0x0008384F File Offset: 0x00081A4F
		public ConversationAnimationManager()
		{
			this.ConversationAnims = new Dictionary<string, ConversationAnimData>();
			this.LoadConversationAnimData(ModuleHelper.GetModuleFullPath("Sandbox") + "ModuleData/conversation_animations.xml");
		}

		// Token: 0x06001D3C RID: 7484 RVA: 0x0008387C File Offset: 0x00081A7C
		private void LoadConversationAnimData(string xmlPath)
		{
			XmlDocument xmlDocument = this.LoadXmlFile(xmlPath);
			this.LoadFromXml(xmlDocument);
		}

		// Token: 0x06001D3D RID: 7485 RVA: 0x00083898 File Offset: 0x00081A98
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

		// Token: 0x06001D3E RID: 7486 RVA: 0x000838E4 File Offset: 0x00081AE4
		private void LoadFromXml(XmlDocument doc)
		{
			if (doc.ChildNodes.Count <= 1)
			{
				throw new TWXmlLoadException("Incorrect XML document format.");
			}
			if (doc.ChildNodes[1].Name != "ConversationAnimations")
			{
				throw new TWXmlLoadException("Incorrect XML document format.");
			}
			foreach (object obj in doc.DocumentElement.SelectNodes("IdleAnim"))
			{
				XmlNode xmlNode = (XmlNode)obj;
				if (xmlNode.Attributes != null)
				{
					KeyValuePair<string, ConversationAnimData> keyValuePair = new KeyValuePair<string, ConversationAnimData>(xmlNode.Attributes["id"].Value, new ConversationAnimData());
					keyValuePair.Value.IdleAnimStart = xmlNode.Attributes["action_id_1"].Value;
					keyValuePair.Value.IdleAnimLoop = xmlNode.Attributes["action_id_2"].Value;
					keyValuePair.Value.FamilyType = 0;
					XmlAttribute xmlAttribute = xmlNode.Attributes["family_type"];
					int num;
					if (xmlAttribute != null && !string.IsNullOrEmpty(xmlAttribute.Value) && int.TryParse(xmlAttribute.Value, out num))
					{
						keyValuePair.Value.FamilyType = num;
					}
					keyValuePair.Value.MountFamilyType = 0;
					XmlAttribute xmlAttribute2 = xmlNode.Attributes["mount_family_type"];
					int num2;
					if (xmlAttribute2 != null && !string.IsNullOrEmpty(xmlAttribute2.Value) && int.TryParse(xmlAttribute2.Value, out num2))
					{
						keyValuePair.Value.MountFamilyType = num2;
					}
					foreach (object obj2 in xmlNode.ChildNodes)
					{
						XmlNode xmlNode2 = (XmlNode)obj2;
						if (xmlNode2.Name == "Reactions")
						{
							foreach (object obj3 in xmlNode2.ChildNodes)
							{
								XmlNode xmlNode3 = (XmlNode)obj3;
								if (xmlNode3.Name == "Reaction" && xmlNode3.Attributes["id"] != null && xmlNode3.Attributes["action_id"] != null)
								{
									keyValuePair.Value.Reactions.Add(xmlNode3.Attributes["id"].Value, xmlNode3.Attributes["action_id"].Value);
								}
							}
						}
					}
					this.ConversationAnims.Add(keyValuePair.Key, keyValuePair.Value);
				}
			}
		}
	}
}
