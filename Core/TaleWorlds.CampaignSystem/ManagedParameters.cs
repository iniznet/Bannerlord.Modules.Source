using System;
using System.IO;
using System.Xml;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem
{
	public sealed class ManagedParameters : IManagedParametersInitializer
	{
		public static ManagedParameters Instance { get; } = new ManagedParameters();

		public void Initialize(string relativeXmlPath)
		{
			XmlDocument xmlDocument = ManagedParameters.LoadXmlFile(relativeXmlPath);
			this.LoadFromXml(xmlDocument);
		}

		private void LoadFromXml(XmlNode doc)
		{
			XmlNode xmlNode = null;
			if (doc.ChildNodes[1].ChildNodes[0].Name == "managed_campaign_parameters")
			{
				xmlNode = doc.ChildNodes[1].ChildNodes[0].ChildNodes[0];
			}
			while (xmlNode != null)
			{
				ManagedParametersEnum managedParametersEnum;
				if (xmlNode.Name == "managed_campaign_parameter" && xmlNode.NodeType != XmlNodeType.Comment && Enum.TryParse<ManagedParametersEnum>(xmlNode.Attributes["id"].Value, true, out managedParametersEnum))
				{
					this._managedParametersArray[(int)managedParametersEnum] = bool.Parse(xmlNode.Attributes["value"].Value);
				}
				xmlNode = xmlNode.NextSibling;
			}
		}

		private static XmlDocument LoadXmlFile(string path)
		{
			Debug.Print("opening " + path, 0, Debug.DebugColor.White, 17592186044416UL);
			XmlDocument xmlDocument = new XmlDocument();
			StreamReader streamReader = new StreamReader(path);
			string text = streamReader.ReadToEnd();
			xmlDocument.LoadXml(text);
			streamReader.Close();
			return xmlDocument;
		}

		public bool GetManagedParameter(ManagedParametersEnum _managedParametersEnum)
		{
			return this._managedParametersArray[(int)_managedParametersEnum];
		}

		public bool SetManagedParameter(ManagedParametersEnum _managedParametersEnum, bool value)
		{
			this._managedParametersArray[(int)_managedParametersEnum] = value;
			return value;
		}

		private readonly bool[] _managedParametersArray = new bool[2];
	}
}
