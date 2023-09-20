using System;
using System.IO;
using System.Xml;
using TaleWorlds.Library;

namespace TaleWorlds.Core
{
	// Token: 0x02000093 RID: 147
	public sealed class ManagedParameters : IManagedParametersInitializer
	{
		// Token: 0x1700029E RID: 670
		// (get) Token: 0x060007D4 RID: 2004 RVA: 0x0001AF71 File Offset: 0x00019171
		public static ManagedParameters Instance { get; } = new ManagedParameters();

		// Token: 0x060007D5 RID: 2005 RVA: 0x0001AF78 File Offset: 0x00019178
		public static float GetParameter(ManagedParametersEnum managedParameterType)
		{
			return ManagedParameters.Instance._managedParametersArray[(int)managedParameterType];
		}

		// Token: 0x060007D6 RID: 2006 RVA: 0x0001AF86 File Offset: 0x00019186
		public static void SetParameter(ManagedParametersEnum managedParameterType, float newValue)
		{
			ManagedParameters.Instance._managedParametersArray[(int)managedParameterType] = newValue;
		}

		// Token: 0x060007D7 RID: 2007 RVA: 0x0001AF98 File Offset: 0x00019198
		public void Initialize(string relativeXmlPath)
		{
			XmlDocument xmlDocument = ManagedParameters.LoadXmlFile(relativeXmlPath);
			this.LoadFromXml(xmlDocument);
		}

		// Token: 0x060007D8 RID: 2008 RVA: 0x0001AFB3 File Offset: 0x000191B3
		private ManagedParameters()
		{
		}

		// Token: 0x060007D9 RID: 2009 RVA: 0x0001AFC8 File Offset: 0x000191C8
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

		// Token: 0x060007DA RID: 2010 RVA: 0x0001B014 File Offset: 0x00019214
		private void LoadFromXml(XmlNode doc)
		{
			Debug.Print("loading managed_core_parameters.xml", 0, Debug.DebugColor.White, 17592186044416UL);
			if (doc.ChildNodes.Count <= 1)
			{
				throw new TWXmlLoadException("Incorrect XML document format.");
			}
			if (doc.ChildNodes[1].Name != "base")
			{
				throw new TWXmlLoadException("Incorrect XML document format.");
			}
			if (doc.ChildNodes[1].ChildNodes[0].Name != "managed_core_parameters")
			{
				throw new TWXmlLoadException("Incorrect XML document format.");
			}
			XmlNode xmlNode = null;
			if (doc.ChildNodes[1].ChildNodes[0].Name == "managed_core_parameters")
			{
				xmlNode = doc.ChildNodes[1].ChildNodes[0].ChildNodes[0];
			}
			while (xmlNode != null)
			{
				ManagedParametersEnum managedParametersEnum;
				if (xmlNode.Name == "managed_core_parameter" && xmlNode.NodeType != XmlNodeType.Comment && Enum.TryParse<ManagedParametersEnum>(xmlNode.Attributes["id"].Value, true, out managedParametersEnum))
				{
					this._managedParametersArray[(int)managedParametersEnum] = float.Parse(xmlNode.Attributes["value"].Value);
				}
				xmlNode = xmlNode.NextSibling;
			}
		}

		// Token: 0x060007DB RID: 2011 RVA: 0x0001B163 File Offset: 0x00019363
		public float GetManagedParameter(ManagedParametersEnum managedParameterEnum)
		{
			return this._managedParametersArray[(int)managedParameterEnum];
		}

		// Token: 0x0400047F RID: 1151
		private readonly float[] _managedParametersArray = new float[68];
	}
}
