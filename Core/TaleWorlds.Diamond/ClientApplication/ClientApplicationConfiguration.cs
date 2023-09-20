using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using TaleWorlds.Library;

namespace TaleWorlds.Diamond.ClientApplication
{
	// Token: 0x02000054 RID: 84
	public class ClientApplicationConfiguration
	{
		// Token: 0x17000064 RID: 100
		// (get) Token: 0x060001E5 RID: 485 RVA: 0x000057E1 File Offset: 0x000039E1
		// (set) Token: 0x060001E6 RID: 486 RVA: 0x000057E9 File Offset: 0x000039E9
		public string Name { get; set; }

		// Token: 0x17000065 RID: 101
		// (get) Token: 0x060001E7 RID: 487 RVA: 0x000057F2 File Offset: 0x000039F2
		// (set) Token: 0x060001E8 RID: 488 RVA: 0x000057FA File Offset: 0x000039FA
		public string InheritFrom { get; set; }

		// Token: 0x17000066 RID: 102
		// (get) Token: 0x060001E9 RID: 489 RVA: 0x00005803 File Offset: 0x00003A03
		// (set) Token: 0x060001EA RID: 490 RVA: 0x0000580B File Offset: 0x00003A0B
		public string[] Clients { get; set; }

		// Token: 0x17000067 RID: 103
		// (get) Token: 0x060001EB RID: 491 RVA: 0x00005814 File Offset: 0x00003A14
		// (set) Token: 0x060001EC RID: 492 RVA: 0x0000581C File Offset: 0x00003A1C
		public string[] SessionlessClients { get; set; }

		// Token: 0x17000068 RID: 104
		// (get) Token: 0x060001ED RID: 493 RVA: 0x00005825 File Offset: 0x00003A25
		// (set) Token: 0x060001EE RID: 494 RVA: 0x0000582D File Offset: 0x00003A2D
		public SessionProviderType SessionProviderType { get; set; }

		// Token: 0x17000069 RID: 105
		// (get) Token: 0x060001EF RID: 495 RVA: 0x00005836 File Offset: 0x00003A36
		// (set) Token: 0x060001F0 RID: 496 RVA: 0x0000583E File Offset: 0x00003A3E
		public ParameterContainer Parameters { get; set; }

		// Token: 0x060001F1 RID: 497 RVA: 0x00005848 File Offset: 0x00003A48
		public ClientApplicationConfiguration()
		{
			this.Name = "NewlyCreated";
			this.InheritFrom = "";
			this.Clients = new string[0];
			this.SessionlessClients = new string[0];
			this.Parameters = new ParameterContainer();
		}

		// Token: 0x060001F2 RID: 498 RVA: 0x00005894 File Offset: 0x00003A94
		private void FillFromBase(ClientApplicationConfiguration baseConfiguration)
		{
			this.SessionProviderType = baseConfiguration.SessionProviderType;
			this.Parameters = baseConfiguration.Parameters.Clone();
		}

		// Token: 0x060001F3 RID: 499 RVA: 0x000058B4 File Offset: 0x00003AB4
		public static string GetDefaultConfigurationFromFile()
		{
			XmlDocument xmlDocument = new XmlDocument();
			string fileContent = VirtualFolders.GetFileContent(BasePath.Name + "Parameters/ClientProfile.xml");
			if (fileContent == "")
			{
				return "";
			}
			xmlDocument.LoadXml(fileContent);
			return xmlDocument.ChildNodes[0].Attributes["Value"].InnerText;
		}

		// Token: 0x060001F4 RID: 500 RVA: 0x00005916 File Offset: 0x00003B16
		public static void SetDefualtConfigurationCategory(string category)
		{
			ClientApplicationConfiguration._defaultConfigurationCategory = category;
		}

		// Token: 0x060001F5 RID: 501 RVA: 0x0000591E File Offset: 0x00003B1E
		public void FillFrom(string configurationName)
		{
			if (string.IsNullOrEmpty(ClientApplicationConfiguration._defaultConfigurationCategory))
			{
				ClientApplicationConfiguration._defaultConfigurationCategory = ClientApplicationConfiguration.GetDefaultConfigurationFromFile();
			}
			this.FillFrom(ClientApplicationConfiguration._defaultConfigurationCategory, configurationName);
		}

		// Token: 0x060001F6 RID: 502 RVA: 0x00005944 File Offset: 0x00003B44
		public void FillFrom(string configurationCategory, string configurationName)
		{
			XmlDocument xmlDocument = new XmlDocument();
			if (configurationCategory == "")
			{
				return;
			}
			string fileContent = VirtualFolders.GetFileContent(string.Concat(new string[]
			{
				BasePath.Name,
				"Parameters/ClientProfiles/",
				configurationCategory,
				"/",
				configurationName,
				".xml"
			}));
			if (fileContent == "")
			{
				return;
			}
			xmlDocument.LoadXml(fileContent);
			this.Name = Path.GetFileNameWithoutExtension(configurationName);
			XmlNode firstChild = xmlDocument.FirstChild;
			if (firstChild.Attributes != null && firstChild.Attributes["InheritFrom"] != null)
			{
				this.InheritFrom = firstChild.Attributes["InheritFrom"].InnerText;
				ClientApplicationConfiguration clientApplicationConfiguration = new ClientApplicationConfiguration();
				clientApplicationConfiguration.FillFrom(configurationCategory, this.InheritFrom);
				this.FillFromBase(clientApplicationConfiguration);
			}
			ParameterLoader.LoadParametersInto(string.Concat(new string[] { "ClientProfiles/", configurationCategory, "/", configurationName, ".xml" }), this.Parameters);
			foreach (object obj in firstChild.ChildNodes)
			{
				XmlNode xmlNode = (XmlNode)obj;
				if (xmlNode.Name == "SessionProvider")
				{
					string innerText = xmlNode.Attributes["Type"].InnerText;
					this.SessionProviderType = (SessionProviderType)Enum.Parse(typeof(SessionProviderType), innerText);
				}
				else if (xmlNode.Name == "Clients")
				{
					List<string> list = new List<string>();
					foreach (object obj2 in xmlNode.ChildNodes)
					{
						string innerText2 = ((XmlNode)obj2).Attributes["Type"].InnerText;
						list.Add(innerText2);
					}
					this.Clients = list.ToArray();
				}
				else if (xmlNode.Name == "SessionlessClients")
				{
					List<string> list2 = new List<string>();
					foreach (object obj3 in xmlNode.ChildNodes)
					{
						string innerText3 = ((XmlNode)obj3).Attributes["Type"].InnerText;
						list2.Add(innerText3);
					}
					this.SessionlessClients = list2.ToArray();
				}
				else
				{
					xmlNode.Name == "Parameters";
				}
			}
		}

		// Token: 0x040000B6 RID: 182
		private static string _defaultConfigurationCategory = "";
	}
}
