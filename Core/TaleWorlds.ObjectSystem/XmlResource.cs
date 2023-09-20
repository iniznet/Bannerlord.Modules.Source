using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using TaleWorlds.ModuleManager;

namespace TaleWorlds.ObjectSystem
{
	public static class XmlResource
	{
		public static void InitializeXmlInformationList(List<MbObjectXmlInformation> xmlInformation)
		{
			XmlResource.XmlInformationList = xmlInformation;
		}

		public static void GetMbprojxmls(string moduleName)
		{
			string mbprojPath = ModuleHelper.GetMbprojPath(moduleName);
			if (mbprojPath.Length > 0 && File.Exists(mbprojPath))
			{
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.Load(mbprojPath);
				XmlNodeList xmlNodeList = xmlDocument.SelectSingleNode("base").SelectNodes("file");
				if (xmlNodeList != null)
				{
					foreach (object obj in xmlNodeList)
					{
						XmlNode xmlNode = (XmlNode)obj;
						string innerText = xmlNode.Attributes["id"].InnerText;
						string innerText2 = xmlNode.Attributes["name"].InnerText;
						MbObjectXmlInformation mbObjectXmlInformation = new MbObjectXmlInformation
						{
							Id = innerText,
							Name = innerText2,
							ModuleName = moduleName,
							GameTypesIncluded = new List<string>()
						};
						XmlResource.MbprojXmls.Add(mbObjectXmlInformation);
					}
				}
			}
		}

		public static void GetXmlListAndApply(string moduleName)
		{
			string path = ModuleHelper.GetPath(moduleName);
			using (XmlReader.Create(path, new XmlReaderSettings
			{
				IgnoreComments = true
			}))
			{
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.Load(path);
				XmlNodeList xmlNodeList = xmlDocument.SelectSingleNode("Module").SelectNodes("Xmls/XmlNode");
				if (xmlNodeList != null)
				{
					foreach (object obj in xmlNodeList)
					{
						XmlNode xmlNode = (XmlNode)obj;
						XmlNode xmlNode2 = xmlNode.SelectSingleNode("XmlName");
						string innerText = xmlNode2.Attributes["id"].InnerText;
						string innerText2 = xmlNode2.Attributes["path"].InnerText;
						List<string> list = new List<string>();
						XmlNode xmlNode3 = xmlNode.SelectSingleNode("IncludedGameTypes");
						if (xmlNode3 != null)
						{
							foreach (object obj2 in xmlNode3.ChildNodes)
							{
								XmlNode xmlNode4 = (XmlNode)obj2;
								list.Add(xmlNode4.Attributes["value"].InnerText);
							}
						}
						MbObjectXmlInformation mbObjectXmlInformation = new MbObjectXmlInformation
						{
							Id = innerText,
							Name = innerText2,
							ModuleName = moduleName,
							GameTypesIncluded = list
						};
						XmlResource.XmlInformationList.Add(mbObjectXmlInformation);
					}
				}
			}
		}

		public static List<MbObjectXmlInformation> XmlInformationList = new List<MbObjectXmlInformation>();

		public static List<MbObjectXmlInformation> MbprojXmls = new List<MbObjectXmlInformation>();
	}
}
