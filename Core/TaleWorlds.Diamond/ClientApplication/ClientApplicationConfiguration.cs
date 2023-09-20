using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using TaleWorlds.Library;

namespace TaleWorlds.Diamond.ClientApplication
{
	public class ClientApplicationConfiguration
	{
		public string Name { get; set; }

		public string InheritFrom { get; set; }

		public string[] Clients { get; set; }

		public string[] SessionlessClients { get; set; }

		public SessionProviderType SessionProviderType { get; set; }

		public ParameterContainer Parameters { get; set; }

		public ClientApplicationConfiguration()
		{
			this.Name = "NewlyCreated";
			this.InheritFrom = "";
			this.Clients = new string[0];
			this.SessionlessClients = new string[0];
			this.Parameters = new ParameterContainer();
		}

		private void FillFromBase(ClientApplicationConfiguration baseConfiguration)
		{
			this.SessionProviderType = baseConfiguration.SessionProviderType;
			this.Parameters = baseConfiguration.Parameters.Clone();
		}

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

		public static void SetDefualtConfigurationCategory(string category)
		{
			ClientApplicationConfiguration._defaultConfigurationCategory = category;
		}

		public void FillFrom(string configurationName)
		{
			if (string.IsNullOrEmpty(ClientApplicationConfiguration._defaultConfigurationCategory))
			{
				ClientApplicationConfiguration._defaultConfigurationCategory = ClientApplicationConfiguration.GetDefaultConfigurationFromFile();
			}
			this.FillFrom(ClientApplicationConfiguration._defaultConfigurationCategory, configurationName);
		}

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

		private static string _defaultConfigurationCategory = "";
	}
}
