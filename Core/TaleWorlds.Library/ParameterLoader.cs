using System;
using System.IO;
using System.Xml;

namespace TaleWorlds.Library
{
	public class ParameterLoader
	{
		public static ParameterContainer LoadParametersFromClientProfile(string configurationName)
		{
			ParameterContainer parameterContainer = new ParameterContainer();
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(VirtualFolders.GetFileContent(BasePath.Name + "Parameters/ClientProfile.xml"));
			string innerText = xmlDocument.ChildNodes[0].Attributes["Value"].InnerText;
			ParameterLoader.LoadParametersInto(string.Concat(new string[] { "ClientProfiles/", innerText, "/", configurationName, ".xml" }), parameterContainer);
			return parameterContainer;
		}

		public static void LoadParametersInto(string fileFullName, ParameterContainer parameters)
		{
			XmlDocument xmlDocument = new XmlDocument();
			string text = BasePath.Name + "Parameters/" + fileFullName;
			xmlDocument.LoadXml(VirtualFolders.GetFileContent(text));
			foreach (object obj in xmlDocument.FirstChild.ChildNodes)
			{
				XmlNode xmlNode = (XmlNode)obj;
				if (xmlNode.Name == "Parameters")
				{
					XmlAttributeCollection attributes = xmlNode.Attributes;
					string text2;
					if (attributes == null)
					{
						text2 = null;
					}
					else
					{
						XmlAttribute xmlAttribute = attributes["Platforms"];
						text2 = ((xmlAttribute != null) ? xmlAttribute.InnerText : null);
					}
					string text3 = text2;
					if (!string.IsNullOrWhiteSpace(text3))
					{
						if (text3.Split(new char[] { ',' }).FindIndex((string p) => p.Trim().Equals(string.Concat(ApplicationPlatform.CurrentPlatform))) < 0)
						{
							continue;
						}
					}
					foreach (object obj2 in xmlNode.ChildNodes)
					{
						XmlNode xmlNode2 = (XmlNode)obj2;
						string innerText = xmlNode2.Attributes["Name"].InnerText;
						string text4;
						string text5;
						string text6;
						if (ParameterLoader.TryGetFromFile(xmlNode2, out text4))
						{
							text5 = text4;
						}
						else if (ParameterLoader.TryGetFromEnvironment(xmlNode2, out text6))
						{
							text5 = text6;
						}
						else if (xmlNode2.Attributes["DefaultValue"] != null)
						{
							text5 = xmlNode2.Attributes["DefaultValue"].InnerText;
						}
						else
						{
							text5 = xmlNode2.Attributes["Value"].InnerText;
						}
						parameters.AddParameter(innerText, text5, true);
					}
				}
			}
		}

		private static bool TryGetFromFile(XmlNode node, out string value)
		{
			value = "";
			XmlAttributeCollection attributes = node.Attributes;
			if (((attributes != null) ? attributes["LoadFromFile"] : null) != null && node.Attributes["LoadFromFile"].InnerText.ToLower() == "true")
			{
				string innerText = node.Attributes["File"].InnerText;
				if (File.Exists(innerText))
				{
					string text = File.ReadAllText(innerText);
					value = text;
					return true;
				}
			}
			return false;
		}

		private static bool TryGetFromEnvironment(XmlNode node, out string value)
		{
			value = "";
			XmlAttributeCollection attributes = node.Attributes;
			if (((attributes != null) ? attributes["GetFromEnvironment"] : null) != null && node.Attributes["GetFromEnvironment"].InnerText.ToLower() == "true")
			{
				string environmentVariable = Environment.GetEnvironmentVariable(node.Attributes["Variable"].InnerText);
				if (!string.IsNullOrEmpty(environmentVariable))
				{
					value = environmentVariable;
					return true;
				}
			}
			return false;
		}
	}
}
