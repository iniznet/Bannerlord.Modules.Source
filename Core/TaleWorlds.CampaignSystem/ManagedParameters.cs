using System;
using System.IO;
using System.Xml;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem
{
	// Token: 0x02000096 RID: 150
	public sealed class ManagedParameters : IManagedParametersInitializer
	{
		// Token: 0x170004BC RID: 1212
		// (get) Token: 0x06001111 RID: 4369 RVA: 0x0004ABBA File Offset: 0x00048DBA
		public static ManagedParameters Instance { get; } = new ManagedParameters();

		// Token: 0x06001112 RID: 4370 RVA: 0x0004ABC4 File Offset: 0x00048DC4
		public void Initialize(string relativeXmlPath)
		{
			XmlDocument xmlDocument = ManagedParameters.LoadXmlFile(relativeXmlPath);
			this.LoadFromXml(xmlDocument);
		}

		// Token: 0x06001113 RID: 4371 RVA: 0x0004ABE0 File Offset: 0x00048DE0
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

		// Token: 0x06001114 RID: 4372 RVA: 0x0004ACA8 File Offset: 0x00048EA8
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

		// Token: 0x06001115 RID: 4373 RVA: 0x0004ACF1 File Offset: 0x00048EF1
		public bool GetManagedParameter(ManagedParametersEnum _managedParametersEnum)
		{
			return this._managedParametersArray[(int)_managedParametersEnum];
		}

		// Token: 0x06001116 RID: 4374 RVA: 0x0004ACFC File Offset: 0x00048EFC
		public bool SetManagedParameter(ManagedParametersEnum _managedParametersEnum, bool value)
		{
			this._managedParametersArray[(int)_managedParametersEnum] = value;
			return value;
		}

		// Token: 0x040005F2 RID: 1522
		private readonly bool[] _managedParametersArray = new bool[2];
	}
}
