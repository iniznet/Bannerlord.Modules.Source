using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using TaleWorlds.Library;
using TaleWorlds.ModuleManager;

namespace TaleWorlds.CampaignSystem.Conversation
{
	public class ConversationAnimationManager
	{
		public Dictionary<string, ConversationAnimData> ConversationAnims { get; private set; }

		public ConversationAnimationManager()
		{
			this.ConversationAnims = new Dictionary<string, ConversationAnimData>();
			this.LoadConversationAnimData(ModuleHelper.GetModuleFullPath("Sandbox") + "ModuleData/conversation_animations.xml");
		}

		private void LoadConversationAnimData(string xmlPath)
		{
			XmlDocument xmlDocument = this.LoadXmlFile(xmlPath);
			this.LoadFromXml(xmlDocument);
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
